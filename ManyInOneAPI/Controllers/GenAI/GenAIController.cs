using ManyInOneAPI.Infrastructure.Shared;
using ManyInOneAPI.Models.GenAI;
using ManyInOneAPI.Services.GenAI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.ComponentModel.DataAnnotations;

namespace ManyInOneAPI.Controllers.GenAI
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("bucket")]
    public class GenAIController : ControllerBase
    {
        private readonly IGenAIHttpClient _genAIHttpClient;

        public GenAIController(IGenAIHttpClient genAIHttpClient)
        {
            _genAIHttpClient = genAIHttpClient;
        }

        [HttpPost("TextOnly")]
        [EnableRateLimiting("sliding")]
        public async Task<IActionResult> GetTextOnlyInput([Required] TextOnly input)
        {
            Result<GenAIResponse>? res = await _genAIHttpClient.TextOnlyInput(input, HttpContext.RequestAborted);
            if (res.IsSuccess)
            {
                return Ok(res);
            }
            else
            {
                return Ok(res);
            }
        }

        [HttpPost("TextAndImage"), DisableRequestSizeLimit]
        //[Consumes("multipart/form-data")]
        public async Task<IActionResult> TextAndImageAsInput() // [FromForm] IFormFile formFile, string inputText
        {
            // get from request
            var file = Request.Form.Files[0];
            string text = Request.Form["textInput"]!; // Access text input from FormData

            var res = await _genAIHttpClient.TextAndImageAsInput(file, text, HttpContext.RequestAborted);
            return Ok(res);
        }

        [HttpPost("MultiConversation")]
        public async Task<IActionResult> MultiConversation([Required] Conversation input)
        {
            var res = await _genAIHttpClient.MultiTurnConversation(input, HttpContext.RequestAborted);

            return Ok(res);
        }

        [HttpPost("TextSummarize")]
        public async Task<IActionResult> SummarizeText([Required] TextOnly longText)
        {
            var res = await _genAIHttpClient.TextSummarize(longText, HttpContext.RequestAborted);

            return Ok(res);
        }

        [HttpPost("TextToImage")]
        public async Task<IActionResult> GenerateImage([Required] TextOnly longText)
        {
            var res = await _genAIHttpClient.TextToImage(longText, HttpContext.RequestAborted);

            return Ok(res);
        }

        [HttpPost("TextToSpeech")]
        public async Task<IActionResult> GenerateSpeech([Required] TextOnly longText)
        {
            var res = await _genAIHttpClient.TextToSpeech(longText, HttpContext.RequestAborted);

            return Ok(res);
        }
    }
}
