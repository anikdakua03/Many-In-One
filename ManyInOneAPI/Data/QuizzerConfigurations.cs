using ManyInOneAPI.Models.Quizz;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManyInOneAPI.Data
{
    public class QuizzerConfigurations :
        IEntityTypeConfiguration<Question>,
        IEntityTypeConfiguration<OptionsWithAnswer>,
        IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(a => a.CategoryId);

            builder.Property(a => a.CategoryId).HasColumnName("Category Id");
            builder.Property(a => a.CategoryName)
                   .IsRequired()
                   .HasMaxLength(Category.CategoryNameMaxLength)
                   .HasColumnName("Category Name")
                   .HasDefaultValue("User Category");
            builder.Property(a => a.Description)
                  .HasMaxLength(Category.DescriptionMaxLength)
                  .HasColumnName("Description")
                  .HasDefaultValue("No description.");
            builder.Property(a => a.UserId)
                   .IsRequired()
                   .HasColumnName("User Id");

            builder.HasMany(a => a.Questions)
                   .WithOne()
                   .HasForeignKey(c => c.CategoryId)
                   .HasConstraintName("Category_Qs Id")
                   .OnDelete(DeleteBehavior.Cascade);
        }

        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("Questions");

            builder.HasKey(a => a.QuestionId);

            builder.Property(a => a.QuestionId)
                   .IsRequired()
                   .HasColumnName("Question Id");
            builder.Property(a => a.QuestionText)
                   .IsRequired()
                   .HasColumnName("Question");
            builder.Property(a => a.QuestionImageLink)
                   .HasColumnName("Question Image Link");
            builder.Property(a => a.QuestionType)
                   .IsRequired().HasColumnName("Question Type")
                   .HasConversion(b => b.ToString(),
                   c => Enum.Parse<QuestionType>(c))
                   .HasDefaultValue(QuestionType.ANY); // enum to corresponding string value
            builder.Property(a => a.QuestionLevel)
                   .IsRequired()
                   .HasColumnName("Question Level")
                   .HasConversion(b => b.ToString(),
                  c => Enum.Parse<QuestionLevel>(c))
                   .HasDefaultValue(QuestionLevel.ANY); // enum to corresponding string value
            builder.Property(a => a.QuestionTags)
                   .HasColumnName("Question Tags")
                   .HasConversion(
                // when inserting will do this joiing with comma
                (b => string.Join(',', b)), 
                // while returning will split and give a list of strings back
                (b => b.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()));
            builder.Property(a => a.CategoryId)
                   .IsRequired()
                   .HasColumnName("Category Id");

            builder.HasMany(a => a.Options)
                   .WithOne()
                   .HasForeignKey(c => c.QuestionId)
                   .HasConstraintName("Qs_Option Id")
                   .OnDelete(DeleteBehavior.Cascade);
        }

        public void Configure(EntityTypeBuilder<OptionsWithAnswer> builder)
        {
            builder.ToTable("Options");

            builder.HasKey(a => a.OptionId);
            builder.Property(a => a.OptionId)
                   .HasColumnName("Option Id");
            builder.Property(a => a.OptionValue)
                   .IsRequired().HasColumnName("Option Value");
            builder.Property(a => a.AnswerExplanation)
                   .HasColumnName("Answer Explanation")
                   .HasDefaultValue("No explanation provided.");
            builder.Property(a => a.IsAnswer)
                   .HasColumnName("Correct");
        }
    }
}
