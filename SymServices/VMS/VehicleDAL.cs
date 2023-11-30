using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymOrdinary;
using SymphonySofttech.Utilities;
using SymViewModel.VMS;
using System.Reflection;

namespace SymServices.VMS
{
    public class VehicleDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        #endregion

        #region New Methods
        //==================DropDown=================
        public List<VehicleVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<VehicleVM> VMs = new List<VehicleVM>();
            VehicleVM vm;
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
SELECT
af.VehicleID
,af.VehicleNo
FROM Vehicles af
WHERE  1=1 AND af.ActiveStatus = 'Y'
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new VehicleVM();
                    vm.VehicleID = dr["VehicleID"].ToString();
                    vm.VehicleNo = dr["VehicleNo"].ToString();
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
        public List<VehicleVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<VehicleVM> VMs = new List<VehicleVM>();
            VehicleVM vm;
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
 VehicleID
,VehicleCode
,VehicleType
,VehicleNo
,Description
,Comments
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,Info1
,Info2
,Info3
,Info4
,Info5
,DriverName


FROM Vehicles  
WHERE  1=1 AND IsArchive =0

";


                if (Id > 0)
                {
                    sqlText += @" and VehicleID=@VehicleID";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]) || conditionValues[i] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        if (conditionFields[i].ToLower().Contains("like"))
                        {
                            sqlText += " AND " + conditionFields[i] + " '%'+ " + " @" + cField.Replace("like", "").Trim() + " +'%'";
                        }
                        else if (conditionFields[i].Contains(">") || conditionFields[i].Contains("<"))
                        {
                            sqlText += " AND " + conditionFields[i] + " @" + cField;

                        }
                        else
                        {
                            sqlText += " AND " + conditionFields[i] + "= @" + cField;
                        }
                    }
                }
                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]) || conditionValues[j] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        if (conditionFields[j].ToLower().Contains("like"))
                        {
                            objComm.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(), conditionValues[j]);
                        }
                        else
                        {
                            objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                        }
                    }
                }

                if (Id > 0)
                {
                    objComm.Parameters.AddWithValue("@VehicleID", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new VehicleVM();
                    vm.VehicleID = dr["VehicleID"].ToString();
                    vm.Code = dr["VehicleCode"].ToString();
                    vm.VehicleType = dr["VehicleType"].ToString();
                    vm.VehicleNo = dr["VehicleNo"].ToString();
                    vm.Description = dr["Description"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    //vm.CreatedOn = DateTime.Parse(dr["CreatedOn"].ToString());
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    //vm.LastModifiedOn = DateTime.Parse(dr["LastModifiedOn"].ToString());
                    vm.Info1 = dr["Info1"].ToString();
                    vm.Info2 = dr["Info2"].ToString();
                    vm.Info3 = dr["Info3"].ToString();
                    vm.Info4 = dr["Info4"].ToString();
                    vm.Info5 = dr["Info5"].ToString();
                    vm.DriverName = dr["DriverName"].ToString();

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

        //public string[] InsertToVehicle(string VehicleID, string VehicleType, string VehicleNo, string Description,
        //    string Comments,string DriverName, string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy,
        //    string LastModifiedOn, string VehicleCode)
        public string[] InsertToVehicle(VehicleVM vm)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            string vehicleCode;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            int nextId = 0;
            #endregion

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.VehicleType))
                {
                    throw new ArgumentNullException("InsertToVehicle", "Please select one vehicle type.");
                }
                if (string.IsNullOrEmpty(vm.VehicleNo))
                {
                    throw new ArgumentNullException("InsertToVehicle", "Please enter vehicle no.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToVehicle");

                #endregion open connection and transaction

                #region VehicleNo existence checking

                //select @Present = count(distinct VehicleNo) from Vehicles where VehicleNo = @VehicleNo;
                sqlText = "select count(VehicleNo) from Vehicles where VehicleNo =@VehicleNo";
                SqlCommand vhclNoExist = new SqlCommand(sqlText, currConn);
                vhclNoExist.Transaction = transaction;
                vhclNoExist.Parameters.AddWithValue("@VehicleNo", vm.VehicleNo);
                countId = (int)vhclNoExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("InsertToVehicle", "Same vehicle no('" + vm.VehicleNo + "') already exist.");
                }

                #endregion VehicleNo existence checking

                #region Vehicle new id generation

                //select @VehicleID= isnull(max(cast(VehicleID as int)),0)+1 FROM  Vehicles;
                sqlText = "select isnull(max(cast(VehicleID as int)),0)+1 FROM  Vehicles";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                nextId = Convert.ToInt32(cmdNextId.ExecuteScalar());
                if (nextId <= 0)
                {

                    throw new ArgumentNullException("InsertToVehicle", "Unable to create new vehicle");
                }

                #endregion Vehicle new id generation

                vehicleCode = nextId.ToString();
                vm.VehicleID = nextId.ToString();

                #region Insert new vehicle

                sqlText = "";
                sqlText += @" 
INSERT INTO Vehicles(
 VehicleID
,VehicleCode
,VehicleType
,VehicleNo
,Description
,Comments
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,DriverName

) VALUES (
 @VehicleID
,@VehicleCode
,@VehicleType
,@VehicleNo
,@Description
,@Comments
,@ActiveStatus
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn
,@DriverName
         
) 
";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@VehicleID", vm.VehicleID);
                cmdInsert.Parameters.AddWithValue("@VehicleCode", vehicleCode);
                cmdInsert.Parameters.AddWithValue("@VehicleType", vm.VehicleType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VehicleNo", vm.VehicleNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Description", vm.Description ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@DriverName", vm.DriverName ?? Convert.DBNull);

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested vehicle  Information successfully Added.";
                        retResults[2] = "" + nextId;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to add vehicle";
                        retResults[2] = "";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to add vehicle ";
                    retResults[2] = "";
                }

                #endregion Commit

                #endregion Insert new vehicle

            }
            #region catch & Finally
            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
                return retResults;
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

            #endregion

            return retResults;
        }


        //public string[] UpdateToVehicle(string VehicleID, string VehicleType, string VehicleNo, string Description, string Comments, string DriverName, string ActiveStatus, string LastModifiedBy, string LastModifiedOn, string VehicleCode, string dataBaseName)
        public string[] UpdateToVehicle(VehicleVM vm)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            string vehicleCode;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            int nextId = 0;
            #endregion

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.VehicleType))
                {
                    throw new ArgumentNullException("UpdateToVehicle", "Please select one vehicle type.");
                }
                if (string.IsNullOrEmpty(vm.VehicleNo))
                {
                    throw new ArgumentNullException("UpdateToVehicle", "Please enter vehicle no.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UpdateToVehicle");

                #endregion open connection and transaction

                #region Vehicle existence checking by id

                //select @Present = count(VehicleID) from Vehicles where VehicleID = @VehicleID;
                sqlText = "select count(VehicleID) from Vehicles where VehicleID =@VehicleID";
                SqlCommand vhclIDExist = new SqlCommand(sqlText, currConn);
                vhclIDExist.Transaction = transaction;
                vhclIDExist.Parameters.AddWithValue("@VehicleID", vm.VehicleID);

                countId = (int)vhclIDExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateToVehicle", "Could not find requested vehicle id.");
                }

                #endregion Vehicle existence checking by id

                #region VehicleNo existence checking by id and requied field

                sqlText = "select count(VehicleNo) from Vehicles ";
                sqlText += " where  VehicleNo=@VehicleNo";
                sqlText += " and VehicleType=@VehicleType";
                sqlText += " and VehicleId<>@VehicleId";
                SqlCommand vhclNoExist = new SqlCommand(sqlText, currConn);
                vhclNoExist.Transaction = transaction;
                vhclNoExist.Parameters.AddWithValue("@VehicleNo", vm.VehicleNo);
                vhclNoExist.Parameters.AddWithValue("@VehicleType", vm.VehicleType);
                vhclNoExist.Parameters.AddWithValue("@VehicleId", vm.VehicleID);

                countId = (int)vhclNoExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("UpdateToVehicle", "Same vehicle no name already exist.");
                }

                #endregion VehicleNo existence checking by id and requied field

                vehicleCode = vm.VehicleID;

                #region Update vehicle

                sqlText = "";
                sqlText = "update Vehicles set";
                //sqlText += " VehicleCode=@VehicleCode";
                sqlText += "  VehicleType   =@VehicleType";
                sqlText += " ,VehicleNo     =@VehicleNo";
                sqlText += " ,Description   =@Description";
                sqlText += " ,Comments      =@Comments";
                sqlText += " ,ActiveStatus  =@ActiveStatus";
                sqlText += " ,LastModifiedBy=@LastModifiedBy";
                sqlText += " ,LastModifiedOn=@LastModifiedOn";
                sqlText += " ,DriverName    =@DriverName";

                sqlText += " WHERE VehicleID=@VehicleID";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                //cmdUpdate.Parameters.AddWithValue("@VehicleCode", vm.VehicleCode);
                cmdUpdate.Parameters.AddWithValue("@VehicleType", vm.VehicleType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@VehicleNo", vm.VehicleNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Description", vm.Description ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@DriverName", vm.DriverName ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@VehicleID", vm.VehicleID);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Vehicle Information Successfully Update.";
                        retResults[2] = "" + vm.VehicleID;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update vehicles.";
                        retResults[2] = "" + vm.VehicleID;
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update vehicles";
                    retResults[2] = "" + vm.VehicleID;
                }

                #endregion Commit

                #endregion Update vehicle

            }
            #region catch
            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex

                if (transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
                return retResults;
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

            #endregion

            return retResults;
        }

        //==================Delete Vehicle=================
        public string[] DeleteToVehicle(string VehicleID, string databaseName)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = VehicleID;

            SqlConnection currConn = null;
            //SqlTransaction transaction = null;
            //int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(VehicleID))
                {
                    throw new ArgumentNullException("DeleteToVehicle", "Could not find requested vehicle.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region Vehicle existence checking by id

                //select @Present = count(VehicleID) from Vehicles where VehicleID = @VehicleID;
                sqlText = "select count(VehicleID) from Vehicles where VehicleID =@VehicleID";
                SqlCommand vhclIDExist = new SqlCommand(sqlText, currConn);
                vhclIDExist.Parameters.AddWithValue("@VehicleID", VehicleID);
                int foundId = (int)vhclIDExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested customer.";
                    return retResults;
                }

                #endregion Vehicle existence checking by id

                #region Delete vehicle

                sqlText = "delete Vehicles where VehicleID=@VehicleID";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                cmdDelete.Parameters.AddWithValue("@VehicleID", VehicleID);

                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested Vehicle Information Successfully Deleted.";
                }

                #endregion Update vehicle

            }
            #region catch

            catch (SqlException sqlex)
            {

                retResults[0] = "Fail";
                retResults[1] = "Database related error. See the log for details";
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {

                retResults[0] = "Fail";
                retResults[1] = "Unexpected error. See the log for details";
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

            #endregion

            return retResults;
        }

        //==================Search Vehicle=================
        public DataTable SearchVehicleDataTable(string VehicleID, string VehicleType, string VehicleNo, string ActiveStatus)
        {

            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Search Vehicle");

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

                sqlText = @"SELECT VehicleID,
                                isnull(VehicleType,'N/A')VehicleType,
                                isnull(VehicleNo,'N/A')VehicleNo,
                                isnull(Description,'N/A')Description,
                                isnull(Comments,'N/A')Comments,
                                isnull(DriverName,'-')DriverName,
                                isnull(ActiveStatus,'N')ActiveStatus
                                FROM dbo.Vehicles
                                WHERE 
                                (VehicleID LIKE '%' + @VehicleID  + '%' OR @VehicleID IS NULL) 
                                AND (VehicleType LIKE '%' + @VehicleType + '%' OR @VehicleType IS NULL)
                                AND (VehicleNo LIKE '%' + @VehicleNo + '%' OR @VehicleNo IS NULL) 
                                AND (ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
                                and  (VehicleID <>'0') 
                                order by VehicleNo ";

                SqlCommand objCommVehicle = new SqlCommand();
                objCommVehicle.Connection = currConn;
                objCommVehicle.CommandText = sqlText;
                objCommVehicle.CommandType = CommandType.Text;

                if (!objCommVehicle.Parameters.Contains("@VehicleID"))
                { objCommVehicle.Parameters.AddWithValue("@VehicleID", VehicleID); }
                else { objCommVehicle.Parameters["@VehicleID"].Value = VehicleID; }
                if (!objCommVehicle.Parameters.Contains("@VehicleType"))
                { objCommVehicle.Parameters.AddWithValue("@VehicleType", VehicleType); }
                else { objCommVehicle.Parameters["@VehicleType"].Value = VehicleType; }
                if (!objCommVehicle.Parameters.Contains("@VehicleNo"))
                { objCommVehicle.Parameters.AddWithValue("@VehicleNo", VehicleNo); }
                else { objCommVehicle.Parameters["@VehicleNo"].Value = VehicleNo; }

                if (!objCommVehicle.Parameters.Contains("@ActiveStatus"))
                { objCommVehicle.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommVehicle.Parameters["@ActiveStatus"].Value = ActiveStatus; }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVehicle);
                dataAdapter.Fill(dataTable);

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

            return dataTable;
        }

        ////==================Delete =================
        public string[] Delete(VehicleVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteVehicle"; //Method Name
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
                    currConn = _dbsqlConnection.GetConnection();
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
                        sqlText = "update Vehicles set";
                        sqlText += " ActiveStatus=@ActiveStatus";
                        sqlText += " ,LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where VehicleID=@VehicleID";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@VehicleID", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@ActiveStatus", "N");
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy);
                        cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Vehicle Delete", vm.VehicleID + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Vehicle Information Delete", "Could not found any item.");
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
        #endregion

        #region Old Methods


        public static string InsertToVehicle1(SqlCommand objCommVehicle, string VehicleID, string VehicleType, string VehicleNo, string Description, string Comments, string ActiveStatus, string CreatedBy, DateTime CreatedOn, string LastModifiedBy, DateTime LastModifiedOn)
        {
            string result = "-1";

            //string strSQL = "SpInsertUpdateVehicle";
            string strSQL =
@"declare @Present numeric;
select @Present = count(VehicleID) from Vehicles 
where  VehicleID=@VehicleID;
if(@Present <=0 )
BEGIN	


select @Present = count(distinct VehicleNo) from Vehicles 
where  VehicleNo=@VehicleNo;

if(@Present >0 )
select -900;

else
BEGIN
select @VehicleID= isnull(max(cast(VehicleID as int)),0)+1 FROM  Vehicles;

		
insert into Vehicles(
VehicleID,
VehicleType,
VehicleNo,
Description,
Comments,
ActiveStatus,
CreatedBy,
CreatedOn,
LastModifiedBy,
LastModifiedOn)
    values(
@VehicleID,
@VehicleType,
@VehicleNo,
@Description,
@Comments,
@ActiveStatus,
@CreatedBy,
@CreatedOn,
@LastModifiedBy,
@LastModifiedOn)
select @VehicleID;
	END
	END
else
BEGIN

update Vehicles set 
VehicleType=@VehicleType,
VehicleNo=@VehicleNo,
Description=@Description,
Comments=@Comments,
ActiveStatus=@ActiveStatus,
LastModifiedBy=@LastModifiedBy,
LastModifiedOn=@LastModifiedOn
	where VehicleID=@VehicleID;
select @VehicleID;
END";

            objCommVehicle.CommandText = strSQL;
            //objCommVehicle.CommandType = ;
            objCommVehicle.CommandType = CommandType.Text;

            if (!objCommVehicle.Parameters.Contains("@VehicleID"))
            { objCommVehicle.Parameters.AddWithValue("@VehicleID", VehicleID); }
            else { objCommVehicle.Parameters["@VehicleID"].Value = VehicleID; }
            if (!objCommVehicle.Parameters.Contains("@VehicleType"))
            { objCommVehicle.Parameters.AddWithValue("@VehicleType", VehicleType); }
            else { objCommVehicle.Parameters["@VehicleType"].Value = VehicleType; }
            if (!objCommVehicle.Parameters.Contains("@VehicleNo"))
            { objCommVehicle.Parameters.AddWithValue("@VehicleNo", VehicleNo); }
            else { objCommVehicle.Parameters["@VehicleNo"].Value = VehicleNo; }
            if (!objCommVehicle.Parameters.Contains("@Description"))
            { objCommVehicle.Parameters.AddWithValue("@Description", Description); }
            else { objCommVehicle.Parameters["@Description"].Value = Description; }
            if (!objCommVehicle.Parameters.Contains("@Comments"))
            { objCommVehicle.Parameters.AddWithValue("@Comments", Comments); }
            else { objCommVehicle.Parameters["@Comments"].Value = Comments; }
            if (!objCommVehicle.Parameters.Contains("@ActiveStatus"))
            { objCommVehicle.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
            else { objCommVehicle.Parameters["@ActiveStatus"].Value = ActiveStatus; }
            //Common Fields
            if (!objCommVehicle.Parameters.Contains("@CreatedBy"))
            { objCommVehicle.Parameters.AddWithValue("@CreatedBy", CreatedBy); }
            else { objCommVehicle.Parameters["@CreatedBy"].Value = CreatedBy; }
            if (!objCommVehicle.Parameters.Contains("@CreatedOn"))
            { objCommVehicle.Parameters.AddWithValue("@CreatedOn", CreatedOn); }
            else { objCommVehicle.Parameters["@CreatedOn"].Value = CreatedOn; }
            if (!objCommVehicle.Parameters.Contains("@LastModifiedBy"))
            { objCommVehicle.Parameters.AddWithValue("@LastModifiedBy", LastModifiedBy); }
            else { objCommVehicle.Parameters["@LastModifiedBy"].Value = LastModifiedBy; }
            if (!objCommVehicle.Parameters.Contains("@LastModifiedOn"))
            { objCommVehicle.Parameters.AddWithValue("@LastModifiedOn", LastModifiedOn); }
            else { objCommVehicle.Parameters["@LastModifiedOn"].Value = LastModifiedOn; }


            try
            {
                result = objCommVehicle.ExecuteScalar().ToString();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    return "-99";
                }
                else if (ex.Number == 266)
                {
                    return "-266";
                }
                else
                {
                    return "-1";
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            finally
            {

            }

            return result;
        }


        #endregion
    }
}
