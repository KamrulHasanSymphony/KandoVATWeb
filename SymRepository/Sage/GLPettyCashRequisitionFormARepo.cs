using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Sage
{
    public class GLPettyCashRequisitionFormARepo
    {
        public List<GLPettyCashRequisitionFormAVM> SelectByMasterId(int Id)
        {
            try
            {
                return new GLPettyCashRequisitionFormADAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLPettyCashRequisitionFormAVM> SelectById(int Id)
        {
            try
            {
                return new GLPettyCashRequisitionFormADAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLPettyCashRequisitionFormAVM> SelectAll(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLPettyCashRequisitionFormADAL().SelectAll(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLPettyCashRequisitionFormAVM vm)
        {
            try
            {
                return new GLPettyCashRequisitionFormADAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] AcceptReject(GLPettyCashRequisitionFormAVM vm, string[] ids)
        {
            try
            {
                return new GLPettyCashRequisitionFormADAL().AcceptReject(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(GLPettyCashRequisitionFormAVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLPettyCashRequisitionFormADAL().Report(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
