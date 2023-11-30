using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Sage
{
    public class GLPettyCashRequisitionFileRepo
    {
        public List<GLPettyCashRequisitionFileVM> SelectByMasterId(int Id)
        {
            try
            {
                return new GLPettyCashRequisitionFileDAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLPettyCashRequisitionFileVM> SelectById(int Id)
        {
            try
            {
                return new GLPettyCashRequisitionFileDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLPettyCashRequisitionFileVM> SelectAll(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLPettyCashRequisitionFileDAL().SelectAll(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLPettyCashRequisitionFileVM vm)
        {
            try
            {
                return new GLPettyCashRequisitionFileDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    
    }
}
