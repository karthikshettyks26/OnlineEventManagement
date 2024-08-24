using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OnlineEventManagement.Data
{
    public class IdentityDbContextClass : IdentityDbContext
    {
        public IdentityDbContextClass(DbContextOptions<IdentityDbContextClass> options) : base(options)
        {
            
        }

        //Seed roles
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var particpantId = "e88ad7cf-da40-4a41-9c50-1faba05ee43a";
            var adminId = "ae3f62bd-8f07-4f8e-8470-a56f7f42748a";

            var roles = new List<IdentityRole>
            {
                new IdentityRole()
                {
                    Id = particpantId,
                    ConcurrencyStamp = particpantId,
                    Name = "Participant",
                    NormalizedName = "Participant".ToUpper()
                },
                new IdentityRole()
                {
                    Id = adminId,
                    ConcurrencyStamp = adminId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                }
            };

            //seed roles to database.
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
