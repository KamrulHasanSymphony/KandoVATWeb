using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using VATViewModel.DTOs;
using VATServer.Library;
using SymOrdinary;
using System.Web;

namespace SymRepository.VMS
{
    public class BureauSaleRepo
    {
         private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
         public BureauSaleRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

         public BureauSaleRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public string[] SalesInsert(SaleMasterVM Master, List<BureauSaleDetailVM> Details, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new BureauSaleDAL().SalesInsert(Master, Details, transaction, currConn,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdatePrintNew(string InvoiceNo, int PrintCopy)
        {
            try
            {
                return new BureauSaleDAL().UpdatePrintNew(InvoiceNo, PrintCopy,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] SalesUpdate(SaleMasterVM Master, List<BureauSaleDetailVM> Details)
        {
            try
            {
                return new BureauSaleDAL().SalesUpdate(Master, Details,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] SalesPost(SaleMasterVM Master, List<BureauSaleDetailVM> Details)
        {
            try
            {
                return new BureauSaleDAL().SalesPost( Master,  Details,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SearchSaleExportDTNew(string SalesInvoiceNo, string databaseName)
        {
            try
            {
                return new BureauSaleDAL().SearchSaleExportDTNew(SalesInvoiceNo, databaseName,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SearchSalesHeaderDTNew(string SalesInvoiceNo, string CustomerName, string CustomerGroupName, string VehicleType, string VehicleNo, string SerialNo, string InvoiceDateFrom, string InvoiceDateTo, string SaleType, string Trading, string IsPrint, string transactionType, string Post)
        {
            try
            {
                return new BureauSaleDAL().SearchSalesHeaderDTNew(SalesInvoiceNo, CustomerName,CustomerGroupName, VehicleType, VehicleNo, SerialNo, InvoiceDateFrom,InvoiceDateTo, SaleType, Trading, IsPrint, transactionType, Post,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SearchSaleDetailDTNew(string SalesInvoiceNo)
        {
            try
            {
                return new BureauSaleDAL().SearchSaleDetailDTNew(SalesInvoiceNo,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public int LoadIssueItems()
        {
            try
            {
                return new BureauSaleDAL().LoadIssueItems(connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] SalesInsertImport(SaleMasterVM Master, List<SaleDetailVm> Details, List<SaleExportVM> ExportDetails, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new BureauSaleDAL().SalesInsertImport(Master,Details,ExportDetails, transaction, currConn,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] ImportInspectionData(DataTable dtSaleM, string noOfSale)
        {
            try
            {
                return new BureauSaleDAL().ImportInspectionData(dtSaleM, noOfSale,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public decimal ReturnSaleQty(string saleReturnId, string itemNo)
        {
            try
            {
                return new BureauSaleDAL().ReturnSaleQty(saleReturnId, itemNo,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] CurrencyInfo(string salesInvoiceNo)
        {
            try
            {
                return new BureauSaleDAL().CurrencyInfo(salesInvoiceNo,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string GetCategoryName(string itemNo)
        {
            try
            {
                return new BureauSaleDAL().GetCategoryName(itemNo,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //public decimal FormatingNumeric(decimal value, int DecPlace)
        //{
        //    try
        //    {
        //        return new BureauSaleDAL().FormatingNumeric( value,  DecPlace);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        public DataTable GetProductInfo()
        {
            try
            {
                return new BureauSaleDAL().GetProductInfo(connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string UpdateCreditNo(string challanDate)
        {
            try
            {
                return new BureauSaleDAL().UpdateCreditNo(challanDate,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string SearchType(string buId)
        {
            try
            {
                return new BureauSaleDAL().SearchType(buId,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
