namespace ManyInOneAPI.Models.Clasher
{
    public class RankedPlayer
    {
        public List<PlayerRanking>? items { get; set; }
        public Paging? paging { get; set; }
    }

    public class PlayerRanking
    {
        public string? tag { get; set; }
        public string? name { get; set; }
        public int expLevel { get; set; }
        public int trophies { get; set; }
        public int attackWins { get; set; }
        public int defenseWins { get; set; }
        public int rank { get; set; }
        public int previousRank { get; set; }
        public LittleClanInfo? clan { get; set; }
    }

    public class LittleClanInfo
    {
        public string? tag { get; set; }
        public string? name { get; set; }
        public BadgeUrls? badgeUrls { get; set; }
        public League? league { get; set; }
    }
}
