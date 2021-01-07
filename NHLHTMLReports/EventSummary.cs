using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NHLHTMLReports
{
    public class EventSummary : HTMLReport
    {
        Dictionary<string, int> PSMap = new Dictionary<string, int> {
            {"PLAYER_NUMBER",0},
            { "POS",1},
            { "PLAYER_NAME",2},
            { "G", 3},
            { "A", 4},
            { "P", 5},
            { "PLUS_MINUS", 6},
            { "PN", 7},
            { "PIM", 8 },
            { "TOT", 9 },
            { "SHF", 10 },
            { "AVG", 11 },
            { "PP", 12 },
            { "SH", 13 },
            { "EV", 14 },
            { "S", 15 },
            { "AB", 16 },
            { "MS", 17 },
            { "HT", 18 },
            { "GV", 19 },
            { "TK", 20 },
            { "BS", 21 },
            { "FW", 22 },
            { "FL", 23 },
            { "FP", 24 }

        };
        int GameId { get; set; }
        GAME_INFO GameInfo { get; set; }
        List<PLAYER_SUMMARY> PlayerSummaries { get; set; }


        override protected ReportType ReportType { get { return ReportType.ES; } }


        public EventSummary(int gameId)
            : base(gameId)
        {
            GameId = gameId;
        }

        public void DeleteAndParse()
        {
            Delete();
            Parse();
        }
        new public void Parse()
        {
            ParseGameInfo();
            ParsePlayerSummary();
        }

        void ParseGameInfo()
        {
            string HOME_TEAM_XPATH = "//td[@class='lborder + rborder + bborder + homesectionheading']";
            string VISITOR_TEAM_XPATH = "//td[@class='lborder + rborder + bborder + visitorsectionheading']";
            string DATE_XPATH = "//table[@id='GameInfo']/tr[4]/td";
            string TIME_XPATH = "//table[@id='GameInfo']/tr[6]/td";
            string STATUS_XPATH = "//table[@id='GameInfo']/tr[8]/td";
            string HOME_SCORE_XPATH = "//table[@id='Home']/tr[2]/td/table/tr/td[2]";
            string VISITOR_SCORE_XPATH = "//table[@id='Visitor']/tr[2]/td/table/tr/td[2]";

            using (NLPoolEntities context = new NLPoolEntities())
            {
                GameInfo = context.GAME_INFO.Find(GameId);
                if (GameInfo == null)
                {
                    context.Database.Connection.Open();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [NLPOOL].[GAME_INFO] ON");
                    GameInfo = new GAME_INFO();
                    GameInfo.ID = GameId;
                    context.GAME_INFO.Add(GameInfo);
                }
                GameInfo.HOME_TEAM = Document.DocumentNode.SelectSingleNode(HOME_TEAM_XPATH).InnerText;
                GameInfo.VISITOR_TEAM = Document.DocumentNode.SelectSingleNode(VISITOR_TEAM_XPATH).InnerText;
                GameInfo.HOME_SCORE = Int32.Parse(Document.DocumentNode.SelectSingleNode(HOME_SCORE_XPATH).InnerText);
                GameInfo.VISITOR_SCORE = Int32.Parse(Document.DocumentNode.SelectSingleNode(VISITOR_SCORE_XPATH).InnerText);
                GameInfo.STATUS = Document.DocumentNode.SelectSingleNode(STATUS_XPATH).InnerText;
                if (GameInfo.STATUS == "Final")
                {
                    DateTime date = DateTime.Parse(Document.DocumentNode.SelectSingleNode(DATE_XPATH).InnerText);
                    string[] times = Document.DocumentNode.SelectSingleNode(TIME_XPATH).InnerText.Replace("&nbsp;", " ").Split(';');
                    string[] start = times[0].Trim().Split(' ');
                    string[] end = times[1].Trim().Split(' ');
                    GameInfo.START_TIME = date.Add(DateTime.Parse(start[1] + " PM" + GetTimeZoneString(start[2])).TimeOfDay);
                    GameInfo.END_TIME = date.Add(DateTime.Parse(end[1] + " PM" + GetTimeZoneString(end[2])).TimeOfDay);
                }

                context.SaveChanges();
                context.Database.Connection.Close();
            }
            HtmlNode node = Document.DocumentNode.SelectSingleNode(HOME_TEAM_XPATH);

        }
        void ParsePlayerSummary()
        {
            string VISITOR_PLAYER_SUMMARY_XPATH = "//html/body/table/tr[8]/td/table/tr[position()>=3 and position()<=22 and (@class='evenColor' or @class='oddColor')]";
            string HOME_PLAYER_SUMMARY_XPATH = "//html/body/table/tr[8]/td/table/tr[position()>=27 and position()<=48 and (@class='evenColor' or @class='oddColor')]";
            foreach (HtmlNode n in Document.DocumentNode.SelectNodes(HOME_PLAYER_SUMMARY_XPATH))
            {
                List<String> values = new List<string>();
                foreach (HtmlNode child in n.SelectNodes("td"))
                {
                    string val = child.InnerText == "&nbsp;" ? "0" : child.InnerText;
                    values.Add(val);
                }
                SavePlayer(values, GameInfo.HOME_TEAM);
            }
            foreach (HtmlNode n in Document.DocumentNode.SelectNodes(VISITOR_PLAYER_SUMMARY_XPATH))
            {
                List<String> values = new List<string>();
                foreach (HtmlNode child in n.SelectNodes("td"))
                {
                    string val = child.InnerText == "&nbsp;" ? "0" : child.InnerText;
                    values.Add(val);
                }
                SavePlayer(values, GameInfo.VISITOR_TEAM);
            }

        }

        public void SavePlayer(List<String> stats, String team)
        {
            using (NLPoolEntities context = new NLPoolEntities())
            {
                PLAYER_SUMMARY ps;
                int playerNumber = (int)Convert.ChangeType(GetValue(stats, "PLAYER_NUMBER"), typeof(int));
                ps = context.PLAYER_SUMMARY
                    .Where(p => p.GAME_ID == GameId &&
                        p.TEAM == team &&
                        p.PLAYER_NUMBER == playerNumber)
                        .FirstOrDefault();

                if (ps == null)
                {
                    ps = new PLAYER_SUMMARY();
                    ps.TEAM = team;
                    ps.GAME_ID = GameInfo.ID;
                    context.PLAYER_SUMMARY.Add(ps);
                }

                foreach (String s in PSMap.Keys)
                {
                    PropertyInfo pi = ps.GetType().GetProperty(s);
                    Type type = pi.PropertyType;
                    //String value = GetValue(stats, s);
                    var value = GetValue(stats, s);
                    if (value != null)
                    { 
                        pi.SetValue(ps, value);
                    }
                }
                context.SaveChanges();
            }
        }


        public Object GetValue(List<string> values, string property)
        {
            //PropertyInfo pi = new PLAYER_SUMMARY().GetType().GetProperty(property);
            Type type = new PLAYER_SUMMARY().GetType().GetProperty(property).PropertyType;
            String value = values[PSMap[property]];
            Object result = null;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (String.IsNullOrEmpty(value))
                {
                    result = null;
                }
                else if (value.Contains(':'))
                {
                    String[] parts = value.Split(':');
                    TimeSpan ts = new TimeSpan(0, Int32.Parse(parts[0]), Int32.Parse(parts[1]));
                    result = Convert.ChangeType(ts.TotalSeconds, type.GetGenericArguments()[0]);
                }
                else
                {
                    result = Convert.ChangeType(value, type.GetGenericArguments()[0]);
                }
            }
            else
            {
                result = Convert.ChangeType(value, type);
            }
            return result;
        }

        public string GameStatus
        {
            get
            {
                if (GameInfo != null)
                    return GameInfo.STATUS;
                else
                    return null;
            }
        }

        public void Delete()
        {
            using (NLPoolEntities context = new NLPoolEntities())
            {
                context.PLAYER_SUMMARY.RemoveRange(context.PLAYER_SUMMARY.Where(p => p.GAME_ID == GameId).ToList());
            }
        }
    }
}

