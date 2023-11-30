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
    public class GLBDERequisitionFormADAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        public static Thread thread;
        #endregion
        #region Methods

        //==================SelectByMasterId=================
        public List<GLBDERequisitionFormAVM> SelectByMasterId(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLBDERequisitionFormAVM> VMs = new List<GLBDERequisitionFormAVM>();
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
                string[] conditionField = { "fa.GLBDERequisitionId" };
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
        public List<GLBDERequisitionFormAVM> SelectById(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLBDERequisitionFormAVM> VMs = new List<GLBDERequisitionFormAVM>();
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
                string[] conditionField = { "fa.Id" };
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

        //==================SelectAllMRNo=================
        public List<GLBDERequisitionFormAVM> SelectAllMRNoBackup(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
SELECT 
m.Id
,m.Code
,m.TransactionDateTime
,d.MRNo 
FROM GLBDERequisitionFormAs d
LEFT OUTER JOIN GLBDERequisitions m on m.Id=d.GLBDERequisitionId
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
                    vm = new GLBDERequisitionFormAVM();
                    //////////vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.MRNo = dr["MRNo"].ToString();
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

        public List<GLBDERequisitionFormAVM> SelectAllDocNo(string TransactionType, string DocumentNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
SELECT 
m.Id
,m.Code
,m.TransactionDateTime
,d.MRNo 
FROM GLBDERequisitionFormAs d
LEFT OUTER JOIN GLBDERequisitions m on m.Id=d.GLBDERequisitionId
WHERE  1=1
 and DocumentNo not in( select distinct Name from GLDocumentNos
 where Name=@DocumentNo and transactionType=@TransactionType and IsActive=0)

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

                objComm.Parameters.AddWithValue("@TransactionType", TransactionType);
                //objComm.Parameters.AddWithValue("@DocumentNo", DocumentNo);


                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLBDERequisitionFormAVM();
                    //////////vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.MRNo = dr["MRNo"].ToString();
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

        public List<GLBDERequisitionFormAVM> SelectAllMRNo(string TransactionType, string MRNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
SELECT 
m.Id
,m.Code
,m.TransactionDateTime
,d.MRNo 
FROM GLBDERequisitionFormAs d
LEFT OUTER JOIN GLBDERequisitions m on m.Id=d.GLBDERequisitionId
WHERE  1=1
 and MRNo not in( select distinct Name from GLMRNos
 where Name=@MRNo and transactionType=@TransactionType and IsActive=0)

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

                objComm.Parameters.AddWithValue("@TransactionType", TransactionType);
                //objComm.Parameters.AddWithValue("@MRNo", MRNo);


                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLBDERequisitionFormAVM();
                    //////////vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.MRNo = dr["MRNo"].ToString();
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

        //==================SelectAll_In_BDERequisitionPaidDetail=================
        public List<GLBDERequisitionFormAVM> SelectAll_In_BDERequisitionPaidDetail(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
SELECT
 fa.Id
,fa.GLBDERequisitionId
,fa.TransactionDateTime
,fa.BranchId
,fa.MRNo
,fa.MRDate
,fa.DocumentNo
,fa.DocumentType
,fa.CustomerId
,fa.NetPremium
,fa.BDE
,fa.AIT
,ISNULL(fa.BDEAmountWithoutAIT,0)BDEAmountWithoutAIT
,ISNULL(fa.AITAmount          ,0)AITAmount

,fa.BDEAmount
,fa.Post
,isnull(fa.IsRejected           ,'0')IsRejected
,isnull(fa.RejectedBy           ,'0')RejectedBy
,isnull(fa.RejectedDate         ,'19000101')RejectedDate
,isnull(fa.RejectedComments     ,'NA')RejectedComments
,isnull(fa.IsPaid				,'0')IsPaid
,isnull(fa.PaidAmount           ,'0')PaidAmount
,fa.PaidTo
,fa.PaymentDate

,fa.Remarks
,c.Name CustomerName
FROM  GLBDERequisitionFormAs  fa
LEFT OUTER JOIN GLCustomers c ON fa.CustomerId = c.Id
WHERE  1=1
AND fa.Id In (
select GLBDERequisitionDetailId from GLBDERequisitionPaidDetails
WHERE  1=1 AND TransactionType='FormA'
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
                    vm = new GLBDERequisitionFormAVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.GLBDERequisitionId = Convert.ToInt32(dr["GLBDERequisitionId"]);
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.MRNo = dr["MRNo"].ToString();
                    vm.MRDate = Ordinary.StringToDate(dr["MRDate"].ToString());
                    vm.DocumentNo = dr["DocumentNo"].ToString();
                    vm.DocumentType = dr["DocumentType"].ToString();

                    vm.CustomerId = Convert.ToInt32(dr["CustomerId"]);
                    vm.NetPremium = Convert.ToDecimal(dr["NetPremium"]);
                    vm.BDE = Convert.ToDecimal(dr["BDE"]);
                    vm.AIT = Convert.ToDecimal(dr["AIT"]);
                    vm.BDEAmountWithoutAIT = Convert.ToDecimal(dr["BDEAmountWithoutAIT"]);
                    vm.AITAmount = Convert.ToDecimal(dr["AITAmount"]);

                    vm.BDEAmount = Convert.ToDecimal(dr["BDEAmount"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.IsRejected = Convert.ToBoolean(dr["IsRejected"]);
                    vm.RejectedBy = dr["RejectedBy"].ToString();
                    vm.RejectedDate = dr["RejectedDate"].ToString();
                    vm.RejectedComments = dr["RejectedComments"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.CustomerName = dr["CustomerName"].ToString();

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
        public List<GLBDERequisitionFormAVM> SelectAll_NotIn_BDERequisitionPaidDetail(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
SELECT
 fa.Id
,fa.GLBDERequisitionId
,fa.TransactionDateTime
,fa.BranchId
,fa.MRNo
,fa.MRDate
,fa.DocumentNo
,fa.DocumentType
,fa.CustomerId
,fa.NetPremium
,fa.BDE
,fa.AIT
,ISNULL(fa.BDEAmountWithoutAIT,0)BDEAmountWithoutAIT
,ISNULL(fa.AITAmount          ,0)AITAmount

,fa.BDEAmount
,fa.Post
,isnull(fa.IsRejected           ,'0')IsRejected
,isnull(fa.RejectedBy           ,'0')RejectedBy
,isnull(fa.RejectedDate         ,'19000101')RejectedDate
,isnull(fa.RejectedComments     ,'NA')RejectedComments
,isnull(fa.IsPaid				,'0')IsPaid
,isnull(fa.PaidAmount           ,'0')PaidAmount
,fa.PaidTo
,fa.PaymentDate

,fa.Remarks
,c.Name CustomerName
FROM  GLBDERequisitionFormAs  fa
LEFT OUTER JOIN GLCustomers c ON fa.CustomerId = c.Id
WHERE  1=1
AND fa.Id Not In (
select GLBDERequisitionDetailId from GLBDERequisitionPaidDetails
WHERE  1=1 AND TransactionType='FormA'
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
                    vm = new GLBDERequisitionFormAVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.GLBDERequisitionId = Convert.ToInt32(dr["GLBDERequisitionId"]);
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.MRNo = dr["MRNo"].ToString();
                    vm.MRDate = Ordinary.StringToDate(dr["MRDate"].ToString());
                    vm.DocumentNo = dr["DocumentNo"].ToString();
                    vm.DocumentType = dr["DocumentType"].ToString();

                    vm.CustomerId = Convert.ToInt32(dr["CustomerId"]);
                    vm.NetPremium = Convert.ToDecimal(dr["NetPremium"]);
                    vm.BDE = Convert.ToDecimal(dr["BDE"]);
                    vm.AIT = Convert.ToDecimal(dr["AIT"]);
                    vm.BDEAmountWithoutAIT = Convert.ToDecimal(dr["BDEAmountWithoutAIT"]);
                    vm.AITAmount = Convert.ToDecimal(dr["AITAmount"]);

                    vm.BDEAmount = Convert.ToDecimal(dr["BDEAmount"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.IsRejected = Convert.ToBoolean(dr["IsRejected"]);
                    vm.RejectedBy = dr["RejectedBy"].ToString();
                    vm.RejectedDate = dr["RejectedDate"].ToString();
                    vm.RejectedComments = dr["RejectedComments"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.CustomerName = dr["CustomerName"].ToString();

                    vm.IsPaid = Convert.ToBoolean(dr["IsPaid"]);
                    vm.PaidAmount = Convert.ToInt32(dr["BDEAmount"]);
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
        public List<GLBDERequisitionFormAVM> SelectAll(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
SELECT
 fa.Id
,fa.GLBDERequisitionId
,fa.TransactionDateTime
,fa.BranchId
,fa.MRNo
,fa.MRDate
,fa.DocumentNo
,fa.DocumentType
,fa.CustomerId
,fa.NetPremium
,fa.BDE
,fa.AIT
,ISNULL(fa.BDEAmountWithoutAIT,0)BDEAmountWithoutAIT
,ISNULL(fa.AITAmount          ,0)AITAmount

,fa.BDEAmount
,fa.Post
,isnull(fa.IsRejected           ,'0')IsRejected
,isnull(fa.RejectedBy           ,'0')RejectedBy
,isnull(fa.RejectedDate         ,'19000101')RejectedDate
,isnull(fa.RejectedComments     ,'NA')RejectedComments
,isnull(fa.IsPaid				,'0')IsPaid
,isnull(fa.PaidAmount           ,'0')PaidAmount
,fa.PaidTo
,fa.PaymentDate

,fa.Remarks
,c.Name CustomerName
FROM  GLBDERequisitionFormAs  fa
LEFT OUTER JOIN GLCustomers c ON fa.CustomerId = c.Id
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
                    vm = new GLBDERequisitionFormAVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.GLBDERequisitionId = Convert.ToInt32(dr["GLBDERequisitionId"]);
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.MRNo = dr["MRNo"].ToString();
                    vm.MRDate = Ordinary.StringToDate(dr["MRDate"].ToString());
                    vm.DocumentNo = dr["DocumentNo"].ToString();
                    vm.DocumentType = dr["DocumentType"].ToString();

                    vm.CustomerId = Convert.ToInt32(dr["CustomerId"]);
                    vm.NetPremium = Convert.ToDecimal(dr["NetPremium"]);
                    vm.BDE = Convert.ToDecimal(dr["BDE"]);
                    vm.AIT = Convert.ToDecimal(dr["AIT"]);
                    vm.BDEAmountWithoutAIT = Convert.ToDecimal(dr["BDEAmountWithoutAIT"]);
                    vm.AITAmount = Convert.ToDecimal(dr["AITAmount"]);

                    vm.BDEAmount = Convert.ToDecimal(dr["BDEAmount"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.IsRejected = Convert.ToBoolean(dr["IsRejected"]);
                    vm.RejectedBy = dr["RejectedBy"].ToString();
                    vm.RejectedDate = dr["RejectedDate"].ToString();
                    vm.RejectedComments = dr["RejectedComments"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.CustomerName = dr["CustomerName"].ToString();
                    
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
        public string[] Insert(GLBDERequisitionFormAVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Insert GLBDERequisitionFormA"; //Method Name
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


                vm.Id = _cDal.NextId(" GLBDERequisitionFormAs", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO  GLBDERequisitionFormAs(
Id
,GLBDERequisitionId
,TransactionDateTime
,BranchId
,MRNo
,MRDate
,DocumentNo
,DocumentType
,CustomerId
,NetPremium
,BDE
,AIT
,BDEAmountWithoutAIT
,AITAmount
,BDEAmount
,Post
,IsRejected
,RejectedBy
,RejectedDate
,RejectedComments
,Remarks

) VALUES (
@Id
,@GLBDERequisitionId
,@TransactionDateTime
,@BranchId
,@MRNo
,@MRDate
,@DocumentNo
,@DocumentType
,@CustomerId
,@NetPremium
,@BDE
,@AIT
,@BDEAmountWithoutAIT
,@AITAmount
,@BDEAmount
,@Post
,@IsRejected
,@RejectedBy
,@RejectedDate
,@RejectedComments
,@Remarks
) 
";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    string[] DocumentNoElements = vm.DocumentNo.Split('/');
                    vm.DocumentType = DocumentNoElements[4];
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@GLBDERequisitionId", vm.GLBDERequisitionId);
                    cmdInsert.Parameters.AddWithValue("@TransactionDateTime", Ordinary.DateToString(vm.TransactionDateTime));
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Parameters.AddWithValue("@MRNo", vm.MRNo);
                    cmdInsert.Parameters.AddWithValue("@MRDate", Ordinary.DateToString(vm.MRDate));
                    cmdInsert.Parameters.AddWithValue("@DocumentNo", vm.DocumentNo);
                    cmdInsert.Parameters.AddWithValue("@DocumentType", vm.DocumentType);

                    cmdInsert.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                    cmdInsert.Parameters.AddWithValue("@NetPremium", vm.NetPremium);
                    cmdInsert.Parameters.AddWithValue("@BDE", vm.BDE);
                    cmdInsert.Parameters.AddWithValue("@AIT", vm.AIT);
                    cmdInsert.Parameters.AddWithValue("@BDEAmountWithoutAIT", vm.BDEAmountWithoutAIT);
                    cmdInsert.Parameters.AddWithValue("@AITAmount", vm.AITAmount);

                    cmdInsert.Parameters.AddWithValue("@BDEAmount", vm.BDEAmount);
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
                        throw new ArgumentNullException("Unexpected error to update  GLBDERequisitionFormAs.", "");
                    }
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This  GLBDERequisitionFormA already used!";
                    throw new ArgumentNullException("Please Input  GLBDERequisitionFormA Value", "");
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
        ////==================AcceptReject =================
        public string[] AcceptReject(GLBDERequisitionFormAVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Delete GLBDERequisitionFormA"; //Method Name
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
                        sqlText = "update  GLBDERequisitionFormAs set";
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
                                string[] cFields = { "fa.Id" };
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
                                if (1 == 1)
                                {
                                    string ITEmail = new GLSettingDAL().settingValue("Email", "ITEmail", currConn, transaction);

                                    string[] EmailForm = Ordinary.GDICEmailForm("Concern", masterVM.Code, status, url, "PCReq");
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
                        throw new ArgumentNullException(" GLBDERequisitionFormA Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException(" GLBDERequisitionFormA Information Delete", "Could not found any item.");
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
        public DataTable Report(GLBDERequisitionFormAVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
,fa.TransactionDateTime
,c.Name CustomerName
,fa.MRNo
,fa.MRDate
,fa.DocumentNo
,fa.NetPremium
,fa.BDE
,fa.AIT
,ISNULL(fa.BDEAmountWithoutAIT,0)BDEAmountWithoutAIT
,ISNULL(fa.AITAmount          ,0)AITAmount
,fa.BDEAmount
,br.Name BranchName
,fa.Remarks
,fa.GLBDERequisitionId
 ,case 
  when fa.IsRejected =1 then 'Decline' 
  when bde.IsRejected =1 then 'Rejected' 
  when bde.IsApprovedL4 =1 then 'Approval Completed' 
  when bde.IsApprovedL3 =1 then 'Waiting for Approval Level4' 
  when bde.IsApprovedL2 =1 then 'Waiting for Approval Level3' 
  when bde.IsApprovedL1 =1 then 'Waiting for Approval Level2' 
 when bde.Post=1 then 'Waiting for Approval Level1' 
 else 'Not Posted' end  Status
FROM  GLBDERequisitionFormAs  fa
LEFT OUTER JOIN GLCustomers c ON fa.CustomerId = c.Id
LEFT OUTER JOIN GLBranchs br ON fa.BranchId = br.Id
LEFT OUTER JOIN GLBDERequisitions bde ON bde.Id = fa.GLBDERequisitionId
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
                    sqlText += @" and ( fa.IsRejected = 1)";
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
                string[] changeColumn = { "TransactionDateTime", "MRDate" };
                dt = Ordinary.DtMultiColumnStringToDate(dt, changeColumn);
                dt.Columns["TransactionDateTime"].SetOrdinal(1);
                dt.Columns["MRDate"].SetOrdinal(4);
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
        public string[] UpdatePayment(List<GLBDERequisitionFormAVM> VMs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "GLBDERequisitionFormAUpdate"; //Method Name
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
                    sqlText = "UPDATE GLBDERequisitionFormAs SET";
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
                            throw new ArgumentNullException("Unexpected error to update GLBDERequisitionFormAs.", "");
                        }
                    }

                    #endregion SqlExecution

                    retResults[2] = VMs.FirstOrDefault().GLBDERequisitionId.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("GLBDERequisitionFormA Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLBDERequisitionFormA Update", "Could not found any item.");
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
