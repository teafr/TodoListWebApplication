using TodoListApp.WebApi.Extensions;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        _ = builder.Services.AddControllers();
        _ = builder.Services.AddApiEndpoints().ConfigureServices(builder.Configuration).AddDependensies();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            _ = app.UseSwagger();
            _ = app.UseSwaggerUI();
        }

        _ = app.UseHttpsRedirection();
        _ = app.UseAuthorization();
        _ = app.MapControllers();

        SeedStatusData.EnsurePopulated(app);

        app.Run();
    }
}
