using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Sage
{
    public class GLPettyCashRequisitionFormBRepo
    {
        public List<GLPettyCashRequisitionFormBVM> SelectAllCommissionBillDetail(string CommissionBillNo, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLPettyCashRequisitionFormBDAL().SelectAllCommissionBillDetail(CommissionBillNo, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<GLPettyCashRequisitionFormBVM> SelectByMasterId(int Id)
        {
            try
            {
                return new GLPettyCashRequisitionFormBDAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLPettyCashRequisitionFormBVM> SelectById(int Id)
        {
            try
            {
                return new GLPettyCashRequisitionFormBDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLPettyCashRequisitionFormBVM> SelectAll(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLPettyCashRequisitionFormBDAL().SelectAll(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public string[] Insert(GLPettyCashRequisitionFormBVM vm)
        //{
        //    try
        //    {
        //        return new GLPettyCashRequisitionFormBDAL().Insert(vm);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public string[] AcceptReject(GLPettyCashRequisitionFormBVM vm, string[] ids)
        {
            try
            {
                return new GLPettyCashRequisitionFormBDAL().AcceptReject(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report(GLPettyCashRequisitionFormBVM vm, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLPettyCashRequisitionFormBDAL().Report(vm, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
