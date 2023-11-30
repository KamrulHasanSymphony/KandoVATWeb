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
    public class GLBDERequisitionDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        public static Thread thread;
        #endregion
        #region Methods
        #region Charts
        //FormA////==================Date Range(Multiple Month) - Single DocumentType Amount=================Y:Amount - X:Month
        public List<GLBDERequisitionFormAVM> SelectChart1(GLReportVM paramVM, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLBDERequisitionFormAVM> VMs = new List<GLBDERequisitionFormAVM>();
            GLBDERequisitionFormAVM vm;
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
--declare @DocumentType as varchar(20)
--declare @dtFrom as varchar(20)
--declare @dtTo as varchar(20)

--set @dtFrom = '20180201'
--set @dtTo = '20180501'
--set @DocumentType = 'FIR'

declare @PeriodIdFrom as varchar(20)
declare @PeriodIdTo as varchar(20)

select @PeriodIdFrom=id   from GLFiscalYearDetails fd
where @dtFrom between fd.PeriodStart and fd.PeriodEnd 

select @PeriodIdTo=id   from GLFiscalYearDetails fd
where @dtTo between fd.PeriodStart and fd.PeriodEnd 

select fyd.Id GLFiscalYearDetailId, fyd.PeriodName, isnull(t.TransactionAmount,0)TransactionAmount from GLFiscalYearDetails fyd
left outer join (
select
distinct fyd.Id fydId, sum(fa.BDEAmount) TransactionAmount
from GLBDERequisitionFormAs fa
LEFT OUTER JOIN GLFiscalYearDetails  fyd ON fa.TransactionDateTime between PeriodStart and PeriodEnd
WHERE  1=1  
and fa.TransactionDateTime>=@dtFrom
and fa.TransactionDateTime<=@dtTo
and fa.DocumentType = @DocumentType
and fa.BranchId = @BranchId
group by fyd.Id

) as t on t.fydId =  fyd.id
where  fyd.id between @PeriodIdFrom and @PeriodIdTo
";
                if (paramVM.BranchId == 0)
                {
                    sqlText = sqlText.Replace("fa.BranchId = @BranchId", "1=1");
                }


                if (string.IsNullOrWhiteSpace(paramVM.GLDocumentType))
                {
                    sqlText = sqlText.Replace("fa.DocumentType = @DocumentType", "1=1");
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
                objComm.Parameters.AddWithValue("@dtFrom", Ordinary.DateToString(paramVM.DateFrom));
                objComm.Parameters.AddWithValue("@dtTo", Ordinary.DateToString(paramVM.DateTo));
                if (paramVM.BranchId > 0)
                {
                    objComm.Parameters.AddWithValue("@BranchId", paramVM.BranchId);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.GLDocumentType))
                {
                    objComm.Parameters.AddWithValue("@DocumentType", paramVM.GLDocumentType);
                }

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLBDERequisitionFormAVM();
                    vm.GLFiscalYearDetailId = Convert.ToInt32(dr["GLFiscalYearDetailId"]);
                    vm.TransactionAmount = Convert.ToDecimal(dr["TransactionAmount"]);
                    vm.PeriodName = dr["PeriodName"].ToString();
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

        ////==================Date Range(Multiple Month) - All DocumentType Amount=================Y:Amount - X:DocumentType
        public List<GLBDERequisitionFormAVM> SelectChart2(GLReportVM paramVM, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLBDERequisitionFormAVM> VMs = new List<GLBDERequisitionFormAVM>();
            GLBDERequisitionFormAVM vm;
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
--declare @DocumentType as varchar(20)
--declare @dtFrom as varchar(20)
--declare @dtTo as varchar(20)

--set @dtFrom = '20180201'
--set @dtTo = '20180501'


select distinct fa.DocumentType, sum(fa.BDEAmount) TransactionAmount
from GLBDERequisitionFormAs fa
WHERE  1=1  
and fa.TransactionDateTime>=@dtFrom
and fa.TransactionDateTime<=@dtTo
and fa.BranchId = @BranchId

";
                if (paramVM.BranchId == 0)
                {
                    sqlText = sqlText.Replace("fa.BranchId = @BranchId", "1=1");
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

                sqlText += "  group by fa.DocumentType";
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
                objComm.Parameters.AddWithValue("@dtFrom", Ordinary.DateToString(paramVM.DateFrom));
                objComm.Parameters.AddWithValue("@dtTo", Ordinary.DateToString(paramVM.DateTo));
                if (paramVM.BranchId > 0)
                {
                    objComm.Parameters.AddWithValue("@BranchId", paramVM.BranchId);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLBDERequisitionFormAVM();
                    vm.TransactionAmount = Convert.ToDecimal(dr["TransactionAmount"]);
                    vm.DocumentType = dr["DocumentType"].ToString();
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

        #endregion


        //==================DropDown=================
        public List<GLBDERequisitionVM> DropDown(int branchId = 0)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<GLBDERequisitionVM> VMs = new List<GLBDERequisitionVM>();
            GLBDERequisitionVM vm;
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
   FROM GLBDERequisitions af
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
                    vm = new GLBDERequisitionVM();
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
        public List<GLBDERequisitionVM> SelectAllSelfApprove(int Id = 0, int UserId = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
--declare @UserId int
--set @UserId=3
select a.*, br.Name BranchName from(
SELECT
ft.Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,ft.BranchId,ft.Code,ft.TransactionDateTime,ft.ReferenceNo1,ft.ReferenceNo2,ft.ReferenceNo3,ft.Post,ft.Remarks,ft.IsActive,ft.IsArchive,ft.CreatedBy,ft.CreatedAt,ft.CreatedFrom,ft.LastUpdateBy,ft.LastUpdateAt,ft.LastUpdateFrom,ft.PostedBy,ft.PostedAt,ft.PostedFrom,ft.IsApprovedL1,ft.ApprovedByL1,ft.ApprovedDateL1,ft.CommentsL1,ft.IsApprovedL2,ft.ApprovedByL2,ft.ApprovedDateL2,ft.CommentsL2,ft.IsApprovedL3,ft.ApprovedByL3,ft.ApprovedDateL3,ft.CommentsL3,ft.IsApprovedL4,ft.ApprovedByL4,ft.ApprovedDateL4,ft.CommentsL4,ft.IsAudited,ft.AuditedBy,ft.AuditedDate,ft.AuditedComments,ft.IsRejected,ft.RejectedBy,ft.RejectedDate,ft.RejectedComments
FROM GLBDERequisitions  ft
left outer join GLUsers u on u.id=@UserId
WHERE  1=1  
and u.HaveApprovalLevel1=1
and u.HaveBDERequisitionApproval=1
AND ft.IsArchive = 0
and ft.Post=1 and ft.IsRejected=0
and ft.IsApprovedL1=0
union all

SELECT
ft.Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,ft.BranchId,ft.Code,ft.TransactionDateTime,ft.ReferenceNo1,ft.ReferenceNo2,ft.ReferenceNo3,ft.Post,ft.Remarks,ft.IsActive,ft.IsArchive,ft.CreatedBy,ft.CreatedAt,ft.CreatedFrom,ft.LastUpdateBy,ft.LastUpdateAt,ft.LastUpdateFrom,ft.PostedBy,ft.PostedAt,ft.PostedFrom,ft.IsApprovedL1,ft.ApprovedByL1,ft.ApprovedDateL1,ft.CommentsL1,ft.IsApprovedL2,ft.ApprovedByL2,ft.ApprovedDateL2,ft.CommentsL2,ft.IsApprovedL3,ft.ApprovedByL3,ft.ApprovedDateL3,ft.CommentsL3,ft.IsApprovedL4,ft.ApprovedByL4,ft.ApprovedDateL4,ft.CommentsL4,ft.IsAudited,ft.AuditedBy,ft.AuditedDate,ft.AuditedComments,ft.IsRejected,ft.RejectedBy,ft.RejectedDate,ft.RejectedComments

FROM GLBDERequisitions    ft
left outer join GLUsers u on u.id=@UserId
WHERE  1=1  
and u.HaveApprovalLevel2=1
and u.HaveBDERequisitionApproval=1
AND  ft.IsArchive = 0
and  ft.Post=1 and  ft.IsRejected=0
and  ft.IsApprovedL1=1 and  ft.IsApprovedL2=0
union all

SELECT
ft.Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approv Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,ft.BranchId,ft.Code,ft.TransactionDateTime,ft.ReferenceNo1,ft.ReferenceNo2,ft.ReferenceNo3,ft.Post,ft.Remarks,ft.IsActive,ft.IsArchive,ft.CreatedBy,ft.CreatedAt,ft.CreatedFrom,ft.LastUpdateBy,ft.LastUpdateAt,ft.LastUpdateFrom,ft.PostedBy,ft.PostedAt,ft.PostedFrom,ft.IsApprovedL1,ft.ApprovedByL1,ft.ApprovedDateL1,ft.CommentsL1,ft.IsApprovedL2,ft.ApprovedByL2,ft.ApprovedDateL2,ft.CommentsL2,ft.IsApprovedL3,ft.ApprovedByL3,ft.ApprovedDateL3,ft.CommentsL3,ft.IsApprovedL4,ft.ApprovedByL4,ft.ApprovedDateL4,ft.CommentsL4,ft.IsAudited,ft.AuditedBy,ft.AuditedDate,ft.AuditedComments,ft.IsRejected,ft.RejectedBy,ft.RejectedDate,ft.RejectedComments

FROM GLBDERequisitions   ft
left outer join GLUsers u on u.id=@UserId
WHERE  1=1  
and u.HaveApprovalLevel3=1
and u.HaveBDERequisitionApproval=1
AND  ft.IsArchive = 0
and Post=1 and IsRejected=0
and IsApprovedL1=1 and IsApprovedL2=1 and IsApprovedL3=0
union all
SELECT
ft.Id
,case when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' when IsApprovedL3=1 then 'Waiting for Approve Level4'when IsApprovedL2=1 then 'Waiting for Approve Level3'when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level1'end as [Status]
,case when IsApprovedL3=1 then 'Level4'when IsApprovedL2=1 then 'Level3'when IsApprovedL1=1 then 'Level2' else 'Level1'end as [MyStatus]
,ft.BranchId,ft.Code,ft.TransactionDateTime,ft.ReferenceNo1,ft.ReferenceNo2,ft.ReferenceNo3,ft.Post,ft.Remarks,ft.IsActive,ft.IsArchive,ft.CreatedBy,ft.CreatedAt,ft.CreatedFrom,ft.LastUpdateBy,ft.LastUpdateAt,ft.LastUpdateFrom,ft.PostedBy,ft.PostedAt,ft.PostedFrom,ft.IsApprovedL1,ft.ApprovedByL1,ft.ApprovedDateL1,ft.CommentsL1,ft.IsApprovedL2,ft.ApprovedByL2,ft.ApprovedDateL2,ft.CommentsL2,ft.IsApprovedL3,ft.ApprovedByL3,ft.ApprovedDateL3,ft.CommentsL3,ft.IsApprovedL4,ft.ApprovedByL4,ft.ApprovedDateL4,ft.CommentsL4,ft.IsAudited,ft.AuditedBy,ft.AuditedDate,ft.AuditedComments,ft.IsRejected,ft.RejectedBy,ft.RejectedDate,ft.RejectedComments
FROM GLBDERequisitions    ft
left outer join GLUsers u on u.id=@UserId
WHERE  1=1  
and u.HaveApprovalLevel4=1
and u.HaveBDERequisitionApproval=1
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

                    vm = new GLBDERequisitionVM();
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
        public List<GLBDERequisitionVM> SelectAllPosted(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
FROM GLBDERequisitions  
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
FROM GLBDERequisitions  
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
FROM GLBDERequisitions  
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
FROM GLBDERequisitions  
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
FROM GLBDERequisitions  
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
FROM GLBDERequisitions  
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

                    vm = new GLBDERequisitionVM();
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
        public List<GLBDERequisitionVM> SelectAllAudit(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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

FROM GLBDERequisitions m 
LEFT OUTER JOIN GLBDERequisitionFormAs fa ON fa.GLBDERequisitionId = m.Id
LEFT OUTER JOIN GLBDERequisitionFormBs fb ON fb.GLBDERequisitionId = m.Id
LEFT OUTER JOIN GLBDERequisitionFormCs fc ON fc.GLBDERequisitionId = m.Id
LEFT OUTER JOIN GLBDERequisitionFormDs fd ON fd.GLBDERequisitionId = m.Id
LEFT OUTER JOIN GLBDERequisitionFormEs fe ON fe.GLBDERequisitionId = m.Id
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
                    vm = new GLBDERequisitionVM();
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
SELECT
bde.Id
,bde.BranchId
,bde.Code
,bde.TransactionDateTime
,bde.ReferenceNo1
,bde.ReferenceNo2
,bde.ReferenceNo3
,bde.Post
,ISNULL(bde.OpeningBalance       ,0)OpeningBalance
,ISNULL(bde.TotalExpense         ,0)TotalExpense
,ISNULL(bde.TotalRequisition     ,0)TotalRequisition
,ISNULL(bde.FundReceive          ,0)FundReceive
,ISNULL(bde.NextOpening          ,0)NextOpening
,ISNULL(bde.MadeJournal,0)MadeJournal
,bde.Remarks
,bde.IsActive
,bde.IsArchive
,bde.CreatedBy
,bde.CreatedAt
,bde.CreatedFrom
,bde.LastUpdateBy
,bde.LastUpdateAt
,bde.LastUpdateFrom
,bde.PostedBy
,bde.PostedAt
,bde.PostedFrom
,bde.IsApprovedL1
,bde.ApprovedByL1
,bde.ApprovedDateL1
,bde.CommentsL1
,bde.IsApprovedL2
,bde.ApprovedByL2
,bde.ApprovedDateL2
,bde.CommentsL2
,bde.IsApprovedL3
,bde.ApprovedByL3
,bde.ApprovedDateL3
,bde.CommentsL3
,bde.IsApprovedL4
,bde.ApprovedByL4
,bde.ApprovedDateL4
,bde.CommentsL4
,bde.IsAudited
,bde.AuditedBy
,bde.AuditedDate
,bde.AuditedComments
,bde.IsRejected
,bde.RejectedBy
,bde.RejectedDate
,bde.RejectedComments



,br.Name BranchName
,case 
when Post = 0 then 'Waiting for Posting'
when IsRejected=1 then 'Reject'when IsApprovedL4=1 then 'Approval Completed' 
when IsApprovedL3=1 then 'Waiting for Approve Level4' when IsApprovedL2=1 then 'Waiting for Approve Level3'
when IsApprovedL1=1 then 'Waiting for Approve Level2'else 'Waiting for Approve Level 1'
end as [Status]

FROM GLBDERequisitions bde 
LEFT OUTER JOIN GLBranchs br ON bde.BranchId = br.Id
WHERE  1=1  AND bde.IsArchive = 0

";


                if (Id > 0)
                {
                    sqlText += @" and bde.Id=@Id";
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
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.BranchName = dr["BranchName"].ToString();

                    vm.Code = dr["Code"].ToString();
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.ReferenceNo1 = dr["ReferenceNo1"].ToString();
                    vm.ReferenceNo2 = dr["ReferenceNo2"].ToString();
                    vm.ReferenceNo3 = dr["ReferenceNo3"].ToString();
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.OpeningBalance = Convert.ToInt32(dr["OpeningBalance"]);
                    vm.TotalExpense = Convert.ToInt32(dr["TotalExpense"]);
                    vm.TotalRequisition = Convert.ToInt32(dr["TotalRequisition"]);
                    vm.FundReceive = Convert.ToInt32(dr["FundReceive"]);
                    vm.NextOpening = Convert.ToInt32(dr["NextOpening"]);
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
        public string[] Insert(GLBDERequisitionVM vm, List<HttpPostedFileBase> UploadFiles, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertGLBDERequisition"; //Method Name
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
                vm.Code = codedal.NextCodeAcc("GLBDERequisitions", vm.BranchId, Convert.ToDateTime(vm.TransactionDateTime), "BDE", currConn, transaction);
                #endregion Code Generate

                vm.Id = _cDal.NextId("GLBDERequisitions", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO GLBDERequisitions(
Id
,BranchId
,Code
,TransactionDateTime
,ReferenceNo1
,ReferenceNo2
,ReferenceNo3
,Post
,OpeningBalance
,TotalExpense
,TotalRequisition
,FundReceive
,NextOpening
,MadeJournal
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
,@ReferenceNo1
,@ReferenceNo2
,@ReferenceNo3
,@Post
,@OpeningBalance
,@TotalExpense
,@TotalRequisition
,@FundReceive
,@NextOpening
,@MadeJournal
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
                    cmdInsert.Parameters.AddWithValue("@ReferenceNo1", vm.ReferenceNo1 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ReferenceNo2", vm.ReferenceNo2 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ReferenceNo3", vm.ReferenceNo3 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Post", false);
                    cmdInsert.Parameters.AddWithValue("@OpeningBalance", vm.OpeningBalance);
                    cmdInsert.Parameters.AddWithValue("@TotalExpense", vm.TotalExpense);
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

                    cmdInsert.Parameters.AddWithValue("@PostedBy", "0");
                    cmdInsert.Parameters.AddWithValue("@PostedAt", "19000101");
                    cmdInsert.Parameters.AddWithValue("@PostedFrom", "NA");


                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update GLBDERequisitions.", "");
                    }
                    #endregion SqlExecution

                    #region insert Details from Master into Detail Table
                    #region Form A
                    GLBDERequisitionFormADAL _formADAL = new GLBDERequisitionFormADAL();
                    if (vm.glBDERequisitionFormAVMs != null && vm.glBDERequisitionFormAVMs.Count > 0)
                    {
                        foreach (var eeTransactionDVM in vm.glBDERequisitionFormAVMs)
                        {
                            GLBDERequisitionFormAVM dVM = new GLBDERequisitionFormAVM();
                            dVM = eeTransactionDVM;
                            dVM.GLBDERequisitionId = vm.Id;
                            dVM.TransactionDateTime = vm.TransactionDateTime;
                            dVM.BranchId = vm.BranchId;
                            retResults = _formADAL.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                    }
                    #endregion Form A
                    #region Form B
                    GLBDERequisitionFormBDAL _formBDAL = new GLBDERequisitionFormBDAL();
                    if (vm.glBDERequisitionFormBVMs != null && vm.glBDERequisitionFormBVMs.Count > 0)
                    {
                        foreach (var eeTransactionDVM in vm.glBDERequisitionFormBVMs)
                        {
                            GLBDERequisitionFormBVM dVM = new GLBDERequisitionFormBVM();
                            dVM = eeTransactionDVM;
                            dVM.GLBDERequisitionId = vm.Id;
                            dVM.TransactionDateTime = vm.TransactionDateTime;
                            dVM.BranchId = vm.BranchId;
                            retResults = _formBDAL.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                    }
                    #endregion Form B
                    #region Form C
                    GLBDERequisitionFormCDAL _formCDAL = new GLBDERequisitionFormCDAL();
                    if (vm.glBDERequisitionFormCVMs != null && vm.glBDERequisitionFormCVMs.Count > 0)
                    {
                        foreach (var eeTransactionDVM in vm.glBDERequisitionFormCVMs)
                        {
                            GLBDERequisitionFormCVM dVM = new GLBDERequisitionFormCVM();
                            dVM = eeTransactionDVM;
                            dVM.GLBDERequisitionId = vm.Id;
                            dVM.TransactionDateTime = vm.TransactionDateTime;
                            dVM.BranchId = vm.BranchId;
                            retResults = _formCDAL.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                    }
                    #endregion Form C
                    #region Form D
                    GLBDERequisitionFormDDAL _formDDAL = new GLBDERequisitionFormDDAL();
                    if (vm.glBDERequisitionFormDVMs != null && vm.glBDERequisitionFormDVMs.Count > 0)
                    {
                        foreach (var eeTransactionDVM in vm.glBDERequisitionFormDVMs)
                        {
                            GLBDERequisitionFormDVM dVM = new GLBDERequisitionFormDVM();
                            dVM = eeTransactionDVM;
                            dVM.GLBDERequisitionId = vm.Id;
                            dVM.TransactionDateTime = vm.TransactionDateTime;
                            dVM.BranchId = vm.BranchId;
                            retResults = _formDDAL.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                    }
                    #endregion Form D
                    #region Form E
                    GLBDERequisitionFormEDAL _formEDAL = new GLBDERequisitionFormEDAL();
                    if (vm.glBDERequisitionFormEVMs != null && vm.glBDERequisitionFormEVMs.Count > 0)
                    {
                        foreach (var eeTransactionDVM in vm.glBDERequisitionFormEVMs)
                        {
                            GLBDERequisitionFormEVM dVM = new GLBDERequisitionFormEVM();
                            dVM = eeTransactionDVM;
                            dVM.GLBDERequisitionId = vm.Id;
                            dVM.TransactionDateTime = vm.TransactionDateTime;
                            dVM.BranchId = vm.BranchId;
                            retResults = _formEDAL.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                    }
                    #endregion Form E
                    #region FileUpload

                    GLBDERequisitionFileDAL _formFileDAL = new GLBDERequisitionFileDAL();

                    List<GLBDERequisitionFileVM> fileVMs = new List<GLBDERequisitionFileVM>();

                    foreach (HttpPostedFileBase file in UploadFiles)
                    {
                        if (file != null && Array.Exists(vm.FilesToBeUploaded.Split(','), s => s.Equals(file.FileName)))
                        {
                            if (file != null && file.ContentLength > 0)
                            {
                                GLBDERequisitionFileVM fileVM = new GLBDERequisitionFileVM();
                                string path = vm.Id.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHMMss") + "_" + Path.GetFileName(file.FileName);

                                string saveFilePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Files/Sage/BDERequisition"), path);

                                file.SaveAs(saveFilePath);

                                fileVM.GLBDERequisitionId = vm.Id;
                                fileVM.FileName = path;
                                fileVM.FileOrginalName = file.FileName;
                                fileVMs.Add(fileVM);
                            }
                        }
                    }
                    if (fileVMs != null && fileVMs.Count > 0)
                    {
                        retResults = _formFileDAL.Insert(fileVMs, currConn, transaction);
                    }
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }


                    #endregion File Upload
                    #endregion insert Details from Master into Detail Table
                    #region Update Amount
                    GLBDERequisitionVM newVM = new GLBDERequisitionVM();
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
                    retResults[1] = "This GLBDERequisition already used!";
                    throw new ArgumentNullException("Please Input GLBDERequisition Value", "");
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
        public string[] Update(GLBDERequisitionVM vm, List<HttpPostedFileBase> UploadFiles, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "GLBDERequisitionUpdate"; //Method Name
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
                    sqlText = "UPDATE GLBDERequisitions SET";
                    sqlText += "  TransactionDateTime=@TransactionDateTime";
                    sqlText += " , ReferenceNo1=@ReferenceNo1";
                    sqlText += " , ReferenceNo2=@ReferenceNo2";
                    sqlText += " , ReferenceNo3=@ReferenceNo3";
                    sqlText += " , OpeningBalance=@OpeningBalance";
                    sqlText += " , TotalExpense=@TotalExpense";
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
                    cmdUpdate.Parameters.AddWithValue("@ReferenceNo1", vm.ReferenceNo1 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ReferenceNo2", vm.ReferenceNo2 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ReferenceNo3", vm.ReferenceNo3 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@OpeningBalance", vm.OpeningBalance);
                    cmdUpdate.Parameters.AddWithValue("@TotalExpense", vm.TotalExpense);
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
                        throw new ArgumentNullException("Unexpected error to update GLBDERequisitions.", "");
                    }
                    #endregion SqlExecution
                    #region insert Details from Master into Detail Table
                    #region Form A
                    #region Delete Detail
                    try
                    {
                        retResults = _cDal.DeleteTableInformation(vm.Id.ToString(), "GLBDERequisitionFormAs", "GLBDERequisitionId", currConn, transaction);
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
                    if (vm.glBDERequisitionFormAVMs != null && vm.glBDERequisitionFormAVMs.Count > 0)
                    {
                        GLBDERequisitionFormADAL _formADal = new GLBDERequisitionFormADAL();
                        foreach (var formAVM in vm.glBDERequisitionFormAVMs)
                        {
                            GLBDERequisitionFormAVM dVM = new GLBDERequisitionFormAVM();
                            dVM = formAVM;
                            dVM.GLBDERequisitionId = vm.Id;
                            dVM.TransactionDateTime = vm.TransactionDateTime;
                            dVM.BranchId = vm.BranchId;
                            retResults = _formADal.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                    }
                    #endregion Insert Detail Again
                    #endregion Form A
                    #region Form B
                    #region Delete Detail
                    try
                    {
                        retResults = _cDal.DeleteTableInformation(vm.Id.ToString(), "GLBDERequisitionFormBs", "GLBDERequisitionId", currConn, transaction);
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
                    if (vm.glBDERequisitionFormBVMs != null && vm.glBDERequisitionFormBVMs.Count > 0)
                    {
                        GLBDERequisitionFormBDAL _formBDal = new GLBDERequisitionFormBDAL();

                        foreach (var FormBVM in vm.glBDERequisitionFormBVMs)
                        {
                            GLBDERequisitionFormBVM dVM = new GLBDERequisitionFormBVM();
                            dVM = FormBVM;
                            dVM.GLBDERequisitionId = vm.Id;
                            dVM.TransactionDateTime = vm.TransactionDateTime;
                            dVM.BranchId = vm.BranchId;
                            retResults = _formBDal.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                    }
                    #endregion Insert Detail Again
                    #endregion Form B
                    #region Form C
                    #region Delete Detail
                    try
                    {
                        retResults = _cDal.DeleteTableInformation(vm.Id.ToString(), "GLBDERequisitionFormCs", "GLBDERequisitionId", currConn, transaction);
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
                    if (vm.glBDERequisitionFormCVMs != null && vm.glBDERequisitionFormCVMs.Count > 0)
                    {
                        GLBDERequisitionFormCDAL _formCDal = new GLBDERequisitionFormCDAL();

                        foreach (var FormCVM in vm.glBDERequisitionFormCVMs)
                        {
                            GLBDERequisitionFormCVM dVM = new GLBDERequisitionFormCVM();
                            dVM = FormCVM;
                            dVM.GLBDERequisitionId = vm.Id;
                            dVM.TransactionDateTime = vm.TransactionDateTime;
                            dVM.BranchId = vm.BranchId;
                            retResults = _formCDal.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                    }
                    #endregion Insert Detail Again
                    #endregion Form C
                    #region Form D
                    #region Delete Detail
                    try
                    {
                        retResults = _cDal.DeleteTableInformation(vm.Id.ToString(), "GLBDERequisitionFormDs", "GLBDERequisitionId", currConn, transaction);
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

                    if (vm.glBDERequisitionFormDVMs != null && vm.glBDERequisitionFormDVMs.Count > 0)
                    {
                        GLBDERequisitionFormDDAL _formDDal = new GLBDERequisitionFormDDAL();

                        foreach (var FormDVM in vm.glBDERequisitionFormDVMs)
                        {
                            GLBDERequisitionFormDVM dVM = new GLBDERequisitionFormDVM();
                            dVM = FormDVM;
                            dVM.GLBDERequisitionId = vm.Id;
                            dVM.TransactionDateTime = vm.TransactionDateTime;
                            dVM.BranchId = vm.BranchId;
                            retResults = _formDDal.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                    }
                    #endregion Insert Detail Again
                    #endregion Form D
                    #region Form E
                    #region Delete Detail
                    try
                    {
                        retResults = _cDal.DeleteTableInformation(vm.Id.ToString(), "GLBDERequisitionFormEs", "GLBDERequisitionId", currConn, transaction);
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
                    if (vm.glBDERequisitionFormEVMs != null && vm.glBDERequisitionFormEVMs.Count > 0)
                    {
                        GLBDERequisitionFormEDAL _formEDal = new GLBDERequisitionFormEDAL();

                        foreach (var FormEVM in vm.glBDERequisitionFormEVMs)
                        {
                            GLBDERequisitionFormEVM dVM = new GLBDERequisitionFormEVM();
                            dVM = FormEVM;
                            dVM.GLBDERequisitionId = vm.Id;
                            dVM.TransactionDateTime = vm.TransactionDateTime;
                            dVM.BranchId = vm.BranchId;
                            retResults = _formEDal.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                    }
                    #endregion Insert Detail Again
                    #endregion Form E
                    #region FileUpload

                    GLBDERequisitionFileDAL _formFileDAL = new GLBDERequisitionFileDAL();
                    List<GLBDERequisitionFileVM> fileVMs = new List<GLBDERequisitionFileVM>();
                    foreach (HttpPostedFileBase file in UploadFiles)
                    {
                        if (file != null && Array.Exists(vm.FilesToBeUploaded.Split(','), s => s.Equals(file.FileName)))
                        {
                            if (file != null && file.ContentLength > 0)
                            {
                                GLBDERequisitionFileVM fileVM = new GLBDERequisitionFileVM();
                                string path = vm.Id.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHMMss") + "_" + Path.GetFileName(file.FileName);

                                string saveFilePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Files/Sage/BDERequisition"), path);

                                file.SaveAs(saveFilePath);

                                fileVM.GLBDERequisitionId = vm.Id;
                                fileVM.FileName = path;
                                fileVM.FileOrginalName = file.FileName;
                                fileVMs.Add(fileVM);
                            }
                        }
                    }

                    if (fileVMs != null && fileVMs.Count > 0)
                    {
                        retResults = _formFileDAL.Insert(fileVMs, currConn, transaction);
                    }
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion File Upload
                    #endregion insert Details from Master into Detail Table
                    #region Update Amount
                    GLBDERequisitionVM newVM = new GLBDERequisitionVM();
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
                        // throw new ArgumentNullException("GLBDERequisition Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLBDERequisition Update", "Could not found any item.");
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
        public string[] Delete(GLBDERequisitionVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLBDERequisition"; //Method Name
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
                        sqlText = "update GLBDERequisitions set";
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
                        throw new ArgumentNullException("GLBDERequisition Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLBDERequisition Information Delete", "Could not found any item.");
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

        public string[] Post(GLBDERequisitionVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLBDERequisition"; //Method Name
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
                    #region Insert Into GLDocumentNos, GLMRNos From GLBDERequisitionFormAs,  GLBDERequisitionFormEs
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        GLDocumentNoDAL _glDocumentNoDAL = new GLDocumentNoDAL();
                        GLMRNoDAL _glMRNoDAL = new GLMRNoDAL();

                        List<GLDocumentNoVM> glDocumentNoVMs = new List<GLDocumentNoVM>();
                        List<GLMRNoVM> glMRNoVMs = new List<GLMRNoVM>();

                        #region Form A

                        GLBDERequisitionFormAVM bderFormAVM = new GLBDERequisitionFormAVM();
                        List<GLBDERequisitionFormAVM> bderFormAVMs = new List<GLBDERequisitionFormAVM>();
                        GLBDERequisitionFormADAL _bderFormADAL = new GLBDERequisitionFormADAL();
                        bderFormAVMs = _bderFormADAL.SelectByMasterId(Convert.ToInt32(ids[i]), currConn, transaction);

                        foreach (GLBDERequisitionFormAVM item in bderFormAVMs)
                        {
                            if (item.Post)
                            {
                                retResults[1] = "Data already Posted!";
                                throw new ArgumentNullException(retResults[1], "");
                            }
                            #region MRNo

                            GLMRNoVM glMRNoVM = new GLMRNoVM();

                            glMRNoVM.Name = item.MRNo;
                            glMRNoVM.TransactionType = "BDE";
                            glMRNoVM.ReferenceId = Convert.ToInt32(ids[i]);
                            glMRNoVM.BranchId = item.BranchId;
                            glMRNoVM.CreatedBy = vm.PostedBy;
                            glMRNoVM.CreatedAt = vm.PostedAt;
                            glMRNoVM.CreatedFrom = vm.PostedBy;
                            glMRNoVMs.Add(glMRNoVM);

                            {
                                string[] cFields = { "m.Id!", "d.MRNo", "m.Post", "d.IsRejected" };
                                string[] cValues = { ids[i], item.MRNo, "1", "0" };
                                GLBDERequisitionFormAVM formAVM = _bderFormADAL.SelectAllMRNo(glMRNoVM.TransactionType, item.MRNo, cFields, cValues, currConn, transaction).FirstOrDefault();
                                if (formAVM != null)
                                {
                                    retResults[1] = "MR No: " + item.MRNo + " already used! Code: " + formAVM.Code + " Date: " + formAVM.TransactionDateTime;
                                    throw new ArgumentNullException(retResults[1], "");
                                }
                            }
                            #endregion
                            #region DocumentNo

                            GLDocumentNoVM glDocumentNoVM = new GLDocumentNoVM();
                            glDocumentNoVM.Name = item.DocumentNo;
                            glDocumentNoVM.TransactionType = "BDE";
                            glDocumentNoVM.ReferenceId = Convert.ToInt32(ids[i]);
                            glDocumentNoVM.BranchId = item.BranchId;
                            glDocumentNoVM.CreatedBy = vm.PostedBy;
                            glDocumentNoVM.CreatedAt = vm.PostedAt;
                            glDocumentNoVM.CreatedFrom = vm.PostedBy;
                            glDocumentNoVMs.Add(glDocumentNoVM);

                            {
                                string[] cFields = { "m.Id!", "d.DocumentNo", "m.Post", "d.IsRejected" };
                                string[] cValues = { ids[i], item.DocumentNo, "1", "0" };
                                GLBDERequisitionFormAVM formAVM = _bderFormADAL.SelectAllDocNo(glMRNoVM.TransactionType, item.DocumentNo, cFields, cValues, currConn, transaction).FirstOrDefault();
                                if (formAVM != null)
                                {
                                    retResults[1] = "Document No: " + item.DocumentNo + " already used! Code: " + formAVM.Code + " Date: " + formAVM.TransactionDateTime;
                                    throw new ArgumentNullException(retResults[1], "");
                                }
                            }

                            #endregion
                        }
                        #endregion

                        #region Form E

                        GLBDERequisitionFormEVM bderFormEVM = new GLBDERequisitionFormEVM();
                        List<GLBDERequisitionFormEVM> bderFormEVMs = new List<GLBDERequisitionFormEVM>();
                        GLBDERequisitionFormEDAL _bderFormEDAL = new GLBDERequisitionFormEDAL();
                        bderFormEVMs = _bderFormEDAL.SelectByMasterId(Convert.ToInt32(ids[i]), currConn, transaction);

                        foreach (GLBDERequisitionFormEVM item in bderFormEVMs)
                        {
                            if (item.Post)
                            {
                                retResults[1] = "Data already Posted!";
                                throw new ArgumentNullException(retResults[1], "");
                            }

                            #region MRNo


                            GLMRNoVM glMRNoVM = new GLMRNoVM();
                            glMRNoVM.Name = item.MRNo;
                            glMRNoVM.TransactionType = "PCR";
                            glMRNoVM.ReferenceId = Convert.ToInt32(ids[i]);
                            glMRNoVM.BranchId = item.BranchId;
                            glMRNoVM.CreatedBy = vm.PostedBy;
                            glMRNoVM.CreatedAt = vm.PostedAt;
                            glMRNoVM.CreatedFrom = vm.PostedBy;
                            glMRNoVMs.Add(glMRNoVM);

                            {
                                string[] cFields = { "m.Id!", "d.MRNo", "m.Post", "d.IsRejected" };
                                string[] cValues = { ids[i], item.MRNo, "1", "0" };
                                GLBDERequisitionFormEVM formEVM = _bderFormEDAL.SelectAllMRNo(glMRNoVM.TransactionType, item.MRNo, cFields, cValues, currConn, transaction).FirstOrDefault();
                                if (formEVM != null)
                                {
                                    retResults[1] = "MR No: " + item.MRNo + " already used! Code: " + formEVM.Code + " Date: " + formEVM.TransactionDateTime;
                                    throw new ArgumentNullException(retResults[1], "");
                                }
                            }

                            #endregion


                            #region DocumentNo


                            GLDocumentNoVM glDocumentNoVM = new GLDocumentNoVM();
                            glDocumentNoVM.Name = item.DocumentNo;
                            glDocumentNoVM.TransactionType = "PCR";
                            glDocumentNoVM.ReferenceId = Convert.ToInt32(ids[i]);
                            glDocumentNoVM.BranchId = item.BranchId;
                            glDocumentNoVM.CreatedBy = vm.PostedBy;
                            glDocumentNoVM.CreatedAt = vm.PostedAt;
                            glDocumentNoVM.CreatedFrom = vm.PostedBy;
                            glDocumentNoVMs.Add(glDocumentNoVM);


                            {
                                string[] cFields = { "m.Id!", "d.DocumentNo", "m.Post", "d.IsRejected" };
                                string[] cValues = { ids[i], item.DocumentNo, "1", "0" };
                                GLBDERequisitionFormEVM formEVM = _bderFormEDAL.SelectAllDocNo(glDocumentNoVM.TransactionType, item.DocumentNo, cFields, cValues, currConn, transaction).FirstOrDefault();
                                if (formEVM != null)
                                {
                                    retResults[1] = "Document No: " + item.DocumentNo + " already used! Code: " + formEVM.Code + " Date: " + formEVM.TransactionDateTime;
                                    throw new ArgumentNullException(retResults[1], "");
                                }
                            }
                            #endregion

                        }
                        #endregion
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
                        sqlText = "update GLBDERequisitions set";
                        sqlText += " Post=@Post";
                        sqlText += " ,PostedBy=@PostedBy";
                        sqlText += " ,PostedAt=@PostedAt";
                        sqlText += " ,PostedFrom=@PostedFrom";
                        sqlText += " where Id=@Id";
                        sqlText += " update GLBDERequisitionFormAs set post=@Post where GLBDERequisitionId=@Id";
                        sqlText += " update GLBDERequisitionFormBs set post=@Post where GLBDERequisitionId=@Id";
                        sqlText += " update GLBDERequisitionFormCs set post=@Post where GLBDERequisitionId=@Id";
                        sqlText += " update GLBDERequisitionFormDs set post=@Post where GLBDERequisitionId=@Id";
                        sqlText += " update GLBDERequisitionFormEs set post=@Post where GLBDERequisitionId=@Id";


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
                        throw new ArgumentNullException("GLBDERequisition Delete", vm.Id + " could not Delete.");
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
                                //Prameters: (Branch = this Branch; HaveBDERequisitionApproval=1; HaveApprovalLevel1=1) 
                                string urlPrefix = "";
                                //urlPrefix = "http://localhost:50010";
                                urlPrefix = new GLSettingDAL().settingValue("Email", "BDEURLPrefix", currConn, transaction);
                                GLBDERequisitionVM varVM = new GLBDERequisitionVM();
                                varVM = SelectAll(Convert.ToInt32(ids[i]), null, null, currConn, transaction).FirstOrDefault();
                                string url = urlPrefix + "/Sage/BDERequisition/Approve/"+varVM.Id;

                                List<GLUserVM> userVMs = new List<GLUserVM>();
                                GLUserVM userVM = new GLUserVM();
                                userVM.BranchId = varVM.BranchId;
                                string[] conditionFields = { "HaveBDERequisitionApproval", "HaveApprovalLevel1" };
                                string[] conditionValues = { "1", "1" };

                                userVMs = new GLUserDAL().SelectUserForMail(userVM, conditionFields, conditionValues, currConn, transaction);
                                GLEmailDAL _emailDAL = new GLEmailDAL();
                                GLEmailSettingVM emailSettingVM = new GLEmailSettingVM();
                                string status = "Generated and Waiting for Approve Level1";

                                foreach (var item in userVMs)
                                {
                                    string[] EmailForm = Ordinary.GDICEmailForm(item.FullName, varVM.Code, status, url, "BDEReq");
                                    emailSettingVM.MailHeader = EmailForm[0];
                                    emailSettingVM.MailToAddress = item.Email;
                                    emailSettingVM.MailBody = EmailForm[1];
                                    thread = new Thread(c => _emailDAL.SendEmail(emailSettingVM, thread));
                                    thread.Start();
                                }
                            }
                        }
                    //////////}
                    #endregion
                }
                else
                {
                    throw new ArgumentNullException("GLBDERequisition Information Delete", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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

        public string[] RemoveFile(string id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "RemoveFileGLBDERequisition"; //Method Name
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
                GLBDERequisitionFileDAL _formFileDAL = new GLBDERequisitionFileDAL();

                string FileName = _formFileDAL.SelectById(Convert.ToInt32(id), currConn, transaction).FirstOrDefault().FileName;
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Sage\\BDERequisition\\" + FileName;
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                #region Update Settings

                sqlText = "";
                sqlText = " delete  GLBDERequisitionFiles";
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
                    throw new ArgumentNullException("GLBDERequisition Delete", " could not Delete.");
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

        public string[] ApproveReject(GLBDERequisitionVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "ApproveRejectGLBDERequisition"; //Method Name
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


                            #region Insert Into GLFundReceivedBDERequisitions
                            varVM = SelectAll(vm.Id, null, null, currConn, transaction).FirstOrDefault();
                            #region Update Amount
                            GLBDERequisitionVM newVM = new GLBDERequisitionVM();
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

                                GLFundReceivedBDERequisitionVM frBDEReqVM = new GLFundReceivedBDERequisitionVM();
                                frBDEReqVM.BranchId = varVM.BranchId;
                                frBDEReqVM.ReferenceId = vm.Id;
                                frBDEReqVM.FundAmount = varVM.FundReceive;
                                frBDEReqVM.FinalApprovedDate = Ordinary.StringToDate(vm.NameDate);
                                //vm.Remarks = "";
                                frBDEReqVM.CreatedAt = vm.NameDate;
                                frBDEReqVM.CreatedBy = vm.ByName;
                                frBDEReqVM.CreatedFrom = "";
                                retResults = new GLFundReceivedBDERequisitionDAL().Insert(frBDEReqVM);
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
                                //Prameters: (Branch = this Branch; HaveBDERequisitionApproval=1; HaveApprovalLevel1=1) 

                                #region Declarations
                                string urlPrefix = "";
                                urlPrefix = new GLSettingDAL().settingValue("Email", "BDEURLPrefix", currConn, transaction);
                                string url = urlPrefix + "/Sage/BDERequisition/SelfApproveIndex";

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
                                    url = urlPrefix + "/Sage/BDERequisition/Posted/"+varVM.Id;
                                }
                                else if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == true && varVM.IsApprovedL3 == true && varVM.IsApprovedL4 == true)
                                {
                                    status = "Approval Completed";
                                    string[] cFieldsUser = { "u.LogId" };
                                    string[] cValuesUser = { varVM.CreatedBy };
                                    userVMs = new GLUserDAL().SelectAll(0, cFieldsUser, cValuesUser, currConn, transaction);
                                    url = urlPrefix + "/Sage/BDERequisition/Posted/"+varVM.Id;
                                }
                                else if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == true && varVM.IsApprovedL3 == true && varVM.IsApprovedL4 == false)
                                {
                                    status = "Waiting for Approve Level4";
                                    string[] cFieldsUser = { "u.HaveBDERequisitionApproval", "u.HaveApprovalLevel4" };
                                    string[] cValuesUser = { "1", "1" };
                                    userVMs = new GLUserDAL().SelectUserForMail(userVM, cFieldsUser, cValuesUser, currConn, transaction);
                                    url = urlPrefix + "/Sage/BDERequisition/Approve/"+varVM.Id;
                                }
                                else if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == true && varVM.IsApprovedL3 == false)
                                {
                                    status = "Waiting for Approve Level3";
                                    string[] cFieldsUser = { "u.HaveBDERequisitionApproval", "u.HaveApprovalLevel3" };
                                    string[] cValuesUser = { "1", "1" };
                                    userVMs = new GLUserDAL().SelectUserForMail(userVM, cFieldsUser, cValuesUser, currConn, transaction);
                                    url = urlPrefix + "/Sage/BDERequisition/Approve/"+varVM.Id;
                                }
                                else if (varVM.Post == true && varVM.IsRejected == false && varVM.IsApprovedL1 == true && varVM.IsApprovedL2 == false)
                                {
                                    status = "Waiting for Approve Level2";
                                    string[] cFieldsUser = { "u.HaveBDERequisitionApproval", "u.HaveApprovalLevel2" };
                                    string[] cValuesUser = { "1", "1" };
                                    userVMs = new GLUserDAL().SelectUserForMail(userVM, cFieldsUser, cValuesUser, currConn, transaction);
                                    url = urlPrefix + "/Sage/BDERequisition/Approve/"+varVM.Id;
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
                    throw new ArgumentNullException("GLBDERequisition Information Delete", "Could not found any item.");
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

        public string[] ApproveReject(GLBDERequisitionVM vm, string Level, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLBDERequisition"; //Method Name
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
                sqlText = "update GLBDERequisitions set";
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
                    sqlText += " update GLBDERequisitionFormAs set";
                    sqlText += " IsRejected=@IsRejected,RejectedBy=@RejectedBy,RejectedDate=@RejectedDate,RejectedComments=@RejectedComments";
                    sqlText += " where GLBDERequisitionId=@Id";

                    sqlText += " update GLBDERequisitionFormBs set";
                    sqlText += " IsRejected=@IsRejected,RejectedBy=@RejectedBy,RejectedDate=@RejectedDate,RejectedComments=@RejectedComments";
                    sqlText += " where GLBDERequisitionId=@Id";

                    sqlText += " update GLBDERequisitionFormCs set";
                    sqlText += " IsRejected=@IsRejected,RejectedBy=@RejectedBy,RejectedDate=@RejectedDate,RejectedComments=@RejectedComments";
                    sqlText += " where GLBDERequisitionId=@Id";

                    sqlText += " update GLBDERequisitionFormDs set";
                    sqlText += " IsRejected=@IsRejected,RejectedBy=@RejectedBy,RejectedDate=@RejectedDate,RejectedComments=@RejectedComments";
                    sqlText += " where GLBDERequisitionId=@Id";

                    sqlText += " update GLBDERequisitionFormEs set";
                    sqlText += " IsRejected=@IsRejected,RejectedBy=@RejectedBy,RejectedDate=@RejectedDate,RejectedComments=@RejectedComments";
                    sqlText += " where GLBDERequisitionId=@Id";
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
                    throw new ArgumentNullException("GLBDERequisition Delete", vm.Id + " could not Delete.");
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
        public string[] Audit(GLBDERequisitionVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "AuditGLBDERequisition"; //Method Name
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
                        sqlText = "update GLBDERequisitions set";
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
                        throw new ArgumentNullException("GLBDERequisition Audit", vm.Id + " could not Audit.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLBDERequisition Information Audit", "Could not found any item.");
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
        public DataTable Report(GLBDERequisitionVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
bde.Id
,bde.BranchId
,bde.Code
,bde.TransactionDateTime
,bde.ReferenceNo1
,bde.ReferenceNo2
,bde.ReferenceNo3
,bde.Post
,bde.Remarks
,br.Name BranchName

,bde.CreatedBy
,bde.LastUpdateBy
,bde.PostedBy
,bde.ApprovedByL1
,bde.ApprovedByL2
,bde.ApprovedByL3
,bde.ApprovedByL4
,bde.AuditedBy
,bde.RejectedBy
,ISNULL(bde.OpeningBalance       ,0)OpeningBalance
,ISNULL(bde.TotalExpense         ,0)TotalExpense
,ISNULL(bde.TotalRequisition     ,0)TotalRequisition
,ISNULL(bde.FundReceive          ,0)FundReceive
,ISNULL(bde.NextOpening          ,0)NextOpening
,ISNULL(bde.RequisitionFormA          ,0)RequisitionFormA
,ISNULL(bde.RequisitionFormB          ,0)RequisitionFormB
,ISNULL(bde.RequisitionFormC          ,0)RequisitionFormC
,ISNULL(bde.RequisitionFormD          ,0)RequisitionFormD
,ISNULL(bde.RequisitionFormE          ,0)RequisitionFormE

FROM GLBDERequisitions bde 
LEFT OUTER JOIN GLBranchs br ON bde.BranchId = br.Id
WHERE  1=1  AND bde.IsArchive = 0
";

                if (vm.Id > 0)
                {
                    sqlText += @" and bde.Id=@Id";
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


        ////==================BDEmployeeStatementReport=================
        public DataTable BDEmployeeReport(GLBDERequisitionVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
--6-BD Employee Statement

create table #Temp(PeriodName  varchar(20),EmployeeName varchar(100),Amount Decimal(18,2))

insert into #Temp
select distinct fd.PeriodName , replace(emp.Name,' ','') EmployeeName, sum(fb.Amount) TransactionAmount from GLBDERequisitionFormBs fb
left outer join GLEmployees emp on emp.Id=fb.EmployeeId
left outer join GLFiscalYearDetails fd on fb.transactionDateTime between fd.PeriodStart and fd.PeriodEnd
where 1=1
AND fb.BranchId=@BranchId
AND fb.transactionDateTime between @dtFrom and @dtTo
group by PeriodName, emp.Name


DECLARE @cols AS NVARCHAR(MAX),
    @query  AS NVARCHAR(MAX);

SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(c.EmployeeName) 
            FROM #Temp c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

set @query = 'SELECT PeriodName, ' + @cols + ' from 
            (
                select PeriodName
                    , isnull(amount,0)amount
                    , EmployeeName
                from #Temp
           ) x
            pivot 
            (
                sum(amount)
                for EmployeeName in (' + @cols + ')
            ) p '


execute(@query)
drop table #temp
";

                //if (vm.Id > 0)
                //{
                //    sqlText += @" and bde.Id=@Id";
                //}
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
                //if (vm.Id > 0)
                da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                da.SelectCommand.Parameters.AddWithValue("@dtFrom", Ordinary.DateToString(vm.TransactionDateTimeFrom));
                da.SelectCommand.Parameters.AddWithValue("@dtTo", Ordinary.DateToString(vm.TransactionDateTimeTo));

                da.Fill(dt);
                //dt = Ordinary.DtColumnStringToDate(dt, "TransactionDateTime");
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


        ////==================R21 YearAgentCommissionExpenseReport=================
        public DataTable YearAgentCEReport(GLBDERequisitionVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
--R-21--YearAgentCommissionExpenseReport

declare @GLFiscalYearId as varchar(20)
set @GLFiscalYearId=2
create table #Temp(YearName  varchar(200),PeriodNameSl  varchar(200), Amount Decimal(18,2))

insert into #Temp
select distinct fd.Year, SUBSTRING(fd.PeriodNamesl,1,2)
,isnull(sum(ace.AgencyComExpense),0)+isnull(sum(bdea.BDEAmount),0) AgencyComExpense
  from GLFiscalYearDetails fd
left outer  join 
(
select b.TransactionDateTime, (b.PCAmount)AgencyComExpense 
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
) as ace on  ace.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
left outer  join (
select  a.TransactionDateTime, (a.BDEAmount)BDEAmount
from GLBDERequisitionFormAs a
left outer join GLBDERequisitions m on m.Id=a.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
 )bdea on  bdea.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
 left outer  join (
select b.TransactionDateTime, (b.PCAmount)AgencyComExpense 
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
) as ac on  ac.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd

left outer  join(
select b.TransactionDateTime, (b.Amount)Salary 
from GLBDERequisitionFormBs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
 )bdeb on  bdeb.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
left outer  join (
select c.TransactionDateTime, (c.Amount)BankCharge 
from GLBDERequisitionFormCs c
left outer join GLBDERequisitions m on m.Id=c.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0

 )bdec on  bdec.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd

 left outer  join(
 select d.TransactionDateTime, (d.Amount)ContingencyFund
from GLBDERequisitionFormDs d
left outer join GLBDERequisitions m on m.Id=d.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
)bded on  bded.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd

left outer  join (
select e.TransactionDateTime, (e.BDEAmount)PCRecovery
from GLBDERequisitionFormEs e
left outer join GLBDERequisitions m on m.Id=e.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
)bdee on  bdee.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where   fd.GLFiscalYearId between 1 and 2  
group by fd.year,SUBSTRING(fd.PeriodNamesl,1,2)
order by  SUBSTRING(fd.PeriodNamesl,1,2) 

--select * from #temp
DECLARE @cols AS NVARCHAR(MAX),
    @query  AS NVARCHAR(MAX);

SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(c.YearName) 
            FROM #Temp c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

set @query = 'SELECT PeriodNamesL, ' + @cols + ' from 
            (
                select PeriodNamesL
                    , isnull(Amount,0)amount
                    , YearName
                from #Temp
				
           ) x
		   
            pivot 
            (
                 sum(Amount)
                for YearName in (' + @cols + ')
            ) p 
			'

execute(@query)
drop table #temp
";

                //if (vm.Id > 0)
                //{
                //    sqlText += @" and bde.Id=@Id";
                //}
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
                //if (vm.Id > 0)
                //da.SelectCommand.Parameters.AddWithValue("@GLFiscalYearId", vm.GLFiscalYearId);

                da.Fill(dt);
                dt.Columns.Remove("PeriodNamesL");
                dt.Columns.Add("Month", typeof(string));
                dt.Columns["Month"].SetOrdinal(0);
                for (int i = 0; i < 12; i++)
                {
                    dt.Rows[i]["Month"] = Ordinary.MonthNames[i];
                }

                //dt = Ordinary.DtColumnStringToDate(dt, "TransactionDateTime");
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

        ////==================R22 YearAgentCommissionExpenseReport=================
        public DataTable YearBDEExpenseReport(GLBDERequisitionVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
--R-22-YearBDEExpenseReport

declare @GLFiscalYearId as varchar(20)
set @GLFiscalYearId=2
create table #Temp(YearName  varchar(200),PeriodNameSl  varchar(200), Amount Decimal(18,2))

insert into #Temp
select distinct fd.Year, SUBSTRING(fd.PeriodNamesl,1,2)
,isnull(sum(bdea.BDEAmount),0)+isnull(sum(bdeb.Salary),0)+isnull(sum(bdec.BankCharge),0)-isnull(sum(bdee.PCRecovery),0) TotalBDEExpense
  from GLFiscalYearDetails fd
left outer  join 
(
select b.TransactionDateTime, (b.PCAmount)AgencyComExpense 
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
) as ace on  ace.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
left outer  join (
select  a.TransactionDateTime, (a.BDEAmount)BDEAmount
from GLBDERequisitionFormAs a
left outer join GLBDERequisitions m on m.Id=a.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
 )bdea on  bdea.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
 left outer  join (
select b.TransactionDateTime, (b.PCAmount)AgencyComExpense 
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
) as ac on  ac.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd

left outer  join(
select b.TransactionDateTime, (b.Amount)Salary 
from GLBDERequisitionFormBs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
 )bdeb on  bdeb.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
left outer  join (
select c.TransactionDateTime, (c.Amount)BankCharge 
from GLBDERequisitionFormCs c
left outer join GLBDERequisitions m on m.Id=c.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0

 )bdec on  bdec.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd

 left outer  join(
 select d.TransactionDateTime, (d.Amount)ContingencyFund
from GLBDERequisitionFormDs d
left outer join GLBDERequisitions m on m.Id=d.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
)bded on  bded.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd

left outer  join (
select e.TransactionDateTime, (e.BDEAmount)PCRecovery
from GLBDERequisitionFormEs e
left outer join GLBDERequisitions m on m.Id=e.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
)bdee on  bdee.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where   fd.GLFiscalYearId between 1 and 2  
group by fd.year,SUBSTRING(fd.PeriodNamesl,1,2)
order by  SUBSTRING(fd.PeriodNamesl,1,2) 

--select * from #temp
DECLARE @cols AS NVARCHAR(MAX),
    @query  AS NVARCHAR(MAX);

SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(c.YearName) 
            FROM #Temp c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

set @query = 'SELECT PeriodNamesL, ' + @cols + ' from 
            (
                select PeriodNamesL
                    , isnull(Amount,0)amount
                    , YearName
                from #Temp
				
           ) x
		   
            pivot 
            (
                 sum(Amount)
                for YearName in (' + @cols + ')
            ) p 
			'

execute(@query)
drop table #temp
";

                //if (vm.Id > 0)
                //{
                //    sqlText += @" and bde.Id=@Id";
                //}
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
                //if (vm.Id > 0)
                //da.SelectCommand.Parameters.AddWithValue("@GLFiscalYearId", vm.GLFiscalYearId);

                da.Fill(dt);
                dt.Columns.Remove("PeriodNamesL");
                dt.Columns.Add("Month", typeof(string));
                dt.Columns["Month"].SetOrdinal(0);
                for (int i = 0; i < 12; i++)
                {
                    dt.Rows[i]["Month"] = Ordinary.MonthNames[i];
                }

                //dt = Ordinary.DtColumnStringToDate(dt, "TransactionDateTime");
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

        ////==================R23 MonthTotalExpenseReport=================
        public DataTable MonthTotalExpenseReport(GLBDERequisitionVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
--R-23--MonthTotalExpenseReport

declare @GLFiscalYearId as varchar(20)
set @GLFiscalYearId=2


select distinct fd.Id,fd.PeriodName
,isnull(sum(be.BranchExpense),0)BranchExpense
,isnull(sum(ace.AgencyComExpense),0)AgencyComExpense
,isnull(sum(bdea.BDEAmount),0)BDEAmount
,isnull(sum(bdeb.Salary),0)BDEEmployeeSalary
,isnull(sum(bdec.BankCharge),0)BDEBankCharge
,isnull(sum(bdee.PCRecovery),0)PCRecovery
,isnull(sum(bdea.BDEAmount),0) +isnull(sum(bdeb.Salary),0)+isnull(sum(bdec.BankCharge),0)-isnull(sum(bdee.PCRecovery),0) TotalBDEExpense
,isnull(sum(be.BranchExpense),0)+isnull(sum(ace.AgencyComExpense),0)+isnull(sum(bdea.BDEAmount),0) +isnull(sum(bdeb.Salary),0)+isnull(sum(bdec.BankCharge),0)-isnull(sum(bdee.PCRecovery),0) TotalExpense
  from GLFiscalYearDetails fd
 
left outer join (select  d.TransactionDateTime, (d.TransactionAmount)BranchExpense
 from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.Id=d.GLFinancialTransactionId
where m.IsApprovedL4=1 and m.IsRejected=0
 ) as be on be.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
left outer  join (
select b.TransactionDateTime, (b.PCAmount)AgencyComExpense 
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
) as ace on  ace.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
left outer  join (
select  a.TransactionDateTime, (a.BDEAmount)BDEAmount  
from GLBDERequisitionFormAs a
left outer join GLBDERequisitions m on m.Id=a.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
 )bdea on  bdea.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd

left outer  join(
select b.TransactionDateTime, (b.Amount)Salary 
from GLBDERequisitionFormBs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
 )bdeb on  bdeb.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
left outer  join (
select c.TransactionDateTime, (c.Amount)BankCharge 
from GLBDERequisitionFormCs c
left outer join GLBDERequisitions m on m.Id=c.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0

 )bdec on  bdec.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd

 left outer  join(
 select d.TransactionDateTime, (d.Amount)ContingencyFund
from GLBDERequisitionFormDs d
left outer join GLBDERequisitions m on m.Id=d.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
)bded on  bded.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd

left outer  join (
select e.TransactionDateTime, (e.BDEAmount)PCRecovery
from GLBDERequisitionFormEs e
left outer join GLBDERequisitions m on m.Id=e.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
)bdee on  bdee.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where   fd.GLFiscalYearId=@GLFiscalYearId 
group by fd.Id,fd.PeriodName
order by fd.Id
";

                //if (vm.Id > 0)
                //{
                //    sqlText += @" and bde.Id=@Id";
                //}
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
                //if (vm.Id > 0)
                //da.SelectCommand.Parameters.AddWithValue("@GLFiscalYearId", vm.GLFiscalYearId);

                da.Fill(dt);
                dt.Columns.Remove("PeriodName");
                dt.Columns.Add("Month", typeof(string));
                dt.Columns["Month"].SetOrdinal(0);
                for (int i = 0; i < 12; i++)
                {
                    dt.Rows[i]["Month"] = Ordinary.MonthNames[i];
                }

                //dt = Ordinary.DtColumnStringToDate(dt, "TransactionDateTime");
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
        public DataTable OpeningBDERequesition(string TransactionId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
--declare @TransactionId as varchar(20)
declare @DateFrom as varchar(20)
declare @BranchId as varchar(20)
--set @TransactionId=10
declare @Opening as Decimal(18,2)
declare @LastRemitDate as varchar(20)
declare @LastRemitAmount as Decimal(18,2)

select @DateFrom=TransactionDateTime,@BranchId=BranchId from GLPettyCashRequisitions where id=@TransactionId
  set @Opening=0;
 set @LastRemitDate=@DateFrom;
 set @LastRemitAmount=0;
 

 select @Opening=isnull(sum(m.FundAmount),0) from GLFundReceivedBDERequisitions as m
where m.IsReceived=1  
and m.ReceivedAt<@DateFrom
and m.BranchId=@BranchId
 
select @Opening=@Opening-isnull(sum(a.BDEAmount),0)
from GLBDERequisitionFormAs a
left outer join GLBDERequisitions m on m.Id=a.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0 and a.IsRejected=0  and a.IsFundReceived=1
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId
 
select @Opening=@Opening-isnull(sum(b.Amount),0)
from GLBDERequisitionFormBs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0 and b.IsRejected=0  and  b.IsFundReceived=1
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId
 
select @Opening=@Opening-isnull(sum(c.Amount),0)
from GLBDERequisitionFormCs c
left outer join GLBDERequisitions m on m.Id=c.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0 and c.IsRejected=0  and  c.IsFundReceived=1
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId
 
select @Opening=@Opening+isnull(sum(e.BDEAmount),0)
from GLBDERequisitionFormEs e
left outer join GLBDERequisitions m on m.Id=e.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0 and e.IsRejected=0  and  e.IsFundReceived=1
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId

 select top 1 @LastRemitDate=FinalApprovedDate,@LastRemitAmount=FundAmount from GLFundReceivedBDERequisitions
 where IsReceived=1 and  branchid=@BranchId 
 and ReferenceId<@TransactionId
select @Opening Opening,@LastRemitDate LastRemitDate,@LastRemitAmount LastRemitAmount
 


";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

                da.SelectCommand.Parameters.AddWithValue("@TransactionId", TransactionId);

                da.Fill(dt);
                dt = Ordinary.DtColumnStringToDate(dt, "LastRemitDate");
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
        public string[] UpdateAmount(GLBDERequisitionVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
----------DECLARE @Id   int
----------DECLARE @BranchId   int

DECLARE @OpeningBalance  decimal(18, 3)
DECLARE @TotalExpense  decimal(18, 3)
DECLARE @TotalRequisition  decimal(18, 3)
DECLARE @FundReceive  decimal(18, 3)
DECLARE @NextOpening  decimal(18, 3)


DECLARE @RequisitionFormA  decimal(18, 3)
DECLARE @RequisitionFormB  decimal(18, 3)
DECLARE @RequisitionFormC  decimal(18, 3)
DECLARE @RequisitionFormD  decimal(18, 3)
DECLARE @RequisitionFormE  decimal(18, 3)


----------SET @Id=3
----------SET @BranchId=5

--------------@OpeningBalance--------------
SELECT TOP 1 @dateFROM=TransactionDateTime,@PreviousId=Id, @OpeningBalance = ISNULL(NextOpening,0)  FROM GLBDERequisitions 
WHERE BranchId=@BranchId AND  Id<@Id AND Post=1 AND IsRejected=0
ORDER BY Id DESC

SELECT @dateTo=TransactionDateTime FROM GLBDERequisitions WHERE  BranchId=@BranchId AND Id=@Id

SELECT @OpeningBalance = ISNULL(@OpeningBalance,0)

----------SELECT @dateFROM TransactionDateTime,@PreviousId PreviousId
----------SELECT @dateTo TransactionDateTime


--------------@TotalRequisition--------------
set @TotalRequisition = 0;
set @RequisitionFormA = 0;
set @RequisitionFormB = 0;
set @RequisitionFormC = 0;
set @RequisitionFormD = 0;
set @RequisitionFormE = 0;

SELECT @RequisitionFormA = Isnull(SUM(BDEAmount),0) FROM GLBDERequisitionFormAs
WHERE 1=1 
AND GLBDERequisitionId = @Id
AND IsRejected = 0

SELECT @RequisitionFormB = Isnull(SUM(Amount),0) FROM GLBDERequisitionFormBs
WHERE 1=1 AND BranchId=@BranchId 
AND GLBDERequisitionId = @Id
AND IsRejected = 0

SELECT @RequisitionFormC = Isnull(SUM(Amount),0) FROM GLBDERequisitionFormCs
WHERE 1=1 AND BranchId=@BranchId 
AND GLBDERequisitionId = @Id
AND IsRejected = 0

SELECT @RequisitionFormD = Isnull(SUM(Amount),0) FROM GLBDERequisitionFormDs
WHERE 1=1 AND BranchId=@BranchId 
AND GLBDERequisitionId = @Id
AND IsRejected = 0

SELECT @RequisitionFormE = Isnull(SUM(BDEAmount),0) FROM GLBDERequisitionFormEs
WHERE 1=1 AND BranchId=@BranchId 
AND GLBDERequisitionId = @Id
AND IsRejected = 0


SELECT @TotalRequisition = ISNULL(@RequisitionFormA,0) + ISNULL(@RequisitionFormB,0) + ISNULL(@RequisitionFormC,0) + ISNULL(@RequisitionFormD,0) - ISNULL(@RequisitionFormE,0)



--------------@FundReceive--------------

select @FundReceive = ISNULL(@TotalRequisition,0) - ISNULL(@OpeningBalance,0)



--------------------------------@TotalExpense--------------
------@BankCharge------
SELECT @TotalExpense = ISNULL(SUM(TransactionAmount),0) FROM GLBDEExpenseDetails d
LEFT OUTER JOIN GLBDEExpenses m ON m.Id = d.GLBDEExpenseId
WHERE 1=1 AND m.BranchId=@BranchId
AND m.TransactionDateTime between @dateFROM AND @dateTo
AND m.Post=1 AND m.IsRejected=0

------@FormA------
SELECT @TotalExpense = @TotalExpense + ISNULL(SUM(fa.PaidAmount),0) FROM GLBDERequisitionFormAs fa
LEFT OUTER JOIN GLBDERequisitions m ON m.Id = fa.GLBDERequisitionId
WHERE 1=1 AND m.BranchId=@BranchId
AND fa.PaymentDate between @dateFROM AND @dateTo
AND m.Post=1 AND m.IsRejected=0

------@FormB------
SELECT @TotalExpense = @TotalExpense + ISNULL(SUM(fb.PaidAmount),0) FROM GLBDERequisitionFormBs fb
LEFT OUTER JOIN GLBDERequisitions m ON m.Id = fb.GLBDERequisitionId
WHERE 1=1 AND m.BranchId=@BranchId
AND fb.PaymentDate between @dateFROM AND @dateTo
AND m.Post=1 AND m.IsRejected=0

------@FormE------
SELECT @TotalExpense = @TotalExpense - ISNULL(SUM(fe.PaidAmount),0) FROM GLBDERequisitionFormEs fe
LEFT OUTER JOIN GLBDERequisitions m ON m.Id = fe.GLBDERequisitionId
WHERE 1=1 AND m.BranchId=@BranchId
AND fe.PaymentDate between @dateFROM AND @dateTo
AND m.Post=1 AND m.IsRejected=0





--------------@NextOpening--------------
select @NextOpening = @FundReceive - @TotalExpense


----------SELECT @OpeningBalance OpeningBalance, @TotalExpense TotalExpense
----------, @TotalRequisition TotalRequisition
----------, @FundReceive FundReceive, @NextOpening NextOpening
";

                    sqlText += @"


UPDATE GLBDERequisitions SET 
 OpeningBalance=@OpeningBalance
,TotalRequisition=@TotalRequisition
,FundReceive=@FundReceive
,TotalExpense=@TotalExpense

,NextOpening=@NextOpening


,RequisitionFormA=@RequisitionFormA
,RequisitionFormB=@RequisitionFormB
,RequisitionFormC=@RequisitionFormC
,RequisitionFormD=@RequisitionFormD
,RequisitionFormE=@RequisitionFormE



                                WHERE Id=@Id
";
                    {
                        SqlCommand cmdUpdateAmount = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdateAmount.Parameters.AddWithValue("@Id", vm.Id);
                        cmdUpdateAmount.Parameters.AddWithValue("@BranchId", vm.BranchId);
                        var exeRes = cmdUpdateAmount.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        ////////if (transResult <= 0)
                        ////////{
                        ////////    retResults[3] = sqlText;
                        ////////    throw new ArgumentNullException("Unexpected error to update amount.", "");
                        ////////}
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
