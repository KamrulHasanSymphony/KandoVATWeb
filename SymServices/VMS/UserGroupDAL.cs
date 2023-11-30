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
   public class UserGroupDAL
    {
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

              #region User Group Table-By Rupon

        //public string[] InsertToUserGroup(string GroupID,string GroupName, string Comments, string ActiveStatus, string CreatedBy, DateTime CreatedOn, string LastModifiedBy, DateTime LastModifiedOn, string databaseName)
        public string[] InsertToUserGroup(UserGroupVM vm)
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

               
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToUserGroup");
              

            

                #endregion open connection and transaction

                if (!string.IsNullOrEmpty(vm.GroupID))
                {


                    sqlText = "select count(GroupID) from UserGroups where  GroupID=@GroupID";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@GroupID", vm.GroupID);

                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId > 0)
                    {
                        throw new ArgumentNullException("InsertToUserGroup", "User Group already exist");
                    }

                }

                #region Insert User Group

                sqlText = "select count(distinct GroupName) from UserGroups where  GroupName=@GroupName";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;
                cmdNameExist.Parameters.AddWithValue("@GroupName", vm.GroupName);

                int countName = (int)cmdNameExist.ExecuteScalar();
                if (countName > 0)
                {

                    throw new ArgumentNullException("InsertToUserGroup",
                                                    "Requested Group Name  is already exist");
                }

                sqlText = "select isnull(max(cast(GroupID as int)),0)+1 FROM  UserGroups";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                int nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    //THROGUH EXCEPTION BECAUSE WE COULD NOT FIND EXPECTED NEW ID
                    retResults[0] = "Fail";
                    retResults[1] = "Unable to create new User Group Id";
                    throw new ArgumentNullException("InsertToUserGroups",
                                                    "Unable to create new User Group Id");
                }

                sqlText = "";
                sqlText += "insert into UserGroups";
                sqlText += "(";
                sqlText += "GroupID,";
                sqlText += "GroupName,";
                sqlText += "Comments,";
                sqlText += "ActiveStatus,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "LastModifiedBy,";
                sqlText += "LastModifiedOn";
                sqlText += ")";
                sqlText += " values(";
                sqlText += "@nextId,";
                sqlText += "@GroupName,";
                sqlText += "@Comments,";
                sqlText += "@ActiveStatus,";
                sqlText += "@CreatedBy,";
                sqlText += "@CreatedOn,";
                sqlText += "@LastModifiedBy,";
                sqlText += "@LastModifiedOn";
                sqlText += ")";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@nextId", nextId);
                cmdInsert.Parameters.AddWithValue("@GroupName", vm.GroupName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus?"Y":"N");
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
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
                retResults[1] = "Requested User Group successfully added";
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

        //public string[] UpdateUserGroup(string GroupID, string GroupName, string Comments, string ActiveStatus, string UpdatedBy, DateTime CreatedOn, string LastModifiedBy, DateTime LastModifiedOn, string databaseName)
        public string[] UpdateUserGroup(UserGroupVM vm)
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


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UpdateUserGroup");




                #endregion open connection and transaction

                //if (!string.IsNullOrEmpty(GroupID))
                //{


                //    sqlText = "select count(GroupID) from UserGroups where  GroupID='" + GroupID + "'";
                //    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                //    cmdIdExist.Transaction = transaction;
                //    countId = (int)cmdIdExist.ExecuteScalar();
                //    if (countId > 0)
                //    {
                //        throw new ArgumentNullException("UpdateUserGroup", "User Group already exist");
                //    }

                //}

                #region Update User Group

                //sqlText = "select count(distinct GroupName) from UserGroups where  GroupName='" + GroupName + "'";
                //SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                //cmdNameExist.Transaction = transaction;
                //int countName = (int)cmdNameExist.ExecuteScalar();
                //if (countName > 0)
                //{

                //    throw new ArgumentNullException("InsertToUserGroup",
                //                                    "Requested Group Name  is already exist");
                //}

                //sqlText = "select isnull(max(cast(GroupID as int)),0)+1 FROM  UserGroups";
                //SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                //cmdNextId.Transaction = transaction;
                //int nextId = (int)cmdNextId.ExecuteScalar();
                //if (nextId <= 0)
                //{
                //    //THROGUH EXCEPTION BECAUSE WE COULD NOT FIND EXPECTED NEW ID
                //    retResults[0] = "Fail";
                //    retResults[1] = "Unable to create new User Group Id";
                //    throw new ArgumentNullException("InsertToUserGroups",
                //                                    "Unable to create new User Group Id");
                //}
                sqlText = "";
                sqlText += " UPDATE UserGroups SET";
                sqlText += " GroupName      =@GroupName,";
                sqlText += " Comments       =@Comments,";
                sqlText += " ActiveStatus   =@ActiveStatus,";
                sqlText += " LastModifiedBy =@LastModifiedBy,";
                sqlText += " LastModifiedOn =@LastModifiedOn";
                sqlText += " where GroupID  =@GroupID";
               
              
              
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@GroupName", vm.GroupName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus?"Y":"N");
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@GroupID", vm.GroupID ?? Convert.DBNull);

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #endregion Update User Group


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
                retResults[1] = "Requested User Group successfully Updated";
                //retResults[2] = "" + nextId;
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

        public string[] DeleteUserGroup(string GroupID, string databaseName)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = GroupID;

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(GroupID))
                {
                    throw new ArgumentNullException("DeleteUserGroup",
                                "Could not find requested User Groupo UD.");
                }
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                sqlText = "select count(GroupID) from UserGroups where GroupID=@GroupID ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValue("@GroupID", GroupID);

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested User Group.";
                    return retResults;
                }

                sqlText = "delete UserGroups where GroupID=@GroupID";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                cmdDelete.Parameters.AddWithValue("@GroupID", GroupID);

                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested User Group Information successfully deleted";
                }
            }
            #region Catch
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
        public DataTable SearchUserGroupNew(string GroupName, string ActiveStatus)
        {
            string[] retResults = new string[2];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("UserGroups");

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
                            SELECT isnull(NULLIF(c.GroupID,''),0)GroupID, 
                            isnull(NULLIF(c.GroupName,''),'')GroupName,
                            isnull(NULLIF(c.Comments,''),'')Comments,
                            isnull(NULLIF(c.ActiveStatus,''),'')ActiveStatus
                            FROM UserGroups c
                 
                            WHERE 
                                (GroupName  LIKE '%' +  @GroupName  + '%' OR @GroupName IS NULL) 
                            AND (c.ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
                            order by GroupID 
                            ";
                SqlCommand objCommUserGroup = new SqlCommand();
                objCommUserGroup.Connection = currConn;

                objCommUserGroup.CommandText = sqlText;
                objCommUserGroup.CommandType = CommandType.Text;


                if (!objCommUserGroup.Parameters.Contains("@GroupName"))
                { objCommUserGroup.Parameters.AddWithValue("@GroupName", GroupName); }
                else { objCommUserGroup.Parameters["@GroupName"].Value = GroupName; }

                if (!objCommUserGroup.Parameters.Contains("@ActiveStatus"))
                { objCommUserGroup.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommUserGroup.Parameters["@ActiveStatus"].Value = ActiveStatus; }
                // Common Filed
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommUserGroup);

                dataAdapter.Fill(dataTable);
            }
            #region Catch
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
        #endregion
    }

}
