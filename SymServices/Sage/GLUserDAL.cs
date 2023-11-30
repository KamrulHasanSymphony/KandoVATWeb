using SymOrdinary;
using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;


namespace SymServices.Sage
{
    public class GLUserDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================DropDown=================
        public List<GLUserVM> DropDown(string tType = "", int branchId = 0)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<GLUserVM> VMs = new List<GLUserVM>();
            GLUserVM vm;
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
g.Id
--,g.BranchName
   FROM GLUsers g
WHERE  1=1 AND g.IsArchive = 0
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLUserVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
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
        //==================SelectUserForMail=================
        public List<GLUserVM> SelectUserForMail(GLUserVM paramVM, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null
            , SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLUserVM> VMs = new List<GLUserVM>();
            GLUserVM vm;
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
--declare @HaveApprovalLevel1 as bit
--declare @BranchId as int
--declare @HaveExpenseApproval as bit

--set @HaveApprovalLevel1=1
--set @BranchId=2
--set @HaveExpenseApproval=1


SELECT
 u.Id
,u.LogId
,u.Password
,u.Designation
,u.Email
,u.IsAdmin
,u.IsAuditor
,u.IsExpense
,u.IsExpenseRequisition
,u.IsBDERequisition
,u.HaveExpenseApproval
,u.HaveExpenseRequisitionApproval
,u.HaveBDERequisitionApproval
,u.HaveApprovalLevel1
,u.HaveApprovalLevel2
,u.HaveApprovalLevel3
,u.HaveApprovalLevel4
,ISNULL(u.FullName, 'NA') FullName
,u.PFNo
,u.Mobile
,u.PhotoFileName
,u.SignatureFileName
 
,u.Remarks
,u.IsActive
,u.IsArchive
,u.CreatedBy
,u.CreatedAt
,u.CreatedFrom
,u.LastUpdateBy
,u.LastUpdateAt
,u.LastUpdateFrom

FROM GLUsers u
WHERE  1=1  AND u.IsArchive = 0
and u.Id in(
select GLUserId from GLUserBranchs
where BranchId=@BranchId)

--and HaveApprovalLevel1=@HaveApprovalLevel1
--and HaveExpenseApproval=@HaveExpenseApproval

";
                //FullName
                //PFNo
                //Mobile
                //PhotoFileName
                //SignatureFileName



                if (paramVM.Id > 0)
                {
                    sqlText += @" and u.Id=@Id";
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

                if (paramVM.Id > 0)
                {
                    objComm.Parameters.AddWithValue("@Id", paramVM.Id);
                }
                objComm.Parameters.AddWithValue("@BranchId", paramVM.BranchId);


                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLUserVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.LogId = dr["LogId"].ToString();
                    vm.Password = dr["Password"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.IsAdmin = Convert.ToBoolean(dr["IsAdmin"]);
                    vm.IsAuditor = Convert.ToBoolean(dr["IsAuditor"]);
                    vm.IsExpense = Convert.ToBoolean(dr["IsExpense"]);
                    vm.IsExpenseRequisition = Convert.ToBoolean(dr["IsExpenseRequisition"]);
                    vm.IsBDERequisition = Convert.ToBoolean(dr["IsBDERequisition"]);
                    vm.HaveExpenseApproval = Convert.ToBoolean(dr["HaveExpenseApproval"]);
                    vm.HaveExpenseRequisitionApproval = Convert.ToBoolean(dr["HaveExpenseRequisitionApproval"]);
                    vm.HaveBDERequisitionApproval = Convert.ToBoolean(dr["HaveBDERequisitionApproval"]);
                    vm.HaveApprovalLevel1 = Convert.ToBoolean(dr["HaveApprovalLevel1"]);
                    vm.HaveApprovalLevel2 = Convert.ToBoolean(dr["HaveApprovalLevel2"]);
                    vm.HaveApprovalLevel3 = Convert.ToBoolean(dr["HaveApprovalLevel3"]);
                    vm.HaveApprovalLevel4 = Convert.ToBoolean(dr["HaveApprovalLevel4"]);
                    vm.FullName = dr["FullName"].ToString();
                    vm.PFNo = dr["PFNo"].ToString();
                    vm.Mobile = dr["Mobile"].ToString();
                    vm.PhotoFileName = dr["PhotoFileName"].ToString();
                    vm.SignatureFileName = dr["SignatureFileName"].ToString();



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

        //==================SelectAll=================
        public List<GLUserVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null
            , SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLUserVM> VMs = new List<GLUserVM>();
            GLUserVM vm;
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
 u.Id
,u.LogId
,u.Password
,u.Designation
,u.Email
,u.IsAdmin
,u.IsAuditor
,u.IsExpense
,u.IsExpenseRequisition
,ISNULL(u.FullName, 'NA') FullName
,u.PFNo
,u.Mobile
,u.PhotoFileName
,u.SignatureFileName
 
,u.Remarks
,u.IsActive
,u.IsArchive
,u.CreatedBy
,u.CreatedAt
,u.CreatedFrom
,u.LastUpdateBy
,u.LastUpdateAt
,u.LastUpdateFrom

FROM GLUsers u
WHERE  1=1  AND u.IsArchive = 0

";
                //FullName
                //PFNo
                //Mobile
                //PhotoFileName
                //SignatureFileName



                if (Id > 0)
                {
                    sqlText += @" and u.Id=@Id";
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
                    vm = new GLUserVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.LogId = dr["LogId"].ToString();
                    vm.Password = dr["Password"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.IsAdmin = Convert.ToBoolean(dr["IsAdmin"]);
                    vm.IsAuditor = Convert.ToBoolean(dr["IsAuditor"]);
                    vm.IsExpense = Convert.ToBoolean(dr["IsExpense"]);
                    vm.IsExpenseRequisition = Convert.ToBoolean(dr["IsExpenseRequisition"]);
                    //vm.IsBDERequisition = Convert.ToBoolean(dr["IsBDERequisition"]);
                    //vm.HaveExpenseApproval = Convert.ToBoolean(dr["HaveExpenseApproval"]);
                    //vm.HaveExpenseRequisitionApproval = Convert.ToBoolean(dr["HaveExpenseRequisitionApproval"]);
                    //vm.HaveBDERequisitionApproval = Convert.ToBoolean(dr["HaveBDERequisitionApproval"]);
                    //vm.HaveApprovalLevel1 = Convert.ToBoolean(dr["HaveApprovalLevel1"]);
                    //vm.HaveApprovalLevel2 = Convert.ToBoolean(dr["HaveApprovalLevel2"]);
                    //vm.HaveApprovalLevel3 = Convert.ToBoolean(dr["HaveApprovalLevel3"]);
                    //vm.HaveApprovalLevel4 = Convert.ToBoolean(dr["HaveApprovalLevel4"]);
                    vm.FullName = dr["FullName"].ToString();
                    vm.PFNo = dr["PFNo"].ToString();
                    vm.Mobile = dr["Mobile"].ToString();
                    vm.PhotoFileName = dr["PhotoFileName"].ToString();
                    vm.SignatureFileName = dr["SignatureFileName"].ToString();



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
        public string[] Insert(GLUserVM vm, HttpPostedFileBase PhotoFile, HttpPostedFileBase SignatureFile, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertGLUser"; //Method Name
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
                vm.Id = _cDal.NextId("GLUsers", currConn, transaction);
                if (vm != null)
                {
                    #region Checkpoint
                    bool IsExist = false;
                    {
                        string[] cFields = { "LogId" };
                        string[] cValues = { vm.LogId };
                        IsExist = _cDal.ExistCheck("GLUsers", cFields, cValues, currConn, transaction);
                        if (IsExist)
                        {
                            retResults[1] = "This Login Id " + vm.LogId + " Already Used!";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }

                    #endregion

                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO GLUsers(
Id
,LogId
,Password
,Designation
,Email
,IsAdmin
,IsAuditor
,IsExpense
,IsExpenseRequisition
,IsBDERequisition
,HaveExpenseApproval
,HaveExpenseRequisitionApproval
,HaveBDERequisitionApproval
,HaveApprovalLevel1
,HaveApprovalLevel2
,HaveApprovalLevel3
,HaveApprovalLevel4
,FullName
,PFNo
,Mobile
,PhotoFileName
,SignatureFileName
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Id
,@LogId
,@Password
,@Designation
,@Email
,@IsAdmin
,@IsAuditor
,@IsExpense
,@IsExpenseRequisition
,@IsBDERequisition
,@HaveExpenseApproval
,@HaveExpenseRequisitionApproval
,@HaveBDERequisitionApproval
,@HaveApprovalLevel1
,@HaveApprovalLevel2
,@HaveApprovalLevel3
,@HaveApprovalLevel4
,@FullName
,@PFNo
,@Mobile
,@PhotoFileName
,@SignatureFileName
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@LogId", vm.LogId);
                    cmdInsert.Parameters.AddWithValue("@Password", vm.Password);
                    cmdInsert.Parameters.AddWithValue("@Designation", vm.Designation.Trim());
                    cmdInsert.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsAdmin", vm.IsAdmin);
                    cmdInsert.Parameters.AddWithValue("@IsAuditor", vm.IsAuditor);
                    cmdInsert.Parameters.AddWithValue("@IsExpense", vm.IsExpense);
                    cmdInsert.Parameters.AddWithValue("@IsExpenseRequisition", vm.IsExpenseRequisition);
                    cmdInsert.Parameters.AddWithValue("@IsBDERequisition", vm.IsBDERequisition);
                    cmdInsert.Parameters.AddWithValue("@HaveExpenseApproval", vm.HaveExpenseApproval);
                    cmdInsert.Parameters.AddWithValue("@HaveExpenseRequisitionApproval", vm.HaveExpenseRequisitionApproval);
                    cmdInsert.Parameters.AddWithValue("@HaveBDERequisitionApproval", vm.HaveBDERequisitionApproval);
                    cmdInsert.Parameters.AddWithValue("@HaveApprovalLevel1", vm.HaveApprovalLevel1);
                    cmdInsert.Parameters.AddWithValue("@HaveApprovalLevel2", vm.HaveApprovalLevel2);
                    cmdInsert.Parameters.AddWithValue("@HaveApprovalLevel3", vm.HaveApprovalLevel3);
                    cmdInsert.Parameters.AddWithValue("@HaveApprovalLevel4", vm.HaveApprovalLevel4);
                    cmdInsert.Parameters.AddWithValue("@FullName", vm.FullName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@PFNo", vm.PFNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Mobile", vm.Mobile ?? Convert.DBNull);

                    if (PhotoFile != null && PhotoFile.ContentLength > 0)
                    {
                        vm.PhotoFileName = vm.LogId + ".jpg"; ;
                        var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Files/Sage/PhotoFile"), vm.PhotoFileName);
                        PhotoFile.SaveAs(path);
                    }


                    if (SignatureFile != null && SignatureFile.ContentLength > 0)
                    {
                        vm.SignatureFileName = vm.LogId + ".jpg"; ;
                        var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Files/Sage/SignatureFile"), vm.SignatureFileName);
                        SignatureFile.SaveAs(path);
                    }

                    cmdInsert.Parameters.AddWithValue("@PhotoFileName", vm.PhotoFileName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@SignatureFileName", vm.SignatureFileName ?? Convert.DBNull);

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
                        throw new ArgumentNullException("Unexpected error to update GLUsers.", "");
                    }
                    #endregion SqlExecution
                    #region insert Details from Master into Detail Table
                    GLUserBranchDAL _formADAL = new GLUserBranchDAL();
                    if (vm.glUserBranchVMs != null && vm.glUserBranchVMs.Count > 0)
                    {
                        foreach (var detailVM in vm.glUserBranchVMs)
                        {
                            GLUserBranchVM dVM = new GLUserBranchVM();
                            dVM = detailVM;
                            dVM.GLUserId = vm.Id;


                            retResults = _formADAL.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                    }
                    #endregion insert Details from Master into Detail Table
                }
                else
                {
                    retResults[1] = "This GLUser already used!";
                    throw new ArgumentNullException("Please Input GLUser Value", "");
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
        public string[] Update(GLUserVM vm, HttpPostedFileBase PhotoFile, HttpPostedFileBase SignatureFile, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "GLUserUpdate"; //Method Name
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
                    #region Checkpoint
                    bool IsExist = false;
                    {
                        string[] cFields = { "Id!","LogId" };
                        string[] cValues = { vm.Id.ToString(), vm.LogId };
                        IsExist = _cDal.ExistCheck("GLUsers", cFields, cValues, currConn, transaction);
                        if (IsExist)
                        {
                            retResults[1] = "This Login Id " + vm.LogId + " Already Used!";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }

                    #endregion


                    #region SqlText
                    sqlText = "";
                    sqlText = "UPDATE GLUsers SET";


                    sqlText += " LogId=@LogId";
                    sqlText += " , Designation=@Designation";
                    sqlText += " , Email=@Email";
                    sqlText += " , IsAdmin=@IsAdmin";
                    sqlText += " , IsAuditor=@IsAuditor";
                    sqlText += " , IsExpense=@IsExpense";
                    sqlText += " , IsExpenseRequisition=@IsExpenseRequisition";
                    sqlText += " , IsBDERequisition=@IsBDERequisition";
                    sqlText += " , HaveExpenseApproval=@HaveExpenseApproval";
                    sqlText += " , HaveExpenseRequisitionApproval=@HaveExpenseRequisitionApproval";
                    sqlText += " , HaveBDERequisitionApproval=@HaveBDERequisitionApproval";
                    sqlText += " , HaveApprovalLevel1=@HaveApprovalLevel1";
                    sqlText += " , HaveApprovalLevel2=@HaveApprovalLevel2";
                    sqlText += " , HaveApprovalLevel3=@HaveApprovalLevel3";
                    sqlText += " , HaveApprovalLevel4=@HaveApprovalLevel4";

                    sqlText += " , FullName=@FullName";
                    sqlText += " , PFNo=@PFNo";
                    sqlText += " , Mobile=@Mobile";

                    if (PhotoFile != null && PhotoFile.ContentLength > 0)
                    {
                        sqlText += ", PhotoFileName=@PhotoFileName";
                    }


                    if (SignatureFile != null && SignatureFile.ContentLength > 0)
                    {
                        sqlText += ", SignatureFileName=@SignatureFileName";

                    }




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
                    cmdUpdate.Parameters.AddWithValue("@LogId", vm.LogId);
                    cmdUpdate.Parameters.AddWithValue("@Designation", vm.Designation.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsAdmin", vm.IsAdmin);
                    cmdUpdate.Parameters.AddWithValue("@IsAuditor", vm.IsAuditor);
                    cmdUpdate.Parameters.AddWithValue("@IsExpense", vm.IsExpense);
                    cmdUpdate.Parameters.AddWithValue("@IsExpenseRequisition", vm.IsExpenseRequisition);
                    cmdUpdate.Parameters.AddWithValue("@IsBDERequisition", vm.IsBDERequisition);
                    cmdUpdate.Parameters.AddWithValue("@HaveExpenseApproval", vm.HaveExpenseApproval);
                    cmdUpdate.Parameters.AddWithValue("@HaveExpenseRequisitionApproval", vm.HaveExpenseRequisitionApproval);
                    cmdUpdate.Parameters.AddWithValue("@HaveBDERequisitionApproval", vm.HaveBDERequisitionApproval);
                    cmdUpdate.Parameters.AddWithValue("@HaveApprovalLevel1", vm.HaveApprovalLevel1);
                    cmdUpdate.Parameters.AddWithValue("@HaveApprovalLevel2", vm.HaveApprovalLevel2);
                    cmdUpdate.Parameters.AddWithValue("@HaveApprovalLevel3", vm.HaveApprovalLevel3);
                    cmdUpdate.Parameters.AddWithValue("@HaveApprovalLevel4", vm.HaveApprovalLevel4);

                    cmdUpdate.Parameters.AddWithValue("@FullName", vm.FullName ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@PFNo", vm.PFNo ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Mobile", vm.Mobile ?? Convert.DBNull);

                    if (PhotoFile != null && PhotoFile.ContentLength > 0)
                    {
                        vm.PhotoFileName = vm.LogId + ".jpg";
                        var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Files/Sage/PhotoFile"), vm.PhotoFileName);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        PhotoFile.SaveAs(path);

                        //WebImage img = new WebImage(PhotoFile.InputStream);
                        //img.Resize(400, 300, false);
                        //img.Save(path);


                        cmdUpdate.Parameters.AddWithValue("@PhotoFileName", vm.PhotoFileName ?? Convert.DBNull);
                    }


                    if (SignatureFile != null && SignatureFile.ContentLength > 0)
                    {
                        vm.SignatureFileName = vm.LogId + ".jpg";
                        var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Files/Sage/SignatureFile"), vm.SignatureFileName);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        SignatureFile.SaveAs(path);
                        cmdUpdate.Parameters.AddWithValue("@SignatureFileName", vm.SignatureFileName ?? Convert.DBNull);
                    }



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
                        throw new ArgumentNullException("Unexpected error to update GLUsers.", "");
                    }
                    #endregion SqlExecution
                    #region insert Details from Master into Detail Table
                    GLUserBranchDAL _formADal = new GLUserBranchDAL();
                    if (vm.glUserBranchVMs != null && vm.glUserBranchVMs.Count > 0)
                    {
                        #region Delete Detail
                        try
                        {
                            retResults = _cDal.DeleteTableInformation(vm.Id.ToString(), "GLUserBranchs", "GLUserId", currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        catch (Exception)
                        {
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        #endregion Delete Detail
                        #region Insert Detail Again
                        foreach (var detailVM in vm.glUserBranchVMs)
                        {
                            GLUserBranchVM dVM = new GLUserBranchVM();
                            dVM = detailVM;
                            dVM.GLUserId = vm.Id;
                            retResults = _formADal.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                        }
                        #endregion Insert Detail Again
                    }
                    #endregion  insert Details from Master into Detail Table
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("GLUser Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLUser Update", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Data Update Successfully.";
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
        ////==================Delete =================
        public string[] Delete(GLUserVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteGLUser"; //Method Name
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
                        sqlText = "update GLUsers set";
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
                        throw new ArgumentNullException("GLUser Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLUser Information Delete", "Could not found any item.");
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

        public string[] ChangePassword(GLUserVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "GLUser Update"; //Method Name
            int transResult = 0;
            string sqlText = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("GLUserUpdate"); }
                #endregion open connection and transaction
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update GLUsers set";
                    sqlText += " Password=@Password,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " WHERE 1=1 AND LogId=@LogId";
                    if (!vm.IsAdmin)
                    {
                        sqlText += " And Password=@OldPassword";
                    }
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    vm.Password = Ordinary.Encrypt(vm.Password, true);
                    cmdUpdate.Parameters.AddWithValue("@Password", vm.Password);
                    if (!vm.IsAdmin)
                    {
                        vm.OldPassword = Ordinary.Encrypt(vm.OldPassword, true);
                        cmdUpdate.Parameters.AddWithValue("@OldPassword", vm.OldPassword);
                    }
                    cmdUpdate.Parameters.AddWithValue("@LogId", vm.LogId);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult == 0)
                    {
                        retResults[1] = "This employee is not a User Or Invalid old password!";
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("GLUser Update", "No GLUser found");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("User Update", "Could not found any User.");
                }
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "Password Successfully Updated.";
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


        public string[] ForgetPassword(GLUserVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "GLUser Update"; //Method Name
            int transResult = 0;
            string sqlText = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction(""); }
                #endregion open connection and transaction
                if (paramVM != null)
                {
                    #region Check Point
                    string[] conditionFields = { "u.LogId", "u.Email" };
                    string[] conditionValues = { paramVM.LogId, paramVM.Email };

                    GLUserVM vm = new GLUserVM();
                    vm = SelectAll(0, conditionFields, conditionValues, currConn, transaction).FirstOrDefault();
                    if (vm == null)
                    {
                        retResults[1] = "This LogId or Email Not Matched!";
                        return retResults;
                    }

                    //Match LogId and Email
                    //If Matched, Update the Password for the LogId
                    #endregion
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update GLUsers set";
                    sqlText += " Password=@Password";
                    sqlText += " WHERE 1=1 AND LogId=@LogId";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    //paramVM.Password = "123456";
                    paramVM.Password = Ordinary.Encrypt(paramVM.Password, true);
                    cmdUpdate.Parameters.AddWithValue("@Password", paramVM.Password);
                    cmdUpdate.Parameters.AddWithValue("@LogId", paramVM.LogId);
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult == 0)
                    {
                        retResults[1] = "This LogId or Email Not Matched!";
                        throw new ArgumentNullException(retResults[1], "");
                    }
                    retResults[2] = paramVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("GLUser Update", "No GLUser found");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("User Update", "Could not found any User.");
                }
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "Password Successfully Updated.";
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


        #endregion
    }
}
