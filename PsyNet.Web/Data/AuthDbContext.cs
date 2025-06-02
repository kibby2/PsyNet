using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PsyNet.Web.Data
{
    public class AuthDbContext: IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // Seed Roles (User, Admin, SuperAdmin)

            var adminRoleId = "37cc67e1-41ca-461c-bf34-2b5e62dbae32";
            var superAdminRoleId = "3cfd9eee-08cb-4da3-9e6f-c3166b50d3b0";
            var userRoleId = "a0cab2c3-6558-4a1c-be81-dfb39180da3d";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name= "Admin",
                    NormalizedName = "ADMIN",
                    Id = adminRoleId,
                    ConcurrencyStamp = adminRoleId
                },
                new IdentityRole
                {
                    Name = "SuperAdmin",
                    NormalizedName = "SUPERADMIN",
                    Id = superAdminRoleId,
                    ConcurrencyStamp = superAdminRoleId
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER",
                    Id = userRoleId,
                    ConcurrencyStamp = userRoleId
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);

            // Seed SuperAdminUser
            var superAdminId = "472ba632-6133-44a1-b158-6c10bd7d850d";
            var superAdminUser = new IdentityUser
            {
                UserName = "superadmin",
                Email = "superadmin@psynet.com",
                NormalizedEmail = "superadmin@psynet.com".ToUpper(),
                NormalizedUserName = "superadmin".ToUpper(),
                Id = superAdminId,
                PasswordHash = "AQAAAAEAACcQAAAAEMa8/m6Ay8/LDOECUHmozo3iQ8D4uEEnL3GOeev6SH7/by8oEBYnOUOO9DFzDnOPeQ==",// Precomputed hash for "123456"
                ConcurrencyStamp = "7717894b-32a8-4e55-9b32-97771559b802", // Hardcoded value
                SecurityStamp = "51ada72a-ecde-47d9-8eae-3df46910689d" // Hardcoded value
            };


            builder.Entity<IdentityUser>().HasData(superAdminUser);


            // Add All roles to SuperAdminUser
            var superAdminRoles = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = superAdminId
                },
                new IdentityUserRole<string>
                {
                    RoleId = superAdminRoleId,
                    UserId = superAdminId
                },
                new IdentityUserRole<string>
                {
                    RoleId = userRoleId,
                    UserId = superAdminId
                }
            };

            builder.Entity<IdentityUserRole<string>>().HasData(superAdminRoles);

        }
    }
}
