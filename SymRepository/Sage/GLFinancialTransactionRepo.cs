using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace SymRepository.Sage
{
    public class GLFinancialTransactionRepo
    {
        public List<GLFinancialTransactionVM> DropDown(int branchId = 0)
        {
            try
            {
                return new GLFinancialTransactionDAL().DropDown(branchId);
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
                return new GLFinancialTransactionDAL().ExistCheckCommissionBillNo(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

  
        public List<GLFinancialTransactionVM> SelectAllSelfApprove(int Id = 0, int UserId = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLFinancialTransactionDAL().SelectAllSelfApprove(Id, UserId, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<GLFinancialTransactionVM> SelectAllPosted(int Id = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLFinancialTransactionDAL().SelectAllPosted(Id, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public List<GLFinancialTransactionVM> SelectAll(int Id = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLFinancialTransactionDAL().SelectAll(Id, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLFinancialTransactionVM vm, List<HttpPostedFileBase> fileUpload)
        {
            try
            {
                return new GLFinancialTransactionDAL().Insert(vm, fileUpload);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(GLFinancialTransactionVM vm, List<HttpPostedFileBase> fileUpload)
        {
            try
            {
                return new GLFinancialTransactionDAL().Update(vm, fileUpload);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(GLFinancialTransactionVM vm, string[] ids)
        {
            try
            {
                return new GLFinancialTransactionDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Post(GLFinancialTransactionVM vm, string[] ids)
        {
            try
            {
                return new GLFinancialTransactionDAL().Post(vm, ids);
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
                return new GLFinancialTransactionDAL().RemoveFile(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] ApproveReject(GLFinancialTransactionVM vm, string[] ids)
        {
            try
            {
                return new GLFinancialTransactionDAL().ApproveReject(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Audit(GLFinancialTransactionVM vm, string[] ids)
        {
            try
            {
                return new GLFinancialTransactionDAL().Audit(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable PCStatementReport(GLFinancialTransactionVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLFinancialTransactionDAL().PCReport(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    
        public DataTable PCExpenseStatementReport(GLFinancialTransactionVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLFinancialTransactionDAL().PCExpenseReport(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable YearBranchExpenseStatementReport(GLFinancialTransactionVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLFinancialTransactionDAL().YearBranchExpenseReport(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
