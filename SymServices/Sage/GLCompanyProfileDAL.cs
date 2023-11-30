using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using SymphonySofttech.Utilities;
using SymViewModel.Sage;
using SymOrdinary;

namespace SymServices.Sage
{
    public class GLCompanyProfileDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;

        #endregion

#region Methods
        //==================SelectAll=================
        public List<GLCompanyProfileVM> SelectAll(int Id = 0, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLCompanyProfileVM> VMs = new List<GLCompanyProfileVM>();
            GLCompanyProfileVM vm;
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
                    currConn = _dbsqlConnection.GetConnectionSageGL();
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
                sqlText = @"
SELECT
 Id
,Name
,LegalName
,Address
,City
,ZipCode
,TelephoneNo
,MobileNo
,FaxNo
,Email
,ContactPerson
,ContactPersonDesignation
,ContactPersonTelephone
,ContactPersonEmail
,TIN
,BIN
,isnull(VATNo,0)VATNo
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,YearStart
,Year
From GLCompanyProfiles
Where  IsArchive=0
";
                //Name
                //LegalName
                //Address
                //City
                //ZipCode
                //TelephoneNo
                //MobileNo
                //FaxNo
                //Email
                //ContactPerson
                //ContactPersonDesignation
                //ContactPersonTelephone
                //ContactPersonEmail
                //TIN
                //BIN
                if (Id > 0)
                {
                    sqlText += @" and  id=@Id";
                }
                //sqlText += @" ORDER BY Name";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (Id > 0)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLCompanyProfileVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Name = dr["Name"].ToString();
                    vm.LegalName = dr["LegalName"].ToString();
                    vm.Address = dr["Address"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.ZipCode = dr["ZipCode"].ToString();
                    vm.TelephoneNo = dr["TelephoneNo"].ToString();
                    vm.MobileNo = dr["MobileNo"].ToString();
                    vm.FaxNo = dr["FaxNo"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.ContactPerson = dr["ContactPerson"].ToString();
                    vm.ContactPersonDesignation = dr["ContactPersonDesignation"].ToString();
                    vm.ContactPersonTelephone = dr["ContactPersonTelephone"].ToString();
                    vm.ContactPersonEmail = dr["ContactPersonEmail"].ToString();
                    vm.TIN = dr["TIN"].ToString();
                    vm.BIN = dr["BIN"].ToString();
                    vm.VATNo = dr["VATNo"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.YearStart = Ordinary.StringToDate( dr["YearStart"].ToString());
                    vm.Year = dr["Year"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
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
            return VMs;
        }
        //==================Insert =================
        public string[] Insert(GLCompanyProfileVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertGLCompanyProfile"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try
            try
            {
                #region Validation
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
                    currConn = _dbsqlConnection.GetConnectionSageGL();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #region Exist
                //CommonDAL cdal = new CommonDAL();
                //bool check = false;
                //string tableName = "GLCompanyProfile";	
                //string[] fieldName = { "Code", "Name" };
                //string[] fieldValue = { vm.Code.Trim(), vm.Name.Trim() };
                //for (int i = 0; i < fieldName.Length; i++)
                //{
                //    check = cdal.CheckDuplicateInInsertWithGLCompanyProfile(tableName, fieldName[i], fieldValue[i], vm.GLCompanyProfileId, currConn, transaction);
                //    if (check == true)
                //    {
                //        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                //        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                //    }
                //}
                #endregion Exist
                #endregion open connection and transaction
                #region Save
                vm.Id = Ordinary.NextId("GLCompanyProfiles", currConn, transaction).ToString();
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO GLCompanyProfiles(Id
,Name
,LegalName
,Address
,City
,ZipCode
,TelephoneNo
,MobileNo
,FaxNo
,Email
,ContactPerson
,ContactPersonDesignation
,ContactPersonTelephone
,ContactPersonEmail
,TIN
,BIN
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
VALUES (
@Id
,@Name
,@LegalName
,@Address
,@City
,@ZipCode
,@TelephoneNo
,@MobileNo
,@FaxNo
,@Email
,@ContactPerson
,@ContactPersonDesignation
,@ContactPersonTelephone
,@ContactPersonEmail
,@TIN
,@BIN
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) 
";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@Name", vm.Name);
                    cmdInsert.Parameters.AddWithValue("@LegalName", vm.LegalName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ZipCode", vm.ZipCode ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@MobileNo", vm.MobileNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@TIN", vm.TIN ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@BIN", vm.BIN ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, bankVM.Remarks);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.ExecuteNonQuery();
                }
                else
                {
                    retResults[1] = "This GLCompanyProfile already used!";
                    throw new ArgumentNullException("Please Input GLCompanyProfile Value", "");
                }
                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
            }
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
            #region Results
            return retResults;
            #endregion
        }
        //==================Update =================
        public string[] Update(GLCompanyProfileVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "GLCompanyProfile Update"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;
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
                    currConn = _dbsqlConnection.GetConnectionSageGL();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToGLCompanyProfile"); }
                #endregion open connection and transaction
                #region Exist
                //CommonDAL cdal = new CommonDAL();
                //bool check = false;
                //string tableName = "GLCompanyProfile";				
                //string[] fieldName = { "Code", "Name" };
                //string[] fieldValue = { vm.Code.Trim(), vm.Name.Trim() };
                //for (int i = 0; i < fieldName.Length; i++)
                //{
                //    check = cdal.CheckDuplicateInUpdateWithGLCompanyProfile(vm.Id, tableName, fieldName[i], fieldValue[i], vm.GLCompanyProfileId, currConn, transaction);
                //    if (check == true)
                //    {
                //        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                //        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                //    }
                //}
                #endregion Exist
                if (vm != null)
                {
                    #region Fiscalyear
                    GLFiscalYearDAL fiscalYearDal = new GLFiscalYearDAL();
                    List<GLFiscalYearVM> glFiscalYearVMs = fiscalYearDal.SelectAll();
                    GLFiscalYearVM glFiscalYearVM = new GLFiscalYearVM();
                    foreach (GLFiscalYearVM itemVM in glFiscalYearVMs)
                    {
                        glFiscalYearVM = itemVM;
                        break;
                    }
                    if (glFiscalYearVM == null)
                    {
                        List<GLFiscalYearDetailVM> glFiscalYearDVMs = new List<GLFiscalYearDetailVM>();
                        GLFiscalYearDetailVM glFiscalYearDVM;
                        glFiscalYearVM = new GLFiscalYearVM();
                        var date = Ordinary.DateToString(vm.YearStart);
                        DateTime start_date = new DateTime(Convert.ToInt32(date.Substring(0, 4)), Convert.ToInt32(date.Substring(4, 2)), 01);
                        glFiscalYearVM.YearStart = start_date.ToString("dd-MMM-yyyy");
                        glFiscalYearVM.YearEnd = start_date.AddYears(1).AddDays(-1).ToString("dd-MMM-yyyy");
                        glFiscalYearVM.Year = start_date.Year;
                        for (int i = 0; i < 12; i++)
                        {
                            glFiscalYearDVM = new GLFiscalYearDetailVM();
                            glFiscalYearDVM.PeriodName = start_date.AddMonths(i).ToString("MMMM") + "-" + glFiscalYearVM.Year;
                            glFiscalYearDVM.PeriodStart = start_date.AddMonths(i).ToString("dd-MMM-yyyy");
                            glFiscalYearDVM.PeriodEnd = start_date.AddMonths(i + 1).AddDays(-1).ToString("dd-MMM-yyyy");
                            glFiscalYearDVMs.Add(glFiscalYearDVM);
                        }
                        glFiscalYearVM.glFiscalYearDetailVMs = glFiscalYearDVMs;
                        glFiscalYearVM.CreatedAt = DateTime.Now.ToString("yyyyMMdd");
                        glFiscalYearVM.CreatedBy = vm.LastUpdateBy;
                        glFiscalYearVM.CreatedFrom = vm.LastUpdateFrom;
                        string[] result = fiscalYearDal.Insert(glFiscalYearVM, currConn, transaction, true);
                        if (result[0] == "Fail")
                        {
                            throw new ArgumentNullException("Fiscal year error", "Fiscal year error.");
                        }
                    }
                    #endregion
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update GLCompanyProfiles set";
                    sqlText += "   Name=@Name       ";
                    sqlText += " , LegalName=@LegalName       ";
                    sqlText += " , Address=@Address       ";
                    sqlText += " , City=@City       ";
                    sqlText += " , ZipCode=@ZipCode       ";
                    sqlText += " , TelephoneNo=@TelephoneNo       ";
                    sqlText += " , MobileNo=@MobileNo       ";
                    sqlText += " , FaxNo=@FaxNo     ";
                    sqlText += " , Email=@Email       ";
                    sqlText += " , ContactPerson=@ContactPerson       ";
                    sqlText += " , ContactPersonDesignation=@ContactPersonDesignation       ";
                    sqlText += " , ContactPersonTelephone=@ContactPersonTelephone       ";
                    sqlText += " , ContactPersonEmail=@ContactPersonEmail       ";
                    sqlText += " , TIN=@TIN";
                    sqlText += " , BIN=@BIN";
                    sqlText += " , VATNo=@VATNo";
                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " , YearStart=@YearStart";
                    sqlText += " , Year=@Year";
                    sqlText += " where Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name);
                    cmdUpdate.Parameters.AddWithValue("@VATNo", vm.VATNo);
                    cmdUpdate.Parameters.AddWithValue("@LegalName", vm.LegalName ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ZipCode", vm.ZipCode ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@MobileNo", vm.MobileNo ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@TIN", vm.TIN ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@BIN", vm.BIN ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, bankVM.Remarks);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Parameters.AddWithValue("@YearStart", Ordinary.DateToString(glFiscalYearVM.YearStart));
                    cmdUpdate.Parameters.AddWithValue("@Year", glFiscalYearVM.Year);
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", GLCompanyProfileVM.GLCompanyProfileId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("GLCompanyProfile Update", "Could not found any item.");
                }
                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                    retResults[0] = "Success";
                    retResults[1] = "Data Update Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to update GLCompanyProfile.";
                    throw new ArgumentNullException("", "");
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
        //==================Delete =================
        public string[] Delete(GLCompanyProfileVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLCompanyProfile"; //Method Name
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;
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
                    currConn = _dbsqlConnection.GetConnectionSageGL();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToGLCompanyProfile"); }
                #endregion open connection and transaction
                #region Check is  it used
                #endregion Check is  it used
                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update GLCompanyProfiles set";
                        sqlText += " IsActive=@IsActive,";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " ,LastUpdateBy=@LastUpdateBy";
                        sqlText += " ,LastUpdateAt=@LastUpdateAt";
                        sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsActive", false);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("GLCompanyProfile Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("GLCompanyProfile Information Delete", "Could not found any item.");
                }
                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                    retResults[0] = "Success";
                    retResults[1] = "Data Delete Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to delete GLCompanyProfile Information.";
                    throw new ArgumentNullException("", "");
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
        public List<GLCompanyProfileVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<GLCompanyProfileVM> VMs = new List<GLCompanyProfileVM>();
            GLCompanyProfileVM vm;
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnectionSageGL();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
SELECT
Id,
Name
   FROM GLCompanyProfiles
WHERE IsArchive=0 and IsActive=1
   -- ORDER BY Name
";
                SqlCommand _objComm = new SqlCommand(sqlText, currConn);
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLCompanyProfileVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Name = dr["Name"].ToString();
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
        #endregion
    }
}
