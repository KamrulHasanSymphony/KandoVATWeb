using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Sage
{
    public class GLFinancialTransactionFileRepo
    {
        public List<GLFinancialTransactionFileVM> SelectByMasterId(int Id)
        {
            try
            {
                return new GLFinancialTransactionFileDAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLFinancialTransactionFileVM> SelectById(int Id)
        {
            try
            {
                return new GLFinancialTransactionFileDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLFinancialTransactionFileVM> SelectAll(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLFinancialTransactionFileDAL().SelectAll(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLFinancialTransactionFileVM vm)
        {
            try
            {
                return new GLFinancialTransactionFileDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    
    }
}
