using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using SymServices.Common;
using SymViewModel.VMS.DTOs;
using SymServices.VMS.Library;

namespace SymRepository.VMS.Repo
{
    public class CommonRepo
    {
        public string GetTargetId(string tableName, string columnName, string currentId, string btn)
        {
            try
            {
                return new CommonDAL().GetTargetId(tableName, columnName, currentId, btn);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<IdName> IdNameDropdown(string tableName, string Id, string Name, string AllData, string  code)
        {
            try
            {
                return new CommonDAL().IdNameDropdown(tableName, Id, Name, AllData, code);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<IdName> IdNameDropdownOverhead(string Id, string Name, string AllData, string code)
        {
            try
            {
                return new CommonDAL().IdNameDropdownOverhead(Id, Name, AllData, code);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
