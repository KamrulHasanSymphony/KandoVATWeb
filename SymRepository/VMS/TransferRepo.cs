using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Library;
using VATViewModel.DTOs;
using System.Data.SqlClient;
using VMSAPI;
using SymOrdinary;
using System.Web;

namespace SymRepository.VMS
{
    public class TransferRepo
    {
         private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

         public TransferRepo()
         {
            connVM = null;
         }
         public TransferRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

         public TransferRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public IEnumerable<object> GetTrasferColumn()
        {
            try
            {
                string[] columnName = new string[] { "Transfer From No", "Serial No", "Reference No" };
                IEnumerable<object> enumList = from e in columnName select new { Id = e.ToString().Replace(" ", ""), Name = e.ToString() };
                return enumList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<TransferVM> SelectAll(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new TransferDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public List<TransferDetailVM> SelectDetail(string TransferNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new TransferDAL().SelectDetail(TransferNo, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        #region API

        public string SaveTransferIssue(string xml)
        {
            try
            {
                TransferIssueAPI api = new TransferIssueAPI();

                var result = api.SaveTransferIssue(xml);

                return result;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public string GetFileName(string xml)
        {
            try
            {
                TransferIssueAPI api = new TransferIssueAPI();

                var result = api.GetFileName(xml);

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
