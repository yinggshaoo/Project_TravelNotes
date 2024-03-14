using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using TravelNotes.Models;
using Microsoft.VisualBasic;
using System.Web;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Principal;


namespace TravelNotes.Controllers

{
    public class ArticleController : Controller
    {
        private readonly object _lockObject = new object();
        int userID = 1;
        private readonly TravelContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ArticleController(IWebHostEnvironment hostingEnvironment, TravelContext context)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }
        #region 圖片相關
        public void CheckFolderPhotos(string contentImages, int articleId)
        {
            // 解析 contentImages 字符串为对象
            List<string> imageList = JsonSerializer.Deserialize<List<string>>(contentImages)!;
            string directoryPath = Path.Combine(_hostingEnvironment.WebRootPath, $"img\\user{userID}\\article\\{articleId}\\content");

            // 获取目录中的所有文件名
            string[] fileNames = Directory.GetFiles(directoryPath);

            // 遍历文件名列表
            foreach (string fileName in fileNames)
            {
                // 如果文件名不在 imageList 中，就删除该文件
                if (!imageList.Contains(Path.GetFileName(fileName)))
                {
                    System.IO.File.Delete(fileName);
                }
            }

        }
        public string CopyToArticleFolder(IFormFile file, int articleId, string type)
        {
            string imgFolder = Path.Combine(_hostingEnvironment.WebRootPath, "img");
            if (!Directory.Exists(imgFolder))
            {
                Directory.CreateDirectory(imgFolder);
            }
            string userFolder = Path.Combine(imgFolder, "user" + userID);
            if (!Directory.Exists(userFolder))
            {
                Directory.CreateDirectory(userFolder);
            }
            string articleFolder = Path.Combine(userFolder, "article");
            if (!Directory.Exists(articleFolder))
            {
                Directory.CreateDirectory(articleFolder);
            }
            string articleIdFolder = Path.Combine(articleFolder, articleId.ToString());
            if (!Directory.Exists(articleIdFolder))
            {
                Directory.CreateDirectory(articleIdFolder);
            }
            string uploadsFolder = Path.Combine(articleIdFolder, type);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            var stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(stream);


            // Return URL of saved image
            return "/img/" + Path.GetRelativePath(imgFolder, filePath).Replace('\\', '/');
        }
        [HttpPost]
        //文字編輯器上傳圖片
        public IActionResult UploadImage(IFormFile file, int articleId)
        {
            // Return URL of saved image
            string imageUrl = CopyToArticleFolder(file, articleId, "content");

            return Json(new { location = imageUrl });
        }


        [HttpPost]

        public string UploadArticleImage(IFormFile file, int articleId)
        {
            string srcString = CopyToArticleFolder(file, articleId, "articleImage");
            var currentArticle = _context.article.FirstOrDefault(x => x.ArticleId == articleId);
            currentArticle!.Images = srcString;
            
            _context.SaveChanges();
            return srcString;
        }
        #endregion
        #region 草稿相關

