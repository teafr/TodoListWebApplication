using TodoListApp.WebApp.Extensions;

namespace TodoListApp.WebApp;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        _ = builder.Services.AddControllersWithViews();
        _ = builder.Services.ConfigureIdentity().ConfigureServices(builder.Configuration).AddDependencies();

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
