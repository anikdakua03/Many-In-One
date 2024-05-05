namespace ManyInOneAPI.Models.Quizz
{
    public class Question
    {
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; } = "";
        public string? QuestionImageLink{ get; set; }
        public QuestionLevel QuestionLevel { get; set; }
        public QuestionType QuestionType { get; set; }
        public List<string> QuestionTags { get; set; } = new List<string>() { };

        // Qs will have a category Id associated with it for ref not any relation
        public Guid CategoryId {  get; set; }

        // Qs will have a Multiple options associated
        public List<OptionsWithAnswer>  Options { get; set; } = new List<OptionsWithAnswer>() { };
    }

    public enum QuestionType
    {
        ANY = 0,
        MULTIPLE = 1, // MCQ
        MULTIPLE_CORRECT = 2,
        BOOLEAN = 3 // True / False
    }
    public enum QuestionLevel
    {
        ANY = 0,
        EASY = 1,
        MEDIUM = 2,
        HARD = 3
    }
}
