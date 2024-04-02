using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using System.Diagnostics;
using TravelNotes.Models;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;


namespace TravelNotes.Controllers
{
	public class PersonalPageController : Controller
	{
		private readonly IWebHostEnvironment _hostingEnvironment;
		//Set UserId for Testing -> To be Modified Later
		int UserId = 1;
		private readonly TravelContext _context;

		public PersonalPageController(TravelContext context, IWebHostEnvironment hostingEnvironment)
		{
			_context = context;
			_hostingEnvironment = hostingEnvironment;
		}

		//Find User & Search Article for PersonalPage
		public IActionResult PersonalPage(string? search, int? userId)
		{
			users users;//當前頁面使用者
			int loginUserId = 0;
			string cookieValue;
			bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue);
			if (result)
			{
				loginUserId = Convert.ToInt32(cookieValue);
			}
			if (userId == null)
			{
				if (loginUserId != 0)
				{
					users = _context.users.FirstOrDefault(a => a.UserId == loginUserId);
				}
				else
				{
					return RedirectToAction("Login", "Member");
				}
			}
			else
			{
				users = _context.users.FirstOrDefault(a => a.UserId == userId);
			}
			Regex htmlTagRegex = new Regex("<.*?>");
			var articles = _context.article
				.Where(a => a.ArticleState == "發佈" && a.UserId == users.UserId)
				.GroupJoin(_context.Spots, // 進行分組連接
				article => article.SpotId,
				spot => spot.SpotId,
				 (article, spots) => new { Article = article, Spots = spots })
				.SelectMany(
				x => x.Spots.DefaultIfEmpty(), // 對於沒有對應 Spot 的 Article，Spots 將是一個包含一個 null 元素的集合
				(x, spot) => new { Article = x.Article, Spots = spot })
				.ToList(); // 物化查询结果以便在内存中处理
						   // 过滤: 如果提供了搜索字符串，就在内存中应用正则表达式去除HTML标签后进行搜索
			if (!string.IsNullOrWhiteSpace(search))
			{
				articles = articles
					.Where(a => htmlTagRegex.Replace(a.Article.Contents ?? "", "").Contains(search) ||
								a.Article.Title.Contains(search) ||
								a.Spots != null && a.Spots.ScenicSpotName.Contains(search))
					.ToList();
			}
			// 對結果進行 PublishTime 降序排序並轉換為 usersArticleModel 列表
			var dataList = articles
				.OrderByDescending(a => a.Article.PublishTime)
				.Select(a => new usersArticleModel
				{
					article = a.Article,
					user = _context.users.FirstOrDefault(u => u.UserId == a.Article.UserId),
					// 如果你還需要從 Spots 表中獲取信息，你可以在這裡添加
					// 比如，SpotName = a.Spot.ScenicSpotName
				})
				.ToList();
			// 根据用户或其他地方获取 UsersArticleViewModel 类型的数据
			UsersArticleViewModel viewModel = new UsersArticleViewModel();
			// 从 Cookie 中获取密钥值
			string passwordCookie = Request.Cookies["UserPasswordCookie"];
			// 假设 articles 是 IEnumerable<Article> 类型的模型数据
			// 将密钥值设定到模型
			viewModel.PasswordCookie = passwordCookie;
			// 将文章列表的值设定到模型的 article 属性
			// 將資料列表中的文章資料轉換為 IEnumerable<article> 型別的 viewModel.article 屬性
			viewModel.article = (IEnumerable<article>)dataList.Select(a => a.article).ToList();
			// 将用户列表的值设定到模型的 users 属性
			viewModel.users = users;
			ViewBag.loginUserId = loginUserId;
			// 返回视图，并传递模型数据
			return View(viewModel);
		}

