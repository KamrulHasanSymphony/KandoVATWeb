using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Linq;
using VATViewModel.DTOs;
using VATServer.Library;
using VMSAPI;
using SymOrdinary;

namespace SymRepository.VMS
{
    public class SaleInvoiceRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public SaleInvoiceRepo()
        {
            connVM = null;
        }
        public SaleInvoiceRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
        public SaleInvoiceRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }



        public string[] UpdatePrintNew(string SalesInvoiceNo, int PrintCopy)
        {
            try
            {
                return new SaleDAL().UpdatePrintNew(SalesInvoiceNo, PrintCopy, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public string[] SalesInsert(SaleMasterVM Master, List<SaleDetailVm> Details, List<SaleExportVM> ExportDetails, List<TrackingVM> Trackings, SqlTransaction transaction = null, SqlConnection currConn = null, string UserId = "", List<SaleDeliveryTrakingVM> DeliveryTrakings = null)
        {
            try
            {
                return new SaleDAL().SalesInsert(Master, Details, ExportDetails, Trackings, transaction, currConn, 0, connVM, UserId, true, DeliveryTrakings);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<SaleMasterVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SaleMasterVM likeVM = null, string transactionType = null, string Orderby = "Y", string[] ids = null, string Is6_3TollCompleted = null)
        {
            try
            {
                return new SaleDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, likeVM, connVM, transactionType, Orderby, ids, Is6_3TollCompleted);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SaleEngineChassisVM> SelectAllEngineList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SaleMasterVM likeVM = null, string transactionType = null)
        {
            try
            {
                return new SaleDAL().SelectAllEngineList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, likeVM, connVM, transactionType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SaleDetailVm> SelectSaleDetail(string saleInvoiceNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new SaleDAL().SelectSaleDetail(saleInvoiceNo, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] SalesUpdate(SaleMasterVM Master, List<SaleDetailVm> Details, List<SaleExportVM> ExportDetails, List<TrackingVM> Trackings, string UserId = "", List<SaleDeliveryTrakingVM> DeliveryTrakings = null)
        {
            return new SaleDAL().SalesUpdate(Master, Details, ExportDetails, Trackings, connVM, UserId, true, DeliveryTrakings);
        }

        public string[] SalesPost(SaleMasterVM Master, List<SaleDetailVm> Details, List<TrackingVM> Trackings)
        {
            try
            {
                return new SaleDAL().SalesPost(Master, Details, Trackings, null, null, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] ImportExcelFile(SaleMasterVM paramVM)
        {
            try
            {
                return new SaleDAL().ImportExcelFile(paramVM, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] ImportExcelFileFull(SaleMasterVM paramVM)
        {
            try
            {
                return new SaleDAL().ImportExcelFileFull(paramVM, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public string[] ImportExcelIntegrationFile(SaleMasterVM paramVM)
        {
            try
            {
                return new SaleDAL().ImportExcelIntegrationFile(paramVM, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IEnumerable<object> GetSalesColumn()
        {
            try
            {
                string[] columnName = new string[] { "Sales Invoice No", "Serial No", "Vehicle No", "Import ID", "Customer Name", "Customer Code" };
                IEnumerable<object> enumList = from e in columnName select new { Id = e.ToString().Replace(" ", ""), Name = e.ToString() };
                return enumList;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public IEnumerable<object> MPLSalesColumnSearch()
        {
            try
            {
                string[] columnName = new string[] { "Sales Invoice No", "Bank Name", "Bank Code", "Customer Name", "Mode Of Payment", "Instrument No", "Instrument Date" };
                IEnumerable<object> enumList = from e in columnName select new { Id = e.ToString().Replace(" ", ""), Name = e.ToString() };
                return enumList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region Api
        public string[] InsertSaleApi(string saleXML)
        {
            try
            {
                //return new SaleAPI().ImportSale(saleXML, null,1);
                return new string[3];
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public string ImportSale(string xml)
        {
            try
            {
                SaleAPI api = new SaleAPI();

                var result = api.ImportSale(xml);

                return result;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public ResultVM ImportSaleBollore(string xml)
        {
            try
            {
                SaleAPI api = new SaleAPI();

                ResultVM rVM = api.ImportSaleBollore(xml);

                return rVM;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public string[] ErrorLogs(ErrorLogVM vm)
        {
            try
            {
                CommonDAL _cdal = new CommonDAL();

                string[] rVM = _cdal.InsertErrorLogs(vm, null, null, connVM);

                return rVM;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public ResultVM ImportSaleBritishCouncil(string xml)
        {
            try
            {
                SaleAPI api = new SaleAPI();

                ResultVM rVM = api.ImportSaleBritishCouncil(xml);

                return rVM;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        #endregion

        public DataTable SearchByReferenceNo(string SerialNo)
        {
            try
            {
                return new ReceiveDAL().SearchByReferenceNo(SerialNo, "", connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public DataTable GetSource_SaleData_dis_Details(IntegrationParam paramVM)
        {
            try
            {
                return new SaleDAL().GetSource_SaleData_dis_Details(paramVM, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<SalesInvoiceExpVM> SalesInvoiceExpsLoad()
        {
            try
            {
                return new SalesInvoiceExpDAL().SelectAllList(0, null, null, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public string[] SaveTransactions(SaleMasterVM paramVM)
        {
            try
            {
                return new SaleDAL().SaveToTransactionTables(() => { }, paramVM.BranchId, "", connVM, paramVM.Token);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ResultVM Multiple_SalePost(SaleMasterVM paramVM)
        {
            try
            {
                return new SaleDAL().Multiple_SalePost(paramVM, null, null, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public ResultVM Multiple_EngineRemove(SaleEngineChassisVM paramVM)
        {
            try
            {
                return new SaleDAL().Multiple_EngineRemove(paramVM, null, null, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM EngineChange_Credit(SaleEngineChassisVM paramVM)
        {
            try
            {
                return new SaleDAL().EngineChange_Credit(paramVM, null, null, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSaleExcelDataWeb(List<string> Ids, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new SaleDAL().GetSaleExcelDataWeb(Ids, null, null, connVM, conditionFields, conditionValues);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<SaleDetailVm> SearchSaleDetailDTNewList(string saleInvoiceNo, string InvoiceDate, bool SearchPreviousForCNDN = false)
        {
            try
            {
                return new SaleDAL().SearchSaleDetailDTNewList(saleInvoiceNo, InvoiceDate, SearchPreviousForCNDN, null, null, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ResultVM InsertEngine(SaleEngineChassisVM vm)
        {
            try
            {
                return new SaleDAL().InsertEngine(vm, null, null, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public MISExcelVM DownloadMIS_SaleReport(MISExcelVM vm)
        {
            try
            {
                SaleDAL saleDalObj = new SaleDAL();
                return saleDalObj.SaleMISExcelDownload(vm, connVM);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public DataTable GetSaleTrakingExcelData(TrakingSaleVM vm)
        {
            try
            {
                return new SaleDAL().GetSaleTrakingExcelData(vm.CustomerID, vm.ItemNo, vm.InvoiceDateTimeFrom, vm.InvoiceDateTimeTo);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable GetPurchaseTrakingExcelData(TrakingSaleVM vm)
        {
            try
            {
                return new PurchaseDAL().GetPurchaseTrakingExcelData(vm.VendorID, vm.ItemNo, vm.InvoiceDateTimeFrom, vm.InvoiceDateTimeTo, vm.ReceiveDateFrom, vm.ReceiveDateTo, vm.ExpireDateFrom, vm.ExpireDateTo);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable MISStockReport(TrakingSaleVM vm)
        {
            try
            {
                return new PurchaseDAL().MISStockReport(vm.VendorID, vm.ItemNo, vm.InvoiceDateTimeFrom, vm.InvoiceDateTimeTo, vm.ReceiveDateFrom, vm.ReceiveDateTo, vm.ExpireDateFrom, vm.ExpireDateTo);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public DataTable FindTrackingItems(string fItemNo, string vatName, string effectDate, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new TrackingDAL().FindTrackingItems(fItemNo, vatName, effectDate, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<TrackingVM> SelectTrakingsDetail(string itemNo, string isTransaction, string transactionId, string type, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new TrackingDAL().GetTrackingsWeb(itemNo, isTransaction, transactionId, type, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string TrackingUpdate(List<TrackingVM> Trackings, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new TrackingDAL().TrackingUpdate(Trackings, Vtransaction, VcurrConn, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<TrackingVM> SelectTrakingsDetail(List<SaleDetailVm> DetailVMs, string ID, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new SaleDAL().GetTrackingsWeb(DetailVMs, ID, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet ACIReconsileData(string fromDate, string toDate)
        {
            try
            {
                return new SaleDAL().ACIReconsileData(fromDate, toDate, null, null, connVM);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetDateWiseSale(string fromDate, string toDate, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new SaleDAL().GetDateWiseSale(fromDate, toDate, connVM);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SaleDeliveryTrakingVM> SelectSaleDeliveryTrakings(string DeliveryChallanNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new SaleDAL().SelectSaleDeliveryTrakings(DeliveryChallanNo, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] DeliveryChallanInsert(SaleDeliveryTrakingVM DeliveryTrakings, SqlTransaction transaction = null, SqlConnection currConn = null, int branchId = 0, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new SaleDAL().DeliverySalesInsert(DeliveryTrakings, transaction, currConn, 0, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string[] DeliveryChallanUpdate(SaleDeliveryTrakingVM DeliveryTrakings, SqlTransaction transaction = null, SqlConnection currConn = null, int branchId = 0, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new SaleDAL().DeliverySalesUpdate(DeliveryTrakings, transaction, currConn, 0, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM PostDeliveryChallan(ParameterVM Master, string UserId = "")
        {
            return new SaleDAL().PostDeliveryChallan(Master, null, null, connVM);

        }

    }
}
