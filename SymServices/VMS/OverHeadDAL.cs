using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymphonySofttech.Utilities;
using SymViewModel.VMS;

namespace SymServices.VMS
{
    public class OverHeadDAL
    {
        #region Global Variables

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region New Methods

        
        public DataTable SearchOverheadForBOMNew(string ActiveStatus)
        {

            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("CompanyOverheads");

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

                sqlText = @"select Productname Headname,RebatePercent RebatePercent,ProductCode OHCode,ItemNo HeadID   
from  Products p LEFT OUTER JOIN
ProductCategories pc ON p.CategoryID=pc.CategoryID 
 WHERE (pc.IsRaw='Overhead') 
and p.ActiveStatus=@ActiveStatus 
                          order by Productname";

                SqlCommand objCommOverhead = new SqlCommand();
                objCommOverhead.Connection = currConn;
                objCommOverhead.CommandText = sqlText;
                objCommOverhead.CommandType = CommandType.Text;

                #region param

                if (!objCommOverhead.Parameters.Contains("@ActiveStatus"))
                { objCommOverhead.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommOverhead.Parameters["@ActiveStatus"].Value = ActiveStatus; }

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommOverhead);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #region catch

            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                throw ex;
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

            return dataTable;
        }

        #endregion

        //public string[] InsertCompanyOverHead(string HeadID, string HeadName, decimal HeadAmount, string Description,
        //    string Comments, string ActiveStatus, string CreatedBy, DateTime CreatedOn, string LastModifiedBy,
        //    DateTime LastModifiedOn, string RebatePercent, string OHCode)
        public string[] InsertCompanyOverHead(CompanyOverheadVM vm)
        {
            #region Objects & Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string ohCode = vm.OHCode;

            #endregion

            #region try
            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.HeadName))
                {
                    throw new ArgumentNullException("Insert To Head Name Information", "Please enter a Head Name name.");
                }

                #endregion Validation

                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "OverHead") == "Y" ? true : false);
                #endregion settingsValue

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToOverheadInformation");

                #endregion open connection and transaction

               

                #region name existence checking

                sqlText = "select count(distinct HeadName) from CompanyOverheads where  HeadName=@vmHeadName ";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;

                cmdNameExist.Parameters.AddWithValue("@vmHeadName", vm.HeadName);

                int countName = (int)cmdNameExist.ExecuteScalar();
                if (countName > 0)
                {
                    throw new ArgumentNullException("InsertToOverheadInformation",
                                                    "Requested Overhead Name and Account number already exists");
                }
                #endregion

                #region new id generation
                //sqlText = "select isnull(max(cast(HeadID as int)),0)+1 FROM  CompanyOverheads";

                sqlText = "select 'ovh'+ltrim(rtrim(str(isnull(max(substring(headid,4,len(headid))),0)+1))) from CompanyOverheads";
                //sqlText = "select isnull(max(cast(ItemNo as int)),0)+1 FROM  Products";

                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                object objNextId = cmdNextId.ExecuteScalar();
                if(objNextId==null)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unable to create new Overhead information Id";
                    throw new ArgumentNullException("InsertToOverheadInformation",
                                                    "Unable to create new Overhead information Id");
                }

                string nextId = objNextId.ToString();
                if (string.IsNullOrEmpty(nextId))
                {
                    

                    //THROGUH EXCEPTION BECAUSE WE COULD NOT FIND EXPECTED NEW ID
                    retResults[0] = "Fail";
                    retResults[1] = "Unable to create new Overhead information Id";
                    throw new ArgumentNullException("InsertToOverheadInformation",
                                                    "Unable to create new Overhead information Id");
                }

                #region Code
                if (Auto == false)
                {
                    if (string.IsNullOrEmpty(ohCode))
                    {
                        throw new ArgumentNullException("InsertToOverheadInformation", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        sqlText = "select count(HeadID) from CompanyOverheads where  OHCode=@ohCode";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;

                        cmdCodeExist.Parameters.AddWithValue("@ohCode", ohCode);

                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("InsertToCustomer", "Same Over Head  Code('" + ohCode + "') already exist");
                        }
                    }
                }
                else
                {
                    ohCode = nextId.ToString();
                }
                #endregion Code

                #endregion

                #region Insert new row to table

                #region sql statement
                sqlText = "";
                sqlText += "insert into CompanyOverheads";
                sqlText += "(";
                sqlText += "HeadID,";
                sqlText += "HeadName,";
                sqlText += "HeadAmount,";
                sqlText += "Description,";
                sqlText += "Comments,";
                sqlText += "ActiveStatus,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "LastModifiedBy,";
                sqlText += "LastModifiedOn,";
                sqlText += "OHCode,";
                
                sqlText += "RebatePercent";
                sqlText += ")";
                sqlText += " values(";
                sqlText += "@nextId,";
                sqlText += "@HeadName,";
                sqlText += "@HeadAmount,";
                sqlText += "@Description,";
                sqlText += "@Comments,";
                sqlText += "@ActiveStatus,";
                sqlText += "@CreatedBy,";
                sqlText += "@CreatedOn,";
                sqlText += "@LastModifiedBy,";
                sqlText += "@LastModifiedOn,";
                sqlText += "@ohCode,";
                sqlText += "@RebatePercent";
                sqlText += ")";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@nextId", nextId);
                cmdInsert.Parameters.AddWithValue("@HeadName", vm.HeadName??Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@HeadAmount", vm.HeadAmount);
                cmdInsert.Parameters.AddWithValue("@Description", vm.Description ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus?"Y":"N");
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@ohCode", ohCode ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@RebatePercent ", vm.RebatePercent);

                transResult = (int)cmdInsert.ExecuteNonQuery();
                #endregion

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Overhead Information successfully Added";
                        retResults[2] = "" + nextId;
                        retResults[3] = "" + ohCode;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to add Overhead";
                        retResults[2] = "";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to add Overhead";
                    retResults[2] = "";
                }
                #endregion Commit

                #endregion
            }
            #endregion try

            #region catch

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw ex;
            }

            #endregion catch

            #region Finally

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


        //public string[] UpdateCompanyOverHead(string HeadID, string HeadName, decimal HeadAmount, string Description, 
        //    string Comments, string ActiveStatus, string LastModifiedBy, DateTime LastModifiedOn, 
        //    string RebatePercent, string OhCode)
        public string[] UpdateCompanyOverHead(CompanyOverheadVM vm)
        {
            #region Objects & Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.HeadID;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string ohCode = vm.OHCode;

            #endregion

            #region try
            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.HeadID))
                {
                    throw new ArgumentNullException("Update Head ID Information", "Invalid Over Head Id.");
                }

                #endregion Validation

                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "OverHead") == "Y" ? true : false);
                #endregion settingsValue

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("OverheadInformationTransaction");

                #endregion open connection and transaction

                #region id existence checking

                sqlText = "select count(HeadID) from CompanyOverheads where  HeadID=@vmHeadID";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;

                cmdIdExist.Parameters.AddWithValue("@vmHeadID", vm.HeadID);

                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("Update Overhead Information", "Could not find requested Overhead information");
                }
                #endregion

                #region name existence checking

                sqlText = "SELECT COUNT(HeadName) FROM CompanyOverheads ";
                sqlText += " where  HeadName =@vmHeadName";
                sqlText += " and  HeadID <> @vmHeadID ";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;

                cmdNameExist.Parameters.AddWithValue("@vmHeadName", vm.HeadName);
                cmdNameExist.Parameters.AddWithValue("@vmHeadID", vm.HeadID);

                countId = (int)cmdNameExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("Update Overhead Information","Same name Overhead Information already exists !");
                }

                #region Code
                if (Auto == false)
                {
                    if (string.IsNullOrEmpty(ohCode))
                    {
                        throw new ArgumentNullException("InsertToCustomer", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        sqlText = "select count(HeadID) from CompanyOverheads where  OHCode=@ohCode and HeadID <>@vmHeadID ";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;

                        cmdCodeExist.Parameters.AddWithValue("@ohCode", ohCode);
                        cmdCodeExist.Parameters.AddWithValue("@vmHeadID", vm.HeadID);


                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("InsertToCustomer", "Same OH  Code('" + ohCode + "') already exist");
                        }
                    }
                }
                else
                {
                    ohCode = vm.HeadID;
                }
                #endregion Code
                
                #endregion


                #region Update Overhead Information

                #region sql statement
                sqlText = "";
                sqlText = "update CompanyOverheads set";
                sqlText += " HeadName       =@HeadName,";
                sqlText += " HeadAmount     =@HeadAmount,";
                sqlText += " Description    =@Description,";
                sqlText += " Comments       =@Comments,";
                sqlText += " ActiveStatus   =@ActiveStatus,";
                sqlText += " LastModifiedBy =@LastModifiedBy,";
                sqlText += " LastModifiedOn =@LastModifiedOn,";
                sqlText += " OHCode         =@ohCode,";
                sqlText += " RebatePercent  =@RebatePercent ";
                sqlText += " where HeadID   =@HeadID";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@HeadAmount", vm.HeadAmount);
                cmdUpdate.Parameters.AddWithValue("@Description", vm.Description);
                cmdUpdate.Parameters.AddWithValue("@Comments", vm.Comments);
                cmdUpdate.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus ? "Y" : "N");
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@ohCode", ohCode);
                cmdUpdate.Parameters.AddWithValue("@RebatePercent ", vm.RebatePercent);
                cmdUpdate.Parameters.AddWithValue("@HeadName", vm.HeadName);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #endregion

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Overhead Information successfully Updated";
                        retResults[2] = vm.HeadID;
                        retResults[3] = ohCode;
                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to Update Overhead";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to Update Overhead";
                }

                #endregion Commit

                #endregion

            }
            #endregion try

            #region catch

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw ex;
            }

            #endregion catch

            #region Finally

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

        public string[] DeleteOverHeadInformation(string HeadID, string databaseName)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = HeadID;

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(HeadID))
                {
                    throw new ArgumentNullException("DeleteOverheadInformation",
                                "Could not find requested Overhead Id.");
                }
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                sqlText = "select count(HeadID) from CompanyOverheads where HeadID=@HeadID ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValue("@HeadID", HeadID);

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested Overhead Information.";
                    return retResults;
                }

                sqlText = "delete CompanyOverheads where HeadID@HeadID";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                cmdDelete.Parameters.AddWithValue("HeadID", HeadID);
                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested Overhead Information successfully deleted";
                }


            }
            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                throw ex;
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

            return retResults;
        }

        public DataTable SearchOverheadNew(string HeadID, string HeadName, string ActiveStatus, string databaseName)
        {



            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";


            DataTable dataTable = new DataTable("Overhead");
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction



                sqlText = @"
SELECT 
HeadID,
HeadName,
HeadAmount,
Description,
Comments,
ActiveStatus,
RebatePercent,
isnull(OHCode,'N/A')OHCode

FROM dbo.CompanyOverheads
                 
WHERE 
    (HeadID  LIKE '%' +  @HeadID + '%' OR @HeadID IS NULL) 
AND (HeadName LIKE '%' + @HeadName + '%' OR @HeadName IS NULL)
AND (ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
order by HeadName
";
                SqlCommand objCommOverhead = new SqlCommand();
                objCommOverhead.Connection = currConn;
                objCommOverhead.CommandText = sqlText;
                objCommOverhead.CommandType = CommandType.Text;

                if (!objCommOverhead.Parameters.Contains("@HeadID"))
                { objCommOverhead.Parameters.AddWithValue("@HeadID", HeadID); }
                else { objCommOverhead.Parameters["@HeadID"].Value = HeadID; }
                if (!objCommOverhead.Parameters.Contains("@HeadName"))
                { objCommOverhead.Parameters.AddWithValue("@HeadName", HeadName); }
                else { objCommOverhead.Parameters["@HeadName"].Value = HeadName; }
                if (!objCommOverhead.Parameters.Contains("@ActiveStatus"))
                { objCommOverhead.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommOverhead.Parameters["@ActiveStatus"].Value = ActiveStatus; }
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommOverhead);

                dataAdapter.Fill(dataTable);
            }
            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                throw ex;
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

            return dataTable;

        }

    }
}
