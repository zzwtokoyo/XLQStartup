//var args = getArgs();
var activitylist = $("#myActivity");
$(function () {
    wx.ready(function () {
        wx.hideAllNonBaseMenuItem();
    });
    var apiUrl = "/API/Activity/GetMyFavoriteActiviy?openId=" + openId;
    easyweb.server.load(apiUrl, null,
        function (result) {
            var templateHtml = activitylist.html();
            activitylist.empty();
            for (var i = 0; i < result.length; i++) {
                var html = easyweb.getHtmlByTemplate(templateHtml, result[i], true);
                activitylist.append(html);
            }
            if (result.length < 1) {
                activitylist.append("<div class='emptylist'>暂无收藏的活动</div>");
            }
        });
});


function turnToPage(id) {
    window.location = "/Pages/Activity/detail.html?id=" + id + "&openId=" + openId;
}