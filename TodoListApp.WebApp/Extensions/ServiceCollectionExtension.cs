using Microsoft.AspNetCore.Identity;
using TodoListApp.ApiClient.Services;
using TodoListApp.WebApp.Contexts;
using TodoListApp.WebApp.Handler;
using TodoListApp.WebApp.Helpers;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Extensions;

internal static class ServiceCollectionExtension
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        string baseUrl = configuration["WebApiSettings:BaseUrl"] ?? string.Empty;

        _ = services.AddHttpContextAccessor();
        _ = services.AddTransient<AuthHeaderHandler>();

        _ = services.AddHttpClient<TaskApiClientService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        }).AddHttpMessageHandler<AuthHeaderHandler>();

        _ = services.AddHttpClient<TodoListApiClientService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        }).AddHttpMessageHandler<AuthHeaderHandler>();

        return services;
    }

    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        _ = services.AddDbContext<IdentityContext>();
        _ = services.AddScoped<ITodoListWebApiService, TodoListWebApiService>();
        _ = services.AddScoped<ITaskWebApiService, TaskWebApiService>();
        _ = services.AddScoped<JwtTokenGenerator>();

        return services;
    }

    public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {
        _ = services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<IdentityContext>()
        .AddDefaultTokenProviders();

        return services;
    }
}
