using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace NHLHTMLReports
{
    public class ScheduleDownloader
    {

        string BaseUrl = "https://statsapi.web.nhl.com/api/v1/schedule?startDate={0}&endDate={1}&expand=schedule.teams";
        WebProxy Proxy;
        public ScheduleDownloader(WebProxy proxy)
        {
            Proxy = proxy;
        }

        public void DownloadGames(DateTime startDate, DateTime endDate)
        {
            string url = String.Format(BaseUrl, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

            try
            {
                Log.Instance.WriteLine(String.Format("Downloading schedule between {0} and {1}",startDate,endDate));
                Log.Instance.WriteLine(String.Format("Using url: {0}", url));
                StreamReader reader = new StreamReader(HTMLHelper.GetResponseStream(url));
                String json = reader.ReadToEnd();

                using (NLPoolEntities context = new NLPoolEntities())
                {
                    context.Database.ExecuteSqlCommand("nlpool.ParseSchedule @schedule",
                        new SqlParameter("schedule", json));
                }
            }
            catch (WebException we)
            {
                HttpWebResponse errorResponse = we.Response as HttpWebResponse;
                if (errorResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    Log.Instance.WriteLine("Problem with schedule source: " + url);
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLine(ex.ToString());
            }


        }
    }
}
