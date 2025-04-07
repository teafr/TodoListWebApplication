using System.Collections.ObjectModel;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Extensions;

public static class ModelsExtension
{
    public static TodoListApiModel ToTodoListApiModel(this TodoList todoList)
    {
        ArgumentNullException.ThrowIfNull(todoList);

        TodoListApiModel todoListApiModel = new TodoListApiModel
        {
            Id = todoList.Id,
            Title = todoList.Title,
            Description = todoList.Description,
            OwnerId = todoList.OwnerId,
            Tasks = new Collection<TaskApiModel>(),
        };

        if (todoList.Tasks is not null)
        {
            foreach (var task in todoList.Tasks)
            {
                todoListApiModel.Tasks.Add(task.ToTaskApiModel());
            }
        }

        return todoListApiModel;
    }

    public static TaskApiModel ToTaskApiModel(this Models.Task task)
    {
        ArgumentNullException.ThrowIfNull(task);

        return new TaskApiModel
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            CreationDate = task.CreationDate,
            DueDate = task.DueDate,
            Tags = task.Tags ?? new Collection<string>(),
            Comments = task.Comments ?? new Collection<string>(),
            Status = task.Status.ToStatusApiModel(),
            AssigneeId = task.AssigneeId,
        };
    }

    public static StatusApiModel ToStatusApiModel(this Status status)
    {
        ArgumentNullException.ThrowIfNull(status);

        return new StatusApiModel
        {
            Id = status.Id,
            Name = status.Name,
        };
    }

    public static TodoListEntity ToTodoListEntity(this TodoList todoList)
    {
        ArgumentNullException.ThrowIfNull(todoList);

        TodoListEntity todoListEntity = new TodoListEntity
        {
            Id = todoList.Id,
            Title = todoList.Title,
            Description = todoList.Description,
            OwnerId = todoList.OwnerId,
            Tasks = new Collection<TaskEntity>(),
        };

        if (todoList.Tasks is not null)
        {
            foreach (var task in todoList.Tasks)
            {
                todoListEntity.Tasks.Add(task.ToTaskEntity());
            }
        }

        return todoListEntity;
    }

    public static TaskEntity ToTaskEntity(this Models.Task task)
    {
        ArgumentNullException.ThrowIfNull(task);

        return new TaskEntity
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            TodoListId = task.TodoListId,
            Tags = task.Tags,
            Comments = task.Comments,
            StatusId = task.Status.Id,
            CreationDate = task.CreationDate,
            AssigneeId = task.AssigneeId,
        };
    }
}
