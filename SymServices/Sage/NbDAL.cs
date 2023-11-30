using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymOrdinary;
using SymServices.Common;
using SymViewModel.Sage;


namespace SymServices.Sage
{
    public class NbDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods

        //==================DropDownAll=================
        public List<NBViewModel> DropDownAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<NBViewModel> VMs = new List<NBViewModel>();
            NBViewModel vm;
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
SELECT * FROM(
SELECT 
'B' Sl, Id
, Code
, Name
FROM NewBranch
WHERE IsArchive=0 and IsActive=1
UNION 
SELECT 
'A' Sl, '-1' Id
,'000'
, 'ALL Branch' Name  
FROM NewBranch
)
AS a
order by Sl,Name
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new NBViewModel();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Name = dr["Name"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Name = vm.Name + " ( " + vm.Code + " )";
                    vm.Code = vm.Code + "~" + vm.Id;
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
        public List<NBViewModel> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<NBViewModel> VMs = new List<NBViewModel>();
            NBViewModel vm;
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
af.Id
,af.Code
,af.Name
   FROM NewBranch af
WHERE  1=1 AND af.IsArchive = 0
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new NBViewModel();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Name = dr["Name"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Name = vm.Name + " ( " + vm.Code + " )";
                    vm.Code = vm.Code + "~" + vm.Id;
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
        public List<NBViewModel> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<NBViewModel> VMs = new List<NBViewModel>();
            NBViewModel vm;
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
                #region SqlText

                sqlText = @"
SELECT
 Id
,Code
,Name
,Address
,City
,ZipCode
,TelephoneNo
,MobileNo
,FaxNo
,Email
,ContactPerson
,ISNULL(ContactPersonDesignation, 0) ContactPersonDesignation
,ContactPersonTelephone
,ContactPersonEmail
,ISNULL(IsHO, 0)IsHO
,ISNULL(DebitAccountId, 0)DebitAccountId
,ISNULL(CreditAccountId, 0)CreditAccountId
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom

FROM NewBranch  
WHERE  1=1  AND IsArchive = 0

";


                if (Id > 0)
                {
                    sqlText += @" and Id=@Id";
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
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new NBViewModel();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
                    vm.Name = dr["Name"].ToString();
                    vm.Address = dr["Address"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.ZipCode = dr["ZipCode"].ToString();
                    vm.TelephoneNo = dr["TelephoneNo"].ToString();
                    vm.MobileNo = dr["MobileNo"].ToString();
                    vm.FaxNo = dr["FaxNo"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.ContactPerson = dr["ContactPerson"].ToString();
                    vm.ContactPersonDesignation = Convert.ToInt32(dr["ContactPersonDesignation"].ToString());
                    vm.ContactPersonTelephone = dr["ContactPersonTelephone"].ToString();
                    vm.ContactPersonEmail = dr["ContactPersonEmail"].ToString();

                    vm.IsHO = Convert.ToBoolean(dr["IsHO"]);
                    vm.DebitAccountId = Convert.ToInt32(dr["DebitAccountId"]);
                    vm.CreditAccountId = Convert.ToInt32(dr["CreditAccountId"]);

                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
        //==================Insert =================
        public string[] Insert(NBViewModel vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertGLBranch"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
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
                vm.Id = _cDal.NextId("NewBranch", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO NewBranch(
Code
,Name
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
,IsHO 
,DebitAccountId 
,CreditAccountId 
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Code
,@Name
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
,@IsHO
,@DebitAccountId
,@CreditAccountId
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    //cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@Code", vm.Code.Trim());
                    cmdInsert.Parameters.AddWithValue("@Name", vm.Name.Trim());
                    cmdInsert.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ZipCode", vm.ZipCode ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@MobileNo", vm.MobileNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation);
                    cmdInsert.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsHO", vm.IsHO);
                    cmdInsert.Parameters.AddWithValue("@DebitAccountId", vm.DebitAccountId);
                    cmdInsert.Parameters.AddWithValue("@CreditAccountId", vm.CreditAccountId);

                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update NewBranch.", "");
                    }
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This GLBranch already used!";
                    throw new ArgumentNullException("Please Input GLBranch Value", "");
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
        public string[] Update(NBViewModel vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "GLBranchUpdate"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Update"); }
                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Update Settings
                    #region SqlText
                    sqlText = "";
                    sqlText = "UPDATE NewBranch SET";

                    sqlText += "  Code=@Code";
                    sqlText += " , Name=@Name";
                    sqlText += " , Address=@Address";
                    sqlText += " , City=@City";
                    sqlText += " , ZipCode=@ZipCode";
                    sqlText += " , TelephoneNo=@TelephoneNo";
                    sqlText += " , MobileNo=@MobileNo";
                    sqlText += " , FaxNo=@FaxNo";
                    sqlText += " , Email=@Email";
                    sqlText += " , ContactPerson=@ContactPerson";
                    sqlText += " , ContactPersonDesignation=@ContactPersonDesignation";
                    sqlText += " , ContactPersonTelephone=@ContactPersonTelephone";
                    sqlText += " , ContactPersonEmail=@ContactPersonEmail";
                    sqlText += " , IsHO=@IsHO";
                    sqlText += " , DebitAccountId=@DebitAccountId";
                    sqlText += " , CreditAccountId=@CreditAccountId";


                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " WHERE Id=@Id";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@Code", vm.Code.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ZipCode", vm.ZipCode ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@MobileNo", vm.MobileNo ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation);
                    cmdUpdate.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsHO", vm.IsHO);
                    cmdUpdate.Parameters.AddWithValue("@DebitAccountId", vm.DebitAccountId);
                    cmdUpdate.Parameters.AddWithValue("@CreditAccountId", vm.CreditAccountId);

                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update NewBranch.", "");
                    }
                    #endregion SqlExecution

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("GLBranch Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLBranch Update", "Could not found any item.");
                }
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
        ////==================Delete =================
        public string[] Delete(NBViewModel vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLBranch"; //Method Name
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
                    currConn = _dbsqlConnection.GetConnectionSageGL();
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
                        sqlText = "update NewBranch set";
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
                        throw new ArgumentNullException("GLBranch Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLBranch Information Delete", "Could not found any item.");
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
    }
}
