using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Sage
{
    public class GLCeilingDetailRepo
    {
        public List<GLCeilingDetailVM> SelectByMasterId(int Id)
        {
            try
            {
                return new GLCeilingDetailDAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLCeilingDetailVM> SelectById(int Id)
        {
            try
            {
                return new GLCeilingDetailDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLCeilingDetailVM> SelectAllCeilingDetail(int BranchId, int GLFiscalYearId, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLCeilingDetailDAL().SelectAllCeilingDetail(BranchId, GLFiscalYearId, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable SelectAllCeilingDetailDownload(int BranchId, int GLFiscalYearId, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLCeilingDetailDAL().SelectAllCeilingDetailDownload(BranchId, GLFiscalYearId, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLCeilingDetailVM> SelectAll(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLCeilingDetailDAL().SelectAll(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLCeilingDetailVM vm)
        {
            try
            {
                return new GLCeilingDetailDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] ImportExcelFile(string Fullpath, string fileName, GLCeilingDetailVM vm)
        {
            try
            {
                return new GLCeilingDetailDAL().ImportExcelFile(Fullpath, fileName, vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
