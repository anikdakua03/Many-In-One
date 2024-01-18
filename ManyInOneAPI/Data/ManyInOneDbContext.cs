using ManyInOneAPI.Models.Auth;
using ManyInOneAPI.Models.ImageFile;
using ManyInOneAPI.Models.Payment;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ManyInOneAPI.Data
{
    public class ManyInOneDbContext : IdentityDbContext
    {
        public ManyInOneDbContext(DbContextOptions<ManyInOneDbContext> options) : base(options)
        {
                
        }        

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<PaymentDetail> PaymentDetails { get; set; }
    }
}
