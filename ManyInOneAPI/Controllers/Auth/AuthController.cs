using Google.Apis.Auth;
using ManyInOneAPI.Configurations;
using ManyInOneAPI.Data;
using ManyInOneAPI.Models.Auth;
using ManyInOneAPI.Services.Auth;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ManyInOneAPI.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;

        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }


        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDTO userRequestDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var res = await _authService.Register(userRequestDTO);

                    if (res.Errors is null)
                    {
                        return Ok(res);
                    }

                    return BadRequest(res.Errors);

                }
                return BadRequest("Invalid request !! ");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("RegisterWithGoogleLogIn")]
        public async Task<IActionResult> RegisterWithGoogle([FromBody] string credentials)
        {
            try
            {
                if (!credentials.IsNullOrEmpty())
                {
                    var res = await _authService.RegisterWithGoogle(credentials);

                    if (res.Errors is null)
                    {
                        return Ok(res);
                    }

                    return BadRequest(res.Errors);
                }
                return BadRequest("Invalid request !! ");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            try
            {
                var res = await _authService.ConfirmUserEmail(userId, code);

                if (res.Result && res.Errors is null || res.Errors!.Count() == 0)
                {
                    //string htmlContent = $"<p>Your account has been confirmed!</p><p>Click here to login: <a href='/login'>Login</a></p>";
                    return new ContentResult()
                    {
                        Content = res.Message,
                        ContentType = "text/html",
                        StatusCode = 200
                    };
                }
                return BadRequest("Invalid request !! ");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO userRequestDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var res = await _authService.Login(userRequestDTO);

                    if (res.Result)
                    {
                        return Ok(res);
                    }

                    return BadRequest(res.Errors);
                }
                return BadRequest("Invalid request !! ");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("LogInWithGoogle")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] string credentials)
        {
            try
            {
                if (!credentials.IsNullOrEmpty())
                {
                    var res = await _authService.LoginWithGoogle(credentials);

                    if (res.Result)
                    {
                        return Ok(res);
                    }

                    return BadRequest(res.Errors);
                }
                return BadRequest("Invalid request !! ");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("VerifyAndLoginWith2FA")]
        public async Task<IActionResult> LoginWith2FA([FromBody] string code)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var res = await _authService.VerifyAndLoginWith2FA(code);

                    if (res.Result)
                    {
                        return Ok(res);
                    }

                    return BadRequest("Error generating two factor qr code..");
                }
                return BadRequest("Invalid request !! ");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Verify2FA")]
        public async Task<IActionResult> Verify2FA([FromBody] string code)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var res = await _authService.Verify2FA(code);

                    if (res.Result)
                    {
                        return Ok(res);
                    }

                    return BadRequest(res.Message);
                }
                return BadRequest("Invalid request !! ");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Get2FAQRCode")]
        public async Task<IActionResult> Enable2FAAndGetQR([FromBody] string userId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var res = await _authService.LoadSharedKeyAndQrCodeUriAsync(userId);

                    if (res.Result)
                    {
                        return Ok(res);
                    }

                    return BadRequest("Error generating two factor qr code..");
                }
                return BadRequest("Invalid request !! ");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Disable2FA")]
        public async Task<IActionResult> Disable2FA()
        {
            try
            {
                var res = await _authService.Disable2FA();

                if (res.Result)
                {
                    return Ok(res);
                }

                return BadRequest(res.Errors![0]);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("SignOut")]
        public async Task<IActionResult> SignOutuser()
        {
            try 
            {
                var res = await _authService.SignOutUser();

                if (res.Errors is null)
                {
                    return Ok(res);
                }
                return Unauthorized("Invalid user !!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetRefreshToken")]
        public async Task<ActionResult<string>> GetRefreshToken()
        {
            try
            { 
                var res = await _authService.GetRefreshToken();

                if (res.Errors is null)
                {
                    return Ok(res);
                }

                return Unauthorized(res.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetCurrentUser")]
        public async Task<IActionResult> CheckCurrentUser()
        {
            try
            { 
                var res = await _authService.CheckCurrentUser();

                if (res.Errors is null)
                {
                    return Ok(res);
                }

                return Unauthorized(res.Errors); 
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("RevokeToken")]
        public async Task<ActionResult<string>> RevokeToken()
        {
            try
            {
                var res = await _authService.RevokeToken();

                if (res.Errors is null)
                {
                    return Ok(res);
                }
                return BadRequest($"Error while revoking ==> {res.Errors[0]}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("DeleteUserData")]
        public async Task<ActionResult<string>> DeleteUserData()
        {
            try
            {
                var res = await _authService.DeleteAllData();

                if (res.Errors is null)
                {
                    return Ok(res);
                }

                return BadRequest($"Error while revoking ==> {res.Errors[0]}");
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        
    }
}
