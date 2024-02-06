namespace ManyInOneAPI.Models.Clasher
{
    public class WarLog
    {
        public List<All>? items {  get; set; }
        public Paging? paging { get; set; }
    }

    public class All
    {
        public string? result { get; set; }
        public string? endTime { get; set; }
        public int teamSize { get; set; }
        public int attacksPerMember { get; set;}
        public WarClan? clan { get; set; }
        public Opponent? opponent { get; set; }
    }
    public class WarClan : Clan
    {
        public int attacks { get; set; }
        public int stars { get; set; }
        public double destructionPercentage { get; set; }
        public int expEarned { get; set; }  
    }
    public class Opponent : Clan
    {
        public int stars { get; set; }
        public double destructionPercentage { get; set; }
    }
}
