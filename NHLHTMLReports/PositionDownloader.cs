using HtmlAgilityPack;
using NHLHTMLReports.JSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NHLHTMLReports
{
    public class PositionDownloader
    {
        string BaseUrl = "http://stats.tsn.ca/HGET/urn:tsn:nhl:player:{0}/header?type=json";
        string Referer = "http://www.tsn.ca/nhl/player-bio";
        WebProxy Proxy;
        public PositionDownloader(WebProxy proxy)
        {
            Proxy = proxy;
        }
        public void UpdatePosition(NHL_PLAYER player)
        {
            string name = player.TSN_NAME ?? player.SeoName;
            string url = String.Format(BaseUrl, name);
            var stream = HTMLHelper.GetResponseStream(url, Proxy, Referer);
            var serializer = new DataContractJsonSerializer(typeof(PlayerHeader));
            var result = new PlayerHeader();

            if (stream == null)
            {
                player.TSN_NAME = null;
                return;
            }                
            result = (PlayerHeader)serializer.ReadObject(stream);

            string[] positions = result.PositionAcronym.Split('/');

            StringBuilder sb = new StringBuilder();
            foreach (var position in positions)
            {
                switch(position)
                {
                    case "C":
                        sb.Append("C");
                        break;
                    case "RW":
                        sb.Append("R");
                        break;
                    case "LW":
                        sb.Append("L");
                        break;
                    case "W":
                        sb.Append("RL");
                        break;
                    case "D":
                        sb.Append("D");
                        break;
                    case "G":
                        sb.Append("G");
                        break;
                }
            }

            if (sb.ToString() != "")
            {
                player.TSN_NAME = name;
                player.ELIGIBLE_POSITION = sb.ToString();
            }

        }
        public void Download(int startId, int endId)
        {
            using (NLPoolEntities context = new NLPoolEntities())
            {
                foreach (NHL_PLAYER player in context.NHL_PLAYER.Where(p => p.ID >= startId && p.ID <= endId))
                {
                    UpdatePosition(player);
                }
                context.SaveChanges();
            }
        }

        public void Download()
        {
            using (NLPoolEntities context = new NLPoolEntities())
            {
                foreach (NHL_PLAYER player in context.NHL_PLAYER)
                {
                    try
                    {
                        UpdatePosition(player);
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.WriteLine(ex.ToString());
                        continue;
                    }
                }
                context.SaveChanges();
                foreach (NHL_PLAYER player in context.NHL_PLAYER.Where(p=> p.ELIGIBLE_POSITION == ""))
                {
                    PlayerJSON pj = new PlayerJSON(player.ID);
                    pj.Parse();
                }
            }
        }
    }
}
