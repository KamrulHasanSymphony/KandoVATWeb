using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Sage
{
    public class GLBDERequisitionFormDRepo
    {
                public List<GLBDERequisitionFormDVM> SelectByMasterId(int Id)
        {
            try
            {
                return new GLBDERequisitionFormDDAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDERequisitionFormDVM> SelectById(int Id)
        {
            try
            {
                return new GLBDERequisitionFormDDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLBDERequisitionFormDVM> SelectAll(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionFormDDAL().SelectAll(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLBDERequisitionFormDVM vm)
        {
            try
            {
                return new GLBDERequisitionFormDDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] AcceptReject(GLBDERequisitionFormDVM vm, string[] ids)
        {
            try
            {
                return new GLBDERequisitionFormDDAL().AcceptReject(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report(GLBDERequisitionFormDVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLBDERequisitionFormDDAL().Report(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
