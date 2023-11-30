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
    public class VendorGroupRepo
    {
        public string[] InsertToVendorGroup(VendorGroupVM vm)
        {
            try
            {
                return new VendorGroupDAL().InsertToVendorGroup(vm);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateToVendorGroup(VendorGroupVM vm)
        {
            try
            {
                return new VendorGroupDAL().UpdateToVendorGroup(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<VendorGroupVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new VendorGroupDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(VendorGroupVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new VendorGroupDAL().Delete(vm, ids, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<VendorGroupVM> DropDown()
        {
            try
            {
                return new VendorGroupDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
