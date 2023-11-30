using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;


namespace SymRepository.Sage
{
    public class GLEnumDesignationRepo
    {
                public List<GLEnumDesignationVM> DropDown()
        {
            try
            {
                return new GLEnumDesignationDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLEnumDesignationVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GLEnumDesignationDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLEnumDesignationVM vm)
        {
            try
            {
                return new GLEnumDesignationDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(GLEnumDesignationVM vm)
        {
            try
            {
                return new GLEnumDesignationDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(GLEnumDesignationVM vm, string[] ids)
        {
            try
            {
                return new GLEnumDesignationDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
