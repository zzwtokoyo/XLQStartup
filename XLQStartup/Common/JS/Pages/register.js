//短信60秒验证
var InterValObj; //timer变量，控制时间
var count = 61; //间隔函数，1秒执行
var curCount;//当前剩余秒数    
//var args = getArgs();
var member = "";
var container = $("#register");
$(function () {

    share(document.title, "快来注册Adobe会员，参与Adobe精彩活动并获取资料下载！", "http://wechat.adobe-event.cn/pages/Member/register.html?1=1");

    $(".regist-radio").iCheck({
        checkboxClass: 'icheckbox_flat-orange',
        radioClass: 'iradio_flat-orange'
    });
    $(".agreed").iCheck({
        checkboxClass: 'icheckbox_flat',
        radioClass: 'iradio_flat'
    });

    $(".regist-radio").on('ifChecked', function (event) {
        if (event.currentTarget.id == "rdoN") {
            //隐藏
            $("#adobeIDgroup").hide();
        }
        else {
            $("#adobeIDgroup").show();
        }
    });

    //如果已经是注册会员，则跳转到
    //获取当前fans 信息
    var apiUrl = "/API/Member/GetMember?openId=" + openId;
    easyweb.server.load(apiUrl, null,
        function (result) {
            member = result;
            if (member == null) {
                $("#wholePage").show();
                return;
            }
            else if (member.RegTime != null & (member.RegTime != "" & member.RegTime != "0001-01-01 00:00:00")) {
                if (member.CardNo != null || member.CardNo != "") {
                    //会员信息页面
                    window.location = "/Pages/Member/center.html?openId=" + member.OpenId;
                }
                else {
                    //领取卡券页面
                    window.location = "/Pages/Member/receivemembercard.html?openId=" + member.OpenId;
                }
            }

            $("#wholePage").show();

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
});

function register() {

    checkName();
    checkMobile();
    checkEmail();
    checkCompanyName();

    if (!easyweb.binder.checkValid(container)) return false;


    if ($("#chkAgree2").parent().hasClass("checked") == false) {
        alert("请选择同意使用条款和隐私政策");
        return false;
    }

    if ($("#rdoY").parent().hasClass("checked")) {
        var adobeId = $("#txtAdobeId").val();
        if (adobeId == "") {
            alert("请填写AdobeID");
        }
    }

    var code = $("#txtValidateCode").val();

    var apiUrl = "/API/Member/Register?vercode=" + code;
    var parms = adKeywordBatch = easyweb.binder.bindControlToJson(container, parms);
    var ReceiveMessage = $("#chkReceiveMessage").parent().hasClass("checked");
    parms["ReceiveMessage"] = ReceiveMessage;
    parms["OpenId"] = openId;
    easyweb.server.load(apiUrl, { "memberInfo": JSON.stringify(parms), "vercode": code },
        function (result) {
            if (args.actId != undefined && args.actId != "") {
                if (args.from != undefined && args.from != "" && args.from == "attend") {
                    window.location = "/Pages/Activity/attend.html?openId=" + member.OpenId + "&id=" + args.actId;
                }
                else {
                    window.location = "/Pages/Activity/detail.html?openId=" + member.OpenId + "&id=" + args.actId;
                }

            }
            else {
                //进入到领取会员卡界面
                window.location = "/Pages/Member/receivemembercard.html?openId=" + openId;
            }
        });
}

function sendValidateCode() {
    var mobile = $("#txtMobile").val();
    if (mobile == "") {
        alert("请输入手机号");
        return false;
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

function checkValidate() {
    easyweb.binder.checkValid(container);
    return false;
}





