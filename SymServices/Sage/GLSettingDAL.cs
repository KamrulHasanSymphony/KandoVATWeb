using SymOrdinary;
using SymServices.Common;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SymServices.Sage
{
    public class GLSettingDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        #endregion
        #region Methods
        public DataSet SearchSettings()
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            //DataSet dataTable = new DataTable("Search Settings");
            DataSet dataSet = new DataSet("SearchSettings");


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

                sqlText = @"SELECT [Id]
                                      ,[SettingGroup]
                                      ,[SettingName]
                                      ,[SettingValue]
                                      ,[SettingType]
                                      ,[ActiveStatus]
                                      FROM GLSettings
                                      ORDER BY SettingGroup,SettingName;
SELECT DISTINCT s.SettingGroup FROM GLSettings s ORDER BY s.SettingGroup;
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(dataSet);

                #endregion
            }
            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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

            return dataSet;
        }
        public string[] SettingsUpdate(List<GLSettingVM> VMs,SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";
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
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                if (VMs.Any())
                {
                    foreach (var item in VMs)
                    {
                        #region Update Settings
                        sqlText = "";
                        sqlText += "update GLSettings set";
                        sqlText += " SettingValue='" + item.SettingValue + "',";
                        sqlText += " IsActive='" + item.IsActive + "',";
                        sqlText += " LastModifiedBy='" + UserInfoVM.UserName + "',";
                        sqlText += " LastModifiedOn='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                        sqlText += " where SettingGroup='" + item.SettingGroup + "' and SettingName='" + item.SettingName + "'";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);

                        #region Commit

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("SettingsUpdate", item.SettingName + " could not updated.");
                        }

                        #endregion Commit

                        #endregion Update Settings
                    }

                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("SettingsUpdate", "Could not found any item.");
                }


                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                    retResults[0] = "Success";
                    retResults[1] = "Requested Settings Information Successfully Updated.";
                    retResults[2] = "";

                }
                else
                {
                    if (Vtransaction == null) { transaction.Rollback(); }
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update settings.";
                    retResults[2] = "";
                }

            }
            #region catch

            catch (SqlException sqlex)
            {
                retResults[0] = "Fail";//Success or Fail
                if (transaction != null)
                    if (Vtransaction == null) { transaction.Rollback(); }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                if (transaction != null)
                    if (Vtransaction == null) { transaction.Rollback(); }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
        public string[] settingsDataUpdate(GLSettingVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Bank Update"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToBank"); }

                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update GLSettings set";
                    sqlText += " SettingValue=@SettingValue";
                    sqlText += " , Remarks=@Remarks";
                    sqlText += " where SettingGroup=@SettingGroup and  SettingName=@SettingName";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@SettingGroup", vm.SettingGroup.Trim());
                    cmdUpdate.Parameters.AddWithValue("@SettingName", vm.SettingName.Trim());
                    cmdUpdate.Parameters.AddWithValue("@SettingValue", vm.SettingValue.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    //retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", BankVM.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Setting Update", "Could not found any item.");
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
                    retResults[1] = "Data Update Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to update Setting.";
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
        public List<GLSettingVM> SettingsAll(int branchID = 0)
        {
            List<GLSettingVM> VMs = new List<GLSettingVM>();
            GLSettingVM vm;
            SqlConnection currConn = null;
            string sqlText = "";
            try
            {
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionSageGL();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                sqlText = @"SELECT
                            Id
                            ,SettingGroup
                            ,SettingName
                            ,SettingValue
                            ,SettingType
                            ,Remarks
                            FROM GLSettings
WHERE 1=1";


                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new GLSettingVM();
                        vm.Id = dr["Id"].ToString();
                        vm.SettingGroup = dr["SettingGroup"].ToString();
                        vm.SettingName = dr["SettingName"].ToString();
                        vm.SettingValue = dr["SettingValue"].ToString();
                        vm.SettingType = dr["SettingType"].ToString();
                        vm.Remarks = dr["Remarks"].ToString();
                        VMs.Add(vm);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            return VMs;
        }
        public decimal FormatingNumeric(decimal value, int DecPlace)
        {
            object outPutValue = 0;
            string decPointLen = "";
            try
            {
                for (int i = 0; i < DecPlace; i++)
                {
                    decPointLen = decPointLen + "0";
                }
                if (value < 1000)
                {
                    var a = "0." + decPointLen + "";
                    outPutValue = value.ToString(a);
                }
                else
                {
                    var a = "0,0." + decPointLen + "";
                    outPutValue = value.ToString(a);
                }
            }
            #region Catch
            //catch (IndexOutOfRangeException ex)
            //{
            //    FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (NullReferenceException ex)
            //{
            //    FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (FormatException ex)
            //{

            //    FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}

            //catch (SoapHeaderException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }

            //    FileLogger.Log("Program", "FormatTextBoxQty4", exMessage);
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (SoapException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log("Program", "FormatTextBoxQty4", exMessage);
            //}
            //catch (EndpointNotFoundException ex)
            //{
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (WebException ex)
            //{
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            catch (Exception ex)
            {
                //string exMessage = ex.Message;
                //if (ex.InnerException != null)
                //{
                //    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                //                ex.StackTrace;

                //}
                //MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //FileLogger.Log("Program", "FormatTextBoxQty4", exMessage);
            }
            #endregion Catch

            return Convert.ToDecimal(outPutValue);
        }
        public string settingValue(string settingGroup, string settingName, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(settingGroup))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(settingName))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }


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

                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT Id)SettingId FROM GLSettings ";
                sqlText += " WHERE REPLACE(SettingGroup,' ','')='" + settingGroup + "' AND SettingName='" + settingName + "'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                #endregion ProductExist
                #region Last Settings

                int foundId = (int)objfoundId;
                if (foundId > 0)
                {
                    sqlText = "  ";
                    sqlText += " select top 1 SettingValue  FROM GLSettings";
                    sqlText += " WHERE REPLACE(SettingGroup,' ','')='" + settingGroup + "' ";
                    sqlText += " AND SettingName='" + settingName + "'";


                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    object objfoundId1 = cmdExist1.ExecuteScalar();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                    }
                    retResults = (string)objfoundId1;

                }


                #endregion Last Price
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

            }

            #endregion try

            #region Catch and Finall



            catch (Exception ex)
            {
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
        public string[] settingsDataInsert(GLSettingVM vm, string settingGroup, string settingName, string settingType, string settingValue)
        {
            string[] retResults = new string[6];
            //SettingsVM vm = new SettingsVM();
            vm.SettingGroup = settingGroup;
            vm.SettingName = settingName;
            vm.SettingType = settingType;
            vm.SettingValue = settingValue;
            retResults = InsertSettingsData(vm, null, null);
            return retResults;
        }

        public string[] InsertSettingsData(GLSettingVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ

            string sqlText = "";
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "";// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try

            try
            {

                #region Validation

                if (string.IsNullOrEmpty(vm.SettingGroup))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(vm.SettingName))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(vm.SettingType))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(vm.SettingValue))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }

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
                    transaction = currConn.BeginTransaction("Insert");
                }
                #endregion open connection and transaction

                #region Exist Check
                CommonDAL _cDal = new CommonDAL();
                string[] conditionField = { "SettingGroup", "SettingName" };
                string[] conditionValue = { vm.SettingGroup.Trim(), vm.SettingName.Trim() };
                bool exist = _cDal.ExistCheck("GLSettings", conditionField, conditionValue, currConn, transaction);
                if (!exist)
                {

                #endregion Exist Check

                    #region SettingsExist
                    sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from GLSettings where 1=1";
                    SqlCommand cmd111 = new SqlCommand(sqlText, currConn);
                    cmd111.Transaction = transaction;
                    var exeRes = cmd111.ExecuteScalar();
                    int count = Convert.ToInt32(exeRes);


                    #endregion ProductExist

                    #region Last Settings

                    if (true)
                    {
                        sqlText = "  ";
                        sqlText += @" INSERT INTO GLSettings(
Id
,SettingGroup
,SettingName
,SettingValue
,SettingType
,Remarks
) VALUES (
 @Id
,@SettingGroup
,@SettingName
,@SettingValue
,@SettingType
,@Remarks
)";

                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                        cmdInsert.Parameters.AddWithValue("@SettingGroup", vm.SettingGroup);
                        cmdInsert.Parameters.AddWithValue("@SettingName", vm.SettingName);
                        cmdInsert.Parameters.AddWithValue("@SettingValue", vm.SettingValue);
                        cmdInsert.Parameters.AddWithValue("@SettingType", vm.SettingType);
                        cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                        cmdInsert.ExecuteNonQuery();
                        if (Vtransaction == null)
                        {
                            if (transaction != null)
                            {
                                transaction.Commit();
                            }
                        }
                        retResults[0] = "Success";
                        retResults[1] = "Data Save Successfully.";
                        retResults[2] = "0";
                    }
                    #endregion Last Price
                }
            }

            #endregion try

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                if (Vtransaction == null) { transaction.Rollback(); }
                retResults[4] = ex.Message.ToString();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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

        #endregion Methods

    }
}
