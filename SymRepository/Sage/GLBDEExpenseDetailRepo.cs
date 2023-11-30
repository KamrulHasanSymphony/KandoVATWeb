using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Sage
{
    public class GLBDEExpenseDetailRepo
    {
        public List<GLBDEExpenseDetailVM> SelectChart1(GLReportVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDEExpenseDetailDAL().SelectChart1(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDEExpenseDetailVM> SelectChart2(GLReportVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDEExpenseDetailDAL().SelectChart2(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDEExpenseDetailVM> SelectChart3(GLReportVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDEExpenseDetailDAL().SelectChart3(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDEExpenseDetailVM> SelectChart4(GLReportVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDEExpenseDetailDAL().SelectChart4(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDEExpenseDetailVM> SelectByMasterId(int Id)
        {
            try
            {
                return new GLBDEExpenseDetailDAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDEExpenseDetailVM> SelectById(int Id)
        {
            try
            {
                return new GLBDEExpenseDetailDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDEExpenseDetailVM> SelectAll(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDEExpenseDetailDAL().SelectAll(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLBDEExpenseDetailVM vm)
        {
            try
            {
                return new GLBDEExpenseDetailDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable Report(GLBDEExpenseDetailVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDEExpenseDetailDAL().Report(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable VoucherReport(GLBDEExpenseDetailVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDEExpenseDetailDAL().VoucherReport(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
