namespace ManyInOneAPI.Models.Clasher
{
    public class ClanInfo
    {
        public string? tag { get; set; }
        public string? name { get; set; }
        public string? type { get; set; }
        public string? description { get; set; }
        public ClanLocation? location { get; set; }
        public bool isFamilyFriendly { get; set; }
        public BadgeUrls? badgeUrls { get; set; }
        public int clanLevel { get; set; }
        public int clanPoints { get; set; }
        public int clanBuilderBasePoints { get; set; }
        public int clanCapitalPoints { get; set; }
        public CapitalLeague? capitalLeague { get; set; }
        public int requiredTrophies { get; set; }
        public string? warFrequency { get; set; }
        public int warWinStreak { get; set; }
        public int warWins { get; set; }
        public int warTies {  get; set; }
        public int warLosses { get; set; }
        public bool isWarLogPublic { get; set; }
        public WarLeague? warLeague { get; set; }
        public int members { get; set; }
        public List<MemberInfo>? memberList { get; set; }
        public List<Label>? labels { get; set; }
        public int requiredBuilderBaseTrophies { get; set; }
        public int requiredTownhallLevel { get; set; }
        public ClanCapital? clanCapital { get; set; }
    }
    public class ClanLocation
    {
        public int id { get; set; }
        public string? name { get; set; }
        public bool isCountry { get; set; }
    }
    public class BadgeUrls
    {
        public string? small { get; set; }
        public string? large { get; set; }
        public string? medium { get; set; }
    }
    public class CapitalLeague
    {
        public int id { get; set; }
        public string? name { get; set; }
    }
    public class WarLeague
    {
        public int id { get; set; }
        public string? name { get; set; }
    }
    public class MemberInfo
    {
        public string? tag { get; set; }
        public string? name { get; set; }
        public string? role { get; set; }
        public int townHallLevel { get; set; }
        public int expLevel { get; set; }
        public League? league { get; set; }
        public int trophies { get; set; }
        public int builderBaseTrophies { get; set; }
        public int clanRank { get; set; }
        public int previousClanRank { get; set; }
        public int donations { get; set; }
        public int donationsReceived { get; set; }
        public PlayerHouse? playerHouse { get; set; }
        public BuilderBaseLeague? builderBaseLeague { get; set; }
    }
    public class ClanCapital
    {
        public int capitalHallLevel { get; set; }
        public List<District>? districts { get; set; }
    }
    public class District
    {
        public int id { get; set; }
        public string? name { get; set; }
        public int districtHallLevel { get; set; }
    }
}
