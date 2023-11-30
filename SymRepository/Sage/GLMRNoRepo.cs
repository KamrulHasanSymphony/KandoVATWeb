using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Sage
{
    public class GLMRNoRepo
    {
        public List<GLMRNoVM> DropDownGLBDERequisitionFormA(string tType = "", int branchId = 0)
        {
            try
            {
                return new GLMRNoDAL().DropDownGLBDERequisitionFormA(tType, branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLMRNoVM> DropDown(string tType = "", int branchId = 0)
        {
            try
            {
                return new GLMRNoDAL().DropDown(tType, branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLMRNoVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GLMRNoDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLMRNoVM vm)
        {
            try
            {
                return new GLMRNoDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(GLMRNoVM vm)
        {
            try
            {
                return new GLMRNoDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
