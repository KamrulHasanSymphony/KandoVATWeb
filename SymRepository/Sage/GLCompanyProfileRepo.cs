using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Sage
{
    public class GLCompanyProfileRepo
    {
#region Methods
        public List<GLCompanyProfileVM> DropDown()
        {
            try
            {
                return new GLCompanyProfileDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLCompanyProfileVM> SelectAll(int Id=0)
        {
            try
            {
                return new GLCompanyProfileDAL().SelectAll(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLCompanyProfileVM vm)
        {
            try
            {
                return new GLCompanyProfileDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(GLCompanyProfileVM vm)
        {
            try
            {
                return new GLCompanyProfileDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(GLCompanyProfileVM vm, string[] ids)
        {
            try
            {
                return new GLCompanyProfileDAL().Delete(vm,ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
