
var SystemCommonManager= {
    GetAllUserLevel: function () {
        var objUsrLevel = "";
        var jsonParam = "";
        var serviceUrl = "../Common/GetAllUserLevel/";
        window.AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objUsrLevel = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objUsrLevel;
    },
    GetAllProjectData: function () {
        var objUser = "";
        var jsonParam = "";
        var serviceUrl = "../Common/GetAllProjectList/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {
            objUser = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objUser;
    },

}

var SystemCommonHelper = {
    GenerateProjectCombo: function (identity) {
        var objUsr = SystemCommonManager.GetAllProjectData();
        var obj = new Object();
        obj.ProjectName = "---Select Project---";
        obj.ProjectId = "0";
        objUsr.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "ProjectName",
            dataValueField: "ProjectId",
            dataSource: objUsr,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateUserTypeCombo: function (identity) {
        var objSetNo = [{ TypeId: 1, TypeName: "Admin" }, { TypeId: 2, TypeName: "General User" }];
        var obj = new Object();
        obj.TypeName = "---Select---";
        obj.TypeId = 0;
        objSetNo.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "TypeName",
            dataValueField: "TypeId",
            dataSource: objSetNo,
            index: 0,
            suggest: true,
            filter: "contains"
        });
    },
    GenerateUserLevelCombo: function(identity) {
        var objSetNo = SystemCommonManager.GetAllUserLevel();
        var obj = new Object();
        obj.TypeName = "---Select---";
        obj.TypeId = 0;
        objSetNo.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "TypeName",
            dataValueField: "TypeId",
            dataSource: objSetNo,
            index: 0,
            suggest: true,
            filter: "contains"
        });
    }
}