using Microsoft.AspNetCore.Mvc;

namespace ManyInOneAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("check")]
        public ActionResult GetResult()
        {
            return Ok("I am up and running....");
        }
    }
}