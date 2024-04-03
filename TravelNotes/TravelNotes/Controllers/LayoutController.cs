using Microsoft.AspNetCore.Mvc;
using TravelNotes.Models;

namespace TravelNotes.Controllers
{
    public class LayoutController : Controller
    {
        private readonly TravelContext _context;
        public LayoutController(TravelContext context)
        {
            _context = context;
        }
        public string GetUserInformation()
        {
            string userId;
            if (!Request.Cookies.TryGetValue("UsernameCookie", out userId))
            {
                return "NotLogin";
            }
            users user = _context.users.FirstOrDefault(a=>a.UserId == Convert.ToInt32(userId))!;
            if(user.SuperUser != null)
            {
                return user.SuperUser.Trim();
            }
            else
            {
                return "N";
            }
        }
    }
}
