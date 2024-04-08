$(document).ready(function () {
    $('#submitButton').click(function (e) {
        e.preventDefault(); // 防止表單提交
        $('#top').remove();
        // 取出每個選項的值
        var weatherString = $('#weatherSelect').val();
        var interestOneValue = $('#interestOneSelect').val();
        var interestTwoValue = $('#interestTwoSelect').val();
        var interestThreeValue = $('#interestThreeSelect').val();
        var countryString = $('#countrySelect').val();
        if (weatherString === '請選一項天氣' || interestOneValue === '請選興趣之一' || interestTwoValue === '請選興趣之二' || interestThreeValue === '請選興趣之三' || countryString === '請選一個國家') {
            // 如果有任何一個選項沒有被選擇，顯示警告訊息
            alert('請確保每個選項都已選擇！');
        } else {

            $.ajax({
                url: '/AiRecommend/MlHandel',
                type: 'POST',
                data: {
                    Interests1: interestOneValue,
                    Interests2: interestTwoValue,
                    Interests3: interestThreeValue,
                    weather: weatherString,
                    country: countryString
                },
                success: function (response) {
                    var prediction = response[0];
                    var answer = response[1];
                    var additional = response[2];
                    console.log(prediction)
                    console.log(answer)
                    console.log(additional)

                    $("h4").append(`<div id="top">
                            <h4>猜你喜歡的主題 - ${prediction}</h4> </br>
                            <h4>以下是我為您推薦的景點</h4>
                            <div class="card" style="width: 18rem;">
                                <img src="${answer ? answer.pictureUrl1 : ''}" class="card-img-top" alt="..." id="card-img-top">
                                <div class="card-body">
                                    <h5 class="card-title">${answer ? answer.scenicSpotName : ''}</h5>
                                    <p class="card-text">${answer ? answer.phone : ''}</p>
                                    <p class="card-text">${answer ? answer._Address : ''}</p>
                                    <a id="add" class="btn btn-primary">添加到行程</a>
                                    <a id="detail" class="btn btn-primary">看詳細</a>
                                </div>
                            </div>
                            </br>
                            <h4>你也可能會喜歡</h4>
                            <div class="row" id="additionalCards"></div>
                        `);

                    $.each(additional, function (idx, elem) {
                        if (idx % 3 == 0) {
                            $("#additionalCards").append(`<div class="w-100"></div>`);
                        }
                        console.log(this.spotId);
                        $("#additionalCards").append(`
                                <div class="card col-md">
                                <img src="${this.pictureUrl1}" class="card-img-top" alt="..." id="card-img-top">
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
                                    alert("找不到景點，請聯絡管理員")
                                } else if (response === "後端添加失敗") {
                                    alert("重複添加景點，新增失敗")
                                } else {
                                    alert("添加失敗，請先登入");
                                }
                                // $(this).remove();
                                console.log(response);
                            },
                            error: function (xhr, status, error) {
                                console.error(error);
                            }

                        })


                        console.log(cardTitle);

                    });
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
                        })
                    })
                },
                error: function (xhr, status, error) {
                    console.error(error);
                }
            })
        }
    })
})