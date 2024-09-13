using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
                var wr = (HttpWebRequest) WebRequest.Create(html);
                if (Config.Proxy != null)
                {
                    Config.Proxy.UseDefaultCredentials = true;
                    wr.Proxy = Config.Proxy;
                }
                if (referer != null)
                    wr.Referer = referer;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                return wr.GetResponse().GetResponseStream();
            }
            catch (WebException ex)
            {
                Log.Instance.WriteLine(ex.ToString());
                return null;
            }
        }

    }
}
