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

            

            var articlesWithOrWithoutSpots = _context.article
                .Where(a => a.ArticleState == "發佈")
                .GroupJoin(_context.Spots,
                    article => article.SpotId,
                    spot => spot.SpotId,
                    (article, spots) => new { Article = article, Spots = spots })
                .SelectMany(
                    x => x.Spots.DefaultIfEmpty(),
                    (x, spot) => new { Article = x.Article, Spot = spot })
                .ToList();
            var groupJoinQuery = _context.articleOtherTags
            .GroupJoin(
                _context.OtherTags,
                articleOtherTag => articleOtherTag.OtherTagId,
                otherTag => otherTag.OtherTagId,
                (articleOtherTag, otherTags) => new
                {
                    ArticleId = articleOtherTag.ArticleId,
                    OtherTags = otherTags
                }
            ).ToList();
            var combinedResults = articlesWithOrWithoutSpots
                .GroupJoin(
                    groupJoinQuery,
                    articleWithOrWithoutSpot => articleWithOrWithoutSpot.Article.ArticleId,
                    groupJoinResult => groupJoinResult.ArticleId,
                    (articleWithOrWithoutSpot, groupJoinResultCollection) => new
                    {
                        Article = articleWithOrWithoutSpot.Article,
                        Spot = articleWithOrWithoutSpot.Spot,
                        OtherTags = groupJoinResultCollection.SelectMany(gjr => gjr.OtherTags).ToList()
                    }).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                combinedResults = combinedResults
                    .Where(a => htmlTagRegex.Replace(a.Article.Contents ?? "", "").Contains(search) ||
                                a.Article.Title.Contains(search) ||
                                a.Spot != null && a.Spot.ScenicSpotName.Contains(search) ||
                                a.OtherTags.Any(tag => tag.OtherTagName.Contains(search)))
                    .ToList();
            }

            var dataList = combinedResults
                .OrderByDescending(a => a.Article.LikeCount + a.Article.PageView)
                .Select(a => new usersArticleModel
                {
                    article = a.Article,
                    user = _context.users.FirstOrDefault(u => u.UserId == a.Article.UserId),
                    OtherTagIds = a.OtherTags?.Select(ot => ot.OtherTagId).ToList() ?? new List<int>()
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
