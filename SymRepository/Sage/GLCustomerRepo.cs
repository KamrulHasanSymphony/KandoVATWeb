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
    public class GLCustomerRepo
    {
        #region Methods
        public List<string> AutocompleteName(string term)
        {
            try
            {
                return new GLCustomerDAL().AutocompleteName(term);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLCustomerVM> DropDown()
        {
            try
            {
                return new GLCustomerDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLCustomerVM> SelectAll(int Id = 0)
        {
            try
            {
                return new GLCustomerDAL().SelectAll(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLCustomerVM vm)
        {
            try
            {
                return new GLCustomerDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(GLCustomerVM vm)
        {
            try
            {
                return new GLCustomerDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(GLCustomerVM vm, string[] ids)
        {
            try
            {
                return new GLCustomerDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(GLCustomerVM vm)
        {
            try
            {
                return new GLCustomerDAL().Report(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
