//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NHLHTMLReports
{
    using System;
    using System.Collections.Generic;
    
    public partial class GOALIE_SUMMARY
    {
        public int ID { get; set; }
        public Nullable<int> GAME_ID { get; set; }
        public string TEAM { get; set; }
        public Nullable<int> PLAYER_NUMBER { get; set; }
        public string POS { get; set; }
        public string PLAYER_NAME { get; set; }
        public Nullable<System.DateTime> EV { get; set; }
        public Nullable<System.DateTime> PP { get; set; }
        public Nullable<System.DateTime> SH { get; set; }
        public Nullable<System.DateTime> TOT { get; set; }
        public string GSA1 { get; set; }
        public string GSA2 { get; set; }
        public string GSA3 { get; set; }
        public Nullable<int> W { get; set; }
        public Nullable<int> L { get; set; }
        public Nullable<int> OT { get; set; }
        public Nullable<int> SO { get; set; }
        public Nullable<int> PlayerId { get; set; }
    }
}