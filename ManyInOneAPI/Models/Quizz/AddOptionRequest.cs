namespace ManyInOneAPI.Models.Quizz
{
    public class AddOptionRequest
    {
        public required string QuestionText { get; set; }
        public required string OptionValue { get; set; }
        public string? AnswerExplanation { get; set; }
        public bool IsAnswer { get; set; }
    }
}
