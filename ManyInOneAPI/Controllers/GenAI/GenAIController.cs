using ManyInOneAPI.Models.GenAI;
using ManyInOneAPI.Services.GenAI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManyInOneAPI.Controllers.GenAI
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GenAIController : ControllerBase
    {
        private readonly IGenAIHttpClient _genAIHttpClient;

        public GenAIController(IGenAIHttpClient genAIHttpClient)
        {
            _genAIHttpClient = genAIHttpClient;
        }

        [HttpPost("TextOnly")]
        public async Task<IActionResult> GetTextOnlyInput([Required] TextOnly input)
        {
            try
            {
                var res = await _genAIHttpClient.TextOnlyInput(input);

                //if(res)
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("TextAndImage"), DisableRequestSizeLimit]
        //[Consumes("multipart/form-data")]
        public async Task<IActionResult> TextAndImageAsInput() // [FromForm] IFormFile formFile, string inputText
        {
            try
            {
                // get from request
                var file = Request.Form.Files[0];
                string text = Request.Form["textInput"]!; // Access text input from FormData

                var res = await _genAIHttpClient.TextAndImageAsInput(file, text);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest($"Message :--> {ex.Message}");
            }
        }


        #region Model information but not getting , getting Bad request
        //Get model by model name
        [HttpGet("GetModelByName")]
        public async Task<GenAIModelInfo> GetModelByName(string modelName)
        {
            return await _genAIHttpClient.GetModelByName(modelName);
        }

        //Get list of all models
        [HttpGet]
        public async Task<List<GenAIModelInfo>> GetAllModels()
        {
            return await _genAIHttpClient.GetAllModels();
        }

        #endregion
    }
}
