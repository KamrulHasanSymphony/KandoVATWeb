using System;
using System.Collections.Generic;
using System.Data;
using SymServices.VMS;
using SymViewModel.VMS;

namespace SymRepository.VMS
{
    public class IssueDetailRepo
    {

        public List<IssueDetailViewModel> SelectByMasterId(int Id)
        {
            try
            {
                return new IssueDetailDAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<IssueDetailViewModel> SelectById(int Id)
        {
            try
            {
                return new IssueDetailDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<IssueDetailViewModel> SelectAll(string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new IssueDetailDAL().SelectAll(conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Insert(IssueDetailViewModel vm)
        {
            try
            {
                return new IssueDetailDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable VoucherReport(IssueDetailViewModel vm, string[] conditionField = null, string[] conditionValue = null)
        {
            return new IssueDetailDAL().VoucherReport(vm, conditionField, conditionValue);
        }

    }
}
