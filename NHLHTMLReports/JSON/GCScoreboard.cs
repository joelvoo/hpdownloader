using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace NHLHTMLReports
{
    [DataContract]
    public class GCScoreboard
    {
        [DataMember(Name = "games")]
        public Game[] Games { get; set; }
        [DataMember(Name = "refreshInterval")]
        public int RefreshInterval { get; set; }
        [DataMember(Name = "currentDate")]
        public string CurrentDate { get; set; }
        [DataMember(Name = "nextDate")]
        public string NextDate { get; set; }
        [DataMember(Name = "prevDate")]
        public string PreviousDate { get; set; }
        
    }
    
    [DataContract]
    public class Game
    {
        [DataMember(Name = "ata")]
        public String AwayTeamAcronym { get; set; }
        [DataMember(Name = "atc")]
        public String ATC { get; set; }
        [DataMember(Name = "atcommon")]
        public String AwayTeamCommonName { get; set; }
        [DataMember(Name = "atn")]
        public String AwayTeamCityName { get; set; }
        [DataMember(Name = "ats")]
        public String AwayTeamScore { get; set; }
        [DataMember(Name = "atsog")]
        public String AwayTeamShotsOnGoal { get; set; }
        [DataMember(Name = "bs")]
        public String StartTime { get; set; }
        [DataMember(Name = "bsc")]
        public String BSC { get; set; }
        [DataMember(Name = "canationalbroadcasts")]
        public String CanadianBroadcasts { get; set; }
        [DataMember(Name = "gcl")]
        public Boolean GameCenterLive { get; set; }
        [DataMember(Name = "gcll")]
        public Boolean GCLL { get; set; }
        [DataMember(Name = "gs")]
        public String GameStatus { get; set; }
        [DataMember(Name = "hta")]
        public String HomeTeamAcronym { get; set; }
        [DataMember(Name = "htc")]
        public String HTC { get; set; }
        [DataMember(Name = "htcommon")]
        public String HomeTeamCommonName { get; set; }
        [DataMember(Name = "htn")]
        public String HomeTeamCityName { get; set; }
        [DataMember(Name = "hts")]
        public String HomeTeamScore { get; set; }
        [DataMember(Name = "htsog")]
        public String HomeTeamShotsOnGoal { get; set; }
        [DataMember(Name = "id")]
        public int GameID { get; set; }
        [DataMember(Name = "rl")]
        public Boolean RL { get; set; }
        [DataMember(Name = "usnationalbroadcasts")]
        public String USBroadcasts { get; set; }
    }
}
