using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
