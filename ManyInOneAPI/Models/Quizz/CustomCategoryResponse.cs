namespace ManyInOneAPI.Models.Quizz
{
    public class CustomCategoryResponse
    {
        public List<SingleCategory>? Categories { get; set; }
    }
    public class SingleCategory
    {
        public Guid CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public string Description { get; set; } = "";
        public int QuestionCount { get; set; }
    }
}
