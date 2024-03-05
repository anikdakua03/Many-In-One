using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ManyInOneAPI.Models.Auth
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }
        public string? UserId { get; set; } // user id
        public string? JWTId { get; set; } // linked jwt token's id jti
        public string? Token { get; set; } // refresh token
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
