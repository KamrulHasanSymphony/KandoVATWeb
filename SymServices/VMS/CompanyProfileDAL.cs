using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using SymOrdinary;
using SymphonySofttech.Utilities;
using SymViewModel.VMS;

namespace SymServices.VMS
{
    public class CompanyprofileDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
       
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;

        #endregion

        #region New Methods

        public string[] UpdateCompanyProfileNew(CompanyProfileVM companyProfiles)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(companyProfiles.CompanyName))
                {
                    throw new ArgumentNullException("UpdateToVehicle", "Please enter company name.");
                }
                if (string.IsNullOrEmpty(companyProfiles.CompanyLegalName))
                {
                    throw new ArgumentNullException("UpdateToVehicle", "Please enter company legal name.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UpdateCompanyProfileNew");

                #endregion open connection and transaction

                #region CompanyProfile existence checking by id

                //select @Present = count(VehicleID) from Vehicles where VehicleID = @VehicleID;
                sqlText = "select count(CompanyID) from CompanyProfiles where CompanyID =@companyProfilesCompanyID ";
                SqlCommand vhclIDExist = new SqlCommand(sqlText, currConn);
                vhclIDExist.Transaction = transaction;
                vhclIDExist.Parameters.AddWithValue("@companyProfilesCompanyID", companyProfiles.CompanyID);

                countId = (int)vhclIDExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateCompanyProfileNew", "Could not find requested Company Information.");
                }

                #endregion CompanyProfile existence checking by id

                #region companyProfiles existence checking by id and requied field

                //sqlText = "select count(CompanyName) from CompanyProfiles ";
                //sqlText += " where  CompanyID='" + companyProfiles.CompanyID + "'";
                //sqlText += " and CompanyName='" + companyProfiles.CompanyName + "'";
                //SqlCommand vhclNoExist = new SqlCommand(sqlText, currConn);
                //vhclNoExist.Transaction = transaction;
                //countId = (int)vhclNoExist.ExecuteScalar();
                //if (countId > 0)
                //{
                //    throw new ArgumentNullException("UpdateCompanyProfileNew", "Same company profile already exist.");
                //}

                #endregion companyProfiles existence checking by id and requied field

                #region Update company profile

                sqlText = "";
                sqlText = sqlText + "  update CompanyProfiles set ";
                sqlText = sqlText + " CompanyName               =@companyProfilesCompanyName,";
                sqlText = sqlText + " CompanyLegalName          =@companyProfilesCompanyLegalName,";
                sqlText = sqlText + " Address1                  =@companyProfilesAddress1,";
                sqlText = sqlText + " Address2                  =@companyProfilesAddress2,";
                sqlText = sqlText + " Address3                  =@companyProfilesAddress3,";
                sqlText = sqlText + " City                      =@companyProfilesCity,";
                sqlText = sqlText + " ZipCode                   =@companyProfilesZipCode,";
                sqlText = sqlText + " TelephoneNo               =@companyProfilesTelephoneNo,";
                sqlText = sqlText + " FaxNo                     =@companyProfilesFaxNo,";
                sqlText = sqlText + " Email                     =@companyProfilesEmail,";
                sqlText = sqlText + " ContactPerson             =@companyProfilesContactPerson,";
                sqlText = sqlText + " ContactPersonDesignation  =@companyProfilesContactPersonDesignation,";
                sqlText = sqlText + " ContactPersonTelephone    =@companyProfilesContactPersonTelephone,";
                sqlText = sqlText + " ContactPersonEmail        =@companyProfilesContactPersonEmail,";
                sqlText = sqlText + " TINNo                     =@companyProfilesTINNo,";
                sqlText = sqlText + " VatRegistrationNo         =@companyProfilesVatRegistrationNo,";
                sqlText = sqlText + " Comments                  =@companyProfilesComments,";
                sqlText = sqlText + " ActiveStatus              =@companyProfilesActiveStatus,";
                sqlText = sqlText + " LastModifiedBy            =@companyProfilesLastModifiedBy,";
                sqlText = sqlText + " LastModifiedOn            =@companyProfilesLastModifiedOn";
                if (!string.IsNullOrEmpty(companyProfiles.Tom))  
                {                                                
                    sqlText = sqlText + ", Tom                  =@companyProfilesTom,";
                    sqlText = sqlText + " Jary                  =@companyProfilesJary,";
                    sqlText = sqlText + " Miki                  =@companyProfilesMiki,";
                    sqlText = sqlText + " Mouse                 =@companyProfilesMouse";
                }                                                
                sqlText = sqlText + " where CompanyID           =@companyProfilesCompanyID";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@companyProfilesCompanyName", companyProfiles.CompanyName);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesCompanyLegalName", companyProfiles.CompanyLegalName);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesAddress1", companyProfiles.Address1??Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesAddress2", companyProfiles.Address2 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesAddress3", companyProfiles.Address3 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesCity", companyProfiles.City ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesZipCode", companyProfiles.ZipCode ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesTelephoneNo", companyProfiles.TelephoneNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesFaxNo", companyProfiles.FaxNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesEmail", companyProfiles.Email ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesContactPerson", companyProfiles.ContactPerson ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesContactPersonDesignation", companyProfiles.ContactPersonDesignation ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesContactPersonTelephone", companyProfiles.ContactPersonTelephone ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesContactPersonEmail", companyProfiles.ContactPersonEmail ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesTINNo", companyProfiles.TINNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesVatRegistrationNo", companyProfiles.VatRegistrationNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesComments", companyProfiles.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesActiveStatus", companyProfiles.ActiveStatus);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesLastModifiedBy", companyProfiles.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesLastModifiedOn", companyProfiles.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesTom", companyProfiles.Tom ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesJary", companyProfiles.Jary ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesMiki", companyProfiles.Miki ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesMouse", companyProfiles.Mouse ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@companyProfilesCompanyID", companyProfiles.CompanyID ?? Convert.DBNull);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException("UpdateCompanyProfileNew", "Unable to Update Company Information ");
                }

                #region Update Sys DB Information

                string CompanyID = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.CompanyID);
                string CompanyName = Converter.DESEncrypt(PassPhrase, EnKey, companyProfiles.CompanyName);
                sqlText = "";
                sqlText += " update CompanyInformations set " +
                           "CompanyName=@CompanyName" +
                           " where CompanyID=@CompanyID ";
                currConn.ChangeDatabase("SymphonyVATSys");
                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);
                cmdPrefetch.Transaction = transaction;
                cmdPrefetch.Parameters.AddWithValue("@CompanyName", CompanyName);
                cmdPrefetch.Parameters.AddWithValue("@CompanyID", CompanyID);

                transResult = (int)cmdPrefetch.ExecuteNonQuery();
                if (transResult < 0)
                {
                    throw new ArgumentNullException("UpdateCompanyProfileNew", "Unable to Update Company Information ");

                }
                #endregion Update Sys DB Information
                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Company Profile Information Successfully Update.";
                        retResults[2] = "" + companyProfiles.CompanyID;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update company profile.";
                        retResults[2] = "" + companyProfiles.CompanyID;
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update company profile.";
                    retResults[2] = "" + companyProfiles.CompanyID;
                }

                #endregion Commit

                #endregion Update company profile

            }
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

        //==================SelectAll=================
        public List<CompanyProfileVM> SelectAll(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<CompanyProfileVM> VMs = new List<CompanyProfileVM>();
            CompanyProfileVM vm;
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
 CompanyID
,CompanyName
,CompanyLegalName
,Address1
,Address2
,Address3
,City
,ZipCode
,TelephoneNo
,FaxNo
,Email
,ContactPerson
,ContactPersonDesignation
,ContactPersonTelephone
,ContactPersonEmail
,TINNo
,VatRegistrationNo
,Comments
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,StartDateTime
,FYearStart
,FYearEnd
,Info4
,Info5
,Tom
,Jary
,Miki
,Mouse


FROM CompanyProfiles  
WHERE  1=1 AND ActiveStatus = 'Y'

";
                if (Id != null)
                {
                    sqlText += @" and CompanyID=@CompanyID";
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

                if (Id != null)
                {
                    objComm.Parameters.AddWithValue("@CompanyID", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CompanyProfileVM();
                    vm.CompanyID = dr["CompanyID"].ToString();

                    vm.CompanyName = dr["CompanyName"].ToString();
                    vm.CompanyLegalName = dr["CompanyLegalName"].ToString();
                    vm.Address1 = dr["Address1"].ToString();
                    vm.Address2 = dr["Address2"].ToString();
                    vm.Address3 = dr["Address3"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.ZipCode = dr["ZipCode"].ToString();
                    vm.TelephoneNo = dr["TelephoneNo"].ToString();
                    vm.FaxNo = dr["FaxNo"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.ContactPerson = dr["ContactPerson"].ToString();
                    vm.ContactPersonDesignation = dr["ContactPersonDesignation"].ToString();
                    vm.ContactPersonTelephone = dr["ContactPersonTelephone"].ToString();
                    vm.ContactPersonEmail = dr["ContactPersonEmail"].ToString();
                    vm.TINNo = dr["TINNo"].ToString();
                    vm.VatRegistrationNo = dr["VatRegistrationNo"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.StartDateTime = Ordinary.DateTimeToDate(dr["StartDateTime"].ToString());
                    vm.FYearStart = Ordinary.DateTimeToDate(dr["FYearStart"].ToString());
                    vm.FYearEnd = Ordinary.DateTimeToDate(dr["FYearEnd"].ToString());
                    vm.Tom = dr["Tom"].ToString();
                    vm.Jary = dr["Jary"].ToString();
                    vm.Miki = dr["Miki"].ToString();
                    vm.Mouse = dr["Mouse"].ToString();

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
        #endregion

        #region Old Methods

        public DataTable SearchCompanyProfile()
        {
            #region Objects & Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("CProfile");
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
CompanyId,
isnull(CompanyName,'N/A')CompanyName, 
isnull(CompanyLegalName,'N/A')CompanyLegalName ,
isnull(Address1,'N/A')Address1,
isnull(Address2,'N/A')Address2,
isnull(Address3,'N/A')Address3,
isnull(City,'N/A')City,
isnull(ZipCode,'N/A')ZipCode,
isnull(TelephoneNo,'N/A')TelephoneNo ,
isnull(FaxNo,'N/A')FaxNo ,
isnull(Email,'N/A')Email,
isnull(ContactPerson,'N/A')ContactPerson,
isnull(ContactPersonDesignation,'N/A')ContactPersonDesignation,
isnull(ContactPersonTelephone,'N/A')ContactPersonTelephone,
isnull(ContactPersonEmail ,'N/A')ContactPersonEmail,
isnull(VatRegistrationNo,'N/A')VatRegistrationNo,
isnull(TINNo,'N/A')TINNo,
isnull(Comments,'N/A')Comments,
isnull(ActiveStatus,'N')ActiveStatus,
convert(varchar, StartDateTime,120)StartDateTime,
convert(varchar, FYearStart,120)FYearStart,
convert(varchar, FYearEnd,120)FYearEnd

FROM  CompanyProfiles";

                SqlCommand objCommProductType = new SqlCommand();
                objCommProductType.Connection = currConn;
                objCommProductType.CommandText = sqlText;
                objCommProductType.CommandType = CommandType.Text;

                
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProductType);
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

     


        #endregion
    }
}
