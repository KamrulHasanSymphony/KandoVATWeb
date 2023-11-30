var LandManager = {
    GetCmnDivisions() {
        var list = ApiManager.GetList(_baseUrlLand + "/api/CmnDivision/all");
        return list == null ? [] : list;
    },
    GetDistrictByDivisionId: function (divisionId) {
        var objDistrict = "";
        var jsonParam = "";
        var serviceUrl = _baseUrlLand + "/api/CmnDistrict/district/" + divisionId;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {
            objDistrict = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objDistrict;
    },
    GetUpozilaByDistrictId: function (districtId) {
        var objUpozila = "";
        var jsonParam = "";
        var serviceUrl = _baseUrlLand + "/api/CmnUpozila/upozila/" + districtId;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {
            objUpozila = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objUpozila;
    },
    GetMouzaByUpozilaId: function (upozilaId) {
        var objMouza = "";
        var jsonParam = "";
        var serviceUrl = _baseUrlLand + "/api/CmnMouza/mouza/" + upozilaId;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {
            objMouza = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objMouza;
    },
    GetSubRegOfficeByUpozilaId: function (upozilaId) {
        var objSubRegOfc = "";
        var jsonParam = "";
        var serviceUrl = _baseUrlLand + "/api/CmnSubRegOffice/subRegOfc/" + upozilaId;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {
            objSubRegOfc = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objSubRegOfc;
    }
}

var LandHelper = {
    LoadCmnDivisionCombo(cmbCmnDivision) {
        UtilityHelper.LoadCombo(cmbCmnDivision, "DivisionId", "DivisionName", LandManager.GetCmnDivisions(), "--Select Division--");
    },
    LoadCmnDistrictCombo(cmbCmnDistrict) {
        UtilityHelper.LoadCombo(cmbCmnDistrict, "DistrictId", "DistrictName", [], "--Select District--");
    },
    LoadCmnUpozilaCombo(cmbCmnUpozila) {
        UtilityHelper.LoadCombo(cmbCmnUpozila, "UpozilaId", "UpozilaName", [], "--Select Upazila/Thana--");
    },
    LoadCmnMouzaCombo(cmbCmnMouza) {
        UtilityHelper.LoadCombo(cmbCmnMouza, "MouzaId", "MouzaName", [], "--Select Mouza--");
    },
    LoadSubRegOfficeCombo(cmbSubRegOffice) {
        UtilityHelper.LoadCombo(cmbSubRegOffice, "SubRegOfficeId", "SubRegOfficeName", [], "--Select Sub-Registrar Office--");
    },
    ChangeEventOfDivisionCombo: function () {
        var divisionId = $("#cmbCmnDivision").data("kendoComboBox").value();
        var data = LandManager.GetDistrictByDivisionId(divisionId);
        var districtCombo = $("#cmbCmnDistrict").data("kendoComboBox");
        districtCombo.value("");
        districtCombo.text("");
        districtCombo.setDataSource(data);
    },
    ChangeEventOfDistrictCombo: function () {
        var districtId = $("#cmbCmnDistrict").data("kendoComboBox").value();
        var data = LandManager.GetUpozilaByDistrictId(districtId);
        var upozilaCombo = $("#cmbCmnUpozila").data("kendoComboBox");
        upozilaCombo.value("");
        upozilaCombo.text("");
        upozilaCombo.setDataSource(data);
    },
    ChangeEventOfUpozilaCombo: function () {
        var upozilaId = $("#cmbCmnUpozila").data("kendoComboBox").value();
        var data = LandManager.GetMouzaByUpozilaId(upozilaId);
        var mouzaCombo = $("#cmbCmnMouza").data("kendoComboBox");
        mouzaCombo.value("");
        mouzaCombo.text("");
        mouzaCombo.setDataSource(data);
    },
    ChangeEventOfUpozilaComboForSubRegOffice: function () {
        var upozilaId = $("#cmbCmnUpozila").data("kendoComboBox").value();
        var data = LandManager.GetSubRegOfficeByUpozilaId(upozilaId);
        var subRegOfficeCombo = $("#cmbSubRegOffice").data("kendoComboBox");
        subRegOfficeCombo.value("");
        subRegOfficeCombo.text("");
        subRegOfficeCombo.setDataSource(data);
    }
}