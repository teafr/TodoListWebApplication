using TodoListApp.WebApi.Models;
using TodoListApp.WebApp.Models.ViewModels;

namespace TodoListApp.WebApp.Models;

public class StatusModel
{
    public StatusModel(StatusApiModel statusApiModel)
    {
        ArgumentNullException.ThrowIfNull(statusApiModel);

        this.Id = statusApiModel.Id;
        this.Name = statusApiModel.Name ?? string.Empty;
    }

    public StatusModel(StatusViewModel statusViewModel)
    {
        ArgumentNullException.ThrowIfNull(statusViewModel);

        this.Id = statusViewModel.Id;
        this.Name = statusViewModel.Name ?? string.Empty;
    }

    public int Id { get; set; }

    public string Name { get; set; }
}
