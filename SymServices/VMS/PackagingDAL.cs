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
    public class PackagingDAL
    {

        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        public string[] InsertToPackage(string PackID, string PackName, string PackSize, string Uom, string Description, string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn)
        //public string[] InsertToPackage(PackagingInformationVM vm)
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

                if (string.IsNullOrEmpty(PackName))
                {
                    throw new ArgumentNullException("InsertToPackagingInformation","Please enter Nature of Package.");
                }


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToPackagingInformation");

                #endregion open connection and transaction

                if (!string.IsNullOrEmpty(PackID))
                {
                    sqlText = "";
                    sqlText = "select count(PackagingID) from PackagingInformations where  PackagingID=@PackID ";
                    sqlText += " and PackagingNature=@PackName ";
                    sqlText += " and PackagingCapacity=@PackSize";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@PackID", PackID);
                    cmdIdExist.Parameters.AddWithValue("@PackName", PackName);
                    cmdIdExist.Parameters.AddWithValue("@PackSize", PackSize);


                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId > 0)
                    {
                        throw new ArgumentNullException("InsertToPackagingInformation", "Packaging information already exist");
                    }

                }

                #region Insert Packaging Information

                sqlText = "select count(distinct PackagingNature) from PackagingInformations where  PackagingNature=@PackName  and PackagingCapacity=@PackSize";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;
                cmdNameExist.Parameters.AddWithValue("@PackName", PackName);
                cmdNameExist.Parameters.AddWithValue("@PackSize", PackSize);

                int countName = (int)cmdNameExist.ExecuteScalar();
                if (countName > 0)
                {
                    throw new ArgumentNullException("InsertToPackagingInformation","Requested package Name is already exist");
                }

                sqlText = "select isnull(max(cast(PackagingID as int)),0)+1 FROM  PackagingInformations";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                int nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    //THROGUH EXCEPTION BECAUSE WE COULD NOT FIND EXPECTED NEW ID
                    retResults[0] = "Fail";
                    retResults[1] = "Unable to create new Packaging information Id";
                    throw new ArgumentNullException("InsertToPackagingInformation","Unable to create new Packaging information Id");
                }


                sqlText = "";
                sqlText += "insert into PackagingInformations";
                sqlText += "(";
                sqlText += "PackagingID,";
                sqlText += "PackagingNature,";
                sqlText += "PackagingCapacity,";
                sqlText += "UOM,";
                sqlText += "Description,";
                sqlText += "ActiveStatus,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "LastModifiedBy,";
                sqlText += "LastModifiedOn";
                sqlText += ")";
                sqlText += " values(";
                sqlText += "@nextId ,";
                sqlText += "@PackName ,";
                sqlText += "@PackSize ,";
                sqlText += "@Uom ,";
                sqlText += "@Description ,";
                sqlText += "@ActiveStatus ,";
                sqlText += "@CreatedBy ,";
                sqlText += "@CreatedOn ,";
                sqlText += "@LastModifiedBy ,";
                sqlText += "@LastModifiedOn ";
                sqlText += ")";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@nextId", nextId);
                cmdInsert.Parameters.AddWithValue("@PackName", PackName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@PackSize", PackSize ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Uom", Uom ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Description", Description ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", ActiveStatus ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", CreatedOn);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", LastModifiedOn); 

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
                retResults[1] = "Requested Packaging Information successfully added";
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
        
        public string[] UpdatePackage(string PackID, string PackName, string PackSize, string Uom, string Description, string ActiveStatus,string LastModifiedBy, string LastModifiedOn)
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

                if (string.IsNullOrEmpty(PackName))
                {
                    throw new ArgumentNullException("UpdatePackage", "Please enter Package name.");
                }
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UpdatePackage");

                #endregion open connection and transaction

                if (!string.IsNullOrEmpty(PackID))
                {
                    sqlText = "";
                    sqlText = "select count(PackagingID) from PackagingInformations where  PackagingID=@PackID";
                    sqlText += " and PackagingNature=@PackName and PackagingCapacity=@PackSize ";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@PackID", PackID);
                    cmdIdExist.Parameters.AddWithValue("@PackName", PackName);
                    cmdIdExist.Parameters.AddWithValue("@PackSize", PackSize);

                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("UpdatePackage", "Could not find requested Packaging information ");
                    }

                }

                #region Update Packaging Information

                sqlText = "";
                sqlText += "UPDATE PackagingInformations SET ";
                sqlText += " PackagingNature    =@PackName,";
                sqlText += " PackagingCapacity  =@PackSize,";
                sqlText += " UOM                =@Uom,";
                sqlText += " Description        =@Description,";
                sqlText += " ActiveStatus       =@ActiveStatus,";
                sqlText += " LastModifiedBy     =@LastModifiedBy,";
                sqlText += " LastModifiedOn     =@LastModifiedOn";
                sqlText += " where PackagingID  =@PackID";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@PackName", PackName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@PackSize", PackSize ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Uom", Uom ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Description", Description ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", ActiveStatus ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@PackID", PackID ?? Convert.DBNull);

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #endregion Update Packaging Information


                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Packaging Information successfully Updated";

                    }

                }

                #endregion Commit
            }
            #region Catch
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {

                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
             
        public string[] DeletePackageInformation(string PackId)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            //retResults[2] = PackId.ToString();
            retResults[2] = PackId;

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(PackId.ToString()))
                {
                    throw new ArgumentNullException("DeletePackageInformation", "Could not find requested Package Id.");
                }
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction

                sqlText = "select count(PackagingID) from PackagingInformations where PackagingID=@PackId";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValue("@PackId", PackId);

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested Package Information.";
                    return retResults;
                }

                sqlText = "delete PackagingInformations where PackagingID=@PackId";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                cmdDelete.Parameters.AddWithValue("@PackId", PackId);

                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested Package Information successfully deleted";
                }
            }
            catch (SqlException sqlex)
            {
              throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
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

            return retResults;
        }

        public DataTable SearchPackage(string PackName,string PackgeSize,string ActiveStatus)
        {
            string[] retResults = new string[2];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Pacakges");
            
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
                            SELECT isnull(NULLIF(p.PackagingID,''),0)PackagingID, 
                            isnull(NULLIF(p.PackagingNature,''),'')PackNature,
                            isnull(NULLIF(p.PackagingCapacity,''),'')PackCapacity,
                            isnull(NULLIF(p.UOM,''),'')UOM,
                            isnull(NULLIF(p.Description,''),'')Description,
                            isnull(NULLIF(p.ActiveStatus,''),'')ActiveStatus
                            FROM PackagingInformations p
                 
                            WHERE 
                                (p.PackagingNature  LIKE '%' +  @PackName  + '%' OR @PackName IS NULL) 
                            AND (p.PackagingCapacity  LIKE '%' +  @PackgeSize  + '%' OR @PackgeSize IS NULL) 
                            AND (p.ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
                            order by p.PackagingID 
                            ";
                SqlCommand objCommPackage = new SqlCommand();
                objCommPackage.Connection = currConn;

                objCommPackage.CommandText = sqlText;
                objCommPackage.CommandType = CommandType.Text;


                if (!objCommPackage.Parameters.Contains("@PackName"))
                { objCommPackage.Parameters.AddWithValue("@PackName", PackName); }
                else { objCommPackage.Parameters["@PackName"].Value = PackName; }

                if (!objCommPackage.Parameters.Contains("@PackgeSize"))
                { objCommPackage.Parameters.AddWithValue("@PackgeSize", PackgeSize); }
                else { objCommPackage.Parameters["@PackgeSize"].Value = PackgeSize; }

                if (!objCommPackage.Parameters.Contains("@ActiveStatus"))
                { objCommPackage.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommPackage.Parameters["@ActiveStatus"].Value = ActiveStatus; }
                // Common Filed
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommPackage);

                dataAdapter.Fill(dataTable);
            }
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
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

            return dataTable;

        }
    }
}
