namespace ManyInOneAPI.Models.Quizz
{
    public class ClientQuizResponse
    {
        public Guid QuestionId { get; set; }
        public required string QuestionText { get; set; }
        public string? QuestionImageLink { get; set; }
        public required string QuestionType { get; set; }
        public required string QuestionLevel { get; set; }
        public List<string>? QuestionTags { get; set; }
        public string? CategoryName { get; set; }
        public required List<OptionResponse> Options { get; set; }
        public required string Answer { get; set; }
    }
}
