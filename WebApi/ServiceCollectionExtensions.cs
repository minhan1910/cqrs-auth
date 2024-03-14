using Application.AppConfiguration;
using Common.Authorisation;
using Common.Responses.Wrappers;
using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using WebApi.Permissions;

namespace WebApi
{
    public static class ServiceCollectionExtensions
    {
        public static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var seeder = serviceScope.ServiceProvider.GetService<ApplicationDbSeeder>();

            seeder.SeedDatabaseAsync().GetAwaiter().GetResult();

            return app;
        }

        public static IServiceCollection AddIdentitySetting(this IServiceCollection services) 
        {
            services
                .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
                .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>() // each feature has own permission
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;

                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, AppConfiguration config)
        {
            var key = Encoding.ASCII.GetBytes(config.Secret);

            services
                .AddAuthentication(authenticationOptions =>
                {
                    authenticationOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authenticationOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(bearer =>
                {
                    //bearer.IncludeErrorDetails = true; -> assign details error for www-authenticate header
                    bearer.RequireHttpsMetadata = false;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RoleClaimType = ClaimTypes.Role,
                        // token must not be expired
                        ClockSkew = TimeSpan.Zero,
                    };

                    bearer.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = authenticationFailedContext =>
                        {
                            string result = string.Empty;
                            
                            authenticationFailedContext.Response.ContentType = MediaTypeNames.Application.Json;                            

                            if (authenticationFailedContext.Exception is SecurityTokenExpiredException)
                            {
                                authenticationFailedContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                result = JsonConvert.SerializeObject(ResponseWrapper.Fail("The token is expired."));
                            } else
                            {
                                authenticationFailedContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                result = JsonConvert.SerializeObject(ResponseWrapper.Fail("An unhandled error has occured."));
                            }
                            
                            return authenticationFailedContext.Response.WriteAsync(result);
                        },

                        OnChallenge = jwtBearerChallengeContext =>
                        {
                            jwtBearerChallengeContext.HandleResponse();

                            if (!jwtBearerChallengeContext.Response.HasStarted)
                            {
                                jwtBearerChallengeContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                jwtBearerChallengeContext.Response.ContentType = MediaTypeNames.Application.Json;
                                var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized."));
                                return jwtBearerChallengeContext.Response.WriteAsync(result);
                            }

                            return Task.CompletedTask;
                        },

                        OnForbidden = forbiddenContext =>
                        {
                            forbiddenContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            forbiddenContext.Response.ContentType = MediaTypeNames.Application.Json;
                            var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized to access this resource."));
                            return forbiddenContext.Response.WriteAsync(result);
                        }
                    };
                });

            services.AddAuthorization(options =>
            {
                // Empty so we need to analysze it
                IEnumerable<FieldInfo> props = typeof(AppPermissions).GetNestedTypes().SelectMany(c => c.GetFields(
                                    BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy));

                foreach (var prop in props)
                {
                    var propertyValue = prop.GetValue(null);

                    if (propertyValue is not null)
                    {
                        options.AddPolicy(propertyValue.ToString(),
                                          policy => policy.RequireClaim(AppClaim.Permission, propertyValue.ToString()));
                    }
                }
            });

            return services;
        }

        public static AppConfiguration GetApplicationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettingConfiguration = configuration.GetSection(nameof(AppConfiguration));
            
            services.Configure<AppConfiguration>(appSettingConfiguration);              

            return appSettingConfiguration.Get<AppConfiguration>();
        }

        public static void RegisterSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Input your Bearer Token in this format - Bearer {your token here} to access this API"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                 Type = ReferenceType.SecurityScheme,
                                 Id = "Bearer"
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "AdvanceAuth API",
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });
            });
        }
    }
}
