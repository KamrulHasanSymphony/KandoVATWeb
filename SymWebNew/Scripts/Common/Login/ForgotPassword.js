
var ForgotPasswordManager = {
    ForgotPassword: function () {
        var empCode = $("#txtEmpId").val();
        var officeEmail = $("#txtEmailOffice").val();
        var splitArr = officeEmail.split(/@/);

        if (empCode === "" ) {
            AjaxManager.NotifyMsg("txtEmpId", "error", "right", 1000, "Required");
            return;
        }
        if (officeEmail === "" || splitArr[0]==="") {
            AjaxManager.NotifyMsg("txtEmailOffice", "error", "right", 1000, "Required");
            return;
        }
        var passObj = ForgotPasswordHelper.CreateForgotPassObject();
        var objForgotPass = JSON.stringify(passObj);
        var jsonParam = 'objForgotPass:' + objForgotPass;
        var serviceURL = "../Home/ForgotPassword";
        AjaxManager.SendJson2(serviceURL, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            if (jsonData.Message == "Success") {
                AjaxManager.MsgBox('warning', 'center', 'Check Mail', 'A password has been send to your mail. <br/> Please check your email.',
                    [{
                        addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {
                            $noty.close();
                            $("#popupForgotPass").data("kendoWindow").close();
                        }
                    }]);

            }
            else {
                AjaxManager.MsgBox('warning', 'center', 'Warning', jsonData.Message,
                        [{
                            addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {
                                $noty.close();
                            }
                        }]);
            }
        }
        function onFailed(error) {
            AjaxManager.MsgBox('error', 'center', 'Error', error.statusText,
                        [{
                            addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {
                                $noty.close();
                            }
                        }]);
        }

    }
};

var ForgotPasswordHelper = {
    InitForgotPassword: function () {
        $("#popupForgotPass").kendoWindow({
            title: "Password Reset",
            resizeable: false,
            width: "40%",
            actions: ["Close"],
            modal: true,
            scrollable: false,
            visible: false
        });

        $("#txtEmailOffice").val("@hameemgroup.com");

        $("#btnForgotPass").click(function () {
            $("#popupForgotPass").data("kendoWindow").open().center();
        });
        $("#btnCancelForgotPass").click(function () {
            $("#popupForgotPass").data("kendoWindow").close();
        });

        $("#btnResetPass").click(function () {
            ForgotPasswordManager.ForgotPassword();
        });
    },
    CreateForgotPassObject:function() {
        var obj = new Object();
        obj.EmpCode = $("#txtEmpId").val();
        obj.EmailOffice = $("#txtEmailOffice").val();
        return obj;
    }
};