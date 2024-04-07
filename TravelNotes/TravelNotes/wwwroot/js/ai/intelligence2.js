$(document).ready(function () {
    var currentPage = 1;
    var totalPages = 1;

    // Function to load the first page when clicking on locationBtn
    $("#locationBtn").click(function () {
        loadPage(1);
    });

    // Function to load data for a given page number
    function loadPage(pageNumber) {
        var citiesValue = $("#cities").val();

        $.ajax({
            url: "/AiRecommend/PagesNumber",
            method: "GET",
            data: { citiesValue: citiesValue },
            success: function (response) {
                totalPages = Math.ceil(response / 20);
                console.log("Total pages:", totalPages);

                updatePagination(pageNumber);
                loadDataForPage(pageNumber);
            }
        });
    }

    // Function to update pagination
    function updatePagination(pageNumber) {
        $("#pagination").empty();

        console.log(pageNumber)

        var startPage = Math.max(1, pageNumber - 2);
        var endPage = Math.min(totalPages, startPage + 4);

        // console.log(startPage)
        // console.log(endPage)


        if (startPage > 1) {
            $("#pagination").append(`
                    <li class="page-item">
                        <a class="page-link prev" aria-label="Previous">
                            <span aria-hidden="true">&laquo;</span>
                        </a>
                    </li>
                `);
        }

        for (var i = startPage; i <= endPage; i++) {
            $("#pagination").append(`
                    <li class="page-item ${i === pageNumber ? 'active' : ''}">
                        <a class="page-link pageIndex">${i}</a>
                    </li>
                `);
        }

        if (endPage < totalPages) {
            $("#pagination").append(`
                    <li class="page-item">
                        <a class="page-link next" aria-label="Next">
                            <span aria-hidden="true">&raquo;</span>
                        </a>
                    </li>
                `);
        }
    }

    // Function to load data for a specific page
    function loadDataForPage(pageNumber) {
        var citiesValue = $("#cities").val();

        $.ajax({
            url: "/AiRecommend/Itinerary",
            method: "GET",
            data: { city: citiesValue, currentPage: pageNumber },
            success: function (response) {
                $("#additionalCards").empty();
                $.each(response, function (idx, elem) {
                    if (idx % 3 == 0) {
                        $("#additionalCards").append(`<div class="w-100"></div>`);
                    }
                    $("#additionalCards").append(`
                            <div class="col-md">
                                <div class="card-body">
                                    <h5 class="card-title">${this.scenicSpotName ? this.scenicSpotName : ''}</h5>
                                    <p class="card-text">${this.phone ? this.phone : ''}</p>
                                    <p class="card-text">${this._Address ? this._Address : ''}</p>
                                    <a id="add" class="btn btn-primary">添加到行程</a>
                                    <a id="detail" class="btn btn-primary">看詳細</a>
                                </div>
                            </div>
                        `);
                });
            }
        });
    }

    // Event handler for clicking on pagination links
    $(document).on('click', '.pageIndex', function () {
        var pageNumber = parseInt($(this).text());
        loadPage(pageNumber);
    });

    // Event handler for clicking on previous page button
    $(document).on('click', '.prev', function () {
        if (currentPage > 1) {
            currentPage -= 1; // 減少 currentPage 值
            loadPage(currentPage);
        }
    });

    // Event handler for clicking on next page button
    $(document).on('click', '.next', function () {
        var nextPage = currentPage + 1;
        if (nextPage <= totalPages) {
            currentPage += 1; // 增加 currentPage 值
            loadPage(nextPage);
        }
    });

    // Event handler for clicking on "add" button
    $(document).on('click', '#add', function () {
        var cardTitle = $(this).closest('.card-body').find('.card-title').text();
        $.ajax({
            url: '/PersonalPage/Schedule',
            type: 'POST',
            data: {
                scenicSpotName: cardTitle
            },
            success: function (response) {
                if (response === "ok") {
                    alert("添加成功");
                } else if (response === "找不到景點，請聯絡管理員") {
                    alert("找不到景點，請聯絡管理員");
                } else if (response === "後端添加失敗") {
                    alert("重複添加景點，新增失敗");
                } else {
                    alert("添加失敗，請先登入");
                }
                console.log(response);
            },
            error: function (xhr, status, error) {
                console.error(error);
            }
        });
    });

    // Event handler for clicking on "detail" button
    $(document).on('click', '#detail', function () {
        var cardTitle = $(this).closest('.card-body').find('.card-title').text();
        $.ajax({
            url: '/PersonalPage/Detail',
            type: 'POST',
            data: {
                scenicSpotName: cardTitle
            },
            success: function (response) {
                if (response === "請先登入") {
                    alert("請先登入");
                } else {
                    alert(response);
                }
            },
            error: function (xhr, status, error) {
                console.error(error);
            }
        });
    });
});