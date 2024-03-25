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

        //正規化字串做辨別符合目前縣市的景點
        var reg = /[0-9]+/g;
        cityString = $(this).text().trim().replace(reg, "").trim();

        var number = $(this).text().match(/\d+/)[0]; // 傳數量
        var pageSize = PageCut(number); // 0 1 2 3..頁

        defaultPageAjax(cityString);
        toggleActiveClass($(this));
    });


    $(document).on('click', '.pageIndex', function () {
        var currentPage = $(this).text();
        console.log(cityString);
        console.log(currentPage);
        //updateAjaxData(cityString, currentPage);
    });

    $(document).on('click', '#favor', function () {
        var card = this.closest('.card');
        if (card) {
            // 從父節點中找到 class 為 "card-title" 的元素
            var cardTitleElement = card.querySelector('.card-title');
            if (cardTitleElement) {
                // 獲取該元素的文本內容
                var cardTitle = cardTitleElement.textContent;
                // 在這裡你可以對 cardTitle 進行任何你需要的操作
                console.log('Card Title:', cardTitle);
            } else {
                console.error('Card title element not found');
            }
        } else {
            console.error('Card parent element not found');
        }
    })


    //------------------------------以下是副方法---------------------------

    //顯示預設前20筆結果
    function defaultPageAjax(cityString) {
        $.ajax({
            url: "/AiRecommend/DefaultPage",
            method: "GET",
            data: { city: cityString }
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

                                    <div class="col">
                                        <label for="phone">電話:</label>
                                        <input type="tel" id="phone" name="phone" value="${elem.phone}" readonly>
                                    </div>

                                    <div class="col">
                                        <label for="_Address">地址:</label>
                                        <input type="text" id="_Address" name="_Address" value="${elem._Address}" readonly>
                                    </div>

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



    //更新顯示結果以卡片呈現
    //bug : 遇到圖片為null log會回404但不影響功能
    function updateAjaxData(cityString, currentPage) {
        $.ajax({
            url: "/AiRecommend/Itinerary",
            method: "GET",
            data: { city: cityString, currentPage: currentPage }
        }).done(function (data) {
            $("#card-container").empty();
            $.each(data, function (idx, elem) {
                $('#card-container').append(`
                            <div class="card" style="width: 18rem;">
                                    <img src="${elem.pictureUrl1}" class="card-img-top" alt="">
                                    <div class="card-body">
                                        <h5 class="card-title">${elem.scenicSpotName}</h5>
                                    </div>
                                    <ul class="list-group list-group-flush">
                                        <li class="list-group-item">景點/店家電話 : ${elem.phone}</li>
                                        <li class="list-group-item">地址: ${elem._Address}</li>
                                        <li class="list-group-item">營業時間: ${elem.openTime}</li>
                                    </ul>
                                    <div class="card-body">
                                        <a>旅遊資訊 : ${elem.travelInfo}</a>
                                    </div>
                                            <button class="btn btn-primary" id="favor" type="submit">
                                            添加到行程
                                        </button>
                                </div>
                    `);
            })
        });
    }

    //反白按鈕
    function toggleActiveClass(element) {
        if (!element.hasClass("active")) {
            element.addClass("active");
        }
    }

    //推測目前會出現的頁數 ex:總筆數71筆會分3頁一頁20個
    function PageCut(number) {
        var pageSize;
        $.ajax({
            url: "/AiRecommend/Cut",
            method: "POST",
            async: false,
            data: { number: number }
        }).done(function (data) {
            console.log(data);
            //$("#pagination-container ul.pagination").empty();
            for (var i = 1; i <= data; i++) {
                $("#pagination").append(`<li class="page-item"><a class="page-link" href="#">${i}</a></li>`);
                console.log(i);
            }
            pageSize = data;
        });
        return pageSize;
    }

})

