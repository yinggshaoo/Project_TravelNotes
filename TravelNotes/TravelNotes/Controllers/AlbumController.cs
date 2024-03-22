using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TravelNotes.Models;

namespace TravelNotes.Controllers
{
    
    public class AlbumController : Controller
    {
        int userId = 1;
        private readonly ILogger<AlbumController> _logger;
        private readonly TravelContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public AlbumController(ILogger<AlbumController> logger, TravelContext context, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Photo()
        {
            var data = from p in _context.photo
                           //UserId要抓  ****(UserId要更換)***  (AlbumId is NULL 代表只是相片 )
                       where p.UserId == userId && p.PhotoDescription == null && p.AlbumId == null
                       select new photo
                       {
                           PhotoPath = p.PhotoPath,
                           PhotoId = p.PhotoId,
                           UploadDate = p.UploadDate
                       };
            //回傳Travel資料庫裡Photo表的所有PhotoPath
            return View(data.ToList());
        }
        public async Task<IActionResult> Album()
        {
            //一上傳就先創一個垃圾桶
            var GarbagePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/img/user{userId}/garbage");
            Directory.CreateDirectory(GarbagePath);
            bool albumExists = _context.album.Any(a => a.AlbumName == $"Garbage{userId}" && a.UserId == userId);
            if (!albumExists)
            {
                var album = new album
                {
                    AlbumName = $"Garbage{userId}",
                    CreateTime = DateOnly.FromDateTime(DateTime.Now),
                    UserId = userId,
                    State = 1,//在垃圾桶
                };
                _context.album.Add(album);
                await _context.SaveChangesAsync();
            }
            var viewModelList = new List<AlbumPhotosViewModel>();

            // 首先找到UserId等於2的最小AlbumId
            var minAlbumId = await _context.album
                                            .Where(a => a.UserId == userId)
                                            .MinAsync(a => (int?)a.AlbumId); // 使用(int?)以處理查詢結果可能為空的情況

            // 然後選擇除了最小AlbumId之外的所有Album
            var albumsExceptMin = await _context.album
                                                 .Where(a => a.UserId == userId && a.AlbumId != minAlbumId && a.State == 2)
                                                 .ToListAsync();
            // AlbumId != 1  && UserId == 1的都顯示出來 ****(UserId要更換)***
            //var albums = await _context.Albums.Where(a => a.AlbumId != 1 && a.UserId == 1).ToListAsync();

            foreach (var album in albumsExceptMin)
            {
                var photos = await _context.photo.Where(p => p.AlbumId == album.AlbumId && p.PhotoDescription == null).ToListAsync();
                viewModelList.Add(new AlbumPhotosViewModel { Album = album, Photos = photos });
            }
            return View(viewModelList);
        }
        public async Task<IActionResult> Garbage()
        {
            var minAlbumId = await _context.album
                                     .Where(a => a.UserId == userId && a.State == 1)
                                     .MinAsync(a => (int?)a.AlbumId);

            var albums = await _context.album
                                       .Where(a => a.UserId == userId && a.State == 1 && (!minAlbumId.HasValue || a.AlbumId != minAlbumId.Value))
                                       .ToListAsync();

            var targetAlbum = await _context.album
                                            .Where(a => a.UserId == userId)
                                            .OrderBy(a => a.AlbumId)
                                            .FirstOrDefaultAsync();

            if (targetAlbum == null)
            {
                // 如果找不到符合條件的Album，可以根據需求處理，例如返回錯誤訊息或創建新的Album等
                return NotFound(); // 這裡僅作為示例，實際上您可能需要根據業務邏輯進行調整
            }
            var photos = await _context.photo
                              .Where(p => p.UserId == userId && p.PhotoDescription == "1")
                              .ToListAsync();

            var viewModel = new GarbageViewModel
            {
                Photos = photos,
                Albums = albums // ??整?albums列表，如果你只需要?前?中的相簿，可以?建相??性
            };
            return View(viewModel);
        }
        //上傳相片方法
        [HttpPost]
        public async Task<IActionResult> UploadImages(List<IFormFile> imageFiles)
        {

            //條件 IF imageFile不是空的 && 大小比0大
            if (imageFiles != null && imageFiles.Any(f => f.Length > 0))
            {
                foreach (var imageFile in imageFiles)
                {
                    //設定一個變數，裝路徑 wwwroot/img/User01/photo ****(UserId要更換)***
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/img/user{userId}/photo");

                    //判斷是否存在，如果不存在就生成對應路徑
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    var originalFileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageFile.FileName); // 不包含副檔名的原始檔案名稱
                    var extension = Path.GetExtension(imageFile.FileName); // 檔案的副檔名
                    var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss"); // 时间戳记
                    var newFileName = $"{originalFileNameWithoutExtension}_{timeStamp}{extension}"; // 新檔案名稱包含时间戳记

                    //变量 WebPath 组合路径 /img/User01/photo/+ 变量 newFileName ****(UserId要更換)***
                    var webPath = $"/img/user{userId}/photo/{newFileName}";
                    var absolutePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/img/user{userId}/photo", newFileName);

                    //复制一份到指定的路径(根据 变量 absolutePath)
                    using (var stream = new FileStream(absolutePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    //新增 PhotoTitle（带时间戳记）、PhotoPath、UploadDate、UserId=1的数据到数据库  ****(UserId要更換)***
                    var photo = new photo
                    {
                        //没有副檔名的檔案名稱加时间戳记
                        PhotoTitle = $"{originalFileNameWithoutExtension}_{timeStamp}",
                        //图片路径
                        PhotoPath = webPath,
                        //当前日期 格式 yyyy-MM-dd
                        UploadDate = DateOnly.FromDateTime(DateTime.Now),
                        //需要再获取用户ID  ****(UserId要更换)***
                        UserId = userId, // 请根据实际情况替换UserId
                    };
                    _context.photo.Add(photo);

                    //保存
                    await _context.SaveChangesAsync();
                }
                //存一段文字讓前端引用
                TempData["Message"] = "上傳成功";

                //重新回到Privacy頁面
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
            var photo = await _context.photo.FindAsync(model.PhotoId);
            if (photo == null)
            {
                return NotFound(); // 404 Not Found
            }

            photo.PhotoDescription = model.NewDescription;
            await _context.SaveChangesAsync();

            return Ok(); // 200 OK
        }
        [HttpPost]
        public IActionResult UpdateALLPhotoDescription([FromBody] UpdateALLPhotoDescriptionRequest request)
        {
            var photos = _context.photo.Where(p => request.PhotoIds.Contains(p.PhotoId)).ToList();

            if (photos.Count != request.PhotoIds.Count)
            {
                return NotFound("One or more photos not found.");
            }

            foreach (var photo in photos)
            {
                photo.PhotoDescription = request.NewDescription; // 确保这里使用请求中的新描述
            }

            _context.SaveChanges();

            return Ok();
        }
        //相簿圖片刪除方法
        [HttpPost]
        public async Task<IActionResult> AlbumPhotoToGarbage(int photoId, DateOnly ReplaceTime)
        {
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
            if (!string.IsNullOrEmpty(folderName))
            {
                // 检查UserId=1的相册中是否已经存在同名相册（不区分大小写）
                var albumNameExists = await _context.album
                                                    .AnyAsync(a => a.UserId == userId &&
                                                                   a.AlbumName.ToLower() == folderName.ToLower());
                if (albumNameExists)
                {
                    TempData["Message"] = "相簿名稱已存在";
                    return RedirectToAction("Album");
                }

                var path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/img/user{userId}/album", folderName);

                // 檢查資料夾是否已存在并创建
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var album = new album
                {
                    UserId = userId,
                    AlbumName = folderName,
                    CreateTime = DateOnly.FromDateTime(DateTime.Now),
                    State = 2 //不在垃圾桶
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
            if (imageFiles != null && imageFiles.Any(f => f.Length > 0))
            {
                foreach (var imageFile in imageFiles)
                {
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/img/user{userId}/album/{albumName}");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    var originalFileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageFile.FileName); // 不包含副檔名的原始檔案名稱
                    var extension = Path.GetExtension(imageFile.FileName); // 檔案的副檔名
                    var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss"); // 时间戳记
                    var newFileName = $"{originalFileNameWithoutExtension}_{timeStamp}{extension}"; // 新檔案名稱包含时间戳记

                    //变量 webPath 和 absolutePath 使用新的文件名 newFileName
                    var webPath = $"/img/user{userId}/album/{albumName}/{newFileName}";
                    var absolutePath = Path.Combine(_hostingEnvironment.WebRootPath, $"img/user{userId}/album/{albumName}", newFileName);

                    //复制一份到指定的路径(根据 变量 absolutePath)
                    using (var stream = new FileStream(absolutePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    //新增 PhotoTitle（原始文件名不包含扩展名，加时间戳记）、PhotoPath、UploadDate、UserId和AlbumId的数据到数据库
                    var photo = new photo
                    {
                        //文件名不带扩展名，加时间戳记
                        PhotoTitle = $"{originalFileNameWithoutExtension}_{timeStamp}",
                        PhotoPath = webPath,
                        UploadDate = DateOnly.FromDateTime(DateTime.Now),
                        UserId = userId, // 注意这里UserId应该根据实际登录用户动态获取
                        AlbumId = albumId, // 假设 albumId 已经是一个有效的值
                    };
                    _context.photo.Add(photo);

                    //保存
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
            var minAlbumId = await _context.album
                                    .Where(a => a.UserId == userId && a.State == 1)
                                    .MinAsync(a => (int?)a.AlbumId);

            if (!minAlbumId.HasValue)
            {
                // 如果没有找到符合条件的相册，直接返回
                return NotFound("No albums found for the specified conditions.");
            }

            // 先删除minAlbumId相册中的所有图片
            var photosInMinAlbumToDelete = await _context.photo
                                                        .Where(p => p.UserId == userId && p.PhotoDescription == "1")
                                                        .ToListAsync();
            _context.photo.RemoveRange(photosInMinAlbumToDelete);

            // 找出所有状态为1且不是最小AlbumId的相册，并删除这些相册及其照片
            var albumsToDelete = await _context.album
                                                .Where(a => a.UserId == userId && a.State == 1 && a.AlbumId != minAlbumId.Value)
                                                .Include(a => a.photo)
                                                .ToListAsync();

            if (albumsToDelete.Count == 0 && photosInMinAlbumToDelete.Count == 0)
            {
                // 如果没有找到符合删除条件的相册或图片，直接返回
                return NotFound("No albums or photos to delete based on the specified conditions.");
            }

            // 删除这些相册及其照片
            foreach (var album in albumsToDelete)
            {
                _context.photo.RemoveRange(album.photo); // 删除相册中的照片
                _context.album.Remove(album); // 删除相册
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Garbage"); // 假设Garbage是你希望重定向到的地方
        }

        //Garbage刪除所有Album
        [HttpPost]
        public async Task<IActionResult> DeleteAllAlbum()
        {
            var minAlbumId = await _context.album
                                    .Where(a => a.UserId == userId && a.State == 1)
                                    .MinAsync(a => (int?)a.AlbumId);

            if (!minAlbumId.HasValue)
            {
                // 如果?有找到符合?件的相?，?返回
                return NotFound("No albums found for the specified conditions.");
            }

            // 找出所有不需要保留的相?
            var albumsToDelete = await _context.album
                                                .Where(a => a.UserId == userId && a.State == 1 && a.AlbumId != minAlbumId.Value)
                                                .Include(a => a.photo)
                                                .ToListAsync();

            if (albumsToDelete.Count == 0)
            {
                // 如果?有找到符合?除?件的相?，?返回
                return NotFound("No albums to delete except the one with the minimum AlbumId.");
            }
            // ?除?些相?及其照片
            foreach (var album in albumsToDelete)
            {
                _context.photo.RemoveRange(album.photo); // ?除相?中的照片
                _context.album.Remove(album); // ?除相?
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Garbage");
        }
        //Garbage刪除所有Photo
        [HttpPost]
        public async Task<IActionResult> DeleteAllPhoto()
        {
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
            var photosToDelete = await _context.photo
                                               .Where(p => p.PhotoId == photoId)
                                               .ToListAsync();

            if (!photosToDelete.Any())
            {
                return NotFound("No photos found with the specified AlbumId and UserId.");
            }
            _context.photo.RemoveRange(photosToDelete);
            await _context.SaveChangesAsync();

            return Ok("成功"); // 返回成功??
        }
        //刪除相簿到垃圾桶
        [HttpPost]
        public async Task<IActionResult> DeleteAlbum(int albumId)
        {

            var album = await _context.album
                       .FirstOrDefaultAsync(a => a.AlbumId == albumId);

            if (album == null)
            {
                return NotFound();
            }

            // 設置 State 欄位為 1 來表示該相簿現在是非活動狀態
            album.State = 1;

            // 標記實體為已修改，讓 EF Core 知道需要更新這個實體
            _context.Entry(album).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Photo));
        }
        //修改相簿名稱
        [HttpPost]
        public async Task<IActionResult> EditAlbumName(int albumId, string newAlbumName)
        {
            // 查找当前相册
            var album = await _context.album.FirstOrDefaultAsync(a => a.AlbumId == albumId);
            if (album == null)
            {
                return Json(new { success = false, message = "找不到指定的相簿。" });
            }

            // 检查UserId=1的相册中是否有其他相册使用了新的名字（不区分大小写）
            var albumNameExists = await _context.album
                                                .AnyAsync(a => a.UserId == userId &&
                                                               a.AlbumId != albumId &&
                                                               a.AlbumName.ToLower() == newAlbumName.ToLower());
            if (albumNameExists)
            {
                return Json(new { success = false, message = "相簿名稱已存在。" });
            }

            // 更新相册名
            album.AlbumName = newAlbumName;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
        //Garbage還原相片到Photo
        [HttpPost]
        public async Task<IActionResult> ReturnAlbumId(int photoId, DateOnly ReplaceTime)
        {

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
                photo.PhotoDescription = null; // 更新AlbumId
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Garbage"); // 返回成功??
        }
        [HttpPost]
        public async Task<IActionResult> RestoreAlbum(int albumId)
        {
            var album = await _context.album.FindAsync(albumId);
            if (album != null)
            {
                album.State = 2; // ???更新?2
                await _context.SaveChangesAsync();
                return RedirectToAction("Garbage"); // 重定向到适?的?面
            }
            return NotFound(); // 如果找不到相?，返回NotFound?果
        }
        [HttpPost]
        public async Task<IActionResult> UpdateNullAlbumPhotosToMinAlbumId()
        {


            // 查找所有UserId=1且AlbumId为NULL的图片
            var photosToUpdate = await _context.photo
                                               .Where(p => p.UserId == userId && p.AlbumId == null)
                                               .ToListAsync();

            if (!photosToUpdate.Any())
            {
                // 如果没有找到符合条件的图片
                return NotFound("No photos found with NULL AlbumId for the specified UserId.");
            }

            // 更新这些图片的AlbumId为minAlbumId
            photosToUpdate.ForEach(p => p.PhotoDescription = "1");

            await _context.SaveChangesAsync();

            return RedirectToAction("Photo"); // 请将YourRedirectAction替换为实际的重定向目标
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAlb(int albumId)
        {
            // 在这里编写逻辑以根据提供的albumId查找相册
            var album = await _context.album.Include(a => a.photo).FirstOrDefaultAsync(a => a.AlbumId == albumId);
            if (album != null)
            {
                var OtherPhoto = album.photo.Where(p => p.PhotoDescription == "1" && p.UserId == userId).ToList();
                foreach (var photo in OtherPhoto)
                {
                    photo.AlbumId = null;
                    await _context.SaveChangesAsync();
                }
                
                // 先删除相册中的所有图片
                foreach (var photo in album.photo.ToList())
                {
                    _context.photo.Remove(photo);
                }

                // 删除相册
                _context.album.Remove(album);
                await _context.SaveChangesAsync();
                // 可以添加一个消息来通知用户删除操作已成功
                TempData["Message"] = "相簿及其圖片刪除成功";
            }
            else
            {
                // 如果找不到相册，可以添加一个错误消息
                TempData["ErrorMessage"] = "找不到相簿，刪除失敗";
            }

            // 重定向到相应的视图或动作方法
            return RedirectToAction("Garbage"); // 根据实际情况调整重定向目标
        }

    }
}
