using Google.Apis.Auth;
using ManyInOneAPI.Configurations;
using ManyInOneAPI.Data;
using ManyInOneAPI.Infrastructure.Shared;
using ManyInOneAPI.Models.Auth;
using ManyInOneAPI.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
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
            if (ModelState.IsValid)
            {
                var res = await _authService.Register(userRequestDTO);

                return Ok(res);
            }
            // return Ok(Result<object>.Failure(Error.Failure("Failed", "Invalid request !! ")));
            return Ok(Result<object>.Failure(Error.Failure("Failed", "Invalid request !! ")));
        }

        [HttpPost]
        [Route("RegisterWithGoogleLogIn")]
        public async Task<IActionResult> RegisterWithGoogle([FromBody] string credentials)
        {
            if (!credentials.IsNullOrEmpty())
            {
                var res = await _authService.RegisterWithGoogle(credentials);

                return Ok(res);
            }
            return Ok(Result<object>.Failure(Error.Failure("Failed", "Invalid request !! ")));
        }

        [HttpPost]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmail confirmEmailBody)
        {
            if (ModelState.IsValid)
            {
                var res = await _authService.ConfirmUserEmail(confirmEmailBody);

                return Ok(res);
            }
            return Ok(Result<object>.Failure(Error.Failure("Failed", "Invalid request !! ")));
        }

        [HttpPost]
        [Route("ResendConfirmationEmail/{userEmail}")]
        public async Task<IActionResult> ResendConfirmEmail([Required] string userEmail)
        {
            if (!userEmail.IsNullOrEmpty())
            {
                var res = await _authService.ResendConfirmationMail(userEmail);

                return Ok(res);
            }
            return Ok(Result<object>.Failure(Error.Failure("Failed", "Invalid request !! ")));
        }

        [HttpPost]
        [Route("ForgotPasswordEmail/{userEmail}")]
        public async Task<IActionResult> ResetPasswordMail([Required] string userEmail)
        {
            if (!userEmail.IsNullOrEmpty())
            {
                var res = await _authService.ForgotPasswordMail(userEmail);

                return Ok(res);
            }
            return Ok(Result<object>.Failure(Error.Failure("Failed", "Invalid request !! ")));
        }

        [HttpPut]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetUserPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            if (ModelState.IsValid)
            {
                var res = await _authService.ResetPassword(resetPasswordDTO);

                return Ok(res);
            }
            return Ok(Result<object>.Failure(Error.Failure("Failed", "Invalid request !! ")));

        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO userRequestDTO)
        {
            if (ModelState.IsValid)
            {
                var res = await _authService.Login(userRequestDTO);

                return Ok(res);
            }
            return Ok(Result<object>.Failure(Error.Failure("Failed", "Invalid request !! ")));
        }

        [HttpPost]
        [Route("LogInWithGoogle")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] string credentials)
        {
            if (!credentials.IsNullOrEmpty())
            {
                var res = await _authService.LoginWithGoogle(credentials);

                return Ok(res);
            }
            return Ok(Result<object>.Failure(Error.Failure("Failed", "Invalid request !! ")));
        }

        [HttpPost]
        [Route("VerifyAndLoginWith2FA")]
        public async Task<IActionResult> LoginWith2FA([FromBody] Login2FARequest login2FARequest)
        {
            if (ModelState.IsValid)
            {
                var res = await _authService.VerifyAndLoginWith2FA(login2FARequest);

                return Ok(res);
            }
            return Ok(Result<object>.Failure(Error.Failure("Failed", "Invalid request !! ")));
        }

        [HttpPost]
        [Route("Verify2FA")]
        public async Task<IActionResult> Verify2FA([FromBody] string code)
        {
            if (!code.IsNullOrEmpty())
            {
                var res = await _authService.Verify2FA(code);

                return Ok(res);
            }
            return Ok(Result<object>.Failure(Error.Failure("Failed", "Invalid request !! ")));
        }

        [HttpPost]
        [Route("Get2FAQRCode")]
        public async Task<IActionResult> Enable2FAAndGetQR([FromBody] string userId)
        {
            if (!userId.IsNullOrEmpty())
            {
                var res = await _authService.LoadSharedKeyAndQrCodeUriAsync(userId);

                return Ok(res);
            }
            return Ok(Result<object>.Failure(Error.Failure("Failed", "Invalid request !! ")));
        }

        [HttpPost]
        [Route("Disable2FA")]
        public async Task<IActionResult> Disable2FA()
        {
            var res = await _authService.Disable2FA();

            return Ok(res);
        }

        [HttpPost]
        [Route("SignOut")]
        public async Task<IActionResult> SignOutuser()
        {
            var res = await _authService.SignOutUser();

            return Ok(res);
        }

        [HttpGet]
        [Route("GetRefreshToken")]
        public async Task<IActionResult> GetRefreshToken()
        {
            var res = await _authService.GetRefreshToken();

            return Ok(res);
        }

        [HttpGet]
        [Route("GetCurrentUser")]
        public async Task<IActionResult> CheckCurrentUser()
        {
            var res = await _authService.CheckCurrentUser();

            return Ok(res);
        }

        [HttpPost]
        [Route("RevokeToken")]
        public async Task<IActionResult> RevokeToken()
        {
            var res = await _authService.RevokeToken();

            return Ok(res);
        }


        [HttpPost]
        [Route("DeleteUserData")]
        public async Task<IActionResult> DeleteUserData()
        {
            var res = await _authService.DeleteAllData();

            return Ok(res);
        }
    }
}
