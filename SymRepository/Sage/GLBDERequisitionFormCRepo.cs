using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Sage
{
    public class GLBDERequisitionFormCRepo
    {
                public List<GLBDERequisitionFormCVM> SelectByMasterId(int Id)
        {
            try
            {
                return new GLBDERequisitionFormCDAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDERequisitionFormCVM> SelectById(int Id)
        {
            try
            {
                return new GLBDERequisitionFormCDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDERequisitionFormCVM> SelectAll(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionFormCDAL().SelectAll(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLBDERequisitionFormCVM vm)
        {
            try
            {
                return new GLBDERequisitionFormCDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] AcceptReject(GLBDERequisitionFormCVM vm, string[] ids)
        {
            try
            {
                return new GLBDERequisitionFormCDAL().AcceptReject(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(GLBDERequisitionFormCVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionFormCDAL().Report(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
