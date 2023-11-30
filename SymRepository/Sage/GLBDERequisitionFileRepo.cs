using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Sage
{
    public class GLBDERequisitionFileRepo
    {
        public List<GLBDERequisitionFileVM> SelectByMasterId(int Id)
        {
            try
            {
                return new GLBDERequisitionFileDAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDERequisitionFileVM> SelectById(int Id)
        {
            try
            {
                return new GLBDERequisitionFileDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDERequisitionFileVM> SelectAll(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionFileDAL().SelectAll(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLBDERequisitionFileVM vm)
        {
            try
            {
                return new GLBDERequisitionFileDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    
    }
}
