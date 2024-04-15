let myChart;
statistic();
function generateChart(articleData) {
    console.log(articleData);
    if (typeof myChart !== 'undefined' && myChart !== null) {
        // 如果存在，銷毀之
        myChart.destroy();
    }
    // 使用Chart.js創建分布圖
    const ctx = document.getElementById('myChart').getContext('2d');
    if (articleData == null || articleData == undefined || articleData.length == 0) {
        var data = {
            labels: [],
            datasets: [{
                data: []
            }]
        };
    }
    else {
        let myLabels = [];
        let myData = [];
        for (let i = 0; i < articleData.length; i++) {
            let maxLength = 15;
            let currentData = articleData[i];
            let labelName = `id:${currentData.userId} ${currentData.title}`;
            labelName = labelName.length > maxLength ? labelName.substring(0, maxLength) + "..." : labelName
            myLabels.push(labelName);
            myData.push(articleData[i].pageView);
        }
        var data = {
            labels: myLabels,
            datasets: [{
                label: '瀏覽人數',
                backgroundColor: 'rgb(255, 99, 132)',
                borderColor: 'rgb(255, 99, 132)',
                data: myData,
                barThickness: 80 // 固定寬度
            }]
        };
    }

    // 配置选项
    var options = {
        scales: {
            y: {
                beginAtZero: true
            }
        }
    };

    // 创建图表
    myChart = new Chart(ctx, {
        type: 'bar',
        data: data,
        options: options
    });
}
function statistic() {
    //console.log(Number($('#pageViewInput').val()));
    //console.log(Number($('#userIdInput').val()));
    $.ajax({
        type: "get",
        url: "/Backstage/GetArticles", // 控制器的 URL
        data: {
            pageView: Number($('#pageViewInput').val()),
            userId: Number($('#userIdInput').val())
        },
        success: function (articleData) {
            //console.log(articleData);
            generateChart(articleData);
        }
    });
    $('#pageViewInput').val("");
    $('#userIdInput').val("");
}