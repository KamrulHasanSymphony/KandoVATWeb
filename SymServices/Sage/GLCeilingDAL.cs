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

namespace SymServices.Sage
{
    public class GLCeilingDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods

        //==================SelectAll=================
        public List<GLCeilingVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLCeilingVM> VMs = new List<GLCeilingVM>();
            GLCeilingVM vm;
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
c.Id
,b.Name BranchName
,fy.Year FiscalYear
,fy.YearStart YearStart
,fy.YearEnd YearEnd
,ISNULL(tc.Amount,0) TotalCeiling
,c.Post
,c.GLFiscalYearId
,c.BranchId
,c.PostedBy
,c.PostedAt
,c.PostedFrom
,c.Remarks
,c.IsActive
,c.IsArchive
,c.CreatedBy
,c.CreatedAt
,c.CreatedFrom
,c.LastUpdateBy
,c.LastUpdateAt
,c.LastUpdateFrom

FROM GLCeilings c  
left outer join GLBranchs b on c.branchid=b.id
left outer join GLFiscalYears fy on c.GLFiscalYearId=fy.id
left outer join (select distinct  fd.GLFiscalYearId,cd.BranchId,sum(Amount)Amount from GLCeilingDetails cd
left outer join GLFiscalYearDetails fd on cd.GLFiscalYearDetailId=fd.Id
group by fd.GLFiscalYearId,cd.BranchId)Tc on tc.BranchId=c.BranchId and tc.GLFiscalYearId=c.GLFiscalYearId
WHERE  1=1  AND c.IsArchive = 0

";


