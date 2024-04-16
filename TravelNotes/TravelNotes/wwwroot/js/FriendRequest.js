function getUserNameCookie() {
    return document.cookie.split("; ").find(ele => new RegExp('UsernameCookie*').test(ele)).split("=")[1];
}
function showNotification(message) {
    $("#notification").text(message).css("display", "block")
    setTimeout(function () {
        $("#notification").css("display", "none");
    }, 3000)
}
//if (document.getElementById("friendList").children.length == 1) {
//    document.querySelector(".friendListUnit").style = "border-radius: 50px;";
//}
// 批量對邀請列表的所有接受邀請按鈕添加事件監聽
document.getElementById("friendList").addEventListener("click", (e) => {
    if (e.target.classList.contains("acceptFriendRequest")) {
        fetch("/api/Friend", {
            method: "POST",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                UserId: parseInt(getUserNameCookie()),
                FriendId: parseInt(e.target.parentElement.parentElement.children[0].innerText.split(":")[1]),
                Status: true
            })
        }).then(res => {
            return res.json();
        }).then(ret => {
            e.target.parentElement.parentElement.remove();
            showNotification("添加好友成功!");
        });
    }
    if (e.target.classList.contains("rejectFriendRequest")) {
        console.log("friend request rejected");
    }
});