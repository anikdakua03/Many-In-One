namespace ManyInOneAPI.Models.Quizz
{
    public class QuestionResponse
    {
        public Guid QuestionId { get; set; }
        public required string QuestionText { get; set; }
        public required string QuestionImageLink { get; set; }
        public string QuestionType { get; set; } = string.Empty;
        public string QuestionLevel { get; set; } = string.Empty;
        public List<string> QuestionTags { get; set; } = new List<string>();
        public required string  CategoryName{ get; set; }
        public List<OptionsWithAnswer> Options { get; set; } = new List<OptionsWithAnswer>();
    }
    public class OptionResponse
    {
        public Guid OptionId { get; set; }
        public string OptionValue { get; set; } = "";
    }
}
