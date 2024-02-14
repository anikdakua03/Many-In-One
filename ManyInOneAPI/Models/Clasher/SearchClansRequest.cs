using System.ComponentModel.DataAnnotations;

namespace ManyInOneAPI.Models.Clasher
{
    public class SearchClansRequest
    {
        [Required]
        public string? Name { get; set; }
        public string? WarFrequency {  get; set; }
        public int LocationId { get; set; }
        public int MinMembers { get; set; }
        public int MaxMembers { get; set; }
        public int MinClanPoints { get; set; }
        public int MinClanLevel { get; set; }
        public List<int>? Labels {  get; set; }
        public int Limit { get; set; } = 50;
    }
}
