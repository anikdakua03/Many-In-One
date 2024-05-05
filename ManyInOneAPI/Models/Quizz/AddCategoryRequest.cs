namespace ManyInOneAPI.Models.Quizz
{
    public class AddCategoryRequest
    {
        public required string CategoryName { get; set; }
        public string Description { get; set; } = "";
    }
}
