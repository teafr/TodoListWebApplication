using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using TodoListApp.ApiClient.Services;
using TodoListApp.WebApp.Contexts;
using TodoListApp.WebApp.Handlers;
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

        _ = services.Configure<MessageSenderOptions>(configuration.GetSection("Email"));
        _ = services.AddScoped<IEmailSender, EmailSender>();

        return services;
    }

    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        _ = services.AddDbContext<IdentityContext>();
        _ = services.AddScoped<ITodoListWebApiService, TodoListWebApiService>();
        _ = services.AddScoped<ITaskWebApiService, TaskWebApiService>();
        _ = services.AddScoped<ITokenGenerator, JwtTokenGenerator>();

        return services;
    }

    public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {
        _ = services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<IdentityContext>()
        .AddDefaultTokenProviders();

        _ = services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromHours(2);
        });

        return services;
    }
}
