using Application;
using Infrastructure;
using WebApi;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Infrastructure
// Database
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddEmployeeService();
builder.Services.AddInfrastructureDependencies();

// Identity
builder.Services.AddIdentitySetting();
var applicationSettings = builder.Services.GetApplicationSettings(builder.Configuration);
builder.Services.AddJwtAuthentication(applicationSettings);
builder.Services.AddIdentityServices();

// Application Service layer
builder.Services.AddApplicationServices();


builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.RegisterSwagger();

var app = builder.Build();

app.SeedDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.Run();
