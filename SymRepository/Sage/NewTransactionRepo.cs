using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace SymRepository.Sage
{
    public class NewTransactionRepo
    {
        public List<NewTransactionViewModel> DropDown(int branchId = 0)
        {
            try
            {
                return new NewTransactionDAL().DropDown(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public List<NewTransactionViewModel> SelectAll(int Id = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new NewTransactionDAL().SelectAll(Id, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(NewTransactionViewModel vm)
        {
            try
            {
                return new NewTransactionDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(NewTransactionViewModel vm)
        {
            try
            {
                return new NewTransactionDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(NewTransactionViewModel vm, string[] ids)
        {
            try
            {
                return new NewTransactionDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
