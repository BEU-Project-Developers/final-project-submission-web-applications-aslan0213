using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
