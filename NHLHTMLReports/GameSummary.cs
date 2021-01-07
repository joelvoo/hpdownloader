using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NHLHTMLReports
{
    public class GameSummary : HTMLReport
    {
        Dictionary<string, int> GSMap = new Dictionary<string, int> { 
            {"PLAYER_NUMBER",0},
            { "POS",1},
            { "PLAYER_NAME",2},
            { "EV", 3 },
            { "PP", 4 },
            { "SH", 5 },
            { "TOT", 6 },
            { "GSA1", 7 },
            { "GSA2", 8 },
            { "GSA3", 9 }
        };

        int GameId { get; set; }
        GAME_INFO GameInfo { get; set; }
        List<GOALIE_SUMMARY> GoalieSummaries { get; set; }
        protected override ReportType ReportType { get { return ReportType.GS; } }

        public GameSummary(int gameId)
            : base(gameId)
        {
            GameId = gameId;
        }

        new public void Parse()
        {
            ParseGameInfo();
            ParseGoalieSummary();
        }
        public void ParseGameInfo()
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

        public void ParseGoalieSummary()
        {
            string VISITOR_GOALIE_SUMMARY_XPATH = "//table[@id='MainTable']/tr[15]/td/table/tr[position()>=3 and position()<=4 and (@class='evenColor' or @class='oddColor')]";
            string HOME_GOALIE_SUMMARY_XPATH = "//table[@id='MainTable']/tr[15]/td/table/tr[position()>=8 and position()<=11 and (@class='evenColor' or @class='oddColor')]";
            foreach (HtmlNode n in Document.DocumentNode.SelectNodes(HOME_GOALIE_SUMMARY_XPATH))
            {
                List<String> values = new List<string>();
                foreach (HtmlNode child in n.SelectNodes("td"))
                {
                    string val = child.InnerText == "&nbsp;" ? "0" : child.InnerText;
                    values.Add(val);
                }
                SaveGoalie(values, GameInfo.HOME_TEAM);
            }
            foreach (HtmlNode n in Document.DocumentNode.SelectNodes(VISITOR_GOALIE_SUMMARY_XPATH))
            {
                List<String> values = new List<string>();
                foreach (HtmlNode child in n.SelectNodes("td"))
                {
                    string val = child.InnerText == "&nbsp;" ? "0" : child.InnerText;
                    values.Add(val);
                }
                SaveGoalie(values, GameInfo.VISITOR_TEAM);
            }
        }

        public void SaveGoalie(List<string> stats, String team)
        {
            using (NLPoolEntities context = new NLPoolEntities())
            {
                PLAYER_SUMMARY ps;
                int playerNumber = (int)Convert.ChangeType(GetValue(stats, "PLAYER_NUMBER"), typeof(int));
                ps = context.PLAYER_SUMMARY
                    .Where(g => g.GAME_ID == GameId &&
                        g.TEAM == team &&
                        g.PLAYER_NUMBER == playerNumber)
                        .FirstOrDefault();

                if (ps == null)
                {
                    ps = new PLAYER_SUMMARY();
                    ps.TEAM = team;
                    ps.GAME_ID = GameInfo.ID;
                    context.PLAYER_SUMMARY.Add(ps);
                }

                foreach (String s in GSMap.Keys)
                {
                    PropertyInfo pi = ps.GetType().GetProperty(s);
                    Type type = pi.PropertyType;
                    Object value = GetValue(stats, s);

                    if (s == "PLAYER_NAME")
                    {
                        String[] name = stats[GSMap[s]].Split(new Char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                        value = name[0];
                        if (name.Length > 1)
                        {
                            PropertyInfo attrib = ps.GetType().GetProperty(name[1]);
                            attrib.SetValue(ps, 1);
                            int goalsAgainst = team == GameInfo.HOME_TEAM ? GameInfo.VISITOR_SCORE : GameInfo.HOME_SCORE;
                            if (name[1] == "W" && goalsAgainst == 0)
                                ps.SO = 1;

                            else
                                ps.SO = 0;
                        }
                    }

                    pi.SetValue(ps, value);                    
                }
                context.SaveChanges();
            }
        }

        public Object GetValue(List<string> values, string property)
        {
            Type type = new PLAYER_SUMMARY().GetType().GetProperty(property).PropertyType;
            String value = values[GSMap[property]];
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
                    //pi.SetValue(ps, Convert.ChangeType(value, type.GetGenericArguments()[0]));
                }
            }
            else
            {
                result = Convert.ChangeType(value, type);
            }
            return result;
        }
    }
}
