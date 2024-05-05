using ManyInOneAPI.Infrastructure.Shared;
using ManyInOneAPI.Models.Quizz;

namespace ManyInOneAPI.Services.Quizz
{
    public interface IQuizService
    {
        public Task<Result<CategoryResponse>> GetCategories(CancellationToken cancellationToken = default);
        public Task<Result<List<ClientQuizResponse>>> GetQuizQuestions(APIQuizMakerRequest apiQuizMakerRequest, CancellationToken cancellationToken = default);
    }
}
