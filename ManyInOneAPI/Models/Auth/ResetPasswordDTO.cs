using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ManyInOneAPI.Models.Auth
{
    public class ResetPasswordDTO
    {
        [EmailAddress]
        public required string Email { get; set; } = string.Empty;

        public required string Code { get; set; } = string.Empty;

        [MinLength(6), PasswordPropertyText]
        public required string NewPassword { get; set; } = string.Empty;

        [MinLength(6), PasswordPropertyText]
        [Compare("NewPassword", ErrorMessage = "Password doesn't match.")]
        public required string ConfirmPassword { get; set; } = string.Empty;
    }
}
