using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Sage
{
    public class GLUserBranchRepo
    {
        public List<GLUserBranchVM> DropDown(string gluserid = "", string Admin = "N")
        {
            try
            {
                return new GLUserBranchDAL().DropDown(gluserid, Admin);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLUserBranchVM> SelectByMasterId(int Id)
        {
            try
            {
                return new GLUserBranchDAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLUserBranchVM> SelectById(int Id)
        {
            try
            {
                return new GLUserBranchDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLUserBranchVM> SelectAll(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new GLUserBranchDAL().SelectAll(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLUserBranchVM> SelectAllBranch(GLUserBranchVM vm)
        {
            try
            {
                return new GLUserBranchDAL().SelectAllBranch(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Insert(GLUserBranchVM vm)
        {
            try
            {
                return new GLUserBranchDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] InsertUserBranch(List<GLUserBranchVM> VMs, GLUserBranchVM vm)
        {
            try
            {
                return new GLUserBranchDAL().InsertUserBranch(VMs, vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
