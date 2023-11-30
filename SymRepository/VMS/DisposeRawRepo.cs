using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using VATServer.Library;
using VATViewModel.DTOs;
using VMSAPI;

namespace SymRepository.VMS
{
    public class DisposeRawRepo
    {

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public DisposeRawRepo()
        {
             connVM = null;
        }
        public DisposeRawRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public DisposeRawRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }

        public string[] DisposeRawInsert(DisposeRawsMasterVM Master, SqlConnection currConn = null, SqlTransaction transaction = null) 
        {
            return new DisposeRawDAL().DisposeRawsInsert(Master,Master.Details,connVM);
        }

        public string[] DisposeRawsUpdate(DisposeRawsMasterVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, string UserId = "")
        {
            return new DisposeRawDAL().DisposeRawsUpdate(Master,Master.Details,connVM);
        }

        public string[] DisposeRawPost(DisposeRawsMasterVM Master, string UserId = "")
        {
            return new DisposeRawDAL().DisposeRawsPost(Master);

        }

        public List<DisposeRawsMasterVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM likeVM = null) 
        {
            return new DisposeRawDAL().SelectAllList(Id,conditionFields,conditionValues);
        }

        public List<DisposeRawsMasterVM> SelectAllWeb(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM likeVM = null)
        {
            return new DisposeRawDAL().SelectAllList(Id, conditionFields, conditionValues,VcurrConn,Vtransaction);
        }

        public List<BOMNBRVM> SelectAllBOM(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM likeVM = null)
        {
            return new BOMDAL().SelectAllList(null,conditionFields,conditionValues);
        }

        public List<PurchaseDetailVM> SelectPurchaseDetail(string PurchaseInvoiceNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PurchaseDAL().SelectPurchaseDetailList(PurchaseInvoiceNo, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<DisposeRawsDetailVM> SelectDetail(string DisposeNo,string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            return new DisposeRawDAL().Select_DisposeRawDetailVM(DisposeNo,conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
        }
        public string[] ImportExcelFile(IssueMasterVM paramVM)
        {
            try
            {
                return new IssueDAL().ImportExcelFile(paramVM,connVM);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public string[] MultiplePost(string[] Ids)
        {
            try
            {
                return new IssueDAL().MultiplePost(Ids,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetExcelDataWeb(List<string> Ids)
        {
            try
            {
                return new IssueDAL().GetExcelDataWeb(Ids,null,null,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM UpdateAvgPrice_New(AVGPriceVm vm)
        {
            try
            {
                return new IssueDAL().UpdateAvgPrice_New(vm,null,null);
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
