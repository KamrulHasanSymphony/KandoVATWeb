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
    public class TransferReceiveMPLRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public TransferReceiveMPLRepo()
        {
            connVM = null;
        }
        public TransferReceiveMPLRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
        public TransferReceiveMPLRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }


        public string[] TransReceiveMPLInsert(TransferMPLReceiveVM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new TransferReceiveMPLDAL().TransReceiveMPLInsert(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string[] TransReceiveMPLUpdate(TransferMPLReceiveVM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new TransferReceiveMPLDAL().TransReceiveMPLUpdate(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<TransferMPLReceiveVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, string transactionType = null, string Orderby = "Y", string SelectTop = "100")
        {
            try
            {
                return new TransferReceiveMPLDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM, null, transactionType, Orderby, SelectTop);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TransferMPLReceiveDetailVM> SearchTransReceiveMPLDetailList(string transferMPLReceiveId)
        {
            try
            {
                return new TransferReceiveMPLDAL().SearchTransReceiveMPLDetailList(transferMPLReceiveId, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        // Get Transfer Issue Data

        public List<TransferMPLIssueVM> ReceiveIndex(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, string transactionType = null, string Orderby = "Y", string SelectTop = "100")
        {
            try
            {
                return new TransferReceiveMPLDAL().ReceiveIndex(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM, null, transactionType, Orderby, SelectTop);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TransferMPLReceiveDetailVM> SearchTransIssueMPLDetailList(string transferMPLReceiveId)
        {
            try
            {
                return new TransferReceiveMPLDAL().SearchTransIssueMPLDetailList(transferMPLReceiveId, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TransferMPLReceiveVM> GetTransIssueAll(string Id)
        {
            try
            {
                return new TransferReceiveMPLDAL().GetTransIssueAll(Id, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] TransferMPLReceivePost(TransferMPLReceiveVM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new TransferReceiveMPLDAL().TransferMPLReceivePost(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string[] MultipleReceiveSave(string[] Ids, string transactionType, int BranchId, string ReceiveDateTime, string CurrentUser = "", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new TransferReceiveMPLDAL().MultipleReceiveSave(Ids, transactionType, BranchId, ReceiveDateTime, CurrentUser, null, null, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }





    }
}
