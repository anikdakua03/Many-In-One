namespace ManyInOneAPI.Models.Clasher
{
    public class RankedBuilderBasePlayer
    {
        public List<BuilderPlayerRanking>? items { get; set; }
        public Paging? paging { get; set; }
    }

    public class BuilderPlayerRanking
    {
        public string? tag { get; set; }
        public string? name { get; set; }
        public int expLevel { get; set; }
        public int builderBaseTrophies { get; set; }
        public int rank { get; set; }
        public int previousRank { get; set; }
        public ClnInfo? clan { get; set; }
        public BuilderBaseLeague? builderBaseLeague { get; set; }
    }

    public class ClnInfo
    {
        public string? tag { get; set; }
        public string? name { get; set; }
        public BadgeUrls? badgeUrls { get; set; }
    }
}