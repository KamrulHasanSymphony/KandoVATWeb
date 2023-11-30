using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Sage
{
    public class GLBDERequisitionFormERepo
    {
        public List<GLBDERequisitionFormEVM> SelectByMasterId(int Id)
        {
            try
            {
                return new GLBDERequisitionFormEDAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDERequisitionFormEVM> SelectById(int Id)
        {
            try
            {
                return new GLBDERequisitionFormEDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GLBDERequisitionFormEVM> SelectAll_In_BDERequisitionPaidDetail(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionFormEDAL().SelectAll_In_BDERequisitionPaidDetail(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDERequisitionFormEVM> SelectAll_NotIn_BDERequisitionPaidDetail(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionFormEDAL().SelectAll_NotIn_BDERequisitionPaidDetail(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDERequisitionFormEVM> SelectAll(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionFormEDAL().SelectAll(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLBDERequisitionFormEVM vm)
        {
            try
            {
                return new GLBDERequisitionFormEDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] AcceptReject(GLBDERequisitionFormEVM vm, string[] ids)
        {
            try
            {
                return new GLBDERequisitionFormEDAL().AcceptReject(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report(GLBDERequisitionFormEVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionFormEDAL().Report(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
