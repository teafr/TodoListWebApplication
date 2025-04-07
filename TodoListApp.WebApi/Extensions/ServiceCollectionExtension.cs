using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Contexts;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Repositories;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Extensions;

internal static class ServiceCollectionExtension
{
    public static IServiceCollection AddApiEndpoints(this IServiceCollection services)
    {
        _ = services.AddEndpointsApiExplorer();
        _ = services.AddSwaggerGen();
        return services;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddDbContext<TodoListDbContext>(options =>
        {
            _ = options.UseSqlServer(configuration.GetConnectionString("TodoListDbConnection"));
        });

        return services;
    }

    public static IServiceCollection AddDependensies(this IServiceCollection services)
    {
        _ = services.AddTransient<IRepository<StatusEntity>, StatusRepository>();
        _ = services.AddTransient<IRepository<TaskEntity>, TaskRepository>();
        _ = services.AddTransient<IRepository<TodoListEntity>, TodoListRepository>();
        _ = services.AddTransient<ITodoListService, TodoListService>();
        _ = services.AddTransient<ITaskService, TaskService>();

        return services;
    }
}
