using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelNotes.Controllers
{
    [Authorize]
    public class CalimsTest : Controller
    {
        [Authorize(Roles = "Y")]
        public IActionResult Apple()
        {
            return View();
        }

        [Authorize(Roles = "N")]
        public IActionResult Bee()
        {
            return View();
        }
    }
}
