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
    public class VendorGroupDAL
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        //======================InsertToVendorGroup=====================
        //public string[] InsertToVendorGroup(string VendorGroupID, string VendorGroupName,
        //    string VendorGroupDescription, string Comments, string ActiveStatus, string CreatedBy,
        //    string CreatedOn, string LastModifiedBy, string LastModifiedOn, string GroupType,
        //    string datebaseName)
        public string[] InsertToVendorGroup(VendorGroupVM vm)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            int nextId = 0;

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.VendorGroupName))
                {
                    throw new ArgumentNullException("InsertToVendorGroup",
                                                    "Please enter Vendor group name.");
                }
                if (string.IsNullOrEmpty(vm.GroupType))
                {
                    throw new ArgumentNullException("UpdateToVendorGroup",
                                                    "Invalid Vendor group type.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToVendorGroup");

                #endregion open connection and transaction


                #region Vendor Group name existence checking


                sqlText = "select count(VendorGroupID) from VendorGroups where  VendorGroupName=@VendorGroupName  AND IsArchive = 0";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;
                cmdNameExist.Parameters.AddWithValue("@VendorGroupName", vm.VendorGroupName);

                countId = (int)cmdNameExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("InsertToVendorGroup",
                                                    "Same Vendor group name already exist");
                }
                #endregion Vendor Group name existence checking


                #region Vendor Group new id generation
                sqlText = "select isnull(max(cast(VendorGroupID as int)),0)+1 FROM  VendorGroups";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                nextId = Convert.ToInt32(cmdNextId.ExecuteScalar());
                if (nextId <= 0)
                {

                    throw new ArgumentNullException("InsertToVendorGroup",
                                                    "Unable to create new Product No");
                }
                #endregion Customer Group new id generation


                #region Inser new Vendor group
                sqlText = "";
                sqlText += "insert into VendorGroups";
                sqlText += "(";
                sqlText += "VendorGroupID,";
                sqlText += "VendorGroupName,";
                sqlText += "VendorGroupDescription,";
                sqlText += "Comments,";
                sqlText += "ActiveStatus,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "LastModifiedBy,";
                sqlText += "LastModifiedOn,";
                sqlText += "GroupType";
                //sqlText += "Info2,";
                //sqlText += "Info3,";
                //sqlText += "Info4,";
                //sqlText += "Info5";

                sqlText += ")";
                sqlText += " values(";
                sqlText += "'" + nextId + "',";
                sqlText += "@VendorGroupName,";
                sqlText += "@VendorGroupDescription,";
                sqlText += "@Comments,";
                sqlText += "@ActiveStatus,";
                sqlText += "@CreatedBy,";
                sqlText += "@CreatedOn,";
                sqlText += "@LastModifiedBy,";
                sqlText += "@LastModifiedOn,";
                sqlText += "@GroupType";
                sqlText += ")";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@VendorGroupName", vm.VendorGroupName);
                cmdInsert.Parameters.AddWithValue("@VendorGroupDescription ", vm.VendorGroupDescription ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@GroupType", vm.GroupType ?? Convert.DBNull);

                transResult = (int)cmdInsert.ExecuteNonQuery();


                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Vendor group Information successfully Added";
                        retResults[2] = "" + nextId;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected erro to add Vendor group";
                        retResults[2] = "";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected erro to add customer group";
                    retResults[2] = "";
                }

                #endregion Commit


                #endregion Inser new Vendor group

            }
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
            ///////////////////////////////////////////////////////////////////////////////////////////////////

            return retResults;
        }
        public static string InsertToVendorGroup1(SqlCommand objCommVendorGroup,
            //string HSCodeNo,
       string VendorGroupID,
       string VendorGroupName,
       string VendorGroupDescription,
       string Comments,
       string ActiveStatus,
       string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn,
       string GroupType, string Info2, string Info3, string Info4, string Info5)
        {
            string result = "-1";

            //string strSQL = "SpInsertUpdateVendorGroup";
            string strSQL = @"
declare @Present numeric;

select @Present = count(VendorGroupID) from VendorGroups 
where  VendorGroupID=@VendorGroupID;
if(@Present <=0 )
BEGIN

select @Present = count(distinct VendorGroupName) from VendorGroups 
where  VendorGroupName=@VendorGroupName;

if(@Present >0 )
select -900;

else
BEGIN

select @VendorGroupID= isnull(max(cast(VendorGroupID as int)),0)+1 FROM  VendorGroups;


insert into VendorGroups(
VendorGroupID,
VendorGroupName,
VendorGroupDescription,
Comments,
ActiveStatus,
CreatedBy,
CreatedOn,
LastModifiedBy,
LastModifiedOn,
GroupType,
Info2,
Info3,
Info4,
Info5)
values(
@VendorGroupID,
@VendorGroupName,
@VendorGroupDescription,
@Comments,
@ActiveStatus,
@CreatedBy,
@CreatedOn,
@LastModifiedBy,
@LastModifiedOn,
@GroupType,
@Info2,
@Info3,
@Info4,
@Info5)

select @VendorGroupID;
END

END

else
BEGIN

update VendorGroups set 

VendorGroupName=@VendorGroupName,
VendorGroupDescription=@VendorGroupDescription,
Comments=@Comments,
ActiveStatus=@ActiveStatus,

LastModifiedBy=@LastModifiedBy,
LastModifiedOn=@LastModifiedOn,
GroupType=@GroupType,
Info2=@Info2,
Info3=@Info3,
Info4=@Info4,
Info5=@Info5

where VendorGroupID=@VendorGroupID;
select @VendorGroupID;
END
";

            objCommVendorGroup.CommandText = strSQL;
            objCommVendorGroup.CommandType = CommandType.Text;
            // Change # 6s

            if (!objCommVendorGroup.Parameters.Contains("@VendorGroupID"))
            { objCommVendorGroup.Parameters.AddWithValue("@VendorGroupID", VendorGroupID); }
            else { objCommVendorGroup.Parameters["@VendorGroupID"].Value = VendorGroupID; }
            if (!objCommVendorGroup.Parameters.Contains("@VendorGroupName"))
            { objCommVendorGroup.Parameters.AddWithValue("@VendorGroupName", VendorGroupName); }
            else { objCommVendorGroup.Parameters["@VendorGroupName"].Value = VendorGroupName; }
            if (!objCommVendorGroup.Parameters.Contains("@VendorGroupDescription"))
            { objCommVendorGroup.Parameters.AddWithValue("@VendorGroupDescription", VendorGroupDescription); }
            else { objCommVendorGroup.Parameters["@VendorGroupDescription"].Value = VendorGroupDescription; }
            if (!objCommVendorGroup.Parameters.Contains("@Comments"))
            { objCommVendorGroup.Parameters.AddWithValue("@Comments", Comments); }
            else { objCommVendorGroup.Parameters["@Comments"].Value = Comments; }
            if (!objCommVendorGroup.Parameters.Contains("@ActiveStatus"))
            { objCommVendorGroup.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
            else { objCommVendorGroup.Parameters["@ActiveStatus"].Value = ActiveStatus; }

            //Common Fields
            if (!objCommVendorGroup.Parameters.Contains("@CreatedBy"))
            { objCommVendorGroup.Parameters.AddWithValue("@CreatedBy", CreatedBy); }
            else { objCommVendorGroup.Parameters["@CreatedBy"].Value = CreatedBy; }
            if (!objCommVendorGroup.Parameters.Contains("@CreatedOn"))
            { objCommVendorGroup.Parameters.AddWithValue("@CreatedOn", CreatedOn); }
            else { objCommVendorGroup.Parameters["@CreatedOn"].Value = CreatedOn; }
            if (!objCommVendorGroup.Parameters.Contains("@LastModifiedBy"))
            { objCommVendorGroup.Parameters.AddWithValue("@LastModifiedBy", LastModifiedBy); }
            else { objCommVendorGroup.Parameters["@LastModifiedBy"].Value = LastModifiedBy; }
            if (!objCommVendorGroup.Parameters.Contains("@LastModifiedOn"))
            { objCommVendorGroup.Parameters.AddWithValue("@LastModifiedOn", LastModifiedOn); }
            else { objCommVendorGroup.Parameters["@LastModifiedOn"].Value = LastModifiedOn; }
            if (!objCommVendorGroup.Parameters.Contains("@GroupType"))
            { objCommVendorGroup.Parameters.AddWithValue("@GroupType", GroupType); }
            else { objCommVendorGroup.Parameters["@GroupType"].Value = GroupType; }
            if (!objCommVendorGroup.Parameters.Contains("@Info2"))
            { objCommVendorGroup.Parameters.AddWithValue("@Info2", Info2); }
            else { objCommVendorGroup.Parameters["@Info2"].Value = Info2; }
            if (!objCommVendorGroup.Parameters.Contains("@Info3"))
            { objCommVendorGroup.Parameters.AddWithValue("@Info3", Info3); }
            else { objCommVendorGroup.Parameters["@Info3"].Value = Info3; }
            if (!objCommVendorGroup.Parameters.Contains("@Info4"))
            { objCommVendorGroup.Parameters.AddWithValue("@Info4", Info4); }
            else { objCommVendorGroup.Parameters["@Info4"].Value = Info4; }
            if (!objCommVendorGroup.Parameters.Contains("@Info5"))
            { objCommVendorGroup.Parameters.AddWithValue("@Info5", Info5); }
            else { objCommVendorGroup.Parameters["@Info5"].Value = Info5; }



            try
            {
                result = objCommVendorGroup.ExecuteScalar().ToString();
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
            catch (Exception ex) { Trace.WriteLine(ex.Message); }
            finally { }

            return result;
        }

        //====================================UpdateToVendorGroup=======
        //public string[] UpdateToVendorGroup(string VendorGroupID, string VendorGroupName,
        //   string VendorGroupDescription, string Comments, string ActiveStatus,
        //   string LastModifiedBy, string LastModifiedOn, string GroupType, string datebaseName)
        public string[] UpdateToVendorGroup(VendorGroupVM vm)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.VendorGroupID;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            int nextId = 0;

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.VendorGroupID))
                {
                    throw new ArgumentNullException("InsertToVendorGroup",
                                                    "Please enter Vendor group id.");
                }
                if (string.IsNullOrEmpty(vm.GroupType))
                {
                    throw new ArgumentNullException("UpdateToVendorGroup",
                                                    "Invalid Vendor group type.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UpdateToVendorGroup");

                #endregion open connection and transaction

                #region Vendor Group name existence checking

                sqlText = "select count(VendorGroupID) from VendorGroups where  VendorGroupID=@VendorGroupID";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;
                cmdIdExist.Parameters.AddWithValue("@VendorGroupID", vm.VendorGroupID);

                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("Update VendorGroup",
                                "Could not find requested VendorGroup  id.");
                }
                #endregion Vendor Group name existence checking

                #region Vendor Group  update
                sqlText = "select count(VendorGroupName) from VendorGroups ";
                sqlText += " where 1=1 AND IsArchive = 0 AND  VendorGroupName =@VendorGroupName ";
                sqlText += " and not VendorGroupID  =@VendorGroupID ";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;
                cmdNameExist.Parameters.AddWithValue("@VendorGroupName", vm.VendorGroupName ?? Convert.DBNull);
                cmdNameExist.Parameters.AddWithValue("@VendorGroupID", vm.VendorGroupID ?? Convert.DBNull);

                countId = (int)cmdNameExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("InsertToVendorGroup",
                                                   "VendorGroup Already Exist!");
                }
                #endregion

                #region Update Vendor group
                sqlText = "";
                sqlText += "update VendorGroups set";
                sqlText += " VendorGroupName        =@VendorGroupName,";
                sqlText += " VendorGroupDescription =@VendorGroupDescription,";
                sqlText += " Comments               =@Comments,";
                sqlText += " ActiveStatus           =@ActiveStatus,";
                sqlText += " LastModifiedBy         =@LastModifiedBy,";
                sqlText += " LastModifiedOn         =@LastModifiedOn,";
                sqlText += " GroupType              =@GroupType";
                sqlText += " where VendorGroupID    =@VendorGroupID";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@VendorGroupName", vm.VendorGroupName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VendorGroupDescription", vm.VendorGroupDescription ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@GroupType", vm.GroupType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VendorGroupID", vm.VendorGroupID ?? Convert.DBNull);

                transResult = (int)cmdInsert.ExecuteNonQuery();


                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Vendor group Information successfully Updated.";


                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update vendor group";

                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update vendor group";

                }

                #endregion Commit


                #endregion Inser new Vendor group

            }
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
            ///////////////////////////////////////////////////////////////////////////////////////////////////

            return retResults;
        }

        //======================================DeleteVendorGroup=======
        public string[] DeleteVendorGroup(string VendorGroupID, string databaseName)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = VendorGroupID;

            SqlConnection currConn = null;
            int countId = 0;
            string sqlText = "";

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(VendorGroupID))
                {
                    throw new ArgumentNullException("DeleteVendor",
                                "Could not find requested VendorGroup ID.");
                }
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction


                sqlText = "select count(VendorGroupID) from VendorGroups where VendorGroupID=@VendorGroupID ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValue("@VendorGroupID", VendorGroupID);

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested VendorGroups.";
                    return retResults;
                }

                sqlText = "delete VendorGroups where VendorGroupID=@VendorGroupID ";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                cmdDelete.Parameters.AddWithValue("@VendorGroupID", VendorGroupID);

                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested Vendor Group  Information successfully deleted.";
                }
            }
            catch (SqlException sqlex)
            {

                retResults[0] = "Fail";
                retResults[1] = "Database related error. See the log for details.";
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {

                retResults[0] = "Fail";
                retResults[1] = "Unexpected error. See the log for details.";
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

        //==============================SearchVendorGroup=========================================
        public DataTable SearchVendorGroupNew(string VendorGroupID, string VendorGroupName, string VendorGroupDescription, string ActiveStatus, string GroupType, string databaseName)
        {
            #region Objects & Variables

            //string Description = "";
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Search Vendor Groups");
            #endregion
            #region try
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
                            VendorGroupID,
                            isnull(VendorGroupName,'N/A')VendorGroupName,
                            isnull(VendorGroupDescription,'N/A')VendorGroupDescription,
                            isnull(Comments,'N/A')Comments,
                            isnull(ActiveStatus,'N/A')ActiveStatus,
                            GroupType
                            FROM         dbo.VendorGroups
                 
                            WHERE (VendorGroupID LIKE '%' +  @VendorGroupID + '%' OR @VendorGroupID IS NULL) 
                            AND (VendorGroupName LIKE '%' + @VendorGroupName + '%' OR @VendorGroupName IS NULL)
                            AND (VendorGroupDescription LIKE '%' + @VendorGroupDescription + '%' OR @VendorGroupDescription IS NULL)
                            AND (ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
                            AND (GroupType LIKE '%' + @GroupType + '%' OR @GroupType IS NULL)

                            and VendorGroupID <> 0

                            order by VendorGroupName
                            ";
                SqlCommand objCommVendorGroup = new SqlCommand();
                objCommVendorGroup.Connection = currConn;
                objCommVendorGroup.CommandText = sqlText;
                objCommVendorGroup.CommandType = CommandType.Text;

                if (!objCommVendorGroup.Parameters.Contains("@VendorGroupID"))
                { objCommVendorGroup.Parameters.AddWithValue("@VendorGroupID", VendorGroupID); }
                else { objCommVendorGroup.Parameters["@VendorGroupID"].Value = VendorGroupID; }

                if (!objCommVendorGroup.Parameters.Contains("@VendorGroupName"))
                { objCommVendorGroup.Parameters.AddWithValue("@VendorGroupName", VendorGroupName); }
                else { objCommVendorGroup.Parameters["@VendorGroupName"].Value = VendorGroupName; }

                if (!objCommVendorGroup.Parameters.Contains("@VendorGroupDescription"))
                { objCommVendorGroup.Parameters.AddWithValue("@VendorGroupDescription", VendorGroupDescription); }
                else { objCommVendorGroup.Parameters["@VendorGroupDescription"].Value = VendorGroupDescription; }

                if (!objCommVendorGroup.Parameters.Contains("@ActiveStatus"))
                { objCommVendorGroup.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommVendorGroup.Parameters["@ActiveStatus"].Value = ActiveStatus; }

                if (!objCommVendorGroup.Parameters.Contains("@GroupType"))
                { objCommVendorGroup.Parameters.AddWithValue("@GroupType", GroupType); }
                else { objCommVendorGroup.Parameters["@GroupType"].Value = GroupType; }


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVendorGroup);
                dataAdapter.Fill(dataTable);

                // Common Filed
                #endregion
            }
            #endregion
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

        //==================DropDownAll=================
        public List<VendorGroupVM> DropDownAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<VendorGroupVM> VMs = new List<VendorGroupVM>();
            VendorGroupVM vm;
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
SELECT * FROM(
SELECT 
'B' Sl, VendorGroupID
, VendorGroupName
FROM VendorGroups
WHERE ActiveStatus='Y'
UNION 
SELECT 
'A' Sl, '-1' Id
, 'ALL Product' VendorGroupName  
FROM VendorGroups
)
AS a
order by Sl,VendorGroupName
";



                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new VendorGroupVM();
                    vm.VendorGroupID = dr["VendorGroupID"].ToString(); ;
                    vm.VendorGroupName = dr["VendorGroupName"].ToString();
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


        //==================DropDown=================
        public List<VendorGroupVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<VendorGroupVM> VMs = new List<VendorGroupVM>();
            VendorGroupVM vm;
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
af.VendorGroupID
,af.VendorGroupName
FROM VendorGroups af
WHERE  1=1 AND af.ActiveStatus = 'Y'
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new VendorGroupVM();
                    vm.VendorGroupID = dr["VendorGroupID"].ToString();
                    vm.VendorGroupName = dr["VendorGroupName"].ToString();
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
        public List<VendorGroupVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<VendorGroupVM> VMs = new List<VendorGroupVM>();
            VendorGroupVM vm;
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
 VendorGroupID
,VendorGroupName
,VendorGroupDescription
,GroupType
,Comments
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,Info3
,Info4
,Info5
,Info2

FROM VendorGroups  
WHERE  1=1 AND IsArchive = 0

";


                if (Id > 0)
                {
                    sqlText += @" and VendorGroupID=@VendorGroupID";
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
                    objComm.Parameters.AddWithValue("@VendorGroupID", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new VendorGroupVM();
                    vm.VendorGroupID = dr["VendorGroupID"].ToString();
                    vm.VendorGroupName = dr["VendorGroupName"].ToString();
                    vm.VendorGroupDescription = dr["VendorGroupDescription"].ToString();
                    vm.GroupType = dr["GroupType"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.Info3 = dr["Info3"].ToString();
                    vm.Info4 = dr["Info4"].ToString();
                    vm.Info5 = dr["Info5"].ToString();
                    vm.Info2 = dr["Info2"].ToString();

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

        ////==================Delete =================
        public string[] Delete(VendorGroupVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteVendorGroup"; //Method Name
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
                        sqlText = "update VendorGroups set";
                        sqlText += " ActiveStatus=@ActiveStatus";
                        sqlText += " ,LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where VendorGroupID=@VendorGroupID";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@VendorGroupID", ids[i]);
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
                        throw new ArgumentNullException("VendorGroup Delete", vm.VendorGroupID + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("VendorGroup Information Delete", "Could not found any item.");
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
    }
}
