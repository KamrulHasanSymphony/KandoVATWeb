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
    public class CurrencyRepo
    {
        public string[] InsertToCurrency(CurrencyVM vm)
        {
            try
            {
                return new CurrenciesDAL().InsertToCurrency(vm);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateCurrency(CurrencyVM vm)
        {
            try
            {
                return new CurrenciesDAL().UpdateCurrency(vm);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] Delete(CurrencyVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new CurrenciesDAL().Delete(vm, ids, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<CurrencyVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new CurrenciesDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<CurrencyVM> DropDown()
        {
            try
            {
                return new CurrenciesDAL().DropDown();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
