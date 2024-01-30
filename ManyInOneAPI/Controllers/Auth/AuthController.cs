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
            if (ModelState.IsValid)
            {
                var res = await _authService.Register(userRequestDTO);

                if(res.Errors is null)
                {
                    return Ok(res);
                }
                //var emailExists = await _userManager.FindByEmailAsync(userRequestDTO.Email);

                //if (emailExists is not null)
                //{
                //    return BadRequest("Invalid user, email already exists !!!");
                //}

                //// then create that user
                //var newUser = new IdentityUser()
                //{
                //    Email = userRequestDTO.Email,
                //    UserName = userRequestDTO.Name
                //};
                //var isCreated = await _userManager.CreateAsync(newUser, userRequestDTO.Password!);
                //if (isCreated.Succeeded)
                //{
                //    var jwttoken = GenerateJwtToken(newUser);

                //    return Ok(new RegistrationResponse()
                //    {
                //        Token = $"{jwttoken.Result} -> Registration successful and shipped also!! ",
                //        Result = true
                //    });
                //}

                return BadRequest(res.Errors);

            }
            return BadRequest("Invalid request !! ");
        }

        [HttpPost]
        [Route("RegisterWithGoogleLogIn")]
        public async Task<IActionResult> RegisterWithGoogle([FromBody] string credentials)
        {
            if (!credentials.IsNullOrEmpty())
            {
                var res = await _authService.RegisterWithGoogle(credentials);

                if(res.Errors is null)
                {
                    return Ok(res);
                }
                //    // google auth settings
                //    var settings = new GoogleJsonWebSignature.ValidationSettings()
                //    {
                //        Audience = new List<string> { _authConfig.GoogleClientId! }
                //    };

                //    // check the credential with the settings
                //    var payload = await GoogleJsonWebSignature.ValidateAsync(credentials, settings);

                //    if (payload is null)
                //    {
                //        return BadRequest("User doesn't exists !!!");
                //    }

                //    // then get the user from payload and check in db
                //    var existingUser = await _userManager.FindByEmailAsync(payload.Email!);
                //    if (existingUser is not null)
                //    {
                //        return BadRequest("User already exists !!");
                //    }

                //    // then get the user from payload create 
                //    var newUser = new IdentityUser()
                //    {
                //        UserName = payload.FamilyName + payload.GivenName,
                //        Email = payload.Email
                //    };
                //    // create and also add login info with provider
                //    var loginInfo = new UserLoginInfo("GOOGLE",payload.Subject,"Google");

                //    var res = await _userManager.AddLoginAsync(newUser, loginInfo);

                //    if(res is null)
                //    {
                //        return BadRequest("Error creating the user !!");
                //    }
                //    // other wise generate token for th euser and allow to log in
                //    var jwttoken = GenerateJwtToken(newUser);

                //    return Ok(new RegistrationResponse()
                //    {
                //        Token = "Registration with google log in successful and shipped also!! ",
                //        Result = true
                //    });
                return BadRequest(res.Errors);
            }
            return BadRequest("Invalid request !! ");
        }

        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            var res = await _authService.ConfirmUserEmail(userId, code);

            if(res.Result && res.Errors is null || res.Errors!.Count() == 0)
            {
                //string htmlContent = $"<p>Your account has been confirmed!</p><p>Click here to login: <a href='/login'>Login</a></p>";
                return new ContentResult()
                {
                    Content = res.Message,
                    ContentType = "text/html",
                    StatusCode = 200
                };
                //return Ok(res);
            }
            return BadRequest("Invalid request !! ");
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO userRequestDTO)
        {
            if (ModelState.IsValid)
            {
                var res = await _authService.Login(userRequestDTO);

                if(res.Errors is null ) //&& !res.TwoFAEnabled
                {
                    return Ok(res);
                }
                //else if(res.Errors is null && res.TwoFAEnabled)
                //{
                //    return Redirect(res.Message!);
                //}

                return BadRequest(res.Errors);
            }
            return BadRequest("Invalid request !! ");
        }

        [HttpPost]
        [Route("LogInWithGoogle")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] string credentials)
        {
            if (!credentials.IsNullOrEmpty())
            {
                var res = await _authService.LoginWithGoogle(credentials);

                if(res.Errors is null)
                {
                    return Ok(res);
                }
                // google auth settings
                //var settings = new GoogleJsonWebSignature.ValidationSettings()
                //{
                //    Audience = new List<string> { _authConfig.GoogleClientId!}
                //};

                //// check the credential with the settings
                //var payload = await GoogleJsonWebSignature.ValidateAsync(credentials, settings);
                
                //if (payload is null)
                //{
                //    return BadRequest("User doesn't exists !!!");
                //}

                //// then get the user from payload and check in db
                //var existingUser = await _userManager.FindByEmailAsync(payload.Email!);

                //if (existingUser is null)
                //{
                //    return BadRequest("Invalid user !!");
                //}

                //// other wise generate token for th euser and allow to log in
                //var jwttoken = GenerateJwtToken(existingUser);

                return BadRequest(res.Errors);
            }
            return BadRequest("Invalid request !! ");
        }

        [HttpPost]
        [Route("VerifyAndLoginWith2FA")]
        public async Task<IActionResult> LoginWith2FA([FromBody] string code)
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

        [HttpPost]
        [Route("Verify2FA")]
        public async Task<IActionResult> Verify2FA([FromBody] string code)
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

        [HttpPost]
        [Route("Get2FAQRCode")]
        public async Task<IActionResult> Enable2FAAndGetQR([FromBody] string userId)
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

        [HttpPost]
        [Route("Disable2FA")]
        public async Task<IActionResult> Disable2FA()
        {
            var res = await _authService.Disable2FA();

            if (res.Result)
            {
                return Ok(res);
            }

            return BadRequest(res.Errors![0]);
        }

        [HttpPost]
        [Route("SignOut")]
        public async Task<IActionResult> SignOutuser()
        {
            
            var res = await _authService.SignOutUser();

            if (res.Errors is null)
            {
                return Ok(res);
            }
            return Unauthorized("Invalid user !!");
        }

        [HttpGet]
        [Route("GetRefreshToken")]
        public async Task<ActionResult<string>> GetRefreshToken()
        {
            var res = await _authService.GetRefreshToken();

            if(res.Errors is null)
            {
                return Ok(res);
            }
            // get prev refresh token from request cookies
            //var refreshToken = Request.Cookies["x-refresh-token"];

            //// also need to check that refresh tokens expiration time 
            //var currTokenExpiryTime = await _dbContext.RefreshTokens.Where(a => a.Token == refreshToken).Select(b => b.ExpiryDate).FirstOrDefaultAsync();

            //if (currTokenExpiryTime >= DateTime.Now) // means expired
            //{
            //    return BadRequest("Plases log in again to conitnue ...");
            //}

            //// get the jwt token id linked with this refresh token
            //var userId = await _dbContext.RefreshTokens.Where(a => a.Token == refreshToken).Select(b => b.UserId).FirstOrDefaultAsync();
            //// get from datbase from refresh token table
            //var user = await _userManager.Users.FirstOrDefaultAsync(a => a.Id == userId);

            //if (user is null) //|| user.TokenExpires < DateTime.UtcNow) 
            //{
            //    return Unauthorized("User is unauthorized!!");
            //}
            //// other wiser generate refresh token
            //GenerateRefreshToken();

            return Unauthorized(res.Errors);
        }

        [HttpGet]
        [Route("GetCurrentUser")]
        public async Task<IActionResult> CheckCurrentUser()
        {

            var res = await _authService.CheckCurrentUser();

            if (res.Errors is null)
            {
                return Ok(res);
            }
            return Unauthorized(res.Errors);
        }

        [HttpDelete]
        [Route("RevokeToken")]
        public async Task<ActionResult<string>> RevokeToken() //string token/ should be by user id
        {
            var res = await _authService.RevokeToken();

            if(res.Errors is null)
            {
                return Ok(res);
            }
            //// this is in scenarios when jwt token and refresh token both doesnt work
            //// so this will be called from frontend to remove all token for that user 
            //var token = Request.Cookies["x-refresh-token"];
            //// get the related user id first
            //var user = await _dbContext.RefreshTokens.FirstOrDefaultAsync(a => a.Token == token);
            //// get prev refresh token from request and remove all refresh token related to this user
            //var refToken = await _dbContext.RefreshTokens.Where(a => a.UserId == user!.UserId).ToListAsync();

            //// delete from database
            //_dbContext.RefreshTokens.RemoveRange(refToken!);
            //await _dbContext.SaveChangesAsync();

            return BadRequest($"Error while revoking ==> {res.Errors[0]}");
        }


        [HttpPost]
        [Route("DeleteUserData")]
        public async Task<ActionResult<string>> DeleteUserData()
        {
            var res = await _authService.DeleteAllData();

            if (res.Errors is null)
            {
                return Ok(res);
            }

            return BadRequest($"Error while revoking ==> {res.Errors[0]}");
        }
        //#region All Utilities
        ////=============================================== Utilities =====================================================
        //private async Task<string> GenerateJwtToken(IdentityUser user)
        //{
        //    var jwtTokenhandler = new JwtSecurityTokenHandler();
        //    //_configuration.GetSection("JwtConfig:Secret").Value!

        //    var key = Encoding.UTF8.GetBytes(_authConfig.Secret!);

        //    var tokenDescriptor = new SecurityTokenDescriptor()
        //    {
        //        Subject = new ClaimsIdentity(new[]
        //        {
        //            new Claim("Id", user.Id),
        //            new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
        //            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
        //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()!) // this jti is for tracking every jwt token , generated 
        //        }),

        //        Expires = DateTime.UtcNow.AddHours(1), // need to be as geenral short like in min ,
        //        //when implement refresh token , change here
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512)
        //    };

        //    var token = jwtTokenhandler.CreateToken(tokenDescriptor);

        //    //var secu = tokenDescriptor;
        //    var encryptedToken = jwtTokenhandler.WriteToken(token);

        //    // attach to cookies
        //    // before attaching encrypt the token
        //    SetJWTToken(encryptedToken);


        //    var refreshToken = GenerateRefreshToken();

        //    await SetRefreshToken(refreshToken, token.Id, user);

        //    return encryptedToken;
        //}

        //private RefreshToken GenerateRefreshToken()
        //{
        //    // this can be random string or another jwt token
        //    var refreshToken = new RefreshToken()
        //    {
        //        Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
        //        AddedDate = DateTime.UtcNow,
        //        ExpiryDate = DateTime.UtcNow.AddDays(1)
        //    };

        //    return refreshToken;
        //}

        //private void SetJWTToken(string encryptedToken)
        //{
        //    // this will set refresh token into cookies
        //    HttpContext.Response.Cookies.Append("x-access-token", encryptedToken,
        //     new CookieOptions
        //     {
        //         Expires = DateTime.Now.ToLocalTime().AddMinutes(45),
        //         Secure = true,
        //         HttpOnly = true,
        //         IsEssential = true,
        //         SameSite = SameSiteMode.None
        //     });
        //}

        //private async Task<bool> SetRefreshToken(RefreshToken refreshToken, string jwtId, IdentityUser user)
        //{
        //    // this will set refresh token into cookies
        //    HttpContext.Response.Cookies.Append("x-refresh-token", refreshToken.Token!, new CookieOptions
        //    {
        //        Expires = refreshToken.ExpiryDate,
        //        Secure = true,
        //        HttpOnly = true,
        //        IsEssential = true,
        //        SameSite = SameSiteMode.None
        //    });

        //    // also add to the database in the refresh token table
        //    var newRefreshToken = new RefreshToken()
        //    {
        //        UserId = user.Id,
        //        JWTId = jwtId, // attaching linked jwt token id
        //        Token = refreshToken.Token,
        //        IsUsed = true,
        //        IsRevoked = false,
        //        AddedDate = DateTime.UtcNow,
        //        ExpiryDate = DateTime.UtcNow.AddDays(30)
        //    };

        //    var res = await _dbContext.RefreshTokens.AddAsync(newRefreshToken);
        //    await _dbContext.SaveChangesAsync();

        //    return true;
        //}

        //#endregion
    }
}
