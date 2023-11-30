using SymOrdinary;
using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SymServices.Sage
{
    public class GLUserBranchDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods

        public List<GLUserBranchVM> DropDown(string gluserid = "", string Admin = "N")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<GLUserBranchVM> VMs = new List<GLUserBranchVM>();
            GLUserBranchVM vm;
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
                if (Admin == "Y")
                {
                    sqlText = @"
select b.Id,b.Name,b.Code from GLBranchs  b
where b.isactive=1 and b.isarchive=0 ";

                }
                else
                {
                    sqlText = @"
select b.Id,b.Name,b.Code from GLUserBranchs ub
left outer join GLBranchs b on b.id=ub.BranchId
left outer join Glusers u on u.id=ub.gluserid
where b.isactive=1 and b.isarchive=0 
";

                    if (!string.IsNullOrWhiteSpace(gluserid) )
                    {
                        sqlText += @" and  u.LogId=@gluserid";
                    }
                }
                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                if (!string.IsNullOrWhiteSpace(gluserid) && Admin == "N")
                {
                    objComm.Parameters.AddWithValue("@gluserid", gluserid);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLUserBranchVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Name = dr["Name"].ToString().Trim() + " ( " + dr["Code"].ToString().Trim() + " )";
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

        //==================SelectByMasterId=================
        public List<GLUserBranchVM> SelectByMasterId(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLUserBranchVM> VMs = new List<GLUserBranchVM>();
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
                string[] conditionField = { "ub.GLUserId" };
                string[] conditionValue = { Id.ToString() };
                VMs = SelectAll(conditionField, conditionValue, currConn, transaction);
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
        //==================SelectById=================
        public List<GLUserBranchVM> SelectById(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLUserBranchVM> VMs = new List<GLUserBranchVM>();
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
                string[] conditionField = { "ub.Id" };
                string[] conditionValue = { Id.ToString() };
                VMs = SelectAll(conditionField, conditionValue, currConn, transaction);
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

        //==================SelectAll=================
        public List<GLUserBranchVM> SelectAll(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLUserBranchVM> VMs = new List<GLUserBranchVM>();
            GLUserBranchVM vm;
            #endregion
            try
            {
                #region open connection and transaction
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionSageGL();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                else if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                else if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion open connection and transaction

                #region sql statement
                sqlText = @"
SELECT 
ub.Id
,ub.GLUserId
,ub.BranchId
,b.Name BranchName
From GLUserBranchs ub
LEFT OUTER JOIN GLBranchs b ON ub.BranchId = b.Id
Where   1=1
";


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
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLUserBranchVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.GLUserId = Convert.ToInt32(dr["GLUserId"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.BranchName = dr["BranchName"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
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


        //==================SelectAll=================
        public List<GLUserBranchVM> SelectAllBranch(GLUserBranchVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLUserBranchVM> VMs = new List<GLUserBranchVM>();
            GLUserBranchVM vm;
            #endregion
            try
            {
                #region open connection and transaction
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionSageGL();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                else if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                else if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion open connection and transaction

                #region sql statement
                sqlText = @"
--declare @GLUserId as int
--set @GLUserId = 1

SELECT * FROM 
(
SELECT 
ub.BranchId
,b.Name+' ('+b.Code+')' BranchName
,b.Address BranchAddress
,1 IsBranchChecked
From GLUserBranchs ub
LEFT OUTER JOIN GLBranchs b ON ub.BranchId = b.Id
Where   1=1
and ub.GLUserId = @GLUserId

UNION ALL

SELECT
b.Id                    BranchId
,b.Name+' ('+b.Code+')'  BranchName
,b.Address              BranchAddress
,0 IsBranchChecked
From GLBranchs b
Where   1=1
and b.Id NOT IN(SELECT BranchId From GLUserBranchs WHERE 1=1 and GLUserId=@GLUserId)
)
as a
ORDER BY BranchName
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@GLUserId", paramVM.GLUserId);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLUserBranchVM();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.BranchName = dr["BranchName"].ToString();
                    vm.BranchAddress = dr["BranchAddress"].ToString();
                    vm.IsBranchChecked = Convert.ToBoolean(dr["IsBranchChecked"]);
                    VMs.Add(vm);
                }
                dr.Close();
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
        public string[] Insert(GLUserBranchVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = { "Fail", "Fail", Id.ToString(), sqlText, "ex", "InsertGLUserBranch" };
            //0 - Success or Fail//1 - Success or Fail Message//2 - Return Id//3 - SQL Query//4 - catch ex//5 - Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try
            try
            {
                #region open connection and transaction
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionPF();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                else if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                else if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #region Exist
                //CommonDAL cdal = new CommonDAL();
                //bool check = false;
                //string tableName = "GLUserBranch";	
                //string[] fieldName = { "Code", "Name" };
                //string[] fieldValue = { vm.Code.Trim(), vm.Name.Trim() };
                //for (int i = 0; i < fieldName.Length; i++)
                //{
                //    check = cdal.CheckDuplicateInInsertWithBranch(tableName, fieldName[i], fieldValue[i], vm.BranchId, currConn, transaction);
                //    if (check == true)
                //    {
                //        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                //        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                //    }
                //}
                #endregion Exist
                #endregion open connection and transaction
                #region Save
                vm.Id = Ordinary.NextId("GLUserBranchs", currConn, transaction);
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO GLUserBranchs(Id
,GLUserId

,BranchId
) 
VALUES (@Id
,@GLUserId
,@BranchId
) 
";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@GLUserId", vm.GLUserId);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.ExecuteNonQuery();
                }
                else
                {
                    retResults[1] = "This GLUserBranch already used!";
                    throw new ArgumentNullException("Please Input GLUserBranch Value", "");
                }
                #endregion Save
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }


        //==================Insert =================
        public string[] InsertUserBranch(List<GLUserBranchVM> VMs, GLUserBranchVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "UserBranch"; //Method Name
            int transResult = 0;
            string sqlText = "";
            bool iSTransSuccess = false;

            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Insert"); }

                #endregion open connection and transaction

                sqlText = "  ";
                sqlText += @" INSERT INTO dbo.GLUserBranchs
(
GLUserId
,BranchId
)
VALUES
(
@GLUserId
,@BranchId

)";
                if (VMs.Count >= 1)
                {
                    #region Update Settings
                    #region CheckPoint
                    string[] conditionFields = { "GLUserId" };
                    string[] conditionValues = { vm.GLUserId.ToString() };
                    retResults = _cDal.DeleteTableMultiCondition("GLUserBranchs", conditionFields, conditionValues, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        retResults[1] = "Update Fail!";
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    #endregion CheckPoint
                    foreach (var item in VMs)
                    {
                        if (!item.IsBranchChecked)
                        {
                            continue;
                        }
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@GLUserId", vm.GLUserId);
                        cmdInsert.Parameters.AddWithValue("@BranchId", item.BranchId);
                        transResult = cmdInsert.ExecuteNonQuery();
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #endregion Insert Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException(" User Branch", "Could not found any item.");
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
                    retResults[1] = "User Branch Saved Successfully.";

                }
                else
                {
                    retResults[1] = "Unexpected error to User Branch Insert.";
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

        #endregion Methods

    }
}
