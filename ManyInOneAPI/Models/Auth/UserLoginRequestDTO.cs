using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ManyInOneAPI.Models.Auth
{
    public class UserLoginRequestDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6), PasswordPropertyText]
        public string Password { get; set; } = string.Empty;
    }
}
