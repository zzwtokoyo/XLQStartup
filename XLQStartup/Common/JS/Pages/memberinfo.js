//短信60秒验证
var InterValObj; //timer变量，控制时间
var count = 61; //间隔函数，1秒执行
var curCount;//当前剩余秒数
var container = $("#memberInfo");
//var args = getArgs();

$(function () {
    wx.ready(function () {
        wx.hideAllNonBaseMenuItem();
    });

    var selProvince = $("#ProvinceReg");
    var selCity = $("#CityReg");

    easyweb.binder.bindSelect(selProvince, regionModel.ProvinceList, {
        displayMember: "Name", valueMember: 'Code'
        //emptyOption: { value: "", text: "省份" }
    });
    easyweb.binder.bindSelect(selCity, regionModel.CityList, {
        displayMember: "Name", valueMember: 'Code', parentMember: 'ParentCode',
        parentSelect: selProvince,
        parentChange: function () {
            $('.selectpicker').selectpicker('refresh');
        }
        //emptyOption: { value: "", text: "城市" }
    });
    Init();
});

function Init() {

    var apiUrl = "/API/Member/GetMember"
    easyweb.server.load(apiUrl, { "openId": openId },
        function (result) {
            easyweb.binder.bindJsonToControl(container, result);
            $('#ProvinceReg').selectpicker('val', result.ProvinceReg);
            $('#ProvinceReg').change();
            $('#CityReg').selectpicker('val', result.CityReg);
            $('#Industry').selectpicker('val', result.Industry);
            $('#CompanyTitle').selectpicker('val', result.CompanyTitle);
            $('#CompanySize').selectpicker('val', result.CompanySize);
            //if (result.AdobeID != null && result.AdobeID != "") {
            //    $('#AdobeID').html(result.AdobeID);
            //}
            //else {
            //    $('#AdobeID').html("无");
            //}

        });
}

function updateMember() {
    checkName();
    checkMobile();
    checkEmail();
    checkCompanyName();

    if (!easyweb.binder.checkValid(container)) return false;
    var code = $("#txtValidateCode").val();
    var apiUrl = "/API/Member/Update?vercode=" + code;
    var parms = adKeywordBatch = easyweb.binder.bindControlToJson(container, parms);
    var ReceiveMessage = $("#chkReceiveMessage").val() == "on" ? true : false
    parms["ReceiveMessage"] = ReceiveMessage;
    parms["OpenId"] = openId;
    easyweb.server.load(apiUrl, { "memberInfo": JSON.stringify(parms), "vercode": code },
        function (result) {
            alert("修改会员信息成功");
        });
}

function sendValidateCode() {
    var mobile = $("#txtMobile").val();
    if (mobile == "") {
        alert("请输入手机号");
        return;
    } else {

        var patrn = /^1[3|4|5|7|8][0-9]\d{8}$/;
        var result = patrn.test(mobile);
        if (result == false) {
            alert("手机号码格式错误");
            return false;
        }
    }
    curCount = count;
    canSendCode = 0;
    $("#btnSendCode").attr("disabled", "true"); //设置button效果，开始计时

    InterValObj = window.setInterval(SetRemainTime, 1000); //启动计时器，1秒执行一次

    var apiUrl = "/API/Member/SendRegCode?mobile=" + $("#txtMobile").val();
    easyweb.server.load(apiUrl, null,
        function (result) {
            alert("发送成功");
        });
}



//timer处理函数
function SetRemainTime() {
    if (curCount == 0) {
        window.clearInterval(InterValObj);//停止计时器
        canSendCode = 1;
        $("#btnSendCode").removeAttr("disabled");//启用按钮
        $("#btnSendCode").text("发送验证码");
    }
    else {
        curCount--;
        $("#btnSendCode").text(curCount + "秒后重试");
    }
}

