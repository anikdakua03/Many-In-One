namespace ManyInOneAPI.Models.Clasher
{
    public class RankedClanCapital
    {
        public List<ClanCapitalRanking>? items { get; set; }
        public Paging? paging { get; set; }
    }

    public class ClanCapitalRanking
    {
        public string? tag { get; set; }
        public string? name { get; set; }
        public ClanLoc? location { get; set; }
        public BadgeUrls? badgeUrls { get; set; }
        public int clanLevel { get; set; }
        public int members { get; set; }
        public int clanPoints { get; set; }
        public int rank { get; set; }
        public int previousRank { get; set; }
        public int clanCapitalPoints { get; set; }
    }
}