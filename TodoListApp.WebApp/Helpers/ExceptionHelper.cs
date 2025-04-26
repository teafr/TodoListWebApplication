namespace TodoListApp.WebApp.Helpers;

public static class ExceptionHelper
{
    public static void CheckViewModel<T>(T viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
    }
}
