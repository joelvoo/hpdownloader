using NHLHTMLReports.JSON;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace NHLHTMLReports
{
    class PlayerJSON : PlayerHTML
    {
        WebProxy Proxy;

        public new String BaseUrl
        {
            get
            {
                return "https://statsapi.web.nhl.com/api/v1/people/{0}?hydrate=currentTeam";
            }
        }
        public new String ReportURL
        {
            get
            {
                return String.Format(BaseUrl, Id.ToString());
            }
        }
        public PlayerJSON(int id, WebProxy proxy = null)
        {

            Id = id;
            Proxy = proxy;

        }

        new public void Parse()
        {
            ParsePlayer();
        }

        public new void ParsePlayer()
        {
            var stream = HTMLHelper.GetResponseStream(ReportURL, Proxy);
            var serializer = new DataContractJsonSerializer(typeof(PlayerResult));
            var result = new PlayerResult();

            result = (PlayerResult)serializer.ReadObject(stream);

            using (NLPoolEntities context = new NLPoolEntities())
            {
                DbContextTransaction t = context.Database.BeginTransaction();
                NHL_PLAYER player = context.NHL_PLAYER.Find(Id);

                var person = result.people[0];
                
                if (player == null)
                    if (person.currentTeam != null)
                    {
                        player = new NHL_PLAYER();
                        player.ID = Id;
                        context.NHL_PLAYER.Add(player);
                    }
                    else
                    {
                        Log.Instance.WriteLine("{0} is not an active player.", new string[] { person.fullName });
                        return;
                    }
                if (person.currentTeam != null)
                {
                    var team = person.currentTeam.name == "Montréal Canadiens" ?
                        "MONTREAL CANADIENS" : person.currentTeam.name.ToUpper();
                    player.TEAM = team;
                    player.NHLTeamCode = person.currentTeam.abbreviation;
                }
                player.FIRST_NAME = person.firstName;
                player.LAST_NAME = person.lastName;
                if (person.primaryNumber != null)
                    player.PLAYER_NUMBER =  Int16.Parse(person.primaryNumber);                              
                player.ACTIVE = person.active;
                player.ELIGIBLE_POSITION = player.ELIGIBLE_POSITION == "" || player.ELIGIBLE_POSITION == null ? 
                    person.primaryPosition.code : player.ELIGIBLE_POSITION;
                context.SaveChanges();
                t.Commit();

                Log.Instance.WriteLine("{0} updated.", new string[] { player.Name.ToString() });
            }
        }
    }
}
