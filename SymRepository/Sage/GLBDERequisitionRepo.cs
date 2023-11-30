using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace SymRepository.Sage
{
    public class GLBDERequisitionRepo
    {
        #region Charts
        //FormA////==================Date Range(Multiple Month) - Single DocumentType Amount=================Y:Amount - X:Month
        public List<GLBDERequisitionFormAVM> SelectChart1(GLReportVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GLBDERequisitionDAL().SelectChart1(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        ////==================Date Range(Multiple Month) - All DocumentType Amount=================Y:Amount - X:DocumentType
        public List<GLBDERequisitionFormAVM> SelectChart2(GLReportVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GLBDERequisitionDAL().SelectChart2(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        public List<GLBDERequisitionVM> DropDown(int branchId = 0)
        {
            try
            {
                return new GLBDERequisitionDAL().DropDown(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDERequisitionVM> SelectAllSelfApprove(int Id = 0, int UserId = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionDAL().SelectAllSelfApprove(Id, UserId, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDERequisitionVM> SelectAllPosted(int Id = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionDAL().SelectAllPosted(Id, conditionField, conditionValue);
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
                return new GLBDERequisitionDAL().SelectAll(Id, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLBDERequisitionVM vm, List<HttpPostedFileBase> fileUpload)
        {
            try
            {
                return new GLBDERequisitionDAL().Insert(vm, fileUpload);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(GLBDERequisitionVM vm, List<HttpPostedFileBase> fileUpload)
        {
            try
            {
                return new GLBDERequisitionDAL().Update(vm, fileUpload);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(GLBDERequisitionVM vm, string[] ids)
        {
            try
            {
                return new GLBDERequisitionDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Post(GLBDERequisitionVM vm, string[] ids)
        {
            try
            {
                return new GLBDERequisitionDAL().Post(vm, ids);
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
                return new GLBDERequisitionDAL().RemoveFile(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] ApproveReject(GLBDERequisitionVM vm, string[] ids)
        {
            try
            {
                return new GLBDERequisitionDAL().ApproveReject(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Audit(GLBDERequisitionVM vm, string[] ids)
        {
            try
            {
                return new GLBDERequisitionDAL().Audit(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(GLBDERequisitionVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionDAL().Report(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable BDEmployeeReport(GLBDERequisitionVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionDAL().BDEmployeeReport(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable YearAgentCEReport(GLBDERequisitionVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionDAL().YearAgentCEReport(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable YearBDEExpenseReport(GLBDERequisitionVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionDAL().YearBDEExpenseReport(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable MonthTotalExpenseReport(GLBDERequisitionVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionDAL().MonthTotalExpenseReport(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable OpeningBDERequesition(string TransactionId)
        {
            try
            {
                return new GLBDERequisitionDAL().OpeningBDERequesition(TransactionId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
