namespace ManyInOneAPI.Models.Clasher
{
    public class NormalLeague
    {
        public List<Normal>? items { get; set; }
        public Paging? paging { get; set; }
    }
    public class Normal
    {
        public int id { get; set; }
        public string? name { get; set; }
        public IconUrls? iconUrls { get; set; }
    }
}
