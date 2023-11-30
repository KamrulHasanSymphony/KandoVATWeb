using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SymOrdinary;
using SymServices.Common;
using SymViewModel.Sage;
using System.Web.Mvc;
using System.Drawing;
using System.Threading;


namespace SymServices.Sage
{
    public class NewTransactionDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        public static Thread thread;
        #endregion
        #region Methods
        //==================DropDown=================
        public List<NewTransactionViewModel> DropDown(int branchId = 0)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<NewTransactionViewModel> VMs = new List<NewTransactionViewModel>();
            NewTransactionViewModel vm;
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnectionSageGL();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
SELECT
af.Id
,af.Code
   FROM NewTransactions af
WHERE  1=1
";

                if (branchId > 0)
                {
                    sqlText += @" and BranchId=@BranchId";
                }
                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                if (branchId > 0)
                {
                    objComm.Parameters.AddWithValue("@BranchId", branchId);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new NewTransactionViewModel();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Name = dr["Code"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return VMs;
        }


        //==================ExistCheckCommissionBillNo=================
        //public string ExistCheckCommissionBillNo(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        //{
        //    #region Variables
        //    string retResults = "";
        //    SqlConnection currConn = null;
        //    SqlTransaction transaction = null;
        //    string sqlText = "";
        //    #endregion
        //    try
        //    {
        //        #region open connection and transaction
        //        #region New open connection and transaction
        //        if (VcurrConn != null)
        //        {
        //            currConn = VcurrConn;
        //        }
        //        if (Vtransaction != null)
        //        {
        //            transaction = Vtransaction;
        //        }
        //        #endregion New open connection and transaction
        //        if (currConn == null)
        //        {
        //            currConn = _dbsqlConnection.GetConnectionSageGL();
        //            if (currConn.State != ConnectionState.Open)
        //            {
        //                currConn.Open();
        //            }
        //        }
        //        if (transaction == null)
        //        {
        //            transaction = currConn.BeginTransaction("");
        //        }
        //        #endregion open connection and transaction

        //        NewTransactionViewModel vm = SelectAll(0, conditionFields, conditionValues, currConn, transaction).FirstOrDefault();
        //        if (vm != null)
        //        {
        //            retResults = "This Commission Bill No. " + vm.CommissionBillNo + " already used! Code: " + vm.Code + " Date: " + vm.TransactionDateTime + " Please Select Another!";
        //        }

        //        if (Vtransaction == null && transaction != null)
        //        {
        //            transaction.Commit();
        //        }

        //    }
        //    #region catch
        //    catch (SqlException sqlex)
        //    {
        //        throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
        //    }
        //    #endregion
        //    #region finally
        //    finally
        //    {
        //        if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
        //        {
        //            currConn.Close();
        //        }
        //    }
        //    #endregion
        //    return retResults;
        //}


        //==================SelectAllSelfApprove=================

//        public List<NewTransactionViewModel> SelectAllSelfApprove(int Id = 0, int UserId = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
//        {
//            #region Variables
//            SqlConnection currConn = null;
//            SqlTransaction transaction = null;
//            string sqlText = "";
//            List<NewTransactionViewModel> VMs = new List<NewTransactionViewModel>();
//            NewTransactionViewModel vm;
//            #endregion
//            try
//            {
//                #region open connection and transaction
//                #region New open connection and transaction
//                if (VcurrConn != null)
//                {
//                    currConn = VcurrConn;
//                }
//                if (Vtransaction != null)
//                {
//                    transaction = Vtransaction;
//                }
//                #endregion New open connection and transaction
//                if (currConn == null)
//                {
//                    currConn = _dbsqlConnection.GetConnectionSageGL();
//                    if (currConn.State != ConnectionState.Open)
//                    {
//                        currConn.Open();
//                    }
//                }
//                if (transaction == null)
//                {
//                    transaction = currConn.BeginTransaction("");
//                }
//                #endregion open connection and transaction
//                #region sql statement
//                #region SqlText

//                sqlText = @"
//--declare @UserId int
//--set @UserId=3
//select a.*, br.Name BranchName from(
//SELECT
//ft.Id
//,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
//,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
//,ft.BranchId,ft.Code,ft.TransactionDateTime,ft.TransactionType,ft.ReferenceNo1,ft.ReferenceNo2,ft.ReferenceNo3,ft.Post
//
//,ft.Remarks,ft.IsActive,ft.IsArchive,ft.CreatedBy,ft.CreatedAt,ft.CreatedFrom,ft.LastUpdateBy,ft.LastUpdateAt,ft.LastUpdateFrom,ft.PostedBy,ft.PostedAt,ft.PostedFrom,ft.IsApprovedL1,ft.ApprovedByL1,ft.ApprovedDateL1,ft.CommentsL1,ft.IsApprovedL2,ft.ApprovedByL2,ft.ApprovedDateL2,ft.CommentsL2,ft.IsApprovedL3,ft.ApprovedByL3,ft.ApprovedDateL3,ft.CommentsL3,ft.IsApprovedL4,ft.ApprovedByL4,ft.ApprovedDateL4,ft.CommentsL4,ft.IsAudited,ft.AuditedBy,ft.AuditedDate,ft.AuditedComments,ft.IsRejected,ft.RejectedBy,ft.RejectedDate,ft.RejectedComments
//FROM NewTransactions  ft
//left outer join GLUsers u on u.id=@UserId
//WHERE  1=1  
//and u.HaveApprovalLevel1=1
//and u.HaveExpenseApproval=1
//AND ft.IsArchive = 0
//and ft.Post=1 and ft.IsRejected=0
//and ft.IsApprovedL1=0
//union all
//
//SELECT
//ft.Id
//,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
//,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
//,ft.BranchId,ft.Code,ft.TransactionDateTime,ft.TransactionType,ft.ReferenceNo1,ft.ReferenceNo2,ft.ReferenceNo3,ft.Post
//
//,ft.Remarks,ft.IsActive,ft.IsArchive,ft.CreatedBy,ft.CreatedAt,ft.CreatedFrom,ft.LastUpdateBy,ft.LastUpdateAt,ft.LastUpdateFrom,ft.PostedBy,ft.PostedAt,ft.PostedFrom,ft.IsApprovedL1,ft.ApprovedByL1,ft.ApprovedDateL1,ft.CommentsL1,ft.IsApprovedL2,ft.ApprovedByL2,ft.ApprovedDateL2,ft.CommentsL2,ft.IsApprovedL3,ft.ApprovedByL3,ft.ApprovedDateL3,ft.CommentsL3,ft.IsApprovedL4,ft.ApprovedByL4,ft.ApprovedDateL4,ft.CommentsL4,ft.IsAudited,ft.AuditedBy,ft.AuditedDate,ft.AuditedComments,ft.IsRejected,ft.RejectedBy,ft.RejectedDate,ft.RejectedComments
//
//FROM NewTransactions    ft
//left outer join GLUsers u on u.id=@UserId
//WHERE  1=1  
//and u.HaveApprovalLevel2=1
//and u.HaveExpenseApproval=1
//AND  ft.IsArchive = 0
//and  ft.Post=1 and  ft.IsRejected=0
//and  ft.IsApprovedL1=1 and  ft.IsApprovedL2=0
//union all
//
//SELECT
//ft.Id
//,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approv Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
//,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
//,ft.BranchId,ft.Code,ft.TransactionDateTime,ft.TransactionType,ft.ReferenceNo1,ft.ReferenceNo2,ft.ReferenceNo3,ft.Post
//
//,ft.Remarks,ft.IsActive,ft.IsArchive,ft.CreatedBy,ft.CreatedAt,ft.CreatedFrom,ft.LastUpdateBy,ft.LastUpdateAt,ft.LastUpdateFrom,ft.PostedBy,ft.PostedAt,ft.PostedFrom,ft.IsApprovedL1,ft.ApprovedByL1,ft.ApprovedDateL1,ft.CommentsL1,ft.IsApprovedL2,ft.ApprovedByL2,ft.ApprovedDateL2,ft.CommentsL2,ft.IsApprovedL3,ft.ApprovedByL3,ft.ApprovedDateL3,ft.CommentsL3,ft.IsApprovedL4,ft.ApprovedByL4,ft.ApprovedDateL4,ft.CommentsL4,ft.IsAudited,ft.AuditedBy,ft.AuditedDate,ft.AuditedComments,ft.IsRejected,ft.RejectedBy,ft.RejectedDate,ft.RejectedComments
//
//FROM NewTransactions   ft
//left outer join GLUsers u on u.id=@UserId
//WHERE  1=1  
//and u.HaveApprovalLevel3=1
//and u.HaveExpenseApproval=1
//AND  ft.IsArchive = 0
//and Post=1 and IsRejected=0
//and IsApprovedL1=1 and IsApprovedL2=1 and IsApprovedL3=0
//union all
//SELECT
//ft.Id
//,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
//,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
//,ft.BranchId,ft.Code,ft.TransactionDateTime,ft.TransactionType,ft.ReferenceNo1,ft.ReferenceNo2,ft.ReferenceNo3,ft.Post
//
//,ft.Remarks,ft.IsActive,ft.IsArchive,ft.CreatedBy,ft.CreatedAt,ft.CreatedFrom,ft.LastUpdateBy,ft.LastUpdateAt,ft.LastUpdateFrom,ft.PostedBy,ft.PostedAt,ft.PostedFrom,ft.IsApprovedL1,ft.ApprovedByL1,ft.ApprovedDateL1,ft.CommentsL1,ft.IsApprovedL2,ft.ApprovedByL2,ft.ApprovedDateL2,ft.CommentsL2,ft.IsApprovedL3,ft.ApprovedByL3,ft.ApprovedDateL3,ft.CommentsL3,ft.IsApprovedL4,ft.ApprovedByL4,ft.ApprovedDateL4,ft.CommentsL4,ft.IsAudited,ft.AuditedBy,ft.AuditedDate,ft.AuditedComments,ft.IsRejected,ft.RejectedBy,ft.RejectedDate,ft.RejectedComments
//FROM NewTransactions    ft
//left outer join GLUsers u on u.id=@UserId
//WHERE  1=1  
//and u.HaveApprovalLevel4=1
//and u.HaveExpenseApproval=1
//AND  ft.IsArchive = 0
//and Post=1 and  ft.IsRejected=0
//and IsApprovedL1=1 and IsApprovedL2=1 and IsApprovedL3=1 and IsApprovedL4=0
//  
//) as a
//LEFT OUTER JOIN GLBranchs br ON a.BranchId = br.Id
//WHERE  1=1
//";


//                if (Id > 0)
//                {
//                    sqlText += @" and a.Id=@Id";
//                }

//                string cField = "";
//                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
//                {
//                    for (int i = 0; i < conditionFields.Length; i++)
//                    {
//                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]))
//                        {
//                            continue;
//                        }
//                        cField = conditionFields[i].ToString();
//                        cField = Ordinary.StringReplacing(cField);
//                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
//                    }
//                }
//                #endregion SqlText
//                #region SqlExecution

//                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
//                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
//                {
//                    for (int j = 0; j < conditionFields.Length; j++)
//                    {
//                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]))
//                        {
//                            continue;
//                        }
//                        cField = conditionFields[j].ToString();
//                        cField = Ordinary.StringReplacing(cField);
//                        objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
//                    }
//                }

//                if (Id > 0)
//                {
//                    objComm.Parameters.AddWithValue("@Id", Id);
//                }
//                objComm.Parameters.AddWithValue("@UserId", UserId);

//                SqlDataReader dr;
//                dr = objComm.ExecuteReader();
//                while (dr.Read())
//                {

//                    vm = new NewTransactionViewModel();
//                    vm.Id = Convert.ToInt32(dr["Id"]);
//                    vm.Status = dr["Status"].ToString();
//                    vm.MyStatus = dr["MyStatus"].ToString();
//                    vm.BranchName = dr["BranchName"].ToString();

//                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
//                    vm.Code = dr["Code"].ToString();
//                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
//                    vm.TransactionType = dr["TransactionType"].ToString();
//                    vm.ReferenceNo1 = dr["ReferenceNo1"].ToString();
//                    vm.ReferenceNo2 = dr["ReferenceNo2"].ToString();
//                    vm.ReferenceNo3 = dr["ReferenceNo3"].ToString();
//                    vm.Post = Convert.ToBoolean(dr["Post"]);

//                    vm.Remarks = dr["Remarks"].ToString();
//                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
//                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
//                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
//                    vm.CreatedBy = dr["CreatedBy"].ToString();
//                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
//                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
//                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
//                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
//                    vm.PostedBy = dr["PostedBy"].ToString();
//                    vm.PostedAt = Ordinary.StringToDate(dr["PostedAt"].ToString());
//                    vm.PostedFrom = dr["PostedFrom"].ToString();

//                    vm.IsApprovedL1 = Convert.ToBoolean(dr["IsApprovedL1"]);
//                    vm.ApprovedByL1 = dr["ApprovedByL1"].ToString();
//                    vm.ApprovedDateL1 = Ordinary.StringToDate(dr["ApprovedDateL1"].ToString());
//                    vm.CommentsL1 = dr["CommentsL1"].ToString();

//                    vm.IsApprovedL2 = Convert.ToBoolean(dr["IsApprovedL2"]);
//                    vm.ApprovedByL2 = dr["ApprovedByL2"].ToString();
//                    vm.ApprovedDateL2 = Ordinary.StringToDate(dr["ApprovedDateL2"].ToString());
//                    vm.CommentsL2 = dr["CommentsL2"].ToString();

//                    vm.IsApprovedL3 = Convert.ToBoolean(dr["IsApprovedL3"]);
//                    vm.ApprovedByL3 = dr["ApprovedByL3"].ToString();
//                    vm.ApprovedDateL3 = Ordinary.StringToDate(dr["ApprovedDateL3"].ToString());
//                    vm.CommentsL3 = dr["CommentsL3"].ToString();

//                    vm.IsApprovedL4 = Convert.ToBoolean(dr["IsApprovedL4"]);
//                    vm.ApprovedByL4 = dr["ApprovedByL4"].ToString();
//                    vm.ApprovedDateL4 = Ordinary.StringToDate(dr["ApprovedDateL4"].ToString());
//                    vm.CommentsL4 = dr["CommentsL4"].ToString();

//                    vm.IsApprovedL1 = Convert.ToBoolean(dr["IsApprovedL1"]);
//                    vm.ApprovedByL1 = dr["ApprovedByL1"].ToString();
//                    vm.ApprovedDateL1 = Ordinary.StringToDate(dr["ApprovedDateL1"].ToString());
//                    vm.CommentsL1 = dr["CommentsL1"].ToString();

//                    vm.IsAudited = Convert.ToBoolean(dr["IsAudited"]);
//                    vm.AuditedBy = dr["AuditedBy"].ToString();
//                    vm.AuditedDate = Ordinary.StringToDate(dr["AuditedDate"].ToString());
//                    vm.AuditedComments = dr["AuditedComments"].ToString();

//                    vm.IsRejected = Convert.ToBoolean(dr["IsRejected"]);
//                    vm.RejectedBy = dr["RejectedBy"].ToString();
//                    vm.RejectedDate = Ordinary.StringToDate(dr["RejectedDate"].ToString());
//                    vm.RejectedComments = dr["RejectedComments"].ToString();

//                    VMs.Add(vm);
//                }
//                dr.Close();

//                #endregion SqlExecution

//                if (Vtransaction == null && transaction != null)
//                {
//                    transaction.Commit();
//                }
//                #endregion
//            }
//            #region catch
//            catch (SqlException sqlex)
//            {
//                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
//            }
//            catch (Exception ex)
//            {
//                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
//            }
//            #endregion
//            #region finally
//            finally
//            {
//                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
//                {
//                    currConn.Close();
//                }
//            }
//            #endregion
//            return VMs;
//        }

        //==================SelectAllPosted=================

//        public List<NewTransactionViewModel> SelectAllPosted(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
//        {
//            #region Variables
//            SqlConnection currConn = null;
//            SqlTransaction transaction = null;
//            string sqlText = "";
//            List<NewTransactionViewModel> VMs = new List<NewTransactionViewModel>();
//            NewTransactionViewModel vm;
//            #endregion
//            try
//            {
//                #region open connection and transaction
//                #region New open connection and transaction
//                if (VcurrConn != null)
//                {
//                    currConn = VcurrConn;
//                }
//                if (Vtransaction != null)
//                {
//                    transaction = Vtransaction;
//                }
//                #endregion New open connection and transaction
//                if (currConn == null)
//                {
//                    currConn = _dbsqlConnection.GetConnectionSageGL();
//                    if (currConn.State != ConnectionState.Open)
//                    {
//                        currConn.Open();
//                    }
//                }
//                if (transaction == null)
//                {
//                    transaction = currConn.BeginTransaction("");
//                }
//                #endregion open connection and transaction
//                #region sql statement
//                #region SqlText

//                sqlText = @"
//select a.*, br.Name BranchName from(
//SELECT
//Id
//,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
//,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
//,BranchId, Code, TransactionDateTime,TransactionType, ReferenceNo1, ReferenceNo2, ReferenceNo3, Post
//,Remarks, IsActive, IsArchive, CreatedBy, CreatedAt, CreatedFrom, LastUpdateBy, LastUpdateAt, LastUpdateFrom, PostedBy, PostedAt, PostedFrom
//,IsApprovedL1, ApprovedByL1, ApprovedDateL1, CommentsL1, IsApprovedL2, ApprovedByL2, ApprovedDateL2, CommentsL2, IsApprovedL3, ApprovedByL3, ApprovedDateL3, CommentsL3, IsApprovedL4, ApprovedByL4, ApprovedDateL4, CommentsL4, IsAudited, AuditedBy, AuditedDate, AuditedComments, IsRejected, RejectedBy, RejectedDate, RejectedComments
//
//FROM NewTransactions  
//WHERE  1=1  AND IsArchive = 0
// and Post=1 and IsRejected=1
//
// union all
//SELECT
//Id
//,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
//,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
//,BranchId, Code, TransactionDateTime,TransactionType, ReferenceNo1, ReferenceNo2, ReferenceNo3, Post
//,Remarks, IsActive, IsArchive, CreatedBy, CreatedAt, CreatedFrom, LastUpdateBy, LastUpdateAt, LastUpdateFrom, PostedBy, PostedAt, PostedFrom
//
//,IsApprovedL1, ApprovedByL1, ApprovedDateL1, CommentsL1, IsApprovedL2, ApprovedByL2, ApprovedDateL2, CommentsL2, IsApprovedL3, ApprovedByL3, ApprovedDateL3, CommentsL3, IsApprovedL4, ApprovedByL4, ApprovedDateL4, CommentsL4, IsAudited, AuditedBy, AuditedDate, AuditedComments, IsRejected, RejectedBy, RejectedDate, RejectedComments
//
//FROM NewTransactions  
//WHERE  1=1  AND IsArchive = 0
// and Post=1 and IsRejected=0
// and IsApprovedL1=0
//
// union all
//
// SELECT
//Id
//,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
//,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
//,BranchId, Code, TransactionDateTime,TransactionType, ReferenceNo1, ReferenceNo2, ReferenceNo3, Post
//,Remarks, IsActive, IsArchive, CreatedBy, CreatedAt, CreatedFrom, LastUpdateBy, LastUpdateAt, LastUpdateFrom, PostedBy, PostedAt, PostedFrom
//
//,IsApprovedL1, ApprovedByL1, ApprovedDateL1, CommentsL1, IsApprovedL2, ApprovedByL2, ApprovedDateL2, CommentsL2, IsApprovedL3, ApprovedByL3, ApprovedDateL3, CommentsL3, IsApprovedL4, ApprovedByL4, ApprovedDateL4, CommentsL4, IsAudited, AuditedBy, AuditedDate, AuditedComments, IsRejected, RejectedBy, RejectedDate, RejectedComments
//FROM NewTransactions  
//WHERE  1=1  AND IsArchive = 0
//and Post=1 and IsRejected=0
// and IsApprovedL1=1 and IsApprovedL2=0
//
// union all
//
// SELECT
//Id
//,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approv Completed' when IsApprovedL3=1 then 'Waiting for Approve Level3'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
//,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
//,BranchId, Code, TransactionDateTime,TransactionType, ReferenceNo1, ReferenceNo2, ReferenceNo3, Post
//,Remarks, IsActive, IsArchive, CreatedBy, CreatedAt, CreatedFrom, LastUpdateBy, LastUpdateAt, LastUpdateFrom, PostedBy, PostedAt, PostedFrom
//
//,IsApprovedL1, ApprovedByL1, ApprovedDateL1, CommentsL1, IsApprovedL2, ApprovedByL2, ApprovedDateL2, CommentsL2, IsApprovedL3, ApprovedByL3, ApprovedDateL3, CommentsL3, IsApprovedL4, ApprovedByL4, ApprovedDateL4, CommentsL4, IsAudited, AuditedBy, AuditedDate, AuditedComments, IsRejected, RejectedBy, RejectedDate, RejectedComments
//
//FROM NewTransactions  
//WHERE  1=1  AND IsArchive = 0
//and Post=1 and IsRejected=0
// and IsApprovedL1=1 and IsApprovedL2=1 and IsApprovedL3=0
// 
//  union all
//
// SELECT
//Id
//,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level3'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
//,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
//,BranchId, Code, TransactionDateTime,TransactionType, ReferenceNo1, ReferenceNo2, ReferenceNo3, Post
//,Remarks, IsActive, IsArchive, CreatedBy, CreatedAt, CreatedFrom, LastUpdateBy, LastUpdateAt, LastUpdateFrom, PostedBy, PostedAt, PostedFrom
//
//,IsApprovedL1, ApprovedByL1, ApprovedDateL1, CommentsL1, IsApprovedL2, ApprovedByL2, ApprovedDateL2, CommentsL2, IsApprovedL3, ApprovedByL3, ApprovedDateL3, CommentsL3, IsApprovedL4, ApprovedByL4, ApprovedDateL4, CommentsL4, IsAudited, AuditedBy, AuditedDate, AuditedComments, IsRejected, RejectedBy, RejectedDate, RejectedComments
//
//FROM NewTransactions  
//WHERE  1=1  AND IsArchive = 0
//and Post=1 and IsRejected=0
//and IsApprovedL1=1 and IsApprovedL2=1 and IsApprovedL3=1 and IsApprovedL4=0
//
//UNION ALL
//
// SELECT
//Id
//,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level3'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
//,case when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
//,BranchId, Code, TransactionDateTime,TransactionType, ReferenceNo1, ReferenceNo2, ReferenceNo3, Post
//,Remarks, IsActive, IsArchive, CreatedBy, CreatedAt, CreatedFrom, LastUpdateBy, LastUpdateAt, LastUpdateFrom, PostedBy, PostedAt, PostedFrom
//
//,IsApprovedL1, ApprovedByL1, ApprovedDateL1, CommentsL1, IsApprovedL2, ApprovedByL2, ApprovedDateL2, CommentsL2, IsApprovedL3, ApprovedByL3, ApprovedDateL3, CommentsL3, IsApprovedL4, ApprovedByL4, ApprovedDateL4, CommentsL4, IsAudited, AuditedBy, AuditedDate, AuditedComments, IsRejected, RejectedBy, RejectedDate, RejectedComments
//
//FROM NewTransactions  
//WHERE  1=1  AND IsArchive = 0
//and Post=1 and IsRejected=0
// and IsApprovedL1=1 and IsApprovedL2=1 and IsApprovedL3=1 and IsApprovedL4=1
// ) as a
//LEFT OUTER JOIN GLBranchs br ON a.BranchId = br.Id
//WHERE  1=1
//";


//                if (Id > 0)
//                {
//                    sqlText += @" and a.Id=@Id";
//                }

//                string cField = "";
//                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
//                {
//                    for (int i = 0; i < conditionFields.Length; i++)
//                    {
//                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]))
//                        {
//                            continue;
//                        }
//                        cField = conditionFields[i].ToString();
//                        cField = Ordinary.StringReplacing(cField);
//                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
//                    }
//                }
//                #endregion SqlText
//                #region SqlExecution

//                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
//                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
//                {
//                    for (int j = 0; j < conditionFields.Length; j++)
//                    {
//                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]))
//                        {
//                            continue;
//                        }
//                        cField = conditionFields[j].ToString();
//                        cField = Ordinary.StringReplacing(cField);
//                        objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
//                    }
//                }

//                if (Id > 0)
//                {
//                    objComm.Parameters.AddWithValue("@Id", Id);
//                }
//                SqlDataReader dr;
//                dr = objComm.ExecuteReader();
//                while (dr.Read())
//                {
//                    vm = new NewTransactionViewModel();
//                    vm.Id = Convert.ToInt32(dr["Id"]);
//                    vm.Status = dr["Status"].ToString();
//                    vm.MyStatus = dr["MyStatus"].ToString();

//                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
//                    vm.BranchName = dr["BranchName"].ToString();
//                    vm.Code = dr["Code"].ToString();
//                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
//                    vm.TransactionType = dr["TransactionType"].ToString();
//                    vm.ReferenceNo1 = dr["ReferenceNo1"].ToString();
//                    vm.ReferenceNo2 = dr["ReferenceNo2"].ToString();
//                    vm.ReferenceNo3 = dr["ReferenceNo3"].ToString();
//                    vm.Post = Convert.ToBoolean(dr["Post"]);
//                    vm.Remarks = dr["Remarks"].ToString();
//                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
//                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
//                    vm.CreatedAt = Ordinary.StringToDate(Ordinary.StringToDate(dr["CreatedAt"].ToString()));
//                    vm.CreatedBy = dr["CreatedBy"].ToString();
//                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
//                    vm.LastUpdateAt = Ordinary.StringToDate(Ordinary.StringToDate(dr["LastUpdateAt"].ToString()));
//                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
//                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
//                    vm.PostedBy = dr["PostedBy"].ToString();
//                    vm.PostedAt = Ordinary.StringToDate(dr["PostedAt"].ToString());
//                    vm.PostedFrom = dr["PostedFrom"].ToString();

//                    vm.IsApprovedL1 = Convert.ToBoolean(dr["IsApprovedL1"]);
//                    vm.ApprovedByL1 = dr["ApprovedByL1"].ToString();
//                    vm.ApprovedDateL1 = Ordinary.StringToDate(dr["ApprovedDateL1"].ToString());
//                    vm.CommentsL1 = dr["CommentsL1"].ToString();

//                    vm.IsApprovedL2 = Convert.ToBoolean(dr["IsApprovedL2"]);
//                    vm.ApprovedByL2 = dr["ApprovedByL2"].ToString();
//                    vm.ApprovedDateL2 = Ordinary.StringToDate(dr["ApprovedDateL2"].ToString());
//                    vm.CommentsL2 = dr["CommentsL2"].ToString();

//                    vm.IsApprovedL3 = Convert.ToBoolean(dr["IsApprovedL3"]);
//                    vm.ApprovedByL3 = dr["ApprovedByL3"].ToString();
//                    vm.ApprovedDateL3 = Ordinary.StringToDate(dr["ApprovedDateL3"].ToString());
//                    vm.CommentsL3 = dr["CommentsL3"].ToString();

//                    vm.IsApprovedL4 = Convert.ToBoolean(dr["IsApprovedL4"]);
//                    vm.ApprovedByL4 = dr["ApprovedByL4"].ToString();
//                    vm.ApprovedDateL4 = Ordinary.StringToDate(dr["ApprovedDateL4"].ToString());
//                    vm.CommentsL4 = dr["CommentsL4"].ToString();

//                    vm.IsApprovedL1 = Convert.ToBoolean(dr["IsApprovedL1"]);
//                    vm.ApprovedByL1 = dr["ApprovedByL1"].ToString();
//                    vm.ApprovedDateL1 = Ordinary.StringToDate(dr["ApprovedDateL1"].ToString());
//                    vm.CommentsL1 = dr["CommentsL1"].ToString();

//                    vm.IsAudited = Convert.ToBoolean(dr["IsAudited"]);
//                    vm.AuditedBy = dr["AuditedBy"].ToString();
//                    vm.AuditedDate = Ordinary.StringToDate(dr["AuditedDate"].ToString());
//                    vm.AuditedComments = dr["AuditedComments"].ToString();

//                    vm.IsRejected = Convert.ToBoolean(dr["IsRejected"]);
//                    vm.RejectedBy = dr["RejectedBy"].ToString();
//                    vm.RejectedDate = Ordinary.StringToDate(dr["RejectedDate"].ToString());
//                    vm.RejectedComments = dr["RejectedComments"].ToString();

//                    VMs.Add(vm);
//                }
//                dr.Close();

//                #endregion SqlExecution

//                if (Vtransaction == null && transaction != null)
//                {
//                    transaction.Commit();
//                }
//                #endregion
//            }
//            #region catch
//            catch (SqlException sqlex)
//            {
//                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
//            }
//            catch (Exception ex)
//            {
//                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
//            }
//            #endregion
//            #region finally
//            finally
//            {
//                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
//                {
//                    currConn.Close();
//                }
//            }
//            #endregion
//            return VMs;
//        }

        //==================SelectAll=================

        public List<NewTransactionViewModel> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<NewTransactionViewModel> VMs = new List<NewTransactionViewModel>();
            NewTransactionViewModel vm;
            #endregion
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionSageGL();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region sql statement
                #region SqlText

                sqlText = @"
SELECT
ft.Id
,ft.BranchId
,ft.Code
,ft.AccountId
,a.Name AccountName
,a.Code AccountCode
,ft.TransactionDateTime
,ISNULL(ft.SubTotal,0)SubTotal
,ISNULL(ft.VATAmount,0)VATAmount
,a.Name AccountName
,br.Name BranchName

FROM NewTransactions  ft
LEFT OUTER JOIN GLAccounts a on ft.AccountId=a.id 
LEFT OUTER JOIN GLBranchs br ON ft.BranchId = br.Id
WHERE  1=1  AND ft.IsActive = 1
";


                if (Id > 0)
                {
                    sqlText += @" and ft.Id=@Id";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }
                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]))
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }

                if (Id > 0)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new NewTransactionViewModel();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.BranchName = dr["BranchName"].ToString();
                    vm.AccountId = Convert.ToInt32(dr["AccountId"]);
                    vm.Code = dr["Code"].ToString();
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.AccountName = dr["AccountName"].ToString();
                    vm.SubTotal = Convert.ToDecimal(dr["SubTotal"]);
                    vm.VATAmount = Convert.ToDecimal(dr["VATAmount"]);
                    //vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    VMs.Add(vm);
                }
                dr.Close();

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return VMs;
        }
        //==================Insert =================
        public string[] Insert(NewTransactionViewModel vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertNewTransaction"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            #endregion
            #region Try
            try
            {
                #region Validation
                #endregion Validation
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionSageGL();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region Save
                vm.Id = _cDal.NextId("NewTransactions", currConn, transaction);
                #region Code Generate
                GLCodeDAL codedal = new GLCodeDAL();
                vm.Code = codedal.NextCodeAcc("NewTransactions", vm.BranchId, Convert.ToDateTime(vm.TransactionDateTime), "EXP", currConn, transaction);
                #endregion Code Generate

                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO NewTransactions(
Id
,BranchId
,Code
,AccountId
,TransactionDateTime
,SubTotal
,VATAmount,
IsActive
) VALUES (
 @Id
,@BranchId
,@Code
,@AccountId
,@TransactionDateTime
,@SubTotal
,@VATAmount
,@IsActive
) 
";
                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Parameters.AddWithValue("@Code", vm.Code);
                    cmdInsert.Parameters.AddWithValue("@AccountId", vm.AccountId);
                    cmdInsert.Parameters.AddWithValue("@TransactionDateTime", Ordinary.DateToString(vm.TransactionDateTime));
                    cmdInsert.Parameters.AddWithValue("@SubTotal", vm.SubTotal);
                    cmdInsert.Parameters.AddWithValue("@VATAmount", vm.VATAmount);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update NewTransactions.", "");
                    }
                    #endregion SqlExecution
                    #region insert Details from Master into Detail Table
                    #region Form A
                    NewTransactionDetailDAL _formADAL = new NewTransactionDetailDAL();
                    if (vm.NewTransactionDetailVMs != null && vm.NewTransactionDetailVMs.Count > 0)
                    {
                        foreach (var eeTransactionDVM in vm.NewTransactionDetailVMs)
                        {
                            NewTransactionDetailViewModel dVM = new NewTransactionDetailViewModel();
                            dVM = eeTransactionDVM;
                            dVM.NewTransactionId = vm.Id;
                            dVM.BranchId = vm.BranchId;
                            retResults = _formADAL.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                    }
                    #endregion Form A


                    #endregion insert Details from Master into Detail Table
                 
                }
                else
                {
                    retResults[1] = "This NewTransaction already used!";
                    throw new ArgumentNullException("Please Input NewTransaction Value", "");
                }


                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
            }

            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }

        //==================Update =================
        public string[] Update(NewTransactionViewModel vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "NewTransactionUpdate"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionSageGL();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("Update"); }
                #endregion open connection and transaction

                if (vm != null)
                {

                    #region Update Settings
                    #region SqlText
                    sqlText = "";
                    sqlText = "UPDATE NewTransactions SET";
                    sqlText += "  TransactionDateTime=@TransactionDateTime";
                    sqlText += " , AccountId=@AccountId";

                    sqlText += " , SubTotal=@SubTotal";
                    sqlText += " , VATAmount=@VATAmount";
                    sqlText += " WHERE Id=@Id";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@AccountId", vm.AccountId);
                    cmdUpdate.Parameters.AddWithValue("@TransactionDateTime", Ordinary.DateToString(vm.TransactionDateTime));

                    cmdUpdate.Parameters.AddWithValue("@SubTotal", vm.SubTotal);
                    cmdUpdate.Parameters.AddWithValue("@VATAmount", vm.VATAmount);
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update NewTransactions.", "");
                    }
                    #endregion SqlExecution
                    #region insert Details from Master into Detail Table
                    #region Form A

                    NewTransactionDetailDAL _formADal = new NewTransactionDetailDAL();
                    if (vm.NewTransactionDetailVMs != null && vm.NewTransactionDetailVMs.Count > 0)
                    {
                        #region Delete Detail
                        try
                        {
                            retResults = _cDal.DeleteTableInformation(vm.Id.ToString(), "NewTransactionDetails", "NewTransactionId", currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }

                        }
                        catch (Exception)
                        {
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        #endregion Delete Detail
                        #region Insert Detail Again
                        foreach (var formAVM in vm.NewTransactionDetailVMs)
                        {
                            NewTransactionDetailViewModel dVM = new NewTransactionDetailViewModel();
                            dVM = formAVM;
                            dVM.NewTransactionId = vm.Id;
                            dVM.BranchId = vm.BranchId;
                            retResults = _formADal.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        #endregion Insert Detail Again
                    }
                    #endregion Form A          
                    #endregion insert Details from Master into Detail Table
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("NewTransaction Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("NewTransaction Update", "Could not found any item.");
                }
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return retResults;
        }
        ////==================Delete =================
        public string[] Delete(NewTransactionViewModel vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteNewTransaction"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string retVal = "";
            #endregion
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionSageGL();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("Delete"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {

                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update NewTransactions set";
                        sqlText += " IsActive=@IsActive";
                        sqlText += " where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsActive", false);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("NewTransaction Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("NewTransaction Information Delete", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Delete Successfully.";
                retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return retResults;
        }
        #endregion
    }
}
