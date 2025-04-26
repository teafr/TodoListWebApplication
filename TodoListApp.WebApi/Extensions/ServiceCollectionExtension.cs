using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TodoListApp.Database.Contexts;
using TodoListApp.Database.Entities;
using TodoListApp.Database.Repositories;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Extensions;

internal static class ServiceCollectionExtension
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddDbContext<TodoListDbContext>(options =>
        {
            _ = options.UseSqlServer(configuration.GetConnectionString("TodoListDbConnection"));
        });

        _ = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JwtBearer:Issuer"],
                ValidAudience = configuration["JwtBearer:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtBearer:SecurityKey"])),
            };
        });

        return services;
    }

    public static IServiceCollection AddDependensies(this IServiceCollection services)
    {
        _ = services.AddScoped<IRepository<TaskEntity>, TaskRepository>();
        _ = services.AddScoped<IRepository<TodoListEntity>, TodoListRepository>();
        _ = services.AddScoped<ITodoListService, TodoListService>();
        _ = services.AddScoped<ITaskService, TaskService>();

        return services;
    }
}
