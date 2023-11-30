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
    public class ReportDSRepo
    {

        public DataSet ProductNew(string ItemNo, string CategoryID, string IsRaw)
        {
            try
            {
                return new ReportDSDAL().ProductNew(ItemNo, CategoryID, IsRaw);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet ProductCategoryNew(string cgID)
        {
            try
            {
                return new ReportDSDAL().ProductCategoryNew(cgID);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public DataSet VendorGroupNew(string VendorGroupID)
        {
            try
            {
                return new ReportDSDAL().VendorGroupNew(VendorGroupID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet CustomerGroupNew(string CustomerGroupID)
        {
            try
            {
                return new ReportDSDAL().CustomerGroupNew(CustomerGroupID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet VendorReportNew(string VendorID, string VendorGroupID)
        {
            try
            {
                return new ReportDSDAL().VendorReportNew(VendorID, VendorGroupID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet CustomerNew(string cId, string cgId)
        {
            try
            {
                return new ReportDSDAL().CustomerNew(cId, cgId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet BankNew(string bankId)
        {
            try
            {
                return new ReportDSDAL().BankNew(bankId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet ComapnyProfile(string cmpId)
        {
            try
            {
                return new ReportDSDAL().ComapnyProfile(cmpId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet VAT16New(string ItemNo, string UserName, string StartDate, string EndDate, string post1, string post2, string ReportName)
        {
            try
            {
                return new ReportDSDAL().VAT16New(ItemNo, UserName, StartDate, EndDate, post1, post2, ReportName);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet VAT18New(string UserName, string StartDate, string EndDate, string post1, string post2)
        {
            try
            {
                return new ReportDSDAL().VAT18New(UserName, StartDate, EndDate, post1, post2);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet BureauVAT17Report(string ItemNo, string StartDate, string EndDate, string post1, string post2)
        {
            try
            {
                return new ReportDSDAL().BureauVAT17Report(ItemNo, StartDate, EndDate, post1, post2);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet VAT17New(string ItemNo, string StartDate, string EndDate, string post1, string post2)
        {
            try
            {
                return new ReportDSDAL().VAT17New(ItemNo, StartDate, EndDate, post1, post2);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<VAT_16> VAT16List(DataSet ReportResult)
        {
            try
            {
                return new ReportDSDAL().VAT16List(ReportResult);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable MonthlyPurchases(string PurchaseInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo,
           string VendorId, string ItemNo, string CategoryID, string ProductType, string TransactionType, string Post,
           string PurchaseType, string VendorGroupId, string FromBOM, string UOM, string UOMn, decimal UOMc, decimal TotalQty, decimal rCostPrice)
        {
            try
            {
                return new ReportDSDAL().MonthlyPurchases(PurchaseInvoiceNo, InvoiceDateFrom, InvoiceDateTo, VendorId, ItemNo, CategoryID, ProductType, TransactionType, Post, PurchaseType, VendorGroupId, FromBOM, UOM, UOMn, UOMc, TotalQty, rCostPrice);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet PurchaseNew(string PurchaseInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo,
                                   string VendorId, string ItemNo, string CategoryID, string ProductType,
                                   string TransactionType, string Post,
                                   string PurchaseType, string VendorGroupId, string FromBOM, string UOM, string UOMn,
                                   decimal UOMc, decimal TotalQty, decimal rCostPrice, bool pCategoryLike = false, string PGroup = "")
        {
            try
            {
                return new ReportDSDAL().PurchaseNew( PurchaseInvoiceNo, InvoiceDateFrom, InvoiceDateTo,
                                   VendorId, ItemNo, CategoryID, ProductType,
                                   TransactionType, Post,
                                   PurchaseType, VendorGroupId, FromBOM, UOM, UOMn,
                                  UOMc, TotalQty, rCostPrice, pCategoryLike,  PGroup);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
