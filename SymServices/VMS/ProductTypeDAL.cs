using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymOrdinary;
using SymServices.Common;
using SymViewModel.VMS;


namespace SymServices.VMS
{
    public class ProductTypeDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods

        //==================DropDownAll=================
        public List<ProductTypeViewModel> DropDownAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<ProductTypeViewModel> VMs = new List<ProductTypeViewModel>();
            ProductTypeViewModel vm;
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
'B' Sl, TypeID
, ProductType
FROM ProductTypes
WHERE ActiveStatus='Y'
UNION 
SELECT 
'A' Sl, '-1' Id
, 'ALL Product' ProductType  
FROM ProductTypes
)
AS a
order by Sl,ProductType
";



                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ProductTypeViewModel();
                    vm.TypeID = dr["TypeID"].ToString(); ;
                    vm.ProductType = dr["ProductType"].ToString();
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
        public List<ProductTypeViewModel> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<ProductTypeViewModel> VMs = new List<ProductTypeViewModel>();
            ProductTypeViewModel vm;
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
af.TypeID
,af.ProductType
FROM ProductTypes af
WHERE  1=1 AND af.ActiveStatus = 'Y'
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ProductTypeViewModel();
                    vm.TypeID = dr["TypeID"].ToString();
                    vm.ProductType = dr["ProductType"].ToString();
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
        public List<ProductTypeViewModel> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ProductTypeViewModel> VMs = new List<ProductTypeViewModel>();
            ProductTypeViewModel vm;
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
 TypeID
,ProductType
,Comments
,Description
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,Info2
,Info3
,Info4
,Info5
,TypeID1
,TypeID12
,TypeID123
,BOMId

FROM ProductTypes  
WHERE  1=1 AND ActiveStatus = 'Y'

";


                if (Id > 0)
                {
                    sqlText += @" and TypeID=@TypeID";
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
                    objComm.Parameters.AddWithValue("@TypeID", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ProductTypeViewModel();
                    vm.TypeID = dr["TypeID"].ToString();
                    vm.ProductType = dr["ProductType"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.Description = dr["Description"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString() == "Y" ? true : false;
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = DateTime.Parse(dr["CreatedOn"].ToString());
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = DateTime.Parse(dr["LastModifiedOn"].ToString());
                    vm.Info2 = dr["Info2"].ToString();
                    vm.Info3 = dr["Info3"].ToString();
                    vm.Info4 = dr["Info4"].ToString();
                    vm.Info5 = dr["TypeID"].ToString();
                    vm.TypeID1 = dr["TypeID1"].ToString();
                    vm.TypeID12 = dr["TypeID12"].ToString();
                    vm.TypeID123 = dr["TypeID123"].ToString();
                    vm.BOMId = dr["BOMId"].ToString();
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
        public string[] Insert(ProductTypeViewModel vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertProductType"; //Method Name
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
                #region Save
                vm.TypeID = _cDal.NextIdWithType("ProductTypes", currConn, transaction).ToString();
                //vm.TypeID = "14";

                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO ProductTypes(
 TypeID
,ProductType
,Comments
,Description
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,Info2
,Info3
,Info4
,Info5
,TypeID1
,TypeID12
,TypeID123
,BOMId

) VALUES (
 @TypeID
,@ProductType
,@Comments
,@Description
,@ActiveStatus
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn
,@Info2
,@Info3
,@Info4
,@Info5
,@TypeID1
,@TypeID12
,@TypeID123
,@BOMId              
) 
";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                    cmdInsert.Parameters.AddWithValue("@TypeID", vm.TypeID);
                    cmdInsert.Parameters.AddWithValue("@ProductType", vm.ProductType);
                    cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Description", vm.Description ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus ? "Y" : "N");
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                    cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                    cmdInsert.Parameters.AddWithValue("@Info2", vm.Info2 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Info3", vm.Info3 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Info4", vm.Info4 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Info5", vm.Info5 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@TypeID1", vm.TypeID1 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@TypeID12", vm.TypeID12 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@TypeID123", vm.TypeID123 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@BOMId", vm.BOMId ?? Convert.DBNull);                

                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update ProductType.", "");
                    }
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This ProductType already used!";
                    throw new ArgumentNullException("Please Input ProductType Value", "");
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
                retResults[2] = vm.TypeID.ToString();
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
        public string[] Update(ProductTypeViewModel vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "ProductTypeUpdate"; //Method Name
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
                    currConn = _dbsqlConnection.GetConnection();
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
                    sqlText = "UPDATE ProductTypes SET ";

                    sqlText += " ProductType=@ProductType";
                    sqlText += " ,Comments=@Comments";
                    sqlText += " ,Description=@Description";
                    sqlText += " ,LastModifiedBy=@LastModifiedBy";
                    sqlText += " ,LastModifiedOn=@LastModifiedOn";
                    sqlText += " ,Info2=@Info2";
                    sqlText += " ,Info3=@Info3";
                    sqlText += " ,Info4=@Info4";
                    sqlText += " ,Info5=@Info5";
                    sqlText += " ,TypeID1=@TypeID1";
                    sqlText += " ,TypeID12=@TypeID12";
                    sqlText += " ,TypeID123=@TypeID123";
                    sqlText += " ,BOMId=@BOMId";

                    sqlText += " WHERE TypeID=@TypeID";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);

                    cmdUpdate.Parameters.AddWithValue("@TypeID", vm.TypeID);
                    cmdUpdate.Parameters.AddWithValue("@ProductType", vm.ProductType);
                    cmdUpdate.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Description", vm.Description ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                    cmdUpdate.Parameters.AddWithValue("@Info2", vm.Info2 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Info3", vm.Info3 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Info4", vm.Info4 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Info5", vm.Info5 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@TypeID1", vm.TypeID1 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@TypeID12", vm.TypeID12 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@TypeID123", vm.TypeID123 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@BOMId", vm.BOMId ?? Convert.DBNull);    

 
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update ProductType.", "");
                    }
                    #endregion SqlExecution

                    retResults[2] = vm.TypeID.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("ProductType Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("ProductType Update", "Could not found any item.");
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
                retResults[2] = vm.TypeID.ToString();
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
        public string[] Delete(ProductTypeViewModel vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteProductType"; //Method Name
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
                        sqlText = "update ProductTypes set";
                        sqlText += " ActiveStatus=@ActiveStatus";
                        sqlText += " ,LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " where TypeID=@TypeID";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@TypeID", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@ActiveStatus", "N");
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
                        throw new ArgumentNullException("ProductType Delete", vm.TypeID + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("ProductType Information Delete", "Could not found any item.");
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
