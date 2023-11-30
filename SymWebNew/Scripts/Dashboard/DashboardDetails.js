var DashboardDeatilManager = {

};

var spinner = null;

var DashboardDeatilHelper = {
    InitDashboardDetails: function () {
        QuickLinkHelper.InitQuickLink();
        OrderStatusHelper.InitOrderStatus();
        PendingDemandSummaryHelper.InitPendingDemandSummary();
    }
};