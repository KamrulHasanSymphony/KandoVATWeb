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
    public class GLBDERequisitionJournalDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================SelectAll =================
        public List<GLBDERequisitionJournalVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLBDERequisitionJournalVM> VMs = new List<GLBDERequisitionJournalVM>();
            GLBDERequisitionJournalVM vm;
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
brj.Id                               
,brj.BranchId
,br.Name BranchName
,brj.Code                            
,brj.TransactionDateTime             
,brj.TransactionType
,ISNULL(brj.Post, 0) Post
,brj.Remarks                         
,brj.IsActive                        
,brj.IsArchive                       
,brj.CreatedBy                       
,brj.CreatedAt                       
,brj.CreatedFrom                     
,brj.LastUpdateBy                    
,brj.LastUpdateAt                    
,brj.LastUpdateFrom                  
,brj.PostedBy                        
,brj.PostedAt                        
,brj.PostedFrom

                                  
FROM GLBDERequisitionJournals  brj
LEFT OUTER JOIN GLBranchs br ON brj.BranchId = br.id
WHERE  1=1  AND brj.IsArchive = 0
";


                if (Id > 0)
                {
                    sqlText += @" and brj.Id=@Id";
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
                    vm = new GLBDERequisitionJournalVM();
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
        public string[] Insert(GLBDERequisitionJournalVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertGLBDERequisitionJournal"; //Method Name
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
                vm.Id = _cDal.NextId("GLBDERequisitionJournals", currConn, transaction);
                DateTime transDate = Convert.ToDateTime(Ordinary.StringToDate(vm.TransactionDateTime));
                vm.Code = codedal.NextCodeAcc("GLBDERequisitionJournals", vm.BranchId, transDate, vm.TransactionType, currConn, transaction);

                #endregion Code Generate
                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO GLBDERequisitionJournals(
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
                        throw new ArgumentNullException("Unexpected error to update GLBDERequisitionJournals.", "");
                    }
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This GLBDERequisitionJournal already used!";
                    throw new ArgumentNullException("Please Input GLBDERequisitionJournal Value", "");
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
        public string[] CreateJournal(GLBDERequisitionJournalVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                    GLBDERequisitionDAL _bdeReqDAL = new GLBDERequisitionDAL();

                    GLBDERequisitionJournalVM brjVM = new GLBDERequisitionJournalVM();
                    string[] conditionFields = {"bde.MadeJournal"};
                    string[] conditionValues = {"0"};

                    brjVM.BranchId = _bdeReqDAL.SelectAll(Convert.ToInt32(ids[0]),conditionFields, conditionValues, currConn, transaction).FirstOrDefault().BranchId;


                    brjVM.TransactionDateTime = vm.TransactionDateTime;
                    brjVM.CreatedBy = vm.CreatedBy;
                    brjVM.CreatedAt = vm.CreatedAt;
                    brjVM.CreatedFrom = vm.CreatedFrom;
                    brjVM.Post = false;

                    brjVM.TransactionType = vm.TransactionType;

                    retResults = Insert(brjVM, currConn, transaction);
                    string GLBDERequisitionJournalId = retResults[2];
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }

                    for (int i = 0; i < ids.Length - 1; i++)
                    {

                        #region Exist Check
                        string[] conditionField = { "GLBDERequisitionId" };
                        string[] conditionValue = { ids[i] };
                        var exist = _cDal.ExistCheck("GLBDERequisitionJournalDetails", conditionField, conditionValue, currConn
                            , transaction);
                        if (exist)
                        {
                            retResults[1] = "BDE Requisition Already Exist";
                            throw new ArgumentNullException("BDE Requisition Already Exist", "BDE Requisition Already Exist");
                        }
                        #endregion Exist Check

                        sqlText = "";
                        sqlText += @"
                                    insert into GLBDERequisitionJournalDetails(BranchId,GLBDERequisitionJournalId,GLBDERequisitionId)
                                    select branchid,@GLBDERequisitionJournalId,Id from GLBDERequisitions
                                    where Id=@GLBDERequisitionId
                                    
                                    update GLBDERequisitions set MadeJournal=1
                                    where id=@GLBDERequisitionId
                        ";
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@GLBDERequisitionId", ids[i]);
                        cmdInsert.Parameters.AddWithValue("@GLBDERequisitionJournalId", retResults[2]);
                        var exeRes = cmdInsert.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    
                    //retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("BDE Requisition Journal Create", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Delete Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("BDE Requisition Journal Create", "Could not found any item.");
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
                    retResults[1] = "BDE Requisition Journal Created Successfully!";
                }
                else
                {
                    retResults[1] = "Unexpected error to Create BDE Requisition Journal!";
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
        public string[] Delete(GLBDERequisitionJournalVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLBDERequisitionJournal"; //Method Name
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
                        retVal = _cDal.SelectFieldValue("GLBDERequisitionJournals", "Post", "Id", ids[i].ToString(), currConn, transaction);
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
                        #region Update GLBDERequisitionJournalDetails
                        sqlText = "";
                        sqlText += " ";
                        sqlText += "DELETE GLBDERequisitionJournalDetails";
                        sqlText += " WHERE GLBDERequisitionJournalId=@Id";
                        SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        var exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update GLBDERequisitionJournalDetails.", "");
                        }
                        sqlText = " ";
                        sqlText = "DELETE GLBDERequisitionJournals";
                        sqlText += " WHERE Id=@Id";
                        cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update GLBDERequisitionJournalDetails.", "");
                        }
                        #endregion Update GLBDERequisitionJournalDetails
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("GLBDERequisitionJournal Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("GLBDERequisitionJournal Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete GLBDERequisitionJournal Information.";
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

        #endregion
    }
}
