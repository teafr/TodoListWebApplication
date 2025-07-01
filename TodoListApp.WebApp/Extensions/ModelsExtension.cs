using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApp.Helpers;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Models.ViewModels;
using TodoListApp.WebApp.Models.ViewModels.AuthenticationModels;

namespace TodoListApp.WebApp.Extensions;

public static class ModelsExtension
{
    public static TodoListApiModel ToTodoListApiModel(this TodoListModel todoList)
    {
        ExceptionHelper.CheckObjectForNull(todoList);
        return new TodoListApiModel
        {
            Id = todoList.Id,
            Title = todoList.Title,
            Description = todoList.Description,
            OwnerId = todoList.OwnerId,
            Editors = todoList.Editors ?? new List<string>(),
            Tasks = todoList.Tasks?.Select(task => task.ToTaskApiModel()).ToList() ?? new List<TaskApiModel>(),
        };
    }

    public static TaskApiModel ToTaskApiModel(this TaskModel task)
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
            TodoListId = task.TodoListId,
            AssigneeId = task.AssigneeId,
        };
    }

    public static StatusApiModel ToStatusApiModel(this StatusModel status)
    {
        ExceptionHelper.CheckObjectForNull(status);
        return new StatusApiModel
        {
            Id = status.Id,
            Name = status.Name,
        };
    }

    public static TodoListViewModel ToTodoListViewModel(this TodoListModel todoList, [FromServices] UserManager<ApplicationUser> userManager, int currentPage = 1)
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
            TasksList = new ListOfTasks(todoList.Tasks ?? new List<TaskModel>(), currentPage),
        };
    }

    public static TaskViewModel ToTaskViewModel(this TaskModel task, ApplicationUser? assignee = null)
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

    public static StatusViewModel ToStatusViewModel(this StatusModel status)
    {
        ExceptionHelper.CheckObjectForNull(status);
        return new StatusViewModel
        {
            Id = status.Id,
            Name = status.Name,
        };
    }
}
