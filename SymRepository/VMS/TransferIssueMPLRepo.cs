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
    public class TransferIssueMPLRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public TransferIssueMPLRepo()
        {
            connVM = null;
        }
        public TransferIssueMPLRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
        public TransferIssueMPLRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }


        public string[] TransIssueMPLInsert(TransferMPLIssueVM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new TransferIssueMPLDAL().TransIssueMPLInsert(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string[] TransIssueMPLUpdate(TransferMPLIssueVM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new TransferIssueMPLDAL().TransIssueMPLUpdate(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<TransferMPLIssueVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, string transactionType = null, string Orderby = "Y", string SelectTop = "100")
        {
            try
            {
                return new TransferIssueMPLDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM,null,transactionType, Orderby, SelectTop);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TransferMPLIssueDetailVM> SearchTransIssueMPLDetailList(string transferMPLIssueId)
        {
            try
            {
                return new TransferIssueMPLDAL().SearchTransIssueMPLDetailList(transferMPLIssueId, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] TransferMPLIssuePost(TransferMPLIssueVM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new TransferIssueMPLDAL().TransferMPLIssuePost(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



    }
}
