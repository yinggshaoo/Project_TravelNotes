using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TravelNotes.Models;

namespace TravelNotes.Controllers
{
    
    public class AlbumController : Controller
    {
        int userId = 0;
        private readonly ILogger<AlbumController> _logger;
        private readonly TravelContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public AlbumController(ILogger<AlbumController> logger, TravelContext context, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Photo(int? userId)
        {
            string cookieValue;
            int UserId;
            int login = 0;
            if (userId == null)
            {
                if (Request.Cookies.TryGetValue("UsernameCookie", out  cookieValue))
                {
                    UserId = Convert.ToInt32(cookieValue);
                    login = UserId;
                }
                else
                {
                    return RedirectToAction("Login", "Member");
                }
            }
            else {
                UserId = (int)userId;
            }
            var photos = _context.photo
                                 .Where(p => p.UserId == UserId && p.PhotoDescription == null && p.AlbumId == null)
                                 .Select(p => new photo
                                 {
                                     PhotoPath = p.PhotoPath,
                                     PhotoId = p.PhotoId,
                                     UploadDate = p.UploadDate
                                 }).ToList();
            bool IsMyPage = login == UserId;
            ViewBag.IsMyPage = IsMyPage;
            ViewBag.UserPage = userId;

            return View(photos);
        }
        public async Task<IActionResult> Album(int? userId)
        {
            string cookieValue;
            int UserId;
            int login = 0;
            if (userId == null)
            {
                if (Request.Cookies.TryGetValue("UsernameCookie", out cookieValue))
                {
                    UserId = Convert.ToInt32(cookieValue);
                    login = UserId;
                }
                else
                {
                    return RedirectToAction("Login", "Member");
                }
            }
            else
            {
                UserId = (int)userId;
            }
            bool albumExists = _context.album.Any(a => a.AlbumName == $"Garbage{UserId}" && a.UserId == UserId);
            if (!albumExists)
            {
                var album = new album
                {
                    AlbumName = $"Garbage{UserId}",
                    CreateTime = DateOnly.FromDateTime(DateTime.Now),
                    UserId = (int)UserId,
                    State = 1,
                };
                _context.album.Add(album);
                await _context.SaveChangesAsync();
            }
            var viewModelList = new List<AlbumPhotosViewModel>();

            var minAlbumId = await _context.album
                                            .Where(a => a.UserId == UserId)
                                            .MinAsync(a => (int?)a.AlbumId);

            var albumsExceptMin = await _context.album
                                                 .Where(a => a.UserId == UserId && a.AlbumId != minAlbumId && a.State == 2)
                                                 .ToListAsync();
            foreach (var album in albumsExceptMin)
            {
                var photos = await _context.photo.Where(p => p.AlbumId == album.AlbumId && p.PhotoDescription == null).ToListAsync();
                viewModelList.Add(new AlbumPhotosViewModel { Album = album, Photos = photos });
            }
            bool IsMyPage = login == UserId;
            ViewBag.IsMyPage = IsMyPage;
            ViewBag.UserPage = userId;
            return View(viewModelList);
        }
        public async Task<IActionResult> Garbage(int? userId)
        {
            string cookieValue;
            int UserId;
            int login = 0;
            if (userId == null)
            {
                if (Request.Cookies.TryGetValue("UsernameCookie", out cookieValue))
                {
                    UserId = Convert.ToInt32(cookieValue);
                    login = UserId;
                }
                else
                {
                    return RedirectToAction("Login", "Member");
                }
            }
            else
            {
                UserId = (int)userId;
            }
            var minAlbumId = await _context.album
                                     .Where(a => a.UserId == UserId && a.State == 1)
                                     .MinAsync(a => (int?)a.AlbumId);

            var albums = await _context.album
                                       .Where(a => a.UserId == UserId && a.State == 1 && (!minAlbumId.HasValue || a.AlbumId != minAlbumId.Value))
                                       .ToListAsync();

            var targetAlbum = await _context.album
                                            .Where(a => a.UserId == UserId)
                                            .OrderBy(a => a.AlbumId)
                                            .FirstOrDefaultAsync();

            if (targetAlbum == null)
            {
                return NotFound();
            }
            var photos = await _context.photo
                              .Where(p => p.UserId == UserId && p.PhotoDescription == "1")
                              .ToListAsync();

            var viewModel = new GarbageViewModel
            {
                Photos = photos,
                Albums = albums
            };
            bool IsMyPage = login == UserId;
            ViewBag.IsMyPage = IsMyPage;
            return View(viewModel);
        }
        //上傳相片方法
        [HttpPost]
        public async Task<IActionResult> UploadImages(List<IFormFile> imageFiles)
        {
            string cookieValue;
            bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue!);
            if (result)
            {
                userId = Convert.ToInt32(cookieValue);

            }
            if (imageFiles != null && imageFiles.Any(f => f.Length > 0))
            {
                foreach (var imageFile in imageFiles)
                {
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/img/user{userId}/photo");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    var originalFileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageFile.FileName);
                    var extension = Path.GetExtension(imageFile.FileName);
                    var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    var newFileName = $"{originalFileNameWithoutExtension}_{timeStamp}{extension}";
                    var webPath = $"/img/user{userId}/photo/{newFileName}";
                    var absolutePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/img/user{userId}/photo", newFileName);

                    using (var stream = new FileStream(absolutePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    var photo = new photo
                    {
                        PhotoTitle = $"{originalFileNameWithoutExtension}_{timeStamp}",
                        PhotoPath = webPath,
                        UploadDate = DateOnly.FromDateTime(DateTime.Now),
                        UserId = userId,
                    };
                    _context.photo.Add(photo);
                    await _context.SaveChangesAsync();
                }
                TempData["Message"] = "上傳成功";
                return RedirectToAction("Photo");
            }
            else
            {
                TempData["Message"] = "請選擇一張相片上傳";
                return RedirectToAction("Photo");
            }
        }
        //Photo刪除相片方法
        [HttpPost]
        public async Task<IActionResult> UpdateAlbumId(int photoId, DateOnly ReplaceTime)
        {
            string cookieValue;
            bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue!);
            if (result)
            {
                userId = Convert.ToInt32(cookieValue);

            }
            var photo = await _context.photo.Where(p => p.AlbumId == null & p.PhotoId == photoId).FirstOrDefaultAsync();
            var NowTime = DateTime.Today;
            string formattedDate = NowTime.ToString("yyyy-MM-dd");
            DateOnly date = DateOnly.Parse(formattedDate);
            if (photo == null)
            {
                return NotFound();
            }

            photo.PhotoDescription = "1";

            photo.UploadDate = date;

            await _context.SaveChangesAsync();


            return RedirectToAction("Photo");
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePhotoDescription([FromBody] UpdateDescriptionModel model)
        {
            string cookieValue;
            bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue!);
            if (result)
            {
                userId = Convert.ToInt32(cookieValue);

            }
            var photo = await _context.photo.FindAsync(model.PhotoId);
            if (photo == null)
            {
                return NotFound();
            }

            photo.PhotoDescription = model.NewDescription;
            await _context.SaveChangesAsync();

            return Ok();
        }
        [HttpPost]
        public IActionResult UpdateALLPhotoDescription([FromBody] UpdateALLPhotoDescriptionRequest request)
        {
            string cookieValue;
            bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue!);
            if (result)
            {
                userId = Convert.ToInt32(cookieValue);

            }
            var photos = _context.photo.Where(p => request.PhotoIds.Contains(p.PhotoId)).ToList();

            if (photos.Count != request.PhotoIds.Count)
            {
                return NotFound("One or more photos not found.");
            }

            foreach (var photo in photos)
            {
                photo.PhotoDescription = request.NewDescription;
            }

            _context.SaveChanges();

            return Ok();
        }
        //相簿圖片刪除方法
        [HttpPost]
        public async Task<IActionResult> AlbumPhotoToGarbage(int photoId, DateOnly ReplaceTime)
        {
            string cookieValue;
            bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue!);
            if (result)
            {
                userId = Convert.ToInt32(cookieValue);

            }
            var photo = await _context.photo.Where(p => p.PhotoId == photoId).FirstOrDefaultAsync();
            var NowTime = DateTime.Today;
            string formattedDate = NowTime.ToString("yyyy-MM-dd");
            DateOnly date = DateOnly.Parse(formattedDate);
            if (photo == null)
            {
                return NotFound();
            }
            photo.PhotoDescription = "1";
            photo.UploadDate = date;
            await _context.SaveChangesAsync();
            return RedirectToAction("Album");
        }

