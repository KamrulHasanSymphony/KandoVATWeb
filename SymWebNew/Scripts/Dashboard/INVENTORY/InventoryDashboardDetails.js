var InventoryDashboardManager = {
    PendingConsignmentGridDataSource: function (userId) {
        var gridDataSource = new kendo.data.DataSource({
            type: "json",
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            allowUnsort: true,
            pageSize: 10,
            transport: {
                read: {
                    url: _baseUrl + '/api/Consignment/GetConsignmentGridForReceive/' + $.trim(userId),
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json;",
                    cache: false,
                    async: false
                },
                parameterMap: function (options) {
                    return JSON.stringify(options);
                }
            },
            batch: true,
            schema: {
                model: {
                    id: "ConsignmentId",
                    fields: {
                        ConsignmentDate: { type: "date", editable: false, format: "dd-MMM-yyyy"},
                        DeliveryChallanDate: { type: "date", editable: false, format: "dd-MMM-yyyy"},
                        ComInvoiceDate: { type: "date", editable: false, format: "dd-MMM-yyyy"}
                    }
                },
                data: "Items", total: "TotalCount"
            }
        });
        return gridDataSource;
    },
    PendingRequisitionGridDataSource: function (userId) {
        var gridDataSource = new kendo.data.DataSource({
            type: "json",
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            allowUnsort: true,
            pageSize: 10,
            transport: {
                read: {
                    url: _baseUrl + '/api/InvIssue/GetRequisitionGridForIssue/' + $.trim(userId),
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json;",
                    cache: false,
                    async: false
                },
                parameterMap: function (options) {
                    return JSON.stringify(options);
                }
            },
            batch: true,
            schema: {
                model: {
                    id: "RequisitionId",
                    fields: {
                        RequisitionId: { editable: false },
                        RequisitionNo: { type: "string", validation: { required: true } },
                        RequisitionDate: { type: "date", editable: false }
                    }
                },
                data: "Items", total: "TotalCount"
            }
        });
        return gridDataSource;
    },
    PendingIssueReceiveGridDataSource: function (userId) {
        var gridDataSource = new kendo.data.DataSource({
            type: "json",
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            allowUnsort: true,
            pageSize: 10,
            transport: {
                read: {
                    url: _baseUrl + "/api/StoreIssueReceive/GetIssueGridForIssueReceive/" + $.trim(userId),
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json;",
                    cache: false,
                    async: false
                },
                parameterMap: function (options) {
                    return JSON.stringify(options);
                }
            },
            batch: true,
            schema: {
                model: {
                    id: "IssueId",
                    fields: {
                        IssueDate: {
                            type: "date", editable: false
                        },
                    }
                },
                data: "Items", total: "TotalCount"
            }
        });
        return gridDataSource;
    }
}

