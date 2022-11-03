//该 JS 由 zgcwkj 编写于 20210527

//Url跳转
function JumpUrl(url) {
    window.location.href = url;
}

//返回上一页
function BackToPage() {
    window.history.go(-1);
}

//通过路径获取传递的参数
function GetQueryString(name) {
    var result = window.location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (result == null || result.length < 1) {
        return "";
    }
    return result[1];
}

//前台角色控制
function RoleControl() {
    //通过地址获取传递的变量
    var behavior = GetQueryString("Behavior");
    //避免变量为空的问题
    if (behavior != "") {
        //通过字符分割获取到拥有的权限
        var behaviors = behavior.split(",");
        //将拥有的权限显示出来
        for (i = 0; i < behaviors.length; i++) {
            $("." + behaviors[i]).removeClass("layui-hide");
        }
    }
}

//下拉框绑定数据
function SelectBinding(name, url) {
    $.post(url, function (data) {
        for (var i = 0; i < data.length; i++) {
            $(name).append("<option value='" + data[i].id + "'>" + data[i].name + "</option>");
        }
        form.render();
    });
}

//下拉框绑定数据并选择指定的列
function SelectBindingById(name, url, id) {
    $.post(url, function (data) {
        for (var i = 0; i < data.length; i++) {
            $(name).append("<option value='" + data[i].id + "'>" + data[i].name + "</option>");
        }
        $(name).val(id);
        form.render();
    });
}

//打开新的窗体方法
function Open_the_form() {
    //js是不支持重载的，通过但是arguments的参数个数可以实现重载的效果：
    if (arguments.length == 1)
        popup = window.open(arguments[0], 'MenuPopup', 'width=' + window.screen.width + ',height=' + window.screen.height + ',location=no,scrollbars=no,menubars=no,toolbars=no,resizable=yes');
    else if (arguments.length == 2)
        popup = window.open(arguments[0], 'MenuPopup', 'width=' + window.screen.width + ',height=' + arguments[1] + ',location=no,scrollbars=no,menubars=no,toolbars=no,resizable=yes');
    else if (arguments.length == 3)
        popup = window.open(arguments[0], 'MenuPopup', 'width=' + arguments[1] + ',height=' + arguments[2] + ',location=no,scrollbars=no,menubars=no,toolbars=no,resizable=yes');
    else
        alert('参数错误，请检查程序！'); return;
    popup.focus();//获得焦点
}