using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;

namespace SymRepository.Sage
{
    public class GLEnumBusinessNatureRepo
    {
        public List<GLEnumBusinessNatureVM> DropDown()
        {
            try
            {
                return new GLEnumBusinessNatureDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLEnumBusinessNatureVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GLEnumBusinessNatureDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLEnumBusinessNatureVM vm)
        {
            try
            {
                return new GLEnumBusinessNatureDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(GLEnumBusinessNatureVM vm)
        {
            try
            {
                return new GLEnumBusinessNatureDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(GLEnumBusinessNatureVM vm, string[] ids)
        {
            try
            {
                return new GLEnumBusinessNatureDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
