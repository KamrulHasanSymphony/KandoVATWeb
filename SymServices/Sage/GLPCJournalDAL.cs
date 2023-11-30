using SymOrdinary;
using SymServices.Common;
using SymServices.Sage;
using SymViewModel.Sage;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
namespace SymServices.Sage
{
    public class GLPCJournalDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================SelectAll =================
        public List<GLPCJournalVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLPCJournalVM> VMs = new List<GLPCJournalVM>();
            GLPCJournalVM vm;
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
pcj.Id                               
,pcj.BranchId
,br.Name BranchName
,pcj.Code                            
,pcj.TransactionDateTime             
,pcj.TransactionType
,ISNULL(pcj.Post, 0) Post
,pcj.Remarks                         
,pcj.IsActive                        
,pcj.IsArchive                       
,pcj.CreatedBy                       
,pcj.CreatedAt                       
,pcj.CreatedFrom                     
,pcj.LastUpdateBy                    
,pcj.LastUpdateAt                    
,pcj.LastUpdateFrom                  
,pcj.PostedBy                        
,pcj.PostedAt                        
,pcj.PostedFrom

                                  
FROM GLPCJournals  pcj
LEFT OUTER JOIN GLBranchs br ON pcj.BranchId = br.id
WHERE  1=1  AND pcj.IsArchive = 0
";


