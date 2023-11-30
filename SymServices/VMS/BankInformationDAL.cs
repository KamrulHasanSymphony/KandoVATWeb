using System;
using System.Data.SqlClient;
using System.Data;
using SymViewModel.VMS;
using System.Collections.Generic;
using SymOrdinary;
using System.Linq;
using System.Reflection;


namespace SymServices.VMS
{

    public class BankInformationDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        //==================DropDownAll=================
        public List<BankInformationVM> DropDownAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<BankInformationVM> VMs = new List<BankInformationVM>();
            BankInformationVM vm;
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
'B' Sl, BankID
, BankName
FROM BankInformations
WHERE ActiveStatus='Y'
UNION 
SELECT 
'A' Sl, '-1' BankID
, 'ALL Bank' BankName  
FROM BankInformations
)
AS a
order by Sl,BankName
";



                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new BankInformationVM();
                    vm.BankID = dr["BankID"].ToString(); ;
                    vm.BankName = dr["BankName"].ToString();
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
        public List<BankInformationVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<BankInformationVM> VMs = new List<BankInformationVM>();
            BankInformationVM vm;
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
af.BankID
,af.BankName
FROM BankInformations af
WHERE  1=1 AND af.ActiveStatus = 'Y'
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new BankInformationVM();
                    vm.BankID = dr["BankID"].ToString();
                    vm.BankName = dr["BankName"].ToString();
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
        public List<BankInformationVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<BankInformationVM> VMs = new List<BankInformationVM>();
            BankInformationVM vm;
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
 BankID
,BankCode
,BankName
,BranchName
,AccountNumber
,Address1
,Address2
,Address3
,City
,TelephoneNo
,FaxNo
,Email
,ContactPerson
,ContactPersonDesignation
,ContactPersonTelephone
,ContactPersonEmail
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

FROM BankInformations  
WHERE  1=1 AND IsArchive = 0

