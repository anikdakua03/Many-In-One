namespace ManyInOneAPI.Models.Quizz
{
    public class QuizMakerRequest
    {
        public required int QuestionCount { get; set; } = 5;
        public required string CategoryId{ get; set; }
        public required string QuestionType { get; set; }
        public required string QuestionLevel{ get; set; }
    }
}
