using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using VATServer.Library;
using VATViewModel.DTOs;
using VMSAPI;

namespace SymRepository.VMS
{
    public class ProductTransferRepo
    {

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public ProductTransferRepo()
        {
             connVM = null;
        }
        public ProductTransferRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public ProductTransferRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }

        public string[] Insert(ProductTransfersVM Master, SqlTransaction transaction = null, SqlConnection currConn = null,string UserId = "") 
        {
            return new ProductTransferDAL().Insert(Master, Master.Details, null, null, connVM, UserId);
        }

        public string[] Update(ProductTransfersVM Master, string UserId = "")
        {
            return new ProductTransferDAL().Update(Master, Master.Details, connVM, UserId);
        }

        public string[] Post(ProductTransfersVM Master)
        {
            return new ProductTransferDAL().PostTransfer(Master, null, null, connVM);

        }

        public List<ProductTransfersVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM likeVM = null) 
        {
            return new ProductTransferDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, likeVM, connVM);
        }

        public List<ProductTransfersDetailVM> SearchTransferDetail(string TransferNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            return new ProductTransferDAL().SelectTransferDetail(TransferNo, null, null, null, null,connVM);
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

        #region API

        public string SaveIssue(string xml)
        {
            try
            {
                IssueAPI api = new IssueAPI();

                var result = api.SaveIssue(xml);

                return result;
            }
            catch (Exception e)
            {

                throw e;
            }
        }


        #endregion
    }
}
