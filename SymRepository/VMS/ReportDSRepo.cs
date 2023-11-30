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
    public class ReportDSRepo
    {
          private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

          public ReportDSRepo()
          {
               connVM = null;
          }
          public ReportDSRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
          public ReportDSRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public DataSet ProductNew(string ItemNo, string CategoryID, string IsRaw)
        {
            try
            {
                return new ReportDSDAL().ProductNew(ItemNo, CategoryID, IsRaw,connVM);
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
                return new ReportDSDAL().ProductCategoryNew(cgID,connVM);
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
                return new ReportDSDAL().VendorGroupNew(VendorGroupID,connVM);
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
                return new ReportDSDAL().CustomerGroupNew(CustomerGroupID,connVM);
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
                return new ReportDSDAL().VendorReportNew(VendorID, VendorGroupID,connVM);
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
                return new ReportDSDAL().CustomerNew(cId, cgId,connVM);
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
                return new ReportDSDAL().BankNew(bankId,connVM);
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
                return new ReportDSDAL().ComapnyProfile(cmpId,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        ////public DataSet VAT16New(string ItemNo, string UserName, string StartDate, string EndDate, string post1, string post2, string ReportName, int branchId)
        public DataSet VAT16New(VAT6_1ParamVM vm)
        {
            try
            {
                ////return new ReportDSDAL().VAT6_1_WithConn(ItemNo, UserName, StartDate, EndDate, post1, post2, ReportName, branchId);
                return new VATRegistersDAL().VAT6_1_WithConn(vm, null, null, connVM);
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
                return new ReportDSDAL().VAT18New(UserName, StartDate, EndDate, post1, post2,connVM);
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
                return new DataSet();//new ReportDSDAL().BureauVAT17Report(ItemNo, StartDate, EndDate, post1, post2);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet VAT17New_Backup(string ItemNo, string StartDate, string EndDate, string post1, string post2)
        {
            return null;
        }


        public DataSet VAT17New(VAT6_2ParamVM vm)
        {
            try
            {
                return new VATRegistersDAL().VAT6_2(vm, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<VAT_16VM> VAT16List(DataSet ReportResult)
        {
            try
            {
                return new ReportDSDAL().VAT16List(ReportResult,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<VAT_17VM> VAT17List(DataSet ReportResult, string itemNo)
        {
            try
            {
                return new ReportDSDAL().VAT17List(ReportResult, itemNo,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable MonthlyPurchases(string PurchaseInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo, string VendorId, string ItemNo, string CategoryID, string ProductType, string TransactionType, string Post, string PurchaseType, string VendorGroupId, string FromBOM, string UOM, string UOMn, decimal UOMc, decimal TotalQty, decimal rCostPrice)
        {
            try
            {
                return new ReportDSDAL().MonthlyPurchases(PurchaseInvoiceNo, InvoiceDateFrom, InvoiceDateTo, VendorId, ItemNo, CategoryID, ProductType, TransactionType, Post, PurchaseType, VendorGroupId, FromBOM, UOM, UOMn, UOMc, TotalQty, rCostPrice,0,"",connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet PurchaseNew(string PurchaseInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo, string VendorId, string ItemNo, string CategoryID, string ProductType, string TransactionType, string Post, string PurchaseType, string VendorGroupId, string FromBOM, string UOM, string UOMn, decimal UOMc, decimal TotalQty, decimal rCostPrice, bool pCategoryLike = false, string PGroup = "")
        {
            try
            {
                return new ReportDSDAL().PurchaseNew(PurchaseInvoiceNo, InvoiceDateFrom, InvoiceDateTo,
                                   VendorId, ItemNo, CategoryID, ProductType,
                                   TransactionType, Post,
                                   PurchaseType, VendorGroupId, FromBOM, UOM, UOMn,
                                  UOMc, TotalQty, rCostPrice, pCategoryLike, PGroup, 0, "", connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet IssueNew(string IssueNo, string IssueDateFrom, string IssueDateTo, string itemNo, string categoryID, string productType, string TransactionType, string Post, string waste, bool pCategoryLike = false, string PGroup = "")
        {
            try
            {
                return new ReportDSDAL().IssueNew(IssueNo, IssueDateFrom, IssueDateTo,
                                   itemNo, categoryID, productType, TransactionType,
                                   Post, waste,
                                   pCategoryLike, PGroup,0,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet IssueMis(string IssueId, int BranchId = 0)
        {
            try
            {
                return new ReportDSDAL().IssueMis(IssueId, BranchId,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet SD_Data(string UserName, string StartDate, string EndDate, string post1, string post2)
        {
            try
            {
                return new ReportDSDAL().SD_Data(UserName, StartDate, EndDate, post1, post2, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet RptVAT7Report(string vat7No)
        {
            try
            {
                return new ReportDSDAL().RptVAT7Report(vat7No, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet Current_AC_VAT18(string StartDate, string EndDate, string post1, string post2)
        {
            try
            {
                return new ReportDSDAL().Current_AC_VAT18(StartDate, EndDate, post1, post2, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet ReceiveNew(string ReceiveNo, string ReceiveDateFrom, string ReceiveDateTo, string itemNo, string categoryID, string productType, string transactionType, string post)
        {
            try
            {
                return new ReportDSDAL().ReceiveNew(ReceiveNo, ReceiveDateFrom, ReceiveDateTo, itemNo, categoryID, productType, transactionType, post,"0",0, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet ReceiveMis(string ReceiveId)
        {
            try
            {
                return new ReportDSDAL().ReceiveMis(ReceiveId,"0",0,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet PurchaseMis(string PurchaseId)
        {
            try
            {
                return new ReportDSDAL().PurchaseMis(PurchaseId,0,"",connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet BureauSaleNew(string SalesInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo, string Customerid, string ItemNo, string CategoryID, string productType, string TransactionType, string Post, string onlyDiscount, bool bPromotional, string CustomerGroupID)
        {
            try
            {
                return new ReportDSDAL().BureauSaleNew(SalesInvoiceNo, InvoiceDateFrom, InvoiceDateTo, Customerid, ItemNo, CategoryID, productType, TransactionType, Post, onlyDiscount, bPromotional, CustomerGroupID,"1",0,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet SaleNewWithChassisEngine(string SalesInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo, string Customerid, string ItemNo, string CategoryID, string productType, string TransactionType, string Post, string onlyDiscount, bool bPromotional, string CustomerGroupID, string chassis, string engine)
        {
            try
            {
                return new ReportDSDAL().SaleNewWithChassisEngine(SalesInvoiceNo, InvoiceDateFrom, InvoiceDateTo, Customerid, ItemNo, CategoryID, productType, TransactionType, Post, onlyDiscount, bPromotional, CustomerGroupID, chassis, engine,"0",0,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet SaleNew(string SalesInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo, string Customerid, string ItemNo, string CategoryID,
            string productType, string TransactionType, string Post, string onlyDiscount, bool bPromotional, string CustomerGroupID, bool pCategoryLike = false,
            string PGroup = "", string ShiftId = "0", int branchId = 0, string VatType = "", string OrderBy = "",string ReportType="")
        {
            try
            {
                return new ReportDSDAL().SaleNew(SalesInvoiceNo, InvoiceDateFrom, InvoiceDateTo, Customerid, ItemNo, CategoryID, productType, TransactionType,
                    Post, onlyDiscount, bPromotional, CustomerGroupID, pCategoryLike, PGroup, ShiftId, branchId, VatType, connVM, OrderBy, "", "N", "", ReportType);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public DataSet VATCreditNoteMis(string SalesInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo, string Customerid, string ItemNo, string CategoryID,
            string productType, string TransactionType, string Post, string onlyDiscount, bool bPromotional, string CustomerGroupID, bool pCategoryLike = false,
            string PGroup = "", string ShiftId = "0", int branchId = 0, string VatType = "", string OrderBy = "")
        {
            try
            {
                return new ReportDSDAL().VATCreditNoteMis(SalesInvoiceNo, InvoiceDateFrom, InvoiceDateTo, Customerid, ItemNo, CategoryID, productType, TransactionType,
                    Post, onlyDiscount, bPromotional, CustomerGroupID, pCategoryLike, PGroup, ShiftId, branchId, VatType, connVM, OrderBy);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet BureauSaleMis(string SaleId)
        {
            try
            {
                return new ReportDSDAL().BureauSaleMis(SaleId,"0",0,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet SaleMis(string SaleId)
        {
            try
            {
                return new ReportDSDAL().SaleMis(SaleId,"0",0,"",connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet VAT18Breakdown(string PeriodName, string ExportInBDT)
        {
            try
            {
                return new ReportDSDAL().VAT18Breakdown(PeriodName, ExportInBDT,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet StockNew(string ProductNo, string CategoryNo, string ItemType, string StartDate, string EndDate, string Post1, string Post2, bool WithoutZero = false, bool pCategoryLike = false, string PGroup = "")
        {
            try
            {
                return new ReportDSDAL().StockNew(ProductNo, CategoryNo, ItemType, StartDate, EndDate, Post1, Post2, WithoutZero, pCategoryLike, PGroup,0,connVM);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataSet SaleReceiveMIS(string StartDate, string EndDate, string ShiftId = "1", string Post = null)
        {
            try
            {
                return new ReportDSDAL().SaleReceiveMIS(StartDate, EndDate, ShiftId, Post,connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataSet BOMNew(string BOMId, string VATName, string IsPercent)
        {
            try
            {
                return new ReportDSDAL().BOMNew(BOMId, VATName, IsPercent,0,connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataSet DepositNew(string DepositNo, string DepositDateFrom, string DepositDateTo, string BankID, string Post, string transactionType)
        {
            try
            {
                return new ReportDSDAL().DepositNew(DepositNo, DepositDateFrom, DepositDateTo, BankID, Post, transactionType,connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataSet Adjustment(string HeadId, string AdjType, string StartDate, string EndDate, string Post)
        {
            try
            {
                return new ReportDSDAL().Adjustment(HeadId, AdjType, StartDate, EndDate, Post,0,connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataSet InputOutputCoEfficient(string RawItemNo, string StartDate, string EndDate, string Post1, string Post2)
        {
            try
            {
                return new ReportDSDAL().InputOutputCoEfficient(RawItemNo, StartDate, EndDate, Post1, Post2,connVM);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataSet StockWastage(string ProductNo, string CategoryNo, string ItemType, string StartDate, string EndDate, string Post1, string Post2, bool WithoutZero = false)
        {
            try
            {
                return new ReportDSDAL().StockWastage(ProductNo, CategoryNo, ItemType, StartDate, EndDate, Post1, Post2, WithoutZero,0,connVM);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataSet TollRegister(string ItemNo, string UserName, string StartDate, string EndDate, string post1, string post2)
        {
            try
            {
                return new ReportDSDAL().TollRegister(ItemNo, UserName, StartDate, EndDate, post1, post2,connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataSet SerialStockStatus(string ItemNo, string CategoryID, string ProductType, string StartDate, string ToDate, string post1, string post2)
        {
            try
            {
                return new ReportDSDAL().SerialStockStatus(ItemNo, CategoryID, ProductType, StartDate, ToDate, post1, post2,connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataSet PurchaseWithLCInfo(string PurchaseInvoiceNo, string LCDateFrom, string LCDateTo, string VendorId, string ItemNo, string VendorGroupId, string LCNo, string Post)
        {
            try
            {
                return new ReportDSDAL().PurchaseWithLCInfo(PurchaseInvoiceNo, LCDateFrom, LCDateTo, VendorId, ItemNo, VendorGroupId, LCNo, Post,connVM);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable MIS19(string StartDate, string EndDate)
        {
            try
            {
                return new ReportDSDAL().MIS19(StartDate, EndDate,connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataSet VAT19NewNewformat(string PeriodName, string ExportInBDT)
        {
            try
            {
                return new ReportDSDAL().VAT19NewNewformat(PeriodName, ExportInBDT,connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataSet TrasurryDepositeNew(string DepositId)
        {
            try
            {
                return new ReportDSDAL().TrasurryDepositeNew(DepositId,connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataSet VDSDepositNew(string DepositId)
        {
            try
            {
                return new ReportDSDAL().VDSDepositNew(DepositId,connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public DataSet ComapnyProfileString(string companyId, string userId)
        {
            try
            {
                ReportDSDAL reportDsdal = new ReportDSDAL();
                DataSet ReportResult = reportDsdal.ComapnyProfileString(companyId, userId,connVM);

                return ReportResult;
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        public DataTable BureauMonthlySales(string SalesInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo,
                                     string Customerid, string ItemNo, string CategoryID, string productType,
                                     string TransactionType, string Post, string onlyDiscount, bool bPromotional,
                                     string CustomerGroupID, string ShiftId = "1", int branchId = 0, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new ReportDSDAL().BureauMonthlySales(SalesInvoiceNo, InvoiceDateFrom, InvoiceDateTo, Customerid, ItemNo, CategoryID, productType,
                                      TransactionType, Post, onlyDiscount, bPromotional, CustomerGroupID, ShiftId, branchId, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public DataTable MonthlySales(string SalesInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo,
                                      string Customerid, string ItemNo, string CategoryID, string productType,
                                      string TransactionType, string Post, string onlyDiscount, bool bPromotional,
                                      string CustomerGroupID, string ShiftId = "1", int branchId = 0, string VatType = "", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new ReportDSDAL().MonthlySales(SalesInvoiceNo, InvoiceDateFrom, InvoiceDateTo,
                                      Customerid, ItemNo, CategoryID, productType,
                                      TransactionType, Post, onlyDiscount, bPromotional,
                                      CustomerGroupID, ShiftId, branchId, VatType,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }






    }


}
