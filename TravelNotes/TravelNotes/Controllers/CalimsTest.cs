using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelNotes.Controllers
{
    //[Authorize]
    public class CalimsTest : Controller
    {
        //[Authorize(Roles = "Y")]
        public IActionResult Apple()
        {
            string userId;
            var test = Request.Cookies.TryGetValue("UsernameCookie", out userId);
            ViewBag.UserId = userId;
            return View();
        }

        [Authorize(Roles = "N")]
        public IActionResult Bee()
        {
            return View();
        }
    }
}
