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
    public class GLFundReceivedBDERequisitionDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================DropDown=================
        public List<GLFundReceivedBDERequisitionVM> DropDown(string tType = "", int branchId = 0)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<GLFundReceivedBDERequisitionVM> VMs = new List<GLFundReceivedBDERequisitionVM>();
            GLFundReceivedBDERequisitionVM vm;
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
bb.Id
   FROM GLFundReceivedBDERequisitions brf
WHERE  1=1 AND bb.IsArchive = 0
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLFundReceivedBDERequisitionVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
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
        public List<GLFundReceivedBDERequisitionVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLFundReceivedBDERequisitionVM> VMs = new List<GLFundReceivedBDERequisitionVM>();
            GLFundReceivedBDERequisitionVM vm;
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
 brf.Id
,brf.BranchId
,brf.ReferenceId
,brf.FundAmount
,brf.FinalApprovedDate
,brf.IsReceived
,brf.TransactionDateTime

,br.Code ReferenceCode
,br.TransactionDateTime ReferenceDate
,b.Name BranchName

,brf.Remarks
,brf.IsActive
,brf.IsArchive
,brf.CreatedBy
,brf.CreatedAt
,brf.CreatedFrom
,brf.LastUpdateBy
,brf.LastUpdateAt
,brf.LastUpdateFrom
,brf.ReceivedBy
,brf.ReceivedAt
,brf.ReceivedFrom
FROM GLFundReceivedBDERequisitions brf
LEFT OUTER JOIN GLBDERequisitions br ON brf.ReferenceId = br.Id
LEFT OUTER JOIN GLBranchs b ON brf.BranchId = b.Id
WHERE  1=1  AND brf.IsArchive = 0

";
                //Id
                //BranchId
                //ReferenceId
                //FundAmount
                //FinalApprovedDate
                //IsReceived

                //ReceivedBy
                //ReceivedAt
                //ReceivedFrom
                if (Id > 0)
                {
                    sqlText += @" and brf.Id=@Id";
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
                    vm = new GLFundReceivedBDERequisitionVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.ReferenceId = Convert.ToInt32(dr["ReferenceId"]);
                    vm.FundAmount = Convert.ToDecimal(dr["FundAmount"]);
                    vm.FinalApprovedDate = Ordinary.StringToDate(dr["FinalApprovedDate"].ToString());
                    vm.IsReceived = Convert.ToBoolean(dr["IsReceived"]);
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());

                    vm.ReferenceCode = dr["ReferenceCode"].ToString();
                    vm.ReferenceDate = Ordinary.StringToDate(dr["ReferenceDate"].ToString());
                    vm.BranchName = dr["BranchName"].ToString();

                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.ReceivedAt = Ordinary.StringToDate(dr["ReceivedAt"].ToString());
                    vm.ReceivedBy = dr["ReceivedBy"].ToString();
                    vm.ReceivedFrom = dr["ReceivedFrom"].ToString();
                    VMs.Add(vm);
                    //Id
                    //BranchId
                    //ReferenceId
                    //FundAmount
                    //FinalApprovedDate
                    //IsReceived

                    //ReceivedBy
                    //ReceivedAt
                    //ReceivedFrom
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
        public string[] Insert(GLFundReceivedBDERequisitionVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertGLFundReceivedBDERequisition"; //Method Name
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


                vm.Id = _cDal.NextId("GLFundReceivedBDERequisitions", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO GLFundReceivedBDERequisitions(
Id
,BranchId
,ReferenceId
,FundAmount
,FinalApprovedDate
,IsReceived
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Id
,@BranchId
,@ReferenceId
,@FundAmount
,@FinalApprovedDate
,@IsReceived
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Parameters.AddWithValue("@ReferenceId", vm.ReferenceId);
                    cmdInsert.Parameters.AddWithValue("@FundAmount", vm.FundAmount);
                    cmdInsert.Parameters.AddWithValue("@FinalApprovedDate", Ordinary.DateToString(vm.FinalApprovedDate));
                    cmdInsert.Parameters.AddWithValue("@IsReceived", false);

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
                        throw new ArgumentNullException("Unexpected error to update GLFundReceivedBDERequisitions.", "");
                    }
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This GLFundReceivedBDERequisition already used!";
                    throw new ArgumentNullException("Please Input GLFundReceivedBDERequisition Value", "");
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
                retResults[1] = "Fund Received - BDE Requisition Data Created Successfully!";
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
        public string[] Delete(GLFundReceivedBDERequisitionVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLFundReceivedBDERequisition"; //Method Name
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
                        sqlText = "update GLFundReceivedBDERequisitions set";
                        sqlText += " IsActive=@IsActive";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " ,LastUpdateBy=@LastUpdateBy";
                        sqlText += " ,LastUpdateAt=@LastUpdateAt";
                        sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsActive", false);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("GLFundReceivedBDERequisition Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLFundReceivedBDERequisition Information Delete", "Could not found any item.");
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

        public string[] Receive(GLFundReceivedBDERequisitionVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLFundReceivedBDERequisition"; //Method Name
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

                    #region Update IsFundReceived GLBDERequisition Master and Details
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        int ReferenceId = SelectAll(Convert.ToInt32(ids[i]), null, null, currConn, transaction).FirstOrDefault().ReferenceId;

                        sqlText = "";
                        sqlText += @" update GLBDERequisitions set IsFundReceived=@IsFundReceived 
where Id = @GLBDERequisitionId";
                        sqlText += @" update GLBDERequisitionFormAs set IsFundReceived=@IsFundReceived 
where GLBDERequisitionId = @GLBDERequisitionId And IsRejected = 0";
                        sqlText += @" update GLBDERequisitionFormBs set IsFundReceived=@IsFundReceived 
where GLBDERequisitionId = @GLBDERequisitionId And IsRejected = 0";
                        sqlText += @" update GLBDERequisitionFormCs set IsFundReceived=@IsFundReceived 
where GLBDERequisitionId = @GLBDERequisitionId And IsRejected = 0";
                        sqlText += @" update GLBDERequisitionFormDs set IsFundReceived=@IsFundReceived 
where GLBDERequisitionId = @GLBDERequisitionId And IsRejected = 0";
                        sqlText += @" update GLBDERequisitionFormEs set IsFundReceived=@IsFundReceived 
where GLBDERequisitionId = @GLBDERequisitionId And IsRejected = 0";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@IsFundReceived", true);
                        cmdUpdate.Parameters.AddWithValue("@GLBDERequisitionId", ReferenceId);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }

                    #endregion
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update GLFundReceivedBDERequisitions set";
                        sqlText += " IsReceived=@IsReceived";
                        sqlText += " ,ReceivedBy=@ReceivedBy";
                        sqlText += " ,ReceivedAt=@ReceivedAt";
                        sqlText += " ,ReceivedFrom=@ReceivedFrom";
                        sqlText += " ,TransactionDateTime=@TransactionDateTime";
                        sqlText += " ,Remarks=@Remarks";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsReceived", true);
                        cmdUpdate.Parameters.AddWithValue("@ReceivedBy", vm.ReceivedBy);
                        cmdUpdate.Parameters.AddWithValue("@ReceivedAt", vm.ReceivedAt);
                        cmdUpdate.Parameters.AddWithValue("@ReceivedFrom", vm.ReceivedFrom);
                        cmdUpdate.Parameters.AddWithValue("@TransactionDateTime", Ordinary.DateToString(vm.TransactionDateTime));
                        cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("GLFundReceivedBDERequisition Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLFundReceivedBDERequisition Information Receive", "");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Data Received Successfully.";
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
