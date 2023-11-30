using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SymOrdinary;
using SymServices.Common;
using SymViewModel.Sage;
using System.Threading;
using System.Linq;

namespace SymServices.Sage
{
    public class GLBDERequisitionFormBDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        public static Thread thread;
        #endregion
        #region Methods
                //==================SelectByMasterId=================
        public List<GLBDERequisitionFormBVM> SelectByMasterId(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLBDERequisitionFormBVM> VMs = new List<GLBDERequisitionFormBVM>();
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
                string[] conditionField = { "fb.GLBDERequisitionId" };
                string[] conditionValue = { Id.ToString() };
                VMs = SelectAll(conditionField, conditionValue, currConn, transaction);
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
        //==================SelectById=================
        public List<GLBDERequisitionFormBVM> SelectById(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLBDERequisitionFormBVM> VMs = new List<GLBDERequisitionFormBVM>();
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
                string[] conditionField = { "fb.Id" };
                string[] conditionValue = { Id.ToString() };
                VMs = SelectAll(conditionField, conditionValue, currConn, transaction);
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
        
        //==================SelectAll_In_BDERequisitionPaidDetail=================
        public List<GLBDERequisitionFormBVM> SelectAll_In_BDERequisitionPaidDetail(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLBDERequisitionFormBVM> VMs = new List<GLBDERequisitionFormBVM>();
            GLBDERequisitionFormBVM vm;
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
 fb.Id
,fb.GLBDERequisitionId
,fb.BranchId
,fb.TransactionDateTime
,fb.EmployeeId
,fb.Amount
,fb.Post
,isnull(fb.IsRejected           ,'0')IsRejected
,isnull(fb.RejectedBy           ,'0')RejectedBy
,isnull(fb.RejectedDate         ,'19000101')RejectedDate
,isnull(fb.RejectedComments     ,'NA')RejectedComments
,isnull(fb.IsPaid				,'0')IsPaid
,isnull(fb.PaidAmount           ,'0')PaidAmount
,fb.PaidTo
,fb.PaymentDate


,fb.Remarks
,e.Name EmployeeName
,br.Name BranchName
,bde.Code
FROM  GLBDERequisitionFormBs  fb
LEFT OUTER JOIN GLEmployees e ON fb.EmployeeId = e.Id
LEFT OUTER JOIN GLBranchs br ON fb.BranchId = br.Id
LEFT OUTER JOIN GLBDERequisitions bde ON bde.Id = fb.GLBDERequisitionId
WHERE  1=1
AND fb.Id IN (
SELECT GLBDERequisitionDetailId FROM GLBDERequisitionPaidDetails
WHERE  1=1 AND TransactionType='FormB'
)
";


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

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLBDERequisitionFormBVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.GLBDERequisitionId = Convert.ToInt32(dr["GLBDERequisitionId"]);
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.EmployeeId =Convert.ToInt32( dr["EmployeeId"] );
                    vm.Amount = Convert.ToDecimal(dr["Amount"] );
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.IsRejected = Convert.ToBoolean(dr["IsRejected"]);
                    vm.RejectedBy = dr["RejectedBy"].ToString();
                    vm.RejectedDate = dr["RejectedDate"].ToString();
                    vm.RejectedComments = dr["RejectedComments"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.EmployeeName = dr["EmployeeName"].ToString();

                    vm.IsPaid = Convert.ToBoolean(dr["IsPaid"]);
                    vm.PaidAmount = Convert.ToInt32(dr["PaidAmount"]);
                    vm.PaidTo = dr["PaidTo"].ToString();
                    vm.PaymentDate = Ordinary.StringToDate(dr["PaymentDate"].ToString());

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
       
        //==================SelectAll_NotIn_BDERequisitionPaidDetail=================
        public List<GLBDERequisitionFormBVM> SelectAll_NotIn_BDERequisitionPaidDetail(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLBDERequisitionFormBVM> VMs = new List<GLBDERequisitionFormBVM>();
            GLBDERequisitionFormBVM vm;
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
 fb.Id
,fb.GLBDERequisitionId
,fb.BranchId
,fb.TransactionDateTime
,fb.EmployeeId
,fb.Amount
,fb.Post
,isnull(fb.IsRejected           ,'0')IsRejected
,isnull(fb.RejectedBy           ,'0')RejectedBy
,isnull(fb.RejectedDate         ,'19000101')RejectedDate
,isnull(fb.RejectedComments     ,'NA')RejectedComments
,isnull(fb.IsPaid				,'0')IsPaid
,isnull(fb.PaidAmount           ,'0')PaidAmount
,fb.PaidTo
,fb.PaymentDate


,fb.Remarks
,e.Name EmployeeName
,br.Name BranchName
,bde.Code
FROM  GLBDERequisitionFormBs  fb
LEFT OUTER JOIN GLEmployees e ON fb.EmployeeId = e.Id
LEFT OUTER JOIN GLBranchs br ON fb.BranchId = br.Id
LEFT OUTER JOIN GLBDERequisitions bde ON bde.Id = fb.GLBDERequisitionId
WHERE  1=1
AND fb.Id NOT IN (
SELECT GLBDERequisitionDetailId FROM GLBDERequisitionPaidDetails
WHERE  1=1 AND TransactionType='FormB'
)
";


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

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLBDERequisitionFormBVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.GLBDERequisitionId = Convert.ToInt32(dr["GLBDERequisitionId"]);
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.EmployeeId =Convert.ToInt32( dr["EmployeeId"] );
                    vm.Amount = Convert.ToDecimal(dr["Amount"] );
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.IsRejected = Convert.ToBoolean(dr["IsRejected"]);
                    vm.RejectedBy = dr["RejectedBy"].ToString();
                    vm.RejectedDate = dr["RejectedDate"].ToString();
                    vm.RejectedComments = dr["RejectedComments"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.EmployeeName = dr["EmployeeName"].ToString();

                    vm.IsPaid = Convert.ToBoolean(dr["IsPaid"]);
                    //vm.PaidAmount = Convert.ToInt32(dr["PaidAmount"]);
                    vm.PaidAmount = Convert.ToInt32(dr["Amount"]);

                    vm.PaidTo = dr["PaidTo"].ToString();
                    vm.PaymentDate = Ordinary.StringToDate(dr["PaymentDate"].ToString());

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
        public List<GLBDERequisitionFormBVM> SelectAll(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLBDERequisitionFormBVM> VMs = new List<GLBDERequisitionFormBVM>();
            GLBDERequisitionFormBVM vm;
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
 fb.Id
,fb.GLBDERequisitionId
,fb.BranchId
,fb.TransactionDateTime
,fb.EmployeeId
,fb.Amount
,fb.Post
,isnull(fb.IsRejected           ,'0')IsRejected
,isnull(fb.RejectedBy           ,'0')RejectedBy
,isnull(fb.RejectedDate         ,'19000101')RejectedDate
,isnull(fb.RejectedComments     ,'NA')RejectedComments
,isnull(fb.IsPaid				,'0')IsPaid
,isnull(fb.PaidAmount           ,'0')PaidAmount
,fb.PaidTo
,fb.PaymentDate


,fb.Remarks
,e.Name EmployeeName
,br.Name BranchName
,bde.Code
FROM  GLBDERequisitionFormBs  fb
LEFT OUTER JOIN GLEmployees e ON fb.EmployeeId = e.Id
LEFT OUTER JOIN GLBranchs br ON fb.BranchId = br.Id
LEFT OUTER JOIN GLBDERequisitions bde ON bde.Id = fb.GLBDERequisitionId
WHERE  1=1
";


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

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLBDERequisitionFormBVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.GLBDERequisitionId = Convert.ToInt32(dr["GLBDERequisitionId"]);
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.EmployeeId =Convert.ToInt32( dr["EmployeeId"] );
                    vm.Amount = Convert.ToDecimal(dr["Amount"] );
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.IsRejected = Convert.ToBoolean(dr["IsRejected"]);
                    vm.RejectedBy = dr["RejectedBy"].ToString();
                    vm.RejectedDate = dr["RejectedDate"].ToString();
                    vm.RejectedComments = dr["RejectedComments"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.EmployeeName = dr["EmployeeName"].ToString();

                    vm.IsPaid = Convert.ToBoolean(dr["IsPaid"]);
                    vm.PaidAmount = Convert.ToInt32(dr["PaidAmount"]);
                    vm.PaidTo = dr["PaidTo"].ToString();
                    vm.PaymentDate = Ordinary.StringToDate(dr["PaymentDate"].ToString());

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
        public string[] Insert(GLBDERequisitionFormBVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Insert GLBDERequisitionFormB"; //Method Name
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


                vm.Id = _cDal.NextId(" GLBDERequisitionFormBs", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO  GLBDERequisitionFormBs(
Id
,GLBDERequisitionId
,TransactionDateTime
,BranchId
,EmployeeId
,Amount
,Post
,IsRejected
,Remarks

) VALUES (
@Id
,@GLBDERequisitionId
,@TransactionDateTime
,@BranchId
,@EmployeeId
,@Amount
,@Post
,@IsRejected
,@Remarks
) 
";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@GLBDERequisitionId", vm.GLBDERequisitionId);
                    cmdInsert.Parameters.AddWithValue("@TransactionDateTime", Ordinary.DateToString(vm.TransactionDateTime));
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@Amount", vm.Amount);
                    
                    cmdInsert.Parameters.AddWithValue("@Post", false);
                    cmdInsert.Parameters.AddWithValue("@IsRejected", false);
                    cmdInsert.Parameters.AddWithValue("@RejectedBy", vm.RejectedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@RejectedDate", vm.RejectedDate ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@RejectedComments", vm.RejectedComments ?? Convert.DBNull);

                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);

                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update  GLBDERequisitionFormBs.", "");
                    }
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This  GLBDERequisitionFormB already used!";
                    throw new ArgumentNullException("Please Input  GLBDERequisitionFormB Value", "");
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
        ////==================Delete =================
        public string[] AcceptReject(GLBDERequisitionFormBVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Delete GLBDERequisitionFormB"; //Method Name
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
                        sqlText = "update  GLBDERequisitionFormBs set";
                        sqlText += " IsRejected=@IsRejected";
                        sqlText += " ,RejectedBy=@RejectedBy";
                        sqlText += " ,RejectedDate=@RejectedDate";
                        sqlText += " ,RejectedComments=@RejectedComments";
                        sqlText += " where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsRejected", vm.IsRejected);
                        cmdUpdate.Parameters.AddWithValue("@RejectedBy", vm.RejectedBy);
                        cmdUpdate.Parameters.AddWithValue("@RejectedDate", vm.RejectedDate);
                        cmdUpdate.Parameters.AddWithValue("@RejectedComments", vm.RejectedComments ?? Convert.DBNull);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);

                        #region Variables
                        List<GLUserVM> userVMs = new List<GLUserVM>();
                        GLEmailDAL _emailDAL = new GLEmailDAL();
                        GLEmailSettingVM emailSettingVM = new GLEmailSettingVM();
                        GLBDERequisitionVM masterVM = new GLBDERequisitionVM();
                        #endregion Variables
                        #region Send Email
                        string SendEmail = new GLSettingDAL().settingValue("Email", "SendEmail", currConn, transaction);
                        if (SendEmail == "Y")
                        {
                            string urlPrefix = "";
                            urlPrefix = new GLSettingDAL().settingValue("Email", "BDEURLPrefix", currConn, transaction);
                            string url = urlPrefix + "/Sage/BDERequisition/SelfApproveIndex";
                            {
                                string[] cFields = { "fb.Id" };
                                string[] cValues = { ids[i] };
                                masterVM = new GLBDERequisitionDAL().SelectAllAudit(0, cFields, cValues, currConn, transaction).FirstOrDefault();
                                string status = "Rejected";
                                string[] cFieldsUser = { "u.LogId" };
                                string[] cValuesUser = { masterVM.CreatedBy };
                                userVMs = new GLUserDAL().SelectAll(0, cFieldsUser, cValuesUser, currConn, transaction);
                                url = urlPrefix + "/Sage/BDERequisition/Posted/" + masterVM.Id;

                                foreach (var item in userVMs)
                                {
                                    string[] EmailForm = Ordinary.GDICEmailForm(item.FullName, masterVM.Code, status, url, "BDEReq");
                                    emailSettingVM.MailHeader = EmailForm[0];
                                    emailSettingVM.MailToAddress = item.Email;
                                    emailSettingVM.MailBody = EmailForm[1];

                                    thread = new Thread(c => _emailDAL.SendEmail(emailSettingVM, thread));
                                    thread.Start();
                                }
                                #region Mail To IT (If Neccessary)
                                if (1 == 0)
                                {
                                    string ITEmail = new GLSettingDAL().settingValue("Email", "ITEmail", currConn, transaction);

                                    string[] EmailForm = Ordinary.GDICEmailForm("Concern", masterVM.Code, status, url, "BDEReq");
                                    emailSettingVM.MailHeader = EmailForm[0];
                                    emailSettingVM.MailToAddress = ITEmail;
                                    emailSettingVM.MailBody = EmailForm[1];

                                    thread = new Thread(c => _emailDAL.SendEmail(emailSettingVM, thread));
                                    thread.Start();
                                }
                                #endregion

                            }
                        }
                        #endregion
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(" GLBDERequisitionFormB Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException(" GLBDERequisitionFormB Information Delete", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    if (vm.IsRejected)
                    {
                        retResults[1] = "Data Reject Successfully!";
                    }
                    else
                    {
                        retResults[1] = "Data Accept Successfully!";
                    }
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

        ////==================Report=================
        public DataTable Report(GLBDERequisitionFormBVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
bde.Code
,fb.TransactionDateTime
,e.Name EmployeeName
,fb.Amount
,br.Name BranchName
,fb.Remarks
,fb.GLBDERequisitionId
 ,case 
  when fb.IsRejected =1 then 'Decline' 
  when bde.IsRejected =1 then 'Rejected' 
  when bde.IsApprovedL4 =1 then 'Approval Completed' 
  when bde.IsApprovedL3 =1 then 'Waiting for Approval Level4' 
  when bde.IsApprovedL2 =1 then 'Waiting for Approval Level3' 
  when bde.IsApprovedL1 =1 then 'Waiting for Approval Level2' 
 when bde.Post=1 then 'Waiting for Approval Level1' 
 else 'Not Posted' end  Status
FROM  GLBDERequisitionFormBs  fb
LEFT OUTER JOIN GLEmployees e ON fb.EmployeeId = e.Id
LEFT OUTER JOIN GLBranchs br ON fb.BranchId = br.Id
LEFT OUTER JOIN GLBDERequisitions bde ON bde.Id = fb.GLBDERequisitionId
WHERE  1=1
";
                 if (vm.Status == "Created")
                {
                    sqlText += @" and ( bde.IsRejected <> 1 and  bde.IsApprovedL4 <> 1 and bde.Post<>1)";
                }
                else if (vm.Status == "Posted")
                {
                    sqlText += @" and ( bde.IsRejected <> 1 and  bde.IsApprovedL4 <> 1 and bde.Post=1)";
                }
                else if (vm.Status == "Rejected")
                {
                    sqlText += @" and ( bde.IsRejected = 1 and  bde.IsApprovedL4 <> 1 and bde.Post=1)";
                }
                else if (vm.Status == "Approval Completed")
                {
                    sqlText += @" and ( bde.IsRejected <> 1 and  bde.IsApprovedL4 = 1 and bde.Post=1)";
                }
                else if (vm.Status == "Decline")
                {
                    sqlText += @" and ( fb.IsRejected = 1)";
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
                sqlText += @" ORDER BY bde.TransactionDateTime, bde.Code";

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
                da.Fill(dt);
                string[] changeColumn = {"TransactionDateTime"};
                dt = Ordinary.DtMultiColumnStringToDate(dt, changeColumn);
                dt.Columns["TransactionDateTime"].SetOrdinal(1);
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


        //==================UpdatePayment =================
        public string[] UpdatePayment(List<GLBDERequisitionFormBVM> VMs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "GLBDERequisitionFormBUpdate"; //Method Name
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

                if (VMs != null && VMs.Count > 0)
                {
                    #region Update Settings
                    #region SqlText
                    sqlText = "";
                    sqlText = "UPDATE GLBDERequisitionFormBs SET";
                    sqlText += "  IsPaid=@IsPaid";
                    sqlText += " , PaidAmount=@PaidAmount";
                    sqlText += " , PaidTo=@PaidTo";
                    sqlText += " , PaymentDate=@PaymentDate";
                    sqlText += " WHERE Id=@Id";

                    #endregion SqlText
                    #region SqlExecution
                    foreach (var vm in VMs)
                    {
                        vm.PaymentDate = vm.TransactionDateTime;

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                        cmdUpdate.Parameters.AddWithValue("@IsPaid", vm.IsPaid);
                        cmdUpdate.Parameters.AddWithValue("@PaidAmount", vm.PaidAmount);
                        cmdUpdate.Parameters.AddWithValue("@PaidTo", vm.PaidTo ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValue("@PaymentDate", Ordinary.DateToString(vm.PaymentDate));

                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update GLBDERequisitionFormBs.", "");
                        }
                    }

                    #endregion SqlExecution

                    retResults[2] = VMs.FirstOrDefault().GLBDERequisitionId.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("GLBDERequisitionFormB Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLBDERequisitionFormB Update", "Could not found any item.");
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
                retResults[2] = VMs.FirstOrDefault().GLBDERequisitionId.ToString();
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
