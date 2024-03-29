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
		//[HttpPost]
		public IActionResult PersonalPage(string? search,int? userId)
		{
			users users;//��e�����ϥΪ�
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
			var articles = _context.article
				.Where(a => a.ArticleState == "�o�G" && a.UserId == users.UserId)
				.ToList();
			// �w?��?��?��?�ǰt HTML ??
			Regex htmlTagRegex = new Regex("<.*?>");
			// �ϥ� LINQ �b?�s��?��?�@�B��??�M�Ƨ�
			articles = articles
				.Where(a => string.IsNullOrWhiteSpace(search) ||
					(a.Contents != null && htmlTagRegex.Replace(a.Contents, "").Contains(search)) ||
					(a.Title != null && a.Title.Contains(search)))
				.OrderByDescending(a => a.PublishTime)
				.ToList();
			// ??�u?�Ψ�L�a��?�� UsersArticleViewModel ?����?�u
			UsersArticleViewModel viewModel = new UsersArticleViewModel();
			// ? Cookie ��?���K?��
			string passwordCookie = Request.Cookies["UserPasswordCookie"];
			// ��? articles �O IEnumerable<Article> ?�����ҫ�?�u
			// ?�K?��?????�ҫ�
			viewModel.PasswordCookie = passwordCookie;
			viewModel.article = articles; // ?�峹�C��?��???�ҫ��� article ?��
			viewModel.users = users;
			ViewBag.loginUserId = loginUserId;
			// ��^??�A�}???�ҫ�?????
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
				// �ͦ��n??���� URL
				string loginUrl = Url.Content("~/Member/Login");  // ��?���z?�ت� URL �ͦ���k

				// �ϥ� HttpResponse ���w�V��ͦ��� URL
				return Redirect(loginUrl).ToString();
				//�B�z Cookie ���s�b�����p
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
			// ��s??�ҫ�������??���H��
			var viewModel = new UsersArticleViewModel
			{
				// ��L??�ҫ�?�u
				users = UserImage // ��s��??���H��
			};
			//return PartialView("ArticleCard", viewModel); // ?��s�Z��??�ҫ�??��??��
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
				//�B�z Cookie ���s�b�����p
			}

			// ?�d�ͤ�r�q�O�_?�šA�p�G?��??��?�m? null
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
			//a.Pwd = ComputeSHA256Hash(Pwd); // ?��K?���Ʀ}�O�s
			//}
			a.Nickname = Nickname;
			a.Birthday = Birthday;
			a.Address = Address;
			a.Introduction = Introduction;
			a.Interest = Interest;
			_context.SaveChanges();
			// ��s�K? Cookie
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
			
			return "look:" + Yid + "�Ӥ�id:" + PhotoId;
		}

		//Model
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

        //�˵���{
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

        //�qai����[�J���{
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

            //�o���ˬd�O�_�P��Ʈw����
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

        //������{
        public IActionResult Remove(string scenicSpotName)
        {

            string userId;
            Request.Cookies.TryGetValue("UsernameCookie", out userId);
            int id = Convert.ToInt32(userId);


            //�ˬd�ϥΪ̤�����
            if (scenicSpotName != null)
            {
                //��ثe�����IID
                var findSpotId = (from s in _context.Spots
                                  where s.ScenicSpotName == scenicSpotName
                                  select s.SpotId).ToList();
                int spotId = Convert.ToInt32(findSpotId[0]);

                //��ثe���y����
                var checkQuery = (from f in _context.myFavor
                                  where f.UserId == id && f.SpotId == spotId
                                  select f).ToList();



                if (checkQuery.Any())
                {
                    // �p�G�s�b�۲Ū����ðO���A�R������
                    _context.myFavor.RemoveRange(checkQuery);

                    // �x�s�ܧ���Ʈw
                    _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("ViewSchedule");
        }

		// �g�K��
		public IActionResult MarkDown(string scenicSpotName)
		{
            string userId;
            Request.Cookies.TryGetValue("UsernameCookie", out userId);
            int id = Convert.ToInt32(userId);

            if (scenicSpotName != null)
			{
                //��ثe�����IID
                var findSpotId = (from s in _context.Spots
                                  where s.ScenicSpotName == scenicSpotName
                                  select s.SpotId).ToList();
                int spotId = Convert.ToInt32(findSpotId[0]);

                //��ثe���y����
                var checkQuery = (from f in _context.myFavor
                                  where f.UserId == id && f.SpotId == spotId
                                  select f).ToList();

				int uuid = checkQuery[0].SpotId;

                return RedirectToAction("CreateDraft", "Article", new { SpotId = uuid });
			}

			else
			{
                return RedirectToAction("fail","Member");
            }
        }
    }
}
