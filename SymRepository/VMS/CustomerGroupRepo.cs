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
    public class CustomerGroupRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public CustomerGroupRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
        public CustomerGroupRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        //private SymOrdinary.ShampanIdentity identity;

        //public CustomerGroupRepo(SymOrdinary.ShampanIdentity identity)
        //{
        //    // TODO: Complete member initialization
        //    this.identity = identity;
        //}

        public CustomerGroupRepo()
        {
            // TODO: Complete member initialization
        }
        public string[] InsertToCustomerGroupNew(CustomerGroupVM vm)
        {
            try
            {
                return new CustomerGroupDAL().InsertToCustomerGroupNew(vm,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateToCustomerGroupNew(CustomerGroupVM vm)
        {
            try
            {
                return new CustomerGroupDAL().UpdateToCustomerGroupNew(vm,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<CustomerGroupVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new CustomerGroupDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CustomerGroupVM> DropDown()
        {
            try
            {
                return new CustomerGroupDAL().DropDown(connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] Delete(CustomerGroupVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new CustomerGroupDAL().Delete(vm, ids, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
