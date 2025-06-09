namespace TodoListApp.WebApp.Extensions;

public static class WebApplicationExtension
{
    public static WebApplication AddRoutes(this WebApplication app)
    {
        _ = app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Tasks}/{action=Index}/{taskId:int?}",
            defaults: new { controller = "Tasks", action = "Index" });

        _ = app.MapControllerRoute(
            "error",
            "Error",
            new { Controller = "Home", action = "Error" });

        return app;
    }
}
