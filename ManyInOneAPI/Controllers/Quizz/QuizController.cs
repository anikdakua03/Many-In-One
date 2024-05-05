using ManyInOneAPI.Models.Quizz;
using ManyInOneAPI.Repositories.Quizz;
using ManyInOneAPI.Services.Quizz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManyInOneAPI.Controllers.Quizz
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly IQuizRepository _quizRepository;

        public QuizController(IQuizService quizService, IQuizRepository quizRepository)
        {
            _quizService = quizService;
            _quizRepository = quizRepository;
        }

        #region Custom quiz

        [HttpGet("GetAllQuestions")]
        public async Task<IActionResult> GetQuestions()
        {
            var res = await _quizRepository.GetQuestions();

            return Ok(res);
        }

        [HttpPost("AddQuestion")]
        public async Task<IActionResult> AddQuestion([FromBody] AddQuestionRequest qReq, CancellationToken cancellationToken)
        {
            var res = await _quizRepository.AddQuestion(qReq, cancellationToken);

            return Ok(res);
        }

        [HttpPost("AddQuestionsFromExcel")]
        [RequestSizeLimit(1_000_000)] // max 1 MB
        public async Task<IActionResult> AddQuestions(CancellationToken cancellationToken)
        {
            var res = await _quizRepository.AddQuestions(cancellationToken);

            return Ok(res);
        }

        [HttpPut("UpdateQuestion")]
        public async Task<IActionResult> UpdateQuestion([FromBody] UpdateQuestionRequest qReq, CancellationToken cancellationToken)
        {
            var res = await _quizRepository.UpdateQuestion(qReq, cancellationToken);

            return Ok(res);
        }

        [HttpDelete("RemoveQuestion/{questionId}")]
        public async Task<IActionResult> RemoveQuestion(string questionId, CancellationToken cancellationToken)
        {
            var res = await _quizRepository.RemoveQuestion(questionId, cancellationToken);

            return Ok(res);
        }


        [HttpGet("GetQuizCategories")]
        public IActionResult GetAllCustomCategory()
        {
            var res =  _quizRepository.GetCategories();

            return Ok(res);
        }

        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromBody]AddCategoryRequest catReq, CancellationToken cancellationToken)
        {
            var res = await _quizRepository.AddCategory(catReq, cancellationToken);

            return Ok(res);
        }

        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory([FromBody]UpdateCategoryName updateCategoryName, CancellationToken cancellationToken)
        {
            var res = await _quizRepository.UpdateCategory(updateCategoryName, cancellationToken);

            return Ok(res);
        }

        [HttpDelete("RemoveCategory/{categoryId}")]
        public async Task<IActionResult> RemoveCategory(string categoryId, CancellationToken cancellationToken)   
        {
            var res = await _quizRepository.RemoveCategory(categoryId, cancellationToken);

            return Ok(res);
        }


        [HttpPost("AddOption")]
        public async Task<IActionResult> AddOption([FromBody] AddOptionRequest opReq, CancellationToken cancellationToken)
        {
            var res = await _quizRepository.AddOption(opReq, cancellationToken);


            return Ok(res);
        }

        #region More granularity in options but not used
        [HttpPut("UpdateOption")]
        public async Task<IActionResult> UpdateOption([FromBody] UpdateOptionRequest updateOptionRequest, CancellationToken cancellationToken)
        {
            var res = await _quizRepository.UpdateOption(updateOptionRequest, cancellationToken);

            return Ok(res);
        }

        [HttpDelete("RemoveOption/{optionId}")]
        public async Task<IActionResult> RemoveOption(string optionId, CancellationToken cancellationToken)
        {
            var res = await _quizRepository.RemoveOption(optionId, cancellationToken);

            return Ok(res);
        }

        #endregion


        [HttpPost("QuizMaker")]
        public async Task<IActionResult> MakeQuiz(QuizMakerRequest quizMakerRequest, CancellationToken cancellationToken)
        {
            var res = await _quizRepository.MakeQuiz(quizMakerRequest, cancellationToken);

            return Ok(res);
        }

        [HttpPost("GetQuizResult")]
        public async Task<IActionResult> GetQuizScore(UserAnswerRequest userAnswerRequest, CancellationToken cancellationToken)
        {
            var res = await _quizRepository.GetQuizScore(userAnswerRequest, cancellationToken);

            return Ok(res);
        }

        #endregion


        #region external quiz api

        [HttpGet("GetQuizCategoriesFromAPI")]
        public async Task<IActionResult> GetAllCategory(CancellationToken cancellationToken)
        {
            var res = await _quizService.GetCategories(cancellationToken);

            return Ok(res);
        }

        [HttpPost("GetQuizQssFromAPI")]
        public async Task<IActionResult> GetQuizQss(APIQuizMakerRequest aPIQuizMakerRequest, CancellationToken cancellationToken)
        {
            var res = await _quizService.GetQuizQuestions(aPIQuizMakerRequest, cancellationToken);

            return Ok(res);
        }

        #endregion
    }
}
