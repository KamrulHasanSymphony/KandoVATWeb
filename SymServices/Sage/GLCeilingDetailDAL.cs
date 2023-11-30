using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using SymOrdinary;
using SymServices.Common;
using SymViewModel.Sage;
using SymViewModel.Common;
using System.IO;
using Excel;

namespace SymServices.Sage
{
    public class GLCeilingDetailDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================SelectByMasterId=================
        public List<GLCeilingDetailVM> SelectByMasterId(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLCeilingDetailVM> VMs = new List<GLCeilingDetailVM>();
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
                string[] conditionField = { "fd.GLCeilingId" };
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
        public List<GLCeilingDetailVM> SelectById(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLCeilingDetailVM> VMs = new List<GLCeilingDetailVM>();
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
                string[] conditionField = { "fd.Id" };
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
        ////==================SelectAllCeilingDetail=================

        public DataTable SelectAllCeilingDetailDownload(int BranchId, int GLFiscalYearId, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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

select GLAccounts.id AccountId,  GLAccounts.Name AccountName, GLAccounts.Code AccountCode
,isnull(b.A,0)January,isnull(b.B,0)February,isnull(b.C,0)March ,isnull(b.D,0)April 
,isnull(b.E,0)May ,isnull(b.F,0)June ,isnull(b.G,0)July ,isnull(b.H,0)August 
,isnull(b.I,0)September ,isnull(b.J,0)October,isnull(b.K,0)November,isnull(b.L,0)December 
,(
 isnull(b.A  ,0)
+isnull(b.B  ,0)
+isnull(b.C  ,0)
+isnull(b.D  ,0)
+isnull(b.E  ,0)
+isnull(b.F  ,0)
+isnull(b.G  ,0)
+isnull(b.H  ,0)
+isnull(b.I  ,0)
+isnull(b.J ,0)
+isnull(b.K ,0)
+isnull(b.L ,0)
+isnull(b.B  ,0)
) LineTotal
from GLAccounts
left outer join(
select distinct AccountId
,sum(isnull(A,0))A,sum(isnull(B,0))B,sum(isnull(C,0))C ,sum(isnull(D,0))D ,sum(isnull(E,0))E ,sum(isnull(F,0))F ,sum(isnull(G,0))G ,sum(isnull(H,0))H ,sum(isnull(I,0))I ,sum(isnull(J,0))J ,sum(isnull(K,0))K ,sum(isnull(L,0))L 
from(
select * from GLCeilingDetails    

pivot (
   MAX (Amount)     
   for PeriodSl in ([A],[B],[C],D,E,F,G,H,I,J,K,L))  
   as MaxIncomePerDay 
   ) as a
   left outer join GLCeilings c on c.Id=a.GLCeilingId
   where 1=1
   and c.BranchId=@BranchId and c.GLFiscalYearId=@GLFiscalYearId

    group by  AccountId) as b on b.AccountId=GLAccounts.id
   where GLAccounts.isbde=0 and GLAccounts.AccountNature='Dr' AND GLAccounts.BranchId=@BranchId
   AND GLAccounts.BusinessNature IN('Expense', 'BankCharge')
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
                #endregion SqlText
                #region SqlExecution

                //SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);

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
                        da.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }
                da.SelectCommand.Parameters.AddWithValue("@BranchId", BranchId);
                da.SelectCommand.Parameters.AddWithValue("@GLFiscalYearId", GLFiscalYearId);

                da.SelectCommand.Transaction = transaction;
                da.Fill(dt);

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
            return dt;
        }

        public List<GLCeilingDetailVM> SelectAllCeilingDetail(int BranchId, int GLFiscalYearId, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLCeilingDetailVM> VMs = new List<GLCeilingDetailVM>();
            GLCeilingDetailVM vm;
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

                DataTable dt = new DataTable();
                dt = SelectAllCeilingDetailDownload(BranchId, GLFiscalYearId, conditionFields, conditionValues, currConn, transaction);
                #region sql statement

                #region SqlExecution


                foreach (DataRow dr in dt.Rows)
                {
                    vm = new GLCeilingDetailVM();
                    vm.AccountId = Convert.ToInt32(dr["AccountId"]);
                    vm.AccountName = dr["AccountName"].ToString();
                    vm.AccountCode = dr["AccountCode"].ToString();
                    vm.AccountName = vm.AccountCode + " ( " + vm.AccountName + " )";

                    vm.AmountP1 = Convert.ToDecimal(dr["January"]);
                    vm.AmountP2 = Convert.ToDecimal(dr["February"]);
                    vm.AmountP3 = Convert.ToDecimal(dr["March"]);
                    vm.AmountP4 = Convert.ToDecimal(dr["April"]);
                    vm.AmountP5 = Convert.ToDecimal(dr["May"]);
                    vm.AmountP6 = Convert.ToDecimal(dr["June"]);
                    vm.AmountP7 = Convert.ToDecimal(dr["July"]);
                    vm.AmountP8 = Convert.ToDecimal(dr["August"]);
                    vm.AmountP9 = Convert.ToDecimal(dr["September"]);
                    vm.AmountP10 = Convert.ToDecimal(dr["October"]);
                    vm.AmountP11 = Convert.ToDecimal(dr["November"]);
                    vm.AmountP12 = Convert.ToDecimal(dr["December"]);
                    vm.LineTotal = Convert.ToDecimal(dr["LineTotal"]);
                    
                    VMs.Add(vm);
                }

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
        ////==================SelectAll=================
        public List<GLCeilingDetailVM> SelectAll(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLCeilingDetailVM> VMs = new List<GLCeilingDetailVM>();
            GLCeilingDetailVM vm;
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
 fd.Id
,fd.BranchId
,fd.GLCeilingId
,fd.AccountId
,fd.GLFiscalYearDetailId
,fd.PeriodStart
,fd.PeriodEnd
,fd.Amount
,fd.Post
,acc.Name AccountName
FROM  GLCeilingDetails fd 
LEFT OUTER JOIN GLAccounts acc ON fd.AccountId = acc.Id
WHERE  1=1

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

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLCeilingDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.GLCeilingId = Convert.ToInt32(dr["GLCeilingId"]);
                    vm.AccountId = Convert.ToInt32(dr["AccountId"]);
                    vm.GLFiscalYearDetailId = Convert.ToInt32(dr["GLFiscalYearDetailId"]);
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.PeriodEnd = dr["PeriodEnd"].ToString();
                    vm.Amount = Convert.ToDecimal(dr["Amount"]);
                    vm.AccountName = dr["AccountName"].ToString();
                    vm.Post = Convert.ToBoolean(dr["Post"]);

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
        public string[] Insert(GLCeilingDetailVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Insert GLCeilingDetail"; //Method Name
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


                vm.Id = _cDal.NextId(" GLCeilingDetails", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO  GLCeilingDetails(
Id
,BranchId
,GLCeilingId
,AccountId
,GLFiscalYearDetailId
,PeriodSl
,PeriodStart
,PeriodEnd
,Amount
,Post

) VALUES (
@Id
,@BranchId
,@GLCeilingId
,@AccountId
,@GLFiscalYearDetailId
,@PeriodSl
,@PeriodStart
,@PeriodEnd
,@Amount
,@Post

) 
";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Parameters.AddWithValue("@GLCeilingId", vm.GLCeilingId);
                    cmdInsert.Parameters.AddWithValue("@AccountId", vm.AccountId);
                    cmdInsert.Parameters.AddWithValue("@GLFiscalYearDetailId", vm.GLFiscalYearDetailId);
                    cmdInsert.Parameters.AddWithValue("@PeriodSl", vm.PeriodSl);
                    cmdInsert.Parameters.AddWithValue("@PeriodStart", vm.PeriodStart);
                    cmdInsert.Parameters.AddWithValue("@PeriodEnd", vm.PeriodEnd);
                    cmdInsert.Parameters.AddWithValue("@Amount", vm.Amount);
                    cmdInsert.Parameters.AddWithValue("@Post", false);


                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update  GLCeilingDetails.", "");
                    }
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This  GLCeilingDetail already used!";
                    throw new ArgumentNullException("Please Input  GLCeilingDetail Value", "");
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
        //==================Insert =================
        public string[] Insert(List<GLCeilingDetailVM> VMs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertGLCeilingDetail"; //Method Name
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


                int next_Id = _cDal.NextId("GLCeilingDetails", currConn, transaction);
                if (VMs != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO  GLCeilingDetails(
Id
,BranchId
,GLCeilingId
,AccountId
,GLFiscalYearDetailId
,PeriodSl
,PeriodStart
,PeriodEnd
,Amount
,Post

) VALUES (
@Id
,@BranchId
,@GLCeilingId
,@AccountId
,@GLFiscalYearDetailId
,@PeriodSl
,@PeriodStart
,@PeriodEnd
,@Amount
,@Post

) 
";

                    #endregion SqlText
                    GLCeilingDetailDAL _dal = new GLCeilingDetailDAL();
                    List<GLCeilingDetailVM> glCeilingDetailVMs = new List<GLCeilingDetailVM>();
                    if (VMs != null && VMs.Count > 0)
                    {
                        int GLCeilingId = VMs.FirstOrDefault().GLCeilingId;
                        int BranchId = VMs.FirstOrDefault().BranchId;

                        retResults = _cDal.DeleteTableInformation(GLCeilingId.ToString(), "GLCeilingDetails", "GLCeilingId", currConn, transaction);
                        if (retResults[0] == "Fail")
                        {
                            throw new ArgumentNullException(retResults[1], retResults[1]);
                        }
                        #region FiscalYearDetail
                        GLFiscalYearVM glFYearVM = new GLFiscalYearVM();
                        GLFiscalYearDAL _glFYearDAL = new GLFiscalYearDAL();
                        glFYearVM = _glFYearDAL.SelectAll(VMs.FirstOrDefault().GLFiscalYearId.ToString()).FirstOrDefault();
                        List<GLFiscalYearDetailVM> glFYearDVMs = new List<GLFiscalYearDetailVM>();
                        glFYearDVMs = glFYearVM.glFiscalYearDetailVMs;
                        #endregion
                        #region insert Details from Master into Detail Table

                        GLCeilingDetailVM dVM = new GLCeilingDetailVM();

                        foreach (GLCeilingDetailVM detailVM in VMs)
                        {
                            #region Data Assign
                            foreach (GLFiscalYearDetailVM item in glFYearDVMs)
                            {
                                dVM = new GLCeilingDetailVM();
                                dVM.GLCeilingId = GLCeilingId;
                                dVM.BranchId = BranchId;
                                dVM.AccountId = detailVM.AccountId;
                                dVM.GLFiscalYearDetailId = item.Id;
                                dVM.PeriodSl = item.PeriodSl;
                                dVM.PeriodStart = item.PeriodStart;
                                dVM.PeriodEnd = item.PeriodEnd;
                                #region Switching
                                switch (item.PeriodSl.ToLower())
                                {
                                    case "a":
                                        dVM.Amount = detailVM.AmountP1;
                                        break;
                                    case "b":
                                        dVM.Amount = detailVM.AmountP2;
                                        break;
                                    case "c":
                                        dVM.Amount = detailVM.AmountP3;
                                        break;
                                    case "d":
                                        dVM.Amount = detailVM.AmountP4;
                                        break;
                                    case "e":
                                        dVM.Amount = detailVM.AmountP5;
                                        break;
                                    case "f":
                                        dVM.Amount = detailVM.AmountP6;
                                        break;
                                    case "g":
                                        dVM.Amount = detailVM.AmountP7;
                                        break;
                                    case "h":
                                        dVM.Amount = detailVM.AmountP8;
                                        break;
                                    case "i":
                                        dVM.Amount = detailVM.AmountP9;
                                        break;
                                    case "j":
                                        dVM.Amount = detailVM.AmountP10;
                                        break;
                                    case "k":
                                        dVM.Amount = detailVM.AmountP11;
                                        break;
                                    case "l":
                                        dVM.Amount = detailVM.AmountP12;
                                        break;
                                }
                                #endregion
                                glCeilingDetailVMs.Add(dVM);
                            }
                            #endregion
                        }
                        //retResults = _dal.InsertCeilingDetailList(glCeilingDetailVMs, currConn, transaction);
                        //if (retResults[0] == "Fail")
                        //{
                        //    throw new ArgumentNullException(retResults[1], "");
                        //}
                    }
                        #endregion insert Details from Master into Detail Table

                    #region SqlExecution

                    foreach (GLCeilingDetailVM item in glCeilingDetailVMs)
                    {
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@Id", next_Id);
                        cmdInsert.Parameters.AddWithValue("@BranchId", item.BranchId);
                        cmdInsert.Parameters.AddWithValue("@GLCeilingId", item.GLCeilingId);
                        cmdInsert.Parameters.AddWithValue("@AccountId", item.AccountId);
                        cmdInsert.Parameters.AddWithValue("@GLFiscalYearDetailId", item.GLFiscalYearDetailId);
                        cmdInsert.Parameters.AddWithValue("@PeriodSl", item.PeriodSl);
                        cmdInsert.Parameters.AddWithValue("@PeriodStart", Ordinary.DateToString(item.PeriodStart));
                        cmdInsert.Parameters.AddWithValue("@PeriodEnd", Ordinary.DateToString(item.PeriodEnd));
                        cmdInsert.Parameters.AddWithValue("@Amount", item.Amount);
                        cmdInsert.Parameters.AddWithValue("@Post", false);

                        var exeRes = cmdInsert.ExecuteNonQuery();
                        next_Id++;
                    }
                    //transResult = Convert.ToInt32(exeRes);
                    //if (transResult <= 0)
                    //{
                    //    retResults[3] = sqlText;
                    //    throw new ArgumentNullException("Unexpected error to update  GLCeilingDetails.", "");
                    //}
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This  GLCeilingDetail already used!";
                    throw new ArgumentNullException("Please Input  GLCeilingDetail Value", "");
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
                retResults[2] = next_Id.ToString();
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

        public string[] ImportExcelFile(string Fullpath, string fileName, GLCeilingDetailVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "ImportExcelFile"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            List<GLCeilingDetailVM> VMs = new List<GLCeilingDetailVM>();
            GLCeilingDetailVM vm;
            #region try
            try
            {
                DataSet ds = new DataSet();
                System.Data.DataTable dt = new System.Data.DataTable();
                FileStream stream = System.IO.File.Open(Fullpath, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;
                if (fileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (fileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();
                dt = ds.Tables[0];
                reader.Close();
                //dt = ds.Tables[0].Select("empCode <>''").CopyToDataTable();

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

                foreach (DataRow dr in dt.Rows)
                {

                    vm = new GLCeilingDetailVM();
                    vm.AccountId = Convert.ToInt32(dr["AccountId"]);
                    vm.AccountName = dr["AccountName"].ToString();
                    vm.AmountP1 = Convert.ToDecimal(dr["January"]);
                    vm.AmountP2 = Convert.ToDecimal(dr["February"]);
                    vm.AmountP3 = Convert.ToDecimal(dr["March"]);
                    vm.AmountP4 = Convert.ToDecimal(dr["April"]);
                    vm.AmountP5 = Convert.ToDecimal(dr["May"]);
                    vm.AmountP6 = Convert.ToDecimal(dr["June"]);
                    vm.AmountP7 = Convert.ToDecimal(dr["July"]);
                    vm.AmountP8 = Convert.ToDecimal(dr["August"]);
                    vm.AmountP9 = Convert.ToDecimal(dr["September"]);
                    vm.AmountP10 = Convert.ToDecimal(dr["October"]);
                    vm.AmountP11 = Convert.ToDecimal(dr["November"]);
                    vm.AmountP12 = Convert.ToDecimal(dr["December"]);

                    VMs.Add(vm);
                }

                if (VMs != null && VMs.Count > 0)
                {
                    VMs.FirstOrDefault().GLCeilingId = paramVM.GLCeilingId;
                    VMs.FirstOrDefault().GLFiscalYearId = paramVM.GLFiscalYearId;
                    VMs.FirstOrDefault().BranchId = paramVM.BranchId;

                    retResults = Insert(VMs, currConn, transaction);
                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException("CeilingDetail - Unexpected Error", "");
                    }
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
                //retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
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


        #endregion
    }
}
