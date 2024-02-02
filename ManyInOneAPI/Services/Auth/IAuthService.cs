using ManyInOneAPI.Models.Auth;
using Microsoft.AspNetCore.Identity;

namespace ManyInOneAPI.Services.Auth
{
    public interface IAuthService
    {
        public Task<AuthResult> Register(UserRegistrationRequestDTO userRequestDTO);
        public Task<AuthResult> RegisterWithGoogle(string credentials);
        public Task<AuthResult> ConfirmUserEmail(string userId, string code);
        public Task<AuthResult> Login(UserLoginRequestDTO userRequestDTO);
        public Task<AuthResult> LoginWithGoogle(string credentials);
        public Task<AuthResult> VerifyAndLoginWith2FA(string credentials);
        public Task<AuthResult> Verify2FA(string credentials);
        public Task<AuthResult> Disable2FA();
        public Task<TwoFAResponse> LoadSharedKeyAndQrCodeUriAsync(string userId);
        public Task<AuthResult> SignOutUser();
        public Task<AuthResult> GetRefreshToken();
        public Task<AuthResult> CheckCurrentUser();
        public Task<AuthResult> RevokeToken();
        public Task<AuthResult> DeleteAllData();
    }
}
