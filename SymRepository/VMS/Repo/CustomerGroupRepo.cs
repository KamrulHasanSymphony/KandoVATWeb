using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymServices.VMS.Library;
using SymViewModel.VMS.DTOs;
using System.Data;
using System.Data.SqlClient;

namespace SymRepository.VMS.Repo
{
    public class CustomerGroupRepo
    {
        public string[] InsertToCustomerGroupNew(CustomerGroupVM vm)
        {
            try
            {
                return new CustomerGroupDAL().InsertToCustomerGroupNew(vm);
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
                return new CustomerGroupDAL().UpdateToCustomerGroupNew(vm);
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
                return new CustomerGroupDAL().SelectAll(Id, conditionFields, conditionValues);
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
                return new CustomerGroupDAL().DropDown();
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
                return new CustomerGroupDAL().Delete(vm, ids, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
