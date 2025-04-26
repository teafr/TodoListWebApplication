using Microsoft.AspNetCore.Mvc;

namespace TodoListApp.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Error()
        {
            return this.View();
        }
    }
}
