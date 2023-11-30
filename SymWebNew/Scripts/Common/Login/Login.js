$(document).ready(function () {
    $("#txtLoginId").focus();
    if (localStorage.chkbx && localStorage.chkbx !== "") {
        $("#chkRememeberMe").attr("checked", "checked");
        $("#txtLoginId").val(localStorage.usrname);
        $("#txtpassword").val(localStorage.pass);
    } else {
        $("#chkRememeberMe").removeAttr("checked");
        $("#txtLoginId").val("");
        $("#txtpassword").val("");
    }
    $("#btnLogIn").click(function () {
        UserLoginManager.LogInToSystem();
    });
    $("#txtpassword").keypress(function (event) {
        if (event.keyCode === 13) {
            if ($("#txtLoginId").val() === "") {
                $("#txtLoginId").focus();
            } else {
                UserLoginManager.LogInToSystem();
            }
        }
    });
    $("#txtLoginId").keypress(function (event) {
        if (event.keyCode === 13) {
            if ($("#txtpassword").val() === "") {
                $("#txtpassword").focus();
            } else {
                UserLoginManager.LogInToSystem();
            }
        }
    });
    $("#btnLogOff").click(function () {
        LogOffManager.LogOff();
    });

    ForgotPasswordHelper.InitForgotPassword();

    //UserProjectManage
    //UserProjectHelper.IntiUserProject();
});

