var args = getArgs();
var openId = args.openId;

function checkName() {
    var mobile = $("#txtName").val();
    if (mobile == "") {
        alert("请输入姓名");
        return false;
    }
    else if (mobile.length < 2) {
        alert("姓名填写有误，只能由中文或英文组成，且长度不能少于2个字符。");
    }
    else {

        var patrn = /^[A-Za-z\u4e00-\u9fa5]+$/;
        var result = patrn.test(mobile);
        if (result == false) {
            alert("姓名填写有误，只能由中文或英文组成，且长度不能少于2个字符。");
            return false;
        }
    }
}

function checkMobile() {
    var mobile = $("#txtMobile").val();
    if (mobile == "") {
        alert("请输入手机号");
        return false;
    } else {

        var patrn = /^1[3|4|5|7|8][0-9]\d{8}$/;
        var result = patrn.test(mobile);
        if (result == false) {
            alert("手机号码格式错误。请填写11位数字正确手机号码。");
            return false;
        }
    }
}

function checkEmail() {
    var mobile = $("#txtEmail").val();
    if (mobile == "") {
        alert("请输入邮件");
        return false;
    }

    else {

        var patrn = /^[A-Za-z0-9d]+([-_.][A-Za-z0-9d]+)*@([A-Za-z0-9d]+[-.])+[A-Za-zd]{2,5}$/;
        var result = patrn.test(mobile);
        if (result == false) {
            alert("邮箱格式错误");
            return false;
        }
    }
}

function checkAdobeID() {
    var mobile = $("#txtAdobeId").val();
    if (mobile == "") {
        return false;
    }
    else {
        var patrn = /^[A-Za-z0-9d]+([-_.][A-Za-z0-9d]+)*@([A-Za-z0-9d]+[-.])+[A-Za-zd]{2,5}$/;
        var result = patrn.test(mobile);
        if (result == false) {
            alert("格式错误。Adobe ID为您此前注册Adobe ID的邮箱地址，请正确填写。");
            return false;
        }
    }
}

function checkCompanyName() {
    var mobile = $("#txtCompanyName").val();
    if (mobile == "") {
        alert("请输入公司名称");
        return false;
    }
    else if (mobile.length < 2) {
        alert("“公司名称”填写有误，只能由中英文或数字组成，且长度不少于2个字符。");
    }
    else {

        var patrn = /^[A-Za-z\u4e00-\u9fa5\0-9]+$/;
        var result = patrn.test(mobile);
        if (result == false) {
            alert("“公司名称”填写有误，只能由中英文或数字组成，且长度不少于2个字符。");
            return false;
        }
    }
}