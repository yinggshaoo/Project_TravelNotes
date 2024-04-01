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

        var number = $(this).text().match(/\d+/);
        if (number && number.length > 0) {
            number = number[0];
            var pageSize = PageCut(number, cityString);
        }

        defaultpageajax(cityString);
        toggleActiveClass($(this));
    });

    //------------------------------以下是副方法---------------------------
    function defaultpageajax(citystring) {
        $.ajax({
            url: "/AiRecommend/Itinerary",
            method: "get",
            data: { city: citystring, currentPage: "1" }
        }).done(function (data) {
            $("#card-form").empty(); // 清空表單容器
            $.each(data, function (idx, elem) {
                $("#card-form").append(`
                    <form action="/PersonalPage/Schedule" method="post">
                        <table class="table table-striped">
                            <tr>
                                <td>
                                    <input type="text" id="scenicSpotName" name="scenicSpotName" value="${elem.scenicSpotName}" readonly>
                                </td>
                                <td>
                                    <input type="tel" id="phone" name="phone" value="${elem.phone}" readonly>
                                </td>
                                <td>
                                    <input type="text" id="_Address" name="_Address" value="${elem._Address}" readonly>
                                </td>
                                <td>
                                    <button type="submit" class="btn btn-primary btn-sm">添加到行程</button>
                                </td>
                            </tr>
                        </table>
                    </form>
                `);
            });
        });
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
            data: { number: number }
        }).done(function (data) {
            $("#pagination").empty();
            for (var i = 1; i <= data; i++) {
                $("#pagination").append(`
                    <li class="page-item">
                        <a class="page-link pageIndex">${i}</a>
                    </li>
                `);
            }

            $(".pageIndex").click(function () {
                $("#card-form").empty(); // 清空表單容器
                var currentPageNumber = $(this).text(); // 將點擊的頁碼轉換為整數並存儲到全域變數中

                $.ajax({
                    url: "/AiRecommend/Itinerary",
                    method: "get",
                    data: { city: cityString, currentPage: currentPageNumber }
                }).done(function (data) {
                    $.each(data, function (idx, elem) {
                        $("#card-form").append(`
                            <form action="/PersonalPage/Schedule" method="post">
                                <table class="table table-striped">
                                    <tr>
                                        <td>
                                            <input type="text" class="input-group-text" id="scenicSpotName" name="scenicSpotName" value="${elem.scenicSpotName}" readonly>
                                        </td>
                                        <td>
                                            <input type="tel" class="input-group-text" id="phone" name="phone" value="${elem.phone}" readonly>
                                        </td>
                                        <td>
                                            <input type="text" class="input-group-text" id="_Address" name="_Address" value="${elem._Address}" readonly>
                                        </td>
                                        <td>
                                            <button type="submit" class="btn btn-primary btn-sm">添加到行程</button>
                                        </td>
                                    </tr>
                                </table>
                            </form>
                        `);
                    });
                });
            });
        });
    }
});
