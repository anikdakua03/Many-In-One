using Google.Apis.Auth;
using ManyInOneAPI.Configurations;
using ManyInOneAPI.Data;
using ManyInOneAPI.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace ManyInOneAPI.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ManyInOneDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AuthConfig _authConfig;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly UrlEncoder _urlEncoder;
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public AuthService(ManyInOneDbContext dbContext, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager, IOptionsMonitor<AuthConfig> optionsMonitor, SignInManager<IdentityUser> signInManager, IEmailService emailService, UrlEncoder urlEncoder)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _authConfig = optionsMonitor.CurrentValue;
            _signInManager = signInManager;
            _emailService = emailService;
            _urlEncoder = urlEncoder;
        }

        public async Task<AuthResult> Register(UserRegistrationRequestDTO userRequestDTO)
        {
            var emailExists = await _userManager.FindByEmailAsync(userRequestDTO.Email);
            if (emailExists is not null)
            {
                return new RegistrationResponse()
                { 
                    Errors = new List<string>() 
                    { 
                        "Invalid user, email already exists !!!",
                        "Another error"
                    },
                    Result = false
                };
            }

            // then create that user
            var newUser = new IdentityUser()
            {
                Email = userRequestDTO.Email,
                UserName = userRequestDTO.Email.Split('@')[0],
                EmailConfirmed = false // making it false explicitly, to make sure of
            };
            var isCreated = await _userManager.CreateAsync(newUser, userRequestDTO.Password!);
            if (isCreated.Succeeded)
            {
                // will send a verification code to that email to confirm
                var userId = await _userManager.GetUserIdAsync(newUser);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser); // 
                //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var emailSend = await _emailService.SendMailAsync(newUser.Id, newUser.Email,  WebUtility.UrlEncode(code));

                //var jwttoken = GenerateJwtToken(newUser);

                return new RegistrationResponse()
                {
                    Message = $" Registration successful , confirm your mail , just send to t you {emailSend.Message} !!",
                    Result = true
                };
            }

            return new RegistrationResponse()
            { Errors = new List<string>() {"Error creating user !!!", isCreated!.Errors!.Select(a => a.Description)!.ToString()! },
                    Result = false };
        }

        public async Task<AuthResult> RegisterWithGoogle(string credentials)
        {
            // google auth settings
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _authConfig.GoogleClientId! }
            };

            // check the credential with the settings
            var payload = await GoogleJsonWebSignature.ValidateAsync(credentials, settings);

            if (payload is null)
            {
                return new AuthResult() { Errors = new List<string>() {"User doesn't exists !!!" } ,
                    Result = false };
            }

            // then get the user from payload and check in db
            var existingUser = await _userManager.FindByEmailAsync(payload.Email!);
            if (existingUser is not null)
            {
                return new AuthResult() { Errors = new List<string>() { "User already exists, so try to log in with it!!!" } ,
                    Result = false};
            }

            // then get the user from payload create 
            var newUser = new IdentityUser()
            {
                UserName = payload.FamilyName + payload.GivenName,
                Email = payload.Email,
                EmailConfirmed = true // logging in with google so
            };

            // create and also add login info with provider
            var loginInfo = new UserLoginInfo("GOOGLE", payload.Subject, "Google");
            
            await _userManager.AddLoginAsync(newUser, loginInfo);
            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (!result.Succeeded)
            {
                return new AuthResult() { Errors = new List<string>() { "Error creating the user !!" } ,
                    Result = false};
            }

            // create the user
            var res = await _userManager.CreateAsync(newUser);

            if (!res.Succeeded)
            {
                return new AuthResult() { Errors = new List<string>() {"Error creating the user !!"} ,
                    Result = false };
            }
            var emailSend = await _emailService.SendGreetingMailAsync(newUser.Email);
            // other wise generate token for th euser and allow to log in
            var jwttoken = GenerateJwtToken(newUser);

            return new RegistrationResponse()
            {
                Message = "Registration with google log in successful and shipped also!! ",
                Result = true
            };
        }

        public async Task<AuthResult> ConfirmUserEmail(string userId, string code)
        {
            // Do something if token is expired, else continue with confirmation
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Invalid email parameters !!"
                    }
                };
            }

            // verify from user manager
            var res = await _userManager.ConfirmEmailAsync(user, code);

            if(!res.Succeeded)
            {
                return new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Failed to confirm mail !"
                    }
                };
            }
            return new AuthResult()
            {
                // here need to put some html whrer redirect link will be given
                Result = res.Succeeded,
                Message = $"<div style=\"text-align: center; vertical-align: middle; height: 100px;\"><h1>Your account has been confirmed!</h1><h3>Click here to login: <a href='{_authConfig.Audience}/login'>Login</a></h3></div>"
            };
        }

        public async Task<AuthResult> Login(UserLoginRequestDTO userRequestDTO)
        {
            var existingUser = await _userManager.FindByEmailAsync(userRequestDTO.Email);

            if (existingUser is null)
            {
                return new AuthResult() { Result = false, Errors = new List<string>() { "User doesn't exists !!!"} };
            }

            if(!existingUser.EmailConfirmed)
            {
                return new AuthResult() { Errors = new List<string>() {"Email not confirmed yet!!!" },
                    Result = false };
            }


            // then check password user
            var passwordMatch = await _userManager.CheckPasswordAsync(existingUser, userRequestDTO.Password!);

            if (!passwordMatch)
            {
                return new AuthResult() { Errors = new List<string>() {"Invalid user !!" } ,
                    Result = false};
            }

            if (existingUser.TwoFactorEnabled)
            {
                // send 2 fa code to verify
                return new AuthResult() { emailConfirmed = true, TwoFAEnabled = true, Message = "Put Two factor authentication code ..",
                    Result = true };
            }
            // sign in the user
            var useCookieScheme = true;
            var isPersistent = true;
            _signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

            var result = await _signInManager.PasswordSignInAsync(userRequestDTO.Email, userRequestDTO.Password, isPersistent, lockoutOnFailure: true);

            var jwttoken = GenerateJwtToken(existingUser);

            return new LoginResponse()
            {
                Message = "Login successful and shipped also!! ",
                Result = true,
                UserId = existingUser.Id
            };
        }


        public async Task<AuthResult> LoginWithGoogle(string credentials)
        {
            // google auth settings
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _authConfig.GoogleClientId! }
            };

            // check the credential with the settings
            var payload = await GoogleJsonWebSignature.ValidateAsync(credentials, settings);

            if (payload is null)
            {
                return new AuthResult() { Errors = new List<string>() {"User doesn't exists !!!" },
                    Result = false };
            }

            // then get the user from payload and check in db
            var existingUser = await _userManager.FindByEmailAsync(payload.Email!);

            if (existingUser is null)
            {
                return new AuthResult() { Errors = new List<string>() {"Invalid user !!" },
                    Result = false };
            }

            // other wise generate token for th euser and allow to log in
            var jwttoken = GenerateJwtToken(existingUser);

            return new LoginResponse()
            {
                Message = "Login successful with google and shipped also!! ",
                Result = true,
                UserId = existingUser.Id
            };
        }

        public async Task<AuthResult> VerifyAndLoginWith2FA(string credentials)
        {
            // verify code
            var verifiedRes = await Verify2FA(credentials);

            if(!verifiedRes.Result)
            {
                return new AuthResult() { Errors = verifiedRes.Errors! ,
                    Result = false};
            }

            // then get the user from payload and check in db
            var existingUser = await _userManager.FindByIdAsync(verifiedRes.UserId!);

            if (existingUser is null)
            {
                return new AuthResult() { Errors = new List<string>() { "Invalid user !!" } ,
                    Result = false};
            }

            // other wise generate token for th euser and allow to log in
            var jwttoken = GenerateJwtToken(existingUser);

            return new LoginResponse()
            {
                Message = "Login successful and shipped also!! ",
                Result = true,
                UserId = existingUser.Id
            };
        }


        public async Task<AuthResult> Verify2FA(string credentials)
        {
            var token = _httpContextAccessor.HttpContext!.Request.Cookies["x-access-token"];

            if (token.IsNullOrEmpty())
            {
                return new AuthResult() { Errors = new List<string>() { "All tokens expired, login afgain to continue !!" },
                    Result = false };
            }
            // get user 
            var currUser = await GetUserFromJWT(token!);

            // Strip spaces and hyphens
            var verificationCode = credentials;//.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(currUser, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                return new AuthResult() { Errors = new List<string>() { "Verification code is invalid." },
                    Result = false };
            }

            await _userManager.SetTwoFactorEnabledAsync(currUser, true);

            return new AuthResult() { TwoFAEnabled = true, Message = "2FA code verified", Result = true };
        }

        public async Task<AuthResult> Disable2FA()
        {
            var refreshToken = _httpContextAccessor.HttpContext!.Request.Cookies["x-refresh-token"];

            if (refreshToken.IsNullOrEmpty())
            {
                return new AuthResult() { Errors = new List<string>() { "Login expired ,, Plases log in again to conitnue ..." },
                    Result = false };
            }

            // get the jwt token id linked with this refresh token
            var userId = await _dbContext.RefreshTokens.Where(a => a.Token == refreshToken).Select(b => b.UserId).FirstOrDefaultAsync();

            // get from database from refresh token table
            var user = await _userManager.Users.FirstOrDefaultAsync(a => a.Id == userId);


            if (user == null)
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "Unable to load user with ID"},
                    Result = false
                };
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);

            if (!disable2faResult.Succeeded)
            {
                return new AuthResult() { Errors = new List<string>() { "Unexpected error occurred disabling 2FA." },
                    Result = false };
            }

            return new AuthResult()
            {
                Message = "2fa has been disabled. You can reenable 2fa when you setup an authenticator app",
                TwoFAEnabled = false,
                    Result = true
            };
            
            //return RedirectToPage("./TwoFactorAuthentication");
        }

        public async Task<TwoFAResponse> LoadSharedKeyAndQrCodeUriAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new TwoFAResponse()
                {
                    Result = false,
                    Message = "Invalid user !!"
                }; ;
            }
            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var SharedKey = FormatKey(unformattedKey!);

            var email = await _userManager.GetEmailAsync(user);
            var AuthenticatorUri = GenerateQrCodeUri(email!, unformattedKey!);

            return new TwoFAResponse()
            {
                QR = AuthenticatorUri,
                SharedKey = SharedKey,
                Result = true,
                Message = "Verify now the code from authenticator app !!"
            };
        }

        public async Task<AuthResult> SignOutUser()
        {
            await _signInManager.SignOutAsync();

            await RevokeToken();

            return new AuthResult() { Result = true, Message = " Sign out successfully !!" };
        }

        public async Task<AuthResult> GetRefreshToken()
        {
            // get prev refresh token from request cookies
            var refreshToken = _httpContextAccessor.HttpContext!.Request.Cookies["x-refresh-token"];

            if(refreshToken.IsNullOrEmpty())
            {
                return new AuthResult() { Errors = new List<string>() { "Login expired ,, Plases log in again to conitnue ..."} ,
                    Result = false};
            }

            // get the jwt token id linked with this refresh token
            var userId = await _dbContext.RefreshTokens.Where(a => a.Token == refreshToken).Select(b => b.UserId).FirstOrDefaultAsync();

            // get from database from refresh token table
            var user = await _userManager.Users.FirstOrDefaultAsync(a => a.Id == userId);

            if (user is null) //|| user.TokenExpires < DateTime.UtcNow) 
            {
                return new AuthResult() { Errors = new List<string>() {"User is unauthorized!!" },
                    Result = false };
            }
            // other wiser generate refresh token
            await GenerateJwtToken(user);

            return new AuthResult() { Message = "Refresh token set into cookies successfully !! ", Result = true };
        }

        public async Task<AuthResult> CheckCurrentUser()
        {
            var currUserRes = new AuthResult();
            var currToken = _httpContextAccessor.HttpContext!.Request.Cookies["x-access-token"];

            // but if there is no access token
            // then need to get first the refresh token and then genrate set new token and refresh token also
            if (currToken.IsNullOrEmpty())
            {
                var token = _httpContextAccessor.HttpContext!.Request.Cookies["x-refresh-token"];
                if(token.IsNullOrEmpty())
                {
                    return new AuthResult() { Errors = new List<string>() {"All session , token expired, please log again to continue !!" }, Result = false };
                }

                var currUserId = await _dbContext.RefreshTokens.Where(a => a.Token == token).Select(b => b.UserId).FirstOrDefaultAsync();

                var user = await _userManager.FindByIdAsync(currUserId!);

                if (user is null)
                {
                    return new AuthResult() { Errors = new List<string>() { "Invalid User !!!" }, Result = false };
                }
                //  aslo set the access token
                await GenerateJwtToken(user!);

                currUserRes.UserId = currUserId!;
                currUserRes.TwoFAEnabled = user!.TwoFactorEnabled;
                currUserRes.emailConfirmed = user!.EmailConfirmed;
            }
            else
            {
                // other wise give the user email
                var user = await GetUserFromJWT(currToken!);

                // after that also verifies from user maanger
                var isvalidUser = await _userManager.FindByEmailAsync(user.Email!);

                // need to check what does it return
                //var check = await _signInManager.IsSignedIn(jwt.Claims);

                if (isvalidUser is null)
                {
                    return new AuthResult() { Errors = new List<string>() {"Invalid User !!!" }, Result = false };
                }

                currUserRes.UserId = isvalidUser!.Id!;
                currUserRes.TwoFAEnabled = user.TwoFactorEnabled;
                currUserRes.emailConfirmed = user!.EmailConfirmed;

            }
            currUserRes.Result = true;
            return currUserRes;
        }

        public async Task<AuthResult> RevokeToken() 
        {
            // this is in scenarios when jwt token and refresh token both doesnt work
            // so this will be called from frontend to remove all token for that user 
            var token = _httpContextAccessor.HttpContext!.Request.Cookies["x-refresh-token"];

            if (token.IsNullOrEmpty())
            {
                return new AuthResult() { Errors = new List<string>() {"All tokens expired, login afgain to continue !!" }, Result = false };
            }
            // get the related user id first
                var currUsersToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(a => a.Token == token);
            // get prev refresh token from request and remove all refresh token related to this user
            var refTokens = await _dbContext.RefreshTokens.Where(a => a.UserId == currUsersToken!.UserId).ToListAsync();

            // set the refresh token as invalid
            _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-access-token");
            _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-refresh-token");

            // delete from database
            _dbContext.RefreshTokens.RemoveRange(refTokens!);
            await _dbContext.SaveChangesAsync();


            return new AuthResult() { Message = "All refresh token revoked successfully !! ", Result = true };
        }


        public async Task<AuthResult> DeleteAllData()
        {
            var currToken = _httpContextAccessor.HttpContext!.Request.Cookies["x-access-token"];
            var currUser = new IdentityUser();
            // but if there is no access token
            // then need to get first the refresh token and then genrate set new token and refresh token also
            if (currToken.IsNullOrEmpty())
            {
                var token = _httpContextAccessor.HttpContext!.Request.Cookies["x-refresh-token"];
                if (token.IsNullOrEmpty())
                {
                    return new AuthResult() { Errors = new List<string>() { "All session , token expired, please log again to continue !!" }, Result = false };
                }

                var currUserId = await _dbContext.RefreshTokens.Where(a => a.Token == token).Select(b => b.UserId).FirstOrDefaultAsync();

                currUser = await _userManager.FindByIdAsync(currUserId!);

                // set the refresh token as invalid
                _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-refresh-token");
            }
            else
            {
                // other wise give the user email
                var user = await GetUserFromJWT(currToken!);

                // after that also verifies from user maanger
                var isvalidUser = await _userManager.FindByEmailAsync(user.Email!);

                if (isvalidUser is null)
                {
                    return new AuthResult() { Errors = new List<string>() { "Invalid User !!!" }, Result = false };
                }

                currUser = isvalidUser;

                // set the access token as invalid
                _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-access-token");
            }
            // delete all things 
            var result = await _userManager.DeleteAsync(currUser!);
            
            if (!result.Succeeded)
            {
                return new AuthResult() { Errors = new List<string>() { "Unexpected error occurred deleting user." }, Result = false };
            }
            // get prev refresh token from request and remove all refresh token related to this user
            var refTokens = await _dbContext.RefreshTokens.Where(a => a.UserId == currUser!.Id).ToListAsync();

            _dbContext.RefreshTokens.RemoveRange(refTokens!);

            await _dbContext.SaveChangesAsync();

            await _signInManager.SignOutAsync();

            return new AuthResult() { Message = "User deleted themselves.", Result = true };
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
                    new Claim(JwtRegisteredClaimNames.Aud, _authConfig.Audience!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()!) // this jti is for tracking every jwt token , generated 
                }),
                Issuer = _authConfig.Issuer,
                Audience = _authConfig.Audience,
                Expires = DateTime.Now.ToLocalTime().AddMinutes(30), // need to be as geenral short like in min ,
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
                ExpiryDate = DateTime.Now.ToLocalTime().AddDays(1)
            };

            return refreshToken;
        }

        private void SetJWTToken(string encryptedToken)
        {
            // this will set refresh token into cookies
            _httpContextAccessor.HttpContext!.Response.Cookies.Append("x-access-token", encryptedToken,
             new CookieOptions
             {
                 Expires = DateTime.Now.ToLocalTime().AddMinutes(30),
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
                AddedDate = DateTime.Now.ToLocalTime(),
                ExpiryDate = DateTime.Now.ToLocalTime().AddDays(1)
            };

            var res =  _dbContext.RefreshTokens.Update(newRefreshToken);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private async Task<IdentityUser> GetUserFromJWT(string credential)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(credential);

            // after that also verifies from user maanger
            var isvalidUser = await _userManager.FindByEmailAsync(jwt.Subject);

            return isvalidUser!;
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
            AuthenticatorUriFormat,
                _urlEncoder.Encode("ManyInOne"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }


        #endregion
    }
}
