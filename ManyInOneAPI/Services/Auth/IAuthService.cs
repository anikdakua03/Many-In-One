using ManyInOneAPI.Models.Auth;

namespace ManyInOneAPI.Services.Auth
{
    public interface IAuthService
    {
        public Task<AuthResult> Register(UserRegistrationRequestDTO userRequestDTO);

        public Task<AuthResult> RegisterWithGoogle(string credentials);

        public Task<AuthResult> Login(UserLoginRequestDTO userRequestDTO);

        public Task<AuthResult> LoginWithGoogle(string credentials);

        public Task<AuthResult> GetRefreshToken();


        public Task<AuthResult> RevokeToken();
    }
}
