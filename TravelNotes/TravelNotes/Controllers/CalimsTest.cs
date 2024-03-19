using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelNotes.Controllers
{
    [Authorize]
    public class CalimsTest : Controller
    {
        [Authorize(Roles = "Admin")]
        public IActionResult Apple()
        {
            return View();
        }

        [Authorize(Roles = "Normal")]
        public IActionResult Bee()
        {
            return View();
        }
    }
}
