using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;

namespace SymRepository.Sage
{
    public class SageRepo
    {
        
        public List<GLAMFVM> DropDown(string branchCode = "")
        {
            try
            {
                return new SageDAL().DropDown(branchCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLAMFVM> SelectAll(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new SageDAL().SelectAll(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
