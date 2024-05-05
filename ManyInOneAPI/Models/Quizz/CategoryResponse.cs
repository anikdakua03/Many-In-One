namespace ManyInOneAPI.Models.Quizz
{
    public class CategoryResponse
    {
        public List<IndividualCategory>? Trivia_categories { get; set; }
    }
    public class IndividualCategory
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
