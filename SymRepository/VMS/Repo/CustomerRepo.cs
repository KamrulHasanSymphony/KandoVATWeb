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
    public class CustomerRepo
    {
        public List<CustomerVM> DropDown()
        {
            try
            {
                return new CustomerDAL().DropDown();
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
                return new CustomerDAL().InsertToCustomerNew(vm);
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
                return new CustomerDAL().UpdateToCustomerNew(vm);
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
                return new CustomerDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
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
                return new CustomerDAL().Delete(vm, ids, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
