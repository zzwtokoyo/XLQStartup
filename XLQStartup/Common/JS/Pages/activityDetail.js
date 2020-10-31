//var args = getArgs();
var activity;
var mediaUrl = "";
$(function () {
    InitActivity();
    share(document.title, "我发现了一个精彩的活动，快来一起参与吧！", "http://wechat.adobe-event.cn/pages/Activity/detail.html?id=" + args.id);
});

function InitActivity() {
    var apiUrl = "/API/Activity/GetActivity"
    var container = $("#");
    easyweb.server.load(apiUrl, { "id": args.id, "openId": openId },
        function (result) {
            activity = result;
            $("#detailPhoto").attr("src", result.DetailPhoto)
            $("#title").html(result.Title);
            if (result.AttendNum != null) {
                $("#attendNumber").html(result.AttendNum);
            }
            else {
                $("#attendNumber").html(0);
            }

            $("#type").html(result.Name);
            $("#summary").html(result.Summary);
            $("#content").html(result.Content);
            $("#time").html(result.ShowTime);
            $("#DownloadInfo").html(result.DownloadInfo);
            $("#mediaUrl").html(result.PlayInfo);
            mediaUrl = result.MediaURL;
            //$("#mediaUrl").attr("href", result.MediaURL);
            $("#star").addClass(result.Favorite);
            //easyweb.binder.bindJsonToControl(container, result);
        });
}

function favorite(obj) {
    var id = activity.Id;
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

function turnToMedia() {
    if (mediaUrl != undefined && mediaUrl != null && mediaUrl != "") {
        var apiUrl = "/API/Member/GetMember?openId=" + openId;
        easyweb.server.load(apiUrl, null,
            function (result) {
                member = result;
                if (member == null || (member.RegTime == null || member.RegTime == "" || member.RegTime == "0001-01-01 00:00:00")) {
                    window.location = "/Pages/Member/register.html?openId=" + openId + "&actId=" + args.id;;
                }
                else {
                    window.location = mediaUrl;
                }
            });
    }
    else {
        alert("暂无相关内容，敬请期待");
    }
}

function sendMedia() {
    if (activity.DownloadInfo == "暂无资料下载") {
        return false;
    }
    else {
        var apiUrl = "/API/Member/GetMember?openId=" + openId;
        easyweb.server.load(apiUrl, null,
            function (result) {
                member = result;
                if (member == null || (member.RegTime == null || member.RegTime == "" || member.RegTime == "0001-01-01 00:00:00")) {
                    window.location = "/Pages/Member/register.html?openId=" + openId + "&actId=" + args.id;;
                }
                else {
                    var sendUrl = "/API/Activity/SendActivityEmail?openId=" + openId + "&activityId=" + args.id;
                    easyweb.server.load(sendUrl, null,
                        function (result) {

                        });
                    alert("资料已邮件发送至注册时填写的邮箱地址，请注意查收！若邮箱地址填写有误，请点击“会员服务”-“会员中心”更改提交。");
                }
            });

    }
}