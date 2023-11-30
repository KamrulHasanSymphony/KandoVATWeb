using SymServices.Sage;
using SymViewModel.Sage;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;


namespace SymRepository.Sage
{
    public class GLPCJournalRepo
    {
        public List<GLPCJournalVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GLPCJournalDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] Delete(GLPCJournalVM vm, string[] ids)
        {
            try
            {
                return new GLPCJournalDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] CreateJournal(GLPCJournalVM vm, string[] ids)
        {
            try
            {
                return new GLPCJournalDAL().CreateJournal(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public string[] Update(GLPCJournalVM vm)
        //{
        //    try
        //    {
        //        return new GLPCJournalDAL().Update(vm);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public List<GLPCJournalVM> SelectAllToPost(string Id = "")
        {
            try
            {
                return new GLPCJournalDAL().SelectAllToPost(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Post(string[] ids, ShampanIdentityVM auditVM)
        {
            try
            {
                return new GLPCJournalDAL().Post(ids, auditVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GLPCJournalReport(string Id = "")
        {
            try
            {
                return new GLPCJournalDAL().GLPCJournalReport(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable VoucherReport(GLPCJournalDetailVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLPCJournalDAL().VoucherReport(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable VoucherReportPettyCashRequisitionJournal(GLPCJournalDetailVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLPCJournalDAL().VoucherReportPettyCashRequisitionJournal(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
