using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Sage
{
    public class GLFundReceivedPettyCashRequisitionRepo
    {
        public List<GLFundReceivedPettyCashRequisitionVM> DropDown(string tType = null, int branchId = 0)
        {
            try
            {
                return new GLFundReceivedPettyCashRequisitionDAL().DropDown(tType, branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLFundReceivedPettyCashRequisitionVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GLFundReceivedPettyCashRequisitionDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLFundReceivedPettyCashRequisitionVM vm)
        {
            try
            {
                return new GLFundReceivedPettyCashRequisitionDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(GLFundReceivedPettyCashRequisitionVM vm, string[] ids)
        {
            try
            {
                return new GLFundReceivedPettyCashRequisitionDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Receive(GLFundReceivedPettyCashRequisitionVM vm, string[] ids)
        {
            try
            {
                return new GLFundReceivedPettyCashRequisitionDAL().Receive(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
    }
}
