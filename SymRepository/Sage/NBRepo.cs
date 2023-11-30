using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;

namespace SymRepository.Sage
{
    public class NBRepo
    {
        public List<NBViewModel> DropDownAll()
        {
            try
            {
                return new NbDAL().DropDownAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NBViewModel> DropDown()
        {
            try
            {
                return new NbDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NBViewModel> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new NbDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(NBViewModel vm)
        {
            try
            {
                return new NbDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(NBViewModel vm)
        {
            try
            {
                return new NbDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(NBViewModel vm, string[] ids)
        {
            try
            {
                return new NbDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
