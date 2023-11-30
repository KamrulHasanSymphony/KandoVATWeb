using SymServices.Sage;
using SymServices.Common;
using SymViewModel.Sage;

using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Sage
{
    public class GLEmployeeRepo
    {
        #region Methods
        public List<string> AutocompleteName(string term)
        {
            try
            {
                return new GLEmployeeDAL().AutocompleteName(term);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLEmployeeVM> DropDown(int BranchId)
        {
            try
            {
                return new GLEmployeeDAL().DropDown(BranchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLEmployeeVM> SelectAll(int Id = 0)
        {
            try
            {
                return new GLEmployeeDAL().SelectAll(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLEmployeeVM vm)
        {
            try
            {
                return new GLEmployeeDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(GLEmployeeVM vm)
        {
            try
            {
                return new GLEmployeeDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(GLEmployeeVM vm, string[] ids)
        {
            try
            {
                return new GLEmployeeDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(GLEmployeeVM vm)
        {
            try
            {
                return new GLEmployeeDAL().Report(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