		//Upload & Save Headshot DataList for PersonalPage
		[HttpPost]
		public string UserImage(IFormFile file)
		{
			string cookieValue;
			bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue);
			if (result)
			{
				UserId = Convert.ToInt32(cookieValue);
			}
			else
			{
				// 生成登??面的 URL
				string loginUrl = Url.Content("~/Member/Login");  // 替?成您?目的 URL 生成方法

				// 使用 HttpResponse 重定向到生成的 URL
				return Redirect(loginUrl).ToString();
				//處理 Cookie 不存在的情況
			}
			string imgFolder = Path.Combine(_hostingEnvironment.WebRootPath, "img");
			if (!Directory.Exists(imgFolder))
			{
				Directory.CreateDirectory(imgFolder);
			}
			string userFolder = Path.Combine(imgFolder, "user" + UserId);
			if (!Directory.Exists(userFolder))
			{
				Directory.CreateDirectory(userFolder);
			}
			string headshotFolder = Path.Combine(userFolder, "headshot");
			if (!Directory.Exists(headshotFolder))
			{
				Directory.CreateDirectory(headshotFolder);
			}
			string uploadsFolder = Path.Combine(headshotFolder);
			if (!Directory.Exists(uploadsFolder))
			{
				Directory.CreateDirectory(uploadsFolder);
			}
			string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
			string filePath = Path.Combine(uploadsFolder + "\\", uniqueFileName);
			var stream = new FileStream(filePath, FileMode.Create);
			file.CopyTo(stream);
			string srcString = "/img/" + Path.GetRelativePath(imgFolder, filePath).Replace('\\', '/');
			var UserImage = _context.users.FirstOrDefault(x => x.UserId == UserId);
			UserImage!.Headshot = srcString;
			Response.Cookies.Append("UserheadshotCookie", srcString);
			_context.SaveChanges();
			// 更新模型中的使用者像信息
			var viewModel = new UsersArticleViewModel
			{
				users = UserImage
			};
			return ("Ok");
		}

		//Update & Save User DataList for PersonalPage
		[HttpPost]
		public IActionResult Save(string userName, string Phone, string Mail, string Gender, string Pwd, string Nickname, DateOnly? Birthday, string Address, string Introduction, string Interest)
		{
			string cookieValue;
			bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue);
			if (result)
			{
				UserId = Convert.ToInt32(cookieValue);
			}
			else
			{
				return RedirectToAction("Login", "Member");
				//處理 Cookie 不存在的情況
			}

			// 檢查生日字段是否為空，如果為空則將其設置為 null
			if (string.IsNullOrEmpty(Request.Form["Birthday"]))
			{
				Birthday = null;
			}

			int userId = Convert.ToInt32(cookieValue);
			users a = _context.users.FirstOrDefault(a => a.UserId == UserId);
			a.UserName = userName;
			a.Phone = Phone;
			a.Mail = Mail;
			a.Gender = Gender;
			a.Pwd = ComputeSHA256Hash(Pwd);
			//if (!string.IsNullOrEmpty(Pwd))
			//{
			//a.Pwd = ComputeSHA256Hash(Pwd); // 計算密碼的 SHA-256 雜湊值並保存
			//}
			a.Nickname = Nickname;
			a.Birthday = Birthday;
			a.Address = Address;
			a.Introduction = Introduction;
			a.Interest = Interest;
			_context.SaveChanges();
			// 更新密碼 Cookie
			Response.Cookies.Append("UserPasswordCookie", Pwd);
			return RedirectToAction("PersonalPage");
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

		//Load Photo DataList for LookBack
		public IActionResult Year(int? userId)
		{
			string cookieValue;
			bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue);
			int loginUserId = 0;
			if (result)
			{
				loginUserId = Convert.ToInt32(cookieValue);
			}
			var LookBack = _context.LookBack.Where(a => a.UserId == userId).ToList();
			var photo = _context.photo.Where(p => p.UserId == userId && p.PhotoDescription != "1").ToList();
			//photo.FirstOrDefault().PhotoPath
			var data = from p in photo
					   join l in LookBack
					   on p.PhotoId equals l.PhotoId
					   select new LookBackPhotoViewModel2
					   {
						   PhotoPath = p.PhotoPath,
						   Yid = l.Yid,
					   };
			var bag = data.ToList();
			var LookBackPhotoViewModel = new LookBackPhotoViewModel
			{
				LookBack = LookBack,
				photo = photo,
				photoPaths = bag,
			};
			ViewBag.pic1 = bag.FirstOrDefault(a => a.Yid == 1);
			ViewBag.pic2 = bag.FirstOrDefault(a => a.Yid == 2);
			ViewBag.pic3 = bag.FirstOrDefault(a => a.Yid == 3);
			ViewBag.pic4 = bag.FirstOrDefault(a => a.Yid == 4);
			ViewBag.pic5 = bag.FirstOrDefault(a => a.Yid == 5);
			ViewBag.pic6 = bag.FirstOrDefault(a => a.Yid == 6);
			ViewBag.pic7 = bag.FirstOrDefault(a => a.Yid == 7);
			ViewBag.pic8 = bag.FirstOrDefault(a => a.Yid == 8);
			ViewBag.pic9 = bag.FirstOrDefault(a => a.Yid == 9);
			ViewBag.pic10 = bag.FirstOrDefault(a => a.Yid == 10);
			ViewBag.pic11 = bag.FirstOrDefault(a => a.Yid == 11);
			ViewBag.pic12 = bag.FirstOrDefault(a => a.Yid == 12);
			ViewBag.loginUserId = loginUserId;
			ViewBag.isMyPage = loginUserId == userId;
			return View(LookBackPhotoViewModel);
		}

		//Upload & Save for LookBack Page
		[HttpPost]
		public string SaveLookBackImage(int Yid, int PhotoId, int id, int? userId)
		{
			string cookieValue;
			bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue);
			if (result)
			{
				UserId = Convert.ToInt32(cookieValue);
			}
			var LookBackImage = _context.LookBack.FirstOrDefault(a => a.UserId == UserId);
			var data = _context.LookBack.FirstOrDefault(a => a.Yid == Yid && a.UserId == UserId);

			if (data != null)
			{
				data.PhotoId = PhotoId;

			}
			else
			{
				LookBack lookBack = new LookBack();
				lookBack.PhotoId = PhotoId;
				lookBack.Yid = Yid;
				lookBack.UserId = UserId;
				_context.LookBack.Add(lookBack);
			}

			_context.SaveChanges();

			return "look:" + Yid + "照片id:" + PhotoId;
		}

		//Model
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		//檢視行程
		public IActionResult ViewSchedule()
		{
			string userId;
			Request.Cookies.TryGetValue("UsernameCookie", out userId);
			int id = Convert.ToInt32(userId);

			if (userId != null)
			{
				var query = (from s in _context.Spots
							 where (from o in _context.myFavor
									where o.UserId == id
									select o.SpotId).Contains(s.SpotId)
							 select s).ToList();

				return View(query);
			}

			return RedirectToAction("fail", "Member");

		}

		//從ai那邊加入到行程
		public IActionResult Schedule(string scenicSpotName)
		{
			if (scenicSpotName == null)
			{
				return RedirectToAction("ViewSchedule");
			}

			string userId;
			Request.Cookies.TryGetValue("UsernameCookie", out userId);
			int id = Convert.ToInt32(userId);
			var query = (from o in _context.Spots
						 where o.ScenicSpotName == scenicSpotName
						 select o.SpotId).ToList();

			var viewQuery = _context.Spots.Where(x => x.ScenicSpotName == scenicSpotName).ToList();

			int value = Convert.ToInt32(query[0]);

			//這裡檢查是否與資料庫重複
			var checkRepect = (from f in _context.myFavor
							   where f.UserId == id && f.SpotId == value
							   select f).ToList();

			var flag = checkRepect.Count > 0;



			if (userId != null && flag == false)
			{
				myFavor favor = new myFavor();
				favor.UserId = id;
				favor.SpotId = value;
				_context.myFavor.Add(favor);
				_context.SaveChanges();
			}
			else
			{
				return RedirectToAction("fail", "Member");
			}


			return RedirectToAction("ViewSchedule");
		}

		//移除行程
		public IActionResult Remove(string scenicSpotName)
		{

			string userId;
			Request.Cookies.TryGetValue("UsernameCookie", out userId);
			int id = Convert.ToInt32(userId);


			//檢查使用者不為空
			if (scenicSpotName != null)
			{
				//抓目前的景點ID
				var findSpotId = (from s in _context.Spots
								  where s.ScenicSpotName == scenicSpotName
								  select s.SpotId).ToList();
				int spotId = Convert.ToInt32(findSpotId[0]);

				//抓目前的流水號
				var checkQuery = (from f in _context.myFavor
								  where f.UserId == id && f.SpotId == spotId
								  select f).ToList();



				if (checkQuery.Any())
				{
					// 如果存在相符的收藏記錄，刪除它們
					_context.myFavor.RemoveRange(checkQuery);

					// 儲存變更到資料庫
					_context.SaveChangesAsync();
				}
			}

			return RedirectToAction("ViewSchedule");
		}

		// 寫貼文
		public IActionResult MarkDown(string scenicSpotName)
		{
			string userId;
			Request.Cookies.TryGetValue("UsernameCookie", out userId);
			int id = Convert.ToInt32(userId);

			if (scenicSpotName != null)
			{
				//抓目前的景點ID
				var findSpotId = (from s in _context.Spots
								  where s.ScenicSpotName == scenicSpotName
								  select s.SpotId).ToList();
				int spotId = Convert.ToInt32(findSpotId[0]);

				//抓目前的流水號
				var checkQuery = (from f in _context.myFavor
								  where f.UserId == id && f.SpotId == spotId
								  select f).ToList();

				int uuid = checkQuery[0].SpotId;

				return RedirectToAction("CreateDraft", "Article", new { SpotId = uuid });
			}

			else
			{
				return RedirectToAction("fail", "Member");
			}
		}
	}
}
