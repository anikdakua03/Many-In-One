using ManyInOneAPI.Models.GenAI;
using ManyInOneAPI.Services.GenAI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManyInOneAPI.Controllers.GenAI
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenAIController : ControllerBase
    {
        private readonly IGenAIHttpClient _geAIHttpClient;

        public GenAIController(IGenAIHttpClient geAIHttpClient)
        {
            _geAIHttpClient = geAIHttpClient;
        }

        [HttpPost("TextOnly")]
        public async Task<ActionResult<GenAIResponse>> GetTextOnlyInput([Required] TextOnly input)
        {
            var res = await _geAIHttpClient.TextOnlyInput(input);

            //if(res)
            return Ok(res);
        }

        [HttpPost("TextAndImage"), DisableRequestSizeLimit]
        //[Consumes("multipart/form-data")]
        public async Task<ActionResult<GenAIResponse>> TextAndImageAsInput() // [FromForm] IFormFile formFile, string inputText
        {
            try
            {
                // get from request
                var file = Request.Form.Files[0];
                string text = Request.Form["textInput"]!; // Access text input from FormData

                var res = await _geAIHttpClient.TextAndImageAsInput(file, text);
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
            return await _geAIHttpClient.GetModelByName(modelName);
        }

        //Get list of all models
        [HttpGet]
        public async Task<List<GenAIModelInfo>> GetAllModels()
        {
            return await _geAIHttpClient.GetAllModels();
        }

        #endregion
    }
}