                if (Id > 0)
                {
                    sqlText += @" and c.Id=@Id";
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
                    vm = new GLCeilingVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.FiscalYear = dr["FiscalYear"].ToString();
                    vm.YearStart = Ordinary.StringToDate(dr["YearStart"].ToString());
                    vm.YearEnd = Ordinary.StringToDate(dr["YearEnd"].ToString());
                    vm.BranchName = dr["BranchName"].ToString();
                    vm.TotalCeiling = Convert.ToDecimal(dr["TotalCeiling"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.GLFiscalYearId = Convert.ToInt32(dr["GLFiscalYearId"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.PostedBy = dr["PostedBy"].ToString();
                    vm.PostedAt = dr["PostedAt"].ToString();
                    vm.PostedFrom = dr["PostedFrom"].ToString();

                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedAt = dr["CreatedAt"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateAt = dr["LastUpdateAt"].ToString();
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
        public string[] Insert(GLCeilingVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertGLCeiling"; //Method Name
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


                vm.Id = _cDal.NextId("GLCeilings", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO GLCeilings(
Id
,BranchId
,GLFiscalYearId
,Post
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
 

) VALUES (
@Id
,@BranchId
,@GLFiscalYearId
,@Post
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
) 
";


                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Parameters.AddWithValue("@GLFiscalYearId", vm.GLFiscalYearId);
                    cmdInsert.Parameters.AddWithValue("@Post", false);

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
                        throw new ArgumentNullException("Unexpected error to update GLCeilings.", "");
                    }
                    #endregion SqlExecution
                    #region insert Details from Master into Detail Table
                    #region Form D
                    GLCeilingDetailDAL _DAL = new GLCeilingDetailDAL();
                    if (vm.glCeilingDetailVMs != null && vm.glCeilingDetailVMs.Count > 0)
                    {
                        foreach (var eeTransactionDVM in vm.glCeilingDetailVMs)
                        {
                            GLCeilingDetailVM dVM = new GLCeilingDetailVM();
                            dVM = eeTransactionDVM;
                            dVM.GLCeilingId = vm.Id;
                            dVM.BranchId = vm.BranchId;
                            retResults = _DAL.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                    }
                    #endregion Form D
                    #endregion insert Details from Master into Detail Table
                }
                else
                {
                    retResults[1] = "This GLCeiling already used!";
                    throw new ArgumentNullException("Please Input GLCeiling Value", "");
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
        //==================Update =================
        public string[] Update(GLCeilingVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "GLCeilingUpdate"; //Method Name
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
                    sqlText = "UPDATE GLCeilings SET";
                    sqlText += "  Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " WHERE Id=@Id";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);

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
                        throw new ArgumentNullException("Unexpected error to update GLCeilings.", "");
                    }
                    #endregion SqlExecution
                    #region insert Details from Master into Detail Table
                    #region Form A

                    GLCeilingDetailDAL _formADal = new GLCeilingDetailDAL();
                    if (vm.glCeilingDetailVMs != null && vm.glCeilingDetailVMs.Count > 0)
                    {
                        #region Delete Detail
                        try
                        {
                            retResults = _cDal.DeleteTableInformation(vm.Id.ToString(), "GLCeilingDetails", "GLCeilingId", currConn, transaction);
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
                        foreach (var formAVM in vm.glCeilingDetailVMs)
                        {
                            GLCeilingDetailVM dVM = new GLCeilingDetailVM();
                            dVM = formAVM;
                            dVM.GLCeilingId = vm.Id;
                            dVM.BranchId = vm.BranchId;
                            retResults = _formADal.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        #endregion Insert Detail Again
                    }
                    #endregion Form A

                    #endregion insert Details from Master into Detail Table
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("GLCeiling Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLCeiling Update", "Could not found any item.");
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
        public string[] Delete(GLCeilingVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLCeiling"; //Method Name
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
                    #region Check Posted or Not Posted
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        retVal = _cDal.SelectFieldValue("GLCeilings", "Post", "Id", ids[i].ToString(), currConn, transaction);
                        vm.Post = Convert.ToBoolean(retVal);
                        if (vm.Post == true)
                        {
                            retResults[0] = "Fail";
                            retResults[1] = "Data Alreday Posted! Cannot be Deleted.";
                            throw new ArgumentNullException("Data Alreday Posted! Cannot Deleted.", "");
                        }
                    }
                    #endregion Check Posted or Not Posted
                    #region Delete Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText += " ";
                        sqlText += "DELETE GLCeilingDetails";
                        sqlText += " WHERE GLCeilingId=@Id";
                        SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        var exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update GLCeilingDetails.", "");
                        }
                        sqlText = " ";
                        sqlText = "DELETE GLCeilings";
                        sqlText += " WHERE Id=@Id";
                        cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update GLCeilings.", "");
                        }
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("GLCeiling Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Delete Settings
                }
                else
                {
                    throw new ArgumentNullException("GLCeiling Information Delete", "Could not found any item.");
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

        public string[] Post(GLCeilingVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLCeiling"; //Method Name
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
                        sqlText = "update GLCeilings set";
                        sqlText += " Post=@Post";
                        sqlText += " ,PostedBy=@PostedBy";
                        sqlText += " ,PostedAt=@PostedAt";
                        sqlText += " ,PostedFrom=@PostedFrom";
                        sqlText += " where Id=@Id";
                        sqlText += " update GLCeilingDetails set post=@Post where GLCeilingId=@Id";


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
                        throw new ArgumentNullException("GLCeiling Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLCeiling Information Delete", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Data Posted Successfully.";
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

        //==================Insert =================
        public string[] InsertCeiling(GLCeilingVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertGLCeiling"; //Method Name
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
                GLCeilingDetailDAL _dal = new GLCeilingDetailDAL();

                if (vm != null)
                {
                    if (vm.Id == 0)
                    {
                        vm.Id = _cDal.NextId("GLCeilings", currConn, transaction);
                        #region SqlText
                        sqlText = "  ";
                        sqlText += @" 
INSERT INTO GLCeilings(
Id
,BranchId
,GLFiscalYearId
,Post
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
 

) VALUES (
@Id
,@BranchId
,@GLFiscalYearId
,@Post
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
) 
";


                        #endregion SqlText
                        #region SqlExecution
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                        cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                        cmdInsert.Parameters.AddWithValue("@GLFiscalYearId", vm.GLFiscalYearId);
                        cmdInsert.Parameters.AddWithValue("@Post", false);

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
                            throw new ArgumentNullException("Unexpected error to update GLCeilings.", "");
                        }


                        #endregion SqlExecution
                    }
                    else if (vm.Id > 0)
                    {
                        #region SqlText
                        sqlText = "  ";
                        sqlText += @" 
UPDATE GLCeilings
SET Remarks=@Remarks
WHERE Id=@Id
";
                        #endregion SqlText
                        #region SqlExecution
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@Id", vm.Id);

                        cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                        var exeRes = cmdInsert.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update GLCeilings.", "");
                        }
                        #endregion
                    }

                    #region Detail Insert
                    vm.glCeilingDetailVMs.FirstOrDefault().GLCeilingId = vm.Id;
                    retResults = _dal.Insert(vm.glCeilingDetailVMs, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion
                }
                else
                {
                    retResults[1] = "This GLCeiling already used!";
                    throw new ArgumentNullException("Please Input GLCeiling Value", "");
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

        public DataSet FindBalance(string TransactionDateTime, string AccountId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet ds = new DataSet();
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
                sqlText = @"
--declare @TransactionDateTime as varchar(20)
--declare @AccountId as varchar(20)
declare @GLFiscalYearDetailId as varchar(20)
declare @PeriodStart as varchar(20)
declare @PeriodEnd as varchar(20)
declare @Balance as decimal(18,2)

--set @TransactionDateTime='20180315'
--set @AccountId=3


select @GLFiscalYearDetailId=id,@PeriodStart=PeriodStart,@PeriodEnd=PeriodEnd from GLFiscalYearDetails
where 1=1
and @TransactionDateTime>=PeriodStart
and @TransactionDateTime<=PeriodEnd
select isnull(sum(Ceilings),0)Ceilings,isnull(sum(Usege),0)Usege,isnull(sum(Ceilings),0)-isnull(sum(Usege),0) Balance
from(
select  Amount Ceilings,0 Usege from GLCeilingDetails
where 1=1
and AccountId=@AccountId
and GLFiscalYearDetailId=@GLFiscalYearDetailId
 union all

select  0, sum(TransactionAmount) from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.id=d.GLFinancialTransactionId
where 1=1
and m.IsRejected=0
and d.AccountId=@AccountId
and  d.TransactionDateTime>=@PeriodStart
and  d.TransactionDateTime<=@PeriodEnd
 
) as a

SELECT Id, AccountId, GLFiscalYearDetailId, Amount From GLCeilingDetails
where 1=1
and AccountId=@AccountId
and GLFiscalYearDetailId=@GLFiscalYearDetailId
";


                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@TransactionDateTime", Ordinary.DateToString(TransactionDateTime));
                da.SelectCommand.Parameters.AddWithValue("@AccountId", AccountId);

                da.Fill(ds);
                
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
            return ds;
        }

        #endregion
    }
}
