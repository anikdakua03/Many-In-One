using Azure;
using ManyInOneAPI.Configurations;
using ManyInOneAPI.Infrastructure.Shared;
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
        public async Task<Result<GenAIResponse>> TextOnlyInput(TextOnly inputText, CancellationToken cancellationToken)
        {
            // Create JSON request content
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
            var address = $"{_genAiConfig.GenAIBaseUrl}:generateContent?key={_genAiConfig.API_KEY}";

            var response = await _httpClient.PostAsync(address, new StringContent(requestJson, Encoding.UTF8, "application/json"), cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var responseText = ExtractTextFromStringResponse(responseString);

                return Result<GenAIResponse>.Success(new GenAIResponse() { ResponseMessage = responseText, Succeed = true } );
            }
            else
            {
                return Result<GenAIResponse>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }
        }

        //Generate text from text-and-image input(multimodal)
        public async Task<Result<GenAIResponse>> TextAndImageAsInput(IFormFile formFile, string inputText, CancellationToken cancellationToken = default)
        {
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
            var response = await _httpClient.PostAsync(uri, new StringContent(requestJson, Encoding.UTF8, "application/json"), cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseText = ExtractTextFromStringResponse(responseJson);

                return Result<GenAIResponse>.Success(new GenAIResponse() { ResponseMessage = responseText, Succeed = true });
            }
            else
            {
                return Result<GenAIResponse>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }
        }

        //Build multi-turn conversations(chat)
        public async Task<Result<GenAIResponse>> MultiTurnConversation(Conversation allChats, CancellationToken cancellationToken = default)
        {
            var requestJson = JsonSerializer.Serialize(allChats);

            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var address = $"{_genAiConfig.GenAIBaseUrl}:generateContent?key={_genAiConfig.API_KEY}";

            var response = await _httpClient.PostAsync(address, new StringContent(requestJson, Encoding.UTF8, "application/json"), cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var responseText = ExtractTextFromStringResponse(responseString);

                return Result<GenAIResponse>.Success(new GenAIResponse() { ResponseMessage = responseText, Succeed = true });
            }
            else
            {
                return Result<GenAIResponse>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }
        }

        // Text summarization
        public async Task<Result<GenAIResponse>> TextSummarize(TextOnly longText, CancellationToken cancellationToken = default)
        {
            // will allow only 500 long words text
            var isVeryLong = CheckForVeryLongText(longText.InputText);

            if(isVeryLong) 
            {
                return Result<GenAIResponse>.Failure(Error.Validation("Validation", "Currently more than 500 words are not allowed to summarize. "));
            }

            var requestJson = JsonSerializer.Serialize(longText.InputText);

            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _genAiConfig.HF_KEY);

            var response = await _httpClient.PostAsync("https://api-inference.huggingface.co/models/facebook/bart-large-cnn", new StringContent(requestJson, Encoding.UTF8, "application/json"), cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var responseObj = JsonSerializer.Deserialize<Dictionary<string, string>[]>(responseString);

                var text = responseObj![0]["summary_text"];
                return Result<GenAIResponse>.Success(new GenAIResponse() { ResponseMessage = text, Succeed = true });
            }
            else
            {
                return Result<GenAIResponse>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }
        }

        // Text to Image generation
        public async Task<Result<GenAIResponse>> TextToImage(TextOnly longText, CancellationToken cancellationToken = default)
        {
            var requestJson = JsonSerializer.Serialize(longText.InputText);

            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _genAiConfig.HF_KEY);

            string[] links = { "https://api-inference.huggingface.co/models/stabilityai/stable-diffusion-xl-base-1.0", "https://api-inference.huggingface.co/models/runwayml/stable-diffusion-v1-5" };
            Random rand = new Random();
            // Generate a random index less than the size of the array.
            int index = rand.Next(links.Length);

            var response = await _httpClient.PostAsync(links[index], new StringContent(requestJson, Encoding.UTF8, "application/json"), cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                //response content-type : content/jpeg
                var responseByteArr = await response.Content.ReadAsByteArrayAsync();
                //Convert byte arry to base64string
                string imageBase64Data = Convert.ToBase64String(responseByteArr);
                string imgDataURL = string.Format("data:image/jpeg;base64,{0}", imageBase64Data);

                return Result<GenAIResponse>.Success(new GenAIResponse() { ResponseMessage = imgDataURL, Succeed = true });
            }
            else
            {
                return Result<GenAIResponse>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }
        }

        public async Task<Result<GenAIResponse>> TextToSpeech(TextOnly longText, CancellationToken cancellationToken = default)
        {
            var requestJson = JsonSerializer.Serialize(longText.InputText);

            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _genAiConfig.HF_KEY);

            string address = "https://api-inference.huggingface.co/models/facebook/mms-tts-eng";

            var response = await _httpClient.PostAsync(address, new StringContent(requestJson, Encoding.UTF8, "application/json"), cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                //response content-type : content/jpeg
                var responseByteArr = await response.Content.ReadAsByteArrayAsync();
                //Convert byte arry to base64string
                string audioBase64Data = Convert.ToBase64String(responseByteArr);
                string audDataURL = string.Format("data:audio/ogg;base64,{0}", audioBase64Data);

                return Result<GenAIResponse>.Success(new GenAIResponse() { ResponseMessage = audDataURL, Succeed = true });
            }
            else
            {
                return Result<GenAIResponse>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }
        }

        #region Model info and other utilities
        //Get model by model name
        //public async Task<GenAIModelInfo> GetModelByName(string modelName)
        //{
        //    // Set request headers
        //    _httpClient.DefaultRequestHeaders.Accept.Clear();

        //    // Send GET request with error handling

        //        var address = $"https://generativelanguage.googleapis.com/v1/models/{modelName}?key={_genAiConfig.API_KEY})";

        //        var response = await _httpClient.GetAsync(address);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var responseString = await response.Content.ReadAsStringAsync();
        //            var responseStringJSON = await response.Content.ReadFromJsonAsync<GenAIModelInfo>();
        //            //var responseText = ExtractTextFromStringResponse(responseString);

        //            return new GenAIModelInfo() { };
        //        }
        //        else
        //        {
        //            return new GenAIModelInfo() { };
        //        }
        //}

        //Get list of all models
        //public Task<List<GenAIModelInfo>> GetAllModels()
        //{
        //    throw new NotImplementedException();
        //}

        // extract informatiom from the response
        private string ExtractTextFromStringResponse(string responseString)
        {
            var jsonData = JsonSerializer.Deserialize<GeminiResponse>(responseString)!;
            //string extractedText = jsonData.candidates[0].content.parts[0].text;

            return jsonData!.candidates![0].content!.parts![0].text!;
        }

        private bool CheckForVeryLongText(string inputText)
        {
            var wordCount = inputText.Split(" ").Length;
            return wordCount > 500 ? true : false;
        }
        #endregion
    }
}
