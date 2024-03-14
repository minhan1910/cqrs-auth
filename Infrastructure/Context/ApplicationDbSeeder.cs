using Common.Authorisation;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public class ApplicationDbSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public ApplicationDbSeeder(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task SeedDatabaseAsync()
        {
            await CheckAndApplyPendingMigrationAsync();

            // Seed roles
            await SeedRolesAsync();

            // Seed Users
            await SeedAdminUsersAsync();
        }

        public async Task CheckAndApplyPendingMigrationAsync()
        {
            if (_context.Database.GetAppliedMigrations().Any())
            {
                await _context.Database.MigrateAsync();
            }
        }

        public async Task SeedRolesAsync()
        {
            foreach (var roleName in AppRoles.DefaultRoles)
            {
                if (await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == roleName) is not ApplicationRole role)
                {
                    role = new ApplicationRole
                    {
                        Name = roleName,
                        Description = $"{roleName} Role"
                    };

                    await _roleManager.CreateAsync(role);
                }

                // Assign Permissions
                if (roleName == AppRoles.Admin)
                {
                    // Admin
                    await AssignPermissionsToRoleAsync(role, AppPermissions.AdminPermissions);
                } 
                else if (roleName == AppRoles.Basic)
                {
                    // Basic
                    await AssignPermissionsToRoleAsync(role, AppPermissions.BasicPermissions);
                }
            }
        }

        // say claims => permissions
        public async Task AssignPermissionsToRoleAsync(ApplicationRole role, IReadOnlyList<AppPermission> permisisons)
        {
            var currentClaims = await _roleManager.GetClaimsAsync(role);

            foreach (var permission in permisisons)
            {
                if (!currentClaims.Any(claim => claim.Type == AppClaim.Permission && claim.Value == permission.Name))
                {
                    await _context.RoleClaims.AddAsync(new ApplicationRoleClaim
                    {
                        RoleId = role.Id,
                        ClaimType = AppClaim.Permission,
                        ClaimValue = permission.Name,
                        Descrption = permission.Description,
                        Group = permission.Group,
                    });

                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task SeedAdminUsersAsync()
        {
            var userFromDb = await _userManager.FindByEmailAsync(AppCredentials.Email);

            //if (userFromDb != null)
            //{
            //    await _userManager.DeleteAsync(userFromDb);
            //}

            if (userFromDb == null)
            {
                //var adminUsername = AppCredentials.Email.Split('@')[0];
                var adminUsername = AppCredentials.Email[..AppCredentials.Email.IndexOf('@')].ToLowerInvariant();

                var adminUser = new ApplicationUser
                {
                    Email = AppCredentials.Email,
                    UserName = adminUsername,
                    FirstName = "An",
                    LastName = "Minh",
                    NormalizedUserName = adminUsername.ToUpperInvariant(),
                    NormalizedEmail = AppCredentials.Email.ToUpperInvariant(),
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                };
                
                adminUser.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(adminUser, AppCredentials.DefaultPassword);

                // create user
                var resultOfUserCreated = await _userManager.CreateAsync(adminUser);

                // assign role to user (admin)
                if (resultOfUserCreated.Succeeded)
                {
                    if (!await _userManager.IsInRoleAsync(adminUser, AppRoles.Basic) &&
                        !await _userManager.IsInRoleAsync(adminUser, AppRoles.Admin))
                    {
                        await _userManager.AddToRolesAsync(adminUser, AppRoles.DefaultRoles);
                    }
                }
            }
        }
    }
}