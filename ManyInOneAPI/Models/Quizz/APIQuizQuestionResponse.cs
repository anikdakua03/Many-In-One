namespace ManyInOneAPI.Models.Quizz
{
    public class APIQuizQuestionResponse
    {
        public int Response_code { get; set; }
        public List<QuizQsResult>? Results { get; set; }

    }
    public class QuizQsResult
    {
        public string? type { get; set; }
        public string? difficulty { get; set; }
        public string? category { get; set; }
        public string? question { get; set; }
        public string? correct_answer { get; set; }
        public List<string>? Incorrect_answers { get; set; }
    }
}
