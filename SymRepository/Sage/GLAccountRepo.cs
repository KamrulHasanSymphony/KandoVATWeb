using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;


namespace SymRepository.Sage
{
    public class GLAccountRepo
    {
        public List<GLAccountVM> DropDown(int branchId = 0, string FormName = "")
        {
            try
            {
                return new GLAccountDAL().DropDown(branchId, FormName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLAccountVM> DropDownHO(int branchId = 0)
        {
            try
            {
                return new GLAccountDAL().DropDownHO(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLAccountVM> DropDownExpenseVATBDE(int branchId = 0)
        {
            try
            {
                return new GLAccountDAL().DropDownExpenseVATBDE(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLAccountVM> DropDownExpenseAITBDE(int branchId = 0)
        {
            try
            {
                return new GLAccountDAL().DropDownExpenseAITBDE(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLAccountVM> DropDownAgentCommissionVAT(int branchId = 0)
        {
            try
            {
                return new GLAccountDAL().DropDownAgentCommissionVAT(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLAccountVM> DropDownAgentCommissionAIT(int branchId = 0)
        {
            try
            {
                return new GLAccountDAL().DropDownAgentCommissionAIT(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLAccountVM> DropDownExpenseVAT(int branchId = 0)
        {
            try
            {
                return new GLAccountDAL().DropDownExpenseVAT(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLAccountVM> DropDownExpenseAIT(int branchId = 0)
        {
            try
            {
                return new GLAccountDAL().DropDownExpenseAIT(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

         public List<GLAccountVM> DropDownBDEExpenseCreditBDE(int branchId = 0)
        {
            try
            {
                return new GLAccountDAL().DropDownBDEExpenseCredit(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLAccountVM> DropDownPCExpenseCredit(int branchId = 0)
        {
            try
            {
                return new GLAccountDAL().DropDownPCExpenseCredit(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLAccountVM> DropDownPCExpense(int branchId = 0)
        {
            try
            {
                return new GLAccountDAL().DropDownPCExpense(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLAccountVM> DropDownBDEExpense(int branchId = 0)
        {
            try
            {
                return new GLAccountDAL().DropDownBDEExpense(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLAccountVM> DropDownPCRequesition(int branchId = 0)
        {
            try
            {
                return new GLAccountDAL().DropDownPCRequesition(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLAccountVM> DropDownPCRequesitionBankCharge(int branchId = 0)
        {
            try
            {
                return new GLAccountDAL().DropDownPCRequesitionBankCharge(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLAccountVM> DropDownPCRequesitionContingency(int branchId = 0)
        {
            try
            {
                return new GLAccountDAL().DropDownPCRequesitionContingency(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLAccountVM> DropDownBDERequesitionBankCharge(int branchId = 0)
        {
            try
            {
                return new GLAccountDAL().DropDownBDERequesitionBankCharge(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLAccountVM> DropDownBDERequesitionContingency(int branchId = 0)
        {
            try
            {
                return new GLAccountDAL().DropDownBDERequesitionContingency(branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLAccountVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GLAccountDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLAccountVM vm)
        {
            try
            {
                return new GLAccountDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(GLAccountVM vm)
        {
            try
            {
                return new GLAccountDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(GLAccountVM vm, string[] ids)
        {
            try
            {
                return new GLAccountDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
