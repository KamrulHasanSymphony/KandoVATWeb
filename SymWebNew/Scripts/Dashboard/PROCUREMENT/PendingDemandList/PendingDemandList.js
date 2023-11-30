var wnd, detailsTemplate;

var PendingDemandSummaryManager = {
    gridDataSource: function (userId) {
        var gridDataSource = new kendo.data.DataSource({
            type: "json",
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            allowUnsort: true,
            pageSize: 10,
            transport: {
                read: {
                    url: _baseUrl + '/api/Dashboards/GetProcurementPendingDemandList/' + userId,
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
                    fields: {
                        DemandDate: {
                            type: "date",
                            template: '#= kendo.toString("dd-MMM-yyyy") #',
                            editable: false
                        },
                    }
                },
                data: "Items", total: "TotalCount"
            }
        });
        return gridDataSource;
    },

    ListDemandDataSource: function (demandId) {
        var gridDataSource = new kendo.data.DataSource({
            type: "json",
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            allowUnsort: true,
            pageSize: 10,
            transport: {
                read: {
                    url: _baseUrl + "/api/PurDemands/GetDemandDashboardByDemandId/" + demandId,
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
                    fields: {
                        
                    }
                },
                data: "Items", total: "TotalCount"
            }
        });
        return gridDataSource;
    }
}
var PendingDemandSummaryHelper = {
    InitPendingDemandSummary: function () {
        PendingDemandSummaryHelper.InitModalList();
        PendingDemandSummaryHelper.GeneratePendingDemandGrid();
    },

    GeneratePendingDemandGrid: function () {
        var userId = CurrentUser.USERID;
        $("#grdPendingDemandSummary").kendoGrid({
            dataSource: PendingDemandSummaryManager.gridDataSource(userId),
            pageable: {
                refresh: true,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true
            },
            filterable: true,
            sortable: true,
            toolbar: ["search"],
            search: {
                fields: ["DemandNo"]
            },
            columns: [
                { field: "DemandId", hidden: true, width: 10 },
                { field: "DemandNo", title: "Demand No", sortable: true, width: 20 },
                { field: "DemandDate", title: "Demand Date", sortable: true, width: 20, template: '#=kendo.toString(DemandDate==null?"":DemandDate,"dd-MMM-yyyy")#' },
                { field: "ItemName", title: "Item", sortable: true, width: 30 },
                { field: "BuyerName", title: "Buyer", sortable: true, width: 30 },
                { field: "ConcernProcurementOfficers", title: "Concern Proc. Officers", sortable: true, width: 60 }
            ],
            editable: "popup",
            //editable: false,
            selectable: "row",
            navigatable: true,
            dataBound: function () {
                var thisObj = $(this);
                var dataView = this.dataSource.view();
                for (var i = 0; i < dataView.length; i++) {
                    var uid = dataView[i].uid;
                    if (dataView[i].Color.trim().length > 0) {
                        $("#grdPendingDemandSummary tbody").find("tr[data-uid=" + uid + "]").css("background-color", dataView[i].Color);
                        $("#grdPendingDemandSummary tbody").find("tr[data-uid=" + uid + "]").css("color", "#ffffff");
                    }
                }

            }
        });

        $("#grdPendingDemandSummary").on("dblclick", "tr.k-state-selected", function () {
            var gview = $("#grdPendingDemandSummary").data("kendoGrid");
            if (gview.select().length > 0) {
                var selectedItem = gview.dataItem(gview.select());
                if (selectedItem != null) {
                    PendingDemandSummaryHelper.LoadDemandSelectedItems(selectedItem.DemandId);

                    $("#divDemandModal").data("kendoWindow").open().center();
                }
            }
        });
    },
    InitModalList() {
        $("#divDemandModal").kendoWindow({
            title: "Demand Details",
            resizeable: true,
            scrollable: false,
            width: "80%",
            actions: ["Close"],
            modal: true,
            animation: {
                close: {
                    effects: "fade:out"
                },
            }
        });

        $("#gridPendingDemandHistory").kendoGrid({
            noRecords: true,
            messages: {
                noRecords: "NO DATA FOUND"
            },
            dataSource: [],
            pageable: {
                refresh: true,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true
            },
            filterable: true,
            sortable: true,
            toolbar: ["search"],
            search: {
                fields: ["DemandNo"]
            },
            //filterable: false,
            //sortable: false,
            columns: [
                { field: "StyleNo", title: "Style No", sortable: true },
                { field: "Consumption", title: "Consumption", sortable: true, template: "#= Consumption # #= CmnUoms.UOMName # " },
                { field: "ItemName", title: "Item", sortable: true },
                { field: "ItemDesc", title: "Item Desc", sortable: true },
                //{ field: "CmnUoms.UOMName", title: "UOM", sortable: true },
                { field: "ItemSizeName", title: "Size", sortable: true },
                { field: "ItemColorName", title: "Color", sortable: true },
                { field: "OrderQty", title: "Order Qty", sortable: true },
                { field: "Rate", title: "Rate", sortable: true },
                { field: "DemandQty", title: "Demand Qty", sortable: true },
                { field: "ExcessPercent", title: "Excess %", sortable: true },
                { field: "DemandPercent", title: "Demand %", sortable: true }
            ],
            editable: false,
            selectable: "row",
            navigatable: true,
            dataBound: function (e) {

            },
            dataBinding: function (e) {

            }
        });

        $("#btnDemandModalClose").click(function () {
            $("#divDemandModal").data("kendoWindow").close();
        });
    },
    LoadDemandSelectedItems(list) {
        var lists = $("#gridPendingDemandHistory").data("kendoGrid");
        var data = PendingDemandSummaryManager.ListDemandDataSource(list);
        lists.setDataSource(data);
    },
}