var InventoryDashboardHelper = {
    InitInventoryDashboard: function () {
        InventoryDashboardHelper.InitPopup();
        InventoryDashboardHelper.GeneratePendingConsignmentRcvGrid();
        InventoryDashboardHelper.GenerateRequisitionPendingGrid();
        InventoryDashboardHelper.GenerateIssuePendingReceiveGrid();
        ConsignmentRcvDetailsHelper.InitConsignmentRcvDetails();
        //InvIssueDetailsHelper.InitInvIssueDetails();
        //StoreIssueReceiveDetailsHelper.InitStoreIssueReceiveDetails();
    },
    InitPopup: function () {
        $("#popupConsignDetails").kendoWindow({
            title: "Consignment Details",
            resizeable: true,
            width: "90%",
            actions: ["Pin", "Refresh", "Maximize", "Close"],
            modal: true,
            visible: false
        });
        $("#popupIssueDetails").kendoWindow({
            title: "Issue Details",
            resizeable: true,
            width: "90%",
            actions: ["Pin", "Refresh", "Maximize", "Close"],
            modal: true,
            visible: false
        });
        $("#popupIssueRcvDetails").kendoWindow({
            title: "Issue Receive Details",
            resizeable: true,
            width: "90%",
            actions: ["Pin", "Refresh", "Maximize", "Close"],
            modal: true,
            visible: false
        });
    },
    GeneratePendingConsignmentRcvGrid: function () {
        $("#grdPendingConsignment").kendoGrid({
            dataSource: InventoryDashboardManager.PendingConsignmentGridDataSource(CurrentUser.USERID),
            pageable: {
                refresh: true,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true
            },
            filterable: true,
            sortable: true,
            noRecords: true,
            messages: {
                noRecords: "NO DATA FOUND"
            },
            columns: [
                { field: "ConsignmentId", hidden: true },
                { field: "ConsignmentNo", title: "Cons No", sortable: true },
                { field: "ConsignmentDate", title: "Cons Date", sortable: true, template: '#=kendo.toString(kendo.parseDate(ConsignmentDate==null?"":ConsignmentDate),"dd-MMM-yyyy")#' },
                { field: "DeliveryChallanNo", title: "Challan No", sortable: true },
                //{ field: "DeliveryChallanDate", title: "Cln Date", sortable: true, template: '#=kendo.toString(kendo.parseDate(DeliveryChallanDate==null?"":DeliveryChallanDate),"dd-MMM-yyyy")#' },
                { field: "ComInvoiceNo", title: "Invoice No", sortable: true },
                //{ field: "ComInvoiceDate", title: "Inv Date", sortable: true, template: '#=kendo.toString(kendo.parseDate(ComInvoiceDate==null?"":ComInvoiceDate),"dd-MMM-yyyy")#' },
                { field: "SupplierName", title: "Supplier", sortable: true },
                { field: "VehicleNo", title: "Vehicle", sortable: true },
                {
                    command: [{
                        name: "edit", text: "Details", iconClass: "k-icon k-i-eye", className: "k-success", click: InventoryDashboardHelper.ClickEventForDetailsButton
                    }], title: "Action &nbsp;"
                }],
            toolbar: [{ name: "search", text: "Search By Cons No" }],
            search: {
                fields: ["ConsignmentNo"]
            },
            editable: false,
            selectable: "row",
            navigatable: true
        });
    },
    ClickEventForDetailsButton: function (e) {
        e.preventDefault();
        var grid = $("#grdPendingConsignment").data("kendoGrid");
        var tr = $(e.currentTarget).closest("tr");
        var selectedItem = this.dataItem(tr);
        grid.select(tr);
        if (selectedItem != null) {
            $("#divConsignRcvDetails").show();
            $("#popupConsignDetails").data("kendoWindow").open().center();
            ConsignmentRcvDetailsHelper.FillConsignmentRcvMasterForm(selectedItem);
        }
    },
    GenerateRequisitionPendingGrid: function () {
        $("#grdPendingRequisition").kendoGrid({
            dataSource: InventoryDashboardManager.PendingRequisitionGridDataSource(CurrentUser.USERID),
            pageable: {
                refresh: true,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true
            },
            filterable: true,
            sortable: true,
            noRecords: true,
            messages: {
                noRecords: "NO DATA FOUND"
            },
            columns: [
                { field: "RequisitionId", hidden: true },
                { field: "RequisitionNo", title: "Req. No", width: 15, sortable: true },
                { field: "RequisitionDate", title: "Req. Date", width: 15, sortable: true, template: '#= kendo.toString(kendo.parseDate(RequisitionDate==null?"":RequisitionDate), "dd-MMM-yyyy") #' },
                { field: "StoreFromId", hidden: true },
                { field: "StoreFrom", title: "Store From", width: 20, sortable: true },
                { field: "StoreToId", hidden: true },
                //{ field: "StoreTo", title: "Store To", width: 15, sortable: true },
                { field: "StyleId", hidden: true },
                { field: "StyleDetailsId", hidden: true },
                //{ field: "StyleNo", title: "Style", width: 15, sortable: true },
                { field: "Remarks", title: "Remarks", width: 10, sortable: true },
                {
                    command: [{
                        name: "edit", text: "Issue", iconClass: "k-icon k-i-arrow-down", className: "k-success", click: InventoryDashboardHelper.ClickEventForIssued
                    }], width: 10, title: "Action &nbsp;"
                }
            ],
            editable: false,
            selectable: "row",
            navigatable: true,
            scrollable: true,
            toolbar: [{ name: "search", text: "Search By Req No" }],
            search: {
                fields: ["RequisitionNo"]
            }
        });

        //    $("#grdLandSummaryDistrict").on("dblclick", "tr.k-state-selected", function () {
        //        var grid = $("#grdLandSummaryDistrict").data("kendoGrid");
        //        if (grid.select().length > 0) {
        //            var selectedItem = grid.dataItem(grid.select());
        //            if (selectedItem != null) {
        //                InventoryDashboardHelper.LoadUpozilaByDistrictIdSummaryGrid(selectedItem);
        //                $("#popupUpozilaByDistrictIdSummary").data("kendoWindow").open().center();
        //            }
        //        }
        //    });
    },
    ClickEventForIssued: function (e) {
        e.preventDefault();
        var grid = $("#grdPendingRequisition").data("kendoGrid");
        var tr = $(e.currentTarget).closest("tr");
        var selectedItem = this.dataItem(tr);
        grid.select(tr);
        if (selectedItem != null) {
            $("#divIssueDetails").show();
            $("#popupIssueDetails").data("kendoWindow").open().center();
            InvIssueDetailsHelper.FillIssueMasterFormByRequisition(selectedItem);
        }
    },
    GenerateIssuePendingReceiveGrid: function () {
        $("#grdPendingReceive").kendoGrid({
            dataSource: InventoryDashboardManager.PendingIssueReceiveGridDataSource(CurrentUser.USERID),
            pageable: {
                refresh: true,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true
            },
            filterable: true,
            sortable: true,
            noRecords: true,
            messages: {
                noRecords: "NO DATA FOUND"
            },
            columns: [
                { field: "IssueId", hidden: true },
                { field: "IssueNo", title: "Issue No", sortable: true },
                { field: "IssueDate", title: "Date", sortable: true, template: '#= kendo.toString(kendo.parseDate(IssueDate==null?"":IssueDate), "dd-MMM-yyyy") #' },
                { field: "StyleNo", title: "Style", sortable: true },
                { field: "RequisitionNo", title: "Req. No", sortable: true },
                { field: "StoreFromName", title: "From", sortable: true },
                //{ field: "StoreToName", title: "Store To", sortable: true },
                { field: "Remarks", title: "Remarks", sortable: true },
                {
                    command: [{
                        name: "edit", text: "Acknowledge", iconClass: "k-icon k-i-check-circle", className: "k-success", click: InventoryDashboardHelper.ClickEventForReceive
                    }], title: "Action &nbsp;"
                }
            ],
            editable: false,
            selectable: "row",
            navigatable: true,
            scrollable: true,
            toolbar: [{ name: "search", text: "Search By Issue No" }],
            search: {
                fields: ["IssueNo"]
            }
        });
    },
    ClickEventForReceive: function (e) {
        e.preventDefault();
        var grid = $("#grdPendingReceive").data("kendoGrid");
        var tr = $(e.currentTarget).closest("tr");
        var selectedItem = this.dataItem(tr);
        grid.select(tr);
        if (selectedItem != null) {
            $("#divIssueRcvDetails").show();
            $("#popupIssueRcvDetails").data("kendoWindow").open().center();
            StoreIssueReceiveDetailsHelper.FillIssueMasterForm(selectedItem);
        }
    }

    //FillTotalLandAmount: function () {
    //    var totalLandAmountAcres = InventoryDashboardManager.GetTotalLandAmount();
    //    $("#spnTotalLandAmountAcres").html(totalLandAmountAcres);
    //},
    //GenerateLandSummaryDistrictChart: function() {
    //    var chart = AmCharts.makeChart("chartdivLandSummaryDistrict", {
    //        "type": "pie",
    //        "theme": "none",
    //        "dataProvider": InventoryDashboardManager.GetAllLandSummaryDistrictList(),
    //        "valueField": "TotalLandAcres",
    //        "titleField": "DistrictName",
    //        "balloon": {
    //            "fixedPosition": true
    //        },
    //        "export": {
    //            "enabled": true
    //        }
    //    });
    //    return chart;
    //},
}