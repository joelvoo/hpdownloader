using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NHLHTMLReports
{
    class PlayerHTML : HTMLReport
    {
        private static Dictionary<string, string> Positions = new Dictionary<string, string>()
        {
            {"Center","C"},
            {"Right","R"},
            {"Left","L"},
            {"Defense","D"},
            {"Goalie","G"},
            {"Null",null }
        };
        public int Id { get; set; }
        public new String BaseUrl
        {
            get
            {
                return "http://www.nhl.com/ice/m_player.htm?id=";
            }
        }
        public new String ReportURL
        {
            get
            {
                return BaseUrl + Id.ToString();
            }
        }

        public PlayerHTML() { }

        public PlayerHTML(int id, WebProxy proxy = null)
        {

            Id = id;
            Document = new HtmlDocument();
            Document.Load(HTMLHelper.GetResponseStream(ReportURL, Config.Proxy));

        }

        new public void Parse()
        {
            ParsePlayer();
        }

        public void ParsePlayer()
        {

            string FIRST_XPATH = "//span[@class='firstName']";
            string LAST_XPATH = "//span[@class='lastName']";
            string NUM_XPATH = "//div[@class='sweater']";
            string TEAM_XPATH = "//div[@class='teamName']";
            string POS_XPATH = "//div[@class='vitals']/div/div/div/div";
            HtmlNode node;

            string first = Document.DocumentNode.SelectSingleNode(FIRST_XPATH).InnerText;
            string last = Document.DocumentNode.SelectSingleNode(LAST_XPATH).InnerText;
            string team = Document.DocumentNode.SelectSingleNode(TEAM_XPATH).InnerText;
            if (team == "MontrÃ©al Canadiens")
                team = "Montreal Canadiens";
            int num = 0;
            if ((node = Document.DocumentNode.SelectSingleNode(NUM_XPATH)) != null)
                Int32.TryParse(node.InnerHtml, out num);
            string position = null;
            if ((node = Document.DocumentNode.SelectSingleNode(POS_XPATH)) != null &&
                node.InnerHtml.Contains("position:"))
            {
                string key = Positions.Keys.FirstOrDefault<string>(p => node.InnerText.Contains(p)) ?? "Null";
                position = Positions[key];
            }

            using (NLPoolEntities context = new NLPoolEntities())
            {
                DbContextTransaction t = context.Database.BeginTransaction();
                //context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [NLPOOL].[dbo].[NHL_PLAYER] ON");
                NHL_PLAYER player = context.NHL_PLAYER.Find(Id);
                if (player == null)
                    if (team != "")
                    {
                        player = new NHL_PLAYER();
                        player.ID = Id;
                        context.NHL_PLAYER.Add(player);
                    }
                    else
                    {
                        Log.Instance.WriteLine("{0} {1} is not an active player.", new string[] { first, last });
                        return;
                    }

                player.FIRST_NAME = first;
                player.LAST_NAME = last;
                player.PLAYER_NUMBER = num;
                player.TEAM = team.ToUpper();
                player.ELIGIBLE_POSITION = player.ELIGIBLE_POSITION ?? position;
                context.SaveChanges();
                t.Commit();
            }
        }
    }
}
