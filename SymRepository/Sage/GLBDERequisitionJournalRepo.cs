using SymServices.Sage;
using SymViewModel.Sage;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;


namespace SymRepository.Sage
{
    public class GLBDERequisitionJournalRepo
    {
        public List<GLBDERequisitionJournalVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GLBDERequisitionJournalDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(GLBDERequisitionJournalVM vm, string[] ids)
        {
            try
            {
                return new GLBDERequisitionJournalDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] CreateJournal(GLBDERequisitionJournalVM vm, string[] ids)
        {
            try
            {
                return new GLBDERequisitionJournalDAL().CreateJournal(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
