
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
    console.log(ret);
})