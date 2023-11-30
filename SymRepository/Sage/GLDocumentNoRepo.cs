using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Sage
{
    public class GLDocumentNoRepo
    {
        public List<GLDocumentNoVM> DropDown(string tType = null, int branchId = 0)
        {
            try
            {
                return new GLDocumentNoDAL().DropDown(tType, branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLDocumentNoVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GLDocumentNoDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLDocumentNoVM vm)
        {
            try
            {
                return new GLDocumentNoDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(GLDocumentNoVM vm)
        {
            try
            {
                return new GLDocumentNoDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
