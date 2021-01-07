using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHLHTMLReports
{
    public class GameDownloader
    {
        Log log = Log.Instance;
        public void Download(Int32 gameId)
        {
            log.WriteLine("Downloading game: " + gameId.ToString());
            try {
                EventSummary es = new EventSummary(gameId);
                es.Parse();

                GameSummary gs = new GameSummary(gameId);
                gs.Parse();
            }
            catch (Exception ex)
            {
                log.WriteLine(ex.ToString());
            }
        }

        public void Download(DateTime date)
        {
            foreach(Int32 id in GetGameIDs(date))
            {
                Download(id);
            }
        }

        public void Download()
        {
            List<Int32> ids = GetActiveGameIDs();
            foreach (Int32 id in ids)
            {
                Download(id);
            }
        }

        public void Download(DateTime start, DateTime end)
        {
            for (DateTime date = start; date.Date <= end.Date; date = date.AddDays(1))
            {
                Download(date);
            }

        }

        public void Download(Int32 start, Int32 end)
        {
            for (Int32 id = start; id <= end; id++)
            {
                Download(id);
            }

        }
        
        public List<Int32> GetActiveGameIDs()
        {
            DateTime activeDate = DateTime.Now;

            using (NLPoolEntities context = new NLPoolEntities())
            {
                return context.GAME_INFO
                    .Where(g => g.START_TIME <= activeDate && g.END_TIME == null)
                    .Select(g => g.ID).ToList<Int32>();
            }

            
        }

        public List<Int32> GetGameIDs(DateTime date)
        {
            using (NLPoolEntities context = new NLPoolEntities())
            {
                DateTime dayAfter = date.Date.AddDays(1);
                return context.GAME_INFO
                    .Where(g => g.START_TIME >= date.Date && g.START_TIME <= dayAfter)
                    .Select(g => g.ID).ToList<Int32>();
            }
        }
    }
}
