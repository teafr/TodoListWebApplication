using Microsoft.AspNetCore.Identity;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApp.Helpers;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Models.ViewModels;

namespace TodoListApp.WebApp.Extensions;

public static class ModelsExtension
{
    public static TodoListApiModel ToTodoListApiModel(this TodoListModel todoList)
    {
        ExceptionHelper.CheckViewModel(todoList);
        return new TodoListApiModel
        {
            Id = todoList.Id,
            Title = todoList.Title,
            Description = todoList.Description,
            OwnerId = todoList.OwnerId,
            Tasks = todoList.Tasks?.Select(task => task.ToTaskApiModel()).ToList() ?? new List<TaskApiModel>(),
        };
    }

    public static TaskApiModel ToTaskApiModel(this TaskModel task)
    {
        ExceptionHelper.CheckViewModel(task);
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
        ExceptionHelper.CheckViewModel(status);
        return new StatusApiModel
        {
            Id = status.Id,
            Name = status.Name,
        };
    }

    public static TodoListViewModel ToTodoListViewModel(this TodoListModel todoList, int currentPage = 1)
    {
        ExceptionHelper.CheckViewModel(todoList);
        return new TodoListViewModel
        {
            Id = todoList.Id,
            Title = todoList.Title,
            Description = todoList.Description,
            OwnerId = todoList.OwnerId,
            TasksList = new ListOfTasks(todoList.Tasks ?? new List<TaskModel>(), currentPage),
        };
    }

    public static TaskViewModel ToTaskViewModel(this TaskModel task, IdentityUser? assignee = null)
    {
        ExceptionHelper.CheckViewModel(task);
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
        ExceptionHelper.CheckViewModel(status);
        return new StatusViewModel
        {
            Id = status.Id,
            Name = status.Name,
        };
    }
}
