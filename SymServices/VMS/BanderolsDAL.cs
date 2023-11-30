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
    public class BanderolsDAL
    {

        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        //==================Insert Banderol=================

        //public string[] InsertToBanderol(string BanderolID, string BanderolName, string BanderolSize, string Uom,  string Description, string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn)
        public string[] InsertToBanderol(BanderolVM vm)
        
        {

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[1] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.BanderolName))
                {
                    throw new ArgumentNullException("InsertToBanderolInformation",
                                                    "Please enter Banderol name.");
                }


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToBanderolInformation");

                #endregion open connection and transaction

                if (!string.IsNullOrEmpty(vm.BanderolID))
                {
                    sqlText = "";
                    sqlText = "select count(BanderolID) from Banderols where  BanderolID=@BanderolID";
                    sqlText += " and BanderolName=@BanderolName";
                    sqlText += " and BanderolSize=@BanderolSize";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@BanderolID", vm.BanderolID);
                    cmdIdExist.Parameters.AddWithValue("@BanderolName", vm.BanderolName);
                    cmdIdExist.Parameters.AddWithValue("@BanderolSize", vm.BanderolSize);

                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId > 0)
                    {
                        throw new ArgumentNullException("InsertToBanderolInformation", "Banderol information already exist");
                    }

                }

                #region Insert Banderol Information

                sqlText = "select count(distinct BanderolName) from Banderols where  BanderolName=@BanderolName and BanderolSize=@BanderolSize";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;
                cmdNameExist.Parameters.AddWithValue("@BanderolName", vm.BanderolName);
                cmdNameExist.Parameters.AddWithValue("@BanderolSize", vm.BanderolSize);

                int countName = (int)cmdNameExist.ExecuteScalar();
                if (countName > 0)
                {

                    throw new ArgumentNullException("InsertToBanderolInformation",
                                                    "Requested Banderol Name  is already exist");
                }

                sqlText = "select isnull(max(cast(BanderolID as int)),0)+1 FROM  Banderols";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                int nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    //THROGUH EXCEPTION BECAUSE WE COULD NOT FIND EXPECTED NEW ID
                    retResults[0] = "Fail";
                    retResults[1] = "Unable to create new Banderol information Id";
                    throw new ArgumentNullException("InsertToBanderolInformation",
                                                    "Unable to create new Banderol information Id");
                }

                vm.BanderolID = nextId.ToString();

                sqlText = "";
                sqlText += "insert into Banderols";
                sqlText += "(";
                sqlText += "BanderolID,";
                sqlText += "BanderolName,";
                sqlText += "BanderolSize,";
                sqlText += "UOM,"; 
                sqlText += "Description,";
                sqlText += "ActiveStatus,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "LastModifiedBy,";
                sqlText += "LastModifiedOn";
                sqlText += ")";
                sqlText += " values(";

                sqlText += " BanderolID     =@BanderolID";
                sqlText += ",BanderolName   =@BanderolName";
                sqlText += ",BanderolSize   =@BanderolSize";
                sqlText += ",UOM            =@UOM";
                sqlText += ",OpeningQty     =@OpeningQty";
                sqlText += ",Description    =@Description";
                sqlText += ",ActiveStatus   =@ActiveStatus";
                sqlText += ",CreatedBy      =@CreatedBy";
                sqlText += ",CreatedOn      =@CreatedOn";
                sqlText += ",LastModifiedBy =@LastModifiedBy";
                sqlText += ",LastModifiedOn =@LastModifiedOn";


                sqlText += ")";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValue("@BanderolID", vm.BanderolID);
                cmdInsert.Parameters.AddWithValue("@BanderolName", vm.BanderolName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BanderolSize", vm.BanderolSize ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@UOM", vm.UOM ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@OpeningQty", vm.OpeningQty);
                cmdInsert.Parameters.AddWithValue("@Description", vm.Description ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus ? "Y" : "N");
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);


                transResult = (int)cmdInsert.ExecuteNonQuery();

                #endregion Insert Currency Information


                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();

                    }

                }

                #endregion Commit

                retResults[0] = "Success";
                retResults[1] = "Requested Banderol Information successfully added";
                retResults[2] = "" + nextId;
            }
            #region Catch
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


        //public string[] UpdateBanderol(string BanderolID, string BanderolName, string BanderolSize, string Uom, string Description, string ActiveStatus, string LastModifiedBy, string LastModifiedOn)
        public string[] UpdateBanderol(BanderolVM vm)
        {

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[1] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.BanderolName))
                {
                    throw new ArgumentNullException("UpdateBanderolInformation",
                                                    "Please enter Banderol name.");
                }


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction("UpdateBanderolInformation");

                #endregion open connection and transaction

                if (!string.IsNullOrEmpty(vm.BanderolID))
                {
                    sqlText = "";
                    sqlText = "select count(BanderolID) from Banderols where  BanderolID=@BanderolID";
                    sqlText += " and BanderolName=@BanderolName";
                    sqlText += " and BanderolSize=@BanderolSize";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@BanderolID", vm.BanderolID);
                    cmdIdExist.Parameters.AddWithValue("@BanderolName", vm.BanderolName);
                    cmdIdExist.Parameters.AddWithValue("@BanderolSize", vm.BanderolSize);

                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("UpdateBanderolInformation", "Could not find requested Banderol information ");
                    }

                }

                #region Update Banderol Information

                sqlText = "";
                sqlText += "UPDATE Banderols SET ";
                sqlText += " BanderolName   =@BanderolName";
                sqlText += ",BanderolSize   =@BanderolSize";
                sqlText += ",UOM            =@UOM";
                sqlText += ",Description    =@Description";
                sqlText += ",ActiveStatus   =@ActiveStatus";
                sqlText += ",LastModifiedBy =@LastModifiedBy";
                sqlText += ",LastModifiedOn =@LastModifiedOn";

                sqlText += " where BanderolID=@BanderolID";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValue("@BanderolName", vm.BanderolName??Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BanderolSize", vm.BanderolSize ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@UOM", vm.UOM ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Description", vm.Description ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus?"Y":"N");
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);


                transResult = (int)cmdInsert.ExecuteNonQuery();

                #endregion Update Currency Information


                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Banderol Information successfully Updated";
                        retResults[2] = "" + vm.BanderolID;

                    }

                }

                #endregion Commit
            }
            #region Catch
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

        public string[] DeleteBanderolInformation(string BanderolID)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = BanderolID;

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(BanderolID.ToString()))
                {
                    throw new ArgumentNullException("DeleteBanderolInformation",
                                "Could not find requested Banderol .");
                }
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction

                sqlText = "select count(BanderolID) from Banderols where BanderolID=@BanderolID";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValue("@BanderolID", BanderolID);
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested Banderol Information.";
                    return retResults;
                }

                sqlText = "delete Banderols where BanderolID=@BanderolID";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                cmdDelete.Parameters.AddWithValue("@BanderolID", BanderolID);

                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested Banderol Information successfully deleted";
                }


            }
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

        public DataTable SearchBanderols(string BanderolName,string BandeSize, string OpeningDateFrom, string OpeningDateTo, string ActiveStatus)
        {
            string[] retResults = new string[2];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Banderols");

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
                           SELECT isnull(NULLIF(b.BanderolID,''),0)BanderolID, 
                            isnull(NULLIF(b.BanderolName,''),'')BanderolName,
                            isnull(NULLIF(b.BanderolSize,''),'')BanderolSize,
                            isnull(NULLIF(b.UOM,''),'')UOM,
                            -- isnull(NULLIF(b.OpeningQty,0),0)OpeningQty,
                           -- convert (varchar,b.OpeningDate,120)OpeningDate,
                            isnull(NULLIF(b.Description,''),'')Description,
                            isnull(NULLIF(b.ActiveStatus,''),'')ActiveStatus
                            FROM Banderols b
                 
                            WHERE 
                                (BanderolName  LIKE '%' +  @BanderolName  + '%' OR @BanderolName IS NULL) 
                             AND (BanderolSize  LIKE '%' +  @BandeSize  + '%' OR @BandeSize IS NULL) 
                            AND (b.ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
order by BanderolID
                            ";
                SqlCommand objCommBanderol = new SqlCommand();
                objCommBanderol.Connection = currConn;

                objCommBanderol.CommandText = sqlText;
                objCommBanderol.CommandType = CommandType.Text;


                if (!objCommBanderol.Parameters.Contains("@BanderolName"))
                { objCommBanderol.Parameters.AddWithValue("@BanderolName", BanderolName); }
                else { objCommBanderol.Parameters["@BanderolName"].Value = BanderolName; }

                if (!objCommBanderol.Parameters.Contains("@BandeSize"))
                { objCommBanderol.Parameters.AddWithValue("@BandeSize", BandeSize); }
                else { objCommBanderol.Parameters["@BandeSize"].Value = BandeSize; }

                if (OpeningDateFrom == "")
                {
                    if (!objCommBanderol.Parameters.Contains("@OpeningDateFrom"))
                    { objCommBanderol.Parameters.AddWithValue("@OpeningDateFrom", System.DBNull.Value); }
                    else { objCommBanderol.Parameters["@OpeningDateFrom"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommBanderol.Parameters.Contains("@OpeningDateFrom"))
                    { objCommBanderol.Parameters.AddWithValue("@OpeningDateFrom", OpeningDateFrom); }
                    else { objCommBanderol.Parameters["@OpeningDateFrom"].Value = OpeningDateFrom; }
                }

                if (OpeningDateTo == "")
                {
                    if (!objCommBanderol.Parameters.Contains("@OpeningDateTo"))
                    { objCommBanderol.Parameters.AddWithValue("@OpeningDateTo", System.DBNull.Value); }
                    else { objCommBanderol.Parameters["@OpeningDateTo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommBanderol.Parameters.Contains("@OpeningDateTo"))
                    { objCommBanderol.Parameters.AddWithValue("@OpeningDateTo", OpeningDateTo); }
                    else { objCommBanderol.Parameters["@OpeningDateTo"].Value = OpeningDateTo; }
                }

                if (!objCommBanderol.Parameters.Contains("@ActiveStatus"))
                { objCommBanderol.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommBanderol.Parameters["@ActiveStatus"].Value = ActiveStatus; }
                // Common Filed
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommBanderol);

                dataAdapter.Fill(dataTable);
            }
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
