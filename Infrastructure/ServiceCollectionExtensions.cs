using Application.Services.Employees;
using Application.Services.Identity;
using Infrastructure.Context;
using Infrastructure.Services.Employees;
using Infrastructure.Services.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => options
                    .UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<ApplicationDbSeeder>();

            return services;
        }

        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services
                .AddTransient<ITokenService, TokenService>()
                .AddTransient<IUserService, UserService>()
                .AddHttpContextAccessor()
                .AddTransient<ICurrentUserService, CurrentUserService>()
                .AddTransient<IRoleService, RoleService>();

            return services;
        }

        public static IServiceCollection AddEmployeeService(this IServiceCollection services)
        {
            services.AddTransient<IEmployeeService, EmployeeService>();

            return services;
        }

        public static void AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        } 
    }
}
