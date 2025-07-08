using Microsoft.AspNetCore.Identity;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApp.Helpers;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Models.AuthenticationModels;

namespace TodoListApp.WebApp.Extensions;

public static class ModelsExtension
{
    public static TodoListViewModel ToTodoListViewModel(this TodoListApiModel todoList, UserManager<ApplicationUser> userManager, int currentPage = 1)
    {
        ExceptionHelper.CheckObjectForNull(todoList);
        ExceptionHelper.CheckObjectForNull(userManager);

        return new TodoListViewModel
        {
            Id = todoList.Id,
            Title = todoList.Title,
            Description = todoList.Description,
            Owner = userManager.FindByIdAsync(todoList.OwnerId).Result,
            Editors = todoList.Editors?.Select(editorId => userManager.FindByIdAsync(editorId).Result).ToList() ?? new List<ApplicationUser>(),
            TasksList = new TodoListTasks(todoList, currentPage),
        };
    }

    public static TaskViewModel ToTaskViewModel(this TaskApiModel task, ApplicationUser? assignee = null)
    {
        ExceptionHelper.CheckObjectForNull(task);

        return new TaskViewModel
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            CreationDate = task.CreationDate,
            DueDate = task.DueDate,
            Tags = task.Tags ?? new List<string>(),
            Comments = task.Comments ?? new List<string>(),
            Status = task.Status.ToStatusViewModel(),
            Assignee = assignee,
            TodoListId = task.TodoListId,
        };
    }

    public static StatusViewModel ToStatusViewModel(this StatusApiModel status)
    {
        ExceptionHelper.CheckObjectForNull(status);

        return new StatusViewModel
        {
            Id = status.Id,
            Name = status.Name,
        };
    }

    public static TodoListApiModel ToTodoListApiModel(this TodoListViewModel todoList)
    {
        ExceptionHelper.CheckObjectForNull(todoList);
        return new TodoListApiModel
        {
            Id = todoList.Id,
            Title = todoList.Title,
            Description = todoList.Description,
            OwnerId = todoList.Owner?.Id ?? string.Empty,
            Editors = todoList.Editors?.Select(editor => editor.Id).ToList() ?? new List<string>(),
            Tasks = todoList.TasksList?.Tasks.Select(task => task.ToTaskApiModel()).ToList() ?? new List<TaskApiModel>(),
        };
    }

    public static TaskApiModel ToTaskApiModel(this TaskViewModel task)
    {
        ExceptionHelper.CheckObjectForNull(task);
        return new TaskApiModel
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            CreationDate = task.CreationDate,
            DueDate = task.DueDate,
            Tags = task.Tags ?? new List<string>(),
            Comments = task.Comments ?? new List<string>(),
            Status = task.Status.ToStatusApiModel(),
            AssigneeId = task.Assignee?.Id,
            TodoListId = task.TodoListId,
        };
    }

    public static StatusApiModel ToStatusApiModel(this StatusViewModel status)
    {
        ExceptionHelper.CheckObjectForNull(status);
        return new StatusApiModel
        {
            Id = status.Id,
            Name = status.Name,
        };
    }
}
