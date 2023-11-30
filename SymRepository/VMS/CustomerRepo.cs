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
    public class CustomerRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public CustomerRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public CustomerRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public List<CustomerVM> DropDown()
        {
            try
            {
                return new CustomerDAL().DropDown(connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CustomerVM> DropDownByCustomerID(int CustomerID=0)
        {
            try
            {
                return new CustomerDAL().DropDownByCustomerID(CustomerID, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<CustomerVM> DropDownByGroup(string groupId)
        {
            try
            {
                return new CustomerDAL().DropDownByGroup(groupId, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertToCustomerNew(CustomerVM vm)
        {
            try
            {
                return new CustomerDAL().InsertToCustomerNew(vm,false,null,null,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        public ResultVM IntegrationSyncCustomers(IntegrationParam vm)
        {
            try
            {
                //return new ProductDAL().InsertToProduct(vm, Trackings, ItemType);
                return new ImportDAL().CustomerSync_ACI(vm,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public string[] UpdateToCustomerNew(CustomerVM vm)
        {
            try
            {
                return new CustomerDAL().UpdateToCustomerNew(vm,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<CustomerVM> SelectAll(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new CustomerDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(CustomerVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new CustomerDAL().Delete(vm, ids, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<object> GetCustomerColumn()
        {
            try
            {
                string[] columnName = new string[] { "Customer Code", "Customer Name", "City", "Contact Person", "VAT Registration No" };
                IEnumerable<object> enumList = from e in columnName select new { Id = e.ToString().Replace(" ", ""), Name = e.ToString() };
                return enumList;
            }
            catch (Exception)
            {
                
                throw;
            }
        }


        public string[] InsertToCustomerAddress(CustomerAddressVM vm)
        {
            try
            {
                return new CustomerDAL().InsertToCustomerAddress(vm, null, null, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] UpdateToCustomerAddress(CustomerAddressVM vm)
        {
            try
            {
                return new CustomerDAL().UpdateToCustomerAddress(vm, null, null, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SearchCustomerAddress(string CustomerID)
        {
            try
            {
                return new CustomerDAL().SearchCustomerAddress(CustomerID, "", "", connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] DeleteCustomerAddress(string CustomerId, string id)
        {
            try
            {
                return new CustomerDAL().DeleteCustomerAddress(CustomerId, id, null, null, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        public string[] InsertToCustomerDiscountNew(CustomerDiscountVM vm)
        {
            try
            {
                return new CustomerDiscountDAL().InsertToCustomerDiscountNew(vm, false, null, null, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] UpdateToCustomerDiscountNew(CustomerDiscountVM vm)
        {
            try
            {
                return new CustomerDiscountDAL().UpdateToCustomerDiscountNew(vm, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] DeleteCustomerDiscount(string CustomerId,string id)
        {
            try
            {
                return new CustomerDiscountDAL().DeleteCustomerDiscount(CustomerId, id, null, null, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SearchCustomerDiscount(string CustomerID)
        {
            try
            {
                return new CustomerDiscountDAL().SearchCustomerDiscount(CustomerID, "", "", connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<string> Autocomplete(string term)
        {
            try
            {
                return new CustomerDAL().AutocompleteCustomer(term, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetExcelData(List<string> Ids)
        {
            try
            {
                return new CustomerDAL().GetExcelData(Ids, null, null, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetExcelAddress(List<string> Ids)
        {
            try
            {
                return new CustomerDAL().GetExcelAddress(Ids, null, null, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string[] ImportExcelIntegrationFile(IntegrationParam paramVM)
        {
            try
            {
                CustomerDAL customerDal = new CustomerDAL();

                return customerDal.ImportExcelIntegrationFile(paramVM, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
