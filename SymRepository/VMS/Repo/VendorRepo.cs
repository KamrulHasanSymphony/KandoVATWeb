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
    public class VendorRepo
    {
        public List<VendorVM> DropDown() {
            try
            {
                return new VendorDAL().DropDown();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string[] InsertToVendorNewSQL(VendorVM vm)
        {
            try
            {
                return new VendorDAL().InsertToVendorNewSQL(vm);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateVendorNewSQL(VendorVM vm)
        {
            try
            {
                return new VendorDAL().UpdateVendorNewSQL(vm);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public List<VendorVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new VendorDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] Delete(VendorVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new VendorDAL().Delete(vm, ids, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