        //創建Album方法
        [HttpPost]
        public async Task<IActionResult> CreateFolder(string folderName)
        {
            string cookieValue;
            bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue!);
            if (result)
            {
                userId = Convert.ToInt32(cookieValue);

            }
            if (!string.IsNullOrEmpty(folderName))
            {
                var albumNameExists = await _context.album
                                                    .AnyAsync(a => a.UserId == userId &&
                                                                   a.AlbumName.ToLower() == folderName.ToLower());
                if (albumNameExists)
                {
                    TempData["Message"] = "相簿名稱已存在";
                    return RedirectToAction("Album");
                }

                var path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/img/user{userId}/album", folderName);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var album = new album
                {
                    UserId = userId,
                    AlbumName = folderName,
                    CreateTime = DateOnly.FromDateTime(DateTime.Now),
                    State = 2
                };
                _context.album.Add(album);
                await _context.SaveChangesAsync();
                TempData["Message"] = "新增相簿成功";
            }
            else
            {
                TempData["Message"] = "相簿名稱不能為空";
            }
            return RedirectToAction("Album");
        }
        //在指定Album上傳Photo方法
        [HttpPost]
        public async Task<IActionResult> UploadPhotosToAlbum(List<IFormFile> imageFiles, int albumId, string albumName)
        {
            string cookieValue;
            bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue!);
            if (result)
            {
                userId = Convert.ToInt32(cookieValue);

            }
            if (imageFiles != null && imageFiles.Any(f => f.Length > 0))
            {
                foreach (var imageFile in imageFiles)
                {
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/img/user{userId}/album/{albumName}");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    var originalFileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageFile.FileName);
                    var extension = Path.GetExtension(imageFile.FileName);
                    var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    var newFileName = $"{originalFileNameWithoutExtension}_{timeStamp}{extension}";

                    var webPath = $"/img/user{userId}/album/{albumName}/{newFileName}";
                    var absolutePath = Path.Combine(_hostingEnvironment.WebRootPath, $"img/user{userId}/album/{albumName}", newFileName);

                    using (var stream = new FileStream(absolutePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    var photo = new photo
                    {
                        PhotoTitle = $"{originalFileNameWithoutExtension}_{timeStamp}",
                        PhotoPath = webPath,
                        UploadDate = DateOnly.FromDateTime(DateTime.Now),
                        UserId = userId,
                        AlbumId = albumId,
                    };
                    _context.photo.Add(photo);

                    await _context.SaveChangesAsync();
                }
                TempData["Message"] = "上傳成功";
                return RedirectToAction("Album");
            }
            else
            {
                TempData["Message"] = "請選擇至少一張相片上傳";
                return RedirectToAction("Album");
            }
        }

        //Garbage刪除全部東西
        [HttpPost]
        public async Task<IActionResult> DeleteByAlbumAndUserId()
        {
            string cookieValue;
            bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue!);
            if (result)
            {
                userId = Convert.ToInt32(cookieValue);

            }
            var minAlbumId = await _context.album
                                    .Where(a => a.UserId == userId && a.State == 1)
                                    .MinAsync(a => (int?)a.AlbumId);

            if (!minAlbumId.HasValue)
            {
                return NotFound("No albums found for the specified conditions.");
            }
            var photosInMinAlbumToDelete = await _context.photo
                                                        .Where(p => p.UserId == userId && p.PhotoDescription == "1")
                                                        .ToListAsync();
            _context.photo.RemoveRange(photosInMinAlbumToDelete);

            var albumsToDelete = await _context.album
                                                .Where(a => a.UserId == userId && a.State == 1 && a.AlbumId != minAlbumId.Value)
                                                .Include(a => a.photo)
                                                .ToListAsync();

            if (albumsToDelete.Count == 0 && photosInMinAlbumToDelete.Count == 0)
            {
                return NotFound("No albums or photos to delete based on the specified conditions.");
            }

            foreach (var album in albumsToDelete)
            {
                _context.photo.RemoveRange(album.photo);
                _context.album.Remove(album);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Garbage");
        }

        //Garbage刪除所有Album
        [HttpPost]
        public async Task<IActionResult> DeleteAllAlbum()
        {
            string cookieValue;
            bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue!);
            if (result)
            {
                userId = Convert.ToInt32(cookieValue);

            }
            var minAlbumId = await _context.album
                                    .Where(a => a.UserId == userId && a.State == 1)
                                    .MinAsync(a => (int?)a.AlbumId);

            if (!minAlbumId.HasValue)
            {
                return NotFound("No albums found for the specified conditions.");
            }

            var albumsToDelete = await _context.album
                                                .Where(a => a.UserId == userId && a.State == 1 && a.AlbumId != minAlbumId.Value)
                                                .Include(a => a.photo)
                                                .ToListAsync();

            if (albumsToDelete.Count == 0)
            {
                return NotFound("No albums to delete except the one with the minimum AlbumId.");
            }
            foreach (var album in albumsToDelete)
            {
                var photosToDelete = album.photo.Where(p => p.PhotoDescription != "1").ToList();

                _context.photo.RemoveRange(photosToDelete);
                _context.album.Remove(album);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Garbage");
        }
        //Garbage刪除所有Photo
        [HttpPost]
        public async Task<IActionResult> DeleteAllPhoto()
        {
            string cookieValue;
            bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue!);
            if (result)
            {
                userId = Convert.ToInt32(cookieValue);

            }
            var photosToDelete = await _context.photo
                                               .Where(p => p.UserId == userId && p.PhotoDescription == "1")
                                               .ToListAsync();
            if (!photosToDelete.Any())
            {
                return NotFound("No photos found with the specified AlbumId and UserId.");
            }

            _context.photo.RemoveRange(photosToDelete);
            await _context.SaveChangesAsync();


            return RedirectToAction("Garbage");
        }
        //Garbage刪除單張Photo
        [HttpPost]
        public async Task<IActionResult> GarbageDelete(int photoId)
        {
            string cookieValue;
            bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue!);
            if (result)
            {
                userId = Convert.ToInt32(cookieValue);

            }
            var photosToDelete = await _context.photo
                                               .Where(p => p.PhotoId == photoId)
                                               .ToListAsync();

            if (!photosToDelete.Any())
            {
                return NotFound("No photos found with the specified AlbumId and UserId.");
            }
            _context.photo.RemoveRange(photosToDelete);
            await _context.SaveChangesAsync();

            return Ok("成功");
        }
        //刪除相簿到垃圾桶
        [HttpPost]
        public async Task<IActionResult> DeleteAlbum(int albumId)
        {
            string cookieValue;
            bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue!);
            if (result)
            {
                userId = Convert.ToInt32(cookieValue);

            }
            var album = await _context.album
                       .FirstOrDefaultAsync(a => a.AlbumId == albumId);

            if (album == null)
            {
                return NotFound();
            }

            album.State = 1;
            album.CreateTime = DateOnly.FromDateTime(DateTime.Now);

            _context.Entry(album).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Photo));
        }
        //修改相簿名稱
        [HttpPost]
        public async Task<IActionResult> EditAlbumName(int albumId, string newAlbumName)
        {
            string cookieValue;
            bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue!);
            if (result)
            {
                userId = Convert.ToInt32(cookieValue);

            }
            var album = await _context.album.FirstOrDefaultAsync(a => a.AlbumId == albumId);
            if (album == null)
            {
                return Json(new { success = false, message = "找不到指定的相簿。" });
            }

            var albumNameExists = await _context.album
                                                .AnyAsync(a => a.UserId == userId &&
                                                               a.AlbumId != albumId &&
                                                               a.AlbumName.ToLower() == newAlbumName.ToLower());
            if (albumNameExists)
            {
                return Json(new { success = false, message = "相簿名稱已存在。" });
            }

            album.AlbumName = newAlbumName;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
        //Garbage還原相片到Photo
        [HttpPost]
        public async Task<IActionResult> ReturnAlbumId(int photoId, DateOnly ReplaceTime)
        {
            string cookieValue;
            bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue!);
            if (result)
            {
                userId = Convert.ToInt32(cookieValue);

            }
            var photo = await _context.photo.Where(p => p.PhotoId == photoId).FirstOrDefaultAsync();
            var NowTime = DateTime.Today;
            string formattedDate = NowTime.ToString("yyyy-MM-dd");
            DateOnly date = DateOnly.Parse(formattedDate);
            if (photo == null)
            {
                return NotFound();
            }
            photo.UploadDate = date;
            if (photo.PhotoDescription == "1")
            {
                photo.PhotoDescription = null;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Garbage");
        }
        [HttpPost]
        public async Task<IActionResult> RestoreAlbum(int albumId)
        {
            string cookieValue;
            bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue!);
            if (result)
            {
                userId = Convert.ToInt32(cookieValue);

            }
            var album = await _context.album.FindAsync(albumId);
            if (album != null)
            {
                album.State = 2;
                await _context.SaveChangesAsync();
                return RedirectToAction("Garbage");
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateNullAlbumPhotosToMinAlbumId()
        {
            string cookieValue;
            bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue!);
            if (result)
            {
                userId = Convert.ToInt32(cookieValue);

            }
            var photosToUpdate = await _context.photo
                                               .Where(p => p.UserId == userId && p.AlbumId == null)
                                               .ToListAsync();

            if (!photosToUpdate.Any())
            {
                return NotFound("No photos found with NULL AlbumId for the specified UserId.");
            }

            photosToUpdate.ForEach(p => p.PhotoDescription = "1");

            await _context.SaveChangesAsync();

            return RedirectToAction("Photo");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAlb(int albumId)
        {
            string cookieValue;
            bool result = Request.Cookies.TryGetValue("UsernameCookie", out cookieValue!);
            if (result)
            {
                userId = Convert.ToInt32(cookieValue);

            }
            var album = await _context.album.Include(a => a.photo).FirstOrDefaultAsync(a => a.AlbumId == albumId);
            if (album != null)
            {
                var OtherPhoto = album.photo.Where(p => p.PhotoDescription == "1" && p.UserId == userId).ToList();
                foreach (var photo in OtherPhoto)
                {
                    photo.AlbumId = null;
                    await _context.SaveChangesAsync();
                }
                
                foreach (var photo in album.photo.ToList())
                {
                    _context.photo.Remove(photo);
                }

                _context.album.Remove(album);
                await _context.SaveChangesAsync();
                TempData["Message"] = "相簿及其圖片刪除成功";
            }
            else
            {
                TempData["ErrorMessage"] = "找不到相簿，刪除失敗";
            }

            return RedirectToAction("Garbage");
        }

    }
}
