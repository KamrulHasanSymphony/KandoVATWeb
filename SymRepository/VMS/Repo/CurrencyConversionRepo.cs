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
    public class CurrencyConversionRepo
    {
        public string[] InsertToCurrencyConversion(CurrencyConversionVM vm)
        {
            try
            {
                return new CurrencyConversionDAL().InsertToCurrencyConversion(vm);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateCurrencyConversion(CurrencyConversionVM vm)
        {
            try
            {
                return new CurrencyConversionDAL().UpdateCurrencyConversion(vm);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] Delete(CurrencyConversionVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new CurrencyConversionDAL().Delete(vm, ids, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<CurrencyConversionVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new CurrencyConversionDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
