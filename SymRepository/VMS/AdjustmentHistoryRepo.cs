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
    public class AdjustmentHistoryRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public AdjustmentHistoryRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
        public AdjustmentHistoryRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public string[] InsertAdjustmentHistory(AdjustmentHistoryVM vm)
        {
            try
            {
                return new AdjustmentHistoryDAL().InsertAdjustmentHistory(vm,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateAdjustmentHistory(AdjustmentHistoryVM vm)
        {
            try
            {
                return new AdjustmentHistoryDAL().UpdateAdjustmentHistory(vm,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] PostAdjustmentHistory(AdjustmentHistoryVM vm)
        {
            try
            {
                return new AdjustmentHistoryDAL().PostAdjustmentHistory(vm,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] PostAdjHistory(AdjustmentHistoryVM vm)
        {
            try
            {
                return new AdjustmentHistoryDAL().PostAdjustmentHistory(vm.AdjHistoryID,vm.AdjId,"",0,0,0,0,0,0,"","","","",vm.LastModifiedBy,vm.LastModifiedOn,0,0,"",vm.Post,vm.AdjHistoryNo, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SearchAdjustmentHistory(string AdjHistoryNo, string AdjReferance, string AdjType, string Post, string AdjFromDate, string AdjToDate,int BranchId = 0)
        {
            try
            {
                return new AdjustmentHistoryDAL().SearchAdjustmentHistory(AdjHistoryNo, AdjReferance, AdjType, Post, AdjFromDate, AdjToDate,BranchId,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<AdjustmentHistoryVM> SelectAll(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new AdjustmentHistoryDAL().SelectAll(Id, conditionFields, conditionValues,null,null,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

     public List<AdjustmentHistoryVM> SelectAllCashPayable(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new AdjustmentHistoryDAL().SelectAllCashPayable(Id, conditionFields, conditionValues,null,null,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
    }
}
