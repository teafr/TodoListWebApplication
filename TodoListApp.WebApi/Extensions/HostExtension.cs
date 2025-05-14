using System.Globalization;
using Serilog;

namespace TodoListApp.WebApi.Extensions;

public static class HostExtension
{
    public static void ConfigureHost(this IHostBuilder hostBuilder)
    {
        _ = hostBuilder.UseSerilog((context, loggerConfig) =>
        {
            _ = loggerConfig.WriteTo.Console(formatProvider: CultureInfo.InvariantCulture);
        });
    }
}
