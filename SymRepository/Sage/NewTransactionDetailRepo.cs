using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Sage
{
    public class NewTransactionDetailRepo
    {
       

        public List<NewTransactionDetailViewModel> SelectByMasterId(int Id)
        {
            try
            {
                return new NewTransactionDetailDAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<NewTransactionDetailViewModel> SelectById(int Id)
        {
            try
            {
                return new NewTransactionDetailDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<NewTransactionDetailViewModel> SelectAll(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new NewTransactionDetailDAL().SelectAll(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(NewTransactionDetailViewModel vm)
        {
            try
            {
                return new NewTransactionDetailDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
