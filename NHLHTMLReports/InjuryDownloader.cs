using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NHLHTMLReports
{
    public class InjuryDownloader
    {
        string BaseUrl = "http://stats.tsn.ca/GET/urn:tsn:nhl:injuries?type=json";
        string Referer = "http://www.tsn.ca/nhl/injuries";
        Log log = Log.Instance;

        WebProxy Proxy;
        public InjuryDownloader(WebProxy proxy)
        {
            Proxy = proxy;
        }
        public void Download()
        {            
            StreamReader reader = new StreamReader(HTMLHelper.GetResponseStream(BaseUrl,Proxy,Referer));
            String json = reader.ReadToEnd();
            //String injuries = JSONP.ParseJSON(reader.ReadToEnd());

            using (NLPoolEntities context = new NLPoolEntities())
            {
                //context.Database.Connection.Open();
                int result = context.Database.ExecuteSqlCommand(
                    "nlpool.UpdateInjuries  @injuries",
                new SqlParameter("injuries", json));

                log.WriteLine(result.ToString() + " Injuries successfully updated.");
            }           
        }
    }
}
