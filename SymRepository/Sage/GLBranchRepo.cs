using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;

namespace SymRepository.Sage
{
    public class GLBranchRepo
    {
        public List<GLBranchVM> DropDownAll()
        {
            try
            {
                return new GLBranchDAL().DropDownAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLBranchVM> DropDown()
        {
            try
            {
                return new GLBranchDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLBranchVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GLBranchDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLBranchVM vm)
        {
            try
            {
                return new GLBranchDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(GLBranchVM vm)
        {
            try
            {
                return new GLBranchDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(GLBranchVM vm, string[] ids)
        {
            try
            {
                return new GLBranchDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
