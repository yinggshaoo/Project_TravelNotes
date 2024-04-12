function getUserNameCookie() {
    return document.cookie.split("; ").find(ele => new RegExp('UsernameCookie*').test(ele)).split("=")[1];
}
if (document.getElementById("friendList").children.length == 1) {
    document.querySelector(".friendListUnit").style = "border-radius: 50px;";
}
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
            e.target.innerText = ret.message;
        });
    }
    if (e.target.classList.contains("rejectFriendRequest")) {
        console.log("friend request rejected");
    }
});