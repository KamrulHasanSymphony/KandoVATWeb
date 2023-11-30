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
    public class GLBDERequisitionPaidDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        public static Thread thread;
        #endregion
        #region Methods

        //==================DropDown=================
        public List<GLBDERequisitionPaidVM> DropDown(int branchId = 0)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<GLBDERequisitionPaidVM> VMs = new List<GLBDERequisitionPaidVM>();
            GLBDERequisitionPaidVM vm;
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
   FROM GLBDERequisitionPaids af
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
                    vm = new GLBDERequisitionPaidVM();
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
        public List<GLBDERequisitionPaidVM> SelectAllSelfApprove(int Id = 0, int UserId = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLBDERequisitionPaidVM> VMs = new List<GLBDERequisitionPaidVM>();
            GLBDERequisitionPaidVM vm;
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
,ft.BranchId,ft.Code,ft.TransactionDateTime,ft.ReferenceNo1,ft.ReferenceNo2,ft.ReferenceNo3,ft.Post,ft.Remarks,ft.IsActive,ft.IsArchive,ft.CreatedBy,ft.CreatedAt,ft.CreatedFrom,ft.LastUpdateBy,ft.LastUpdateAt,ft.LastUpdateFrom,ft.PostedBy,ft.PostedAt,ft.PostedFrom,ft.IsApprovedL1,ft.ApprovedByL1,ft.ApprovedDateL1,ft.CommentsL1,ft.IsApprovedL2,ft.ApprovedByL2,ft.ApprovedDateL2,ft.CommentsL2,ft.IsApprovedL3,ft.ApprovedByL3,ft.ApprovedDateL3,ft.CommentsL3,ft.IsApprovedL4,ft.ApprovedByL4,ft.ApprovedDateL4,ft.CommentsL4,ft.IsAudited,ft.AuditedBy,ft.AuditedDate,ft.AuditedComments,ft.IsRejected,ft.RejectedBy,ft.RejectedDate,ft.RejectedComments
FROM GLBDERequisitionPaids  ft
left outer join GLUsers u on u.id=@UserId
WHERE  1=1  
and u.HaveApprovalLevel1=1
and u.HaveBDERequisitionPaidApproval=1
AND ft.IsArchive = 0
and ft.Post=1 and ft.IsRejected=0
and ft.IsApprovedL1=0
union all

SELECT
ft.Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,ft.BranchId,ft.Code,ft.TransactionDateTime,ft.ReferenceNo1,ft.ReferenceNo2,ft.ReferenceNo3,ft.Post,ft.Remarks,ft.IsActive,ft.IsArchive,ft.CreatedBy,ft.CreatedAt,ft.CreatedFrom,ft.LastUpdateBy,ft.LastUpdateAt,ft.LastUpdateFrom,ft.PostedBy,ft.PostedAt,ft.PostedFrom,ft.IsApprovedL1,ft.ApprovedByL1,ft.ApprovedDateL1,ft.CommentsL1,ft.IsApprovedL2,ft.ApprovedByL2,ft.ApprovedDateL2,ft.CommentsL2,ft.IsApprovedL3,ft.ApprovedByL3,ft.ApprovedDateL3,ft.CommentsL3,ft.IsApprovedL4,ft.ApprovedByL4,ft.ApprovedDateL4,ft.CommentsL4,ft.IsAudited,ft.AuditedBy,ft.AuditedDate,ft.AuditedComments,ft.IsRejected,ft.RejectedBy,ft.RejectedDate,ft.RejectedComments

FROM GLBDERequisitionPaids    ft
left outer join GLUsers u on u.id=@UserId
WHERE  1=1  
and u.HaveApprovalLevel2=1
and u.HaveBDERequisitionPaidApproval=1
AND  ft.IsArchive = 0
and  ft.Post=1 and  ft.IsRejected=0
and  ft.IsApprovedL1=1 and  ft.IsApprovedL2=0
union all

SELECT
ft.Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approv Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,ft.BranchId,ft.Code,ft.TransactionDateTime,ft.ReferenceNo1,ft.ReferenceNo2,ft.ReferenceNo3,ft.Post,ft.Remarks,ft.IsActive,ft.IsArchive,ft.CreatedBy,ft.CreatedAt,ft.CreatedFrom,ft.LastUpdateBy,ft.LastUpdateAt,ft.LastUpdateFrom,ft.PostedBy,ft.PostedAt,ft.PostedFrom,ft.IsApprovedL1,ft.ApprovedByL1,ft.ApprovedDateL1,ft.CommentsL1,ft.IsApprovedL2,ft.ApprovedByL2,ft.ApprovedDateL2,ft.CommentsL2,ft.IsApprovedL3,ft.ApprovedByL3,ft.ApprovedDateL3,ft.CommentsL3,ft.IsApprovedL4,ft.ApprovedByL4,ft.ApprovedDateL4,ft.CommentsL4,ft.IsAudited,ft.AuditedBy,ft.AuditedDate,ft.AuditedComments,ft.IsRejected,ft.RejectedBy,ft.RejectedDate,ft.RejectedComments

FROM GLBDERequisitionPaids   ft
left outer join GLUsers u on u.id=@UserId
WHERE  1=1  
and u.HaveApprovalLevel3=1
and u.HaveBDERequisitionPaidApproval=1
AND  ft.IsArchive = 0
and Post=1 and IsRejected=0
and IsApprovedL1=1 and IsApprovedL2=1 and IsApprovedL3=0
union all
SELECT
ft.Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,ft.BranchId,ft.Code,ft.TransactionDateTime,ft.ReferenceNo1,ft.ReferenceNo2,ft.ReferenceNo3,ft.Post,ft.Remarks,ft.IsActive,ft.IsArchive,ft.CreatedBy,ft.CreatedAt,ft.CreatedFrom,ft.LastUpdateBy,ft.LastUpdateAt,ft.LastUpdateFrom,ft.PostedBy,ft.PostedAt,ft.PostedFrom,ft.IsApprovedL1,ft.ApprovedByL1,ft.ApprovedDateL1,ft.CommentsL1,ft.IsApprovedL2,ft.ApprovedByL2,ft.ApprovedDateL2,ft.CommentsL2,ft.IsApprovedL3,ft.ApprovedByL3,ft.ApprovedDateL3,ft.CommentsL3,ft.IsApprovedL4,ft.ApprovedByL4,ft.ApprovedDateL4,ft.CommentsL4,ft.IsAudited,ft.AuditedBy,ft.AuditedDate,ft.AuditedComments,ft.IsRejected,ft.RejectedBy,ft.RejectedDate,ft.RejectedComments
FROM GLBDERequisitionPaids    ft
left outer join GLUsers u on u.id=@UserId
WHERE  1=1  
and u.HaveApprovalLevel4=1
and u.HaveBDERequisitionPaidApproval=1
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

                    vm = new GLBDERequisitionPaidVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Status = dr["Status"].ToString();
                    vm.MyStatus = dr["MyStatus"].ToString();
                    vm.BranchName = dr["BranchName"].ToString();

                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.Code = dr["Code"].ToString();
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
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
        public List<GLBDERequisitionPaidVM> SelectAllPosted(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLBDERequisitionPaidVM> VMs = new List<GLBDERequisitionPaidVM>();
            GLBDERequisitionPaidVM vm;
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
select a.*,br.Name BranchName from(
SELECT
Id
,case when IsRejected=1 then 'Reject' when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,BranchId
,Code
,TransactionDateTime
,ReferenceNo1
,ReferenceNo2
,ReferenceNo3
,Post
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,PostedBy
,PostedAt
,PostedFrom

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
FROM GLBDERequisitionPaids  
WHERE  1=1  AND IsArchive = 0
 and Post=1 and IsRejected=1
 

 union all
SELECT
Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,BranchId
,Code
,TransactionDateTime
,ReferenceNo1
,ReferenceNo2
,ReferenceNo3
,Post
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,PostedBy
,PostedAt
,PostedFrom

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
FROM GLBDERequisitionPaids  
WHERE  1=1  AND IsArchive = 0
 and Post=1 and IsRejected=0
 and IsApprovedL1=0

 union all

 SELECT
Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,BranchId
,Code
,TransactionDateTime
,ReferenceNo1
,ReferenceNo2
,ReferenceNo3
,Post
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,PostedBy
,PostedAt
,PostedFrom

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
FROM GLBDERequisitionPaids  
WHERE  1=1  AND IsArchive = 0
and Post=1 and IsRejected=0
 and IsApprovedL1=1 and IsApprovedL2=0

 union all

 SELECT
Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,BranchId
,Code
,TransactionDateTime
,ReferenceNo1
,ReferenceNo2
,ReferenceNo3
,Post
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,PostedBy
,PostedAt
,PostedFrom

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
FROM GLBDERequisitionPaids  
WHERE  1=1  AND IsArchive = 0
and Post=1 and IsRejected=0
 and IsApprovedL1=1 and IsApprovedL2=1 and IsApprovedL3=0
 
  union all

 SELECT
Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,BranchId
,Code
,TransactionDateTime
,ReferenceNo1
,ReferenceNo2
,ReferenceNo3
,Post
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,PostedBy
,PostedAt
,PostedFrom

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
FROM GLBDERequisitionPaids  
WHERE  1=1  AND IsArchive = 0
and Post=1 and IsRejected=0
 and IsApprovedL1=1 and IsApprovedL2=1 and IsApprovedL3=1 and IsApprovedL4=0

  union all

 SELECT
Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,BranchId
,Code
,TransactionDateTime
,ReferenceNo1
,ReferenceNo2
,ReferenceNo3
,Post
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,PostedBy
,PostedAt
,PostedFrom

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
FROM GLBDERequisitionPaids  
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

                    vm = new GLBDERequisitionPaidVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Status = dr["Status"].ToString();
                    vm.MyStatus = dr["MyStatus"].ToString();
                    vm.BranchName = dr["BranchName"].ToString();

                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.Code = dr["Code"].ToString();
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
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
        //==================SelectAllAudit=================
        public List<GLBDERequisitionPaidVM> SelectAllAudit(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLBDERequisitionPaidVM> VMs = new List<GLBDERequisitionPaidVM>();
            GLBDERequisitionPaidVM vm;
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

FROM GLBDERequisitionPaids m 
LEFT OUTER JOIN GLBDERequisitionPaidFormAs fa ON fa.GLBDERequisitionPaidId = m.Id
LEFT OUTER JOIN GLBDERequisitionPaidFormBs fb ON fb.GLBDERequisitionPaidId = m.Id
LEFT OUTER JOIN GLBDERequisitionPaidFormCs fc ON fc.GLBDERequisitionPaidId = m.Id
LEFT OUTER JOIN GLBDERequisitionPaidFormDs fd ON fd.GLBDERequisitionPaidId = m.Id
LEFT OUTER JOIN GLBDERequisitionPaidFormEs fe ON fe.GLBDERequisitionPaidId = m.Id
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
                    vm = new GLBDERequisitionPaidVM();
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
        public List<GLBDERequisitionVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLBDERequisitionVM> VMs = new List<GLBDERequisitionVM>();
            GLBDERequisitionVM vm;
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
SELECT distinct
 bp.Id
,bp.BranchId
,bp.Code
,bp.TransactionDateTime
,bp.TransactionType
,bp.ReferenceNo1
,bp.ReferenceNo2
,bp.ReferenceNo3
,bp.Post
,bp.Remarks
,bp.IsActive
,bp.IsArchive
,bp.CreatedBy
,bp.CreatedAt
,bp.CreatedFrom
,bp.LastUpdateBy
,bp.LastUpdateAt
,bp.LastUpdateFrom
,bp.PostedBy
,bp.PostedAt
,bp.PostedFrom
,bp.IsApprovedL1
,bp.ApprovedByL1
,bp.ApprovedDateL1
,bp.CommentsL1
,bp.IsApprovedL2
,bp.ApprovedByL2
,bp.ApprovedDateL2
,bp.CommentsL2
,bp.IsApprovedL3
,bp.ApprovedByL3
,bp.ApprovedDateL3
,bp.CommentsL3
,bp.IsApprovedL4
,bp.ApprovedByL4
,bp.ApprovedDateL4
,bp.CommentsL4
,bp.IsAudited
,bp.AuditedBy
,bp.AuditedDate
,bp.AuditedComments
,bp.IsRejected
,bp.RejectedBy
,bp.RejectedDate
,bp.RejectedComments
,ISNULL(bpd.GLBDERequisitionId,0) GLBDERequisitionId


,br.Name BranchName
,case 
when bp.Post = 0 then 'Waiting for Posting'
when bp.IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' 
when bp.IsApprovedL3=1 then 'Waiting for Approve Level4' when bp.IsApprovedL2=1 then 'Waiting for Approve Level3'
when bp.IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level 1'
end as [Status]

FROM GLBDERequisitionPaids bp 
LEFT OUTER JOIN GLBranchs br ON bp.BranchId = br.Id
LEFT OUTER JOIN GLBDERequisitionPaidDetails bpd ON bp.Id = bpd.GLBDERequisitionPaidId
WHERE  1=1  AND bp.IsArchive = 0

";


                if (Id > 0)
                {
                    sqlText += @" and bp.Id=@Id";
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

                    vm = new GLBDERequisitionVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.GLBDERequisitionId = Convert.ToInt32(dr["GLBDERequisitionId"]);
                    
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.BranchName = dr["BranchName"].ToString();

                    vm.Code = dr["Code"].ToString();
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.TransactionType = dr["TransactionType"].ToString();
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
        //==================Insert =================
        public string[] Insert(GLBDERequisitionVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertGLBDERequisitionPaid"; //Method Name
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
                #region Code Generate
                GLCodeDAL codedal = new GLCodeDAL();
                vm.Code = codedal.NextCodeAcc("GLBDERequisitionPaids", vm.BranchId, Convert.ToDateTime(vm.TransactionDateTime), "BDE", currConn, transaction);
                #endregion Code Generate

                vm.Id = _cDal.NextId("GLBDERequisitionPaids", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO GLBDERequisitionPaids(
Id
,BranchId
,Code
,TransactionDateTime
,TransactionType
,ReferenceNo1
,ReferenceNo2
,ReferenceNo3
,Post
,Remarks
,IsActive,IsArchive
,CreatedBy,CreatedAt,CreatedFrom
,PostedBy,PostedAt,PostedFrom
,IsApprovedL1,ApprovedByL1,ApprovedDateL1,CommentsL1
,IsApprovedL2,ApprovedByL2,ApprovedDateL2,CommentsL2
,IsApprovedL3,ApprovedByL3,ApprovedDateL3,CommentsL3
,IsApprovedL4,ApprovedByL4,ApprovedDateL4,CommentsL4
,IsAudited,AuditedBy,AuditedDate
,AuditedComments,IsRejected,RejectedBy,RejectedDate,RejectedComments

) VALUES (
@Id
,@BranchId
,@Code
,@TransactionDateTime
,@TransactionType
,@ReferenceNo1
,@ReferenceNo2
,@ReferenceNo3
,@Post
,@Remarks
,@IsActive,@IsArchive
,@CreatedBy,@CreatedAt,@CreatedFrom
,@PostedBy,@PostedAt,@PostedFrom
,@IsApprovedL1,@ApprovedByL1,@ApprovedDateL1,@CommentsL1
,@IsApprovedL2,@ApprovedByL2,@ApprovedDateL2,@CommentsL2
,@IsApprovedL3,@ApprovedByL3,@ApprovedDateL3,@CommentsL3
,@IsApprovedL4,@ApprovedByL4,@ApprovedDateL4,@CommentsL4
,@IsAudited,@AuditedBy,@AuditedDate,@AuditedComments
,@IsRejected,@RejectedBy,@RejectedDate,@RejectedComments

) 
";


                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Parameters.AddWithValue("@Code", vm.Code);
                    cmdInsert.Parameters.AddWithValue("@TransactionDateTime", Ordinary.DateToString(vm.TransactionDateTime));
                    cmdInsert.Parameters.AddWithValue("@TransactionType", vm.TransactionType ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ReferenceNo1", vm.ReferenceNo1 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ReferenceNo2", vm.ReferenceNo2 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ReferenceNo3", vm.ReferenceNo3 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Post", false);
                    

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

                    cmdInsert.Parameters.AddWithValue("@PostedBy", "0");
                    cmdInsert.Parameters.AddWithValue("@PostedAt", "19000101");
                    cmdInsert.Parameters.AddWithValue("@PostedFrom", "NA");


                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update GLBDERequisitionPaids.", "");
                    }
                    #endregion SqlExecution

                    #region insert Details from Master into Detail Table
                   
                    GLBDERequisitionPaidDetailDAL _detaiDAL = new GLBDERequisitionPaidDetailDAL();
                    List<GLBDERequisitionPaidDetailVM> dVMs = new List<GLBDERequisitionPaidDetailVM>();
                    #region FormA
                    if (vm.glBDERequisitionFormAVMs != null && vm.glBDERequisitionFormAVMs.Count > 0)
                    {
                        foreach (var detailVM in vm.glBDERequisitionFormAVMs)
                        {
                            GLBDERequisitionPaidDetailVM dVM = new GLBDERequisitionPaidDetailVM();
                            ////dVM = detailVM;
                            dVM.BranchId = detailVM.BranchId;
                            dVM.GLBDERequisitionPaidId = vm.Id;
                            dVM.GLBDERequisitionId = detailVM.GLBDERequisitionId;
                            dVM.GLBDERequisitionDetailId = detailVM.Id;
                            dVM.TransactionType = vm.TransactionType;
                            dVMs.Add(dVM);
                        }

                        retResults = _detaiDAL.Insert(dVMs, currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }
                    #endregion
                    #region FormB

                    if (vm.glBDERequisitionFormBVMs != null && vm.glBDERequisitionFormBVMs.Count > 0)
                    {
                        foreach (var detailVM in vm.glBDERequisitionFormBVMs)
                        {
                            GLBDERequisitionPaidDetailVM dVM = new GLBDERequisitionPaidDetailVM();
                            ////dVM = detailVM;
                            dVM.BranchId = detailVM.BranchId;
                            dVM.GLBDERequisitionPaidId = vm.Id;
                            dVM.GLBDERequisitionId = detailVM.GLBDERequisitionId;
                            dVM.GLBDERequisitionDetailId = detailVM.Id;
                            dVM.TransactionType = vm.TransactionType;
                            dVMs.Add(dVM);
                        }

                        retResults = _detaiDAL.Insert(dVMs, currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }
                    #endregion
                    #region FormE

                    if (vm.glBDERequisitionFormEVMs != null && vm.glBDERequisitionFormEVMs.Count > 0)
                    {
                        foreach (var detailVM in vm.glBDERequisitionFormEVMs)
                        {
                            GLBDERequisitionPaidDetailVM dVM = new GLBDERequisitionPaidDetailVM();
                            ////dVM = detailVM;
                            dVM.BranchId = detailVM.BranchId;
                            dVM.GLBDERequisitionPaidId = vm.Id;
                            dVM.GLBDERequisitionId = detailVM.GLBDERequisitionId;
                            dVM.GLBDERequisitionDetailId = detailVM.Id;
                            dVM.TransactionType = vm.TransactionType;
                            dVMs.Add(dVM);
                        }

                        retResults = _detaiDAL.Insert(dVMs, currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }

                    #endregion
                    #endregion insert Details from Master into Detail Table
                    #region Update Payment
                    #region FormA
                    GLBDERequisitionFormADAL _formADAL = new GLBDERequisitionFormADAL();
                    if (vm.glBDERequisitionFormAVMs != null && vm.glBDERequisitionFormAVMs.Count > 0)
                    {
                        vm.glBDERequisitionFormAVMs.Select(c => {c.TransactionDateTime = vm.TransactionDateTime; return c;}).ToList();

                        retResults = _formADAL.UpdatePayment(vm.glBDERequisitionFormAVMs, currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }
                    #endregion
                    #region FormB
                    GLBDERequisitionFormBDAL _formBDAL = new GLBDERequisitionFormBDAL();
                    if (vm.glBDERequisitionFormBVMs != null && vm.glBDERequisitionFormBVMs.Count > 0)
                    {
                        vm.glBDERequisitionFormBVMs.Select(c => {c.TransactionDateTime = vm.TransactionDateTime; return c;}).ToList();

                        retResults = _formBDAL.UpdatePayment(vm.glBDERequisitionFormBVMs, currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }
                    #endregion
                    #region FormE
                    GLBDERequisitionFormEDAL _formEDAL = new GLBDERequisitionFormEDAL();
                    if (vm.glBDERequisitionFormEVMs != null && vm.glBDERequisitionFormEVMs.Count > 0)
                    {
                        vm.glBDERequisitionFormEVMs.Select(c => {c.TransactionDateTime = vm.TransactionDateTime; return c;}).ToList();

                        retResults = _formEDAL.UpdatePayment(vm.glBDERequisitionFormEVMs, currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }
                    #endregion
                    #endregion

                }
                else
                {
                    retResults[1] = "This GLBDERequisitionPaid already used!";
                    throw new ArgumentNullException("Please Input GLBDERequisitionPaid Value", "");
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
        public string[] Update(GLBDERequisitionVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "GLBDERequisitionPaidUpdate"; //Method Name
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
                    sqlText = "UPDATE GLBDERequisitionPaids SET";
                    sqlText += "  TransactionDateTime=@TransactionDateTime";
                    sqlText += " , TransactionType=@TransactionType";
                    sqlText += " , ReferenceNo1=@ReferenceNo1";
                    sqlText += " , ReferenceNo2=@ReferenceNo2";
                    sqlText += " , ReferenceNo3=@ReferenceNo3";
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
                    cmdUpdate.Parameters.AddWithValue("@TransactionType", vm.TransactionType ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ReferenceNo1", vm.ReferenceNo1 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ReferenceNo2", vm.ReferenceNo2 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ReferenceNo3", vm.ReferenceNo3 ?? Convert.DBNull);

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
                        throw new ArgumentNullException("Unexpected error to update GLBDERequisitionPaids.", "");
                    }
                    #endregion SqlExecution

                    #region insert Details from Master into Detail Table
                    #region Delete Detail
                    try
                    {
                        retResults = _cDal.DeleteTableInformation(vm.Id.ToString(), "GLBDERequisitionPaidDetails", "GLBDERequisitionPaidId", currConn, transaction);
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
                    GLBDERequisitionPaidDetailDAL _detaiDAL = new GLBDERequisitionPaidDetailDAL();
                    List<GLBDERequisitionPaidDetailVM> dVMs = new List<GLBDERequisitionPaidDetailVM>();
                    #region FormA
                    if (vm.glBDERequisitionFormAVMs != null && vm.glBDERequisitionFormAVMs.Count > 0)
                    {
                        foreach (var detailVM in vm.glBDERequisitionFormAVMs)
                        {
                            GLBDERequisitionPaidDetailVM dVM = new GLBDERequisitionPaidDetailVM();
                            dVM.BranchId = detailVM.BranchId;
                            dVM.GLBDERequisitionPaidId = vm.Id;
                            dVM.GLBDERequisitionId = detailVM.GLBDERequisitionId;
                            dVM.GLBDERequisitionDetailId = detailVM.Id;
                            dVM.TransactionType = vm.TransactionType;
                            dVMs.Add(dVM);
                        }

                        retResults = _detaiDAL.Insert(dVMs, currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }
                    #endregion
                    #region FormB

                    if (vm.glBDERequisitionFormBVMs != null && vm.glBDERequisitionFormBVMs.Count > 0)
                    {
                        foreach (var detailVM in vm.glBDERequisitionFormBVMs)
                        {
                            GLBDERequisitionPaidDetailVM dVM = new GLBDERequisitionPaidDetailVM();
                            ////dVM = detailVM;
                            dVM.BranchId = detailVM.BranchId;
                            dVM.GLBDERequisitionPaidId = vm.Id;
                            dVM.GLBDERequisitionId = detailVM.GLBDERequisitionId;
                            dVM.GLBDERequisitionDetailId = detailVM.Id;
                            dVM.TransactionType = vm.TransactionType;
                            dVMs.Add(dVM);
                        }

                        retResults = _detaiDAL.Insert(dVMs, currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }
                    #endregion
                    #region FormE

                    if (vm.glBDERequisitionFormEVMs != null && vm.glBDERequisitionFormEVMs.Count > 0)
                    {
                        foreach (var detailVM in vm.glBDERequisitionFormEVMs)
                        {
                            GLBDERequisitionPaidDetailVM dVM = new GLBDERequisitionPaidDetailVM();
                            ////dVM = detailVM;
                            dVM.BranchId = detailVM.BranchId;
                            dVM.GLBDERequisitionPaidId = vm.Id;
                            dVM.GLBDERequisitionId = detailVM.GLBDERequisitionId;
                            dVM.GLBDERequisitionDetailId = detailVM.Id;
                            dVM.TransactionType = vm.TransactionType;
                            dVMs.Add(dVM);
                        }

                        retResults = _detaiDAL.Insert(dVMs, currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }

                    #endregion
                    #endregion
                    #endregion insert Details from Master into Detail Table
                    #region Update Payment
                    #region FormA
                    GLBDERequisitionFormADAL _formADAL = new GLBDERequisitionFormADAL();
                    if (vm.glBDERequisitionFormAVMs != null && vm.glBDERequisitionFormAVMs.Count > 0)
                    {
                        vm.glBDERequisitionFormAVMs.Select(c => {c.TransactionDateTime = vm.TransactionDateTime; return c;}).ToList();
                        retResults = _formADAL.UpdatePayment(vm.glBDERequisitionFormAVMs, currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }
                    #endregion
                    #region FormB
                    GLBDERequisitionFormBDAL _formBDAL = new GLBDERequisitionFormBDAL();
                    if (vm.glBDERequisitionFormBVMs != null && vm.glBDERequisitionFormBVMs.Count > 0)
                    {
                        vm.glBDERequisitionFormBVMs.Select(c => {c.TransactionDateTime = vm.TransactionDateTime; return c;}).ToList();

                        retResults = _formBDAL.UpdatePayment(vm.glBDERequisitionFormBVMs, currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }
                    #endregion
                    #region FormE
                    GLBDERequisitionFormEDAL _formEDAL = new GLBDERequisitionFormEDAL();
                    if (vm.glBDERequisitionFormEVMs != null && vm.glBDERequisitionFormEVMs.Count > 0)
                    {
                        vm.glBDERequisitionFormEVMs.Select(c => {c.TransactionDateTime = vm.TransactionDateTime; return c;}).ToList();

                        retResults = _formEDAL.UpdatePayment(vm.glBDERequisitionFormEVMs, currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }
                    #endregion
                    #endregion

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("GLBDERequisitionPaid Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLBDERequisitionPaid Update", "Could not found any item.");
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
        public string[] Delete(GLBDERequisitionPaidVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLBDERequisitionPaid"; //Method Name
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
                        sqlText = "update GLBDERequisitionPaids set";
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
                        throw new ArgumentNullException("GLBDERequisitionPaid Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLBDERequisitionPaid Information Delete", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Data Delete Successfully.";
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

        public string[] Post(GLBDERequisitionPaidVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLBDERequisitionPaid"; //Method Name
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
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update GLBDERequisitionPaids set";
                        sqlText += " Post=@Post";
                        sqlText += " ,PostedBy=@PostedBy";
                        sqlText += " ,PostedAt=@PostedAt";
                        sqlText += " ,PostedFrom=@PostedFrom";
                        sqlText += " where Id=@Id";
                        sqlText += " update GLBDERequisitionPaidDetails set post=@Post where GLBDERequisitionPaidId=@Id";
                      

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
                        throw new ArgumentNullException("GLBDERequisitionPaid Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    #region Send Mail to the User
                    ////////////////////////if (retResults[0] != "Fail")
                    ////////////////////////{
                    ////////////////SendEmail = new GLSettingDAL().settingValue("Email", "SendEmail", currConn, transaction);
                    ////////////////if (SendEmail == "Y")
                    ////////////////{
                    ////////////////    for (int i = 0; i < ids.Length - 1; i++)
                    ////////////////    {
                    ////////////////        //Prameters: (Branch = this Branch; HaveBDERequisitionPaidApproval=1; HaveApprovalLevel1=1) 
                    ////////////////        string urlPrefix = "";
                    ////////////////        //urlPrefix = "http://localhost:50010";
                    ////////////////        urlPrefix = new GLSettingDAL().settingValue("Email", "BDEURLPrefix", currConn, transaction);
                    ////////////////        GLBDERequisitionVM varVM = new GLBDERequisitionVM();
                    ////////////////        varVM = SelectAll(Convert.ToInt32(ids[i]), null, null, currConn, transaction).FirstOrDefault();
                    ////////////////        string url = urlPrefix + "/Sage/BDERequisitionPaid/Approve/" + varVM.Id;

                    ////////////////        List<GLUserVM> userVMs = new List<GLUserVM>();
                    ////////////////        GLUserVM userVM = new GLUserVM();
                    ////////////////        userVM.BranchId = varVM.BranchId;
                    ////////////////        string[] conditionFields = { "HaveBDERequisitionPaidApproval", "HaveApprovalLevel1" };
                    ////////////////        string[] conditionValues = { "1", "1" };

                    ////////////////        userVMs = new GLUserDAL().SelectUserForMail(userVM, conditionFields, conditionValues, currConn, transaction);
                    ////////////////        GLEmailDAL _emailDAL = new GLEmailDAL();
                    ////////////////        GLEmailSettingVM emailSettingVM = new GLEmailSettingVM();
                    ////////////////        string status = "Generated and Waiting for Approve Level1";

                    ////////////////        foreach (var item in userVMs)
                    ////////////////        {
                    ////////////////            string[] EmailForm = Ordinary.GDICEmailForm(item.FullName, varVM.Code, status, url, "BDEReq");
                    ////////////////            emailSettingVM.MailHeader = EmailForm[0];
                    ////////////////            emailSettingVM.MailToAddress = item.Email;
                    ////////////////            emailSettingVM.MailBody = EmailForm[1];
                    ////////////////            thread = new Thread(c => _emailDAL.SendEmail(emailSettingVM, thread));
                    ////////////////            thread.Start();
                    ////////////////        }
                    ////////////////    }
                    ////////////////}
                    //////////////////////////}
                    #endregion
                }
                else
                {
                    throw new ArgumentNullException("GLBDERequisitionPaid Information Delete", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                retResults[0] = "Success";
                ////////if (SendEmail == "Y")
                ////////{
                ////////    retResults[1] = "Data Posted Successfully! And Email Sent to the Respective Persons!";
                ////////}
                ////////else
                ////////{
                    retResults[1] = "Data Posted Successfully!";
                ////////}
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

        public string[] ApproveReject(GLBDERequisitionPaidVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "ApproveRejectGLBDERequisitionPaid"; //Method Name
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
                    GLBDERequisitionVM varVM = new GLBDERequisitionVM();
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
                                //Prameters: (Branch = this Branch; HaveBDERequisitionPaidApproval=1; HaveApprovalLevel1=1) 

                                #region Declarations
                                string urlPrefix = "";
                                urlPrefix = new GLSettingDAL().settingValue("Email", "BDEURLPrefix", currConn, transaction);
                                string url = urlPrefix + "/Sage/BDERequisitionPaid/SelfApproveIndex";

                                varVM = new GLBDERequisitionVM();
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
                                    url = urlPrefix + "/Sage/BDERequisitionPaid/Posted/" + varVM.Id;
                                }
                                else if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == true && varVM.IsApprovedL3 == true && varVM.IsApprovedL4 == true)
                                {
                                    status = "Approval Completed";
                                    string[] cFieldsUser = { "u.LogId" };
                                    string[] cValuesUser = { varVM.CreatedBy };
                                    userVMs = new GLUserDAL().SelectAll(0, cFieldsUser, cValuesUser, currConn, transaction);
                                    url = urlPrefix + "/Sage/BDERequisitionPaid/Posted/" + varVM.Id;
                                }
                                else if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == true && varVM.IsApprovedL3 == true && varVM.IsApprovedL4 == false)
                                {
                                    status = "Waiting for Approve Level4";
                                    string[] cFieldsUser = { "u.HaveBDERequisitionPaidApproval", "u.HaveApprovalLevel4" };
                                    string[] cValuesUser = { "1", "1" };
                                    userVMs = new GLUserDAL().SelectUserForMail(userVM, cFieldsUser, cValuesUser, currConn, transaction);
                                    url = urlPrefix + "/Sage/BDERequisitionPaid/Approve/" + varVM.Id;
                                }
                                else if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == true && varVM.IsApprovedL3 == false)
                                {
                                    status = "Waiting for Approve Level3";
                                    string[] cFieldsUser = { "u.HaveBDERequisitionPaidApproval", "u.HaveApprovalLevel3" };
                                    string[] cValuesUser = { "1", "1" };
                                    userVMs = new GLUserDAL().SelectUserForMail(userVM, cFieldsUser, cValuesUser, currConn, transaction);
                                    url = urlPrefix + "/Sage/BDERequisitionPaid/Approve/" + varVM.Id;
                                }
                                else if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == false)
                                {
                                    status = "Waiting for Approve Level2";
                                    string[] cFieldsUser = { "u.HaveBDERequisitionPaidApproval", "u.HaveApprovalLevel2" };
                                    string[] cValuesUser = { "1", "1" };
                                    userVMs = new GLUserDAL().SelectUserForMail(userVM, cFieldsUser, cValuesUser, currConn, transaction);
                                    url = urlPrefix + "/Sage/BDERequisitionPaid/Approve/" + varVM.Id;
                                }
                                #endregion
                                #region Send Mail
                                foreach (var item in userVMs)
                                {
                                    string[] EmailForm = Ordinary.GDICEmailForm(item.FullName, varVM.Code, status, url, "BDEReq");
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
                    throw new ArgumentNullException("GLBDERequisitionPaid Information Delete", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #region SuccessResult
                retResults[0] = "Success";
                if (SendEmail == "Y")
                {
                    retResults[1] = retResults[1] + " And Email Sent to the Respective Persons!";
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

        public string[] ApproveReject(GLBDERequisitionPaidVM vm, string Level, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLBDERequisitionPaid"; //Method Name
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

                #region Update Settings
                sqlText = "";
                sqlText = "update GLBDERequisitionPaids set";
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
                    sqlText += " update GLBDERequisitionPaidFormAs set";
                    sqlText += " IsRejected=@IsRejected,RejectedBy=@RejectedBy,RejectedDate=@RejectedDate,RejectedComments=@RejectedComments";
                    sqlText += " where GLBDERequisitionPaidId=@Id";

                    sqlText += " update GLBDERequisitionPaidFormBs set";
                    sqlText += " IsRejected=@IsRejected,RejectedBy=@RejectedBy,RejectedDate=@RejectedDate,RejectedComments=@RejectedComments";
                    sqlText += " where GLBDERequisitionPaidId=@Id";

                    sqlText += " update GLBDERequisitionPaidFormCs set";
                    sqlText += " IsRejected=@IsRejected,RejectedBy=@RejectedBy,RejectedDate=@RejectedDate,RejectedComments=@RejectedComments";
                    sqlText += " where GLBDERequisitionPaidId=@Id";

                    sqlText += " update GLBDERequisitionPaidFormDs set";
                    sqlText += " IsRejected=@IsRejected,RejectedBy=@RejectedBy,RejectedDate=@RejectedDate,RejectedComments=@RejectedComments";
                    sqlText += " where GLBDERequisitionPaidId=@Id";

                    sqlText += " update GLBDERequisitionPaidFormEs set";
                    sqlText += " IsRejected=@IsRejected,RejectedBy=@RejectedBy,RejectedDate=@RejectedDate,RejectedComments=@RejectedComments";
                    sqlText += " where GLBDERequisitionPaidId=@Id";
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

                #region Commit
                if (transResult <= 0)
                {
                    throw new ArgumentNullException("GLBDERequisitionPaid Delete", vm.Id + " could not Delete.");
                }
                #endregion Commit
                #endregion Update Settings
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #region SuccessResult
                retResults[0] = "Success";
                //retResults[1] = "Data Save Successfully.";
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

        ////==================Audit =================
        public string[] Audit(GLBDERequisitionPaidVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "AuditGLBDERequisitionPaid"; //Method Name
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
                        sqlText = "update GLBDERequisitionPaids set";
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
                        throw new ArgumentNullException("GLBDERequisitionPaid Audit", vm.Id + " could not Audit.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLBDERequisitionPaid Information Audit", "Could not found any item.");
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

        ////==================Report=================
        public DataTable Report(GLBDERequisitionPaidVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
bp.Id
,bp.BranchId
,bp.Code
,bp.TransactionDateTime
,bp.ReferenceNo1
,bp.ReferenceNo2
,bp.ReferenceNo3
,bp.Post
,bp.Remarks
,br.Name BranchName

,bp.CreatedBy
,bp.LastUpdateBy
,bp.PostedBy
,bp.ApprovedByL1
,bp.ApprovedByL2
,bp.ApprovedByL3
,bp.ApprovedByL4
,bp.AuditedBy
,bp.RejectedBy

FROM GLBDERequisitionPaids bp 
LEFT OUTER JOIN GLBranchs br ON bp.BranchId = br.Id
WHERE  1=1  AND bp.IsArchive = 0
";

                if (vm.Id > 0)
                {
                    sqlText += @" and bp.Id=@Id";
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

        #endregion
    }
}
