namespace TodoListApp.WebApp.Helpers;

public static class ExceptionHelper
{
    public static void CheckObjectForNull<T>(T viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
    }
}
