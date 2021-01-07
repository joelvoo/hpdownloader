using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHLHTMLReports
{
    public partial class NHL_PLAYER
    {
        public string Name
        {
            get
            {
                return FIRST_NAME + " " + LAST_NAME;
            }
        }
        public string SeoName
        {
            get
            {
                return (FIRST_NAME + "-" + LAST_NAME).Replace(' ','-').ToLower();
            }
        }
    }
}
