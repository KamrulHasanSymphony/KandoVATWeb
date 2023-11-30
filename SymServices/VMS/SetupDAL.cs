using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using SymphonySofttech.Utilities;
using SymViewModel.VMS;

namespace SymServices.VMS
{
    public class SetupDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;

        #endregion

        #region New Methods

       
        public string[] InsertToSetupNew(SetupMaster setupMaster)
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
                if (setupMaster != null)
                {
                    #region Validation

                    if (string.IsNullOrEmpty(setupMaster.PurchaseP))
                    {
                        throw new ArgumentNullException("InsertToSetupNew", "Please enter purchase.");
                    }
                    if (string.IsNullOrEmpty(setupMaster.SaleP))
                    {
                        throw new ArgumentNullException("InsertToSetupNew", "Please enter sale.");
                    }

                    #endregion Validation

                    #region open connection and transaction

                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                    transaction = currConn.BeginTransaction("InsertToSetupNew");

                    #endregion open connection and transaction

                    #region Setup existence checking by id

                    sqlText = "select count(PurchaseP) from Setup";
                    SqlCommand setupExist = new SqlCommand(sqlText, currConn);
                    setupExist.Transaction = transaction;
                    countId = (int)setupExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("InsertToSetupNew", "Could not find requested setup.");
                    }

                    #endregion Setup existence checking by id

                    #region Update Setup

                    sqlText = "";
                    sqlText = "update Setup set";
                    sqlText += " PurchaseP          =@setupMasterPurchaseP,";
                    sqlText += " PurchaseIDL        =@setupMasterPurchaseIDL,";
                    sqlText += " PurchaseNYID       =@setupMasterPurchaseNYID,";
                    sqlText += " PurchaseTradingP   =@setupMasterPurchaseTradingP,";
                    sqlText += " PurchaseTradingIDL =@setupMasterPurchaseTradingIDL,";
                    sqlText += " PurchaseTradingNYID=@setupMasterPurchaseTradingNYID,";
                    sqlText += " IssueP             =@setupMasterIssueP,";
                    sqlText += " IssueIDL           =@setupMasterIssueIDL,";
                    sqlText += " IssueNYID          =@setupMasterIssueNYID,";
                    sqlText += " IssueReturnP       =@setupMasterIssueReturnP,";
                    sqlText += " IssueReturnIDL     =@setupMasterIssueReturnIDL,";
                    sqlText += " IssueReturnNYID    =@setupMasterIssueReturnNYID,";
                    sqlText += " ReceiveP           =@setupMasterReceiveP,";
                    sqlText += " ReceiveIDL         =@setupMasterReceiveIDL,";
                    sqlText += " ReceiveNYID        =@setupMasterReceiveNYID,";
                    sqlText += " TransferP          =@setupMasterTransferP,";
                    sqlText += " TransferIDL        =@setupMasterTransferIDL,";
                    sqlText += " TransferNYID       =@setupMasterTransferNYID,";

                    sqlText += " SaleP              =@setupMasterSaleP,";
                    sqlText += " SaleIDL            =@setupMasterSaleIDL,";
                    sqlText += " SaleNYID           =@setupMasterSaleNYID,";
                    sqlText += " SaleServiceP       =@setupMasterSaleServiceP,";
                    sqlText += " SaleServiceIDL     =@setupMasterSaleServiceIDL,";
                    sqlText += " SaleServiceNYID    =@setupMasterSaleServiceNYID,";

                    sqlText += " SaleExportP        =@setupMasterSaleExportP,";
                    sqlText += " SaleExportIDL      =@setupMasterSaleExportIDL,";
                    sqlText += " SaleExportNYID     =@setupMasterSaleExportNYID,";
                    sqlText += " SaleTradingP       =@setupMasterSaleTradingP,";
                    sqlText += " SaleTradingIDL     =@setupMasterSaleTradingIDL,";
                    sqlText += " SaleTradingNYID    =@setupMasterSaleTradingNYID,";

                    sqlText += " SaleTenderP        =@setupMasterSaleTenderP,";
                    sqlText += " SaleTenderIDL      =@setupMasterSaleTenderIDL,";
                    sqlText += " SaleTenderNYID     =@setupMasterSaleTenderNYID,";
                    sqlText += " DNP                =@setupMasterDNP,";
                    sqlText += " DNIDL              =@setupMasterDNIDL,";
                    sqlText += " DNNYID             =@setupMasterDNNYID,";

                    sqlText += " CNP                =@setupMasterCNP,";
                    sqlText += " CNIDL              =@setupMasterCNIDL,";
                    sqlText += " CNNYID             =@setupMasterCNNYID,";
                    sqlText += " DepositP           =@setupMasterDepositP,";
                    sqlText += " DepositIDL         =@setupMasterDepositIDL,";
                    sqlText += " DepositNYID        =@setupMasterDepositNYID,";

                    sqlText += " VDSP               =@setupMasterVDSP,";
                    sqlText += " VDSIDL             =@setupMasterVDSIDL,";
                    sqlText += " VDSNYID            =@setupMasterVDSNYID,";
                    sqlText += " TollIssueP         =@setupMasterTollIssueP,";
                    sqlText += " TollIssueIDL       =@setupMasterTollIssueIDL,";
                    sqlText += " TollIssueNYID      =@setupMasterTollIssueNYID,";

                    sqlText += " TollReceiveP       =@setupMasterTollReceiveP,";
                    sqlText += " TollReceiveIDL     =@setupMasterTollReceiveIDL,";
                    sqlText += " TollReceiveNYID    =@setupMasterTollReceiveNYID,";
                    sqlText += " DSFP               =@setupMasterDSFP,";
                    sqlText += " DSFIDL             =@setupMasterDSFIDL,";
                    sqlText += " DSFNYID            =@setupMasterDSFNYID,";

