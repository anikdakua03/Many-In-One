namespace ManyInOneAPI.Models.Quizz
{
    public class APIQuizMakerRequest
    {
        public int QuestionCount { get; set; } = 10;
        public int CategoryId { get; set; }
        public string? QuestionType { get; set; }
        public string? QuestionLevel { get; set; }
    }
}
