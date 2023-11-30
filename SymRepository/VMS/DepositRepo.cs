using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using VATServer.Library;
using VATViewModel.DTOs;

namespace SymRepository.VMS
{
    public class DepositRepo
    {

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public DepositRepo()
        {
                connVM = null;
        }
        public DepositRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public DepositRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public string[] DepositInsert(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory)
        {
            return new DepositDAL().DepositInsert(Master, vds, adjHistory, null, null, connVM);
        }

        public List<DepositMasterVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, string transactionOpening = "")
        {
            try
            {
                return new DepositDAL().SelectAllList(Id, conditionFields, conditionValues, null, null, connVM, transactionOpening);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] DepositUpdate(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory)
        {
            return new DepositDAL().DepositUpdate(Master, vds, adjHistory,connVM);
        }

        public string[] DepositPost(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory)
        {
            try
            {
                return new DepositDAL().DepositPost(Master, vds, adjHistory, connVM);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public ResultVM MultiplePost(DepositMasterVM paramVM)
        {
            try
            {
                return new DepositDAL().MultiplePost(paramVM,null,null,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetVDSExcelDataWeb(List<string> Ids)
        {
            try
            {
                return new DepositDAL().GetExcelData(Ids, null, null, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetVDSExcelDataWithcustomerWeb(List<string> Ids)
        {
            try
            {
                return new DepositDAL().GetExcelDataWithCustomer(Ids, null, null, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string[] ImportExcelFile(DepositMasterVM vm)
        {
            try
            {

                return new DepositDAL().ImportExcelFile(vm, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] UpdateVdsComplete(string flag, string VdsId, SysDBInfoVMTemp connVM = null, string TransactionType = "")
        {
            return new DepositDAL().UpdateVdsComplete(flag, VdsId, connVM, TransactionType);
        }



    }
}
