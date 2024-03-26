$(document).ready(function () {
    $.ajax({
        url: "/AiRecommend/Subtotal",
        method: "GET",
    }).done(function (data) {
        $.each(data, function (idx, elem) {
            $("#" + idx + "-C").append(elem.count);
        })
    });

    var cityString = "";
    
    $(".city-btn").click(function () {
        $(".city-btn").removeClass("active");     
        var reg = /[0-9]+/g;
        cityString = $(this).text().trim().replace(reg, "").trim();

        var number = $(this).text().match(/\d+/)[0];
        var pageSize = PageCut(number, cityString);

        defaultpageajax(cityString);
        toggleActiveClass($(this));
    })


    


    //------------------------------以下是副方法---------------------------
    function defaultpageajax(citystring) {
        $.ajax({
            url: "/AiRecommend/Itinerary",
            method: "get",
            data: { city: citystring, currentPage: "1" }
        }).done(function (data) {
            $("#form-container").empty();
            $.each(data, function (idx, elem) {
                $('#form-container').append(
                    `
                        <form action="/PersonalPage/Schedule" method="post">
                            <div class="container">
                                <div class="row">
                                    <div class="col">
                                        <label for="scenicSpotName">景點名稱:</label>
                                       <input type="text" id="scenicSpotName" name="scenicSpotName" value="${elem.scenicSpotName}" readonly>
                                    </div>

                                    <div class="col"></div>

                                    <div class="col">
                                        <label for="phone">電話:</label>
                                        <input type="tel" id="phone" name="phone" value="${elem.phone}" readonly>
                                    </div>

                                    <div class="col"></div>

                                    <div class="col">
                                        <label for="_Address">地址:</label>
                                        <input type="text" id="_Address" name="_Address" value="${elem._Address}" readonly>
                                    </div>

                                    <div class="col"></div>

                                    <div class="col">
                                        <button type="submit" class="btn btn-primary btn-sm">添加到行程</button>
                                    </div>
                                </div>
                            </div>
                        </form>
                    `
                );
            })
        })
    }

    //反白按鈕
    function toggleActiveClass(element) {
        if (!element.hasClass("active")) {
            element.addClass("active");
        }
    }

    function PageCut(number, cityString) {
        $.ajax({
            url: "/AiRecommend/Cut",
            method: "POST",
            async: false,
            data: { number: number }
        }).done(function (data) {
            console.log(data)
            $("#pagination").empty();
            for (var i = 1; i <= data; i++) {
                $("#pagination").append(`
                    <li class= "page-item">
                        <a class="page-link pageIndex">${i}</a>
                    </li>`    
                );
            }

            $(".pageIndex").click(function () {
                var currentPageNumber = $(this).text(); // 將點擊的頁碼轉換為整數並存儲到全域變數中
                console.log("Current Page: " + currentPageNumber); // 印出當前頁碼

                $.ajax({
                    url: "/AiRecommend/Itinerary",
                    method: "get",
                    data: { city: cityString, currentPage: currentPageNumber }
                }).done(function (data) {
                    $("#form-container").empty();
                    $.each(data, function (idx, elem) {
                        $('#form-container').append(
                            `
                        <form action="/PersonalPage/Schedule" method="post">
                            <div class="container">
                                <div class="row">
                                    <div class="col">
                                        <label for="scenicSpotName">景點名稱:</label>
                                       <input type="text" id="scenicSpotName" name="scenicSpotName" value="${elem.scenicSpotName}" readonly>
                                    </div>

                                    <div class="col"></div>

                                    <div class="col">
                                        <label for="phone">電話:</label>
                                        <input type="tel" id="phone" name="phone" value="${elem.phone}" readonly>
                                    </div>

                                    <div class="col"></div>

                                    <div class="col">
                                        <label for="_Address">地址:</label>
                                        <input type="text" id="_Address" name="_Address" value="${elem._Address}" readonly>
                                    </div>

                                    <div class="col"></div>

                                    <div class="col">
                                        <button type="submit" class="btn btn-primary btn-sm">添加到行程</button>
                                    </div>
                                </div>
                            </div>
                        </form>
                    `
                        );
                    })
                })
            });
        })
    }
})