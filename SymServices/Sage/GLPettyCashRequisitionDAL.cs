using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymOrdinary;
using SymServices.Common;
using SymViewModel.Sage;
using System.Threading;
using System.Web;
using System.IO;

namespace SymServices.Sage
{
    public class GLPettyCashRequisitionDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        public static Thread thread;
        #endregion
        #region Methods
        //==================DropDown=================
        public List<GLPettyCashRequisitionVM> DropDown(int branchId = 0)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<GLPettyCashRequisitionVM> VMs = new List<GLPettyCashRequisitionVM>();
            GLPettyCashRequisitionVM vm;
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
   FROM GLPettyCashRequisitions af
WHERE  1=1 AND af.IsArchive = 0
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
                    vm = new GLPettyCashRequisitionVM();
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

        //==================SelectAllSelfApprove=================
        public List<GLPettyCashRequisitionVM> SelectAllSelfApprove(int Id = 0, int UserId = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLPettyCashRequisitionVM> VMs = new List<GLPettyCashRequisitionVM>();
            GLPettyCashRequisitionVM vm;
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
--declare @UserId int
--set @UserId=3
select a.*, br.Name BranchName from(
SELECT
ft.Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,ft.BranchId,ft.Code,ft.TransactionDateTime
,ft.CommissionBillNo
,ft.ReferenceNo1,ft.ReferenceNo2,ft.ReferenceNo3,ft.Post,ft.Remarks,ft.IsActive,ft.IsArchive,ft.CreatedBy,ft.CreatedAt,ft.CreatedFrom,ft.LastUpdateBy,ft.LastUpdateAt,ft.LastUpdateFrom,ft.PostedBy,ft.PostedAt,ft.PostedFrom,ft.IsApprovedL1,ft.ApprovedByL1,ft.ApprovedDateL1,ft.CommentsL1,ft.IsApprovedL2,ft.ApprovedByL2,ft.ApprovedDateL2,ft.CommentsL2,ft.IsApprovedL3,ft.ApprovedByL3,ft.ApprovedDateL3,ft.CommentsL3,ft.IsApprovedL4,ft.ApprovedByL4,ft.ApprovedDateL4,ft.CommentsL4,ft.IsAudited,ft.AuditedBy,ft.AuditedDate,ft.AuditedComments,ft.IsRejected,ft.RejectedBy,ft.RejectedDate,ft.RejectedComments
FROM GLPettyCashRequisitions  ft
left outer join GLUsers u on u.id=@UserId
WHERE  1=1  
and u.HaveApprovalLevel1=1
and u.HaveExpenseRequisitionApproval=1
AND ft.IsArchive = 0
and ft.Post=1 and ft.IsRejected=0
and ft.IsApprovedL1=0
union all

SELECT
ft.Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,ft.BranchId,ft.Code,ft.TransactionDateTime
,ft.CommissionBillNo
,ft.ReferenceNo1,ft.ReferenceNo2,ft.ReferenceNo3,ft.Post,ft.Remarks,ft.IsActive,ft.IsArchive,ft.CreatedBy,ft.CreatedAt,ft.CreatedFrom,ft.LastUpdateBy,ft.LastUpdateAt,ft.LastUpdateFrom,ft.PostedBy,ft.PostedAt,ft.PostedFrom,ft.IsApprovedL1,ft.ApprovedByL1,ft.ApprovedDateL1,ft.CommentsL1,ft.IsApprovedL2,ft.ApprovedByL2,ft.ApprovedDateL2,ft.CommentsL2,ft.IsApprovedL3,ft.ApprovedByL3,ft.ApprovedDateL3,ft.CommentsL3,ft.IsApprovedL4,ft.ApprovedByL4,ft.ApprovedDateL4,ft.CommentsL4,ft.IsAudited,ft.AuditedBy,ft.AuditedDate,ft.AuditedComments,ft.IsRejected,ft.RejectedBy,ft.RejectedDate,ft.RejectedComments

FROM GLPettyCashRequisitions    ft
left outer join GLUsers u on u.id=@UserId
WHERE  1=1  
and u.HaveApprovalLevel2=1
and u.HaveExpenseRequisitionApproval=1
AND  ft.IsArchive = 0
and  ft.Post=1 and  ft.IsRejected=0
and  ft.IsApprovedL1=1 and  ft.IsApprovedL2=0
union all

SELECT
ft.Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approv Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,ft.BranchId,ft.Code,ft.TransactionDateTime
,ft.CommissionBillNo
,ft.ReferenceNo1,ft.ReferenceNo2,ft.ReferenceNo3,ft.Post,ft.Remarks,ft.IsActive,ft.IsArchive,ft.CreatedBy,ft.CreatedAt,ft.CreatedFrom,ft.LastUpdateBy,ft.LastUpdateAt,ft.LastUpdateFrom,ft.PostedBy,ft.PostedAt,ft.PostedFrom,ft.IsApprovedL1,ft.ApprovedByL1,ft.ApprovedDateL1,ft.CommentsL1,ft.IsApprovedL2,ft.ApprovedByL2,ft.ApprovedDateL2,ft.CommentsL2,ft.IsApprovedL3,ft.ApprovedByL3,ft.ApprovedDateL3,ft.CommentsL3,ft.IsApprovedL4,ft.ApprovedByL4,ft.ApprovedDateL4,ft.CommentsL4,ft.IsAudited,ft.AuditedBy,ft.AuditedDate,ft.AuditedComments,ft.IsRejected,ft.RejectedBy,ft.RejectedDate,ft.RejectedComments

FROM GLPettyCashRequisitions   ft
left outer join GLUsers u on u.id=@UserId
WHERE  1=1  
and u.HaveApprovalLevel3=1
and u.HaveExpenseRequisitionApproval=1
AND  ft.IsArchive = 0
and Post=1 and IsRejected=0
and IsApprovedL1=1 and IsApprovedL2=1 and IsApprovedL3=0
union all
SELECT
ft.Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,ft.BranchId,ft.Code,ft.TransactionDateTime
,ft.CommissionBillNo
,ft.ReferenceNo1,ft.ReferenceNo2,ft.ReferenceNo3,ft.Post,ft.Remarks,ft.IsActive,ft.IsArchive,ft.CreatedBy,ft.CreatedAt,ft.CreatedFrom,ft.LastUpdateBy,ft.LastUpdateAt,ft.LastUpdateFrom,ft.PostedBy,ft.PostedAt,ft.PostedFrom,ft.IsApprovedL1,ft.ApprovedByL1,ft.ApprovedDateL1,ft.CommentsL1,ft.IsApprovedL2,ft.ApprovedByL2,ft.ApprovedDateL2,ft.CommentsL2,ft.IsApprovedL3,ft.ApprovedByL3,ft.ApprovedDateL3,ft.CommentsL3,ft.IsApprovedL4,ft.ApprovedByL4,ft.ApprovedDateL4,ft.CommentsL4,ft.IsAudited,ft.AuditedBy,ft.AuditedDate,ft.AuditedComments,ft.IsRejected,ft.RejectedBy,ft.RejectedDate,ft.RejectedComments
FROM GLPettyCashRequisitions    ft
left outer join GLUsers u on u.id=@UserId
WHERE  1=1  
and u.HaveApprovalLevel4=1
and u.HaveExpenseRequisitionApproval=1
AND  ft.IsArchive = 0
and Post=1 and  ft.IsRejected=0
and IsApprovedL1=1 and IsApprovedL2=1 and IsApprovedL3=1 and IsApprovedL4=0
  
) as a
LEFT OUTER JOIN GLBranchs br ON a.BranchId = br.Id
WHERE  1=1
";


                if (Id > 0)
                {
                    sqlText += @" and a.Id=@Id";
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
                objComm.Parameters.AddWithValue("@UserId", UserId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {

                    vm = new GLPettyCashRequisitionVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Status = dr["Status"].ToString();
                    vm.MyStatus = dr["MyStatus"].ToString();
                    vm.BranchName = dr["BranchName"].ToString();

                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.Code = dr["Code"].ToString();
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());

                    vm.CommissionBillNo = dr["CommissionBillNo"].ToString();
                    vm.ReferenceNo1 = dr["ReferenceNo1"].ToString();
                    vm.ReferenceNo2 = dr["ReferenceNo2"].ToString();
                    vm.ReferenceNo3 = dr["ReferenceNo3"].ToString();
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.PostedBy = dr["PostedBy"].ToString();
                    vm.PostedAt = Ordinary.StringToDate(dr["PostedAt"].ToString());
                    vm.PostedFrom = dr["PostedFrom"].ToString();

                    vm.IsApprovedL1 = Convert.ToBoolean(dr["IsApprovedL1"]);
                    vm.ApprovedByL1 = dr["ApprovedByL1"].ToString();
                    vm.ApprovedDateL1 = Ordinary.StringToDate(dr["ApprovedDateL1"].ToString());
                    vm.CommentsL1 = dr["CommentsL1"].ToString();

                    vm.IsApprovedL2 = Convert.ToBoolean(dr["IsApprovedL2"]);
                    vm.ApprovedByL2 = dr["ApprovedByL2"].ToString();
                    vm.ApprovedDateL2 = Ordinary.StringToDate(dr["ApprovedDateL2"].ToString());
                    vm.CommentsL2 = dr["CommentsL2"].ToString();

                    vm.IsApprovedL3 = Convert.ToBoolean(dr["IsApprovedL3"]);
                    vm.ApprovedByL3 = dr["ApprovedByL3"].ToString();
                    vm.ApprovedDateL3 = Ordinary.StringToDate(dr["ApprovedDateL3"].ToString());
                    vm.CommentsL3 = dr["CommentsL3"].ToString();

                    vm.IsApprovedL4 = Convert.ToBoolean(dr["IsApprovedL4"]);
                    vm.ApprovedByL4 = dr["ApprovedByL4"].ToString();
                    vm.ApprovedDateL4 = Ordinary.StringToDate(dr["ApprovedDateL4"].ToString());
                    vm.CommentsL4 = dr["CommentsL4"].ToString();

                    vm.IsApprovedL1 = Convert.ToBoolean(dr["IsApprovedL1"]);
                    vm.ApprovedByL1 = dr["ApprovedByL1"].ToString();
                    vm.ApprovedDateL1 = Ordinary.StringToDate(dr["ApprovedDateL1"].ToString());
                    vm.CommentsL1 = dr["CommentsL1"].ToString();

                    vm.IsAudited = Convert.ToBoolean(dr["IsAudited"]);
                    vm.AuditedBy = dr["AuditedBy"].ToString();
                    vm.AuditedDate = Ordinary.StringToDate(dr["AuditedDate"].ToString());
                    vm.AuditedComments = dr["AuditedComments"].ToString();

                    vm.IsRejected = Convert.ToBoolean(dr["IsRejected"]);
                    vm.RejectedBy = dr["RejectedBy"].ToString();
                    vm.RejectedDate = Ordinary.StringToDate(dr["RejectedDate"].ToString());
                    vm.RejectedComments = dr["RejectedComments"].ToString();

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

        //==================SelectAllPosted=================
        public List<GLPettyCashRequisitionVM> SelectAllPosted(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLPettyCashRequisitionVM> VMs = new List<GLPettyCashRequisitionVM>();
            GLPettyCashRequisitionVM vm;
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
select a.*, br.Name BranchName from(
SELECT
Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Approved Level4'when IsApprovedL2=1 then 'Approved Level3'when IsApprovedL1=1 then 'Approved Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,BranchId,Code,TransactionDateTime
,CommissionBillNo
,ReferenceNo1,ReferenceNo2,ReferenceNo3,Post
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,PostedBy,PostedAt,PostedFrom,IsApprovedL1,ApprovedByL1,ApprovedDateL1,CommentsL1,IsApprovedL2,ApprovedByL2,ApprovedDateL2,CommentsL2,IsApprovedL3,ApprovedByL3,ApprovedDateL3,CommentsL3,IsApprovedL4,ApprovedByL4,ApprovedDateL4,CommentsL4,IsAudited,AuditedBy,AuditedDate,AuditedComments,IsRejected,RejectedBy,RejectedDate,RejectedComments
FROM GLPettyCashRequisitions  
WHERE  1=1  AND IsArchive = 0
 and Post=1 and IsRejected=1
 

 union all
SELECT
Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,BranchId,Code,TransactionDateTime
,CommissionBillNo
,ReferenceNo1,ReferenceNo2,ReferenceNo3,Post
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,PostedBy,PostedAt,PostedFrom,IsApprovedL1,ApprovedByL1,ApprovedDateL1,CommentsL1,IsApprovedL2,ApprovedByL2,ApprovedDateL2,CommentsL2,IsApprovedL3,ApprovedByL3,ApprovedDateL3,CommentsL3,IsApprovedL4,ApprovedByL4,ApprovedDateL4,CommentsL4,IsAudited,AuditedBy,AuditedDate,AuditedComments,IsRejected,RejectedBy,RejectedDate,RejectedComments
FROM GLPettyCashRequisitions  
WHERE  1=1  AND IsArchive = 0
 and Post=1 and IsRejected=0
 and IsApprovedL1=0

 union all

 SELECT
Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,BranchId,Code,TransactionDateTime
,CommissionBillNo
,ReferenceNo1,ReferenceNo2,ReferenceNo3,Post
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,PostedBy,PostedAt,PostedFrom,IsApprovedL1,ApprovedByL1,ApprovedDateL1,CommentsL1,IsApprovedL2,ApprovedByL2,ApprovedDateL2,CommentsL2,IsApprovedL3,ApprovedByL3,ApprovedDateL3,CommentsL3,IsApprovedL4,ApprovedByL4,ApprovedDateL4,CommentsL4,IsAudited,AuditedBy,AuditedDate,AuditedComments,IsRejected,RejectedBy,RejectedDate,RejectedComments
FROM GLPettyCashRequisitions  
WHERE  1=1  AND IsArchive = 0
and Post=1 and IsRejected=0
 and IsApprovedL1=1 and IsApprovedL2=0

 union all

 SELECT
Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approv Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,BranchId,Code,TransactionDateTime
,CommissionBillNo
,ReferenceNo1,ReferenceNo2,ReferenceNo3,Post
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,PostedBy,PostedAt,PostedFrom,IsApprovedL1,ApprovedByL1,ApprovedDateL1,CommentsL1,IsApprovedL2,ApprovedByL2,ApprovedDateL2,CommentsL2,IsApprovedL3,ApprovedByL3,ApprovedDateL3,CommentsL3,IsApprovedL4,ApprovedByL4,ApprovedDateL4,CommentsL4,IsAudited,AuditedBy,AuditedDate,AuditedComments,IsRejected,RejectedBy,RejectedDate,RejectedComments
FROM GLPettyCashRequisitions  
WHERE  1=1  AND IsArchive = 0
and Post=1 and IsRejected=0
 and IsApprovedL1=1 and IsApprovedL2=1 and IsApprovedL3=0
 
  union all

 SELECT
Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,BranchId,Code,TransactionDateTime
,CommissionBillNo
,ReferenceNo1,ReferenceNo2,ReferenceNo3,Post
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,PostedBy,PostedAt,PostedFrom,IsApprovedL1,ApprovedByL1,ApprovedDateL1,CommentsL1,IsApprovedL2,ApprovedByL2,ApprovedDateL2,CommentsL2,IsApprovedL3,ApprovedByL3,ApprovedDateL3,CommentsL3,IsApprovedL4,ApprovedByL4,ApprovedDateL4,CommentsL4,IsAudited,AuditedBy,AuditedDate,AuditedComments,IsRejected,RejectedBy,RejectedDate,RejectedComments
FROM GLPettyCashRequisitions  
WHERE  1=1  AND IsArchive = 0
and Post=1 and IsRejected=0
 and IsApprovedL1=1 and IsApprovedL2=1 and IsApprovedL3=1 and IsApprovedL4=0

  union all

 SELECT
Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,BranchId,Code,TransactionDateTime
,CommissionBillNo
,ReferenceNo1,ReferenceNo2,ReferenceNo3,Post
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,PostedBy,PostedAt,PostedFrom,IsApprovedL1,ApprovedByL1,ApprovedDateL1,CommentsL1,IsApprovedL2,ApprovedByL2,ApprovedDateL2,CommentsL2,IsApprovedL3,ApprovedByL3,ApprovedDateL3,CommentsL3,IsApprovedL4,ApprovedByL4,ApprovedDateL4,CommentsL4,IsAudited,AuditedBy,AuditedDate,AuditedComments,IsRejected,RejectedBy,RejectedDate,RejectedComments
FROM GLPettyCashRequisitions  
WHERE  1=1  AND IsArchive = 0
and Post=1 and IsRejected=0
 and IsApprovedL1=1 and IsApprovedL2=1 and IsApprovedL3=1 and IsApprovedL4=1
 ) as a
LEFT OUTER JOIN GLBranchs br ON a.BranchId = br.Id
WHERE  1=1
";


                if (Id > 0)
                {
                    sqlText += @" and a.Id=@Id";
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
                    vm = new GLPettyCashRequisitionVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Status = dr["Status"].ToString();
                    vm.MyStatus = dr["MyStatus"].ToString();

                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.BranchName = dr["BranchName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());

                    vm.CommissionBillNo = dr["CommissionBillNo"].ToString();
                    vm.ReferenceNo1 = dr["ReferenceNo1"].ToString();
                    vm.ReferenceNo2 = dr["ReferenceNo2"].ToString();
                    vm.ReferenceNo3 = dr["ReferenceNo3"].ToString();
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    vm.CreatedAt = Ordinary.StringToDate(Ordinary.StringToDate(dr["CreatedAt"].ToString()));
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(Ordinary.StringToDate(dr["LastUpdateAt"].ToString()));
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.PostedBy = dr["PostedBy"].ToString();
                    vm.PostedAt = Ordinary.StringToDate(dr["PostedAt"].ToString());
                    vm.PostedFrom = dr["PostedFrom"].ToString();

                    vm.IsApprovedL1 = Convert.ToBoolean(dr["IsApprovedL1"]);
                    vm.ApprovedByL1 = dr["ApprovedByL1"].ToString();
                    vm.ApprovedDateL1 = Ordinary.StringToDate(dr["ApprovedDateL1"].ToString());
                    vm.CommentsL1 = dr["CommentsL1"].ToString();

                    vm.IsApprovedL2 = Convert.ToBoolean(dr["IsApprovedL2"]);
                    vm.ApprovedByL2 = dr["ApprovedByL2"].ToString();
                    vm.ApprovedDateL2 = Ordinary.StringToDate(dr["ApprovedDateL2"].ToString());
                    vm.CommentsL2 = dr["CommentsL2"].ToString();

                    vm.IsApprovedL3 = Convert.ToBoolean(dr["IsApprovedL3"]);
                    vm.ApprovedByL3 = dr["ApprovedByL3"].ToString();
                    vm.ApprovedDateL3 = Ordinary.StringToDate(dr["ApprovedDateL3"].ToString());
                    vm.CommentsL3 = dr["CommentsL3"].ToString();

                    vm.IsApprovedL4 = Convert.ToBoolean(dr["IsApprovedL4"]);
                    vm.ApprovedByL4 = dr["ApprovedByL4"].ToString();
                    vm.ApprovedDateL4 = Ordinary.StringToDate(dr["ApprovedDateL4"].ToString());
                    vm.CommentsL4 = dr["CommentsL4"].ToString();

                    vm.IsApprovedL1 = Convert.ToBoolean(dr["IsApprovedL1"]);
                    vm.ApprovedByL1 = dr["ApprovedByL1"].ToString();
                    vm.ApprovedDateL1 = Ordinary.StringToDate(dr["ApprovedDateL1"].ToString());
                    vm.CommentsL1 = dr["CommentsL1"].ToString();

                    vm.IsAudited = Convert.ToBoolean(dr["IsAudited"]);
                    vm.AuditedBy = dr["AuditedBy"].ToString();
                    vm.AuditedDate = Ordinary.StringToDate(dr["AuditedDate"].ToString());
                    vm.AuditedComments = dr["AuditedComments"].ToString();

                    vm.IsRejected = Convert.ToBoolean(dr["IsRejected"]);
                    vm.RejectedBy = dr["RejectedBy"].ToString();
                    vm.RejectedDate = Ordinary.StringToDate(dr["RejectedDate"].ToString());
                    vm.RejectedComments = dr["RejectedComments"].ToString();

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
        //==================SelectAllAudit=================
        public List<GLPettyCashRequisitionVM> SelectAllAudit(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLPettyCashRequisitionVM> VMs = new List<GLPettyCashRequisitionVM>();
            GLPettyCashRequisitionVM vm;
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
distinct m.Id
,m.Code
,m.BranchId
,m.CreatedBy
,m.LastUpdateBy
,m.PostedBy
,m.ApprovedByL1
,m.ApprovedByL2
,m.ApprovedByL3
,m.ApprovedByL4
,m.AuditedBy
,m.RejectedBy

FROM GLPettyCashRequisitions m 
LEFT OUTER JOIN GLPettyCashRequisitionFormAs fa ON fa.GLPettyCashRequisitionId = m.Id
LEFT OUTER JOIN GLPettyCashRequisitionFormBs fb ON fb.GLPettyCashRequisitionId = m.Id
WHERE  1=1  AND m.IsArchive = 0

";


                if (Id > 0)
                {
                    sqlText += @" and m.Id=@Id";
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
                    vm = new GLPettyCashRequisitionVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.PostedBy = dr["PostedBy"].ToString();
                    vm.ApprovedByL1 = dr["ApprovedByL1"].ToString();
                    vm.ApprovedByL2 = dr["ApprovedByL2"].ToString();
                    vm.ApprovedByL3 = dr["ApprovedByL3"].ToString();
                    vm.ApprovedByL4 = dr["ApprovedByL4"].ToString();
                    vm.AuditedBy = dr["AuditedBy"].ToString();
                    vm.RejectedBy = dr["RejectedBy"].ToString();
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

        //==================SelectAll=================
        public List<GLPettyCashRequisitionVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLPettyCashRequisitionVM> VMs = new List<GLPettyCashRequisitionVM>();
            GLPettyCashRequisitionVM vm;
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
pc.Id
,pc.BranchId
,pc.Code
,pc.TransactionDateTime
,ISNULL(pc.CommissionBillNo,0)CommissionBillNo

,pc.ReferenceNo1
,pc.ReferenceNo2
,pc.ReferenceNo3
,ISNULL(pc.OpeningBalance       ,0)OpeningBalance
,ISNULL(pc.TotalExpense         ,0)TotalExpense
,ISNULL(pc.TotalAgencyCommission,0)TotalAgencyCommission
,ISNULL(pc.TotalRequisition     ,0)TotalRequisition
,ISNULL(pc.FundReceive          ,0)FundReceive
,ISNULL(pc.NextOpening          ,0)NextOpening

,ISNULL(pc.RequisitionExpense          ,0)RequisitionExpense
,ISNULL(pc.RequisitionBankCharge          ,0)RequisitionBankCharge
,ISNULL(pc.RequisitionContingency          ,0)RequisitionContingency
,ISNULL(pc.RequisitionAgencyCommission          ,0)RequisitionAgencyCommission


,ISNULL(pc.MadeJournal          ,0)MadeJournal

,pc.Post
,pc.Remarks
,pc.IsActive
,pc.IsArchive
,pc.CreatedBy
,pc.CreatedAt
,pc.CreatedFrom
,pc.LastUpdateBy
,pc.LastUpdateAt
,pc.LastUpdateFrom
,pc.PostedBy
,pc.PostedAt
,pc.PostedFrom

,pc.IsApprovedL1
,pc.ApprovedByL1
,pc.ApprovedDateL1
,pc.CommentsL1
,pc.IsApprovedL2
,pc.ApprovedByL2
,pc.ApprovedDateL2
,pc.CommentsL2
,pc.IsApprovedL3
,pc.ApprovedByL3
,pc.ApprovedDateL3
,pc.CommentsL3
,pc.IsApprovedL4
,pc.ApprovedByL4
,pc.ApprovedDateL4
,pc.CommentsL4
,pc.IsAudited
,pc.AuditedBy
,pc.AuditedDate
,pc.AuditedComments
,pc.IsRejected
,pc.RejectedBy
,pc.RejectedDate
,pc.RejectedComments

,br.Name BranchName
,case
when Post = 0 then 'Waiting for Posting' 
when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' 
when IsApprovedL3=1 then 'Waiting for Approve Level4' when IsApprovedL2=1 then 'Waiting for Approve Level3'
when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level 1'
end as [Status]


FROM GLPettyCashRequisitions pc 
LEFT OUTER JOIN GLBranchs br ON pc.BranchId = br.Id
WHERE  1=1  AND pc.IsArchive = 0

";


                if (Id > 0)
                {
                    sqlText += @" and pc.Id=@Id";
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
                    vm = new GLPettyCashRequisitionVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.BranchName = dr["BranchName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());

                    vm.CommissionBillNo = dr["CommissionBillNo"].ToString();
                    vm.ReferenceNo1 = dr["ReferenceNo1"].ToString();
                    vm.ReferenceNo2 = dr["ReferenceNo2"].ToString();
                    vm.ReferenceNo3 = dr["ReferenceNo3"].ToString();
                    vm.Post = Convert.ToBoolean(dr["Post"]);

                    vm.OpeningBalance = Convert.ToDecimal(dr["OpeningBalance"]);
                    vm.TotalExpense = Convert.ToDecimal(dr["TotalExpense"]);
                    vm.TotalAgencyCommission = Convert.ToDecimal(dr["TotalAgencyCommission"]);
                    vm.TotalRequisition = Convert.ToDecimal(dr["TotalRequisition"]);
                    vm.FundReceive = Convert.ToDecimal(dr["FundReceive"]);
                    vm.NextOpening = Convert.ToDecimal(dr["NextOpening"]);

                    vm.RequisitionExpense = Convert.ToDecimal(dr["RequisitionExpense"]);
                    vm.RequisitionBankCharge = Convert.ToDecimal(dr["RequisitionBankCharge"]);
                    vm.RequisitionContingency = Convert.ToDecimal(dr["RequisitionContingency"]);
                    vm.RequisitionAgencyCommission = Convert.ToDecimal(dr["RequisitionAgencyCommission"]);


                    vm.MadeJournal = Convert.ToBoolean(dr["MadeJournal"]);


                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.PostedBy = dr["PostedBy"].ToString();
                    vm.PostedAt = dr["PostedAt"].ToString();
                    vm.PostedFrom = dr["PostedFrom"].ToString();

                    vm.IsApprovedL1 = Convert.ToBoolean(dr["IsApprovedL1"]);
                    vm.ApprovedByL1 = dr["ApprovedByL1"].ToString();
                    vm.ApprovedDateL1 = Ordinary.StringToDate(dr["ApprovedDateL1"].ToString());
                    vm.CommentsL1 = dr["CommentsL1"].ToString();

                    vm.IsApprovedL2 = Convert.ToBoolean(dr["IsApprovedL2"]);
                    vm.ApprovedByL2 = dr["ApprovedByL2"].ToString();
                    vm.ApprovedDateL2 = Ordinary.StringToDate(dr["ApprovedDateL2"].ToString());
                    vm.CommentsL2 = dr["CommentsL2"].ToString();

                    vm.IsApprovedL3 = Convert.ToBoolean(dr["IsApprovedL3"]);
                    vm.ApprovedByL3 = dr["ApprovedByL3"].ToString();
                    vm.ApprovedDateL3 = Ordinary.StringToDate(dr["ApprovedDateL3"].ToString());
                    vm.CommentsL3 = dr["CommentsL3"].ToString();

                    vm.IsApprovedL4 = Convert.ToBoolean(dr["IsApprovedL4"]);
                    vm.ApprovedByL4 = dr["ApprovedByL4"].ToString();
                    vm.ApprovedDateL4 = Ordinary.StringToDate(dr["ApprovedDateL4"].ToString());
                    vm.CommentsL4 = dr["CommentsL4"].ToString();

                    vm.IsApprovedL1 = Convert.ToBoolean(dr["IsApprovedL1"]);
                    vm.ApprovedByL1 = dr["ApprovedByL1"].ToString();
                    vm.ApprovedDateL1 = Ordinary.StringToDate(dr["ApprovedDateL1"].ToString());
                    vm.CommentsL1 = dr["CommentsL1"].ToString();

                    vm.IsAudited = Convert.ToBoolean(dr["IsAudited"]);
                    vm.AuditedBy = dr["AuditedBy"].ToString();
                    vm.AuditedDate = Ordinary.StringToDate(dr["AuditedDate"].ToString());
                    vm.AuditedComments = dr["AuditedComments"].ToString();

                    vm.IsRejected = Convert.ToBoolean(dr["IsRejected"]);
                    vm.RejectedBy = dr["RejectedBy"].ToString();
                    vm.RejectedDate = Ordinary.StringToDate(dr["RejectedDate"].ToString());
                    vm.RejectedComments = dr["RejectedComments"].ToString();
                    vm.Status = dr["Status"].ToString();
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

        //==================ExistCheckCommissionBillNo=================
        public string ExistCheckCommissionBillNo(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string retResults = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
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

                GLPettyCashRequisitionVM vm = SelectAll(0, conditionFields, conditionValues, currConn, transaction).FirstOrDefault();
                if (vm != null)
                {
                    retResults = "This Commission Bill No. " + vm.CommissionBillNo + " already used! Code: " + vm.Code + " Date: " + vm.TransactionDateTime + " Please Select Another!";
                }


                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

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
            return retResults;
        }

        //==================Insert =================
        public string[] Insert(GLPettyCashRequisitionVM vm, List<HttpPostedFileBase> UploadFiles, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertGLPettyCashRequisition"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string existResult = "";
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


                vm.Id = _cDal.NextId("GLPettyCashRequisitions", currConn, transaction);
                #region Code Generate
                GLCodeDAL codedal = new GLCodeDAL();
                vm.Code = codedal.NextCodeAcc("GLPettyCashRequisitions", vm.BranchId, Convert.ToDateTime(vm.TransactionDateTime), "PCR", currConn, transaction);
                #endregion Code Generate
                if (vm != null)
                {
                    #region ExistCheck
                    if (vm.glPettyCashRequisitionFormBVMs != null && vm.glPettyCashRequisitionFormBVMs.Count > 0)
                    {
                        vm.CommissionBillNo = vm.glPettyCashRequisitionFormBVMs.FirstOrDefault().CommissionBillNo;
                    }


                    if (!string.IsNullOrWhiteSpace(vm.CommissionBillNo))
                    {
                        string[] cFields = { "pc.CommissionBillNo", "pc.IsRejected", "pc.BranchId" };
                        string[] cValues = { vm.CommissionBillNo, "0", vm.BranchId.ToString() };
                        existResult = ExistCheckCommissionBillNo(cFields, cValues, currConn, transaction);
                        if (!string.IsNullOrWhiteSpace(existResult))
                        {
                            retResults[1] = existResult;
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }
                    #endregion
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO GLPettyCashRequisitions(
Id
,BranchId
,Code
,TransactionDateTime
,CommissionBillNo
,ReferenceNo1
,ReferenceNo2
,ReferenceNo3
,Post
,OpeningBalance
,TotalExpense
,TotalAgencyCommission
,TotalRequisition
,FundReceive
,NextOpening
,MadeJournal

,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,IsApprovedL1
,ApprovedByL1
,ApprovedDateL1
,CommentsL1
,IsApprovedL2
,ApprovedByL2
,ApprovedDateL2
,CommentsL2
,IsApprovedL3
,ApprovedByL3
,ApprovedDateL3
,CommentsL3
,IsApprovedL4
,ApprovedByL4
,ApprovedDateL4
,CommentsL4
,IsAudited
,AuditedBy
,AuditedDate
,AuditedComments
,IsRejected
,RejectedBy
,RejectedDate
,RejectedComments


) VALUES (
@Id
,@BranchId
,@Code
,@TransactionDateTime
,@CommissionBillNo
,@ReferenceNo1
,@ReferenceNo2
,@ReferenceNo3
,@Post
,@OpeningBalance
,@TotalExpense
,@TotalAgencyCommission
,@TotalRequisition
,@FundReceive
,@NextOpening
,@MadeJournal

,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
,@IsApprovedL1
,@ApprovedByL1
,@ApprovedDateL1
,@CommentsL1
,@IsApprovedL2
,@ApprovedByL2
,@ApprovedDateL2
,@CommentsL2
,@IsApprovedL3
,@ApprovedByL3
,@ApprovedDateL3
,@CommentsL3
,@IsApprovedL4
,@ApprovedByL4
,@ApprovedDateL4
,@CommentsL4
,@IsAudited
,@AuditedBy
,@AuditedDate
,@AuditedComments
,@IsRejected
,@RejectedBy
,@RejectedDate
,@RejectedComments

) 
";


                    #endregion SqlText
                    #region SqlExecution
                    #region Get BranchId from BranchCode for AgentCommission - Needed or Not Needed

                    #endregion


                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Parameters.AddWithValue("@Code", vm.Code);
                    cmdInsert.Parameters.AddWithValue("@TransactionDateTime", Ordinary.DateToString(vm.TransactionDateTime));

                    cmdInsert.Parameters.AddWithValue("@CommissionBillNo", vm.CommissionBillNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ReferenceNo1", vm.ReferenceNo1 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ReferenceNo2", vm.ReferenceNo2 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ReferenceNo3", vm.ReferenceNo3 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Post", false);

                    cmdInsert.Parameters.AddWithValue("@OpeningBalance", vm.OpeningBalance);
                    cmdInsert.Parameters.AddWithValue("@TotalExpense", vm.TotalExpense);
                    cmdInsert.Parameters.AddWithValue("@TotalAgencyCommission", vm.TotalAgencyCommission);
                    cmdInsert.Parameters.AddWithValue("@TotalRequisition", vm.TotalRequisition);
                    cmdInsert.Parameters.AddWithValue("@FundReceive", vm.FundReceive);
                    cmdInsert.Parameters.AddWithValue("@NextOpening", vm.NextOpening);
                    cmdInsert.Parameters.AddWithValue("@MadeJournal", false);


                    cmdInsert.Parameters.AddWithValue("@IsApprovedL1", false);
                    cmdInsert.Parameters.AddWithValue("@ApprovedByL1", "0");
                    cmdInsert.Parameters.AddWithValue("@ApprovedDateL1", "19000101");
                    cmdInsert.Parameters.AddWithValue("@CommentsL1", vm.CommentsL1 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsApprovedL2", false);
                    cmdInsert.Parameters.AddWithValue("@ApprovedByL2", "0");
                    cmdInsert.Parameters.AddWithValue("@ApprovedDateL2", "19000101");
                    cmdInsert.Parameters.AddWithValue("@CommentsL2", vm.CommentsL2 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsApprovedL3", false);
                    cmdInsert.Parameters.AddWithValue("@ApprovedByL3", "0");
                    cmdInsert.Parameters.AddWithValue("@ApprovedDateL3", "19000101");
                    cmdInsert.Parameters.AddWithValue("@CommentsL3", vm.CommentsL3 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsApprovedL4", false);
                    cmdInsert.Parameters.AddWithValue("@ApprovedByL4", "0");
                    cmdInsert.Parameters.AddWithValue("@ApprovedDateL4", "19000101");
                    cmdInsert.Parameters.AddWithValue("@CommentsL4", vm.CommentsL4 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsAudited", false);
                    cmdInsert.Parameters.AddWithValue("@AuditedBy", "0");
                    cmdInsert.Parameters.AddWithValue("@AuditedDate", "19000101");
                    cmdInsert.Parameters.AddWithValue("@AuditedComments", vm.AuditedComments ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsRejected", false);
                    cmdInsert.Parameters.AddWithValue("@RejectedBy", "0");
                    cmdInsert.Parameters.AddWithValue("@RejectedDate", "19000101");
                    cmdInsert.Parameters.AddWithValue("@RejectedComments", vm.RejectedComments ?? Convert.DBNull);

                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update GLPettyCashRequisitions.", "");
                    }
                    #endregion SqlExecution

                    #region insert Details from Master into Detail Table
                    GLPettyCashRequisitionFormADAL _formADAL = new GLPettyCashRequisitionFormADAL();
                    List<GLPettyCashRequisitionFormAVM> VMs = new List<GLPettyCashRequisitionFormAVM>();

                    #region Form A
                    {
                        if (vm.glPettyCashRequisitionFormAVMs != null && vm.glPettyCashRequisitionFormAVMs.Count > 0)
                        {
                            foreach (var detailVM in vm.glPettyCashRequisitionFormAVMs)
                            {
                                GLPettyCashRequisitionFormAVM dVM = new GLPettyCashRequisitionFormAVM();
                                dVM = detailVM;
                                dVM.GLPettyCashRequisitionId = vm.Id;
                                dVM.TransactionDateTime = vm.TransactionDateTime;
                                dVM.BranchId = vm.BranchId;
                                VMs.Add(dVM);
                            }

                            if (VMs != null && VMs.Count > 0)
                            {
                                retResults = _formADAL.Insert(VMs, currConn, transaction);
                                if (retResults[0] == "Fail")
                                {
                                    throw new ArgumentNullException(retResults[1], "");
                                }
                            }

                        }
                    }
                    #endregion Form A
                    #region Form AB
                    {
                        VMs = new List<GLPettyCashRequisitionFormAVM>();
                        if (vm.glPettyCashRequisitionFormBankChargeVMs != null && vm.glPettyCashRequisitionFormBankChargeVMs.Count > 0)
                        {
                            foreach (var detailVM in vm.glPettyCashRequisitionFormBankChargeVMs)
                            {
                                GLPettyCashRequisitionFormAVM dVM = new GLPettyCashRequisitionFormAVM();
                                dVM = detailVM;
                                dVM.GLPettyCashRequisitionId = vm.Id;
                                dVM.TransactionDateTime = vm.TransactionDateTime;
                                dVM.BranchId = vm.BranchId;
                                VMs.Add(dVM);
                            }
                            if (VMs != null && VMs.Count > 0)
                            {
                                retResults = _formADAL.Insert(VMs, currConn, transaction);
                                if (retResults[0] == "Fail")
                                {
                                    throw new ArgumentNullException(retResults[1], "");
                                }
                            }

                        }
                    }
                    #endregion Form AB
                    #region Form AC
                    {
                        VMs = new List<GLPettyCashRequisitionFormAVM>();
                        if (vm.glPettyCashRequisitionFormExpectedFundVMs != null && vm.glPettyCashRequisitionFormExpectedFundVMs.Count > 0)
                        {
                            foreach (var detailVM in vm.glPettyCashRequisitionFormExpectedFundVMs)
                            {
                                GLPettyCashRequisitionFormAVM dVM = new GLPettyCashRequisitionFormAVM();
                                dVM = detailVM;
                                dVM.GLPettyCashRequisitionId = vm.Id;
                                dVM.TransactionDateTime = vm.TransactionDateTime;
                                dVM.BranchId = vm.BranchId;
                                VMs.Add(dVM);
                            }
                            if (VMs != null && VMs.Count > 0)
                            {
                                retResults = _formADAL.Insert(VMs, currConn, transaction);
                                if (retResults[0] == "Fail")
                                {
                                    throw new ArgumentNullException(retResults[1], "");
                                }
                            }
                        }
                    }
                    #endregion Form AC
                    #region Form B
                    GLPettyCashRequisitionFormBDAL _formBDAL = new GLPettyCashRequisitionFormBDAL();
                    List<GLPettyCashRequisitionFormBVM> formBVMs = new List<GLPettyCashRequisitionFormBVM>();

                    if (vm.glPettyCashRequisitionFormBVMs != null && vm.glPettyCashRequisitionFormBVMs.Count > 0)
                    {
                        foreach (var detailVM in vm.glPettyCashRequisitionFormBVMs)
                        {
                            GLPettyCashRequisitionFormBVM dVM = new GLPettyCashRequisitionFormBVM();
                            dVM = detailVM;
                            dVM.GLPettyCashRequisitionId = vm.Id;
                            dVM.TransactionDateTime = vm.TransactionDateTime;
                            dVM.BranchId = vm.BranchId;
                            dVM.CommissionBillNo = vm.CommissionBillNo;

                            dVM.CreatedAt = vm.CreatedAt;
                            dVM.CreatedBy = vm.CreatedBy;
                            dVM.CreatedFrom = vm.CreatedFrom;

                            formBVMs.Add(dVM);
                        }
                        if (formBVMs != null && formBVMs.Count > 0)
                        {
                            retResults = _formBDAL.Insert(formBVMs, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                    }
                    #endregion Form B
                    #region FileUpload

                    GLPettyCashRequisitionFileDAL _formFileDAL = new GLPettyCashRequisitionFileDAL();

                    List<GLPettyCashRequisitionFileVM> fileVMs = new List<GLPettyCashRequisitionFileVM>();

                    foreach (HttpPostedFileBase file in UploadFiles)
                    {
                        if (file != null && Array.Exists(vm.FilesToBeUploaded.Split(','), s => s.Equals(file.FileName)))
                        {
                            if (file != null && file.ContentLength > 0)
                            {
                                GLPettyCashRequisitionFileVM fileVM = new GLPettyCashRequisitionFileVM();
                                string path = vm.Id.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHMMss") + "_" + Path.GetFileName(file.FileName);

                                string saveFilePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Files/Sage/PettyCashRequisition"), path);

                                file.SaveAs(saveFilePath);

                                fileVM.GLPettyCashRequisitionId = vm.Id;
                                fileVM.FileName = path;
                                fileVM.FileOrginalName = file.FileName;
                                fileVMs.Add(fileVM);
                            }
                        }
                    }
                    if (fileVMs != null && fileVMs.Count > 0)
                    {
                        retResults = _formFileDAL.Insert(fileVMs, currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                            throw new ArgumentNullException(retResults[1], "");
                        }

                    }
                    #endregion File Upload

                    #endregion insert Details from Master into Detail Table

                    #region Update Amount
                    GLPettyCashRequisitionVM newVM = new GLPettyCashRequisitionVM();
                    newVM.Id = vm.Id;
                    newVM.BranchId = vm.BranchId;
                    retResults = UpdateAmount(newVM, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion
                }
                else
                {
                    retResults[1] = "This GLPettyCashRequisition already used!";
                    throw new ArgumentNullException("Please Input GLPettyCashRequisition Value", "");
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
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
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
        public string[] Update(GLPettyCashRequisitionVM vm, List<HttpPostedFileBase> UploadFiles, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "GLPettyCashRequisitionUpdate"; //Method Name
            int transResult = 0;
            string existResult = "";
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
                    #region ExistCheck
                    if (vm.glPettyCashRequisitionFormBVMs != null && vm.glPettyCashRequisitionFormBVMs.Count > 0)
                    {
                        vm.CommissionBillNo = vm.glPettyCashRequisitionFormBVMs.FirstOrDefault().CommissionBillNo;
                    }
                    if (!string.IsNullOrWhiteSpace(vm.CommissionBillNo))
                    {
                        string[] cFields = { "pc.Id!", "pc.CommissionBillNo", "pc.IsRejected", "pc.BranchId" };
                        string[] cValues = { vm.Id.ToString(), vm.CommissionBillNo, "0", vm.BranchId.ToString() };
                        existResult = ExistCheckCommissionBillNo(cFields, cValues, currConn, transaction);
                        if (!string.IsNullOrWhiteSpace(existResult))
                        {
                            retResults[1] = existResult;
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }


                    #endregion
                    #region Update Settings
                    #region SqlText
                    sqlText = "";
                    sqlText = "UPDATE GLPettyCashRequisitions SET";
                    sqlText += "  TransactionDateTime=@TransactionDateTime";

                    sqlText += " , CommissionBillNo=@CommissionBillNo";
                    sqlText += " , ReferenceNo1=@ReferenceNo1";
                    sqlText += " , ReferenceNo2=@ReferenceNo2";
                    sqlText += " , ReferenceNo3=@ReferenceNo3";
                    sqlText += " , OpeningBalance=@OpeningBalance";
                    sqlText += " , TotalExpense=@TotalExpense";
                    sqlText += " , TotalAgencyCommission=@TotalAgencyCommission";
                    sqlText += " , TotalRequisition=@TotalRequisition";
                    sqlText += " , FundReceive=@FundReceive";
                    sqlText += " , NextOpening=@NextOpening";

                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " WHERE Id=@Id";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@TransactionDateTime", Ordinary.DateToString(vm.TransactionDateTime));
                    cmdUpdate.Parameters.AddWithValue("@CommissionBillNo", vm.CommissionBillNo ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ReferenceNo1", vm.ReferenceNo1 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ReferenceNo2", vm.ReferenceNo2 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ReferenceNo3", vm.ReferenceNo3 ?? Convert.DBNull);

                    cmdUpdate.Parameters.AddWithValue("@OpeningBalance", vm.OpeningBalance);
                    cmdUpdate.Parameters.AddWithValue("@TotalExpense", vm.TotalExpense);
                    cmdUpdate.Parameters.AddWithValue("@TotalAgencyCommission", vm.TotalAgencyCommission);
                    cmdUpdate.Parameters.AddWithValue("@TotalRequisition", vm.TotalRequisition);
                    cmdUpdate.Parameters.AddWithValue("@FundReceive", vm.FundReceive);
                    cmdUpdate.Parameters.AddWithValue("@NextOpening", vm.NextOpening);


                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update GLPettyCashRequisitions.", "");
                    }
                    #endregion SqlExecution
                    #region insert Details from Master into Detail Table
                    #region Form A, AB, AC
                    #region Delete Detail
                    try
                    {
                        retResults = _cDal.DeleteTableInformation(vm.Id.ToString(), "GLPettyCashRequisitionFormAs", "GLPettyCashRequisitionId", currConn, transaction);
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
                    GLPettyCashRequisitionFormADAL _formADAL = new GLPettyCashRequisitionFormADAL();
                    List<GLPettyCashRequisitionFormAVM> VMs = new List<GLPettyCashRequisitionFormAVM>();
                    #region Form A
                    if (vm.glPettyCashRequisitionFormAVMs != null && vm.glPettyCashRequisitionFormAVMs.Count > 0)
                    {
                        foreach (var formAVM in vm.glPettyCashRequisitionFormAVMs)
                        {
                            GLPettyCashRequisitionFormAVM dVM = new GLPettyCashRequisitionFormAVM();
                            dVM = formAVM;
                            dVM.GLPettyCashRequisitionId = vm.Id;
                            dVM.TransactionDateTime = vm.TransactionDateTime;
                            dVM.BranchId = vm.BranchId;
                            VMs.Add(dVM);
                        }
                        if (VMs != null && VMs.Count > 0)
                        {
                            retResults = _formADAL.Insert(VMs, currConn, transaction);

                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                    }
                    #endregion Form A
                    #region Form AB
                    {
                        VMs = new List<GLPettyCashRequisitionFormAVM>();

                        if (vm.glPettyCashRequisitionFormBankChargeVMs != null && vm.glPettyCashRequisitionFormBankChargeVMs.Count > 0)
                        {
                            foreach (var detailVM in vm.glPettyCashRequisitionFormBankChargeVMs)
                            {
                                GLPettyCashRequisitionFormAVM dVM = new GLPettyCashRequisitionFormAVM();
                                dVM = detailVM;
                                dVM.GLPettyCashRequisitionId = vm.Id;
                                dVM.TransactionDateTime = vm.TransactionDateTime;
                                dVM.BranchId = vm.BranchId;
                                VMs.Add(dVM);
                            }
                            if (VMs != null && VMs.Count > 0)
                            {
                                retResults = _formADAL.Insert(VMs, currConn, transaction);

                                if (retResults[0] == "Fail")
                                {
                                    throw new ArgumentNullException(retResults[1], "");
                                }
                            }
                        }
                    }
                    #endregion Form AB
                    #region Form AC
                    {
                        VMs = new List<GLPettyCashRequisitionFormAVM>();
                        if (vm.glPettyCashRequisitionFormExpectedFundVMs != null && vm.glPettyCashRequisitionFormExpectedFundVMs.Count > 0)
                        {
                            foreach (var detailVM in vm.glPettyCashRequisitionFormExpectedFundVMs)
                            {
                                GLPettyCashRequisitionFormAVM dVM = new GLPettyCashRequisitionFormAVM();
                                dVM = detailVM;
                                dVM.GLPettyCashRequisitionId = vm.Id;
                                dVM.TransactionDateTime = vm.TransactionDateTime;
                                dVM.BranchId = vm.BranchId;
                                VMs.Add(dVM);
                            }
                            if (VMs != null && VMs.Count > 0)
                            {
                                retResults = _formADAL.Insert(VMs, currConn, transaction);

                                if (retResults[0] == "Fail")
                                {
                                    throw new ArgumentNullException(retResults[1], "");
                                }
                            }
                        }
                    }
                    #endregion Form AC
                    #endregion Insert Detail Again
                    #endregion
                    #region Form B

                    GLPettyCashRequisitionFormBDAL _formBDal = new GLPettyCashRequisitionFormBDAL();
                    List<GLPettyCashRequisitionFormBVM> formBVMs = new List<GLPettyCashRequisitionFormBVM>();

                    if (vm.glPettyCashRequisitionFormBVMs != null && vm.glPettyCashRequisitionFormBVMs.Count > 0)
                    {
                        #region Delete Detail
                        try
                        {
                            retResults = _cDal.DeleteTableInformation(vm.Id.ToString(), "GLPettyCashRequisitionFormBs", "GLPettyCashRequisitionId", currConn, transaction);
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
                        foreach (var FormBVM in vm.glPettyCashRequisitionFormBVMs)
                        {
                            GLPettyCashRequisitionFormBVM dVM = new GLPettyCashRequisitionFormBVM();
                            dVM = FormBVM;
                            dVM.GLPettyCashRequisitionId = vm.Id;
                            dVM.TransactionDateTime = vm.TransactionDateTime;
                            dVM.BranchId = vm.BranchId;
                            dVM.CommissionBillNo = vm.CommissionBillNo;


                            dVM.CreatedAt = vm.LastUpdateAt;
                            dVM.CreatedBy = vm.LastUpdateBy;
                            dVM.CreatedFrom = vm.LastUpdateFrom;

                            formBVMs.Add(dVM);
                        }
                        if (formBVMs != null && formBVMs.Count > 0)
                        {
                            retResults = _formBDal.Insert(formBVMs, currConn, transaction);

                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        #endregion Insert Detail Again
                    }
                    #endregion Form B
                    #region FileUpload

                    GLPettyCashRequisitionFileDAL _formFileDAL = new GLPettyCashRequisitionFileDAL();
                    List<GLPettyCashRequisitionFileVM> fileVMs = new List<GLPettyCashRequisitionFileVM>();
                    foreach (HttpPostedFileBase file in UploadFiles)
                    {
                        if (file != null && Array.Exists(vm.FilesToBeUploaded.Split(','), s => s.Equals(file.FileName)))
                        {
                            if (file != null && file.ContentLength > 0)
                            {
                                GLPettyCashRequisitionFileVM fileVM = new GLPettyCashRequisitionFileVM();
                                string path = vm.Id.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHMMss") + "_" + Path.GetFileName(file.FileName);

                                string saveFilePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Files/Sage/PettyCashRequisition"), path);

                                file.SaveAs(saveFilePath);

                                fileVM.GLPettyCashRequisitionId = vm.Id;
                                fileVM.FileName = path;
                                fileVM.FileOrginalName = file.FileName;
                                fileVMs.Add(fileVM);
                            }
                        }
                    }
                    if (fileVMs != null && fileVMs.Count > 0)
                    {
                        retResults = _formFileDAL.Insert(fileVMs, currConn, transaction);

                        if (retResults[0] == "Fail")
                        {
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }
                    #endregion File Upload

                    #endregion insert Details from Master into Detail Table
                    #region Update Amount
                    GLPettyCashRequisitionVM newVM = new GLPettyCashRequisitionVM();
                    newVM.Id = vm.Id;
                    newVM.BranchId = vm.BranchId;
                    retResults = UpdateAmount(newVM, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("GLPettyCashRequisition Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLPettyCashRequisition Update", "Could not found any item.");
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
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
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
        public string[] Delete(GLPettyCashRequisitionVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLPettyCashRequisition"; //Method Name
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
                        sqlText = "update GLPettyCashRequisitions set";
                        sqlText += " IsActive=@IsActive";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " ,LastUpdateBy=@LastUpdateBy";
                        sqlText += " ,LastUpdateAt=@LastUpdateAt";
                        sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsActive", false);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("GLPettyCashRequisition Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLPettyCashRequisition Information Delete", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Delete Successfully.";
                #endregion SuccessResult
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
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

        public string[] Post(GLPettyCashRequisitionVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLPettyCashRequisition"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string retVal = "";
            string SendEmail = "";
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
                    #region Insert Into GLDocumentNos, GLMRNos From GLPettyCashRequisitionFormBs
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        GLPettyCashRequisitionFormBVM pcrFormBVM = new GLPettyCashRequisitionFormBVM();
                        List<GLPettyCashRequisitionFormBVM> pcrFormBVMs = new List<GLPettyCashRequisitionFormBVM>();
                        GLPettyCashRequisitionFormBDAL _pcrFormBDAL = new GLPettyCashRequisitionFormBDAL();
                        pcrFormBVMs = _pcrFormBDAL.SelectByMasterId(Convert.ToInt32(ids[i]), currConn, transaction);

                        GLDocumentNoDAL _glDocumentNoDAL = new GLDocumentNoDAL();
                        GLMRNoDAL _glMRNoDAL = new GLMRNoDAL();

                        List<GLDocumentNoVM> glDocumentNoVMs = new List<GLDocumentNoVM>();
                        List<GLMRNoVM> glMRNoVMs = new List<GLMRNoVM>();


                        foreach (GLPettyCashRequisitionFormBVM item in pcrFormBVMs)
                        {
                            if (item.Post)
                            {
                                retResults[1] = "Data already Posted!";
                                throw new ArgumentNullException(retResults[1], "");
                            }


                            #region MRNo
                            GLMRNoVM glMRNoVM = new GLMRNoVM();

                            glMRNoVM.Name = item.MRNo;
                            glMRNoVM.TransactionType = "PettyCash";
                            glMRNoVM.ReferenceId = Convert.ToInt32(ids[i]);
                            glMRNoVM.BranchId = item.BranchId;
                            glMRNoVM.CreatedBy = vm.PostedBy;
                            glMRNoVM.CreatedAt = vm.PostedAt;
                            glMRNoVM.CreatedFrom = vm.PostedBy;
                            glMRNoVMs.Add(glMRNoVM);
                            {
                                string[] cFields = { "m.Id!", "d.MRNo", "m.Post", "d.IsRejected" };
                                string[] cValues = { ids[i], item.MRNo, "1", "0" };
                                GLPettyCashRequisitionFormBVM formBVM = _pcrFormBDAL.SelectAllMRNo(glMRNoVM.TransactionType, item.MRNo, cFields, cValues, currConn, transaction).FirstOrDefault();
                                if (formBVM != null)
                                {
                                    retResults[1] = "MR No: " + item.MRNo + " already used! Code: " + formBVM.Code + " Date: " + formBVM.TransactionDateTime + " CommissionBillNo: " + formBVM.CommissionBillNo;
                                    throw new ArgumentNullException(retResults[1], "");
                                }
                            }
                            #endregion


                            #region DocumentNo
                            GLDocumentNoVM glDocumentNoVM = new GLDocumentNoVM();
                            glDocumentNoVM.Name = item.DocumentNo;
                            glDocumentNoVM.TransactionType = "PettyCash";
                            glDocumentNoVM.ReferenceId = Convert.ToInt32(ids[i]);
                            glDocumentNoVM.BranchId = item.BranchId;
                            glDocumentNoVM.CreatedBy = vm.PostedBy;
                            glDocumentNoVM.CreatedAt = vm.PostedAt;
                            glDocumentNoVM.CreatedFrom = vm.PostedBy;
                            glDocumentNoVMs.Add(glDocumentNoVM);
                            {
                                string[] cFields = { "m.Id!", "d.DocumentNo", "m.Post", "d.IsRejected" };
                                string[] cValues = { ids[i], item.DocumentNo, "1", "0" };
                                GLPettyCashRequisitionFormBVM formBVM = _pcrFormBDAL.SelectAllDocNo(glDocumentNoVM.TransactionType, item.DocumentNo, cFields, cValues, currConn, transaction).FirstOrDefault();
                                if (formBVM != null)
                                {
                                    retResults[1] = "Document No: " + item.DocumentNo + " already used! Code: " + formBVM.Code + " Date: " + formBVM.TransactionDateTime + " CommissionBillNo: " + formBVM.CommissionBillNo;
                                    throw new ArgumentNullException(retResults[1], "");
                                }
                            }

                            #endregion

                        }

                        if (glMRNoVMs != null && glMRNoVMs.Count > 0)
                        {
                            retResults = _glMRNoDAL.Insert(glMRNoVMs, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }

                        if (glDocumentNoVMs != null && glDocumentNoVMs.Count > 0)
                        {
                            retResults = _glDocumentNoDAL.Insert(glDocumentNoVMs, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }

                    }

                    #endregion
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update GLPettyCashRequisitions set";
                        sqlText += " Post=@Post";
                        sqlText += " ,PostedBy=@PostedBy";
                        sqlText += " ,PostedAt=@PostedAt";
                        sqlText += " ,PostedFrom=@PostedFrom";
                        sqlText += " where Id=@Id";
                        sqlText += " update GLPettyCashRequisitionFormAs set post=@Post where GLPettyCashRequisitionId=@Id";
                        sqlText += " update GLPettyCashRequisitionFormBs set post=@Post where GLPettyCashRequisitionId=@Id";


                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@Post", true);
                        cmdUpdate.Parameters.AddWithValue("@PostedBy", vm.PostedBy);
                        cmdUpdate.Parameters.AddWithValue("@PostedAt", vm.PostedAt);
                        cmdUpdate.Parameters.AddWithValue("@PostedFrom", vm.PostedFrom);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("GLPettyCashRequisition Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    #region Send Mail to the User
                    ////////if (retResults[0] != "Fail")
                    ////////{
                        SendEmail = new GLSettingDAL().settingValue("Email", "SendEmail", currConn, transaction);
                        if (SendEmail == "Y")
                        {
                            for (int i = 0; i < ids.Length - 1; i++)
                            {
                                //Prameters: (Branch = this Branch; HaveExpenseRequisitionApproval=1; HaveApprovalLevel1=1) 
                                string urlPrefix = "";
                                //urlPrefix = "http://localhost:50010";
                                urlPrefix = new GLSettingDAL().settingValue("Email", "PettyCashURLPrefix", currConn, transaction);
                                GLPettyCashRequisitionVM varVM = new GLPettyCashRequisitionVM();
                                varVM = SelectAll(Convert.ToInt32(ids[i]), null, null, currConn, transaction).FirstOrDefault();
                                string url = urlPrefix + "/Sage/PettyCashRequisition/Approve/" + varVM.Id;
                                List<GLUserVM> userVMs = new List<GLUserVM>();
                                GLUserVM userVM = new GLUserVM();
                                userVM.BranchId = varVM.BranchId;
                                string[] conditionFields = { "HaveExpenseRequisitionApproval", "HaveApprovalLevel1" };
                                string[] conditionValues = { "1", "1" };

                                userVMs = new GLUserDAL().SelectUserForMail(userVM, conditionFields, conditionValues, currConn, transaction);
                                GLEmailDAL _emailDAL = new GLEmailDAL();
                                GLEmailSettingVM emailSettingVM = new GLEmailSettingVM();
                                string status = "Generated and Waiting for Approve Level1";

                                foreach (var item in userVMs)
                                {
                                    string[] EmailForm = Ordinary.GDICEmailForm(item.FullName, varVM.Code, status, url, "PCReq");
                                    emailSettingVM.MailHeader = EmailForm[0];
                                    emailSettingVM.MailToAddress = item.Email;
                                    emailSettingVM.MailBody = EmailForm[1];

                                    thread = new Thread(c => _emailDAL.SendEmail(emailSettingVM, thread));
                                    thread.Start();
                                }
                            }
                        }
                    ////////}
                    #endregion
                }
                else
                {
                    throw new ArgumentNullException("GLPettyCashRequisition Information Delete", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    if (SendEmail == "Y")
                    {
                        retResults[1] = "Data Posted Successfully! And Email Sent to the Respective Persons!";
                    }
                    else
                    {
                        retResults[1] = "Data Posted Successfully!";
                    }
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
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

        public string[] RemoveFile(string id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "RemoveFileGLPettyCashRequisition"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Delete"); }
                #endregion open connection and transaction
                GLPettyCashRequisitionFileDAL _formFileDAL = new GLPettyCashRequisitionFileDAL();

                string FileName = _formFileDAL.SelectById(Convert.ToInt32(id), currConn, transaction).FirstOrDefault().FileName;
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Sage\\PettyCashRequisition\\" + FileName;
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                #region Update Settings

                sqlText = "";
                sqlText = " delete  GLPettyCashRequisitionFiles";
                sqlText += " where Id=@Id";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@Id", id);
                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                retResults[2] = "";// Return Id
                retResults[3] = sqlText; //  SQL Query
                #region Commit
                if (transResult <= 0)
                {
                    throw new ArgumentNullException("GLPettyCashRequisition Delete", " could not Delete.");
                }
                #endregion Commit
                #endregion Update Settings

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "File Removed Successfully.";
                }
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

        public string[] ApproveReject(GLPettyCashRequisitionVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLPettyCashRequisition"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string SendEmail = "";
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
                    GLUserDAL _uDal = new GLUserDAL();
                    GLUserVM uVM = new GLUserVM();
                    GLPettyCashRequisitionVM varVM = new GLPettyCashRequisitionVM();
                    string[] conditionFields = { "u.LogId" };
                    string[] conditionValues = { vm.ByName };
                    uVM = _uDal.SelectAll(0, conditionFields, conditionValues, currConn, transaction).FirstOrDefault();

                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        vm.Id = Convert.ToInt32(ids[i]);

                        if (vm.IsRejected)
                        {
                            #region Approval Level Check
                            varVM = SelectAll(vm.Id, null, null, currConn, transaction).FirstOrDefault();
                            if (varVM.IsApprovedL4)
                            {
                                retResults[1] = "After Approve Level 4 Cannot Reject!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                            #endregion
                            vm.MyStatus = "r";
                            retResults = ApproveReject(vm, vm.MyStatus, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                            #region Delete GLDocumentNos, GLMRNos by ReferenceId
                            retResults = _cDal.DeleteTableInformation(vm.Id.ToString(), "GLMRNos", "ReferenceId", currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                            retResults = _cDal.DeleteTableInformation(vm.Id.ToString(), "GLDocumentNos", "ReferenceId", currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                            #endregion
                        }
                        else
                        {
                            #region  HaveApprovalLevel1 Start
                            if (uVM.HaveApprovalLevel1)
                            {
                                varVM = SelectAll(vm.Id, null, null, currConn, transaction).FirstOrDefault();
                                if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == false)
                                {
                                    vm.MyStatus = "l1";
                                    retResults = ApproveReject(vm, vm.MyStatus, currConn, transaction);
                                    if (retResults[0] == "Fail")
                                    {
                                        throw new ArgumentNullException(retResults[1], "");
                                    }
                                    else
                                    {
                                        if (uVM.HaveApprovalLevel2)
                                        {
                                            varVM = SelectAll(vm.Id, null, null, currConn, transaction).FirstOrDefault();
                                            if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == false)
                                            {
                                                vm.MyStatus = "l2";
                                                retResults = ApproveReject(vm, vm.MyStatus, currConn, transaction);
                                                if (retResults[0] == "Fail")
                                                {
                                                    throw new ArgumentNullException(retResults[1], "");
                                                }
                                                else
                                                {
                                                    if (uVM.HaveApprovalLevel3)
                                                    {
                                                        varVM = SelectAll(vm.Id, null, null, currConn, transaction).FirstOrDefault();
                                                        if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == true && varVM.IsApprovedL3 == false)
                                                        {
                                                            vm.MyStatus = "l3";
                                                            retResults = ApproveReject(vm, vm.MyStatus, currConn, transaction);
                                                            if (retResults[0] == "Fail")
                                                            {
                                                                throw new ArgumentNullException(retResults[1], "");
                                                            }
                                                            else
                                                            {
                                                                if (uVM.HaveApprovalLevel4)
                                                                {
                                                                    varVM = SelectAll(vm.Id, null, null, currConn, transaction).FirstOrDefault();
                                                                    if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == true && varVM.IsApprovedL3 == true && varVM.IsApprovedL4 == false)
                                                                    {
                                                                        vm.MyStatus = "l4";
                                                                        retResults = ApproveReject(vm, vm.MyStatus, currConn, transaction);
                                                                        if (retResults[0] == "Fail")
                                                                        {
                                                                            throw new ArgumentNullException(retResults[1], "");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            #endregion  HaveApprovalLevel1 End
                            #region  HaveApprovalLevel2 Start

                            if (uVM.HaveApprovalLevel2)
                            {
                                varVM = SelectAll(vm.Id, null, null, currConn, transaction).FirstOrDefault();
                                if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == false)
                                {
                                    vm.MyStatus = "l2";
                                    retResults = ApproveReject(vm, vm.MyStatus, currConn, transaction);
                                    if (retResults[0] == "Fail")
                                    {
                                        throw new ArgumentNullException(retResults[1], "");
                                    }
                                    else
                                    {
                                        if (uVM.HaveApprovalLevel3)
                                        {
                                            varVM = SelectAll(vm.Id, null, null, currConn, transaction).FirstOrDefault();
                                            if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == true && varVM.IsApprovedL3 == false)
                                            {
                                                vm.MyStatus = "l3";
                                                retResults = ApproveReject(vm, vm.MyStatus, currConn, transaction);
                                                if (retResults[0] == "Fail")
                                                {
                                                    throw new ArgumentNullException(retResults[1], "");
                                                }
                                                else
                                                {
                                                    if (uVM.HaveApprovalLevel4)
                                                    {
                                                        varVM = SelectAll(vm.Id, null, null, currConn, transaction).FirstOrDefault();
                                                        if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == true && varVM.IsApprovedL3 == true && varVM.IsApprovedL4 == false)
                                                        {
                                                            vm.MyStatus = "l4";
                                                            retResults = ApproveReject(vm, vm.MyStatus, currConn, transaction);
                                                            if (retResults[0] == "Fail")
                                                            {
                                                                throw new ArgumentNullException(retResults[1], "");
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }


                            #endregion  HaveApprovalLevel2 End
                            #region  HaveApprovalLevel3 Start

                            if (uVM.HaveApprovalLevel3)
                            {
                                varVM = SelectAll(vm.Id, null, null, currConn, transaction).FirstOrDefault();
                                if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == true && varVM.IsApprovedL3 == false)
                                {
                                    vm.MyStatus = "l3";
                                    retResults = ApproveReject(vm, vm.MyStatus, currConn, transaction);
                                    if (retResults[0] == "Fail")
                                    {
                                        throw new ArgumentNullException(retResults[1], "");
                                    }
                                    else
                                    {
                                        if (uVM.HaveApprovalLevel4)
                                        {
                                            varVM = SelectAll(vm.Id, null, null, currConn, transaction).FirstOrDefault();
                                            if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == true && varVM.IsApprovedL3 == true && varVM.IsApprovedL4 == false)
                                            {
                                                vm.MyStatus = "l4";
                                                retResults = ApproveReject(vm, vm.MyStatus, currConn, transaction);
                                                if (retResults[0] == "Fail")
                                                {
                                                    throw new ArgumentNullException(retResults[1], "");
                                                }
                                            }
                                        }
                                    }
                                }
                            }


                            #endregion  HaveApprovalLevel3 End
                            #region  HaveApprovalLevel4 Start

                            if (uVM.HaveApprovalLevel4)
                            {
                                varVM = SelectAll(vm.Id, null, null, currConn, transaction).FirstOrDefault();
                                if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == true && varVM.IsApprovedL3 == true && varVM.IsApprovedL4 == false)
                                {
                                    vm.MyStatus = "l4";
                                    retResults = ApproveReject(vm, vm.MyStatus, currConn, transaction);
                                    if (retResults[0] == "Fail")
                                    {
                                        throw new ArgumentNullException(retResults[1], "");
                                    }
                                }
                            }
                            #endregion  HaveApprovalLevel4 End


                            #region Insert Into GLFundReceivedPettyCashRequisitions

                            varVM = SelectAll(vm.Id, null, null, currConn, transaction).FirstOrDefault();
                            #region Update Amount
                            GLPettyCashRequisitionVM newVM = new GLPettyCashRequisitionVM();
                            newVM.Id = vm.Id;
                            newVM.BranchId = varVM.BranchId;
                            retResults = UpdateAmount(newVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                            #endregion
                            varVM = SelectAll(vm.Id, null, null, currConn, transaction).FirstOrDefault();
                            if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == true && varVM.IsApprovedL3 == true && varVM.IsApprovedL4 == true)
                            {
                                GLFundReceivedPettyCashRequisitionVM frPCReqVM = new GLFundReceivedPettyCashRequisitionVM();
                                frPCReqVM.BranchId = varVM.BranchId;
                                frPCReqVM.ReferenceId = vm.Id;
                                frPCReqVM.FundAmount = varVM.FundReceive;
                                frPCReqVM.FinalApprovedDate = Ordinary.StringToDate(vm.NameDate);
                                //vm.Remarks = "";
                                frPCReqVM.CreatedAt = vm.NameDate;
                                frPCReqVM.CreatedBy = vm.ByName;
                                frPCReqVM.CreatedFrom = "";
                                retResults = new GLFundReceivedPettyCashRequisitionDAL().Insert(frPCReqVM);
                                if (retResults[0] == "Fail")
                                {
                                    throw new ArgumentNullException(retResults[1], "");
                                }
                            }
                            #endregion



                        }
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    #endregion Commit
                    #endregion Update Settings
                    #region Send Mail to the User
                    if (retResults[0] != "Fail")
                    {
                        SendEmail = new GLSettingDAL().settingValue("Email", "SendEmail", currConn, transaction);
                        if (SendEmail == "Y")
                        {
                            for (int i = 0; i < ids.Length - 1; i++)
                            {
                                //Prameters: (Branch = this Branch; HaveExpenseRequisitionApproval=1; HaveApprovalLevel1=1) 

                                #region Declarations
                                string urlPrefix = "";
                                //urlPrefix = "http://localhost:50010";
                                urlPrefix = new GLSettingDAL().settingValue("Email", "PettyCashURLPrefix", currConn, transaction);
                                string url = urlPrefix + "/Sage/PettyCashRequisition/SelfApproveIndex";

                                varVM = new GLPettyCashRequisitionVM();
                                varVM = SelectAll(Convert.ToInt32(ids[i]), null, null, currConn, transaction).FirstOrDefault();
                                List<GLUserVM> userVMs = new List<GLUserVM>();
                                GLUserVM userVM = new GLUserVM();
                                userVM.BranchId = varVM.BranchId;
                                GLEmailDAL _emailDAL = new GLEmailDAL();
                                GLEmailSettingVM emailSettingVM = new GLEmailSettingVM();
                                #endregion
                                #region Check Status and Select Users
                                string status = "";
                                //If Approval Completed/Rejected - Send Mail who created/posted
                                //If Posted/Approval 1,2,3 - Send Mail to Superior
                                if (varVM.IsRejected == true)
                                {
                                    status = "Rejected";
                                    string[] cFieldsUser = { "u.LogId" };
                                    string[] cValuesUser = { varVM.CreatedBy };
                                    userVMs = new GLUserDAL().SelectAll(0, cFieldsUser, cValuesUser, currConn, transaction);
                                    url = urlPrefix + "/Sage/PettyCashRequisition/Posted/" + varVM.Id;
                                }
                                else if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == true && varVM.IsApprovedL3 == true && varVM.IsApprovedL4 == true)
                                {
                                    status = "Approval Completed";
                                    string[] cFieldsUser = { "u.LogId" };
                                    string[] cValuesUser = { varVM.CreatedBy };
                                    userVMs = new GLUserDAL().SelectAll(0, cFieldsUser, cValuesUser, currConn, transaction);
                                    url = urlPrefix + "/Sage/PettyCashRequisition/Posted/" + varVM.Id;
                                }
                                else if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == true && varVM.IsApprovedL3 == true && varVM.IsApprovedL4 == false)
                                {
                                    status = "Waiting for Approve Level4";
                                    string[] cFieldsUser = { "u.HaveExpenseRequisitionApproval", "u.HaveApprovalLevel4" };
                                    string[] cValuesUser = { "1", "1" };
                                    userVMs = new GLUserDAL().SelectUserForMail(userVM, cFieldsUser, cValuesUser, currConn, transaction);
                                    url = urlPrefix + "/Sage/PettyCashRequisition/Approve/" + varVM.Id;
                                }
                                else if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == true && varVM.IsApprovedL3 == false)
                                {
                                    status = "Waiting for Approve Level3";
                                    string[] cFieldsUser = { "u.HaveExpenseRequisitionApproval", "u.HaveApprovalLevel3" };
                                    string[] cValuesUser = { "1", "1" };
                                    userVMs = new GLUserDAL().SelectUserForMail(userVM, cFieldsUser, cValuesUser, currConn, transaction);
                                    url = urlPrefix + "/Sage/PettyCashRequisition/Approve/" + varVM.Id;
                                }
                                else if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == false)
                                {
                                    status = "Waiting for Approve Level2";
                                    string[] cFieldsUser = { "u.HaveExpenseRequisitionApproval", "u.HaveApprovalLevel2" };
                                    string[] cValuesUser = { "1", "1" };
                                    userVMs = new GLUserDAL().SelectUserForMail(userVM, cFieldsUser, cValuesUser, currConn, transaction);
                                    url = urlPrefix + "/Sage/PettyCashRequisition/Approve/" + varVM.Id;
                                }
                                #endregion
                                #region Send Mail
                                foreach (var item in userVMs)
                                {
                                    string[] EmailForm = Ordinary.GDICEmailForm(item.FullName, varVM.Code, status, url, "PCReq");
                                    emailSettingVM.MailHeader = EmailForm[0];
                                    emailSettingVM.MailToAddress = item.Email;
                                    emailSettingVM.MailBody = EmailForm[1];

                                    thread = new Thread(c => _emailDAL.SendEmail(emailSettingVM, thread));
                                    thread.Start();
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    throw new ArgumentNullException("GLPettyCashRequisition Information Delete", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #region SuccessResult
                retResults[0] = "Success";
                if (SendEmail == "Y")
                {
                    retResults[1] = "Data Update Successfully! And Email Sent to the Respective Persons!";
                }
                else
                {
                    retResults[1] = retResults[1] + " ";
                }
                #endregion SuccessResult
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
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
        public string[] ApproveReject(GLPettyCashRequisitionVM vm, string Level, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLPettyCashRequisition"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Delete"); }
                #endregion open connection and transaction

                #region Update Settings
                sqlText = "";
                sqlText = "update GLPettyCashRequisitions set";
                if (Level.ToLower() == "l1")
                    sqlText += " IsApprovedL1=@IsApprovedL1,ApprovedByL1=@ApprovedByL1,ApprovedDateL1=@ApprovedDateL1,CommentsL1=@CommentsL1";
                else if (Level.ToLower() == "l2")
                    sqlText += " IsApprovedL2=@IsApprovedL2,ApprovedByL2=@ApprovedByL2,ApprovedDateL2=@ApprovedDateL2,CommentsL2=@CommentsL2";
                else if (Level.ToLower() == "l3")
                    sqlText += " IsApprovedL3=@IsApprovedL3,ApprovedByL3=@ApprovedByL3,ApprovedDateL3=@ApprovedDateL3,CommentsL3=@CommentsL3";
                else if (Level.ToLower() == "l4")
                    sqlText += " IsApprovedL4=@IsApprovedL4,ApprovedByL4=@ApprovedByL4,ApprovedDateL4=@ApprovedDateL4,CommentsL4=@CommentsL4";
                else if (Level.ToLower() == "r")
                    sqlText += " IsRejected=@IsRejected,RejectedBy=@RejectedBy,RejectedDate=@RejectedDate,RejectedComments=@RejectedComments";
                sqlText += " where Id=@Id ";

                if (Level.ToLower() == "r")
                {
                    sqlText += " update GLPettyCashRequisitionFormAs set";
                    sqlText += " IsRejected=@IsRejected,RejectedBy=@RejectedBy,RejectedDate=@RejectedDate,RejectedComments=@RejectedComments";
                    sqlText += " where GLPettyCashRequisitionId=@Id";

                    sqlText += " update GLPettyCashRequisitionFormBs set";
                    sqlText += " IsRejected=@IsRejected,RejectedBy=@RejectedBy,RejectedDate=@RejectedDate,RejectedComments=@RejectedComments";
                    sqlText += " where GLPettyCashRequisitionId=@Id";
                }
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                if (Level.ToLower() == "l1")
                {
                    cmdUpdate.Parameters.AddWithValue("@IsApprovedL1", true);
                    cmdUpdate.Parameters.AddWithValue("@ApprovedByL1", vm.ByName);
                    cmdUpdate.Parameters.AddWithValue("@ApprovedDateL1", vm.NameDate);
                    cmdUpdate.Parameters.AddWithValue("@CommentsL1", vm.NameComments ?? Convert.DBNull);
                }
                else if (Level.ToLower() == "l2")
                {
                    cmdUpdate.Parameters.AddWithValue("@IsApprovedL2", true);
                    cmdUpdate.Parameters.AddWithValue("@ApprovedByL2", vm.ByName);
                    cmdUpdate.Parameters.AddWithValue("@ApprovedDateL2", vm.NameDate);
                    cmdUpdate.Parameters.AddWithValue("@CommentsL2", vm.NameComments ?? Convert.DBNull);
                }
                else if (Level.ToLower() == "l3")
                {
                    cmdUpdate.Parameters.AddWithValue("@IsApprovedL3", true);
                    cmdUpdate.Parameters.AddWithValue("@ApprovedByL3", vm.ByName);
                    cmdUpdate.Parameters.AddWithValue("@ApprovedDateL3", vm.NameDate);
                    cmdUpdate.Parameters.AddWithValue("@CommentsL3", vm.NameComments ?? Convert.DBNull);
                }
                else if (Level.ToLower() == "l4")
                {
                    cmdUpdate.Parameters.AddWithValue("@IsApprovedL4", true);
                    cmdUpdate.Parameters.AddWithValue("@ApprovedByL4", vm.ByName);
                    cmdUpdate.Parameters.AddWithValue("@ApprovedDateL4", vm.NameDate);
                    cmdUpdate.Parameters.AddWithValue("@CommentsL4", vm.NameComments ?? Convert.DBNull);
                }
                else if (Level.ToLower() == "r")
                {
                    cmdUpdate.Parameters.AddWithValue("@IsRejected", true);
                    cmdUpdate.Parameters.AddWithValue("@RejectedBy", vm.ByName);
                    cmdUpdate.Parameters.AddWithValue("@RejectedDate", vm.NameDate);
                    cmdUpdate.Parameters.AddWithValue("@RejectedComments", vm.NameComments ?? Convert.DBNull);
                }

                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                retResults[2] = "";// Return Id
                retResults[3] = sqlText; //  SQL Query
                #region Commit
                if (transResult <= 0)
                {
                    throw new ArgumentNullException("GLPettyCashRequisition Delete", vm.Id + " could not Delete.");
                }
                #endregion Commit
                #endregion Update Settings
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #region SuccessResult
                retResults[0] = "Success";
                if (Level.ToLower() == "l1")
                {
                    retResults[1] = "Data Approved Level1 Successfully!";
                }
                else if (Level.ToLower() == "l2")
                {
                    retResults[1] = "Data Approved Level2 Successfully!";
                }
                else if (Level.ToLower() == "l3")
                {
                    retResults[1] = "Data Approved Level3 Successfully!";
                }
                else if (Level.ToLower() == "l4")
                {
                    retResults[1] = "Data Approved Level4 Successfully!";
                }
                else if (Level.ToLower() == "r")
                {
                    retResults[1] = "Data Rejected Successfully!";
                }

                #endregion SuccessResult
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
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

        ////==================Audit =================
        public string[] Audit(GLPettyCashRequisitionVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "AuditGLPettyCashRequisition"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Audit"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {

                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update GLPettyCashRequisitions set";
                        sqlText += " IsAudited=@IsAudited,AuditedBy=@AuditedBy,AuditedDate=@AuditedDate,AuditedComments=@AuditedComments";
                        sqlText += " where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsAudited", true);
                        cmdUpdate.Parameters.AddWithValue("@AuditedBy", vm.ByName);
                        cmdUpdate.Parameters.AddWithValue("@AuditedDate", vm.NameDate);
                        cmdUpdate.Parameters.AddWithValue("@AuditedComments", vm.NameComments ?? Convert.DBNull);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("GLPettyCashRequisition Audit", vm.Id + " could not Audit.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLPettyCashRequisition Information Audit", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Audit Successfully.";
                #endregion SuccessResult
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
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

        ////==================Report=================
        public DataTable Report(GLPettyCashRequisitionVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
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
pc.Id
,pc.BranchId
,pc.Code
,pc.TransactionDateTime
,pc.ReferenceNo1
,pc.ReferenceNo2
,pc.ReferenceNo3
,pc.Post

,ISNULL(pc.OpeningBalance         ,0)OpeningBalance
,ISNULL(pc.TotalExpense           ,0)TotalExpense
,ISNULL(pc.TotalAgencyCommission  ,0)TotalAgencyCommission
,ISNULL(pc.TotalRequisition       ,0)TotalRequisition
,ISNULL(pc.FundReceive            ,0)FundReceive
,ISNULL(pc.NextOpening            ,0)NextOpening


,ISNULL(pc.RequisitionExpense          ,0)RequisitionExpense
,ISNULL(pc.RequisitionBankCharge          ,0)RequisitionBankCharge
,ISNULL(pc.RequisitionContingency          ,0)RequisitionContingency
,ISNULL(pc.RequisitionAgencyCommission          ,0)RequisitionAgencyCommission

,pc.Remarks
,br.Name BranchName

,pc.CreatedBy
,pc.LastUpdateBy
,pc.PostedBy
,pc.ApprovedByL1
,pc.ApprovedByL2
,pc.ApprovedByL3
,pc.ApprovedByL4
,pc.AuditedBy
,pc.RejectedBy



FROM GLPettyCashRequisitions pc 
LEFT OUTER JOIN GLBranchs br ON pc.BranchId = br.Id
WHERE  1=1  AND pc.IsArchive = 0
";

                if (vm.Id > 0)
                {
                    sqlText += @" and pc.Id=@Id";
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

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

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
                        da.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }
                if (vm.Id > 0)
                    da.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);

                da.Fill(dt);
                dt = Ordinary.DtColumnStringToDate(dt, "TransactionDateTime");
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
            return dt;
        }

        public DataSet OpeningPettyCashRequesition(string TransactionId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet dt = new DataSet();
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
--declare @TransactionId as varchar(20)
declare @DateFrom as varchar(20)
declare @BranchId as varchar(20)
--set @TransactionId=5
declare @Opening as Decimal(18,2)
declare @LastRemitDate as varchar(20)
declare @LastRemitAmount as Decimal(18,2)
declare @PettyCashExp as Decimal(18,2)
declare @pcAmount as Decimal(18,2)
declare @NetPremium as Decimal(18,2)
declare @CommissionAmount as Decimal(18,2)
declare @AITAmount as Decimal(18,2)

select @DateFrom=TransactionDateTime,@BranchId=BranchId from GLPettyCashRequisitions where id=@TransactionId
  set @Opening=0;
 set @LastRemitDate=@DateFrom;
 set @LastRemitAmount=0;

 select @Opening=isnull(sum(FundAmount),0)  from GLFundReceivedPettyCashRequisitions m
 where m.IsReceived=1  
and m.ReceivedAt<@DateFrom
and m.BranchId=@BranchId

select @Opening=@Opening-isnull(sum(d.TransactionAmount),0)
 from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.Id=d.GLFinancialTransactionId
where m.IsApprovedL4=1 and m.IsRejected=0
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId

select @Opening=@Opening-isnull(sum(b.PCAmount),0)
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0 and b.IsRejected=0  and  b.IsFundReceived=1
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId

 select top 1 @LastRemitDate=FinalApprovedDate,@LastRemitAmount=FundAmount from GLFundReceivedPettyCashRequisitions
 where IsReceived=1 and  branchid=@BranchId 
 and ReferenceId<@TransactionId
select @Opening Opening,@LastRemitDate LastRemitDate,@LastRemitAmount LastRemitAmount




select distinct @PettyCashExp=sum(Amount)  from GLPettyCashRequisitionFormAs fa
where GLPettyCashRequisitionId =@TransactionId
and fa.IsRejected=0
group by GLPettyCashRequisitionId



select  @pcAmount=sum(pcAmount) ,@NetPremium=sum(NetPremium),@CommissionAmount=sum(CommissionAmount),@AITAmount=sum(AITAmount) 
from GLPettyCashRequisitionFormBs fb
where GLPettyCashRequisitionId =@TransactionId
and fb.IsRejected=0
group by GLPettyCashRequisitionId



select @PettyCashExp PettyCashExp ,@pcAmount pcAmount,@NetPremium NetPremium,@CommissionAmount CommissionAmount,@AITAmount AITAmount
 


";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

                da.SelectCommand.Parameters.AddWithValue("@TransactionId", TransactionId);

                da.Fill(dt);
                //dt = Ordinary.DtColumnStringToDate(dt, "LastRemitDate");
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
            return dt;
        }


        //==================UpdateAmount =================
        public string[] UpdateAmount(GLPettyCashRequisitionVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertGLPettyCashRequisition"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string existResult = "";
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
                if (vm != null)
                {
                    #region Update Amount
                    sqlText = " ";
                    sqlText += @"
DECLARE @dateFROM   varchar(20)
DECLARE @dateTo   varchar(20)

DECLARE @PreviousId   int
-------------------------DECLARE @Id   int
-------------------------DECLARE @BranchId   int

DECLARE @OpeningBalance  decimal(18, 3)
DECLARE @TotalExpense  decimal(18, 3)
DECLARE @TotalAgencyCommission  decimal(18, 3)
DECLARE @TotalRequisition  decimal(18, 3)
DECLARE @FundReceive  decimal(18, 3)
DECLARE @NextOpening  decimal(18, 3)

DECLARE @RequisitionExpense  decimal(18, 3)
DECLARE @RequisitionBankCharge  decimal(18, 3)
DECLARE @RequisitionContingency  decimal(18, 3)
DECLARE @RequisitionAgencyCommission  decimal(18, 3)



--SET @Id=3
--SET @BranchId=5
--------------@OpeningBalance--------------

SELECT TOP 1 @dateFROM=TransactionDateTime,@PreviousId=Id, @OpeningBalance = ISNULL(NextOpening,0)  FROM GLPettyCashRequisitions 
WHERE BranchId=@BranchId AND  Id<@Id AND Post=1 AND IsRejected=0
ORDER BY Id DESC

SELECT @dateTo=TransactionDateTime FROM GLPettyCashRequisitions WHERE  BranchId=@BranchId AND Id=@Id

SELECT @OpeningBalance = ISNULL(@OpeningBalance,0)

----------SELECT @dateFROM TransactionDateTime,@PreviousId PreviousId
----------SELECT @dateTo TransactionDateTime



--------------@TotalRequisition--------------
set @TotalRequisition = 0;

set @RequisitionExpense = 0;
set @RequisitionBankCharge = 0;
set @RequisitionContingency = 0;
set @RequisitionAgencyCommission = 0;


select @RequisitionExpense = ISNULL(SUM(Amount),0) From GLPettyCashRequisitionFormAs
WHERE 1=1 
AND BusinessNature = 'Expense'
AND BranchId=@BranchId
AND GLPettyCashRequisitionId = @Id
AND IsRejected = 0

select @RequisitionBankCharge = ISNULL(SUM(Amount),0) From GLPettyCashRequisitionFormAs
WHERE 1=1 
AND BusinessNature = 'BankCharge'
AND BranchId=@BranchId
AND GLPettyCashRequisitionId = @Id
AND IsRejected = 0

select @RequisitionContingency = ISNULL(SUM(Amount),0) From GLPettyCashRequisitionFormAs
WHERE 1=1 
AND BusinessNature = 'Contingency'
AND BranchId=@BranchId
AND GLPettyCashRequisitionId = @Id
AND IsRejected = 0


select @RequisitionAgencyCommission = Isnull(SUM(PCAmount),0) From GLPettyCashRequisitionFormBs
WHERE 1=1 AND BranchId=@BranchId
AND GLPettyCashRequisitionId = @Id
AND IsRejected = 0


select @TotalRequisition = @RequisitionExpense + @RequisitionBankCharge + @RequisitionContingency + @RequisitionAgencyCommission




--------------@FundReceive--------------

select @FundReceive = @TotalRequisition - @OpeningBalance


--------------@TotalExpense--------------
SELECT @TotalExpense = ISNULL(SUM(TransactionAmount),0) FROM GLFinancialTransactionDetails d
LEFT OUTER JOIN GLFinancialTransactions m ON m.Id = d.GLFinancialTransactionId
WHERE 1=1 AND m.BranchId=@BranchId
AND m.TransactionDateTime between @dateFROM AND @dateTo
AND m.Post=1 AND m.IsRejected=0 and m.TransactionType = 'Other'


--------------@TotalAgencyCommission--------------
SELECT @TotalAgencyCommission = ISNULL(SUM(TransactionAmount),0) FROM GLFinancialTransactionDetails d
LEFT OUTER JOIN GLFinancialTransactions m ON m.Id = d.GLFinancialTransactionId
WHERE 1=1 AND m.BranchId=@BranchId
AND m.TransactionDateTime between @dateFROM AND @dateTo
AND m.Post=1 AND m.IsRejected=0 and m.TransactionType = 'AgentCommission'


--------------@NextOpening--------------
select @NextOpening = @FundReceive - ( @TotalExpense + @TotalAgencyCommission)


--------SELECT @OpeningBalance OpeningBalance, @TotalExpense TotalExpense
--------, @TotalAgencyCommission TotalAgencyCommission, @TotalRequisition TotalRequisition
--------, @FundReceive FundReceive, @NextOpening NextOpening



";

                    sqlText += @"


UPDATE GLPettyCashRequisitions SET 
 OpeningBalance=@OpeningBalance
,TotalRequisition=@TotalRequisition
,FundReceive=@FundReceive
,TotalExpense=@TotalExpense
,TotalAgencyCommission=@TotalAgencyCommission
,NextOpening=@NextOpening
,RequisitionExpense=@RequisitionExpense
,RequisitionBankCharge=@RequisitionBankCharge
,RequisitionContingency=@RequisitionContingency
,RequisitionAgencyCommission=@RequisitionAgencyCommission


                                WHERE Id=@Id
";
                    {
                        SqlCommand cmdUpdateAmount = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdateAmount.Parameters.AddWithValue("@Id", vm.Id);
                        cmdUpdateAmount.Parameters.AddWithValue("@BranchId", vm.BranchId);
                        var exeRes = cmdUpdateAmount.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update amount.", "");
                        }
                    }
                    #endregion
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
                retResults[1] = "Amount Update Successfully.";
                retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
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

        #endregion
    }
}
