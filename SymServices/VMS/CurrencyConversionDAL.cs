using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SymOrdinary;
using SymViewModel.VMS;
using System.Reflection;


namespace SymServices.VMS
{
    public class CurrencyConversionDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        //==================SelectAll=================
        public List<CurrencyConversionVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<CurrencyConversionVM> VMs = new List<CurrencyConversionVM>();
            CurrencyConversionVM vm;
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
 cc.CurrencyConversionId
,cc.CurrencyCodeFrom
,cc.CurrencyCodeTo
,c.CurrencyName CurrencyNameFrom
,b.CurrencyName CurrencyNameTo
,cc.CurrencyRate
,cc.Comments
,cc.ActiveStatus
,cc.CreatedBy
,cc.CreatedOn
,cc.LastModifiedBy
,cc.LastModifiedOn
,cc.ConversionDate

FROM CurrencyConversion cc left outer join Currencies c on cc.CurrencyCodeFrom=c.CurrencyId
left outer join Currencies b on cc.CurrencyCodeTo=b.CurrencyId
WHERE  1=1 and cc.IsArchive=0
";



                if (Id > 0)
                {
                    sqlText += @" and cc.CurrencyConversionId=@CurrencyConversionId";
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
                    objComm.Parameters.AddWithValue("@CurrencyConversionId", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CurrencyConversionVM();
                    vm.CurrencyConversionId = dr["CurrencyConversionId"].ToString();
                    vm.CurrencyCodeFrom = dr["CurrencyCodeFrom"].ToString();
                    vm.CurrencyCodeTo = dr["CurrencyCodeTo"].ToString();
                    vm.CurrencyNameFrom = dr["CurrencyNameFrom"].ToString();
                    vm.CurrencyNameTo = dr["CurrencyNameTo"].ToString();
                    vm.CurrencyRate = Convert.ToDecimal(dr["CurrencyRate"]);
                    vm.Comments = dr["Comments"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.ConvertionDate = Ordinary.DateToDate(dr["ConversionDate"].ToString());

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

        //public string[] InsertToCurrencyConversion(string CurrencyConversionId, string CurrencyCodeFrom, string CurrencyCodeTo, string CurrencyRate,string ConvertionDate,string Comments, string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, string databaseName)
        public string[] InsertToCurrencyConversion(CurrencyConversionVM vm)
        {


            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[1] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            int nextId = 0;

            try
            {
                #region Validation

                //if (string.IsNullOrEmpty(CurrencyConversionId))
                //{
                //    throw new ArgumentNullException("InsertToCurrencyConversionInformation",
                //                                    "Please enter Currency ConversionID.");
                //}


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("CurrencyConversion", "ConversionDate", "datetime", currConn);

                transaction = currConn.BeginTransaction("InsertToConversionInfo");


                #region Add BOMId
                sqlText = "";
                sqlText += "update CurrencyConversion set ConversionDate='1900/01/01' where ConversionDate is null ";

                SqlCommand cmdBOMId1 = new SqlCommand(sqlText, currConn);
                cmdBOMId1.Transaction = transaction;
                cmdBOMId1.ExecuteScalar();

                #endregion Add BOMId

                #endregion open connection and transaction

                #region duplicate check
                string[] cFields={"CurrencyCodeFrom","CurrencyCodeTo"};
                string[] cValues = { vm.CurrencyCodeFrom, vm.CurrencyCodeTo};
                var currConversion = SelectAll(0, cFields, cValues, currConn, transaction);
                if (currConversion.Count > 0)
                {
                    retResults[1] = "Same conversion already exists";
                    throw new ArgumentNullException("", retResults[1]);
                }

                #endregion
                if (!string.IsNullOrEmpty(vm.CurrencyConversionId))
                {

                    sqlText = "select count(CurrencyConversionId) from CurrencyConversion where  CurrencyConversionId=@CurrencyConversionId";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@CurrencyConversionId", vm.CurrencyConversionId);
                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId > 0)
                    {
                        throw new ArgumentNullException("InsertToCurrencyConversionInformation", "Currency Conversion information already exist");
                    }

                }

                #region Insert Currency Information

                //sqlText = "select count(distinct CurrencyConversionId) from CurrencyConversion where  CurrencyConversionId='" + CurrencyConversionId + "'";
                //SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                //cmdNameExist.Transaction = transaction;
                //int countName = (int)cmdNameExist.ExecuteScalar();
                //if (countName > 0)
                //{

                //    throw new ArgumentNullException("InsertToCurrencyConversionInformation",
                //                                    "Requested Currency Conversion  is already exist");
                //}

                sqlText = "select isnull(max(cast(CurrencyConversionId as int)),0)+1 FROM  CurrencyConversion";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                nextId = Convert.ToInt32(cmdNextId.ExecuteScalar());
                if (nextId <= 0)
                {
                    //THROGUH EXCEPTION BECAUSE WE COULD NOT FIND EXPECTED NEW ID
                    retResults[0] = "Fail";
                    retResults[1] = "Unable to create new Currency Conversion information Id";
                    throw new ArgumentNullException("InsertToCurrencyConversionInformation",
                                                    "Unable to create new Currency Conversion information Id");
                }
                vm.CurrencyConversionId = nextId.ToString();
                sqlText = "";

                sqlText += @" 
INSERT INTO CurrencyConversion(
 CurrencyConversionId
,CurrencyCodeFrom
,CurrencyCodeTo
,CurrencyRate
,Comments
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,ConversionDate
) VALUES (
 @CurrencyConversionId
,@CurrencyCodeFrom
,@CurrencyCodeTo
,@CurrencyRate
,@Comments
,@ActiveStatus
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn
,@ConversionDate        
) 
";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@CurrencyConversionId", vm.CurrencyConversionId);
                cmdInsert.Parameters.AddWithValue("@CurrencyCodeFrom", vm.CurrencyCodeFrom ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CurrencyCodeTo", vm.CurrencyCodeTo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CurrencyRate", vm.CurrencyRate);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@ConversionDate", Ordinary.DateToDate(vm.ConvertionDate));

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #endregion Insert Currency Information


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
                retResults[1] = "Requested Currency Information successfully added";
                retResults[2] = "" + nextId;

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

        //public string[] UpdateCurrencyConversion(string CurrencyConversionId, string CurrencyCodeFrom, string CurrencyCodeTo, string CurrencyRate,string ConvertionDate, string Comments, string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, string databaseName)
        public string[] UpdateCurrencyConversion(CurrencyConversionVM vm)
        {

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[1] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
                int nextId = 0;

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.CurrencyConversionId))
                {
                    throw new ArgumentNullException("InsertConvertInfo",
                                                    "Please enter Currency ConversionID.");
                }


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("CurrencyConversion", "ConversionDate", "datetime", currConn);

                transaction = currConn.BeginTransaction("InsertConvertInfo");


                #region Add BOMId
                sqlText = "";
                sqlText += "update CurrencyConversion set ConversionDate='1900/01/01' where ConversionDate is null ";


                SqlCommand cmdBOMId1 = new SqlCommand(sqlText, currConn);
                cmdBOMId1.Transaction = transaction;
                cmdBOMId1.ExecuteScalar();




                #endregion Add BOMId
                #endregion open connection and transaction


                #region Update Currency Information

                sqlText = "select isnull(max(cast(CurrencyConversionId as int)),0)+1 FROM  CurrencyConversion";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                nextId = Convert.ToInt32(cmdNextId.ExecuteScalar());
                if (nextId <= 0)
                {
                    //THROGUH EXCEPTION BECAUSE WE COULD NOT FIND EXPECTED NEW ID
                    retResults[0] = "Fail";
                    retResults[1] = "Unable to create new Currency Conversion information Id";
                    throw new ArgumentNullException("InsertConvertInfo",
                                                    "Unable to create new Currency Conversion information Id");
                }

                sqlText = "";
                sqlText += "UPDATE CurrencyConversion SET";

                sqlText += "  CurrencyCodeFrom            =@CurrencyCodeFrom";
                sqlText += " ,CurrencyCodeTo              =@CurrencyCodeTo";
                sqlText += " ,CurrencyRate                =@CurrencyRate";
                sqlText += " ,Comments                    =@Comments";
                sqlText += " ,ActiveStatus                =@ActiveStatus";
                sqlText += " ,LastModifiedBy              =@LastModifiedBy";
                sqlText += " ,LastModifiedOn              =@LastModifiedOn";
                sqlText += " where CurrencyConversionId   =@CurrencyConversionId";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValue("@CurrencyCodeFrom", vm.CurrencyCodeFrom);
                cmdInsert.Parameters.AddWithValue("@CurrencyCodeTo", vm.CurrencyCodeTo);
                cmdInsert.Parameters.AddWithValue("@CurrencyRate", vm.CurrencyRate);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@CurrencyConversionId", vm.CurrencyConversionId ?? Convert.DBNull);

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #endregion Insert Currency Information


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
                retResults[1] = "Requested Currency Conversion Information successfully Updated";
                //retResults[2] = "" + nextId;

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

        public string[] DeleteCurrencyConversionInformation(string CurrencyConversionId, string databaseName)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = CurrencyConversionId;

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(CurrencyConversionId))
                {
                    throw new ArgumentNullException("InsertToCurrencyConversionInformation",
                                "Could not find requested Currency Conversion Code.");
                }
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("CurrencyConversion", "ConversionDate", "datetime", currConn);
                #endregion open connection and transaction

                sqlText = "select count(CurrencyConversionId) from CurrencyConversion where CurrencyConversionId=@CurrencyConversionId";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValue("@CurrencyConversionId", CurrencyConversionId);
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested Currency Conversion Information.";
                    return retResults;
                }

