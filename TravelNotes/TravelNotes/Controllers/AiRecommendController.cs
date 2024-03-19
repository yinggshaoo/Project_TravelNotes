using TravelNotes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using TravelNotes;


namespace Lab0225_InitProject.Controllers
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

        [HttpGet]
        public List<Spots> DefaultPage(string city)
        {
            var spots = _context.Spots.Where(x => x.City == city)
                                       .Skip(0)
                                       .Take(10)
                                       .ToList();
            return spots;
        }

        [HttpGet]
        public List<Spots> Itinerary(string city, string currentPage)
        {
            int pageSize = 20; // 定義每頁顯示的記錄數

            int totalSpots = _context.Spots.Where(x => x.City == city).Count();
            int totalPages = (int)Math.Ceiling((double)totalSpots / pageSize); // 總頁數

            int currentPageIndex = Convert.ToInt32(currentPage); // 當前頁碼
            if (currentPageIndex < 1)
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

        public List<Spots> MlHandel(string Interests1, string Interests2, string Interests3,string weather, string country)
        {
            var sampleData = new TravelML.ModelInput()
            {
                Col0 = weather,
                Col1 = Interests1,
                Col2 = Interests2,
                Col3 = Interests3,
                Col4 = country,
            };

            //Load model and predict output
            var result = TravelML.Predict(sampleData);
            string prediction = result.PredictedLabel;
            var query = _context.Spots.Where(x => x.City == prediction);
            var ReturnFontend = Shuffle(query.ToList());

            return ReturnFontend.ToList();

            //return $"OK-POST-V2-{1}-{Interests1}-{Interests2}-{Interests3}-{weather}-{country}-{prediction}";
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

        [HttpGet]
        public Array Subtotal()
        {
            var statisticsCity = _context.Spots
                            .GroupBy(s => s.City)
                            .Select(g => new { City = g.Key, Count = g.Count() })
                            .ToList();

            return statisticsCity.ToArray();
        }

        [HttpPost]
        public int Cut(int number)
        {
            int totalPages = number / 20;
            return totalPages;
        }

        

        

    }
}

