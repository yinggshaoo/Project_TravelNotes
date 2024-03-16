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


        public IActionResult Login(string email, string password)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var user = _context.AspNetUsers.FirstOrDefault(x => x.Email == email);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "登入失敗 - 找不到使用者";
                    return RedirectToAction("Login");
                }

                var hashedPassword = ComputeSHA256Hash(password);
                if (hashedPassword == user.PasswordHash)
                {
                    return RedirectToAction("Index", "Home"); // 登入成功後導向首頁或其他目標頁面
                }
                else
                {
                    TempData["ErrorMessage"] = "登入失敗 - 密碼錯誤";
                    return RedirectToAction("Login");
                }
            }
                
            return View();
        }

        public IActionResult fail() 
        {
            ViewBag.Text = "失敗了!";
            return View();
        }


        public IActionResult Register(string email, string password, string confirmPassword)
		{
			if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(confirmPassword))
			{

                string hashedPassword = ComputeSHA256Hash(password);
                Random random = new Random();
                char randomLetter = (char)('A' + random.Next(26));
                string hashedId = ComputeSHA256Hash(password + randomLetter);


                var user = new AspNetUsers
                {
                    Id = hashedId,
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


                return RedirectToAction("Login");
            }
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



        
	}
}
