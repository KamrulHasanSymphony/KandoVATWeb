using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Sage
{
    public class GLCeilingRepo
    {

        public List<GLCeilingVM> SelectAll(int Id = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLCeilingDAL().SelectAll(Id, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLCeilingVM vm)
        {
            try
            {
                return new GLCeilingDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(GLCeilingVM vm)
        {
            try
            {
                return new GLCeilingDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(GLCeilingVM vm, string[] ids)
        {
            try
            {
                return new GLCeilingDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Post(GLCeilingVM vm, string[] ids)
        {
            try
            {
                return new GLCeilingDAL().Post(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertCeiling(GLCeilingVM vm)
        {
            try
            {
                return new GLCeilingDAL().InsertCeiling(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet FindBalance(string TransactionDateTime, string AccountId)
        {
            try
            {
                return new GLCeilingDAL().FindBalance(TransactionDateTime, AccountId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
    }
}