                sqlText = "delete CurrencyConversion where CurrencyConversionId=@CurrencyConversionId";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                cmdDelete.Parameters.AddWithValue("@CurrencyConversionId", CurrencyConversionId);
                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested Currency Conversion Information successfully deleted";
                }


            }
            #region Cathc

            catch (SqlException sqlex)
            {

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

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



        public DataTable SearchCurrencyConversion(string CurrencyCodeFrom, string CurrencyCodeF, string CurrencyNameF,
        string CurrencyCodeTo, string CurrencyCodeT, string CurrencyNameT, string CurrencyRate, string ConversionDate, string ActiveStatus)
        {
            #region Objects & Variables


            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("currencyConversion");
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
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("CurrencyConversion", "ConversionDate", "datetime", currConn);
                #endregion open connection and transaction
                #region sql statement

                sqlText = "";
                sqlText += "  SELECT cc.CurrencyConversionId,";
                sqlText += "  cc.CurrencyCodeFrom, cF.CurrencyCode CurrencyCodeF,cf.CurrencyName CurrencyNameF,";
                sqlText += "  cc.CurrencyCodeTo, ct.CurrencyCode CurrencyCodeT,ct.CurrencyName CurrencyNameT,";
                sqlText += "  cc.CurrencyRate," +
                           "convert (varchar,isnull(cc.ConversionDate,'01/01/1900'),120)ConversionDate," +
                           "cc.Comments,cc.ActiveStatus";
                sqlText += "  FROM CurrencyConversion cc LEFT OUTER JOIN";
                sqlText += "  (SELECT c.CurrencyCode,c.CurrencyName,c.CurrencyId";
                sqlText += "  FROM Currencies c WHERE c.CurrencyId IN(";
                sqlText += "  SELECT MIN(c.CurrencyId)CurrencyId FROM Currencies c ";
                sqlText += "  GROUP BY c.CurrencyCode,c.CurrencyName";
                sqlText += "  )) AS cF ON cc.CurrencyCodeFrom=cF.CurrencyId LEFT OUTER JOIN";
                sqlText += "  (SELECT c.CurrencyCode,c.CurrencyName,c.CurrencyId FROM Currencies c WHERE c.CurrencyId IN(";
                sqlText += "  SELECT MIN(c.CurrencyId)CurrencyId FROM Currencies c ";
                sqlText += "  GROUP BY c.CurrencyCode,c.CurrencyName";
                sqlText += "  )) AS cT ON cc.CurrencyCodeTo=cT.CurrencyId";
                sqlText += "  WHERE 	(cc.CurrencyCodeFrom  LIKE '%' +  '" + CurrencyCodeFrom + "' + '%' OR cc.CurrencyCodeFrom IS NULL) ";
                sqlText += "  and (cF.CurrencyCode LIKE '%' + '" + CurrencyCodeF + "' + '%' OR cF.CurrencyCode IS NULL)	";
                sqlText += "  and (cF.CurrencyName LIKE '%' + '" + CurrencyNameF + "' + '%' OR cF.CurrencyName IS NULL)";
                sqlText += "  and (cc.CurrencyCodeTo LIKE '%' + '" + CurrencyCodeTo + "' + '%' OR cc.CurrencyCodeTo IS NULL)";
                sqlText += "  and (cT.CurrencyCode LIKE '%' + '" + CurrencyCodeT + "' + '%' OR cT.CurrencyCode IS NULL)";
                sqlText += "  and (cT.CurrencyName LIKE '%' + '" + CurrencyNameT + "' + '%' OR cT.CurrencyName IS NULL)";
                sqlText += "  and (cc.CurrencyRate LIKE '%' + '" + CurrencyRate + "' + '%' OR cc.CurrencyRate IS NULL)";
                //sqlText += "  and (cc.ConversionDate LIKE '%' + '" + ConversionDate + "' + '%' OR cc.ConversionDate IS NULL)";
                sqlText += "  and (cc.ActiveStatus LIKE '%' + '" + ActiveStatus + "' + '%' OR cc.ActiveStatus IS NULL)	";

                sqlText += "  order by cF.CurrencyCode,cT.CurrencyCode";

                SqlCommand objCommProductType = new SqlCommand();
                objCommProductType.Connection = currConn;
                objCommProductType.CommandText = sqlText;
                objCommProductType.CommandType = CommandType.Text;

                //if (!objCommProductType.Parameters.Contains("@CurrencyCodeFrom"))
                //{ objCommProductType.Parameters.AddWithValue("@CurrencyCodeFrom", CurrencyCodeFrom); }
                //else { objCommProductType.Parameters["@CurrencyCodeFrom"].Value = CurrencyCodeFrom; }

                //if (!objCommProductType.Parameters.Contains("@CurrencyCodeF"))
                //{ objCommProductType.Parameters.AddWithValue("@CurrencyCodeF", CurrencyCodeF); }
                //else { objCommProductType.Parameters["@CurrencyCodeF"].Value = CurrencyCodeF; }
                //if (!objCommProductType.Parameters.Contains("@CurrencyNameF"))
                //{ objCommProductType.Parameters.AddWithValue("@CurrencyNameF", CurrencyNameF); }
                //else { objCommProductType.Parameters["@CurrencyNameF"].Value = CurrencyNameF; }

                //if (!objCommProductType.Parameters.Contains("@CurrencyCodeTo"))
                //{ objCommProductType.Parameters.AddWithValue("@CurrencyCodeTo", CurrencyCodeTo); }
                //else { objCommProductType.Parameters["@CurrencyCodeTo"].Value = CurrencyCodeTo; }
                //if (!objCommProductType.Parameters.Contains("@CurrencyCodeT"))
                //{ objCommProductType.Parameters.AddWithValue("@CurrencyCodeT", CurrencyCodeT); }
                //else { objCommProductType.Parameters["@CurrencyCodeT"].Value = CurrencyCodeT; }
                //if (!objCommProductType.Parameters.Contains("@CurrencyNameT"))
                //{ objCommProductType.Parameters.AddWithValue("@CurrencyNameT", CurrencyNameT); }
                //else { objCommProductType.Parameters["@CurrencyNameT"].Value = CurrencyNameT; }

                //if (!objCommProductType.Parameters.Contains("@ActiveStatus"))
                //{ objCommProductType.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                //else { objCommProductType.Parameters["@ActiveStatus"].Value = ActiveStatus; }



                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProductType);
                dataAdapter.Fill(dataTable);
                #endregion
            }
            #endregion
            #region catch


            catch (SqlException sqlex)
            {

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

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

        public DataTable SearchCurrencyConversionNew(string CurrencyCodeF, string CurrencyNameF
            , string CurrencyCodeT, string CurrencyNameT, string ActiveStatus, string convDate)
        {
            #region Objects & Variables


            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("currencyConversion");
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
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("CurrencyConversion", "ConversionDate", "datetime", currConn);
                #endregion open connection and transaction
                #region sql statement

                sqlText = "";

                sqlText +=
                    "  SELECT b.CurrencyConversionId, a.CurrencyCodeFrom,cf.CurrencyCode CurrencyCodeF,cf.CurrencyName CurrencyNameF";
                sqlText +=
                    "  , b.CurrencyCodeTo,ct.CurrencyCode CurrencyCodeT,ct.CurrencyName CurrencyNameT, b.CurrencyRate,convert (varchar,isnull(a.ConversionDate,'01/01/1900'),120)ConversionDate ";
                sqlText += "  ,b.Comments,b.ActiveStatus";
                sqlText += "  FROM";
                sqlText += "  (SELECT DISTINCT cc.CurrencyCodeFrom,CurrencyCodeTo,MAX(cc.ConversionDate)ConversionDate";
                sqlText += "  from CurrencyConversion cc ";
                sqlText += "  where ConversionDate<='" + convDate + "'";
                sqlText += "  GROUP BY cc.CurrencyCodeFrom,CurrencyCodeTo) AS A,";
                sqlText += "  (SELECT * FROM CurrencyConversion cc) AS b,";
                sqlText += "  (	SELECT * FROM Currencies) AS CF,";
                sqlText += "  (	SELECT * FROM Currencies) AS CT";

                sqlText +=
                    " WHERE a.CurrencyCodeFrom=b.CurrencyCodeFrom AND a.ConversionDate=b.ConversionDate " +
                    "AND a.CurrencyCodeTo=b.CurrencyCodeTo";
                sqlText += "  AND a.CurrencyCodeFrom=cF.CurrencyId AND a.CurrencyCodeTo=ct.CurrencyId";
                sqlText += "  and (cF.CurrencyCode LIKE '%' + '" + CurrencyCodeF + "' + '%' OR cF.CurrencyCode IS NULL)	";
                sqlText += "  and (cF.CurrencyName LIKE '%' + '" + CurrencyNameF + "' + '%' OR cF.CurrencyName IS NULL)";
                sqlText += "  and (cT.CurrencyCode LIKE '%' + '" + CurrencyCodeT + "' + '%' OR cT.CurrencyCode IS NULL)";
                sqlText += "  and (cT.CurrencyName LIKE '%' + '" + CurrencyNameT + "' + '%' OR cT.CurrencyName IS NULL)";
                sqlText += "  and (b.ActiveStatus LIKE '%' + '" + ActiveStatus + "' + '%' OR b.ActiveStatus IS NULL)	";

                sqlText += "  order by cF.CurrencyCode,cT.CurrencyCode";

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
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

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

        public string[] Delete(CurrencyConversionVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteCurrency"; //Method Name
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
                        sqlText = "update CurrencyConversion set";
                        sqlText += " ActiveStatus=@ActiveStatus";
                        sqlText += " ,LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where CurrencyConversionId=@CurrencyConversionId";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@CurrencyConversionId", Convert.ToInt32(ids[i]));
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
                        throw new ArgumentNullException("Currency Delete", vm.CurrencyConversionId + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Currency Information Delete", "Could not found any item.");
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




    }
}
