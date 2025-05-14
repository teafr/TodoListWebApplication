using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace TodoListApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    protected async Task<IActionResult> ExecuteWithValidation(Func<Task> action)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(action);
            await action();
            return this.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            Log.Warning(ex, "{Message}", new { ex.Message });
            return this.BadRequest();
        }
        catch (InvalidDataException ex)
        {
            Log.Warning(ex, "{Message}", new { ex.Message });
            return this.NotFound();
        }
    }
}
