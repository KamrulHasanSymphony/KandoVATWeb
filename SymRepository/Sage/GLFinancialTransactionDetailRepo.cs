using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Sage
{
    public class GLFinancialTransactionDetailRepo
    {
        public List<GLFinancialTransactionDetailVM> SelectChart1(GLReportVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLFinancialTransactionDetailDAL().SelectChart1(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLFinancialTransactionDetailVM> SelectChart2(GLReportVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLFinancialTransactionDetailDAL().SelectChart2(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLFinancialTransactionDetailVM> SelectChart3(GLReportVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLFinancialTransactionDetailDAL().SelectChart3(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLFinancialTransactionDetailVM> SelectChart4(GLReportVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLFinancialTransactionDetailDAL().SelectChart4(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLFinancialTransactionDetailVM> SelectAllFromPCR(GLFinancialTransactionVM paramVM)
        {
            try
            {
                return new GLFinancialTransactionDetailDAL().SelectAllFromPCR(paramVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<GLFinancialTransactionDetailVM> SelectByMasterId(int Id)
        {
            try
            {
                return new GLFinancialTransactionDetailDAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLFinancialTransactionDetailVM> SelectById(int Id)
        {
            try
            {
                return new GLFinancialTransactionDetailDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLFinancialTransactionDetailVM> SelectAll(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLFinancialTransactionDetailDAL().SelectAll(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLFinancialTransactionDetailVM vm)
        {
            try
            {
                return new GLFinancialTransactionDetailDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable Report(GLFinancialTransactionDetailVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLFinancialTransactionDetailDAL().Report(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable VoucherReport(GLFinancialTransactionDetailVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLFinancialTransactionDetailDAL().VoucherReport(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
