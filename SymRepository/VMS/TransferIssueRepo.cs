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
    public class TransferIssueRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public TransferIssueRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public TransferIssueRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public IEnumerable<object> GetTransferIssueColumn()
        {
            try
            {
                string[] columnName = new string[] { "Transfer Issue No", "Reference No", "Import ID Excel", "VehicleNo" };
                IEnumerable<object> enumList = from e in columnName select new { Id = e.ToString().Replace(" ", ""), Name = e.ToString() };
                return enumList;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public List<TransferIssueVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new TransferIssueDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TransferIssueDetailVM> SelectDetail(string TransferIssueNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new TransferIssueDAL().SelectDetail(TransferIssueNo, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Insert(TransferIssueVM Master, List<TransferIssueDetailVM> Details, SqlTransaction transaction=null, SqlConnection currConn=null)
        {
            try
            {
                return new TransferIssueDAL().Insert(Master, Details, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] Update(TransferIssueVM Master, List<TransferIssueDetailVM> Details)
        {
            try
            {
                return new TransferIssueDAL().Update(Master, Details, connVM);
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        public string[] Post(TransferIssueVM Master)
        {
            try
            {
                return new TransferIssueDAL().PostTransfer(Master, null, null, connVM);
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
                return new TransferIssueDAL().MultiplePost(Ids, connVM,null);

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
                return new TransferIssueDAL().GetExcelDataWeb(Ids,null,null,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable SearchTransfer(TransferVM vm)
        {
            try
            {
                return new TransferIssueDAL().SearchTransfer(vm, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet TransferMovement(string ItemNo, string FDate, string TDate, int BranchId = 0, bool Summery = false)
        {
            try
            {
                return new TransferIssueDAL().TransferMovement(ItemNo, FDate, TDate, BranchId, null, null, Summery, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] ImportExcelFile(TransferIssueVM vm)
        {
            try
            {

                return new TransferIssueDAL().ImportExcelFile(vm, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public MISExcelVM DownloadMIS_TransferIssueReport(string IssueNo, string IssueDateFrom, string IssueDateTo, string TType, int BranchId = 0
            , int TransferTo = 0, string ShiftId = "0")
        {
            try
            {
                TransferIssueDAL saleDalObj = new TransferIssueDAL();
                return saleDalObj.TransferIssueMISExcelDownload(IssueNo, IssueDateFrom, IssueDateTo, TType, BranchId , TransferTo, connVM);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

    }
}
