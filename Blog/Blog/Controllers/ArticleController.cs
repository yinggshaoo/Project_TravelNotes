using Blog.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;



namespace Blog.Controllers
{
	
	public class ArticleController : Controller
	{
		int userID = 1;
		private readonly TravelContext _context;
		private readonly IWebHostEnvironment _hostingEnvironment;
		public ArticleController(IWebHostEnvironment hostingEnvironment,TravelContext context)
		{
			_hostingEnvironment = hostingEnvironment;
			_context = context;
		}
        #region 圖片上傳
        [HttpPost]
        //文字編輯器上傳圖片
        public IActionResult UploadImage([FromForm] IFormFile file)
        {
            string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "img");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Return URL of saved image
            string imageUrl = uniqueFileName;


            return Json(new { location = imageUrl });
        }

        [HttpPost]
        //文字編輯器上傳圖片
        public IActionResult UploadArticleImage(IFormFile file, int articleId, string title)
        {
            string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "img\\articleImg");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);


            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            string srcString = "/img/articleImg/" + uniqueFileName;
            var currentArticle = _context.Articles.FirstOrDefault(x => x.ArticleId == articleId);
            currentArticle!.Images = srcString;
            if (title == null)
            {
                currentArticle!.Title = "";
            }
            else
            {
                currentArticle!.Title = title;
            }
            _context.SaveChanges();
            return Ok();
        }
        #endregion
        #region 草稿相關
        public IActionResult Draft()
        {
			var data = _context.Articles.Where(a =>a.UserId == userID).ToList();
            return View(data);
        }

		public IActionResult DraftView(int editId)
		{
			ViewBag.EditId = editId;
            var data = _context.Articles.Where(a => a.UserId == userID).ToList();
            return View("Draft", data);
        }
		[HttpPost]
        public IActionResult SaveDraft(int articleId, string title,string subtitle,DateTime travelTime, string content)
		{
			var currentArticle = _context.Articles.FirstOrDefault(x => x.ArticleId == articleId);
			if (title==null)
			{
				currentArticle!.Title = "";
            }
			else
			{
                currentArticle!.Title = title;
            }
			currentArticle.Subtitle = subtitle;
			currentArticle.TravelTime = travelTime;
			currentArticle.Contents = content;
			_context.SaveChanges();
			return Ok();
        }
        public IActionResult CreateDraft()
        {
			Article article = new Article();
			article.UserId = userID;//之後要改
			article.Title = "";
			article.Subtitle = "";
			article.PublishTime = DateTime.Now;
			article.TravelTime = DateTime.Now;
			article.Contents = "";
			article.Location = "";
			article.Images = "";
			article.LikeCount = 0;
			article.PageView = 0;
			article.ArticleState = "草稿";
			_context.Add(article);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost]
        public IActionResult DeleteDraft(int articleId)
        {
            var articleToDelete = _context.Articles.FirstOrDefault(a => a.ArticleId == articleId);            
            _context.Articles.Remove(articleToDelete); // 从数据库中移除文章
            _context.SaveChanges(); // 保存更改

            return Ok();
        }
        
        #endregion
        #region 文章相關
        public IActionResult ArticleEdit(int editId)
        {
			Article data = _context.Articles.FirstOrDefault(a => a.ArticleId == editId)!;
            return View(data);
        }
        [HttpPost]
        public IActionResult SaveArticle(int articleId, string title, string subtitle, DateTime travelTime, string content)
        {
            var currentArticle = _context.Articles.FirstOrDefault(x => x.ArticleId == articleId);
            if (title == null)
            {
                currentArticle!.Title = "";
            }
            else
            {
                currentArticle!.Title = title;
            }
            currentArticle.Subtitle = subtitle;
            currentArticle.TravelTime = travelTime;
            currentArticle.PublishTime = DateTime.Now;
            currentArticle.Contents = content;
            _context.SaveChanges();
            return Ok();
        }
        #endregion
    }
}
