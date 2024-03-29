using TravelNotes.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TravelNotes.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TravelContext _context;


        public HomeController(ILogger<HomeController> logger, TravelContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index(string search)
        {
            Regex htmlTagRegex = new Regex("<.*?>");

            // 从数据库检索所有状态为"發佈"的文章
            var articlesWithSpots = _context.article
                .Where(a => a.ArticleState == "發佈")
                .Join(_context.Spots, // 连接Spots表
                      article => article.SpotId,
                      spot => spot.SpotId,
                      (article, spot) => new { Article = article, Spot = spot })
                .ToList(); // 物化查询结果以便在内存中处理

            // 过滤: 如果提供了搜索字符串，就在内存中应用正则表达式去除HTML标签后进行搜索
            if (!string.IsNullOrWhiteSpace(search))
            {
                articlesWithSpots = articlesWithSpots
                    .Where(a => htmlTagRegex.Replace(a.Article.Contents ?? "", "").Contains(search) ||
                                a.Article.Title.Contains(search) ||
                                a.Spot.ScenicSpotName.Contains(search))
                    .ToList();
            }

            // 对结果进行LikeCount降序排序并转换为usersArticleModel列表
            var dataList = articlesWithSpots
                .OrderByDescending(a => a.Article.LikeCount)
                .Select(a => new usersArticleModel
                {
                    article = a.Article,
                    user = _context.users.FirstOrDefault(u => u.UserId == a.Article.UserId),
                    // 如果你还需要从Spots表中获取信息，你可以在这里添加
                    // 比如，SpotName = a.Spot.ScenicSpotName
                })
                .ToList();

            string userId;
            users loginUser = new users();
            if (Request.Cookies.TryGetValue("UsernameCookie", out userId))
            {
                loginUser = _context.users.FirstOrDefault(a => a.UserId == Convert.ToInt32(userId));
            }
            else
            {
                loginUser.UserId = 0;
            }
            ViewBag.loginUser = loginUser;

            return View(dataList);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
