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

        //public IActionResult Index(string search)
        //{
        //    List<article> articles;
        //    if (!string.IsNullOrWhiteSpace(search))
        //    {
        //        // 当提供搜索条件时，根据Title和Contents搜索，并按LikeCount降序排序
        //        articles = _context.article
        //                    .Where(a => (a.Contents != null && a.Contents.Contains(search) && a.ArticleState == "發佈")
        //                             || (a.Title != null && a.Title.Contains(search) && a.ArticleState == "發佈"))
        //                    .OrderByDescending(a => a.LikeCount) // 按LikeCount从大到小排序
        //                    .ToList();
        //    }
        //    else
        //    {
        //        // 当没有提供搜索条件时，直接按LikeCount降序排序显示所有文章
        //        articles = _context.article
        //            .Where(a => a.ArticleState == "發佈")
        //                    .OrderByDescending(a => a.LikeCount) // 按LikeCount从大到小排序
        //                    .ToList();
        //    }
        //    //List<article> articles1 = _context.article.ToList();
        //    List<usersArticleModel> dataList = new List<usersArticleModel>();
        //    foreach (article article in articles)
        //    {
        //        users user = _context.users.FirstOrDefault(a => a.UserId == article.UserId);
        //        usersArticleModel data = new usersArticleModel();
        //        data.article = article;
        //        data.user = user;
        //        dataList.Add(data);
        //    }

        //    return View(dataList);
        //}
        public IActionResult Index(string search)
        {
            // 定义正则表达式来匹配HTML标签
            Regex htmlTagRegex = new Regex("<.*?>");

            // 检索所有状态为"發佈"的文章
            var articles = _context.article
                .Where(a => a.ArticleState == "發佈")
                .ToList() // 先将查询结果物化，将数据拉取到内存中
                .Where(a => !string.IsNullOrWhiteSpace(search) ?
                    (a.Contents != null && htmlTagRegex.Replace(a.Contents, "").Contains(search)) ||
                    (a.Title != null && a.Title.Contains(search)) : true)
                .OrderByDescending(a => a.LikeCount) // 对结果进行LikeCount降序排序
                .ToList();

            // 转换为usersArticleModel列表
            List<usersArticleModel> dataList = articles.Select(article => new usersArticleModel
            {
                article = article,
                user = _context.users.FirstOrDefault(u => u.UserId == article.UserId)
            }).ToList();

            return View(dataList);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
