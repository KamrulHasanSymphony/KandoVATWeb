using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace SymRepository.Sage
{
    public class GLBDEExpenseRepo
    {
        public List<GLBDEExpenseVM> DropDown(int branchId = 0)
        {
            try
            {
                return new GLBDEExpenseDAL().DropDown(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDEExpenseVM> SelectAllSelfApprove(int Id = 0, int UserId = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDEExpenseDAL().SelectAllSelfApprove(Id, UserId, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<GLBDEExpenseVM> SelectAllPosted(int Id = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDEExpenseDAL().SelectAllPosted(Id, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDEExpenseVM> SelectAll(int Id = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDEExpenseDAL().SelectAll(Id, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLBDEExpenseVM vm, List<HttpPostedFileBase> fileUpload)
        {
            try
            {
                return new GLBDEExpenseDAL().Insert(vm, fileUpload);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(GLBDEExpenseVM vm, List<HttpPostedFileBase> fileUpload)
        {
            try
            {
                return new GLBDEExpenseDAL().Update(vm, fileUpload);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(GLBDEExpenseVM vm, string[] ids)
        {
            try
            {
                return new GLBDEExpenseDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Post(GLBDEExpenseVM vm, string[] ids)
        {
            try
            {
                return new GLBDEExpenseDAL().Post(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] RemoveFile(string id)
        {
            try
            {
                return new GLBDEExpenseDAL().RemoveFile(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] ApproveReject(GLBDEExpenseVM vm, string[] ids)
        {
            try
            {
                return new GLBDEExpenseDAL().ApproveReject(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Audit(GLBDEExpenseVM vm, string[] ids)
        {
            try
            {
                return new GLBDEExpenseDAL().Audit(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

      

    }
}
