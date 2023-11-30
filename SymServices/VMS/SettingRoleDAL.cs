using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymViewModel.VMS;

namespace SymServices.VMS
{
    public class SettingRoleDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        #endregion

        #region Methods

       
        public DataSet SearchSettingsRole()
        {

            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            //DataSet dataTable = new DataTable("Search Settings");
            DataSet dataSet = new DataSet("SearchSettingsRole");
            SqlTransaction transaction = null;

            #endregion

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                //transaction = currConn.BeginTransaction("InsertSettingsRoll");

                #endregion open connection and transaction

                #region sql statement search from settings

                sqlText = @" Select * from Settings
                                 ORDER BY SettingGroup,SettingName;
";

                SqlCommand cmdSettingRole = new SqlCommand();
                cmdSettingRole.Connection = currConn;
                cmdSettingRole.CommandText = sqlText;
                cmdSettingRole.CommandType = CommandType.Text;
                DataTable dt = new DataTable("Search Settings");
                
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmdSettingRole);
                dataAdapter.Fill(dt);
                foreach (DataRow item in dt.Rows)
                {

                    sqlText = "  ";
                    sqlText += " SELECT COUNT(DISTINCT SettingId)SettingId FROM SettingsRole ";
                    sqlText += " WHERE SettingGroup=@itemSettingGroup AND SettingName=@itemSettingName ";
                    sqlText += " AND SettingType=@itemSettingType AND UserId='" + UserInfoVM.UserId + "'";
                    SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                    cmdExist.Transaction = transaction;
                    cmdExist.Parameters.AddWithValue("@itemSettingGroup", item["SettingGroup"].ToString());
                    cmdExist.Parameters.AddWithValue("@itemSettingName", item["SettingName"].ToString());
                    cmdExist.Parameters.AddWithValue("@itemSettingType", item["SettingType"].ToString());


                    object objfoundId = cmdExist.ExecuteScalar();
                    if (objfoundId == null)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                    }
                    int found = (int) objfoundId;
                    if (found<=0)// not exist
                    {
                        sqlText = "  ";
                        sqlText +=
                            " INSERT INTO SettingsRole(	SettingGroup,SettingName,SettingValue,SettingType,UserID,ActiveStatus,CreatedBy,CreatedOn,LastModifiedBy,LastModifiedOn)";
                        sqlText += " VALUES(";
                        sqlText += "@itemSettingGroup,";
                        sqlText += "@itemSettingName,";
                        sqlText += "@itemSettingValue,";
                        sqlText += "@itemSettingType,";
                        sqlText += " '" + UserInfoVM.UserId + "',";
                        sqlText += " 'Y',";
                        sqlText += " '" + UserInfoVM.UserName + "',";
                        sqlText += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                        sqlText += " '" + UserInfoVM.UserName + "',";
                        sqlText += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                        sqlText += " )";

                        SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                        cmdExist1.Transaction = transaction;
                        cmdExist1.Parameters.AddWithValue("@itemSettingGroup", item["SettingGroup"].ToString());
                        cmdExist1.Parameters.AddWithValue("@itemSettingName", item["SettingName"].ToString());
                        cmdExist1.Parameters.AddWithValue("@itemSettingValue", item["SettingValue"].ToString());
                        cmdExist1.Parameters.AddWithValue("@itemSettingType", item["SettingType"].ToString());
                        object objfoundId1 = cmdExist1.ExecuteNonQuery();
                        if (objfoundId1 == null)
                        {
                            throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                        }
                        transResult = (int)objfoundId1;
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                        }
                    }
                   

                }
               
                sqlText = @"SELECT [SettingId]
                                      ,[SettingGroup]
                                      ,[SettingName]
                                      ,[SettingValue]
                                      ,[SettingType]
                                      ,[ActiveStatus]
                                      FROM SettingsRole where UserID=@userId
                                     ORDER BY SettingGroup,SettingName;
