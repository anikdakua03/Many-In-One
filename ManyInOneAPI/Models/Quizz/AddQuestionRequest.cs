namespace ManyInOneAPI.Models.Quizz
{
    public class AddQuestionRequest
    {
        public required string QuestionText { get; set; } 
        public string? QuestionImageLink { get; set; }
        public string QuestionType { get; set; } = "ANY";
        public string QuestionLevel { get; set; } = "ANY";
        public List<string> QuestionTags { get; set; } = new List<string>() { };
        public required string CategoryName { get; set; }
        public List<OptionsReq> Options { get; set; } = new List<OptionsReq>() { };
    }
    public class OptionsReq
    {
        public required string OptionValue { get; set; }
        public string? AnswerExplanation { get; set; } 
        public required bool IsAnswer { get; set; }

    }
}
