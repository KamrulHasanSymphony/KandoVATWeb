var MerchantManager = {
    GetBuyers()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/Buyers/all" );
        return list == null ? [] : list;
    },
    GetAllStyleDropdown: function ()
    {
        var objstyle = "";
        var jsonParam = "";
        var serviceUrl = _baseUrl + "/api/styles/all";
        AjaxManager.GetJsonResult( serviceUrl, jsonParam, false, false, onSuccess, onFailed );
        function onSuccess( jsonData )
        {
            objstyle = jsonData;
        }
        function onFailed( error )
        {
            window.alert( error.statusText );
        }
        return objstyle;
    },
    GetTenures()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/CmnTenures/all" );
        return list == null ? [] : list;
    },
    GetPaymentTerm()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/CmnPaymentTerms/all" );
        return list == null ? [] : list;
    },
    GetPort()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/PortOfLoading/all" );
        return list == null ? [] : list;
    },
    GetSaleContract()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/ComSalesContract/all" );
        return list == null ? [] : list;
    },
    GetConsignee()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/ComConsignee/all" );
        return list == null ? [] : list;
    },
    GetShipmentTerm()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/CmnShipmentTerm/all" );
        return list == null ? [] : list;
    },
    GetBank()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/CmnBank/all" );
        return list == null ? [] : list;
    },
    GetStatuses( flagNo )
    {
        var list = ApiManager.GetList( _baseUrl + "/api/CmnStatuses/GetCmnStatus/" + flagNo );
        return list == null ? [] : list;
    },
    GetProductCategories()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/MerchProductCategory/all" );
        return list == null ? [] : list;
    },
    GetProducts()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/MerchProducts/all" );
        return list == null ? [] : list;
    },
    GetProductTypes()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/MerchProductType/all" );
        return list == null ? [] : list;
    },
    GetAgents()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/Agent/all" );
        return list == null ? [] : list;
    },
    GetSeasons()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/StyleSeason/all" );
        return list == null ? [] : list;
    },
    GetVendors()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/CommonCompanies/all" );
        return list == null ? [] : list;
    },
    GetCompanies()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/CommonCompanies/all" );
        return list == null ? [] : list;
    },
    GetWashTypes()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/WashTypes/all" );
        return list == null ? [] : list;
    },
    GetUoM()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/CmnUoms/all" );
        return list == null ? [] : list;
    },
    GetFabricCompositions()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/MerchFabricComposition/all" );
        return list == null ? [] : list;
    },
    GetStyleDetailById( styleDetailId )
    {
        var obj = null;
        if ( $.trim( styleDetailId ).length > 0 ) obj = ApiManager.GetList( _baseUrl + "/api/Styles/GetStyleDetailById/" + styleDetailId );
        return obj;
    },
    GetStyleById( styleId )
    {
        var obj = null;
        if ( $.trim( styleId ).length > 0 ) obj = ApiManager.GetList( _baseUrl + "/api/Styles/" + styleId );
        return obj;
    },
    GetSuppliers()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/Suppliers/all" );
        return list == null ? [] : list;
    },


    //GetPaymentTerm() {
    //    var list = ApiManager.GetList(_baseUrl + "/api/CmnPaymentTerms/all");
    //    return list == null ? [] : list;
    //},
    //GetPortOfLoading() {
    //    var list = ApiManager.GetList(_baseUrl + "/api/PortOfLoading/all");
    //    return list == null ? [] : list;
    //},
    GetAllStyle: function ( searchKey )
    {
        var objStyle = "";
        var jsonParam = "";
        var serviceUrl = _baseUrl + "/api/Styles/GetAllStyleDropdown/" + $.trim( searchKey );
        AjaxManager.GetJsonResult( serviceUrl, jsonParam, false, false, onSuccess, onFailed );
        function onSuccess( jsonData )
        {
            objStyle = jsonData;
        }
        function onFailed( error )
        {
            window.alert( error.statusText );
        }
        return objStyle;
    },
    GetAllSalescontact: function ( searchKey )
    {
        var objContact = "";
        var jsonParam = "";
        var serviceUrl = _baseUrl + "/api/ComSalesContract/GetAllContactDropdown/" + $.trim( searchKey );
        AjaxManager.GetJsonResult( serviceUrl, jsonParam, false, false, onSuccess, onFailed );
        function onSuccess( jsonData )
        {
            objStyle = jsonData;
        }
        function onFailed( error )
        {
            window.alert( error.statusText );
        }
        return objContact;
    },
    GetAllItem: function ( searchKey )
    {
        var objItem = "";
        var jsonParam = "";
        var serviceUrl = _baseUrl + "/api/MerchItems/GetItemDropdown/" + searchKey;
        AjaxManager.GetJsonResult( serviceUrl, jsonParam, false, false, onSuccess, onFailed );
        function onSuccess( jsonData )
        {
            objItem = jsonData;
        }
        function onFailed( error )
        {
            window.alert( error.statusText );
        }
        return objItem;
    },
    GetAllStyleWithStatus: function ( searchKey )
    {
        var objStyle = "";
        var jsonParam = "";
        var serviceUrl = _baseUrl + "/api/Styles/GetParentStyleByTextDropdown/" + $.trim( searchKey );
        AjaxManager.GetJsonResult( serviceUrl, jsonParam, false, false, onSuccess, onFailed );
        function onSuccess( jsonData )
        {
            objStyle = jsonData;
        }
        function onFailed( error )
        {
            window.alert( error.statusText );
        }
        return objStyle;
    },
    GetAllColor: function ()
    {
        var objColor = "";
        var jsonParam = "";
        var serviceUrl = _baseUrl + "/api/CmnColors/all";
        AjaxManager.GetJsonResult( serviceUrl, jsonParam, false, false, onSuccess, onFailed );
        function onSuccess( jsonData )
        {
            objColor = jsonData;
        }
        function onFailed( error )
        {
            window.alert( error.statusText );
        }
        return objColor;
    },
    GetAllColor1: function () {
        var list = ApiManager.GetList(_baseUrl + "/api/CmnColors/all");
        return list == null ? [] : list;

        //var objColor = "";
        //var jsonParam = "";
        //var serviceUrl = _baseUrl + "/api/CmnColors/all";
        //AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);
        //function onSuccess(jsonData) {
        //    objColor = jsonData;
        //}
        //function onFailed(error) {
        //    window.alert(error.statusText);
        //}
        //return objColor;
    },
    GetAllSize: function ()
    {
        var objSize = "";
        var jsonParam = "";
        var serviceUrl = _baseUrl + "/api/CmnSizes/all";
        AjaxManager.GetJsonResult( serviceUrl, jsonParam, false, false, onSuccess, onFailed );
        function onSuccess( jsonData )
        {
            objSize = jsonData;
        }
        function onFailed( error )
        {
            window.alert( error.statusText );
        }
        return objSize;
    },
    GetAllDemand: function ( statusId, searchKey )
    {
        if ( $.trim( statusId ) == "" ) statusId = AjaxManager.DefaultGuidId();
        var objStyle = "";
        var jsonParam = "";
        var serviceUrl = _baseUrl + "/api/PurDemands/SelectDemandNoOrStatus/" + statusId + "/" + $.trim( searchKey );
        AjaxManager.GetJsonResult( serviceUrl, jsonParam, false, false, onSuccess, onFailed );
        function onSuccess( jsonData )
        {
            objStyle = jsonData;
        }
        function onFailed( error )
        {
            window.alert( error.statusText );
        }
        return objStyle;
    },
    GetPoListByStyleDetailIds: function ( styleDetailIds )
    {
        var poHistoryData = [];
        var jsonParam = JSON.stringify( styleDetailIds );
        var serviceUrl = _baseUrl + "/api/PurchaseOrders/GetNoOfPOCreatedByStyleDetailIdList";
        AjaxManager.PostJsonApi( serviceUrl, jsonParam, onSuccess, onFailed );
        function onSuccess( jsonData )
        {
            poHistoryData = jsonData;
        }
        function onFailed( error )
        {
            AjaxManager.MsgBox( 'error', 'center', 'Error', error.statusText,
                [{
                    addClass: 'btn btn-primary', text: 'Ok', onClick: function ( $noty )
                    {
                        $noty.close();
                    }
                }] );
        }

        return poHistoryData;
    },
    LoadExccessPercentData: function ()
    {
        var objExcess = [];
        for ( var i = 0; i <= 10; i++ )
        {
            var obj1 = new Object();
            obj1.ExcessPercent = i;
            obj1.ExcessPercent1 = i + "%";
            objExcess.push( obj1 );
        }

        return objExcess;
    },
    GetAllLeadTime: function ( buyerId, brandId, deptId )
    {
        var objLeadTime = "";
        var jsonParam = "";
        var serviceUrl = _baseUrl + "/api/LeadTimes/GetLeadTimeByBuyerBrandDepartment/" + buyerId + "/" + brandId + "/" + deptId;
        AjaxManager.GetJsonResult( serviceUrl, jsonParam, false, false, onSuccess, onFailed );
        function onSuccess( jsonData )
        {
            objLeadTime = jsonData;
        }
        function onFailed( error )
        {
            window.alert( error.statusText );
        }
        return objLeadTime;
    },
    GetAllCurrency: function ()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/CmnCurrency/all" );
        return list == null ? [] : list;
    },
    GetAllMaterialSource()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/MaterialSource/all" );
        return list == null ? [] : list;
    },
    GetAllCountry: function ()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/Countries/all" );
        return list == null ? [] : list;
    },
    GetAllMerchItem: function ()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/MerchItems/all" );
        return list == null ? [] : list;
    },
    GetAllItemCategory: function ()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/MerchItemCategories/all" );
        return list == null ? [] : list;
    },
    GetAllItemGroup: function ()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/MerchItemGroups/all" );
        return list == null ? [] : list;
    },
    GetAllBrand: function ()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/BuyerBrands/all" );
        return list == null ? [] : list;
    },
    GetAllProductCategory: function ()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/MerchProductCategory/all" );
        return list == null ? [] : list;
    },
    GetSupplierByItemId: function ( itemId )
    {
        var list = ApiManager.GetList( _baseUrl + "/api/Suppliers/GetSupplierByItemId/" + itemId );
        return list == null ? [] : list;
    },
    GetFabricTypeCompositions: function ()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/MerchantFabricTypes/all" );
        return list == null ? [] : list;
    },
    GetMaturityType: function ()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/CmnLcMaturityType/all" );
        return list == null ? [] : list;
    },
    GetSC: function ()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/ComSalesContract/all" );
        return list == null ? [] : list;
    },

    GetScType: function ()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/ComScType/all" );
        return list == null ? [] : list;
    },
    GetScFor: function ()
    {
        var list = ApiManager.GetList( _baseUrl + "/api/ComScfor/all" );
        return list == null ? [] : list;
    },
    GetAllShipmentMode: function () {
        var objShipmentMode = "";
        var jsonParam = "";
        var serviceUrl = _baseUrl + "/api/CmnShipmentMode/all";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {
            objShipmentMode = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objShipmentMode;
    },
    GetAllExportLc: function () {
        var list = ApiManager.GetList(_baseUrl + "/api/ComExportLc/GetAllExportLc");
        return list == null ? [] : list;
    },
    GetAllDocStatus: function () {
        var list = ApiManager.GetList(_baseUrl + "/api/DocStatus/all");
        return list == null ? [] : list;
    },
    GetEmployee() {
        var list = ApiManager.GetList(_baseUrl + "/api/MerchEmpOrganograms/GetAllEmpFromOrganogram");
        return list == null ? [] : list;
    },
    GetTransactionType() {
        var list = ApiManager.GetList(_baseUrl + "/api/TransactionType/all");
        return list == null ? [] : list;
    },
    GetConfirmPO() {
        var list = ApiManager.GetList(_baseUrl + "/api/PurchaseOrders/GetConfirmPO");
        return list == null ? [] : list;
    },

    /*********** Inventory Common **************/

    GetAllCmnStoreDropdown: function () {
        var objStore = "";
        var jsonParam = "";
        var serviceUrl = _baseUrl + "/api/CmnStore/GetAllCmnStores";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {
            objStore = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objStore;
    },
    GetPackageType() {
        var list = ApiManager.GetList(_baseUrl + "/api/PackageType/all");
        return list == null ? [] : list;
    },
    GetAllBin() {
        var list = ApiManager.GetList(_baseUrl + "/api/InvBin/all");
        return list == null ? [] : list;
    },
    GetAllShelves() {
        var list = ApiManager.GetList(_baseUrl + "/api/InvShelves/all");
        return list == null ? [] : list;
    },
    GetAllShade() {
        var list = ApiManager.GetList(_baseUrl + "/api/InvShade/all");
        return list == null ? [] : list;
    },
    GetUserStorePermissionByUserId: function (userId) {
        var list = ApiManager.GetList(_baseUrl + "/api/UserStorePermission/GetUserStorePermissionByUserId/" + userId);
        return list == null ? [] : list;
    },
    GetAllPODropdown: function () {
        var objPO = "";
        var jsonParam = "";
        var serviceUrl = _baseUrl + "/api/PurchaseOrders/all";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {
            objPO = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objPO;
    },
}
var MerchantHelper = {
    //All Load Combos methods Depend on this global ajaxSetup at (ajaxManager.common.js)
    //$.ajaxSetup({
    //    async: false
    //});
    LoadStoreComboByUserId: function (identity, userId) {
        $("#" + identity).kendoComboBox({
            placeholder: "--Select Store--",
            dataValueField: "StoreId",
            dataTextField: "StoreName",
            dataSource: MerchantManager.GetUserStorePermissionByUserId(userId),
            index: 0,
            suggest: true,
            filter: "contains"
        });
    },
    LoadShadeCombo(cboId) {
        UtilityHelper.LoadCombo(cboId, "ShadeId", "ShadeName", MerchantManager.GetAllShade(), "--Select Shade--");
    },
    LoadShelvesCombo(cboId) {
        UtilityHelper.LoadCombo(cboId, "ShelveId", "ShelveNo", MerchantManager.GetAllShelves(), "--Select Shelves--");
    },
    LoadBinCombo(cboId) {
        UtilityHelper.LoadCombo(cboId, "BinId", "BinNumber", MerchantManager.GetAllBin(), "--Select Bin--");
    },
    LoadPackageTypeCombo(cboId) {
        UtilityHelper.LoadCombo(cboId, "PackTypeId", "PackTypeName", MerchantManager.GetPackageType(), "--Select Package Type--");
    },
    LoadStoreCombo(cboId) {
        UtilityHelper.LoadCombo(cboId, "StoreId", "StoreName", MerchantManager.GetAllCmnStoreDropdown(), "--Select Store--");
    },
    LoadTransactionTypeCombo(cboId) {
        UtilityHelper.LoadCombo(cboId, "TransTypeId", "TransTypeName", MerchantManager.GetTransactionType(), "--Select Transaction Type--");
    },
    LoadPOConfirmCombo(cboId) {
        UtilityHelper.LoadCombo(cboId, "Poid", "Pono", MerchantManager.GetConfirmPO(), "--Select PO--");
    },
    LoadEmployeeCombo(cboId) {
        UtilityHelper.LoadCombo(cboId, "MerchEmpOrganogramId", "Name", MerchantManager.GetEmployee(), "--Select Employee--");
    },
    LoadDocStatusCombo(cboId) {
        UtilityHelper.LoadCombo(cboId, "DocStatusId", "DocStatusName", MerchantManager.GetAllDocStatus(), "--Select DocStatus--");
    },
    LoadExportLcCombo(cboId) {
        UtilityHelper.LoadCombo(cboId, "Lcid", "Lcno", MerchantManager.GetAllExportLc(), "--Select ExportLc--");
    },
    LoadBuyerCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "BuyerId", "BuyerName", MerchantManager.GetBuyers(), "--Select Buyer--" );
    },
    LoadTenuresCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "TenureId", "TenureName", MerchantManager.GetTenures(), "--Select Tenure--" );
    },
    LoadConsigneeCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "ConsigneeId", "ConsigneeName", MerchantManager.GetConsignee(), "--Select Consignee--" );
    },
    LoadPortCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "Polid", "Polname", MerchantManager.GetPort(), "--Select Port--" );
    },
    LoadPaymentTermCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "PaymentTermsId", "PaymentTermsName", MerchantManager.GetPaymentTerm(), "--Select Payment Term--" );
    },
    LoadSaleContractCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "Scid", "Scno", MerchantManager.GetSaleContract(), "--Select Contract--" );
    },
    LoadLCMultiSelectCombo(cboId) {
        UtilityHelper.LoadMultiSelectCombo(cboId, "Scid", "Scno", MerchantManager.GetSaleContract(), "--Select Contract--");
    },
    LoadShipmentTermsCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "ShipmentTermsId", "ShipmentTermsName", MerchantManager.GetShipmentTerm(), "--Select Shipment Term--" );
    },
    LoadShipmentModeCombo(cboId) {
        UtilityHelper.LoadCombo(cboId, "CmnShipmentModeId", "ModeName", MerchantManager.GetAllShipmentMode(), "--Select Shipment Mode--");
    },
    LoadBankCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "BankId", "BankName", MerchantManager.GetBank(), "--Select Bank--" );
    },
    LoadStatusCombo( cboId, flagNo )
    {
        UtilityHelper.LoadCombo( cboId, "StatusId", "StatusName", MerchantManager.GetStatuses( flagNo ), "--Select Style Status--" );
    },
    LoadProductCategoryCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "ProductCategoryId", "ProductCategoryName", MerchantManager.GetProductCategories(), "--Select Product Category--" );
    },
    LoadProductCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "ProductId", "ProductName", MerchantManager.GetProducts(), "--Select Product--" );
    },
    LoadProductTypeCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "ProductTypeId", "ProductTypeName", MerchantManager.GetProductTypes(), "--Select Product Type--" );
    },
    LoadAgentCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "AgentId", "AgentName", MerchantManager.GetAgents(), "--Select Agent--" );
    },
    LoadSeasonCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "SeasonId", "SeasonName", MerchantManager.GetSeasons(), "--Select Season--" );
    },
    LoadYearCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "YearNo", "YearNo", MerchantHelper.LoadYears(), "--Select Year--" );
    },
    LoadVendorCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "CompanyId", "CompanyName", MerchantManager.GetVendors(), "--Select Vendor--" );
    },
    LoadCompanyCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "CompanyId", "CompanyName", MerchantManager.GetCompanies(), "--Select Production Factory--" );
    },
    LoadWashTypeCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "WashTypeId", "WashTypeName", MerchantManager.GetWashTypes(), "--Select Wash Type--" );
    },
    LoadUoMCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "UOMId", "UOMName", MerchantManager.GetUoM(), "--Select Unit--" );
    },
    LoadFabricCompositionCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "FabricCompositionId", "CompositionName", MerchantManager.GetFabricCompositions(), "--Select Composition--" );
    },
    LoadSupplierCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "SupplierId", "SupplierName", MerchantManager.GetSuppliers(), "--Select Supplier--" );
    },
    LoadScTypeCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "SctypeId", "SctypeName", MerchantManager.GetScType(), "--Select SCtype--" );
    },
    LoadScForCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "ScforId", "ScforName", MerchantManager.GetScFor(), "--Select SCfor--" );
    },
    LoadCountryCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "CountryId", "CountryName", MerchantManager.GetAllCountry(), "--Select Country--" );
    },
    LoadCountryShortNameCombo(cboId)
    {
        UtilityHelper.LoadCombo(cboId, "CountryId", "CountryShort", MerchantManager.GetAllCountry(), "--Select Country--" );
    },
    LoadMerchItemCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "ItemId", "ItemName", MerchantManager.GetAllMerchItem(), "--Select Item--" );
    },
    LoadItemCategoryCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "ItemCategoryId", "CategoryName", MerchantManager.GetAllItemCategory(), "--Select Category--" );
    },
    LoadItemGroupCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "ItemGroupId", "GroupName", MerchantManager.GetAllItemGroup(), "--Select Group--" );
    },
    LoadBrandCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "BrandId", "BuyerBrandName", MerchantManager.GetAllBrand(), "--Select Brand--" );
    },
    LoadFabricTypeCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "FabricTypeId", "FabricType", MerchantManager.GetFabricTypeCompositions(), "--Select Fabric Type--" );
    },
    LoadMaturityTypeCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "LcmaturityId", "LcmaturityName", MerchantManager.GetMaturityType(), "--Select Maturity Type--" );
    },
    LoadSCCombo( cboId )
    {
        UtilityHelper.LoadCombo( cboId, "Scid", "Scno", MerchantManager.GetSC(), "--Select SC/LC--" );
    },
    LoadYears()
    {
        var years = [],
            year = ( new Date() ).getFullYear(),
            takeYear = 5,
            startFrom = 1;
        while ( startFrom <= takeYear )
        {
            years.push( {
                YearNo: year--
            } );
            startFrom++;
        }
        return years;
    },

    GenerateStyleMultiColumnComboBox: function ( ctrlId )
    {
        $( "#" + ctrlId ).kendoMultiColumnComboBox( {
            dataSource: [],
            dataTextField: "StyleNo",
            dataValueField: "StyleDetailId",
            columns: [
                { field: "StyleNo", title: "Style No", width: 120 },
                { field: "BuyerName", title: "Buyer", width: 120 },
                { field: "BrandName", title: "Brand", width: 120 },
                { field: "DepartmentName", title: "Department", width: 120 },
                { field: "SeasonName", title: "Season", width: 100 },
                { field: "Year", title: "Year", width: 80 },
                { field: "StatusName", title: "Status", width: 100 }
            ],
            filter: "startswith",
            filterFields: ["StyleNo"],
            footerTemplate: 'Total #: instance.dataSource.total() # items found',
            index: 0,
            placeholder: "---Select Style---",
            height: 400

        } );
    },
    
    GenerateItemMultiColumnComboBox: function ( ctrlId )
    {
        $( "#" + ctrlId ).kendoMultiColumnComboBox( {
            dataSource: [],
            dataTextField: "ItemName",
            dataValueField: "ItemId",
            columns: [
                { field: "ItemName", title: "Item Name", width: 200 },
                { field: "ItemCategoryName", title: "Item Category", width: 200 },
                { field: "ItemGroupName", title: "Item Group", width: 200 }

            ],
            filter: "startswith",
            filterFields: ["ItemName", "ItemCategoryName", "ItemName"],
            footerTemplate: 'Total #: instance.dataSource.total() # items found',
            index: 1,
            height: 400,
            placeholder: "---Select Item---"
        } );
    },
    GenerateDemandMultiColumnComboBox: function ( ctrlId )
    {
        $( "#" + ctrlId ).kendoMultiColumnComboBox( {
            dataSource: [],
            dataTextField: "DemandNo",
            dataValueField: "DemandId",
            columns: [
                { field: "DemandNo", title: "Demand No", width: 100 },
                { field: "DemandDate", title: "Demand Date", width: 100, template: '#=kendo.toString(DemandDate==null?"":DemandDate,"dd-MMM-yyyy")#' },
                { field: "DueDate", title: "Due Date", width: 100, template: '#=kendo.toString(DueDate==null?"":DueDate,"dd-MMM-yyyy")#' },
                { field: "StatusName", title: "StatusName", width: 150 },
            ],
            filter: "startswith",
            filterFields: ["DemandNo"],
            footerTemplate: 'Total #: instance.dataSource.total() # items found',
            index: 0,
            placeholder: "---Select Demand---",
            height: 400
        } );
    },

    GeneratePOCombo: function ( identity )
    {
        $( "#" + identity ).kendoComboBox( {
            placeholder: "--Select PO--",
            dataValueField: "POId",
            dataTextField: "PONo",
            dataSource: [],
            index: 0,
            suggest: true,
            filter: "contains"
        } );
    },
    GenerateColorCombo: function ( identity )
    {
        var objColor = MerchantManager.GetAllColor();
        var obj = new Object();
        obj.ColorName = "---Select---";
        obj.ColorId = AjaxManager.DefaultGuidId();
        objColor.unshift( obj );
        $( "#" + identity ).kendoComboBox( {
            placeholder: "--Select--",
            dataValueField: "ColorId",
            dataTextField: "ColorName",
            dataSource: objColor,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function ()
            {
                AjaxManager.isValidItem( identity, true );
            }
        } );
    },
    GenerateSizeCombo: function ( identity )
    {
        var objSize = MerchantManager.GetAllSize();
        var obj = new Object();
        obj.SizeName = "---Select---";
        obj.SizeId = AjaxManager.DefaultGuidId();
        objSize.unshift( obj );
        $( "#" + identity ).kendoComboBox( {
            placeholder: "--Select--",
            dataValueField: "SizeId",
            dataTextField: "SizeName",
            dataSource: objSize,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function ()
            {
                AjaxManager.isValidItem( identity, true );
            }
        } );
    },

    GenerateExccessCombo: function ( identity )
    {
        var objExcess = MerchantManager.LoadExccessPercentData();
        var obj = new Object();
        obj.ExcessPercent1 = "---Select---";
        obj.ExcessPercent = "0";
        objExcess.unshift( obj );
        $( "#" + identity ).kendoComboBox( {
            placeholder: "Select",
            dataTextField: "ExcessPercent1",
            dataValueField: "ExcessPercent",
            dataSource: objExcess,
            index: 0
        } );
    },
    GenerateBuyerCostingStatusCombo: function ( identity )
    {
        var objColor = MerchantManager.GetAllCostingStatus();
        var obj = new Object();
        obj.StatusName = "---Select---";
        obj.StatusId = AjaxManager.DefaultGuidId();
        objColor.unshift( obj );
        $( "#" + identity ).kendoComboBox( {
            placeholder: "--Select--",
            dataValueField: "StatusId",
            dataTextField: "StatusName",
            dataSource: objColor,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function ()
            {
                AjaxManager.isValidItem( identity, true );
            }
        } );
    },
    GenerateLeadTimeCombo: function ( identity )
    {
        var objLeadTime = []; //MerchantManager.GetAllLeadTime();
        var obj = new Object();
        obj.LeadTimeName = "---Select---";
        obj.LeadTimeId = AjaxManager.DefaultGuidId();
        objLeadTime.unshift( obj );
        $( "#" + identity ).kendoComboBox( {
            placeholder: "--Select--",
            dataValueField: "LeadTimeId",
            dataTextField: "LeadTimeName",
            dataSource: objLeadTime,
            index: 0,
            suggest: true,
            filter: "contains",
            change: function ()
            {
                AjaxManager.isValidItem( identity, true );
            }
        } );
    },
    GenerateCurrencyCombo: function ( identity )
    {
        var objCurrency = MerchantManager.GetAllCurrency();
        $( "#" + identity ).kendoComboBox( {
            placeholder: "--Select--",
            dataSource: objCurrency,
            dataValueField: "CurrencyId",
            dataTextField: "CurrencyCode",
            index: 0,
            suggest: true,
            filter: "contains",
            change: function ()
            {
                AjaxManager.isValidItem( identity, true );
            },
        } );
    },

    GeneratePOCombo: function (identity) {
        var objColor = MerchantManager.GetAllPODropdown();
        $("#" + identity).kendoComboBox({
            placeholder: "--Select--",
            dataSource: objColor,
            dataValueField: "Poid",
            dataTextField: "Pono",
            index: 0,
            suggest: true,
            filter: "contains",
            change: function () {
                AjaxManager.isValidItem(identity, true);
            },
        });
    },
}