SELECT DISTINCT s.SettingGroup FROM SettingsRole s ORDER BY s.SettingGroup;
";

                SqlCommand cmdSettingRole1 = new SqlCommand();
                cmdSettingRole1.Connection = currConn;
                cmdSettingRole1.CommandText = sqlText;
                cmdSettingRole1.CommandType = CommandType.Text;

                if (!cmdSettingRole1.Parameters.Contains("@userId"))
                { cmdSettingRole1.Parameters.AddWithValue("@userId", UserInfoVM.UserId); }
                else { cmdSettingRole1.Parameters["@userId"].Value = UserInfoVM.UserId; }



                SqlDataAdapter dataAdapter1 = new SqlDataAdapter(cmdSettingRole1);
                dataAdapter1.Fill(dataSet);

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
                       #region SettingsExist
                        sqlText = "  ";
                        sqlText += " SELECT COUNT(DISTINCT SettingId)SettingId FROM SettingsRole ";
                        sqlText += " WHERE SettingGroup=@itemSettingGroup AND SettingName=@itemSettingName AND SettingType=@itemSettingType AND UserId='" + UserInfoVM.UserId + "'";
                        SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                        cmdExist.Transaction = transaction;
                        cmdExist.Parameters.AddWithValue("@itemSettingGroup", item.SettingGroup);
                        cmdExist.Parameters.AddWithValue("@itemSettingName", item.SettingName);
                        cmdExist.Parameters.AddWithValue("@itemSettingType", item.SettingType);


                        object objfoundId = cmdExist.ExecuteScalar();
                        if (objfoundId == null)
                        {
                            throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                        }
                        #endregion SettingsExist


                        #region Last Settings

                        int foundId = (int)objfoundId;
                        if (foundId <= 0)
                        {
                            sqlText = "  ";
                            sqlText +=
                                " INSERT INTO SettingsRole(	SettingGroup,SettingName,SettingValue,SettingType,UserID,ActiveStatus,CreatedBy,CreatedOn,LastModifiedBy,LastModifiedOn)";
                            sqlText += " VALUES(";
                            sqlText += "@itemSettingGroup,";
                            sqlText += "@itemSettingName,";
                            sqlText += "@itemSettingValue,";
                            sqlText += "@itemSettingType,";
                            sqlText += " '" + UserInfoVM.UserId + "',";
                            sqlText += " 'Y',";
                            sqlText += " '" + UserInfoVM.UserName + "',";
                            sqlText += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                            sqlText += " '" + UserInfoVM.UserName + "',";
                            sqlText += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                            sqlText += " )";

                            SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                            cmdExist1.Transaction = transaction;
                            cmdExist1.Parameters.AddWithValue("@itemSettingGroup", item.SettingGroup);
                            cmdExist1.Parameters.AddWithValue("@itemSettingName", item.SettingName);
                            cmdExist1.Parameters.AddWithValue("@itemSettingValue", item.SettingValue);
                            cmdExist1.Parameters.AddWithValue("@itemSettingType", item.SettingType);

                            object objfoundId1 = cmdExist1.ExecuteNonQuery();
                            if (objfoundId1 == null)
                            {
                                throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                            }
                            transResult = (int) objfoundId1;
                        }
                            #endregion Last Price

                        else
                        {
                            #region Update Settings

                            sqlText = "";
                            sqlText += "update SettingsRole set";
                            sqlText += " SettingValue=@itemSettingValue,";
                            sqlText += " ActiveStatus=@itemActiveStatus";
                            //sqlText += " LastModifiedBy='" + item.LastModifiedBy + "',";
                            //sqlText += " LastModifiedOn='" + item.LastModifiedOn + "'";
                            sqlText += " where SettingGroup=@itemSettingGroup  and SettingName=@itemSettingName AND UserId='" + UserInfoVM.UserId + "'";

                            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                            cmdUpdate.Transaction = transaction;
                            cmdUpdate.Parameters.AddWithValue("@itemSettingValue", item.SettingValue ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@itemActiveStatus", item.ActiveStatus ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@itemSettingGroup", item.SettingGroup ?? Convert.DBNull);
                            cmdUpdate.Parameters.AddWithValue("@itemSettingName", item.SettingName ?? Convert.DBNull);

                            transResult = (int)cmdUpdate.ExecuteNonQuery();
                            #endregion Update Settings

                        }
                       

                        #region Commit

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("SettingsUpdate", item.SettingName + " could not updated.");
                        }

                        #endregion Commit

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
                        cmdInsert.Parameters.AddWithValue("@vUOMPrice1", vUOMPrice1);
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
                        sqlText += " where (IssueHeaders.IssueNo= @vIssueNo)";

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

        public string settingsDataInsert(string settingGroup, string settingName, string settingType, string settingValue,string userId,
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
                else if (string.IsNullOrEmpty(userId))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Select user");
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
                sqlText += " SELECT COUNT(DISTINCT SettingId)SettingId FROM SettingsRole ";
                sqlText += " WHERE SettingGroup=@settingGroup AND SettingName=@settingName AND SettingType=@settingType AND UserId=@userId";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@settingGroup", settingGroup);
                cmdExist.Parameters.AddWithValue("@settingName", settingName);
                cmdExist.Parameters.AddWithValue("@settingType", settingType);
                cmdExist.Parameters.AddWithValue("@userId", userId);

                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                #endregion ProductExist
                #region Last Settings

                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = "  ";
                    sqlText += " INSERT INTO SettingsRole(	UserID,SettingGroup,	SettingName,SettingValue,SettingType,ActiveStatus,CreatedBy,CreatedOn,LastModifiedBy,LastModifiedOn)";
                    sqlText += " VALUES(";
                    sqlText += "@userId,";
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
                    cmdExist1.Parameters.AddWithValue("@userId", userId);
                    cmdExist1.Parameters.AddWithValue("@settingGroup", settingGroup ?? Convert.DBNull);
                    cmdExist1.Parameters.AddWithValue("@settingName", settingName ?? Convert.DBNull);
                    cmdExist1.Parameters.AddWithValue("@settingValue", settingValue ?? Convert.DBNull);
                    cmdExist1.Parameters.AddWithValue("@settingType", settingType ?? Convert.DBNull);

                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                    }
                    int save = (int) objfoundId1;
                    if (save<=0)
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
                cmdExist.Parameters.AddWithValue("@settingGroup", settingGroup);
                cmdExist.Parameters.AddWithValue("@settingName", settingName);

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
                    sqlText += " AND SettingName=@settingName ";
                    
                   
                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    cmdExist1.Parameters.AddWithValue("@settingGroup", settingGroup ?? Convert.DBNull);
                    cmdExist1.Parameters.AddWithValue("@settingName", settingName ?? Convert.DBNull);

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
                sqlText += " WHERE SettingGroup=@settingGroup AND SettingName=@settingName ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@settingGroup", settingGroup);
                cmdExist.Parameters.AddWithValue("@settingName", settingName);

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
                    cmdUpdate.Parameters.AddWithValue("@settingGroup", settingGroup);
                    cmdUpdate.Parameters.AddWithValue("@settingName", settingName);

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

    }
}
