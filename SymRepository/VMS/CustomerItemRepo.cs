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
    public class CustomerItemRepo
    {

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public CustomerItemRepo()
        {
             connVM = null;
        }
        public CustomerItemRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public CustomerItemRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }

        public string[] CustomerItemInsert(CustomerItemVM Master, SqlTransaction transaction = null, SqlConnection currConn = null)
        {
            return new CustomerItemDAL().CustomerItemInsert(Master, Master.Details, transaction, currConn, connVM);
        }

        public string[] CustomerItemUpdate(CustomerItemVM Master, SqlTransaction transaction = null, SqlConnection currConn = null)
        {
            return new CustomerItemDAL().CustomerItemUpdate(Master, Master.Details, currConn, transaction, connVM);
        }

        public string[] CustomerItemPost(CustomerItemVM Master, string UserId = "")
        {
            return new CustomerItemDAL().CustomerItemPost(Master, Master.Details, null, null, connVM, UserId);

        }

        public List<CustomerItemVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            return new CustomerItemDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
        }

        public List<CustomerItemVM> SelectCustomerList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            return new CustomerItemDAL().SelectCustomerList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
        }

        public List<CustomerItemDetailsVM> SelectAllDetail(string CustomerID, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            return new CustomerItemDAL().SelectAllDetail(CustomerID, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
        }

        public List<CustomerItemVM> SelectAllCustomerList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            return new CustomerItemDAL().SelectAllCustomerList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
        }

        public ResultVM BillProcess(CustomerItemVM Master, IntegrationParam paramVM, SqlTransaction transaction = null, SqlConnection currConn = null)
        {
            return new CustomerItemDAL().BillProcess(Master, paramVM, transaction, currConn, connVM);

        }

        public DataTable GetExcelDataWeb(List<string> Ids)
        {
            try
            {
                return new CustomerItemDAL().SelectCustomer(0, null, null, null, null, connVM, Ids);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable SelectAllCustomer_Export(List<string> Ids)
        {
            try
            {
                return new CustomerItemDAL().SelectAllCustomer_Export(Ids, null, null, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM CustomerBillProcess(CustomerBillProcessVM Master, SqlTransaction transaction = null, SqlConnection currConn = null)
        {
            return new CustomerItemDAL().AddCustomerBillProcess(Master, currConn, transaction, connVM);

        }

        public List<CustomerBillProcessVM> SelectAllCustomerBillProcessList(string CustomerId = "", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            return new CustomerItemDAL().SelectAllCustomerBillProcessList(CustomerId, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
        }



    }
}