        [HttpGet]
        public IActionResult Draft(int? articleId)
        {
            ViewBag.articleId = articleId;
            var data = _context.article.Where(a => a.UserId == userID && a.ArticleState == "草稿").ToList();
            //var data = _context.Articles.Where(a => a.UserId == userID ).ToList();
            if(data == null)
            {
                return View("errorView");
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult SaveDraft(int articleId, string title, string subtitle, DateTime travelTime, string content, string contentImages)
        {
            CheckFolderPhotos(contentImages, articleId);
            var currentArticle = _context.article.FirstOrDefault(x => x.ArticleId == articleId);
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
            currentArticle.Contents = content;
            _context.SaveChanges();
            return Ok();
        }
        public IActionResult CreateDraft()
        {
            article article = new article();
            article.UserId = userID;//之後要改
            article.Title = "";
            article.Subtitle = "";
            article.TravelTime = DateTime.Now;
            article.Contents = "";
            article.Images = "";
            article.LikeCount = 0;
            article.PageView = 0;
            article.ArticleState = "草稿";
            _context.Add(article);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost]
        public IActionResult PublishDraft(int articleId)
        {
            article articleToPublish = _context.article.FirstOrDefault(a => a.ArticleId == articleId)!;
            articleToPublish.PublishTime = DateTime.Now;
            articleToPublish.ArticleState = "發佈";
            _context.SaveChanges(); // 保存更改

            return Ok();
        }

        [HttpPost]
        public IActionResult DeleteDraft(int articleId)
        {
            article articleToDelete = _context.article.FirstOrDefault(a => a.ArticleId == articleId)!;
            _context.article.Remove(articleToDelete); // 从数据库中移除文章
            _context.SaveChanges(); // 保存更改

            return Ok();
        }

        #endregion
        #region 文章編輯相關

        public IActionResult ArticleEdit(int articleId)
        {
            article data = _context.article.FirstOrDefault(a => a.ArticleId == articleId && a.ArticleState == "發佈")!;
            if (data == null)
            {
                return View("errorView");
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult SaveArticle(int articleId, string title, string subtitle, DateTime travelTime, string content, string contentImages)
        {
            // 解析 contentImages 字符串为对象
            List<string> imageList = JsonSerializer.Deserialize<List<string>>(contentImages)!;
            string directoryPath = Path.Combine(_hostingEnvironment.WebRootPath, "img\\user1\\article\\3038\\content");

            // 获取目录中的所有文件名
            string[] fileNames = Directory.GetFiles(directoryPath);

            // 遍历文件名列表
            foreach (string fileName in fileNames)
            {
                // 如果文件名不在 imageList 中，就删除该文件
                if (!imageList.Contains(Path.GetFileName(fileName)))
                {
                    System.IO.File.Delete(fileName);
                }
            }


            var currentArticle = _context.article.FirstOrDefault(x => x.ArticleId == articleId);
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
        [HttpPost]
        public IActionResult DeleteArticle(int articleId)
        {
            var articleToDelete = _context.article.FirstOrDefault(a => a.ArticleId == articleId);
            _context.article.Remove(articleToDelete); // 从数据库中移除文章
            _context.SaveChanges(); // 保存更改

            return Ok();
        }

        #endregion
        #region 文章頁面相關
        public IActionResult ArticleView(int articleId)
        {
            article data = _context.article.FirstOrDefault(a => a.ArticleId == articleId && a.ArticleState == "發佈")!;
            if (data == null)
            {
                return View("errorView");
            }
            var messageBoards = from m in _context.messageBoard.Where(a => a.ArticleId == articleId)
                        join u in _context.users
                        on m.UserId equals u.UserId
                        select new
                        {
                            MessageId = m.MessageId,
                            Contents = m.Contents,
                            UserId = m.UserId,
                            MessageTime = m.MessageTime,
                            Haedshot = u.Haedshot
                        };
            ViewBag.messageBoards = messageBoards;
            return View(data);
        }
        [HttpPost]
        public int LikeArticle(int articleId)
        {

            lock (_lockObject)
            {
                article data = _context.article.FirstOrDefault(a => a.ArticleId == articleId)!;
                if (data != null)
                {
                    data.LikeCount += 1;
                    _context.SaveChanges();
                    return (int)data.LikeCount!;
                }
                else
                {
                    // 如果找不到文章，可能需要采取适当的错误处理措施
                    return -1; // 或者抛出异常
                }
            }
        }
        [HttpPost]
        public int ViewArticle(int articleId)
        {
            lock (_lockObject)
            {
                article data = _context.article.FirstOrDefault(a => a.ArticleId == articleId)!;
                if (data != null)
                {
                    data.PageView += 1;
                    _context.SaveChanges();
                    return (int)data.PageView!;
                }
                else
                {
                    // 如果找不到文章，可能需要采取适当的错误处理措施
                    return -1; // 或者抛出异常
                }
            }
        }
        [HttpPost]
        public string InsertMessage(int articleId, string message)
        {
            messageBoard messageBoard = new messageBoard();
            messageBoard.ArticleId = articleId;
            messageBoard.UserId = userID;
            messageBoard.Contents = message;
            messageBoard.MessageTime = DateOnly.FromDateTime(DateTime.Now);
            _context.messageBoard.Add(messageBoard);
            _context.SaveChanges();
            return message;
        }
        #endregion
        public IActionResult TestArticle()
        {
            var article = _context.article.FirstOrDefault(a => a.ArticleId == 4);
            var user = _context.users.FirstOrDefault();
            var data = new usersArticleModel()
            {
                article = article,
                user = user
            };
            return View(article);


        }
        //public void DisplayUserInfo()
        //{
        //    // 获取当前 HTTP 上下文
        //    HttpContext context = HttpContext.Current;

        //    // 检查当前用户是否已通过身份验证
        //    if (context.User.Identity.IsAuthenticated)
        //    {
        //        // 获取当前用户的身份信息
        //        IIdentity identity = context.User.Identity;

        //        // 输出当前用户的用户名
        //        string username = identity.Name;
        //        Console.WriteLine("Username: " + username);

        //        // 检查当前用户是否属于某个角色
        //        if (context.User.IsInRole("Admin"))
        //        {
        //            Console.WriteLine("User is in Admin role.");
        //        }
        //        else
        //        {
        //            Console.WriteLine("User is not in Admin role.");
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("User is not authenticated.");
        //    }
        //}

    }
}
