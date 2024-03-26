using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using System.Text.Json;
using TravelNotes.Models;
using Microsoft.VisualBasic;
using System.Web;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace TravelNotes.Controllers

{
    public class ArticleController : Controller
    {
        private readonly object _lockObject = new object();
        //int userID = 1;
        private readonly TravelContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ArticleController(IWebHostEnvironment hostingEnvironment, TravelContext context)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
            
			

		}
        #region 圖片相關
        [HttpPost]
        public void Test()
        {

        }
        [HttpPost]
        public void CheckFolderPhotos(string contentImages, int articleId,string type)
        {
            string userId;
            Request.Cookies.TryGetValue("UsernameCookie", out userId);

            // 解析 contentImages 字符串为对象
            List<string> imageList = JsonSerializer.Deserialize<List<string>>(contentImages)!;
            string directoryPath = Path.Combine(_hostingEnvironment.WebRootPath, $"img\\user{userId}\\article\\{articleId}\\{type}");

            if(!Directory.Exists(directoryPath))
            {
                return;
            }
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
            string userId;
            Request.Cookies.TryGetValue("UsernameCookie", out userId);
            string imgFolder = Path.Combine(_hostingEnvironment.WebRootPath, "img");
            if (!Directory.Exists(imgFolder))
            {
                Directory.CreateDirectory(imgFolder);
            }
            string userFolder = Path.Combine(imgFolder, "user" + userId);
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

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				file.CopyTo(stream);
			}


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
            string fileName = srcString.Substring(srcString.LastIndexOf('/') + 1);
            string folderImage = JsonSerializer.Serialize(new List<string>() { fileName });
            CheckFolderPhotos(folderImage, articleId, "articleImage");
            
            var currentArticle = _context.article.FirstOrDefault(x => x.ArticleId == articleId);
            currentArticle!.Images = srcString;
            
            _context.SaveChanges();
            return srcString;
        }
        #endregion
        #region 文章相關

        [HttpGet]
        public IActionResult Draft(int? articleId)
        {
            string userId;
            if (!Request.Cookies.TryGetValue("UsernameCookie", out userId))
            {
                return RedirectToAction("Login", "Member");
            }
            
            article target;
            if (articleId == null)
            {
                target = _context.article.Where(a => a.UserId == Convert.ToInt32(userId) && a.ArticleState == "草稿").OrderBy(a=>a.ArticleId).LastOrDefault()!;

            }
            else
            {
                target = _context.article.FirstOrDefault(a => a.ArticleId == articleId)!;
            }
            if (target == null)
            {
                return RedirectToAction("CreateDraft");
            }
            if (CheckOwner(target.UserId.ToString()))
            {
                RedirectToAction("errorView");
            }
            ViewBag.target = target;
            var data = _context.article.Where(a => a.UserId == Convert.ToInt32(userId) && a.ArticleState == "草稿").ToList();
            ViewBag.spotTagList = _context.Spots.ToList();
            ViewBag.otherTagList = _context.OtherTags.ToList();
            List<int> otherTagsIds = _context.articleOtherTags.Where(a => a.ArticleId == target.ArticleId).Select(a => a.OtherTagId).ToList();
            ViewBag.currentOtherTags = _context.OtherTags.Where(a=>otherTagsIds.Contains(a.OtherTagId)).ToList();
            if (data == null)
            {
                return View("errorView");
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(int articleId, string title, string subtitle, DateTime travelTime, 
            string content,string contentImages,int SpotId,string ScenicSpotName,string City,string currentOtherTags)
        {
            lock (_lockObject)
            {
                CheckFolderPhotos(contentImages, articleId, "content");
                var currentArticle = _context.article.FirstOrDefault(x => x.ArticleId == articleId);
                if (SpotId == 0)
                {
                    if (!string.IsNullOrEmpty(ScenicSpotName)) 
                    {
                        Spots newSpot = new Spots();
                        newSpot.ScenicSpotName = ScenicSpotName;
                        newSpot.City = City;
                        _context.Spots.Add(newSpot);
                        _context.SaveChanges();
                        var orderSpots = _context.Spots.OrderBy(a => a.SpotId);
                        currentArticle!.SpotId = orderSpots.LastOrDefault()!.SpotId;
                    }
                    
                }
                else
                {
                    currentArticle!.SpotId = SpotId;
                }
                List<OtherTags> articleOtherTags = JsonSerializer.Deserialize<List<OtherTags>>(currentOtherTags)!;
                List<int> myOtherTagId = new List<int>();
                for(int i = 0; i < articleOtherTags.Count; i++)
                {
                    if (articleOtherTags[i].OtherTagId == 0)
                    {
                        List<string> OtherTagNames = _context.OtherTags.Select(a => a.OtherTagName).ToList();
                        if (!OtherTagNames.Contains(articleOtherTags[i].OtherTagName))
                        {
                            OtherTags newOtherTag = new OtherTags();
                            newOtherTag.OtherTagName = articleOtherTags[i].OtherTagName;
                            _context.OtherTags.Add(newOtherTag);
                            _context.SaveChanges();
                            myOtherTagId.Add(_context.OtherTags.FirstOrDefault(a=>a.OtherTagName == newOtherTag.OtherTagName)!.OtherTagId);
                        }
                    }
                    else
                    {
                        myOtherTagId.Add(articleOtherTags[i].OtherTagId);
                    }
                }
                foreach (var item in _context.articleOtherTags.Where(a=>a.ArticleId == articleId)) 
                {
                    _context.articleOtherTags.Remove(item);
                }
                _context.SaveChanges();
                foreach(int tagId in myOtherTagId)
                {
                    articleOtherTags aot = new articleOtherTags();
                    aot.ArticleId = articleId;
                    aot.OtherTagId = tagId;
                    _context.articleOtherTags.Add(aot);
                }
                currentArticle!.Title = title;
                currentArticle.Subtitle = subtitle;
                currentArticle.TravelTime = travelTime;
                currentArticle.Contents = content;
                _context.SaveChanges();
                return Ok();
            }
               
        }
        public IActionResult CreateDraft(int? SpotId)
        {
            string userId;
            if (!Request.Cookies.TryGetValue("UsernameCookie", out userId))
            {
                return RedirectToAction("Login", "Member");
            }
            article article = new article();
            article.UserId = Convert.ToInt32(userId);//之後要改
            article.Title = "";
            article.Subtitle = "";
            article.TravelTime = DateTime.Now;
            article.Contents = "";
            article.Images = "";
            article.LikeCount = 0;
            article.PageView = 0;
            article.ArticleState = "草稿";
            article.SpotId = SpotId;
            _context.Add(article);
            _context.SaveChanges();
            return RedirectToAction("Draft");
        }

        [HttpPost]
        public IActionResult PublishDraft(int articleId)
        {
            string userId;
            if (!Request.Cookies.TryGetValue("UsernameCookie", out userId))
            {
                return RedirectToAction("Login", "Member");
            }
            article articleToPublish = _context.article.FirstOrDefault(a => a.ArticleId == articleId)!;
            if (CheckOwner(articleToPublish.UserId.ToString()))
            {
                RedirectToAction("errorView");
            }
            articleToPublish.PublishTime = DateTime.Now;
            articleToPublish.ArticleState = "發佈";
            _context.SaveChanges(); // 保存更改

            return Ok();
        }

        [HttpPost]
        public IActionResult Delete(int articleId)
        {
            string userId;
            if (!Request.Cookies.TryGetValue("UsernameCookie", out userId))
            {
                return RedirectToAction("Login", "Member");
            }
            article articleToDelete = _context.article.FirstOrDefault(a => a.ArticleId == articleId)!;
            if (CheckOwner(articleToDelete.UserId.ToString()))
            {
                RedirectToAction("errorView");
            }
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
                            Haedshot = u.Headshot,
                        };
            ViewBag.messageBoards = messageBoards;
            users user = _context.users.FirstOrDefault(a => a.UserId == data.UserId)!;
            ViewBag.user = user;
            Spots spots = _context.Spots.FirstOrDefault(a=>a.SpotId == data.SpotId)!;
            ViewBag.spot = spots;
            List<int> otherTagsIds = _context.articleOtherTags.Where(a => a.ArticleId == data.ArticleId).Select(a => a.OtherTagId).ToList();
            ViewBag.otherTags = _context.OtherTags.Where(a => otherTagsIds.Contains(a.OtherTagId)).ToList();
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
            string userId;
            if (!Request.Cookies.TryGetValue("UsernameCookie", out userId))
            {
                return "Not Login";
            }
            messageBoard messageBoard = new messageBoard();
            messageBoard.ArticleId = articleId;
            messageBoard.UserId = Convert.ToInt32(userId);
            messageBoard.Contents = message;
            messageBoard.MessageTime = DateTime.Now;
            _context.messageBoard.Add(messageBoard);
            _context.SaveChanges();
            return "Ok";
        }
        #endregion
        public bool CheckOwner(string checkId)
        {
            string userId;
            if (!Request.Cookies.TryGetValue("UsernameCookie", out userId))
            {
                return false;
            }
            if(userId != checkId)
            {
                return false;
            }
            return true;
        }
        public IActionResult TestArticle()
        {
            bool isUser = User.Identity.IsAuthenticated;



            return View();
        }
        
    }
}
