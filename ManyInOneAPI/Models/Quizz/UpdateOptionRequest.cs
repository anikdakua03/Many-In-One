namespace ManyInOneAPI.Models.Quizz
{
    public class UpdateOptionRequest
    {
        public required string OptionValue { get; set; }
        public required string NewOptionValue { get; set; }
        public string? AnswerExplanation { get; set; }
        public bool IsAnswer { get; set; }
    }
}
