namespace ManyInOneAPI.Models.Clasher
{
    public class ClashResponse<T>
    {
        public bool Succeed { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors {  get; set; }
        public T? Result { get; set; } // here T is any class we want to put here
    }
}
