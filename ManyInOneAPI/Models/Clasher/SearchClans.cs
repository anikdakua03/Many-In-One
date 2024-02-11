namespace ManyInOneAPI.Models.Clasher
{
    public class SearchClans
    {
        public List<SearchClanInfo>? items { get; set; }
        public Paging? paging { get; set; }
    }
    public class SearchClanInfo
    {
        public string? tag { get; set; }
        public string? name { get; set; }
        public string? type { get; set; }
        public ClanLoc? location {  get; set; }
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
        public int warTies { get; set; }
        public int warLosses { get; set; }
        public bool isWarLogPublic { get; set; }
        public WarLeague? warLeague { get; set; }
        public int members { get; set; }
        public List<Label>? labels { get; set; }
        public int requiredBuilderBaseTrophies { get; set; }
        public int requiredTownhallLevel { get; set; }
        public ChatLanguage? chatLanguage { get; set; }

    }
    public class ClanLoc : ClanLocation
    {
        public string? countryCode { get; set; }
    }
    public class ChatLanguage
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? languageCode { get; set; }
    }
}
