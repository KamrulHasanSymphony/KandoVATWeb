var CurrentUser = [];


var UserLoginManager = {
    LogInToSystem: function () {
        // CurrentUser = null;
        ikrLoader.Show();
        var logonId = $("#txtLoginId").val();
        var pass = $("#txtpassword").val();
        var ipAddress = $("#divIPAddress").text();

        if (logonId === "" && pass === "") {
            ikrLoader.Hide();
            AjaxManager.NotifyMsg("txtLoginId", "error", "right", 1000, "Required");
            AjaxManager.NotifyMsg("txtpassword", "error", "right", 1000, "Required");
            return;
        }

        if (logonId === "" && pass !== "") {
            ikrLoader.Hide();
            AjaxManager.NotifyMsg("txtLoginId", "error", "right", 1000, "Required");
            return;
        }
        if (pass === "" && logonId !== "") {
            ikrLoader.Hide();
            AjaxManager.NotifyMsg("txtpassword", "error", "right", 1000, "Required");
            return;
        }
        var validator = $("#divUserLogin").kendoValidator().data("kendoValidator"),
            status = $(".status");
        if (validator.validate()) {
            var jsonParam = 'loginId=' + logonId + '&password=' + pass + "&ipAddress=" + ipAddress;
            var serviceURL = "../Home/ValidateUserLogin";
            AjaxManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
        }
        function onSuccess(jsonData) {
            ikrLoader.Hide();
            if (jsonData == "Failed") {
                AjaxManager.MsgBox('error', 'center', 'Login Failed', 'Wrong UserId or Password !</br> Try again.',
                    [{
                        addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {
                            $noty.close();
                            $("#txtpassword").focus();
                        }
                    }]);

            }
            else if (jsonData == "CHANGE") {
                AjaxManager.MsgBox('information', 'center', 'Notification:', 'Please change password after first login',
                    [{
                        addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {
                            $noty.close();
                            window.location.href = "../Home/ChangePassword";
                        }
                    }]);
            }
            else if (jsonData == "CHANGESHORT" || jsonData == "CHANGELEAVE") {
                AjaxManager.MsgBox('success', 'center', 'Chnage Password:', 'Login successful but need to change Password.',
                    [{
                        addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {
                            $noty.close();
                            window.location.href = "../Home/ChangePassword";
                        }
                    }]);
            }
            else if (jsonData == "CHANGESuccess") {
                AjaxManager.MsgBox('success', 'center', 'Chnage Password:', 'Login successful but need to change Password.',
                    [{
                        addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {
                            $noty.close();
                            window.location.href = "../Home/ChangePassword";
                        }
                    }]);
            }
            else if (jsonData == "NOTPERMITTED") {
                AjaxManager.MsgBox('warning', 'center', 'Warning:', 'You have no permission to Login! Please Contact With IT',
                    [{
                        addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {
                            $noty.close();

                        }
                    }]);
            }
            else if (jsonData == "Success") {
                //window.location.href = "../Dashboard/Dashboard";
                UserLoginHelper.RememberMe();
                window.location.href = "../Home/Home";
                //window.location.href = "../Home/Project";
            }
            else {

                AjaxManager.MsgBox('error', 'center', 'Login Failed', jsonData,
                    [{
                        addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {
                            $noty.close();
                            $("#txtpassword").focus();
                        }
                    }]);
            }
        }
        function onFailed(error) {
            AjaxManager.MsgBox('error', 'center', 'Login Failed', error,
                [{
                    addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {
                        $noty.close();
                        $("#txtpassword").focus();
                    }
                }]);
        }

    },
    ResetErrorColor: function () {
        $("#txtLoginId").closest('span').css("border-color", "");
        $("#txtpassword").closest('span').css("border-color", "");
    },
    resetPassword: function () {
        var loginId = $('#txtLoginIdForResetPassword').val();
        var oldpass = $('#txtoldPass').val();
        var password = $('#txtnewPass').val();
        var confirmpass = $('#txtResetConfirmPass').val();

        if (loginId == "") {
            alert("Login ID cannot be blank!");
            $('#txtLoginId').focus();
            return false;
        }

        if (oldpass == password) {
            alert("New password must have to be different from old password!");
            $('#txtnewPass').val('');
            $('#txtResetConfirmPass').val('');
            $('#txtnewPass').focus();
            return false;
        }

        if (password != confirmpass) {
            alert("Password does not match");
            $('#txtResetConfirmPass').val('');
            $('#txtconfirmPass').focus();
            return false;
        }

        var jsonParam = "loginId=" + loginId + "&oldpassword=" + oldpass + "&newpassword=" + confirmpass;
        var serviceURL = "../Home/ResetUserPassword";
        AjaxManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {

            if (jsonData == "Success") {
                AjaxManager.MsgBox('success', 'center', 'Reset Successfull', 'Password reset successfully, Thank you!',
                    [{
                        addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {
                            $noty.close();
                            window.location.href = "../Home/Login";
                        }
                    }]);
            }
            else {
                AjaxManager.MsgBox('error', 'center', 'Failed', jsonData,
                    [{
                        addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {
                            $noty.close();
                        }
                    }]);
            }
        }
        function onFailed(error) {
            AjaxManager.MsgBox('error', 'center', 'Failed', error,
                [{
                    addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {
                        $noty.close();
                    }
                }]);
        }
    },

    ChangePassword: function () {
        var password = $('#txtnewPass').val();
        var confirmpass = $('#txtconfirmPass').val();
        if (password != confirmpass) {
            alert("Password doesnot match");
            $('#txtconfirmPass').val('');
            $('#txtconfirmPass').focus();
            return false;
        }
        var jsonParam = "password=" + password;
        var serviceURL = "../Home/ChangePassword";
        AjaxManager.SendJson(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            if (jsonData == "Success") {
                AjaxManager.MsgBox('success', 'center', 'Success:', 'Password Change Successfully',
                    [{
                        addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {
                            $noty.close();
                            var url = "../Home/Login";
                            window.location.href = url;
                        }
                    }]);
            }
            else {
                AjaxManager.MsgBox('error', 'center', 'Failed', jsonData,
                    [{
                        addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {
                            $noty.close();
                        }
                    }]);
            }
        }
        function onFailed(error) {
            AjaxManager.MsgBox('error', 'center', 'Failed', error,
                [{
                    addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {
                        $noty.close();
                    }
                }]);
        }
    },

    getCurrentUser: function () {
        var jsonParam = '';
        var pathName = window.location.pathname;
        var pageName = pathName.substring(pathName.lastIndexOf('/') + 1);
        var serviceURL = "../Home/GetCurrentUser";
        AjaxManager.GetJsonResult(serviceURL, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {
            CurrentUser = jsonData;
            if (CurrentUser != undefined) {
                var userName = "Welcome: " + CurrentUser.USERNAME + " - [ " + CurrentUser.EMPID + " ]";
                $("#lblWelcome").html(userName);
                //if (CurrentUser.FullLogoPath != null) {
                //    $("#headerLogo").attr('style', 'background-image: url("' + CurrentUser.FullLogoPath + '") !important');
                //}
            }
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
    },
};
var UserLoginHelper = {
    RememberMe: function () {
        if ($('#chkRememeberMe').is(':checked')) {
            localStorage.usrname = $('#txtLoginId').val();
            localStorage.pass = $('#txtpassword').val();
            localStorage.chkbx = $('#chkRememeberMe').val();
        } else {
            localStorage.usrname = "";
            localStorage.pass = "";
            localStorage.chkbx = "";
        }
    }
};