using System;
using System.Collections.Generic;
using SymServices.VMS.Library;
using SymViewModel.VMS.DTOs;

namespace SymRepository.Vms
{
    public class ProductTypeRepo
    {
        public List<ProductTypeViewModel> DropDownAll()
        {
            try
            {
                return new ProductTypeDAL().DropDownAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ProductTypeViewModel> DropDown()
        {
            try
            {
                return new ProductTypeDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ProductTypeViewModel> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new ProductTypeDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(ProductTypeViewModel vm)
        {
            try
            {
                return new ProductTypeDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(ProductTypeViewModel vm)
        {
            try
            {
                return new ProductTypeDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(ProductTypeViewModel vm, string[] ids)
        {
            try
            {
                return new ProductTypeDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
