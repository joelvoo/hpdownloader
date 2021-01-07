using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NHLHTMLReports
{
    public class PlayerDownloader
    {
        public void Download(int playerId)
        {
            try
            {
                PlayerJSON pj = new PlayerJSON(playerId);
                pj.Parse();
            }
            catch (WebException we)
            {
                Log.Instance.WriteLine("Failed with {0} with error:\n {1}", new string[] { playerId.ToString(), we.ToString() });
            }
        }

        public void Download(int startId, int endId)
        {
            for (int i = startId; i <= endId; i++)
            {
                Download(i);
            }
        }
        public void Download()
        {

            RosterJSON pj = new RosterJSON();
            pj.Parse();

        }
    }
}
