
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


// test getting UsernameCookie
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
console.log(getUserNameCookie());
