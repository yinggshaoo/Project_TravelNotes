function getUserNameCookie() {
    return document.cookie.split("; ").find(ele => new RegExp('UsernameCookie*').test(ele)).split("=")[1];
}
document.getElementById("friendList").addEventListener("click", handleDeleteFriend);
function handleDeleteFriend(e) {
    if (!e.target.classList.contains("endFriendshipBtn")) {
        return;
    }
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