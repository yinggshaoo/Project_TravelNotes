// 勿動


//if (document.getElementById("friendList").children.length == 1) {
//    document.querySelector(".friendListUnit").style = "border-radius: 50px;";
//}
//// 批量對邀請列表的所有接受邀請按鈕添加事件監聽
//document.getElementById("friendList").addEventListener("click", (e) => {
//    if (e.target.classList.contains("acceptFriendRequest")) {
//        fetch("/api/Friend", {
//            method: "POST",
//            headers: {
//                "Accept": "application/json",
//                "Content-Type": "application/json"
//            },
//            body: JSON.stringify({
//    @{
//                    string userid;
//                    ViewContext.HttpContext.Request.Cookies.TryGetValue("UsernameCookie", out userid);
//                }
//                    UserId: parseInt(@userid),
//                FriendId: parseInt(e.target.previousElementSibling.previousElementSibling.innerText.split(":")[1]),
//                Status: true
//                })
//    }).then(res => {
//        return res.json();
//    }).then(ret => {
//        e.target.innerText = ret.message;
//    });
//        }
//    });