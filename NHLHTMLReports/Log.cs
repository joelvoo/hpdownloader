using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NHLHTMLReports
{
    public sealed class Log
    {
        private static Log instance = new Log();
        private Log()
        {
        }

        public static Log Instance { get { return instance; } }
        public String FileName
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(System.Environment.CurrentDirectory);
                sb.Append("\\logs\\log_");
                sb.Append(DateTime.Today.Year.ToString());
                sb.Append(DateTime.Today.Month.ToString());
                sb.Append(DateTime.Today.Day.ToString());
                sb.Append(".txt");
                return sb.ToString();
            }
        }

        public void WriteLine(String message, object[] args)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(FileName);
            file.Directory.Create();
            StreamWriter w = File.AppendText(FileName);
            w.Write("{0}  ",DateTime.Now.ToString());
            w.WriteLine(message, args);

            w.Close();
        }

        public void WriteLine(String message)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(FileName);
            file.Directory.Create();
            StreamWriter w = File.AppendText(FileName);
            w.Write("{0}  ", DateTime.Now.ToString());
            w.WriteLine(message);

            w.Close();
        }
    }
}
