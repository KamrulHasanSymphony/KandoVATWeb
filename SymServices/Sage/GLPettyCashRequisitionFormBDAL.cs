using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SymOrdinary;
using SymServices.Common;
using SymViewModel.Sage;
using System.Linq;
using System.Threading;

namespace SymServices.Sage
{
    public class GLPettyCashRequisitionFormBDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        public static Thread thread;
        #endregion
        #region Methods
        //==================SelectAllCommissionBillDetail=================
        public List<GLPettyCashRequisitionFormBVM> SelectAllCommissionBillDetail(string CommissionBillNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLPettyCashRequisitionFormBVM> VMs = new List<GLPettyCashRequisitionFormBVM>();
            GLPettyCashRequisitionFormBVM vm;
            #endregion
            try
            {
                #region SelectAllCommissionBillDetail
                List<CommissionBillDetailVM> cbdVMs = new List<CommissionBillDetailVM>();
                {
                    string[] cFields = { "cbd.CommissionBillNo" };
                    string[] cValues = { CommissionBillNo };
                    cbdVMs = new CommissionBillDAL().SelectAllDetail(cFields, cValues);

                }

                #endregion

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

                #region SqlExecution

                foreach (CommissionBillDetailVM item in cbdVMs)
                {
                    vm = new GLPettyCashRequisitionFormBVM();
                    //////vm.BranchId = Convert.ToInt32(dr["BranchId"]);//Get Id from Code
                    ////////vm.GLPettyCashRequisitionId = Convert.ToInt32(dr["GLPettyCashRequisitionId"]);

                    vm.BranchCode = item.BranchCode;
                    vm.TransactionDateTime = item.ComDate;
                    vm.CommissionBillNo = item.CommissionBillNo;
                    vm.MRNo = item.MRNo;
                    vm.MRDate = item.ComDate;
                    vm.DocumentNo = item.DocumentNo;
                    //////vm.CustomerId = Convert.ToInt32(dr["CustomerId"]);//Get CustomerId from CustomerCode if Not Exist than Create

                    vm.NetPremium = item.NetPremium;
                    vm.CommissionRate = item.RateOfCommission;
                    vm.CommissionAmount = item.CommissionAmount;
                    vm.AITRate = item.TaxRate;
                    vm.AITAmount = item.TaxAmount;

                    vm.PCAmount = item.NetCommission;
                    vm.Post = false;
                    //////////vm.Remarks = dr["Remarks"].ToString();
                    vm.DocumentType = item.Class;
                    vm.CustomerName = item.InsuredName;
                    vm.CustomerCode = item.CustomerID;


                    //vm.PC = Convert.ToDecimal(dr["PC"]);

                    VMs.Add(vm);
                }

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

        //==================SelectByMasterId=================
        public List<GLPettyCashRequisitionFormBVM> SelectByMasterId(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLPettyCashRequisitionFormBVM> VMs = new List<GLPettyCashRequisitionFormBVM>();
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
                string[] conditionField = { "fb.GLPettyCashRequisitionId" };
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
        public List<GLPettyCashRequisitionFormBVM> SelectById(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLPettyCashRequisitionFormBVM> VMs = new List<GLPettyCashRequisitionFormBVM>();
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
         //==================SelectAllMRNo=================
        public List<GLPettyCashRequisitionFormBVM> SelectAllDocNo(string TransactionType,string DocumentNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLPettyCashRequisitionFormBVM> VMs = new List<GLPettyCashRequisitionFormBVM>();
            GLPettyCashRequisitionFormBVM vm;
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
,m.CommissionBillNo
,m.TransactionDateTime
,d.MRNo 
FROM GLPettyCashRequisitionFormBs d
LEFT OUTER JOIN GLPettyCashRequisitions m on m.Id=d.GLPettyCashRequisitionId
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
                    vm = new GLPettyCashRequisitionFormBVM();
                    //////////vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.CommissionBillNo = dr["CommissionBillNo"].ToString();
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
        public List<GLPettyCashRequisitionFormBVM> SelectAllMRNo(string TransactionType, string MRNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLPettyCashRequisitionFormBVM> VMs = new List<GLPettyCashRequisitionFormBVM>();
            GLPettyCashRequisitionFormBVM vm;
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
,m.CommissionBillNo
,m.TransactionDateTime
,d.MRNo 
FROM GLPettyCashRequisitionFormBs d
LEFT OUTER JOIN GLPettyCashRequisitions m on m.Id=d.GLPettyCashRequisitionId
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
                    vm = new GLPettyCashRequisitionFormBVM();
                    //////////vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.CommissionBillNo = dr["CommissionBillNo"].ToString();
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


        //==================SelectAll=================
        public List<GLPettyCashRequisitionFormBVM> SelectAll(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLPettyCashRequisitionFormBVM> VMs = new List<GLPettyCashRequisitionFormBVM>();
            GLPettyCashRequisitionFormBVM vm;
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
,fb.GLPettyCashRequisitionId
,fb.TransactionDateTime
,fb.CommissionBillNo
,fb.BranchId
,fb.MRNo
,fb.MRDate
,fb.DocumentNo
,fb.CustomerId
,fb.NetPremium
,ISNULL(CommissionRate,0)CommissionRate
,ISNULL(CommissionAmount,0)CommissionAmount
,ISNULL(AITRate,0)AITRate
,ISNULL(AITAmount,0)AITAmount




,fb.PCAmount
,fb.Post
,fb.Remarks
,fb.IsRejected
,fb.RejectedBy
,fb.RejectedDate
,fb.RejectedComments
,fb.DocumentType

,c.Name CustomerName
,c.Name CustomerCode
--,fb.PC
FROM  GLPettyCashRequisitionFormBs  fb
LEFT OUTER JOIN GLCustomers c ON fb.CustomerId = c.Id
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
                    vm = new GLPettyCashRequisitionFormBVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.GLPettyCashRequisitionId = Convert.ToInt32(dr["GLPettyCashRequisitionId"]);
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.CommissionBillNo = dr["CommissionBillNo"].ToString();
                    vm.MRNo = dr["MRNo"].ToString();
                    vm.MRDate = Ordinary.StringToDate(dr["MRDate"].ToString());
                    vm.DocumentNo = dr["DocumentNo"].ToString();
                    vm.CustomerId = Convert.ToInt32(dr["CustomerId"]);

                    vm.NetPremium = Convert.ToDecimal(dr["NetPremium"]);
                    vm.CommissionRate = Convert.ToDecimal(dr["CommissionRate"]);
                    vm.CommissionAmount = Convert.ToDecimal(dr["CommissionAmount"]);
                    vm.AITRate = Convert.ToDecimal(dr["AITRate"]);
                    vm.AITAmount = Convert.ToDecimal(dr["AITAmount"]);


                    vm.PCAmount = Convert.ToDecimal(dr["PCAmount"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsRejected = Convert.ToBoolean(dr["IsRejected"]);
                    vm.RejectedBy = dr["RejectedBy"].ToString();
                    vm.RejectedDate = dr["RejectedDate"].ToString();
                    vm.RejectedComments = dr["RejectedComments"].ToString();
                    vm.DocumentType = dr["DocumentType"].ToString();

                    vm.CustomerName = dr["CustomerName"].ToString();
                    vm.CustomerCode = dr["CustomerCode"].ToString();
                    //vm.PC = Convert.ToDecimal(dr["PC"]);

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
        public string[] Insert(List<GLPettyCashRequisitionFormBVM> VMs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Insert GLPettyCashRequisitionFormB"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int NextId = 0;
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


                NextId = _cDal.NextId("GLPettyCashRequisitionFormBs", currConn, transaction);


                if (VMs != null && VMs.Count > 0)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO  GLPettyCashRequisitionFormBs(
Id
,GLPettyCashRequisitionId
,TransactionDateTime
,CommissionBillNo
,BranchId
,MRNo
,MRDate
,DocumentNo
,DocumentType
,CustomerId
,NetPremium
,CommissionRate
,CommissionAmount
,AITRate
,AITAmount

--,PC
,PCAmount
,Post
,Remarks
,IsRejected
,RejectedBy
,RejectedDate
,RejectedComments


) VALUES (
@Id
,@GLPettyCashRequisitionId
,@TransactionDateTime
,@CommissionBillNo
,@BranchId
,@MRNo
,@MRDate
,@DocumentNo
,@DocumentType
,@CustomerId
,@NetPremium
,@CommissionRate
,@CommissionAmount
,@AITRate
,@AITAmount

--,@PC
,@PCAmount
,@Post
,@Remarks
,@IsRejected
,@RejectedBy
,@RejectedDate
,@RejectedComments
) 
";

                    #endregion SqlText
                    foreach (GLPettyCashRequisitionFormBVM vm in VMs)
                    {
                        #region Checkpoint

                        ////////string[] DocumentNoElements = vm.DocumentNo.Split('/');
                        ////////vm.DocumentType = DocumentNoElements[3];

                        ////////string BranchCode = new GLBranchDAL().SelectAll(vm.BranchId).FirstOrDefault().Code;
                        ////////if (!vm.DocumentNo.Contains(BranchCode))
                        ////////{
                        ////////    retResults[1] = "Document No. of other Branch Can't be accepted! Document No: " + vm.DocumentNo;
                        ////////    throw new ArgumentNullException(retResults[1], "");
                        ////////    //retResults[1] = "Document No: " + item.Name + " already exist!";
                        ////////    //return retResults;
                        ////////}

                        #region Customer Check
                        //////if Customer Not Exist, Create Customer
                        //get CustomerId from CustomerCode
                        {
                            if (vm.CustomerId == 0)
                            {
                                string[] cFields = { "Code" };
                                string[] cValues = { vm.CustomerCode };
                                GLCustomerVM customerVM = new GLCustomerVM();
                                customerVM = new GLCustomerDAL().SelectAll(0, cFields, cValues, currConn, transaction).FirstOrDefault();
                                if (customerVM == null)
                                {
                                    customerVM = new GLCustomerVM();
                                    customerVM.Code = vm.CustomerCode;
                                    customerVM.Name = vm.CustomerName;

                                    customerVM.CreatedBy = vm.CreatedBy;
                                    customerVM.CreatedAt = vm.CreatedAt;
                                    customerVM.CreatedFrom = vm.CreatedFrom;
                                    retResults = new GLCustomerDAL().Insert(customerVM, currConn, transaction);

                                    if (retResults[0] == "Fail")
                                    {
                                        throw new ArgumentNullException(retResults[1], "");
                                    }

                                    vm.CustomerId = Convert.ToInt32(retResults[2]);
                                }
                                else
                                {
                                    vm.CustomerId = customerVM.Id;
                                }
                            }
                        }




                        #endregion
                        #endregion
                        #region SqlExecution
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                        cmdInsert.Parameters.AddWithValue("@Id", NextId);
                        cmdInsert.Parameters.AddWithValue("@GLPettyCashRequisitionId", vm.GLPettyCashRequisitionId);
                        cmdInsert.Parameters.AddWithValue("@TransactionDateTime", Ordinary.DateToString(vm.TransactionDateTime));
                        cmdInsert.Parameters.AddWithValue("@CommissionBillNo", vm.CommissionBillNo ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);


                        cmdInsert.Parameters.AddWithValue("@MRNo", vm.MRNo ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@MRDate", Ordinary.DateToString(vm.MRDate));
                        cmdInsert.Parameters.AddWithValue("@DocumentNo", vm.DocumentNo ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@DocumentType", vm.DocumentType ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                        cmdInsert.Parameters.AddWithValue("@NetPremium", vm.NetPremium);

                        cmdInsert.Parameters.AddWithValue("@CommissionRate", vm.CommissionRate);
                        cmdInsert.Parameters.AddWithValue("@CommissionAmount", vm.CommissionAmount);
                        cmdInsert.Parameters.AddWithValue("@AITRate", vm.AITRate);
                        cmdInsert.Parameters.AddWithValue("@AITAmount", vm.AITAmount);


                        //cmdInsert.Parameters.AddWithValue("@PC", vm.PC);
                        cmdInsert.Parameters.AddWithValue("@PCAmount", vm.PCAmount);

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
                            throw new ArgumentNullException("Unexpected error to update  GLPettyCashRequisitionFormBs.", "");
                        }
                        #endregion SqlExecution
                        NextId++;
                    }
                }
                else
                {
                    retResults[1] = "This  GLPettyCashRequisitionFormB already used!";
                    throw new ArgumentNullException("Please Input  GLPettyCashRequisitionFormB Value", "");
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
                retResults[2] = VMs.FirstOrDefault().GLPettyCashRequisitionId.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                retResults[2] = VMs.FirstOrDefault().GLPettyCashRequisitionId.ToString();

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                }

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
        public string[] AcceptReject(GLPettyCashRequisitionFormBVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Delete GLPettyCashRequisitionFormB"; //Method Name
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
                        sqlText = "update  GLPettyCashRequisitionFormBs set";
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
                        GLPettyCashRequisitionVM masterVM = new GLPettyCashRequisitionVM();
                        #endregion Variables
                        #region Send Email
                        string SendEmail = new GLSettingDAL().settingValue("Email", "SendEmail", currConn, transaction);
                        if (SendEmail == "Y")
                        {
                            string urlPrefix = "";
                            urlPrefix = new GLSettingDAL().settingValue("Email", "PettyCashURLPrefix", currConn, transaction);
                            string url = urlPrefix + "/Sage/PettyCashRequisition/SelfApproveIndex";
                            {
                                string[] cFields = { "fb.Id" };
                                string[] cValues = { ids[i] };
                                masterVM = new GLPettyCashRequisitionDAL().SelectAllAudit(0, cFields, cValues, currConn, transaction).FirstOrDefault();
                                string status = "Rejected";
                                string[] cFieldsUser = { "u.LogId" };
                                string[] cValuesUser = { masterVM.CreatedBy };
                                userVMs = new GLUserDAL().SelectAll(0, cFieldsUser, cValuesUser, currConn, transaction);
                                url = urlPrefix + "/Sage/PettyCashRequisition/Posted/" + masterVM.Id;

                                foreach (var item in userVMs)
                                {
                                    string[] EmailForm = Ordinary.GDICEmailForm(item.FullName, masterVM.Code, status, url, "PCReq");
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
                        throw new ArgumentNullException(" GLPettyCashRequisitionFormB Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings

                }
                else
                {
                    throw new ArgumentNullException(" GLPettyCashRequisitionFormB Information Delete", "Could not found any item.");
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
        public DataTable Report(GLPettyCashRequisitionFormBVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
pcr.Code
,fb.TransactionDateTime
,c.Name CustomerName
,fb.CommissionBillNo
,fb.MRNo
,fb.MRDate
,fb.DocumentNo
,fb.NetPremium
,fb.CommissionRate
,fb.CommissionAmount
,fb.AITRate
,fb.AITAmount

,fb.PC
,fb.PCAmount
,br.Name BranchName
,fb.Remarks
,fb.GLPettyCashRequisitionId
 ,case 
  when fb.IsRejected =1 then 'Decline' 
  when pcr.IsRejected =1 then 'Rejected' 
  when pcr.IsApprovedL4 =1 then 'Approval Completed' 
  when pcr.IsApprovedL3 =1 then 'Waiting for Approval Level4' 
  when pcr.IsApprovedL2 =1 then 'Waiting for Approval Level3' 
  when pcr.IsApprovedL1 =1 then 'Waiting for Approval Level2' 
 when pcr.Post=1 then 'Waiting for Approval Level 1' 
 else 'Not Posted' end  Status

FROM  GLPettyCashRequisitionFormBs  fb
LEFT OUTER JOIN GLCustomers c ON fb.CustomerId = c.Id
LEFT OUTER JOIN GLBranchs br ON fb.BranchId = br.Id
LEFT OUTER JOIN GLPettyCashRequisitions pcr ON pcr.Id = fb.GLPettyCashRequisitionId
WHERE  1=1
";

                if (vm.Status == "Created")
                {
                    sqlText += @" and ( pcr.IsRejected <> 1 and  pcr.IsApprovedL4 <> 1 and pcr.Post<>1)";
                }
                else if (vm.Status == "Posted")
                {
                    sqlText += @" and ( pcr.IsRejected <> 1 and  pcr.IsApprovedL4 <> 1 and pcr.Post=1)";
                }
                else if (vm.Status == "Rejected")
                {
                    sqlText += @" and ( pcr.IsRejected = 1 and  pcr.IsApprovedL4 <> 1 and pcr.Post=1)";
                }
                else if (vm.Status == "Approval Completed")
                {
                    sqlText += @" and ( pcr.IsRejected <> 1 and  pcr.IsApprovedL4 = 1 and pcr.Post=1)";
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
                sqlText += @" ORDER BY pcr.TransactionDateTime, pcr.Code";


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
                string[] columnChange = { "TransactionDateTime", "MRDate" };
                dt = Ordinary.DtMultiColumnStringToDate(dt, columnChange);
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

        #endregion
    }
}
