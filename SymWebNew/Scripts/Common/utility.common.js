var UtilityHelper = {
    IsNullOrEmpty(value) {
        if (value == "" || value == null) return true;
        return false;
    },
    LoadCombo(cboId, valueField, textField, dataList, placeholder) {
        if (cboId.trim() == "" || valueField.trim() == "" || textField.trim() == "") return null;
        dataList = dataList == "" ? [] : dataList;
        var obj = new Object();
        obj[valueField] = AjaxManager.DefaultGuidId();
        obj[textField] = "---Select---";
        dataList.unshift(obj);
        $("#" + cboId).kendoComboBox({
            placeholder: placeholder,
            dataValueField: valueField,
            dataTextField: textField,
            dataSource: dataList,
            //index: 0,
            suggest: true,
            filter: "contains"
            //change: function () {

            //}
        });
    },
    LoadCombo1(cboId, valueField, textField, dataList, placeholder) {
        debugger;
        if (cboId.trim() == "" || valueField.trim() == "" || textField.trim() == "") return null;
        dataList = dataList == "" ? [] : dataList;
        var obj = new Object();
        obj[valueField] = "";
        obj[textField] = "---Select---";
        dataList.unshift(obj);
        $("#" + cboId).kendoComboBox({
            placeholder: placeholder,
            dataValueField: valueField,
            dataTextField: textField,
            dataSource: dataList,
            //index: 0,
            suggest: true,
            filter: "contains"
            //change: function () {

            //}
        });
    },
    LoadMultiSelectCombo(cboId, valueField, textField, dataList, placeholder) {
        if (cboId.trim() == "" || valueField.trim() == "" || textField.trim() == "") return null;
        dataList = dataList == "" ? [] : dataList;
        var obj = new Object();
        obj[valueField] = AjaxManager.DefaultGuidId();
        obj[textField] = "---Select---";
        dataList.unshift(obj);
        $("#" + cboId).kendoMultiSelect({
          
            placeholder: placeholder,
            dataValueField: valueField,
            dataTextField: textField,
            dataSource: dataList,
            //index: 0,
            suggest: true,
            filter: "contains"
            //change: function () {

            //}
        });
    },
    InitDatePicker(identity) {
        $("#" + identity).kendoDatePicker({
            value: new Date(),
            format: "dd-MMM-yyyy"
        });
    },
    InitDatePickerMaxRange(identity) {
        $("#" + identity).kendoDatePicker({
            value: new Date(),
            format: "dd-MMM-yyyy",
            max: new Date()
        });
    },
    GetDatePickerValue(identity) {
        return kendo.toString($("#" + identity).data("kendoDatePicker").value(), "dd-MMM-yyyy");
    },
    ResetMultiColumnComboBox(identity) {
        var cboStyle = $("#" + identity).data("kendoMultiColumnComboBox");
        cboStyle.value("");
        cboStyle.setDataSource([]);
        cboStyle.select(0);
    },
    
    KendoNumericTextBox: function (ctrlId, isDisabled, isFormated) {
        // isDisabled = isDisabled === undefined ? true : false;
        // isFormated = isFormated === undefined ? true : false;
        isFormated ? $("#" + ctrlId).kendoNumericTextBox({ format: "#", min: 0 }) : $("#" + ctrlId).kendoNumericTextBox({ format: "n4",decimals: 4, min: 0 });
        isDisabled ? $("#" + ctrlId).data("kendoNumericTextBox").enable(false) : $("#" + ctrlId).data("kendoNumericTextBox").enable(true);
    },
    CheckNull:function() {
        
    },
    EmptyThenDefaultGuidId(fieldValue) {
        if (fieldValue === "" || typeof fieldValue === "undefined" || fieldValue === "0" || fieldValue == null || fieldValue === undefined) return AjaxManager.DefaultGuidId();
        return fieldValue;
    },
    NumericTextBoxEditor4: function (container, options) {
        $('<input name="' + options.field + '"/>')
            .appendTo(container)
            .kendoNumericTextBox({
                format: "n4",
                decimals: 4,
                min: 0
            });
    },
    HasError(myObj) {
        if (!myObj.Success) {
            AjaxManager.MsgBox('error', 'center', 'Error Message', myObj.Message,
                [{
                    addClass: 'btn btn-primary', text: 'Ok', onClick($noty) {
                        $noty.close();
                    }
                }]);
            return true;
        }
        return false;
    },
    IsNaN: function (val) {
        if (val.toString() === "NaN") {
            return 0;
        } else {
            return val;
        }
    },
    ZeroIfNullorEmpty(fieldValue) {
        if (fieldValue === "" || typeof fieldValue === "undefined" || fieldValue === "0" || fieldValue == null || fieldValue === undefined) return 0;
        return fieldValue;
    },
}

var UtilityUnShift = {
    GetUnshiftForStyleSearch() {
        return {
            StyleDetailId: "",
            StyleNo: "--Select Style--",
            StatusName: "",
            BuyerName: "",
            BrandName: "",
            DepartmentName: "",
            SeasonName: "",
            Year: ""
        };
    },
    GetUnshiftDemandSearch() {
        return {
            DemandId: "",
            DemandNo: "--Select Demand--",
            DemandDate: "",
            DueDate: "",
            StatusName: ""
        };
    },
    GetUnshiftPendingbookingSearch() {
        return {
            BookingId: "",
            BookingNo: "--Select Booking--",
            StyleNo: "",
            ItemName: "",
            SupplierId: "",
            SupplierName: "",
          //  StatusName: ""
        };
    },
    GetUnshiftBBLCComboGridSearch() {
        return {
            Bblcid: "",
            BblctempId: "",
            Bblcno: "--Select BBLc No--",
            SupplierName: "",
            OpenBankName: "",
            AdvisingBankName: "",
            SupplierBankName: "",
            BbCurrancyName: "",
            TradeTermName: "",
            PaymentTermName: "",
            PortOfLoadingName: "",
            PortOfDischargeName: "",
        };
    },
}