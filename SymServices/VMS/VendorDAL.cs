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
    public class VendorDAL
    {

        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #region New Mothods

        static string[] columnName = new string[] { "Vendor Code", "Vendor Name", "Address 1", "TIN No", "Contact Person", "VAT Registration No" };
        public IEnumerable<object> GetVendorColumn()
        {
            IEnumerable<object> enumList = from e in columnName
                                           select new { Id = e.ToString().Replace(" ", ""), Name = e.ToString() };
            return enumList;
        }

        //public string[] InsertToVendorNewSQL(string VendorID,
        //                                     string VendorName, string VendorGroupID, string Address1, string Address2,
        //                                     string Address3, string City, string TelephoneNo, string FaxNo,
        //                                     string Email,
        //                                     string StartDateTime, string ContactPerson,
        //                                     string ContactPersonDesignation,
        //                                     string ContactPersonTelephone, string ContactPersonEmail,
        //                                     string VATRegistrationNo, string TINNo,
        //                                     string Comments, string ActiveStatus,
        //                                     string CreatedBy, string CreatedOn, string LastModifiedBy,
        //                                     string LastModifiedOn,
        //string Country, string VendorCode, string VDSPercent, string BusinessType, string BusinessCode)
        public string[] InsertToVendorNewSQL(VendorVM vm)
        {
            #region Variables


            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            string vendorCode = vm.VendorCode;

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

                if (string.IsNullOrEmpty(vm.VendorName))
                {
                    throw new ArgumentNullException("InsertToVendorInformation",
                                                    "Please enter vendor name.");
                }


                #endregion Validation

                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Vendor") == "Y" ? true : false);
                #endregion settingsValue
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                //commonDal.TableFieldAdd("Vendors", "VDSPercent", "decimal(25, 9)", currConn);

                transaction = currConn.BeginTransaction("InsertToVendorInformation");

                #endregion open connection and transaction



                #region Insert Vendor Information


                sqlText = "select isnull(max(cast(VendorID as int)),0)+1 FROM  Vendors";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                nextId = Convert.ToInt32(cmdNextId.ExecuteScalar());
                if (nextId <= 0)
                {
                    //THROGUH EXCEPTION BECAUSE WE COULD NOT FIND EXPECTED NEW ID
                    retResults[0] = "Fail";
                    retResults[1] = "Unable to create new Vendor information Id";
                    throw new ArgumentNullException("InsertToVendorInformation",
                                                    "Unable to create new Vendor information Id");
                }
                #region Code
                if (Auto == false)
                {
                    if (string.IsNullOrEmpty(vendorCode))
                    {
                        throw new ArgumentNullException("InsertToVendorInformation", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        sqlText = "select count(VendorID) from Vendors where  VendorCode=@vendorCode";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        cmdCodeExist.Parameters.AddWithValue("@vendorCode", vendorCode);

                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("InsertToVendorInformation", "Same customer  Code('" + vendorCode + "') already exist");
                        }
                    }
                }
                else
                {
                    vendorCode = nextId.ToString();
                }
                #endregion Code


                sqlText = "";
                sqlText += "insert into Vendors";
                sqlText += "(";
                sqlText += "VendorID,";
                sqlText += "VendorName,";
                sqlText += "VendorGroupID,";
                sqlText += "Address1,";
                sqlText += "Address2,";
                sqlText += "Address3,";
                sqlText += "City,";
                sqlText += "TelephoneNo,";
                sqlText += "FaxNo,";
                sqlText += "Email,";
                sqlText += "StartDateTime,";
                sqlText += "ContactPerson,";
                sqlText += "ContactPersonDesignation,";
                sqlText += "ContactPersonTelephone,";
                sqlText += "ContactPersonEmail,";
                sqlText += "VATRegistrationNo,";
                sqlText += "TINNo,";
                sqlText += "Comments,";
                sqlText += "ActiveStatus,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "LastModifiedBy,";
                sqlText += "LastModifiedOn,";
                sqlText += "Country,";
                sqlText += "VendorCode,";
                sqlText += "BusinessType,";
                sqlText += "BusinessCode,";
                sqlText += "VDSPercent";

                sqlText += ")";
                sqlText += " values(";
                sqlText += "@nextId,";
                sqlText += "@VendorName,";
                sqlText += "@VendorGroupID,";
                sqlText += "@Address1,";
                sqlText += "@Address2,";
                sqlText += "@Address3,";
                sqlText += "@City,";
                sqlText += "@TelephoneNo,";
                sqlText += "@FaxNo,";
                sqlText += "@Email,";
                sqlText += "@StartDateTime,";
                sqlText += "@ContactPerson,";
                sqlText += "@ContactPersonDesignation,";
                sqlText += "@ContactPersonTelephone,";
                sqlText += "@ContactPersonEmail,";
                sqlText += "@VATRegistrationNo,";
                sqlText += "@TINNo,";
                sqlText += "@Comments,";
                sqlText += "@ActiveStatus,";
                sqlText += "@CreatedBy,";
                sqlText += "@CreatedOn,";
                sqlText += "@LastModifiedBy,";
                sqlText += "@LastModifiedOn,";
                sqlText += "@Country,";
                sqlText += "@vendorCode,";
                sqlText += "@BusinessType,";
                sqlText += "@BusinessCode,";
                sqlText += "@VDSPercent";


                sqlText += ")";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@nextId", nextId);
                cmdInsert.Parameters.AddWithValue("@VendorName", vm.VendorName);
                cmdInsert.Parameters.AddWithValue("@VendorGroupID", vm.VendorGroupID);
                cmdInsert.Parameters.AddWithValue("@Address1", vm.Address1 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Address2", vm.Address2 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Address3", vm.Address3 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@StartDateTime", vm.StartDateTime ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VATRegistrationNo", vm.VATRegistrationNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@TINNo", vm.TINNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@vendorCode", vm.VendorCode ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BusinessType", vm.BusinessType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BusinessCode", vm.BusinessCode ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VDSPercent", vm.VDSPercent);

                transResult = (int)cmdInsert.ExecuteNonQuery();



                #endregion Insert Vendor Information


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
                retResults[1] = "Requested Vendor Information successfully added";
                retResults[2] = "" + nextId;
                retResults[3] = "" + vendorCode;


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

            return retResults;

        }



        //=======================UPDATE===============================
        //public string[] UpdateVendorNewSQL(string VendorID,
        //                                   string VendorName, string VendorGroupID, string Address1, string Address2,
        //                                   string Address3, string City, string TelephoneNo, string FaxNo, string Email,
        //                                   string StartDateTime, string ContactPerson, string ContactPersonDesignation,
        //                                   string ContactPersonTelephone, string ContactPersonEmail,
        //                                   string VATRegistrationNo, string TINNo,
        //                                   string Comments, string ActiveStatus,
        //                                   string CreatedBy, string CreatedOn, string LastModifiedBy,
        //                                   string LastModifiedOn,
        //                                   string Country, string VendorCode, string VDSPercent, string BusinessType, string BusinessCode)
        public string[] UpdateVendorNewSQL(VendorVM vm)
        {
            #region Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.VendorID;

            string vendorCode = vm.VendorCode;

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

                if (string.IsNullOrEmpty(vm.VendorName))
                {
                    throw new ArgumentNullException("InsertToVendorInformation",
                                                    "Please enter vendor name.");
                }


                #endregion Validation
                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Vendor") == "Y" ? true : false);
                #endregion settingsValue
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                //commonDal.TableFieldAdd("Vendors", "VDSPercent", "decimal(25, 9)", currConn);

                transaction = currConn.BeginTransaction("InsertToVendorInformation");

                #endregion open connection and transaction

                if (!string.IsNullOrEmpty(vm.VendorID))
                {


                    sqlText = "select count(VendorID) from Vendors where  VendorID=@VendorID ";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@VendorID", vm.VendorID);

                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("InsertToVendorInformation",
                                                        "Could not find requested Vendor information ");
                    }

                }


                #region Code
                if (Auto == false)
                {
                    if (string.IsNullOrEmpty(vendorCode))
                    {
                        throw new ArgumentNullException("InsertToVendorInformation", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        sqlText = "select count(VendorID) from Vendors where  VendorCode=@VendorCode and VendorID <>@VendorID ";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        cmdCodeExist.Parameters.AddWithValue("@VendorCode", vm.VendorCode);
                        cmdCodeExist.Parameters.AddWithValue("@VendorID", vm.VendorID);

                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("InsertToVendorInformation", "Same customer  Code('" + vendorCode + "') already exist");
                        }
                    }
                }
                else
                {
                    vendorCode = vm.VendorID;
                }
                #endregion Code


                #region Insert Vendor Information



                sqlText = "";
                sqlText += "UPDATE  Vendors SET ";
                sqlText += " VendorName                 =@VendorName,";
                sqlText += " VendorGroupID              =@VendorGroupID,";
                sqlText += " Address1                   =@Address1,";
                sqlText += " Address2                   =@Address2,";
                sqlText += " Address3                   =@Address3,";
                sqlText += " City                       =@City,";
                sqlText += " TelephoneNo                =@TelephoneNo,";
                sqlText += " FaxNo                      =@FaxNo,";
                sqlText += " Email                      =@Email,";
                sqlText += " StartDateTime              =@StartDateTime,";
                sqlText += " ContactPerson              =@ContactPerson,";
                sqlText += " ContactPersonDesignation   =@ContactPersonDesignation,";
                sqlText += " ContactPersonTelephone     =@ContactPersonTelephone,";
                sqlText += " ContactPersonEmail         =@ContactPersonEmail,";
                sqlText += " VATRegistrationNo          =@VATRegistrationNo,";
                sqlText += " TINNo                      =@TINNo,";
                sqlText += " Comments                   =@Comments,";
                sqlText += " ActiveStatus               =@ActiveStatus,";
                //sqlText += " Option1                    =@Option1,";
                //sqlText += " Option2                    =@Option2,";
                //sqlText += " Option3                    =@Option3,";
                //sqlText += " Option4                    =@Option4,";
                sqlText += " LastModifiedBy             =@LastModifiedBy,";
                sqlText += " LastModifiedOn             =@LastModifiedOn,";
                sqlText += " Country                    =@Country,";
                sqlText += " VDSPercent                 =@VDSPercent,";
                sqlText += " BusinessType               =@BusinessType,";
                sqlText += " BusinessCode               =@BusinessCode,";
                sqlText += " VendorCode                 =@vendorCode";
                sqlText += " where VendorID             =@VendorID";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@VendorName", vm.VendorName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VendorGroupID", vm.VendorGroupID ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Address1", vm.Address1 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Address2", vm.Address2 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Address3", vm.Address3 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@StartDateTime", vm.StartDateTime);
                cmdInsert.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VATRegistrationNo", vm.VATRegistrationNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@TINNo", vm.TINNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VDSPercent", vm.VDSPercent);
                cmdInsert.Parameters.AddWithValue("@BusinessType", vm.BusinessType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BusinessCode", vm.BusinessCode ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@vendorCode", vm.VendorCode ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VendorID", vm.VendorID ?? Convert.DBNull);

                transResult = (int)cmdInsert.ExecuteNonQuery();



                #endregion Update Vendor Information


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
                retResults[1] = "Requested Vendor Information successfully Updated";
                retResults[2] = "" + vm.VendorID;
                retResults[3] = "" + vm.VendorCode;


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

            return retResults;

        }


        public string[] DeleteVendorInformation(string VendorID)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = VendorID;

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(VendorID))
                {
                    throw new ArgumentNullException("InsertToVendorInformation",
                                                    "Could not find requested Vendor Id.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("Vendors", "VDSPercent", "decimal(25, 9)", currConn);
                #endregion open connection and transaction

                sqlText = "select count(VendorID) from Vendors where VendorID=@VendorID ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValue("@VendorID", VendorID);

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested Vendor Information.";
                    return retResults;
                }

                sqlText = "delete Vendors where VendorID=@VendorID";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                cmdDelete.Parameters.AddWithValue("@VendorID", VendorID);

                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested Vendor Information successfully deleted";
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


        //==========================SearchVendorNew=====================
        public DataTable SearchVendor(
            string VendorCode
            , string VendorName
            , string VendorGroupID
            , string VendorGroupName
            , string City
            , string TelephoneNo
            , string FaxNo
            , string Email
            , string StartDateFrom
            , string StartDateTo
            , string ContactPerson
            , string ContactPersonDesignation
            , string ContactPersonTelephone
            , string ContactPersonEmail
            , string TINNo
            , string VATRegistrationNo
            , string ActiveStatus)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("Search Vendor");

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("Vendors", "VDSPercent", "decimal(25, 9)", currConn);
                #endregion open connection and transaction

                #region SqlStatement

                sqlText = @"
                            SELECT    
                            Vendors.VendorID,
                            isnull(Vendors.VendorName,'N/A')VendorName,
                            Vendors.VendorGroupID,
                            isnull(VendorGroups.VendorGroupName,'N/A')VendorGroupName,
                            isnull(Vendors.City,'N/A')City,
                            isnull(Vendors.Address1,'N/A')Address1,
                            isnull(Vendors.Address2,'N/A')Address2,
                            isnull(Vendors.Address3,'N/A')Address3,
                            isnull(Vendors.TelephoneNo,'N/A')TelephoneNo ,
                            isnull(Vendors.FaxNo,'N/A')FaxNo,
                            isnull(Vendors.Email,'N/A')Email,
                            convert (varchar,Vendors.StartDateTime,120)StartDate,
                            isnull(Vendors.ContactPerson,'N/A')ContactPerson ,
                            isnull(Vendors.ContactPersonDesignation,'N/A')ContactPersonDesignation ,
                            isnull(Vendors.ContactPersonTelephone,'N/A')ContactPersonTelephone,
                            isnull(Vendors.ContactPersonEmail,'N/A')ContactPersonEmail ,
                            isnull(Vendors.VATRegistrationNo,'N/A')VATRegistrationNo ,
                            isnull(Vendors.TINNo,'N/A')TINNo ,
                            isnull(Vendors.Comments,'N/A')Comments,
                            isnull(Vendors.ActiveStatus,'N/A')ActiveStatus,
                            isnull(Vendors.VendorCode,'N/A')VendorCode,
                            isnull(Vendors.VDSPercent,0)VDSPercent,
                            isnull(Vendors.BusinessType,'N/A')BusinessType,
                            isnull(Vendors.BusinessCode,'N/A')BusinessCode,
                            isnull(Vendors.Country,'N/A')Country

                            FROM         Vendors INNER JOIN
                            VendorGroups ON Vendors.VendorGroupID = VendorGroups.VendorGroupID               
                            WHERE (Vendors.VendorCode LIKE '%' +  @VendorCode  + '%' OR @VendorCode IS NULL) 
                            AND (Vendors.VendorName LIKE '%' + @VendorName + '%' OR @VendorName IS NULL)
                            AND (Vendors.VendorGroupID LIKE '%' + @VendorGroupID + '%' OR @VendorGroupID IS NULL) 
                            AND (VendorGroups.VendorGroupName LIKE '%' + @VendorGroupName + '%' OR @VendorGroupName IS NULL)
                            AND (Vendors.City LIKE '%' + @City + '%' OR @City IS NULL) 
                            AND (Vendors.TelephoneNo LIKE '%' + @TelephoneNo + '%' OR @TelephoneNo IS NULL) 
                            AND (Vendors.FaxNo LIKE '%' + @FaxNo + '%' OR @FaxNo IS NULL) 
                            AND (Vendors.Email LIKE '%' + @Email + '%' OR @Email IS NULL) 
                            AND (Vendors.StartDateTime>= @StartDateFrom OR @StartDateFrom IS NULL)
                            AND (Vendors.StartDateTime<= @StartDateTo OR @StartDateTo IS NULL)
                            AND (Vendors.ContactPerson LIKE '%' + @ContactPerson + '%' OR @ContactPerson IS NULL)
                            AND (Vendors.ContactPersonDesignation LIKE '%' + @ContactPersonDesignation + '%' OR @ContactPersonDesignation IS NULL)
                            AND (Vendors.ContactPersonTelephone LIKE '%' + @ContactPersonTelephone + '%' OR @ContactPersonTelephone IS NULL)
                            AND (Vendors.ContactPersonEmail LIKE '%' + @ContactPersonEmail + '%' OR @ContactPersonEmail IS NULL)
                            AND (Vendors.TINNo LIKE '%' + @TINNo + '%' OR @TINNo IS NULL)
                            AND (Vendors.VATRegistrationNo LIKE '%' + @VATRegistrationNo + '%' OR @VATRegistrationNo IS NULL)
                            AND (Vendors.ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)

                            and (Vendors.VendorID <>'0') 
                            order by Vendors.VendorName
                            ";

                SqlCommand objCommVendor = new SqlCommand();
                objCommVendor.Connection = currConn;
                objCommVendor.CommandText = sqlText;
                objCommVendor.CommandType = CommandType.Text;

                #region Parameter

                if (!objCommVendor.Parameters.Contains("@VendorCode"))
                {
                    objCommVendor.Parameters.AddWithValue("@VendorCode", VendorCode);
                }
                else
                {
                    objCommVendor.Parameters["@VendorCode"].Value = VendorCode;
                }
                if (!objCommVendor.Parameters.Contains("@VendorName"))
                {
                    objCommVendor.Parameters.AddWithValue("@VendorName", VendorName);
                }
                else
                {
                    objCommVendor.Parameters["@VendorName"].Value = VendorName;
                }
                if (!objCommVendor.Parameters.Contains("@VendorGroupID"))
                {
                    objCommVendor.Parameters.AddWithValue("@VendorGroupID", VendorGroupID);
                }
                else
                {
                    objCommVendor.Parameters["@VendorGroupID"].Value = VendorGroupID;
                }
                if (!objCommVendor.Parameters.Contains("@VendorGroupName"))
                {
                    objCommVendor.Parameters.AddWithValue("@VendorGroupName", VendorGroupName);
                }
                else
                {
                    objCommVendor.Parameters["@VendorGroupName"].Value = VendorGroupName;
                }
                if (!objCommVendor.Parameters.Contains("@City"))
                {
                    objCommVendor.Parameters.AddWithValue("@City", City);
                }
                else
                {
                    objCommVendor.Parameters["@City"].Value = City;
                }
                if (!objCommVendor.Parameters.Contains("@TelephoneNo"))
                {
                    objCommVendor.Parameters.AddWithValue("@TelephoneNo", TelephoneNo);
                }
                else
                {
                    objCommVendor.Parameters["@TelephoneNo"].Value = TelephoneNo;
                }
                if (!objCommVendor.Parameters.Contains("@FaxNo"))
                {
                    objCommVendor.Parameters.AddWithValue("@FaxNo", FaxNo);
                }
                else
                {
                    objCommVendor.Parameters["@FaxNo"].Value = FaxNo;
                }
                if (!objCommVendor.Parameters.Contains("@Email"))
                {
                    objCommVendor.Parameters.AddWithValue("@Email", Email);
                }
                else
                {
                    objCommVendor.Parameters["@Email"].Value = Email;
                }
                if (StartDateFrom == "")
                {
                    if (!objCommVendor.Parameters.Contains("@StartDateFrom"))
                    {
                        objCommVendor.Parameters.AddWithValue("@StartDateFrom", DBNull.Value);
                    }
                    else
                    {
                        objCommVendor.Parameters["@StartDateFrom"].Value = DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommVendor.Parameters.Contains("@StartDateFrom"))
                    {
                        objCommVendor.Parameters.AddWithValue("@StartDateFrom", StartDateFrom);
                    }
                    else
                    {
                        objCommVendor.Parameters["@StartDateFrom"].Value = StartDateFrom;
                    }
                }
                if (StartDateTo == "")
                {
                    if (!objCommVendor.Parameters.Contains("@StartDateTo"))
                    {
                        objCommVendor.Parameters.AddWithValue("@StartDateTo", DBNull.Value);
                    }
                    else
                    {
                        objCommVendor.Parameters["@StartDateTo"].Value = DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommVendor.Parameters.Contains("@StartDateTo"))
                    {
                        objCommVendor.Parameters.AddWithValue("@StartDateTo", StartDateTo);
                    }
                    else
                    {
                        objCommVendor.Parameters["@StartDateTo"].Value = StartDateTo;
                    }
                }
                if (!objCommVendor.Parameters.Contains("@ContactPerson"))
                {
                    objCommVendor.Parameters.AddWithValue("@ContactPerson", ContactPerson);
                }
                else
                {
                    objCommVendor.Parameters["@ContactPerson"].Value = ContactPerson;
                }
                if (!objCommVendor.Parameters.Contains("@ContactPersonDesignation"))
                {
                    objCommVendor.Parameters.AddWithValue("@ContactPersonDesignation", ContactPersonDesignation);
                }
                else
                {
                    objCommVendor.Parameters["@ContactPersonDesignation"].Value = ContactPersonDesignation;
                }
                if (!objCommVendor.Parameters.Contains("@ContactPersonTelephone"))
                {
                    objCommVendor.Parameters.AddWithValue("@ContactPersonTelephone", ContactPersonTelephone);
                }
                else
                {
                    objCommVendor.Parameters["@ContactPersonTelephone"].Value = ContactPersonTelephone;
                }
                if (!objCommVendor.Parameters.Contains("@ContactPersonEmail"))
                {
                    objCommVendor.Parameters.AddWithValue("@ContactPersonEmail", ContactPersonEmail);
                }
                else
                {
                    objCommVendor.Parameters["@ContactPersonEmail"].Value = ContactPersonEmail;
                }
                if (!objCommVendor.Parameters.Contains("@TINNo"))
                {
                    objCommVendor.Parameters.AddWithValue("@TINNo", TINNo);
                }
                else
                {
                    objCommVendor.Parameters["@TINNo"].Value = TINNo;
                }
                if (!objCommVendor.Parameters.Contains("@VATRegistrationNo"))
                {
                    objCommVendor.Parameters.AddWithValue("@VATRegistrationNo", VATRegistrationNo);
                }
                else
                {
                    objCommVendor.Parameters["@VATRegistrationNo"].Value = VATRegistrationNo;
                }
                if (!objCommVendor.Parameters.Contains("@ActiveStatus"))
                {
                    objCommVendor.Parameters.AddWithValue("@ActiveStatus", ActiveStatus);
                }
                else
                {
                    objCommVendor.Parameters["@ActiveStatus"].Value = ActiveStatus;
                }

                #endregion Parameter

                // Common Filed
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVendor);
                dataAdapter.Fill(dataTable);

                #endregion

            }
            #endregion

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


        //==========================SearchVendorSingleDTNew=====================
        public DataTable SearchVendorSingleDTNew(string VendorCode, string VendorName, string VendorGroupID,
                                                 string VendorGroupName, string TINNo, string VATRegistrationNo,
                                                 string ActiveStatus)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("Search Transaction History");

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("Vendors", "VDSPercent", "decimal(25, 9)", currConn);
                #endregion open connection and transaction

                #region SQL Statement

                sqlText = @"
                            SELECT    
                            Vendors.VendorID,
                            isnull(Vendors.VendorName,'N/A')VendorName,
                            Vendors.VendorGroupID,
                            isnull(VendorGroups.VendorGroupName,'N/A')VendorGroupName,
                            isnull(Vendors.Address1,'N/A')Address1,
                            isnull(Vendors.Address2,'N/A')Address2,
                            isnull(Vendors.Address3,'N/A')Address3,
                            isnull(Vendors.TelephoneNo,'N/A')TelephoneNo ,
                            isnull(Vendors.FaxNo,'N/A')FaxNo,
                            isnull(Vendors.Email,'N/A')Email,
                            isnull(Vendors.VATRegistrationNo,'N/A')VATRegistrationNo ,
                            isnull(Vendors.TINNo,'N/A')TINNo ,
                            isnull(Vendors.ActiveStatus,'N/A')ActiveStatus,
                            isnull(Vendors.VendorCode,'N/A')VendorCode,
                            isnull(Vendors.Country,'N/A')Country,
                            GroupType

                            FROM         Vendors INNER JOIN
                            VendorGroups ON Vendors.VendorGroupID = VendorGroups.VendorGroupID     
          
                            WHERE (Vendors.VendorCode LIKE '%' +  @VendorCode  + '%' OR @VendorCode IS NULL) 
                            AND (Vendors.VendorName LIKE '%' + @VendorName + '%' OR @VendorName IS NULL)
                            AND (Vendors.VendorGroupID LIKE '%' + @VendorGroupID + '%' OR @VendorGroupID IS NULL) 
                            AND (VendorGroups.VendorGroupName LIKE '%' + @VendorGroupName + '%' OR @VendorGroupName IS NULL)
                            AND (Vendors.TINNo LIKE '%' + @TINNo + '%' OR @TINNo IS NULL)
                            AND (Vendors.VATRegistrationNo LIKE '%' + @VATRegistrationNo + '%' OR @VATRegistrationNo IS NULL)
                            AND (Vendors.ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)

                            and (Vendors.VendorID <>'0') 
                            order by Vendors.VendorName
                            ";

                #endregion

                #region SQL Command

                SqlCommand objCommVendor = new SqlCommand();
                objCommVendor.Connection = currConn;

                objCommVendor.CommandText = sqlText;
                objCommVendor.CommandType = CommandType.Text;

                #endregion

                #region Parameter

                if (!objCommVendor.Parameters.Contains("@VendorCode"))
                {
                    objCommVendor.Parameters.AddWithValue("@VendorCode", VendorCode);
                }
                else
                {
                    objCommVendor.Parameters["@VendorCode"].Value = VendorCode;
                }
                if (!objCommVendor.Parameters.Contains("@VendorName"))
                {
                    objCommVendor.Parameters.AddWithValue("@VendorName", VendorName);
                }
                else
                {
                    objCommVendor.Parameters["@VendorName"].Value = VendorName;
                }
                if (!objCommVendor.Parameters.Contains("@VendorGroupID"))
                {
                    objCommVendor.Parameters.AddWithValue("@VendorGroupID", VendorGroupID);
                }
                else
                {
                    objCommVendor.Parameters["@VendorGroupID"].Value = VendorGroupID;
                }
                if (!objCommVendor.Parameters.Contains("@VendorGroupName"))
                {
                    objCommVendor.Parameters.AddWithValue("@VendorGroupName", VendorGroupName);
                }
                else
                {
                    objCommVendor.Parameters["@VendorGroupName"].Value = VendorGroupName;
                }

                if (!objCommVendor.Parameters.Contains("@TINNo"))
                {
                    objCommVendor.Parameters.AddWithValue("@TINNo", TINNo);
                }
                else
                {
                    objCommVendor.Parameters["@TINNo"].Value = TINNo;
                }
                if (!objCommVendor.Parameters.Contains("@VATRegistrationNo"))
                {
                    objCommVendor.Parameters.AddWithValue("@VATRegistrationNo", VATRegistrationNo);
                }
                else
                {
                    objCommVendor.Parameters["@VATRegistrationNo"].Value = VATRegistrationNo;
                }
                if (!objCommVendor.Parameters.Contains("@ActiveStatus"))
                {
                    objCommVendor.Parameters.AddWithValue("@ActiveStatus", ActiveStatus);
                }
                else
                {
                    objCommVendor.Parameters["@ActiveStatus"].Value = ActiveStatus;
                }


                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVendor);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally


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

            #endregion

            return dataTable;

        }


        //==================DropDownAll=================
        public List<VendorVM> DropDownAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<VendorVM> VMs = new List<VendorVM>();
            VendorVM vm;
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
'B' Sl, VendorID
, VendorName
FROM Vendors
WHERE ActiveStatus='Y'
UNION 
SELECT 
'A' Sl, '-1' VendorID
, 'ALL Product' VendorName  
FROM Vendors
)
AS a
order by Sl,VendorName
";



                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new VendorVM();
                    vm.VendorID = dr["VendorID"].ToString(); ;
                    vm.VendorName = dr["VendorName"].ToString();
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
        public List<VendorVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<VendorVM> VMs = new List<VendorVM>();
            VendorVM vm;
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
af.VendorID
,af.VendorName
FROM Vendors af
WHERE  1=1 AND af.ActiveStatus = 'Y'
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new VendorVM();
                    vm.VendorID = dr["VendorID"].ToString();
                    vm.VendorName = dr["VendorName"].ToString();
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
        public List<VendorVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<VendorVM> VMs = new List<VendorVM>();
            VendorVM vm;
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
 VendorID
