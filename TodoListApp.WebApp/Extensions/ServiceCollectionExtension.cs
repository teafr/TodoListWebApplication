using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoListApp.ApiClient.Extensions;
using TodoListApp.WebApp.Contexts;

namespace TodoListApp.WebApp.Extensions;

internal static class ServiceCollectionExtension
{
    public static IServiceCollection AddApiEndpoints(this IServiceCollection services)
    {
        _ = services.AddEndpointsApiExplorer();
        return services;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddDbContext<IdentityContext>(options =>
        {
            _ = options.UseSqlServer(configuration.GetConnectionString("TodoListDbConnection"));
        });

        _ = services.AddIdentityCore<IdentityUser>().AddEntityFrameworkStores<IdentityContext>().AddApiEndpoints();

        string apiBaseAddress = configuration["ApiBaseAddress"] ?? string.Empty;
        services.AddTodoListApiClientService(options => options.ApiBaseAdress = apiBaseAddress);
        services.AddTaskApiClientService(options => options.ApiBaseAdress = apiBaseAddress);

        return services;
    }

    //public static IServiceCollection AddDependensies(this IServiceCollection services)
    //{
    //    //_ = services.AddTransient<ITodoListService, TodoListService>();
    //    //_ = services.AddTransient<ITaskService, TaskService>();

    //    return services;
    //}
}
