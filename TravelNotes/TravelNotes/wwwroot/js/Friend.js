
document.getElementById("friendList").addEventListener("click", (e) => { if (e.target.classList.contains("endFriendshipBtn")) { handleDeleteFriend(e) } })
function handleDeleteFriend(e) {
    friendId = e.target.parentElement.firstElementChild.innerText.split(":")[1];
    userId = getUserNameCookie()
    return fetch("/api/Friend", {
        method: "DELETE",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
        },
        body: JSON.stringify({
            UserId: parseInt(userId),
            FriendId: parseInt(friendId),
        })
    }).then(res => {
        return res.json();
    }).then(ret => {
        if (ret.success) {
            location.reload();
        }
    })
}




function getUserNameCookie() {
    var cookieArray = document.cookie.split("; ");
    var userid
    cookieArray.forEach(ele => {
        if (new RegExp('UsernameCookie*').test(ele)) {
            userid = ele.split("=")[1];
        }
    })
    return userid;
}