
//document.getElementById("friendReqBtn").onmouseenter = handleFriendRequestList;

function handleFriendRequestList(e) {
    var frList = document.createElement("div");

    frList.setAttribute("style", `position: absolute; border: 1px solid black; z-index: 1; background-color: white; border-radius: 2rem;`);
    frList.innerText = "friend1"
    e.currentTarget.appendChild(frList);
}