using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SymViewModel.Sage;
using SymOrdinary;

using SymServices.Common;

namespace SymServices.Sage
{
    public class GLFiscalYearDAL
    {
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #region Methods
        public List<GLFiscalYearVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<GLFiscalYearVM> VMs = new List<GLFiscalYearVM>();
            GLFiscalYearVM vm;
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
f.Id
,f.Year Name
   FROM GLFiscalYears f
WHERE  1=1 AND f.IsArchive = 0
Order By Name desc
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLFiscalYearVM();
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

        public List<GLFiscalYearDetailVM> DropDownFiscalYearDetail(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLFiscalYearDetailVM> VMs = new List<GLFiscalYearDetailVM>();
            GLFiscalYearDetailVM vm = new GLFiscalYearDetailVM();
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
,PeriodName Name
    From GLFiscalYearDetails
where  1=1
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

                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new GLFiscalYearDetailVM();
                        vm.Id = Convert.ToInt32(dr["Id"]);
                        vm.Name = dr["Name"].ToString();
                        VMs.Add(vm);
                    }
                    dr.Close();
                }

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


        public List<GLFiscalYearVM> SelectAll(string Id = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<GLFiscalYearVM> VMs = new List<GLFiscalYearVM>();
            GLFiscalYearVM vm = new GLFiscalYearVM();
            List<GLFiscalYearDetailVM> GLFiscalYearDetailVMs = new List<GLFiscalYearDetailVM>();
            GLFiscalYearDetailVM GLFiscalYearDetailVM = new GLFiscalYearDetailVM();
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
Id
,BranchId
,Year
,YearStart
,YearEnd
,YearLock
,Remarks
,IsActive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From GLFiscalYears
WHERE 1=1 AND IsArchive = 0
";
                if (!string.IsNullOrWhiteSpace(Id))
                {
                    sqlText += " AND Id=@Id";
                }
                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                if (!string.IsNullOrWhiteSpace(Id))
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLFiscalYearVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.Year = Convert.ToInt32(dr["Year"]);
                    vm.YearStart = Ordinary.StringToDate(dr["YearStart"].ToString());
                    vm.YearEnd = Ordinary.StringToDate(dr["YearEnd"].ToString());
                    vm.YearLock = Convert.ToBoolean(dr["YearLock"]);
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
                sqlText = @"
SELECT
 Id
,GLFiscalYearId
,PeriodSl
,PeriodName
,PeriodStart
,PeriodEnd
,PeriodLock
,Remarks
    From GLFiscalYearDetails
where  GLFiscalYearId=@GLFiscalYearId
";
                SqlCommand cmdDetails = new SqlCommand();
                cmdDetails.Connection = currConn;
                cmdDetails.CommandText = sqlText;
                cmdDetails.CommandType = CommandType.Text;
                cmdDetails.Parameters.AddWithValue("@GLFiscalYearId", vm.Id);
                using (SqlDataReader ddr = cmdDetails.ExecuteReader())
                {
                    while (ddr.Read())
                    {
                        GLFiscalYearDetailVM = new GLFiscalYearDetailVM();
                        GLFiscalYearDetailVM.Id = Convert.ToInt32(ddr["Id"]);
                        GLFiscalYearDetailVM.GLFiscalYearId = Convert.ToInt32(ddr["GLFiscalYearId"]);
                        GLFiscalYearDetailVM.PeriodSl = ddr["PeriodSl"].ToString();
                        GLFiscalYearDetailVM.PeriodName = ddr["PeriodName"].ToString();

                        GLFiscalYearDetailVM.PeriodStart = Ordinary.StringToDate(ddr["PeriodStart"].ToString());
                        GLFiscalYearDetailVM.PeriodEnd = Ordinary.StringToDate(ddr["PeriodEnd"].ToString());
                        GLFiscalYearDetailVM.PeriodLock = Convert.ToBoolean(ddr["PeriodLock"]);
                        GLFiscalYearDetailVM.Remarks = ddr["Remarks"].ToString();
                        GLFiscalYearDetailVMs.Add(GLFiscalYearDetailVM);
                    }
                    ddr.Close();
                }
                vm.glFiscalYearDetailVMs = GLFiscalYearDetailVMs;
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
        public string[] Insert(GLFiscalYearVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool callFromOutSide = false)
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
            retResults[5] = "InsertGLFiscalYear"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL _cDal = new CommonDAL();
            #endregion
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
                #region Exist
                //
                #endregion Exist
                vm.Id = _cDal.NextId("GLFiscalYears", currConn, transaction);
                #region Save
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO GLFiscalYears(
Id
,BranchId
,Year
,YearStart
,YearEnd
,YearLock
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) VALUES (
 @Id
,@BranchId
,@Year
,@YearStart
,@YearEnd
,@YearLock
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom)";
                    SqlCommand _cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    _cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    _cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    _cmdInsert.Parameters.AddWithValue("@Year", vm.Year);
                    _cmdInsert.Parameters.AddWithValue("@YearStart", Ordinary.DateToString(vm.YearStart));
                    _cmdInsert.Parameters.AddWithValue("@YearEnd", Ordinary.DateToString(vm.YearEnd));
                    _cmdInsert.Parameters.AddWithValue("@YearLock", vm.YearLock);
                    _cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    _cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    _cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    _cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    _cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    _cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    _cmdInsert.ExecuteNonQuery();
                    SqlCommand cmdDetails;
                    sqlText = @" INSERT INTO GLFiscalYearDetails(
 Id
,GLFiscalYearId
,Year
,PeriodSl
,PeriodName
,PeriodStart
,PeriodEnd
,PeriodLock
,Remarks
) VALUES (
@Id
,@GLFiscalYearId
,@Year
,@PeriodSl
,@PeriodName
,@PeriodStart
,@PeriodEnd
,@PeriodLock
,@Remarks
)";
                    int FiscalYearDetailId = _cDal.NextId("GLFiscalYearDetails", currConn, transaction);
                    int i = 0;
                    string[] Alphabet = Ordinary.Alphabet;
                    foreach (GLFiscalYearDetailVM item in vm.glFiscalYearDetailVMs)
                    {
                        cmdDetails = new SqlCommand(sqlText, currConn, transaction);
                        cmdDetails.Parameters.AddWithValue("@Id", FiscalYearDetailId);
                        cmdDetails.Parameters.AddWithValue("@GLFiscalYearId", vm.Id);
                        cmdDetails.Parameters.AddWithValue("@Year", vm.Year);
                        cmdDetails.Parameters.AddWithValue("@PeriodSl", Alphabet[i]);
                        cmdDetails.Parameters.AddWithValue("@PeriodName", item.PeriodName);
                        cmdDetails.Parameters.AddWithValue("@PeriodStart", Ordinary.DateToString(item.PeriodStart));
                        cmdDetails.Parameters.AddWithValue("@PeriodEnd", Ordinary.DateToString(item.PeriodEnd));
                        cmdDetails.Parameters.AddWithValue("@PeriodLock", item.PeriodLock);
                        cmdDetails.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);
                        cmdDetails.ExecuteNonQuery();
                        i++;
                        FiscalYearDetailId++;
                    }
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
        public string[] Update(GLFiscalYearVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ
            string sqlText = "";
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = vm.Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "UpdateGLFiscalYear"; //Method Name
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #region Try
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
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
                #region Exist
                //
                #endregion Exist
                #region Save
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += " Update GLFiscalYears set";
                    sqlText += " YearLock           =@YearLock   ";
                    sqlText += " ,Remarks           =@Remarks    ";
                    sqlText += " ,LastUpdateBy   =@LastUpdateBy  ";
                    sqlText += " ,LastUpdateAt   =@LastUpdateAt  ";
                    sqlText += " ,LastUpdateFrom =@LastUpdateFrom";
                    sqlText += " where Id=@Id                    ";
                    SqlCommand _cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    _cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    _cmdUpdate.Parameters.AddWithValue("@YearLock", vm.YearLock);
                    _cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    _cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    _cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    _cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    _cmdUpdate.ExecuteNonQuery();
                    SqlCommand cmdDetails;
                    foreach (GLFiscalYearDetailVM item in vm.glFiscalYearDetailVMs)
                    {
                        sqlText = " ";
                        sqlText += " Update GLFiscalYearDetails set";
                        sqlText += " PeriodLock=@PeriodLock      ";
                        sqlText += " ,Remarks=@Remarks           ";
                        sqlText += " where Id=@Id                ";
                        cmdDetails = new SqlCommand(sqlText, currConn, transaction);
                        cmdDetails.Parameters.AddWithValue("@Id", item.Id);
                        cmdDetails.Parameters.AddWithValue("@PeriodLock", item.PeriodLock);
                        cmdDetails.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);
                        cmdDetails.ExecuteNonQuery();
                    }
                }
                #endregion Save
                #region Commit
                if (transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Commit
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Update Successfully.";
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
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }


        public List<GLFiscalYearDetailVM> SelectAllFiscalYearDetail(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLFiscalYearDetailVM> VMs = new List<GLFiscalYearDetailVM>();
            GLFiscalYearDetailVM vm = new GLFiscalYearDetailVM();
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
,GLFiscalYearId
,PeriodSl
,PeriodName
,PeriodStart
,PeriodEnd
,PeriodLock
,ISNULL(Year,1900)Year
,Remarks
    From GLFiscalYearDetails
where  1=1
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

                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new GLFiscalYearDetailVM();
                        vm.Id = Convert.ToInt32(dr["Id"]);
                        vm.GLFiscalYearId = Convert.ToInt32(dr["GLFiscalYearId"]);
                        vm.PeriodSl = dr["PeriodSl"].ToString();
                        vm.PeriodName = dr["PeriodName"].ToString();

                        vm.PeriodStart = Ordinary.StringToDate(dr["PeriodStart"].ToString());
                        vm.PeriodEnd = Ordinary.StringToDate(dr["PeriodEnd"].ToString());
                        vm.PeriodLock = Convert.ToBoolean(dr["PeriodLock"]);
                        vm.Year = Convert.ToInt32(dr["Year"]);
                        vm.Remarks = dr["Remarks"].ToString();
                        VMs.Add(vm);
                    }
                    dr.Close();
                }

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


        #endregion Methods

    }
}
