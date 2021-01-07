using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NHLHTMLReports.JSON
{

    //public class CurrentTeam
    //{
    //    public int id { get; set; }
    //    public string name { get; set; }
    //    public string link { get; set; }
    //}

    //public class PrimaryPosition
    //{
    //    public string code { get; set; }
    //    public string name { get; set; }
    //    public string type { get; set; }
    //    public string abbreviation { get; set; }
    //}

    //public class Person
    //{
    //    public int id { get; set; }
    //    public string fullName { get; set; }
    //    public string link { get; set; }
    //    public string firstName { get; set; }
    //    public string lastName { get; set; }
    //    public string primaryNumber { get; set; }
    //    public string birthDate { get; set; }
    //    public int currentAge { get; set; }
    //    public string birthCity { get; set; }
    //    public string birthStateProvince { get; set; }
    //    public string birthCountry { get; set; }
    //    public string nationality { get; set; }
    //    public string height { get; set; }
    //    public int weight { get; set; }
    //    public bool active { get; set; }
    //    public bool alternateCaptain { get; set; }
    //    public bool captain { get; set; }
    //    public bool rookie { get; set; }
    //    public string shootsCatches { get; set; }
    //    public string rosterStatus { get; set; }
    //    public CurrentTeam currentTeam { get; set; }
    //    public PrimaryPosition primaryPosition { get; set; }
    //}
    //public class Position
    //{
    //    public string code { get; set; }
    //    public string name { get; set; }
    //    public string type { get; set; }
    //    public string abbreviation { get; set; }
    //}

    //[DataContract]
    //public class PlayerResult
    //{
    //    [DataMember]
    //    public string copyright { get; set; }
    //    [DataMember]
    //    public List<Person> people { get; set; }
    //}
    //public class Roster
    //{
    //    public Person person { get; set; }
    //    public Position position { get; set; }
    //    public string jerseyNumber { get; set; }
    //}

    //[DataContract]
    //public class RosterResult
    //{
    //    [DataMember]
    //    public string copyright { get; set; }
    //    [DataMember]
    //    public List<Roster> roster { get; set; }
    //    public string link { get; set; }
    //}
    public class TimeZone
    {
        public string id { get; set; }
        public int offset { get; set; }
        public string tz { get; set; }
    }

    public class Venue
    {
        public int id { get; set; }
        public string name { get; set; }
        public string link { get; set; }
        public string city { get; set; }
        public TimeZone timeZone { get; set; }
    }

    public class Division
    {
        public int id { get; set; }
        public string name { get; set; }
        public string nameShort { get; set; }
        public string link { get; set; }
        public string abbreviation { get; set; }
    }

    public class Conference
    {
        public int id { get; set; }
        public string name { get; set; }
        public string link { get; set; }
    }

    public class CurrentTeam
    {
        public int id { get; set; }
        public string name { get; set; }
        public string link { get; set; }
        public Venue venue { get; set; }
        public string abbreviation { get; set; }
        public string teamName { get; set; }
        public string locationName { get; set; }
        public string firstYearOfPlay { get; set; }
        public Division division { get; set; }
        public Conference conference { get; set; }
        public Franchise franchise { get; set; }
        public string shortName { get; set; }
        public string officialSiteUrl { get; set; }
        public int franchiseId { get; set; }
        public bool active { get; set; }
    }

    public class PrimaryPosition
    {
        public string code { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string abbreviation { get; set; }
    }

    public class Person
    {
        public int id { get; set; }
        public string fullName { get; set; }
        public string link { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string primaryNumber { get; set; }
        public string birthDate { get; set; }
        public int currentAge { get; set; }
        public string birthCity { get; set; }
        public string birthCountry { get; set; }
        public string nationality { get; set; }
        public string height { get; set; }
        public int weight { get; set; }
        public bool active { get; set; }
        public bool alternateCaptain { get; set; }
        public bool captain { get; set; }
        public bool rookie { get; set; }
        public string shootsCatches { get; set; }
        public string rosterStatus { get; set; }
        public CurrentTeam currentTeam { get; set; }
        public PrimaryPosition primaryPosition { get; set; }
    }
    [DataContract]
    public class PlayerResult
    {
        [DataMember]
        public string copyright { get; set; }
        [DataMember]
        public List<Person> people { get; set; }
    }
    [DataContract]
    public class Position
    {
        public string code { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string abbreviation { get; set; }
    }

    public class RosterPlayer
    {
        public Person person { get; set; }
        public string jerseyNumber { get; set; }
        public Position position { get; set; }
    }

    public class Roster
    {
        public List<RosterPlayer> roster { get; set; }
        public string link { get; set; }
    }

    public class Franchise
    {
        public int franchiseId { get; set; }
        public int firstSeasonId { get; set; }
        public int mostRecentTeamId { get; set; }
        public string teamName { get; set; }
        public string locationName { get; set; }
        public string link { get; set; }
        public Roster roster { get; set; }
    }

    public class Team
    {
        public int id { get; set; }
        public string name { get; set; }
        public string link { get; set; }
        public Venue venue { get; set; }
        public string abbreviation { get; set; }
        public string teamName { get; set; }
        public string locationName { get; set; }
        public string firstYearOfPlay { get; set; }
        public Division division { get; set; }
        public Conference conference { get; set; }
        public Franchise franchise { get; set; }
        public string shortName { get; set; }
        public string officialSiteUrl { get; set; }
        public int franchiseId { get; set; }
        public bool active { get; set; }
    }

    public class RosterResult
    {
        public string copyright { get; set; }
        public List<Team> teams { get; set; }
    }
}