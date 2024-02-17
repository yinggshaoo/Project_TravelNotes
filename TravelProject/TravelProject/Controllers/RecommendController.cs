using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using TravelProject.Models;
namespace TravelProject.Controllers
{
    public class RecommendController : Controller
    {
        public IActionResult Index()
        {
            List<TestData> data = new List<TestData> 
            {
                new TestData { Id = 1, Label="台北", Image="./img/taipei/101.png", Title="台北101"},
                new TestData { Id = 2, Label="台中", Image="./img/taichung/大坑.jpg", Title = "台中健行"},
                new TestData { Id = 3, Label="台南", Image="./img/Tainan/府城.jpg", Title="台南美食"},
                new TestData { Id = 4, Label="高雄", Image="./img/Kaohsiung/85大樓.jpg", Title="高雄一日遊"}
            };

            return View(data);
        }

        [HttpPost]
        public IActionResult SearchResultPartialView(string[] GetLabel)
        {
            //var query = pokemon.Where(x => x != null);
            var query
                = from p in GetLabel
                  where p.Length > 0
                  select new TestData
                  {
                      Label = p,
                  };

            // 這裡會經過推薦演算法的處理後回傳List結果

            return PartialView("~/Views/Shared/SearchResultPartialView.cshtml", query.ToList());
        }

    }
}

