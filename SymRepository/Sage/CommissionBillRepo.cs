using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;


namespace SymRepository.Sage
{
    public class CommissionBillRepo
    {
         public List<CommissionBillVM> DropDownFromPCR(int branchId = 0)
        {
            try
            {
                return new CommissionBillDAL().DropDownFromPCR(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CommissionBillVM> DropDown(int branchId = 0)
        {
            try
            {
                return new CommissionBillDAL().DropDown(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CommissionBillVM> SelectAll(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new CommissionBillDAL().SelectAll(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CommissionBillDetailVM> SelectAllDetail(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new CommissionBillDAL().SelectAllDetail(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
