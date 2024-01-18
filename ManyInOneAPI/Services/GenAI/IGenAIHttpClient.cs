using ManyInOneAPI.Models.GenAI;

namespace ManyInOneAPI.Services.GenAI
{
    public interface IGenAIHttpClient
    {
        //Text only input --> (Will use gemini-pro model)
        public Task<GenAIResponse> TextOnlyInput(TextOnly inputText);

        //Multi-turn conversations / chat --> (Will use gemini-pro model)

        //Streamed responses --> (Will use gemini-pro model)

        //Text and image input --> (Will use gemini-pro-vision model)
        public Task<GenAIResponse> TextAndImageAsInput(IFormFile formFile, string inputText); //

        #region Model info
        //Get model by model name
        public Task<GenAIModelInfo> GetModelByName(string modelName);
        //Get list of all models
        public Task<List<GenAIModelInfo>> GetAllModels();
        #endregion
    }
}
