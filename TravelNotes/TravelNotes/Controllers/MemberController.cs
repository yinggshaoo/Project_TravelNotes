using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using TravelNotes.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

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


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var user = await _context.AspNetUsers.FirstOrDefaultAsync(x => x.Email == email);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "登入失敗 - 找不到使用者";
                    return RedirectToAction("Login");
                }
                else
                {
                    var hashedPassword = ComputeSHA256Hash(password);
                    if (hashedPassword == user.PasswordHash)
                    {

                        var roles = user.Role.ToString().Trim();
                        // 這裡討論完寫 權限不足要導向哪裡
                        //if(roles == "Admin")
                        //if (roles =="Normal")
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, "username"),
                            new Claim(ClaimTypes.Role, roles)
						};

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                        //這裡撈大頭貼資料 顯示在首頁
                        TempData["Welcome"] = user.Email;
                        return RedirectToAction("Bee", "CalimsTest");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "登入失敗 - 密碼錯誤";
                        return RedirectToAction("Login");
                    }
                }
            }

            return View();
        }



        public async Task<IActionResult> Logout()
        {
            // 執行登出操作，清除用戶身份驗證信息
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // 返回到首頁或其他適當的頁面
            return RedirectToAction("Login");
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
                    AccessFailedCount = 0,
                    Role = "Normal"
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
