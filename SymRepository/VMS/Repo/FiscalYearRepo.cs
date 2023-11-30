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
    public class FiscalYearRepo
    {
        public List<FiscalYearVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new FiscalYearDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] FiscalYearUpdate(List<FiscalYearVM> Details,string modifiedBy) {
            return new FiscalYearDAL().FiscalYearUpdate(Details, modifiedBy);
        }

        public string[] FiscalYearInsert(List<FiscalYearVM> Details)
        {
            return new FiscalYearDAL().FiscalYearInsert(Details);
        }

        public List<FiscalYearVM> DropDownAll() {
            return new FiscalYearDAL().DropDownAll();
        }

        public int LockChek() 
        {
            return new FiscalYearDAL().LockChek();
        }

        public string MaxDate() 
        {
            return new FiscalYearDAL().MaxDate();
        }

    }
}
