using Google.Apis.Auth;
using ManyInOneAPI.Configurations;
using ManyInOneAPI.Data;
using ManyInOneAPI.Infrastructure.Shared;
using ManyInOneAPI.Models.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
        //private readonly ManyInOneDbContext _dbContext;
        private readonly ManyInOnePgDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AuthConfig _authConfig;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly UrlEncoder _urlEncoder;
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public AuthService(ManyInOnePgDbContext dbContext, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager, IOptionsMonitor<AuthConfig> optionsMonitor, SignInManager<IdentityUser> signInManager, IEmailService emailService, UrlEncoder urlEncoder)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _authConfig = optionsMonitor.CurrentValue;
            _signInManager = signInManager;
            _emailService = emailService;
            _urlEncoder = urlEncoder;
        }

        public async Task<Result<AuthResult>> Register(UserRegistrationRequestDTO userRequestDTO)
        {
            var emailExists = await _userManager.FindByEmailAsync(userRequestDTO.Email);

            if (emailExists is not null)
            {
                return Result<AuthResult>.Failure(Error.Conflict("Conflict", "User already exists. "));
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
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                var emailSend = await _emailService.SendMailAsync(newUser.Id, newUser.Email, WebUtility.UrlEncode(code));

                if (!emailSend.Result)
                {
                    return Result<AuthResult>.Failure(Error.Failure("Failed to send Mail", $"Cannot send send mail to {emailSend.Message}"));
                }

                return Result<AuthResult>.Success(new AuthResult()
                {
                    Message = $" Registration successful , confirm your mail , just send to you {emailSend.Message} !!",
                    Result = true
                });
            }

            return Result<AuthResult>.Failure(Error.Validation("Error Registering User", isCreated!.Errors!.Select(a => a.Description)!.ToString()!));
        }

        public async Task<Result<AuthResult>> RegisterWithGoogle(string credentials)
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
                return Result<AuthResult>.Failure(Error.Failure("Invalid", "Error signing in with Google. "));
            }

            // then get the user from payload and check in db
            var existingUser = await _userManager.FindByEmailAsync(payload.Email!);
            if (existingUser is not null)
            {
                return Result<AuthResult>.Failure(Error.Conflict("Conflict", "User already exists, so try to log in with correct. "));
            }

            // then get the user from payload and create the user
            var newUser = new IdentityUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = payload.FamilyName + payload.GivenName,
                Email = payload.Email,
                EmailConfirmed = true // logging in with google so
            };

            // add the user
            var userAdded = await _userManager.CreateAsync(newUser);

            if (!userAdded.Succeeded)
            {
                return Result<AuthResult>.Failure(Error.Validation("Failed", "Error while signing in with Google. "));
            }

            //  also add login info with provider
            var loginInfo = new UserLoginInfo("GOOGLE", payload.Subject, "Google");

            var res = await _userManager.AddLoginAsync(newUser, loginInfo);

            if (!res.Succeeded)
            {
                return Result<AuthResult>.Failure(Error.Validation("Failed", "Error while signing in with Google. "));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (!result.Succeeded)
            {
                return Result<AuthResult>.Failure(Error.Validation("Failed", "Error while signing in with Google. "));
            }

            var emailSend = await _emailService.SendGreetingMailAsync(newUser.Email);
            // other wise generate token for the user and allow to log in
            // other wiser generate refresh token
            var newRefToken = GenerateJwtToken(newUser);

            // also add to the database in the refresh token table
            await _dbContext.RefreshTokens.AddAsync(newRefToken);
            await _dbContext.SaveChangesAsync();

            return Result<AuthResult>.Success(new AuthResult()
            {
                Message = "Registration with google log in successful and shipped also!! ",
                Result = true,
                UserId = newUser!.Id,
                UserName = newUser!.UserName
            });
        }

        public async Task<Result<AuthResult>> ConfirmUserEmail(ConfirmEmail confirmEmail)
        {
            // first check the user
            var user = await _userManager.FindByIdAsync(confirmEmail.UserId);
            if (user is null)
            {
                return Result<AuthResult>.Failure(Error.NotFound("Not Found", "Invalid User. "));
            }
            // if already confirmed email
            if (user.EmailConfirmed)
            {
                return Result<AuthResult>.Failure(Error.Conflict("Conflict", "User email confirmed already , please log in to continue !!"));
            }

            // verify from user manager
            var res = await _userManager.ConfirmEmailAsync(user, confirmEmail.ConfirmationCode);

            if (!res.Succeeded)
            {
                return Result<AuthResult>.Failure(Error.Failure("Failed to Confirm", "Please try again confirming or resend confirmation link. "));
            }

            var emailSend = await _emailService.SendGreetingMailAsync(user.Email!);

            return Result<AuthResult>.Success(new AuthResult()
            {
                Result = true,
                Message = "Your email has been verified successfully , please login now."
            });
        }

        public async Task<Result<AuthResult>> ResendConfirmationMail(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail!);

            // will send a verification code to that user s email if valid user, again
            if (user is null)
            {
                return Result<AuthResult>.Failure(Error.NotFound("Not Found", "Invalid User. "));
            }

            // if already confirmed email
            if (user.EmailConfirmed)
            {
                return Result<AuthResult>.Failure(Error.Conflict("Conflict", "User email confirmed already , please log in to continue !!"));
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user!);

            var emailSend = await _emailService.SendMailAsync(user.Id, user.Email!, WebUtility.UrlEncode(code));

            if (!emailSend.Result)
            {
                return Result<AuthResult>.Failure(Error.Failure("Failed to send mail", "Please try again sending link. "));
            }

            return Result<AuthResult>.Success(new AuthResult()
            {
                Result = true,
                Message = $"Confirmation mail sent again to your mail , just send to you. Please check your mail. {emailSend.Message}. "
            });
        }

        public async Task<Result<AuthResult>> ForgotPasswordMail(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail!);

            // will send a verification code to that user s email if valid user, again
            if (user is null)
            {
                return Result<AuthResult>.Failure(Error.NotFound("Not Found", "Invalid User. "));
            }

            // if user doesnt have confirmed email , cannot reset password
            if (!user.EmailConfirmed)
            {
                return Result<AuthResult>.Failure(Error.Validation("Invalid", "User email not confirmed yet. "));
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user!);

            var emailSend = await _emailService.SendPasswordResetMailAsync(user.Id, user.Email!, WebUtility.UrlEncode(code));

            if (!emailSend.Result)
            {
                return Result<AuthResult>.Failure(Error.Failure("Failed to send mail", "Please try again sending link. "));
            }

            return Result<AuthResult>.Success(new AuthResult()
            {
                Result = true,
                Message = $"Reset password mail sent to you. Please check your mail. {emailSend.Message}. "
            });
        }

        public async Task<Result<AuthResult>> ResetPassword(ResetPasswordDTO resetPassword)
        {
            // first check the user
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user is null)
            {
                return Result<AuthResult>.Failure(Error.NotFound("Not Found", "Invalid User. "));
            }
            // if not confirmed email
            if (!user.EmailConfirmed)
            {
                return Result<AuthResult>.Failure(Error.Validation("Invalid", "User email not confirmed yet. "));
            }

            // verify from user manager
            var res = await _userManager.ResetPasswordAsync(user, resetPassword.Code, resetPassword.NewPassword);

            if (!res.Succeeded)
            {
                return Result<AuthResult>.Failure(Error.Failure("Failed to reset password. ", "Please try again . "));
            }

            return Result<AuthResult>.Success(new AuthResult()
            {
                Result = true,
                Message = "Password has been reset successfully , please login now with new password. "
            });
        }

        public async Task<Result<AuthResult>> Login(UserLoginRequestDTO userRequestDTO)
        {
            var existingUser = await _userManager.FindByEmailAsync(userRequestDTO.Email);

            if (existingUser is null)
            {
                return Result<AuthResult>.Failure(Error.NotFound("Not Found", "Invalid User. "));
            }

            if (!existingUser.EmailConfirmed)
            {
                return Result<AuthResult>.Failure(Error.Validation("Invalid", "User email not confirmed yet. "));
            }


            // then check password user
            var passwordMatch = await _userManager.CheckPasswordAsync(existingUser, userRequestDTO.Password!);

            if (!passwordMatch)
            {
                return Result<AuthResult>.Failure(Error.Validation("Invalid", "Invalid User credentials, please try again with correct ones. "));
            }

            if (existingUser.TwoFactorEnabled)
            {
                // send 2 fa code to verify
                return Result<AuthResult>.Success(new AuthResult()
                {
                    UserId = existingUser.Id,
                    EmailConfirmed = existingUser.EmailConfirmed,
                    TwoFAEnabled = existingUser.TwoFactorEnabled,
                    Message = "Enter Two factor authentication code ..",
                    Result = true
                });
            }
            // sign in the user
            var useCookieScheme = true;
            var isPersistent = true;

            _signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

            var result = await _signInManager.PasswordSignInAsync(userRequestDTO.Email, userRequestDTO.Password, isPersistent, lockoutOnFailure: true); // this things need to check , why it is failing

            // other wiser generate refresh token
            var newRefToken = GenerateJwtToken(existingUser);

            // also add to the database in the refresh token table

            await _dbContext.RefreshTokens.AddAsync(newRefToken);
            await _dbContext.SaveChangesAsync();

            return Result<AuthResult>.Success(new AuthResult()
            {
                Message = "Login successful and shipped also!! ",
                Result = true,
                UserId = existingUser.Id,
                UserName = existingUser.UserName,
                EmailConfirmed = existingUser.EmailConfirmed,
                TwoFAEnabled = existingUser.TwoFactorEnabled
            });
        }

        public async Task<Result<AuthResult>> LoginWithGoogle(string credentials)
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
                return Result<AuthResult>.Failure(Error.NotFound("Not Found", "Invalid User. "));
            }

            // then get the user from payload and check in db
            var existingUser = await _userManager.FindByEmailAsync(payload.Email!);

            if (existingUser is null)
            {
                return Result<AuthResult>.Failure(Error.NotFound("Not Found", "Invalid User. "));
            }

            // other wise generate token for th euser and allow to log in
            // other wiser generate refresh token
            var newRefToken = GenerateJwtToken(existingUser);

            // also add to the database in the refresh token table
            await _dbContext.RefreshTokens.AddAsync(newRefToken);
            await _dbContext.SaveChangesAsync();

            return Result<AuthResult>.Success(new AuthResult()
            {
                Message = "Login successful with google. ",
                Result = true,
                UserId = existingUser.Id,
                UserName = existingUser.UserName,
                EmailConfirmed = existingUser.EmailConfirmed,
                TwoFAEnabled = existingUser.TwoFactorEnabled
            });
        }

        public async Task<Result<AuthResult>> VerifyAndLoginWith2FA(Login2FARequest login2FARequest)
        {
            var currUser = await _userManager.FindByIdAsync(login2FARequest.CurrUserId!);
            if (currUser is null)
            {
                return Result<AuthResult>.Failure(Error.NotFound("Not Found", "Invalid User. "));
            }

            bool validCode = await _userManager.VerifyTwoFactorTokenAsync(currUser!, _userManager.Options.Tokens.AuthenticatorTokenProvider, login2FARequest.TwoFACode!);

            if (!validCode)
            {
                return Result<AuthResult>.Failure(Error.Validation("Invalid Code", "Code is invalid. Try with correct code. "));
            }

            // other wise generate token for the user and allow to log in
            // other wiser generate refresh token
            var newRefToken = GenerateJwtToken(currUser);

            // also add to the database in the refresh token table
            await _dbContext.RefreshTokens.AddAsync(newRefToken);
            await _dbContext.SaveChangesAsync();

            return Result<AuthResult>.Success(new AuthResult()
            {
                Message = "Login successful with 2 Factor verification. ",
                Result = true,
                UserId = currUser.Id,
                UserName = currUser.UserName,
                EmailConfirmed = currUser.EmailConfirmed,
                TwoFAEnabled = currUser.TwoFactorEnabled
            });
        }

        public async Task<Result<AuthResult>> Verify2FA(string credentials)
        {
            var token = _httpContextAccessor.HttpContext!.Request.Cookies["x-access-token"];

            if (token.IsNullOrEmpty())
            {
                // then get try refreshing tokens if there refresh token exists
                var refToken = _httpContextAccessor.HttpContext!.Request.Cookies["x-refresh-token"];
                if (refToken.IsNullOrEmpty())
                {
                    return Result<AuthResult>.Failure(Error.Failure("Failed", "Login expired ,, Please log in again to continue .... "));
                }
                // other wise generate new token
                await GetRefreshToken();
                // try again 
                return Result<AuthResult>.Failure(Error.Failure("Failed", "Please try again. "));
            }
            // get user 
            var currUser = await GetUserFromJWT(token!);

            // Strip spaces and hyphens
            var verificationCode = credentials;

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(currUser, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                return Result<AuthResult>.Failure(Error.Validation("Invalid", "Invalid verification code. "));
            }

            await _userManager.SetTwoFactorEnabledAsync(currUser, true);

            return Result<AuthResult>.Success(new AuthResult()
            {
                Message = "2 Factor verification successful. ",
                Result = true,
                UserId = currUser.Id,
                UserName = currUser.UserName,
                EmailConfirmed = currUser.EmailConfirmed,
                TwoFAEnabled = currUser.TwoFactorEnabled
            });
        }

        public async Task<Result<AuthResult>> Disable2FA()
        {
            var acToken = _httpContextAccessor.HttpContext!.Request.Cookies["x-access-token"];

            if (acToken.IsNullOrEmpty())
            {
                // then get try refreshing tokens if there refresh token exists
                var refToken = _httpContextAccessor.HttpContext!.Request.Cookies["x-refresh-token"];
                if (refToken.IsNullOrEmpty())
                {
                    return Result<AuthResult>.Failure(Error.Failure("Failed", "Login expired ,, Please log in again to continue .... "));
                }
                // other wise generate new token
                await GetRefreshToken();
                // try again 
                await Disable2FA();
            }

            // get the user from jwt token
            var user = await GetUserFromJWT(acToken!);

            if (user == null)
            {
                return Result<AuthResult>.Failure(Error.NotFound("Not Found", "User doesn't exists. "));
            }
            // if already disabled
            if (!user.TwoFactorEnabled)
            {
                return Result<AuthResult>.Failure(Error.Conflict("Conflict", "Invalid, Two factor not enabled yet . "));
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);

            if (!disable2faResult.Succeeded)
            {
                return Result<AuthResult>.Failure(Error.Failure("Failed", "Unexpected error occurred disabling 2FA. "));
            }

            return Result<AuthResult>.Success(new AuthResult()
            {
                Message = "Two factor has been disabled. You can enable again when you setup an authenticator app. ",
                Result = true,
                UserId = user.Id,
                UserName = user.UserName,
                EmailConfirmed = user.EmailConfirmed,
                TwoFAEnabled = user.TwoFactorEnabled
            });
        }

        public async Task<Result<TwoFAResponse>> LoadSharedKeyAndQrCodeUriAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<TwoFAResponse>.Failure(Error.NotFound("Not Found", "User doesn't exists. "));
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

            return Result<TwoFAResponse>.Success(new TwoFAResponse()
            {
                QR = AuthenticatorUri,
                SharedKey = SharedKey,
                Result = true,
                Message = "Verify now the code from authenticator app !!"
            });
        }

        public async Task<Result<AuthResult>> SignOutUser()
        {
            await _signInManager.SignOutAsync();
            await RevokeToken();

            return Result<AuthResult>.Success(new AuthResult()
            {
                Message = "Signed out successfully. ",
                Result = true,
            });
        }

        public async Task<Result<AuthResult>> GetRefreshToken()
        {
            // get prev refresh token from request cookies
            var refreshToken = _httpContextAccessor.HttpContext!.Request.Cookies["x-refresh-token"];
            if (refreshToken.IsNullOrEmpty())
            {
                return Result<AuthResult>.Failure(Error.NotFound("Not Found", "Login expired , Please log out and then log in again to continue ... "));
            }

            // get the jwt token id linked with this refresh token
            var userId = await _dbContext.RefreshTokens.AsNoTracking().Where(a => a.Token == refreshToken).Select(b => b.UserId).FirstOrDefaultAsync();

            // get from database from refresh token table
            var user = await _userManager.Users.FirstOrDefaultAsync(a => a.Id == userId);

            if (user is null)
            {
                return Result<AuthResult>.Failure(Error.NotFound("Not Found", "User doesn't exists. "));
            }

            // other wiser generate refresh token
            var newRefToken = GenerateJwtToken(user);

            // also add to the database in the refresh token table
            await _dbContext.RefreshTokens.AddAsync(newRefToken);
            await _dbContext.SaveChangesAsync();

            return Result<AuthResult>.Success(new AuthResult()
            {
                Message = "Token refreshed successfully. ",
                Result = true,
                UserId = user.Id,
                UserName = user.UserName,
                EmailConfirmed = user.EmailConfirmed,
                TwoFAEnabled = user.TwoFactorEnabled
            });
        }

        public async Task<Result<AuthResult>> CheckCurrentUser()
        {
            var currUserRes = new AuthResult();
            var currToken = _httpContextAccessor.HttpContext!.Request.Cookies["x-access-token"];

            // but if there is no access token
            // then need to get first the refresh token and then genrate set new token and refresh token also
            if (currToken.IsNullOrEmpty())
            {
                var token = _httpContextAccessor.HttpContext!.Request.Cookies["x-refresh-token"];
                if (token.IsNullOrEmpty())
                {
                    return Result<AuthResult>.Failure(Error.Failure("Failed", "All session , token expired, please log again to continue. "));
                }

                var currUserId = await _dbContext.RefreshTokens.AsNoTracking().Where(a => a.Token == token).Select(b => b.UserId).FirstOrDefaultAsync();

                var user = await _userManager.FindByIdAsync(currUserId!);

                if (user is null)
                {
                    return Result<AuthResult>.Failure(Error.NotFound("Not Found", "User doesn't exists. "));
                }
                //  aslo set the access token
                // other wiser generate refresh token
                var newRefToken = GenerateJwtToken(user);

                // also add to the database in the refresh token table

                await _dbContext.RefreshTokens.AddAsync(newRefToken);
                await _dbContext.SaveChangesAsync();

                currUserRes.UserId = currUserId!;
                currUserRes.TwoFAEnabled = user!.TwoFactorEnabled;
                currUserRes.EmailConfirmed = user!.EmailConfirmed;
                currUserRes.UserName = user!.UserName;
            }
            else
            {
                // other wise give the user email
                var user = await GetUserFromJWT(currToken!);

                // after that also verifies from user manager
                var isvalidUser = await _userManager.FindByEmailAsync(user.Email!);


                if (isvalidUser is null)
                {
                    return Result<AuthResult>.Failure(Error.NotFound("Not Found", "User doesn't exists. "));
                }

                currUserRes.UserId = isvalidUser!.Id!;
                currUserRes.TwoFAEnabled = user.TwoFactorEnabled;
                currUserRes.EmailConfirmed = user!.EmailConfirmed;
                currUserRes.UserName = user!.UserName;

            }
            currUserRes.Result = true;

            return Result<AuthResult>.Success(currUserRes);
        }

        public async Task<Result<AuthResult>> RevokeToken()
        {
            // this is in scenarios when jwt token and refresh token both does'nt work
            // so this will be called from frontend to remove all token for that user 
            var token = _httpContextAccessor.HttpContext!.Request.Cookies["x-refresh-token"];

            if (token.IsNullOrEmpty())
            {
                // delete these also , for ensuring nothing left
                _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-access-token");
                _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-app-user");
                _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-user-name");
                _httpContextAccessor.HttpContext!.Response.Cookies.Delete("twofa-enable");

                return Result<AuthResult>.Failure(Error.Failure("Failed", "All session , token expired, please log again to continue. "));
            }
            // get the related user id first
            var currUsersToken = await _dbContext.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(a => a.Token == token);
            if (currUsersToken is null)
            {
                _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-access-token");
                _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-refresh-token");
                _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-app-user");
                _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-user-name");
                _httpContextAccessor.HttpContext!.Response.Cookies.Delete("twofa-enable");
                return Result<AuthResult>.Failure(Error.Failure("Failed", "Error occurred, login again to continue !. "));
            }
            // get prev refresh token from request and remove all refresh token related to this user
            var refTokens = await _dbContext.RefreshTokens.AsNoTracking().Where(a => a.UserId == currUsersToken!.UserId).ToListAsync();

            // set the refresh token as invalid
            _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-access-token");
            _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-refresh-token");
            _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-app-user");
            _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-user-name");
            _httpContextAccessor.HttpContext!.Response.Cookies.Delete("twofa-enable");

            // delete from database
            _dbContext.RefreshTokens.RemoveRange(refTokens!);
            await _dbContext.SaveChangesAsync();

            return Result<AuthResult>.Success(new AuthResult() { Message = "All refresh token revoked successfully !! ", Result = true });
        }

        public async Task<Result<AuthResult>> DeleteAllData()
        {
            var currToken = _httpContextAccessor.HttpContext!.Request.Cookies["x-access-token"];
            var currUser = new IdentityUser();
            // but if there is no access token
            // then need to get first the refresh token and then generate set new token and refresh token also
            if (currToken.IsNullOrEmpty())
            {
                var token = _httpContextAccessor.HttpContext!.Request.Cookies["x-refresh-token"];
                if (token.IsNullOrEmpty())
                {
                    return Result<AuthResult>.Failure(Error.Failure("Failed", "All session , token expired, please log again to continue. "));
                }

                var currUserId = await _dbContext.RefreshTokens.AsNoTracking().Where(a => a.Token == token).Select(b => b.UserId).FirstOrDefaultAsync();

                currUser = await _userManager.FindByIdAsync(currUserId!);

                if (currUser is null)
                {
                    return Result<AuthResult>.Failure(Error.NotFound("Not Found", "User doesn't exists. "));
                }

                // set the refresh token as invalid
                _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-refresh-token");
                _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-app-user");
                _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-user-name");
                _httpContextAccessor.HttpContext!.Response.Cookies.Delete("twofa-enable");
            }
            else
            {
                // other wise give the user email
                var user = await GetUserFromJWT(currToken!);

                if (user is null)
                {
                    return Result<AuthResult>.Failure(Error.NotFound("Not Found", "User doesn't exists. "));
                }

                // after that also verifies from user manager
                var isvalidUser = await _userManager.FindByEmailAsync(user.Email!);

                if (isvalidUser is null)
                {
                    return Result<AuthResult>.Failure(Error.NotFound("Not Found", "User doesn't exists. "));
                }

                currUser = isvalidUser;

                // set the access token as invalid
                _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-access-token");
                _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-refresh-token");
                _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-app-user");
                _httpContextAccessor.HttpContext!.Response.Cookies.Delete("x-user-name");
                _httpContextAccessor.HttpContext!.Response.Cookies.Delete("twofa-enable");
            }
            // delete all things 
            var result = await _userManager.DeleteAsync(currUser!);

            if (!result.Succeeded)
            {
                return Result<AuthResult>.Failure(Error.Failure("Failed", "Unexpected error occurred deleting user. Try again . "));
            }
            // get prev refresh token from request and remove all refresh token related to this user
            var refTokens = await _dbContext.RefreshTokens.AsNoTracking().Where(a => a.UserId == currUser!.Id).ToListAsync();
            if (refTokens is null)
            {
                return Result<AuthResult>.Failure(Error.Validation("Invalid", "Unexpected error occurred deleting user. Try again . "));
            }

            _dbContext.RefreshTokens.RemoveRange(refTokens!);

            await _dbContext.SaveChangesAsync();

            await _signInManager.SignOutAsync();

            return Result<AuthResult>.Success(new AuthResult() { Message = "User deleted themselves successfully !! ", Result = true });
        }

        #region All Utilities
        private RefreshToken GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenhandler = new JwtSecurityTokenHandler();

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
                Expires = DateTime.Now.ToLocalTime().AddHours(1), // need to be as general short like in min ,
                //when implement refresh token , change here
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512)
            };

            var token = jwtTokenhandler.CreateToken(tokenDescriptor);

            //var secu = tokenDescriptor;
            var encryptedToken = jwtTokenhandler.WriteToken(token);

            // attach to cookies
            // before attaching encrypt the token
            //SetJWTToken(encryptedToken);

            // this will set access token into cookies
            _httpContextAccessor.HttpContext!.Response.Cookies.Append("x-access-token", encryptedToken,
             new CookieOptions
             {
                 Expires = DateTime.Now.ToLocalTime().AddHours(1),
                 Secure = true,
                 HttpOnly = true,
                 IsEssential = true,
                 SameSite = SameSiteMode.Lax
             });


            //var refreshToken = GenerateRefreshToken();
            var refreshToken = new RefreshToken()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                UserId = user.Id,
                JWTId = token.Id, // attaching linked jwt token id
                IsUsed = true,
                IsRevoked = false,
                AddedDate = DateTime.Now.ToLocalTime(),
                ExpiryDate = DateTime.Now.ToLocalTime().AddDays(5)
            };

            //await SetRefreshToken(refreshToken, token.Id, user);
            // this will set refresh token into cookies
            _httpContextAccessor.HttpContext!.Response.Cookies.Append("x-refresh-token", refreshToken.Token!, new CookieOptions
            {
                Expires = refreshToken.ExpiryDate,
                Secure = true,
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax // none is not good 
            });

            return refreshToken;
        }

        #region Not needed currently
        private RefreshToken GenerateRefreshToken()
        {
            // this can be random string or another jwt token
            var refreshToken = new RefreshToken()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.Now.ToLocalTime().AddDays(5)
            };

            return refreshToken;
        }

        private void SetJWTToken(string encryptedToken)
        {
            // this will set refresh token into cookies
            _httpContextAccessor.HttpContext!.Response.Cookies.Append("x-access-token", encryptedToken,
             new CookieOptions
             {
                 Expires = DateTime.Now.ToLocalTime().AddHours(1),
                 Secure = true,
                 HttpOnly = true,
                 IsEssential = true,
                 SameSite = SameSiteMode.Lax
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
                SameSite = SameSiteMode.Lax // none is not good 
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
                ExpiryDate = DateTime.Now.ToLocalTime().AddDays(5)
            };
            try
            {
                await _dbContext.RefreshTokens.AddAsync(newRefreshToken);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var er = ex;
                return false;
            }

            return true;
        }

        #endregion

        private async Task<IdentityUser> GetUserFromJWT(string credential)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(credential);

            // after that also verifies from user manager
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
