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
    public class GLPettyCashRequisitionFormADAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        public static Thread thread;

        #endregion
        #region Methods
        //==================SelectByMasterId=================
        public List<GLPettyCashRequisitionFormAVM> SelectByMasterId(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLPettyCashRequisitionFormAVM> VMs = new List<GLPettyCashRequisitionFormAVM>();
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
                string[] conditionField = { "fa.GLPettyCashRequisitionId" };
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
        public List<GLPettyCashRequisitionFormAVM> SelectById(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLPettyCashRequisitionFormAVM> VMs = new List<GLPettyCashRequisitionFormAVM>();
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

        //==================SelectAll=================
        public List<GLPettyCashRequisitionFormAVM> SelectAll(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLPettyCashRequisitionFormAVM> VMs = new List<GLPettyCashRequisitionFormAVM>();
            GLPettyCashRequisitionFormAVM vm;
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
,fa.GLPettyCashRequisitionId
,fa.BranchId
,fa.TransactionDateTime
,fa.AccountId
,fa.Amount
,fa.BusinessNature
,fa.Post
,isnull(fa.IsRejected           ,'0')IsRejected
,isnull(fa.RejectedBy           ,'0')RejectedBy
,isnull(fa.RejectedDate         ,'19000101')RejectedDate
,isnull(fa.RejectedComments     ,'NA')RejectedComments
,fa.Remarks
,acc.Name AccountName
,acc.Code AccountCode
FROM  GLPettyCashRequisitionFormAs  fa
LEFT OUTER JOIN GLAccounts acc ON fa.AccountId = acc.Id
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
                    vm = new GLPettyCashRequisitionFormAVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.GLPettyCashRequisitionId = Convert.ToInt32(dr["GLPettyCashRequisitionId"]);
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.AccountId = Convert.ToInt32(dr["AccountId"]);
                    vm.Amount = Convert.ToDecimal(dr["Amount"]);
                    vm.BusinessNature = dr["BusinessNature"].ToString();
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.IsRejected = Convert.ToBoolean(dr["IsRejected"]);
                    vm.RejectedBy = dr["RejectedBy"].ToString();
                    vm.RejectedDate = dr["RejectedDate"].ToString();
                    vm.RejectedComments = dr["RejectedComments"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.AccountName = dr["AccountName"].ToString();
                    vm.AccountCode = dr["AccountCode"].ToString();
                    vm.AccountName = vm.AccountCode + " ( " + vm.AccountName + " )";

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
        public string[] Insert(GLPettyCashRequisitionFormAVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Insert GLPettyCashRequisitionFormA"; //Method Name
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


                vm.Id = _cDal.NextId(" GLPettyCashRequisitionFormAs", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO  GLPettyCashRequisitionFormAs(
Id
,GLPettyCashRequisitionId
,TransactionDateTime
,BranchId
,AccountId
,Amount
,BusinessNature
,Post
,IsRejected
,Remarks

) VALUES (
@Id
,@GLPettyCashRequisitionId
,@TransactionDateTime
,@BranchId
,@AccountId
,@Amount
,@BusinessNature
,@Post
,@IsRejected
,@Remarks
) 
";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@GLPettyCashRequisitionId", vm.GLPettyCashRequisitionId);
                    cmdInsert.Parameters.AddWithValue("@TransactionDateTime", Ordinary.DateToString(vm.TransactionDateTime));
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Parameters.AddWithValue("@AccountId", vm.AccountId);
                    cmdInsert.Parameters.AddWithValue("@Amount", vm.Amount);
                    cmdInsert.Parameters.AddWithValue("@BusinessNature", vm.BusinessNature);

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
                        throw new ArgumentNullException("Unexpected error to update  GLPettyCashRequisitionFormAs.", "");
                    }
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This  GLPettyCashRequisitionFormA already used!";
                    throw new ArgumentNullException("Please Input  GLPettyCashRequisitionFormA Value", "");
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

        //==================Insert List =================
        public string[] Insert(List<GLPettyCashRequisitionFormAVM> VMs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Insert GLPettyCashRequisitionFormA"; //Method Name
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


                NextId = _cDal.NextId("GLPettyCashRequisitionFormAs", currConn, transaction);
                if (VMs != null && VMs.Count > 0)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO  GLPettyCashRequisitionFormAs(
Id
,GLPettyCashRequisitionId
,TransactionDateTime
,BranchId
,AccountId
,Amount
,BusinessNature
,Post
,IsRejected
,Remarks

) VALUES (
@Id
,@GLPettyCashRequisitionId
,@TransactionDateTime
,@BranchId
,@AccountId
,@Amount
,@BusinessNature
,@Post
,@IsRejected
,@Remarks
) 
";

                    #endregion SqlText
                    #region SqlExecution
                    foreach (GLPettyCashRequisitionFormAVM vm in VMs)
                    {
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@Id", NextId);
                        cmdInsert.Parameters.AddWithValue("@GLPettyCashRequisitionId", vm.GLPettyCashRequisitionId);
                        cmdInsert.Parameters.AddWithValue("@TransactionDateTime", Ordinary.DateToString(vm.TransactionDateTime));
                        cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                        cmdInsert.Parameters.AddWithValue("@AccountId", vm.AccountId);
                        cmdInsert.Parameters.AddWithValue("@Amount", vm.Amount);
                        cmdInsert.Parameters.AddWithValue("@BusinessNature", vm.BusinessNature);

                        cmdInsert.Parameters.AddWithValue("@Post", false);
                        cmdInsert.Parameters.AddWithValue("@IsRejected", false);
                        cmdInsert.Parameters.AddWithValue("@RejectedBy", vm.RejectedBy ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@RejectedDate", vm.RejectedDate ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@RejectedComments", vm.RejectedComments ?? Convert.DBNull);

                        cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);

                        var exeRes = cmdInsert.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);

                        NextId++;
                    }

                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update  GLPettyCashRequisitionFormAs.", "");
                    }
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This  GLPettyCashRequisitionFormA already used!";
                    throw new ArgumentNullException("Please Input  GLPettyCashRequisitionFormA Value", "");
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
        public string[] AcceptReject(GLPettyCashRequisitionFormAVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Delete GLPettyCashRequisitionFormA"; //Method Name
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
                        sqlText = "update  GLPettyCashRequisitionFormAs set";
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
                                string[] cFields = { "fa.Id" };
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
                                if (1 == 0)
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
                        throw new ArgumentNullException(" GLPettyCashRequisitionFormA Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException(" GLPettyCashRequisitionFormA Information Delete", "Could not found any item.");
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
        public DataTable Report(GLPettyCashRequisitionFormAVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
,fa.TransactionDateTime
,acc.Name AccountName
,fa.Amount
,acc.BusinessNature BusinessNature
,br.Name BranchName
,fa.Remarks
,fa.GLPettyCashRequisitionId
 ,case 
  when fa.IsRejected =1 then 'Decline' 
  when pcr.IsRejected =1 then 'Rejected' 
  when pcr.IsApprovedL4 =1 then 'Approval Completed' 
  when pcr.IsApprovedL3 =1 then 'Waiting for Approval Level4' 
  when pcr.IsApprovedL2 =1 then 'Waiting for Approval Level3' 
  when pcr.IsApprovedL1 =1 then 'Waiting for Approval Level2' 
 when pcr.Post=1 then 'Waiting for Approval Level 1' 
 else 'Not Posted' end  Status

FROM  GLPettyCashRequisitionFormAs  fa
LEFT OUTER JOIN GLAccounts acc ON fa.AccountId = acc.Id
LEFT OUTER JOIN GLBranchs br ON fa.BranchId = br.Id
LEFT OUTER JOIN GLPettyCashRequisitions pcr ON pcr.Id = fa.GLPettyCashRequisitionId
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
                dt = Ordinary.DtColumnStringToDate(dt, "TransactionDateTime");
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

        #endregion
    }
}
