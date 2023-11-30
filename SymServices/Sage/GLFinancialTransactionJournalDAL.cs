using SymOrdinary;
using SymServices.Common;
using SymViewModel.Acc;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
namespace SymServices.Sage
{
    public class GLFinancialTransactionJournalDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================SelectAll =================
        public List<GLFinancialTransactionJournalVM> SelectAll(int Id = 0, int branchId = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLFinancialTransactionJournalVM> VMs = new List<GLFinancialTransactionJournalVM>();
            GLFinancialTransactionJournalVM vm;
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
                    currConn = _dbsqlConnection.GetConnectionAcc();
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
ej.Id                               
,ej.BranchId
,br.Name BranchName
,ej.Code                            
,ej.TransactionDateTime             
,ej.TransactionType
,ISNULL(ej.Post, 0) Post
,ej.Remarks                         
,ej.IsActive                        
,ej.IsArchive                       
,ej.CreatedBy                       
,ej.CreatedAt                       
,ej.CreatedFrom                     
,ej.LastUpdateBy                    
,ej.LastUpdateAt                    
,ej.LastUpdateFrom                  
,ej.PostedBy                        
,ej.PostedAt                        
,ej.PostedFrom

                                  
FROM GLFinancialTransactionJournals  ej
LEFT OUTER JOIN GLBranchs br ON ej.BranchId = br.id
WHERE  1=1  AND ej.IsArchive = 0
";


                if (Id > 0)
                {
                    sqlText += @" and ej.Id=@Id";
                }
                if (branchId > 0)
                {
                    sqlText += @" and ej.BranchId=@branchId";
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
                if (branchId > 0)
                {
                    objComm.Parameters.AddWithValue("@branchId", branchId);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLFinancialTransactionJournalVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.BranchName = dr["BranchName"].ToString();
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.Post = Convert.ToBoolean(dr["Post"]);

                    //TransactionType
                    //IsPS
                    vm.PostedBy = dr["PostedBy"].ToString();
                    vm.PostedAt = Ordinary.StringToDate(dr["PostedAt"].ToString());
                    vm.PostedFrom = dr["PostedFrom"].ToString();

                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
        public string[] Insert(GLFinancialTransactionJournalVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertGLFinancialTransactionJournal"; //Method Name
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
                    currConn = _dbsqlConnection.GetConnectionAcc();
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
                vm.Code = codedal.NextCodeAcc("GLFinancialTransactionJournals", vm.BranchId, Convert.ToDateTime(vm.TransactionDateTime), "VCH", currConn, transaction);
                #endregion Code Generate                
                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO GLFinancialTransactionJournals(
Id
,BranchId
,Code
,TransactionDateTime
,TransactionType
,Post
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Id
,@BranchId
,@Code
,@TransactionDateTime
,@TransactionType
,@Post
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Parameters.AddWithValue("@Code", vm.Code);
                    cmdInsert.Parameters.AddWithValue("@TransactionDateTime", vm.TransactionDateTime);
                    cmdInsert.Parameters.AddWithValue("@TransactionType", vm.TransactionType);
                    cmdInsert.Parameters.AddWithValue("@Post", vm.Post);

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
                        throw new ArgumentNullException("Unexpected error to update GLFinancialTransactionJournals.", "");
                    }
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This GLFinancialTransactionJournal already used!";
                    throw new ArgumentNullException("Please Input GLFinancialTransactionJournal Value", "");
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
        public string[] Update(GLFinancialTransactionJournalVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "AccountFinalUpdate"; //Method Name
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
                    currConn = _dbsqlConnection.GetConnectionAcc();
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
                    sqlText = "UPDATE GLFinancialTransactionJournals SET";
                    sqlText += " TransactionDateTime=@TransactionDateTime";
                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " WHERE Id=@Id";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@TransactionDateTime", vm.TransactionDateTime);

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
                        throw new ArgumentNullException("Unexpected error to update AccountFinals.", "");
                    }
                    #endregion SqlExecution

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("AccountFinal Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("AccountFinal Update", "Could not found any item.");
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

        ////==================CreateJournal =================
        public string[] CreateJournal(GLFinancialTransactionJournalVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "CreateJournal"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;
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
                    currConn = _dbsqlConnection.GetConnectionAcc();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction(""); }
                #endregion open connection and transaction
                #region Check is  it used
                #endregion Check is  it used
                if (ids.Length >= 1)
                {

                    #region Delete Settings
                    GLFinancialTransactionDAL GLFinancialTransactionDAL = new GLFinancialTransactionDAL();

                    GLFinancialTransactionJournalVM ejvm = new GLFinancialTransactionJournalVM();
                    string[] conditionFields = {"tr.MadeJournal"};
                    string[] conditionValues = {"0"};

                    ejvm.BranchId = GLFinancialTransactionDAL.SelectAll(Convert.ToInt32(ids[0]),conditionFields, conditionValues, currConn, transaction).FirstOrDefault().BranchId;


                    ejvm.TransactionDateTime = vm.TransactionDateTime;
                    ejvm.CreatedBy = vm.CreatedBy;
                    ejvm.CreatedAt = vm.CreatedAt;
                    ejvm.CreatedFrom = vm.CreatedFrom;
                    ejvm.Post = false;

                    ejvm.TransactionType = vm.TransactionType;

                    retResults = Insert(ejvm, currConn, transaction);
                    string GLFinancialTransactionJournalId = retResults[2];
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }

                    for (int i = 0; i < ids.Length - 1; i++)
                    {

                        #region Exist Check
                        string[] conditionField = { "GLFinancialTransactionId" };
                        string[] conditionValue = { ids[i] };
                        var exist = _cDal.ExistCheck("GLFinancialTransactionJournalDetails", conditionField, conditionValue, currConn
                            , transaction);
                        if (exist)
                        {
                            retResults[1] = "Expense Entry Already Exist";
                            throw new ArgumentNullException("Expense Entry Already Exist", "Expense Entry Already Exist");
                        }
                        #endregion Exist Check

                        sqlText = "";
                        sqlText += @"
                                    insert into GLFinancialTransactionJournalDetails(BranchId,GLFinancialTransactionJournalId,GLFinancialTransactionId)
                                    select branchid,@GLFinancialTransactionJournalId,Id from GLFinancialTransactionransactions
                                    where Id=@GLFinancialTransactionransactionId
                                    
                                    update GLFinancialTransactionransactions set MadeJournal=1
                                    where id=@GLFinancialTransactionransactionId
                        ";
                        SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@GLFinancialTransactionransactionId", ids[i]);
                        cmdDelete.Parameters.AddWithValue("@GLFinancialTransactionJournalId", retResults[2]);
                        var exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    if (transResult > 0)
                    {
                        ShampanIdentityVM auditVM = new ShampanIdentityVM();
                        auditVM.CreatedBy = vm.CreatedBy;
                        auditVM.CreatedAt = vm.CreatedAt;
                        auditVM.CreatedFrom = vm.CreatedFrom;
                        string[] id = new string[2];
                        id[0] = GLFinancialTransactionJournalId;
                        //retResults = Post(id, auditVM, currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                            throw new ArgumentNullException(retResults[1], "");
                        }

                    }
                    //retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Purchase Journal Create", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Delete Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Purchase Journal Create", "Could not found any item.");
                }
                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                    retResults[0] = "Success";
                    retResults[1] = "Purchase Journal Created Successfully!";
                }
                else
                {
                    retResults[1] = "Unexpected error to Create Purchase Journal!";
                    throw new ArgumentNullException("", "");
                }
            }
            #region catch
            catch (Exception ex)
            {

                retResults[0] = "Fail"; //catch ex
                retResults[4] = ex.Message; //catch ex
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
            return retResults;
        }



        //==================Delete =================
        public string[] Delete(GLFinancialTransactionJournalVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLFinancialTransactionJournal"; //Method Name
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;
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
                    currConn = _dbsqlConnection.GetConnectionAcc();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction(""); }
                #endregion open connection and transaction
                #region Check is  it used
                #endregion Check is  it used
                if (ids.Length >= 1)
                {
                    #region Check Posted or Not Posted
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        string retVal = "";
                        retVal = _cDal.SelectFieldValue("GLFinancialTransactionJournals", "Post", "Id", ids[i].ToString(), currConn, transaction);
                        vm.Post = Convert.ToBoolean(retVal);
                        if (vm.Post == true)
                        {
                            retResults[0] = "Fail";
                            retResults[1] = "Data Alreday Posted! Cannot be Deleted.";
                            throw new ArgumentNullException("Data Alreday Posted! Cannot Deleted.", "");
                        }
                    }
                    #endregion Check Posted or Not Posted
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        #region Update GLFinancialTransactionJournalDetails
                        sqlText = "";
                        sqlText += " ";
                        sqlText += "DELETE GLFinancialTransactionJournalDetails";
                        sqlText += " WHERE GLFinancialTransactionJournalId=@Id";
                        SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        var exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update GLFinancialTransactionJournalDetails.", "");
                        }
                        sqlText = " ";
                        sqlText = "DELETE GLFinancialTransactionJournals";
                        sqlText += " WHERE Id=@Id";
                        cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update GLFinancialTransactionJournalDetails.", "");
                        }
                        #endregion Update GLFinancialTransactionJournalDetails
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("GLFinancialTransactionJournal Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("GLFinancialTransactionJournal Information Delete", "Could not found any item.");
                }
                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                    retResults[0] = "Success";
                    retResults[1] = "Data Delete Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to delete GLFinancialTransactionJournal Information.";
                    throw new ArgumentNullException("", "");
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
            return retResults;
        }
        //public string[] Post(string[] ids, ShampanIdentityVM auditVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        //{
        //    #region Variables
        //    string[] retResults = new string[6];
        //    retResults[0] = "Fail";//Success or Fail
        //    retResults[1] = "Fail";// Success or Fail Message
        //    retResults[2] = "0";// Return Id
        //    retResults[3] = "sqlText"; //  SQL Query
        //    retResults[4] = "ex"; //catch ex
        //    retResults[5] = "Post"; //Method Name
        //    SqlConnection currConn = null;
        //    SqlTransaction transaction = null;
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
        //            currConn = _dbsqlConnection.GetConnectionAcc();
        //            if (currConn.State != ConnectionState.Open)
        //            {
        //                currConn.Open();
        //            }
        //        }
        //        #endregion open connection and transaction
        //        #region Check is  it used
        //        #endregion Check is  it used
        //        if (ids.Length >= 1)
        //        {
        //            #region Check Posted or Not Posted
        //            bool Post = false;
        //            for (int i = 0; i < ids.Length - 1; i++)
        //            {
        //                string retVal = "";
        //                retVal = _cDal.SelectFieldValue("GLFinancialTransactionJournals", "Post", "Id", ids[i].ToString(), currConn, transaction);
        //                Post = Convert.ToBoolean(retVal);
        //                if (Post == true)
        //                {
        //                    retResults[0] = "Fail";
        //                    retResults[1] = "Data Alreday Posted!";
        //                    throw new ArgumentNullException(retResults[1], "");
        //                }
        //            }
        //            #endregion Check Posted or Not Posted
        //            #region Update Settings
        //            for (int i = 0; i < ids.Length - 1; i++)
        //            {
        //                #region CheckPoint

        //                List<GLFinancialTransactionJournalVM> sJournalVMs = new List<GLFinancialTransactionJournalVM>();
        //                GLFinancialTransactionJournalVM sJournalVM = new GLFinancialTransactionJournalVM();
        //                sJournalVM = SelectAll(Convert.ToInt32(ids[i]),0, null, null, currConn, transaction).FirstOrDefault();

        //                sJournalVMs = SelectAllToPost(ids[i], currConn, transaction);
        //                foreach (var item in sJournalVMs)
        //                {
        //                    if (item.AccountId == 0)
        //                    {
        //                        retResults[1] = "Account Head Not Exist - Could not  Post! " + sJournalVM.Code;
        //                        throw new ArgumentNullException(retResults[1], ids[i]);
        //                    }
        //                }

        //                #endregion CheckPoint

        //                #region Post
        //                retResults = _cDal.FieldPost("GLFinancialTransactionJournals", "Id", ids[i], currConn, transaction);
        //                if (retResults[0].ToLower() == "fail")
        //                {
        //                    throw new ArgumentNullException("GLFinancialTransactionJournals Post", ids[i] + " could not Post.");
        //                }
        //                #endregion Post

        //                #region Insert Into JournalEntry and JournalEntryDetail
        //                #region Assing JournalEntryVM
        //                JournalEntryDAL _JEntryDAL = new JournalEntryDAL();
        //                JournalEntryVM JEntryVM = new JournalEntryVM();

        //                JEntryVM.BranchId = sJournalVM.BranchId;
        //                JEntryVM.Code = "";
        //                JEntryVM.TransactionDateTime = sJournalVM.TransactionDateTime;
        //                JEntryVM.TransactionType = sJournalVM.TransactionType;
        //                JEntryVM.Remarks = sJournalVM.TransactionType;
        //                JEntryVM.ReferenceNo1 = "";
        //                JEntryVM.ReferenceNo2 = sJournalVM.Id.ToString();
        //                JEntryVM.ReferenceNo3 = sJournalVM.Code;
        //                JEntryVM.IsPS = sJournalVM.IsPS;
        //                JEntryVM.CreatedBy = auditVM.CreatedBy;
        //                JEntryVM.CreatedAt = auditVM.CreatedAt;
        //                JEntryVM.CreatedFrom = auditVM.CreatedFrom;
        //                #endregion Assing JournalEntryVM
        //                #region Assing JournalEntryDetailVM

        //                List<JournalEntryDetailVM> JEntryDVMs = new List<JournalEntryDetailVM>();
        //                JournalEntryDetailVM JEntryDVM = new JournalEntryDetailVM();
        //                foreach (var item in sJournalVMs)
        //                {
        //                    JEntryDVM = new JournalEntryDetailVM();
        //                    JEntryDVM.AccountId = item.AccountId;
        //                    JEntryDVM.DebitAmount = item.Dr;
        //                    JEntryDVM.CreditAmount = item.Cr;
        //                    JEntryDVMs.Add(JEntryDVM);
        //                }

        //                JEntryVM.journalEntryDetailVMs = JEntryDVMs;
        //                #endregion Assing JournalEntryDetailVM

        //                #region Insert
        //                retResults = _JEntryDAL.Insert(JEntryVM, currConn, transaction);
        //                if (retResults[0] == "Fail")
        //                {
        //                    throw new ArgumentNullException(retResults[1], ids[i]);
        //                }

        //                #endregion Insert


        //                #endregion Insert Into JournalEntry and JournalEntryDetail
        //            }
        //            #endregion Update Settings
        //        }
        //        else
        //        {
        //            throw new ArgumentNullException("GLFinancialTransactionJournal Information Post", "Could not found any item.");
        //        }
        //    }
        //    #region catch
        //    catch (Exception ex)
        //    {
        //        retResults[0] = "Fail";//Success or Fail
        //        retResults[4] = ex.Message; //catch ex
        //        return retResults;
        //    }
        //    finally
        //    {
        //        if (VcurrConn == null)
        //        {
        //            if (currConn != null)
        //            {
        //                if (currConn.State == ConnectionState.Open)
        //                {
        //                    currConn.Close();
        //                }
        //            }
        //        }
        //    }
        //    #endregion
        //    return retResults;
        //}

        public List<GLFinancialTransactionJournalVM> SelectAllToPost(string Id = "" , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLFinancialTransactionJournalVM> VMs = new List<GLFinancialTransactionJournalVM>();
            GLFinancialTransactionJournalVM vm;
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
                    currConn = _dbsqlConnection.GetConnectionAcc();
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
--declare @Id as varchar(20)
--set @Id=5

  select a.HeadId,af.AccountName HeadName,Dr,Cr  from(
select br.ExpenseAccountId HeadId  ,sum(subtotal) Dr,0 Cr from GLFinancialTransactionransactionDetails
left outer join Branchs br on br.Id=GLFinancialTransactionransactionDetails.BranchId
where GLFinancialTransactionransactionDetails.GLFinancialTransactionransactionId in(
select sjd.GLFinancialTransactionId from GLFinancialTransactionJournalDetails sjd
where sjd.GLFinancialTransactionJournalId=@Id)
group by br.ExpenseAccountId
union all
select br.CashAccountId HeadId,0 Dr,sum(subtotal) Cr from GLFinancialTransactionransactionDetails
left outer join Branchs br on br.Id=GLFinancialTransactionransactionDetails.BranchId
where GLFinancialTransactionransactionDetails.GLFinancialTransactionransactionId in(
select sjd.GLFinancialTransactionId from GLFinancialTransactionJournalDetails sjd
where sjd.GLFinancialTransactionJournalId=@Id)
group by br.CashAccountId
) as a
left outer join AccountFinals af on a.HeadId=af.Id
";



                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                objComm.Parameters.AddWithValue("@Id", Id);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLFinancialTransactionJournalVM();
                    vm.HeadId = Convert.ToInt32(dr["HeadId"]);
                    vm.HeadName = dr["HeadName"].ToString();

                    vm.AccountId = Convert.ToInt32(dr["HeadId"]);
                    vm.AccountName = dr["HeadName"].ToString();

                    vm.Dr = Convert.ToDecimal(dr["Dr"]);
                    vm.Cr = Convert.ToDecimal(dr["Cr"]);
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

        public DataTable GLFinancialTransactionJournalReport(string Id = "" )
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
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionAcc();
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

                List<GLFinancialTransactionJournalVM> VMs = new List<GLFinancialTransactionJournalVM>();
                VMs = SelectAllToPost(Id, currConn, transaction);
                dt = Ordinary.ToDataTable(VMs);

                #region sql statement
              

                if (transaction != null)
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
                if (currConn != null && currConn.State == ConnectionState.Open)
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
