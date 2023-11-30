using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymOrdinary;
using SymViewModel.VMS;
using System.Reflection;
namespace SymServices.VMS
{
    public class SettingDAL
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
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"SELECT [SettingId]
                                      ,[SettingGroup]
                                      ,[SettingName]
                                      ,[SettingValue]
                                      ,[SettingType]
                                      ,[ActiveStatus]
                                      FROM Settings
                                      ORDER BY SettingGroup,SettingName;
SELECT DISTINCT s.SettingGroup FROM Settings s ORDER BY s.SettingGroup;
";
                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVehicle);
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
        public string[] SettingsUpdate(List<SettingsVM> settingsVM)
        {
            #region Variables
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            bool iSTransSuccess = false;
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UpdateToSettings");
                #endregion open connection and transaction
         //int tt = 0;
                if (settingsVM.Any())
                {
                    foreach (var item in settingsVM)
                    {
                        //tt++;
                        //Debug.WriteLine(tt.ToString());
                        #region Update Settings
                        sqlText = "";
                        sqlText += "update Settings set";
                        sqlText += " SettingValue=@itemSettingValue,";
                        sqlText += " ActiveStatus=@itemActiveStatus,";
                        sqlText += " LastModifiedBy='" + UserInfoVM.UserName + "',";
                        sqlText += " LastModifiedOn='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                        sqlText += " where SettingGroup=@itemSettingGroup  and SettingName=@itemSettingName ";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Transaction = transaction;
                        cmdUpdate.Parameters.AddWithValue("@itemSettingValue", item.SettingValue);
                        cmdUpdate.Parameters.AddWithValue("@itemActiveStatus", item.ActiveStatus);
                        cmdUpdate.Parameters.AddWithValue("@itemSettingGroup", item.SettingGroup);
                        cmdUpdate.Parameters.AddWithValue("@itemSettingName", item.SettingName);
                        transResult = (int)cmdUpdate.ExecuteNonQuery();
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
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Requested Settings Information Successfully Updated.";
                    retResults[2] = "";
                }
                else
                {
                    transaction.Rollback();
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update settings.";
                    retResults[2] = "";
                }
            }
            #region catch
            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
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
            return retResults;
        }
        public void SettingsUpdate(string companyId)
        {
            #region Variables
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            #endregion
            try
            {
                #region open connection and transaction
                CommonDAL commonDal = new CommonDAL();
                commonDal.DatabaseTableChanges();
                SaleDAL sdal= new SaleDAL();
               sdal.LoadIssueItems();
                #region Security 20140101
               commonDal.SetSecurity(companyId);
                #endregion Security 20140101
                #endregion open connection and transaction
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (ArgumentNullException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #endregion
        }
        private void IssuePriceUpdate(SqlConnection currConn)
        {
            #region Initializ
            string retResults = "0";
            string sqlText = "";
            SqlTransaction transaction = null;
            int transResult = 0;
            try
            {
                #endregion
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                    transaction = currConn.BeginTransaction(MessageVM.receiveMsgMethodNameInsert);
                }
                #endregion open connection and transaction
                #region find Null
                #region Update if Null
                sqlText = "  ";
                sqlText +=
                    " select  count(distinct Itemno) from IssueDetails WHERE UOMQty IS NULL ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                transResult = (int)cmdExist.ExecuteScalar();
                if (transResult > 0)
                {
                    #region Update if Null
                    sqlText = "  ";
                    sqlText +=
                        " UPDATE IssueDetails SET UOMc = 1, UOMQty = Quantity,uomn=UOM,UOMPrice = CostPrice WHERE UOMQty IS NULL ";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    transResult = (int)cmdExist1.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                        MessageVM.receiveMsgSaveNotSuccessfully);
                    }
                    #endregion ProductExist
                }
                #endregion ProductExist
                #endregion find Null
                #region
                sqlText = "";
                sqlText +=
                    "   SELECT IssueNo,ItemNo,IssueLineNo,isnull(UOMPrice,0)UOMPrice,isnull(CostPrice,0)CostPrice,isnull(uomc,0)uomc,isnull(SubTotal,0)SubTotal,IssueDateTime,isnull(Quantity,0)Quantity";
                sqlText += " FROM IssueDetails ";
                //sqlText += " where  IssueNo='REC-0034/0713' ";
                DataTable dataTable = new DataTable("RIFB");
                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                reportDataAdapt.Fill(dataTable);
                if (dataTable != null || dataTable.Rows.Count > 0)
                {
                    ProductDAL productDal = new ProductDAL();
                    foreach (DataRow BRItem in dataTable.Rows)
                    {
                        string vIssueNo = BRItem["IssueNo"].ToString();
                        string vItemNo = BRItem["ItemNo"].ToString();
                        string vIssueLineNo = BRItem["IssueLineNo"].ToString();
                        //DateTime vIssueDateTime =Convert.ToDateTime(BRItem["IssueDateTime"].ToString());
                        string vIssueDateTime = BRItem["IssueDateTime"].ToString();
                        decimal vSubTotal = Convert.ToDecimal(BRItem["SubTotal"].ToString());
                        decimal vUomc = Convert.ToDecimal(BRItem["uomc"].ToString());
                        decimal vCostPrice = Convert.ToDecimal(BRItem["CostPrice"].ToString());
                        decimal vUOMPrice = Convert.ToDecimal(BRItem["UOMPrice"].ToString());
                        decimal vQuantity = Convert.ToDecimal(BRItem["Quantity"].ToString());
                        //decimal vUOMPrice1 = productDal.AvgPrice(vItemNo, vIssueDateTime, currConn, transaction);
                        decimal vUOMPrice1 = 0;
                        DataTable priceData = productDal.AvgPriceNew(vItemNo, vIssueDateTime, null, null,false);
                        decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                        decimal quantity = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());
                        if (quantity > 0)
                        {
                            vUOMPrice1 = amount / quantity;
                        }
                        else
                        {
                            vUOMPrice1 = 0;
                        }
                        decimal vCostPrice1 = vUOMPrice1*vUomc;
                        decimal vSubTotal1 = vCostPrice1*vQuantity;
                        #region Update UnitCost
                        sqlText = "  ";
                        sqlText += " UPDATE IssueDetails SET ";
                        sqlText += " UOMPrice=@vUOMPrice1,";
                        sqlText += " CostPrice=@vCostPrice1,";
                        sqlText += " SubTotal =@vSubTotal1";
                        sqlText += " WHERE IssueNo=@vIssueNo";
                        sqlText += " AND ItemNo=@vItemNo";
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.Parameters.AddWithValue("@vUOMPrice1", vUOMPrice1);
                        cmdInsert.Parameters.AddWithValue("@vCostPrice1", vCostPrice1);
                        cmdInsert.Parameters.AddWithValue("@vSubTotal1", vSubTotal1);
                        cmdInsert.Parameters.AddWithValue("@vIssueNo", vIssueNo ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@vItemNo", vItemNo ?? Convert.DBNull);
                        transResult = (int) cmdInsert.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                            MessageVM.receiveMsgSaveNotSuccessfully);
                        }
                        #region Update Issue Header
                        sqlText = "";
                        sqlText += " update IssueHeaders set ";
                        sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                        sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                        sqlText += " where (IssueHeaders.IssueNo=@vIssueNo)";
                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                        cmdUpdateIssue.Transaction = transaction;
                        cmdUpdateIssue.Parameters.AddWithValue("@vIssueNo", vIssueNo);
                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                            MessageVM.receiveMsgUnableToSaveIssue);
                        }
                        #endregion Update Issue Header
                        #endregion Update UnitCost
                    }
                }
                #endregion
                #region Commit
                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #region Catch and Finall
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
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
            #endregion Catch and Finall
        }
        #endregion
        public string settingsDataInsert(string settingGroup, string settingName, string settingType, string settingValue,
            SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ
            string retResults = "0";
            string sqlText = "";
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
                else if (string.IsNullOrEmpty(settingType))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(settingValue))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                #endregion Validation
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                #endregion open connection and transaction
                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT SettingId)SettingId FROM Settings ";
                sqlText += " WHERE SettingGroup=@settingGroup AND SettingName=@settingName" +
                           " AND SettingType=@settingType ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@settingName", settingName);
                cmdExist.Parameters.AddWithValue("@settingGroup", settingGroup);
                cmdExist.Parameters.AddWithValue("@settingType", settingType);
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                #endregion ProductExist
                #region Last Settings
                int foundId = (int) objfoundId;
                if (foundId <= 0)
                {
                    sqlText = "  ";
                    sqlText +=
                        " INSERT INTO Settings(	SettingGroup,	SettingName,SettingValue,SettingType,ActiveStatus,CreatedBy,CreatedOn,LastModifiedBy,LastModifiedOn)";
                    sqlText += " VALUES(";
                    sqlText += "@settingGroup,";
                    sqlText += "@settingName,";
                    sqlText += "@settingValue,";
                    sqlText += "@settingType,";
                    sqlText += " 'Y',";
                    sqlText += " '" + UserInfoVM.UserName + "',";
                    sqlText += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                    sqlText += " '" + UserInfoVM.UserName + "',";
                    sqlText += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    sqlText += " )";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    cmdExist1.Parameters.AddWithValue("@settingGroup", settingGroup);
                    cmdExist1.Parameters.AddWithValue("@settingName", settingName);
                    cmdExist1.Parameters.AddWithValue("@settingValue", settingValue);
                    cmdExist1.Parameters.AddWithValue("@settingType", settingType);
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                    }
                    int save = (int) objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                    }
                }
                #endregion Last Price
                #region insert data into settingRole table
                //if (!string.IsNullOrEmpty(userId))
                //{
                //    SettingRoleDAL settingRoleDal = new SettingRoleDAL();
                //    settingRoleDal.settingsDataInsert(settingGroup, settingName, settingType, settingValue, userId,
                //                                      currConn, transaction);
                //}
                #endregion
            }
                #endregion try
            #region Catch and Finall
            #region Catch
            catch (Exception ex)
            {
                retResults = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                transaction.Rollback();
                FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
                return retResults;
            }
            #endregion
            finally
            {
                //if (currConn == null)
                //{
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                //}
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }
        //public string ProductTypeDataInsert(string ProductType, SqlConnection currConn, SqlTransaction transaction)
        //{
        //    #region Initializ
        //    string retResults = "0";
        //    string sqlText = "";
        //    #endregion
        //    #region Try
        //    try
        //    {
        //        #region Validation
        //        if (string.IsNullOrEmpty(ProductType))
        //        {
        //            throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
        //        }
        //        #endregion Validation
        //        #region open connection and transaction
        //        if (currConn == null)
        //        {
        //            currConn = _dbsqlConnection.GetConnection();
        //            if (currConn.State != ConnectionState.Open)
        //            {
        //                currConn.Open();
        //            }
        //        }
        //        #endregion open connection and transaction
        //        #region ProductID
        //        sqlText = "select isnull(max(cast(TypeID as int)),0)+1 FROM  ProductTypes ";
        //        SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
        //        cmdNextId.Transaction = transaction;
        //        object objNextId = cmdNextId.ExecuteScalar();
        //        if (objNextId == null)
        //        {
        //            throw new ArgumentNullException("InsertToOverheadInformation",
        //                                             "Unable to create new Overhead information Id");
        //        }
        //        string nextId = objNextId.ToString();
        //        if (string.IsNullOrEmpty(nextId))
        //        {
        //            throw new ArgumentNullException("InsertToOverheadInformation",
        //                                            "Unable to create new Overhead information Id");
        //        }
        //        #endregion ProductID
        //        #region SettingsExist
        //        sqlText = "  ";
        //        sqlText += " SELECT COUNT(DISTINCT ProductType)ProductType FROM ProductTypes ";
        //        sqlText += " WHERE ProductType='" + ProductType + "'";
        //        SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
        //        cmdExist.Transaction = transaction;
        //        object objfoundId = cmdExist.ExecuteScalar();
        //        if (objfoundId == null)
        //        {
        //            throw new ArgumentNullException("settingsDataInsert", "Please Input ProductTypes Value");
        //        }
        //        #endregion ProductExist
        //        #region Last Settings
        //        int foundId = (int)objfoundId;
        //        if (foundId <= 0)
        //        {
        //            sqlText = "  ";
        //            sqlText += " INSERT INTO [dbo].[ProductTypes] ([TypeID],[ProductType],[Comments],[ActiveStatus],[CreatedBy],[CreatedOn],[LastModifiedBy],[LastModifiedOn],[Description])";
        //            sqlText += " VALUES(";
        //            sqlText += " '" + nextId + "',";
        //            sqlText += " '" + ProductType + "',";
        //            sqlText += " 'NA',";
        //            sqlText += " 'Y',";
        //            sqlText += " 'admin',";
        //            sqlText += " '1900-01-01',";
        //            sqlText += " 'admin',";
        //            sqlText += " '1900-01-01',";
        //            sqlText += " '" + ProductType + "'";
        //            sqlText += " )";
        //            SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
        //            cmdExist1.Transaction = transaction;
        //            object objfoundId1 = cmdExist1.ExecuteNonQuery();
        //            if (objfoundId1 == null)
        //            {
        //                throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
        //            }
        //            int save = (int)objfoundId1;
        //            if (save <= 0)
        //            {
        //                throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
        //            }
        //        }
        //        #endregion Last Price
        //    }
        //    #endregion try
        //    #region Catch and Finall
        //    catch (SqlException sqlex)
        //    {
        //        throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
        //        //throw sqlex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
        //        //throw ex;
        //    }
        //    finally
        //    {
        //        if (currConn == null)
        //        {
        //            if (currConn.State == ConnectionState.Open)
        //            {
        //                currConn.Close();
        //            }
        //        }
        //    }
        //    #endregion
        //    #region Results
        //    return retResults;
        //    #endregion
        //}
        public string ProductCategoryDataInsert(SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ
            string retResults = "0";
            string sqlText = "";
            #endregion
            #region Try
            try
            {
              #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                #endregion open connection and transaction
                #region Exist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT CategoryID)CategoryID FROM ProductCategories ";
                sqlText += " WHERE CategoryID='0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("DataInsert", "Please Input ProductCategories Value");
                }
                #endregion ProductExist
                #region Insert
                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = " ";
                    sqlText += " INSERT [dbo].[ProductCategories] ([CategoryID], [CategoryName], [Description], [Comments], [IsRaw], [HSCodeNo], [VATRate], [PropergatingRate], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [SD], [Trading], [NonStock], [Info4], [Info5]) VALUES (N'0', N'NA', N'NA', N'NA', N'Overhead', N'0.00', CAST(30.000000000 AS Decimal(25, 9)), N'N', N'N', N'admin', CAST(0x0000A16400F8CA3C AS DateTime), N'admin', CAST(0x0000A1A30106ECFC AS DateTime), CAST(30.000000000 AS Decimal(25, 9)), N'N', N'N', N'NA', N'NA')";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input ProductCategories Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input ProductCategories Value");
                    }
                }
                #endregion Last Price
            }
            #endregion try
            #region Catch and Finall
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
            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }
        public string ProductDataInsert(SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ
            string retResults = "0";
            string sqlText = "";
            #endregion
            #region Try
            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                #endregion open connection and transaction
                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT ItemNo)ItemNo FROM Products ";
                sqlText += " WHERE ItemNo='ovh0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("DataInsert", "Please Input ProductCategories Value");
                }
                #endregion ProductExist
                #region Insert
                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = " ";
                    sqlText += " INSERT [dbo].[Products] ([ItemNo], [ProductCode], [ProductName], [ProductDescription], [CategoryID], [UOM], [CostPrice], [SalesPrice], [NBRPrice], [ReceivePrice], [IssuePrice], [TenderPrice], [ExportPrice], [InternalIssuePrice], [TollIssuePrice], [TollCharge], [OpeningBalance], [SerialNo], [HSCodeNo], [VATRate], [Comments], [SD], [PacketPrice], [Trading], [TradingMarkUp], [NonStock], [QuantityInHand], [OpeningDate], [RebatePercent], [TVBRate], [CnFRate], [InsuranceRate], [CDRate], [RDRate], [AITRate], [TVARate], [ATVRate], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [OpeningTotalCost]) VALUES (N'ovh0', N'ovh0', N'Margin', N'-', N'0', N'-', CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), N'-', N'', CAST(0.000000000 AS Decimal(25, 9)), N'', CAST(0.000000000 AS Decimal(25, 9)), CAST(0.000000000 AS Decimal(25, 9)), N'N', CAST(0.000000000 AS Decimal(25, 9)), N'N', CAST(0.000000000 AS Decimal(25, 9)), CAST(0x0000A1A40105ED84 AS DateTime), CAST(0.000000000 AS Decimal(25, 9)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'Y', N'admin', CAST(0x0000A1A401060044 AS DateTime), N'admin', CAST(0x0000A1A401224A74 AS DateTime), NULL)";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input ProductCategories Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input ProductCategories Value");
                    }
                }
                #endregion Last Price
            }
            #endregion try
            #region Catch and Finall
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
            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }
        public string BankDataInsert(SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ
            string retResults = "0";
            string sqlText = "";
            #endregion
            #region Try
            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                #endregion open connection and transaction
                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT BankID)BankID FROM BankInformations ";
                sqlText += " WHERE BankID='0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("DataInsert", "Please Input ProductCategories Value");
                }
                #endregion ProductExist
                #region Insert
                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = " ";
                    sqlText += " INSERT [BankInformations] ([BankID], [BankCode], [BankName], [BranchName], [AccountNumber], [Address1], [Address2], [Address3], [City], [TelephoneNo], [FaxNo], [Email], [ContactPerson], [ContactPersonDesignation], [ContactPersonTelephone], [ContactPersonEmail], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5]) VALUES (N'0', N'0', N'NA', N'NA', N'NA', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'-', N'Y', N'admin', CAST(0x0000A19A00C0D9EC AS DateTime), N'admin', CAST(0x0000A19A00C0D9EC AS DateTime), NULL, NULL, NULL, NULL, NULL)";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input ProductCategories Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input ProductCategories Value");
                    }
                }
                #endregion Last Price
            }
            #endregion try
            #region Catch and Finall
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
            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }
        public string VendorGroupInsert(SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ
            string retResults = "0";
            string sqlText = "";
            #endregion
            #region Try
            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                #endregion open connection and transaction
                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT VendorGroupID)BankID FROM VendorGroups ";
                sqlText += " WHERE VendorGroupID='0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("DataInsert", "Please Input VendorGroups Value");
                }
                #endregion ProductExist
                #region Insert
                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = " ";
                    sqlText += " INSERT [dbo].[VendorGroups] ([VendorGroupID], [VendorGroupName], [VendorGroupDescription], [GroupType], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info3], [Info4], [Info5], [Info2]) VALUES (N'0', N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A''N/A', N'N/A', NULL)";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input VendorGroups Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input VendorGroups Value");
                    }
                }
                #endregion Last Price
            }
            #endregion try
            #region Catch and Finall
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
            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }
        public string VendorInsert(SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ
            string retResults = "0";
            string sqlText = "";
            #endregion
            #region Try
            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                #endregion open connection and transaction
                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT VendorID)VendorID FROM Vendors ";
                sqlText += " WHERE VendorID='0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("DataInsert", "Please Input Vendors Value");
                }
                #endregion ProductExist
                #region Insert
                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = " ";
                    sqlText += " INSERT [dbo].[Vendors] ([VendorID], [VendorCode], [VendorName], [VendorGroupID], [Address1], [Address2], [Address3], [City], [TelephoneNo], [FaxNo], [Email], [StartDateTime], [ContactPerson], [ContactPersonDesignation], [ContactPersonTelephone], [ContactPersonEmail], [VATRegistrationNo], [TINNo], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Country], [Info2], [Info3], [Info4], [Info5]) VALUES (N'0', NULL, N'N/A', N'0', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A')";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input Vendors Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input Vendors Value");
                    }
                }
                #endregion Last Price
            }
            #endregion try
            #region Catch and Finall
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
            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }
        public string CustomerGroupInsert(SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ
            string retResults = "0";
            string sqlText = "";
            #endregion
            #region Try
            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                #endregion open connection and transaction
                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT CustomerGroupID)CustomerGroupID FROM CustomerGroups ";
                sqlText += " WHERE CustomerGroupID='0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("DataInsert", "Please Input CustomerGroups Value");
                }
                #endregion ProductExist
                #region Insert
                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = " ";
                    sqlText += " INSERT [dbo].[CustomerGroups] ([CustomerGroupID], [CustomerGroupName], [CustomerGroupDescription], [GroupType], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5]) VALUES (N'0', N'N/A', N'N/A', N'Local', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'admin', CAST(0x0000A17500C8DF0C AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A')";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input CustomerGroups Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input CustomerGroups Value");
                    }
                }
                #endregion Last Price
            }
            #endregion try
            #region Catch and Finall
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
            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }
        public string CustomerInsert(SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ
            string retResults = "0";
            string sqlText = "";
            #endregion
            #region Try
            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                #endregion open connection and transaction
                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT CustomerID)CustomerID FROM Customers ";
                sqlText += " WHERE CustomerID='0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("DataInsert", "Please Input Customers Value");
                }
                #endregion ProductExist
                #region Insert
                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = " ";
                    sqlText += " INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [CustomerGroupID], [Address1], [Address2], [Address3], [City], [TelephoneNo], [FaxNo], [Email], [StartDateTime], [ContactPerson], [ContactPersonDesignation], [ContactPersonTelephone], [ContactPersonEmail], [TINNo], [VATRegistrationNo], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info2], [Info3], [Info4], [Info5], [Country]) VALUES (N'0', NULL, N'N/A', N'0', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', NULL)";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input Customers Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input Customers Value");
                    }
                }
                #endregion Last Price
            }
            #endregion try
            #region Catch and Finall
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
            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }
        public string VehicleInsert( SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ
            string retResults = "0";
            string sqlText = "";
            #endregion
            #region Try
            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                #endregion open connection and transaction
                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT VehicleID)VehicleID FROM Vehicles ";
                sqlText += " WHERE VehicleID='0'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("DataInsert", "Please Input Vehicles Value");
                }
                #endregion ProductExist
                #region Insert
                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = " ";
                    sqlText += " INSERT [dbo].[Vehicles] ([VehicleID], [VehicleCode], [VehicleType], [VehicleNo], [Description], [Comments], [ActiveStatus], [CreatedBy], [CreatedOn], [LastModifiedBy], [LastModifiedOn], [Info1], [Info2], [Info3], [Info4], [Info5]) VALUES (N'0', NULL, N'N/A', N'N/A', N'N/A', N'N/A', N'Y', N'Admin', CAST(0x0000000000000000 AS DateTime), N'Admin', CAST(0x0000000000000000 AS DateTime), N'N/A', N'N/A', N'N/A', N'N/A', N'N/A')";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input Vehicles Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("DataInsert", "Please Input Vehicles Value");
                    }
                }
                #endregion Last Price
            }
            #endregion try
            #region Catch and Finall
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
            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }
        public string UpdateTablesData(SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ
            string retResults = "0";
            string sqlText = "";
            #endregion
            #region Try
            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                #endregion open connection and transaction
                #region Last Settings
                //sqlText += " UPDATE ProductCategories SET 	IsRaw ='Service' where IsRaw = 'Non Stock' ";
                sqlText += " UPDATE AdjustmentHistorys SET 	AdjType ='Credit Payable' where AdjType = 'Credit Payble' ";
                sqlText += " UPDATE AdjustmentHistorys SET 	AdjType ='Cash Payable' where AdjType = 'Cash Payble'";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                    }
                #endregion Last Price
            }
            #endregion try
            #region Catch and Finall
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
            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }
        public string settingsDataDelete(string settingGroup, string settingName, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ
            string retResults = "0";
            string sqlText = "";
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
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                #endregion open connection and transaction
                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT SettingId)SettingId FROM Settings ";
                sqlText += " WHERE REPLACE(SettingGroup,' ','')=@settingGroup AND SettingName=@settingName ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@settingName", settingName);
                cmdExist.Parameters.AddWithValue("@settingGroup", settingGroup);
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                #endregion ProductExist
                #region Last Settings
                int foundId = (int)objfoundId;
                if (foundId >0)
                {
                    sqlText = "  ";
                    sqlText += " DELETE FROM Settings";
                    sqlText += " WHERE REPLACE(SettingGroup,' ','')=@settingGroup ";
                    sqlText += " AND SettingName=@settingName";
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    cmdExist1.Parameters.AddWithValue("@settingGroup", settingGroup);
                    cmdExist1.Parameters.AddWithValue("@settingName", settingName);
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                    }
                }
                #endregion Last Price
            }
            #endregion try
            #region Catch and Finall
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
            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }
        public string settingsDataUpdate(string settingGroup, string settingName,string settingGroupNew, string settingNameNew
            ,SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ
            string retResults = "0";
            string sqlText = "";
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
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                #endregion open connection and transaction
                #region ProductExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT SettingId)SettingId FROM Settings ";
                sqlText += " WHERE SettingGroup=@settingGroup  AND SettingName=@settingName ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@settingName", settingName);
                cmdExist.Parameters.AddWithValue("@settingGroup", settingGroup);
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                #endregion ProductExist
                #region Last Price
                int foundId = (int)objfoundId;
                if (foundId > 0)
                {
                    sqlText = "";
                    sqlText = "update Settings set";
                    sqlText += " SettingName=@settingNameNew ,";
                    sqlText += " SettingValue=@settingGroupNew ";
                    sqlText += " where SettingGroup=@settingGroup and SettingName=@settingName";
                    //sqlText += " where SettingId='" + item.SettingId + "'" + " and SettingGroup='" + item.SettingGroup + "' and SettingName='" + item.SettingName + "'";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Transaction = transaction;
                    cmdUpdate.Parameters.AddWithValue("@settingNameNew", settingNameNew);
                    cmdUpdate.Parameters.AddWithValue("@settingGroupNew", settingGroupNew);
                    cmdUpdate.Parameters.AddWithValue("@settingName", settingName);
                    cmdUpdate.Parameters.AddWithValue("@settingGroup", settingGroup);
                    object objfoundId1 = cmdUpdate.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                    }
                }
                #endregion Last Price
            }
            #endregion try
            #region Catch and Finall
