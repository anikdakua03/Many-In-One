namespace ManyInOneAPI.Models.Clasher
{
    public class Location
    {
        public List<Item>? items { get; set; }
        //public Paging? paging { get; set; }
    }
    public class Item
    {
        public int id { get; set; }
        public string? name { get; set; }
        public bool isCountry { get; set; }
    }

    public class Paging
    {
        // No properties defined as the "cursors" object within "paging" is empty.
        //public string? Cursors { get; set; }
    }
}
