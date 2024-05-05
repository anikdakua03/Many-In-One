namespace ManyInOneAPI.Models.Quizz
{
    public class UpdateCategoryName
    {
        public required string CategoryName { get; set; }
        public required string NewCategoryName { get; set; }
        public string Description { get; set; } = "";
    }
}
