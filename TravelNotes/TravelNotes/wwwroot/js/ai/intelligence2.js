$(document).ready(function () {
    var maxSelections = 3;
    var selectedInterests = [];
    var weatherString = "";
    var countryString = "";

    $(".interest-btn").click(function () {
        var interestValue = $(this).text().trim();

        if ($(this).hasClass("active")) {

            selectedInterests = selectedInterests.filter(item => item !== interestValue);
        } else {
            if (selectedInterests.length >= maxSelections) {

                alert("最多選3個!")
                return;
            }

            selectedInterests.push(interestValue);
        }

        $(this).toggleClass("active");
        //updateAjaxData();
    });

    $(".weather-btn").click(function () {
        $(".weather-btn").removeClass("active");
        weatherString = $(this).text().trim();
        console.log(weatherString);
        toggleActiveClass($(this));
        //updateAjaxData();
    });

    $(".country-btn").click(function () {
        $(".country-btn").removeClass("active");
        countryString = $(this).text().trim();
        console.log(countryString);
        toggleActiveClass($(this));
        //updateAjaxData();
    });

    $("#submit").click(function () {
        updateAjaxData(selectedInterests, weatherString, countryString);
        disableAllButtons();
    })

    $("#reset").click(function () {
        location.reload(); // 重新加载页面
    });

    function toggleActiveClass(element) {
        if (!element.hasClass("active")) {
            element.addClass("active");
        }
    }

    function updateAjaxData(selectedInterests, weatherString, countryString) {

        $.ajax({
            url: "/AiRecommend/MlHandel",
            method: "POST",
            data: {
                Interests1: selectedInterests[0],
                Interests2: selectedInterests[1],
                Interests3: selectedInterests[2],
                weather: weatherString,
                country: countryString
            }
        }).done(function (data) {
            $("h4").append(`<h3>以下可能是您會喜歡的地點</h3>`)
            $.each(data, function (idx, elem) {
                if (idx <= 100) {
                    $("h4").after(`
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
                    console.log(idx + " " + elem.scenicSpotName);
                } else {
                    return false;
                }
            })
        })
    }

    function disableAllButtons() {
        $(".btn").prop("disabled", true); // 禁用所有按钮
        $("#reset").prop("disabled", false)
    }

});


