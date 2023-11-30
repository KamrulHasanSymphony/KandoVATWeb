$(document).ready(function () {
    $(".kDatePicker").kendoDatePicker({
        formate: "yyyy-MM-dd",
        value: new Date()
    });

    $(".kTextbox").kendoTextBox({
        
    });
    $(".kTextarea").kendoTextArea({
        rows: 2,
        maxLength: 100,
        resize: "both"
        //placeholder: "Tell us a little bit about yourself..."
    });
    //$('.kCheckBox').kendoCheckBox({
    //    checked: false,
    //    //label: "Rear side airbags"
    //});
    //$(".kDropdown").kendoDropDownList({
    //    filter: "contains"
    //});
});