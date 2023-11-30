using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;


namespace SymRepository.Sage
{
    public class GLEnumDepartmentRepo
    {
                public List<GLEnumDepartmentVM> DropDown()
        {
            try
            {
                return new GLEnumDepartmentDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLEnumDepartmentVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GLEnumDepartmentDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLEnumDepartmentVM vm)
        {
            try
            {
                return new GLEnumDepartmentDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(GLEnumDepartmentVM vm)
        {
            try
            {
                return new GLEnumDepartmentDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(GLEnumDepartmentVM vm, string[] ids)
        {
            try
            {
                return new GLEnumDepartmentDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
