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
    class RosterJSON : PlayerHTML
    {
        WebProxy Proxy;
        string baseURL = "https://statsapi.web.nhl.com/api/v1/teams?hydrate=franchise(roster(person))";
        public new String BaseUrl
        {
            get
            {
                return "https://statsapi.web.nhl.com/api/v1/people/";
            }
        }
        public new String ReportURL
        {
            get
            {
                return String.Format(baseURL, Id.ToString());
            }
        }
        public RosterJSON(WebProxy proxy = null)
        {

            Proxy = proxy;

        }

        new public void Parse()
        {
            ParsePlayer();
        }

        public new void ParsePlayer()
        {
            var stream = HTMLHelper.GetResponseStream(baseURL, Proxy);
            var serializer = new DataContractJsonSerializer(typeof(RosterResult));
            var result = new RosterResult();

            if (stream == null)
                return;
            result = (RosterResult)serializer.ReadObject(stream);


            using (NLPoolEntities context = new NLPoolEntities())
            {
                DbContextTransaction t = context.Database.BeginTransaction();
                var totalCount = 0;
                foreach (var team in result.teams)
                {
                    foreach (var rosterPosition in team.franchise.roster.roster)
                    {
                        var person = rosterPosition.person;
                        NHL_PLAYER player = context.NHL_PLAYER.Find(person.id);

                        if (player == null)
                            if (person.currentTeam != null)
                            {
                                player = new NHL_PLAYER();
                                player.ID = person.id;
                                context.NHL_PLAYER.Add(player);
                            }
                            else
                            {
                                Log.Instance.WriteLine("{0} is not an active player.", new string[] { person.fullName });
                                return;
                            }
                        var teamName = person.currentTeam.name == "Montréal Canadiens" ?
                            "MONTREAL CANADIENS" : person.currentTeam.name.ToUpper();

                        player.FIRST_NAME = person.firstName;
                        player.LAST_NAME = person.lastName;
                        if (person.primaryNumber != null)
                            player.PLAYER_NUMBER = Int16.Parse(person.primaryNumber);
                        player.TEAM = teamName;
                        player.NHLTeamCode = team.abbreviation;
                        player.ACTIVE = person.active;
                        player.ELIGIBLE_POSITION = player.ELIGIBLE_POSITION == "" || player.ELIGIBLE_POSITION == null ?
                            person.primaryPosition.code : player.ELIGIBLE_POSITION;
                        context.SaveChanges();
                    }
                    totalCount += team.franchise.roster.roster.Count();
                }
                Log.Instance.WriteLine("{0} players updated.", new string[] { totalCount.ToString() });
                t.Commit();
            }
        }
    }
}

