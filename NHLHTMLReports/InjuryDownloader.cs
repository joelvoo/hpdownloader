using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace NHLHTMLReports
{
    public class InjuryDownloader
    {
        string BaseUrl = "https://stats.sports.bellmedia.ca/sports/hockey/leagues/nhl/playerInjuries?type=json";
        string Referer = "https://www.tsn.ca/nhl/injuries";
        Log log = Log.Instance;

        WebProxy Proxy;
        public InjuryDownloader(WebProxy proxy)
        {
            Proxy = proxy;
        }
        public async Task DownloadAsync()
        {
            HttpClient client = new HttpClient();

            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            try
            {

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, BaseUrl);
                //requestMessage.Headers.TransferEncodingChunked = true;
                //requestMessage.Headers.Connection.Add("keep-alive");
                //requestMessage.Headers.Add("X-Frame-Options", "DENY");
                //requestMessage.Headers.Add("X-Content-Type-Options", "nosniff");
                //requestMessage.Headers.Add("X-Xss-Protection", "0");
                //requestMessage.Headers.Add("Vary","accept-encoding,Origin");
                //requestMessage.Headers.Add("Age", "128");
                //requestMessage.Headers.Add("Content-Type", "application/json");
                
                HttpResponseMessage response = await client.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();

                //StreamReader reader = new StreamReader(HTMLHelper.GetResponseStream(BaseUrl,Proxy));
                String json = await response.Content.ReadAsStringAsync();
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
            catch (HttpRequestException ex)
            {
                log.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                log.WriteLine(ex.ToString());
            }
        }
    }
}