                if (Id > 0)
                {
                    sqlText += @" and pcj.Id=@Id";
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
                    vm = new GLPCJournalVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.BranchName = dr["BranchName"].ToString();
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.Post = Convert.ToBoolean(dr["Post"]);

                    //TransactionType
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
        public string[] Insert(GLPCJournalVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertGLPCJournal"; //Method Name
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
                vm.Id = _cDal.NextId("GLPCJournals", currConn, transaction);
                DateTime transDate = Convert.ToDateTime(Ordinary.StringToDate(vm.TransactionDateTime));
                string codeTType = "ACJ";
                if (vm.TransactionType.ToLower()=="other")
                {
                    codeTType = "PCJ";
                }
                vm.Code = codedal.NextCodeAcc("GLPCJournals", vm.BranchId, transDate, codeTType, currConn, transaction);

                #endregion Code Generate
                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO GLPCJournals(
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
                        throw new ArgumentNullException("Unexpected error to update GLPCJournals.", "");
                    }
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This GLPCJournal already used!";
                    throw new ArgumentNullException("Please Input GLPCJournal Value", "");
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
        

        ////==================CreateJournal =================
        public string[] CreateJournal(GLPCJournalVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                    currConn = _dbsqlConnection.GetConnectionSageGL();
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
                    #region Create Journal
                    GLFinancialTransactionDAL glFTDAL = new GLFinancialTransactionDAL();

                    GLPCJournalVM ejvm = new GLPCJournalVM();
                    string[] conditionFields = {"ft.MadeJournal"};
                    string[] conditionValues = {"0"};

                    ejvm.BranchId = glFTDAL.SelectAll(Convert.ToInt32(ids[0]),conditionFields, conditionValues, currConn, transaction).FirstOrDefault().BranchId;


                    ejvm.TransactionDateTime = vm.TransactionDateTime;
                    ejvm.CreatedBy = vm.CreatedBy;
                    ejvm.CreatedAt = vm.CreatedAt;
                    ejvm.CreatedFrom = vm.CreatedFrom;
                    ejvm.Post = false;

                    ejvm.TransactionType = vm.TransactionType;

                    retResults = Insert(ejvm, currConn, transaction);
                    string GLPCJournalId = retResults[2];
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }

                    for (int i = 0; i < ids.Length - 1; i++)
                    {

                        #region Exist Check
                        string[] conditionField = { "GLFinancialTransactionId" };
                        string[] conditionValue = { ids[i] };
                        var exist = _cDal.ExistCheck("GLPCJournalDetails", conditionField, conditionValue, currConn
                            , transaction);
                        if (exist)
                        {
                            retResults[1] = "Petty Cash Already Exist";
                            throw new ArgumentNullException("Petty Cash Already Exist", "Petty Cash Already Exist");
                        }
                        #endregion Exist Check

                        sqlText = "";
                        sqlText += @"
                                    insert into GLPCJournalDetails(BranchId,GLPCJournalId,GLFinancialTransactionId)
                                    select branchid,@GLPCJournalId,Id from GLFinancialTransactions
                                    where Id=@GLFinancialTransactionId
                                    
                                    update GLFinancialTransactions set MadeJournal=1
                                    where id=@GLFinancialTransactionId
                        ";
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@GLFinancialTransactionId", ids[i]);
                        cmdInsert.Parameters.AddWithValue("@GLPCJournalId", retResults[2]);
                        var exeRes = cmdInsert.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    //if (transResult > 0)
                    //{
                    //    ShampanIdentityVM auditVM = new ShampanIdentityVM();
                    //    auditVM.CreatedBy = vm.CreatedBy;
                    //    auditVM.CreatedAt = vm.CreatedAt;
                    //    auditVM.CreatedFrom = vm.CreatedFrom;
                    //    string[] id = new string[2];
                    //    id[0] = GLPCJournalId;
                    //    retResults = Post(id, auditVM, currConn, transaction);
                    //    if (retResults[0] == "Fail")
                    //    {
                    //        throw new ArgumentNullException(retResults[1], "");
                    //    }

                    //}
                    //retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Petty Cash Journal Create", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Delete Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Petty Cash Journal Create", "Could not found any item.");
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
                    retResults[1] = "Petty Cash Journal Created Successfully!";
                }
                else
                {
                    retResults[1] = "Unexpected error to Create Petty Cash Journal!";
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
        public string[] Delete(GLPCJournalVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLPCJournal"; //Method Name
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
                    currConn = _dbsqlConnection.GetConnectionSageGL();
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
                        retVal = _cDal.SelectFieldValue("GLPCJournals", "Post", "Id", ids[i].ToString(), currConn, transaction);
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
                        #region Update GLPCJournalDetails
                        sqlText = "";
                        sqlText += " ";
                        sqlText += "DELETE GLPCJournalDetails";
                        sqlText += " WHERE GLPCJournalId=@Id";
                        SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        var exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update GLPCJournalDetails.", "");
                        }
                        sqlText = " ";
                        sqlText = "DELETE GLPCJournals";
                        sqlText += " WHERE Id=@Id";
                        cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update GLPCJournalDetails.", "");
                        }
                        #endregion Update GLPCJournalDetails
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("GLPCJournal Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("GLPCJournal Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete GLPCJournal Information.";
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
        public string[] Post(string[] ids, ShampanIdentityVM auditVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Post"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            //try
            //{
            //    #region open connection and transaction
            //    #region New open connection and transaction
            //    if (VcurrConn != null)
            //    {
            //        currConn = VcurrConn;
            //    }
            //    if (Vtransaction != null)
            //    {
            //        transaction = Vtransaction;
            //    }
            //    #endregion New open connection and transaction
            //    if (currConn == null)
            //    {
            //        currConn = _dbsqlConnection.GetConnectionSageGL();
            //        if (currConn.State != ConnectionState.Open)
            //        {
            //            currConn.Open();
            //        }
            //    }
            //    #endregion open connection and transaction
            //    #region Check is  it used
            //    #endregion Check is  it used
            //    if (ids.Length >= 1)
            //    {
            //        #region Check Posted or Not Posted
            //        bool Post = false;
            //        for (int i = 0; i < ids.Length - 1; i++)
            //        {
            //            string retVal = "";
            //            retVal = _cDal.SelectFieldValue("GLPCJournals", "Post", "Id", ids[i].ToString(), currConn, transaction);
            //            Post = Convert.ToBoolean(retVal);
            //            if (Post == true)
            //            {
            //                retResults[0] = "Fail";
            //                retResults[1] = "Data Alreday Posted!";
            //                throw new ArgumentNullException(retResults[1], "");
            //            }
            //        }
            //        #endregion Check Posted or Not Posted
            //        #region Update Settings
            //        for (int i = 0; i < ids.Length - 1; i++)
            //        {
            //            #region CheckPoint

            //            List<GLPCJournalVM> sJournalVMs = new List<GLPCJournalVM>();
            //            GLPCJournalVM sJournalVM = new GLPCJournalVM();
            //            sJournalVM = SelectAll(Convert.ToInt32(ids[i]),0, null, null, currConn, transaction).FirstOrDefault();

            //            sJournalVMs = SelectAllToPost(ids[i], currConn, transaction);
            //            foreach (var item in sJournalVMs)
            //            {
            //                if (item.AccountId == 0)
            //                {
            //                    retResults[1] = "Account Head Not Exist - Could not  Post! " + sJournalVM.Code;
            //                    throw new ArgumentNullException(retResults[1], ids[i]);
            //                }
            //            }

            //            #endregion CheckPoint

            //            #region Post
            //            retResults = _cDal.FieldPost("GLPCJournals", "Id", ids[i], currConn, transaction);
            //            if (retResults[0].ToLower() == "fail")
            //            {
            //                throw new ArgumentNullException("GLPCJournals Post", ids[i] + " could not Post.");
            //            }
            //            #endregion Post

            //            #region Insert Into JournalEntry and JournalEntryDetail
            //            #region Assing JournalEntryVM
            //            //JournalEntryDAL _JEntryDAL = new JournalEntryDAL();
            //            //JournalEntryVM JEntryVM = new JournalEntryVM();

            //            //JEntryVM.BranchId = sJournalVM.BranchId;
            //            //JEntryVM.Code = "";
            //            //JEntryVM.TransactionDateTime = sJournalVM.TransactionDateTime;
            //            //JEntryVM.TransactionType = sJournalVM.TransactionType;
            //            //JEntryVM.Remarks = sJournalVM.TransactionType;
            //            //JEntryVM.ReferenceNo1 = "";
            //            //JEntryVM.ReferenceNo2 = sJournalVM.Id.ToString();
            //            //JEntryVM.ReferenceNo3 = sJournalVM.Code;
            //            //JEntryVM.CreatedBy = auditVM.CreatedBy;
            //            //JEntryVM.CreatedAt = auditVM.CreatedAt;
            //            //JEntryVM.CreatedFrom = auditVM.CreatedFrom;
            //            #endregion Assing JournalEntryVM
            //            #region Assing JournalEntryDetailVM

            //            //List<JournalEntryDetailVM> JEntryDVMs = new List<JournalEntryDetailVM>();
            //            //JournalEntryDetailVM JEntryDVM = new JournalEntryDetailVM();
            //            //foreach (var item in sJournalVMs)
            //            //{
            //            //    JEntryDVM = new JournalEntryDetailVM();
            //            //    JEntryDVM.AccountId = item.AccountId;
            //            //    JEntryDVM.DebitAmount = item.Dr;
            //            //    JEntryDVM.CreditAmount = item.Cr;
            //            //    JEntryDVMs.Add(JEntryDVM);
            //            //}

            //            //JEntryVM.journalEntryDetailVMs = JEntryDVMs;
            //            #endregion Assing JournalEntryDetailVM

            //            #region Insert
            //            retResults = _JEntryDAL.Insert(JEntryVM, currConn, transaction);
            //            if (retResults[0] == "Fail")
            //            {
            //                throw new ArgumentNullException(retResults[1], ids[i]);
            //            }

            //            #endregion Insert


            //            #endregion Insert Into JournalEntry and JournalEntryDetail
            //        }
            //        #endregion Update Settings
            //    }
            //    else
            //    {
            //        throw new ArgumentNullException("GLPCJournal Information Post", "Could not found any item.");
            //    }
            //}
            //#region catch
            //catch (Exception ex)
            //{
            //    retResults[0] = "Fail";//Success or Fail
            //    retResults[4] = ex.Message; //catch ex
            //    return retResults;
            //}
            //finally
            //{
            //    if (VcurrConn == null)
            //    {
            //        if (currConn != null)
            //        {
            //            if (currConn.State == ConnectionState.Open)
            //            {
            //                currConn.Close();
            //            }
            //        }
            //    }
            //}
            //#endregion
            return retResults;
        }

        public List<GLPCJournalVM> SelectAllToPost(string Id = "" , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLPCJournalVM> VMs = new List<GLPCJournalVM>();
            GLPCJournalVM vm;
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
--declare @Id as varchar(20)
--set @Id=5

  select a.HeadId,af.AccountName HeadName,Dr,Cr  from(
select br.ExpenseAccountId HeadId  ,sum(subtotal) Dr,0 Cr from GLFinancialTransactionDetails
left outer join Branchs br on br.Id=GLFinancialTransactionDetails.BranchId
where GLFinancialTransactionDetails.GLFinancialTransactionId in(
select sjd.GLFinancialTransactionId from GLPCJournalDetails sjd
where sjd.GLPCJournalId=@Id)
group by br.ExpenseAccountId
union all
select br.CashAccountId HeadId,0 Dr,sum(subtotal) Cr from GLFinancialTransactionDetails
left outer join Branchs br on br.Id=GLFinancialTransactionDetails.BranchId
where GLFinancialTransactionDetails.GLFinancialTransactionId in(
select sjd.GLFinancialTransactionId from GLPCJournalDetails sjd
where sjd.GLPCJournalId=@Id)
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
                    vm = new GLPCJournalVM();
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

        public DataTable GLPCJournalReport(string Id = "" )
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

                List<GLPCJournalVM> VMs = new List<GLPCJournalVM>();
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


         ////==================Report=================
        public DataTable VoucherReport(GLPCJournalDetailVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
--declare @GLPCJournalId as varchar(10)
--set @GLPCJournalId = 1

select acc.Name AccountName, b.Name BranchName,pcj.Code,pcj.TransactionDateTime, a.* 
,case when a.isDebit=1 then a.TransactionAmount else 0 end Dr
,case when a.isDebit=0 then a.TransactionAmount else 0 end Cr
 from(
select distinct branchid,AccountId,IsDebit, ISNULL(Remarks, 'NA')Remarks, sum(TransactionAmount)TransactionAmount  from GLFinancialTransactionDetails
where GLFinancialTransactionId in (select GLFinancialTransactionId   from GLPCJournalDetails 
where glpcjournalid=@GLPCJournalId)
group by branchid,AccountId,IsDebit,Remarks
union all
select distinct branchid, AccountId,IsDebit,Remarks,sum(GrandTotal)TransactionAmount from GLFinancialTransactions
where id in (select GLFinancialTransactionId   from GLPCJournalDetails 
where glpcjournalid=@GLPCJournalId)
group by branchid,AccountId,IsDebit,Remarks
union all
select distinct branchid, VATAccountId,1 IsDebit,'' Remarks,sum(VATAmount)TransactionAmount from GLFinancialTransactions
where id in (select GLFinancialTransactionId   from GLPCJournalDetails 
where glpcjournalid=@GLPCJournalId)
group by branchid,VATAccountId,IsDebit
union all

select distinct branchid, AITAccountId,1 IsDebit,'' Remarks,sum(TaxAmount)TransactionAmount from GLFinancialTransactions
where id in (select GLFinancialTransactionId   from GLPCJournalDetails 
where glpcjournalid=@GLPCJournalId)
group by branchid,AITAccountId,IsDebit
) a 

 left outer join GLBranchs b on a.BranchId=b.id
LEFT OUTER JOIN GLAccounts acc ON a.AccountId = acc.Id
left outer join GLPCJournals pcj on a.branchid=pcj.branchid and pcj.id=@GLPCJournalId
where transactionAmount not in(0)
order by dr desc


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
                da.SelectCommand.Parameters.AddWithValue("@GLPCJournalId", vm.GLPCJournalId);

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

        public DataTable VoucherReportPettyCashRequisitionJournal(GLPCJournalDetailVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
--declare @GLPCRequisitionJournalId as varchar(10)
declare @FundReceive as decimal(18,2)
declare @BranchId as varchar(10)



select @BranchId=BranchId  from GLPCRequisitionJournals
where id=@GLPCRequisitionJournalId

select @FundReceive=sum(FundReceive) from GLPettyCashRequisitions where Id in (select GLPettyCashRequisitionId   from GLPCRequisitionJournalDetails 
where GLPCRequisitionJournalId=@GLPCRequisitionJournalId)
select acc.Code+' ( '+ acc.Name+' )' AccountName, b.Name BranchName,pcj.Code,pcj.TransactionDateTime, a.* 
,case when a.isDebit=1 then a.TransactionAmount else 0 end Dr
,case when a.isDebit=0 then a.TransactionAmount else 0 end Cr
from (
select Id BranchId,DebitAccountId AccountId,1 IsDebit,'' Remarks,@FundReceive TransactionAmount from GLBranchs where id=@BranchId
union all
select Id BranchId,CreditAccountId AccountId,0 IsDebit,'' Remarks,@FundReceive TransactionAmount from GLBranchs where id=@BranchId
) as a

 left outer join GLBranchs b on a.BranchId=b.id
LEFT OUTER JOIN GLAccounts acc ON a.AccountId = acc.Id
left outer join GLPCRequisitionJournals pcj on a.branchid=pcj.branchid and pcj.id=@GLPCRequisitionJournalId
where transactionAmount not in(0)
order by dr desc


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
                da.SelectCommand.Parameters.AddWithValue("@GLPCRequisitionJournalId", vm.GLPCJournalId);

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
