using ManyInOneAPI.Models.Auth;
using ManyInOneAPI.Models.Payment;
using ManyInOneAPI.Models.Quizz;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ManyInOneAPI.Data
{
    public class ManyInOneDbContext : IdentityDbContext
    {
        public ManyInOneDbContext(DbContextOptions<ManyInOneDbContext> options) : base(options)
        {
                
        }

        // when working IdentityDbContext , this on nodek creating methods needs to be here to
        // correctly configure all key relation mapping to identity table.
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ManyInOneDbContext).Assembly);
        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PaymentDetail> PaymentDetails { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<OptionsWithAnswer> Options { get; set; }
    }
}
