using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NHLHTMLReports
{
    static public class HTMLHelper
    {
        static public Stream GetResponseStream(string html, WebProxy proxy = null, string referer = null )
        {
            try
            {
                //HttpClient client = new();

                var wr = (HttpWebRequest) WebRequest.Create(html);
                if (Config.Proxy != null)
                {
                    Config.Proxy.UseDefaultCredentials = true;
                    wr.Proxy = Config.Proxy;
                }
                if (referer != null)
                    wr.Referer = referer;

                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                WebResponse resp = wr.GetResponse();
                return resp.GetResponseStream();
            }
            catch (WebException ex)
            {
                Log.Instance.WriteLine(ex.ToString());
                return null;
            }
        }

    }
}
