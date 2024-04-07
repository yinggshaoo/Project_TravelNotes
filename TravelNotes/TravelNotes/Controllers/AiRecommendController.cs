using TravelNotes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using TravelNotes;


namespace TravelNotes.Controllers
{
    public class AiRecommendController : Controller
    {
        private readonly TravelContext _context;

        public AiRecommendController(TravelContext context)
        {
            _context = context;
        }
 
        public async Task<IActionResult> Index()
        {
            var tagLists = await _context.TagList.ToListAsync();
            var spots = await _context.Spots.ToListAsync();
            var cityList = await _context.UniqueCity.ToListAsync();

            ArrayList myAL = new ArrayList();
            myAL.Add(tagLists);
            myAL.Add(spots);
            myAL.Add(cityList);
            return View(myAL.ToArray());
        }
        
        // AI推薦功能
        public IActionResult MlHandel(string Interests1, string Interests2, string Interests3, string weather, string country)
        {

            var sampleData = new TravelModel5.ModelInput()
            {
                Col0 = weather,
                Col1 = Interests1,
                Col2 = Interests2,
                Col3 = Interests3,
                Col4 = country,
            };

            //Load model and predict output
            var result = TravelModel5.Predict(sampleData);
            string prediction = result.PredictedLabel;
            //TempData["prediction"] = prediction;

            var answer = new List<Spots>();
            var additional = new List<Spots>();

            if(prediction == "太魯")
            {
                prediction = "太魯閣";
            }

            if (prediction != null && Interests1!= null && Interests2 != null && Interests3 != null && weather != null && country != null)
            {
                answer = (from s in _context.Spots
                          where s.ScenicSpotName.Contains(prediction)
                          select s).ToList();

                if (answer.Count > 1)
                {
                    Random rand = new Random();
                    answer = answer.OrderBy(x => rand.Next()).ToList();
                }

                // 如果 prediction 不為 null，根據預測值查找相符的景點
                additional = (from o in _context.Spots
                              where o.DescriptionDetail.Contains(prediction)
                              select o).ToList();

                additional = additional.Take(10).ToList();
            }
            else
            {
                var allSpots = _context.Spots.ToList(); // 取得所有的景點
                allSpots = (List<Spots>)Shuffle(allSpots); // 隨機排序所有的景點
                answer = allSpots.Take(10).ToList(); // 選擇前10個景點
            }

            // 大風 野餐 瑜珈 攀岩 巴西
            return new JsonResult(new object[] { prediction, answer[0], additional });
        }

        private IList<T> Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            Random random = new Random();
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }


        //區域過濾功能
        [HttpGet]
        public List<Spots> Itinerary(string city, string currentPage)
        {
            int pageSize = 20; // 定義每頁顯示的記錄數

            int totalSpots = _context.Spots.Where(x => x.City == city).Count();
            int totalPages = (int)Math.Ceiling((double)totalSpots / pageSize); // 總頁數

            int currentPageIndex = Convert.ToInt32(currentPage); // 當前頁碼
            if (currentPageIndex <= 1)
            {
                currentPageIndex = 1; // 如果 currentPage 小於 1，則設置為第一頁
            }
            else if (currentPageIndex > totalPages)
            {
                currentPageIndex = totalPages; // 如果 currentPage 大於總頁數，則設置為最後一頁
            }

            int skip = (currentPageIndex - 1) * pageSize; // 計算要跳過的記錄數

            var spots = _context.Spots.Where(x => x.City == city)
                                       .Skip(skip)
                                       .Take(pageSize)
                                       .ToList();

            return spots;
        }

        public int PagesNumber(string citiesValue) {
            var query = (from c in _context.Spots
                        where c.City == citiesValue
                        select c).ToList();
            int result = query.Count;

            return result;
        }



        //0407廢棄
        [HttpGet]
        public Array Subtotal()
        {
            var statisticsCity = _context.Spots
                            .GroupBy(s => s.City)
                            .Select(g => new { City = g.Key, Count = g.Count() })
                            .ToList();

            return statisticsCity.ToArray();
        }

        //0407廢棄
        [HttpPost]
        public int Cut(int number)
        {
            int totalPages = number / 20;
            return totalPages;
        }
    }
}

