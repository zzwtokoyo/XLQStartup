var activitylist = $("#activitylist");
//var args = getArgs();
$(function () {
    searchActivity();

    share(document.title, "Adobe市场活动往期回顾", "http://wechat.adobe-event.cn/pages/Activity/ongoing.html?1=1");
});
function searchActivity() {
    var apiUrl = "/API/Activity/GetShowActivity"

    easyweb.server.load(apiUrl,
        {
            "time": $("#time").val(),
            "creativeField": $("#creativeField").val(),
            "creativeTool": $("#creativeTool").val(),
            "status": $("#status").val(),
            "openId": args.openId
        },
        function (result) {

            var templateHtml = $("#template").html();
            activitylist.empty();
            for (var i = 0; i < result.length; i++) {
                var html = easyweb.getHtmlByTemplate(templateHtml, result[i], true);
                activitylist.append(html);
            }
            if (result.length < 1) {
                activitylist.append("<div class='emptylist'>暂无历史活动</div>");
            }
            //easyweb.binder.bindJsonToControl(container, result);
        });
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

function turnToPage(id) {
    window.location = "/Pages/Activity/detail.html?id=" + id + "&openId=" + openId;
}