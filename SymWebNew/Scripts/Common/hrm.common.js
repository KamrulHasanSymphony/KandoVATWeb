/// <reference path="common.js" />
var HrmCommonManager = {
    GenerateYearList:function() {
        var lvYearList = [];
        var currentYear = (new Date()).getFullYear();
        for (var i = 2001; i <= currentYear; i++) {
            var lvYear = new Object();
            lvYear.LeaveYearName = i;
            lvYear.LeaveYear = i;
            lvYearList.push(lvYear);
        }
        return lvYearList.sort(function (a, b) {
            return b.LeaveYear - a.LeaveYear;
        });
    },
    GetHRMModeInfo: function (unitId) {
        var objPs = "";
        var jsonParam = "";
        var serviceUrl = "../Common/GetHRMMode?unitId=" + unitId;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {
            objPs = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objPs;
    },
    GetAllDepartmentForTransport: function () {
        var objComp = "";
        var jsonParam = "";
        var serviceUrl = "../Common/GetAllDepartment/";
        window.AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objComp = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objComp;
    },
    GetAllCompany: function () {
        var objComp = "";
        var jsonParam = "";
        var serviceUrl = "../Company/GetAllCompany/";
        window.AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objComp = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objComp;
    },
    GetAllCompanyNoPermission: function () {
        var objComp = "";
        var jsonParam = "";
        var serviceUrl = "../Company/GetAllCompany2/";
        window.AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objComp = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objComp;
    },
    GetAllUnit: function () {
        var objUnit = "";
        var jsonParam = "";
        var serviceUrl = "../Unit/GetAll/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objUnit = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        debugger;
        return objUnit;
    },
    GetAllLegalCompany: function () {
        var objComp = "";
        var jsonParam = "";
        var serviceUrl = "../Company/GetAllLegalCompany/";
        window.AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objComp = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objComp;
    },


    GetAllUnitByCompId: function (compId) {
        debugger;
        var objUnit = "";
        var jsonParam = "";
        var serviceUrl = "../Unit/GetAllUnitByCompId/?compId=" + compId;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objUnit = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objUnit;
    },
    GetAllUnitByCompIdNoPermission: function (compId) {
        var objUnit = "";
        var jsonParam = "";
        var serviceUrl = "../Unit/GetAllUnitByCompId2/?compId=" + compId;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objUnit = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objUnit;
    },
    GetAllDepartment: function (unitId) {
        var objDept = "";
        var jsonParam = "";
        var serviceUrl = "../Department/GetAll/?unitId=" + unitId;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objDept = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objDept;
    },
    GetAllDepartmentNoPermission: function (unitId) {
        var objDept = "";
        var jsonParam = "";
        var serviceUrl = "../Department/GetAll2/?unitId=" + unitId;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objDept = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objDept;
    },
    GetAllMultiUnit: function (objParam) {
        var objDept = "";
        var jsonParam = "objParam:" + JSON.stringify(objParam);
        var serviceUrl = "../Unit/GetAllMultiUnit/";
        AjaxManager.SendJson2(serviceUrl, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objDept = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objDept;
    },
    GetAllMultiDepartment: function (objParam) {
        var objDept = "";
        var jsonParam = "objParam:" + JSON.stringify(objParam);
        var serviceUrl = "../Department/GetAllMultiDepartment/";
        AjaxManager.SendJson2(serviceUrl, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objDept = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objDept;
    },
    GetAllSection: function (unitId, deptId) {
        var objSec = "";
        var jsonParam = "";
        var serviceUrl = "../Section/GetAll/?unitId=" + unitId + "&deptId=" + deptId;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objSec = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objSec;
    },
    GetAllMultiSection: function (objParam) {
        var objSec = "";
        var jsonParam = "objParam:" + JSON.stringify(objParam);
        var serviceUrl = "../Section/GetAllMultiSection/";
        AjaxManager.SendJson2(serviceUrl, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objSec = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objSec;
    },
    GetAllWing: function (unitId, deptId, secId) {
        var objWing = "";
        var jsonParam = "";
        var serviceUrl = "../Wing/GetAll/?unitId=" + unitId + "&deptId=" + deptId + "&secId=" + secId;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objWing = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objWing;
    },
    GetAllMultiWing: function (objParam) {
        var objWings = "";
        var jsonParam = "objParam:" + JSON.stringify(objParam);
        var serviceUrl = "../Wing/GetAllMultiWing/";
        AjaxManager.SendJson2(serviceUrl, jsonParam, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objWings = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objWings;
    },
    GetAllTeam: function (unitId, deptId, secId, wingId) {
        var objTeam = "";
        var jsonParam = "";
        var serviceUrl = "../Team/GetAll/?unitId=" + unitId + "&deptId=" + deptId + "&secId=" + secId + "&wingId=" + wingId;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objTeam = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objTeam;
    },
    GetAllMultiTeam: function (objParam) {
        var objTeams = "";
        var jsonParam = "objParam:" + JSON.stringify(objParam);
        var serviceUrl = "../Team/GetAllMultiTeam/";
        AjaxManager.SendJson2(serviceUrl, jsonParam, onSuccess, onFailed);
        function onSuccess(jsonData) {
            objTeams = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objTeams;
    },
    GetAllDesignation: function () {
        var objDes = "";
        var jsonParam = "";
        var serviceUrl = "../Designation/GetAll/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objDes = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objDes;
    },
    GetAllDesignationByDept: function (unitId, deptId) {
        var objDesig = "";
        var jsonParam = "";
        var serviceUrl = "../Designation/GetAllDesignationByDept/?unitId=" + unitId + "&deptId=" + deptId;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objDesig = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objDesig;
    },
    GetAllDesignationGroup: function () {
        var objDesGroup = "";
        var jsonParam = "";
        var serviceUrl = "../Designation/GetAllDesignationGroup/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objDesGroup = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objDesGroup;
    },
    GetAllPosition: function () {
        var objPos = "";
        var jsonParam = "";
        var serviceUrl = "../Position/GetAll/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objPos = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objPos;
    },
    GetAllEmploymentStatus: function () {
        var objEmpSt = "";
        var jsonParam = "";
        var serviceUrl = "../EmploymentStatus/GetAll/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objEmpSt = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objEmpSt;
    },
    GetAllEmploymentType: function () {
        var objEmpTy = "";
        var jsonParam = "";
        var serviceUrl = "../EmploymentType/GetAll/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objEmpTy = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objEmpTy;
    },
    GetAllEmploymentCategory: function () {
        var objEmpCat = "";
        var jsonParam = "";
        var serviceUrl = "../EmploymentCategory/GetAll/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objEmpCat = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objEmpCat;
    },

    GetAllCountry: function () {
        var objCou = "";
        var jsonParam = "";
        var serviceUrl = "../Country/GetAll/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objCou = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objCou;
    },
    GetAllDivision: function () {
        var objCou = "";
        var jsonParam = "";
        var serviceUrl = "../Country/GetAllDivision/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objCou = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objCou;
    },
    GetAllDistrict: function (divisionId) {
        var objCou = "";
        var jsonParam = "";
        var serviceUrl = "../Country/GetAllDistrict/?divisionId=" + divisionId;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objCou = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objCou;
    },
    GetAllDistrictCombo: function () {
        var objCou = "";
        var jsonParam = "";
        var serviceUrl = "../Common/GetAllDistrict/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objCou = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        debugger;
        return objCou;
    },
    GetAllThana: function (districtId) {
        var objCou = "";
        var jsonParam = "";
        var serviceUrl = "../Country/GetAllThana/?districtId=" + districtId;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objCou = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objCou;
    },
    GetAllGender: function () {
        var objGen = "";
        var jsonParam = "";
        var serviceUrl = "../Gender/GetAll/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objGen = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objGen;
    },
    GetAllReligion: function () {
        var objRel = "";
        var jsonParam = "";
        var serviceUrl = "../Religion/GetAll/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objRel = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objRel;
    },
    GetAllMaritalStatus: function () {
        var objMar = "";
        var jsonParam = "";
        var serviceUrl = "../MaritalStatus/GetAll/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objMar = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objMar;
    },
    GetAllBloodGroup: function () {
        var objBlo = "";
        var jsonParam = "";
        var serviceUrl = "../BloodGroup/GetAll/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objBlo = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objBlo;
    },
    GetAllShift: function () {
        var objShift = "";
        var jsonParam = "";
        var serviceUrl = "../EmployeeShift/GetAll/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objShift = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objShift;
    },

    GetAllShiftType: function () {
        var objShiftType = "";
        var jsonParam = "";
        var serviceUrl = "../Shift/GetAllShiftType/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objShiftType = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objShiftType;
    },
    GetAllLeaveType: function (isHalfDay) {
        var objLeaveType = "";
        var jsonParam = "";
        var serviceUrl = "../Leave/GetAllLeaveType/?isHalfDay=" + isHalfDay;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objLeaveType = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objLeaveType;
    },
    GetAllManualLeaveType: function (isHalfDay) {
        var objLeaveType = "";
        var jsonParam = "";
        var serviceUrl = "../Leave/GetAllManualLeaveType/?isHalfDay=" + isHalfDay;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objLeaveType = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objLeaveType;
    },
    GetAllSiteDutyType: function () {
        var objLeaveType = "";
        var jsonParam = "";
        var serviceUrl = "../SiteDuty/GetAllSiteDutyType/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objLeaveType = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objLeaveType;
    },
    GetAllApplyTo: function () {
        var objLeaveType = "";
        var jsonParam = "";
        var serviceUrl = "../Leave/GetAllLeaveApplyTo/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objLeaveType = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objLeaveType;
    },
    GetAllActionStatus: function () {
        var objLeaveType = "";
        var jsonParam = "";
        var serviceUrl = "../Leave/GetAllActionStatus/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objLeaveType = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objLeaveType;
    },
    GetAllSeperationType: function () {
        var objEmpStatus = "";
        var jsonParam = "";
        var serviceUrl = "../EmployeeClearance/GetAllSeperationType/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objEmpStatus = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objEmpStatus;
    },
    GetAllOfficialInOutType: function() {
        var objInOut = "";
        var jsonParam = "";
        var serviceUrl = "../OfficialInOut/GetAllOfficialInOutType/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objInOut = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objInOut;
    },
    GetAllPaymentMode: function() {
        var objInOut = "";
        var jsonParam = "";
        var serviceUrl = "../Common/GetAllPaymentMode/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objInOut = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objInOut;
    },
    GetAllCurrency: function() {
        var objInOut = "";
        var jsonParam = "";
        var serviceUrl = "../Common/GetAllCurrency/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objInOut = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objInOut;
    },
    GetAllSalaryPayCompany: function() {
        var objInOut = "";
        var jsonParam = "";
        var serviceUrl = "../Common/GetAllSalaryPayCompany/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objInOut = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objInOut;
    },
    GetAllBank: function() {
        var objInOut = "";
        var jsonParam = "";
        var serviceUrl = "../Common/GetAllBank/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objInOut = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objInOut;
    },
    GetAllBranch: function(bankId) {
        var objInOut = "";
        var jsonParam = "";
        var serviceUrl = "../Common/GetAllBranchByBank/?bankId="+bankId;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objInOut = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objInOut;
    },
    GetAllCostHead: function() {
        var objCostHead = "";
        var jsonParam = "";
        var serviceUrl = "../Common/GetAllCostHead/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objCostHead = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objCostHead;
    },
    GetAllFormula: function() {
        var objFormula = "";
        var jsonParam = "";
        var serviceUrl = "../Common/GetAllFormula/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objFormula = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objFormula;
    },
    GetAllIncrementType: function () {
        var objIncrType = "";
        var jsonParam = "";
        var serviceUrl = "../Common/GetAllIncrementType/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objIncrType = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objIncrType;
    },
    GetAllDegree: function () {
        var objDegree = "";
        var jsonParam = "";
        var serviceUrl = "../Education/GetAllDegree/";
        window.AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objDegree = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objDegree;
    },
    GetAllBoard: function () {
        var objBoard = "";
        var jsonParam = "";
        var serviceUrl = "../Education/GetAllBoard/";
        window.AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objBoard = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objBoard;
    },
    GetAllResult: function () {
        var objResult = "";
        var jsonParam = "";
        var serviceUrl = "../Education/GetAllResult/";
        window.AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objResult = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objResult;
    },
    GetAllPayrollType: function() {
        var objPayrollType = "";
        var jsonParam = "";
        var serviceUrl = "../EmployeeSalary/GetAllPayrollType/";
        window.AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objPayrollType = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objPayrollType;
    },
    GetAllJobLocation: function () {
        var objJobLoc = "";
        var jsonParam = "";
        var serviceUrl = "../Company/GetAllJobLocation/";
        window.AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objJobLoc = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objJobLoc;
    },
    GetUserAccessPermission: function (pathname) {
        var objAccess = "";
        var jsonParam = "";
        var serviceUrl = "../User/GetUserAccessPermission/?pathname=" + pathname;
        window.AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {
            objAccess = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objAccess;
    },
    GetUserAccessPermissionByUser: function (pathname,userId) {
        var objAccess = "";
        var jsonParam = "";
        var serviceUrl = "../User/GetUserAccessPermissionByUser/?pathname=" + pathname + "&userId=" + userId;
        window.AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {
            objAccess = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objAccess;
    },
    GetAllDesigSpec: function () {
        var objDes = "";
        var jsonParam = "";
        var serviceUrl = "../DesignationSpec/GetAllDesigSpecification/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objDes = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objDes;
    },
    GetReportComboData: function (menuId) {
        var objReportDate = "";
        var jsonParam = "";
        var serviceUrl = "../User/GetReportComboData/?menuId=" + menuId;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {
            objReportDate = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objReportDate;
    },
    GetAllIOUReqestTo: function () {
        var objLeaveType = "";
        var jsonParam = "";
        var serviceUrl = "../IouRequest/GetAllIouReqestToDeptWise/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objLeaveType = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objLeaveType;
    },
    GetAllIOUReqestToForApprover: function (reqById, reqAmt) {
        var objLeaveType = "";
        var jsonParam = "";
        var serviceUrl = "../IouRequest/GetAllIouReqestToDeptWise_ForApprover/?reqById=" + reqById + "&reqAmt=" + reqAmt;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objLeaveType = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objLeaveType;
    },
    GetAllIouActionStatus: function () {
        var objLeaveType = "";
        var jsonParam = "";
        var serviceUrl = "../IouRequest/GetAllIouActionStatus/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objLeaveType = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objLeaveType;
    },
    GetAllCriterion: function() {
        var objCriterion = "";
        var jsonParam = "";
        var serviceUrl = "../IouRequest/GetAllCriterion/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objCriterion = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objCriterion;
    }
};


var HrmCommonHelper = {
    GenerateCompanyCombo: function (identity) {
        debugger;
        var objComp = HrmCommonManager.GetAllCompany();
        var obj = new Object();
        obj.CompanyName = "---Select---";
        obj.CompanyId = 0;
        objComp.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "CompanyName",
            dataValueField: "CompanyId",
            dataSource: objComp,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                window.AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateCompanyComboNoPermission: function (identity) {
        var objComp = HrmCommonManager.GetAllCompanyNoPermission();
        var obj = new Object();
        obj.CompanyName = "---Select---";
        obj.CompanyId = 0;
        objComp.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "CompanyName",
            dataValueField: "CompanyId",
            dataSource: objComp,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                window.AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateUnitByCompCombo: function (identity) {
        var objUnit = [];
        var obj = new Object();
        obj.UnitName = "---Select---";
        obj.UnitNameBan = "---Select---";
        obj.UnitId = 0;
        objUnit.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "UnitName",
            dataValueField: "UnitId",
            dataSource: objUnit,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }

        });
    },
    GenerateUnitCombo: function (identity) {
        var objUnit = HrmCommonManager.GetAllUnit();
        var obj = new Object();
        obj.UnitName = "---Select---";
        obj.UnitNameBan = "---Select---";
        obj.UnitId = 0;
        objUnit.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "UnitName",
            dataValueField: "UnitId",
            dataSource: objUnit,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
            
        });
    },
    GenerateUnitComboAll: function (identity) {
        var objUnit = HrmCommonManager.GetAllUnit();
        var obj = new Object();
        obj.UnitName = "All";
        obj.UnitId = 0;
        objUnit.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "UnitName",
            dataValueField: "UnitId",
            dataSource: objUnit,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },

    GenerateDepartmentComboforTransport: function (identity) {
        var objDepat = HrmCommonManager.GetAllDepartmentForTransport();
        var obj = new Object();
        obj.DepartmentName = "---Select---";
        obj.DepartmentID = 0;
        objDepat.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "DepartmentName",
            dataValueField: "DepartmentID",
            dataSource: objDepat,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },

    GenerateDepartmentCombo: function (identity) {
        var objDepat = [];// HrmCommonManager.GetAllDepartment();
        var obj = new Object();
        obj.DepartmentName = "---Select---";
        obj.DepartmentID = 0;
        objDepat.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "DepartmentName",
            dataValueField: "DepartmentID",
            dataSource: objDepat,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateDepartmentComboAll: function (identity) {
        var objDepat = [];// HrmCommonManager.GetAllDepartment();
        var obj = new Object();
        obj.DepartmentName = "All";
        obj.DepartmentID = 0;
        objDepat.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "DepartmentName",
            dataValueField: "DepartmentID",
            dataSource: objDepat,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },

    GenerateUnitComboByCompanyId: function (identity) {
        var objUnit = [];// HrmCommonManager.GetAllUnitByCompId(compId);
        var obj = new Object();
        obj.UnitName = "---Select---";
        obj.UnitNameBan = "---Select---";
        obj.UnitId = 0;
        objUnit.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "UnitName",
            dataValueField: "UnitId",
            dataSource: objUnit,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }

        });
    },

    GenerateSectionCombo: function (identity) {
        var objSec = [];// HrmCommonManager.GetAllSection();
        var obj = new Object();
        obj.SectionName = "---Select---";
        obj.SectionID = 0;
        objSec.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "SectionName",
            dataValueField: "SectionID",
            dataSource: objSec,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateWingCombo: function (identity) {
        var objWing = [];//HrmCommonManager.GetAllWing();
        var obj = new Object();
        obj.WingName = "---Select---";
        obj.WingID = 0;
        objWing.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "WingName",
            dataValueField: "WingID",
            dataSource: objWing,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateTeamCombo: function (identity) {
        var objTeam = [];// HrmCommonManager.GetAllTeam();
        var obj = new Object();
        obj.TeamName = "---Select---";
        obj.TeamID = 0;
        objTeam.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "TeamName",
            dataValueField: "TeamID",
            dataSource: objTeam,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },

    ChangeEventForLoadCombo: function () {
        $("#ddlCompany").change(function () {
            HrmCommonHelper.CompanyChange();
        });
        $("#ddlUnit").change(function () {
            HrmCommonHelper.UnitChange();
        });
        $("#ddlDepartment").change(function () {
            HrmCommonHelper.DepartmentChange();
        });
        $("#ddlSection").change(function () {
            HrmCommonHelper.SectionChange();
        });
        $("#ddlWing").change(function () {
            HrmCommonHelper.WingChange();
        });

    },

    //===========Without Permission=============
    ChangeEventForLoadComboNoPermission: function () {
        $("#ddlCompany").change(function () {
            HrmCommonHelper.CompanyChangeNoPermission();
        });
        $("#ddlUnit").change(function () {
            HrmCommonHelper.UnitChangeNoPermission();
        });
        $("#ddlDepartment").change(function () {
            HrmCommonHelper.DepartmentChange();
        });
        $("#ddlSection").change(function () {
            HrmCommonHelper.SectionChange();
        });
        $("#ddlWing").change(function () {
            HrmCommonHelper.WingChange();
        });

    },
    CompanyChange: function () {
        var compId = $("#ddlCompany").data("kendoComboBox").value();
        var unitcombo = $("#ddlUnit").data("kendoComboBox");
        var deptCombo = $("#ddlDepartment").data("kendoComboBox");
        var secCombo = $("#ddlSection").data("kendoComboBox");
        var wingCombo = $("#ddlWing").data("kendoComboBox");
        var teamCombo = $("#ddlTeam").data("kendoComboBox");


        if (compId !== "0" && compId !== "") {
            var unitList = HrmCommonManager.GetAllUnitByCompId(compId);
            var obj = new Object();
            obj.UnitName = "---Select---";
            obj.UnitId = 0;
            unitList.unshift(obj);

            unitcombo.value("");
            unitcombo.text("");
            unitcombo.select(0);
            unitcombo.setDataSource(unitList);

            deptCombo.value("");
            deptCombo.text("");
            //deptCombo.select(0);
            deptCombo.setDataSource([]);
            secCombo.value("");
            secCombo.text("");
            secCombo.setDataSource([]);
            secCombo.select(0);
            wingCombo.value("");
            wingCombo.text("");
            wingCombo.setDataSource([]);
            wingCombo.select(0);
            teamCombo.value("");
            teamCombo.text("");
            teamCombo.setDataSource([]);
            teamCombo.select(0);
        } else {
            unitcombo.value("");
            unitcombo.text("");
            unitcombo.select(0);
            unitcombo.setDataSource([]);
            deptCombo.value("");
            deptCombo.text("");
            deptCombo.setDataSource([]);
            deptCombo.select(0);
            secCombo.value("");
            secCombo.text("");
            secCombo.setDataSource([]);
            secCombo.select(0);
            wingCombo.value("");
            wingCombo.text("");
            wingCombo.setDataSource([]);
            wingCombo.select(0);
            teamCombo.value("");
            teamCombo.text("");
            teamCombo.setDataSource([]);
            teamCombo.select(0);

        }
    },
    UnitChange: function () {
        var unitId = $("#ddlUnit").data("kendoComboBox").value();
        var deptCombo = $("#ddlDepartment").data("kendoComboBox");
        var secCombo = $("#ddlSection").data("kendoComboBox");
        var wingCombo = $("#ddlWing").data("kendoComboBox");
        var teamCombo = $("#ddlTeam").data("kendoComboBox");


        if (unitId !== "0" && unitId !== "") {
            var deptList = HrmCommonManager.GetAllDepartment(unitId);
            var obj = new Object();
            obj.DepartmentName = "---Select---";
            obj.DepartmentID = 0;
            deptList.unshift(obj);

            deptCombo.value("");
            deptCombo.text("");
            deptCombo.select(0);
            deptCombo.setDataSource(deptList);

            secCombo.value("");
            secCombo.text("");
            secCombo.setDataSource([]);
            secCombo.select(0);
            wingCombo.value("");
            wingCombo.text("");
            wingCombo.setDataSource([]);
            wingCombo.select(0);
            teamCombo.value("");
            teamCombo.text("");
            teamCombo.setDataSource([]);
            teamCombo.select(0);
        } else {
            deptCombo.value("");
            deptCombo.text("");
            deptCombo.setDataSource([]);
            deptCombo.select(0);
            secCombo.value("");
            secCombo.text("");
            secCombo.setDataSource([]);
            secCombo.select(0);
            wingCombo.value("");
            wingCombo.text("");
            wingCombo.setDataSource([]);
            wingCombo.select(0);
            teamCombo.value("");
            teamCombo.text("");
            teamCombo.setDataSource([]);
            teamCombo.select(0);

        }
    },
    DepartmentChange: function () {
        var unitId = $("#ddlUnit").data("kendoComboBox").value();
        var deptId = $("#ddlDepartment").data("kendoComboBox").value();
        var secCombo = $("#ddlSection").data("kendoComboBox");
        var wingCombo = $("#ddlWing").data("kendoComboBox");
        var teamCombo = $("#ddlTeam").data("kendoComboBox");

       // var desigCombo = $("#ddlDesignation").data("kendoComboBox");
        if (deptId !== "0" && deptId !== "") {
            var sectionList = HrmCommonManager.GetAllSection(unitId, deptId);
            
            var obj = new Object();
            obj.SectionName = "---Select---";
            obj.SectionID = 0;
            sectionList.unshift(obj);

            secCombo.value("");
            secCombo.text("");
            secCombo.select(0);
            secCombo.setDataSource(sectionList);
            wingCombo.value("");
            wingCombo.text("");
            wingCombo.setDataSource([]);
            wingCombo.select(0);
            teamCombo.value("");
            teamCombo.text("");
            teamCombo.setDataSource([]);
            teamCombo.select(0);

            //var desigList = HrmCommonManager.GetAllDesignationByDept(unitId, deptId);
            //var obj1 = new Object();
            //obj1.DesignationName = "---Select---";
            //obj1.DesignationID = 0;
            //desigList.unshift(obj1);

            //desigCombo.value("");
            //desigCombo.text("");
            //desigCombo.select(0);
            //desigCombo.setDataSource(desigList);

        } else {
            secCombo.value("");
            secCombo.text("");
            secCombo.setDataSource([]);
            secCombo.select(0);
            wingCombo.value("");
            wingCombo.text("");
            wingCombo.setDataSource([]);
            wingCombo.select(0);
            teamCombo.value("");
            teamCombo.text("");
            teamCombo.setDataSource([]);
            teamCombo.select(0);
        }
    },
    SectionChange: function () {
        var unitId = $("#ddlUnit").data("kendoComboBox").value();
        var deptId = $("#ddlDepartment").data("kendoComboBox").value();
        var secId = $("#ddlSection").data("kendoComboBox").value();
        var wingCombo = $("#ddlWing").data("kendoComboBox");
        var teamCombo = $("#ddlTeam").data("kendoComboBox");
       
        if (secId !== "0" && secId !== "") {
            var wingList = HrmCommonManager.GetAllWing(unitId, deptId, secId);
            var obj = new Object();
            obj.WingName = "---Select---";
            obj.WingID = 0;
            wingList.unshift(obj);
            wingCombo.value("");
            wingCombo.text("");
            wingCombo.select(0);
            wingCombo.setDataSource(wingList);

            teamCombo.value("");
            teamCombo.text("");
            teamCombo.setDataSource([]);
            teamCombo.select(0);
        } else {
            wingCombo.value("");
            wingCombo.text("");
            wingCombo.setDataSource([]);
            wingCombo.select(0);
            teamCombo.value("");
            teamCombo.text("");
            teamCombo.setDataSource([]);
            teamCombo.select(0);
        }
    },
    WingChange: function () {
        var unitId = $("#ddlUnit").data("kendoComboBox").value();
        var deptId = $("#ddlDepartment").data("kendoComboBox").value();
        var secId = $("#ddlSection").data("kendoComboBox").value();
        var wingId = $("#ddlWing").data("kendoComboBox").value();
        var teamCombo = $("#ddlTeam").data("kendoComboBox");
        if (wingId !== "0" && wingId !== "") {
            var teamList = HrmCommonManager.GetAllTeam(unitId, deptId, secId, wingId);
            var obj = new Object();
            obj.TeamName = "---Select---";
            obj.TeamID = 0;
            teamList.unshift(obj);
            teamCombo.value("");
            teamCombo.text("");
            teamCombo.select(0);
            teamCombo.setDataSource(teamList);

        } else {
            teamCombo.value("");
            teamCombo.text("");
            teamCombo.select(0);
            teamCombo.setDataSource([]);
        }
    },

    //=========================Withou Permission================
    CompanyChangeNoPermission: function () {
        var compId = $("#ddlCompany").data("kendoComboBox").value();
        var unitcombo = $("#ddlUnit").data("kendoComboBox");
        var deptCombo = $("#ddlDepartment").data("kendoComboBox");
        var secCombo = $("#ddlSection").data("kendoComboBox");
        var wingCombo = $("#ddlWing").data("kendoComboBox");
        var teamCombo = $("#ddlTeam").data("kendoComboBox");

        if (compId !== "0" && compId !== "") {
            var unitList = HrmCommonManager.GetAllUnitByCompIdNoPermission(compId);
            var obj = new Object();
            obj.UnitName = "---Select---";
            obj.UnitId = 0;
            unitList.unshift(obj);

            unitcombo.value("");
            unitcombo.text("");
            unitcombo.select(0);
            unitcombo.setDataSource(unitList);

            deptCombo.value("");
            deptCombo.text("");
            //deptCombo.select(0);
            deptCombo.setDataSource([]);
            secCombo.value("");
            secCombo.text("");
            secCombo.setDataSource([]);
            secCombo.select(0);
            wingCombo.value("");
            wingCombo.text("");
            wingCombo.setDataSource([]);
            wingCombo.select(0);
            teamCombo.value("");
            teamCombo.text("");
            teamCombo.setDataSource([]);
            teamCombo.select(0);
        } else {
            unitcombo.value("");
            unitcombo.text("");
            unitcombo.select(0);
            unitcombo.setDataSource([]);
            deptCombo.value("");
            deptCombo.text("");
            deptCombo.setDataSource([]);
            deptCombo.select(0);
            secCombo.value("");
            secCombo.text("");
            secCombo.setDataSource([]);
            secCombo.select(0);
            wingCombo.value("");
            wingCombo.text("");
            wingCombo.setDataSource([]);
            wingCombo.select(0);
            teamCombo.value("");
            teamCombo.text("");
            teamCombo.setDataSource([]);
            teamCombo.select(0);

        }
    },
    UnitChangeNoPermission: function () {
        var unitId = $("#ddlUnit").data("kendoComboBox").value();
        var deptCombo = $("#ddlDepartment").data("kendoComboBox");
        var secCombo = $("#ddlSection").data("kendoComboBox");
        var wingCombo = $("#ddlWing").data("kendoComboBox");
        var teamCombo = $("#ddlTeam").data("kendoComboBox");

        if (unitId !== "0" && unitId !== "") {
            var deptList = HrmCommonManager.GetAllDepartmentNoPermission(unitId);
            var obj = new Object();
            obj.DepartmentName = "---Select---";
            obj.DepartmentID = 0;
            deptList.unshift(obj);

            deptCombo.value("");
            deptCombo.text("");
            deptCombo.select(0);
            deptCombo.setDataSource(deptList);

            secCombo.value("");
            secCombo.text("");
            secCombo.setDataSource([]);
            secCombo.select(0);
            wingCombo.value("");
            wingCombo.text("");
            wingCombo.setDataSource([]);
            wingCombo.select(0);
            teamCombo.value("");
            teamCombo.text("");
            teamCombo.setDataSource([]);
            teamCombo.select(0);
        } else {
            deptCombo.value("");
            deptCombo.text("");
            deptCombo.setDataSource([]);
            deptCombo.select(0);
            secCombo.value("");
            secCombo.text("");
            secCombo.setDataSource([]);
            secCombo.select(0);
            wingCombo.value("");
            wingCombo.text("");
            wingCombo.setDataSource([]);
            wingCombo.select(0);
            teamCombo.value("");
            teamCombo.text("");
            teamCombo.setDataSource([]);
            teamCombo.select(0);

        }
    },

    //==================Multi Select combobox===================
    GenerateCompanyMultiCombo: function (identity) {
        var objUnit = HrmCommonManager.GetAllCompany();
        $("#" + identity).kendoMultiSelect({
            placeholder: "---Select---",
            dataTextField: "CompanyName",
            dataValueField: "CompanyId",
            dataSource: objUnit,
            //index: 0,
            filter: "contains",
           // autoClose: false
        });
    },

    GenerateUnitMultiCombo: function (identity) {
       // var objUnit = HrmCommonManager.GetAllUnit();
        $("#" + identity).kendoMultiSelect({
            placeholder: "---Select---",
            dataTextField: "UnitName",
            dataValueField: "UnitId",
            dataSource: [],
            //index: 0,
            filter: "contains",
           // autoClose: false
        });
    },

    GenerateDepartmentMultiCombo: function (identity) {
        $("#" + identity).kendoMultiSelect({
            placeholder: "---Select---",
            dataTextField: "DepartmentName",
            dataValueField: "DepartmentID",
            dataSource: [],
            filter: "contains",
            autoClose: false
        });
    },
    GenerateMultiSectionCombo: function (identity) {
        $("#" + identity).kendoMultiSelect({
            placeholder: "---Select---",
            dataTextField: "SectionName",
            dataValueField: "SectionID",
            dataSource: [],
            index: 0,
            suggest: true,
            filter: "contains",
            autoClose: false
        });
    },

    GenerateMultiWingCombo: function (identity) {
        $("#" + identity).kendoMultiSelect({
            placeholder: "---Select---",
            dataTextField: "WingName",
            dataValueField: "WingID",
            dataSource: [],
            index: 0,
            suggest: true,
            filter: "contains",
            autoClose: false
        });
    },
    GenerateMultiTeamCombo: function (identity) {
        $("#" + identity).kendoMultiSelect({
            placeholder: "---Select---",
            dataTextField: "TeamName",
            dataValueField: "TeamID",
            dataSource: [],
            index: 0,
            suggest: true,
            filter: "contains",
            autoClose: false
        });
    },

    GenerateMultiDesignationCombo: function (identity) {
        var objDes = HrmCommonManager.GetAllDesignation();
        $("#" + identity).kendoMultiSelect({
            placeholder: "---Select---",
            dataTextField: "DesignationName",
            dataValueField: "DesignationID",
            dataSource: objDes,
            index: 0,
            suggest: true,
            filter: "contains",
            autoClose: false
        });
    },

    GenerateMultiJobLocationCombo: function (identity) {
        var objJobLocation = HrmCommonManager.GetAllJobLocation();
        $("#" + identity).kendoMultiSelect({
            placeholder: "Select",
            dataTextField: "JobLocationName",
            dataValueField: "JobLocationId",
            dataSource: objJobLocation,
            index: 0,
            suggest: true,
            filter: "contains",
            autoClose: false
        });
    },

    ChangeEventForLoadMultiCombo: function () {
        $("#cmbMultiCompany").change(function () {
            HrmCommonHelper.MultiCompanyChange();
        });
        $("#cmbMultiUnit").change(function () {
             HrmCommonHelper.MultiUnitChange();
        });
        $("#cmbMultiDepartment").change(function () {
            HrmCommonHelper.MultiDepartmentChange();
        });
        $("#cmbMultiSection").change(function () {
            HrmCommonHelper.MultiSectionChange();
        });
        $("#cmbMultiWing").change(function () {
            HrmCommonHelper.MultiWingChange();
        });

    },

    MultiCompanyChange: function () {
        var compIds = $("#cmbMultiCompany").data("kendoMultiSelect").value();
        var unitCombo = $("#cmbMultiUnit").data("kendoMultiSelect");
        var deptCombo = $("#cmbMultiDepartment").data("kendoMultiSelect");
        var secCombo = $("#cmbMultiSection").data("kendoMultiSelect");
        var wingCombo = $("#cmbMultiWing").data("kendoMultiSelect");
        var teamCombo = $("#cmbMultiTeam").data("kendoMultiSelect");

        if (compIds.length > 0) {
            var objParam = new Object();
            objParam.CompanyIds = compIds;
            var unitList = HrmCommonManager.GetAllMultiUnit(objParam);
            unitCombo.value("");
            unitCombo.setDataSource(unitList);

            deptCombo.value("");
            deptCombo.setDataSource([]);
            secCombo.value("");
            secCombo.setDataSource([]);
            wingCombo.value("");
            wingCombo.setDataSource([]);
            teamCombo.value("");
            teamCombo.setDataSource([]);
        } else {
            unitCombo.value("");
            unitCombo.setDataSource([]);
            deptCombo.value("");
            deptCombo.setDataSource([]);
            secCombo.value("");
            secCombo.setDataSource([]);
            wingCombo.value("");
            wingCombo.setDataSource([]);
            teamCombo.value("");
            teamCombo.setDataSource([]);

        }
    },
    MultiUnitChange: function () {
        var unitIds = $("#cmbMultiUnit").data("kendoMultiSelect").value();
        var deptCombo = $("#cmbMultiDepartment").data("kendoMultiSelect");
        var secCombo = $("#cmbMultiSection").data("kendoMultiSelect");
        var wingCombo = $("#cmbMultiWing").data("kendoMultiSelect");
        var teamCombo = $("#cmbMultiTeam").data("kendoMultiSelect");
     
        if (unitIds.length>0){
            var objParam = new Object();
            objParam.UnitIds = unitIds;
            var deptList = HrmCommonManager.GetAllMultiDepartment(objParam);
            deptCombo.value("");
            deptCombo.setDataSource(deptList);

            secCombo.value("");
            secCombo.setDataSource([]);
            wingCombo.value("");
            wingCombo.setDataSource([]);
            teamCombo.value("");
            teamCombo.setDataSource([]);
        } else {
            
            deptCombo.value("");
            deptCombo.setDataSource([]);
            secCombo.value("");
            secCombo.setDataSource([]);
            wingCombo.value("");
            wingCombo.setDataSource([]);
            teamCombo.value("");
            teamCombo.setDataSource([]);

        }
    },
    MultiDepartmentChange: function () {
        var unitIds = $("#cmbMultiUnit").data("kendoMultiSelect").value();
        var deptIds = $("#cmbMultiDepartment").data("kendoMultiSelect").value();
        var secCombo = $("#cmbMultiSection").data("kendoMultiSelect");
        var wingCombo = $("#cmbMultiWing").data("kendoMultiSelect");
        var teamCombo = $("#cmbMultiTeam").data("kendoMultiSelect");
        if (deptIds.length>0) {
            var objParam = new Object();
            objParam.UnitIds = unitIds;
            objParam.DepartmentIds = deptIds;
           
            var sectionList = HrmCommonManager.GetAllMultiSection(objParam);
        
           // secCombo.value("").text("").select(0);
            secCombo.setDataSource(sectionList);
            wingCombo.value("");
           // wingCombo.text("");
            wingCombo.setDataSource([]);
          //  wingCombo.select(0);
            teamCombo.value("");
           // teamCombo.text("");
            teamCombo.setDataSource([]);
           // teamCombo.select(0);
        } else {
            secCombo.value("");
           // secCombo.text("");
            secCombo.setDataSource([]);
           // secCombo.select(0);
            wingCombo.value("");
          //  wingCombo.text("");
            wingCombo.setDataSource([]);
           // wingCombo.select(0);
            teamCombo.value("");
           // teamCombo.text("");
            teamCombo.setDataSource([]);
          //  teamCombo.select(0);
        }
    },
    MultiSectionChange: function () {
        var unitIds = $("#cmbMultiUnit").data("kendoMultiSelect").value();
        var deptIds = $("#cmbMultiDepartment").data("kendoMultiSelect").value();
        var secIds = $("#cmbMultiSection").data("kendoMultiSelect").value();
        var wingCombo = $("#cmbMultiWing").data("kendoMultiSelect");
        var teamCombo = $("#cmbMultiTeam").data("kendoMultiSelect");
        if (secIds.length > 0) {
            var objParam = new Object();
            objParam.UnitIds = unitIds;
            objParam.DepartmentIds = deptIds;
            objParam.SectionIds = secIds;
            var wingList = HrmCommonManager.GetAllMultiWing(objParam);
          
            wingCombo.value("");
          //  wingCombo.text("");
           // wingCombo.select(0);
            wingCombo.setDataSource(wingList);

            teamCombo.value("");
           // teamCombo.text("");
            teamCombo.setDataSource([]);
            //teamCombo.select(0);
        } else {
            wingCombo.value("");
          //  wingCombo.text("");
            wingCombo.setDataSource([]);
           // wingCombo.select(0);
            teamCombo.value("");
          //  teamCombo.text("");
            teamCombo.setDataSource([]);
           // teamCombo.select(0);
        }
    },
    MultiWingChange: function () {
        var unitIds = $("#cmbMultiUnit").data("kendoMultiSelect").value();
        var deptIds = $("#cmbMultiDepartment").data("kendoMultiSelect").value();
        var secIds = $("#cmbMultiSection").data("kendoMultiSelect").value();
        var wingIds = $("#cmbMultiWing").data("kendoMultiSelect").value();
        var teamCombo = $("#cmbMultiTeam").data("kendoMultiSelect");
        if (wingIds.length > 0) {
            var objParam = new Object();
            objParam.UnitIds = unitIds;
            objParam.DepartmentIds = deptIds;
            objParam.SectionIds = secIds;
            objParam.WingIds = wingIds;
            var teamList = HrmCommonManager.GetAllMultiTeam(objParam);
           
            teamCombo.value("");
           // teamCombo.text("");
           // teamCombo.select(0);
            teamCombo.setDataSource(teamList);

        } else {
            teamCombo.value("");
           // teamCombo.text("");
           // teamCombo.select(0);
            teamCombo.setDataSource([]);
        }
    },

    //By Permission
    ChangeEventForLoadMultiComboWithPermission: function () {
        $("#cmbMultiCompany").change(function () {
            HrmCommonHelper.MultiCompanyChangeWithPermission();
        });
        $("#cmbMultiUnit").change(function () {
            HrmCommonHelper.MultiUnitChangeWithPermission();
        });
    },
    MultiCompanyChangeWithPermission: function () {
        var compIds = $("#cmbMultiCompany").data("kendoMultiSelect").value();
        var unitCombo = $("#cmbMultiUnit").data("kendoMultiSelect");
        var deptCombo = $("#cmbMultiDepartment").data("kendoMultiSelect");
  

        if (compIds.length > 0) {
            var objParam = new Object();
            objParam.CompanyIds = compIds;
            var unitList = HrmCommonManager.GetAllMultiUnit(objParam);
            unitCombo.value("");
            unitCombo.setDataSource(unitList);

            deptCombo.value("");
            deptCombo.setDataSource([]);
         
        } else {
            unitCombo.value("");
            unitCombo.setDataSource([]);
            deptCombo.value("");
            deptCombo.setDataSource([]);

        }
    },
    MultiUnitChangeWithPermission: function () {
        var unitIds = $("#cmbMultiUnit").data("kendoMultiSelect").value();
        var deptCombo = $("#cmbMultiDepartment").data("kendoMultiSelect");

        if (unitIds.length > 0) {
            var objParam = new Object();
            objParam.UnitIds = unitIds;
            var deptList = HrmCommonManager.GetAllMultiDepartment(objParam);
            deptCombo.value("");
            deptCombo.setDataSource(deptList);
          
        } else {
            deptCombo.value("");
            deptCombo.setDataSource([]);
        }
    },


  //--====================End=========================================

    GenerateDesignationCombo: function (identity) {
        //var objDes = [];// HrmCommonManager.GetAllDesignation();
        var objDes = HrmCommonManager.GetAllDesignation();
        var obj = new Object();
        obj.DesignationName = "---Select---";
        obj.DesignationID = 0;
        objDes.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "DesignationName",
            dataValueField: "DesignationID",
            dataSource: objDes,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateDesignationComboAll: function (identity) {
        var objDes = HrmCommonManager.GetAllDesignation();
        var obj = new Object();
        obj.DesignationName = "All";
        obj.DesignationID = 0;
        objDes.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "DesignationName",
            dataValueField: "DesignationID",
            dataSource: objDes,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateDesignationGroupCombo: function (identity) {
        var objDesGroup = HrmCommonManager.GetAllDesignationGroup();
        var obj = new Object();
        obj.DesGroupName = "---Select---";
        obj.DesGroupID = 0;
        objDesGroup.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "DesGroupName",
            dataValueField: "DesGroupID",
            dataSource: objDesGroup,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GeneratePositionCombo: function (identity) {
        var objPos = HrmCommonManager.GetAllPosition();
        var obj = new Object();
        obj.PositionName = "---Select---";
        obj.PositionID = 0;
        objPos.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "PositionName",
            dataValueField: "PositionID",
            dataSource: objPos,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GeneratePositionComboAll: function (identity) {
        var objPos = HrmCommonManager.GetAllPosition();
        var obj = new Object();
        obj.PositionName = "All";
        obj.PositionID = 0;
        objPos.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "PositionName",
            dataValueField: "PositionID",
            dataSource: objPos,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateStatusCombo: function (identity) {
        var objSta = HrmCommonManager.GetAllEmploymentStatus();
        //var obj = new Object();
        //obj.EmpStatusName = "---Select---";
        //obj.EmpStatusID = 0;
        //objSta.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "EmpStatusName",
            dataValueField: "EmpStatusID",
            dataSource: objSta,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateStatusComboALL: function (identity) {
        var objSta = HrmCommonManager.GetAllEmploymentStatus();
        var obj = new Object();
        obj.EmpStatusName = "All";
        obj.EmpStatusID = 0;
        objSta.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "EmpStatusName",
            dataValueField: "EmpStatusID",
            dataSource: objSta,
            index: 1,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateTypeCombo: function (identity) {
        var objTyp = HrmCommonManager.GetAllEmploymentType();
        var obj = new Object();
        obj.EmpTypeName = "---Select---";
        obj.EmpTypeID = 0;
        objTyp.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "EmpTypeName",
            dataValueField: "EmpTypeID",
            dataSource: objTyp,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateTypeComboAll: function (identity) {
        var objTyp = HrmCommonManager.GetAllEmploymentType();
        var obj = new Object();
        obj.EmpTypeName = "All";
        obj.EmpTypeID = 0;
        objTyp.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "EmpTypeName",
            dataValueField: "EmpTypeID",
            dataSource: objTyp,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateEmpTypeMultiSelect: function (identity) {
        var objTyp = HrmCommonManager.GetAllEmploymentType();
        $("#" + identity).kendoMultiSelect({
            placeholder: "Select",
            dataTextField: "EmpTypeName",
            dataValueField: "EmpTypeID",
            dataSource: objTyp,
            index: 0
        });
    },
    GenerateCategoryCombo: function (identity) {
        var objCat = HrmCommonManager.GetAllEmploymentCategory();
        var obj = new Object();
        obj.EmpCategoryName = "---Select---";
        obj.EmpCategoryID = 0;
        objCat.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "EmpCategoryName",
            dataValueField: "EmpCategoryID",
            dataSource: objCat,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateCategoryComboAll: function (identity) {
        var objCat = HrmCommonManager.GetAllEmploymentCategory();
        var obj = new Object();
        obj.EmpCategoryName = "All";
        obj.EmpCategoryID = 0;
        objCat.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "EmpCategoryName",
            dataValueField: "EmpCategoryID",
            dataSource: objCat,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateGenderCombo: function (identity) {
        var objGen = HrmCommonManager.GetAllGender();
        var obj = new Object();
        obj.GenderName = "---Select---";
        obj.GenderID = 0;
        objGen.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "---Select---",
            dataTextField: "GenderName",
            dataValueField: "GenderID",
            dataSource: objGen,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateGenderComboAll: function (identity) {
        var objGen = HrmCommonManager.GetAllGender();
        var obj = new Object();
        obj.GenderName = "All";
        obj.GenderID = 0;
        objGen.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "---Select---",
            dataTextField: "GenderName",
            dataValueField: "GenderID",
            dataSource: objGen,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateReligionCombo: function (identity) {
        var objReli = HrmCommonManager.GetAllReligion();
        var obj = new Object();
        obj.ReligionName = "---Select---";
        obj.ReligionID = 0;
        objReli.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "---Select---",
            dataTextField: "ReligionName",
            dataValueField: "ReligionID",
            dataSource: objReli,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateReligionComboAll: function (identity) {
        var objReli = HrmCommonManager.GetAllReligion();
        var obj = new Object();
        obj.ReligionName = "All";
        obj.ReligionID = 0;
        objReli.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "---Select---",
            dataTextField: "ReligionName",
            dataValueField: "ReligionID",
            dataSource: objReli,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateMaritalStatusCombo: function (identity) {
        var objMari = HrmCommonManager.GetAllMaritalStatus();
        var obj = new Object();
        obj.MaritalStatusName = "---Select---";
        obj.MaritalStatusID = 0;
        objMari.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "MaritalStatusName",
            dataValueField: "MaritalStatusID",
            dataSource: objMari,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateBloodGroupCombo: function (identity) {
        var objBloo = HrmCommonManager.GetAllBloodGroup();
        var obj = new Object();
        obj.BloodGroupName = "---Select---";
        obj.BloodGroupID = 0;
        objBloo.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "BloodGroupName",
            dataValueField: "BloodGroupID",
            dataSource: objBloo,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateBloodGroupComboAll: function (identity) {
        var objBloo = HrmCommonManager.GetAllBloodGroup();
        var obj = new Object();
        obj.BloodGroupName = "All";
        obj.BloodGroupID = 0;
        objBloo.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "BloodGroupName",
            dataValueField: "BloodGroupID",
            dataSource: objBloo,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateCountryCombo: function (identity) {
        var objCou = HrmCommonManager.GetAllCountry();
        var obj = new Object();
        obj.CountryName = "---Select---";
        obj.CountryID = 0;
        objCou.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "CountryName",
            dataValueField: "CountryID",
            dataSource: objCou,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateDivisionCombo: function (identity) {
        var objdiv = HrmCommonManager.GetAllDivision();
        var obj = new Object();
        obj.DivisionName = "---Select---";
        obj.DivisionID = 0;
        objdiv.unshift(obj);

        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "DivisionName",
            dataValueField: "DivisionID",
            dataSource: objdiv,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateDistrictCombo: function (identity, divisionId) {
        var objdist = HrmCommonManager.GetAllDistrict(divisionId);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "DistrictName",
            dataValueField: "DistrictID",
            dataSource: objdist,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateDistrictCombo1: function (identity) {
        debugger;
        var objdist = HrmCommonManager.GetAllDistrictCombo();
        var obj = new Object();
        obj.DistrictName = "---Select---";
        obj.DistrictID = 0;
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "DistrictName",
            dataValueField: "DistrictID",
            dataSource: objdist,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateThanaCombo: function (identity, districtId) {
        var objThana = HrmCommonManager.GetAllThana(districtId);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "ThanaName",
            dataValueField: "ThanaID",
            dataSource: objThana,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateShiftCombo: function (identity) {
        var objShift = HrmCommonManager.GetAllShift();
        var obj = new Object();
        obj.ShiftName = "---Select---";
        obj.ShiftID = 0;
        objShift.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "ShiftName",
            dataValueField: "ShiftID",
            dataSource: objShift,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateShiftTypeCombo: function (identity) {
        var objShift = HrmCommonManager.GetAllShiftType();
        var obj = new Object();
        obj.ShiftTypeName = "---Select---";
        obj.ShiftTypeID = 0;
        objShift.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "ShiftTypeName",
            dataValueField: "ShiftTypeID",
            dataSource: objShift,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateWeekOffCombo: function (identity) {
        var objWeekOff = [{ DayID: 1, DayName: 'SUNDAY' },
                { DayID: 2, DayName: 'MONDAY' },
                { DayID: 3, DayName: 'TUESDAY' },
                { DayID: 4, DayName: 'WEDNESDAY' },
                { DayID: 5, DayName: 'THURSDAY' },
                { DayID: 6, DayName: 'FRIDAY' },
                { DayID: 7, DayName: 'SATURDAY' }];
        var obj = new Object();
        obj.DayName = "---Select---";
        obj.DayID = 0;
        objWeekOff.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "DayName",
            dataValueField: "DayID",
            dataSource: objWeekOff,
            index: 0,
            suggest: true,
            filter: "contains"
        });
    },
    GenerateLeaveTypeCombo: function (identity) {
        var isHalfDay = 0;
        var objLeaveType = HrmCommonManager.GetAllLeaveType(isHalfDay);
        var obj = new Object();
        obj.LeaveName = "---Select---";
        obj.LeaveId = 0;
        objLeaveType.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "LeaveName",
            dataValueField: "LeaveId",
            dataSource: objLeaveType,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateManualLeaveTypeCombo: function (identity) {
        var isHalfDay = 0;
        var objLeaveType = HrmCommonManager.GetAllManualLeaveType(isHalfDay);
        var obj = new Object();
        obj.LeaveName = "---Select---";
        obj.LeaveId = 0;
        objLeaveType.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "LeaveName",
            dataValueField: "LeaveId",
            dataSource: objLeaveType,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateSiteDutyTypeCombo: function (identity) {
        var objTour = HrmCommonManager.GetAllSiteDutyType();
        var obj = new Object();
        obj.TourName = "---Select---";
        obj.TourId = 0;
        objTour.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "TourName",
            dataValueField: "TourId",
            dataSource: objTour,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateLeaveApplyToCombo: function (identity) {
        var objApplyTo = HrmCommonManager.GetAllApplyTo();
        //var obj = new Object();
        //obj.Name = "---Select---";
        //obj.EmpID = 0;
        //objApplyTo.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "Name",
            dataValueField: "EmpID",
            dataSource: objApplyTo,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateActionStatusCombo: function (identity) {
        var objActionStatus = HrmCommonManager.GetAllActionStatus();
        $("#" + identity).kendoDropDownList({
            placeholder: "Select",
            dataTextField: "ActionStatusName",
            dataValueField: "ActionStatusId",
            dataSource: objActionStatus,
            index: 0
        });
    },
    GenerateSeperationTypeCombo: function (identity) {
        var objEmpStatus = HrmCommonManager.GetAllSeperationType();
        var obj = new Object();
        obj.EmpStatusName = "All";
        obj.EmpStatusId = "0";
        objEmpStatus.unshift(obj);
        $("#" + identity).kendoDropDownList({
            placeholder: "Select",
            dataTextField: "EmpStatusName",
            dataValueField: "EmpStatusId",
            dataSource: objEmpStatus,
            index: 0
        });
    },
    GenerateOfficialInOutTypeCombo: function (identity) {
        var objTour = HrmCommonManager.GetAllOfficialInOutType();
        var obj = new Object();
        obj.TypeName = "---Select---";
        obj.TypeId = 0;
        objTour.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "TypeName",
            dataValueField: "TypeId",
            dataSource: objTour,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GeneratePaymentModeCombo: function (identity) {
        var objPayMode = HrmCommonManager.GetAllPaymentMode();
        var obj = new Object();
        obj.PaymentModeName = "---Select---";
        obj.PaymentModeId = 0;
        objPayMode.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "PaymentModeName",
            dataValueField: "PaymentModeId",
            dataSource: objPayMode,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GeneratePaymentModeComboAll: function (identity) {
        var objPayMode = HrmCommonManager.GetAllPaymentMode();
        var obj = new Object();
        obj.PaymentModeName = "All";
        obj.PaymentModeId = 0;
        objPayMode.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "PaymentModeName",
            dataValueField: "PaymentModeId",
            dataSource: objPayMode,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GeneratePaymentModeMultiSelect: function (identity) {
        var objPayMode = HrmCommonManager.GetAllPaymentMode();
        $("#" + identity).kendoMultiSelect({
            placeholder: "Select",
            dataTextField: "PaymentModeName",
            dataValueField: "PaymentModeId",
            dataSource: objPayMode,
            index: 0
        });
    },
    GeneratePaymentCurrencyCombo: function (identity) {
        var objCur = HrmCommonManager.GetAllCurrency();
        var obj = new Object();
        obj.CurrencyName = "---Select---";
        obj.CurrencyId = 0;
        objCur.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "CurrencyName",
            dataValueField: "CurrencyId",
            dataSource: objCur,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateSalaryPayCompanyCombo: function (identity) {
        var objComp = HrmCommonManager.GetAllSalaryPayCompany();
        var obj = new Object();
        obj.ShortName = "---Select---";
        obj.CompanyId = 0;
        objComp.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "ShortName",
            dataValueField: "CompanyId",
            dataSource: objComp,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateBankCombo: function (identity) {
        var objBank = HrmCommonManager.GetAllBank();
        var obj = new Object();
        obj.BankName = "---Select---";
        obj.BankId = 0;
        objBank.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "BankName",
            dataValueField: "BankId",
            dataSource: objBank,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateSalaryCalculationFormulaCombo: function (identity) {
        var objFormula = HrmCommonManager.GetAllFormula();
        var obj = new Object();
        obj.FormulaName = "---Select---";
        obj.FormulaId = 0;
        objFormula.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "FormulaName",
            dataValueField: "FormulaId",
            dataSource: objFormula,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateBranchComboByBankId: function (identity) {
        var objBranch = [];
        var obj = new Object();
        obj.BranchName = "---Select---";
        obj.BranchId = 0;
        objBranch.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "BranchName",
            dataValueField: "BranchId",
            dataSource: objBranch,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateCostHeadCombo: function (identity) {
        var objCostHead = HrmCommonManager.GetAllCostHead();
        var obj = new Object();
        obj.CostHeadName = "---Select---";
        obj.CostHeadId = 0;
        objCostHead.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "CostHeadName",
            dataValueField: "CostHeadId",
            dataSource: objCostHead,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateIncrementTypeCombo: function (identity) {
        var objCostHead = HrmCommonManager.GetAllIncrementType();
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "TypeName",
            dataValueField: "TypeId",
            dataSource: objCostHead,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateIncrementTypeComboAll: function (identity) {
        var objInc = HrmCommonManager.GetAllIncrementType();
        var obj = new Object();
        obj.TypeName = "All";
        obj.TypeId = 0;
        objInc.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "TypeName",
            dataValueField: "TypeId",
            dataSource: objInc,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateAttendanceStatusCombo: function (identity) {
        var objEmpStatus = [
            { StatusId: "P", StatusName: "Present" },
            { StatusId: "A", StatusName: "Absent" },
            { StatusId: "LT", StatusName: "Late" },
            { StatusId: "EXL", StatusName: "Excessive Late" },
            { StatusId: "EE", StatusName: "Early Exit" },
            { StatusId: "NOP", StatusName: "No Out Punch" },
            { StatusId: "H", StatusName: "Holiday" },
            { StatusId: "WO", StatusName: "Weekoff" }];
        //var obj = new Object();
        //obj.StatusName = "All";
        //obj.StatusId = "0";
        //objEmpStatus.unshift(obj);
        $("#" + identity).kendoMultiSelect({
            placeholder: "Select",
            dataTextField: "StatusName",
            dataValueField: "StatusId",
            dataSource: objEmpStatus,
            index: 0
        });
    },
    GenerateDegreeCombo: function (identity) {
        var objDegree = HrmCommonManager.GetAllDegree();
        var obj = new Object();
        obj.DegreeName = "---Select---";
        obj.DegreeID = 0;
        objDegree.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "DegreeName",
            dataValueField: "DegreeID",
            dataSource: objDegree,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateBoardCombo: function (identity) {
        var objBoard = HrmCommonManager.GetAllBoard();
        var obj = new Object();
        obj.BoardName = "---Select---";
        obj.BoardID = 0;
        objBoard.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "BoardName",
            dataValueField: "BoardID",
            dataSource: objBoard,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GeneratePayrollTypeCombo: function(identity) {
        var objPayrollType = HrmCommonManager.GetAllPayrollType();
        var obj = new Object();
        obj.TypeName = "---Select---";
        obj.TypeId = 0;
        objPayrollType.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "TypeName",
            dataValueField: "TypeId",
            dataSource: objPayrollType,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function() {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateJobLocationCombo: function (identity) {
        var objJobLocation = HrmCommonManager.GetAllJobLocation();
        var obj = new Object();
        obj.JobLocationName = "---Select---";
        obj.JobLocationId = 0;
        objJobLocation.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "JobLocationName",
            dataValueField: "JobLocationId",
            dataSource: objJobLocation,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateJobLocationComboAll: function (identity) {
        var objJobLocation = HrmCommonManager.GetAllJobLocation();
        var obj = new Object();
        obj.JobLocationName = "All";
        obj.JobLocationId = 0;
        objJobLocation.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "JobLocationName",
            dataValueField: "JobLocationId",
            dataSource: objJobLocation,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateDesigSpecCombo: function (identity) {
        var objDes = HrmCommonManager.GetAllDesigSpec();
        var obj = new Object();
        obj.DesignationSpecification = "---Select---";
        obj.DesignationSpecId = 0;
        objDes.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "DesignationSpecification",
            dataValueField: "DesignationSpecId",
            dataSource: objDes,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },

    GenerateLoanTypeCombo: function (identity) {
        var objDes = [{ LoanTypeId: 1, LoanTypeName :"Home Loan"}];
        var obj = new Object();
        obj.LoanTypeName = "---Select---";
        obj.LoanTypeId = 0;
        objDes.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "LoanTypeName",
            dataValueField: "LoanTypeId",
            dataSource: objDes,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },
    GenerateReportCombo: function (identity,menuId) {
        var objRpt = HrmCommonManager.GetReportComboData(menuId);
        var obj = new Object();
        obj.ReportName = "---Select---";
        obj.ReportId = 0;
        objRpt.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "ReportName",
            dataValueField: "ReportId",
            dataSource: objRpt,
            index: 0,
            suggest: true,
            filter: "contains",
            height: 500,
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },

    GenerateIOURequestToCombo: function (identity) {
        var objApplyTo = HrmCommonManager.GetAllIOUReqestTo();
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "Name",
            dataValueField: "EmpID",
            dataSource: objApplyTo,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    },

    GenerateIouReqActionStatusCombo: function (identity) {
        var objActionStatus = HrmCommonManager.GetAllIouActionStatus();
        $("#" + identity).kendoDropDownList({
            placeholder: "Select",
            dataTextField: "ActionStatusName",
            dataValueField: "ActionStatusId",
            dataSource: objActionStatus,
            index: 0
        });
    },
    GenerateCriterionCombo: function (identity) {
        var objCriterion = HrmCommonManager.GetAllCriterion();
        var obj = new Object();
        obj.CriterionName = "---Select---";
        obj.CriterionId = 0;
        objCriterion.unshift(obj);
        $("#" + identity).kendoComboBox({
            placeholder: "Select",
            dataTextField: "CriterionName",
            dataValueField: "CriterionId",
            dataSource: objCriterion,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            }
        });
    }
};
