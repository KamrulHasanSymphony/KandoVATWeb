using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace SymRepository.Sage
{
    public class GLBDERequisitionPaidRepo
    {
        
        public List<GLBDERequisitionPaidVM> DropDown(int branchId = 0)
        {
            try
            {
                return new GLBDERequisitionPaidDAL().DropDown(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDERequisitionPaidVM> SelectAllSelfApprove(int Id = 0, int UserId = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionPaidDAL().SelectAllSelfApprove(Id, UserId, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDERequisitionPaidVM> SelectAllPosted(int Id = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionPaidDAL().SelectAllPosted(Id, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDERequisitionVM> SelectAll(int Id = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionPaidDAL().SelectAll(Id, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLBDERequisitionVM vm)
        {
            try
            {
                return new GLBDERequisitionPaidDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(GLBDERequisitionVM vm)
        {
            try
            {
                return new GLBDERequisitionPaidDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(GLBDERequisitionPaidVM vm, string[] ids)
        {
            try
            {
                return new GLBDERequisitionPaidDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Post(GLBDERequisitionPaidVM vm, string[] ids)
        {
            try
            {
                return new GLBDERequisitionPaidDAL().Post(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] ApproveReject(GLBDERequisitionPaidVM vm, string[] ids)
        {
            try
            {
                return new GLBDERequisitionPaidDAL().ApproveReject(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Audit(GLBDERequisitionPaidVM vm, string[] ids)
        {
            try
            {
                return new GLBDERequisitionPaidDAL().Audit(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(GLBDERequisitionPaidVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionPaidDAL().Report(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
