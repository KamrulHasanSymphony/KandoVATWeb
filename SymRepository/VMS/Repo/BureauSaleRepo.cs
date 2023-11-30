using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymServices.VMS.Library;
using SymViewModel.VMS.DTOs;
using System.Data;
using System.Data.SqlClient;

namespace SymRepository.VMS.Repo
{
    public class BureauSaleRepo
    {
        public string[] SalesInsert(SaleMasterVM Master, List<BureauSaleDetailVM> Details, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new BureauSaleDAL().SalesInsert(Master, Details, transaction, currConn);
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
                return new BureauSaleDAL().UpdatePrintNew(InvoiceNo, PrintCopy);
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
                return new BureauSaleDAL().SalesUpdate(Master, Details);
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
                return new BureauSaleDAL().SalesPost( Master,  Details);
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
                return new BureauSaleDAL().SearchSaleExportDTNew(SalesInvoiceNo, databaseName);
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
                return new BureauSaleDAL().SearchSalesHeaderDTNew(SalesInvoiceNo, CustomerName,CustomerGroupName, VehicleType, VehicleNo, SerialNo, InvoiceDateFrom,InvoiceDateTo, SaleType, Trading, IsPrint, transactionType, Post);
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
                return new BureauSaleDAL().SearchSaleDetailDTNew(SalesInvoiceNo);
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
                return new BureauSaleDAL().LoadIssueItems();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] SalesInsertImport(SaleMasterVM Master, List<SaleDetailVM> Details, List<SaleExportVM> ExportDetails, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new BureauSaleDAL().SalesInsertImport(Master,Details,ExportDetails, transaction, currConn);
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
                return new BureauSaleDAL().ImportInspectionData(dtSaleM, noOfSale);
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
                return new BureauSaleDAL().ReturnSaleQty(saleReturnId, itemNo);
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
                return new BureauSaleDAL().CurrencyInfo(salesInvoiceNo);
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
                return new BureauSaleDAL().GetCategoryName(itemNo);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public decimal FormatingNumeric(decimal value, int DecPlace)
        {
            try
            {
                return new BureauSaleDAL().FormatingNumeric( value,  DecPlace);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable GetProductInfo()
        {
            try
            {
                return new BureauSaleDAL().GetProductInfo();
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
                return new BureauSaleDAL().UpdateCreditNo(challanDate);
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
                return new BureauSaleDAL().SearchType(buId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
