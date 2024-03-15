using Microsoft.AspNetCore.Mvc;

namespace TravelNotes.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
