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
    public class CustomerGroupDAL
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        //==================DropDownAll=================
        public List<CustomerGroupVM> DropDownAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<CustomerGroupVM> VMs = new List<CustomerGroupVM>();
            CustomerGroupVM vm;
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
'B' Sl, CustomerGroupID
, CustomerGroupName
FROM CustomerGroups
WHERE ActiveStatus='Y'
UNION 
SELECT 
'A' Sl, '-1' Id
, 'ALL Group' CustomerGroupName  
FROM CustomerGroups
)
AS a
order by Sl,CustomerGroupName
";



                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CustomerGroupVM();
                    vm.CustomerGroupID = dr["CustomerGroupID"].ToString(); ;
                    vm.CustomerGroupName = dr["CustomerGroupName"].ToString();
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
        public List<CustomerGroupVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<CustomerGroupVM> VMs = new List<CustomerGroupVM>();
            CustomerGroupVM vm;
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
af.CustomerGroupID
,af.CustomerGroupName
FROM CustomerGroups af
WHERE  1=1 AND af.ActiveStatus = 'Y'
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CustomerGroupVM();
                    vm.CustomerGroupID = dr["CustomerGroupID"].ToString();
                    vm.CustomerGroupName = dr["CustomerGroupName"].ToString();
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
        public List<CustomerGroupVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<CustomerGroupVM> VMs = new List<CustomerGroupVM>();
            CustomerGroupVM vm;
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
 CustomerGroupID
,CustomerGroupName
,CustomerGroupDescription
,GroupType
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

FROM CustomerGroups  
WHERE  1=1 AND IsArchive = 0

