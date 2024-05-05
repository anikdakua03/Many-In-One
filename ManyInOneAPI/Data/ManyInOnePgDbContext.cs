using ManyInOneAPI.Models.Auth;
using ManyInOneAPI.Models.Payment;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ManyInOneAPI.Data
{
    public class ManyInOnePgDbContext : IdentityDbContext
    {
        public ManyInOnePgDbContext(DbContextOptions<ManyInOnePgDbContext> options) : base(options)
        {
                
        }

        // when working IdentityDbContext , this on nodek creating methods needs to be here to
        // correctly configure all key relation mapping to identity table.
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // explicitely mentioned to use "varchar" for this , bcs causing issue while updating db after migartion
            builder.Entity<PaymentDetail>().Property(p => p.CardNumber).HasColumnType("varchar");
            base.OnModelCreating(builder);
        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PaymentDetail> PaymentDetails { get; set; }
    }
}
