using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ManyInOneAPI.Models.Auth
{
    public class UserRegistrationRequestDTO
    {
        [Required, MinLength(3)]
        public string Name { get; set; } = String.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = String.Empty;

        [Required, MinLength(6), PasswordPropertyText]
        public string Password { get; set; } = String.Empty;
    }
}
