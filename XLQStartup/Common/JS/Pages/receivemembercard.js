//var args = getArgs();
function getMemberCard() {
    wx.ready(function () {
        wx.hideAllNonBaseMenuItem();
    });
    var apiUrl = "/API/Member/GetMemberCard"

    easyweb.server.load(apiUrl, { "openId": openId },
        function (result) {
            //领取会员卡成功
            alert("领取会员卡成功");
            window.location = "/Pages/Member/center.html?openId=" + openId;
        });
}