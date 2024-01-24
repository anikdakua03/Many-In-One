using Google.Apis.Auth;
using ManyInOneAPI.Configurations;
using ManyInOneAPI.Data;
using ManyInOneAPI.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ManyInOneAPI.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ManyInOneDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AuthConfig _authConfig;
        private readonly SignInManager<IdentityUser> _signInManager;
        public AuthService(ManyInOneDbContext dbContext, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager, IOptionsMonitor<AuthConfig> optionsMonitor, SignInManager<IdentityUser> signInManager)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _authConfig = optionsMonitor.CurrentValue;
            _signInManager = signInManager;
        }

        public async Task<AuthResult> Register(UserRegistrationRequestDTO userRequestDTO)
        {
            List<string> errors = new List<string>();
            var emailExists = await _userManager.FindByEmailAsync(userRequestDTO.Email);

            if (emailExists is not null)
            {
                errors.Add("Invalid user, email already exists !!!");

                return new RegistrationResponse()
                { Errors = errors };
            }

            // then create that user
            var newUser = new IdentityUser()
            {
                Email = userRequestDTO.Email,
                UserName = userRequestDTO.Name
            };
            var isCreated = await _userManager.CreateAsync(newUser, userRequestDTO.Password!);
            if (isCreated.Succeeded)
            {
                var jwttoken = GenerateJwtToken(newUser);

                return new RegistrationResponse()
                {
                    Token = $"{jwttoken.Result} -> Registration successful and shipped also!! ",
                    Result = true
                };
            }

            errors.Add("Error creating user !!!");

            return new RegistrationResponse()
            { Errors = errors };
        }


        public async Task<AuthResult> RegisterWithGoogle(string credentials)
        {
            List<string> errors = new List<string>();

            // google auth settings
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _authConfig.GoogleClientId! }
            };

            // check the credential with the settings
            var payload = await GoogleJsonWebSignature.ValidateAsync(credentials, settings);

            if (payload is null)
            {
                errors.Add("User doesn't exists !!!");
                return new AuthResult() { Errors = errors };
            }

            // then get the user from payload and check in db
            var existingUser = await _userManager.FindByEmailAsync(payload.Email!);
            if (existingUser is not null)
            {
                errors.Add("User already exists !!");
                return new AuthResult() { Errors = errors };
            }

            // then get the user from payload create 
            var newUser = new IdentityUser()
            {
                UserName = payload.FamilyName + payload.GivenName,
                Email = payload.Email
            };
            // create the user
            var res = await _userManager.CreateAsync(newUser);

            if (!res.Succeeded)
            {
                errors.Add("Error creating the user !!");
                return new AuthResult() { Errors = errors };
            }

            // create and also add login info with provider
            var loginInfo = new UserLoginInfo("GOOGLE", payload.Subject, "Google");
            
            await _userManager.AddLoginAsync(newUser, loginInfo);
           

            // other wise generate token for th euser and allow to log in
            var jwttoken = GenerateJwtToken(newUser);

            return new RegistrationResponse()
            {
                Token = "Registration with google log in successful and shipped also!! ",
                Result = true
            };
        }


        public async Task<AuthResult> Login(UserLoginRequestDTO userRequestDTO)
        {
            List<string> errors = new List<string>();
            var existingUser = await _userManager.FindByEmailAsync(userRequestDTO.Email);

            if (existingUser is null)
            {
                errors.Add("User doesn't exists !!!");
                return new AuthResult() { Errors = errors };
            }

            // then check password user
             var passwordMatch = await _userManager.CheckPasswordAsync(existingUser, userRequestDTO.Password!);

            if (!passwordMatch)
            {
                errors.Add("Invalid user !!");
                return new AuthResult() { Errors = errors };
            }

            // sign in the user
            var useCookieScheme = true;
            var isPersistent = true;
            _signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

            var result = await _signInManager.PasswordSignInAsync(userRequestDTO.Email, userRequestDTO.Password, isPersistent, lockoutOnFailure: true);

            var jwttoken = GenerateJwtToken(existingUser);

            return new LoginResponse()
            {
                Token = "Login successful and shipped also!! ",
                Result = true
            };
        }


        public async Task<AuthResult> LoginWithGoogle(string credentials)
        {
            List<string> errors = new List<string>();
            // google auth settings
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _authConfig.GoogleClientId! }
            };

            // check the credential with the settings
            var payload = await GoogleJsonWebSignature.ValidateAsync(credentials, settings);

            if (payload is null)
            {
                errors.Add("User doesn't exists !!!");
                return new AuthResult() { Errors = errors };
            }

            // then get the user from payload and check in db
            var existingUser = await _userManager.FindByEmailAsync(payload.Email!);

            if (existingUser is null)
            {
                errors.Add("Invalid user !!");
                return new AuthResult() { Errors = errors };
            }

            // other wise generate token for th euser and allow to log in
            var jwttoken = GenerateJwtToken(existingUser);

            return new LoginResponse()
            {
                Token = "Login successful with google and shipped also!! ",
                Result = true
            };
        }


        public async Task<AuthResult> SignOutUser()
        {
            // get prev refresh token from request cookies
            //var token = _httpContextAccessor.HttpContext!.Request.Cookies["x-access-token"];

            await _signInManager.SignOutAsync();

            await RevokeToken();

            return new AuthResult() { Result = true, Token = " Sign out successfully !!" };
        }

        public async Task<AuthResult> GetRefreshToken()
        {
            List<string> errors = new List<string>();

            // get prev refresh token from request cookies
            var refreshToken = _httpContextAccessor.HttpContext!.Request.Cookies["x-refresh-token"];

            if(refreshToken.IsNullOrEmpty())
            {
                errors.Add("Login expired ,, Plases log in again to conitnue ...");
                return new AuthResult() { Errors = errors };
            }

            // get the jwt token id linked with this refresh token
            var userId = await _dbContext.RefreshTokens.Where(a => a.Token == refreshToken).Select(b => b.UserId).FirstOrDefaultAsync();

            // get from datbase from refresh token table
            var user = await _userManager.Users.FirstOrDefaultAsync(a => a.Id == userId);

            if (user is null) //|| user.TokenExpires < DateTime.UtcNow) 
            {
                errors.Add("User is unauthorized!!");
                return new AuthResult() { Errors = errors };
            }
            // other wiser generate refresh token
            await GenerateJwtToken(user);

            return new AuthResult() { Token = "Refresh token set into cookies successfully !! ", Result = true };
        }

        public async Task<AuthResult> CheckCurrentUser()
        {
            List<string> errors = new List<string>();
            string userEmail = "";
            var currToken = _httpContextAccessor.HttpContext!.Request.Cookies["x-access-token"];

            // but if there is no access token
            // then need to get first the refresh token and then genrate set new token and refresh token also
            if (currToken.IsNullOrEmpty())
            {
                var refresh = _httpContextAccessor.HttpContext!.Request.Cookies["x-refresh-token"];
                if(refresh.IsNullOrEmpty())
                {
                    errors.Add("All session , token expired, please log again to continue !!");
                    return new AuthResult() { Errors = errors, Result = false };
                }

                var userId = await _dbContext.RefreshTokens.Where(a => a.Token == refresh).Select(b => b.UserId).FirstOrDefaultAsync();

                var user= await _userManager.FindByIdAsync(userId!);
                //  aslo set the access token
                await GenerateJwtToken(user!);

                userEmail = user!.Email!;
            }
            else
            {
                // other wise give the user email
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(currToken);

                // after that also verifies from user maanger
                var isvalidUser = await _userManager.FindByEmailAsync(jwt.Subject);

                // need to check what does it return
                //var check = await _signInManager.IsSignedIn(jwt.Claims);

                userEmail = isvalidUser!.Email!;

                if (isvalidUser is null)
                {
                    errors.Add("Invalid User !!!");
                    return new AuthResult() { Errors = errors };
                }
            }

            return new AuthResult() { UserEmail = userEmail, Result = true };
        }

        public async Task<AuthResult> RevokeToken() //string token/ should be by user id
        {
            List<string> errors = new List<string>();
            // this is in scenarios when jwt token and refresh token both doesnt work
            // so this will be called from frontend to remove all token for that user 
            var token = _httpContextAccessor.HttpContext!.Request.Cookies["x-refresh-token"];

            if (token.IsNullOrEmpty())
            {
                errors.Add("All tokens expired, login afgain to continue !!");
                return new AuthResult() { Errors = errors };
            }
            // get the related user id first
                var user = await _dbContext.RefreshTokens.FirstOrDefaultAsync(a => a.Token == token);
            // get prev refresh token from request and remove all refresh token related to this user
            var refToken = await _dbContext.RefreshTokens.Where(a => a.UserId == user!.UserId).ToListAsync();

            // delete from database
            _dbContext.RefreshTokens.RemoveRange(refToken!);
            await _dbContext.SaveChangesAsync();


            return new AuthResult() { Token = "All refresh token revoked successfully !! ", Result = true };
        }


        #region All Utilities
        //=============================================== Utilities =====================================================
        private async Task<string> GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenhandler = new JwtSecurityTokenHandler();
            //_configuration.GetSection("JwtConfig:Secret").Value!

            var key = Encoding.UTF8.GetBytes(_authConfig.Secret!);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()!) // this jti is for tracking every jwt token , generated 
                }),

                Expires = DateTime.UtcNow.AddHours(1), // need to be as geenral short like in min ,
                //when implement refresh token , change here
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512)
            };

            var token = jwtTokenhandler.CreateToken(tokenDescriptor);

            //var secu = tokenDescriptor;
            var encryptedToken = jwtTokenhandler.WriteToken(token);

            // attach to cookies
            // before attaching encrypt the token
            SetJWTToken(encryptedToken);


            var refreshToken = GenerateRefreshToken();

            await SetRefreshToken(refreshToken, token.Id, user);

            return encryptedToken;
        }

        private RefreshToken GenerateRefreshToken()
        {
            // this can be random string or another jwt token
            var refreshToken = new RefreshToken()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(1)
            };

            return refreshToken;
        }

        private void SetJWTToken(string encryptedToken)
        {
            // this will set refresh token into cookies
            _httpContextAccessor.HttpContext!.Response.Cookies.Append("x-access-token", encryptedToken,
             new CookieOptions
             {
                 Expires = DateTime.Now.ToLocalTime().AddMinutes(45),
                 Secure = true,
                 HttpOnly = true,
                 IsEssential = true,
                 SameSite = SameSiteMode.None // chamged to none , to check if it is working?!
             });
        }

        private async Task<bool> SetRefreshToken(RefreshToken refreshToken, string jwtId, IdentityUser user)
        {
            // this will set refresh token into cookies
            _httpContextAccessor.HttpContext!.Response.Cookies.Append("x-refresh-token", refreshToken.Token!, new CookieOptions
            {
                Expires = refreshToken.ExpiryDate,
                Secure = true,
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.None // chamged to none , to check if it is working?!
            });

            // also add to the database in the refresh token table
            var newRefreshToken = new RefreshToken()
            {
                UserId = user.Id,
                JWTId = jwtId, // attaching linked jwt token id
                Token = refreshToken.Token,
                IsUsed = true,
                IsRevoked = false,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(30)
            };

            var res =  _dbContext.RefreshTokens.Update(newRefreshToken);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        #endregion
    }
}
