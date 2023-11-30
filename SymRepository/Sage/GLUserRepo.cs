using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Web;

namespace SymRepository.Sage
{
    public class GLUserRepo
    {
        public List<GLUserVM> DropDown(string tType = null, int branchId = 0)
        {
            try
            {
                return new GLUserDAL().DropDown(tType, branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLUserVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GLUserDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLUserVM vm, HttpPostedFileBase PhotoFile, HttpPostedFileBase SignatureFile)
        {
            try
            {
                return new GLUserDAL().Insert(vm, PhotoFile, SignatureFile);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(GLUserVM vm, HttpPostedFileBase PhotoFile, HttpPostedFileBase SignatureFile)
        {
            try
            {
                return new GLUserDAL().Update(vm, PhotoFile, SignatureFile);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(GLUserVM vm, string[] ids)
        {
            try
            {
                return new GLUserDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] ChangePassword(GLUserVM vm)
        {
            try
            {
                return new GLUserDAL().ChangePassword(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] ForgetPassword(GLUserVM vm)
        {
            try
            {
                return new GLUserDAL().ForgetPassword(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
