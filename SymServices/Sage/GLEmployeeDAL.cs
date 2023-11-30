using SymOrdinary;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Linq;
using SymServices.Common;

namespace SymServices.Sage
{
    public class GLEmployeeDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
        #region Methods
        //==================SelectAll=================
        public List<GLEmployeeVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLEmployeeVM> VMs = new List<GLEmployeeVM>();
            GLEmployeeVM vm;
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
 e.Id
,e.Code
,e.Name
,e.MobileNo
,e.Address
,e.City
,e.TelephoneNo
,e.FaxNo
,e.Email
,e.Designation
,e.Department
,e.TIN
,e.BIN
,ISNULL(e.BranchId, 0) BranchId
,b.Name BranchName
,b.Code BranchCode
,e.Remarks
,e.IsActive
,e.IsArchive
,e.CreatedBy
,e.CreatedAt
,e.CreatedFrom
,e.LastUpdateBy
,e.LastUpdateAt
,e.LastUpdateFrom
From GLEmployees e
LEFT OUTER JOIN GLBranchs b ON b.Id = e.BranchId 
Where  e.IsArchive=0
";
                if (Id > 0)
                {
                    sqlText += @" and e.Id=@Id";
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
                sqlText += @" ORDER BY e.Name";

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
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLEmployeeVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
                    vm.Name = dr["Name"].ToString();
                    vm.MobileNo = dr["MobileNo"].ToString();
                    vm.Address = dr["Address"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.TelephoneNo = dr["TelephoneNo"].ToString();
                    vm.FaxNo = dr["FaxNo"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.TIN = dr["TIN"].ToString();
                    vm.BIN = dr["BIN"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.BranchName = dr["BranchName"].ToString();
                    vm.BranchCode = dr["BranchCode"].ToString();
                    vm.BranchName = vm.BranchName + " ("+vm.BranchCode+")";

                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
        public string[] Insert(GLEmployeeVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertGLEmployee"; //Method Name
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
                #endregion open connection and transaction
                #region Save
                vm.Id = Ordinary.NextId("GLEmployees", currConn, transaction);
                if (vm != null)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO GLEmployees(Id
,Code
,Name
,MobileNo
,Address
,City
,TelephoneNo
,FaxNo
,Email
,Designation
,Department
,TIN
,BIN
,BranchId
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
VALUES (@Id
,@Code
,@Name
,@MobileNo
,@Address
,@City
,@TelephoneNo
,@FaxNo
,@Email
,@Designation
,@Department
,@TIN
,@BIN
,@BranchId
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) 
";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@Code", vm.Code);
                    cmdInsert.Parameters.AddWithValue("@Name", vm.Name);
                    cmdInsert.Parameters.AddWithValue("@MobileNo", vm.MobileNo);
                    cmdInsert.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Designation", vm.Designation ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Department", vm.Department ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@TIN", vm.TIN ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@BIN", vm.BIN ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);

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
                    retResults[1] = "This GLGLEmployee already used!";
                    throw new ArgumentNullException("Please Input GLGLEmployee Value", "");
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
        public string[] Update(GLEmployeeVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee GLGLEmployee Update"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToGLEmployee"); }
                #endregion open connection and transaction
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update GLEmployees set";
                    sqlText += " Code =@Code";
                    sqlText += " , Name =@Name";
                    sqlText += " , MobileNo =@MobileNo";
                    sqlText += " , Address =@Address";
                    sqlText += " , City =@City";
                    sqlText += " , TelephoneNo =@TelephoneNo";
                    sqlText += " , FaxNo =@FaxNo";
                    sqlText += " , Email =@Email";
                    sqlText += " , Designation =@Designation";
                    sqlText += " , Department =@Department";
                    sqlText += " , TIN =@TIN";
                    sqlText += " , BIN =@BIN";
                    sqlText += " , BranchId =@BranchId";

                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@Code", vm.Code);
                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name);
                    cmdUpdate.Parameters.AddWithValue("@MobileNo", vm.MobileNo);
                    cmdUpdate.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Designation", vm.Designation ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Department", vm.Department ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@TIN", vm.TIN ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@BIN", vm.BIN ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@BranchId", vm.BranchId);

                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, bankVM.Remarks);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);



                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", GLEmployeeVM.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("GLGLEmployee Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update GLGLEmployee.";
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
        public string[] Delete(GLEmployeeVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLEmployee"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToGLEmployee"); }
                #endregion open connection and transaction
                #region Check is  it used
                #endregion Check is  it used
                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update GLEmployees set";
                        sqlText += " IsActive=@IsActive";
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
                        throw new ArgumentNullException("GLGLEmployee Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("GLGLEmployee Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete GLGLEmployee Information.";
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
        public List<string> AutocompleteName(string term)
        {
            #region Variables
            SqlConnection currConn = null;
            List<string> vms = new List<string>();
            string sqlText = "";
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
                sqlText = @"SELECT Name   FROM GLEmployees ";
                sqlText += @" WHERE Name like '%" + term + "%' order by  Name";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    vms.Insert(i, dr["Name"].ToString());
                    i++;
                }
                dr.Close();
                vms.Sort();
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
            return vms;
        }
        public List<GLEmployeeVM> DropDown(int branchId = 0)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<GLEmployeeVM> VMs = new List<GLEmployeeVM>();
            GLEmployeeVM vm;
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
   FROM GLEmployees
WHERE IsArchive=0 and IsActive=1
   -- 
";
                if (branchId > 0)
                {
                    sqlText += @" AND BranchId=@BranchId";
                }
                sqlText += @" ORDER BY Name";

                SqlCommand _objComm = new SqlCommand(sqlText, currConn);
                if (branchId > 0)
                {
                    _objComm.Parameters.AddWithValue("@BranchId", branchId);
                }
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLEmployeeVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
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
        //==================Report=================
        public DataTable Report(GLEmployeeVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
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
e.Code
,e.Name
,e.MobileNo
,e.Address
,e.City
,e.TelephoneNo
,e.FaxNo
,e.Email
,e.Designation
,e.TIN
,e.BIN
,e.BranchId
,e.Remarks
From GLEmployees e
Where  e.IsArchive=0
";
                if (!string.IsNullOrWhiteSpace(vm.Name))
                {
                    sqlText += @" and  e.Name=@Name";
                }
                //sqlText += @" ORDER BY Name";
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                if (!string.IsNullOrWhiteSpace(vm.Name))
                {
                    da.SelectCommand.Parameters.AddWithValue("@Name", vm.Name);
                }
                da.Fill(dt);
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
            return dt;
        }
        public int SelectByName(string Name, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            int result = 0;
            #endregion
            try
            {
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
                #region open connection and transaction
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
 e.Id
,e.Name
From GLEmployees e
";
                sqlText += @" where e.name=@Name";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@Name", Name);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    result = Convert.ToInt32(dr["Id"]);
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
            return result;
        }
        #endregion
    }
}
