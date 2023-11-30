using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace SymRepository.Sage
{
    public class GLPettyCashRequisitionRepo
    {
        public List<GLPettyCashRequisitionVM> DropDown(int branchId = 0)
        {
            try
            {
                return new GLPettyCashRequisitionDAL().DropDown(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLPettyCashRequisitionVM> SelectAllSelfApprove(int Id = 0, int UserId = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLPettyCashRequisitionDAL().SelectAllSelfApprove(Id, UserId, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLPettyCashRequisitionVM> SelectAllPosted(int Id = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLPettyCashRequisitionDAL().SelectAllPosted(Id, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLPettyCashRequisitionVM> SelectAll(int Id = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLPettyCashRequisitionDAL().SelectAll(Id, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string ExistCheckCommissionBillNo(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GLPettyCashRequisitionDAL().ExistCheckCommissionBillNo(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Insert(GLPettyCashRequisitionVM vm, List<HttpPostedFileBase> fileUpload)
        {
            try
            {
                return new GLPettyCashRequisitionDAL().Insert(vm, fileUpload);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(GLPettyCashRequisitionVM vm, List<HttpPostedFileBase> fileUpload)
        {
            try
            {
                return new GLPettyCashRequisitionDAL().Update(vm, fileUpload);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(GLPettyCashRequisitionVM vm, string[] ids)
        {
            try
            {
                return new GLPettyCashRequisitionDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Post(GLPettyCashRequisitionVM vm, string[] ids)
        {
            try
            {
                return new GLPettyCashRequisitionDAL().Post(vm, ids);
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
                return new GLPettyCashRequisitionDAL().RemoveFile(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] ApproveReject(GLPettyCashRequisitionVM vm, string[] ids)
        {
            try
            {
                return new GLPettyCashRequisitionDAL().ApproveReject(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Audit(GLPettyCashRequisitionVM vm, string[] ids)
        {
            try
            {
                return new GLPettyCashRequisitionDAL().Audit(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report(GLPettyCashRequisitionVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLPettyCashRequisitionDAL().Report(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet OpeningPettyCashRequesition(string TransactionId)
        {
            try
            {
                return new GLPettyCashRequisitionDAL().OpeningPettyCashRequesition(TransactionId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
