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

            var participantId = "c24092c6-9d1c-4717-a7dd-1171c3be4c19";
            var adminId = "8329416d-f8c6-482c-b886-7b8990eede18";

            var roles = new List<IdentityRole>
            {
                new IdentityRole()
                {
                    Id = participantId,
                    ConcurrencyStamp = participantId,
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
