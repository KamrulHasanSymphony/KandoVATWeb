using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Sage
{
    public class GLBDERequisitionFormBRepo
    {
        public List<GLBDERequisitionFormBVM> SelectByMasterId(int Id)
        {
            try
            {
                return new GLBDERequisitionFormBDAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDERequisitionFormBVM> SelectById(int Id)
        {
            try
            {
                return new GLBDERequisitionFormBDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLBDERequisitionFormBVM> SelectAll_In_BDERequisitionPaidDetail(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionFormBDAL().SelectAll_In_BDERequisitionPaidDetail(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDERequisitionFormBVM> SelectAll_NotIn_BDERequisitionPaidDetail(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionFormBDAL().SelectAll_NotIn_BDERequisitionPaidDetail(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDERequisitionFormBVM> SelectAll(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionFormBDAL().SelectAll(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLBDERequisitionFormBVM vm)
        {
            try
            {
                return new GLBDERequisitionFormBDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] AcceptReject(GLBDERequisitionFormBVM vm, string[] ids)
        {
            try
            {
                return new GLBDERequisitionFormBDAL().AcceptReject(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(GLBDERequisitionFormBVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionFormBDAL().Report(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
