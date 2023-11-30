using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;

namespace SymRepository.Sage
{
    public class ProductRepo
    {
        public List<NewProductViewModel> DropDownAll()
        {
            try
            {
                return new ProductDAL().DropDownAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NewProductViewModel> DropDown()
        {
            try
            {
                return new ProductDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NewProductViewModel> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new ProductDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(NewProductViewModel vm)
        {
            try
            {
                return new ProductDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(NewProductViewModel vm)
        {
            try
            {
                return new ProductDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(NewProductViewModel vm, string[] ids)
        {
            try
            {
                return new ProductDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
