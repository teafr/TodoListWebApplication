using TodoListApp.WebApi.Extensions;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.ConfigureHost();

        _ = builder.Services.AddControllers();
        _ = builder.Services.AddEndpointsApiExplorer();
        _ = builder.Services.AddSwaggerGen();

        _ = builder.Services.ConfigureServices(builder.Configuration).AddDependensies();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            _ = app.UseSwagger();
            _ = app.UseSwaggerUI();
        }

        _ = app.UseRouting();

        _ = app.UseAuthentication();
        _ = app.UseAuthorization();

        _ = app.UseHttpsRedirection();
        _ = app.MapControllers();

        SeedStatusData.EnsurePopulated(app);

        app.Run();
    }
}
