using SymOrdinary;
using SymServices.Common;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;

namespace SymServices.Sage
{
    public class GLDocumentNoDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================DropDown=================
        public List<GLDocumentNoVM> DropDown(string tType = "", int branchId = 0)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<GLDocumentNoVM> VMs = new List<GLDocumentNoVM>();
            GLDocumentNoVM vm;
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
select distinct Id, name from GLDocumentNos where TransactionType='PettyCash'
and isactive=1
and name not in(
select distinct name from GLDocumentNos where TransactionType=@tType
and isactive=1
)
";
                if (branchId > 0)
                {
                    sqlText += @" and BranchId=@BranchId";
                }
                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                objComm.Parameters.AddWithValue("@tType", tType == null ? "" : tType);
                if (branchId > 0)
                {
                    objComm.Parameters.AddWithValue("@BranchId", branchId);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLDocumentNoVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Name = dr["Name"].ToString();
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
        //==================SelectAll=================
        public List<GLDocumentNoVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLDocumentNoVM> VMs = new List<GLDocumentNoVM>();
            GLDocumentNoVM vm;
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
SELECT
Id
,Name
,case when  TransactionType='PettyCash' then 'Petty Cash'
 when  TransactionType='BDE' then 'Business Development Expense'
 when  TransactionType='PCR' then 'PC Recovery' else '' end TransactionType
,ReferenceId
,ISNULL(BranchId,0) BranchId
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom

from GLDocumentNos
WHERE  1=1 AND IsArchive = 0
";

                if (Id > 0)
                {
                    sqlText += @" and Id=@Id";
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
                    vm = new GLDocumentNoVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Name = dr["Name"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.ReferenceId = Convert.ToInt32(dr["ReferenceId"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
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
        public string[] Insert(GLDocumentNoVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertGLDocumentNo"; //Method Name
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


                vm.Id = _cDal.NextId("GLDocumentNos", currConn, transaction);
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO GLDocumentNos(
Id
,Name
,TransactionType
,ReferenceId
,BranchId
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Id
,@Name
,@TransactionType
,@ReferenceId
,@BranchId
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";


                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@Name", vm.Name);
                    cmdInsert.Parameters.AddWithValue("@TransactionType", vm.TransactionType ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ReferenceId", vm.ReferenceId);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);

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
                        throw new ArgumentNullException("Unexpected error to update GLDocumentNos.", "");
                    }
                }
                else
                {
                    retResults[1] = "This GLDocumentNo already used!";
                    throw new ArgumentNullException("Please Input GLDocumentNo Value", "");
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



        //==================InsertList =================
        public string[] Insert(List<GLDocumentNoVM> VMs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertGLDocumentNo"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            bool exist = false;
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


                int NextId = _cDal.NextId("GLDocumentNos", currConn, transaction);
                if (VMs != null && VMs.Count > 0)
                {
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO GLDocumentNos(
Id
,Name
,TransactionType
,ReferenceId
,BranchId
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Id
,@Name
,@TransactionType
,@ReferenceId
,@BranchId
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";
                    //string tType = VMs.FirstOrDefault().TransactionType;

                    foreach (GLDocumentNoVM item in VMs)
                    {
                        #region CheckPoint
                        //if (tType != "PettyCash")
                        //{
                            ////string[] conditionFields = { "ReferenceId!", "Name", "IsActive", "TransactionType" };
                            ////string[] conditionValues = { item.ReferenceId.ToString(), item.Name.Trim(), "1", item.TransactionType };
                            ////exist = _cDal.ExistCheck("GLDocumentNos", conditionFields, conditionValues, currConn, transaction);
                            ////if (exist)
                            ////{
                            ////    retResults[1] = "Document No: " + item.Name + " already used!";
                            ////    return retResults;
                            ////    //throw new ArgumentNullException(retResults[1], "");
                            ////}
                        //}
                        #endregion

                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@Id", NextId);
                        cmdInsert.Parameters.AddWithValue("@Name", item.Name);
                        cmdInsert.Parameters.AddWithValue("@TransactionType", item.TransactionType ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@ReferenceId", item.ReferenceId);
                        cmdInsert.Parameters.AddWithValue("@BranchId", item.BranchId);

                        cmdInsert.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", item.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", item.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", item.CreatedFrom);
                        var exeRes = cmdInsert.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        NextId++;
                    }

                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update GLDocumentNos.", "");
                    }
                }
                else
                {
                    retResults[1] = "Please Input GLDocumentNo Value!";
                    throw new ArgumentNullException(retResults[1], "");
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
                retResults[2] = NextId.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                //if (Vtransaction == null && transaction != null)
                //{
                if (Vtransaction == null) { transaction.Rollback(); }
                //}

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
        public string[] Update(GLDocumentNoVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Update"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToGLDocumentNo"); }
                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "UPDATE GLDocumentNos SET";
                    sqlText += " Name=@Name";
                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " WHERE Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", false);
                    //TransactionType
                    //ReferenceId
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update GLDocumentNos.", "");
                    }

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("GLDocumentNo Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLDocumentNo Update", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Data Update Successfully.";
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
        #endregion
    }
}
