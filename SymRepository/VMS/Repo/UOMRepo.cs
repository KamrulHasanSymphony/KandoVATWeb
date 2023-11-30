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
    public class UOMRepo
    {
        public string[] InsertToUOM(UOMConversionVM vm)
        {
            try
            {
                return new UOMDAL().InsertToUOMNew(vm);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateUOM(UOMConversionVM vm)
        {
            try
            {
                return new UOMDAL().UpdateUOM(vm);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] Delete(UOMConversionVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new UOMDAL().Delete(vm, ids, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<UOMConversionVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new UOMDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }
}
