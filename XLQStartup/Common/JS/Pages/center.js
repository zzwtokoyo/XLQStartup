var args = getArgs();
var openId = args.openId;

$(function () {

    wx.ready(function () {
        wx.hideAllNonBaseMenuItem();
    });

    var apiUrl = "/API/Member/GetMember?openId=" + openId;
    easyweb.server.load(apiUrl, null,
        function (result) {
            member = result;

            if (member == null || (member.RegTime == null || member.RegTime == "" || member.RegTime == "0001-01-01 00:00:00")) {
                window.location = "/Pages/Member/register.html?openId=" + openId;
            }
            else if (member.CardNo == null || member.CardNo == "") {
                //领取卡券页面
                window.location = "/Pages/Member/receivemembercard.html?openId=" + openId;
            }
            else
            {
                $("#wholePage").show();
            }
        });
});
function turnTopage(type) {
    var goUrl = "/Pages/Member/memberinfo.html";
    switch (type) {
        case "member":
            goUrl = "/Pages/Member/memberinfo.html";
            break;
        case "activity":
            goUrl = "/Pages/Member/myactivity.html";
            break;
        case "favorite":
            goUrl = "/Pages/Member/myfavorite.html";
            break;
    }

    window.location = goUrl + "?openId=" + openId;
}