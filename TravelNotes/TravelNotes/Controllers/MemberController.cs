using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using TravelNotes.Models;

namespace TravelNotes.Controllers
{
	public class MemberController : Controller
	{
        private readonly TravelContext _context;

        public MemberController(TravelContext context)
        {
            _context = context;
        }


        public IActionResult Index()
		{
			return View();
		}

		public IActionResult Register(string email, string password, string confirmPassword)
		{
			if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(confirmPassword))
			{

                string hashedPassword = ComputeSHA256Hash(password);
                ViewBag.Email = email;
				ViewBag.Password = hashedPassword;
				ViewBag.ConfirmPassword = confirmPassword;

                var user = new AspNetUsers
                {
                    Id = "default",
                    Email = email,
                    EmailConfirmed = false,
                    PasswordHash = hashedPassword, // 密碼應該是哈希後的值
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                    AccessFailedCount = 0
                };

                _context.AspNetUsers.Add(user);
                _context.SaveChanges();


                return RedirectToAction("Success", new { email = email, password = hashedPassword, confirmPassword = confirmPassword });
            }
			return View();
		}

		public IActionResult Success(string email, string hashedPassword, string confirmPassword) 
		{
            ViewBag.Email = email;
            ViewBag.Password = hashedPassword;
            ViewBag.ConfirmPassword = confirmPassword;
            return View();
		}


        private string ComputeSHA256Hash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }



        public IActionResult Login()
		{
			return View();
		}
	}
}
