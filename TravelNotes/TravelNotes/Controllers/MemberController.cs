using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using TravelNotes.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Http;


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

                        string headshot = _headshot?.FirstOrDefault() ?? "Default String";


                        //string? headshot = _headshot[0].ToString(); 

                        //權限
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                        // cookies
                        Response.Cookies.Append("UsernameCookie", userId);
                        Response.Cookies.Append("UserheadshotCookie", headshot);
                        Response.Cookies.Append("UserPasswordCookie", password);
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

            Response.Cookies.Delete("UsernameCookie");
            Response.Cookies.Delete("UserheadshotCookie");
            Response.Cookies.Delete("UserPasswordCookie");
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
                        Headshot= "/images/userImageDefault.jpg"
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

        public IActionResult ForgotPwd()
        {
            return View();
        }

        public IActionResult SendEmail(string toAddress)
        {
            var user = _context.users.FirstOrDefault(u => u.Mail == toAddress);
            var SendPwd = "";
            if (user != null)
            {
                string randomString = GenerateRandomString(8);
                user.Pwd = randomString;
                SendPwd += randomString;
                _context.SaveChanges();
                Console.WriteLine("密碼已成功更新。");
            }
            else
            {
                Console.WriteLine("找不到對應郵件地址的使用者。");
            }

            try
            {
                MailMessage msg = new MailMessage();
                msg.To.Add(toAddress);
                msg.From = new MailAddress("travelnotes9802@gmail.com", 
                    "TravelNotes.org", 
                    Encoding.UTF8);

                /* 上面3個參數分別是發件人地址（可以隨便寫），發件人姓名，編碼*/
                msg.Subject = "旅行筆記-忘記密碼";//郵件標題
                msg.SubjectEncoding = Encoding.UTF8;//郵件標題編碼
                msg.Body = $"這是您暫時的密碼 : {SendPwd}"; //郵件內容
                msg.BodyEncoding = Encoding.UTF8;//郵件內容編碼 
                //msg.Attachments.Add(new Attachment(@"D:\test2.docx"));  //附件
                msg.IsBodyHtml = true;//是否是HTML郵件 
                                      //msg.Priority = MailPriority.High;//郵件優先級 

                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential("Aa0977706956@gmail.com",
                    "etyymhpjhwfxfxym"); //這裡要填正確的帳號跟密碼
                client.Host = "smtp.gmail.com"; //設定smtp Server
                client.Port = 25; //設定Port
                client.EnableSsl = true; //gmail預設開啟驗證
                client.Send(msg); //寄出信件
                client.Dispose();
                msg.Dispose();
                TempData["SuccessMessage"] = "郵件發送成功";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.ToString();
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("ForgotPwd");
        }

        static string GenerateRandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length);
                stringBuilder.Append(chars[index]);
            }

            return stringBuilder.ToString();
        }
    }
}
