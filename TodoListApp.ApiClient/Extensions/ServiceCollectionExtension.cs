using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TodoListApp.ApiClient.Models;

namespace TodoListApp.ApiClient.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddTodoListApiClientService(this IServiceCollection services, Action<ApiClientOptions> options)
        {
            ArgumentNullException.ThrowIfNull(options);
            _ = services.Configure(options);

            _ = services.AddSingleton(provider =>
            {
                var options = provider.GetRequiredService<IOptions<ApiClientOptions>>().Value;
                return new TodoListApiClientService(options);
            });
        }

        public static void AddTaskApiClientService(this IServiceCollection services, Action<ApiClientOptions> options)
        {
            ArgumentNullException.ThrowIfNull(options);
            _ = services.Configure(options);

            _ = services.AddSingleton(provider =>
            {
                var options = provider.GetRequiredService<IOptions<ApiClientOptions>>().Value;
                return new TaskApiClientService(options);
            });
        }
    }
}
