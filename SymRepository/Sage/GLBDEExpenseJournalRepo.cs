using SymServices.Sage;
using SymViewModel.Sage;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;


namespace SymRepository.Sage
{
    public class GLBDEExpenseJournalRepo
    {
        public List<GLBDEExpenseJournalVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GLBDEExpenseJournalDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(GLBDEExpenseJournalVM vm, string[] ids)
        {
            try
            {
                return new GLBDEExpenseJournalDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] CreateJournal(GLBDEExpenseJournalVM vm, string[] ids)
        {
            try
            {
                return new GLBDEExpenseJournalDAL().CreateJournal(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable VoucherReportBDEExpense(GLBDEExpenseJournalDetailVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDEExpenseJournalDAL().VoucherReportBDEExpense(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable VoucherReportBDERequisition(GLBDEExpenseJournalDetailVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDEExpenseJournalDAL().VoucherReportBDERequisition(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
