namespace ManyInOneAPI.Models.Quizz
{
    public class UserAnswerRequest
    {
        public required List<UserAnswer> UserQsAnswers { get; set; } = new List<UserAnswer>();
    }

    public class UserAnswer
    {
        public required string QuestionId { get; set; }
        public List<string> SelectedAnswer { get; set; } = new List<string>();
    }
}
