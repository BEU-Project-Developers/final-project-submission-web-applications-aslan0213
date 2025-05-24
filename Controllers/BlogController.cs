using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
