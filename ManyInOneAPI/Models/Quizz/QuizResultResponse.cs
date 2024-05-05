namespace ManyInOneAPI.Models.Quizz
{
    public class QuizResultResponse
    {
        public int TotalQs { get; set; } = 0;
        public int TotalCorrect { get; set; } = 0;
        public int TotalScore { get; set; } = 0;
        public int TotalTime { get; set; } = 0;
        public double Percentage { get; set; } = 0.0;
        public bool HasPassed { get; set;}
        public List<CorrectAnswer> Results { get; set; } = new List<CorrectAnswer>();
    }

    public class CorrectAnswer
    {
        public string QuestionId { get; set; } = "";
        public List<string> AnswerExplanation { get; set; } = new List<string>();
    }
}
