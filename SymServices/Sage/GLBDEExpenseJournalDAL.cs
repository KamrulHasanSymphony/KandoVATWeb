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
    public class GLBDEExpenseJournalDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================SelectAll =================
        public List<GLBDEExpenseJournalVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLBDEExpenseJournalVM> VMs = new List<GLBDEExpenseJournalVM>();
            GLBDEExpenseJournalVM vm;
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

                                  
FROM GLBDEExpenseJournals  brj
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
                    vm = new GLBDEExpenseJournalVM();
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
        public string[] Insert(GLBDEExpenseJournalVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertGLBDEExpenseJournal"; //Method Name
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
                vm.Id = _cDal.NextId("GLBDEExpenseJournals", currConn, transaction);
                DateTime transDate = Convert.ToDateTime(Ordinary.StringToDate(vm.TransactionDateTime));
                vm.Code = codedal.NextCodeAcc("GLBDEExpenseJournals", vm.BranchId, transDate, vm.TransactionType, currConn, transaction);

                #endregion Code Generate
                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO GLBDEExpenseJournals(
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
                        throw new ArgumentNullException("Unexpected error to update GLBDEExpenseJournals.", "");
                    }
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This GLBDEExpenseJournal already used!";
                    throw new ArgumentNullException("Please Input GLBDEExpenseJournal Value", "");
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
        public string[] CreateJournal(GLBDEExpenseJournalVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                    GLBDEExpenseDAL _bdeeDAL = new GLBDEExpenseDAL();

                    GLBDEExpenseJournalVM bejVM = new GLBDEExpenseJournalVM();
                    string[] conditionFields = {"bdee.MadeJournal"};
                    string[] conditionValues = {"0"};

                    bejVM.BranchId = _bdeeDAL.SelectAll(Convert.ToInt32(ids[0]),conditionFields, conditionValues, currConn, transaction).FirstOrDefault().BranchId;


                    bejVM.TransactionDateTime = vm.TransactionDateTime;
                    bejVM.CreatedBy = vm.CreatedBy;
                    bejVM.CreatedAt = vm.CreatedAt;
                    bejVM.CreatedFrom = vm.CreatedFrom;
                    bejVM.Post = false;

                    bejVM.TransactionType = vm.TransactionType;

                    retResults = Insert(bejVM, currConn, transaction);
                    string GLBDEExpenseJournalId = retResults[2];
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }

                    for (int i = 0; i < ids.Length - 1; i++)
                    {

                        #region Exist Check
                        string[] conditionField = { "GLBDEExpenseId" };
                        string[] conditionValue = { ids[i] };
                        var exist = _cDal.ExistCheck("GLBDEExpenseJournalDetails", conditionField, conditionValue, currConn
                            , transaction);
                        if (exist)
                        {
                            retResults[1] = "BDE Expense Already Exist";
                            throw new ArgumentNullException("BDE Expense Already Exist", "BDE Expense Already Exist");
                        }
                        #endregion Exist Check

                        sqlText = "";
                        sqlText += @"
                                    insert into GLBDEExpenseJournalDetails(BranchId,GLBDEExpenseJournalId,GLBDEExpenseId)
                                    select branchid,@GLBDEExpenseJournalId,Id from GLBDEExpenses
                                    where Id=@GLBDEExpenseId
                                    
                                    update GLBDEExpenses set MadeJournal=1
                                    where id=@GLBDEExpenseId
                        ";
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@GLBDEExpenseId", ids[i]);
                        cmdInsert.Parameters.AddWithValue("@GLBDEExpenseJournalId", retResults[2]);
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
                    //    id[0] = GLBDEExpenseJournalId;
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
                        throw new ArgumentNullException("BDE Expense Journal Create", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Delete Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("BDE Expense Journal Create", "Could not found any item.");
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
                    retResults[1] = "BDE Expense Journal Created Successfully!";
                }
                else
                {
                    retResults[1] = "Unexpected error to Create BDE Expense Journal!";
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
        public string[] Delete(GLBDEExpenseJournalVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLBDEExpenseJournal"; //Method Name
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
                        retVal = _cDal.SelectFieldValue("GLBDEExpenseJournals", "Post", "Id", ids[i].ToString(), currConn, transaction);
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
                        #region Update GLBDEExpenseJournalDetails
                        sqlText = "";
                        sqlText += " ";
                        sqlText += "DELETE GLBDEExpenseJournalDetails";
                        sqlText += " WHERE GLBDEExpenseJournalId=@Id";
                        SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        var exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update GLBDEExpenseJournalDetails.", "");
                        }
                        sqlText = " ";
                        sqlText = "DELETE GLBDEExpenseJournals";
                        sqlText += " WHERE Id=@Id";
                        cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update GLBDEExpenseJournalDetails.", "");
                        }
                        #endregion Update GLBDEExpenseJournalDetails
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("GLBDEExpenseJournal Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("GLBDEExpenseJournal Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete GLBDEExpenseJournal Information.";
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

        public DataTable VoucherReportBDEExpense(GLBDEExpenseJournalDetailVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
--declare @GLBDEExpenseJournalId as varchar(10)
--set @GLBDEExpenseJournalId = 1

select acc.Name AccountName, b.Name BranchName,pcj.Code,pcj.TransactionDateTime, a.* 
,case when a.isDebit=1 then a.TransactionAmount else 0 end Dr
,case when a.isDebit=0 then a.TransactionAmount else 0 end Cr
 from(
select distinct branchid,AccountId,IsDebit, ISNULL(Remarks, 'NA')Remarks, sum(TransactionAmount)TransactionAmount 
 from GLBDEExpenseDetails
where GLBDEExpenseId in (select GLBDEExpenseId   from GLBDEExpenseJournalDetails 
where GLBDEExpenseJournalId=@GLBDEExpenseJournalId)
group by branchid,AccountId,IsDebit,Remarks
union all
select distinct branchid, AccountId,IsDebit,Remarks,sum(GrandTotal)TransactionAmount 
from GLBDEExpenses
where id in (select GLBDEExpenseId   from GLBDEExpenseJournalDetails 
where GLBDEExpenseJournalId=@GLBDEExpenseJournalId)
group by branchid,AccountId,IsDebit,Remarks
union all
select distinct branchid, VATAccountId,1 IsDebit,'' Remarks,sum(VATAmount)TransactionAmount 
from GLBDEExpenses
where id in (select GLBDEExpenseId   from GLBDEExpenseJournalDetails 
where GLBDEExpenseJournalId=@GLBDEExpenseJournalId)
group by branchid,VATAccountId,IsDebit
union all

select distinct branchid, AITAccountId,1 IsDebit,'' Remarks,sum(TaxAmount)TransactionAmount 
from GLBDEExpenses
where id in (select GLBDEExpenseId   from GLBDEExpenseJournalDetails 
where GLBDEExpenseJournalId=@GLBDEExpenseJournalId)
group by branchid,AITAccountId,IsDebit
) a 

 left outer join GLBranchs b on a.BranchId=b.id
LEFT OUTER JOIN GLAccounts acc ON a.AccountId = acc.Id
left outer join GLPCJournals pcj on a.branchid=pcj.branchid and pcj.id=@GLBDEExpenseJournalId
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
                da.SelectCommand.Parameters.AddWithValue("@GLBDEExpenseJournalId", vm.GLBDEExpenseId);

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

        public DataTable VoucherReportBDERequisition(GLBDEExpenseJournalDetailVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
--declare @GLBDERequisitionJournalId as varchar(10)
declare @FundReceive as decimal(18,2)
declare @BranchId as varchar(10)

--set @GLBDERequisitionJournalId=1
select @BranchId=BranchId  from GLPCRequisitionJournals
where id=@GLBDERequisitionJournalId
select @FundReceive=sum(FundReceive) 
from GLBDERequisitions where Id in (select GLBDERequisitionId   from GLBDERequisitionJournalDetails 
where GLBDERequisitionJournalId=@GLBDERequisitionJournalId)

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
left outer join GLBDERequisitionJournals pcj on a.branchid=pcj.branchid and pcj.id=@GLBDERequisitionJournalId
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
                da.SelectCommand.Parameters.AddWithValue("@GLBDERequisitionJournalId", vm.GLBDEExpenseId);

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
