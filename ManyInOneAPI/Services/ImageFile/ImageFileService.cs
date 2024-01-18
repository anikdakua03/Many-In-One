namespace ManyInOneAPI.Services.ImageFile
{
    public class ImageFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageFileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        protected string GetFilePaths(string imageCode)
        {
            return _webHostEnvironment.WebRootPath + "\\Gemini\\Upload\\" + imageCode;
        }
    }
}
