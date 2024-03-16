using TravelNotes.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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
            //IEnumerable<article> articles;

            //if (!string.IsNullOrWhiteSpace(search))
            //{
            //    // 当提供搜索条件时，根据Title和Contents搜索，并按LikeCount降序排序
            //    articles = _context.article
            //                .Where(a => (a.Contents != null && a.Contents.Contains(search) && a.ArticleState=="發佈")
            //                         || (a.Title != null && a.Title.Contains(search) && a.ArticleState == "發佈"))
            //                .OrderByDescending(a => a.LikeCount) // 按LikeCount从大到小排序
            //                .ToList();
            //}
            //else
            //{
            //    // 当没有提供搜索条件时，直接按LikeCount降序排序显示所有文章
            //    articles = _context.article
            //        .Where(a => a.ArticleState == "發佈")
            //                .OrderByDescending(a => a.LikeCount) // 按LikeCount从大到小排序
            //                .ToList();
            //}

            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
