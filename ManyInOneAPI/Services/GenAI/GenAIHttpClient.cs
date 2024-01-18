using ManyInOneAPI.Configurations;
using ManyInOneAPI.Models.GenAI;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ManyInOneAPI.Services.GenAI
{
    public class GenAIHttpClient : IGenAIHttpClient
    {
        private readonly GenAIConfig _genAiConfig;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly HttpClient _httpClient;
        public GenAIHttpClient(HttpClient httpClient, IWebHostEnvironment webHostEnvironment, IOptionsMonitor<GenAIConfig> optionsMonitor)
        {
            _httpClient = httpClient;
            _webHostEnvironment = webHostEnvironment;
            _genAiConfig = optionsMonitor.CurrentValue;
        }

        //Generate text from text-only input
        public async Task<GenAIResponse> TextOnlyInput(TextOnly inputText)
        {
            // Create JSON request content
            // while sending we can do some custimization on getting response
            // Configuration --> https://ai.google.dev/tutorials/rest_quickstart#configuration
            var requestContent = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = inputText.InputText
                            }
                        }
                    }
                }
            };

            var requestJson = JsonSerializer.Serialize(requestContent);

            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Send POST request with error handling
            try
            {
                // need :
                // https://generativelanguage.googleapis.com/v1/models/gemini-pro:generateContent?key=$API_KEY \
                var address = $"{_genAiConfig.GenAIBaseUrl}:generateContent?key={_genAiConfig.API_KEY}";

                var response = await _httpClient.PostAsync(address, new StringContent(requestJson, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var responseText = ExtractTextFromStringResponse(responseString);

                    return new GenAIResponse() { ResponseMessage = responseText, Succeed = true };
                }
                else
                {
                    return new GenAIResponse() { ErrorMessage = $"Failed with status code :--> {response.StatusCode}", Succeed = false };
                }
            }
            catch (Exception ex)
            {
                return new GenAIResponse() { ErrorMessage = $"Failed with error message :--> {ex.Message}", Succeed = false };
            }
        }

        //Generate text from text-and-image input(multimodal)
        public async Task<GenAIResponse> TextAndImageAsInput(IFormFile formFile, string inputText)
        {
            // Create JSON request content
            // while sending we can do some custimization on getting response
            // Configuration --> https://ai.google.dev/tutorials/rest_quickstart#configuration

            // first convert IFileForm to memory stream then to byte array

            // Read image file as byte array
            using var imageStream = formFile.OpenReadStream();
            var imageBytes = new byte[imageStream.Length];
            await imageStream.ReadAsync(imageBytes);

            var imageBase64 = Convert.ToBase64String(imageBytes);

            // we need to create it as dynamic because it cannot be compiled in compile time , this custom made object
            dynamic requestContent = new
            {
                contents = new dynamic[]
                {
                    new
                    {
                        parts = new dynamic[]
                        {
                            new { text = inputText },
                            new
                            {
                                inlineData = new
                                {
                                   mimeType = "image/jpeg" , // Adjust for other image types
                                   data = imageBase64
                                }
                            }
                        }
                    }
                }
            };

            var requestJson = JsonSerializer.Serialize(requestContent);

            // Set up address and headers
            string uri = $"{_genAiConfig.ProVisionUrl}key={_genAiConfig.API_KEY}";
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Send POST request with error handling
            try
            {
                var response = await _httpClient.PostAsync(uri, new StringContent(requestJson, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var responseText = ExtractTextFromStringResponse(responseJson);

                    return new GenAIResponse() { ResponseMessage = responseText, Succeed = true };
                }
                else
                {
                    return new GenAIResponse() { ErrorMessage = $"Failed with status code :--> {response.RequestMessage}", Succeed = false };
                }
            }
            catch (Exception ex)
            {
                return new GenAIResponse() { ErrorMessage = $"Failed with error message :--> {ex.Message}", Succeed = false };
            }
        }

        //Build multi-turn conversations(chat)

        #region Model info and other utilities
        //Get model by model name
        public async Task<GenAIModelInfo> GetModelByName(string modelName)
        {
            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();

            // Send GET request with error handling
            try
            {
                // need :
                // https://generativelanguage.googleapis.com/v1/models/gemini-pro?key=$API_KEY
                var address = $"https://generativelanguage.googleapis.com/v1/models/{modelName}?key={_genAiConfig.API_KEY})";

                var response = await _httpClient.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseStringJSON = await response.Content.ReadFromJsonAsync<GenAIModelInfo>();
                    //var responseText = ExtractTextFromStringResponse(responseString);

                    return new GenAIModelInfo() { };
                }
                else
                {
                    return new GenAIModelInfo() { };
                }
            }
            catch (Exception)
            {
                return new GenAIModelInfo() { };
            }
        }

        //Get list of all models
        public Task<List<GenAIModelInfo>> GetAllModels()
        {
            throw new NotImplementedException();
        }

        // extract informatiom from the response
        private string ExtractTextFromStringResponse(string responseString)
        {
            var  jsonData = JsonSerializer.Deserialize<TextOnlyResponse>(responseString)!;
            //string extractedText = jsonData.candidates[0].content.parts[0].text;

            return jsonData.candidates[0].content.parts[0].text!;
        }

        #endregion
    }
}
