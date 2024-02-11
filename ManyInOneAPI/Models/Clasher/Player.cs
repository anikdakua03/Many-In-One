namespace ManyInOneAPI.Models.Clasher
{
    public class Player
    {
        public string? tag { get; set; }
        public string? name { get; set; }
        public int townHallLevel { get; set; }
        public int townHallWeaponLevel { get; set; }
        public int expLevel { get; set; }
        public int trophies { get; set; }
        public int bestTrophies { get; set; }
        public int warStars { get; set; }
        public int attackWins { get; set; }
        public int defenseWins { get; set; }
        public int builderHallLevel { get; set; }
        public int builderBaseTrophies { get; set; }
        public int bestBuilderBaseTrophies { get; set; }
        public string? role { get; set; }
        public string? warPreference { get; set; }
        public int donations { get; set; }
        public int donationsReceived { get; set; }
        public int clanCapitalContributions { get; set; }
        public Clan? clan { get; set; }
        public League? league { get; set; }
        public BuilderBaseLeague? builderBaseLeague { get; set; }
        public LegendStatistics? legendStatistics { get; set; }
        public List<Achievement>? achievements { get; set; }
        public PlayerHouse? playerHouse { get; set; }
        public List<Label>? labels { get; set; }
        public List<Troop>? troops { get; set;}
        public List<Hero>? heroes { get; set; }
        public List<HeroEquipment>? heroEquipment {  get; set; }
        public List<Spell>? spells { get; set; }
    }

    public class Clan
    {
        public string? tag { get; set; }
        public string? name { get; set; }
        public int clanLevel { get; set; }
        public BadgeUrls? badgeUrls { get; set; }
        public class BadgeUrls
        {
            public string? small { get; set; }
            public string? large { get; set; }
            public string? medium { get; set; }
        }
    }
    public class League
    {
        public int id { get; set; }
        public string? name { get; set; }
        public IconUrls? iconUrls { get; set; }
    }
    public class IconUrls
    {
        public string? small { get; set; }
        public string? tiny { get; set; }
        public string? medium { get; set; }
    }
    public class BuilderBaseLeague
    {
        public int id { get; set; }
        public string? name { get; set; }
    }
    public class LegendStatistics
    {
        public int legendTrophies { get; set; }
        public PreviousSeason? previousSeason { get; set; }
        public BestSeason? bestSeason { get; set; }
        public CurrentSeason? currentSeason { get; set; }

        public class PreviousSeason
        {
            public string? id { get; set; }
            public int rank { get; set; }
            public int trophies { get; set; }
        }
        public class BestSeason
        {
            public string? id { get; set; }
            public int rank { get; set; }
            public int trophies { get; set; }
        }

        public class CurrentSeason
        {
            public int rank { get; set; }
            public int trophies { get; set; }
        }
    }
    public class Achievement
    {
        public string? name { get; set; }
        public int stars { get; set; }
        public int value { get; set; }
        public int target { get; set; }
        public string? info { get; set; }
        public string? completionInfo { get; set; }
        public string? village { get; set; }
    }
    public class PlayerHouse
    {
        public List<Element>? elements { get; set;}

        public class Element 
        {
            public int id { get; set; }
            public string? type { get; set; }
        }
    }
    public class Label
    {
        public int id { get; set; }
        public string? name { get; set; }
        public IconUrls? iconUrls { get; set; }
        public class IconUrls
        {
            public string? small { get; set; }
            public string? medium { get; set; }
        }
    }
    public class Troop
    {
        public string? name { get; set; }
        public int level { get; set; }
        public int maxLevel { get; set; }
        public string? village { get; set; }
        public bool superTroopIsActive { get; set; }
    }
    public class Hero
    {
        public string? name { get; set; }
        public int level { get; set; }
        public int maxLevel { get; set; }
        public List<Equipment>? equipment { get; set; }
        public string? village { get; set; }
    }
    public class Equipment
    {
        public string? name { get; set; }
        public int level { get; set; }
        public int maxLevel { get; set; }
        public string? village { get; set; }
    }
    public class HeroEquipment
    {
        public string? name { get; set; }
        public int level { get; set; }
        public int maxLevel { get; set; }
        public string? village { get; set; }
    }
    public class Spell
    {
        public string? name { get; set; }
        public int level { get; set; }
        public int maxLevel { get; set; }
        public string? village { get; set; }
    }
}
