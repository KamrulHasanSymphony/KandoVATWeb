using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Sage
{
    public class GLFundReceivedBDERequisitionRepo
    {
        public List<GLFundReceivedBDERequisitionVM> DropDown(string tType = null, int branchId = 0)
        {
            try
            {
                return new GLFundReceivedBDERequisitionDAL().DropDown(tType, branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLFundReceivedBDERequisitionVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GLFundReceivedBDERequisitionDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLFundReceivedBDERequisitionVM vm)
        {
            try
            {
                return new GLFundReceivedBDERequisitionDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(GLFundReceivedBDERequisitionVM vm, string[] ids)
        {
            try
            {
                return new GLFundReceivedBDERequisitionDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Receive(GLFundReceivedBDERequisitionVM vm, string[] ids)
        {
            try
            {
                return new GLFundReceivedBDERequisitionDAL().Receive(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
    }
}