#region Catch
            catch (Exception ex)
            {
                retResults = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                transaction.Rollback();
                FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
                return retResults;
            }
            #endregion
            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }
        public bool CheckUserAccess()
        {
            bool isAlloweduser = false;
            CommonDAL commonDal=new CommonDAL();
            bool isAccessTransaction =
                Convert.ToBoolean(commonDal.settings("Transaction", "AccessTransaction") == "Y" ? true : false);
            if (!isAccessTransaction)
            {
                string userName = commonDal.settings("Transaction", "AccessUser");
                if (userName.ToLower() == UserInfoVM.UserName.ToLower())
                {
                    isAlloweduser = true;
                }
            }
            else
            {
                isAlloweduser = true;
            }
            return isAlloweduser;
        }
        public string UpdateInternalIssueValue()
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";
            string result = "";
            CommonDAL commDal = new CommonDAL();
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameInsert);
                #endregion open connection and transaction
                #region sql statement
                #region Issue
                sqlText = @"SELECT [ItemNo]
                                  ,[IssueDateTime]
                                  ,[IssueNo]
                                  ,[Quantity]
                                  ,[Transactiontype]
                                   from IssueDetails where Transactiontype = 'InternalIssue' and IssueDateTime > '2013-10-08';
                                ";
                DataTable dt = new DataTable();
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;
                SqlDataAdapter adptInterIssue = new SqlDataAdapter(cmdIdExist);
                adptInterIssue.Fill(dt);
                if (dt == null || dt.Rows.Count <= 0)
                {
                    //throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                }
                else
                {
                    ProductDAL proDal = new ProductDAL();
                    string itemNo, issueDate, issueNo = string.Empty;
                    DateTime issueDateTime = DateTime.Now;
                    DataTable avgPriceData = new DataTable();
                    decimal AvgRate, NBRPrice, Qty = 0;
                    foreach (DataRow item in dt.Rows)
                    {
                        itemNo = item["ItemNo"].ToString();
                        issueDateTime = Convert.ToDateTime(item["IssueDateTime"].ToString());
                        issueNo = item["IssueNo"].ToString();
                        Qty = Convert.ToDecimal(item["Quantity"].ToString());
                        #region Find Avg
                        issueDate = issueDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                        avgPriceData = proDal.AvgPriceNew(itemNo, issueDate, currConn, transaction, false);
                        decimal amount = Convert.ToDecimal(avgPriceData.Rows[0]["Amount"].ToString());
                        decimal quantity = Convert.ToDecimal(avgPriceData.Rows[0]["Quantity"].ToString());
                        if (quantity > 0)
                        {
                            AvgRate = amount / quantity;
                        }
                        else
                        {
                            AvgRate = 0;
                        }
                        #endregion Find Avg
                        #region Issue Settings
                        int IssuePlaceQty = Convert.ToInt32(commDal.settings("Issue", "Quantity"));
                        int IssuePlaceAmt = Convert.ToInt32(commDal.settings("Issue", "Amount"));
                        AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                        Qty = FormatingNumeric(Qty, IssuePlaceQty);
                        #endregion Issue Settings
                        #region Find NBR Price
                        //NBRPrice = proDal.GetLastNBRPriceFromBOM(itemNo, "VAT 1 (Internal Issue)", issueDate, currConn, transaction);
                        #endregion Find NBR Price
                        #region Update Issue Details
                        sqlText = "";
                        sqlText += " UPDATE IssueDetails SET NBRPrice ='0', ";
                        sqlText += " CostPrice =@AvgRate, ";
                        sqlText += " SubTotal =@SubTotal, ";
                        sqlText += " UOMPrice =@AvgRate";
                        sqlText += " where ItemNo=@itemNo and IssueNo=@issueNo   and  Transactiontype = 'InternalIssue' ";
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@AvgRate", AvgRate);
                        cmdInsDetail.Parameters.AddWithValue("@SubTotal", FormatingNumeric(AvgRate * Qty, IssuePlaceAmt));
                        cmdInsDetail.Parameters.AddWithValue("@issueNo", issueNo ?? Convert.DBNull);
                        transResult = (int)cmdInsDetail.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                        }
                        #endregion Update Issue Details
                    }
                }
                #endregion  Issue
                #region Receive
                sqlText = " ";
                sqlText += @"SELECT [ItemNo]
                                  ,[ReceiveNo]
                                  ,[CostPrice]
                                  ,[Quantity]
                                  ,[Transactiontype]
                                   from ReceiveDetails where Transactiontype = 'InternalIssue'
                                ";
                dt = new DataTable();
                cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;
                SqlDataAdapter adptInterRecei = new SqlDataAdapter(cmdIdExist);
                adptInterRecei.Fill(dt);
                if (dt == null || dt.Rows.Count <= 0)
                {
                    //throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                }
                else
                {
                    string itemNo, receiveNo = string.Empty;
                    decimal CostPrice, Qty = 0;
                    foreach (DataRow item in dt.Rows)
                    {
                        itemNo = item["ItemNo"].ToString();
                        receiveNo = item["ReceiveNo"].ToString();
                        Qty = Convert.ToDecimal(item["Quantity"].ToString());
                        CostPrice = Convert.ToDecimal(item["CostPrice"].ToString());
                        int ReceivePlaceQty = Convert.ToInt32(commDal.settings("Receive", "Quantity"));
                        int ReceivePlaceAmt = Convert.ToInt32(commDal.settings("Receive", "Amount"));
                        CostPrice = FormatingNumeric(CostPrice, ReceivePlaceAmt);
                        Qty = FormatingNumeric(Qty, ReceivePlaceQty);
                        #region Update Receive Details
                        sqlText = "";
                        sqlText += " UPDATE ReceiveDetails SET ";
                        sqlText += " SubTotal =@SubTotal ";
                        sqlText += " where ItemNo=@itemNo  and ReceiveNo=@receiveNo   and  Transactiontype = 'InternalIssue' ";
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@itemNo", itemNo);
                        cmdInsDetail.Parameters.AddWithValue("@SubTotal", FormatingNumeric(CostPrice * Qty, ReceivePlaceAmt));
                        cmdInsDetail.Parameters.AddWithValue("@receiveNo", receiveNo ?? Convert.DBNull);
                        transResult = (int)cmdInsDetail.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                        }
                    }
                        #endregion Update Receive Details
                }
                #endregion Receive
                #endregion
                #region Commit
                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        result = "Success";
                    }
                }
                #endregion Commit
            }
            #region catch
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
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
            return result;
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
        //==================SelectAll=================
        public List<SettingsVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<SettingsVM> VMs = new List<SettingsVM>();
            SettingsVM vm;
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
                    currConn = _dbsqlConnection.GetConnection();
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
 SettingId
