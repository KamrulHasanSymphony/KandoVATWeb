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
    public class MPLBankDepositSlipRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public MPLBankDepositSlipRepo()
        {
            connVM = null;
        }
        public MPLBankDepositSlipRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
        public MPLBankDepositSlipRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }


        public string[] InsertToMPLBankDepositSlip(MPLBankDepositSlipHeaderVM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new MPLBankDepositSlipDAL().InsertToMPLBankDepositSlip(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string[] UpdateMPLBankDepositSlip(MPLBankDepositSlipHeaderVM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new MPLBankDepositSlipDAL().UpdateMPLBankDepositSlip(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<MPLBankDepositSlipHeaderVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, string transactionType = null, string Orderby = "Y", string SelectTop = "100")
        {
            try
            {
                return new MPLBankDepositSlipDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM, null, transactionType, Orderby, SelectTop);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MPLBankDepositSlipDetailVM> SearchMPLBankDepositSlipDetailList(string transferMPLIssueId)
        {
            try
            {
                return new MPLBankDepositSlipDAL().SearchMPLBankDepositSlipDetailList(transferMPLIssueId, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MPLBankDepositSlipDetailVM> GetMPLBankDepositSlipDetailList(string branchId, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new MPLBankDepositSlipDAL().GetMPLBankDepositSlipDetailList(branchId, conditionFields, conditionValues, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MPLBankDepositSlipDetailVM> DropDown()
        {
            try
            {
                return new MPLBankDepositSlipDAL().DropDown(connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
