$(document).ready(function () {
    var user = CurrentUser;
    $("#bodyTab").kendoTabStrip({
        animation: {open: {effects: "fadeIn"}},
        select: onSelect
    });
   
    DashboardDeatilHelper.InitDashboardDetails();
    LandMasterReportHelper.InitLandMasterReport();
    InventoryDashboardHelper.InitInventoryDashboard();

    //Tab Click Event
    function onSelect(e) {
        var tabId = $(e.item).attr("id");
        if (tabId === "tabUser") {
           
        }
        else if (tabId === "tabAdmin") {
         
        }
    }

    $('#js-news').ticker();

});