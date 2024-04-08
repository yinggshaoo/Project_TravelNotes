using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelNotes.Models;

namespace TravelNotes.Controllers
{
    public class FriendController(TravelContext ctx) : Controller
    {
        [HttpGet]
        [Route("api/IsFriend/{CurrentUserId}/{FriendUserId}")]
        public JsonResult IsFriend(string CurrentUserId, string FriendUserId)
        {
            if(ctx.Friend.Any(f => f.UserId == Convert.ToInt32(CurrentUserId) && f.FriendId == Convert.ToInt32(FriendUserId))) return Json(new
            {
                success = true,
            });
            return Json(new
            {
                success = false,
            });
        }
        [HttpGet]
        [Route("api/IsFriendRequestSent/{CurrentUserId}/{FriendUserId}")]
        public JsonResult IsFriendRequestSent(string CurrentUserId, string FriendUserId)
        {
            if (ctx.FriendRequest.Any(fr => fr.SenderUserId == Convert.ToInt32(CurrentUserId) && fr.ReceiverUserId == Convert.ToInt32(FriendUserId))) return Json(new
            {
                success = true
            });
            return Json(new
            {
                success = false,
            });
        }
        // 可運作
        [HttpPost]
        [Route("api/FriendRequest")]
        public JsonResult AddFriendRequest([FromBody] FriendRequest friendRequest)
        {
            // 檢查自己加自己
            if (friendRequest.SenderUserId == friendRequest.ReceiverUserId)
            {
                return Json(new
                {
                    code = 400,
                    message = "可憐~"
                });
            }
            // 檢查已經是朋友
            if (ctx.Friend.Any(fr => fr.UserId == friendRequest.SenderUserId && fr.FriendId == friendRequest.ReceiverUserId))
            {
                return Json(new
                {
                    code = 400,
                    message = "已經加過好友"
                });
            }
            // 檢查已經邀請過
            if (ctx.FriendRequest.Any(fr => fr.SenderUserId == friendRequest.SenderUserId && fr.ReceiverUserId == friendRequest.ReceiverUserId))
            {
                return Json(new
                {
                    code = 400,
                    message = "已經送出過好友邀請"
                });
            }
            // 檢查已經被邀請過
            if (ctx.FriendRequest.Any(fr => fr.SenderUserId == friendRequest.ReceiverUserId && fr.ReceiverUserId == friendRequest.SenderUserId))
            {
                return Json(new
                {
                    code = 400,
                    message = "已經被邀"
                });
            }
            // 檢查邀請者埃滴存在
            if (!ctx.users.Any(u => u.UserId == friendRequest.SenderUserId))
            {
                return Json(new
                {
                    code = 400,
                    message = "幽靈是你~"
                });
            }
            // 檢查被邀請者埃滴存在
            if (!ctx.users.Any(u => u.UserId == friendRequest.ReceiverUserId))
            {
                return Json(new
                {
                    code = 400,
                    message = "冥婚?"
                });
            }
            ctx.FriendRequest.Add(friendRequest);
            ctx.SaveChanges();
            return Json(new
            {
                code = 200,
                message = "增加好友邀請成功!"
            });
        }
        // 可運作
        [HttpPost]
        [Route("api/Friend")]
        public JsonResult AddFriend([FromBody] Friend friend)
        {
            // 檢查輸入值合法性
            if (friend.UserId == 0 || friend.FriendId == 0)
            {
                return Json(new
                {
                    code = 400,
                    message = "客戶端錯誤"
                });
            }
            // 檢查邀請是否存在
            if (!ctx.FriendRequest.Any(fr => fr.SenderUserId == friend.FriendId && fr.ReceiverUserId == friend.UserId))
            {
                return Json(new
                {
                    code = 400,
                    message = "無此邀請"
                });
            }
            // 同意者與邀請者的關係
            ctx.Friend.Add(friend);
            // 邀請者與同意者的關係
            ctx.Friend.Add(new Friend
            {
                UserId = friend.FriendId,
                FriendId = friend.UserId,
                Status = true,
            });
            // 將邀請狀態設置為結案
            var friendRequestToUpdate = ctx.FriendRequest.Where(fr => fr.SenderUserId == friend.FriendId && fr.ReceiverUserId == friend.UserId).Single();
            friendRequestToUpdate.Status = 1;

            ctx.SaveChanges();
            return Json(new
            {
                code = 200,
                message = "增加好友成功!"
            });
        }

        /// <summary>
        /// 好友列表
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Friend/{UserId}")]
        public ViewResult Friends(int UserId)
        {
            var ret = ctx.Friend.Where(f => f.UserId == UserId).Include(f => f.FriendUser).ToList();
            return View(ret);
        }





        /// <summary>
        /// 得到的交友邀請
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpGet]
        [Route("FriendRequests/{UserId}")]
        public ViewResult FriendRequests(int UserId)
        {
            var ret = ctx.FriendRequest.Where(fr => fr.ReceiverUserId == UserId).Include(fr => fr.SenderUser).ToList();
            return View(ret);
        }


        /// <summary>
        /// 送出的交友邀請
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpGet]
        [Route("FriendRequestsSent/{UserId}")]
        public ViewResult FriendRequestsSent(int UserId)
        {
            var ret = ctx.FriendRequest.Where(fr => fr.SenderUserId == UserId).Include(fr => fr.ReceiverUser).ToList();
            return View(ret);
        }
    }
}
