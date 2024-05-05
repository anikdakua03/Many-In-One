using ManyInOneAPI.Infrastructure.Shared;
using ManyInOneAPI.Models.GenAI;

namespace ManyInOneAPI.Services.GenAI
{
    public interface IGenAIHttpClient
    {
        //Text only input --> (Will use gemini-pro model)
        public Task<Result<GenAIResponse>> TextOnlyInput(TextOnly inputText, CancellationToken cancellationToken = default);

        //Multi-turn conversations / chat --> (Will use gemini-pro model)
        public Task<Result<GenAIResponse>> MultiTurnConversation(Conversation inputText, CancellationToken cancellationToken = default);

        //Text and image input --> (Will use gemini-pro-vision model)
        public Task<Result<GenAIResponse>> TextAndImageAsInput(IFormFile formFile, string inputText, CancellationToken cancellationToken = default);

        public Task<Result<GenAIResponse>> TextSummarize(TextOnly longText, CancellationToken cancellationToken = default);

        public Task<Result<GenAIResponse>> TextToImage(TextOnly longText, CancellationToken cancellationToken = default);

        public Task<Result<GenAIResponse>> TextToSpeech(TextOnly longText, CancellationToken cancellationToken = default);

        #region Model info
        //Get model by model name
        //public Task<GenAIModelInfo> GetModelByName(string modelName);
        ////Get list of all models
        //public Task<List<GenAIModelInfo>> GetAllModels();
        #endregion
    }
}
