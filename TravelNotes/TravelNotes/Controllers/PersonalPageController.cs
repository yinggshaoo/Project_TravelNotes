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
		//Set UserId for Testing -> To be Modified Later
		int UserId = 1;
		private readonly TravelContext _context;

		public PersonalPageController(TravelContext context, IWebHostEnvironment hostingEnvironment)
		{
			_context = context;
			_hostingEnvironment = hostingEnvironment;
		}

		//Load Article DataList for PersonaPage
		public IActionResult PersonalPage()
		{
			var users = _context.users.FirstOrDefault(a => a.UserId == UserId);
			var article = _context.article.Where(p => p.UserId == UserId);
			var UsersArticleViewModel = new UsersArticleViewModel
			{
					users = users,
					article = article
			};
			return View(UsersArticleViewModel);
			//var usersArticleViewModel = _context.UsersArticleViewModels.FirstOrDefault();
			//ViewBag.article = _context.article.Where((a => a.UserId == UserId)).ToList();
			//return View(_context.users.ToList());
		}

		//**Set Article in PersonalPage -> Not working
		//public ActionResult PersonalPage(int UserId)
		//{
		//	var a = _context.article.Where((a => a.UserId == UserId));
		//	ViewData["article"] = a.ToList();
		//	return View();
		//}

		//Search Article for PersonalPage
		[HttpPost]
		public IActionResult PersonalPage(string search)
		{
			var a = _context.article.Where((a => a.UserId == UserId));
			var b = a.Where(a => a.Contents.IndexOf(search) > -1);
			ViewBag.article = b.ToList();
			return View(_context.users.ToList());
		}

		//Upload & Save Headshot DataList for PersonalPage
		[HttpPost]
		public string UserImage(IFormFile file)
		{
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
			_context.SaveChanges();
			return ("Ok");
		}

		//Update & Save User DataList for PersonalPage
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
			_context.SaveChanges();
			return RedirectToAction("PersonalPage");
		}

		//Load Photo DataList for LookBack
		public IActionResult Year()
		{
			var LookBack = _context.LookBack.FirstOrDefault(a => a.UserId == UserId);
			var photo = _context.photo.Where(p => p.UserId == UserId);
			var LookBackPhotoViewModel = new LookBackPhotoViewModel
			{
				LookBack = (IEnumerable<LookBack>)LookBack,
				photo = (IEnumerable<photo>)photo
			};
			return View(LookBackPhotoViewModel);
		}

		//Upload & Save for LookBack Page
		[HttpPost]
		public string SaveLookBackImage(int Yid,int UserId,int PhotoId)
		{
			return ("Ok");
		}

		//Model
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		//Create a ArticleView Page for testing
		public IActionResult Article()
		{
			ViewBag.article = _context.article.ToList();
			return View(_context.users.ToList());
		}

		//Create a Album Page for testing
		public IActionResult Album()
		{
			//var photos = from o in _context.Photos
			//             where o.userId == UserId
			//             select o;
			return View(_context.photo.ToList());
		}

		//Upload LookBackImage in Album Page for testing
		[HttpPost]
		public string YearImage(IFormFile file)
		{
			string uploadsFolder = _hostingEnvironment.WebRootPath + "\\img";
			if (!Directory.Exists(uploadsFolder))
			{
				Directory.CreateDirectory(uploadsFolder);
			}
			string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
			string filePath = Path.Combine(uploadsFolder, uniqueFileName);
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
			_context.SaveChanges();
			return ("OK");
		}

		//Create a Schedule Page for testing
		public IActionResult Schedule()
		{
			return View();
		}

        //**Add a User Data for test
        //[HttpPosteeeeee]
        //public IActionResult AddUserData(string userName, string Phone, string Mail, string Gender, string Pwd, string Nickname, DateOnly Birthday, string Address, string Introduction, string Interest)
        //{
        //	User n = new User
        //	{
        //	    UserId = UserId,
        //	    UserName = userName,
        //	    Phone = Phone,
        //	    Mail = Mail,
        //	    Gender = Gender,
        //	    Pwd = Pwd,
        //	    Nickname = Nickname,
        //	    Birthday = Birthday,
        //	    Address = Address,
        //	    Introduction = Introduction,
        //	    Interest = Interest
        //	};
        //	_context.Users.Add(n);
        //	return View(_context.Users.ToList());
        //}

        //**Set Article in PersonalPage for testing
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
        [HttpPost]
        public IActionResult AddTripPlanning()
        {
            return View();
        }

    }
}
