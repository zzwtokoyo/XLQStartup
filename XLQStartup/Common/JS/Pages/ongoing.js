var activitylist = $("#activitylist");
//var args = getArgs();
$(function () {
    searchActivity();

    share(document.title, "Adobe市场活动陆续开展中，快来报名参与体验吧！", "http://wechat.adobe-event.cn/pages/Activity/ongoing.html?1=1");
    //sample 调用的页面自行处理
    //wx.ready(function () {
    //    wx.hideAllNonBaseMenuItem();
    //});
});



function searchActivity() {
    var apiUrl = "/API/Activity/GetShowActivity"

    easyweb.server.load(apiUrl,
        {
            "time": $("#time").val(),
            "creativeField": $("#creativeField").val(),
            "creativeTool": $("#creativeTool").val(),
            "status": $("#status").val(),
            "openId": openId
        },
        function (result) {

            var templateHtml = $("#template").html();
            activitylist.empty();
            for (var i = 0; i < result.length; i++) {
                var html = easyweb.getHtmlByTemplate(templateHtml, result[i], true);
                activitylist.append(html);
            }

            if (result.length < 1) {
                activitylist.append("<div class='emptylist'>暂无活动,敬请关注</div>");
            }
        });
}

function attend(id) {
    var apiUrl = "/API/Activity/AttendActivity";
    easyweb.server.load(apiUrl, { "activityId": id, "openId": openId },
        function (result) {
            window.location = "/Pages/Activity/attend.html?id=" + id + "&openId=" + openId;
            //alert("参加成功");
        });
}

function attendOrDetail(id, text) {
    if (text == "我要参加") {
        turnToAttendPage(id);
    }
    else {
        turnToPage(id);
    }
}

function favorite(id, obj) {
    if ($(obj.parentNode).hasClass("highlight")) {
        cancelFavorite(id, obj);
    }
    else {
        var apiUrl = "/API/Activity/FavoriteActivity";
        easyweb.server.load(apiUrl, { "activityId": id, "openId": openId },
            function (result) {
                //收藏完成后，样式change
                $(obj.parentNode).addClass("highlight");
            });
    }
}

function cancelFavorite(id, obj) {
    var apiUrl = "/API/Activity/CancelFavorite";
    easyweb.server.load(apiUrl, { "activityId": id, "openId": openId },
        function (result) {
            //取消收藏完成后，样式change
            $(obj.parentNode).removeClass("highlight");
        });
}
function turnToAttendPage(id) {
    window.location = "/Pages/Activity/attend.html?id=" + id + "&openId=" + openId;
}

function turnToPage(id) {
    window.location = "/Pages/Activity/detail.html?id=" + id + "&openId=" + openId;
}