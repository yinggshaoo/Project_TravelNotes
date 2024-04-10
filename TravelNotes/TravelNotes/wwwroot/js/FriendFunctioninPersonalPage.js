function getUserNameCookie() {
    return document.cookie.split("; ").find(ele => new RegExp('UsernameCookie*').test(ele)).split("=")[1];
}

//function handleFriendRequestList(e) {
//    var frList = document.createElement("div");

//    frList.setAttribute("style", `position: absolute; border: 1px solid black; z-index: 1; background-color: white; border-radius: 2rem;`);
//    frList.innerText = "friend1"
//    e.currentTarget.appendChild(frList);
//}

function NumberofFriendRequests() {
    return fetch("/api/NumberofFriendRequests").then(res => {
        return res.json();
    }).then(ret => {
        if (ret.success) {
            return ret.number;
        }
        return null;
    })
}

NumberofFriendRequests().then(ret => {
    // 如果不是我的Page 不要顯示交友邀請數量
    if (document.location.href.split("=").length == 1) {
        document.querySelector("#friendReqBtn>span").innerHTML = `${ret}`;
    }

})

//if its not your page
if (document.location.href.split("=").length > 1) {
    // 綁定送交友邀請事件
    document.querySelector("#sendFriendRequestButtonWrapper").addEventListener("click", (e) => {
        if (!e.currentTarget.firstChild.classList.contains("fa-user-plus")) {
            return;
        }
        fetch("/api/FriendRequest", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json"
            },
            body: JSON.stringify({
                SenderUserId: parseInt(getUserNameCookie()),
                ReceiverUserId: parseInt(document.location.href.split("=")[1]),
                Status: 0
            })
        }).then(res => {
            return res.json();
        }).then(ret => {
            if (ret.code == 200) {
                document.getElementById("sendFriendRequestButtonWrapper").innerHTML = `<i class="fa-solid fa-paper-plane"></i>&nbsp;<span>收回邀請</span>`;
                document.getElementById("sendFriendRequestButtonWrapper").addEventListener("click", deleteFriendRequest);
            }
            else {
                alert(ret.message);
            }
        });
    });

    // check any friend relation
    fetch(`/api/IsFriendRequestSent/${getUserNameCookie()}/${document.location.href.split("=")[1]}`).then(res => {
        return res.json();
    }).then(ret => {
        if (ret.success) {
            // 如果已經送出邀請顯示送出圖像
            document.getElementById("sendFriendRequestButtonWrapper").innerHTML = `<i class="fa-solid fa-paper-plane"></i>&nbsp;收回邀請`;


            if (document.getElementById("sendFriendRequestButtonWrapper").innerHTML == `<i class="fa-solid fa-paper-plane"></i>&nbsp;收回邀請`) {
                document.getElementById("sendFriendRequestButtonWrapper").addEventListener("click", deleteFriendRequest);
            }
        }
        else {
            // 還沒送出邀請
            document.getElementById("sendFriendRequestButtonWrapper").innerHTML = `<i class="fa-solid fa-user-plus"></i>&nbsp;加入好友`;
        }
    }).then(() => {
        // 如果已經是好友
        fetch(`/api/IsFriend/${getUserNameCookie()}/${document.location.href.split("=")[1]}`).then(res => {
            return res.json();
        }).then(ret => {
            console.log(ret)
            if (ret.success) {
                document.getElementById("sendFriendRequestButtonWrapper").innerHTML = `<i class="fa-solid fa-user-group"></i>朋友`;
            }
        }).then(() => {
            // TODO: 如果已經送出邀請 對這個(#sendFriendRequestButtonWrapper)元素加入 刪除邀請 事件監聽

        })
    })
}
function deleteFriendRequest(e) {
    fetch("/api/FriendRequest", {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
        },
        body: JSON.stringify({
            SenderUserId: parseInt(getUserNameCookie()),
            ReceiverUserId: parseInt(document.location.href.split("=")[1]),
        })
    }).then(res => {
        return res.json();
    }).then(ret => {
        if (ret.success) {
            // 刪除好友邀請成功 將icon回到加好友圖示
            document.getElementById("sendFriendRequestButtonWrapper").innerHTML = `<i class="fa-solid fa-user-plus"></i>&nbsp;加入好友`;
        }
    })
}


function friend() {
    if (document.location.href.split("=").length == 1) {
        document.location.href = `/Friend/${getUserNameCookie()}/`;
    }
    else {
        document.location.href = `/Friend/${document.location.href.split("=")[1]}/`;
    }
}


function friendRequest() {
    if (document.location.href.split("=").length == 1) {
        document.location.href = `/FriendRequests/${getUserNameCookie()}/`;
    }
    else {
        document.location.href = `/FriendRequests/${document.location.href.split("=")[1]}/`;
    }
}
function friendRequestSent() {
    if (document.location.href.split("=").length == 1) {
        document.location.href = `/FriendRequestsSent/${getUserNameCookie()}/`;
    }
    else {
        document.location.href = `/FriendRequestsSent/${document.location.href.split("=")[1]}/`;
    }
}
document.getElementById("friendReqBtn").addEventListener("mouseenter", (e) => {
    e.currentTarget.getElementsByTagName("span")[0].style.color = "white";
    e.stopPropagation();
}, true);
document.getElementById("friendReqBtn").addEventListener("mouseleave", (e) => {
    e.currentTarget.getElementsByTagName("span")[0].style.color = "black";
    e.stopPropagation();
});