";
                if (Id != 0)
                {
                    sqlText += @" and BankID=@BankID";
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

                if (Id != 0)
                {
                    objComm.Parameters.AddWithValue("@BankID", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new BankInformationVM();
                    vm.BankID = dr["BankID"].ToString();
                    vm.BankCode = dr["BankCode"].ToString();
                    vm.BankName = dr["BankName"].ToString();
                    vm.BranchName = dr["BranchName"].ToString();
                    vm.AccountNumber = dr["AccountNumber"].ToString();
                    vm.Address1 = dr["Address1"].ToString();
                    vm.Address2 = dr["Address2"].ToString();
                    vm.Address3 = dr["Address3"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.TelephoneNo = dr["TelephoneNo"].ToString();
                    vm.FaxNo = dr["FaxNo"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.ContactPerson = dr["ContactPerson"].ToString();
                    vm.ContactPersonDesignation = dr["ContactPersonDesignation"].ToString();
                    vm.ContactPersonTelephone = dr["ContactPersonTelephone"].ToString();
                    vm.ContactPersonEmail = dr["ContactPersonEmail"].ToString();
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

        private void ErrorReturn(BankInformationVM vm)
        {
            if(string.IsNullOrWhiteSpace(vm.ContactPerson))
            {
                vm.ContactPerson = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.ContactPersonDesignation))
            {
                vm.ContactPersonDesignation = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.ContactPersonTelephone))
            {
                vm.ContactPersonTelephone = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.ContactPersonEmail))
            {
                vm.ContactPersonEmail = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.ContactPersonEmail)) 
            {
                vm.ContactPersonEmail = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.ContactPersonEmail)) 
            {
                vm.ContactPersonEmail = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.ContactPersonEmail)) 
            {
                vm.ContactPersonEmail = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.ContactPersonEmail)) 
            {
                vm.ContactPersonEmail = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.ContactPersonEmail)) 
            {
                vm.ContactPersonEmail = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.ContactPersonEmail)) 
            {
                vm.ContactPersonEmail = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.ContactPersonEmail)) 
            {
                vm.ContactPersonEmail = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.ContactPersonEmail)) 
            {
                vm.ContactPersonEmail = "-";
            }
        }

        //public string[] InsertToBankInformation(string BankID, string BankName, string BranchName, string AccountNumber, 
        //    string Address1, string Address2, string Address3, string City, string TelephoneNo, string FaxNo, string Email, 
        //    string ContactPerson, string ContactPersonDesignation, string ContactPersonTelephone, string ContactPersonEmail,
        //    string Comments, string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy,
        //    string LastModifiedOn, string BankCode)
        public string[] InsertToBankInformation(BankInformationVM vm)
        {
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            string bankCode = vm.BankCode;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {
                ErrorReturn(vm);
                #region Validation

                if (string.IsNullOrEmpty(vm.BankName))
                {
                    throw new ArgumentNullException("InsertToBankInformation",
                                                    "Please enter bank name.");
                }
                if (string.IsNullOrEmpty(vm.BranchName))
                {
                    throw new ArgumentNullException("InsertToBankInformation",
                                                    "Please enter branch name.");
                }
                if (string.IsNullOrEmpty(vm.AccountNumber))
                {
                    throw new ArgumentNullException("InsertToBankInformation",
                                                    "Please enter Account Number.");
                }


                #endregion Validation

                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Bank") == "Y" ? true : false);
                #endregion settingsValue

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToBankInformation");


                #endregion open connection and transaction

                #region existence check
                string[] cFields = { "BankName", "BranchName" };
                string[] cValues = new string[] { vm.BankName, vm.BranchName };
                var banks = SelectAll(0, cFields, cValues,currConn,transaction);
                if (banks.Count > 0)
                {
                    retResults[1] = "Same branch already exists under the bank";
                    throw new ArgumentNullException("", retResults[1]);
                }

                #endregion

                #region Insert Bank Information

                //sqlText = "select count(distinct BankName) from BankInformations where  BankName='" + BankName +
                //          "' and " + "AccountNumber='" + AccountNumber + "'";
                //SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                //cmdNameExist.Transaction = transaction;
                //int countName = (int)cmdNameExist.ExecuteScalar();
                //if (countName > 0)
                //{

                //    throw new ArgumentNullException("InsertToBankInformation",
                //                                    "Requested Bank Name('" + BankName + "') and Account number('" + AccountNumber + "') is already exist");
                //}

                sqlText = "select isnull(max(cast(BankID as int)),0)+1 FROM  BankInformations";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                int nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {

                    throw new ArgumentNullException("InsertToBankInformation",
                                                    "Unable to create new Bank information Id");
                }
                #region Code
                if (Auto == false)
                {
                    if (string.IsNullOrEmpty(bankCode))
                    {
                        throw new ArgumentNullException("InsertToBankInformation", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        sqlText = "select count(BankID) from BankInformations where  BankCode=@bankCode";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        cmdCodeExist.Parameters.AddWithValue("@bankCode", bankCode);

                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("InsertToCustomer", "Same Bank  Code('" + bankCode + "') already exist");
                        }
                    }
                }
                else
                {
                    bankCode = nextId.ToString();
                }
                #endregion Code

                vm.BankID = nextId.ToString();

                sqlText = "";
                sqlText += "insert into BankInformations";
                sqlText += "(";
                sqlText += "BankID,";
                sqlText += "BankName,";
                sqlText += "BranchName,";
                sqlText += "AccountNumber,";
                sqlText += "Address1,";
                sqlText += "Address2,";
                sqlText += "Address3,";
                sqlText += "City,";
                sqlText += "TelephoneNo,";
                sqlText += "FaxNo,";
                sqlText += "Email,";
                sqlText += "ContactPerson,";
                sqlText += "ContactPersonDesignation,";
                sqlText += "ContactPersonTelephone,";
                sqlText += "ContactPersonEmail,";
                sqlText += "Comments,";
                sqlText += "ActiveStatus,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "LastModifiedBy,";
                sqlText += "LastModifiedOn,";
                sqlText += "BankCode";
                sqlText += ")";
                sqlText += " values(";
                sqlText += "@BankID";
                sqlText += ",@BankName";
                sqlText += ",@BranchName";
                sqlText += ",@AccountNumber";
                sqlText += ",@Address1";
                sqlText += ",@Address2";
                sqlText += ",@Address3";
                sqlText += ",@City";
                sqlText += ",@TelephoneNo";
                sqlText += ",@FaxNo";
                sqlText += ",@Email";
                sqlText += ",@ContactPerson";
                sqlText += ",@ContactPersonDesignation";
                sqlText += ",@ContactPersonTelephone";
                sqlText += ",@ContactPersonEmail";
                sqlText += ",@Comments";
                sqlText += ",@ActiveStatus";
                sqlText += ",@CreatedBy";
                sqlText += ",@CreatedOn";
                sqlText += ",@LastModifiedBy";
                sqlText += ",@LastModifiedOn";
                sqlText += ",@BankCode";
                sqlText += ")";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValue("@BankID", vm.BankID);
                cmdInsert.Parameters.AddWithValue("@BankCode", vm.BankCode ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BankName", vm.BankName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BranchName", vm.BranchName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@AccountNumber", vm.AccountNumber ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Address1", vm.Address1 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Address2", vm.Address2 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Address3", vm.Address3 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);

                transResult = (int)cmdInsert.ExecuteNonQuery();



                #endregion Insert Bank Information




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
                retResults[1] = "Requested Bank Information successfully added";
                retResults[2] = "" + nextId;
                retResults[3] = "" + bankCode;



            }
            #region Catch

            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = vm.BankID; //catch ex
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

            return retResults;
        }

        //public string[] UpdateBankInformation(string BankID, string BankName, string BranchName, string AccountNumber, 
        //    string Address1, string Address2, string Address3, string City, string TelephoneNo, string FaxNo,
        //    string Email, string ContactPerson, string ContactPersonDesignation,
        //    string ContactPersonTelephone, string ContactPersonEmail, string Comments,
        //    string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy,
        //    string LastModifiedOn, string BankCode)
        public string[] UpdateBankInformation(BankInformationVM vm)
        {
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.BankID;

            string bankCode = vm.BankCode;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {
                ErrorReturn(vm);
                #region Validation

                if (string.IsNullOrEmpty(vm.BankID))
                {
                    throw new ArgumentNullException("InsertToBankInformation",
                                                    "Please enter select bank information to update.");
                }
                if (string.IsNullOrEmpty(vm.BankName))
                {
                    throw new ArgumentNullException("InsertToBankInformation",
                                                    "Please enter bank name.");
                }
                if (string.IsNullOrEmpty(vm.BranchName))
                {
                    throw new ArgumentNullException("InsertToBankInformation",
                                                    "Please enter branch name.");
                }
                if (string.IsNullOrEmpty(vm.AccountNumber))
                {
                    throw new ArgumentNullException("InsertToBankInformation",
                                                    "Please enter Account Number.");
                }

                #endregion Validation
                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Bank") == "Y" ? true : false);
                #endregion settingsValue
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("BankInformationTransaction");

                #endregion open connection and transaction

                /*Checking existance of provided bank Id information*/
                sqlText = "select count(BankID) from BankInformations where  BankID=@BankID";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;

                cmdIdExist.Parameters.AddWithValue("@BankID", vm.BankID);

                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateBankInformation", "Could not find requested bank information");

                }
                //sqlText = "select count(distinct BankName) from BankInformations where  BankName='" + BankName +
                //         "' and " + "AccountNumber='" + AccountNumber + "'" +
                //          "and  BankID<>'" + BankID + "'";
                //SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                //cmdNameExist.Transaction = transaction;
                //int countName = (int)cmdNameExist.ExecuteScalar();
                //if (countName > 0)
                //{

                //    throw new ArgumentNullException("UpdateBankInformation",
                //                                    "Requested Bank Name('" + BankName + "') and Account number('" + AccountNumber + "') is already exist");
                //}
                #region Code
                if (Auto == false)
                {
                    if (string.IsNullOrEmpty(bankCode))
                    {
                        throw new ArgumentNullException("UpdateBankInformation", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        sqlText = "select count(BankID) from BankInformations where  BankCode=@BankCode" +
                                  " and BankID <>@BankID";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        cmdCodeExist.Parameters.AddWithValue("@BankCode", vm.BankCode);
                        cmdCodeExist.Parameters.AddWithValue("@BankID", vm.BankID);

                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("UpdateBankInformation", "Same Bank  Code('" + bankCode + "') already exist");
                        }
                    }
                }
                else
                {
                    bankCode = vm.BankID;
                }
                #endregion Code

                #region Update Bank Information



                sqlText = "";
                sqlText = "update BankInformations set";

                sqlText += "  BankName                  =@BankName";
                sqlText += " ,BranchName                =@BranchName";
                sqlText += " ,AccountNumber             =@AccountNumber";
                sqlText += " ,Address1                  =@Address1";
                sqlText += " ,Address2                  =@Address2";
                sqlText += " ,Address3                  =@Address3";
                sqlText += " ,City                      =@City";
                sqlText += " ,TelephoneNo               =@TelephoneNo";
                sqlText += " ,FaxNo                     =@FaxNo";
                sqlText += " ,Email                     =@Email";
                sqlText += " ,ContactPerson             =@ContactPerson";
                sqlText += " ,ContactPersonDesignation  =@ContactPersonDesignation";
                sqlText += " ,ContactPersonTelephone    =@ContactPersonTelephone";
                sqlText += " ,ContactPersonEmail        =@ContactPersonEmail";
                sqlText += " ,Comments                  =@Comments";
                sqlText += " ,ActiveStatus              =@ActiveStatus";
                sqlText += " ,LastModifiedBy            =@LastModifiedBy";
                sqlText += " ,LastModifiedOn            =@LastModifiedOn";
                sqlText += " ,BankCode                  =@BankCode";

                sqlText += " where BankID               =@BankID";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@BankName", vm.BankName);
                cmdUpdate.Parameters.AddWithValue("@BranchName", vm.BranchName ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@AccountNumber", vm.AccountNumber ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Address1", vm.Address1 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Address2", vm.Address2 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Address3", vm.Address3 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@BankCode", vm.BankCode ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@BankID", vm.BankID ?? Convert.DBNull);


                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #endregion Update Bank Information

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Bank Information successfully updated";
                        retResults[2] = vm.BankID;
                        retResults[3] = bankCode;
                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to Update Bank";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to Update Bank";
                }



                #endregion Commit

            }
            #region Catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = vm.BankID; //catch ex

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

            return retResults;
        }

        public string[] DeleteBankInformation(string BankID, string databaseName)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = BankID;

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(BankID))
                {
                    throw new ArgumentNullException("InsertToBankInformation",
                                "Could not find requested Bank Id.");
                }
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                sqlText = "select count(BankID) from BankInformations where BankID=@BankID";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValue("@BankID", BankID);

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested Bank Information.";
                    return retResults;
                }

                sqlText = "delete BankInformations where BankID=@BankID";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                cmdDelete.Parameters.AddWithValue("@BankID", BankID);
                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested Bank Information successfully deleted";
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

        public DataTable SearchBankDT(string BankCode, string BankName, string BranchName, string AccountNumber,
            string City, string TelephoneNo, string FaxNo, string Email, string ContactPerson,
            string ContactPersonDesignation, string ContactPersonTelephone, string ContactPersonEmail,
            string ActiveStatus)
        {

            //string[] retResults = new string[2];
            //retResults[0] = "Fail";
            //retResults[1] = "Fail";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";


            DataTable dataTable = new DataTable("Bank");

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                sqlText = @"SELECT 
                            BankID,
                            isnull(BankName,'N/A')BankName,
                            isnull(BranchName,'N/A')BranchName,
                            isnull(AccountNumber,'N/A')AccountNumber,
                            isnull(Address1,'N/A')Address1,
                            isnull(Address2,'N/A')Address2,
                            isnull(Address3,'N/A')Address3,
                            isnull(City,'N/A')City,
                            isnull(TelephoneNo,'N/A')TelephoneNo,
                            isnull(FaxNo,'N/A')FaxNo,
                            isnull(Email,'N/A')Email,
                            isnull(ContactPerson,'N/A')ContactPerson,
                            isnull(ContactPersonDesignation,'N/A')ContactPersonDesignation,
                            isnull(ContactPersonTelephone,'N/A')ContactPersonTelephone ,
                            isnull(ContactPersonEmail,'N/A')ContactPersonEmail,
                            isnull(Comments,'N/A')Comments,
                            isnull(BankCode,'N/A')BankCode,
                            isnull(ActiveStatus,'N')ActiveStatus
                            FROM dbo.BankInformations
                            WHERE (BankCode LIKE '%' + @BankCode + '%' OR @BankCode IS NULL) 
                            AND (BankName LIKE '%' + @BankName + '%' OR @BankName IS NULL)
                            AND (BranchName LIKE '%' + @BranchName	 + '%' OR @BranchName	 IS NULL) 
                            AND (AccountNumber LIKE '%' + @AccountNumber	 + '%' OR @AccountNumber	 IS NULL)
                            AND (City LIKE '%' + @City + '%' OR @City IS NULL) 
                            AND (TelephoneNo LIKE '%' + @TelephoneNo + '%' OR @TelephoneNo IS NULL) 
                            AND (FaxNo LIKE '%' + @FaxNo + '%' OR @FaxNo IS NULL) 
                            AND (Email LIKE '%' + @Email + '%' OR @Email IS NULL) 
                            AND (ContactPerson LIKE '%' + @ContactPerson + '%' OR @ContactPerson IS NULL)
                            AND (ContactPersonDesignation LIKE '%' + @ContactPersonDesignation + '%' OR @ContactPersonDesignation IS NULL)
                            AND (ContactPersonTelephone LIKE '%' + @ContactPersonTelephone + '%' OR @ContactPersonTelephone IS NULL)
                            AND (ContactPersonEmail LIKE '%' + @ContactPersonEmail + '%' OR @ContactPersonEmail IS NULL)
                            AND (ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
                            order by BankName
                            --order by CAST ( BankID AS numeric(18, 0) )";

                SqlCommand objCommBankInformation = new SqlCommand();
                objCommBankInformation.Connection = currConn;
                objCommBankInformation.CommandText = sqlText;
                objCommBankInformation.CommandType = CommandType.Text;

                if (!objCommBankInformation.Parameters.Contains("@BankCode"))
                { objCommBankInformation.Parameters.AddWithValue("@BankCode", BankCode); }
                else { objCommBankInformation.Parameters["@BankCode"].Value = BankCode; }
                if (!objCommBankInformation.Parameters.Contains("@BankName"))
                { objCommBankInformation.Parameters.AddWithValue("@BankName", BankName); }
                else { objCommBankInformation.Parameters["@BankName"].Value = BankName; }
                if (!objCommBankInformation.Parameters.Contains("@BranchName"))
                { objCommBankInformation.Parameters.AddWithValue("@BranchName", BranchName); }
                else { objCommBankInformation.Parameters["@BranchName"].Value = BranchName; }
                if (!objCommBankInformation.Parameters.Contains("@AccountNumber"))
                { objCommBankInformation.Parameters.AddWithValue("@AccountNumber", AccountNumber); }
                else { objCommBankInformation.Parameters["@AccountNumber"].Value = AccountNumber; }
                if (!objCommBankInformation.Parameters.Contains("@City"))
                { objCommBankInformation.Parameters.AddWithValue("@City", City); }
                else { objCommBankInformation.Parameters["@City"].Value = City; }
                if (!objCommBankInformation.Parameters.Contains("@TelephoneNo"))
                { objCommBankInformation.Parameters.AddWithValue("@TelephoneNo", TelephoneNo); }
                else { objCommBankInformation.Parameters["@TelephoneNo"].Value = TelephoneNo; }
                if (!objCommBankInformation.Parameters.Contains("@FaxNo"))
                { objCommBankInformation.Parameters.AddWithValue("@FaxNo", FaxNo); }
                else { objCommBankInformation.Parameters["@FaxNo"].Value = FaxNo; }
                if (!objCommBankInformation.Parameters.Contains("@Email"))
                { objCommBankInformation.Parameters.AddWithValue("@Email", Email); }
                else { objCommBankInformation.Parameters["@Email"].Value = Email; }
                if (!objCommBankInformation.Parameters.Contains("@ContactPerson"))
                { objCommBankInformation.Parameters.AddWithValue("@ContactPerson", ContactPerson); }
                else { objCommBankInformation.Parameters["@ContactPerson"].Value = ContactPerson; }
                if (!objCommBankInformation.Parameters.Contains("@ContactPersonDesignation"))
                { objCommBankInformation.Parameters.AddWithValue("@ContactPersonDesignation", ContactPersonDesignation); }
                else { objCommBankInformation.Parameters["@ContactPersonDesignation"].Value = ContactPersonDesignation; }
                if (!objCommBankInformation.Parameters.Contains("@ContactPersonTelephone"))
                { objCommBankInformation.Parameters.AddWithValue("@ContactPersonTelephone", ContactPersonTelephone); }
                else { objCommBankInformation.Parameters["@ContactPersonTelephone"].Value = ContactPersonTelephone; }
                if (!objCommBankInformation.Parameters.Contains("@ContactPersonEmail"))
                { objCommBankInformation.Parameters.AddWithValue("@ContactPersonEmail", ContactPersonEmail); }
                else { objCommBankInformation.Parameters["@ContactPersonEmail"].Value = ContactPersonEmail; }
                if (!objCommBankInformation.Parameters.Contains("@ActiveStatus"))
                { objCommBankInformation.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommBankInformation.Parameters["@ActiveStatus"].Value = ActiveStatus; }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommBankInformation);

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


        public string[] Delete(BankInformationVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteBank"; //Method Name
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
                        sqlText = "update BankInformations set";
                        sqlText += " ActiveStatus=@ActiveStatus";
                        sqlText += " ,LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where BankID=@BankID";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@BankID", ids[i]);
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
                        throw new ArgumentNullException("Bank Delete", vm.BankID + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Bank Information Delete", "Could not found any item.");
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
