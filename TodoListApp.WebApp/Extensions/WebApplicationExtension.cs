namespace TodoListApp.WebApp.Extensions;

public static class WebApplicationExtension
{
    public static WebApplication AddRoutes(this WebApplication app)
    {
        _ = app.MapControllerRoute(
            name: "search",
            pattern: "Tasks/SearchTasks/{property:alpha}",
            defaults: new { controller = "Tasks", action = "SearchTasks" });

        _ = app.MapControllerRoute(
            name: "taskEdit",
            pattern: "Tasks/Edit/{taskId:int}",
            defaults: new { controller = "Tasks", action = "Edit" });

        _ = app.MapControllerRoute(
            name: "taskCreate",
            pattern: "Tasks/AddTask/{todoListId:int?}",
            defaults: new { controller = "Tasks", action = "AddTask" });

        _ = app.MapControllerRoute(
            name: "taskDetails",
            pattern: "Tasks/Details/{taskId:int}",
            defaults: new { controller = "Tasks", action = "Details" });

        _ = app.MapControllerRoute(
            name: "sorting",
            pattern: "Tasks/SortBy{sortBy:alpha}/Page{currentPage:int}",
            defaults: new { controller = "Tasks", action = "Index", currentPage = 1, sortBy = "CreationDate" });

        _ = app.MapControllerRoute(
            name: "todoListTasksPagination",
            pattern: "TodoLists/{todoListId:int}/Tasks/Page{currentPage:int}",
            defaults: new { controller = "TodoLists", action = "GetTasks", currentPage = 1 });

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
