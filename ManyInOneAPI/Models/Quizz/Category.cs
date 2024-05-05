namespace ManyInOneAPI.Models.Quizz
{
    public class Category
    {
        public const int CategoryNameMaxLength = 50;
        public const int DescriptionMaxLength = 250;
        public Guid CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public string Description { get; set; } = "";
        public Guid UserId { get; set; }// will not be empty , have user id

        // will have list of questions
        public List<Question>? Questions { get; set; }
    }
}
