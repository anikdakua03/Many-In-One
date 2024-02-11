namespace ManyInOneAPI.Models.Clasher
{
    public class CapitalRaidSeason
    {
        public List<CapitalLog>? items { get; set; }
        public Paging? paging { get; set; }
    }

    public class CapitalLog
    {
        public string? state { get; set; }
        public string? startTime { get; set; }
        public string? endTime {  get; set; }
        public int capitalTotalLoot {  get; set; }
        public int raidsCompleted { get; set; }
        public int totalAttacks {  get; set; }
        public int enemyDistrictsDestroyed { get; set; }
        public int offensiveReward { get; set; }
        public int defensiveReward { get; set; }
        public List<CapMemberInfo>? members { get; set; }
        public List<AttackLog>? attackLog { get; set; }
        public List<DefenseLog>? defenseLog { get; set; }
    }

    public class DefenseLog
    {
        public DefAttacker? attacker { get; set; }
        public int attackCount { get; set; }
        public int districtCount { get; set; }
        public int districtsDestroyed { get; set; }
        public List<CapDistrict>? districts { get; set; }
    }

    public class DefAttacker
    {
        public string? tag { get; set; }
        public string? name { get; set; }
        public int level { get; set; }
        public BadgeUrls? badgeUrls { get; set; }
    }

    public class CapMemberInfo
    {
        public string? tag { get; set; }
        public string? name { get; set; }
        public int attacks { get; set; }
        public int attackLimit { get; set; }
        public int bonusAttackLimit { get; set; }
        public int capitalResourcesLooted { get; set; }
    }
    public class AttackLog
    {
        public Defender? defender { get; set; }
        public int attackCount { get; set; }
        public int districtCount { get; set; }
        public int districtsDestroyed { get; set; }
        public List<CapDistrict>? districts { get; set; }
    }

    public class CapDistrict
    {
        public int id { get; set; }
        public string? name { get; set; }
        public int districtHallLevel { get; set; }
        public int destructionPercent { get; set; }
        public int stars { get; set; }
        public int attackCount { get; set; }
        public int totalLooted { get; set; }
        public List<Attacks>? attacks { get; set; }
    }

    public class Attacks
    {
        public Attacker? attacker { get; set; }
        public int destructionPercent { get; set; }
        public int stars { get; set; }
    }
    public class Attacker
    {
        public string? tag { get; set; }
        public string? name { get; set; }
    }

    public class Defender
    {
        public string? tag { get; set; }
        public string? name { get; set; }
        public int level { get; set; }
        public BadgeUrls? badgeUrls { get; set; }
    }
}
