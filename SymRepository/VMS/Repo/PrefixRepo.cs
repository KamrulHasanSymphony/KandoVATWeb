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
    public class PrefixRepo
    {
        public List<CodeVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PrefixDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] PrefixDataUpdate(CodeVM vm, SqlConnection VcurrConn=null, SqlTransaction Vtransaction=null) {
            return new PrefixDAL().PrefixDataUpdate(vm, VcurrConn, Vtransaction);
        }

        public List<CodeVM> DropDownAll() {
            return new PrefixDAL().DropDownAll();
        }

    }
}
