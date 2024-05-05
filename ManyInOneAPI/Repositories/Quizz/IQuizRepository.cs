using ManyInOneAPI.Infrastructure.Shared;
using ManyInOneAPI.Models.Quizz;

namespace ManyInOneAPI.Repositories.Quizz
{
    public interface IQuizRepository
    {
        public Task<Result<List<QuestionResponse>>> GetQuestions();
        public Task<Result<string>> AddQuestion(AddQuestionRequest qReq, CancellationToken cancellationToken);
        public Task<Result<string>> AddQuestions(CancellationToken cancellationToken);
        public Task<Result<string>> UpdateQuestion(UpdateQuestionRequest qReq, CancellationToken cancellationToken);
        public Task<Result<string>> RemoveQuestion(string questionId, CancellationToken cancellationToken);


        public Result<CustomCategoryResponse> GetCategories();
        public Task<Result<string>> AddCategory(AddCategoryRequest catReq, CancellationToken cancellationToken);
        public Task<Result<string>> UpdateCategory(UpdateCategoryName updateCategoryName, CancellationToken cancellationToken);
        public Task<Result<string>> RemoveCategory(string categoryId, CancellationToken cancellationToken);


        public Task<Result<string>> AddOption(AddOptionRequest addOpReq, CancellationToken cancellationToken);
        public Task<Result<string>> UpdateOption(UpdateOptionRequest updateOpReq, CancellationToken cancellationToken);
        public Task<Result<string>> RemoveOption(string optionId, CancellationToken cancellationToken);

        
        public Task<Result<List<ClientCustomQuizResponse>>> MakeQuiz(QuizMakerRequest quizMakerRequest, CancellationToken cancellationToken);
        public Task<Result<QuizResultResponse>> GetQuizScore(UserAnswerRequest userAnswerRequest, CancellationToken cancellationToken);
    }
}
