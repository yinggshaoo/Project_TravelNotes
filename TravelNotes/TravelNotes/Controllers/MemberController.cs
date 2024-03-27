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
using Microsoft.Net.Http.Headers;

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
                var user = await _context.users.FirstOrDefaultAsync(x => x.Mail == email);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "登入失敗 - 找不到使用者";
                    return RedirectToAction("Login");
                }
                else
                {
                    var hashedPassword = ComputeSHA256Hash(password);
                    if (hashedPassword == user.Pwd)
                    {

                        var roles = user.SuperUser.ToString().Trim();
                        var userId = user.UserId.ToString();
                        
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, "username"),
                            new Claim(ClaimTypes.Role, roles),
						};

                        var _headshot = (from h in _context.users
                                       where h.UserId == Convert.ToInt32(userId)
                                       select h.Headshot).ToList();

                        string headshot = _headshot[0].ToString(); 

                        //權限
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                        // cookies
                        Response.Cookies.Append("UsernameCookie", userId);
                        Response.Cookies.Append("UserheadshotCookie", headshot);
                        Response.Cookies.Append("UserheadshotCookie", password);
                        return RedirectToAction("Index", "AiRecommend");
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

            string userId;
            var test = Request.Cookies.TryGetValue("UsernameCookie", out userId);
            userId = null;

            // 返回到首頁或其他適當的頁面
            return RedirectToAction("Login");
        }

		public IActionResult fail() 
        {
            ViewBag.Text = "失敗了!";
            return View();
        }

        public IActionResult NoAccess()
        {
            return View();
        }


        //已更改成使用user表 棄用aspnetuser
        public IActionResult Register(string email, string password, string confirmPassword)
		{
			if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(confirmPassword))
			{

                string hashedPassword = ComputeSHA256Hash(password);

                //缺比對資料庫中有無重複的信箱
                var repeatMail = (from o in _context.users
                                 where o.Mail == email
                                 select o.Mail).ToList();

                if(repeatMail.Count == 0)
                {
                    var user = new users
                    {
                        Mail = email,
                        Pwd = hashedPassword, // 密碼應該是哈希後的值
                        SuperUser = "N",
                        Headshot= "default headshot"
                    };

                    _context.users.Add(user);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "註冊成功";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["ErrorMessage"] = "註冊失敗 - 帳號已被註冊";
                    return RedirectToAction("Login");
                }
                
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