,SettingGroup
,SettingName
,SettingValue
,SettingType
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
FROM Settings
WHERE  1=1
";
                if (Id > 0)
                {
                    sqlText += @" and SettingId=@SettingId";
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
                    objComm.Parameters.AddWithValue("@SettingId", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SettingsVM();
                    vm.SettingId = dr["SettingId"].ToString();
                    vm.SettingGroup = dr["SettingGroup"].ToString();
                    vm.SettingName = dr["SettingName"].ToString();
                    vm.SettingValue = dr["SettingValue"].ToString();
                    vm.SettingType = dr["SettingType"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
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
        //================= Update Single============
        public string[] settingsDataUpdate(SettingsVM vm, SqlConnection VcurrConn=null, SqlTransaction Vtransaction=null)
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
                int nextId = 0;
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
                    currConn = _dbsqlConnection.GetConnection();
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
                    sqlText = "update Settings set";
                    sqlText += " SettingValue=@SettingValue";
                    sqlText += " where SettingGroup=@SettingGroup and  SettingName=@SettingName";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@SettingGroup", vm.SettingGroup.Trim());
                    cmdUpdate.Parameters.AddWithValue("@SettingName", vm.SettingName.Trim());
                    cmdUpdate.Parameters.AddWithValue("@SettingValue", vm.SettingValue.Trim());
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
            #region Catch and Finally
            #region Catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex
                 if (VcurrConn == null){transaction.Rollback();}
                FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
                return retResults;
            }
            #endregion
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
        //==================DropDownAll==========
        public List<SettingsVM> DropDownAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<SettingsVM> VMs = new List<SettingsVM>();
            SettingsVM vm;
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
SELECT distinct SettingGroup from Settings union select 'AllGroup' SettingGroup from Settings
WHERE  1=1
";
                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SettingsVM();
                    vm.SettingGroup = dr["SettingGroup"].ToString(); ;
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
    }
}
