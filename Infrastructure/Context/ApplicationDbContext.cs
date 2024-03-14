using Domain;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string,
                                                          IdentityUserClaim<string>, IdentityUserRole<string>,
                                                          IdentityUserLogin<string>, ApplicationRoleClaim, 
                                                          IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // must be first line
            base.OnModelCreating(builder);

            SetDecimalColumnType(builder);

            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

        private void SetDecimalColumnType(ModelBuilder builder)
        {
            var decimalPropertyOfEntities =
              builder
              .Model
              .GetEntityTypes()
              .SelectMany(t => t.GetProperties())
              .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?));

            foreach (var property in decimalPropertyOfEntities)
            {
                property.SetColumnType("decimal(18,2)");
            }
        }

        public DbSet<Employee> Employees => Set<Employee>();
    }
}