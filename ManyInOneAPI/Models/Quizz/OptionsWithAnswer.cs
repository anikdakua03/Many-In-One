namespace ManyInOneAPI.Models.Quizz
{
    public class OptionsWithAnswer
    {
        public Guid OptionId { get; set; }
        public string OptionValue { get; set; } = "";
        
        public string? AnswerExplanation { get; set; }
        public bool IsAnswer { get; set; }

        // Qs will have a Question Id associated
        public Guid QuestionId { get; set; }
    }
}
