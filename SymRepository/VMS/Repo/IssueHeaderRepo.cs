using SymServices.VMS.Library;
using SymViewModel.VMS.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace SymRepository.VMS.Repo
{
    public class IssueHeaderRepo
    {
        public string[] IssueInsert(IssueMasterVM Master, SqlTransaction transaction=null, SqlConnection currConn=null) 
        {
            return new IssueDAL().IssueInsert(Master, transaction, currConn);
        }

        public string[] IssueUpdate(IssueMasterVM Master)
        {
            return new IssueDAL().IssueUpdate(Master);
        }

        public string[] IssuePost(IssueMasterVM Master)
        {
            return new IssueDAL().IssuePost(Master);

        }

        public List<IssueMasterVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null) 
        {
            return new IssueDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
        }

        public List<IssueDetailVM> SelectIssueDetail(string issueNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            return new IssueDAL().SelectIssueDetail(issueNo, conditionFields, conditionValues, VcurrConn, Vtransaction);
        }
    }
}
