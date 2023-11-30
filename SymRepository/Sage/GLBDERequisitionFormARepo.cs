using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Sage
{
    public class GLBDERequisitionFormARepo
    {
        public List<GLBDERequisitionFormAVM> SelectByMasterId(int Id)
        {
            try
            {
                return new GLBDERequisitionFormADAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDERequisitionFormAVM> SelectById(int Id)
        {
            try
            {
                return new GLBDERequisitionFormADAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLBDERequisitionFormAVM> SelectAll_In_BDERequisitionPaidDetail(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionFormADAL().SelectAll_In_BDERequisitionPaidDetail(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDERequisitionFormAVM> SelectAll_NotIn_BDERequisitionPaidDetail(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionFormADAL().SelectAll_NotIn_BDERequisitionPaidDetail(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDERequisitionFormAVM> SelectAll(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionFormADAL().SelectAll(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLBDERequisitionFormAVM vm)
        {
            try
            {
                return new GLBDERequisitionFormADAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] AcceptReject(GLBDERequisitionFormAVM vm, string[] ids)
        {
            try
            {
                return new GLBDERequisitionFormADAL().AcceptReject(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(GLBDERequisitionFormAVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionFormADAL().Report(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
