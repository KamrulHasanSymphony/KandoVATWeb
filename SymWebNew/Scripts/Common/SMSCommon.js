
var smsCommonManager = {

    GetHierarchyCompany: function () {
        var objCompany = "";
        var jsonParam = "";
        var serviceUrl = "../Company/GetMotherCompany/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objCompany = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objCompany;
    },

    GenerateBranchCombo: function (companyId) {
        var objBranch = "";
        var jsonParam = "companyId=" + companyId;
        var serviceUrl = "../../Branch/GetBranchByCompanyIdForCombo/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objBranch = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objBranch;
    },
    GenerateAllBranchCombo: function (rootCompanyId) {
        var objBranch = "";
        var jsonParam = "rootCompanyId=" + rootCompanyId;
        var serviceUrl = "../../Branch/GetAllBranchByCompanyIdForCombo/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objBranch = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objBranch;
    },

    GetActionButtonByState: function (stateId) {
        var objAction = "";
        var jsonParam = "stateId=" + stateId;
        var serviceUrl = "../Status/GetActionByStateIdAndUserId/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objAction = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objAction;
    },

    GetClient: function (companyId) {
        var objClient = "";
        var jsonParam = "companyId=" + companyId;
        var serviceUrl = "../Client/GetClient/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objClient = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objClient;
    },

    GenerateDesignationCombo: function (companyId) {
        var objDesignation = "";
        var jsonParam = "companyId=" + companyId + "&status=" + 1;
        var serviceUrl = "../Designation/GetAllDesignationByCompanyIdAndStatus/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objDesignation = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objDesignation;
    },

    GenerateDesignationByDepartmentIdCombo: function (departmentId) {
        var objDesignation = "";
        var jsonParam = "departmentId=" + departmentId + "&status=" + 1;
        var serviceUrl = "../Designation/GenerateDesignationByDepartmentIdCombo/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objDesignation = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objDesignation;
    },

    GetTrainingType: function () {
        var obj = "";
        var JsonParam = "";
        var serviceUrl = "../TrainingInfo/GetTrainingTypeForCombo";
        AjaxManager.GetJsonResult(serviceUrl, JsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {

            obj = jsonData;
        }
        function onFailed() {
            window.alert(error.statusText);
        }

        return obj;
    },

    GetDataForAnyCombo: function (serviceUrl) {
        var obj = "";
        var JsonParam = "";
        //var serviceUrl = "../TrainingInfo/GetTrainingTypeForCombo";
        AjaxManager.GetJsonResult(serviceUrl, JsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {

            obj = jsonData;
        }
        function onFailed() {
            window.alert(error.statusText);
        }

        return obj;
    },

    GenerateCommonGrid: function (ctlId, url, columns) {
        $("#" + ctlId).kendoGrid({
            dataSource: smsCommonManager.gridDataSource(url),
            pageable: {
                refresh: true,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true
            },
            filterable: true,
            sortable: true,
            columns: columns,
            editable: false,
            navigatable: true,
            selectable: "row",
        });
    },

    gridDataSource: function (url) {
        var gridDataSource = new kendo.data.DataSource({
            type: "json",
            serverPaging: true,

            serverSorting: true,

            serverFiltering: true,

            allowUnsort: true,

            pageSize: 10,

            transport: {
                read: {
                    //url: '../AccessControl/GetAccessControlSummary/',
                    url: url,

                    type: "POST",

                    dataType: "json",

                    contentType: "application/json; charset=utf-8"
                },
                update: {
                    //url: '../AccessControl/GetAccessControlSummary/',
                    url: url,
                    dataType: "json"
                },

                parameterMap: function (options) {

                    return JSON.stringify(options);

                }
            },
            schema: { data: "Items", total: "TotalCount" }
        });
        return gridDataSource;
    },

    GetBranchInfoByBranchId: function (branchId) {
        var objBranch = "";
        var jsonParam = "branchId=" + branchId;
        var serviceUrl = "../../Branch/GetBranchInfoByBranchId/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {

            objBranch = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objBranch;
    },

    GetDiscountAmountByType: function (discountType) {
        var objDiscount = "";
        var jsonParam = "discountType=" + discountType;
        var serviceUrl = "../../Discount/GetDiscountInfoByType/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {

            objDiscount = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objDiscount;
    },

    GetDiscountTypeCombo: function () {
        var objDiscountType = "";
        var jsonParam = "";
        var serviceUrl = "../../Discount/GetDiscountTypeCombo/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objDiscountType = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objDiscountType;
    },

    GetAllItemData: function () {
        var objModel = "";
        var jsonParam = "";
        var serviceUrl = "../Product/GetAllItemForCombo/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {
            objModel = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objModel;
    },

    GetUserLevel: function (module) {
        var isRootLevelUser = "";
        var jsonParam = "module=" + module;
        var serviceUrl = "../Common/CheckIsRootLevelAdmin/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {
            isRootLevelUser = jsonData;
        }
        function onFailed(error) {
        }
        return isRootLevelUser;
    },

    GetAllPaymentTypeData: function () {
        var obj = "";
        var jsonParam = "";
        var serviceUrl = "../Common/GetAllPaymentTypeData/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {
            obj = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return obj;
    },

    GetAllSupplierData: function () {
        var obj = "";
        var jsonParam = "";
        var serviceUrl = "../Common/GetAllSupplierData/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {
            obj = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return obj;
    },

    GetAllUserData: function () {
        var objUser = "";
        var jsonParam = "";
        var serviceUrl = "../Home/GetUserList/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objUser = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objUser;
    },
    GetAllUserDataBySearchKey: function (srcKey) {
        var objUser = "";
        var jsonParam = "";
        var serviceUrl = "../Home/GetUserListBySerachKey?srcKey=" + srcKey;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objUser = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objUser;
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
    }
};


var smsCommonHelper = {
    initePanelBer: function (ctlDivId) {
        var original = $("#" + ctlDivId).clone(true);
        original.find(".k-state-active").removeClass("k-state-active");

        $(".configuration input").change(function () {
            var panelBar = $("#" + ctlDivId),
                clone = original.clone(true);

            panelBar.data("kendoPanelBar").collapse($("#" + ctlDivId + " .k-link"));

            panelBar.replaceWith(clone);

            initPanelBar();
        });

        var initPanelBar = function () {
            $("#" + ctlDivId).kendoPanelBar({ animation: { expand: { duration: 500, } } });
        };

        initPanelBar();

    },
    GenerateProjectCombo: function (identity) {
        var objUsr = smsCommonManager.GetAllProjectData();
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

    GenerareHierarchyCompanyCombo: function (identity) {
        var objCompany = smsCommonManager.GetHierarchyCompany();
        $("#" + identity).kendoComboBox({
            placeholder: "Select Company",
            dataTextField: "CompanyName",
            dataValueField: "CompanyId",
            dataSource: objCompany,

        });
    },

    GenerateBranchCombo: function (companyId, identity) {
        var objBranch = smsCommonManager.GenerateBranchCombo(companyId);
        $("#" + identity).kendoComboBox({
            placeholder: "All",
            dataTextField: "BranchName",
            dataValueField: "BranchId",
            dataSource: objBranch,

            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateAllBranchCombo: function (rootCompanyId, identity) {
        var objBranch = smsCommonManager.GenerateAllBranchCombo(rootCompanyId);
        $("#" + identity).kendoComboBox({
            placeholder: "All",
            dataTextField: "BranchName",
            dataValueField: "BranchId",
            dataSource: objBranch
        });
    },
    GetDepartmentByCompanyId: function (companyId, identity) {
        var objDepartment = new Object();

        objDepartment = smsCommonManager.GetDepartmentByCompanyId(companyId);

        $("#" + identity).kendoComboBox({
            placeholder: "Select Department",
            dataTextField: "DepartmentName",
            dataValueField: "DepartmentId",
            dataSource: objDepartment
        });

    },

    commonValidator: function (ctlId) {
        var data = [];
        var validator = $("#" + ctlId).kendoValidator().data("kendoValidator"),
            status = $(".status");
        if (validator.validate()) {
            status.text("").addClass("valid");
            return true;
        } else {
            status.text("Oops! There is invalid data in the form.").addClass("invalid");
            return false;
        }

    },

    GenerateDivisionCombo: function (sectionId, identity) {
        //var objSection = smsCommonManager.GetClassData(sectionId);
        $("#" + identity).kendoComboBox({
            placeholder: "All",
            dataTextField: "DivisionName",
            dataValueField: "DivisionId",
            dataSource: [],
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateDistrictCombo: function (sectionId, identity) {
        //var objSection = smsCommonManager.GetClassData(sectionId);
        $("#" + identity).kendoComboBox({
            placeholder: "All",
            dataTextField: "DistrictName",
            dataValueField: "DistrictId",
            dataSource: [],
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateThanaCombo: function (sectionId, identity) {
        //var objSection = smsCommonManager.GetClassData(sectionId);
        $("#" + identity).kendoComboBox({
            placeholder: "All",
            dataTextField: "ThanaName",
            dataValueField: "ThanaId",
            dataSource: [],
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
   
    GenerateStatusCombo: function (identity) {

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "Status",
            dataValueField: "StatusId",
            index: 0,
            animation: false,
            autoWidth: true,
            dataSource: [{ StatusId: "0", Status: "All" }, { StatusId: "A", Status: "Active" }, { StatusId: "D", Status: "InActive" }]//MeetingDetailsManager.GetClientData()
        });
    },
  
    GenerateUserBlankCombo: function (identity) {
        //var objUsr = smsCommonManager.GetAllUserData();
        var objUsr = [];
        var obj = new Object();
        obj.USERNAME = "---Select User---";
        obj.EMPID = "0";
        objUsr.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "USERNAME",
            dataValueField: "EMPID",
            dataSource: objUsr,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateUserCombo: function (identity) {
        var objUsr = smsCommonManager.GetAllUserData();
        var obj = new Object();
        obj.USERNAME = "All";
        obj.EMPID = "0";
        objUsr.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "USERNAME",
            dataValueField: "EMPID",
            dataSource: objUsr,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateUserCombo2: function (identity) {
        var objUsr = smsCommonManager.GetAllUserData();
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "USRNAME",
            dataValueField: "USRID",
            dataSource: objUsr,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateUserCombo3: function (identity) {

        var objUsr = smsCommonManager.GetAllUserData();
        var obj = new Object();
        obj.USRNAME = "--Select--";
        obj.USRID = "0";
        objUsr.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "USRNAME",
            dataValueField: "USRID",
            dataSource: objUsr,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    }
};