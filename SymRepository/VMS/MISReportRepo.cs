using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using VATServer.Library;
using VATServer.Library.Integration;
using VATViewModel;
using VATViewModel.DTOs;
using VMSAPI;

namespace SymRepository.VMS
{
   public class MISReportRepo
    {
         private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
         public MISReportRepo()
           {
                connVM = null;
           }
        public MISReportRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public MISReportRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
         public DataSet StockMovement( StockMovementVM vm,SqlConnection VcurrConn
          , SqlTransaction Vtransaction,  SysDBInfoVMTemp connVM = null )
         {
             try
             {
                return new ProductDAL().StockMovement(vm, null, null, connVM);

             }
             catch (Exception ex)
             {
                 throw ex;
             }
         }
         public ResultVM DataProcess(IntegrationParam vm)
         {
             try
             {
                 return new UNILEVERIntegrationDAL().DataProcess(vm, null, null, connVM);
             }
             catch (Exception)
             {

                 throw;
             }
         }

         public DataTable GetTempData()
         {
             try
             {
                 return new UNILEVERIntegrationDAL().GetTempData(connVM);

             }
             catch (Exception ex)
             {
                 throw ex;
             }
         }
         public DataTable GetTempDataDetails()
         {
             try
             {
                 return new UNILEVERIntegrationDAL().GetTempDataDetails(connVM);

             }
             catch (Exception ex)
             {
                 throw ex;
             }
         }
         //public DataSet StockMovement(string itemNo, string tranDate, string tranDateTo, int BranchId, SqlConnection VcurrConn
         // , SqlTransaction Vtransaction, string Post1, string Post2, string ProdutType
         //   , string ProdutCategoryId, SysDBInfoVMTemp connVM = null, string gName = "", bool chkCategoryLike = false, string FormNumeric = "2", string UserId = "")
         //{
         //    try
         //    {
         //        return new ProductDAL().StockMovement(itemNo, tranDate,  tranDateTo,  BranchId,  VcurrConn
         //    ,  Vtransaction,  Post1,  Post2,  ProdutType, ProdutCategoryId,  connVM ,  gName,  chkCategoryLike ,  FormNumeric,  UserId);

         //    }
         //    catch (Exception ex)
         //    {
         //        throw ex;
         //    }
         //}

         public DataTable GetYearlySaleData(SaleMISViewModel vm)
         {
             try
             {
                 return new ReportDSDAL().GetYearlySaleData(vm,null,null,connVM);
             }
             catch (Exception ex)
             {
                 throw ex;
             }
         }

         public DataSet GetMonthlySaleData(SaleMISViewModel vm, string FirstPriodID="", string SecondPriodID="")
         {
             try
             {
                 return new ReportDSDAL().GetMonthlySaleData(vm, FirstPriodID, SecondPriodID, null, null, connVM);
             }
             catch (Exception ex)
             {
                 throw ex;
             }
         }
         public DataTable GetYearlyPurchaseData(PurchaseMISViewModel vm)
         {
             try
             {
                 return new ReportDSDAL().GetYearlyPurchaseData(vm, null, null, connVM);
             }
             catch (Exception ex)
             {
                 throw ex;
             }
         }

         public DataSet GetMonthlyPurchaseData(PurchaseMISViewModel vm, string FirstPriodID = "", string SecondPriodID = "")
         {
             try
             {
                 return new ReportDSDAL().GetMonthlyPurchaseData(vm, FirstPriodID, SecondPriodID, null, null, connVM);
             }
             catch (Exception ex)
             {
                 throw ex;
             }
         }
         public DataTable GetTransferIssueData(TransferMISViewModel vm)
         {
             try
             {
                 return new ReportDSDAL().GetTransferIssueData(vm, null, null, connVM);
             }
             catch (Exception ex)
             {
                 throw ex;
             }
         }
         public DataTable GetTransferIssueDetailsData(TransferMISViewModel vm)
         {
             try
             {
                 return new ReportDSDAL().GetTransferIssueDetailsData(vm, null, null, connVM);
             }
             catch (Exception ex)
             {
                 throw ex;
             }
         }
         public DataTable GetTransferReceiveData(TransferMISViewModel vm)
         {
             try
             {
                 return new ReportDSDAL().GetTransferReceiveData(vm, null, null, connVM);
             }
             catch (Exception ex)
             {
                 throw ex;
             }
         }
         public DataTable GetTransferReceiveDetailsData(TransferMISViewModel vm)
         {
             try
             {
                 return new ReportDSDAL().GetTransferReceiveDetailsData(vm, null, null, connVM);
             }
             catch (Exception ex)
             {
                 throw ex;
             }
         }
    }
}
