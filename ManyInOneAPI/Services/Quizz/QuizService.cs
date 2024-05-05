using ManyInOneAPI.Constants;
using ManyInOneAPI.Infrastructure.Encryption;
using ManyInOneAPI.Infrastructure.Shared;
using ManyInOneAPI.Models.Quizz;
using ManyInOneAPI.Repositories.Quizz;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace ManyInOneAPI.Services.Quizz
{
    public class QuizService : IQuizService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;

        public QuizService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<CategoryResponse>> GetCategories(CancellationToken cancellationToken)
        {
            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var address = $"{AppConstant.QuizCategoryApiUrl}";

            var response = await _httpClient.GetAsync(address, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseString1 = await response.Content.ReadFromJsonAsync<CategoryResponse>();

                return Result<CategoryResponse>.Success(responseString1!);
            }
            else
            {
                return Result<CategoryResponse>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }
        }

        public async Task<Result<List<ClientQuizResponse>>> GetQuizQuestions(APIQuizMakerRequest apiQuizMakerRequest, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue("Id");

            // or also can check from cookies currect user

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result<List<ClientQuizResponse>>.Failure(Error.NotFound("Not Found", "User doesn't exists or Invalid user to create a quiz."));
            }

            if (apiQuizMakerRequest.QuestionCount > 50)
            { 
                return Result<List<ClientQuizResponse>>.Failure(Error.Validation("Validation", "Cannot proceed more than 50 questions.")); 
            }
            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var address = GetQuizQuestionURL(apiQuizMakerRequest);

            var response = await _httpClient.GetAsync(address, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadFromJsonAsync<APIQuizQuestionResponse>();

                // prepare for client response
                var final = APIResToClientRes(responseJson!);

                return Result<List<ClientQuizResponse>>.Success(final);
            }
            else
            {
                return Result<List<ClientQuizResponse>>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }
        }

        private string GetQuizQuestionURL(APIQuizMakerRequest body)
        {
            var baseAdd = AppConstant.QuizApiUrl;
            var address = baseAdd;

            var (qsNum, categoryId,qsLvl, qsType) = (body.QuestionCount, body.CategoryId, body.QuestionLevel, body.QuestionType);

            address += $"amount={qsNum}";

            address += $"&category={categoryId}";

            if(!string.IsNullOrWhiteSpace(qsLvl) && qsLvl != "any") 
            {
                address += $"&difficulty={qsLvl}";
            }

            if (!string.IsNullOrWhiteSpace(qsType) && qsType != "any")
            {
                address += $"&type={qsType}";
            }

            return address;
        }

        private List<ClientQuizResponse> APIResToClientRes(APIQuizQuestionResponse body)
        {
            var result = new List<ClientQuizResponse>();

            for (int i = 0; i < body.Results!.Count; i++)
            {
                var aqr = body.Results[i];

                var ops = new List<OptionResponse>() { new OptionResponse() { OptionId = Guid.Empty, OptionValue = aqr.correct_answer! } };

                // add incorrect also
                var incorrects = aqr.Incorrect_answers!;

                for (int j = 0; j < incorrects.Count; j++)
                {
                    ops.Add(new OptionResponse()
                    {
                        OptionId = Guid.Empty,
                        OptionValue = incorrects[j]
                    });
                }

                var cqr = new ClientQuizResponse()
                {
                    QuestionId = Guid.NewGuid(),
                    QuestionText = aqr.question!,
                    QuestionImageLink = "NA",
                    QuestionType = aqr.type!,
                    QuestionLevel = aqr.difficulty!,
                    QuestionTags = [],
                    CategoryName = aqr.category!,
                    Options = ops,
                    Answer = AESOperation.Encrypt(aqr.correct_answer!)
                };

                result.Add(cqr);
            }

            return result;
        }
    }
}
