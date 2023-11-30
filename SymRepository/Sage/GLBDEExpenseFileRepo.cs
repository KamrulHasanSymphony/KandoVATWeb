using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Sage
{
    public class GLBDEExpenseFileRepo
    {
        public List<GLBDEExpenseFileVM> SelectByMasterId(int Id)
        {
            try
            {
                return new GLBDEExpenseFileDAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDEExpenseFileVM> SelectById(int Id)
        {
            try
            {
                return new GLBDEExpenseFileDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDEExpenseFileVM> SelectAll(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDEExpenseFileDAL().SelectAll(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLBDEExpenseFileVM vm)
        {
            try
            {
                return new GLBDEExpenseFileDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
