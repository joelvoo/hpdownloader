using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NHLHTMLReports
{
    public class Config
    {
        static Config()
        {            
            //Assembly assembly = Assembly.GetExecutingAssembly();
            //XmlSerializer xml = new XmlSerializer(typeof(Config));
            //TextReader reader = new StreamReader(@"D:\myXml.xml");
            //TextReader reader = new StreamReader(assembly.Location+@"\mapping.xml");

            //object obj = deserializer.Deserialize(reader);
            //AddressDirectory XmlData = (AddressDirectory)obj;
            //reader.Close();
        }

        static public WebProxy Proxy { get; set; }
    }
}
