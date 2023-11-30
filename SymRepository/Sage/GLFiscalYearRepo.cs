using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Sage
{
    public class GLFiscalYearRepo
    {
        GLFiscalYearDAL _dal = new GLFiscalYearDAL();
        public List<GLFiscalYearVM> DropDown()
        {
            try
            {
                return _dal.DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLFiscalYearDetailVM> DropDownFiscalYearDetail(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return _dal.DropDownFiscalYearDetail(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLFiscalYearVM> SelectAll(string Id = "")
        {
            try
            {
                return _dal.SelectAll(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLFiscalYearVM vm)
        {
            try
            {
                return _dal.Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(GLFiscalYearVM vm)
        {
            try
            {
                return _dal.Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLFiscalYearDetailVM> SelectAllFiscalYearDetail(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return _dal.SelectAllFiscalYearDetail(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
