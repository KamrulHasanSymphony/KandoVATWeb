using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VATServer.Library;
using VATViewModel.DTOs;

namespace SymRepository.VMS
{
    public class TransferReceiveRepo
    {
         private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

         public TransferReceiveRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

         public TransferReceiveRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public IEnumerable<object> GetTransferReceiveColumn()
        {
            try
            {
                string[] columnName = new string[] { "Transfer Receive No", "Transfer From No", "Reference No" };
                IEnumerable<object> enumList = from e in columnName select new { Id = e.ToString().Replace(" ", ""), Name = e.ToString() };
                return enumList;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public List<TransferReceiveVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new TransferReceiveDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TransferReceiveDetailVM> SelectDetail(string TransferReceiveNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new TransferReceiveDAL().SelectDetail(TransferReceiveNo, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Insert(TransferReceiveVM Master, List<TransferReceiveDetailVM> Details, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new TransferReceiveDAL().Insert(Master,Details,transaction, currConn,connVM);
            }
            
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] Update(TransferReceiveVM Master, List<TransferReceiveDetailVM> Details)
        {
            try
            {
                return new TransferReceiveDAL().Update(Master,Details,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Post(TransferReceiveVM Master)
        {
            try
            {
                return new TransferReceiveDAL().Post(Master,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] MultiplePost(string[] Ids)
        {
            try
            {
                return new TransferReceiveDAL().MultiplePost(Ids,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string[] MultipleSave(string[] Ids, string transactionType, int BranchId, string TransactionDateTime, string CurrentUser = "", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new TransferReceiveDAL().MultipleSave(Ids, transactionType, BranchId, TransactionDateTime, CurrentUser, null, null, connVM);

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
                return new TransferReceiveDAL().GetExcelData(Ids, null, null, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