                    sqlText += " DSRP               =@setupMasterDSRP,";
                    sqlText += " DSRIDL             =@setupMasterDSRIDL,";
                    sqlText += " DSRNYID            =@setupMasterDSRNYID,";
                    sqlText += " IssueFromBOM       =@setupMasterIssueFromBOM,";
                    sqlText += " PrepaidVAT         =@setupMasterPrepaidVAT,";
                    sqlText += " CYear              =@setupMasterCYear";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Transaction = transaction;
                    cmdUpdate.Parameters.AddWithValue("@setupMasterPurchaseP", setupMaster.PurchaseP ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterPurchaseIDL",setupMaster.PurchaseIDL);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterPurchaseNYID", setupMaster.PurchaseNYID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterPurchaseTradingP", setupMaster.PurchaseTradingP ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterPurchaseTradingIDL",setupMaster.PurchaseTradingIDL);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterPurchaseTradingNYID", setupMaster.PurchaseTradingNYID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterIssueP", setupMaster.IssueP ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterIssueIDL",setupMaster.IssueIDL);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterIssueNYID", setupMaster.IssueNYID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterIssueReturnP", setupMaster.IssueReturnP ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterIssueReturnIDL",setupMaster.IssueReturnIDL);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterIssueReturnNYID", setupMaster.IssueReturnNYID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterReceiveP", setupMaster.ReceiveP ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterReceiveIDL",setupMaster.ReceiveIDL);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterReceiveNYID", setupMaster.ReceiveNYID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterTransferP", setupMaster.TransferP ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterTransferIDL",setupMaster.TransferIDL);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterTransferNYID", setupMaster.TransferNYID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterSaleP", setupMaster.SaleP ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterSaleIDL",setupMaster.SaleIDL);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterSaleNYID", setupMaster.SaleNYID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterSaleServiceP", setupMaster.SaleServiceP ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterSaleServiceIDL",setupMaster.SaleServiceIDL);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterSaleServiceNYID", setupMaster.SaleServiceNYID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterSaleExportP", setupMaster.SaleExportP ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterSaleExportIDL",setupMaster.SaleExportIDL);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterSaleExportNYID", setupMaster.SaleExportNYID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterSaleTradingP", setupMaster.SaleTradingP ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterSaleTradingIDL",setupMaster.SaleTradingIDL);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterSaleTradingNYID", setupMaster.SaleTradingNYID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterSaleTenderP", setupMaster.SaleTenderP ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterSaleTenderIDL",setupMaster.SaleTenderIDL);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterSaleTenderNYID", setupMaster.SaleTenderNYID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterDNP", setupMaster.DNP ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterDNIDL",setupMaster.DNIDL);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterDNNYID", setupMaster.DNNYID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterCNIDL",setupMaster.CNIDL);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterCNNYID", setupMaster.CNNYID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterDepositP", setupMaster.DepositP ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterDepositIDL",setupMaster.DepositIDL);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterDepositNYID", setupMaster.DepositNYID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterVDSP", setupMaster.VDSP ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterVDSIDL",setupMaster.VDSIDL);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterVDSNYID", setupMaster.VDSNYID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterTollIssueP", setupMaster.TollIssueP ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterTollIssueIDL",setupMaster.TollIssueIDL);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterTollIssueNYID", setupMaster.TollIssueNYID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterTollReceiveP", setupMaster.TollReceiveP ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterTollReceiveIDL",setupMaster.TollReceiveIDL);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterTollReceiveNYID", setupMaster.TollReceiveNYID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterDSFP", setupMaster.DSFP ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterDSFIDL",setupMaster.DSFIDL);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterDSFNYID", setupMaster.DSFNYID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterDSRP", setupMaster.DSRP ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterDSRIDL",setupMaster.DSRIDL);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterDSRNYID", setupMaster.DSRNYID ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterIssueFromBOM", setupMaster.IssueFromBOM ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterPrepaidVAT", setupMaster.PrepaidVAT ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@setupMasterCYear", setupMaster.CYear ?? Convert.DBNull);

                    transResult = (int)cmdUpdate.ExecuteNonQuery();

                    #region Commit

                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                            retResults[0] = "Success";
                            retResults[1] = "Requested Setup Information Successfully Updated.";
                            retResults[2] = "" + 1;

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected error to update setup.";
                            retResults[2] = "" + 0;
                        }

                    }
                    else
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update setup information.";
                        retResults[2] = "" + 0;
                    }

                    #endregion Commit

                    #endregion Update Setup
                }
                else
                {
                    throw new ArgumentNullException("SetupMaster", "Setup is null.");
                }

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

        public string SearchSetupNew(string databaseName)
        {

            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            string sqlReturn = string.Empty;

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

                sqlText = @"SELECT 
                            PurchaseP
                            ,PurchaseIDL
                            ,PurchaseNYID
                            ,PurchaseTradingP
                            ,PurchaseTradingIDL
                            ,PurchaseTradingNYID
                            ,IssueP
                            ,IssueIDL
                            ,IssueNYID
                            ,IssueReturnP
                            ,IssueReturnIDL
                            ,IssueReturnNYID
                            ,ReceiveP
                            ,ReceiveIDL
                            ,ReceiveNYID
                            ,TransferP
                            ,TransferIDL
                            ,TransferNYID
                            ,SaleP
                            ,SaleIDL
                            ,SaleNYID
                            ,SaleServiceP
                            ,SaleServiceIDL
                            ,SaleServiceNYID
                            ,SaleTradingP
                            ,SaleTradingIDL
                            ,SaleTradingNYID
                            ,SaleExportP
                            ,SaleExportIDL
                            ,SaleExportNYID
                            ,SaleTenderP
                            ,SaleTenderIDL
                            ,SaleTenderNYID
                            ,DNP
                            ,DNIDL
                            ,DNNYID
                            ,CNP
                            ,CNIDL
                            ,CNNYID
                            ,DepositP
                            ,DepositIDL
                            ,DepositNYID
                            ,VDSP
                            ,VDSIDL
                            ,VDSNYID
                            ,TollIssueP
                            ,TollIssueIDL
                            ,TollIssueNYID
                            ,TollReceiveP
                            ,TollReceiveIDL
                            ,TollReceiveNYID
                            ,DSFP
                            ,DSFIDL
                            ,DSFNYID
                            ,DSRP
                            ,DSRIDL
                            ,DSRNYID
                            ,IssueFromBOM
                            ,PrepaidVAT
                            ,CYear

                            FROM setup";

                SqlCommand objCommSearchSetup = new SqlCommand();
                objCommSearchSetup.Connection = currConn;
                objCommSearchSetup.CommandText = sqlText;
                objCommSearchSetup.CommandType = CommandType.Text;



                #endregion
            }
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

            return sqlReturn;
        }

      
        public DataTable ResultIssueBOMNew(string databaseName)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("ResultIssueBOM");

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

                sqlText = @"select IssueFromBOM from setup;";

                SqlCommand objCommVATStatus = new SqlCommand();
                objCommVATStatus.Connection = currConn;
                objCommVATStatus.CommandText = sqlText;
                objCommVATStatus.CommandType = CommandType.Text;

                #region param

                //no param

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVATStatus);
                dataAdapter.Fill(dataTable);

                #endregion
            }
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

        public DataTable SearchSetupDataTable(string databaseName)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Setup");

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

                sqlText = @"SELECT 
                                    PurchaseP
                                    ,PurchaseIDL
                                    ,PurchaseNYID
                                    ,PurchaseTradingP
                                    ,PurchaseTradingIDL
                                    ,PurchaseTradingNYID
                                    ,IssueP
                                    ,IssueIDL
                                    ,IssueNYID
                                    ,IssueReturnP
                                    ,IssueReturnIDL
                                    ,IssueReturnNYID
                                    ,ReceiveP
                                    ,ReceiveIDL
                                    ,ReceiveNYID
                                    ,TransferP
                                    ,TransferIDL
                                    ,TransferNYID
                                    ,SaleP
                                    ,SaleIDL
                                    ,SaleNYID
                                    ,SaleServiceP
                                    ,SaleServiceIDL
                                    ,SaleServiceNYID
                                    ,SaleTradingP
                                    ,SaleTradingIDL
                                    ,SaleTradingNYID
                                    ,SaleExportP
                                    ,SaleExportIDL
                                    ,SaleExportNYID
                                    ,SaleTenderP
                                    ,SaleTenderIDL
                                    ,SaleTenderNYID
                                    ,DNP
                                    ,DNIDL
                                    ,DNNYID
                                    ,CNP
                                    ,CNIDL
                                    ,CNNYID
                                    ,DepositP
                                    ,DepositIDL
                                    ,DepositNYID
                                    ,VDSP
                                    ,VDSIDL
                                    ,VDSNYID
                                    ,TollIssueP
                                    ,TollIssueIDL
                                    ,TollIssueNYID
                                    ,TollReceiveP
                                    ,TollReceiveIDL
                                    ,TollReceiveNYID
                                    ,DSFP
                                    ,DSFIDL
                                    ,DSFNYID
                                    ,DSRP
                                    ,DSRIDL
                                    ,DSRNYID
                                    ,IssueFromBOM
                                    ,PrepaidVAT
                                    ,CYear
                                    FROM setup";

                SqlCommand objCommSearchSetup = new SqlCommand();
                objCommSearchSetup.Connection = currConn;
                objCommSearchSetup.CommandText = sqlText;
                objCommSearchSetup.CommandType = CommandType.Text;

                #region param

                //no param

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSearchSetup);
                dataAdapter.Fill(dataTable);

                #endregion
            }
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

        #region Old Methods

        public static string InsertToSetup(
string PurchaseP
, decimal PurchaseIDL
, string PurchaseNYID
, string PurchaseTradingP
, decimal PurchaseTradingIDL
, string PurchaseTradingNYID
, string IssueP
, decimal IssueIDL
, string IssueNYID
, string IssueReturnP
, decimal IssueReturnIDL
, string IssueReturnNYID
, string ReceiveP
, decimal ReceiveIDL
, string ReceiveNYID
, string TransferP
, decimal TransferIDL
, string TransferNYID
, string SaleP
, decimal SaleIDL
, string SaleNYID
, string SaleServiceP
, decimal SaleServiceIDL
, string SaleServiceNYID
, string SaleExportP
, decimal SaleExportIDL
, string SaleExportNYID
, string SaleTradingP
, decimal SaleTradingIDL
, string SaleTradingNYID
, string SaleTenderP
, decimal SaleTenderIDL
, string SaleTenderNYID
, string DNP
, decimal DNIDL
, string DNNYID
, string CNP
, decimal CNIDL
, string CNNYID
, string DepositP
, decimal DepositIDL
, string DepositNYID
, string VDSP
, decimal VDSIDL
, string VDSNYID
, string TollIssueP
, decimal TollIssueIDL
, string TollIssueNYID
, string TollReceiveP
, decimal TollReceiveIDL
, string TollReceiveNYID
, string DSFP
, decimal DSFIDL
, string DSFNYID
, string DSRP
, decimal DSRIDL
, string DSRNYID
, string IssueFromBOM
, string PrepaidVAT
, string CYear
, string databaseName
)
        {
            //string decryptedData123 = Converter.DESDecrypt(PassPhrase, EnKey, encryptedData123);
            //string decryptedData = Converter.DESDecrypt(PassPhrase, EnKey, encryptedData);

            //string[] SetupLines = decryptedData.Split(LineDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //string[] SetupFields = SetupLines[0].Split(FieldDelimeter.ToCharArray(), StringSplitOptions.None);

            SqlConnection cnDefault = null;
            //cnDefault = DBConnection.GetConnection(decryptedData123);
            cnDefault = DBConnection.GetConnection(databaseName);
            SqlTransaction transaction = cnDefault.BeginTransaction("SetupTransaction");
            SqlCommand objCmd = new SqlCommand();
            objCmd.Connection = cnDefault;
            objCmd.Transaction = transaction;
            try
            {

                //// Start Param
                //string PurchaseP = SetupFields[0];
                //decimal PurchaseIDL = Convert.ToDecimal(SetupFields[1]);
                //string PurchaseNYID = SetupFields[2];
                //string PurchaseTradingP = SetupFields[3];
                //decimal PurchaseTradingIDL = Convert.ToDecimal(SetupFields[4]);
                //string PurchaseTradingNYID = SetupFields[5];
                //string IssueP = SetupFields[6];
                //decimal IssueIDL = Convert.ToDecimal(SetupFields[7]);
                //string IssueNYID = SetupFields[8];
                //string IssueReturnP = SetupFields[9];
                //decimal IssueReturnIDL = Convert.ToDecimal(SetupFields[10]);
                //string IssueReturnNYID = SetupFields[11];
                //string ReceiveP = SetupFields[12];
                //decimal ReceiveIDL = Convert.ToDecimal(SetupFields[13]);
                //string ReceiveNYID = SetupFields[14];
                //string TransferP = SetupFields[15];
                //decimal TransferIDL = Convert.ToDecimal(SetupFields[16]);
                //string TransferNYID = SetupFields[17];
                //string SaleP = SetupFields[18];
                //decimal SaleIDL = Convert.ToDecimal(SetupFields[19]);
                //string SaleNYID = SetupFields[20];
                //string SaleServiceP = SetupFields[21];
                //decimal SaleServiceIDL = Convert.ToDecimal(SetupFields[22]);
                //string SaleServiceNYID = SetupFields[23];
                //string SaleTradingP = SetupFields[24];
                //decimal SaleTradingIDL = Convert.ToDecimal(SetupFields[25]);
                //string SaleTradingNYID = SetupFields[26];
                //string SaleExportP = SetupFields[27];
                //decimal SaleExportIDL = Convert.ToDecimal(SetupFields[28]);
                //string SaleExportNYID = SetupFields[29];
                //string SaleTenderP = SetupFields[30];
                //decimal SaleTenderIDL = Convert.ToDecimal(SetupFields[31]);
                //string SaleTenderNYID = SetupFields[32];
                //string DNP = SetupFields[33];
                //decimal DNIDL = Convert.ToDecimal(SetupFields[34]);
                //string DNNYID = SetupFields[35];
                //string CNP = SetupFields[36];
                //decimal CNIDL = Convert.ToDecimal(SetupFields[37]);
                //string CNNYID = SetupFields[38];
                //string DepositP = SetupFields[39];
                //decimal DepositIDL = Convert.ToDecimal(SetupFields[40]);
                //string DepositNYID = SetupFields[41];
                //string VDSP = SetupFields[42];
                //decimal VDSIDL = Convert.ToDecimal(SetupFields[43]);
                //string VDSNYID = SetupFields[44];
                //string TollIssueP = SetupFields[45];
                //decimal TollIssueIDL = Convert.ToDecimal(SetupFields[46]);
                //string TollIssueNYID = SetupFields[47];
                //string TollReceiveP = SetupFields[48];
                //decimal TollReceiveIDL = Convert.ToDecimal(SetupFields[49]);
                //string TollReceiveNYID = SetupFields[50];

                //string DSFP = SetupFields[51];
                //decimal DSFIDL = Convert.ToDecimal(SetupFields[52]);
                //string DSFNYID = SetupFields[53];
                //string DSRP = SetupFields[54];
                //decimal DSRIDL = Convert.ToDecimal(SetupFields[55]);
                //string DSRNYID = SetupFields[56];

                //string IssueFromBOM = SetupFields[57];
                //string PrepaidVAT = SetupFields[58];
                //string CYear = SetupFields[59];
                //// End Param



                string SetupResult = InsertToSetup1(objCmd
    , PurchaseP
    , PurchaseIDL
    , PurchaseNYID
    , PurchaseTradingP
    , PurchaseTradingIDL
    , PurchaseTradingNYID
    , IssueP
    , IssueIDL
    , IssueNYID
    , IssueReturnP
    , IssueReturnIDL
    , IssueReturnNYID
    , ReceiveP
    , ReceiveIDL
    , ReceiveNYID
    , TransferP
    , TransferIDL
    , TransferNYID
    , SaleP
    , SaleIDL
    , SaleNYID
    , SaleServiceP
    , SaleServiceIDL
    , SaleServiceNYID
    , SaleExportP
    , SaleExportIDL
    , SaleExportNYID
    , SaleTradingP
    , SaleTradingIDL
    , SaleTradingNYID
    , SaleTenderP
    , SaleTenderIDL
    , SaleTenderNYID
    , DNP
    , DNIDL
    , DNNYID
    , CNP
    , CNIDL
    , CNNYID
    , DepositP
    , DepositIDL
    , DepositNYID
    , VDSP
    , VDSIDL
    , VDSNYID
    , TollIssueP
    , TollIssueIDL
    , TollIssueNYID
    , TollReceiveP
    , TollReceiveIDL
    , TollReceiveNYID

    , DSFP
    , DSFIDL
    , DSFNYID
    , DSRP
    , DSRIDL
    , DSRNYID

    , IssueFromBOM
    , PrepaidVAT
    , CYear
        );

                if (Convert.ToInt32(SetupResult) > 0)
                {
                    transaction.Commit();
                    return SetupResult.ToString();
                }
                else /////rollback
                {
                    transaction.Rollback(); DBConnection.CloseConnection();
                    return "-1";
                }
            }
            catch (Exception ex)
            {

                transaction.Rollback(); DBConnection.CloseConnection();
                return "-1";
            }
        }

        private static string InsertToSetup1(
SqlCommand objCommSetup
, string PurchaseP
, decimal PurchaseIDL
, string PurchaseNYID
, string PurchaseTradingP
, decimal PurchaseTradingIDL
, string PurchaseTradingNYID
, string IssueP
, decimal IssueIDL
, string IssueNYID
, string IssueReturnP
, decimal IssueReturnIDL
, string IssueReturnNYID
, string ReceiveP
, decimal ReceiveIDL
, string ReceiveNYID
, string TransferP
, decimal TransferIDL
, string TransferNYID
, string SaleP
, decimal SaleIDL
, string SaleNYID
, string SaleServiceP
, decimal SaleServiceIDL
, string SaleServiceNYID
, string SaleExportP
, decimal SaleExportIDL
, string SaleExportNYID
, string SaleTradingP
, decimal SaleTradingIDL
, string SaleTradingNYID
, string SaleTenderP
, decimal SaleTenderIDL
, string SaleTenderNYID
, string DNP
, decimal DNIDL
, string DNNYID
, string CNP
, decimal CNIDL
, string CNNYID
, string DepositP
, decimal DepositIDL
, string DepositNYID
, string VDSP
, decimal VDSIDL
, string VDSNYID
, string TollIssueP
, decimal TollIssueIDL
, string TollIssueNYID
, string TollReceiveP
, decimal TollReceiveIDL
, string TollReceiveNYID
, string DSFP
, decimal DSFIDL
, string DSFNYID
, string DSRP
, decimal DSRIDL
, string DSRNYID
, string IssueFromBOM
, string PrepaidVAT
, string CYear
 )
        {
            string result = "-1";

            //string strSQL = "SpInsertUpdateSetup";
            string strSQL = @"update Setup set 
 PurchaseP=@PurchaseP
,PurchaseIDL=@PurchaseIDL
,PurchaseNYID=@PurchaseNYID
,PurchaseTradingP=@PurchaseTradingP
,PurchaseTradingIDL=@PurchaseTradingIDL
,PurchaseTradingNYID=@PurchaseTradingNYID
,IssueP=@IssueP
,IssueIDL=@IssueIDL
,IssueNYID=@IssueNYID
,IssueReturnP=@IssueReturnP
,IssueReturnIDL=@IssueReturnIDL
,IssueReturnNYID=@IssueReturnNYID
,ReceiveP=@ReceiveP
,ReceiveIDL=@ReceiveIDL
,ReceiveNYID=@ReceiveNYID
,TransferP=@TransferP
,TransferIDL=@TransferIDL
,TransferNYID=@TransferNYID
,SaleP=@SaleP
,SaleIDL=@SaleIDL
,SaleNYID=@SaleNYID
,SaleServiceP=@SaleServiceP
,SaleServiceIDL=@SaleServiceIDL
,SaleServiceNYID=@SaleServiceNYID
,SaleExportP=@SaleExportP
,SaleExportIDL=@SaleExportIDL
,SaleExportNYID=@SaleExportNYID
,SaleTradingP=@SaleTradingP
,SaleTradingIDL=@SaleTradingIDL
,SaleTradingNYID=@SaleTradingNYID
,SaleTenderP=@SaleTenderP
,SaleTenderIDL=@SaleTenderIDL
,SaleTenderNYID=@SaleTenderNYID
,DNP=@DNP
,DNIDL=@DNIDL
,DNNYID=@DNNYID
,CNP=@CNP
,CNIDL=@CNIDL
,CNNYID=@CNNYID
,DepositP=@DepositP
,DepositIDL=@DepositIDL
,DepositNYID=@DepositNYID
,VDSP=@VDSP
,VDSIDL=@VDSIDL
,VDSNYID=@VDSNYID
,TollIssueP=@TollIssueP
,TollIssueIDL=@TollIssueIDL
,TollIssueNYID=@TollIssueNYID
,TollReceiveP=@TollReceiveP
,TollReceiveIDL=@TollReceiveIDL
,TollReceiveNYID=@TollReceiveNYID
,DSFP=@DSFP
,DSFIDL=@DSFIDL
,DSFNYID=@DSFNYID
,DSRP=@DSRP
,DSRIDL=@DSRIDL
,DSRNYID=@DSRNYID
,IssueFromBOM=@IssueFromBOM
,PrepaidVAT=@PrepaidVAT
,CYear=@CYear";

            objCommSetup.CommandText = strSQL;
            objCommSetup.CommandType = CommandType.Text;

            #region Parameter

            if (!objCommSetup.Parameters.Contains("@PurchaseP"))
            { objCommSetup.Parameters.AddWithValue("@PurchaseP", PurchaseP); }
            else { objCommSetup.Parameters["@PurchaseP"].Value = PurchaseP; }
            if (!objCommSetup.Parameters.Contains("@PurchaseIDL"))
            { objCommSetup.Parameters.AddWithValue("@PurchaseIDL", PurchaseIDL); }
            else { objCommSetup.Parameters["@PurchaseIDL"].Value = PurchaseIDL; }
            if (!objCommSetup.Parameters.Contains("@PurchaseNYID"))
            { objCommSetup.Parameters.AddWithValue("@PurchaseNYID", PurchaseNYID); }
            else { objCommSetup.Parameters["@PurchaseNYID"].Value = PurchaseNYID; }
            if (!objCommSetup.Parameters.Contains("@PurchaseTradingP"))
            { objCommSetup.Parameters.AddWithValue("@PurchaseTradingP", PurchaseTradingP); }
            else { objCommSetup.Parameters["@PurchaseTradingP"].Value = PurchaseTradingP; }

            if (!objCommSetup.Parameters.Contains("@PurchaseTradingIDL"))
            { objCommSetup.Parameters.AddWithValue("@PurchaseTradingIDL", PurchaseTradingIDL); }
            else { objCommSetup.Parameters["@PurchaseTradingIDL"].Value = PurchaseTradingIDL; }

            if (!objCommSetup.Parameters.Contains("@PurchaseTradingNYID"))
            { objCommSetup.Parameters.AddWithValue("@PurchaseTradingNYID", PurchaseTradingNYID); }
            else { objCommSetup.Parameters["@PurchaseTradingNYID"].Value = PurchaseTradingNYID; }

            if (!objCommSetup.Parameters.Contains("@IssueP"))
            { objCommSetup.Parameters.AddWithValue("@IssueP", IssueP); }
            else { objCommSetup.Parameters["@IssueP"].Value = IssueP; }

            if (!objCommSetup.Parameters.Contains("@IssueIDL"))
            { objCommSetup.Parameters.AddWithValue("@IssueIDL", IssueIDL); }
            else { objCommSetup.Parameters["@IssueIDL"].Value = IssueIDL; }

            if (!objCommSetup.Parameters.Contains("@IssueNYID"))
            { objCommSetup.Parameters.AddWithValue("@IssueNYID", IssueNYID); }
            else { objCommSetup.Parameters["@IssueNYID"].Value = IssueNYID; }

            if (!objCommSetup.Parameters.Contains("@IssueReturnP"))
            { objCommSetup.Parameters.AddWithValue("@IssueReturnP", IssueReturnP); }
            else { objCommSetup.Parameters["@IssueReturnP"].Value = IssueReturnP; }

            if (!objCommSetup.Parameters.Contains("@IssueReturnIDL"))
            { objCommSetup.Parameters.AddWithValue("@IssueReturnIDL", IssueReturnIDL); }
            else { objCommSetup.Parameters["@IssueReturnIDL"].Value = IssueReturnIDL; }

            if (!objCommSetup.Parameters.Contains("@IssueReturnNYID"))
            { objCommSetup.Parameters.AddWithValue("@IssueReturnNYID", IssueReturnNYID); }
            else { objCommSetup.Parameters["@IssueReturnNYID"].Value = IssueReturnNYID; }

            if (!objCommSetup.Parameters.Contains("@ReceiveP"))
            { objCommSetup.Parameters.AddWithValue("@ReceiveP", ReceiveP); }
            else { objCommSetup.Parameters["@ReceiveP"].Value = ReceiveP; }

            if (!objCommSetup.Parameters.Contains("@ReceiveIDL"))
            { objCommSetup.Parameters.AddWithValue("@ReceiveIDL", ReceiveIDL); }
            else { objCommSetup.Parameters["@ReceiveIDL"].Value = ReceiveIDL; }

            if (!objCommSetup.Parameters.Contains("@ReceiveNYID"))
            { objCommSetup.Parameters.AddWithValue("@ReceiveNYID", ReceiveNYID); }
            else { objCommSetup.Parameters["@ReceiveNYID"].Value = ReceiveNYID; }

            if (!objCommSetup.Parameters.Contains("@TransferP"))
            { objCommSetup.Parameters.AddWithValue("@TransferP", TransferP); }
            else { objCommSetup.Parameters["@TransferP"].Value = TransferP; }

            if (!objCommSetup.Parameters.Contains("@TransferIDL"))
            { objCommSetup.Parameters.AddWithValue("@TransferIDL", TransferIDL); }
            else { objCommSetup.Parameters["@TransferIDL"].Value = TransferIDL; }

            if (!objCommSetup.Parameters.Contains("@TransferNYID"))
            { objCommSetup.Parameters.AddWithValue("@TransferNYID", TransferNYID); }
            else { objCommSetup.Parameters["@TransferNYID"].Value = TransferNYID; }

            if (!objCommSetup.Parameters.Contains("@SaleP"))
            { objCommSetup.Parameters.AddWithValue("@SaleP", SaleP); }
            else { objCommSetup.Parameters["@SaleP"].Value = SaleP; }

            if (!objCommSetup.Parameters.Contains("@SaleIDL"))
            { objCommSetup.Parameters.AddWithValue("@SaleIDL", SaleIDL); }
            else { objCommSetup.Parameters["@SaleIDL"].Value = SaleIDL; }

            if (!objCommSetup.Parameters.Contains("@SaleNYID"))
            { objCommSetup.Parameters.AddWithValue("@SaleNYID", SaleNYID); }
            else { objCommSetup.Parameters["@SaleNYID"].Value = SaleNYID; }

            if (!objCommSetup.Parameters.Contains("@SaleServiceP"))
            { objCommSetup.Parameters.AddWithValue("@SaleServiceP", SaleServiceP); }
            else { objCommSetup.Parameters["@SaleServiceP"].Value = SaleServiceP; }

            if (!objCommSetup.Parameters.Contains("@SaleServiceIDL"))
            { objCommSetup.Parameters.AddWithValue("@SaleServiceIDL", SaleServiceIDL); }
            else { objCommSetup.Parameters["@SaleServiceIDL"].Value = SaleServiceIDL; }

            if (!objCommSetup.Parameters.Contains("@SaleServiceNYID"))
            { objCommSetup.Parameters.AddWithValue("@SaleServiceNYID", SaleServiceNYID); }
            else { objCommSetup.Parameters["@SaleServiceNYID"].Value = SaleServiceNYID; }

            if (!objCommSetup.Parameters.Contains("@SaleExportP"))
            { objCommSetup.Parameters.AddWithValue("@SaleExportP", SaleExportP); }
            else { objCommSetup.Parameters["@SaleExportP"].Value = SaleExportP; }

            if (!objCommSetup.Parameters.Contains("@SaleExportIDL"))
            { objCommSetup.Parameters.AddWithValue("@SaleExportIDL", SaleExportIDL); }
            else { objCommSetup.Parameters["@SaleExportIDL"].Value = SaleExportIDL; }

            if (!objCommSetup.Parameters.Contains("@SaleExportNYID"))
            { objCommSetup.Parameters.AddWithValue("@SaleExportNYID", SaleExportNYID); }
            else { objCommSetup.Parameters["@SaleExportNYID"].Value = SaleExportNYID; }

            if (!objCommSetup.Parameters.Contains("@SaleTradingP"))
            { objCommSetup.Parameters.AddWithValue("@SaleTradingP", SaleTradingP); }
            else { objCommSetup.Parameters["@SaleTradingP"].Value = SaleTradingP; }

            if (!objCommSetup.Parameters.Contains("@SaleTradingIDL"))
            { objCommSetup.Parameters.AddWithValue("@SaleTradingIDL", SaleTradingIDL); }
            else { objCommSetup.Parameters["@SaleTradingIDL"].Value = SaleTradingIDL; }

            if (!objCommSetup.Parameters.Contains("@SaleTradingNYID"))
            { objCommSetup.Parameters.AddWithValue("@SaleTradingNYID", SaleTradingNYID); }
            else { objCommSetup.Parameters["@SaleTradingNYID"].Value = SaleTradingNYID; }

            if (!objCommSetup.Parameters.Contains("@SaleTenderP"))
            { objCommSetup.Parameters.AddWithValue("@SaleTenderP", SaleTenderP); }
            else { objCommSetup.Parameters["@SaleTenderP"].Value = SaleTenderP; }

            if (!objCommSetup.Parameters.Contains("@SaleTenderIDL"))
            { objCommSetup.Parameters.AddWithValue("@SaleTenderIDL", SaleTenderIDL); }
            else { objCommSetup.Parameters["@SaleTenderIDL"].Value = SaleTenderIDL; }

            if (!objCommSetup.Parameters.Contains("@SaleTenderNYID"))
            { objCommSetup.Parameters.AddWithValue("@SaleTenderNYID", SaleTenderNYID); }
            else { objCommSetup.Parameters["@SaleTenderNYID"].Value = SaleTenderNYID; }

            if (!objCommSetup.Parameters.Contains("@DNP"))
            { objCommSetup.Parameters.AddWithValue("@DNP", DNP); }
            else { objCommSetup.Parameters["@DNP"].Value = DNP; }

            if (!objCommSetup.Parameters.Contains("@DNIDL"))
            { objCommSetup.Parameters.AddWithValue("@DNIDL", DNIDL); }
            else { objCommSetup.Parameters["@DNIDL"].Value = DNIDL; }

            if (!objCommSetup.Parameters.Contains("@DNNYID"))
            { objCommSetup.Parameters.AddWithValue("@DNNYID", DNNYID); }
            else { objCommSetup.Parameters["@DNNYID"].Value = DNNYID; }

            if (!objCommSetup.Parameters.Contains("@CNP"))
            { objCommSetup.Parameters.AddWithValue("@CNP", CNP); }
            else { objCommSetup.Parameters["@CNP"].Value = CNP; }

            if (!objCommSetup.Parameters.Contains("@CNIDL"))
            { objCommSetup.Parameters.AddWithValue("@CNIDL", CNIDL); }
            else { objCommSetup.Parameters["@CNIDL"].Value = CNIDL; }

            if (!objCommSetup.Parameters.Contains("@CNNYID"))
            { objCommSetup.Parameters.AddWithValue("@CNNYID", CNNYID); }
            else { objCommSetup.Parameters["@CNNYID"].Value = CNNYID; }

            if (!objCommSetup.Parameters.Contains("@DepositP"))
            { objCommSetup.Parameters.AddWithValue("@DepositP", DepositP); }
            else { objCommSetup.Parameters["@DepositP"].Value = DepositP; }

            if (!objCommSetup.Parameters.Contains("@DepositIDL"))
            { objCommSetup.Parameters.AddWithValue("@DepositIDL", DepositIDL); }
            else { objCommSetup.Parameters["@DepositIDL"].Value = DepositIDL; }

            if (!objCommSetup.Parameters.Contains("@DepositNYID"))
            { objCommSetup.Parameters.AddWithValue("@DepositNYID", DepositNYID); }
            else { objCommSetup.Parameters["@DepositNYID"].Value = DepositNYID; }

            if (!objCommSetup.Parameters.Contains("@VDSP"))
            { objCommSetup.Parameters.AddWithValue("@VDSP", VDSP); }
            else { objCommSetup.Parameters["@VDSP"].Value = VDSP; }

            if (!objCommSetup.Parameters.Contains("@VDSIDL"))
            { objCommSetup.Parameters.AddWithValue("@VDSIDL", VDSIDL); }
            else { objCommSetup.Parameters["@VDSIDL"].Value = VDSIDL; }

            if (!objCommSetup.Parameters.Contains("@VDSNYID"))
            { objCommSetup.Parameters.AddWithValue("@VDSNYID", VDSNYID); }
            else { objCommSetup.Parameters["@VDSNYID"].Value = VDSNYID; }

            if (!objCommSetup.Parameters.Contains("@TollIssueP"))
            { objCommSetup.Parameters.AddWithValue("@TollIssueP", TollIssueP); }
            else { objCommSetup.Parameters["@TollIssueP"].Value = TollIssueP; }

            if (!objCommSetup.Parameters.Contains("@TollIssueIDL"))
            { objCommSetup.Parameters.AddWithValue("@TollIssueIDL", TollIssueIDL); }
            else { objCommSetup.Parameters["@TollIssueIDL"].Value = TollIssueIDL; }

            if (!objCommSetup.Parameters.Contains("@TollIssueNYID"))
            { objCommSetup.Parameters.AddWithValue("@TollIssueNYID", TollIssueNYID); }
            else { objCommSetup.Parameters["@TollIssueNYID"].Value = TollIssueNYID; }

            if (!objCommSetup.Parameters.Contains("@TollReceiveP"))
            { objCommSetup.Parameters.AddWithValue("@TollReceiveP", TollReceiveP); }
            else { objCommSetup.Parameters["@TollReceiveP"].Value = TollReceiveIDL; }

            if (!objCommSetup.Parameters.Contains("@TollReceiveIDL"))
            { objCommSetup.Parameters.AddWithValue("@TollReceiveIDL", TollReceiveIDL); }
            else { objCommSetup.Parameters["@TollReceiveIDL"].Value = TollReceiveNYID; }

            if (!objCommSetup.Parameters.Contains("@TollReceiveNYID"))
            { objCommSetup.Parameters.AddWithValue("@TollReceiveNYID", TollReceiveNYID); }
            else { objCommSetup.Parameters["@TollReceiveNYID"].Value = TollReceiveNYID; }


            if (!objCommSetup.Parameters.Contains("@DSFP"))
            { objCommSetup.Parameters.AddWithValue("@DSFP", DSFP); }
            else { objCommSetup.Parameters["@DSFP"].Value = DSFP; }

            if (!objCommSetup.Parameters.Contains("@DSFIDL"))
            { objCommSetup.Parameters.AddWithValue("@DSFIDL", DSFIDL); }
            else { objCommSetup.Parameters["@DSFIDL"].Value = DSFIDL; }

            if (!objCommSetup.Parameters.Contains("@DSFNYID"))
            { objCommSetup.Parameters.AddWithValue("@DSFNYID", DSFNYID); }
            else { objCommSetup.Parameters["@DSFNYID"].Value = DSFNYID; }

            if (!objCommSetup.Parameters.Contains("@DSRP"))
            { objCommSetup.Parameters.AddWithValue("@DSRP", DSRP); }
            else { objCommSetup.Parameters["@DSRP"].Value = DSRIDL; }

            if (!objCommSetup.Parameters.Contains("@DSRIDL"))
            { objCommSetup.Parameters.AddWithValue("@DSRIDL", DSRIDL); }
            else { objCommSetup.Parameters["@DSRIDL"].Value = DSRIDL; }

            if (!objCommSetup.Parameters.Contains("@DSRNYID"))
            { objCommSetup.Parameters.AddWithValue("@DSRNYID", DSRNYID); }
            else { objCommSetup.Parameters["@DSRNYID"].Value = DSRNYID; }


            if (!objCommSetup.Parameters.Contains("@IssueFromBOM"))
            { objCommSetup.Parameters.AddWithValue("@IssueFromBOM", IssueFromBOM); }
            else { objCommSetup.Parameters["@IssueFromBOM"].Value = IssueFromBOM; }
            if (!objCommSetup.Parameters.Contains("@PrepaidVAT"))
            { objCommSetup.Parameters.AddWithValue("@PrepaidVAT", PrepaidVAT); }
            else { objCommSetup.Parameters["@PrepaidVAT"].Value = PrepaidVAT; }
            if (!objCommSetup.Parameters.Contains("@CYear"))
            { objCommSetup.Parameters.AddWithValue("@CYear", CYear); }
            else { objCommSetup.Parameters["@CYear"].Value = CYear; }

            #endregion Parameter



            try
            {
                result = objCommSetup.ExecuteNonQuery().ToString();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                { return "-99"; }
                else { return "-9"; }
            }
            catch (Exception ex) { Trace.WriteLine(ex.Message); }
            finally { }

            return result;

        }

        //public static string SearchSetup(string encryptedData123)
        public static string SearchSetup(string databaseName)
        {
            //string decryptedData123 = Converter.DESDecrypt(PassPhrase, EnKey, encryptedData123);

            SqlConnection cnDefault = null;
            //cnDefault = DBConnection.GetConnection(decryptedData123);
            cnDefault = DBConnection.GetConnection(databaseName);
            SqlCommand objCmd = new SqlCommand();
            objCmd.Connection = cnDefault;

            return SetupDAL.SearchSetup1(objCmd);
        }

        private static string SearchSetup1(SqlCommand objCommSearchSetup)
        {
            string decryptedData = string.Empty;

            //string strSQL = "SpSearchSetup";
            string strSQL = @"SELECT 
                                    PurchaseP
                                    ,PurchaseIDL
                                    ,PurchaseNYID
                                    ,PurchaseTradingP
                                    ,PurchaseTradingIDL
                                    ,PurchaseTradingNYID
                                    ,IssueP
                                    ,IssueIDL
                                    ,IssueNYID
                                    ,IssueReturnP
                                    ,IssueReturnIDL
                                    ,IssueReturnNYID
                                    ,ReceiveP
                                    ,ReceiveIDL
                                    ,ReceiveNYID
                                    ,TransferP
                                    ,TransferIDL
                                    ,TransferNYID
                                    ,SaleP
                                    ,SaleIDL
                                    ,SaleNYID
                                    ,SaleServiceP
                                    ,SaleServiceIDL
                                    ,SaleServiceNYID
                                    ,SaleTradingP
                                    ,SaleTradingIDL
                                    ,SaleTradingNYID
                                    ,SaleExportP
                                    ,SaleExportIDL
                                    ,SaleExportNYID
                                    ,SaleTenderP
                                    ,SaleTenderIDL
                                    ,SaleTenderNYID
                                    ,DNP
                                    ,DNIDL
                                    ,DNNYID
                                    ,CNP
                                    ,CNIDL
                                    ,CNNYID
                                    ,DepositP
                                    ,DepositIDL
                                    ,DepositNYID
                                    ,VDSP
                                    ,VDSIDL
                                    ,VDSNYID
                                    ,TollIssueP
                                    ,TollIssueIDL
                                    ,TollIssueNYID
                                    ,TollReceiveP
                                    ,TollReceiveIDL
                                    ,TollReceiveNYID
                                    ,DSFP
                                    ,DSFIDL
                                    ,DSFNYID
                                    ,DSRP
                                    ,DSRIDL
                                    ,DSRNYID
                                    ,IssueFromBOM
                                    ,PrepaidVAT
                                    ,CYear
                                    FROM setup";

            objCommSearchSetup.CommandText = strSQL;
            objCommSearchSetup.CommandType = CommandType.Text;

            try
            {
                SqlDataReader reader = objCommSearchSetup.ExecuteReader();
                while (reader.Read())
                {
                    for (int j = 0; j < reader.FieldCount; j++)
                    {
                        decryptedData = decryptedData + FieldDelimeter + reader[j].ToString();
                    }
                    decryptedData = decryptedData + LineDelimeter;
                }
                reader.Close();
            }
            catch (SqlException ex)
            {
                Trace.Write(ex.Message);
            }
            string encriptedData = Converter.DESEncrypt(PassPhrase, EnKey, decryptedData);
            return encriptedData;


        }

        //public  DataSet ResultVATStatus(string encryptedData, string encryptedData123)
        public DataSet ResultVATStatus(DateTime StartDate, string databaseName)
        {


            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataSet reportDataset = new DataSet();


            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                sqlText = @"
declare @PVat [decimal](18, 5);
SET @PVat=0;
select @PVat =sum(Amt) FROM(
select 'Z1' Sl ,isnull(sum(DepositAmount),0)Amt
from  Deposits where post='Y' 
AND Deposits.TransactionType  IN('Treasury-Opening')
UNION ALL
select 'Z2' Sl ,isnull(sum(DepositAmount),0)Amt
from  Deposits where post='Y' and DepositDateTime < @StartDate
AND Deposits.TransactionType  IN('Treasury')
UNION ALL


SELECT  'Z3' Sl ,  isnull(sum(PID.VATAmount),0)Amt
FROM PurchaseInvoiceDetails PID
where PID.post='Y' and PID.ReceiveDate< @StartDate and Type in('Local-VAT','Local-Tarrif') 
AND PID.TransactionType IN('Other','Trading','TollReceive','Service','ServiceNS',
'TollReceive-WIP','PurchaseCN')

UNION ALL
SELECT  'Z4' Sl ,  isnull(sum(PID.RebateAmount),0)Amt
FROM PurchaseInvoiceDetails PID
where PID.post='Y' and PID.ReceiveDate<@StartDate and Type in('Local-VAT','Local-Tarrif') 
AND PID.TransactionType IN('InputService')
UNION ALL

SELECT 'Z5' Sl ,   isnull(sum(PID.VATAmount),0)Amt
FROM PurchaseInvoiceDetails PID
where PID.post='Y' and PID.ReceiveDate< @StartDate and Type in('Import-VAT','Import-Tarrif') 
AND PID.TransactionType IN('Import','TradingImport','InputServiceImport','ServiceImport','ServiceNSImport')


UNION ALL
SELECT 'Z6' Sl ,  -isnull(sum(PID.VATAmount),0)Amt
FROM PurchaseInvoiceDetails PID
where PID.post='Y' and PID.ReceiveDate<  @StartDate and Type in('Local-VAT','Local-Tarrif') 
AND PID.TransactionType IN('PurchaseReturn','PurchaseDN')

UNION ALL
SELECT 'Z7' Sl ,-isnull(sum(SID.VATAmount),0)Amt
FROM SalesInvoiceDetails SID
where SID.post='Y' and SID.invoicedatetime<   @StartDate and Type='VAT' 
AND SID.TransactionType IN('Other','ServiceStock','Service','Trading','TradingTender','Tender','Debit','InternalIssue','TollFinishIssue','PackageSale','PackageProduction','ServiceNS')


UNION ALL
SELECT 'Z8' Sl ,-isnull(sum(CurrencyValue*VATRate/100),0)Amt
FROM SalesInvoiceDetails SID 
where SID.post='Y' and SID.invoicedatetime<   @StartDate and Type='Export' 
AND SID.TransactionType IN('Export','ExportService','ExportServiceNS','ExportTender','ExportTrading','ExportTradingTender','ExportPackage')

UNION ALL

SELECT 'Z9' Sl ,isnull(sum(SID.VATAmount),0)Amt
FROM SalesInvoiceDetails SID
where SID.post='Y' and SID.invoicedatetime<   @StartDate and Type='VAT' 
AND SID.TransactionType IN('Credit')
UNION ALL

select 'Z10' Sl ,-isnull(sum(isnull(SIH.AppVATAmount,0)+isnull(SIH.AppVATAmountImport,0)),0) Amt
from  DisposeHeaders SIH
where SIH.DisposeDate < @StartDate AND 
SIH.TransactionType IN('VAT26')
AND (SIH.Post ='Y')
UNION ALL
select 'Z10' Sl ,-isnull(sum(isnull(SIH.AppVATAmount,0)+isnull(SIH.AppVATAmountImport,0)),0) Amt
from  DisposeHeaders SIH
where SIH.DisposeDate < @StartDate AND 
SIH.TransactionType IN('VAT27')
AND (SIH.Post ='Y')

UNION ALL
select 'Z11' Sl ,-sum(isnull(SIH.AdjAmount,0)) Amt
from  AdjustmentHistorys SIH
where SIH.AdjDate <   @StartDate
AND SIH.AdjType in('Credit Payable')
AND (SIH.Post ='Y')
UNION ALL
select 'Z12' Sl ,-sum(isnull(SIH.AdjAmount,0)) Amt
from  AdjustmentHistorys SIH
where SIH.AdjDate <   @StartDate
AND SIH.AdjType in('Shortage Rebatable')
AND (SIH.Post ='Y')
UNION ALL
select 'Z13' Sl ,sum(isnull(SIH.AdjAmount,0))   Amt
from  AdjustmentHistorys SIH
where SIH.AdjDate <  @StartDate
AND SIH.AdjType in('Rebatable')
AND (SIH.Post='Y')

UNION ALL
select 'Z14' Sl ,sum(isnull(SIH.TotalDDBack,0))   Amt
from  DutyDrawBackHeader SIH
where SIH.DDBackDate <  @StartDate
AND (SIH.Post='Y')


)AS a 
SELECT @PVat
";
                SqlCommand objCommVATStatus = new SqlCommand();
                objCommVATStatus.Connection = currConn;
                objCommVATStatus.CommandText = sqlText;
                objCommVATStatus.CommandType = CommandType.Text;


                if (!objCommVATStatus.Parameters.Contains("@StartDate"))
                { objCommVATStatus.Parameters.AddWithValue("@StartDate", StartDate); }
                else { objCommVATStatus.Parameters["@StartDate"].Value = StartDate; }


                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(objCommVATStatus);
                reportDataAdapt.Fill(reportDataset);


            }
            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                throw ex;
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
            return reportDataset;
            /////////////////////////////////////////
            
            //string decryptedData123 = Converter.DESDecrypt(PassPhrase, EnKey, encryptedData123);

            //string decryptedData = Converter.DESDecrypt(PassPhrase, EnKey, encryptedData);
            //string[] VATStatusLines = decryptedData.Split(LineDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //string[] VATStatusFields = VATStatusLines[0].Split(FieldDelimeter.ToCharArray(), StringSplitOptions.None);

            //DateTime StartDate = Convert.ToDateTime(VATStatusFields[0]);

            //SqlConnection cnDefault = null;
            ////cnDefault = DBConnection.GetConnection(decryptedData123);
            //cnDefault = DBConnection.GetConnection(databaseName);
            //SqlCommand objCmd = new SqlCommand();
            //objCmd.Connection = cnDefault;

            //return SetupDAL.ResultVATStatus1(objCmd, StartDate);
        }




        private static DataSet ResultIssueBOM(SqlCommand objCommVATStatus)
        {
            DataSet reportDataset = new DataSet();
            try
            {
                string strSQL = "select IssueFromBOM from setup;";
                objCommVATStatus.CommandText = strSQL;
                objCommVATStatus.CommandType = CommandType.Text;

                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(objCommVATStatus);
                reportDataAdapt.Fill(reportDataset);
            }
            catch (SqlException ex)
            {
                Trace.Write(ex.Message);
            }
            return reportDataset;
        }

        #endregion

    }
}
