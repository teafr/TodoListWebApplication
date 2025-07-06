using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Extensions;

namespace TodoListApp.WebApp;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.ConfigureHost();

        _ = builder.Services.AddControllersWithViews(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
        _ = builder.Services.ConfigureIdentity().ConfigureServices(builder.Configuration).AddDependencies(builder.Configuration);

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            _ = app.UseExceptionHandler("/Home/Error");
            _ = app.UseHsts();
        }

        _ = app.UseHttpsRedirection();
        _ = app.UseStaticFiles();

        _ = app.UseRouting();

        _ = app.UseAuthentication();
        _ = app.UseAuthorization();

        _ = app.AddRoutes();

        app.Run();
    }
}