,VendorCode
,VendorName
,VendorGroupID
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
,VATRegistrationNo
,TINNo
,Comments
,ActiveStatus
,isnull(Option1,0)Option1
,isnull(Option2,0)Option2
,isnull(Option3,0)Option3
,isnull(Option4,0)Option4
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,Country
,Info2
,Info3
,Info4
,Info5
,ISNULL(VDSPercent,0) VDSPercent 
,BusinessType
,BusinessCode

FROM Vendors  
WHERE  1=1 AND IsArchive = 0

";
                if (Id > 0)
                {
                    sqlText += @" and VendorID=@VendorID";
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
                    objComm.Parameters.AddWithValue("@VendorID", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new VendorVM();
                    vm.VendorID = dr["VendorID"].ToString();
                    vm.VendorCode = dr["VendorCode"].ToString();
                    vm.VendorName = dr["VendorName"].ToString();
                    vm.VendorGroupID = dr["VendorGroupID"].ToString();
                    vm.Address1 = dr["Address1"].ToString();
                    vm.Address2 = dr["Address2"].ToString();
                    vm.Address3 = dr["Address3"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.TelephoneNo = dr["TelephoneNo"].ToString();
                    vm.FaxNo = dr["FaxNo"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.StartDateTime = Ordinary.DateTimeToDate(dr["StartDateTime"].ToString());
                    //vm.StartDateTime = dr["StartDateTime"].ToString();
                    vm.ContactPerson = dr["ContactPerson"].ToString();
                    vm.ContactPersonDesignation = dr["ContactPersonDesignation"].ToString();
                    vm.ContactPersonTelephone = dr["ContactPersonTelephone"].ToString();
                    vm.ContactPersonEmail = dr["ContactPersonEmail"].ToString();
                    vm.VATRegistrationNo = dr["VATRegistrationNo"].ToString();
                    vm.TINNo = dr["TINNo"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.Country = dr["Country"].ToString();
                    vm.Info2 = dr["Info2"].ToString();
                    vm.Info3 = dr["Info3"].ToString();
                    vm.Info4 = dr["Info4"].ToString();
                    vm.Info5 = dr["Info5"].ToString();
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

        public string[] Delete(VendorVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteVendor"; //Method Name
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
                        sqlText = "update Vendors set";
                        sqlText += " ActiveStatus=@ActiveStatus";
                        sqlText += " ,LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where VendorID=@VendorID";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@VendorID", ids[i]);
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
                        throw new ArgumentNullException("Vendor Delete", vm.VendorID + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Vendor Information Delete", "Could not found any item.");
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

        //====================others==============================
    }
}
