using Microsoft.AspNetCore.Mvc;

namespace TCS_Cliente.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
