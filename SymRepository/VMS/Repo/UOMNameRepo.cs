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
    public class UOMNameRepo
    {
        public string[] InsertToUOMName(UOMNameVM vm)
        {
            try
            {
                return new UOMNameDAL().InsertToUOMName(vm);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateUOMName(UOMNameVM vm)
        {
            try
            {
                return new UOMNameDAL().UpdateUOMName(vm);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] Delete(UOMNameVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new UOMNameDAL().Delete(vm, ids, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<UOMNameVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new UOMNameDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<UOMNameVM> DropDown()
        {
            try
            {
                return new UOMNameDAL().DropDown();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