";


                if (Id > 0)
                {
                    sqlText += @" and CustomerGroupID=@CustomerGroupID";
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
                    objComm.Parameters.AddWithValue("@CustomerGroupID", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CustomerGroupVM();
                    vm.CustomerGroupID = dr["CustomerGroupID"].ToString();
                    vm.CustomerGroupName = dr["CustomerGroupName"].ToString();
                    vm.CustomerGroupDescription = dr["CustomerGroupDescription"].ToString();
                    vm.GroupType = dr["GroupType"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.Info1 = dr["Info1"].ToString();
                    vm.Info2 = dr["Info2"].ToString();
                    vm.Info3 = dr["Info3"].ToString();
                    vm.Info4 = dr["Info4"].ToString();
                    vm.Info5 = dr["Info5"].ToString();

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

        //public string[] InsertToCustomerGroupNew(string CustomerGroupID,string CustomerGroupName,string CustomerGroupDescription,string Comments,
        //string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, string GroupType, string databaseName)
        public string[] InsertToCustomerGroupNew(CustomerGroupVM vm)
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

                if (string.IsNullOrEmpty(vm.CustomerGroupName))
                {
                    throw new ArgumentNullException("InsertToCustomerGroup",
                                                    "Please enter customer group name.");
                }
                if (string.IsNullOrEmpty(vm.GroupType))
                {
                    throw new ArgumentNullException("UpdateToCustomerGroup",
                                                    "Invalid customer group type.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToCustomerGroup");

                #endregion open connection and transaction


                #region Customer Group name existence checking 


                sqlText = "select count(CustomerGroupID) from CustomerGroups where  CustomerGroupName=@CustomerGroupName";
                    SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                    cmdNameExist.Transaction = transaction;
                    cmdNameExist.Parameters.AddWithValue("@CustomerGroupName", vm.CustomerGroupName);                    
                    countId = (int)cmdNameExist.ExecuteScalar();
                    if (countId > 0)
                    {
                        throw new ArgumentNullException("InsertToCustomerGroup",
                                                        "Same customer group name already exist");
                    }
                #endregion Customer Group name existence checking
                
                
                #region Customer Group new id generation
                    sqlText = "select isnull(max(cast(CustomerGroupID as int)),0)+1 FROM  CustomerGroups";
                    SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                    cmdNextId.Transaction = transaction;
                    nextId = Convert.ToInt32(cmdNextId.ExecuteScalar());
                    if (nextId <= 0)
                    {

                        throw new ArgumentNullException("InsertToCustomerGroup",
                                                        "Unable to create new Product No");
                    }
                #endregion Customer Group new id generation


                #region Inser new customer group
                    vm.CustomerGroupID = nextId.ToString();
                    
                    sqlText = "";
                    sqlText += @" 
INSERT INTO CustomerGroups(
 CustomerGroupID
,CustomerGroupName
,CustomerGroupDescription
,GroupType
,Comments
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,IsArchive

) VALUES (
 @CustomerGroupID
,@CustomerGroupName
,@CustomerGroupDescription
,@GroupType
,@Comments
,@ActiveStatus
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn  
,@IsArchive   
) 
";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.Parameters.AddWithValue("@CustomerGroupID", vm.CustomerGroupID);
                    cmdInsert.Parameters.AddWithValue("@CustomerGroupName", vm.CustomerGroupName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@CustomerGroupDescription", vm.CustomerGroupDescription ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@GroupType", vm.GroupType ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                    cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);                    

                    transResult = (int)cmdInsert.ExecuteNonQuery();


                    #region Commit


                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                            retResults[0] = "Success";
                            retResults[1] = "Requested customer group Information successfully Added";
                            retResults[2] = "" + nextId;

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected erro to add customer group";
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


                #endregion Inser new customer group

            }
            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex

                transaction.Rollback();

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


        //public string[] UpdateToCustomerGroupNew(string CustomerGroupID,string CustomerGroupName, string CustomerGroupDescription, string Comments,
        //string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, string GroupType, string databaseName)
        public string[] UpdateToCustomerGroupNew(CustomerGroupVM vm)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.CustomerGroupID;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
                int nextId = 0;

            #region try
            try
            {
                #region Validation
                if (string.IsNullOrEmpty(vm.CustomerGroupID))
                {
                    throw new ArgumentNullException("Update Customer Group",
                                                    "Invalid Customer Group Id, Customer Group Not Found.");
                }
                if (string.IsNullOrEmpty(vm.GroupType))
                {
                    throw new ArgumentNullException("UpdateToCustomerGroup",
                                                    "Invalid customer group type.");
                }
                if (string.IsNullOrEmpty(vm.CustomerGroupName))
                {
                    throw new ArgumentNullException("UpdateToCustomerGroup",
                                                    "Please enter customer group name.");
                }


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UpdateToCustomerGroup");

                #endregion open connection and transaction

                #region Customer Group existence checking

                sqlText = "select count(CustomerGroupID) from CustomerGroups where  CustomerGroupID=@CustomerGroupID";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;
                cmdIdExist.Parameters.AddWithValue("@CustomerGroupID", vm.CustomerGroupID);

                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateToCustomerGroup",
                                "Could not find requested customer group id.");
                }

                #endregion Customer Group existence checking

                #region Customer Group name existence checking
                sqlText = "select count(CustomerGroupName) from CustomerGroups ";
                sqlText += " where  CustomerGroupName=@CustomerGroupName";
                sqlText += " and not CustomerGroupID=@CustomerGroupID";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;
                cmdNameExist.Parameters.AddWithValue("@CustomerGroupName", vm.CustomerGroupName);
                cmdNameExist.Parameters.AddWithValue("@CustomerGroupID", vm.CustomerGroupID);

                countId = (int)cmdNameExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("UpdateToCustomerGroup",
                                                    "Same customer group name already exist");
                }
                #endregion Customer Group name existence checking

                #region Inser new customer group
                sqlText = "";
                sqlText = "update CustomerGroups set";
                sqlText += " CustomerGroupName          =@CustomerGroupName";
                sqlText += " ,CustomerGroupDescription  =@CustomerGroupDescription";
                sqlText += " ,GroupType                 =@GroupType";
                sqlText += " ,Comments                  =@Comments";
                sqlText += " ,ActiveStatus              =@ActiveStatus";
                sqlText += " ,LastModifiedBy            =@LastModifiedBy";
                sqlText += " ,LastModifiedOn            =@LastModifiedOn";
                sqlText += " WHERE CustomerGroupID      =@CustomerGroupID";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@CustomerGroupName", vm.CustomerGroupName);
                cmdUpdate.Parameters.AddWithValue("@CustomerGroupDescription", vm.CustomerGroupDescription ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@GroupType", vm.GroupType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdUpdate.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@CustomerGroupID", vm.CustomerGroupID);

                
                transResult = (int)cmdUpdate.ExecuteNonQuery();


                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested customer group Information successfully Update";


                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected erro to update customer group";

                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected erro to update customer group";
                }

                #endregion Commit


                #endregion Inser new customer group

            }
            #endregion

            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex

                transaction.Rollback();

                FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
                return retResults;
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

          return retResults;
        }
        public string[] DeleteCustomerGroupNew(string CustomerGroupID, string databaseName)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = CustomerGroupID;

            SqlConnection currConn = null;
            //int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {
                #region Transaction Used
                CommonDAL commonDal = new CommonDAL();
                bool tranHas = commonDal.TransactionUsed("Customers", "CustomerGroupID", CustomerGroupID, currConn);
                if (tranHas == true)
                {
                    throw new ArgumentNullException("Used In Transaction",
                        "Requested information could not Deleted," +
                         " This information is Used in Customers");
                }
                #endregion Transaction Used
                #region Validation
                if (string.IsNullOrEmpty(CustomerGroupID))
                {
                    throw new ArgumentNullException("DeleteCustomerGroup",
                                "Could not find requested Product No.");
                }
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                sqlText = "select count(CustomerGroupID) from CustomerGroups where CustomerGroupID=@CustomerGroupID";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValue("@CustomerGroupID", CustomerGroupID);
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested customer group.";
                    return retResults;
                }

                sqlText = "delete CustomerGroups where CustomerGroupID=@CustomerGroupID";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                cmdDelete.Parameters.AddWithValue("@CustomerGroupID", CustomerGroupID);

                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested customer group Information successfully deleted";
                }
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
       
       return retResults;
        }

        public string[] Delete(CustomerGroupVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteCustomerGroup"; //Method Name
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
                        sqlText = "update CustomerGroups set";
                        sqlText += " ActiveStatus=@ActiveStatus";
                        sqlText += " ,LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";

                        sqlText += " where CustomerGroupID=@CustomerGroupID";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@CustomerGroupID", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@ActiveStatus", "N");
                        cmdUpdate.Parameters.AddWithValue("@IsArchive",true);
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
                        throw new ArgumentNullException("CustomerGroup Delete", vm.CustomerGroupID + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("CustomerGroup Information Delete", "Could not found any item.");
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


        public DataTable SearchCustomerGroupNew(string CustomerGroupID, string CustomerGroupName,string CustomerGroupDescription,
        string ActiveStatus, string GroupType, string databaseName)
        {
            #region Objects & Variables

            //string Description = "";
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("CustomerGroups");
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
CustomerGroupID,
isnull(CustomerGroupName,'N/A')CustomerGroupName,
isnull(CustomerGroupDescription,'N/A')CustomerGroupDescription,	
isnull(Comments,'N/A')Comments,	isnull(ActiveStatus,'N')ActiveStatus,
isnull(GroupType,'Local')GroupType

FROM         dbo.CustomerGroups
WHERE 	(CustomerGroupID  LIKE '%' +  @CustomerGroupID + '%' OR @CustomerGroupID IS NULL) 

AND (CustomerGroupName LIKE '%' + @CustomerGroupName + '%' OR @CustomerGroupName IS NULL)
AND (CustomerGroupDescription LIKE '%' + @CustomerGroupDescription + '%' OR @CustomerGroupDescription IS NULL) 	
AND (ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)	
AND (GroupType LIKE '%' + @GroupType + '%' OR @GroupType IS NULL)	
and(CustomerGroupID <>0)
order by CustomerGroupName 
";

                SqlCommand objCommCustomerGroup = new SqlCommand();
                objCommCustomerGroup.Connection = currConn;
                objCommCustomerGroup.CommandText = sqlText;
                objCommCustomerGroup.CommandType = CommandType.Text;

                if (!objCommCustomerGroup.Parameters.Contains("@CustomerGroupID"))
                { objCommCustomerGroup.Parameters.AddWithValue("@CustomerGroupID", CustomerGroupID); }
                else { objCommCustomerGroup.Parameters["@CustomerGroupID"].Value = CustomerGroupID; }

                if (!objCommCustomerGroup.Parameters.Contains("@CustomerGroupName"))
                { objCommCustomerGroup.Parameters.AddWithValue("@CustomerGroupName", CustomerGroupName); }
                else { objCommCustomerGroup.Parameters["@CustomerGroupName"].Value = CustomerGroupName; }

                if (!objCommCustomerGroup.Parameters.Contains("@CustomerGroupDescription"))
                { objCommCustomerGroup.Parameters.AddWithValue("@CustomerGroupDescription", CustomerGroupDescription); }
                else { objCommCustomerGroup.Parameters["@CustomerGroupDescription"].Value = CustomerGroupDescription; }
                // Common Filed

                if (!objCommCustomerGroup.Parameters.Contains("@ActiveStatus"))
                { objCommCustomerGroup.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommCustomerGroup.Parameters["@ActiveStatus"].Value = ActiveStatus; }

                if (!objCommCustomerGroup.Parameters.Contains("@GroupType"))
                { objCommCustomerGroup.Parameters.AddWithValue("@GroupType", GroupType); }
                else { objCommCustomerGroup.Parameters["@GroupType"].Value = GroupType; }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCustomerGroup);
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
    }
}
