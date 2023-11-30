using SymServices.Enum;
using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Enum
{
    public class EnumColumnListRepo
    {
        public List<EnumColumnListVM> DropDown(string tableName = "")
        {
            try
            {
                return new EnumColumnListDAL().DropDown(tableName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
