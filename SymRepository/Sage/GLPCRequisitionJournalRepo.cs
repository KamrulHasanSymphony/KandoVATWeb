using SymServices.Sage;
using SymViewModel.Sage;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;


namespace SymRepository.Sage
{
    public class GLPCRequisitionJournalRepo
    {
        public List<GLPCRequisitionJournalVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GLPCRequisitionJournalDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] Delete(GLPCRequisitionJournalVM vm, string[] ids)
        {
            try
            {
                return new GLPCRequisitionJournalDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] CreateJournal(GLPCRequisitionJournalVM vm, string[] ids)
        {
            try
            {
                return new GLPCRequisitionJournalDAL().CreateJournal(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public string[] Update(GLPCRequisitionJournalVM vm)
        //{
        //    try
        //    {
        //        return new GLPCRequisitionJournalDAL().Update(vm);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public List<GLPCRequisitionJournalVM> SelectAllToPost(string Id = "")
        {
            try
            {
                return new GLPCRequisitionJournalDAL().SelectAllToPost(Id);
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
                return new GLPCRequisitionJournalDAL().Post(ids, auditVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report(string Id = "")
        {
            try
            {
                return new GLPCRequisitionJournalDAL().Report(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
