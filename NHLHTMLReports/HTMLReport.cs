using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NHLHTMLReports
{
    public class HTMLReport
    {
        public HTMLReport() { }

        protected HtmlDocument Document { get; set; }
        String Season { get; set; }
        protected virtual ReportType ReportType { get; set; }
        SeasonType SeasonType { get; set; }
        String GameNumber { get; set; }
        SByte Round { get; set; }
        SByte Series { get; set; }
        SByte Game { get; set; }

        public HTMLReport(int gameId, WebProxy proxy = null)
        {
            int year = gameId / 1000000;
            int game = gameId % 1000000;

            SeasonType = (SeasonType)Enum.Parse(typeof(SeasonType), (game / 10000).ToString());
            Season = year.ToString() + (year + 1).ToString();
            GameNumber = (game % 10000).ToString();

            Document = new HtmlDocument();
            Document.Load(HTMLHelper.GetResponseStream(ReportURL, Config.Proxy));
        }

        public HTMLReport(String url, WebProxy proxy = null)
        {
            Document = new HtmlDocument();
            Document.Load(HTMLHelper.GetResponseStream(url, proxy));
        }

        public HTMLReport(string season, ReportType reportType, SeasonType seasonType, string gameNumber, WebProxy proxy = null)
        {
            Season = season;
            ReportType = reportType;
            SeasonType = seasonType;
            GameNumber = gameNumber;

            Document = new HtmlDocument();
            Document.Load(HTMLHelper.GetResponseStream(ReportURL, proxy));
        }

        public String BaseUrl
        {
            get
            {
                return "http://www.nhl.com/scores/htmlreports/";
            }
        }

        public String ReportFilename
        {
            get
            {
                if (GameNumber != null)
                {
                    return GetReportFilename(ReportType, SeasonType, GameNumber);
                }
                else if (SeasonType == SeasonType.POST && Round > 0 && Series > 0 && Game > 0)
                {
                    return GetReportFilename(ReportType, SeasonType, Round, Series, Game);
                }
                else
                    throw new InvalidParametersException();
            }
        }

        public String ReportURL
        {
            get
            {
                return BaseUrl + Season + "/" + ReportFilename;
            }
        }

        public static string GetReportFilename(ReportType reportType, SeasonType seasonType, string gameNumber)
        {
            Object type = Convert.ChangeType(seasonType, seasonType.GetTypeCode());
            return reportType.ToString() + type.ToString().PadLeft(2, '0') + gameNumber.PadLeft(4, '0') + ".HTM";
        }

        public static string GetReportFilename(ReportType reportType, SeasonType seasonType, int round, int series, int game)
        {
            if ((seasonType != SeasonType.POST) ||
                !(round >= 1 && round <= 4) ||
                !(series >= 1 && series <= 8 / Math.Pow(2, round - 1)) ||
                !(game >= 1 && game <= 7))
            {
                throw new InvalidParametersException();
            }

            Object type = Convert.ChangeType(seasonType, seasonType.GetTypeCode());
            string gameNumber = round.ToString().PadLeft(2, '0') + series.ToString() + game.ToString();
            return reportType.ToString() + type.ToString().PadLeft(2, '0') + gameNumber.PadLeft(4, '0') + ".HTM";
        }

        public void Parse() { }

        protected string GetTimeZoneString(string abbreviation)
        {
            switch (abbreviation)
            {
                case "PST":
                    return " -8";
                case "PDT":
                case "MST":
                    return " -7";
                case "MDT":
                case "CST":
                    return " -6";
                case "CDT":
                case "EST":
                    return " -5";
                case "EDT":
                    return " -4";
            }

            return "";
        }

    }

    public class InvalidParametersException : Exception
    {

    }

    public enum ReportType
    {
        ES, GS, FC, PL, TV, TH, RO, SS, SO
    }

    public enum SeasonType
    {
        PRE = 1, REG = 2, POST = 3
    }

}
