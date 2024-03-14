using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using System.Diagnostics;
using TravelNotes.Models;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TravelNotes.Controllers
{
	public class PersonalPageController : Controller
	{
		private readonly IWebHostEnvironment _hostingEnvironment;
		int UserId = 1;
		private readonly TravelContext _context;

		public PersonalPageController(TravelContext context, IWebHostEnvironment hostingEnvironment)
		{
			_context = context;
			_hostingEnvironment = hostingEnvironment;
		}

		public IActionResult Index()
		{

			return View(_context.users.ToList());
		}

		public IActionResult Privacy()
		{
			return View();
		}

		public IActionResult Schedule()
		{
			return View();
		}

		public IActionResult Article()
		{
			ViewBag.article = _context.article.ToList();
			return View(_context.users.ToList());
		}

		public IActionResult PersonalPage()
		{
			ViewBag.article = _context.article.Where((a => a.UserId == 2)).ToList();
			return View(_context.users.ToList());
		}

		[HttpPost]
		public IActionResult PersonalPage(string search)
		{
			var a = _context.article.Where((a => a.UserId == 2));
			var b = a.Where((a => a.Contents.IndexOf(search) > -1));
			ViewBag.article = b.ToList();
			return View(_context.users.ToList());
		}
		//private TravelContext db = new TravelContext();
		//public IActionResult PersonalPage(int ArticleId, int UserId, string Title, DateTime PublishTime, string Contents, string Images,string Articles)
		//{
		//    var data = db.Articles.ToList();
		//    //var r = _context.Articles.FirstOrDefault(x => x.UserId == UserId);
		//    ViewBag.WholeTable = data;
		//    ViewBag.Articles = Articles;
		//    ViewBag.ArticleId = ArticleId;
		//    ViewBag.Title = Title;
		//    ViewBag.PublishTime = PublishTime;
		//    ViewBag.Contents = Contents;
		//    ViewBag.Images = Images;

		//    return View(_context.Articles.ToList());
		//}

		public IActionResult Year()
		{
			
			return View(_context.photo.ToList());
		}

		public IActionResult Album()
		{
			//var photos = from o in _context.Photos
			//             where o.userId == UserId
			//             select o;
			return View(_context.photo.ToList());
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		[HttpPost]
		public IActionResult Save(string userName, string Phone, string Mail, string Gender, string Pwd, string Nickname, DateOnly Birthday, string Address, string Introduction, string Interest)
		{
			users a = _context.users.FirstOrDefault(a => a.UserId == UserId);
			a.UserName = userName;
			a.Phone = Phone;
			a.Mail = Mail;
			a.Gender = Gender;
			a.Pwd = Pwd;
			a.Nickname = Nickname;
			a.Birthday = Birthday;
			a.Address = Address;
			a.Introduction = Introduction;
			a.Interest = Interest;



			//新增新的一筆資料
			//User n = new User
			//{
			//    UserId = UserId,
			//    UserName = userName,
			//    Phone = Phone,
			//    Mail = Mail,
			//    Gender = Gender,
			//    Pwd = Pwd,
			//    Nickname = Nickname,
			//    Birthday = Birthday,
			//    Address = Address,
			//    Introduction = Introduction,
			//    Interest = Interest
			//};
			//_context.Users.Add(n);
			_context.SaveChanges();


			//_context.Users.FirstOrDefault(u => u.UserId == userName);

			//var a = _context.Users.ToList();
			//ViewBag.UserName = userName;
			//ViewBag.Introduction = Introduction;

			return RedirectToAction("PersonalPage");
			//return View(_context.Users.ToList());
		}



		[HttpPost]
		public string UserImage(IFormFile file)
		{
			string uploadsFolder = _hostingEnvironment.WebRootPath + "\\img";
			//string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "img\\articleImg");
			if (!Directory.Exists(uploadsFolder))
			{
				Directory.CreateDirectory(uploadsFolder);
			}
			string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
			string filePath = Path.Combine(uploadsFolder, uniqueFileName);


			//using (var stream = new FileStream(filePath, FileMode.Create))
			//{
			//    file.CopyTo(stream);
			//}
			//string filePath = uploadsFolder+"\\"+file.FileName;

			var stream = new FileStream(filePath, FileMode.Create);
			{
				file.CopyTo(stream);
			}
			string srcString = "/img/" + uniqueFileName;
			//var currentArticle = _context.Articles.FirstOrDefault(x => x.ArticleId == articleId);
			//currentArticle!.Images = srcString;
			//_context.SaveChanges();

			var UserImage = _context.users.FirstOrDefault(x => x.UserId == UserId);
			UserImage!.Haedshot = srcString;
			_context.SaveChanges();
			return ("OK");
		}

		//[HttpPost]
		//public async Task<IActionResult> PersonalPage(string search)
		//{
		//	//ViewBag.article = _context.article.Where((a => a.UserId == 2)).ToList();
		//	//方法B: 方法語法
		//	return View(await _context.article.Where(x => x.Contents!.IndexOf(search) > -1).ToListAsync());


		//	//	////方法A: 查詢語法 (SQL)
		//	//	//var data = from p in _context.Pokemons
		//	//	//        where p.PokeName!.IndexOf(bee) > -1
		//	//	//        select p;
		//	//	//return View(await data.ToListAsync());
		//	//}

		//}
		public IActionResult Test()
		{
			//string a = Path.Combine(_hostingEnvironment.WebRootPath, "img\\Headshot");
			//ViewBag.a = a;

			return View();

		}

		[HttpPost]
		public string YearImage(IFormFile file)
		{
			string uploadsFolder = _hostingEnvironment.WebRootPath + "\\img";
			//string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "img\\articleImg");
			if (!Directory.Exists(uploadsFolder))
			{
				Directory.CreateDirectory(uploadsFolder);
			}
			string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
			string filePath = Path.Combine(uploadsFolder, uniqueFileName);


			//using (var stream = new FileStream(filePath, FileMode.Create))
			//{
			//    file.CopyTo(stream);
			//}
			//string filePath = uploadsFolder+"\\"+file.FileName;

			var stream = new FileStream(filePath, FileMode.Create);
			{
				file.CopyTo(stream);
			}
			photo photo = new photo();
			photo.UserId = UserId;
			photo.PhotoTitle = file.FileName;
			string srcString = "/img/" + uniqueFileName;
			photo.PhotoPath = srcString;
			_context.photo.Add(photo);
			//var currentArticle = _context.Articles.FirstOrDefault(x => x.ArticleId == articleId);
			//currentArticle!.Images = srcString;
			//_context.SaveChanges();

			//var YearImage = _context.Photos.FirstOrDefault(x => x.PhotoId == PhotoId);
			//YearImage!.Haedshot = srcString;
			_context.SaveChanges();
			return ("OK");
		}



	}
}
