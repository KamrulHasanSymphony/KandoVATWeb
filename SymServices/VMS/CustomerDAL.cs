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
    public class CustomerDAL
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        public List<CustomerVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<CustomerVM> VMs = new List<CustomerVM>();
            CustomerVM vm;
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
af.CustomerID
,af.CustomerName
FROM Customers af
WHERE  1=1 AND af.ActiveStatus = 'Y'
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CustomerVM();
                    vm.CustomerID = dr["CustomerID"].ToString();
                    vm.CustomerName = dr["CustomerName"].ToString();
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

        static string[] columnName = new string[] { "Customer Code", "Customer Name", "City", "Contact Person", "VAT Registration No" };
        public IEnumerable<object> GetCustomerColumn()
        {
            IEnumerable<object> enumList = from e in columnName
                                           select new { Id = e.ToString().Replace(" ", ""), Name = e.ToString() };
            return enumList;
        }

        public List<CustomerVM> DropDownByGroup(string groupId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<CustomerVM> VMs = new List<CustomerVM>();
            CustomerVM vm;
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
af.CustomerID
,af.CustomerName
FROM Customers af
WHERE  1=1 AND af.ActiveStatus = 'Y' AND af.CustomerGroupID=@groupId
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                objComm.Parameters.AddWithValueAndNullHandle("@groupId", groupId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CustomerVM();
                    vm.CustomerID = dr["CustomerID"].ToString();
                    vm.CustomerName = dr["CustomerName"].ToString();
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
        public List<CustomerVM> SelectAll(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<CustomerVM> VMs = new List<CustomerVM>();
            CustomerVM vm;
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
 CustomerID
,CustomerCode
,CustomerName
,CustomerGroupID
,Address1
,Address2
,Address3
,City
,TelephoneNo
,FaxNo
,Email
,StartDateTime
,ContactPerson
,ContactPersonDesignation
,ContactPersonTelephone
,ContactPersonEmail
,TINNo
,VATRegistrationNo
,Comments
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,Info2
,Info3
,Info4
,Info5
,Country
,ISNULL(VDSPercent,0) VDSPercent 
,BusinessType
,BusinessCode

FROM Customers  
WHERE  1=1 AND IsArchive = 0

";
                if (Id != null)
                {
                    sqlText += @" and CustomerID=@CustomerID";
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

                if (Id != null)
                {
                    objComm.Parameters.AddWithValue("@CustomerID", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CustomerVM();
                    vm.CustomerID = dr["CustomerID"].ToString();

                    vm.CustomerCode = dr["CustomerCode"].ToString();
                    vm.CustomerName = dr["CustomerName"].ToString();
                    vm.CustomerGroupID = dr["CustomerGroupID"].ToString();
                    vm.Address1 = dr["Address1"].ToString();
                    vm.Address2 = dr["Address2"].ToString();
                    vm.Address3 = dr["Address3"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.TelephoneNo = dr["TelephoneNo"].ToString();
                    vm.FaxNo = dr["FaxNo"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.StartDateTime = dr["StartDateTime"].ToString();
                    vm.ContactPerson = dr["ContactPerson"].ToString();
                    vm.ContactPersonDesignation = dr["ContactPersonDesignation"].ToString();
                    vm.ContactPersonTelephone = dr["ContactPersonTelephone"].ToString();
                    vm.ContactPersonEmail = dr["ContactPersonEmail"].ToString();
                    vm.TINNo = dr["TINNo"].ToString();
                    vm.VATRegistrationNo = dr["VATRegistrationNo"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.Info2 = dr["Info2"].ToString();
                    vm.Info3 = dr["Info3"].ToString();
                    vm.Info4 = dr["Info4"].ToString();
                    vm.Info5 = dr["Info5"].ToString();
                    vm.Country = dr["Country"].ToString();
                    vm.VDSPercent = Convert.ToDecimal(dr["VDSPercent"]);
                    vm.BusinessType = dr["BusinessType"].ToString();
                    vm.BusinessCode = dr["BusinessCode"].ToString();

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

        //=======================InsertToCustomer==========================
        //public string[] InsertToCustomerNew(string CustomerID, string CustomerName, string CustomerGroupID, string Address1, string Address2, string Address3, string City, string TelephoneNo, string FaxNo, string Email, string StartDateTime, string ContactPerson, string ContactPersonDesignation, string ContactPersonTelephone, string ContactPersonEmail, string TINNo,
        //    string VATRegistrationNo, string Comments, string ActiveStatus, string CreatedBy,
        //    string CreatedOn, string LastModifiedBy, string LastModifiedOn, string Country,
        //    string CustomerCode,string BusinessType, string BusinessCode)
        public string[] InsertToCustomerNew(CustomerVM vm)
        {
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
            string customerCode = vm.CustomerCode;
            int nextId = 0;
            #region Try
            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.CustomerName))
                {
                    throw new ArgumentNullException("InsertToCustomer",
                                                    "Please enter customer group name.");
                }
                if (string.IsNullOrEmpty(vm.CustomerGroupID))
                {
                    throw new ArgumentNullException("UpdateToCustomer",
                                                    "Invalid Customer GroupID type.");
                }

                #endregion Validation
                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Customer") == "Y" ? true : false);
                #endregion settingsValue

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToCustomer");

                #endregion open connection and transaction
                #region Customer  name existence checking

                //select @Present = count(CustomerID) from Customers where CustomerID = @CustomerID;
                //sqlText = "select count(CustomerID) from Customers where  CustomerName='" + CustomerName + "'" +
                //          " and CustomerGroupID='" + CustomerGroupID + "'";
                //SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                //cmdNameExist.Transaction = transaction;
                //countId = (int)cmdNameExist.ExecuteScalar();
                //if (countId > 0)
                //{
                //    throw new ArgumentNullException("InsertToCustomer", "Same customer  name('" + CustomerName + "') already exist under same Group");
                //}

                #endregion Customer Group name existence checking
                #region Customer  new id generation
                sqlText = "select isnull(max(cast(CustomerID as int)),0)+1 FROM  Customers";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("InsertToCustomer",
                                                    "Unable to create new Customer No");
                }
                #region Code
                if (Auto == false)
                {
                    if (string.IsNullOrEmpty(customerCode))
                    {
                        throw new ArgumentNullException("InsertToCustomer", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        sqlText = "select count(CustomerID) from Customers where  CustomerCode=@customerCode";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        cmdCodeExist.Parameters.AddWithValue("@customerCode", customerCode);

                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("InsertToCustomer", "Same customer  Code('" + customerCode + "') already exist");
                        }
                    }
                }
                else
                {
                    customerCode = nextId.ToString();
                }
                #endregion Code

                #endregion Customer  new id generation

                #region Inser new customer
                sqlText = "";

                sqlText += @" 
INSERT INTO Customers(
 CustomerID
,CustomerCode
,CustomerName
,CustomerGroupID
,Address1
,Address2
,Address3
,City
,TelephoneNo
,FaxNo
,Email
,StartDateTime
,ContactPerson
,ContactPersonDesignation
,ContactPersonTelephone
,ContactPersonEmail
,TINNo
,VATRegistrationNo
,Comments
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,Country
,VDSPercent
,BusinessType
,BusinessCode
) VALUES (
 @CustomerID
,@CustomerCode
,@CustomerName
,@CustomerGroupID
,@Address1
,@Address2
,@Address3
,@City
,@TelephoneNo
,@FaxNo
,@Email
,@StartDateTime
,@ContactPerson
,@ContactPersonDesignation
,@ContactPersonTelephone
,@ContactPersonEmail
,@TINNo
,@VATRegistrationNo
,@Comments
,@ActiveStatus
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn
,@Country
,@VDSPercent
,@BusinessType
,@BusinessCode     
) 
";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@CustomerID", nextId.ToString());

                cmdInsert.Parameters.AddWithValue("@CustomerCode", customerCode);
                cmdInsert.Parameters.AddWithValue("@CustomerName", vm.CustomerName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CustomerGroupID", vm.CustomerGroupID);
                cmdInsert.Parameters.AddWithValue("@Address1", vm.Address1 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Address2", vm.Address2 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Address3", vm.Address3 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@StartDateTime", Ordinary.DateToDate(vm.StartDateTime));
                cmdInsert.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VATRegistrationNo", vm.VATRegistrationNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@TINNo", vm.TINNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VDSPercent", vm.VDSPercent);
                cmdInsert.Parameters.AddWithValue("@BusinessType", vm.BusinessType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BusinessCode", vm.BusinessCode ?? Convert.DBNull);
                transResult = (int)cmdInsert.ExecuteNonQuery();

                if (transResult > 0)
                {
                    CustomerAddressVM cvm = new CustomerAddressVM();
                    cvm.CustomerID = nextId.ToString();
                    cvm.CustomerAddress = vm.Address1;
                    retResults = InsertToCustomerAddress(cvm, currConn, transaction);
                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException("", "SQL:" + sqlText);//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                    }

                }

                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested customer  Information successfully Added";
                        retResults[2] = "" + nextId;
                        retResults[3] = "" + customerCode;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected erro to add customer";
                        retResults[2] = "";
                        retResults[3] = "";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected erro to add customer ";
                    retResults[2] = "";
                    retResults[3] = "";
                }

                #endregion Commit


                #endregion Inser new customer

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

        //=======================UpdateToCustomer=========================
        //public string[] UpdateToCustomerNew(string CustomerID, string CustomerName, string CustomerGroupID, string Address1,
        //    string Address2, string Address3, string City, string TelephoneNo, string FaxNo, string Email, string StartDateTime,
        //    string ContactPerson, string ContactPersonDesignation, string ContactPersonTelephone, string ContactPersonEmail, string TINNo,
        //    string VATRegistrationNo, string Comments, string ActiveStatus, string LastModifiedBy,
        //    string LastModifiedOn, string Country, string CustomerCode, string BusinessType, string BusinessCode)
        public string[] UpdateToCustomerNew(CustomerVM vm)
        {
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.CustomerID;

            string customerCode = vm.CustomerCode;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
                int nextId = 0;

            #region Try
            try
            {
                #region Validation
                if (string.IsNullOrEmpty(vm.CustomerID))
                {
                    throw new ArgumentNullException("UpdateToCustomers",
                                                    "Invalid Customer ID");
                }
                if (string.IsNullOrEmpty(vm.CustomerName))
                {
                    throw new ArgumentNullException("UpdateToCustomers",
                                                    "Invalid Customer Name.");
                }
                if (string.IsNullOrEmpty(vm.TelephoneNo))
                {
                    throw new ArgumentNullException("UpdateToCustomers",
                                                    "Please enter customer TelephoneNo");
                }


                #endregion Validation
                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Customer") == "Y" ? true : false);
                #endregion settingsValue

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UpdateToCustomers");

                #endregion open connection and transaction

                #region Customer  existence checking

                sqlText = "select count(CustomerID) from Customers where  CustomerID=@CustomerID";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;
                cmdIdExist.Parameters.AddWithValue("@CustomerID", vm.CustomerID);

                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateToCustomers",
                                "Could not find requested customers  id.");
                }

                #region Code
                if (Auto == false)
                {
                    if (string.IsNullOrEmpty(customerCode))
                    {
                        throw new ArgumentNullException("InsertToCustomer", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        sqlText = "select count(CustomerID) from Customers where  CustomerCode=@CustomerCode" +
                                  "and CustomerID <>@CustomerID";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        cmdCodeExist.Parameters.AddWithValue("@CustomerCode", vm.CustomerCode);
                        cmdCodeExist.Parameters.AddWithValue("@CustomerID", vm.CustomerID);

                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("InsertToCustomer", "Same customer  Code('" + customerCode + "') already exist");
                        }
                    }
                }
                else
                {
                    customerCode = vm.CustomerID;
                }
                #endregion Code
                #endregion Customer Group existence checking

                #region Customer  name existence checking
                //sqlText = "select count(CustomerName) from Customers ";
                //sqlText += " where  CustomerName='" + CustomerName + "'";
                //sqlText += " and CustomerGroupID ='" + CustomerGroupID + "'" +
                //           " and CustomerID <>'" + CustomerID + "'";
                //SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                //cmdNameExist.Transaction = transaction;
                //countId = (int)cmdNameExist.ExecuteScalar();
                //if (countId > 0)
                //{
                //    throw new ArgumentNullException("UpdateToCustomers",
                //                                    "Same customer name already exist");
                //}
                #endregion Customer  name existence checking



                #region Update new customer group
                sqlText = "";
                sqlText = "update Customers set";
                sqlText += "  CustomerCode              =@CustomerCode";
                sqlText += " ,CustomerName              =@CustomerName";
                sqlText += " ,CustomerGroupID           =@CustomerGroupID";
                sqlText += " ,Address1                  =@Address1";
                sqlText += " ,Address2                  =@Address2";
                sqlText += " ,Address3                  =@Address3";
                sqlText += " ,City                      =@City";
                sqlText += " ,TelephoneNo               =@TelephoneNo";
                sqlText += " ,FaxNo                     =@FaxNo";
                sqlText += " ,Email                     =@Email";
                sqlText += " ,StartDateTime             =@StartDateTime";
                sqlText += " ,ContactPerson             =@ContactPerson";
                sqlText += " ,ContactPersonDesignation  =@ContactPersonDesignation";
                sqlText += " ,ContactPersonTelephone    =@ContactPersonTelephone";
                sqlText += " ,ContactPersonEmail        =@ContactPersonEmail";
                sqlText += " ,VATRegistrationNo         =@VATRegistrationNo";
                sqlText += " ,TINNo                     =@TINNo";
                sqlText += " ,Comments                  =@Comments";
                sqlText += " ,ActiveStatus              =@ActiveStatus";
                sqlText += " ,LastModifiedBy            =@LastModifiedBy";
                sqlText += " ,LastModifiedOn            =@LastModifiedOn";
                sqlText += " ,Country                   =@Country";
                sqlText += " ,VDSPercent                =@VDSPercent";
                sqlText += " ,BusinessType              =@BusinessType";
                sqlText += " ,BusinessCode              =@BusinessCode";
                sqlText += " WHERE CustomerID           =@CustomerID";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@CustomerCode", vm.CustomerCode);
                cmdUpdate.Parameters.AddWithValue("@CustomerName", vm.CustomerName);
                cmdUpdate.Parameters.AddWithValue("@CustomerGroupID", vm.CustomerGroupID);
                cmdUpdate.Parameters.AddWithValue("@Address1", vm.Address1 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Address2", vm.Address2 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Address3", vm.Address3 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@StartDateTime", vm.StartDateTime ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@VATRegistrationNo", vm.VATRegistrationNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@TINNo", vm.TINNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@VDSPercent", vm.VDSPercent);
                cmdUpdate.Parameters.AddWithValue("@BusinessType", vm.BusinessType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@BusinessCode", vm.BusinessCode ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@CustomerID", vm.CustomerID ?? Convert.DBNull);

                transResult = (int)cmdUpdate.ExecuteNonQuery();


                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested customers  Information successfully Update";
                        retResults[2] = vm.CustomerID;
                        retResults[3] = customerCode;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update customers ";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update customer group";
                }

                #endregion Commit


                #endregion

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
        //=======================DeleteCustomer==========================
        public string[] DeleteCustomerNew(string CustomerID, string databaseName)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = CustomerID;

            SqlConnection currConn = null;
            //int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(CustomerID))
                {
                    throw new ArgumentNullException("DeleteCustomer",
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


                sqlText = "select count(CustomerID) from Customers where CustomerID=@CustomerID";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValue("@CustomerID", CustomerID);

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested customer.";
                    return retResults;
                }

                sqlText = "delete Customers where CustomerID=@CustomerID";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                cmdDelete.Parameters.AddWithValue("@CustomerID", CustomerID);

                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested customer  Information successfully deleted";
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

        public string[] Delete(CustomerVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteCustomer"; //Method Name
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
                        sqlText = "update Customers set";
                        sqlText += " ActiveStatus=@ActiveStatus";
                        sqlText += " ,LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where CustomerID=@CustomerID";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@CustomerID", ids[i]);
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
                        throw new ArgumentNullException("Customer Delete", vm.CustomerID + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Customer Information Delete", "Could not found any item.");
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

        //========================SearchCustomerInformation===============
        public DataTable SearchCustomer(string CustomerCode, string CustomerName, string CustomerGroupID, string CustomerGroupName,
        string City, string TelephoneNo, string FaxNo, string Email, string StartDateFrom, string StartDateTo, string ContactPerson,
        string ContactPersonDesignation, string ContactPersonTelephone, string ContactPersonEmail, string TINNo, string VATRegistrationNo,
        string ActiveStatus, string GroupType)
        {
            #region Objects & Variables
            //string Description = "";
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("Customers");
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
Customers.CustomerID,
isnull(Customers.CustomerName,'N/A')CustomerName, 
Customers.CustomerGroupID,
isnull(CustomerGroups.CustomerGroupName,'N/A')CustomerGroupName,
isnull(Customers.Address1,'N/A')Address1,
isnull(Customers.Address2,'N/A')Address2 ,
isnull(Customers.Address3,'N/A')Address3,
isnull(Customers.City,'N/A')City,
isnull(Customers.TelephoneNo,'N/A')TelephoneNo,
isnull(Customers.FaxNo,'N/A')FaxNo ,
isnull(Customers.Email,'N/A')Email ,
convert (varchar,Customers.StartDateTime,120)StartDateTime,
isnull(Customers.ContactPerson,'N/A')ContactPerson,
isnull(Customers.ContactPersonDesignation,'N/A')ContactPersonDesignation,
isnull(Customers.ContactPersonTelephone,'N/A')ContactPersonTelephone ,
isnull(Customers.ContactPersonEmail,'N/A')ContactPersonEmail,
isnull(Customers.TINNo,'N/A')TINNo ,
isnull(Customers.VATRegistrationNo,'N/A')VATRegistrationNo ,
isnull(Customers.Comments,'N/A')Comments,
isnull(Customers.ActiveStatus,'N')ActiveStatus,
isnull(Customers.Country,'N/A')Country,
isnull(Customers.CustomerCode,'N/A')CustomerCode,

isnull(Customers.BusinessCode,'N/A')BusinessCode,
isnull(Customers.BusinessType,'N/A')BusinessType,

isnull(CustomerGroups.GroupType,'N/A')GroupType

FROM Customers  LEFT OUTER JOIN
 CustomerGroups ON  Customers.CustomerGroupID =  CustomerGroups.CustomerGroupID
                 
WHERE 
    (Customers.CustomerCode  LIKE '%' +  @CustomerCode  + '%'  OR @CustomerCode IS NULL) 
AND (Customers.CustomerName LIKE '%' + @CustomerName + '%' OR @CustomerName IS NULL)
AND (Customers.CustomerGroupID LIKE '%' + @CustomerGroupID + '%' OR @CustomerGroupID IS NULL) 
AND (CustomerGroups.CustomerGroupName LIKE '%' + @CustomerGroupName + '%' OR @CustomerGroupName IS NULL)
AND (Customers.City LIKE '%' + @City + '%' OR @City IS NULL) 
AND (Customers.TelephoneNo LIKE '%' + @TelephoneNo + '%' OR @TelephoneNo IS NULL) 
AND (Customers.FaxNo LIKE '%' + @FaxNo + '%' OR @FaxNo IS NULL) 
AND (Customers.Email LIKE '%' + @Email + '%' OR @Email IS NULL) 
AND (Customers.StartDateTime>= @StartDateFrom OR @StartDateFrom IS NULL)
AND (Customers.StartDateTime<= @StartDateTo OR @StartDateTo IS NULL)
AND (Customers.ContactPerson LIKE '%' + @ContactPerson + '%' OR @ContactPerson IS NULL)
AND (Customers.ContactPersonDesignation LIKE '%' + @ContactPersonDesignation + '%' OR @ContactPersonDesignation IS NULL)
AND (Customers.ContactPersonTelephone LIKE '%' + @ContactPersonTelephone + '%' OR @ContactPersonTelephone IS NULL)
AND (Customers.ContactPersonEmail LIKE '%' + @ContactPersonEmail + '%' OR @ContactPersonEmail IS NULL)
AND (Customers.TINNo LIKE '%' + @TINNo + '%' OR @TINNo IS NULL)
AND (Customers.VATRegistrationNo LIKE '%' + @VATRegistrationNo + '%' OR @VATRegistrationNo IS NULL)
AND (Customers.ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
AND (CustomerGroups.GroupType LIKE '%' + @GroupType + '%' OR @GroupType IS NULL)



and  (Customers.CustomerID <>'0') 
 order by  Customers.CustomerName
--order by CAST ( Customers.CustomerID AS numeric(18, 0) )

";

                SqlCommand objCommCustomerInformation = new SqlCommand();
                objCommCustomerInformation.Connection = currConn;
                objCommCustomerInformation.CommandText = sqlText;
                objCommCustomerInformation.CommandType = CommandType.Text;

                if (!objCommCustomerInformation.Parameters.Contains("@GroupType"))
                { objCommCustomerInformation.Parameters.AddWithValue("@GroupType", GroupType); }
                else { objCommCustomerInformation.Parameters["@GroupType"].Value = GroupType; }

                if (!objCommCustomerInformation.Parameters.Contains("@CustomerCode"))
                { objCommCustomerInformation.Parameters.AddWithValue("@CustomerCode", CustomerCode); }
                else { objCommCustomerInformation.Parameters["@CustomerCode"].Value = CustomerCode; }

                if (!objCommCustomerInformation.Parameters.Contains("@CustomerName"))
                { objCommCustomerInformation.Parameters.AddWithValue("@CustomerName", CustomerName); }
                else { objCommCustomerInformation.Parameters["@CustomerName"].Value = CustomerName; }
                if (!objCommCustomerInformation.Parameters.Contains("@CustomerGroupID"))
                { objCommCustomerInformation.Parameters.AddWithValue("@CustomerGroupID", CustomerGroupID); }
                else { objCommCustomerInformation.Parameters["@CustomerGroupID"].Value = CustomerGroupID; }
                if (!objCommCustomerInformation.Parameters.Contains("@CustomerGroupName"))
                { objCommCustomerInformation.Parameters.AddWithValue("@CustomerGroupName", CustomerGroupName); }
                else { objCommCustomerInformation.Parameters["@CustomerGroupName"].Value = CustomerGroupName; }
                if (!objCommCustomerInformation.Parameters.Contains("@City"))
                { objCommCustomerInformation.Parameters.AddWithValue("@City", City); }
                else { objCommCustomerInformation.Parameters["@City"].Value = City; }
                if (!objCommCustomerInformation.Parameters.Contains("@TelephoneNo"))
                { objCommCustomerInformation.Parameters.AddWithValue("@TelephoneNo", TelephoneNo); }
                else { objCommCustomerInformation.Parameters["@TelephoneNo"].Value = TelephoneNo; }
                if (!objCommCustomerInformation.Parameters.Contains("@FaxNo"))
                { objCommCustomerInformation.Parameters.AddWithValue("@FaxNo", FaxNo); }
                else { objCommCustomerInformation.Parameters["@FaxNo"].Value = FaxNo; }
                if (!objCommCustomerInformation.Parameters.Contains("@Email"))
                { objCommCustomerInformation.Parameters.AddWithValue("@Email", Email); }
                else { objCommCustomerInformation.Parameters["@Email"].Value = Email; }
                if (StartDateFrom == "")
                {
                    if (!objCommCustomerInformation.Parameters.Contains("@StartDateFrom"))
                    { objCommCustomerInformation.Parameters.AddWithValue("@StartDateFrom", System.DBNull.Value); }
                    else { objCommCustomerInformation.Parameters["@StartDateFrom"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommCustomerInformation.Parameters.Contains("@StartDateFrom"))
                    { objCommCustomerInformation.Parameters.AddWithValue("@StartDateFrom", StartDateFrom); }

                    else { objCommCustomerInformation.Parameters["@StartDateFrom"].Value = StartDateFrom; }
                }
                if (StartDateTo == "")
                {
                    if (!objCommCustomerInformation.Parameters.Contains("@StartDateTo"))
                    { objCommCustomerInformation.Parameters.AddWithValue("@StartDateTo", System.DBNull.Value); }
                    else { objCommCustomerInformation.Parameters["@StartDateTo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommCustomerInformation.Parameters.Contains("@StartDateTo"))
                    { objCommCustomerInformation.Parameters.AddWithValue("@StartDateTo", StartDateTo); }
                    else { objCommCustomerInformation.Parameters["@StartDateTo"].Value = StartDateTo; }
                }
                if (!objCommCustomerInformation.Parameters.Contains("@ContactPerson"))
                { objCommCustomerInformation.Parameters.AddWithValue("@ContactPerson", ContactPerson); }
                else { objCommCustomerInformation.Parameters["@ContactPerson"].Value = ContactPerson; }
                if (!objCommCustomerInformation.Parameters.Contains("@ContactPersonDesignation"))
                { objCommCustomerInformation.Parameters.AddWithValue("@ContactPersonDesignation", ContactPersonDesignation); }
                else { objCommCustomerInformation.Parameters["@ContactPersonDesignation"].Value = ContactPersonDesignation; }
                if (!objCommCustomerInformation.Parameters.Contains("@ContactPersonTelephone"))
                { objCommCustomerInformation.Parameters.AddWithValue("@ContactPersonTelephone", ContactPersonTelephone); }
                else { objCommCustomerInformation.Parameters["@ContactPersonTelephone"].Value = ContactPersonTelephone; }
                if (!objCommCustomerInformation.Parameters.Contains("@ContactPersonEmail"))
                { objCommCustomerInformation.Parameters.AddWithValue("@ContactPersonEmail", ContactPersonEmail); }
                else { objCommCustomerInformation.Parameters["@ContactPersonEmail"].Value = ContactPersonEmail; }
                if (!objCommCustomerInformation.Parameters.Contains("@TINNo"))
                { objCommCustomerInformation.Parameters.AddWithValue("@TINNo", TINNo); }
                else { objCommCustomerInformation.Parameters["@TINNo"].Value = TINNo; }
                if (!objCommCustomerInformation.Parameters.Contains("@VATRegistrationNo"))
                { objCommCustomerInformation.Parameters.AddWithValue("@VATRegistrationNo", VATRegistrationNo); }
                else { objCommCustomerInformation.Parameters["@VATRegistrationNo"].Value = VATRegistrationNo; }

                if (!objCommCustomerInformation.Parameters.Contains("@ActiveStatus"))
                { objCommCustomerInformation.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommCustomerInformation.Parameters["@ActiveStatus"].Value = ActiveStatus; }


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCustomerInformation);
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

        public DataTable SearchCustomerSingleDTNew(string CustomerCode, string CustomerName, string CustomerGroupID,
            string CustomerGroupName, string TINNo, string VATRegistrationNo, string ActiveStatus, string GroupType)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("Customer");

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
Customers.CustomerID,
isnull(Customers.CustomerName,'N/A')CustomerName, 
Customers.CustomerGroupID,
isnull(CustomerGroups.CustomerGroupName,'N/A')CustomerGroupName,
isnull(Customers.Address1,'N/A')Address1,
isnull(Customers.Address2,'N/A')Address2 ,
isnull(Customers.Address3,'N/A')Address3,
isnull(Customers.City,'N/A')City,

isnull(Customers.TelephoneNo,'N/A')TelephoneNo,
isnull(Customers.FaxNo,'N/A')FaxNo ,
isnull(Customers.Email,'N/A')Email ,
isnull(Customers.TINNo,'N/A')TINNo ,
isnull(Customers.VATRegistrationNo,'N/A')VATRegistrationNo ,
isnull(Customers.ActiveStatus,'N')ActiveStatus,
isnull(Customers.Country,'N')Country,
isnull(Customers.CustomerCode,'N/A')CustomerCode ,

isnull(CustomerGroups.GroupType,'N/A')GroupType


FROM Customers  LEFT OUTER JOIN
 CustomerGroups ON  Customers.CustomerGroupID =  CustomerGroups.CustomerGroupID
                 
WHERE 
    (Customers.CustomerCode  LIKE '%' +  @CustomerCode  + '%'  OR @CustomerCode IS NULL) 
AND (Customers.CustomerName LIKE '%' + @CustomerName + '%' OR @CustomerName IS NULL)
AND (Customers.CustomerGroupID LIKE '%' + @CustomerGroupID + '%' OR @CustomerGroupID IS NULL) 
AND (CustomerGroups.CustomerGroupName LIKE '%' + @CustomerGroupName + '%' OR @CustomerGroupName IS NULL)
AND (Customers.TINNo LIKE '%' + @TINNo + '%' OR @TINNo IS NULL)
AND (Customers.VATRegistrationNo LIKE '%' + @VATRegistrationNo + '%' OR @VATRegistrationNo IS NULL)
AND (Customers.ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
AND (CustomerGroups.GroupType LIKE '%' + @GroupType + '%' OR @GroupType IS NULL)


and  (Customers.CustomerID <>'0') 
 order by  Customers.CustomerName

";

                SqlCommand objCommCustomerInformation = new SqlCommand();
                objCommCustomerInformation.Connection = currConn;
                objCommCustomerInformation.CommandText = sqlText;
                objCommCustomerInformation.CommandType = CommandType.Text;

                if (!objCommCustomerInformation.Parameters.Contains("@GroupType"))
                { objCommCustomerInformation.Parameters.AddWithValue("@GroupType", GroupType); }
                else { objCommCustomerInformation.Parameters["@GroupType"].Value = GroupType; }

                if (!objCommCustomerInformation.Parameters.Contains("@CustomerCode"))
                { objCommCustomerInformation.Parameters.AddWithValue("@CustomerCode", CustomerCode); }
                else { objCommCustomerInformation.Parameters["@CustomerCode"].Value = CustomerCode; }

                if (!objCommCustomerInformation.Parameters.Contains("@CustomerName"))
                { objCommCustomerInformation.Parameters.AddWithValue("@CustomerName", CustomerName); }
                else { objCommCustomerInformation.Parameters["@CustomerName"].Value = CustomerName; }
                if (!objCommCustomerInformation.Parameters.Contains("@CustomerGroupID"))
                { objCommCustomerInformation.Parameters.AddWithValue("@CustomerGroupID", CustomerGroupID); }
                else { objCommCustomerInformation.Parameters["@CustomerGroupID"].Value = CustomerGroupID; }
                if (!objCommCustomerInformation.Parameters.Contains("@CustomerGroupName"))
                { objCommCustomerInformation.Parameters.AddWithValue("@CustomerGroupName", CustomerGroupName); }
                else { objCommCustomerInformation.Parameters["@CustomerGroupName"].Value = CustomerGroupName; }

                if (!objCommCustomerInformation.Parameters.Contains("@TINNo"))
                { objCommCustomerInformation.Parameters.AddWithValue("@TINNo", TINNo); }
                else { objCommCustomerInformation.Parameters["@TINNo"].Value = TINNo; }
                if (!objCommCustomerInformation.Parameters.Contains("@VATRegistrationNo"))
                { objCommCustomerInformation.Parameters.AddWithValue("@VATRegistrationNo", VATRegistrationNo); }
                else { objCommCustomerInformation.Parameters["@VATRegistrationNo"].Value = VATRegistrationNo; }

                if (!objCommCustomerInformation.Parameters.Contains("@ActiveStatus"))
                { objCommCustomerInformation.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommCustomerInformation.Parameters["@ActiveStatus"].Value = ActiveStatus; }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCustomerInformation);
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


        #region OLD Method
        public static string InsertTOCustomer1(SqlCommand objCommCustomer, string CustomerID, string CustomerName, string CustomerGroupID,
        string Address1, string Address2, string Address3, string City, string TelephoneNo, string FaxNo, string Email, DateTime StartDateTime,
        string ContactPerson, string ContactPersonDesignation, string ContactPersonTelephone, string ContactPersonEmail, string TINNo,
        string VATRegistrationNo, string Comments, string ActiveStatus, string CreatedBy, DateTime CreatedOn, string LastModifiedBy, DateTime LastModifiedOn,
        string Country)
        {
            string result = "-1";

            //string strSQL = "SpInsertUpdateCustomer";
            //objCommCustomer.CommandText = strSQL;
            //objCommCustomer.CommandType = ;

            string strSQL = @"
declare @Present numeric;

select @Present = count(CustomerID) from Customers 
where  CustomerID=@CustomerID;
if(@Present <=0 )
BEGIN
select @Present = count(distinct CustomerName) from Customers 
where  CustomerName=@CustomerName;

if(@Present >0 )
select -900;
else
BEGIN

select @CustomerID= isnull(max(cast(CustomerID as int)),0)+1 FROM  Customers;

insert into Customers(
CustomerID,
CustomerName,
CustomerGroupID,
Address1,
Address2,
Address3,
City,
TelephoneNo,
FaxNo,
Email,
StartDateTime,
ContactPerson,
ContactPersonDesignation,
ContactPersonTelephone,
ContactPersonEmail,
TINNo,
VATRegistrationNo,
Comments,
ActiveStatus,
CreatedBy,
CreatedOn,
LastModifiedBy,
LastModifiedOn,
Country)
values(
@CustomerID,
@CustomerName,
@CustomerGroupID,
@Address1,
@Address2,
@Address3,
@City,
@TelephoneNo,
@FaxNo,
@Email,
@StartDateTime,
@ContactPerson,
@ContactPersonDesignation,
@ContactPersonTelephone,
@ContactPersonEmail,
@TINNo,
@VATRegistrationNo,
@Comments,
@ActiveStatus,
@CreatedBy,
@CreatedOn,
@LastModifiedBy,
@LastModifiedOn,
@Country)

select @CustomerID;
END

END

else
BEGIN

update Customers set 


CustomerName=@CustomerName,
CustomerGroupID=@CustomerGroupID,
Address1=@Address1,
Address2=@Address2,
Address3=@Address3,
City=@City,
TelephoneNo=@TelephoneNo,
FaxNo=@FaxNo,
Email=@Email,
StartDateTime=@StartDateTime,
ContactPerson=@ContactPerson,
ContactPersonDesignation=@ContactPersonDesignation,
ContactPersonTelephone=@ContactPersonTelephone,
ContactPersonEmail=@ContactPersonEmail,
TINNo=@TINNo,
VATRegistrationNo=@VATRegistrationNo,
Comments=@Comments,
ActiveStatus=@ActiveStatus,
LastModifiedBy=@LastModifiedBy,
LastModifiedOn=@LastModifiedOn,
Country=@Country

where CustomerID=@CustomerID;
select @CustomerID;
END";
            objCommCustomer.CommandText = strSQL;
            objCommCustomer.CommandType = CommandType.Text;


            if (!objCommCustomer.Parameters.Contains("@CustomerID"))
            { objCommCustomer.Parameters.AddWithValue("@CustomerID", CustomerID); }
            else { objCommCustomer.Parameters["@CustomerID"].Value = CustomerID; }
            if (!objCommCustomer.Parameters.Contains("@CustomerName"))
            { objCommCustomer.Parameters.AddWithValue("@CustomerName", CustomerName); }
            else { objCommCustomer.Parameters["@CustomerName"].Value = CustomerName; }
            if (!objCommCustomer.Parameters.Contains("@CustomerGroupID"))
            { objCommCustomer.Parameters.AddWithValue("@CustomerGroupID", CustomerGroupID); }
            else { objCommCustomer.Parameters["@CustomerGroupID"].Value = CustomerGroupID; }
            if (!objCommCustomer.Parameters.Contains("@Address1"))
            { objCommCustomer.Parameters.AddWithValue("@Address1", Address1); }
            else { objCommCustomer.Parameters["@Address1"].Value = Address1; }
            if (!objCommCustomer.Parameters.Contains("@Address2"))
            { objCommCustomer.Parameters.AddWithValue("@Address2", Address2); }
            else { objCommCustomer.Parameters["@Address2"].Value = Address2; }
            if (!objCommCustomer.Parameters.Contains("@Address3"))
            { objCommCustomer.Parameters.AddWithValue("@Address3", Address3); }
            else { objCommCustomer.Parameters["@Address3"].Value = Address3; }
            if (!objCommCustomer.Parameters.Contains("@City"))
            { objCommCustomer.Parameters.AddWithValue("@City", City); }
            else { objCommCustomer.Parameters["@City"].Value = City; }
            if (!objCommCustomer.Parameters.Contains("@TelephoneNo"))
            { objCommCustomer.Parameters.AddWithValue("@TelephoneNo", TelephoneNo); }
            else { objCommCustomer.Parameters["@TelephoneNo"].Value = TelephoneNo; }
            if (!objCommCustomer.Parameters.Contains("@FaxNo"))
            { objCommCustomer.Parameters.AddWithValue("@FaxNo", FaxNo); }
            else { objCommCustomer.Parameters["@FaxNo"].Value = FaxNo; }
            if (!objCommCustomer.Parameters.Contains("@Email"))
            { objCommCustomer.Parameters.AddWithValue("@Email", Email); }
            else { objCommCustomer.Parameters["@Email"].Value = Email; }
            if (!objCommCustomer.Parameters.Contains("@StartDateTime"))
            { objCommCustomer.Parameters.AddWithValue("@StartDateTime", StartDateTime); }
            else { objCommCustomer.Parameters["@StartDateTime"].Value = StartDateTime; }
            if (!objCommCustomer.Parameters.Contains("@ContactPerson"))
            { objCommCustomer.Parameters.AddWithValue("@ContactPerson", ContactPerson); }
            else { objCommCustomer.Parameters["@ContactPerson"].Value = ContactPerson; }
            if (!objCommCustomer.Parameters.Contains("@ContactPersonDesignation"))
            { objCommCustomer.Parameters.AddWithValue("@ContactPersonDesignation", ContactPersonDesignation); }
            else { objCommCustomer.Parameters["@ContactPersonDesignation"].Value = ContactPersonDesignation; }
            if (!objCommCustomer.Parameters.Contains("@ContactPersonTelephone"))
            { objCommCustomer.Parameters.AddWithValue("@ContactPersonTelephone", ContactPersonTelephone); }
            else { objCommCustomer.Parameters["@ContactPersonTelephone"].Value = ContactPersonTelephone; }
            if (!objCommCustomer.Parameters.Contains("@ContactPersonEmail"))
            { objCommCustomer.Parameters.AddWithValue("@ContactPersonEmail", ContactPersonEmail); }
            else { objCommCustomer.Parameters["@ContactPersonEmail"].Value = ContactPersonEmail; }
            if (!objCommCustomer.Parameters.Contains("@TINNo"))
            { objCommCustomer.Parameters.AddWithValue("@TINNo", TINNo); }
            else { objCommCustomer.Parameters["@TINNo"].Value = TINNo; }
            if (!objCommCustomer.Parameters.Contains("@VATRegistrationNo"))
            { objCommCustomer.Parameters.AddWithValue("@VATRegistrationNo", VATRegistrationNo); }
            else { objCommCustomer.Parameters["@VATRegistrationNo"].Value = VATRegistrationNo; }
            if (!objCommCustomer.Parameters.Contains("@Comments"))
            { objCommCustomer.Parameters.AddWithValue("@Comments", Comments); }
            else { objCommCustomer.Parameters["@Comments"].Value = Comments; }
            if (!objCommCustomer.Parameters.Contains("@ActiveStatus"))
            { objCommCustomer.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
            else { objCommCustomer.Parameters["@ActiveStatus"].Value = ActiveStatus; }

            //Common Fields
            if (!objCommCustomer.Parameters.Contains("@CreatedBy"))
            { objCommCustomer.Parameters.AddWithValue("@CreatedBy", CreatedBy); }
            else { objCommCustomer.Parameters["@CreatedBy"].Value = CreatedBy; }
            if (!objCommCustomer.Parameters.Contains("@CreatedOn"))
            { objCommCustomer.Parameters.AddWithValue("@CreatedOn", CreatedOn); }
            else { objCommCustomer.Parameters["@CreatedOn"].Value = CreatedOn; }
            if (!objCommCustomer.Parameters.Contains("@LastModifiedBy"))
            { objCommCustomer.Parameters.AddWithValue("@LastModifiedBy", LastModifiedBy); }
            else { objCommCustomer.Parameters["@LastModifiedBy"].Value = LastModifiedBy; }
            if (!objCommCustomer.Parameters.Contains("@LastModifiedOn"))
            { objCommCustomer.Parameters.AddWithValue("@LastModifiedOn", LastModifiedOn); }
            else { objCommCustomer.Parameters["@LastModifiedOn"].Value = LastModifiedOn; }
            if (!objCommCustomer.Parameters.Contains("@Country"))
            { objCommCustomer.Parameters.AddWithValue("@Country", Country); }
            else { objCommCustomer.Parameters["@Country"].Value = Country; }


            try
            { result = objCommCustomer.ExecuteScalar().ToString(); }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                { return "-99"; }
                else if (ex.Number == 266)
                { return "-266"; }
                else
                {
                    return "-1";
                }
            }
            catch (Exception ex) { Trace.WriteLine(ex.Message); }
            finally { }

            return result;
        }




        #endregion

        //-----------------------

        public DataTable SearchCountry(string customer)
        {
            #region Objects & Variables

            SqlConnection currConn = null;

            string sqlText = "";










            DataTable dataTable = new DataTable();
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

select Country from Customers where [CustomerName]=@customer";

                SqlCommand objCommProductType = new SqlCommand();
                objCommProductType.Connection = currConn;
                objCommProductType.CommandText = sqlText;
                objCommProductType.CommandType = CommandType.Text;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProductType);
                if (!objCommProductType.Parameters.Contains("@customer"))
                { objCommProductType.Parameters.AddWithValue("@customer", customer); }
                else { objCommProductType.Parameters["@customer"].Value = customer; }


                dataAdapter.Fill(dataTable);
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

        //--------------------

        //-----------------------

        public DataTable SearchCustomerName(string customer)
        {
            #region Objects & Variables

            SqlConnection currConn = null;

            string sqlText = "";

            DataTable dataTable = new DataTable();
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

select CustomerName from Customers where [CustomerID]=@customer";

                SqlCommand objCommProductType = new SqlCommand();
                objCommProductType.Connection = currConn;
                objCommProductType.CommandText = sqlText;
                objCommProductType.CommandType = CommandType.Text;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProductType);
                if (!objCommProductType.Parameters.Contains("@customer"))
                { objCommProductType.Parameters.AddWithValue("@customer", customer); }
                else { objCommProductType.Parameters["@customer"].Value = customer; }


                dataAdapter.Fill(dataTable);
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

        public DataTable SearchCustomerByCode(string customerCode)
        {
            #region Objects & Variables

            SqlConnection currConn = null;

            string sqlText = "";

            DataTable dataTable = new DataTable();
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

select * from Customers where [CustomerCode]=@CustomerCode";

                SqlCommand objCommProductType = new SqlCommand();
                objCommProductType.Connection = currConn;
                objCommProductType.CommandText = sqlText;
                objCommProductType.CommandType = CommandType.Text;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProductType);
                if (!objCommProductType.Parameters.Contains("@CustomerCode"))
                { objCommProductType.Parameters.AddWithValue("@CustomerCode", customerCode); }
                else { objCommProductType.Parameters["@CustomerCode"].Value = customerCode; }


                dataAdapter.Fill(dataTable);
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


        //--------------------
        public string[] InsertToCustomerAddress(CustomerAddressVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
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

            #region Try
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

                #region Customer  new id generation
                sqlText = "select isnull(max(cast(Id as int)),0)+1 FROM  CustomersAddress";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                int nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("Customers Address",
                                                    "Unable to create new Customers Address");
                }


                #endregion Customer  new id generation

                #region Inser new customer
                sqlText = "";
                sqlText += "insert into CustomersAddress";
                sqlText += "(";
                sqlText += "Id,";
                sqlText += "CustomerID,";
                sqlText += "CustomerAddress";
                sqlText += ")";
                sqlText += " values(";
                sqlText += " '" + nextId + "',";
                sqlText += " '" + vm.CustomerID + "',";
                sqlText += " '" + vm.CustomerAddress + "'";
                sqlText += ")";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                transResult = (int)cmdInsert.ExecuteNonQuery();

                if (transResult > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested customer  Address successfully Added";
                    retResults[2] = "" + nextId;
                    retResults[3] = "" + nextId;
                }
                #region Commit
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                            retResults[0] = "Success";
                            retResults[1] = "Requested customer  Address successfully Added";
                            retResults[2] = "" + nextId;
                            retResults[3] = "" + nextId;

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected erro to add customer";
                            retResults[2] = "";
                            retResults[3] = "";
                        }

                    }
                    else
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected erro to add customer ";
                        retResults[2] = "";
                        retResults[3] = "";
                    }
                }
                #endregion Commit




                #endregion Commit


                #endregion Inser new customer

            }
            #endregion

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

        public string[] UpdateToCustomerAddress(CustomerAddressVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #region Try
            try
            {
                #region Validation
                if (string.IsNullOrEmpty(vm.Id.ToString()))
                {
                    throw new ArgumentNullException("UpdateToCustomers",
                                                    "Invalid Customer  Address");
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


                #region Customer  existence checking





                #endregion Customer Group existence checking

                #region Update new customer group
                sqlText = "";
                sqlText = "update CustomersAddress set";
                sqlText += " CustomerAddress='" + vm.CustomerAddress + "'";
                sqlText += " where id='" + vm.Id + "'";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                            retResults[0] = "Success";
                            retResults[1] = "Requested customers  Address successfully Update";
                            retResults[2] = vm.Id.ToString();
                            retResults[3] = "";

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected error to update customers ";
                        }

                    }
                    else
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update customer group";
                    }
                }




                #endregion Commit


                #endregion

            }
            #endregion
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
            #region finally
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

        public string[] DeleteCustomerAddress(string CustomerID, string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = Id;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(Id))
                {
                    throw new ArgumentNullException("DeleteCustomer",
                                "Could not find requested Address.");
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


                if (!string.IsNullOrEmpty(Id))
                {
                    sqlText = "delete CustomersAddress where Id='" + Id + "'";
                }
                if (!string.IsNullOrEmpty(CustomerID))
                {
                    sqlText = "delete CustomersAddress where CustomerID='" + CustomerID + "'";
                }
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested customer  Address successfully deleted";
                }
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested customers  Address successfully deleted";
                        retResults[2] = "";

                    }
                }

                #endregion Commit

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

            return retResults;
        }

        public DataTable SearchCustomerAddress(string CustomerID, string Id, string address)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("Customer");

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
isnull(Id,'0')Id
,isnull(CustomerID,'0')CustomerID
,isnull(CustomerAddress,'-')CustomerAddress
FROM CustomersAddress 
WHERE 1=1

";
                if (!string.IsNullOrEmpty(CustomerID))
                {
                    sqlText += @"  and CustomerID=@CustomerID";
                }
                if (!string.IsNullOrEmpty(Id))
                {
                    sqlText += @"  and Id=@Id";
                }

                if (!string.IsNullOrEmpty(address))
                {
                    sqlText += @"  and CustomerAddress like '%" + address + "%'";
                }
                sqlText += @"  order by  CustomerAddress";

                SqlCommand objCommCustomerInformation = new SqlCommand();
                objCommCustomerInformation.Connection = currConn;
                objCommCustomerInformation.CommandText = sqlText;
                objCommCustomerInformation.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(CustomerID))
                {
                    if (!objCommCustomerInformation.Parameters.Contains("@CustomerID"))
                    { objCommCustomerInformation.Parameters.AddWithValue("@CustomerID", CustomerID); }
                    else { objCommCustomerInformation.Parameters["@CustomerID"].Value = CustomerID; }
                }
                if (!string.IsNullOrEmpty(Id))
                {
                    if (!objCommCustomerInformation.Parameters.Contains("@Id"))
                    { objCommCustomerInformation.Parameters.AddWithValue("@Id", Id); }
                    else { objCommCustomerInformation.Parameters["@Id"].Value = Id; }
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCustomerInformation);
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




    }

}
