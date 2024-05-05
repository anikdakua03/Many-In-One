namespace ManyInOneAPI.Models.Quizz
{
    public class UpdateQuestionRequest
    {
        public required string QuestionId { get; set; }
        public required string QuestionText { get; set; } 
        public string? QuestionImageLink { get; set; }
        public string QuestionType { get; set; } = "ANY";
        public string QuestionLevel { get; set; } = "ANY";
        public List<string> QuestionTags { get; set; } = new List<string>() { };
        public required string CategoryName { get; set; }
        public List<OptionsReq> Options { get; set; } = new List<OptionsReq>() { };
    }
}
