using ManyInOneAPI.Infrastructure.Shared;
using ManyInOneAPI.Models.Auth;

namespace ManyInOneAPI.Services.Auth
{
    public interface IAuthService
    {
        public Task<Result<AuthResult>> Register(UserRegistrationRequestDTO userRequestDTO);
        public Task<Result<AuthResult>> RegisterWithGoogle(string credentials);
        public Task<Result<AuthResult>> ConfirmUserEmail(ConfirmEmail confirmEmail);
        public Task<Result<AuthResult>> ResendConfirmationMail(string userEmail);
        public Task<Result<AuthResult>> ForgotPasswordMail(string userEmail);
        public Task<Result<AuthResult>> ResetPassword(ResetPasswordDTO resetPasswordDTO);
        public Task<Result<AuthResult>> Login(UserLoginRequestDTO userRequestDTO);
        public Task<Result<AuthResult>> LoginWithGoogle(string credentials);
        public Task<Result<AuthResult>> VerifyAndLoginWith2FA(Login2FARequest login2FARequest);
        public Task<Result<AuthResult>> Verify2FA(string credentials);
        public Task<Result<AuthResult>> Disable2FA();
        public Task<Result<TwoFAResponse>> LoadSharedKeyAndQrCodeUriAsync(string userId);
        public Task<Result<AuthResult>> SignOutUser();
        public Task<Result<AuthResult>> GetRefreshToken();
        public Task<Result<AuthResult>> CheckCurrentUser();
        public Task<Result<AuthResult>> RevokeToken();
        public Task<Result<AuthResult>> DeleteAllData();
    }
}
