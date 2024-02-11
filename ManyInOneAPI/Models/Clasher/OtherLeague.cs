namespace ManyInOneAPI.Models.Clasher
{
    public class OtherLeague
    {
        public List<Other>? items { get; set; }
        public Paging? paging { get; set; }
    }

    public class Other
    {
        public int id { get; set; }
        public string? name { get; set; }
    }
}
