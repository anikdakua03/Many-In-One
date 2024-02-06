namespace ManyInOneAPI.Models.Clasher
{
    public class CurrentClanWarLeagueGroup
    {
        public string? state { get; set; }
        public string? season { get; set; }
        public List<Clans>? clans { get; set; }
        public List<Round>? rounds { get; set; }
    }

    public class Clans : Clan
    {
        public List<Member>? members { get; set; }
        public class Member
        {
            public string? tag { get; set; }
            public string? name { get; set; }
            public int townHallLevel { get; set; }
        }
    }
    public class Round
    {
        public List<string>? warTags { get; set; }
    }
}
