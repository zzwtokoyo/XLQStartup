(function () {
    var apiUrl = "/API/Face/getticket";
    easyweb.server.load(apiUrl, { token: "dfihg", url: location.href },
        function (result) {
            wx.config({
                debug: false,
                appId: result.AppId,
                timestamp: result.Timestamp,
                nonceStr: result.NonceStr,
                signature: result.Signature,
                jsApiList: ["hideAllNonBaseMenuItem", 'onMenuShareTimeline', 'onMenuShareAppMessage', 'onMenuShareQQ', 'onMenuShareWeibo', 'onMenuShareQZone']
            });
           

         

        });
})();


function share(title, desc, redirectUrl)
{
    var share = [];
    share.title = title;
    share.desc = desc;
    share.link = "http://wechat.adobe-event.cn/API/Face/buildBaseURL?direct=" + redirectUrl;
    share.imgUrl = "http://wechat.adobe-event.cn/common/adobeLogo.png";
    share.success = function () {
        //alert("分享成功！");
    }
    share.cancel = function () {
        //alert("取消分享");
    }
   
    wx.ready(function () {
        wx.onMenuShareTimeline(share);
        wx.onMenuShareAppMessage(share);
        wx.onMenuShareQQ(share);
        wx.onMenuShareWeibo(share);
        wx.onMenuShareQZone(share);
    });
}
