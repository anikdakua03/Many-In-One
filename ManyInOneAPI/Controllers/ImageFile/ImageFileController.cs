using ManyInOneAPI.Data;
using ManyInOneAPI.Models.ImageFile;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManyInOneAPI.Controllers.ImageFile
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageFileController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ManyInOneDbContext _dbContext;

        public ImageFileController(IWebHostEnvironment webHostEnvironment, ManyInOneDbContext dbContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _dbContext = dbContext;
        }

        [HttpPut("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile formFile, string imageCode)
        {
            ImageResponse response = new ImageResponse();
            try
            {
                string filePath = GetFilePath(imageCode);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string imagePath = filePath + "\\" + imageCode + ".png";
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);   // removing older file
                }
                FileStream stream = System.IO.File.Create(imagePath);
                await formFile.CopyToAsync(stream);

                response.Code = "200";
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpPut("MultiUploadImage")]
        public async Task<IActionResult> MultiUploadImage(IFormFileCollection fileCollection, string imageCode)
        {
            ImageResponse response = new ImageResponse();
            try
            {
                string filePath = GetFilePath(imageCode);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                foreach (var file in fileCollection)
                {
                    string imagePath = filePath + "\\" + file.FileName;
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);   // removing older file
                    }
                    FileStream stream = System.IO.File.Create(imagePath);
                    await file.CopyToAsync(stream);
                }

                response.Code = "200";
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("GetImage")]
        public string GetImage(string imageCode)
        {
            string ImageUrl = string.Empty;
            string hostUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            try
            {
                string filePath = GetFilePath(imageCode);
                string imagePath = filePath + "\\" + imageCode + ".png";

                if (System.IO.File.Exists(imagePath))
                {
                    ImageUrl = "Image viewing link :-> " + hostUrl + "/Gemini/Upload/" + imageCode + ".png";
                }
                else
                {
                    return ("Image doest not exists !!");
                }
            }
            catch (Exception)
            {

                throw;
            }
            return ImageUrl;
        }

        [HttpGet("GetAllImages")]
        public List<string> GetAllImages(string imageCode)
        {
            List<string> imagesList = new List<string>();

            string hostUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            try
            {
                string filePath = GetFilePath(imageCode);

                if (Directory.Exists(filePath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        string fileName = fileInfo.Name;
                        string imagePath = filePath + "\\" + fileName;
                        if (System.IO.File.Exists(imagePath))
                        {
                            string ImageUrl = "Image viewing link :-> " + hostUrl + "/Gemini/Upload/" + imageCode + "/" + imagePath;

                            imagesList.Add(ImageUrl);
                        }
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
            return imagesList;
        }


        [HttpGet("DownloadImage")]
        public async Task<IActionResult> DownloadImage(string imageCode)
        {
            try
            {
                string filePath = GetFilePath(imageCode);
                string imagePath = filePath + "\\" + imageCode + ".png";

                if (System.IO.File.Exists(imagePath))
                {
                    //if exissts , convert it to memory stream
                    MemoryStream stream = new MemoryStream();
                    FileStream fileStream = new FileStream(imagePath, FileMode.Open);
                    await fileStream.CopyToAsync(stream);
                    stream.Position = 0;

                    return File(stream, "image/png", imageCode + ".png");
                }
                else
                {
                    return NotFound("Image doest not exists !!");
                }
            }
            catch (Exception)
            {
                return NotFound("Image doest not exists !!");
            }
        }

        [HttpDelete("RemoveImage")]
        public string RemoveImageImage(string imageCode)
        {
            try
            {
                string filePath = GetFilePath(imageCode);
                string imagePath = filePath + "\\" + imageCode + ".png";

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                    return ($"{imageCode} deleted successfully !");
                }
                else
                {
                    return ("Image doest not exists !!");
                }
            }
            catch (Exception)
            {
                return ("Image doest not exists !!");
            }
        }

        [HttpDelete("RemoveAllImages")]
        public string RemoveAllImages(string imageCode)
        {
            try
            {
                string filePath = GetFilePath(imageCode);

                if (Directory.Exists(filePath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        fileInfo.Delete();
                    }
                }

            }
            catch (Exception ex)
            {
                return ($"Image doest not exists !! and {ex.Message}");
            }
            return ("Removed successfully !!");
        }

        [HttpPut("DBMultiUploadImage")]
        public async Task<IActionResult> DBMultiUploadImage(IFormFileCollection fileCollection, string imageCode)
        {
            ImageResponse response = new ImageResponse();
            try
            {
                foreach (var file in fileCollection)
                {
                    // first convert it to memory stream
                    MemoryStream stream = new MemoryStream();
                    await file.CopyToAsync(stream);
                    // add to db
                    Image image = new Image()
                    {
                        ImageName = file.FileName,
                        ImageBytes = stream.ToArray()
                    };
                    await _dbContext.Images.AddAsync(image);
                    await _dbContext.SaveChangesAsync();
                }

                response.Code = "200";
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("GetDBAllImages")]
        public async Task<IActionResult> GetDBAllImages(string imageCode)
        {
            List<string> imagesList = new List<string>();
            try
            {
                var images = await _dbContext.Images.Where(a => a.ImageName == imageCode).ToListAsync();

                if (images is not null && images.Count() > 0)
                {
                    foreach (var item in images)
                    {
                        // convert the bytes array to base 64 string
                        imagesList.Add(Convert.ToBase64String(item!.ImageBytes!));
                    }
                    return Ok(imagesList);
                }
                else
                {
                    return NotFound("No image found !!");
                }
            }
            catch (Exception ex)
            {
                return NotFound($"Error message --< {ex.Message}");
            }
        }

        [HttpGet("DBDownloadImage")]
        public async Task<IActionResult> DBDownloadImage(string imageCode)
        {
            try
            {
                var image = await _dbContext.Images.FirstOrDefaultAsync(a => a.ImageName == imageCode);

                if (image is not null)
                {
                    var imageArr = File(image.ImageBytes!, "image/png", image.ImageName);
                    return imageArr;
                }
                else
                {
                    return NotFound("Image doest not exists !!");
                }
            }
            catch (Exception)
            {
                return NotFound("Image doest not exists !!");
            }
        }

        //---------------------------------------
        [NonAction]
        protected string GetFilePath(string imageCode)
        {
            return _webHostEnvironment.WebRootPath + "\\Gemini\\Upload\\";
        }
    }
}
