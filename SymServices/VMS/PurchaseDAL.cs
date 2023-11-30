using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymOrdinary;
using SymphonySofttech.Utilities;
using SymViewModel.VMS;
using System.Reflection;
using Excel;
using System.IO;

namespace SymServices.VMS
{
    public class PurchaseDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDal = new CommonDAL();

        #endregion
        static string[] columnName = new string[] { "Purchase Invoice No", "Serial No", "BE Number", "Vendor Name" };
        public IEnumerable<object> GetPurchaseColumn()
        {
            IEnumerable<object> enumList = from e in columnName
                                           select new { Id = e.ToString().Replace(" ", ""), Name = e.ToString() };
            return enumList;
        }

        public DataTable SearchPurchaseHeaderDTNew(string PurchaseInvoiceNo, string WithVDS, string VendorName,
        string VendorGroupID, string VendorGroupName, string InvoiceDateFrom, string InvoiceDateTo,
        string SerialNo, string TransactionType, string BENumber, string Post)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("ProductType");

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

                sqlText = " ";
                sqlText += " SELECT";
                sqlText += " PurchaseInvoiceHeaders.PurchaseInvoiceNo,   ";
                sqlText += " PurchaseInvoiceHeaders.VendorID,";
                sqlText += " isnull(PurchaseInvoiceHeaders.CustomHouse,'N/A')CustomHouse,";
                sqlText += " isnull(Vendors.VendorName,'N/A')VendorName,";
                sqlText += " Vendors.VendorGroupID,";
                sqlText += " isnull(VendorGroups.VendorGroupName,'N/A')VendorGroupName,";
                sqlText += " convert (varchar,PurchaseInvoiceHeaders.InvoiceDateTime,120)InvoiceDateTime,";
                sqlText += " isnull(PurchaseInvoiceHeaders.TotalAmount,0)TotalAmount,";
                sqlText += " isnull(PurchaseInvoiceHeaders.TotalVATAmount,0)TotalVATAmount,";
                sqlText += " isnull(PurchaseInvoiceHeaders.SerialNo,'N/A')SerialNo,";
                sqlText += " isnull(PurchaseInvoiceHeaders.LCNumber,'N/A')LCNumber,";

                sqlText += " isnull(PurchaseInvoiceHeaders.Comments,'N/A')Comments,";
                sqlText += " PurchaseInvoiceHeaders.ProductType,PurchaseInvoiceHeaders.Post,";
                sqlText += " PurchaseInvoiceHeaders.PurchaseReturnId,";
                sqlText += " PurchaseInvoiceHeaders.WithVDS,";
                sqlText += " convert (varchar,PurchaseInvoiceHeaders.ReceiveDate,120)ReceiveDate,";
                sqlText += " PurchaseInvoiceHeaders.TransactionType,";

                sqlText += " isnull(PurchaseInvoiceHeaders.BENumber,'N/A')BENumber,";

                sqlText += " convert (varchar,PurchaseInvoiceHeaders.LCDate,120)LCDate,";
                sqlText += " isnull(PurchaseInvoiceHeaders.LandedCost,0)LandedCost";

                sqlText += " FROM PurchaseInvoiceHeaders LEFT OUTER JOIN";
                sqlText += " Vendors ON  PurchaseInvoiceHeaders.VendorID =  Vendors.VendorID LEFT OUTER JOIN";
                sqlText += " VendorGroups ON  Vendors.VendorGroupID =  VendorGroups.VendorGroupID                 ";
                sqlText += " WHERE ";
                sqlText += "     (PurchaseInvoiceHeaders.PurchaseInvoiceNo LIKE '%' +  @PurchaseInvoiceNo + '%' OR @PurchaseInvoiceNo IS NULL) ";
                sqlText += " AND (PurchaseInvoiceHeaders.WithVDS  LIKE '%' + @WithVDS  + '%' OR @WithVDS IS NULL) ";
                sqlText += " AND (Vendors.VendorName LIKE '%' + @VendorName + '%' OR @VendorName IS NULL) ";
                sqlText += " AND (Vendors.VendorGroupID LIKE '%' + @VendorGroupID + '%' OR @VendorGroupID IS NULL)";
                sqlText += " AND (VendorGroups.VendorGroupName LIKE '%' + @VendorGroupName + '%' OR @VendorGroupName IS NULL) ";
                //sqlText += " AND (PurchaseInvoiceHeaders.InvoiceDateTime>= @InvoiceDateFrom OR @InvoiceDateFrom IS NULL)";
                //sqlText += " AND (PurchaseInvoiceHeaders.InvoiceDateTime<= @InvoiceDateTo OR @InvoiceDateTo IS NULL)";
                sqlText += " AND (PurchaseInvoiceHeaders.ReceiveDate >= @InvoiceDateFrom OR @InvoiceDateFrom IS NULL)";
                sqlText += " AND (PurchaseInvoiceHeaders.ReceiveDate <= @InvoiceDateTo OR @InvoiceDateTo IS NULL) ";
                sqlText += " AND (PurchaseInvoiceHeaders.SerialNo LIKE '%' + @SerialNo + '%' OR @SerialNo IS NULL) ";
                if (TransactionType == "All")
                //{sqlText += " AND (PurchaseInvoiceHeaders.TransactionType LIKE '%' +@TransactionType + '%' OR @TransactionType IS NULL) ";}
                {
                    sqlText += " AND (PurchaseInvoiceHeaders.TransactionType not in ('PurchaseReturn') ) ";
                }
                else
                {
                    //sqlText += " AND (PurchaseInvoiceHeaders.TransactionType='" + TransactionType + "') ";
                    sqlText += " AND (PurchaseInvoiceHeaders.TransactionType LIKE '%' +@TransactionType + '%' OR @TransactionType IS NULL) ";
                }

                sqlText += " AND (PurchaseInvoiceHeaders.BENumber LIKE '%' + @BENumber + '%' OR @BENumber IS NULL) ";
                sqlText += " AND (PurchaseInvoiceHeaders.Post LIKE '%' + @Post  + '%' OR @Post IS NULL) ";

                SqlCommand objCommPurchaseHeader = new SqlCommand();
                objCommPurchaseHeader.Connection = currConn;
                objCommPurchaseHeader.CommandText = sqlText;
                objCommPurchaseHeader.CommandType = CommandType.Text;

                #region Parameter

                if (!objCommPurchaseHeader.Parameters.Contains("@PurchaseInvoiceNo"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@PurchaseInvoiceNo", PurchaseInvoiceNo); }
                else { objCommPurchaseHeader.Parameters["@PurchaseInvoiceNo"].Value = PurchaseInvoiceNo; }
                if (!objCommPurchaseHeader.Parameters.Contains("@Post"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@Post", Post); }
                else { objCommPurchaseHeader.Parameters["@Post"].Value = Post; }

                if (!objCommPurchaseHeader.Parameters.Contains("@WithVDS"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@WithVDS", WithVDS); }
                else { objCommPurchaseHeader.Parameters["@WithVDS"].Value = WithVDS; }
                if (!objCommPurchaseHeader.Parameters.Contains("@VendorName"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@VendorName", VendorName); }
                else { objCommPurchaseHeader.Parameters["@VendorName"].Value = VendorName; }
                if (!objCommPurchaseHeader.Parameters.Contains("@VendorGroupID"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@VendorGroupID", VendorGroupID); }
                else { objCommPurchaseHeader.Parameters["@VendorGroupID"].Value = VendorGroupID; }
                if (!objCommPurchaseHeader.Parameters.Contains("@VendorGroupName"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@VendorGroupName", VendorGroupName); }
                else { objCommPurchaseHeader.Parameters["@VendorGroupName"].Value = VendorGroupName; }
                if (InvoiceDateFrom == "")
                {
                    if (!objCommPurchaseHeader.Parameters.Contains("@InvoiceDateFrom"))
                    { objCommPurchaseHeader.Parameters.AddWithValue("@InvoiceDateFrom", System.DBNull.Value); }
                    else { objCommPurchaseHeader.Parameters["@InvoiceDateFrom"].Value = System.DBNull.Value; }
                }
                else
                {

                    if (!objCommPurchaseHeader.Parameters.Contains("@InvoiceDateFrom"))
                    { objCommPurchaseHeader.Parameters.AddWithValue("@InvoiceDateFrom", InvoiceDateFrom); }
                    else { objCommPurchaseHeader.Parameters["@InvoiceDateFrom"].Value = InvoiceDateFrom; }
                }
                if (InvoiceDateTo == "")
                {
                    if (!objCommPurchaseHeader.Parameters.Contains("@InvoiceDateTo"))
                    { objCommPurchaseHeader.Parameters.AddWithValue("@InvoiceDateTo", System.DBNull.Value); }
                    else { objCommPurchaseHeader.Parameters["@InvoiceDateTo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommPurchaseHeader.Parameters.Contains("@InvoiceDateTo"))
                    { objCommPurchaseHeader.Parameters.AddWithValue("@InvoiceDateTo", InvoiceDateTo); }
                    else { objCommPurchaseHeader.Parameters["@InvoiceDateTo"].Value = InvoiceDateTo; }
                }

                if (!objCommPurchaseHeader.Parameters.Contains("@SerialNo"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@SerialNo", SerialNo); }
                else { objCommPurchaseHeader.Parameters["@SerialNo"].Value = SerialNo; }
                if (!objCommPurchaseHeader.Parameters.Contains("@TransactionType"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@TransactionType", TransactionType); }
                else { objCommPurchaseHeader.Parameters["@TransactionType"].Value = TransactionType; }
                if (!objCommPurchaseHeader.Parameters.Contains("@BENumber"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@BENumber", BENumber); }
                else { objCommPurchaseHeader.Parameters["@BENumber"].Value = BENumber; }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommPurchaseHeader);
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

        public DataTable SearchPurchaseHeaderDTNewDuty(string PurchaseInvoiceNo, string WithVDS, string VendorName,
string VendorGroupID, string VendorGroupName, string InvoiceDateFrom, string InvoiceDateTo,
string SerialNo, string TransactionType, string BENumber, string Post)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("ProductType");

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

                sqlText = " ";
                sqlText += " SELECT";
                sqlText += " PurchaseInvoiceHeaders.PurchaseInvoiceNo,   ";
                sqlText += " PurchaseInvoiceHeaders.VendorID,";
                sqlText += " isnull(PurchaseInvoiceHeaders.CustomHouse,'N/A')CustomHouse,";
                sqlText += " isnull(Vendors.VendorName,'N/A')VendorName,";
                sqlText += " Vendors.VendorGroupID,";
                sqlText += " isnull(VendorGroups.VendorGroupName,'N/A')VendorGroupName,";
                sqlText += " convert (varchar,PurchaseInvoiceHeaders.InvoiceDateTime,120)InvoiceDateTime,";
                sqlText += " isnull(PurchaseInvoiceHeaders.TotalAmount,0)TotalAmount,";
                sqlText += " isnull(PurchaseInvoiceHeaders.TotalVATAmount,0)TotalVATAmount,";
                sqlText += " isnull(PurchaseInvoiceHeaders.SerialNo,'N/A')SerialNo,";
                sqlText += " isnull(PurchaseInvoiceHeaders.LCNumber,'N/A')LCNumber,";

                sqlText += " isnull(PurchaseInvoiceHeaders.Comments,'N/A')Comments,";
                sqlText += " PurchaseInvoiceHeaders.ProductType,PurchaseInvoiceHeaders.Post,";
                sqlText += " PurchaseInvoiceHeaders.PurchaseReturnId,";
                sqlText += " PurchaseInvoiceHeaders.WithVDS,";
                sqlText += " convert (varchar,PurchaseInvoiceHeaders.ReceiveDate,120)ReceiveDate,";
                sqlText += " PurchaseInvoiceHeaders.TransactionType,";

                sqlText += " isnull(PurchaseInvoiceHeaders.BENumber,'N/A')BENumber,";

                sqlText += " convert (varchar,PurchaseInvoiceHeaders.LCDate,120)LCDate,";
                sqlText += "  Select * from PurchaseInvoiceDuties where PurchaseInvoiceNo in(";

                sqlText += " SELECT PurchaseInvoiceHeaders.PurchaseInvoiceNo FROM PurchaseInvoiceHeaders LEFT OUTER JOIN";
                sqlText += " Vendors ON  PurchaseInvoiceHeaders.VendorID =  Vendors.VendorID LEFT OUTER JOIN";
                sqlText += " VendorGroups ON  Vendors.VendorGroupID =  VendorGroups.VendorGroupID                 ";
                sqlText += " WHERE ";
                sqlText += "     (PurchaseInvoiceHeaders.PurchaseInvoiceNo LIKE '%' +  @PurchaseInvoiceNo + '%' OR @PurchaseInvoiceNo IS NULL) ";
                sqlText += " AND (PurchaseInvoiceHeaders.WithVDS  LIKE '%' + @WithVDS  + '%' OR @WithVDS IS NULL) ";
                sqlText += " AND (Vendors.VendorName LIKE '%' + @VendorName + '%' OR @VendorName IS NULL) ";
                sqlText += " AND (Vendors.VendorGroupID LIKE '%' + @VendorGroupID + '%' OR @VendorGroupID IS NULL)";
                sqlText += " AND (VendorGroups.VendorGroupName LIKE '%' + @VendorGroupName + '%' OR @VendorGroupName IS NULL) ";
                //sqlText += " AND (PurchaseInvoiceHeaders.InvoiceDateTime<= @InvoiceDateTo OR @InvoiceDateTo IS NULL)";
                sqlText += " AND (PurchaseInvoiceHeaders.ReceiveDate >= @InvoiceDateFrom OR @InvoiceDateFrom IS NULL)";
                sqlText += " AND (PurchaseInvoiceHeaders.ReceiveDate <= @InvoiceDateTo OR @InvoiceDateTo IS NULL) ";
                sqlText += " AND (PurchaseInvoiceHeaders.SerialNo LIKE '%' + @SerialNo + '%' OR @SerialNo IS NULL) ";
                if (TransactionType == "All")
                {
                    sqlText += " AND (PurchaseInvoiceHeaders.TransactionType not in ('PurchaseReturn') ) ";
                }
                else
                {
                    sqlText += " AND (PurchaseInvoiceHeaders.TransactionType LIKE '%' +@TransactionType + '%' OR @TransactionType IS NULL) ";
                }

                sqlText += " AND (PurchaseInvoiceHeaders.BENumber LIKE '%' + @BENumber + '%' OR @BENumber IS NULL) ";
                sqlText += " AND (PurchaseInvoiceHeaders.Post LIKE '%' + @Post  + '%' OR @Post IS NULL) ";
                sqlText += " )";
                SqlCommand objCommPurchaseHeader = new SqlCommand();
                objCommPurchaseHeader.Connection = currConn;
                objCommPurchaseHeader.CommandText = sqlText;
                objCommPurchaseHeader.CommandType = CommandType.Text;

                #region Parameter

                if (!objCommPurchaseHeader.Parameters.Contains("@PurchaseInvoiceNo"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@PurchaseInvoiceNo", PurchaseInvoiceNo); }
                else { objCommPurchaseHeader.Parameters["@PurchaseInvoiceNo"].Value = PurchaseInvoiceNo; }
                if (!objCommPurchaseHeader.Parameters.Contains("@Post"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@Post", Post); }
                else { objCommPurchaseHeader.Parameters["@Post"].Value = Post; }

                if (!objCommPurchaseHeader.Parameters.Contains("@WithVDS"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@WithVDS", WithVDS); }
                else { objCommPurchaseHeader.Parameters["@WithVDS"].Value = WithVDS; }
                if (!objCommPurchaseHeader.Parameters.Contains("@VendorName"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@VendorName", VendorName); }
                else { objCommPurchaseHeader.Parameters["@VendorName"].Value = VendorName; }
                if (!objCommPurchaseHeader.Parameters.Contains("@VendorGroupID"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@VendorGroupID", VendorGroupID); }
                else { objCommPurchaseHeader.Parameters["@VendorGroupID"].Value = VendorGroupID; }
                if (!objCommPurchaseHeader.Parameters.Contains("@VendorGroupName"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@VendorGroupName", VendorGroupName); }
                else { objCommPurchaseHeader.Parameters["@VendorGroupName"].Value = VendorGroupName; }
                if (InvoiceDateFrom == "")
                {
                    if (!objCommPurchaseHeader.Parameters.Contains("@InvoiceDateFrom"))
                    { objCommPurchaseHeader.Parameters.AddWithValue("@InvoiceDateFrom", System.DBNull.Value); }
                    else { objCommPurchaseHeader.Parameters["@InvoiceDateFrom"].Value = System.DBNull.Value; }
                }
                else
                {

                    if (!objCommPurchaseHeader.Parameters.Contains("@InvoiceDateFrom"))
                    { objCommPurchaseHeader.Parameters.AddWithValue("@InvoiceDateFrom", InvoiceDateFrom); }
                    else { objCommPurchaseHeader.Parameters["@InvoiceDateFrom"].Value = InvoiceDateFrom; }
                }
                if (InvoiceDateTo == "")
                {
                    if (!objCommPurchaseHeader.Parameters.Contains("@InvoiceDateTo"))
                    { objCommPurchaseHeader.Parameters.AddWithValue("@InvoiceDateTo", System.DBNull.Value); }
                    else { objCommPurchaseHeader.Parameters["@InvoiceDateTo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommPurchaseHeader.Parameters.Contains("@InvoiceDateTo"))
                    { objCommPurchaseHeader.Parameters.AddWithValue("@InvoiceDateTo", InvoiceDateTo); }
                    else { objCommPurchaseHeader.Parameters["@InvoiceDateTo"].Value = InvoiceDateTo; }
                }

                if (!objCommPurchaseHeader.Parameters.Contains("@SerialNo"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@SerialNo", SerialNo); }
                else { objCommPurchaseHeader.Parameters["@SerialNo"].Value = SerialNo; }
                if (!objCommPurchaseHeader.Parameters.Contains("@TransactionType"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@TransactionType", TransactionType); }
                else { objCommPurchaseHeader.Parameters["@TransactionType"].Value = TransactionType; }
                if (!objCommPurchaseHeader.Parameters.Contains("@BENumber"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@BENumber", BENumber); }
                else { objCommPurchaseHeader.Parameters["@BENumber"].Value = BENumber; }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommPurchaseHeader);
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

        public DataTable SearchPurchaseHeaderDTNew2(string PurchaseInvoiceNo, string WithVDS, string VendorName,
     string VendorGroupID, string VendorGroupName, string InvoiceDateFrom, string InvoiceDateTo,
     string SerialNo, string T1Type, string T2Type, string BENumber, string Post)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("ProductType1");

            #endregion

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "ItemNo", "varchar(20)", currConn);
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "DutyCompleteQuantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "DutyCompleteQuantityPercent", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "LineCost", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "UnitCost", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "Quantity", "decimal(25, 9)", currConn);
                #endregion open connection and transaction

                #region sql statement

                sqlText += " SELECT";
                sqlText += " PurchaseInvoiceHeaders.PurchaseInvoiceNo,   ";
                sqlText += " PurchaseInvoiceHeaders.VendorID,";
                sqlText += " isnull(PurchaseInvoiceHeaders.CustomHouse,'N/A')CustomHouse,";
                sqlText += " isnull(Vendors.VendorName,'N/A')VendorName,";
                sqlText += " Vendors.VendorGroupID,";
                sqlText += " isnull(VendorGroups.VendorGroupName,'N/A')VendorGroupName,";
                sqlText += " convert (varchar,PurchaseInvoiceHeaders.InvoiceDateTime,120)InvoiceDateTime,";
                sqlText += " isnull(PurchaseInvoiceHeaders.TotalAmount,0)TotalAmount,";
                sqlText += " isnull(PurchaseInvoiceHeaders.TotalVATAmount,0)TotalVATAmount,";
                sqlText += " isnull(PurchaseInvoiceHeaders.SerialNo,'N/A')SerialNo,";
                sqlText += " isnull(PurchaseInvoiceHeaders.LCNumber,'N/A')LCNumber,";
                sqlText += " isnull(PurchaseInvoiceHeaders.Comments,'N/A')Comments,";
                sqlText += " PurchaseInvoiceHeaders.ProductType,PurchaseInvoiceHeaders.Post,";
                sqlText += " PurchaseInvoiceHeaders.PurchaseReturnId,";
                sqlText += " PurchaseInvoiceHeaders.WithVDS,";
                sqlText += " convert (varchar,PurchaseInvoiceHeaders.ReceiveDate,120)ReceiveDate,";
                sqlText += " isnull(PurchaseInvoiceHeaders.BENumber,'N/A')BENumber,";

                sqlText += " PurchaseInvoiceHeaders.LCDate,";
                sqlText += " isnull(PurchaseInvoiceHeaders.LandedCost,0)LandedCost";

                sqlText += " FROM PurchaseInvoiceHeaders LEFT OUTER JOIN";
                sqlText += " Vendors ON  PurchaseInvoiceHeaders.VendorID =  Vendors.VendorID LEFT OUTER JOIN";
                sqlText += " VendorGroups ON  Vendors.VendorGroupID =  VendorGroups.VendorGroupID                 ";
                sqlText += " WHERE ";
                sqlText += "     (PurchaseInvoiceHeaders.PurchaseInvoiceNo LIKE '%' +  @PurchaseInvoiceNo + '%' OR @PurchaseInvoiceNo IS NULL) ";
                sqlText += " AND (PurchaseInvoiceHeaders.WithVDS  LIKE '%' + @WithVDS  + '%' OR @WithVDS IS NULL) ";
                sqlText += " AND (Vendors.VendorName LIKE '%' + @VendorName + '%' OR @VendorName IS NULL) ";
                sqlText += " AND (Vendors.VendorGroupID LIKE '%' + @VendorGroupID + '%' OR @VendorGroupID IS NULL)";
                sqlText += " AND (VendorGroups.VendorGroupName LIKE '%' + @VendorGroupName + '%' OR @VendorGroupName IS NULL) ";
                sqlText += " AND (PurchaseInvoiceHeaders.InvoiceDateTime>= @InvoiceDateFrom OR @InvoiceDateFrom IS NULL)";
                sqlText += " AND (PurchaseInvoiceHeaders.InvoiceDateTime<= @InvoiceDateTo OR @InvoiceDateTo IS NULL)";
                sqlText += " AND (PurchaseInvoiceHeaders.SerialNo LIKE '%' + @SerialNo + '%' OR @SerialNo IS NULL) ";
                sqlText += " AND (PurchaseInvoiceHeaders.TransactionType=@T1Type)";
                sqlText += " OR (PurchaseInvoiceHeaders.TransactionType=@T2Type)";
                sqlText += " AND (PurchaseInvoiceHeaders.BENumber LIKE '%' + @BENumber + '%' OR @BENumber IS NULL) ";
                sqlText += " AND (PurchaseInvoiceHeaders.Post LIKE '%' + @Post  + '%' OR @Post IS NULL) ";

                SqlCommand objCommPurchaseHeader = new SqlCommand();
                objCommPurchaseHeader.Connection = currConn;
                objCommPurchaseHeader.CommandText = sqlText;
                objCommPurchaseHeader.CommandType = CommandType.Text;

                #region Parameter

                if (!objCommPurchaseHeader.Parameters.Contains("@PurchaseInvoiceNo"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@PurchaseInvoiceNo", PurchaseInvoiceNo); }
                else { objCommPurchaseHeader.Parameters["@PurchaseInvoiceNo"].Value = PurchaseInvoiceNo; }
                if (!objCommPurchaseHeader.Parameters.Contains("@Post"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@Post", Post); }
                else { objCommPurchaseHeader.Parameters["@Post"].Value = Post; }

                if (!objCommPurchaseHeader.Parameters.Contains("@WithVDS"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@WithVDS", WithVDS); }
                else { objCommPurchaseHeader.Parameters["@WithVDS"].Value = WithVDS; }
                if (!objCommPurchaseHeader.Parameters.Contains("@VendorName"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@VendorName", VendorName); }
                else { objCommPurchaseHeader.Parameters["@VendorName"].Value = VendorName; }
                if (!objCommPurchaseHeader.Parameters.Contains("@VendorGroupID"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@VendorGroupID", VendorGroupID); }
                else { objCommPurchaseHeader.Parameters["@VendorGroupID"].Value = VendorGroupID; }
                if (!objCommPurchaseHeader.Parameters.Contains("@VendorGroupName"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@VendorGroupName", VendorGroupName); }
                else { objCommPurchaseHeader.Parameters["@VendorGroupName"].Value = VendorGroupName; }
                if (InvoiceDateFrom == "")
                {
                    if (!objCommPurchaseHeader.Parameters.Contains("@InvoiceDateFrom"))
                    { objCommPurchaseHeader.Parameters.AddWithValue("@InvoiceDateFrom", System.DBNull.Value); }
                    else { objCommPurchaseHeader.Parameters["@InvoiceDateFrom"].Value = System.DBNull.Value; }
                }
                else
                {

                    if (!objCommPurchaseHeader.Parameters.Contains("@InvoiceDateFrom"))
                    { objCommPurchaseHeader.Parameters.AddWithValue("@InvoiceDateFrom", InvoiceDateFrom); }
                    else { objCommPurchaseHeader.Parameters["@InvoiceDateFrom"].Value = InvoiceDateFrom; }
                }
                if (InvoiceDateTo == "")
                {
                    if (!objCommPurchaseHeader.Parameters.Contains("@InvoiceDateTo"))
                    { objCommPurchaseHeader.Parameters.AddWithValue("@InvoiceDateTo", System.DBNull.Value); }
                    else { objCommPurchaseHeader.Parameters["@InvoiceDateTo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommPurchaseHeader.Parameters.Contains("@InvoiceDateTo"))
                    { objCommPurchaseHeader.Parameters.AddWithValue("@InvoiceDateTo", InvoiceDateTo); }
                    else { objCommPurchaseHeader.Parameters["@InvoiceDateTo"].Value = InvoiceDateTo; }
                }

                if (!objCommPurchaseHeader.Parameters.Contains("@SerialNo"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@SerialNo", SerialNo); }
                else { objCommPurchaseHeader.Parameters["@SerialNo"].Value = SerialNo; }
                if (!objCommPurchaseHeader.Parameters.Contains("@T1Type"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@T1Type", T1Type); }
                else { objCommPurchaseHeader.Parameters["@T1Type"].Value = T2Type; }
                if (!objCommPurchaseHeader.Parameters.Contains("@T2Type"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@T2Type", T2Type); }
                else { objCommPurchaseHeader.Parameters["@T2Type"].Value = T2Type; }
                if (!objCommPurchaseHeader.Parameters.Contains("@BENumber"))
                { objCommPurchaseHeader.Parameters.AddWithValue("@BENumber", BENumber); }
                else { objCommPurchaseHeader.Parameters["@BENumber"].Value = BENumber; }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommPurchaseHeader);
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


        public DataTable SearchPurchaseDetailDTNew(string PurchaseInvoiceNo, string TransactionType)
        {

            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("ProductType");

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
                                    PurchaseInvoiceDetails.PurchaseInvoiceNo,
                                    PurchaseInvoiceDetails.POLineNo,
                                    PurchaseInvoiceDetails.ItemNo, 
                                    isnull(PurchaseInvoiceDetails.Quantity,0)Quantity,
                                    isnull(PurchaseInvoiceDetails.CostPrice,0)CostPrice,
                                    isnull(PurchaseInvoiceDetails.NBRPrice,0)NBRPrice ,
                                    isnull(PurchaseInvoiceDetails.UOM,'N/A')UOM ,
                                    isnull(PurchaseInvoiceDetails.VATRate,0)VATRate,
                                    isnull(PurchaseInvoiceDetails.VATAmount,0)VATAmount,
                                    isnull(PurchaseInvoiceDetails.SubTotal,0)SubTotal,
                                    isnull(PurchaseInvoiceDetails.Comments,'N/A')Comments,
                                    isnull(Products.ProductName,'N/A')ProductName,
                                    isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,
                                    isnull(PurchaseInvoiceDetails.SD,0)SD,
                                    isnull(PurchaseInvoiceDetails.SDAmount,0)SDAmount,
                                    Type,
                                    isnull(Products.ProductCode,'N/A')ProductCode,
                                    isnull(PurchaseInvoiceDetails.UOMQty,0)UOMQty,
                                    isnull(PurchaseInvoiceDetails.UOMn,PurchaseInvoiceDetails.UOM)UOMn,
                                    isnull(PurchaseInvoiceDetails.UOMc,0)UOMc,
                                    isnull(PurchaseInvoiceDetails.UOMPrice,0)UOMPrice,

                                    isnull(PurchaseInvoiceDetails.DollerValue,0)DollerValue,
                                    isnull(PurchaseInvoiceDetails.CurrencyValue,0)CurrencyValue,
                                    isnull(PurchaseInvoiceDetails.RebateRate,0)RebateRate,
                                    isnull(PurchaseInvoiceDetails.RebateAmount,0)RebateAmount,
                                    isnull(PurchaseInvoiceDetails.CnFAmount,0)CnFAmount,
                                    isnull(PurchaseInvoiceDetails.InsuranceAmount,0)InsuranceAmount,
                                    isnull(PurchaseInvoiceDetails.AssessableValue,0)AssessableValue,
                                    isnull(PurchaseInvoiceDetails.CDAmount,0)CDAmount,
                                    isnull(PurchaseInvoiceDetails.RDAmount,0)RDAmount,
                                    isnull(PurchaseInvoiceDetails.TVBAmount,0)TVBAmount,
                                    isnull(PurchaseInvoiceDetails.TVAAmount,0)TVAAmount,
                                    isnull(PurchaseInvoiceDetails.ATVAmount,0)ATVAmount,
                                    isnull(PurchaseInvoiceDetails.OthersAmount,0)OthersAmount,
                                    isnull(PurchaseInvoiceDetails.ReturnTransactionType,'')ReturnTransactionType
                                    FROM dbo.PurchaseInvoiceDetails  
                                    left outer join
                                    Products on PurchaseInvoiceDetails.ItemNo=Products.ItemNo 
                                    WHERE ";
                sqlText += @"   (PurchaseInvoiceDetails.PurchaseInvoiceNo = '" + PurchaseInvoiceNo + "')";
                sqlText += @"   and (PurchaseInvoiceDetails.TransactionType ='" + TransactionType + "')";
                if (TransactionType == "TollReceive")
                {
                    sqlText += @"   and (PurchaseInvoiceDetails.ItemNo in (select distinct itemno from products 
                    where categoryid in (select distinct categoryid from ProductCategories where israw='Overhead')))";

                }
                sqlText += @"  order by PurchaseInvoiceDetails.ItemNo";


                SqlCommand objCommPurchaseDetail = new SqlCommand();
                objCommPurchaseDetail.Connection = currConn;
                objCommPurchaseDetail.CommandText = sqlText;
                objCommPurchaseDetail.CommandType = CommandType.Text;


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommPurchaseDetail);
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

        public DataTable SearchPurchaseDutyDTNew(string PurchaseInvoiceNo)
        {

            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Duty");

            #endregion

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "ItemNo", "varchar(20)", currConn);
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "DutyCompleteQuantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "DutyCompleteQuantityPercent", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "LineCost", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "UnitCost", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "Quantity", "decimal(25, 9)", currConn);
                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT isnull(pid.ItemNo,'0')ItemNo,
isnull(PID.CnFInp,0)CnFInp,
 isnull(PID.CnFRate,0)CnFRate,
isnull( PID.CnFAmount,0)CnFAmount, isnull(PID.InsuranceInp,0)InsuranceInp,
isnull(PID.InsuranceRate,0)InsuranceRate, isnull(PID.InsuranceAmount,0)InsuranceAmount,
       isnull(PID.AssessableInp,0)AssessableInp, isnull(PID.AssessableValue,0)AssessableValue, isnull(PID.CDInp,0)CDInp, 
       isnull(PID.CDRate,0)CDRate, isnull(PID.CDAmount,0)CDAmount, isnull(PID.RDInp,0)RDInp, isnull(PID.RDRate,0)RDRate,
       isnull(PID.RDAmount,0)RDAmount, isnull(PID.TVBInp,0)TVBInp, isnull(PID.TVBRate,0)TVBRate, isnull(PID.TVBAmount,0)TVBAmount,
       isnull(PID.SDInp,0)SDInp, isnull(PID.SD,0)SD, isnull(PID.SDAmount,0)SDAmount, isnull(PID.VATInp,0)VATInp, isnull(PID.VATRate,0)VATRate,
       isnull(PID.VATAmount,0)VATAmount, isnull(PID.TVAInp,0)TVAInp, isnull(PID.TVARate,0)TVARate, isnull(PID.TVAAmount,0)TVAAmount,
        isnull(PID.ATVInp,0)ATVInp, isnull(PID.ATVRate,0)ATVRate, isnull(PID.ATVAmount,0)ATVAmount,
       isnull(PID.OthersInp,0)OthersInp, isnull(PID.OthersRate,0)OthersRate,isnull(PID.OthersAmount,0)OthersAmount,
        ISNULL(NULLIF(Remarks,''),'NA')Remarks,isnull(Products.ProductCode,'N/A')ProductCode,
isnull(Products.ProductName,'N/A')ProductName
  FROM PurchaseInvoiceDuties PID left outer JOIN
  Products on isnull(PID.ItemNo,0)=Products.ItemNo                                     WHERE 
                                    (PID.PurchaseInvoiceNo = @PurchaseInvoiceNo)

order by ItemNo
";

                SqlCommand objCommPurchaseDetail = new SqlCommand();
                objCommPurchaseDetail.Connection = currConn;
                objCommPurchaseDetail.CommandText = sqlText;
                objCommPurchaseDetail.CommandType = CommandType.Text;

                if (!objCommPurchaseDetail.Parameters.Contains("@PurchaseInvoiceNo"))
                { objCommPurchaseDetail.Parameters.AddWithValue("@PurchaseInvoiceNo", PurchaseInvoiceNo); }
                else { objCommPurchaseDetail.Parameters["@PurchaseInvoiceNo"].Value = PurchaseInvoiceNo; }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommPurchaseDetail);
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

        public DataTable SearchPurchaseDutyDTDownload(string PurchaseInvoiceNo)
        {

            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Duty");

            #endregion

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT 
PID.PurchaseInvoiceNo ID
,PIH.InvoiceDateTime
,isnull(Products.ProductCode,'N/A')Item_Code
,isnull(Products.ProductName,'N/A')Item_Name
,isnull( PID.CnFAmount,0)CnF_Amount 
, isnull(PID.InsuranceAmount,0)Insurance_Amount
, isnull(PID.AssessableValue,0)Assessable_Value
, isnull(PID.CDAmount,0)CD_Amount
,isnull(PID.RDAmount,0)RD_Amount
, isnull(PID.TVBAmount,0)TVB_Amount
, isnull(PID.SDAmount,0)SD_Amount
,isnull(PID.VATAmount,0)VAT_Amount
, isnull(PID.TVAAmount,0)TVA_Amount
, isnull(PID.ATVAmount,0)ATV_Amount
,isnull(PID.OthersAmount,0)Others_Amount
,isnull(PId.AssessableValue,0)Total_Price
,isnull(PID.Quantity,0)Quantity
,ISNULL(NULLIF(Remarks,''),'NA')Remarks
FROM PurchaseInvoiceDuties PID left outer JOIN
  PurchaseInvoiceHeaders PIH  on PID.PurchaseInvoiceNo=pih.PurchaseInvoiceNo left outer JOIN
Products on isnull(PID.ItemNo,0)=Products.ItemNo                                 

WHERE  (PID.PurchaseInvoiceNo = @PurchaseInvoiceNo)
";

                SqlCommand objCommPurchaseDetail = new SqlCommand();
                objCommPurchaseDetail.Connection = currConn;
                objCommPurchaseDetail.CommandText = sqlText;
                objCommPurchaseDetail.CommandType = CommandType.Text;

                if (!objCommPurchaseDetail.Parameters.Contains("@PurchaseInvoiceNo"))
                { objCommPurchaseDetail.Parameters.AddWithValue("@PurchaseInvoiceNo", PurchaseInvoiceNo); }
                else { objCommPurchaseDetail.Parameters["@PurchaseInvoiceNo"].Value = PurchaseInvoiceNo; }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommPurchaseDetail);
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

        private void SetDefaultValue(PurchaseMasterVM vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Comments))
            {
                vm.Comments = "-";
            }

            if (string.IsNullOrWhiteSpace(vm.SerialNo))
            {
                vm.SerialNo = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.LCNumber))
            {
                vm.LCNumber = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.BENumber))
            {
                vm.BENumber = "-";
            }
        }


        public string[] PurchaseInsert(PurchaseMasterVM Master, List<PurchaseDetailVM> Details, List<PurchaseDutiesVM> Duties,
        List<TrackingVM> Trackings, SqlTransaction transaction, SqlConnection currConn)
        {

            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            //SqlConnection currConn = null;
            //SqlTransaction transaction = null;
            SqlConnection vcurrConn = currConn;
            if (vcurrConn == null)
            {
                currConn = null;
                transaction = null;
            }
            int transResult = 0;
            string sqlText = "";
            string newID = "";
            string PostStatus = "";
            int IDExist = 0;
            ProductDAL productDal = new ProductDAL();


            int nextId = 0;
            #endregion Initializ

            #region Try
            try
            {
                SetDefaultValue(Master);
                #region Validation for Header


                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.PurchasemsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.ReceiveDate) < DateTime.MinValue || Convert.ToDateTime(Master.ReceiveDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, "Please Check Invoice Data and Time");

                }



                #endregion Validation for Header
                #region open connection and transaction
                if (vcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                    transaction = currConn.BeginTransaction(MessageVM.PurchasemsgMethodNameInsert);

                }

                CommonDAL commonDal = new CommonDAL();



                bool TollReceiveWithIssue = Convert.ToBoolean(commonDal.settings("TollReceive", "WithIssue") == "Y" ? true : false);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.ReceiveDate;
                string transactionYearCheck = Convert.ToDateTime(Master.ReceiveDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock
                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK

                #region Find Transaction Exist

                sqlText = "";
                sqlText = sqlText + "select COUNT(PurchaseInvoiceNo) from PurchaseInvoiceHeaders WHERE PurchaseInvoiceNo=@MasterPurchaseInvoiceNo ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                cmdExistTran.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo ?? Convert.DBNull);
                //IDExist = (int)cmdExistTran.ExecuteScalar();
                object objIDExist = cmdExistTran.ExecuteScalar();
                if (objIDExist != null)
                {
                    IDExist = Convert.ToInt32(objIDExist);
                }

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.PurchasemsgFindExistID);
                }

                #endregion Find Transaction Exist

                #region Purchase ID Create
                if (string.IsNullOrEmpty(Master.TransactionType))
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                }
                if (Master.TransactionType == "Other")
                {
                    newID = commonDal.TransactionCode("Purchase", "Other", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                              "ReceiveDate", Master.ReceiveDate, currConn, transaction);
                }

                else if (Master.TransactionType == "PurchaseDN")
                {
                    newID = commonDal.TransactionCode("Purchase", "PurchaseDN", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                              "ReceiveDate", Master.ReceiveDate, currConn, transaction);
                }
                else if (Master.TransactionType == "PurchaseCN")
                {
                    newID = commonDal.TransactionCode("Purchase", "PurchaseCN", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                              "ReceiveDate", Master.ReceiveDate, currConn, transaction);
                }

                else if (Master.TransactionType == "Trading") //s
                {
                    newID = commonDal.TransactionCode("Purchase", "Trading", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                              "ReceiveDate", Master.ReceiveDate, currConn, transaction);
                }

                else if (Master.TransactionType == "TollReceive")
                {
                    newID = commonDal.TransactionCode("TollReceive", "TollReceive", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                             "ReceiveDate", Master.ReceiveDate, currConn, transaction);
                }
                else if (Master.TransactionType == "Import"
                    || Master.TransactionType == "ServiceImport"
                    || Master.TransactionType == "ServiceNSImport"
                    || Master.TransactionType == "TradingImport"
                    || Master.TransactionType == "InputServiceImport")
                {
                    newID = commonDal.TransactionCode("Purchase", "Import", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                              "ReceiveDate", Master.ReceiveDate, currConn, transaction);
                }
                else if (Master.TransactionType == "InputService")
                {
                    newID = commonDal.TransactionCode("Purchase", "InputService", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                              "ReceiveDate", Master.ReceiveDate, currConn, transaction);
                }
                else if (Master.TransactionType == "TollReceiveRaw")
                {
                    newID = commonDal.TransactionCode("TollReceiveRaw", "TollReceiveRaw", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                              "ReceiveDate", Master.ReceiveDate, currConn, transaction);
                }
                else if (Master.TransactionType == "PurchaseReturn")
                {
                    newID = commonDal.TransactionCode("Purchase", "PurchaseReturn", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                              "ReceiveDate", Master.ReceiveDate, currConn, transaction);
                }
                else if (Master.TransactionType == "Service")
                {
                    newID = commonDal.TransactionCode("Purchase", "Service", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                              "ReceiveDate", Master.ReceiveDate, currConn, transaction);
                }
                else if (Master.TransactionType == "ServiceNS")
                {
                    newID = commonDal.TransactionCode("Purchase", "ServiceNS", "PurchaseInvoiceHeaders", "PurchaseInvoiceNo",
                                              "ReceiveDate", Master.ReceiveDate, currConn, transaction);
                }
                if (string.IsNullOrEmpty(newID) || newID == "")
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            "ID Prefetch not set please update Prefetch first");
                }
                // newID = newID + "-W";
                #endregion Purchase ID Create

                #region ID generated completed,Insert new Information in Header
                var Id = _cDal.NextId("PurchaseInvoiceHeaders", currConn, transaction).ToString();

                sqlText = "";
                sqlText += " insert into PurchaseInvoiceHeaders";
                sqlText += " (";
                //////sqlText += " Id,";
                sqlText += " PurchaseInvoiceNo,";
                sqlText += " VendorID,";
                sqlText += " CustomHouse,";
                sqlText += " InvoiceDateTime,";
                sqlText += " TotalAmount,";
                sqlText += " TotalVATAmount,";
                sqlText += " SerialNo,";
                sqlText += " LCNumber,";
                sqlText += " Comments,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " BENumber,";
                sqlText += " ProductType,";
                sqlText += " transactionType,";
                sqlText += " ReceiveDate,";
                sqlText += " WithVDS,";
                sqlText += " PurchaseReturnId,";
                sqlText += " ImportIDExcel,";

                sqlText += " Post,";
                sqlText += " LCDate,";
                sqlText += " LandedCost";
                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                //////sqlText += "@Id,";
                sqlText += "@newID,";
                sqlText += "@MasterVendorID,";
                sqlText += "@MasterCustomHouse,";
                sqlText += "@MasterInvoiceDate,";
                sqlText += "@MasterTotalAmount,";
                sqlText += "@MasterTotalVATAmount,";
                sqlText += "@MasterSerialNo,";
                sqlText += "@MasterLCNumber,";
                sqlText += "@MasterComments,";
                sqlText += "@MasterCreatedBy,";
                sqlText += "@MasterCreatedOn,";
                sqlText += "@MasterBENumber,";
                sqlText += "@MasterProductType,";
                sqlText += "@MasterTransactionType,";
                sqlText += "@MasterReceiveDate,";
                sqlText += "@MasterWithVDS,";
                sqlText += "@MasterReturnId,";
                sqlText += "@MasterImportID,";
                sqlText += "@MasterPost,";
                sqlText += "@MasterLCDate,";
                sqlText += "@MasterLandedCost";
                sqlText += ") SELECT SCOPE_IDENTITY()";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                //////cmdInsert.Parameters.AddWithValue("@Id", Id);
                cmdInsert.Parameters.AddWithValue("@newID", newID);
                cmdInsert.Parameters.AddWithValue("@MasterVendorID", Master.VendorID);
                cmdInsert.Parameters.AddWithValue("@MasterCustomHouse", Master.CustomHouse ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterInvoiceDate", Master.InvoiceDate);
                cmdInsert.Parameters.AddWithValue("@MasterTotalAmount", Master.TotalAmount);
                cmdInsert.Parameters.AddWithValue("@MasterTotalVATAmount", Master.TotalVATAmount);
                cmdInsert.Parameters.AddWithValue("@MasterSerialNo", Master.SerialNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterLCNumber", Master.LCNumber ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@MasterBENumber", Master.BENumber ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterProductType", Master.ProductType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterReceiveDate", Master.ReceiveDate);
                cmdInsert.Parameters.AddWithValue("@MasterWithVDS", Master.WithVDS);
                cmdInsert.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterImportID", Master.ImportID ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterLCDate", Master.LCDate);
                cmdInsert.Parameters.AddWithValue("@MasterLandedCost", Master.LandedCost);

                var exec = cmdInsert.ExecuteScalar();

                transResult = Convert.ToInt32(exec);
                Master.Id = transResult.ToString();

                //////transResult = (int)cmdInsert.ExecuteNonQuery();
                //////Master.Id = Id.ToString();


                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    MessageVM.PurchasemsgSaveNotSuccessfully);
                }


                #endregion ID generated completed,Insert new Information in Header


                #region if Transection not Other Insert Issue / Receive

                #region Purchase For TollReceive

                if (Master.TransactionType == "TollReceive")
                {

                    // not Complete Plz check again
                    #region Insert to Issue Header

                    sqlText = "";
                    sqlText += " insert into IssueHeaders(";
                    //sqlText += " IssueNo,";

                    sqlText += " IssueNo,";
                    sqlText += " IssueDateTime,";
                    sqlText += " TotalVATAmount,";
                    sqlText += " TotalAmount,";
                    sqlText += " SerialNo,";
                    sqlText += " Comments,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " ReceiveNo,";
                    sqlText += " transactionType,";
                    sqlText += " IssueReturnId,";
                    sqlText += " Post";

                    //sqlText += " Post";
                    sqlText += " )";
                    sqlText += " values(	";
                    //sqlText += "'" + Master.PurchaseInvoiceNo + "',";

                    sqlText += "@newID,";
                    sqlText += "@MasterReceiveDate,";
                    sqlText += " 0 ,";
                    sqlText += "0,";
                    sqlText += "@newID,";
                    sqlText += "@MasterComments,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@newID,";
                    sqlText += "@MasterTransactionType,";
                    sqlText += "@MasterReturnId,";
                    sqlText += "@MasterPost";

                    //sqlText += "'" + Master.@Post + "'";
                    sqlText += ")	";

                    SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                    cmdInsertIssue.Transaction = transaction;

                    cmdInsertIssue.Parameters.AddWithValue("@newID", newID);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterReceiveDate", Master.ReceiveDate ?? Convert.DBNull);
                    //////cmdInsertIssue.Parameters.AddWithValue("@newID", newID);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                    ////cmdInsertIssue.Parameters.AddWithValue("@newID", newID);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

                    transResult = (int)cmdInsertIssue.ExecuteNonQuery();
                    if (transResult <= 0)
                    {

                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.PurchasemsgUnableToSaveIssue);
                    }

                    #endregion Insert to Issue Header

                    #region Insert to ReceiveHeaders

                    sqlText = "";
                    sqlText += " insert into ReceiveHeaders(";
                    //sqlText += " IssueNo,";

                    sqlText += " ReceiveNo,";
                    sqlText += " ReceiveDateTime,";
                    sqlText += " TotalAmount,";
                    sqlText += " TotalVATAmount,";
                    sqlText += " SerialNo,";
                    sqlText += " Comments,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " transactionType,";
                    sqlText += " ReceiveReturnId,";
                    sqlText += " Post";

                    //sqlText += " Post";
                    sqlText += " )";
                    sqlText += " values(";
                    //sqlText += "'" + Master.PurchaseInvoiceNo + "',";

                    sqlText += "@newID,";
                    sqlText += "@MasterReceiveDate,";
                    sqlText += "0,";
                    sqlText += "0,";
                    sqlText += "@newID,";
                    sqlText += "@MasterComments,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@MasterTransactionType,";
                    sqlText += "@MasterReturnId,";
                    sqlText += "@MasterPost";

                    //sqlText += "'" + Master.@Post + "'";
                    sqlText += ")	";

                    SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                    cmdInsertReceive.Transaction = transaction;

                    cmdInsertReceive.Parameters.AddWithValue("@newID", newID);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterReceiveDate", Master.ReceiveDate);
                    //////cmdInsert.Parameters.AddWithValue("@newID", newID);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

                    transResult = (int)cmdInsertReceive.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.PurchasemsgUnableToSaveReceive);
                    }

                    #endregion Insert to Receive Header

                }

                #endregion Purchase ID Create For IssueReturn

                #region Purchase For Input Service

                if (Master.TransactionType == "InputService"
                    || Master.TransactionType == "InputServiceImport")
                {
                    #region Insert to Issue Header

                    sqlText = "";
                    sqlText += " insert into IssueHeaders(";

                    sqlText += " IssueNo,";
                    sqlText += " IssueDateTime,";
                    sqlText += " TotalVATAmount,";
                    sqlText += " TotalAmount,";
                    sqlText += " SerialNo,";
                    sqlText += " Comments,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " ReceiveNo,";
                    sqlText += " transactionType,";
                    sqlText += " IssueReturnId,";
                    sqlText += " Post";

                    sqlText += " )";
                    sqlText += " values(	";

                    sqlText += "@newID,";
                    sqlText += "@MasterReceiveDate,";
                    sqlText += " 0 ,";
                    sqlText += "0,";
                    sqlText += "@newID,";
                    sqlText += "@MasterComments,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@newID,";
                    sqlText += "@MasterTransactionType,";
                    sqlText += "@MasterReturnId,";
                    sqlText += "@MasterPost";

                    sqlText += ")	";

                    SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                    cmdInsertIssue.Transaction = transaction;

                    cmdInsertIssue.Parameters.AddWithValue("@newID", newID);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterReceiveDate", Master.ReceiveDate);
                    //////cmdInsertIssue.Parameters.AddWithValue("@newID", newID);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                    //////cmdInsertIssue.Parameters.AddWithValue("@newID", newID);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

                    transResult = (int)cmdInsertIssue.ExecuteNonQuery();
                    if (transResult <= 0)
                    {

                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.PurchasemsgUnableToSaveIssue);
                    }

                    #endregion Insert to Issue Header


                }
                #endregion Purchase For Input Service


                #region Import

                if (Master.TransactionType == "Import"
                || Master.TransactionType == "ServiceImport"
                || Master.TransactionType == "ServiceNSImport"
                || Master.TransactionType == "TradingImport"
                || Master.TransactionType == "InputServiceImport")
                {
                    if (Duties == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.PurchasemsgNoDataToSaveImportDuties);
                    }

                    #region DutyID

                    sqlText = "select isnull(max(cast(PIDutyID as int)),0)+1 FROM  PurchaseInvoiceDuties";
                    SqlCommand cmdDutyNextId = new SqlCommand(sqlText, currConn);
                    cmdDutyNextId.Transaction = transaction;
                    //int nextIdD = (int) cmdDutyNextId.ExecuteScalar();
                    int nextIdD = 0;
                    object objnextIdD = cmdDutyNextId.ExecuteScalar();
                    if (objnextIdD == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                            MessageVM.PurchasemsgDutyIdNotCreate);
                    }
                    else
                    {
                        nextIdD = Convert.ToInt32(objnextIdD);
                    }

                    if (nextIdD <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.PurchasemsgDutyIdNotCreate);
                    }

                    #endregion DutyID

                    #region ID generated completed,Insert new PurchaseInvoiceDuties
                    foreach (var duty in Duties.ToList())
                    {
                        sqlText = "";
                        sqlText += " insert into PurchaseInvoiceDuties";
                        sqlText += " (";
                        sqlText += " PIDutyID,";
                        sqlText += " PurchaseInvoiceNo,";

                        sqlText += " ItemNo,";
                        sqlText += " Quantity,";
                        sqlText += " DutyCompleteQuantity,";
                        sqlText += " DutyCompleteQuantityPercent,";
                        sqlText += " CnFInp,";
                        sqlText += " CnFRate,";
                        sqlText += " CnFAmount,";
                        sqlText += " InsuranceInp,";
                        sqlText += " InsuranceRate,";
                        sqlText += " InsuranceAmount,";
                        sqlText += " AssessableInp,";
                        sqlText += " AssessableValue,";
                        sqlText += " CDInp,";
                        sqlText += " CDRate,";
                        sqlText += " CDAmount,";
                        sqlText += " RDInp,";
                        sqlText += " RDRate,";
                        sqlText += " RDAmount,";
                        sqlText += " TVBInp,";
                        sqlText += " TVBRate,";
                        sqlText += " TVBAmount,";
                        sqlText += " SDInp,";
                        sqlText += " SD,";
                        sqlText += " SDAmount,";
                        sqlText += " VATInp,";
                        sqlText += " VATRate,";
                        sqlText += " VATAmount,";
                        sqlText += " TVAInp,";
                        sqlText += " TVARate,";
                        sqlText += " TVAAmount,";
                        sqlText += " ATVInp,";
                        sqlText += " ATVRate,";
                        sqlText += " ATVAmount,";
                        sqlText += " OthersInp,";
                        sqlText += " OthersRate,";
                        sqlText += " OthersAmount,";
                        sqlText += " UnitCost,";
                        sqlText += " LineCost,";
                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn,";
                        sqlText += " TransactionType,";
                        sqlText += " Post,";
                        sqlText += " Remarks";
                        sqlText += " )";
                        sqlText += " values";
                        sqlText += " (";
                        sqlText += "@nextIdD,";
                        sqlText += "@newID,";
                        sqlText += "@dutyItemNo,";
                        sqlText += "@dutyQuantity,";
                        sqlText += "@dutyDutyCompleteQuantity,";
                        sqlText += "@dutyDutyCompleteQuantityPercent,";
                        sqlText += "@dutyCnFInp,";
                        sqlText += "@dutyCnFRate,";
                        sqlText += "@dutyCnFAmount,";
                        sqlText += "@dutyInsuranceInp,";
                        sqlText += "@dutyInsuranceRate,";
                        sqlText += "@dutyInsuranceAmount,";
                        sqlText += "@dutyAssessableInp,";
                        sqlText += "@dutyAssessableValue,";
                        sqlText += "@dutyCDInp,";
                        sqlText += "@dutyCDRate,";
                        sqlText += "@dutyCDAmount,";
                        sqlText += "@dutyRDInp,";
                        sqlText += "@dutyRDRate,";
                        sqlText += "@dutyRDAmount,";
                        sqlText += "@dutyTVBInp,";
                        sqlText += "@dutyTVBRate,";
                        sqlText += "@dutyTVBAmount,";
                        sqlText += "@dutySDInp,";
                        sqlText += "@dutySD,";
                        sqlText += "@dutySDAmount,";
                        sqlText += "@dutyVATInp,";
                        sqlText += "@dutyVATRate,";
                        sqlText += "@dutyVATAmount,";
                        sqlText += "@dutyTVAInp,";
                        sqlText += "@dutyTVARate,";
                        sqlText += "@dutyTVAAmount,";
                        sqlText += "@dutyATVInp,";
                        sqlText += "@dutyATVRate,";
                        sqlText += "@dutyATVAmount,";
                        sqlText += "@dutyOthersInp,";
                        sqlText += "@dutyOthersRate,";
                        sqlText += "@dutyOthersAmount,";
                        sqlText += "@dutyUnitCost,";
                        sqlText += "@dutyLineCost,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterPost,";
                        sqlText += "@dutyRemarks";
                        sqlText += ")";


                        SqlCommand cmdInsertDuty = new SqlCommand(sqlText, currConn);
                        cmdInsertDuty.Transaction = transaction;

                        cmdInsertDuty.Parameters.AddWithValue("@nextIdD", nextIdD);
                        cmdInsertDuty.Parameters.AddWithValue("@newID", newID ?? Convert.DBNull);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyItemNo", duty.ItemNo ?? Convert.DBNull);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyQuantity", duty.Quantity);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyDutyCompleteQuantity", duty.DutyCompleteQuantity);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyDutyCompleteQuantityPercent", duty.DutyCompleteQuantityPercent);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyCnFInp", duty.CnFInp);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyCnFRate", duty.CnFRate);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyCnFAmount", duty.CnFAmount);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyInsuranceInp", duty.InsuranceInp);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyInsuranceRate", duty.InsuranceRate);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyInsuranceAmount", duty.InsuranceAmount);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyAssessableInp", duty.AssessableInp);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyAssessableValue", duty.AssessableValue);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyCDInp", duty.CDInp);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyCDRate", duty.CDRate);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyCDAmount", duty.CDAmount);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyRDInp", duty.RDInp);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyRDRate", duty.RDRate);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyRDAmount", duty.RDAmount);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyTVBInp", duty.TVBInp);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyTVBRate", duty.TVBRate);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyTVBAmount", duty.TVBAmount);
                        cmdInsertDuty.Parameters.AddWithValue("@dutySDInp", duty.SDInp);
                        cmdInsertDuty.Parameters.AddWithValue("@dutySD", duty.SD);
                        cmdInsertDuty.Parameters.AddWithValue("@dutySDAmount", duty.SDAmount);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyVATInp", duty.VATInp);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyVATRate", duty.VATRate);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyVATAmount", duty.VATAmount);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyTVAInp", duty.TVAInp);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyTVARate", duty.TVARate);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyTVAAmount", duty.TVAAmount);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyATVInp", duty.ATVInp);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyATVRate", duty.ATVRate);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyATVAmount", duty.ATVAmount);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyOthersInp", duty.OthersInp);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyOthersRate", duty.OthersRate);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyOthersAmount", duty.OthersAmount);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyUnitCost", duty.UnitCost);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyLineCost", duty.LineCost);
                        cmdInsertDuty.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsertDuty.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdInsertDuty.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsertDuty.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                        cmdInsertDuty.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsertDuty.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                        cmdInsertDuty.Parameters.AddWithValue("@dutyRemarks", duty.Remarks ?? Convert.DBNull);

                        transResult = (int)cmdInsertDuty.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.PurchasemsgSaveNotSuccessfully);
                        }

                    }

                    #endregion ID generated completed,Insert new Information in Header
                }

                #endregion Importm
                #endregion if Transection not Other Insert Issue /Receive

                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail

                if (Details == null || Details.Count() == 0)
                {
                    retResults[1] = "Please Add Details!";
                    throw new ArgumentNullException(retResults[1], "");
                }


                #endregion Validation for Detail

                #region Insert Detail Table
                var lineNo = 1;
                foreach (var Item in Details.ToList())
                {

                    Item.LineNo = lineNo.ToString();
                    lineNo++;
                    //Item.Type = Master.IsImport ? "Import-" + Item.Type : "Local-" + Item.Type;
                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText +=
                        "select COUNT(PurchaseInvoiceNo) from PurchaseInvoiceDetails WHERE PurchaseInvoiceNo=@newID ";
                    sqlText += " AND ItemNo=@ItemItemNo";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    //IDExist = (int) cmdFindId.ExecuteScalar();

                    cmdFindId.Parameters.AddWithValue("@newID", newID);
                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                    objIDExist = cmdFindId.ExecuteScalar();
                    if (objIDExist != null)
                    {
                        IDExist = Convert.ToInt32(objIDExist);
                    }

                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.PurchasemsgFindExistID);
                    }

                    #endregion Find Transaction Exist

                    #region Insert only DetailTable PurchaseInvoiceDetails

                    sqlText = "";
                    sqlText += " insert into PurchaseInvoiceDetails(";
                    sqlText += " PurchaseInvoiceNo,";
                    sqlText += " POLineNo,";
                    sqlText += " ItemNo,";
                    sqlText += " Quantity,";
                    sqlText += " CostPrice,";
                    sqlText += " NBRPrice,";
                    sqlText += " UOM,";
                    sqlText += " VATRate,";
                    sqlText += " VATAmount,";
                    sqlText += " SubTotal,";
                    sqlText += " Comments,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " SD,";
                    sqlText += " SDAmount,";
                    sqlText += " Type,";
                    sqlText += " ProductType,";
                    sqlText += " BENumber,";
                    sqlText += " InvoiceDateTime,";
                    sqlText += " ReceiveDate,";
                    sqlText += " Post,";
                    sqlText += " UOMQty,";
                    sqlText += " UOMPrice,";
                    sqlText += " UOMc,";
                    sqlText += " UOMn,";
                    sqlText += " RebateRate ,";
                    sqlText += " RebateAmount ,";
                    sqlText += " CnFAmount ,";
                    sqlText += " InsuranceAmount ,";
                    sqlText += " AssessableValue ,";
                    sqlText += " CDAmount ,";
                    sqlText += " RDAmount ,";
                    sqlText += " TVBAmount ,";
                    sqlText += " TVAAmount ,";
                    sqlText += " ATVAmount ,";
                    sqlText += " TransactionType ,";
                    sqlText += " PurchaseReturnId ,";
                    sqlText += " OthersAmount ";
                    if (Master.TransactionType == "PurchaseCN" || Master.TransactionType == "PurchaseDN" || Master.TransactionType == "PurchaseReturn")
                    {
                        sqlText += ", ReturnTransactionType ";
                    }
                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "@newID,";
                    sqlText += "@ItemLineNo,";
                    sqlText += "@ItemItemNo,";
                    sqlText += "@ItemQuantity,";
                    sqlText += "@ItemUnitPrice,";
                    sqlText += "@ItemNBRPrice,";
                    sqlText += "@ItemUOM,";
                    sqlText += "@ItemVATRate,";
                    sqlText += "@ItemVATAmount,";
                    sqlText += "@ItemSubTotal,";
                    sqlText += "@ItemComments,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@ItemSD,";
                    sqlText += "@ItemSDAmount,";
                    sqlText += "@ItemType,";
                    sqlText += "@ItemProductType,";
                    sqlText += "@ItemBENumber,";
                    sqlText += "@MasterInvoiceDate,";
                    sqlText += "@MasterReceiveDate,";
                    sqlText += "@MasterPost,";
                    sqlText += "@ItemUOMQty,";
                    sqlText += "@ItemUOMPrice,";
                    sqlText += "@ItemUOMc,";
                    sqlText += "@ItemUOMn,";
                    sqlText += "@ItemRebateRate,";
                    sqlText += "@ItemRebateAmount,";
                    sqlText += "@ItemCnFAmount,";
                    sqlText += "@ItemInsuranceAmount,";
                    sqlText += "@ItemAssessableValue,";
                    sqlText += "@ItemCDAmount,";
                    sqlText += "@ItemRDAmount,";
                    sqlText += "@ItemTVBAmount,";
                    sqlText += "@ItemTVAAmount,";
                    sqlText += "@ItemATVAmount,";
                    sqlText += "@MasterTransactionType,";
                    sqlText += "@MasterReturnId,";
                    sqlText += "@ItemOthersAmount";
                    if (Master.TransactionType == "PurchaseCN" || Master.TransactionType == "PurchaseDN" || Master.TransactionType == "PurchaseReturn")
                    {
                        sqlText += ",@ItemReturnTransactionType";
                    }

                    //sqlText += "'" + Master.@Post + "'";
                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;

                    cmdInsDetail.Parameters.AddWithValue("@newID", newID);
                    cmdInsDetail.Parameters.AddWithValue("@ItemLineNo", Item.LineNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUnitPrice", Item.UnitPrice);
                    cmdInsDetail.Parameters.AddWithValue("@ItemNBRPrice", Item.NBRPrice);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemVATRate", Item.VATRate);
                    cmdInsDetail.Parameters.AddWithValue("@ItemVATAmount", Item.VATAmount);
                    cmdInsDetail.Parameters.AddWithValue("@ItemSubTotal", Item.SubTotal);
                    cmdInsDetail.Parameters.AddWithValue("@ItemComments", Item.Comments ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemSD", Item.SD);
                    cmdInsDetail.Parameters.AddWithValue("@ItemSDAmount", Item.SDAmount);
                    cmdInsDetail.Parameters.AddWithValue("@ItemType", Item.Type ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemProductType", Item.ProductType ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemBENumber", Item.BENumber ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterInvoiceDate", Master.InvoiceDate ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterReceiveDate", Master.ReceiveDate ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemRebateRate", Item.RebateRate);
                    cmdInsDetail.Parameters.AddWithValue("@ItemRebateAmount", Item.RebateAmount);
                    cmdInsDetail.Parameters.AddWithValue("@ItemCnFAmount", Item.CnFAmount);
                    cmdInsDetail.Parameters.AddWithValue("@ItemInsuranceAmount", Item.InsuranceAmount);
                    cmdInsDetail.Parameters.AddWithValue("@ItemAssessableValue", Item.AssessableValue);
                    cmdInsDetail.Parameters.AddWithValue("@ItemCDAmount", Item.CDAmount);
                    cmdInsDetail.Parameters.AddWithValue("@ItemRDAmount", Item.RDAmount);
                    cmdInsDetail.Parameters.AddWithValue("@ItemTVBAmount", Item.TVBAmount);
                    cmdInsDetail.Parameters.AddWithValue("@ItemTVAAmount", Item.TVAAmount);
                    cmdInsDetail.Parameters.AddWithValue("@ItemATVAmount", Item.ATVAmount);
                    cmdInsDetail.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemOthersAmount", Item.OthersAmount);
                    cmdInsDetail.Parameters.AddWithValue("@ItemReturnTransactionType", Item.ReturnTransactionType ?? Convert.DBNull);

                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.PurchasemsgSaveNotSuccessfully);
                    }

                    #endregion Insert only DetailTable

                    #region Insert Issue and Receive if Transaction is not Other

                    #region Find AVG Rate

                    //decimal AvgRate = productDal.AvgPrice(Item.ItemNo, Master.ReceiveDate, currConn, transaction);

                    #endregion Find AVG Rate

                    #region Transaction is TollReceive


                    if (Master.TransactionType == "TollReceive")
                    {

                        ProductDAL bdal1 = new ProductDAL();
                        var BOMId = bdal1.GetBOMIdFromOH(Item.ItemNo, "VAT 1 (Toll Receive)", Master.ReceiveDate, currConn, transaction);
                        //sss
                        sqlText = "";
                        sqlText +=
                            " SELECT  b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ," +
                            "b.WastageQuantity,b.UOM,b.UOMUQty,b.UOMWQty,b.TransactionType from BOMRaws b  ";
                        sqlText += " where ";
                        sqlText += " BOMId='" + BOMId + "' ";
                        //sqlText += "   and (rawitemtype='raw' or rawitemtype='pack' or rawitemtype='finish') ";
                        sqlText += "   and (rawitemtype='raw' or rawitemtype='pack') ";



                        DataTable dataTable = new DataTable("RIFB");
                        SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                        cmdRIFB.Transaction = transaction;
                        SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                        reportDataAdapt.Fill(dataTable);

                        if (dataTable == null)
                        {
                            if (TollReceiveWithIssue)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                           MessageVM.receiveMsgNoDataToPost);
                            }

                        }
                        else if (dataTable.Rows.Count <= 0)
                        {
                            if (TollReceiveWithIssue)
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                            "There is no Item for Auto Consumption for the Item Name (VAT 1 (Toll Receive)) in price declaration.");
                        }
                        else
                        {
                            decimal vQuantity = 0;
                            decimal vWastage = 0;
                            decimal vStock = 0;
                            string rwUom = "";
                            decimal vConvertionRate = 0;
                            decimal AvgRate = 0;

                            foreach (DataRow BRItem in dataTable.Rows)
                            {
                                #region Declare

                                decimal v1Quantity = 0;
                                string v1RawItemNo = "";
                                decimal v1CostPrice = 0;
                                string v1UOM = "";
                                decimal v1SubTotal = 0;
                                decimal v1Wastage = 0;
                                DateTime v1BOMDate = DateTime.Now.Date;
                                string v1FinishItemNo = "";

                                decimal v1UOMQty = 0;
                                decimal v1UOMPrice = 0;
                                decimal v1UOMc = 0;
                                string v1UOMn = "";
                                string v1BOMId = "";
                                decimal v1UOMWastage = 0;
                                string vTransactionType = "";

                                #endregion Declare

                                #region Update Item Qty

                                #region Find Quantity From Products

                                DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDate, currConn, transaction, false);

                                decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                                decimal quantity = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                                if (quantity > 0)
                                {
                                    AvgRate = amount / quantity;
                                }
                                else
                                {
                                    AvgRate = 0;
                                }

                                vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());

                                #region Issue Settings
                                //CommonDAL commDal = new CommonDAL();
                                //int IssuePlaceQty = Convert.ToInt32(commDal.settings("Issue", "Quantity"));
                                //int IssuePlaceAmt = Convert.ToInt32(commDal.settings("Issue", "Amount"));
                                AvgRate = FormatingNumeric(AvgRate, 4);
                                vQuantity = FormatingNumeric(vQuantity, 4);
                                vWastage = FormatingNumeric(vWastage, 4);
                                #endregion Issue Settings

                                rwUom = BRItem["Uom"].ToString();

                                var rwMajorUom = BRItem["Uomn"].ToString();
                                if (string.IsNullOrEmpty(rwUom))
                                {
                                    throw new ArgumentNullException("ReceiveInsert",
                                                                    "Could not find UOM of raw item");
                                }

                                /*Processing UOM*/

                                UOMDAL uomdal = new UOMDAL();
                                vConvertionRate = uomdal.GetConvertionRate(rwMajorUom, rwUom, "Y", currConn, transaction); //uomc


                                #region valueAssign

                                v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                v1Wastage = (vWastage) * Item.UOMQty;
                                v1BOMId = BOMId;
                                v1RawItemNo = BRItem["RawItemNo"].ToString();
                                v1UOM = BRItem["UOM"].ToString();
                                v1CostPrice = AvgRate * vConvertionRate;
                                v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                v1UOMPrice = AvgRate;
                                v1UOMn = BRItem["UOMn"].ToString();
                                v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                v1FinishItemNo = Item.ItemNo;
                                v1UOMc = vConvertionRate;
                                v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;
                                vTransactionType = BRItem["TransactionType"].ToString();
                                #endregion valueAssign

                                #region Stock
                                var vNegStockAllow = commonDal.settings("Issue", "NegStockAllow");
                                bool NegStockAllow = Convert.ToBoolean(vNegStockAllow == "Y" ? true : false);

                                if (NegStockAllow == false)
                                {
                                    //var stock = productDal.StockInHand(BRItem["RawItemNo"].ToString(),
                                    //                                       Master.ReceiveDateTime,
                                    //                                   currConn, transaction).ToString();

                                    var stock = Convert.ToDecimal(productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(),
                                                                           Master.ReceiveDate,
                                                         currConn, transaction, false).Rows[0]["Quantity"].ToString());

                                    vStock = Convert.ToDecimal(stock);


                                    if ((vStock - v1UOMQty) < 0)
                                    {
                                        string FinName = string.Empty;
                                        string FinCode = string.Empty;
                                        string RawName = string.Empty;
                                        string RawCode = string.Empty;
                                        DataTable finDt = new DataTable();
                                        finDt = productDal.GetProductCodeAndNameByItemNo(Item.ItemNo);
                                        foreach (DataRow FinItem in finDt.Rows)
                                        {
                                            FinName = FinItem["ProductName"].ToString();
                                            FinCode = FinItem["ProductCode"].ToString();
                                        }
                                        DataTable rawDt = new DataTable();
                                        rawDt =
                                            productDal.GetProductCodeAndNameByItemNo(BRItem["RawItemNo"].ToString());
                                        foreach (DataRow RawItem in rawDt.Rows)
                                        {
                                            RawName = RawItem["ProductName"].ToString();
                                            RawCode = RawItem["ProductCode"].ToString();
                                        }

                                        throw new ArgumentNullException("ReceiveInsert",
                                                                        "Stock not Available for Finish Item( Name: " +
                                                                        FinName + " & Code: " + FinCode +
                                                                        " ) \n and consumtion Material ( Name: " +
                                                                        RawName + " & Code: " + RawCode + " )");
                                    }
                                }

                                #endregion Stock

                                #endregion Find Quantity From Products


                                #region Find Quantity From Transaction


                                sqlText = "";
                                sqlText += " insert into IssueDetails(";

                                sqlText += " IssueNo,";
                                sqlText += " IssueLineNo,";
                                sqlText += " ItemNo,";
                                sqlText += " Quantity,";
                                sqlText += " NBRPrice,";
                                sqlText += " CostPrice,";
                                sqlText += " UOM,";
                                sqlText += " VATRate,";
                                sqlText += " VATAmount,";
                                sqlText += " SubTotal,";
                                sqlText += " Comments,";
                                sqlText += " CreatedBy,";
                                sqlText += " CreatedOn,";
                                sqlText += " LastModifiedBy,";
                                sqlText += " LastModifiedOn,";
                                sqlText += " ReceiveNo,";
                                sqlText += " IssueDateTime,";
                                sqlText += " SD,";
                                sqlText += " SDAmount,"; //19
                                sqlText += " Wastage,";
                                sqlText += " BOMDate,";
                                sqlText += " FinishItemNo,";
                                sqlText += " transactionType,";
                                sqlText += " IssueReturnId,";
                                sqlText += " UOMQty,";
                                sqlText += " UOMPrice,";
                                sqlText += " UOMc,";
                                sqlText += " UOMn,";
                                sqlText += " UOMWastage,";
                                sqlText += " BOMId,";

                                sqlText += " Post";
                                sqlText += " )";
                                sqlText += " values( ";
                                sqlText += "@newID,";
                                sqlText += "'1',";
                                sqlText += "@v1RawItemNo, ";
                                sqlText += "@v1Quantity4,";
                                sqlText += "@AvgRate,";
                                sqlText += "@v1CostPrice4,";
                                sqlText += "@v1UOM,";
                                sqlText += " 0,0, ";
                                sqlText += "@v1SubTotal4,";
                                sqlText += "@ItemComments',";
                                sqlText += "@MasterCreatedBy',";
                                sqlText += "@MasterCreatedOn',";
                                sqlText += "@MasterLastModifiedBy',";
                                sqlText += "@MasterLastModifiedOn',";
                                sqlText += "@newID',";
                                sqlText += "@MasterReceiveDate',";
                                sqlText += " 0,	0,";
                                sqlText += "@v1Wastage,	";
                                sqlText += "@v1BOMDate,	";
                                sqlText += "@v1FinishItemNo',";
                                sqlText += "@MasterTransactionType',";
                                sqlText += "@MasterReturnId',";
                                sqlText += "@v1UOMQty4,";
                                sqlText += "@v1UOMPrice4,";
                                sqlText += "@v1UOMc,";
                                sqlText += "@v1UOMn',";
                                sqlText += "@v1UOMWastage',";
                                sqlText += "@v1BOMId',";

                                sqlText += "'N'";
                                sqlText += ")";
                                SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                cmdInsertIssue.Transaction = transaction;

                                cmdInsert.Parameters.AddWithValue("@newID", newID);
                                cmdInsert.Parameters.AddWithValue("@v1RawItemNo", v1RawItemNo ?? Convert.DBNull);
                                cmdInsert.Parameters.AddWithValue("@v1Quantity4)", FormatingNumeric(v1Quantity, 4));
                                cmdInsert.Parameters.AddWithValue("@AvgRate", AvgRate);
                                cmdInsert.Parameters.AddWithValue("@v1CostPrice4)", FormatingNumeric(v1CostPrice, 4));
                                cmdInsert.Parameters.AddWithValue("@v1UOM", v1UOM ?? Convert.DBNull);
                                cmdInsert.Parameters.AddWithValue("@v1SubTotal4)", FormatingNumeric(v1SubTotal, 4));
                                cmdInsert.Parameters.AddWithValue("@ItemComments", Item.Comments ?? Convert.DBNull);
                                cmdInsert.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                                cmdInsert.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                                cmdInsert.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                                cmdInsert.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                                //////cmdInsert.Parameters.AddWithValue("@newID", newID);
                                cmdInsert.Parameters.AddWithValue("@MasterReceiveDate", Master.ReceiveDate ?? Convert.DBNull);
                                cmdInsert.Parameters.AddWithValue("@v1Wastage", v1Wastage);
                                cmdInsert.Parameters.AddWithValue("@v1BOMDate", Convert.ToDateTime(v1BOMDate).ToString("MM/dd/yyyy"));
                                cmdInsert.Parameters.AddWithValue("@v1FinishItemNo", v1FinishItemNo ?? Convert.DBNull);
                                cmdInsert.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                                cmdInsert.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                                cmdInsert.Parameters.AddWithValue("@v1UOMQty4", FormatingNumeric(v1UOMQty, 4));
                                cmdInsert.Parameters.AddWithValue("@v1UOMPrice4)", FormatingNumeric(v1UOMPrice, 4));
                                cmdInsert.Parameters.AddWithValue("@v1UOMc", v1UOMc);
                                cmdInsert.Parameters.AddWithValue("@v1UOMn", v1UOMn ?? Convert.DBNull);
                                cmdInsert.Parameters.AddWithValue("@v1UOMWastage", v1UOMWastage);
                                cmdInsert.Parameters.AddWithValue("@v1BOMId", v1BOMId ?? Convert.DBNull);

                                transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                    MessageVM.receiveMsgUnableToSaveIssue);
                                }

                                #region Update Issue

                                sqlText = "";
                                sqlText += " update IssueHeaders set ";
                                sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                                sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                                sqlText += " where (IssueHeaders.IssueNo=@newID)";

                                SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                                cmdUpdateIssue.Transaction = transaction;

                                cmdUpdateIssue.Parameters.AddWithValue("@newID", newID);

                                transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                    MessageVM.receiveMsgUnableToSaveIssue);
                                }

                                #endregion Update Issue

                                #endregion Qty  check and Update

                                #endregion Qty  check and Update
                            }
                        }
                    }


                    if (Master.TransactionType == "TollReceive")
                    {
                        //decimal AvgRate = productDal.AvgPriceNew(Item.ItemNo,Master.ReceiveDate, currConn, transaction,false);

                        string FinishItemIdFromOH = productDal.GetFinishItemIdFromOH(Item.ItemNo, "VAT 1 (Toll Receive)", Master.ReceiveDate,
                                                                            currConn, transaction).ToString();

                        decimal NBRPrice = productDal.GetLastNBRPriceFromBOM(FinishItemIdFromOH, "VAT 1 (Toll Receive)", Master.ReceiveDate,
                                                                            currConn, transaction);

                        string ItemType = productDal.GetProductTypeByItemNo(FinishItemIdFromOH, currConn, transaction);
                        string UOM = productDal.GetProductTypeByItemNo(FinishItemIdFromOH, currConn, transaction);

                        if (!string.IsNullOrEmpty(ItemType))
                        {
                            if (ItemType == "Finish")
                            {
                                #region Insert to Receive  17 in

                                sqlText = "";
                                sqlText += " insert into ReceiveDetails(";
                                sqlText += " ReceiveNo,";
                                sqlText += " ReceiveLineNo,";
                                sqlText += " ItemNo,";
                                sqlText += " Quantity,";
                                sqlText += " CostPrice,";
                                sqlText += " NBRPrice,";
                                sqlText += " UOM,";
                                sqlText += " VATRate,";
                                sqlText += " VATAmount,";
                                sqlText += " SubTotal,";
                                sqlText += " Comments,";
                                sqlText += " CreatedBy,";
                                sqlText += " CreatedOn,";
                                sqlText += " LastModifiedBy,";
                                sqlText += " LastModifiedOn,";
                                sqlText += " SD,";
                                sqlText += " SDAmount,";
                                sqlText += " ReceiveDateTime,";
                                sqlText += " TransactionType,";
                                sqlText += " ReceiveReturnId,";
                                sqlText += " VATName,";


                                sqlText += " UOMQty,";
                                sqlText += " UOMPrice,";
                                sqlText += " UOMc,";
                                sqlText += " UOMn,";

                                sqlText += " Post";

                                sqlText += " )";
                                sqlText += " values(	";
                                sqlText += "@newID,";
                                sqlText += "@ItemLineNo,";
                                sqlText += "@FinishItemIdFromOH,";
                                sqlText += "@ItemQuantity,";
                                sqlText += "@NBRPrice,";
                                sqlText += "@NBRPrice,";
                                sqlText += "@UOM,";
                                sqlText += " 0,";
                                sqlText += " 0,";
                                sqlText += "@NBRPriceItemUOMQty,";
                                sqlText += "@ItemComments,";
                                sqlText += "@MasterCreatedBy,";
                                sqlText += "@MasterCreatedOn,";
                                sqlText += "@MasterLastModifiedBy,";
                                sqlText += "@MasterLastModifiedOn,";
                                sqlText += "0,";
                                sqlText += "0,";
                                sqlText += "@MasterReceiveDate,";
                                sqlText += "@MasterTransactionType,";
                                sqlText += "@MasterReturnId,";
                                sqlText += " 'VAT 1 (Toll Receive)',";
                                sqlText += "@ItemUOMQty,";
                                sqlText += "@NBRPrice,";
                                sqlText += "'1',";
                                sqlText += "@UOM,";
                                sqlText += "@MasterPost";

                                sqlText += ")	";
                                SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                                cmdInsertReceive.Transaction = transaction;

                                cmdInsertReceive.Parameters.AddWithValue("@newID", newID);
                                cmdInsertReceive.Parameters.AddWithValue("@ItemLineNo", Item.LineNo ?? Convert.DBNull);
                                cmdInsertReceive.Parameters.AddWithValue("@FinishItemIdFromOH", FinishItemIdFromOH ?? Convert.DBNull);
                                cmdInsertReceive.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                                cmdInsertReceive.Parameters.AddWithValue("@NBRPrice", NBRPrice);
                                cmdInsertReceive.Parameters.AddWithValue("@UOM", UOM ?? Convert.DBNull);
                                cmdInsertReceive.Parameters.AddWithValue("@NBRPriceItemUOMQty", NBRPrice * Item.UOMQty);
                                cmdInsertReceive.Parameters.AddWithValue("@ItemComments", Item.Comments ?? Convert.DBNull);
                                cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                                cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                                cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                                cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                                cmdInsertReceive.Parameters.AddWithValue("@MasterReceiveDate", Master.ReceiveDate ?? Convert.DBNull);
                                cmdInsertReceive.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                                cmdInsertReceive.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                                cmdInsertReceive.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                                cmdInsertReceive.Parameters.AddWithValue("@NBRPrice", NBRPrice);
                                cmdInsertReceive.Parameters.AddWithValue("@UOM", UOM ?? Convert.DBNull);
                                cmdInsertReceive.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

                                transResult = (int)cmdInsertReceive.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                    MessageVM.PurchasemsgUnableToSaveReceive);
                                }

                                #endregion Insert to Receive

                                #region Update Receive

                                sqlText = "";

                                sqlText += "  update ReceiveHeaders set TotalAmount=  ";
                                sqlText += " (select sum(Quantity*CostPrice) from ReceiveDetails ";
                                sqlText += " where ReceiveDetails.ReceiveNo =ReceiveHeaders.ReceiveNo) ";
                                sqlText += " where ReceiveHeaders.ReceiveNo=@newID";

                                SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                                cmdUpdateReceive.Transaction = transaction;

                                cmdUpdateReceive.Parameters.AddWithValue("@newID", newID);

                                transResult = (int)cmdUpdateReceive.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                    MessageVM.PurchasemsgUnableToSaveReceive);
                                }

                                #endregion Update Receive
                            }
                            else if (ItemType == "Raw"  //16 in
                                   || ItemType == "Pack"
                                || ItemType == "WIP"
                                || ItemType == "Trading")
                            {
                                #region Insert only DetailTable PurchaseInvoiceDetails

                                sqlText = "";
                                sqlText += " insert into PurchaseInvoiceDetails(";
                                sqlText += " PurchaseInvoiceNo,";
                                sqlText += " POLineNo,";
                                sqlText += " ItemNo,";
                                sqlText += " Quantity,";
                                sqlText += " CostPrice,";
                                sqlText += " NBRPrice,";
                                sqlText += " UOM,";
                                sqlText += " VATRate,";
                                sqlText += " VATAmount,";
                                sqlText += " SubTotal,";
                                sqlText += " Comments,";
                                sqlText += " CreatedBy,";
                                sqlText += " CreatedOn,";
                                sqlText += " LastModifiedBy,";
                                sqlText += " LastModifiedOn,";
                                sqlText += " SD,";
                                sqlText += " SDAmount,";
                                sqlText += " Type,";
                                sqlText += " ProductType,";
                                sqlText += " BENumber,";
                                sqlText += " InvoiceDateTime,";
                                sqlText += " ReceiveDate,";
                                sqlText += " Post,";
                                sqlText += " UOMQty,";
                                sqlText += " UOMPrice,";
                                sqlText += " UOMc,";
                                sqlText += " UOMn,";
                                sqlText += " RebateRate ,";
                                sqlText += " RebateAmount ,";
                                sqlText += " CnFAmount ,";
                                sqlText += " InsuranceAmount ,";
                                sqlText += " AssessableValue ,";
                                sqlText += " CDAmount ,";
                                sqlText += " RDAmount ,";
                                sqlText += " TVBAmount ,";
                                sqlText += " TVAAmount ,";
                                sqlText += " ATVAmount ,";
                                sqlText += " TransactionType ,";
                                sqlText += " PurchaseReturnId ,";
                                sqlText += " OthersAmount ";

                                sqlText += " )";
                                sqlText += " values(	";
                                sqlText += "@newID,";
                                sqlText += "@ItemLineNo,";
                                sqlText += "@FinishItemIdFromOH,";
                                sqlText += "@ItemQuantity,";
                                sqlText += "@NBRPrice,";
                                sqlText += "@NBRPrice,";
                                sqlText += "@UOM,";
                                sqlText += "0,";
                                sqlText += "0,";
                                sqlText += "@NBRPriceItemUOMQty ,";
                                sqlText += "@ItemComments,";
                                sqlText += "@MasterCreatedBy,";
                                sqlText += "@MasterCreatedOn,";
                                sqlText += "@MasterLastModifiedBy,";
                                sqlText += "@MasterLastModifiedOn,";
                                sqlText += "0,";
                                sqlText += "0,";
                                sqlText += "@ItemType,";
                                sqlText += "@ItemProductType,";
                                sqlText += "@ItemBENumber,";
                                sqlText += "@MasterInvoiceDate,";
                                sqlText += "@MasterReceiveDate,";
                                sqlText += "@MasterPost,";
                                sqlText += "@ItemUOMQty,";
                                sqlText += "@NBRPrice,";
                                sqlText += "1,";
                                sqlText += "@UOM,";
                                sqlText += "0,";
                                sqlText += "0,";
                                sqlText += "0,";
                                sqlText += "0,";
                                sqlText += "0,";
                                sqlText += "0,";
                                sqlText += "0,";
                                sqlText += "0,";
                                sqlText += "0,";
                                sqlText += "0,";
                                sqlText += "'TollReceive-WIP',";
                                sqlText += "@MasterReturnId,";
                                sqlText += "@ItemOthersAmount";
                                sqlText += ")	";


                                SqlCommand cmdInsDetailW = new SqlCommand(sqlText, currConn);
                                cmdInsDetailW.Transaction = transaction;

                                cmdInsDetailW.Parameters.AddWithValue("@newID", newID);
                                cmdInsDetailW.Parameters.AddWithValue("@ItemLineNo", Item.LineNo ?? Convert.DBNull);
                                cmdInsDetailW.Parameters.AddWithValue("@FinishItemIdFromOH", FinishItemIdFromOH ?? Convert.DBNull);
                                cmdInsDetailW.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                                cmdInsDetailW.Parameters.AddWithValue("@NBRPrice", NBRPrice);
                                cmdInsDetailW.Parameters.AddWithValue("@NBRPrice", NBRPrice);
                                cmdInsDetailW.Parameters.AddWithValue("@UOM", UOM ?? Convert.DBNull);
                                cmdInsDetailW.Parameters.AddWithValue("@NBRPriceItemUOMQty", NBRPrice * Item.UOMQty);
                                cmdInsDetailW.Parameters.AddWithValue("@ItemComments", Item.Comments ?? Convert.DBNull);
                                cmdInsDetailW.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                                cmdInsDetailW.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                                cmdInsDetailW.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                                cmdInsDetailW.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                                cmdInsDetailW.Parameters.AddWithValue("@ItemType", Item.Type ?? Convert.DBNull);
                                cmdInsDetailW.Parameters.AddWithValue("@ItemProductType", Item.ProductType ?? Convert.DBNull);
                                cmdInsDetailW.Parameters.AddWithValue("@ItemBENumber", Item.BENumber ?? Convert.DBNull);
                                cmdInsDetailW.Parameters.AddWithValue("@MasterInvoiceDate", Master.InvoiceDate);
                                cmdInsDetailW.Parameters.AddWithValue("@MasterReceiveDate", Master.ReceiveDate);
                                cmdInsDetailW.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                                cmdInsDetailW.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                                cmdInsDetailW.Parameters.AddWithValue("@NBRPrice", NBRPrice);
                                cmdInsDetailW.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                                cmdInsDetailW.Parameters.AddWithValue("@ItemOthersAmount", Item.OthersAmount);

                                transResult = (int)cmdInsDetailW.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                    MessageVM.PurchasemsgSaveNotSuccessfully);
                                }

                                #endregion Insert only DetailTable
                            }
                        }

                    }

                    #endregion Transaction is Trading

                    #region Transaction is InputService

                    if (Master.TransactionType == "InputService" || Master.TransactionType == "InputServiceImport")
                    {
                        decimal PurchasePrice = productDal.PurchasePrice(Item.ItemNo, newID, currConn, transaction);

                        #region Insert to Issue

                        sqlText = "";
                        sqlText += " insert into IssueDetails(";
                        //sqlText += " IssueNo,";
                        sqlText += " IssueNo,";
                        sqlText += " IssueLineNo,";
                        sqlText += " ItemNo,";
                        sqlText += " Quantity,";
                        sqlText += " NBRPrice,";
                        sqlText += " CostPrice,";
                        sqlText += " UOM,";
                        sqlText += " VATRate,";
                        sqlText += " VATAmount,";
                        sqlText += " SubTotal,";
                        sqlText += " Comments,";
                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn,";
                        sqlText += " ReceiveNo,";
                        sqlText += " IssueDateTime,";
                        sqlText += " SD,";
                        sqlText += " SDAmount,";
                        sqlText += " Wastage,";
                        sqlText += " BOMDate,";
                        sqlText += " FinishItemNo,";
                        sqlText += " TransactionType,";
                        sqlText += " IssueReturnId,";
                        sqlText += " UOMQty,";
                        sqlText += " UOMPrice,";
                        sqlText += " UOMc,";
                        sqlText += " UOMn,";
                        sqlText += " UOMWastage,";
                        sqlText += " Post";

                        sqlText += " )";
                        sqlText += " values(	";
                        sqlText += "@newID,";
                        sqlText += "@ItemLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemQuantity,";
                        sqlText += " 0,";
                        if (Master.TransactionType == "InputServiceImport")
                        {
                            //PurchasePrice = PurchasePrice + Convert.ToDecimal(Item.ATVAmount) + Convert.ToDecimal(Item.TVAAmount);
                            sqlText += "@PurchasePrice,";
                            sqlText += "@ItemUOM,";
                            sqlText += " 0,	";
                            sqlText += " 0,";
                            sqlText += " (@PurchasePrice*@ItemUOMQty),";
                        }
                        else if (Master.TransactionType == "InputService")
                        {
                            sqlText += "@ItemSubTotal,";
                            sqlText += "@ItemUOM,";
                            sqlText += " 0,	";
                            sqlText += " 0,";
                            sqlText += " (@ItemSubTotal*@ItemUOMQty),";
                        }

                        sqlText += "@ItemComments,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "@newID,";
                        sqlText += "@MasterReceiveDate,";
                        sqlText += " 0,";//SD
                        sqlText += " 0,";
                        sqlText += " 0,	";
                        sqlText += " 0,	";
                        sqlText += " 0,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterReturnId,";
                        sqlText += "@ItemUOMQty,";
                        sqlText += "@ItemUOMPrice,";
                        sqlText += "@ItemUOMc,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "'0',";
                        sqlText += "@MasterPost";

                        sqlText += ")	";
                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                        cmdInsertIssue.Transaction = transaction;

                        cmdInsertIssue.Parameters.AddWithValue("@newID", newID);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemLineNo", Item.LineNo);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);

                        if (Master.TransactionType == "InputServiceImport")
                        {
                            PurchasePrice = PurchasePrice + Convert.ToDecimal(Item.ATVAmount) + Convert.ToDecimal(Item.TVAAmount);
                            cmdInsertIssue.Parameters.AddWithValue("@PurchasePrice", PurchasePrice);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOM", Item.UOM);
                            cmdInsertIssue.Parameters.AddWithValue("@PurchasePrice", PurchasePrice);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                        }

                        else if (Master.TransactionType == "InputService")
                        {
                            cmdInsertIssue.Parameters.AddWithValue("@ItemSubTotal", Item.SubTotal);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOM", Item.UOM);
                            cmdInsertIssue.Parameters.AddWithValue("@PurchasePrice", PurchasePrice);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                        }

                        cmdInsertIssue.Parameters.AddWithValue("@ItemComments", Item.Comments ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                        //////cmdInsertIssue.Parameters.AddWithValue("@newID", newID);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterReceiveDate", Master.ReceiveDate ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMn", Item.UOMn);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

                        transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.PurchasemsgUnableToSaveIssue);
                        }

                        #endregion Insert to Issue

                        #region Update Issue

                        sqlText = "";
                        sqlText += " update IssueHeaders set ";
                        sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                        sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                        sqlText += " where (IssueHeaders.IssueNo= '" + newID + "')";

                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                        cmdUpdateIssue.Transaction = transaction;
                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.PurchasemsgUnableToSaveIssue);
                        }

                        #endregion Update Issue

                    }

                    #endregion Transaction is InputService

                    #endregion Insert Issue and Receive if Transaction is not Other

                }


                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)

                #region Tracking

                if (Trackings.Count() > 0)
                {
                    if (Trackings[0].transactionType == "Purchase_Return")
                    {
                        Trackings[0].ReturnPurchaseID = newID;
                        string trackingUpdate = string.Empty;
                        TrackingDAL trackingDal = new TrackingDAL();
                        trackingUpdate = trackingDal.TrackingUpdate(Trackings, transaction, currConn);

                        if (trackingUpdate == "Fail")
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, "Tracking Information not added.");
                        }
                    }
                    else
                    {


                        foreach (var tracking in Trackings.ToList())
                        {

                            #region Find Heading1 Existence

                            sqlText = "";
                            sqlText += "select COUNT(Heading1) from Trackings WHERE Heading1 =@trackingHeading1";
                            SqlCommand cmdFindHeading1 = new SqlCommand(sqlText, currConn);
                            cmdFindHeading1.Transaction = transaction;
                            cmdFindHeading1.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1);


                            objIDExist = cmdFindHeading1.ExecuteScalar();
                            if (objIDExist != null)
                            {
                                IDExist = Convert.ToInt32(objIDExist);
                            }

                            if (IDExist > 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                "Requested Tracking Information (" + tracking.Heading1 + ") already added.");
                            }

                            #endregion Find Heading1 Existence

                            #region Find Heading2 Existence

                            sqlText = "";
                            sqlText += "select COUNT(Heading2) from Trackings WHERE Heading2 = @trackingHeading2";
                            SqlCommand cmdFindHeading2 = new SqlCommand(sqlText, currConn);
                            cmdFindHeading2.Transaction = transaction;
                            cmdFindHeading2.Parameters.AddWithValue("@trackingHeading2", tracking.Heading2);

                            objIDExist = cmdFindHeading2.ExecuteScalar();
                            if (objIDExist != null)
                            {
                                IDExist = Convert.ToInt32(objIDExist);
                            }

                            if (IDExist > 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                 "Requested Tracking Information (" + tracking.Heading2 + ") already added.");
                            }

                            #endregion Find Heading2 Existence

                            sqlText = "";
                            sqlText += " insert into Trackings";
                            sqlText += " (";

                            sqlText += " PurchaseInvoiceNo,";
                            sqlText += " ItemNo,";
                            sqlText += " TrackLineNo,";
                            sqlText += " Heading1,";
                            sqlText += " Heading2,";
                            sqlText += " Quantity,";
                            sqlText += " UnitPrice,";
                            sqlText += " IsPurchase,";
                            sqlText += " IsIssue,";
                            sqlText += " IsReceive,";
                            sqlText += " IsSale,";
                            sqlText += " Post,";
                            sqlText += " ReceivePost,";
                            sqlText += " SalePost,";
                            sqlText += " IssuePost,";


                            sqlText += " CreatedBy,";
                            sqlText += " CreatedOn,";
                            sqlText += " LastModifiedBy,";
                            sqlText += " LastModifiedOn";

                            sqlText += " )";
                            sqlText += " values";
                            sqlText += " (";

                            sqlText += "@newID,";
                            sqlText += "@trackingItemNo,";
                            sqlText += "@trackingTrackingLineNo,";
                            sqlText += "@trackingHeading1,";
                            sqlText += "@trackingHeading2,";
                            sqlText += "@trackingQuantity,";
                            sqlText += "@trackingUnitPrice,";
                            sqlText += "@trackingIsPurchase,";
                            sqlText += "@trackingIsIssue,";
                            sqlText += "@trackingIsReceive,";
                            sqlText += "@trackingIsSale,";
                            sqlText += "@MasterPost,";
                            sqlText += "'N',";
                            sqlText += "'N',";
                            sqlText += "'N',";

                            sqlText += "@MasterCreatedBy,";
                            sqlText += "@MasterCreatedOn,";
                            sqlText += "@MasterLastModifiedBy,";
                            sqlText += "@MasterLastModifiedOn";

                            sqlText += ")";


                            SqlCommand cmdInsertTrackings = new SqlCommand(sqlText, currConn);
                            cmdInsertTrackings.Transaction = transaction;

                            cmdInsertTrackings.Parameters.AddWithValue("@newID", newID);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingItemNo", tracking.ItemNo);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingTrackingLineNo", tracking.TrackingLineNo);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingHeading2", tracking.Heading2);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingQuantity", tracking.Quantity);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingUnitPrice", tracking.UnitPrice);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingIsPurchase", tracking.IsPurchase);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingIsIssue", tracking.IsIssue);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingIsReceive", tracking.IsReceive);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingIsSale", tracking.IsSale);
                            cmdInsertTrackings.Parameters.AddWithValue("@MasterPost", Master.Post);
                            cmdInsertTrackings.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy);
                            cmdInsertTrackings.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                            cmdInsertTrackings.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdInsertTrackings.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);

                            transResult = (int)cmdInsertTrackings.ExecuteNonQuery();
                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.PurchasemsgSaveNotSuccessfully);
                            }
                        }

                    }

                }

                #endregion Tracking

                #region TrackingWithSale
                if (Master.Post.ToLower() == "y")
                {

                    bool TrackingWithSale = Convert.ToBoolean(commonDal.settingValue("Purchase", "TrackingWithSale") == "Y" ? true : false);
                    if (TrackingWithSale)
                    {
                        DataTable tracDt = new DataTable();
                        sqlText = "";
                        sqlText = @"SELECT    
                                    PurchaseInvoiceDetails.PurchaseInvoiceNo,
                                    PurchaseInvoiceDetails.InvoiceDateTime,
                                    PurchaseInvoiceDetails.ReceiveDate,
                                    PurchaseInvoiceDetails.ItemNo, 
                                    isnull(PurchaseInvoiceDetails.BENumber,'N/A')BENumber ,
                                    isnull(PurchaseInvoiceDetails.Quantity,0)Quantity,
                                    isnull(PurchaseInvoiceDetails.UOM,'N/A')UOM ,
                                    isnull(PurchaseInvoiceDetails.VATRate,0)VATRate,
                                    isnull(PurchaseInvoiceDetails.ReturnTransactionType,'')ReturnTransactionType,
                                    isnull(PurchaseInvoiceHeaders.CustomHouse,'')CustomHouse
                                    FROM dbo.PurchaseInvoiceDetails 
                                    left outer join PurchaseInvoiceHeaders on PurchaseInvoiceHeaders.PurchaseInvoiceNo=PurchaseInvoiceDetails.PurchaseInvoiceNo
                                    WHERE ";
                        sqlText += @"   (PurchaseInvoiceDetails.PurchaseInvoiceNo = '" + newID + "')";
                        sqlText += @"  order by PurchaseInvoiceDetails.ItemNo";
                        SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                        cmdRIFB.Transaction = transaction;
                        SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                        reportDataAdapt.Fill(tracDt);

                        foreach (DataRow dRow in tracDt.Rows)
                        {
                            #region Insert only DetailTable PurchaseInvoiceDetails

                            sqlText = "";
                            sqlText += " insert into PurchaseSaleTrackings(";
                            sqlText += " PurchaseInvoiceNo,";
                            sqlText += " PurchaseInvoiceDateTime,";
                            sqlText += " ReceiveDate,";
                            sqlText += " CustomHouse,";
                            sqlText += " ItemNo,";
                            sqlText += " BENumber,";
                            sqlText += " SalesInvoiceNo,";
                            sqlText += " SaleInvoiceDateTime,";
                            sqlText += " IsSold";
                            sqlText += " )";
                            sqlText += " values(	";
                            sqlText += "'" + dRow["PurchaseInvoiceNo"].ToString() + "',";
                            sqlText += "'" + dRow["InvoiceDateTime"].ToString() + "',";
                            sqlText += "'" + dRow["ReceiveDate"].ToString() + "',";
                            sqlText += "'" + dRow["CustomHouse"].ToString() + "',";
                            sqlText += "'" + dRow["ItemNo"].ToString() + "',";
                            sqlText += "'" + dRow["BENumber"].ToString() + "',";
                            sqlText += "'0',";
                            sqlText += "'01/01/1900',";
                            sqlText += "'0'";
                            sqlText += ")	";

                            decimal qty = Convert.ToDecimal(dRow["Quantity"]);
                            for (int i = 0; i < qty; i++)
                            {
                                SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                                cmdInsDetail.Transaction = transaction;
                                transResult = (int)cmdInsDetail.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                    MessageVM.PurchasemsgSaveNotSuccessfully);
                                }
                            }


                            #endregion Insert only DetailTable
                        }
                    }
                }
                #endregion TrackingWithSale

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText +
                           "select distinct  Post from dbo.PurchaseInvoiceHeaders WHERE PurchaseInvoiceNo='" + newID +
                           "'";

                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                //PostStatus = (string)cmdIPS.ExecuteScalar();
                objIDExist = cmdIPS.ExecuteScalar();
                if (objIDExist != null)
                {
                    PostStatus = Convert.ToString(objIDExist);
                }

                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.PurchasemsgUnableCreatID);
                }


                #endregion Prefetch

                #region Commit


                if (vcurrConn == null)
                {
                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                        }
                    }
                }

                #endregion Commit
                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.PurchasemsgSaveSuccessfully;
                retResults[2] = "" + Master.Id;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex

                if (vcurrConn == null) { transaction.Rollback(); }


                FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
                return retResults;
            }
            #endregion
            finally
            {
                if (vcurrConn == null)
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }
        public string[] PurchaseUpdate(PurchaseMasterVM Master, List<PurchaseDetailVM> Details, List<PurchaseDutiesVM> Duties, List<TrackingVM> Trackings)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            int ReceiveInsertSuccessfully = 0;
            string PostStatus = "";
            ProductDAL productDal = new ProductDAL();



            int nextId = 0;
            #endregion Initializ

            #region Try
            try
            {
                SetDefaultValue(Master);

                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate, MessageVM.PurchasemsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.ReceiveDate) < DateTime.MinValue || Convert.ToDateTime(Master.ReceiveDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate, "Please Check Invoice Data and Time");

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();

                transaction = currConn.BeginTransaction(MessageVM.PurchasemsgMethodNameInsert);


                bool TollReceiveWithIssue = Convert.ToBoolean(commonDal.settings("TollReceive", "WithIssue") == "Y" ? true : false);


                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.ReceiveDate;
                string transactionYearCheck = Convert.ToDateTime(Master.ReceiveDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock
                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK



                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(PurchaseInvoiceNo) from PurchaseInvoiceHeaders WHERE PurchaseInvoiceNo=@MasterPurchaseInvoiceNo";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                int IDExistP = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExistP <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                    MessageVM.PurchasemsgUnableFindExistID);
                }

                #endregion Find ID for Update

                #region ID check completed,update Information in Header

                #region update Header

                sqlText = "";

                sqlText += " update PurchaseInvoiceHeaders set  ";
                sqlText += " VendorID           = @MasterVendorID ,";
                sqlText += " InvoiceDateTime    = @MasterInvoiceDate ,";
                sqlText += " CustomHouse        = @MasterCustomHouse ,";
                sqlText += " TotalAmount        = @MasterTotalAmount ,";
                sqlText += " TotalVATAmount     = @MasterTotalVATAmount,";
                sqlText += " SerialNo           = @MasterSerialNo ,";
                sqlText += " LCNumber           = @MasterLCNumber ,";
                sqlText += " Comments           = @MasterComments ,";
                sqlText += " LastModifiedBy     = @MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn     = @MasterLastModifiedOn ,";
                sqlText += " BENumber           = @MasterBENumber ,";
                sqlText += " ProductType        = @MasterProductType ,";
                sqlText += " transactionType    = @MasterTransactionType ,";
                sqlText += " ReceiveDate        = @MasterReceiveDate ,";
                sqlText += " Post               = @MasterPost, ";
                sqlText += " WithVDS            = @MasterWithVDS, ";
                sqlText += " PurchaseReturnId   = @MasterReturnId, ";
                sqlText += " LCDate             = @MasterLCDate, ";
                sqlText += " LandedCost         = @MasterLandedCost ";
                sqlText += " where  PurchaseInvoiceNo=@MasterPurchaseInvoiceNo ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@MasterVendorID", Master.VendorID);
                cmdUpdate.Parameters.AddWithValue("@MasterInvoiceDate", Master.InvoiceDate ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterCustomHouse", Master.CustomHouse ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterTotalAmount", Master.TotalAmount);
                cmdUpdate.Parameters.AddWithValue("@MasterTotalVATAmount", Master.TotalVATAmount);
                cmdUpdate.Parameters.AddWithValue("@MasterSerialNo", Master.SerialNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLCNumber", Master.LCNumber ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterBENumber", Master.BENumber ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterProductType", Master.ProductType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterReceiveDate", Master.ReceiveDate ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterWithVDS", Master.WithVDS ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLCDate", Master.LCDate ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLandedCost", Master.LandedCost);
                cmdUpdate.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo ?? Convert.DBNull);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                    MessageVM.PurchasemsgUpdateNotSuccessfully);
                }

                #endregion update Header

                #region Import

                if (Master.TransactionType == "Import"
                || Master.TransactionType == "ServiceImport"
                || Master.TransactionType == "ServiceNSImport"
                || Master.TransactionType == "TradingImport"
                || Master.TransactionType == "InputServiceImport")
                {
                    if (Duties.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate, MessageVM.PurchasemsgNoDataToUpdateImportDyties);
                    }
                    foreach (var duty in Duties.ToList())
                    {
                        sqlText = "";
                        sqlText +=
                            "select COUNT(PurchaseInvoiceNo) from PurchaseInvoiceDuties WHERE PurchaseInvoiceNo=@MasterPurchaseInvoiceNo ";
                        sqlText += " AND ItemNo=@dutyItemNo";
                        SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                        cmdFindId.Transaction = transaction;

                        cmdFindId.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                        cmdFindId.Parameters.AddWithValue("@dutyItemNo", duty.ItemNo);

                        decimal DetIDExist = (int)cmdFindId.ExecuteScalar();

                        if (DetIDExist <= 0) // insert
                        {
                            #region Insert
                            sqlText = "select isnull(max(cast(PIDutyID as int)),0)+1 FROM  PurchaseInvoiceDuties";
                            SqlCommand cmdDutyNextId = new SqlCommand(sqlText, currConn);
                            cmdDutyNextId.Transaction = transaction;
                            int nextIdD = (int)cmdDutyNextId.ExecuteScalar();
                            if (nextIdD <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.PurchasemsgDutyIdNotCreate);
                            }

                            sqlText = "";
                            sqlText += " insert into PurchaseInvoiceDuties";
                            sqlText += " (";
                            sqlText += " PIDutyID,";
                            sqlText += " PurchaseInvoiceNo,";
                            sqlText += " ItemNo,";
                            sqlText += " Quantity,";
                            sqlText += " DutyCompleteQuantity,";
                            sqlText += " DutyCompleteQuantityPercent,";
                            sqlText += " CnFInp,";
                            sqlText += " CnFRate,";
                            sqlText += " CnFAmount,";
                            sqlText += " InsuranceInp,";
                            sqlText += " InsuranceRate,";
                            sqlText += " InsuranceAmount,";
                            sqlText += " AssessableInp,";
                            sqlText += " AssessableValue,";
                            sqlText += " CDInp,";
                            sqlText += " CDRate,";
                            sqlText += " CDAmount,";
                            sqlText += " RDInp,";
                            sqlText += " RDRate,";
                            sqlText += " RDAmount,";
                            sqlText += " TVBInp,";
                            sqlText += " TVBRate,";
                            sqlText += " TVBAmount,";
                            sqlText += " SDInp,";
                            sqlText += " SD,";
                            sqlText += " SDAmount,";
                            sqlText += " VATInp,";
                            sqlText += " VATRate,";
                            sqlText += " VATAmount,";
                            sqlText += " TVAInp,";
                            sqlText += " TVARate,";
                            sqlText += " TVAAmount,";
                            sqlText += " ATVInp,";
                            sqlText += " ATVRate,";
                            sqlText += " ATVAmount,";
                            sqlText += " OthersInp,";
                            sqlText += " OthersRate,";
                            sqlText += " OthersAmount,";
                            sqlText += " UnitCost,";
                            sqlText += " LineCost,";
                            sqlText += " CreatedBy,";
                            sqlText += " CreatedOn,";
                            sqlText += " LastModifiedBy,";
                            sqlText += " LastModifiedOn,";
                            sqlText += " TransactionType,";
                            sqlText += " Post,";
                            sqlText += " Remarks";
                            sqlText += " )";
                            sqlText += " values";
                            sqlText += " (";
                            sqlText += "@nextIdD,";
                            sqlText += "@MasterPurchaseInvoiceNo,";
                            sqlText += "@dutyItemNo,";
                            sqlText += "@dutyQuantity,";
                            sqlText += "@dutyDutyCompleteQuantity,";
                            sqlText += "@dutyDutyCompleteQuantityPercent,";
                            sqlText += "@dutyCnFInp,";
                            sqlText += "@dutyCnFRate,";
                            sqlText += "@dutyCnFAmount,";
                            sqlText += "@dutyInsuranceInp,";
                            sqlText += "@dutyInsuranceRate,";
                            sqlText += "@dutyInsuranceAmount,";
                            sqlText += "@dutyAssessableInp,";
                            sqlText += "@dutyAssessableValue,";
                            sqlText += "@dutyCDInp,";
                            sqlText += "@dutyCDRate,";
                            sqlText += "@dutyCDAmount,";
                            sqlText += "@dutyRDInp,";
                            sqlText += "@dutyRDRate,";
                            sqlText += "@dutyRDAmount,";
                            sqlText += "@dutyTVBInp,";
                            sqlText += "@dutyTVBRate,";
                            sqlText += "@dutyTVBAmount,";
                            sqlText += "@dutySDInp,";
                            sqlText += "@dutySD,";
                            sqlText += "@dutySDAmount,";
                            sqlText += "@dutyVATInp,";
                            sqlText += "@dutyVATRate,";
                            sqlText += "@dutyVATAmount,";
                            sqlText += "@dutyTVAInp,";
                            sqlText += "@dutyTVARate,";
                            sqlText += "@dutyTVAAmount,";
                            sqlText += "@dutyATVInp,";
                            sqlText += "@dutyATVRate,";
                            sqlText += "@dutyATVAmount,";
                            sqlText += "@dutyOthersInp,";
                            sqlText += "@dutyOthersRate,";
                            sqlText += "@dutyOthersAmount,";
                            sqlText += "@dutyUnitCost,";
                            sqlText += "@dutyLineCost,";
                            sqlText += "@MasterCreatedBy,";
                            sqlText += "@MasterCreatedOn,";
                            sqlText += "@MasterLastModifiedBy,";
                            sqlText += "@MasterLastModifiedOn,";
                            sqlText += "@MasterTransactionType,";
                            sqlText += "@MasterPost,";
                            sqlText += "@dutyRemarks";
                            sqlText += ")";


                            SqlCommand cmdInsertDuty = new SqlCommand(sqlText, currConn);
                            cmdInsertDuty.Transaction = transaction;

                            cmdInsertDuty.Parameters.AddWithValue("@nextIdD", nextIdD);
                            cmdInsertDuty.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyItemNo", duty.ItemNo);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyQuantity", duty.Quantity);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyDutyCompleteQuantity", duty.DutyCompleteQuantity);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyDutyCompleteQuantityPercent", duty.DutyCompleteQuantityPercent);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyCnFInp", duty.CnFInp);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyCnFRate", duty.CnFRate);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyCnFAmount", duty.CnFAmount);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyInsuranceInp", duty.InsuranceInp);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyInsuranceRate", duty.InsuranceRate);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyInsuranceAmount", duty.InsuranceAmount);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyAssessableInp", duty.AssessableInp);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyAssessableValue", duty.AssessableValue);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyCDInp", duty.CDInp);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyCDRate", duty.CDRate);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyCDAmount", duty.CDAmount);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyRDInp", duty.RDInp);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyRDRate", duty.RDRate);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyRDAmount", duty.RDAmount);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyTVBInp", duty.TVBInp);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyTVBRate", duty.TVBRate);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyTVBAmount", duty.TVBAmount);
                            cmdInsertDuty.Parameters.AddWithValue("@dutySDInp", duty.SDInp);
                            cmdInsertDuty.Parameters.AddWithValue("@dutySD", duty.SD);
                            cmdInsertDuty.Parameters.AddWithValue("@dutySDAmount", duty.SDAmount);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyVATInp", duty.VATInp);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyVATRate", duty.VATRate);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyVATAmount", duty.VATAmount);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyTVAInp", duty.TVAInp);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyTVARate", duty.TVARate);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyTVAAmount", duty.TVAAmount);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyATVInp", duty.ATVInp);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyATVRate", duty.ATVRate);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyATVAmount", duty.ATVAmount);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyOthersInp", duty.OthersInp);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyOthersRate", duty.OthersRate);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyOthersAmount", duty.OthersAmount);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyUnitCost", duty.UnitCost);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyLineCost", duty.LineCost);
                            cmdInsertDuty.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy);
                            cmdInsertDuty.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                            cmdInsertDuty.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdInsertDuty.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                            cmdInsertDuty.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType);
                            cmdInsertDuty.Parameters.AddWithValue("@MasterPost", Master.Post);
                            cmdInsertDuty.Parameters.AddWithValue("@dutyRemarks", duty.Remarks);

                            transResult = (int)cmdInsertDuty.ExecuteNonQuery();
                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.PurchasemsgSaveNotSuccessfully);
                            }
                            #endregion Insert

                        }
                        else // update
                        {
                            #region update Duties

                            sqlText = "";

                            sqlText += " update PurchaseInvoiceDuties set  ";
                            sqlText += " DutyCompleteQuantity   = @dutyQuantity, ";
                            sqlText += " CnFInp                 = @dutyCnFInp, ";
                            sqlText += " CnFRate                = @dutyCnFRate, ";
                            sqlText += " CnFAmount              = @dutyCnFAmount, ";
                            sqlText += " InsuranceInp           = @dutyInsuranceInp, ";
                            sqlText += " InsuranceRate          = @dutyInsuranceRate, ";
                            sqlText += " InsuranceAmount        = @dutyInsuranceAmount, ";
                            sqlText += " AssessableInp          = @dutyAssessableInp, ";
                            sqlText += " AssessableValue        = @dutyAssessableValue, ";
                            sqlText += " CDInp                  = @dutyCDInp, ";
                            sqlText += " CDRate                 = @dutyCDRate, ";
                            sqlText += " CDAmount               = @dutyCDAmount, ";
                            sqlText += " RDInp                  = @dutyRDInp, ";
                            sqlText += " RDRate                 = @dutyRDRate, ";
                            sqlText += " RDAmount               = @dutyRDAmount, ";
                            sqlText += " TVBInp                 = @dutyTVBInp, ";
                            sqlText += " TVBRate                = @dutyTVBRate, ";
                            sqlText += " TVBAmount              = @dutyTVBAmount, ";
                            sqlText += " SDInp                  = @dutySDInp, ";
                            sqlText += " SD                     = @dutySD, ";
                            sqlText += " SDAmount               = @dutySDAmount, ";
                            sqlText += " VATInp                 = @dutyVATInp, ";
                            sqlText += " VATRate                = @dutyVATRate, ";
                            sqlText += " VATAmount              = @dutyVATAmount, ";
                            sqlText += " TVAInp                 = @dutyTVAInp, ";
                            sqlText += " TVARate                = @dutyTVARate, ";
                            sqlText += " TVAAmount              = @dutyTVAAmount, ";
                            sqlText += " ATVInp                 = @dutyATVInp, ";
                            sqlText += " ATVRate                = @dutyATVRate, ";
                            sqlText += " ATVAmount              = @dutyATVAmount, ";
                            sqlText += " OthersInp              = @dutyOthersInp, ";
                            sqlText += " OthersRate             = @dutyOthersRate, ";
                            sqlText += " OthersAmount           = @dutyOthersAmount, ";
                            sqlText += " CreatedBy              = @MasterCreatedBy, ";
                            sqlText += " CreatedOn              = @MasterCreatedOn, ";
                            sqlText += " LastModifiedBy         = @MasterLastModifiedBy, ";
                            sqlText += " LastModifiedOn         = @MasterLastModifiedOn, ";
                            sqlText += " TransactionType        = @MasterTransactionType, ";
                            sqlText += " Post                   = @MasterPost, ";
                            sqlText += " Remarks                = @dutyRemarks";
                            sqlText += " where  PurchaseInvoiceNo = @MasterPurchaseInvoiceNo ";
                            sqlText += " and   ItemNo           = @dutyItemNo ";


                            SqlCommand cmdUpdateDuty = new SqlCommand(sqlText, currConn);
                            cmdUpdateDuty.Transaction = transaction;

                            cmdUpdateDuty.Parameters.AddWithValue("@dutyQuantity", duty.Quantity);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyCnFInp", duty.CnFInp);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyCnFRate", duty.CnFRate);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyCnFAmount", duty.CnFAmount);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyInsuranceInp", duty.InsuranceInp);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyInsuranceRate", duty.InsuranceRate);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyInsuranceAmount", duty.InsuranceAmount);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyAssessableInp", duty.AssessableInp);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyAssessableValue", duty.AssessableValue);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyCDInp", duty.CDInp);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyCDRate", duty.CDRate);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyCDAmount", duty.CDAmount);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyRDInp", duty.RDInp);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyRDRate", duty.RDRate);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyRDAmount", duty.RDAmount);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyTVBInp", duty.TVBInp);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyTVBRate", duty.TVBRate);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyTVBAmount", duty.TVBAmount);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutySDInp", duty.SDInp);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutySD", duty.SD);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutySDAmount", duty.SDAmount);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyVATInp", duty.VATInp);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyVATRate", duty.VATRate);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyVATAmount", duty.VATAmount);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyTVAInp", duty.TVAInp);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyTVARate", duty.TVARate);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyTVAAmount", duty.TVAAmount);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyATVInp", duty.ATVInp);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyATVRate", duty.ATVRate);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyATVAmount", duty.ATVAmount);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyOthersInp", duty.OthersInp);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyOthersRate", duty.OthersRate);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyOthersAmount", duty.OthersAmount);
                            cmdUpdateDuty.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy);
                            cmdUpdateDuty.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                            cmdUpdateDuty.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdUpdateDuty.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                            cmdUpdateDuty.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType);
                            cmdUpdateDuty.Parameters.AddWithValue("@MasterPost", Master.Post);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyRemarks", duty.Remarks);
                            cmdUpdateDuty.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                            cmdUpdateDuty.Parameters.AddWithValue("@dutyItemNo", duty.ItemNo);

                            transResult = (int)cmdUpdateDuty.ExecuteNonQuery();
                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUpdateNotSuccessfully);
                            }

                            #endregion update Duties
                        }
                    }


                }

                #endregion Import

                #region Transaction Not Other

                #region Transaction is TollReceive

                if (Master.TransactionType == "TollReceive")
                {

                    #region update Issue

                    sqlText = "";


                    sqlText += " update IssueHeaders set ";
                    sqlText += " IssueDateTime  = @MasterInvoiceDate,";
                    sqlText += " Comments       = @MasterComments ,";
                    sqlText += " LastModifiedBy = @MasterLastModifiedBy ,";
                    sqlText += " LastModifiedOn = @MasterLastModifiedOn,";
                    sqlText += " transactionType= @MasterTransactionType ,";
                    sqlText += " IssueReturnId  = @MasterReturnId ,";
                    sqlText += " Post           = @MasterPost ";
                    sqlText += " where  IssueNo = @MasterPurchaseInvoiceNo ";


                    SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                    cmdUpdateIssue.Transaction = transaction;

                    cmdUpdateIssue.Parameters.AddWithValue("@MasterInvoiceDate", Master.InvoiceDate);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterComments", Master.Comments);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterPost", Master.Post);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                    transResult = (int)cmdUpdateIssue.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                        MessageVM.PurchasemsgUpdateNotSuccessfully);
                    }

                    #endregion update Issue

                    #region update Receive

                    sqlText = "";


                    sqlText += " update ReceiveHeaders set";
                    sqlText += " ReceiveDateTime    = @MasterReceiveDate ,";
                    sqlText += " Comments           = @MasterComments ,";
                    sqlText += " LastModifiedBy     = @MasterLastModifiedBy ,";
                    sqlText += " LastModifiedOn     = @MasterLastModifiedOn ,";
                    sqlText += " transactionType    = @MasterTransactionType ,";
                    sqlText += " ReceiveReturnId    = @MasterReturnId ,";
                    sqlText += " Post               = @MasterPost ";
                    sqlText += " where  ReceiveNo   = @MasterPurchaseInvoiceNo";




                    SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                    cmdUpdateReceive.Transaction = transaction;

                    cmdUpdateReceive.Parameters.AddWithValue("@MasterReceiveDate", Master.ReceiveDate);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterComments", Master.Comments);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId?? Convert.DBNull);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterPost", Master.Post);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                    transResult = (int)cmdUpdateReceive.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                        MessageVM.PurchasemsgUpdateNotSuccessfully);
                    }

                    #endregion update Receive
                }

                #endregion Transaction is TollReceive

                #region Transaction is InputService

                if (Master.TransactionType == "InputService" || Master.TransactionType == "InputServiceImport")
                {
                    #region update Issue

                    sqlText = "";


                    sqlText += " update IssueHeaders set ";
                    sqlText += " IssueDateTime  = @MasterInvoiceDate,";
                    sqlText += " Comments       = @MasterComments ,";
                    sqlText += " LastModifiedBy = @MasterLastModifiedBy ,";
                    sqlText += " LastModifiedOn = @MasterLastModifiedOn,";
                    sqlText += " IssueReturnId  = @MasterReturnId ,";
                    sqlText += " transactionType= @MasterTransactionType ,";
                    sqlText += " Post           = @MasterPost ";
                    sqlText += " where  IssueNo = @MasterPurchaseInvoiceNo ";


                    SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                    cmdUpdateIssue.Transaction = transaction;

                    cmdUpdateIssue.Parameters.AddWithValue("@MasterInvoiceDate", Master.InvoiceDate);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterComments", Master.Comments);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterPost", Master.Post);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                    transResult = (int)cmdUpdateIssue.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                        MessageVM.PurchasemsgUpdateNotSuccessfully);
                    }

                    #endregion update Issue

                }

                #endregion Transaction is InputService

                #endregion Transaction Not Other


                #endregion ID check completed,update Information in Header


                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate, MessageVM.PurchasemsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table
                var lineNo = 1;
                foreach (var Item in Details.ToList())
                {
                    ////
                    Item.LineNo = lineNo.ToString();
                    lineNo++;
                    if (Master.TransactionType == "TollReceive")
                    {
                        sqlText = "";
                        sqlText += " delete FROM IssueDetails ";
                        sqlText += " WHERE IssueNo=@MasterPurchaseInvoiceNo ";

                        //sqlText += " delete FROM IssueHeaders ";
                        //sqlText += " WHERE IssueNo='" + Master.PurchaseInvoiceNo + "' ";
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();
                        #region MyRegion
                        ProductDAL bdal1 = new ProductDAL();
                        var BOMId = bdal1.GetBOMIdFromOH(Item.ItemNo, "VAT 1 (Toll Receive)", Master.ReceiveDate, currConn, transaction);
                        //sss
                        sqlText = "";
                        sqlText +=
                            " SELECT  b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ," +
                            "b.WastageQuantity,b.UOM,b.UOMUQty,b.UOMWQty,b.TransactionType from BOMRaws b  ";
                        sqlText += " where ";
                        sqlText += " BOMId='" + BOMId + "' ";
                        //sqlText += "   and (rawitemtype='raw' or rawitemtype='pack' or rawitemtype='finish') ";
                        sqlText += "   and (rawitemtype='raw' or rawitemtype='pack') ";



                        DataTable dataTable = new DataTable("RIFB");
                        SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                        cmdRIFB.Transaction = transaction;
                        SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                        reportDataAdapt.Fill(dataTable);

                        if (dataTable == null)
                        {
                            if (TollReceiveWithIssue)
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                            MessageVM.receiveMsgNoDataToPost);
                        }
                        else if (dataTable.Rows.Count <= 0)
                        {
                            if (TollReceiveWithIssue)
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                            "There is no Item for Auto Consumption for the Item Name (VAT 1 (Toll Receive)) in price declaration.");
                        }
                        else
                        {
                            decimal vQuantity = 0;
                            decimal vWastage = 0;
                            decimal vStock = 0;
                            string rwUom = "";
                            decimal vConvertionRate = 0;
                            decimal AvgRate = 0;

                            foreach (DataRow BRItem in dataTable.Rows)
                            {
                                #region Declare

                                decimal v1Quantity = 0;
                                string v1RawItemNo = "";
                                decimal v1CostPrice = 0;
                                string v1UOM = "";
                                decimal v1SubTotal = 0;
                                decimal v1Wastage = 0;
                                DateTime v1BOMDate = DateTime.Now.Date;
                                string v1FinishItemNo = "";

                                decimal v1UOMQty = 0;
                                decimal v1UOMPrice = 0;
                                decimal v1UOMc = 0;
                                string v1UOMn = "";
                                string v1BOMId = "";
                                decimal v1UOMWastage = 0;
                                string vTransactionType = "";

                                #endregion Declare

                                #region Update Item Qty

                                #region Find Quantity From Products

                                DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDate, currConn, transaction, false);

                                decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                                decimal quantity = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                                if (quantity > 0)
                                {
                                    AvgRate = amount / quantity;
                                }
                                else
                                {
                                    AvgRate = 0;
                                }

                                vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());

                                #region Issue Settings
                                //CommonDAL commDal = new CommonDAL();
                                //int IssuePlaceQty = Convert.ToInt32(commDal.settings("Issue", "Quantity"));
                                //int IssuePlaceAmt = Convert.ToInt32(commDal.settings("Issue", "Amount"));
                                AvgRate = FormatingNumeric(AvgRate, 4);
                                vQuantity = FormatingNumeric(vQuantity, 4);
                                vWastage = FormatingNumeric(vWastage, 4);
                                #endregion Issue Settings

                                rwUom = BRItem["Uom"].ToString();

                                var rwMajorUom = BRItem["Uomn"].ToString();
                                if (string.IsNullOrEmpty(rwUom))
                                {
                                    throw new ArgumentNullException("ReceiveInsert",
                                                                    "Could not find UOM of raw item");
                                }

                                /*Processing UOM*/

                                UOMDAL uomdal = new UOMDAL();
                                vConvertionRate = uomdal.GetConvertionRate(rwMajorUom, rwUom, "Y", currConn, transaction); //uomc


                                #region valueAssign

                                v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                v1Wastage = (vWastage) * Item.UOMQty;
                                v1BOMId = BOMId;
                                v1RawItemNo = BRItem["RawItemNo"].ToString();
                                v1UOM = BRItem["UOM"].ToString();
                                v1CostPrice = AvgRate * vConvertionRate;
                                v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                v1UOMPrice = AvgRate;
                                v1UOMn = BRItem["UOMn"].ToString();
                                v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                v1FinishItemNo = Item.ItemNo;
                                v1UOMc = vConvertionRate;
                                v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;
                                vTransactionType = BRItem["TransactionType"].ToString();
                                #endregion valueAssign

                                #region Stock
                                var vNegStockAllow = commonDal.settings("Issue", "NegStockAllow");
                                bool NegStockAllow = Convert.ToBoolean(vNegStockAllow == "Y" ? true : false);

                                if (NegStockAllow == false)
                                {
                                    //var stock = productDal.StockInHand(BRItem["RawItemNo"].ToString(),
                                    //                                       Master.ReceiveDateTime,
                                    //                                   currConn, transaction).ToString();

                                    var stock = Convert.ToDecimal(productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(),
                                                                           Master.ReceiveDate,
                                                         currConn, transaction, false).Rows[0]["Quantity"].ToString());

                                    vStock = Convert.ToDecimal(stock);


                                    if ((vStock - v1UOMQty) < 0)
                                    {
                                        string FinName = string.Empty;
                                        string FinCode = string.Empty;
                                        string RawName = string.Empty;
                                        string RawCode = string.Empty;
                                        DataTable finDt = new DataTable();
                                        finDt = productDal.GetProductCodeAndNameByItemNo(Item.ItemNo);
                                        foreach (DataRow FinItem in finDt.Rows)
                                        {
                                            FinName = FinItem["ProductName"].ToString();
                                            FinCode = FinItem["ProductCode"].ToString();
                                        }
                                        DataTable rawDt = new DataTable();
                                        rawDt =
                                            productDal.GetProductCodeAndNameByItemNo(BRItem["RawItemNo"].ToString());
                                        foreach (DataRow RawItem in rawDt.Rows)
                                        {
                                            RawName = RawItem["ProductName"].ToString();
                                            RawCode = RawItem["ProductCode"].ToString();
                                        }

                                        throw new ArgumentNullException("ReceiveInsert",
                                                                        "Stock not Available for Finish Item( Name: " +
                                                                        FinName + " & Code: " + FinCode +
                                                                        " ) \n and consumtion Material ( Name: " +
                                                                        RawName + " & Code: " + RawCode + " )");
                                    }
                                }

                                #endregion Stock

                                #endregion Find Quantity From Products


                                #region Find Quantity From Transaction


                                sqlText = "";
                                sqlText += " insert into IssueDetails(";

                                sqlText += " IssueNo,";
                                sqlText += " IssueLineNo,";
                                sqlText += " ItemNo,";
                                sqlText += " Quantity,";
                                sqlText += " NBRPrice,";
                                sqlText += " CostPrice,";
                                sqlText += " UOM,";
                                sqlText += " VATRate,";
                                sqlText += " VATAmount,";
                                sqlText += " SubTotal,";
                                sqlText += " Comments,";
                                sqlText += " CreatedBy,";
                                sqlText += " CreatedOn,";
                                sqlText += " LastModifiedBy,";
                                sqlText += " LastModifiedOn,";
                                sqlText += " ReceiveNo,";
                                sqlText += " IssueDateTime,";
                                sqlText += " SD,";
                                sqlText += " SDAmount,"; //19
                                sqlText += " Wastage,";
                                sqlText += " BOMDate,";
                                sqlText += " FinishItemNo,";
                                sqlText += " transactionType,";
                                sqlText += " IssueReturnId,";
                                sqlText += " UOMQty,";
                                sqlText += " UOMPrice,";
                                sqlText += " UOMc,";
                                sqlText += " UOMn,";
                                sqlText += " UOMWastage,";
                                sqlText += " BOMId,";

                                sqlText += " Post";
                                sqlText += " )";
                                sqlText += " values( ";
                                sqlText += "@MasterPurchaseInvoiceNo,";
                                sqlText += "'1',";
                                sqlText += "@v1RawItemNo, ";
                                sqlText += "@v1Quantity4 ,";
                                sqlText += "@AvgRate,";
                                sqlText += "@v1CostPrice4,";
                                sqlText += "@v1UOM,";
                                sqlText += " 0,0, ";
                                sqlText += "@v1SubTotal4,";
                                sqlText += "@ItemComments,";
                                sqlText += "@MasterCreatedBy,";
                                sqlText += "@MasterCreatedOn,";
                                sqlText += "@MasterLastModifiedBy,";
                                sqlText += "@MasterLastModifiedOn,";
                                sqlText += "@MasterPurchaseInvoiceNo,";
                                sqlText += "@MasterReceiveDate,";
                                sqlText += " 0,	0,";
                                sqlText += "@v1Wastage,	";
                                sqlText += "@v1BOMDate,	";
                                sqlText += "@v1FinishItemNo,";
                                sqlText += "@MasterTransactionType,";
                                sqlText += "@MasterReturnId,";
                                sqlText += "@v1UOMQty4,";
                                sqlText += "@v1UOMPrice4,";
                                sqlText += "@v1UOMc,";
                                sqlText += "@v1UOMn,";
                                sqlText += "@v1UOMWastage,";
                                sqlText += "@v1BOMId,";

                                sqlText += "'N'";
                                sqlText += ")";
                                SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                cmdInsertIssue.Transaction = transaction;

                                cmdInsertIssue.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                                cmdInsertIssue.Parameters.AddWithValue("@v1RawItemNo", v1RawItemNo);
                                cmdInsertIssue.Parameters.AddWithValue("@v1Quantity4", FormatingNumeric(v1Quantity, 4));
                                cmdInsertIssue.Parameters.AddWithValue("@AvgRate", AvgRate);
                                cmdInsertIssue.Parameters.AddWithValue("@v1CostPrice4", FormatingNumeric(v1CostPrice, 4));
                                cmdInsertIssue.Parameters.AddWithValue("@v1UOM", v1UOM);
                                cmdInsertIssue.Parameters.AddWithValue("@v1SubTotal4", FormatingNumeric(v1SubTotal, 4));
                                cmdInsertIssue.Parameters.AddWithValue("@ItemComments", Item.Comments);
                                cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy);
                                cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                                cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                                cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                                //////cmdInsertIssue.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                                cmdInsertIssue.Parameters.AddWithValue("@MasterReceiveDate", Master.ReceiveDate);
                                cmdInsertIssue.Parameters.AddWithValue("@v1Wastage", v1Wastage);
                                cmdInsertIssue.Parameters.AddWithValue("@v1BOMDate", Convert.ToDateTime(v1BOMDate).ToString("MM/dd/yyyy"));
                                cmdInsertIssue.Parameters.AddWithValue("@v1FinishItemNo", v1FinishItemNo);
                                cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType);
                                cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId?? Convert.DBNull);
                                cmdInsertIssue.Parameters.AddWithValue("@v1UOMQty4", FormatingNumeric(v1UOMQty, 4));
                                cmdInsertIssue.Parameters.AddWithValue("@v1UOMPrice4", FormatingNumeric(v1UOMPrice, 4));
                                cmdInsertIssue.Parameters.AddWithValue("@v1UOMc", v1UOMc);
                                cmdInsertIssue.Parameters.AddWithValue("@v1UOMn", v1UOMn);
                                cmdInsertIssue.Parameters.AddWithValue("@v1UOMWastage", v1UOMWastage);
                                cmdInsertIssue.Parameters.AddWithValue("@v1BOMId", v1BOMId);

                                transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                    MessageVM.receiveMsgUnableToSaveIssue);
                                }

                                #region Update Issue

                                sqlText = "";
                                sqlText += " update IssueHeaders set ";
                                sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                                sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                                sqlText += " where (IssueHeaders.IssueNo= @MasterPurchaseInvoiceNo)";

                                SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                                cmdUpdateIssue.Transaction = transaction;
                                cmdUpdateIssue.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                                transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                    MessageVM.receiveMsgUnableToSaveIssue);
                                }

                                #endregion Update Issue

                                #endregion Qty  check and Update

                                #endregion Qty  check and Update
                            }
                        }
                        #endregion


                    }

                    #region Find Transaction Mode Insert or Update


                    sqlText = "";
                    sqlText +=
                        "select COUNT(PurchaseInvoiceNo) from PurchaseInvoiceDetails WHERE PurchaseInvoiceNo=@MasterPurchaseInvoiceNo ";
                    sqlText += " AND ItemNo=@ItemItemNo";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;

                    cmdFindId.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);


                    decimal DetIDExist = (int)cmdFindId.ExecuteScalar();
                    if (DetIDExist <= 0)
                    {
                        // Insert

                        #region Insert only DetailTable

                        sqlText = "";
                        sqlText += " insert into PurchaseInvoiceDetails(";
                        //sqlText += " IssueNo,";
                        sqlText += " PurchaseInvoiceNo,";
                        sqlText += " POLineNo,";
                        sqlText += " ItemNo,";
                        sqlText += " Quantity,";
                        sqlText += " CostPrice,";
                        sqlText += " NBRPrice,";
                        sqlText += " UOM,";
                        sqlText += " VATRate,";
                        sqlText += " VATAmount,";
                        sqlText += " SubTotal,";
                        sqlText += " Comments,";
                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn,";
                        sqlText += " SD,";
                        sqlText += " SDAmount,";
                        sqlText += " Type,";
                        sqlText += " ProductType,";
                        sqlText += " BENumber,";
                        sqlText += " InvoiceDateTime,";
                        sqlText += " ReceiveDate,";
                        sqlText += " Post,";
                        sqlText += " UOMQty,";
                        sqlText += " UOMPrice,";
                        sqlText += " UOMc,";
                        sqlText += " UOMn,";

                        sqlText += " RebateRate ,";
                        sqlText += " RebateAmount ,";
                        sqlText += " CnFAmount ,";
                        sqlText += " InsuranceAmount ,";
                        sqlText += " AssessableValue ,";
                        sqlText += " CDAmount ,";
                        sqlText += " RDAmount ,";
                        sqlText += " TVBAmount ,";
                        sqlText += " TVAAmount ,";
                        sqlText += " ATVAmount ,";
                        sqlText += " TransactionType ,";
                        sqlText += " PurchaseReturnId ,";
                        sqlText += " OthersAmount ";
                        if (Master.TransactionType == "PurchaseCN" || Master.TransactionType == "PurchaseDN" || Master.TransactionType == "PurchaseReturn")
                        {
                            sqlText += ", ReturnTransactionType ";
                        }

                        sqlText += " )";
                        sqlText += " values(	";
                        sqlText += "@MasterPurchaseInvoiceNo,";
                        sqlText += "@ItemLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemQuantity,";
                        sqlText += "@ItemUnitPrice,";
                        sqlText += "@ItemNBRPrice,";
                        sqlText += "@ItemUOM,";
                        sqlText += "@ItemVATRate,";
                        sqlText += "@ItemVATAmount,";
                        sqlText += "@ItemSubTotal,";
                        sqlText += "@ItemComments,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "@ItemSD,";
                        sqlText += "@ItemSDAmount,";
                        sqlText += "@ItemType,";
                        sqlText += "@ItemProductType,";
                        sqlText += "@ItemBENumber,";
                        sqlText += "@MasterInvoiceDate,";
                        sqlText += "@MasterReceiveDate,";
                        sqlText += "@MasterPost,";
                        sqlText += "@ItemUOMQty,";
                        sqlText += "@ItemUOMPrice,";
                        sqlText += "@ItemUOMc,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@ItemRebateRate,";
                        sqlText += "@ItemRebateAmount,";
                        sqlText += "@ItemCnFAmount,";
                        sqlText += "@ItemInsuranceAmount,";
                        sqlText += "@ItemAssessableValue,";
                        sqlText += "@ItemCDAmount,";
                        sqlText += "@ItemRDAmount,";
                        sqlText += "@ItemTVBAmount,";
                        sqlText += "@ItemTVAAmount,";
                        sqlText += "@ItemATVAmount,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterReturnId,";
                        sqlText += "@ItemOthersAmount";

                        if (Master.TransactionType == "PurchaseCN" || Master.TransactionType == "PurchaseDN" || Master.TransactionType == "PurchaseReturn")
                        {
                            sqlText += ",@ItemReturnTransactionType ";
                        }

                        sqlText += ")	";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                        cmdInsDetail.Parameters.AddWithValue("@ItemLineNo", Item.LineNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUnitPrice", Item.UnitPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemNBRPrice", Item.NBRPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOM", Item.UOM);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVATRate", Item.VATRate);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVATAmount", Item.VATAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSubTotal", Item.SubTotal);
                        cmdInsDetail.Parameters.AddWithValue("@ItemComments", Item.Comments ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSD", Item.SD);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSDAmount", Item.SDAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemType", Item.Type ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemProductType", Item.ProductType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemBENumber", Item.BENumber ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterInvoiceDate", Master.InvoiceDate ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterReceiveDate", Master.ReceiveDate ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn);
                        cmdInsDetail.Parameters.AddWithValue("@ItemRebateRate", Item.RebateRate);
                        cmdInsDetail.Parameters.AddWithValue("@ItemRebateAmount", Item.RebateAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCnFAmount", Item.CnFAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemInsuranceAmount", Item.InsuranceAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemAssessableValue", Item.AssessableValue);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCDAmount", Item.CDAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemRDAmount", Item.RDAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemTVBAmount", Item.TVBAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemTVAAmount", Item.TVAAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemATVAmount", Item.ATVAmount);
                        cmdInsDetail.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemOthersAmount", Item.OthersAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemReturnTransactionType", Item.ReturnTransactionType ?? Convert.DBNull);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                            MessageVM.PurchasemsgUpdateNotSuccessfully);
                        }

                        #endregion Insert only DetailTable

                        #region Insert Issue and Receive if Transaction is not Other

                        #region Transaction is TollReceive

                        if (Master.TransactionType == "TollReceive")
                        {


                            string FinishItemIdFromOH = productDal.GetFinishItemIdFromOH(Item.ItemNo, "VAT 1 (Toll Receive)", Master.ReceiveDate,
                                                                                    currConn, transaction).ToString();



                            decimal NBRPrice = productDal.GetLastNBRPriceFromBOM(FinishItemIdFromOH, "VAT 1 (Toll Receive)", Master.ReceiveDate,
                                                                                currConn, transaction);

                            string ItemType = productDal.GetProductTypeByItemNo(FinishItemIdFromOH, currConn, transaction);
                            string UOM = productDal.GetProductTypeByItemNo(FinishItemIdFromOH, currConn, transaction);

                            #region Coments
                            //#region Insert to Issue

                            //sqlText = "";
                            //sqlText += " insert into IssueDetails(";
                            ////sqlText += " IssueNo,";
                            //sqlText += " IssueNo,";
                            //sqlText += " IssueLineNo,";
                            //sqlText += " ItemNo,";
                            //sqlText += " Quantity,";
                            //sqlText += " NBRPrice,";
                            //sqlText += " CostPrice,";
                            //sqlText += " UOM,";
                            //sqlText += " VATRate,";
                            //sqlText += " VATAmount,";
                            //sqlText += " SubTotal,";
                            //sqlText += " Comments,";
                            //sqlText += " CreatedBy,";
                            //sqlText += " CreatedOn,";
                            //sqlText += " LastModifiedBy,";
                            //sqlText += " LastModifiedOn,";
                            //sqlText += " ReceiveNo,";
                            //sqlText += " IssueDateTime,";
                            //sqlText += " SD,";
                            //sqlText += " SDAmount,";
                            //sqlText += " Wastage,";
                            //sqlText += " BOMDate,";
                            //sqlText += " FinishItemNo,";
                            //sqlText += " TransactionType,";
                            //sqlText += " IssueReturnId,";
                            //sqlText += " UOMQty,";
                            //sqlText += " UOMPrice,";
                            //sqlText += " UOMc,";
                            //sqlText += " UOMn,";
                            //sqlText += " UOMWastage,";
                            //sqlText += " Post";

                            ////sqlText += " Post";
                            //sqlText += " )";
                            //sqlText += " values(	";
                            ////sqlText += "'" + Master.PurchaseInvoiceNo + "',";
                            //sqlText += "'" + Master.PurchaseInvoiceNo + "',";
                            //sqlText += "'" + Item.LineNo + "',";
                            //sqlText += "'" + Item.ItemNo + "',";
                            //sqlText += "'" + Item.Quantity + "',";
                            //sqlText += " 0,";
                            //sqlText += "'" + Item.UnitPrice + "',";

                            //sqlText += "'" + Item.UOM + "',";
                            //sqlText += " 0,	";
                            //sqlText += " 0,";
                            //sqlText += " " + Item.UnitPrice + "*" + Item.UOMQty + ",";
                            //sqlText += "'" + Item.Comments + "',";
                            //sqlText += "'" + Master.CreatedBy + "',";
                            //sqlText += "'" + Master.CreatedOn + "',";
                            //sqlText += "'" + Master.LastModifiedBy + "',";
                            //sqlText += "'" + Master.LastModifiedOn + "',";
                            //sqlText += "'" + Master.PurchaseInvoiceNo + "',";
                            //sqlText += "'" + Master.ReceiveDate + "',";
                            //sqlText += " 0,";
                            //sqlText += " 0,";
                            //sqlText += " 0,	";
                            //sqlText += " 0,	";
                            //sqlText += " 0,";
                            //sqlText += "'" + Master.TransactionType + "',";

                            //sqlText += "'" + Master.ReturnId + "',";
                            //sqlText += "'" + Item.UOMQty + "',";
                            //sqlText += "'" + Item.UOMPrice + "',";
                            //sqlText += "'" + Item.UOMc + "',";
                            //sqlText += "'" + Item.UOMn + "',";
                            //sqlText += "'0',";
                            //sqlText += "'" + Master.Post + "'";

                            ////sqlText += "'" + Master.@Post + "'";
                            //sqlText += ")	";
                            //SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                            //cmdInsertIssue.Transaction = transaction;
                            //transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                            //if (transResult <= 0)
                            //{
                            //    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                            //                                    MessageVM.PurchasemsgUnableToUpdateIssue);
                            //}

                            //#endregion Insert to Issue

                            //#region Update Issue

                            //sqlText = "";
                            //sqlText += " update IssueHeaders set ";
                            //sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                            //sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                            //sqlText += " where (IssueHeaders.IssueNo= '" + Master.PurchaseInvoiceNo + "')";

                            //SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                            //cmdUpdateIssue.Transaction = transaction;
                            //int UpdateIssue = (int)cmdUpdateIssue.ExecuteNonQuery();

                            //if (UpdateIssue <= 0)
                            //{
                            //    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                            //                                    MessageVM.PurchasemsgUnableToUpdateIssue);
                            //}

                            //#endregion Update Issue
                            #endregion Coments

                            if (!string.IsNullOrEmpty(ItemType))
                            {
                                if (ItemType == "Finish")
                                {

                                    #region Insert to Receive


                                    sqlText = "";
                                    sqlText += " insert into ReceiveDetails(";
                                    //sqlText += " IssueNo,";
                                    sqlText += " ReceiveNo,";
                                    sqlText += " ReceiveLineNo,";
                                    sqlText += " ItemNo,";
                                    sqlText += " Quantity,";
                                    sqlText += " CostPrice,";
                                    sqlText += " NBRPrice,";
                                    sqlText += " UOM,";
                                    sqlText += " VATRate,";
                                    sqlText += " VATAmount,";
                                    sqlText += " SubTotal,";
                                    sqlText += " Comments,";
                                    sqlText += " CreatedBy,";
                                    sqlText += " CreatedOn,";
                                    sqlText += " LastModifiedBy,";
                                    sqlText += " LastModifiedOn,";
                                    sqlText += " SD,";
                                    sqlText += " SDAmount,";
                                    sqlText += " ReceiveDateTime,";
                                    sqlText += " TransactionType,";
                                    sqlText += " ReceiveReturnId,";
                                    sqlText += " VATName,";
                                    sqlText += " UOMQty,";
                                    sqlText += " UOMPrice,";
                                    sqlText += " UOMc,";
                                    sqlText += " UOMn,";
                                    sqlText += " Post";

                                    //sqlText += " Post";
                                    sqlText += " )";
                                    sqlText += " values(	";
                                    sqlText += "@MasterPurchaseInvoiceNo,";
                                    sqlText += "@ItemLineNo,";
                                    sqlText += "@FinishItemIdFromOH,";
                                    sqlText += "@ItemQuantity,";
                                    sqlText += "@NBRPrice,";
                                    sqlText += "@NBRPrice,";
                                    sqlText += "@UOM,";
                                    sqlText += " 0,";
                                    sqlText += " 0,";
                                    sqlText += "@NBRPriceItemUOMQty,";
                                    sqlText += "@ItemComments,";
                                    sqlText += "@MasterCreatedBy,";
                                    sqlText += "@MasterCreatedOn,";
                                    sqlText += "@MasterLastModifiedBy,";
                                    sqlText += "@MasterLastModifiedOn,";
                                    sqlText += "0,";
                                    sqlText += "0,";
                                    sqlText += "@MasterReceiveDate,";
                                    sqlText += "@MasterTransactionType,";
                                    sqlText += "@MasterReturnId,";
                                    sqlText += "'VAT 1 (Toll Receive)',";
                                    sqlText += "@ItemUOMQty,";
                                    sqlText += "@NBRPrice,";
                                    sqlText += "'1',";
                                    sqlText += "@UOM,";
                                    sqlText += "@MasterPost";

                                    sqlText += ")	";
                                    SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                                    cmdInsertReceive.Transaction = transaction;

                                    cmdInsertReceive.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                                    cmdInsertReceive.Parameters.AddWithValue("@ItemLineNo", Item.LineNo);
                                    cmdInsertReceive.Parameters.AddWithValue("@FinishItemIdFromOH", FinishItemIdFromOH);
                                    cmdInsertReceive.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                                    cmdInsertReceive.Parameters.AddWithValue("@NBRPrice", NBRPrice);
                                    cmdInsertReceive.Parameters.AddWithValue("@NBRPrice", NBRPrice);
                                    cmdInsertReceive.Parameters.AddWithValue("@UOM", UOM);
                                    cmdInsertReceive.Parameters.AddWithValue("@NBRPriceItemUOMQty", NBRPrice * Item.UOMQty);
                                    cmdInsertReceive.Parameters.AddWithValue("@ItemComments", Item.Comments);
                                    cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy);
                                    cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                                    cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                                    cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                                    cmdInsertReceive.Parameters.AddWithValue("@MasterReceiveDate", Master.ReceiveDate);
                                    cmdInsertReceive.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType);
                                    cmdInsertReceive.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId?? Convert.DBNull);
                                    cmdInsertReceive.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                                    cmdInsertReceive.Parameters.AddWithValue("@NBRPrice", NBRPrice);
                                    cmdInsertReceive.Parameters.AddWithValue("@UOM", UOM);
                                    cmdInsertReceive.Parameters.AddWithValue("@MasterPost", Master.Post);

                                    transResult = (int)cmdInsertReceive.ExecuteNonQuery();

                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                        MessageVM.PurchasemsgUnableToUpdateReceive);
                                    }

                                    #endregion Insert to Receive

                                    #region Update Receive

                                    sqlText = "";


                                    sqlText += "  update ReceiveHeaders set TotalAmount=  ";
                                    sqlText += " (select sum(Quantity*CostPrice) from ReceiveDetails ";
                                    sqlText += " where ReceiveDetails.ReceiveNo =ReceiveHeaders.ReceiveNo) ";
                                    sqlText += " where ReceiveHeaders.ReceiveNo=@MasterPurchaseInvoiceNo ";


                                    SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                                    cmdUpdateReceive.Transaction = transaction;

                                    cmdUpdateReceive.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                                    transResult = (int)cmdUpdateReceive.ExecuteNonQuery();

                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                        MessageVM.PurchasemsgUnableToUpdateReceive);
                                    }

                                    #endregion Update Receive
                                }
                                else if (ItemType == "Raw"  //16 in
                                  || ItemType == "Pack"
                               || ItemType == "WIP"
                               || ItemType == "Trading")
                                {
                                    #region Insert only DetailTable PurchaseInvoiceDetails

                                    sqlText = "";
                                    sqlText += " insert into PurchaseInvoiceDetails(";
                                    sqlText += " PurchaseInvoiceNo,";
                                    sqlText += " POLineNo,";
                                    sqlText += " ItemNo,";
                                    sqlText += " Quantity,";
                                    sqlText += " CostPrice,";
                                    sqlText += " NBRPrice,";
                                    sqlText += " UOM,";
                                    sqlText += " VATRate,";
                                    sqlText += " VATAmount,";
                                    sqlText += " SubTotal,";
                                    sqlText += " Comments,";
                                    sqlText += " CreatedBy,";
                                    sqlText += " CreatedOn,";
                                    sqlText += " LastModifiedBy,";
                                    sqlText += " LastModifiedOn,";
                                    sqlText += " SD,";
                                    sqlText += " SDAmount,";
                                    sqlText += " Type,";
                                    sqlText += " ProductType,";
                                    sqlText += " BENumber,";
                                    sqlText += " InvoiceDateTime,";
                                    sqlText += " ReceiveDate,";
                                    sqlText += " Post,";
                                    sqlText += " UOMQty,";
                                    sqlText += " UOMPrice,";
                                    sqlText += " UOMc,";
                                    sqlText += " UOMn,";
                                    sqlText += " RebateRate ,";
                                    sqlText += " RebateAmount ,";
                                    sqlText += " CnFAmount ,";
                                    sqlText += " InsuranceAmount ,";
                                    sqlText += " AssessableValue ,";
                                    sqlText += " CDAmount ,";
                                    sqlText += " RDAmount ,";
                                    sqlText += " TVBAmount ,";
                                    sqlText += " TVAAmount ,";
                                    sqlText += " ATVAmount ,";
                                    sqlText += " TransactionType ,";
                                    sqlText += " PurchaseReturnId ,";
                                    sqlText += " OthersAmount ";
                                    if (Master.TransactionType == "PurchaseCN" || Master.TransactionType == "PurchaseDN")
                                    {
                                        sqlText += ", ReturnTransactionType ";
                                    }


                                    sqlText += " )";
                                    sqlText += " values(	";
                                    sqlText += "@MasterPurchaseInvoiceNo,";
                                    sqlText += "@ItemLineNo,";
                                    sqlText += "@FinishItemIdFromOH,";
                                    sqlText += "@ItemQuantity,";
                                    sqlText += "@NBRPrice,";
                                    sqlText += "@NBRPrice,";
                                    sqlText += "@UOM,";
                                    sqlText += "0,";
                                    sqlText += "0,";
                                    sqlText += "@NBRPriceItemUOMQty ,";
                                    sqlText += "@ItemComments,";
                                    sqlText += "@MasterCreatedBy,";
                                    sqlText += "@MasterCreatedOn,";
                                    sqlText += "@MasterLastModifiedBy,";
                                    sqlText += "@MasterLastModifiedOn,";
                                    sqlText += "0,";
                                    sqlText += "0,";
                                    sqlText += "'TollReceive-WIP',";
                                    sqlText += "@ItemProductType,";
                                    sqlText += "@ItemBENumber,";
                                    sqlText += "@MasterInvoiceDate,";
                                    sqlText += "@MasterReceiveDate,";
                                    sqlText += "@MasterPost,";
                                    sqlText += "@ItemUOMQty,";
                                    sqlText += "@NBRPrice,";
                                    sqlText += "1,";
                                    sqlText += "@UOM,";
                                    sqlText += "0,";
                                    sqlText += "0,";
                                    sqlText += "0,";
                                    sqlText += "0,";
                                    sqlText += "0,";
                                    sqlText += "0,";
                                    sqlText += "0,";
                                    sqlText += "0,";
                                    sqlText += "0,";
                                    sqlText += "0,";
                                    sqlText += "'TollReceive-WIP',";
                                    sqlText += "@MasterReturnId,";
                                    sqlText += "@ItemOthersAmount";
                                    if (Master.TransactionType == "PurchaseCN" || Master.TransactionType == "PurchaseDN")
                                    {
                                        sqlText += ",@ItemReturnTransactionType ";
                                    }

                                    sqlText += ")	";


                                    SqlCommand cmdInsDetailW = new SqlCommand(sqlText, currConn);
                                    cmdInsDetailW.Transaction = transaction;

                                    cmdInsDetailW.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                                    cmdInsDetailW.Parameters.AddWithValue("@ItemLineNo", Item.LineNo);
                                    cmdInsDetailW.Parameters.AddWithValue("@FinishItemIdFromOH", FinishItemIdFromOH);
                                    cmdInsDetailW.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                                    cmdInsDetailW.Parameters.AddWithValue("@NBRPrice", NBRPrice);
                                    //////cmdInsDetailW.Parameters.AddWithValue("@NBRPrice", NBRPrice);
                                    cmdInsDetailW.Parameters.AddWithValue("@UOM", UOM);
                                    cmdInsDetailW.Parameters.AddWithValue("@NBRPriceItemUOMQty", NBRPrice * Item.UOMQty);
                                    cmdInsDetailW.Parameters.AddWithValue("@ItemComments", Item.Comments ?? Convert.DBNull);
                                    cmdInsDetailW.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                                    cmdInsDetailW.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                                    cmdInsDetailW.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                                    cmdInsDetailW.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                                    cmdInsDetailW.Parameters.AddWithValue("@ItemProductType", Item.ProductType ?? Convert.DBNull);
                                    cmdInsDetailW.Parameters.AddWithValue("@ItemBENumber", Item.BENumber ?? Convert.DBNull);
                                    cmdInsDetailW.Parameters.AddWithValue("@MasterInvoiceDate", Master.InvoiceDate ?? Convert.DBNull);
                                    cmdInsDetailW.Parameters.AddWithValue("@MasterReceiveDate", Master.ReceiveDate ?? Convert.DBNull);
                                    cmdInsDetailW.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                                    cmdInsDetailW.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                                    //////cmdInsDetailW.Parameters.AddWithValue("@NBRPrice", NBRPrice);
                                    cmdInsDetailW.Parameters.AddWithValue("@UOM", UOM);
                                    cmdInsDetailW.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                                    cmdInsDetailW.Parameters.AddWithValue("@ItemOthersAmount", Item.OthersAmount);
                                    cmdInsDetailW.Parameters.AddWithValue("@ItemReturnTransactionType", Item.ReturnTransactionType ?? Convert.DBNull);

                                    transResult = (int)cmdInsDetailW.ExecuteNonQuery();

                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                        MessageVM.PurchasemsgSaveNotSuccessfully);
                                    }

                                    #endregion Insert only DetailTable

                                }



                            }

                        }

                        #endregion Transaction is TollReceive

                        #region Transaction is InputService

                        if (Master.TransactionType == "InputService" || Master.TransactionType == "InputServiceImport")
                        {
                            decimal PurchasePrice = productDal.PurchasePrice(Item.ItemNo, Master.PurchaseInvoiceNo, currConn, transaction);

                            #region Insert to Issue

                            sqlText = "";
                            sqlText += " insert into IssueDetails(";
                            //sqlText += " IssueNo,";
                            sqlText += " IssueNo,";
                            sqlText += " IssueLineNo,";
                            sqlText += " ItemNo,";
                            sqlText += " Quantity,";
                            sqlText += " NBRPrice,";
                            sqlText += " CostPrice,";
                            sqlText += " UOM,";
                            sqlText += " VATRate,";
                            sqlText += " VATAmount,";
                            sqlText += " SubTotal,";
                            sqlText += " Comments,";
                            sqlText += " CreatedBy,";
                            sqlText += " CreatedOn,";
                            sqlText += " LastModifiedBy,";
                            sqlText += " LastModifiedOn,";
                            sqlText += " ReceiveNo,";
                            sqlText += " IssueDateTime,";
                            sqlText += " SD,";
                            sqlText += " SDAmount,";
                            sqlText += " Wastage,";
                            sqlText += " BOMDate,";
                            sqlText += " FinishItemNo,";
                            sqlText += " TransactionType,";
                            sqlText += " IssueReturnId,";
                            sqlText += " UOMQty,";
                            sqlText += " UOMPrice,";
                            sqlText += " UOMc,";
                            sqlText += " UOMn,";
                            sqlText += " UOMWastage,";
                            sqlText += " Post";

                            sqlText += " )";
                            sqlText += " values(	";
                            sqlText += "@MasterPurchaseInvoiceNo,";
                            sqlText += "@ItemLineNo,";
                            sqlText += "@ItemItemNo,";
                            sqlText += "@ItemQuantity,";
                            sqlText += " 0,";

                            if (Master.TransactionType == "InputServiceImport")
                            {
                                PurchasePrice = PurchasePrice + Convert.ToDecimal(Item.ATVAmount) + Convert.ToDecimal(Item.TVAAmount);
                                sqlText += "@PurchasePrice ,";
                                sqlText += "@ItemUOM,";
                                sqlText += " 0,	";
                                sqlText += " 0,";
                                sqlText += " @PurchasePriceItemUOMQty,";
                            }
                            else if (Master.TransactionType == "InputService")
                            {
                                sqlText += "@ItemSubTotal,";
                                sqlText += "@ItemUOM,";
                                sqlText += " 0,	";
                                sqlText += " 0,";
                                sqlText += "@ItemSubTotalItemUOMQty,";
                            }


                            sqlText += "@ItemComments,";
                            sqlText += "@MasterCreatedBy,";
                            sqlText += "@MasterCreatedOn,";
                            sqlText += "@MasterLastModifiedBy,";
                            sqlText += "@MasterLastModifiedOn,";
                            sqlText += "@MasterPurchaseInvoiceNo,";
                            sqlText += "@MasterReceiveDate,";
                            sqlText += " 0,";
                            sqlText += " 0,";
                            sqlText += " 0,	";
                            sqlText += " 0,	";
                            sqlText += " 0,";
                            sqlText += "@MasterTransactionType,";
                            sqlText += "@MasterReturnId,";
                            sqlText += "@ItemUOMQty,";
                            sqlText += "@ItemUOMPrice,";
                            sqlText += "@ItemUOMc,";
                            sqlText += "@ItemUOMn,";
                            sqlText += "'0',";
                            sqlText += "@MasterPost";

                            sqlText += ")	";
                            SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                            cmdInsertIssue.Transaction = transaction;

                            cmdInsertIssue.Parameters.AddWithValue("@MasterPurchaseInvoiceNo ", Master.PurchaseInvoiceNo);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemLineNo", Item.LineNo);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemComments", Item.Comments);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                            //////cmdInsertIssue.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterReceiveDate", Master.ReceiveDate);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMn", Item.UOMn);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post);

                            if (Master.TransactionType == "InputServiceImport")
                            {
                                PurchasePrice = PurchasePrice + Convert.ToDecimal(Item.ATVAmount) + Convert.ToDecimal(Item.TVAAmount);
                                cmdInsertIssue.Parameters.AddWithValue("@PurchasePrice", PurchasePrice);
                                cmdInsertIssue.Parameters.AddWithValue("@ItemUOM", Item.UOM);
                                cmdInsertIssue.Parameters.AddWithValue("@PurchasePriceItemUOMQty", PurchasePrice * Item.UOMQty);
                            }
                            else if (Master.TransactionType == "InputService")
                            {
                                cmdInsertIssue.Parameters.AddWithValue("@ItemSubTotal", Item.SubTotal);
                                cmdInsertIssue.Parameters.AddWithValue("@ItemUOM", Item.UOM);
                                cmdInsertIssue.Parameters.AddWithValue("@ItemSubTotalItemUOMQty", Item.SubTotal * Item.UOMQty);
                            }
                            transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUnableToUpdateIssue);
                            }

                            #endregion Insert to Issue

                            #region Update Issue

                            sqlText = "";
                            sqlText += " update IssueHeaders set ";
                            sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                            sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                            sqlText += " where (IssueHeaders.IssueNo= @MasterPurchaseInvoiceNo)";

                            SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                            cmdUpdateIssue.Transaction = transaction;
                            cmdUpdateIssue.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                            int UpdateIssue = (int)cmdUpdateIssue.ExecuteNonQuery();

                            if (UpdateIssue <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUnableToUpdateIssue);
                            }

                            #endregion Update Issue

                        }

                        #endregion Transaction is InputService

                        #endregion Insert Issue and Receive if Transaction is not Other
                    }
                    else
                    {
                        //Update

                        #region Update only DetailTable

                        sqlText = "";

                        sqlText += " update PurchaseInvoiceDetails set ";
                        sqlText += " POLineNo           =@ItemLineNo,";
                        sqlText += " Quantity           =@ItemQuantity,";
                        sqlText += " CostPrice          =@ItemUnitPrice,";
                        sqlText += " NBRPrice           =@ItemNBRPrice,";
                        sqlText += " UOM                =@ItemUOM,";
                        sqlText += " VATRate            =@ItemVATRate,";
                        sqlText += " VATAmount          =@ItemVATAmount,";
                        sqlText += " SubTotal           =@ItemSubTotal,";
                        sqlText += " Comments           =@ItemComments,";
                        sqlText += " LastModifiedBy     =@MasterLastModifiedBy,";
                        sqlText += " LastModifiedOn     =@MasterLastModifiedOn,";
                        sqlText += " SD                 =@ItemSD,";
                        sqlText += " SDAmount           =@ItemSDAmount,";
                        sqlText += " Type               =@ItemType,";
                        sqlText += " ProductType        =@ItemProductType,";
                        sqlText += " BENumber           =@ItemBENumber,";
                        sqlText += " InvoiceDateTime    =@MasterInvoiceDate,";
                        sqlText += " ReceiveDate        =@MasterReceiveDate,";
                        sqlText += " Post               =@MasterPost,";
                        sqlText += " UOMQty             =@ItemUOMQty,";
                        sqlText += " UOMPrice           =@ItemUOMPrice,";
                        sqlText += " UOMc               =@ItemUOMc,";
                        sqlText += " UOMn               =@ItemUOMn,";
                        sqlText += " RebateRate         =@ItemRebateRate,";
                        sqlText += " RebateAmount       =@ItemRebateAmount,";
                        sqlText += " CnFAmount          =@ItemCnFAmount,";
                        sqlText += " InsuranceAmount    =@ItemInsuranceAmount,";
                        sqlText += " AssessableValue    =@ItemAssessableValue,";
                        sqlText += " CDAmount           =@ItemCDAmount,";
                        sqlText += " RDAmount           =@ItemRDAmount,";
                        sqlText += " TVBAmount          =@ItemTVBAmount,";
                        sqlText += " TVAAmount          =@ItemTVAAmount,";
                        sqlText += " ATVAmount          =@ItemATVAmount,";
                        sqlText += " PurchaseReturnId   =@MasterReturnId,";
                        sqlText += " TransactionType    =@MasterTransactionType,";
                        sqlText += " OthersAmount       =@ItemOthersAmount";

                        if (Master.TransactionType == "PurchaseCN" || Master.TransactionType == "PurchaseDN" || Master.TransactionType == "PurchaseReturn")
                        {
                            sqlText += ",ReturnTransactionType =@ItemReturnTransactionType";
                        }

                        sqlText += " where  PurchaseInvoiceNo =@MasterPurchaseInvoiceNo ";
                        sqlText += " and ItemNo = @ItemItemNo";



                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValue("@ItemLineNo", Item.LineNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUnitPrice", Item.UnitPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemNBRPrice", Item.NBRPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVATRate", Item.VATRate);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVATAmount", Item.VATAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSubTotal", Item.SubTotal);
                        cmdInsDetail.Parameters.AddWithValue("@ItemComments", Item.Comments ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSD", Item.SD);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSDAmount", Item.SDAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemType", Item.Type ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemProductType", Item.ProductType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemBENumber", Item.BENumber ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterInvoiceDate", Master.InvoiceDate ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterReceiveDate", Master.ReceiveDate ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemRebateRate", Item.RebateRate);
                        cmdInsDetail.Parameters.AddWithValue("@ItemRebateAmount", Item.RebateAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCnFAmount", Item.CnFAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemInsuranceAmount", Item.InsuranceAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemAssessableValue", Item.AssessableValue);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCDAmount", Item.CDAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemRDAmount", Item.RDAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemTVBAmount", Item.TVBAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemTVAAmount", Item.TVAAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemATVAmount", Item.ATVAmount);
                        cmdInsDetail.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType);
                        cmdInsDetail.Parameters.AddWithValue("@ItemOthersAmount", Item.OthersAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemReturnTransactionType", Item.ReturnTransactionType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);


                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                            MessageVM.PurchasemsgUpdateNotSuccessfully);
                        }

                        #endregion Update only DetailTable

                        #region Update Issue and Receive if Transaction is not Other


                        #region Transaction is TollReceive

                        if (Master.TransactionType == "TollReceive")
                        {


                            string FinishItemIdFromOH = productDal.GetFinishItemIdFromOH(Item.ItemNo, "VAT 1 (Toll Receive)", Master.ReceiveDate,
                                                                                    currConn, transaction).ToString();



                            decimal NBRPrice = productDal.GetLastNBRPriceFromBOM(FinishItemIdFromOH, "VAT 1 (Toll Receive)", Master.ReceiveDate,
                                                                                currConn, transaction);

                            string ItemType = productDal.GetProductTypeByItemNo(FinishItemIdFromOH, currConn, transaction);
                            string UOM = productDal.GetProductTypeByItemNo(FinishItemIdFromOH, currConn, transaction);

                            #region Comments
                            //#region Update to Issue

                            //sqlText = "";
                            //sqlText += "select COUNT(IssueNo) from IssueDetails WHERE IssueNo='" + Master.PurchaseInvoiceNo + "' ";
                            //sqlText += " AND ItemNo='" + Item.ItemNo + "'";
                            //SqlCommand cmdFindIdIssue = new SqlCommand(sqlText, currConn);
                            //cmdFindIdIssue.Transaction = transaction;
                            //decimal IDExist = (int)cmdFindIdIssue.ExecuteScalar();
                            //if (IDExist <= 0)
                            //{
                            //    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                            //                                    MessageVM.PurchasemsgUpdateNotSuccessfully);
                            //}

                            //sqlText = "";
                            //sqlText += " update IssueDetails set";
                            //sqlText += " IssueLineNo='" + Item.LineNo + "',";
                            //sqlText += " Quantity=" + Item.Quantity + ",";
                            //sqlText += " CostPrice='" + Item.UnitPrice + "',";
                            //sqlText += " SubTotal=" + Item.UnitPrice + "* " + Item.UOMQty + ",";
                            //sqlText += " Comments='" + Item.Comments + "',";
                            //sqlText += " uom='" + Item.UOM + "',";
                            //sqlText += " LastModifiedBy='" + Master.LastModifiedBy + "',";
                            //sqlText += " LastModifiedOn='" + Master.LastModifiedOn + "',";
                            //sqlText += " IssueDateTime='" + Master.ReceiveDate + "',";
                            //sqlText += " TransactionType='" + Master.TransactionType + "',";
                            //sqlText += " IssueReturnId='" + Master.ReturnId + "',";

                            //sqlText += " UOMQty= " + Item.UOMQty + ",";
                            //sqlText += " UOMPrice= " + Item.UOMPrice + ",";
                            //sqlText += " UOMc= " + Item.UOMc + ",";
                            //sqlText += " UOMn= '" + Item.UOMn + "',";
                            //sqlText += " UOMWastage= '0',";

                            //sqlText += " Post='" + Master.Post + "'";
                            //sqlText += " where  IssueNo ='" + Master.PurchaseInvoiceNo + "' and ItemNo = '" + Item.ItemNo + "'";

                            ////sqlText += "'" + Master.@Post + "'";
                            //SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                            //cmdInsertIssue.Transaction = transaction;
                            //transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                            //if (transResult <= 0)
                            //{
                            //    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                            //                                    MessageVM.PurchasemsgUnableToUpdateIssue);
                            //}

                            //#endregion Update to Issue

                            //#region Update Issue Header

                            //sqlText = "";
                            //sqlText += " update IssueHeaders set ";
                            //sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                            //sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                            //sqlText += " where (IssueHeaders.IssueNo= '" + Master.PurchaseInvoiceNo + "')";

                            //SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                            //cmdUpdateIssue.Transaction = transaction;
                            //transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                            //if (transResult <= 0)
                            //{
                            //    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                            //                                    MessageVM.PurchasemsgUnableToUpdateIssue);
                            //}

                            //#endregion Update Issue Header
                            #endregion Comments

                            if (!string.IsNullOrEmpty(ItemType))
                            {
                                if (ItemType == "Finish")
                                {
                                    #region Update to Receive



                                    sqlText = "";
                                    sqlText += "select COUNT(ReceiveNo) from ReceiveDetails WHERE ReceiveNo='" +
                                               Master.PurchaseInvoiceNo +
                                               "' ";
                                    sqlText += " AND ItemNo='" + FinishItemIdFromOH + "'";
                                    SqlCommand cmdFindIdReceive = new SqlCommand(sqlText, currConn);
                                    cmdFindIdReceive.Transaction = transaction;
                                    decimal IDExist = (int)cmdFindIdReceive.ExecuteScalar();
                                    if (IDExist <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                        MessageVM.PurchasemsgUpdateNotSuccessfully);
                                    }
                                    sqlText = "";
                                    sqlText += " update ReceiveDetails set ";
                                    sqlText += " ReceiveLineNo      =@ItemLineNo,";
                                    sqlText += " Quantity           =@ItemQuantity,";
                                    sqlText += " CostPrice          =@NBRPrice,";
                                    sqlText += " NBRPrice           =@NBRPrice,";
                                    sqlText += " UOM                =@UOM,";
                                    sqlText += " SubTotal           =@NBRPriceItemUOMQty,";
                                    sqlText += " Comments           =@ItemComments,";
                                    sqlText += " LastModifiedBy     =@MasterLastModifiedBy,";
                                    sqlText += " LastModifiedOn     =@MasterLastModifiedOn,";
                                    sqlText += " ReceiveDateTime    =@MasterReceiveDate,";
                                    sqlText += " TransactionType    =@MasterTransactionType,";
                                    sqlText += " ReceiveReturnId    =@MasterReturnId,";
                                    sqlText += " VATName            = 'VAT 1 (Toll Receive)',";

                                    sqlText += " UOMQty             = @ItemUOMQty,";
                                    sqlText += " UOMPrice           = @NBRPrice,";
                                    sqlText += " UOMc               = '1',";
                                    sqlText += " UOMn               = @UOM,";
                                    sqlText += " Post               = @MasterPost";
                                    sqlText += " where  ReceiveNo   = @MasterPurchaseInvoiceNo ";
                                    sqlText += " and ItemNo         = @FinishItemIdFromOH";
                                    SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                                    cmdInsertReceive.Transaction = transaction;

                                    cmdInsertReceive.Parameters.AddWithValue("@ItemLineNo", Item.LineNo);
                                    cmdInsertReceive.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                                    cmdInsertReceive.Parameters.AddWithValue("@NBRPrice", NBRPrice);
                                    cmdInsertReceive.Parameters.AddWithValue("@UOM", UOM);
                                    cmdInsertReceive.Parameters.AddWithValue("@NBRPriceItemUOMQty", NBRPrice * Item.UOMQty);
                                    cmdInsertReceive.Parameters.AddWithValue("@ItemComments", Item.Comments);
                                    cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                                    cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                                    cmdInsertReceive.Parameters.AddWithValue("@MasterReceiveDate", Master.ReceiveDate);
                                    cmdInsertReceive.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType);
                                    cmdInsertReceive.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId?? Convert.DBNull);
                                    cmdInsertReceive.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                                    cmdInsertReceive.Parameters.AddWithValue("@NBRPrice", NBRPrice);
                                    cmdInsertReceive.Parameters.AddWithValue("@UOM", UOM);
                                    cmdInsertReceive.Parameters.AddWithValue("@MasterPost", Master.Post);
                                    cmdInsertReceive.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                                    cmdInsertReceive.Parameters.AddWithValue("@FinishItemIdFromOH", FinishItemIdFromOH);

                                    transResult = (int)cmdInsertReceive.ExecuteNonQuery();

                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                        MessageVM.PurchasemsgUnableToUpdateReceive);
                                    }

                                    #endregion Update to Receive

                                    #region Update Receive Header

                                    sqlText = "";
                                    sqlText += "  update ReceiveHeaders set TotalAmount=  ";
                                    sqlText += " (select sum(Quantity*CostPrice) from ReceiveDetails ";
                                    sqlText += " where ReceiveDetails.ReceiveNo =ReceiveHeaders.ReceiveNo) ";
                                    sqlText += " where ReceiveHeaders.ReceiveNo=@MasterPurchaseInvoiceNo";

                                    SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                                    cmdUpdateReceive.Transaction = transaction;
                                    cmdUpdateReceive.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                                    transResult = (int)cmdUpdateReceive.ExecuteNonQuery();

                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                        MessageVM.PurchasemsgUnableToUpdateReceive);
                                    }

                                    #endregion Update Receive
                                }
                                else if (ItemType == "Raw"  //16 in
                                  || ItemType == "Pack"
                               || ItemType == "WIP"
                               || ItemType == "Trading")
                                {
                                    #region Update only DetailTable PurchaseInvoiceDetails

                                    sqlText = "";

                                    sqlText += " update PurchaseInvoiceDetails set ";
                                    sqlText += " POLineNo                   =@ItemLineNo,";
                                    sqlText += " Quantity                   =@ItemQuantity,";
                                    sqlText += " CostPrice                  =@NBRPrice,";
                                    sqlText += " NBRPrice                   =@NBRPrice,";
                                    sqlText += " UOM                        =@UOM,";
                                    sqlText += " SubTotal                   =@NBRPriceItemUOMQty,";
                                    sqlText += " Comments                   =@ItemComments,";
                                    sqlText += " LastModifiedBy             =@MasterLastModifiedBy,";
                                    sqlText += " LastModifiedOn             =@MasterLastModifiedOn,";
                                    sqlText += " ProductType                =@ItemProductType,";
                                    sqlText += " BENumber                   =@ItemBENumber,";
                                    sqlText += " InvoiceDateTime            =@MasterInvoiceDate,";
                                    sqlText += " ReceiveDate                =@MasterReceiveDate,";
                                    sqlText += " Post                       =@MasterPost,";
                                    sqlText += " UOMQty                     =@ItemUOMQty,";
                                    sqlText += " UOMPrice                   =@NBRPrice,";
                                    sqlText += " UOMc                       = '1',";
                                    sqlText += " UOMn                       =@UOM,";
                                    sqlText += " PurchaseReturnId           =@MasterReturnId,";
                                    sqlText += " OthersAmount               =@ItemOthersAmount";
                                    sqlText += " where  PurchaseInvoiceNo   =@MasterPurchaseInvoiceNo ";
                                    sqlText += " and ItemNo                 =@FinishItemIdFromOH";
                                    sqlText += " and TransactionType        = 'TollReceive-WIP'";


                                    SqlCommand cmdInsDetailW = new SqlCommand(sqlText, currConn);
                                    cmdInsDetailW.Transaction = transaction;

                                    cmdInsDetailW.Parameters.AddWithValue("@ItemLineNo", Item.LineNo);
                                    cmdInsDetailW.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                                    cmdInsDetailW.Parameters.AddWithValue("@NBRPrice", NBRPrice);
                                    //////cmdInsDetailW.Parameters.AddWithValue("@NBRPrice", NBRPrice);
                                    cmdInsDetailW.Parameters.AddWithValue("@UOM", UOM);
                                    cmdInsDetailW.Parameters.AddWithValue("@NBRPriceItemUOMQty", NBRPrice * Item.UOMQty);
                                    cmdInsDetailW.Parameters.AddWithValue("@ItemComments", Item.Comments ?? Convert.DBNull);
                                    cmdInsDetailW.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                                    cmdInsDetailW.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                                    cmdInsDetailW.Parameters.AddWithValue("@ItemProductType", Item.ProductType ?? Convert.DBNull);
                                    cmdInsDetailW.Parameters.AddWithValue("@ItemBENumber", Item.BENumber ?? Convert.DBNull);
                                    cmdInsDetailW.Parameters.AddWithValue("@MasterInvoiceDate", Master.InvoiceDate ?? Convert.DBNull);
                                    cmdInsDetailW.Parameters.AddWithValue("@MasterReceiveDate", Master.ReceiveDate ?? Convert.DBNull);
                                    cmdInsDetailW.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                                    cmdInsDetailW.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                                    //////cmdInsDetailW.Parameters.AddWithValue("@NBRPrice", NBRPrice);
                                    cmdInsDetailW.Parameters.AddWithValue("@UOM", UOM);
                                    cmdInsDetailW.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                                    cmdInsDetailW.Parameters.AddWithValue("@ItemOthersAmount", Item.OthersAmount);
                                    cmdInsDetailW.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo ?? Convert.DBNull);
                                    cmdInsDetailW.Parameters.AddWithValue("@FinishItemIdFromOH", FinishItemIdFromOH);

                                    transResult = (int)cmdInsDetailW.ExecuteNonQuery();

                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                        MessageVM.PurchasemsgUpdateNotSuccessfully);
                                    }

                                    #endregion Update only DetailTable
                                }

                            }



                        }

                        #endregion Transaction is TollReceive

                        #region Transaction is InputService

                        if (Master.TransactionType == "InputService" || Master.TransactionType == "InputServiceImport")
                        {
                            decimal PurchasePrice = productDal.PurchasePrice(Item.ItemNo, Master.PurchaseInvoiceNo, currConn, transaction);

                            #region Update to Issue

                            sqlText = "";
                            sqlText += "select COUNT(IssueNo) from IssueDetails WHERE IssueNo=@MasterPurchaseInvoiceNo ";
                            sqlText += " AND ItemNo='" + Item.ItemNo + "'";
                            SqlCommand cmdFindIdIssue = new SqlCommand(sqlText, currConn);
                            cmdFindIdIssue.Transaction = transaction;

                            cmdFindIdIssue.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                            decimal IDExistIs = (int)cmdFindIdIssue.ExecuteScalar();
                            if (IDExistIs <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUpdateNotSuccessfully);
                            }

                            sqlText = ""; // kamrul
                            sqlText += " update IssueDetails set";
                            sqlText += " IssueLineNo=@ItemLineNo,";
                            sqlText += " Quantity   =@ItemQuantity,";
                            if (Master.TransactionType == "InputServiceImport")
                            {
                                //PurchasePrice = PurchasePrice + Convert.ToDecimal(Item.ATVAmount) + Convert.ToDecimal(Item.TVAAmount);
                                sqlText += " CostPrice=@PurchasePrice,";
                                //decimal subT = PurchasePrice * Convert.ToDecimal(Item.UOMQty);
                                sqlText += " SubTotal=@subT ,";

                            }
                            else if (Master.TransactionType == "InputService")
                            {
                                //decimal cost = Convert.ToDecimal(Item.SubTotal) - Convert.ToDecimal(Item.RebateAmount);
                                //decimal subT = cost * Convert.ToDecimal(Item.UOMQty);
                                sqlText += " CostPrice=@cost,";
                                sqlText += " SubTotal=@subT,";
                            }
                            sqlText += " Comments       =@ItemComments,";
                            sqlText += " uom            =@ItemUOM,";
                            sqlText += " LastModifiedBy =@MasterLastModifiedBy,";
                            sqlText += " LastModifiedOn =@MasterLastModifiedOn,";
                            sqlText += " IssueDateTime  =@MasterReceiveDate,";
                            sqlText += " TransactionType=@MasterTransactionType,";
                            sqlText += " IssueReturnId  =@MasterReturnId,";
                            sqlText += " UOMQty         =@ItemUOMQty,";
                            sqlText += " UOMPrice       =@ItemUOMPrice,";
                            sqlText += " UOMc           =@ItemUOMc,";
                            sqlText += " UOMn           =@ItemUOMn,";
                            sqlText += " UOMWastage     = '0',";
                            sqlText += " Post           =@MasterPost";
                            sqlText += " where  IssueNo =@MasterPurchaseInvoiceNo and ItemNo = @ItemItemNo";

                            SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                            cmdInsertIssue.Transaction = transaction;

                            cmdInsertIssue.Parameters.AddWithValue("@ItemLineNo", Item.LineNo);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemComments", Item.Comments?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOM", Item.UOM);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterReceiveDate", Master.ReceiveDate);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMn", Item.UOMn);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);


                            if (Master.TransactionType == "InputServiceImport")
                            {
                                PurchasePrice = PurchasePrice + Convert.ToDecimal(Item.ATVAmount) + Convert.ToDecimal(Item.TVAAmount);
                                decimal subT = PurchasePrice * Convert.ToDecimal(Item.UOMQty);
                                cmdInsertIssue.Parameters.AddWithValue("@PurchasePrice", PurchasePrice);
                                cmdInsertIssue.Parameters.AddWithValue("@subT", subT);

                            }
                            else if (Master.TransactionType == "InputService")
                            {
                                decimal cost = Convert.ToDecimal(Item.SubTotal) - Convert.ToDecimal(Item.RebateAmount);
                                decimal subT = cost * Convert.ToDecimal(Item.UOMQty);
                                cmdInsertIssue.Parameters.AddWithValue("@cost", cost);
                                cmdInsertIssue.Parameters.AddWithValue("@subT", subT);
                            }

                            transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUnableToUpdateIssue);
                            }

                            #endregion Update to Issue

                            #region Update Issue Header

                            sqlText = "";
                            sqlText += " update IssueHeaders set ";
                            sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                            sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                            sqlText += " where (IssueHeaders.IssueNo= @MasterPurchaseInvoiceNo)";

                            SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                            cmdUpdateIssue.Transaction = transaction;

                            cmdUpdateIssue.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                            transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUnableToUpdateIssue);
                            }

                            #endregion Update Issue Header

                        }

                        #endregion Transaction is InputService



                        #endregion Update Issue and Receive if Transaction is not Other
                    }

                    #endregion Find Transaction Mode Insert or Update


                }//foreach (var Item in Details.ToList())



                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)
                #region Tracking

                if (Trackings.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate, MessageVM.PurchasemsgNoDataToUpdateImportDyties);
                }

                //if (Master.TransactionType == "PurchaseCN" || Master.TransactionType == "PurchaseDN" || Master.TransactionType == "PurchaseReturn")
                //{

                //    for (int i = 0; i < Trackings.Count; i++)
                //    {

                //    }
                //    string trackingUpdate = string.Empty;
                //    TrackingDAL trackingDal = new TrackingDAL();
                //    trackingUpdate = trackingDal.TrackingUpdate(Trackings, transaction, currConn);

                //    if (trackingUpdate == "Fail")
                //    {
                //        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, "Tracking Information not added.");
                //    }
                //}
                //else
                //{

                foreach (var tracking in Trackings.ToList())
                {

                    #region Find Heading1 Existence

                    sqlText = "";
                    sqlText += "select COUNT(Heading1) from Trackings WHERE ";
                    sqlText += " ItemNo=@trackingItemNo";
                    sqlText += " AND Heading1 =@trackingHeading1";

                    SqlCommand cmdFindHeading1 = new SqlCommand(sqlText, currConn);
                    cmdFindHeading1.Transaction = transaction;

                    cmdFindHeading1.Parameters.AddWithValue("@trackingItemNo", tracking.ItemNo);
                    cmdFindHeading1.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1);

                    decimal IDExist = (int)cmdFindHeading1.ExecuteScalar();
                    if (IDExist <= 0)
                    {
                        #region Check Heading2

                        sqlText = "";
                        sqlText += "select COUNT(Heading2) from Trackings WHERE ";
                        sqlText += " ItemNo=@trackingItemNo";
                        sqlText += " AND Heading2 = @trackingHeading2";

                        SqlCommand cmdFindHeading2 = new SqlCommand(sqlText, currConn);
                        cmdFindHeading2.Transaction = transaction;

                        cmdFindHeading2.Parameters.AddWithValue("@trackingItemNo", tracking.ItemNo);
                        cmdFindHeading2.Parameters.AddWithValue("@trackingHeading2", tracking.Heading2);

                        decimal IDExist2 = (int)cmdFindHeading2.ExecuteScalar();
                        #endregion
                        if (IDExist2 <= 0)
                        {
                            // Insert
                            #region Insert
                            sqlText = "";
                            sqlText += " insert into Trackings";
                            sqlText += " (";

                            sqlText += " PurchaseInvoiceNo,";
                            sqlText += " ItemNo,";
                            sqlText += " TrackLineNo,";
                            sqlText += " Heading1,";
                            sqlText += " Heading2,";
                            sqlText += " Quantity,";
                            sqlText += " UnitPrice,";
                            sqlText += " IsPurchase,";
                            sqlText += " IsIssue,";
                            sqlText += " IsReceive,";
                            sqlText += " IsSale,";
                            sqlText += " Post,";
                            sqlText += " ReceivePost,";
                            sqlText += " SalePost,";
                            sqlText += " IssuePost,";


                            sqlText += " CreatedBy,";
                            sqlText += " CreatedOn,";
                            sqlText += " LastModifiedBy,";
                            sqlText += " LastModifiedOn";

                            sqlText += " )";
                            sqlText += " values";
                            sqlText += " (";

                            sqlText += "@MasterPurchaseInvoiceNo,";
                            sqlText += "@trackingItemNo,";
                            sqlText += "@trackingTrackingLineNo,";
                            sqlText += "@trackingHeading1,";
                            sqlText += "@trackingHeading2,";
                            sqlText += "@trackingQuantity,";
                            sqlText += "@trackingUnitPrice,";
                            sqlText += "@trackingIsPurchase,";
                            sqlText += "'N',";
                            sqlText += "'N',";
                            sqlText += "'N',";
                            sqlText += "@MasterPost,";
                            sqlText += "'N',";
                            sqlText += "'N',";
                            sqlText += "'N',";


                            sqlText += "@MasterCreatedBy,";
                            sqlText += "@MasterCreatedOn,";
                            sqlText += "@MasterLastModifiedBy,";
                            sqlText += "@MasterLastModifiedOn";

                            sqlText += ")";


                            SqlCommand cmdInsertTrackings = new SqlCommand(sqlText, currConn);
                            cmdInsertTrackings.Transaction = transaction;

                            cmdInsertTrackings.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingItemNo", tracking.ItemNo);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingTrackingLineNo", tracking.TrackingLineNo);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingHeading2", tracking.Heading2);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingQuantity", tracking.Quantity);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingUnitPrice", tracking.UnitPrice);
                            cmdInsertTrackings.Parameters.AddWithValue("@trackingIsPurchase", tracking.IsPurchase);
                            cmdInsertTrackings.Parameters.AddWithValue("@MasterPost", Master.Post);
                            cmdInsertTrackings.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy);
                            cmdInsertTrackings.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                            cmdInsertTrackings.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdInsertTrackings.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);

                            transResult = (int)cmdInsertTrackings.ExecuteNonQuery();
                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.PurchasemsgSaveNotSuccessfully);
                            }


                            #endregion
                        }
                        else
                        {
                            //Update
                            #region Update
                            sqlText = "";
                            sqlText += " update Trackings set ";
                            sqlText += " TrackLineNo    = @trackingTrackingLineNo,";
                            sqlText += " Heading1       = @trackingHeading1,";
                            sqlText += " Heading2       = @trackingHeading2,";
                            sqlText += " Quantity       = @trackingQuantity,";
                            sqlText += " UnitPrice      = @trackingUnitPrice,";
                            if (Master.TransactionType == "PurchaseCN" || Master.TransactionType == "PurchaseDN" || Master.TransactionType == "PurchaseReturn")
                            {
                                sqlText += " ReturnType         = @trackingReturnType,";
                                sqlText += " ReturnPurchase     = @trackingReturnPurchase,";
                                sqlText += " ReturnPurchaseID   = @trackingReturnPurchaseID,";
                                sqlText += " ReturnPurDate      = @trackingReturnPurDate,";
                            }
                            else
                            {
                                sqlText += " Post= @MasterPost,";
                            }
                            sqlText += " LastModifiedBy = @MasterLastModifiedBy,";
                            sqlText += " LastModifiedOn = @MasterLastModifiedOn";
                            sqlText += " where ItemNo   = @trackingItemNo";
                            sqlText += " and Heading2   = @trackingHeading2";

                            SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                            cmdInsDetail.Transaction = transaction;

                            cmdInsDetail.Parameters.AddWithValue("@trackingTrackingLineNo", tracking.TrackingLineNo);
                            cmdInsDetail.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1);
                            cmdInsDetail.Parameters.AddWithValue("@trackingHeading2", tracking.Heading2);
                            cmdInsDetail.Parameters.AddWithValue("@trackingQuantity", tracking.Quantity);
                            cmdInsDetail.Parameters.AddWithValue("@trackingUnitPrice", tracking.UnitPrice);
                            cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                            cmdInsDetail.Parameters.AddWithValue("@trackingItemNo", tracking.ItemNo);
                            cmdInsDetail.Parameters.AddWithValue("@trackingHeading2", tracking.Heading2);

                            if (Master.TransactionType == "PurchaseCN" || Master.TransactionType == "PurchaseDN" || Master.TransactionType == "PurchaseReturn")
                            {
                                cmdInsDetail.Parameters.AddWithValue("@trackingReturnType      ", tracking.ReturnType);
                                cmdInsDetail.Parameters.AddWithValue("@trackingReturnPurchase  ", tracking.ReturnPurchase);
                                cmdInsDetail.Parameters.AddWithValue("@trackingReturnPurchaseID", tracking.ReturnPurchaseID);
                                cmdInsDetail.Parameters.AddWithValue("@trackingReturnPurDate   ", tracking.ReturnPurDate);
                            }
                            else
                            {
                                cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post);
                            }
                            transResult = (int)cmdInsDetail.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUpdateNotSuccessfully);
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        //Update
                        #region Update
                        sqlText = "";
                        sqlText += " update Trackings set ";
                        sqlText += " TrackLineNo= @trackingTrackingLineNo,";
                        sqlText += " Heading1   = @trackingHeading1,";
                        sqlText += " Heading2   = @trackingHeading2,";
                        sqlText += " Quantity   = @trackingQuantity,";
                        sqlText += " UnitPrice  = @trackingUnitPrice,";


                        if (Master.TransactionType == "PurchaseCN" || Master.TransactionType == "PurchaseDN" || Master.TransactionType == "PurchaseReturn")
                        {
                            sqlText += " ReturnType         =@trackingReturnType,";
                            sqlText += " ReturnPurchase     =@trackingReturnPurchase,";
                            sqlText += " ReturnPurchaseID   =@trackingReturnPurchaseID,";
                            sqlText += " ReturnPurDate      =@trackingReturnPurDate,";
                        }
                        else
                        {
                            sqlText += " Post   = @MasterPost,";
                        }

                        sqlText += " LastModifiedBy =@MasterLastModifiedBy,";
                        sqlText += " LastModifiedOn =@MasterLastModifiedOn ";

                        sqlText += " where ItemNo = @trackingItemNo";
                        sqlText += " and Heading1 = @trackingHeading1";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValue("@trackingTrackingLineNo", tracking.TrackingLineNo);
                        cmdInsDetail.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1);
                        cmdInsDetail.Parameters.AddWithValue("@trackingHeading2", tracking.Heading2);
                        cmdInsDetail.Parameters.AddWithValue("@trackingQuantity", tracking.Quantity);
                        cmdInsDetail.Parameters.AddWithValue("@trackingUnitPrice", tracking.UnitPrice);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValue("@trackingItemNo", tracking.ItemNo);
                        cmdInsDetail.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1);


                        if (Master.TransactionType == "PurchaseCN" || Master.TransactionType == "PurchaseDN" || Master.TransactionType == "PurchaseReturn")
                        {
                            cmdInsDetail.Parameters.AddWithValue("@trackingReturnType", tracking.ReturnType);
                            cmdInsDetail.Parameters.AddWithValue("@trackingReturnPurchase", tracking.ReturnPurchase);
                            cmdInsDetail.Parameters.AddWithValue("@trackingReturnPurchaseID", tracking.ReturnPurchaseID);
                            cmdInsDetail.Parameters.AddWithValue("@trackingReturnPurDate", tracking.ReturnPurDate);
                        }
                        else
                        {
                            cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post);
                        }

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                            MessageVM.PurchasemsgUpdateNotSuccessfully);
                        }
                        #endregion
                    }


                    #endregion Find Heading1 Existence
                }

                #endregion Tracking
                #region Remove row
                sqlText = "";
                sqlText += " SELECT  distinct ItemNo";
                sqlText += " from PurchaseInvoiceDetails WHERE PurchaseInvoiceNo=@MasterPurchaseInvoiceNo";

                DataTable dt = new DataTable("Previous");
                SqlCommand cmdRIFB1 = new SqlCommand(sqlText, currConn);
                cmdRIFB1.Transaction = transaction;
                cmdRIFB1.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                SqlDataAdapter dta = new SqlDataAdapter(cmdRIFB1);
                dta.Fill(dt);
                foreach (DataRow pItem in dt.Rows)
                {
                    var p = pItem["ItemNo"].ToString();

                    //var tt= Details.Find(x => x.ItemNo == p);
                    var tt = Details.Count(x => x.ItemNo.Trim() == p.Trim());
                    if (tt == 0)
                    {
                        sqlText = "";
                        sqlText += " delete FROM PurchaseInvoiceDetails ";
                        sqlText += " WHERE PurchaseInvoiceNo=@MasterPurchaseInvoiceNo ";
                        sqlText += " AND ItemNo='" + p + "'";
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (Master.TransactionType == "Import"
                     || Master.TransactionType == "ServiceImport"
                     || Master.TransactionType == "ServiceNSImport"
                     || Master.TransactionType == "TradingImport"
                     || Master.TransactionType == "InputServiceImport")
                        {
                            sqlText = "";
                            sqlText += " delete FROM PurchaseInvoiceDuties ";
                            sqlText += " WHERE PurchaseInvoiceNo=@MasterPurchaseInvoiceNo ";
                            sqlText += " AND ItemNo='" + p + "'";
                            SqlCommand cmdInsDetail1 = new SqlCommand(sqlText, currConn);
                            cmdInsDetail1.Transaction = transaction;
                            cmdInsDetail1.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                            transResult = (int)cmdInsDetail1.ExecuteNonQuery();
                        }

                        #region Tracking
                        sqlText = "";
                        sqlText += " delete FROM Trackings ";
                        sqlText += " WHERE PurchaseInvoiceNo=@MasterPurchaseInvoiceNo ";
                        sqlText += " AND ItemNo='" + p + "'";
                        SqlCommand cmdInsTracking = new SqlCommand(sqlText, currConn);
                        cmdInsTracking.Transaction = transaction;
                        cmdInsTracking.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                        transResult = (int)cmdInsTracking.ExecuteNonQuery();
                        #endregion
                    }

                }
                #endregion Remove row

                #region return Current ID and Post Status

                sqlText = "";

                sqlText = sqlText + "select distinct Post from PurchaseInvoiceHeaders WHERE PurchaseInvoiceNo=@MasterPurchaseInvoiceNo";


                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate, MessageVM.PurchasemsgUnableCreatID);
                }


                #endregion Prefetch
                #region Commit

                if (transaction != null)
                {
                    transaction.Commit();

                    //////if (transResult > 0)
                    //////{
                    //////}

                }

                #endregion Commit
                #region SuccessResult


                retResults[0] = "Success";
                retResults[1] = MessageVM.PurchasemsgUpdateSuccessfully;
                retResults[2] = Master.Id;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }
        public string[] PurchasePost(PurchaseMasterVM Master, List<PurchaseDetailVM> Details, List<PurchaseDutiesVM> Duties, List<TrackingVM> Trackings)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            //DateTime MinDate = DateTime.MinValue;
            //DateTime MaxDate = DateTime.MaxValue;

            string PostStatus = "";

            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.purchaseMsgMethodNamePost, MessageVM.purchaseMsgNoDataToPost);
                }
                else if (Convert.ToDateTime(Master.ReceiveDate) < DateTime.MinValue || Convert.ToDateTime(Master.ReceiveDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.purchaseMsgMethodNamePost, MessageVM.purchaseMsgCheckDatePost);

                }


                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction(MessageVM.PurchasemsgMethodNameUpdate);
                CommonDAL commonDal = new CommonDAL();
                bool TollReceiveWithIssue = Convert.ToBoolean(commonDal.settings("TollReceive", "WithIssue") == "Y" ? true : false);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.ReceiveDate;
                string transactionYearCheck = Convert.ToDateTime(Master.ReceiveDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock
                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK


                #region Find ID for Post

                sqlText = "";

                sqlText = sqlText +
                          "select COUNT(PurchaseInvoiceNo) from PurchaseInvoiceHeaders WHERE PurchaseInvoiceNo=@MasterPurchaseInvoiceNo ";

                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;

                cmdFindIdUpd.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.purchaseMsgMethodNamePost,
                                                    MessageVM.purchaseMsgUnableFindExistIDPost);
                }

                #endregion Find ID for Update


                #region ID check completed,update Information in Header



                #region update Header

                sqlText = "";

                sqlText += " update PurchaseInvoiceHeaders set  ";
                sqlText += " LastModifiedBy = @MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn = @MasterLastModifiedOn ,";
                sqlText += " Post           = @MasterPost ";
                sqlText += " where  PurchaseInvoiceNo=@MasterPurchaseInvoiceNo ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@MasterPost", Master.Post);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.purchaseMsgMethodNamePost,
                                                    MessageVM.PurchasemsgUpdateNotSuccessfully);
                }

                #endregion update Header

                #region Transaction Not Other

                #region Transaction is TollReceive

                if (Master.TransactionType == "TollReceive")
                {
                    #region update Issue

                    sqlText = "";


                    sqlText += " update IssueHeaders set ";
                    sqlText += " LastModifiedBy=@MasterLastModifiedBy ,";
                    sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                    sqlText += " Post=@MasterPost ";
                    sqlText += " where  IssueNo=@MasterPurchaseInvoiceNo ";


                    SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                    cmdUpdateIssue.Transaction = transaction;

                    cmdUpdateIssue.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterPost", Master.Post);

                    transResult = (int)cmdUpdateIssue.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.purchaseMsgMethodNamePost,
                                                        MessageVM.PurchasemsgUpdateNotSuccessfully);
                    }

                    #endregion update Issue

                    #region update Receive

                    sqlText = "";


                    sqlText += " update ReceiveHeaders set";
                    sqlText += " LastModifiedBy= @MasterLastModifiedBy,";
                    sqlText += " LastModifiedOn= @MasterLastModifiedOn,";
                    sqlText += " Post= @MasterPost ";
                    sqlText += " where  ReceiveNo = @MasterPurchaseInvoiceNo";

                    SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                    cmdUpdateReceive.Transaction = transaction;

                    cmdUpdateReceive.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterPost", Master.Post);

                    transResult = (int)cmdUpdateReceive.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.purchaseMsgMethodNamePost,
                                                        MessageVM.PurchasemsgUpdateNotSuccessfully);
                    }

                    #endregion update Receive
                }

                #endregion Transaction is TollReceive

                #region Transaction is InputService

                if (Master.TransactionType == "InputService" || Master.TransactionType == "InputServiceImport")
                {
                    #region update Issue

                    sqlText = "";


                    sqlText += " update IssueHeaders set ";
                    sqlText += " LastModifiedBy= @MasterLastModifiedBy ,";
                    sqlText += " LastModifiedOn= @MasterLastModifiedOn,";
                    sqlText += " Post= @MasterPost  ";
                    sqlText += " where  IssueNo=@MasterPurchaseInvoiceNo ";


                    SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                    cmdUpdateIssue.Transaction = transaction;

                    cmdUpdateIssue.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterPost", Master.Post);

                    transResult = (int)cmdUpdateIssue.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.purchaseMsgMethodNamePost,
                                                        MessageVM.PurchasemsgUpdateNotSuccessfully);
                    }

                    #endregion update Issue

                }

                #endregion Transaction is InputService

                #region Import s

                if (Master.TransactionType == "Import"
            || Master.TransactionType == "ServiceImport"
            || Master.TransactionType == "ServiceNSImport"
            || Master.TransactionType == "TradingImport"
            || Master.TransactionType == "InputServiceImport")
                {
                    if (Duties.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.purchaseMsgMethodNamePost,
                                                        MessageVM.PurchasemsgNoDataPostToPost);
                    }


                    foreach (var duty in Duties.ToList())
                    {
                        #region update Duties

                        sqlText = "";

                        sqlText += " update PurchaseInvoiceDuties set  ";
                        sqlText += " LastModifiedBy=@MasterLastModifiedBy, ";
                        sqlText += " LastModifiedOn=@MasterLastModifiedOn, ";
                        sqlText += " Post=@MasterPost ";
                        sqlText += " where  PurchaseInvoiceNo=@MasterPurchaseInvoiceNo ";
                        //sqlText += " and  ItemNo= '" + duty.ItemNo + "' ";


                        SqlCommand cmdUpdateDuty = new SqlCommand(sqlText, currConn);
                        cmdUpdateDuty.Transaction = transaction;

                        cmdUpdateDuty.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                        cmdUpdateDuty.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdUpdateDuty.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdUpdateDuty.Parameters.AddWithValue("@MasterPost", Master.Post);

                        transResult = (int)cmdUpdateDuty.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                            MessageVM.PurchasemsgUpdateNotSuccessfully);
                        }

                        #endregion update Duties
                    }
                }

                #endregion Import

                #endregion Transaction Not Other


                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.purchaseMsgMethodNamePost, MessageVM.PurchasemsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update

                    sqlText = "";



                    sqlText +=
                        "select COUNT(PurchaseInvoiceNo) from PurchaseInvoiceDetails WHERE PurchaseInvoiceNo=@MasterPurchaseInvoiceNo";


                    sqlText += " AND ItemNo=@ItemItemNo ";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;

                    cmdFindId.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.purchaseMsgMethodNamePost, MessageVM.purchaseMsgNoDataToPost);
                    }
                    else
                    {
                        //Update


                        #region Update only DetailTable

                        sqlText = "";

                        sqlText += " update PurchaseInvoiceDetails set ";
                        sqlText += " LastModifiedBy =@MasterLastModifiedBy,";
                        sqlText += " LastModifiedOn =@MasterLastModifiedOn,";
                        sqlText += " Post           =@MasterPost";
                        sqlText += " where  PurchaseInvoiceNo =@MasterPurchaseInvoiceNo ";

                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.purchaseMsgMethodNamePost,
                                                            MessageVM.PurchasemsgUpdateNotSuccessfully);
                        }

                        #region Update Item Qty +

                        #region Find Quantity From Products



                        ProductDAL productDal = new ProductDAL();
                        //decimal oldStock = productDal.StockInHand(Item.ItemNo, Master.ReceiveDate, currConn,
                        //transaction);
                        decimal oldStock = Convert.ToDecimal(productDal.AvgPriceNew(Item.ItemNo, Master.ReceiveDate,
                                                             currConn, transaction, true).Rows[0]["Quantity"].ToString());


                        #endregion Find Quantity From Products

                        #region Find Quantity From Transaction

                        sqlText = "";
                        sqlText += "select isnull(Quantity ,0) from PurchaseInvoiceDetails ";
                        sqlText += " WHERE ItemNo='" + Item.ItemNo + "' and PurchaseInvoiceNo= @MasterPurchaseInvoiceNo";
                        SqlCommand cmdTranQty = new SqlCommand(sqlText, currConn);
                        cmdTranQty.Transaction = transaction;

                        cmdTranQty.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                        decimal TranQty = (decimal)cmdTranQty.ExecuteScalar();

                        #endregion Find Quantity From Transaction

                        #region privious Value

                        //sqlText = "";
                        //sqlText += "SELECT (isnull(pid.SubTotal,0)";
                        //sqlText += " +(" + (oldStock - TranQty) + ")*isnull(IssuePrice,0))";
                        //sqlText += " /(" + (oldStock - TranQty) + "+pid.Quantity) ";
                        //sqlText += " FROM PurchaseInvoiceDetails pid LEFT OUTER JOIN";
                        //sqlText += " products p ON pid.ItemNo= p.ItemNo";
                        //sqlText += " WHERE pid.PurchaseInvoiceNo='" + Master.PurchaseInvoiceNo + "' AND pid.ItemNo='" + Item.ItemNo +
                        //           "' ";

                        //SqlCommand cmdCValue = new SqlCommand(sqlText, currConn);
                        //cmdCValue.Transaction = transaction;
                        //decimal currentIssuePrice = (decimal) cmdCValue.ExecuteScalar();

                        #endregion privious Value






                        #endregion Qty  check and UPDATE+

                        #endregion Update only DetailTable

                        #region Update Issue and Receive if Transaction is not Other


                        #region Transaction is TollReceive

                        if (Master.TransactionType == "TollReceive")
                        {
                            string FinishItemIdFromOH = productDal.GetFinishItemIdFromOH(Item.ItemNo, "VAT 1 (Toll Receive)", Master.ReceiveDate,
                                                                           currConn, transaction).ToString();
                            #region Update to Issue

                            sqlText = "";
                            sqlText += "select COUNT(IssueNo) from IssueDetails WHERE IssueNo=@MasterPurchaseInvoiceNo ";
                            //sqlText += " AND ItemNo='" + Item.ItemNo + "'";
                            SqlCommand cmdFindIdIssue = new SqlCommand(sqlText, currConn);
                            cmdFindIdIssue.Transaction = transaction;

                            cmdFindIdIssue.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);


                            IDExist = (int)cmdFindIdIssue.ExecuteScalar();
                            if (IDExist <= 0)
                            {
                                if (TollReceiveWithIssue)
                                    throw new ArgumentNullException(MessageVM.purchaseMsgMethodNamePost,
                                                                    MessageVM.PurchasemsgUpdateNotSuccessfully);
                            }

                            sqlText = "";
                            sqlText += " update IssueDetails set";
                            sqlText += " LastModifiedBy=@MasterLastModifiedBy,";
                            sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                            sqlText += " Post=@MasterPost";
                            sqlText += " where  IssueNo =@MasterPurchaseInvoiceNo";

                            //sqlText += "'" + Master.@Post + "'";
                            SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                            cmdInsertIssue.Transaction = transaction;

                            cmdInsertIssue.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post);

                            transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                if (TollReceiveWithIssue)
                                    throw new ArgumentNullException(MessageVM.purchaseMsgMethodNamePost,
                                                   MessageVM.PurchasemsgUnableToUpdateIssue);
                            }

                            #endregion Find Quantity From Products

                            #region Update to Receive

                            sqlText = "";
                            sqlText += "select COUNT(ReceiveNo) from ReceiveDetails WHERE ReceiveNo='" + Master.PurchaseInvoiceNo +
                                       "' ";
                            sqlText += " AND ItemNo='" + FinishItemIdFromOH + "'";
                            SqlCommand cmdFindIdReceive = new SqlCommand(sqlText, currConn);
                            cmdFindIdReceive.Transaction = transaction;
                            var vIDExist = cmdFindIdReceive.ExecuteScalar();
                            //if (IDExist <= 0)
                            //{
                            //    throw new ArgumentNullException(MessageVM.purchaseMsgMethodNamePost,
                            //                                    MessageVM.PurchasemsgUpdateNotSuccessfully);
                            //}
                            sqlText = "";
                            sqlText += " update ReceiveDetails set ";
                            sqlText += " LastModifiedBy= @MasterLastModifiedBy,";
                            sqlText += " LastModifiedOn= @MasterLastModifiedOn,";
                            sqlText += " Post= @MasterPost";
                            sqlText += " where  ReceiveNo = @MasterPurchaseInvoiceNo ";
                            //sqlText += " and ItemNo = '" + FinishItemIdFromOH + "'";
                            //sqlText += "'" + Master.@Post + "'";
                            SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                            cmdInsertReceive.Transaction = transaction;

                            cmdInsertReceive.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterPost", Master.Post);

                            transResult = (int)cmdInsertReceive.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.purchaseMsgMethodNamePost,
                                                                MessageVM.PurchasemsgUnableToUpdateReceive);
                            }

                            #endregion Update to Receive
                        }


                        #endregion Transaction is TollReceive

                        #region Transaction is InputService

                        if (Master.TransactionType == "InputService" || Master.TransactionType == "InputServiceImport")
                        {

                            #region Update to Issue

                            sqlText = "";
                            sqlText += "select COUNT(IssueNo) from IssueDetails WHERE IssueNo=@MasterPurchaseInvoiceNo";
                            sqlText += " AND ItemNo=@ItemItemNo";
                            SqlCommand cmdFindIdIssue = new SqlCommand(sqlText, currConn);
                            cmdFindIdIssue.Transaction = transaction;

                            cmdFindIdIssue.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                            cmdFindIdIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                            IDExist = (int)cmdFindIdIssue.ExecuteScalar();
                            if (IDExist <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.purchaseMsgMethodNamePost,
                                                                MessageVM.PurchasemsgUpdateNotSuccessfully);
                            }

                            sqlText = "";
                            sqlText += " update IssueDetails set";
                            sqlText += " LastModifiedBy=@MasterLastModifiedBy,";
                            sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                            sqlText += " Post=@MasterPost";
                            sqlText += " where  IssueNo =@MasterPurchaseInvoiceNo";

                            //sqlText += "'" + Master.@Post + "'";
                            SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                            cmdInsertIssue.Transaction = transaction;

                            cmdInsertIssue.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post);

                            transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.purchaseMsgMethodNamePost,
                                                                MessageVM.PurchasemsgUnableToUpdateIssue);
                            }

                            #endregion Find Quantity From Products

                        }

                        #endregion Transaction is InputService



                        #endregion Update Issue and Receive if Transaction is not Other

                    }

                    #endregion Find Transaction Mode Insert or Update
                }


                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)

                #region Tracking
                if (Trackings.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.purchaseMsgMethodNamePost,
                                                    MessageVM.PurchasemsgNoDataPostToPost);
                }


                foreach (var tracking in Trackings.ToList())
                {
                    #region update Trackings

                    sqlText = "";

                    sqlText += " update Trackings set  ";
                    sqlText += " LastModifiedBy= @MasterLastModifiedBy, ";
                    sqlText += " LastModifiedOn= @MasterLastModifiedOn, ";
                    sqlText += " Post= @MasterPost ";
                    sqlText += " where  ItemNo= @trackingItemNo ";
                    sqlText += " and  Heading1= @trackingHeading1 ";


                    SqlCommand cmdUpdateTracking = new SqlCommand(sqlText, currConn);
                    cmdUpdateTracking.Transaction = transaction;

                    cmdUpdateTracking.Parameters.AddWithValue("@trackingItemNo", tracking.ItemNo);
                    cmdUpdateTracking.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1);
                    cmdUpdateTracking.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdUpdateTracking.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdUpdateTracking.Parameters.AddWithValue("@MasterPost", Master.Post);


                    transResult = (int)cmdUpdateTracking.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                        MessageVM.PurchasemsgUpdateNotSuccessfully);
                    }

                    #endregion update Trackings
                }
                #endregion

                #region TrackingWithSale
                bool TrackingWithSale = Convert.ToBoolean(commonDal.settingValue("Purchase", "TrackingWithSale") == "Y" ? true : false);
                if (TrackingWithSale)
                {
                    DataTable tracDt = new DataTable();
                    sqlText = "";
                    sqlText = @"SELECT    
                                    PurchaseInvoiceDetails.PurchaseInvoiceNo,
                                    PurchaseInvoiceDetails.InvoiceDateTime,
                                    PurchaseInvoiceDetails.ReceiveDate,
                                    PurchaseInvoiceDetails.ItemNo, 
                                    isnull(PurchaseInvoiceDetails.BENumber,'N/A')BENumber ,
                                    isnull(PurchaseInvoiceDetails.Quantity,0)Quantity,
                                    isnull(PurchaseInvoiceDetails.UOM,'N/A')UOM ,
                                    isnull(PurchaseInvoiceDetails.VATRate,0)VATRate,
                                    isnull(PurchaseInvoiceDetails.ReturnTransactionType,'')ReturnTransactionType,
                                    isnull(PurchaseInvoiceHeaders.CustomHouse,'')CustomHouse
                                    FROM dbo.PurchaseInvoiceDetails 
                                    left outer join PurchaseInvoiceHeaders on PurchaseInvoiceHeaders.PurchaseInvoiceNo=PurchaseInvoiceDetails.PurchaseInvoiceNo
                                    WHERE ";
                    sqlText += @"   (PurchaseInvoiceDetails.PurchaseInvoiceNo =@MasterPurchaseInvoiceNo)";
                    sqlText += @"  order by PurchaseInvoiceDetails.ItemNo";
                    SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                    cmdRIFB.Transaction = transaction;

                    cmdRIFB.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                    reportDataAdapt.Fill(tracDt);

                    foreach (DataRow dRow in tracDt.Rows)
                    {
                        #region Insert only DetailTable PurchaseInvoiceDetails

                        sqlText = "";
                        sqlText += " insert into PurchaseSaleTrackings(";
                        sqlText += " PurchaseInvoiceNo,";
                        sqlText += " PurchaseInvoiceDateTime,";
                        sqlText += " ReceiveDate,";
                        sqlText += " CustomHouse,";
                        sqlText += " ItemNo,";
                        sqlText += " BENumber,";
                        sqlText += " SalesInvoiceNo,";
                        sqlText += " SaleInvoiceDateTime,";
                        sqlText += " IsSold";
                        sqlText += " )";
                        sqlText += " values(	";
                        sqlText += "'" + dRow["PurchaseInvoiceNo"].ToString() + "',";
                        sqlText += "'" + dRow["InvoiceDateTime"].ToString() + "',";
                        sqlText += "'" + dRow["ReceiveDate"].ToString() + "',";
                        sqlText += "'" + dRow["CustomHouse"].ToString() + "',";
                        sqlText += "'" + dRow["ItemNo"].ToString() + "',";
                        sqlText += "'" + dRow["BENumber"].ToString() + "',";
                        sqlText += "'0',";
                        sqlText += "'01/01/1900',";
                        sqlText += "'0'";
                        sqlText += ")	";

                        decimal qty = Convert.ToDecimal(dRow["Quantity"]);
                        for (int i = 0; i < qty; i++)
                        {
                            SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                            cmdInsDetail.Transaction = transaction;
                            transResult = (int)cmdInsDetail.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.PurchasemsgSaveNotSuccessfully);
                            }
                        }


                        #endregion Insert only DetailTable
                    }
                }
                #endregion TrackingWithSale
                #region return Current ID and Post Status

                sqlText = "";


                sqlText = sqlText + " select distinct Post from PurchaseInvoiceHeaders WHERE PurchaseInvoiceNo=@MasterPurchaseInvoiceNo";

                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;

                cmdIPS.Parameters.AddWithValue("@MasterPurchaseInvoiceNo", Master.PurchaseInvoiceNo);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.purchaseMsgMethodNamePost, MessageVM.PurchasemsgUnableCreatID);
                }


                #endregion Prefetch
                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Commit
                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.purchaseMsgSuccessfullyPost;
                retResults[2] = Master.PurchaseInvoiceNo;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        public DataTable SearchProductbyPurchaseInvoice(string purchaseInvoiceNo)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable();

            #endregion

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "ItemNo", "varchar(20)", currConn);
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "DutyCompleteQuantity", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "DutyCompleteQuantityPercent", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "LineCost", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "UnitCost", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("PurchaseInvoiceDuties", "Quantity", "decimal(25, 9)", currConn);

                #endregion open connection and transaction

                #region sql statement



                sqlText = "";
                sqlText += @"

SELECT pu.[PurchaseInvoiceNo]
	  ,pr.[ProductCode]
	  ,pr.[ProductName] 
      ,pu.[ItemNo]
      ,pu.[Quantity]   
      ,pu.[UOM]
      ,pu.[CostPrice]
      ,pu.[SubTotal]
      ,ISNULL(pu.[CDAmount], 0.000000000)CDAmount
      ,ISNULL(pu.[RDAmount], 0.000000000)RDAmount
      ,ISNULL(pu.[SDAmount], 0.000000000)SDAmount
      ,ISNULL(pu.[VATAmount], 0.000000000)VATAmount
      ,ISNULL(pu.[CnFAmount], 0.000000000)CnFAmount
      ,ISNULL(pu.[InsuranceAmount], 0.000000000)InsuranceAmount
      ,ISNULL(pu.[TVBAmount], 0.000000000)TVBAmount
      ,ISNULL(pu.[TVAAmount], 0.000000000)TVAAmount
      ,ISNULL(pu.[ATVAmount], 0.000000000)ATVAmount
      ,ISNULL(pu.[OthersAmount], 0.000000000)OthersAmount
  FROM PurchaseInvoiceDetails pu,
Products pr
where pu.[ItemNo]=pr.[ItemNo]
and pu.PurchaseInvoiceNo=@purchaseInvoiceNo

";

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;



                if (!objCommProduct.Parameters.Contains("@purchaseInvoiceNo"))
                { objCommProduct.Parameters.AddWithValue("@purchaseInvoiceNo", purchaseInvoiceNo); }
                else { objCommProduct.Parameters["@purchaseInvoiceNo"].Value = purchaseInvoiceNo; }


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
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

        public DataTable SearchPurchaseInvoiceTracking(string purchaseInvoiceNo, string itemNo)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable();

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



                sqlText = "";
                sqlText += @"

SELECT t.[PurchaseInvoiceNo]
      ,pr.[ProductCode]
	  ,pr.[ProductName] 
      ,t.[ItemNo]
	  ,t.[Heading1]
      ,t.[Heading2]
      ,t.[Quantity]
      ,t.[IsPurchase]
      ,t.[Post]
	  ,t.ReturnPurchase
	  ,t.ReturnPurchaseID
	  ,t.ReturnReceive
	  ,t.ReturnReceiveID
	  ,t.ReturnSale
	  ,t.ReturnSaleID
	  ,t.ReturnType
      ,ISNULL(t.[UnitPrice],0) UnitPrice

FROM Trackings t,
Products pr
where t.[ItemNo]=pr.[ItemNo]
and t.PurchaseInvoiceNo=@purchaseInvoiceNo
and t.ItemNo=@itemNo
";

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;



                if (!objCommProduct.Parameters.Contains("@purchaseInvoiceNo"))
                { objCommProduct.Parameters.AddWithValue("@purchaseInvoiceNo", purchaseInvoiceNo); }
                else { objCommProduct.Parameters["@purchaseInvoiceNo"].Value = purchaseInvoiceNo; }

                if (string.IsNullOrEmpty(itemNo))
                {
                    if (!objCommProduct.Parameters.Contains("@itemNo"))
                    { objCommProduct.Parameters.AddWithValue("@itemNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@itemNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@itemNo"))
                    { objCommProduct.Parameters.AddWithValue("@itemNo", itemNo); }
                    else { objCommProduct.Parameters["@itemNo"].Value = itemNo; }
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
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
        //-----------------------

        private DataTable DtPurchaseD;
        public string[] ImportData(DataTable dtPurchaseM, DataTable dtPurchaseD, DataTable dtPurchaseI, DataTable dtPurchaseT)
        {

            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            PurchaseMasterVM purchaseMaster;
            List<PurchaseDutiesVM> purchaseDuties = new List<PurchaseDutiesVM>();
            List<PurchaseDetailVM> purchaseDetails = new List<PurchaseDetailVM>();
            List<TrackingVM> purchaseTrackings = new List<TrackingVM>();


            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion variable

            #region try
            try
            {
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    currConn.Open();
                }

                #region RowCount
                int rowCount = 0;
                int MRow = dtPurchaseM.Rows.Count;
                for (int i = 0; i < dtPurchaseM.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtPurchaseM.Rows[i]["ID"].ToString()))
                    {
                        rowCount++;
                    }

                }
                if (MRow != rowCount)
                {
                    throw new ArgumentNullException("you have select " + MRow.ToString() + " data for import, but you have " + rowCount + " id.");

                }



                #endregion RowCount

                #region ID in Master or Detail table

                for (int i = 0; i < rowCount; i++)
                {
                    string importID = dtPurchaseM.Rows[i]["ID"].ToString();

                    if (!string.IsNullOrEmpty(importID))
                    {
                        DataRow[] DetailRaws = dtPurchaseD.Select("ID='" + importID + "'");
                        if (DetailRaws.Length == 0)
                        {
                            throw new ArgumentNullException("The ID " + importID + " is not available in detail table");
                        }

                    }

                }

                #endregion



                #region Double ID in Master

                for (int i = 0; i < rowCount; i++)
                {
                    string id = dtPurchaseM.Rows[i]["ID"].ToString();
                    DataRow[] tt = dtPurchaseM.Select("ID='" + id + "'");
                    if (tt.Length > 1)
                    {
                        throw new ArgumentNullException("you have duplicate master id " + id + " in import file.");
                    }
                }


                #endregion Double ID in Master

                CommonImport cImport = new CommonImport();

                #region checking from database is exist the information(NULL Check)
                #region Master
                string CurrencyId = string.Empty;
                string USDCurrencyId = string.Empty;
                string PrePurchaseId = string.Empty;



                #region Find Master Column Name

                bool IsCollCDate = false;
                bool IsCollCNo = false;
                bool IsCollandedCost = false;

                for (int i = 0; i < dtPurchaseM.Columns.Count; i++)
                {

                    if (dtPurchaseM.Columns[i].ColumnName.ToString() == "LC_No")
                    {
                        IsCollCNo = true;
                    }
                    else if (dtPurchaseM.Columns[i].ColumnName.ToString() == "LC_Date")
                    {
                        IsCollCDate = true;
                    }
                    else if (dtPurchaseM.Columns[i].ColumnName.ToString() == "Landed_Cost")
                    {
                        IsCollandedCost = true;
                    }
                }
                #endregion Find Master Column Name



                for (int i = 0; i < rowCount; i++)
                {
                    CurrencyId = string.Empty;
                    USDCurrencyId = string.Empty;

                    #region FindVendorId
                    cImport.FindVendorId(dtPurchaseM.Rows[i]["Vendor_Name"].ToString().Trim(),
                                           dtPurchaseM.Rows[i]["Vendor_Code"].ToString().Trim(), currConn, transaction);
                    #endregion FindVendorId

                    #region Check Previous Purchase no.
                    PrePurchaseId = cImport.CheckPrePurchaseNo(dtPurchaseM.Rows[i]["Previous_Purchase_No"].ToString().Trim(), currConn, transaction);
                    #endregion Check Previous Purchase no.

                    #region FindCurrencyId
                    //CurrencyId = cImport.FindCurrencyId(dtPurchaseM.Rows[i]["Currency_Code"].ToString().Trim());
                    //USDCurrencyId = cImport.FindCurrencyId("USD");
                    //cImport.FindCurrencyRateFromBDT(CurrencyId);
                    //cImport.FindCurrencyRateBDTtoUSD(USDCurrencyId);

                    #endregion FindCurrencyId

                    #region Check Date

                    bool IsInvoiceDate;
                    bool IsReceiveDate;
                    //bool IsLCDate;
                    IsInvoiceDate = cImport.CheckDate(dtPurchaseM.Rows[i]["Invoice_Date"].ToString().Trim());
                    if (IsInvoiceDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Invoice_Date field.");
                    }
                    IsReceiveDate = cImport.CheckDate(dtPurchaseM.Rows[i]["Receive_Date"].ToString().Trim());
                    if (IsReceiveDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Receive_Date field.");
                    }

                    //IsLCDate = cImport.CheckDate(dtPurchaseM.Rows[i]["LCDate"].ToString().Trim());
                    //if (IsReceiveDate != true)
                    //{
                    //    throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Receive_Date field.");
                    //}
                    #endregion Check Date

                    #region Yes no check

                    bool post;
                    bool withVDS;

                    post = cImport.CheckYN(dtPurchaseM.Rows[i]["Post"].ToString().Trim());
                    if (post != true)
                    {
                        throw new ArgumentNullException("Please insert Y/N in Post field.");
                    }
                    withVDS = cImport.CheckYN(dtPurchaseM.Rows[i]["With_VDS"].ToString().Trim());
                    if (withVDS != true)
                    {
                        throw new ArgumentNullException("Please insert Y/N in With_VDSe field.");
                    }

                    #endregion Yes no check
                    bool isCost;
                    if (IsCollandedCost == true)
                    {
                        isCost = cImport.CheckNumericBool(dtPurchaseM.Rows[i]["Landed_Cost"].ToString().Trim());
                        if (isCost != true)
                        {
                            throw new ArgumentNullException("Please insert decimal value in Landed_Cost field.");
                        }
                    }

                }
                #endregion Master


                #region Row count for details table
                int DRowCount = 0;
                for (int i = 0; i < dtPurchaseD.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtPurchaseD.Rows[i]["ID"].ToString()))
                    {
                        DRowCount++;
                    }

                }

                #endregion Row count for details table
                #region Details

                for (int j = 0; j < DRowCount; j++)
                {
                    string ItemNo = string.Empty;
                    string UOMn = string.Empty;
                    bool IsQuantity, IsTotalPrice, IsRebateRate, IsSDAmount, IsVatAmount;

                    #region FindItemId
                    if (string.IsNullOrEmpty(dtPurchaseD.Rows[j]["Item_Code"].ToString().Trim()))
                    {
                        throw new ArgumentNullException("Please insert value in 'Item_Code' field.");
                    }
                    ItemNo = cImport.FindItemId(dtPurchaseD.Rows[j]["Item_Name"].ToString().Trim()
                                 , dtPurchaseD.Rows[j]["Item_Code"].ToString().Trim(), currConn, transaction);
                    #endregion FindItemId

                    #region FindUOMn
                    UOMn = cImport.FindUOMn(ItemNo, currConn, transaction);
                    #endregion FindUOMn
                    #region FindUOMc
                    if (DatabaseInfoVM.DatabaseName.ToString() != "Sanofi_DB")
                    {
                        cImport.FindUOMc(UOMn, dtPurchaseD.Rows[j]["UOM"].ToString().Trim(), currConn, transaction);
                    }
                    #endregion FindUOMc

                    #region Numeric value check
                    IsQuantity = cImport.CheckNumericBool(dtPurchaseD.Rows[j]["Quantity"].ToString().Trim());
                    if (IsQuantity != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Quantity field.");
                    }
                    IsTotalPrice = cImport.CheckNumericBool(dtPurchaseD.Rows[j]["Total_Price"].ToString().Trim());
                    if (IsTotalPrice != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Total_Price field.");
                    }
                    IsRebateRate = cImport.CheckNumericBool(dtPurchaseD.Rows[j]["Rebate_Rate"].ToString().Trim());
                    if (IsRebateRate != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Rebate_Rate field.");
                    }
                    IsSDAmount = cImport.CheckNumericBool(dtPurchaseD.Rows[j]["SD_Amount"].ToString().Trim());
                    if (IsSDAmount != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in SD_Amount field.");
                    }
                    IsVatAmount = cImport.CheckNumericBool(dtPurchaseD.Rows[j]["VAT_Amount"].ToString().Trim());
                    if (IsVatAmount != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in VAT_Amount field.");
                    }

                    #endregion
                }


                #endregion Details
                #region Duties
                if (dtPurchaseM.Rows[0]["Transection_Type"].ToString() == "Import"
                     || dtPurchaseM.Rows[0]["Transection_Type"].ToString() == "TradingImport"
                     || dtPurchaseM.Rows[0]["Transection_Type"].ToString() == "ServiceImport"
                     || dtPurchaseM.Rows[0]["Transection_Type"].ToString() == "InputServiceImport"
                     || dtPurchaseM.Rows[0]["Transection_Type"].ToString() == "ServiceNSImport")
                {

                    #region Row count for export details table
                    int IRowCount = 0;
                    for (int i = 0; i < dtPurchaseI.Rows.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(dtPurchaseI.Rows[i]["ID"].ToString()))
                        {
                            IRowCount++;
                        }

                    }
                    #endregion Row count for export details table

                    #region Duties

                    for (int e = 0; e < IRowCount; e++)
                    {
                        string ItemNo = string.Empty;
                        #region FindItemId
                        ItemNo = cImport.FindItemId(dtPurchaseI.Rows[e]["Item_Name"].ToString().Trim()
                                                            , dtPurchaseI.Rows[e]["Item_Code"].ToString().Trim(), currConn, transaction);
                        #endregion FindItemId
                        bool IsCnFAmt, IsInsuranceAmt, IsAssessableValue, IsCDAmt, IsRDAmt, IsTVBAmt, IsSDAmt, IsVATAmt, IsTVAAmt, IsATVAmt, IsOthersAmt;

                        IsCnFAmt = cImport.CheckNumericBool(dtPurchaseI.Rows[e]["CnF_Amount"].ToString().Trim());
                        if (IsCnFAmt != true)
                        {
                            throw new ArgumentNullException("Please insert decimal value in CnF_Amount field.");
                        }

                        IsInsuranceAmt = cImport.CheckNumericBool(dtPurchaseI.Rows[e]["Insurance_Amount"].ToString().Trim());
                        if (IsInsuranceAmt != true)
                        {
                            throw new ArgumentNullException("Please insert decimal value in Insurance_Amount field.");
                        }
                        IsAssessableValue = cImport.CheckNumericBool(dtPurchaseI.Rows[e]["Assessable_Value"].ToString().Trim());
                        if (IsAssessableValue != true)
                        {
                            throw new ArgumentNullException("Please insert decimal value in Assessable_Value field.");
                        }
                        IsCDAmt = cImport.CheckNumericBool(dtPurchaseI.Rows[e]["CD_Amount"].ToString().Trim());
                        if (IsCDAmt != true)
                        {
                            throw new ArgumentNullException("Please insert decimal value in CD_Amount field.");
                        }
                        IsRDAmt = cImport.CheckNumericBool(dtPurchaseI.Rows[e]["RD_Amount"].ToString().Trim());
                        if (IsRDAmt != true)
                        {
                            throw new ArgumentNullException("Please insert decimal value in RD_Amount field.");
                        }
                        IsTVBAmt = cImport.CheckNumericBool(dtPurchaseI.Rows[e]["TVB_Amount"].ToString().Trim());
                        if (IsTVBAmt != true)
                        {
                            throw new ArgumentNullException("Please insert decimal value in TVB_Amount field.");
                        }
                        IsSDAmt = cImport.CheckNumericBool(dtPurchaseI.Rows[e]["SD_Amount"].ToString().Trim());
                        if (IsSDAmt != true)
                        {
                            throw new ArgumentNullException("Please insert decimal value in SD_Amount field.");
                        }
                        IsVATAmt = cImport.CheckNumericBool(dtPurchaseI.Rows[e]["VAT_Amount"].ToString().Trim());
                        if (IsVATAmt != true)
                        {
                            throw new ArgumentNullException("Please insert decimal value in VAT_Amount field.");
                        }
                        IsTVAAmt = cImport.CheckNumericBool(dtPurchaseI.Rows[e]["TVA_Amount"].ToString().Trim());
                        if (IsTVAAmt != true)
                        {
                            throw new ArgumentNullException("Please insert decimal value in TVA_Amount field.");
                        }
                        IsATVAmt = cImport.CheckNumericBool(dtPurchaseI.Rows[e]["ATV_Amount"].ToString().Trim());
                        if (IsATVAmt != true)
                        {
                            throw new ArgumentNullException("Please insert decimal value in ATV_Amount field.");
                        }
                        IsOthersAmt = cImport.CheckNumericBool(dtPurchaseI.Rows[e]["Others_Amount"].ToString().Trim());
                        if (IsOthersAmt != true)
                        {
                            throw new ArgumentNullException("Please insert decimal value in Others_Amount field.");
                        }

                    }

                    #endregion Duties
                }

                #endregion Duties
                #region Trackings
                CommonDAL cmnDal = new CommonDAL();
                string IsTracking = cmnDal.settings("TrackingTrace", "Tracking");
                if (IsTracking == "Y")
                {
                    #region Row count for trackings table
                    int TRowCount = 0;
                    for (int i = 0; i < dtPurchaseT.Rows.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(dtPurchaseT.Rows[i]["ID"].ToString()))
                        {
                            TRowCount++;
                        }

                    }
                    #endregion Row count for trackings table

                    #region Trackings
                    #region Match Quantity
                    if (TRowCount > 0)
                    {
                        for (int detail = 0; detail < DRowCount; detail++)
                        {
                            string dItemNo = string.Empty;
                            decimal qty = 0;
                            decimal trackingQty = 0;


                            string dID = dtPurchaseD.Rows[detail]["ID"].ToString().Trim();
                            string productName = dtPurchaseD.Rows[detail]["Item_Name"].ToString().Trim();
                            dItemNo = cImport.FindItemId(dtPurchaseD.Rows[detail]["Item_Name"].ToString().Trim()
                                     , dtPurchaseD.Rows[detail]["Item_Code"].ToString().Trim(), currConn, transaction);
                            qty = Convert.ToDecimal(dtPurchaseD.Rows[detail]["Quantity"].ToString().Trim());

                            DataRow[] trackingRows = dtPurchaseT.Select("ID='" + dID + "'");
                            if (trackingRows.Length > 0)
                            {
                                foreach (var item in trackingRows)
                                {
                                    string tItemNo = cImport.FindItemId(item["Item_Name"].ToString().Trim()
                                                            , item["Item_Code"].ToString().Trim(), currConn, transaction);

                                    if (string.IsNullOrEmpty(item["Heading_1"].ToString().Trim()))
                                    {
                                        throw new ArgumentNullException("FindTrackingInfo", "Please insert value in Heading_1 field.");
                                    }

                                    if (dItemNo == tItemNo)
                                    {
                                        trackingQty++;
                                    }
                                }


                                if (qty != trackingQty)
                                {
                                    throw new ArgumentNullException("FindTrackingInfo", "Please insert tracking info for Item '(" + productName + ")'");
                                }
                            }
                        }
                    }

                    #endregion
                    //for (int t = 0; t < TRowCount; t++)
                    //{
                    //    string ItemNo = string.Empty;
                    //    #region FindItemId
                    //    ItemNo = cImport.FindItemId(dtPurchaseT.Rows[t]["Item_Name"].ToString().Trim()
                    //                                        , dtPurchaseT.Rows[t]["Item_Code"].ToString().Trim(), currConn, transaction);
                    //    #endregion FindItemId
                    //    if (string.IsNullOrEmpty(dtPurchaseT.Rows[t]["Heading_1"].ToString().Trim()))
                    //    {
                    //        throw new ArgumentNullException("Please insert value in Heading_1 field.");
                    //    }
                    //}


                    #endregion Trackings
                }
                #endregion Trackings

                #endregion checking from database is exist the information(NULL Check)

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                    currConn.Open();
                    transaction = currConn.BeginTransaction("Import Data.");
                }

                string vdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


                for (int i = 0; i < rowCount; i++)
                {

                    #region Master Purchase


                    string importID = dtPurchaseM.Rows[i]["ID"].ToString().Trim();
                    string vendorName = dtPurchaseM.Rows[i]["Vendor_Name"].ToString().Trim();
                    string vendorCode = dtPurchaseM.Rows[i]["Vendor_Code"].ToString().Trim();
                    string CustomHouse = dtPurchaseM.Rows[i]["Custom_House"].ToString().Trim();
                    #region FindVendorId
                    string vendorId = cImport.FindVendorId(vendorName, vendorCode, currConn, transaction);
                    #endregion FindVendorId
                    string invoiceDate = dtPurchaseM.Rows[i]["Invoice_Date"].ToString().Trim();

                    #region CheckNull
                    string referanceNo = cImport.ChecKNullValue(dtPurchaseM.Rows[i]["Referance_No"].ToString().Trim());
                    string bENumber = cImport.ChecKNullValue(dtPurchaseM.Rows[i]["BE_Number"].ToString().Trim());
                    #endregion CheckNull

                    string receiveDate = dtPurchaseM.Rows[i]["Receive_Date"].ToString().Trim();
                    string post = dtPurchaseM.Rows[i]["Post"].ToString().Trim();
                    string withVDS = dtPurchaseM.Rows[i]["With_VDS"].ToString().Trim();

                    #region Check Previous Purchase no.
                    string previousPurchaseNo = cImport.CheckPrePurchaseNo(dtPurchaseM.Rows[i]["Previous_Purchase_No"].ToString().Trim(), currConn, transaction);
                    #endregion Check Previous Purchase no.
                    #region CheckNull
                    string comments = dtPurchaseM.Rows[i]["Comments"].ToString().Trim();
                    string createdBy = dtPurchaseM.Rows[i]["Created_By"].ToString().Trim();
                    string lastModifiedBy = dtPurchaseM.Rows[i]["LastModified_By"].ToString().Trim();
                    string transactionType = dtPurchaseM.Rows[i]["Transection_Type"].ToString().Trim();

                    string lCNumber = "-";
                    string LCDate = "";
                    decimal LandedCost = 0;
                    if (IsCollCNo == true)
                    {
                        lCNumber = cImport.ChecKNullValue(dtPurchaseM.Rows[i]["LC_No"].ToString().Trim());
                    }
                    if (IsCollCDate == true)
                    {
                        if (!string.IsNullOrEmpty(dtPurchaseM.Rows[i]["LC_Date"].ToString().Trim()))
                        {
                            LCDate = Convert.ToDateTime(dtPurchaseM.Rows[i]["LC_Date"].ToString().Trim()).ToString("yyyy-MM-dd") + Convert.ToDateTime(vdateTime).ToString(" HH:mm:ss");
                        }
                    }

                    if (IsCollandedCost == true)
                    {
                        LandedCost = Convert.ToDecimal(cImport.ChecKNullValue(dtPurchaseM.Rows[i]["Landed_Cost"].ToString().Trim()));
                    }

                    #endregion CheckNull

                    #region Master Purchase
                    purchaseMaster = new PurchaseMasterVM();
                    purchaseMaster.VendorID = vendorId;
                    purchaseMaster.InvoiceDate = Convert.ToDateTime(invoiceDate).ToString("yyyy-MM-dd") + Convert.ToDateTime(vdateTime).ToString(" HH:mm:ss");
                    purchaseMaster.SerialNo = referanceNo.Replace(" ", "");
                    purchaseMaster.CustomHouse = CustomHouse;
                    purchaseMaster.Comments = comments;
                    purchaseMaster.CreatedBy = createdBy;
                    purchaseMaster.CreatedOn = vdateTime;
                    purchaseMaster.LastModifiedBy = lastModifiedBy;
                    purchaseMaster.LastModifiedOn = vdateTime;
                    purchaseMaster.BENumber = bENumber;
                    purchaseMaster.TransactionType = transactionType;
                    purchaseMaster.ReceiveDate = Convert.ToDateTime(receiveDate).ToString("yyyy-MM-dd") + Convert.ToDateTime(vdateTime).ToString(" HH:mm:ss");
                    purchaseMaster.Post = post;
                    purchaseMaster.ReturnId = previousPurchaseNo;

                    purchaseMaster.ProductType = "NA";
                    purchaseMaster.WithVDS = withVDS;
                    purchaseMaster.ImportID = importID;
                    purchaseMaster.LCNumber = lCNumber;
                    purchaseMaster.LCDate = LCDate;
                    purchaseMaster.LandedCost = LandedCost;

                    #endregion Master
                    #endregion Master Purchase

                    #region Import Details
                    if (dtPurchaseI.Rows.Count > 0)
                    {


                        DataRow[] ImportRaws;//= new DataRow[];//
                        if (!string.IsNullOrEmpty(importID))
                        {
                            ImportRaws = dtPurchaseI.Select("ID='" + importID + "'");
                        }
                        else
                        {
                            ImportRaws = null;
                        }

                        if (ImportRaws != null && ImportRaws.Length > 0)
                        {
                            DtPurchaseD = dtPurchaseD;
                            purchaseDuties = DutyCalculation(ImportRaws, currConn, transaction);
                        }
                    }

                    #endregion Import Details
                    #region fitemno
                    DataRow[] DetailRaws;//= new DataRow[];//
                    if (!string.IsNullOrEmpty(importID))
                    {

                        DetailRaws =
                           dtPurchaseD.Select("ID='" + importID + "'");

                    }
                    else
                    {
                        DetailRaws = null;
                    }


                    #endregion fitemno


                    #region Details Purchase

                    int Dcounter = 1;
                    decimal totalVatAmount = 0;
                    decimal TotalAmount = 0;

                    #region Juwel 13/10/2015
                    // 
                    purchaseDetails = new List<PurchaseDetailVM>();
                    DataTable dtDistinctItem = DetailRaws.CopyToDataTable().DefaultView.ToTable(true, "Item_Code", "Item_Name", "Type");
                    DataTable dtPurchasesDetail = DetailRaws.CopyToDataTable();

                    string uOM = "", uOMn = "", uOMc = "";

                    foreach (DataRow item in dtDistinctItem.Rows)
                    {
                        string itemCode, itemName, itemNo, type;

                        DataTable dtRepeatedItems = dtPurchasesDetail.Select("[Item_Code] ='" + item["Item_Code"].ToString() + "'").CopyToDataTable();
                        itemCode = item["Item_Code"].ToString().Trim();
                        itemName = item["Item_Name"].ToString().Trim();
                        itemNo = cImport.FindItemId(itemName, itemCode, currConn, transaction);
                        type = item["Type"].ToString().Trim();

                        decimal quantity = 0;
                        decimal totalPrice = 0;
                        decimal rebateRate = 0;
                        decimal sDAmount = 0;
                        decimal vATAmount = 0;

                        foreach (DataRow row in dtRepeatedItems.Rows)
                        {
                            if (DatabaseInfoVM.DatabaseName.ToString() == "Sanofi_DB")
                            {
                                uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                                uOM = uOMn;
                                uOMc = "1";
                            }
                            else
                            {
                                uOM = row["UOM"].ToString().Trim();
                                uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                                uOMc = cImport.FindUOMc(uOMn, uOM, currConn, transaction);
                            }

                            quantity = quantity + Convert.ToDecimal(row["Quantity"].ToString().Trim());
                            totalPrice = totalPrice + Convert.ToDecimal(row["Total_Price"].ToString().Trim());
                            rebateRate = rebateRate + Convert.ToDecimal(row["Rebate_Rate"].ToString().Trim());
                            sDAmount = sDAmount + Convert.ToDecimal(row["SD_Amount"].ToString().Trim());
                            vATAmount = vATAmount + Convert.ToDecimal(row["VAT_Amount"].ToString().Trim());
                        }

                        if (transactionType == "Import"
                    || transactionType == "TradingImport"
                    || transactionType == "ServiceImport"
                    || transactionType == "InputServiceImport"
                    || transactionType == "ServiceNSImport")
                        {
                            type = "Import-" + type;
                        }
                        else
                        {
                            type = "Local-" + type;
                        }

                        #region details

                        PurchaseDetailVM purchaseDetailVm = new PurchaseDetailVM();

                        purchaseDetailVm.LineNo = Dcounter.ToString();
                        purchaseDetailVm.ItemNo = itemNo.ToString();
                        purchaseDetailVm.Quantity = Convert.ToDecimal(quantity.ToString());

                        purchaseDetailVm.UOM = uOM.ToString();
                        purchaseDetailVm.Comments = "NA";
                        purchaseDetailVm.BENumber = bENumber.ToString();
                        purchaseDetailVm.Type = type.ToString();
                        purchaseDetailVm.ProductType = "NA";
                        purchaseDetailVm.UOMn = uOMn.ToString();
                        purchaseDetailVm.UOMc = Convert.ToDecimal(uOMc.ToString());
                        purchaseDetailVm.SubTotal = Convert.ToDecimal(totalPrice);
                        decimal unitPrice = Convert.ToDecimal(Convert.ToDecimal(totalPrice.ToString()) / Convert.ToDecimal(quantity.ToString()));
                        purchaseDetailVm.UnitPrice = unitPrice;
                        purchaseDetailVm.NBRPrice = unitPrice;
                        purchaseDetailVm.UOMPrice = Convert.ToDecimal(unitPrice) / Convert.ToDecimal(uOMc);
                        purchaseDetailVm.UOMQty = Convert.ToDecimal(quantity) * Convert.ToDecimal(uOMc);
                        purchaseDetailVm.VATAmount = Convert.ToDecimal(vATAmount);
                        purchaseDetailVm.SDAmount = Convert.ToDecimal(sDAmount);
                        purchaseDetailVm.RebateRate = Convert.ToDecimal(rebateRate.ToString());
                        purchaseDetailVm.RebateAmount =
                            Convert.ToDecimal(Convert.ToDecimal(vATAmount) * Convert.ToDecimal(rebateRate.ToString()) / 100);

                        purchaseDetailVm.VATRate = Convert.ToDecimal(Convert.ToDecimal(vATAmount) * 100 / Convert.ToDecimal(totalPrice));
                        purchaseDetailVm.SD = Convert.ToDecimal(Convert.ToDecimal(sDAmount) * 100 / Convert.ToDecimal(totalPrice));


                        if (purchaseDuties.Count > 0)
                        {

                            for (int dutyRow = 0; dutyRow < purchaseDuties.Count; dutyRow++)
                            {
                                if (purchaseDuties[dutyRow].ItemNo == itemNo)
                                {
                                    purchaseDetailVm.CnFAmount = purchaseDuties[dutyRow].CnFAmount;
                                    purchaseDetailVm.InsuranceAmount = purchaseDuties[dutyRow].InsuranceAmount;
                                    purchaseDetailVm.AssessableValue = purchaseDuties[dutyRow].AssessableValue;
                                    purchaseDetailVm.CDAmount = purchaseDuties[dutyRow].CDAmount;
                                    purchaseDetailVm.RDAmount = purchaseDuties[dutyRow].RDAmount;
                                    purchaseDetailVm.TVBAmount = purchaseDuties[dutyRow].TVBAmount;
                                    purchaseDetailVm.TVAAmount = purchaseDuties[dutyRow].TVAAmount;
                                    purchaseDetailVm.ATVAmount = purchaseDuties[dutyRow].ATVAmount;
                                    purchaseDetailVm.OthersAmount = purchaseDuties[dutyRow].OthersAmount;
                                }
                            }
                        }
                        else
                        {

                            purchaseDetailVm.CnFAmount = Convert.ToDecimal(0);
                            purchaseDetailVm.InsuranceAmount = Convert.ToDecimal(0);
                            purchaseDetailVm.AssessableValue = Convert.ToDecimal(0);
                            purchaseDetailVm.CDAmount = Convert.ToDecimal(0);
                            purchaseDetailVm.RDAmount = Convert.ToDecimal(0);
                            purchaseDetailVm.TVBAmount = Convert.ToDecimal(0);
                            purchaseDetailVm.TVAAmount = Convert.ToDecimal(0);
                            purchaseDetailVm.ATVAmount = Convert.ToDecimal(0);
                            purchaseDetailVm.OthersAmount = Convert.ToDecimal(0);
                        }

                        #endregion details

                        purchaseDetails.Add(purchaseDetailVm);
                        totalVatAmount = totalVatAmount + Convert.ToDecimal(vATAmount);
                        //vImpTotalPrice = (vImpAV1 + vImpCD1 + vImpRD1 + vImpTVB1 + vImpSD1 + vImpVAT1 + vImpATV1 +
                        //                             vImpOthers1);
                        TotalAmount = TotalAmount + (Convert.ToDecimal(totalPrice) + Convert.ToDecimal(vATAmount) + Convert.ToDecimal(sDAmount));
                        Dcounter++;
                    }

                    //
                    #endregion juwel

                    //purchaseDetails = new List<PurchaseDetailVM>();
                    //foreach (DataRow row in DetailRaws)
                    //{
                    //    string itemCode = row["Item_Code"].ToString().Trim();
                    //    string itemName = row["Item_Name"].ToString().Trim();
                    //    string itemNo = cImport.FindItemId(itemName, itemCode, currConn, transaction);

                    //    string quantity = row["Quantity"].ToString().Trim();
                    //    string totalPrice = row["Total_Price"].ToString().Trim();
                    //    string uOM = "";
                    //    string uOMn = "";
                    //    string uOMc = "";
                    //    if (DatabaseInfoVM.DatabaseName.ToString() == "Sanofi_DB")
                    //    {
                    //        uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                    //        uOM = uOMn;
                    //        uOMc = "1";
                    //    }
                    //    else
                    //    {
                    //        uOM = row["UOM"].ToString().Trim();
                    //        uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                    //        uOMc = cImport.FindUOMc(uOMn, uOM, currConn, transaction);
                    //    }

                    //    string type = row["Type"].ToString().Trim();
                    //    string rebateRate = row["Rebate_Rate"].ToString().Trim();
                    //    string sDAmount = row["SD_Amount"].ToString().Trim();
                    //    string vATAmount = row["VAT_Amount"].ToString().Trim();

                    //    if (transactionType == "Import"
                    //|| transactionType == "TradingImport"
                    //|| transactionType == "ServiceImport"
                    //|| transactionType == "InputServiceImport"
                    //|| transactionType == "ServiceNSImport")
                    //    {
                    //        type = "Import-" + type;
                    //    }
                    //    else
                    //    {
                    //        type = "Local-" + type;
                    //    }

                    //    #region details

                    //    PurchaseDetailVM purchaseDetailVm = new PurchaseDetailVM();

                    //    purchaseDetailVm.LineNo = Dcounter.ToString();
                    //    purchaseDetailVm.ItemNo = itemNo.ToString();
                    //    purchaseDetailVm.Quantity = Convert.ToDecimal(quantity.ToString());

                    //    purchaseDetailVm.UOM = uOM.ToString();
                    //    purchaseDetailVm.Comments = "NA";
                    //    purchaseDetailVm.BENumber = bENumber.ToString();
                    //    purchaseDetailVm.Type = type.ToString();
                    //    purchaseDetailVm.ProductType = "NA";
                    //    purchaseDetailVm.UOMn = uOMn.ToString();
                    //    purchaseDetailVm.UOMc = Convert.ToDecimal(uOMc.ToString());
                    //    purchaseDetailVm.SubTotal = Convert.ToDecimal(totalPrice);
                    //    decimal unitPrice = Convert.ToDecimal(Convert.ToDecimal(totalPrice.ToString()) / Convert.ToDecimal(quantity.ToString()));
                    //    purchaseDetailVm.UnitPrice = unitPrice;
                    //    purchaseDetailVm.NBRPrice = unitPrice;
                    //    purchaseDetailVm.UOMPrice = Convert.ToDecimal(unitPrice) / Convert.ToDecimal(uOMc);
                    //    purchaseDetailVm.UOMQty = Convert.ToDecimal(quantity) * Convert.ToDecimal(uOMc);
                    //    purchaseDetailVm.VATAmount = Convert.ToDecimal(vATAmount);
                    //    purchaseDetailVm.SDAmount = Convert.ToDecimal(sDAmount);
                    //    purchaseDetailVm.RebateRate = Convert.ToDecimal(rebateRate.ToString());
                    //    purchaseDetailVm.RebateAmount =
                    //        Convert.ToDecimal(Convert.ToDecimal(vATAmount) * Convert.ToDecimal(rebateRate.ToString()) / 100);

                    //    purchaseDetailVm.VATRate = Convert.ToDecimal(Convert.ToDecimal(vATAmount) * 100 / Convert.ToDecimal(totalPrice));
                    //    purchaseDetailVm.SD = Convert.ToDecimal(Convert.ToDecimal(sDAmount) * 100 / Convert.ToDecimal(totalPrice));


                    //    if (purchaseDuties.Count > 0)
                    //    {

                    //        for (int dutyRow = 0; dutyRow < purchaseDuties.Count; dutyRow++)
                    //        {
                    //            if (purchaseDuties[dutyRow].ItemNo == itemNo)
                    //            {
                    //                purchaseDetailVm.CnFAmount = purchaseDuties[dutyRow].CnFAmount;
                    //                purchaseDetailVm.InsuranceAmount = purchaseDuties[dutyRow].InsuranceAmount;
                    //                purchaseDetailVm.AssessableValue = purchaseDuties[dutyRow].AssessableValue;
                    //                purchaseDetailVm.CDAmount = purchaseDuties[dutyRow].CDAmount;
                    //                purchaseDetailVm.RDAmount = purchaseDuties[dutyRow].RDAmount;
                    //                purchaseDetailVm.TVBAmount = purchaseDuties[dutyRow].TVBAmount;
                    //                purchaseDetailVm.TVAAmount = purchaseDuties[dutyRow].TVAAmount;
                    //                purchaseDetailVm.ATVAmount = purchaseDuties[dutyRow].ATVAmount;
                    //                purchaseDetailVm.OthersAmount = purchaseDuties[dutyRow].OthersAmount;
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {

                    //        purchaseDetailVm.CnFAmount = Convert.ToDecimal(0);
                    //        purchaseDetailVm.InsuranceAmount = Convert.ToDecimal(0);
                    //        purchaseDetailVm.AssessableValue = Convert.ToDecimal(0);
                    //        purchaseDetailVm.CDAmount = Convert.ToDecimal(0);
                    //        purchaseDetailVm.RDAmount = Convert.ToDecimal(0);
                    //        purchaseDetailVm.TVBAmount = Convert.ToDecimal(0);
                    //        purchaseDetailVm.TVAAmount = Convert.ToDecimal(0);
                    //        purchaseDetailVm.ATVAmount = Convert.ToDecimal(0);
                    //        purchaseDetailVm.OthersAmount = Convert.ToDecimal(0);
                    //    }

                    //    #endregion details

                    //    purchaseDetails.Add(purchaseDetailVm);
                    //    totalVatAmount = totalVatAmount + Convert.ToDecimal(vATAmount);
                    //    //vImpTotalPrice = (vImpAV1 + vImpCD1 + vImpRD1 + vImpTVB1 + vImpSD1 + vImpVAT1 + vImpATV1 +
                    //    //                             vImpOthers1);
                    //    TotalAmount = TotalAmount + (Convert.ToDecimal(totalPrice) + Convert.ToDecimal(vATAmount) + Convert.ToDecimal(sDAmount));
                    //    Dcounter++;

                    //}
                    #endregion Details Purchase
                    purchaseMaster.TotalVATAmount = totalVatAmount;
                    purchaseMaster.TotalAmount = TotalAmount;


                    #region Tracking
                    #region fitemno
                    DataRow[] TrackingRaws;//= new DataRow[];//
                    if (dtPurchaseT != null && dtPurchaseT.Rows.Count > 0)
                    {
                        TrackingRaws =
                           dtPurchaseT.Select("ID='" + importID + "'");
                    }
                    else
                    {
                        TrackingRaws = null;
                    }
                    purchaseTrackings = new List<TrackingVM>();
                    if (TrackingRaws != null && TrackingRaws.Length > 0)
                    {

                        int lineNoT = 1;

                        foreach (DataRow row in TrackingRaws)
                        {
                            string itemCode = row["Item_Code"].ToString().Trim();
                            string itemName = row["Item_Name"].ToString().Trim();
                            string itemNo = cImport.FindItemId(itemName, itemCode, currConn, transaction);

                            string heading1 = row["Heading_1"].ToString().Trim();
                            string heading2 = row["Heading_2"].ToString().Trim();
                            string quantity = "0";
                            string totalPrice = "0";

                            foreach (DataRow row1 in DetailRaws)
                            {
                                string itemCode1 = row1["Item_Code"].ToString().Trim();
                                string itemName1 = row1["Item_Name"].ToString().Trim();
                                string itemNo1 = cImport.FindItemId(itemName1, itemCode1, currConn, transaction);

                                if (itemNo1 == itemNo)
                                {
                                    quantity = row1["Quantity"].ToString().Trim();
                                    totalPrice = row1["Total_Price"].ToString().Trim();

                                }

                            }
                            TrackingVM purchaseTrackingVm = new TrackingVM();
                            purchaseTrackingVm.TrackingLineNo = lineNoT.ToString();
                            purchaseTrackingVm.ItemNo = itemNo;
                            purchaseTrackingVm.Heading1 = heading1;
                            purchaseTrackingVm.Heading2 = heading2;
                            purchaseTrackingVm.Quantity = 1;
                            decimal unitPrice = Convert.ToDecimal(Convert.ToDecimal(totalPrice.ToString()) / Convert.ToDecimal(quantity.ToString()));
                            purchaseTrackingVm.UnitPrice = unitPrice;

                            purchaseTrackingVm.IsPurchase = "Y";
                            purchaseTrackingVm.IsIssue = "N";
                            purchaseTrackingVm.IsReceive = "N";
                            purchaseTrackingVm.IsSale = "N";

                            purchaseTrackings.Add(purchaseTrackingVm);
                            lineNoT++;
                        }
                    }


                    #endregion fitemno
                    #endregion

                    string[] sqlResults = PurchaseInsert(purchaseMaster, purchaseDetails, purchaseDuties, purchaseTrackings, transaction, currConn);
                    retResults[0] = sqlResults[0];
                }
                if (retResults[0] == "Success")
                {
                    transaction.Commit();
                    #region SuccessResult

                    retResults[0] = "Success";
                    retResults[1] = MessageVM.PurchasemsgSaveSuccessfully;
                    retResults[2] = "" + "1";
                    retResults[3] = "" + "N";
                    #endregion SuccessResult
                }
            }
            #endregion try
            #region catch & final
            catch (SqlException sqlex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (ArgumentNullException aeg)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + aeg.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion catch & final

            return retResults;
        }
        //List<PurchaseDutiesVM>
        private List<PurchaseDutiesVM> DutyCalculation(DataRow[] ImportRaws, SqlConnection currConn, SqlTransaction transaction)
        {
            #region settings

            bool IsTotalPrice, FixedVAT, FixedCnF, FixedInsurance, CalculativeAV, FixedCD, FixedRD, FixedTVB, FixedTVA, FixedATV, FixedOthers, FixedSD, FixedVATImp;

            int PurchasePlaceQty;
            int PurchasePlaceAmt;
            decimal RateChangePromote;


            CommonDAL commonDal = new CommonDAL();

            string vPurchasePlaceQty, vPurchasePlaceAmt, vIsTotalPrice, vFixedVAT
                , vFixedCnF, vFixedInsurance, vCalculativeAV, vFixedCD
                , vFixedRD, vFixedTVB, vFixedSD, vFixedVATImp, vFixedTVA, vFixedATV
                , vFixedOthers, vRateChangePromotePercent
                = string.Empty;

            vPurchasePlaceQty = commonDal.settings("Purchase", "Quantity");
            vPurchasePlaceAmt = commonDal.settings("Purchase", "Amount");
            vIsTotalPrice = commonDal.settings("Purchase", "TotalPrice");
            vFixedVAT = commonDal.settings("Purchase", "FixedVAT");
            vFixedCnF = commonDal.settings("ImportPurchase", "FixedCnF");
            vFixedInsurance = commonDal.settings("ImportPurchase", "FixedInsurance");
            vCalculativeAV = commonDal.settings("ImportPurchase", "CalculativeAV");
            vFixedCD = commonDal.settings("ImportPurchase", "FixedCD");
            vFixedRD = commonDal.settings("ImportPurchase", "FixedRD");
            vFixedTVB = commonDal.settings("ImportPurchase", "FixedTVB");
            vFixedSD = commonDal.settings("ImportPurchase", "FixedSD");
            vFixedVATImp = commonDal.settings("ImportPurchase", "FixedVAT");
            vFixedTVA = commonDal.settings("ImportPurchase", "FixedTVA");
            vFixedATV = commonDal.settings("ImportPurchase", "FixedATV");
            vFixedOthers = commonDal.settings("ImportPurchase", "FixedOthers");

            vRateChangePromotePercent = commonDal.settings("Purchase", "RateChangePromote");

            RateChangePromote = Convert.ToDecimal(vRateChangePromotePercent);
            PurchasePlaceQty = Convert.ToInt32(vPurchasePlaceQty);
            PurchasePlaceAmt = Convert.ToInt32(vPurchasePlaceAmt);
            IsTotalPrice = Convert.ToBoolean(vIsTotalPrice == "Y" ? true : false);
            FixedVAT = Convert.ToBoolean(vFixedVAT == "Y" ? true : false);
            FixedCnF = Convert.ToBoolean(vFixedCnF == "Y" ? true : false);
            FixedInsurance = Convert.ToBoolean(vFixedInsurance == "Y" ? true : false);
            CalculativeAV = Convert.ToBoolean(vCalculativeAV == "Y" ? true : false);
            FixedCD = Convert.ToBoolean(vFixedCD == "Y" ? true : false);
            FixedRD = Convert.ToBoolean(vFixedRD == "Y" ? true : false);
            FixedTVB = Convert.ToBoolean(vFixedTVB == "Y" ? true : false);
            FixedTVA = Convert.ToBoolean(vFixedTVA == "Y" ? true : false);
            FixedATV = Convert.ToBoolean(vFixedATV == "Y" ? true : false);
            FixedOthers = Convert.ToBoolean(vFixedOthers == "Y" ? true : false);
            FixedSD = Convert.ToBoolean(vFixedSD == "Y" ? true : false);
            FixedVATImp = Convert.ToBoolean(vFixedVATImp == "Y" ? true : false);

            #endregion settings
            #region Declare Value

            decimal vsubTotal = 0;
            decimal vInpCnf = 0;
            decimal vInpInsurance = 0;
            decimal vInpAV = 0;
            decimal vAVAmount = 0;
            decimal vInpCD = 0;
            decimal vInpRD = 0;
            decimal vInpTVB = 0;
            decimal vInpSD = 0;
            decimal vInpVAT = 0;
            decimal vInpTVA = 0;
            decimal vInpATV = 0;
            decimal vInpOthers = 0;

            decimal vCnfRate = 0;
            decimal vInsuranceRate = 0;
            decimal vAVRate = 0;
            //decimal vAVAmount = 0;
            decimal vCDRate = 0;
            decimal vRDRate = 0;
            decimal vTVBRate = 0;
            decimal vSDRate = 0;
            decimal vVATRate = 0;
            decimal vTVARate = 0;
            decimal vATVRate = 0;
            decimal vOthersRate = 0;

            decimal vCnfAmt = 0;
            decimal vInsuranceAmt = 0;
            decimal vAVAmt = 0;
            //decimal vAVAmount = 0;
            decimal vCDAmt = 0;
            decimal vRDAmt = 0;
            decimal vTVBAmt = 0;
            decimal vSDAmt = 0;
            decimal vVATAmt = 0;
            decimal vTVAAmt = 0;
            decimal vATVAmt = 0;
            decimal vOthersAmt = 0;

            #endregion Declare Value

            CommonImport cImport = new CommonImport();
            List<PurchaseDutiesVM> purchaseDuties = new List<PurchaseDutiesVM>();
            try
            {

                int Icounter = 1;
                foreach (DataRow row in ImportRaws)
                {
                    string itemCode = row["Item_Code"].ToString().Trim();
                    string itemName = row["Item_Name"].ToString().Trim();
                    string itemNo = cImport.FindItemId(itemName, itemCode, currConn, transaction);

                    #region Input Value
                    vInpCnf = Convert.ToDecimal(row["CnF_Amount"].ToString().Trim());
                    vInpInsurance = Convert.ToDecimal(row["Insurance_Amount"].ToString().Trim());

                    vInpAV = Convert.ToDecimal(row["Assessable_Value"].ToString().Trim());
                    //vAVAmount = Convert.ToDecimal(row["Item_Code"].ToString().Trim());

                    vInpCD = Convert.ToDecimal(row["CD_Amount"].ToString().Trim());
                    vInpRD = Convert.ToDecimal(row["RD_Amount"].ToString().Trim());
                    vInpTVB = Convert.ToDecimal(row["TVB_Amount"].ToString().Trim());
                    vInpSD = Convert.ToDecimal(row["SD_Amount"].ToString().Trim());

                    vInpVAT = Convert.ToDecimal(row["VAT_Amount"].ToString().Trim());
                    vInpTVA = Convert.ToDecimal(row["TVA_Amount"].ToString().Trim());
                    vInpATV = Convert.ToDecimal(row["ATV_Amount"].ToString().Trim());
                    vInpOthers = Convert.ToDecimal(row["Others_Amount"].ToString().Trim());

                    #endregion Input Value

                    #region Retrive quantity and total price from details
                    //string vTotalAmt = DtPurchaseD.Select("ID='" + row["ID"] + "' and Item_Code='" + itemNo + "'").First()["Total_Price"].ToString();
                    //string vQuantity = DtPurchaseD.Select("ID='" + row["ID"] + "' and Item_Code='" + itemNo + "'").First()["Quantity"].ToString();
                    string vTotalAmt = "";
                    string vQuantity = "";
                    DataRow[] drDetailsRow = DtPurchaseD.Select("ID='" + row["ID"] + "'");
                    if (drDetailsRow.Length > 0)
                    {
                        foreach (var detailItem in drDetailsRow)
                        {
                            string dItemNo = cImport.FindItemId(detailItem["Item_Name"].ToString(), detailItem["Item_Code"].ToString(), currConn, transaction);

                            if (dItemNo == itemNo)
                            {
                                vTotalAmt = detailItem["Total_Price"].ToString();
                                vQuantity = detailItem["Quantity"].ToString();
                            }
                        }
                    }
                    vsubTotal = Convert.ToDecimal(vTotalAmt);

                    #endregion Retrive quantity and total price from details

                    #region InputValue

                    //if (!string.IsNullOrEmpty(txtCnFInpValue.Text.Trim()))
                    //    vInpCnf = Convert.ToDecimal(txtCnFInpValue.Text.Trim());
                    //else
                    //    vInpCnf = 0;

                    //if (!string.IsNullOrEmpty(txtInsInpValue.Text.Trim()))
                    //    vInpInsurance = Convert.ToDecimal(txtInsInpValue.Text.Trim());
                    //else
                    //    vInpInsurance = 0;

                    //if (!string.IsNullOrEmpty(txtAVInpValue.Text.Trim()))
                    //    vInpAV = Convert.ToDecimal(txtAVInpValue.Text.Trim());
                    //else
                    //    vInpAV = 0;

                    //if (!string.IsNullOrEmpty(txtCDInpValue.Text.Trim()))
                    //    vInpCD = Convert.ToDecimal(txtCDInpValue.Text.Trim());
                    //else
                    //    vInpCD = 0;



                    //if (!string.IsNullOrEmpty(txtRDInpValue.Text.Trim()))
                    //    vInpRD = Convert.ToDecimal(txtRDInpValue.Text.Trim());
                    //else
                    //    vInpRD = 0;

                    //if (!string.IsNullOrEmpty(txtTVBInpValue.Text.Trim()))
                    //    vInpTVB = Convert.ToDecimal(txtTVBInpValue.Text.Trim());
                    //else
                    //    vInpTVB = 0;

                    //if (!string.IsNullOrEmpty(txtSDInpValue.Text.Trim()))
                    //    vInpSD = Convert.ToDecimal(txtSDInpValue.Text.Trim());
                    //else
                    //    vInpSD = 0;

                    //if (!string.IsNullOrEmpty(txtVATInpValue.Text.Trim()))
                    //    vInpVAT = Convert.ToDecimal(txtVATInpValue.Text.Trim());
                    //else
                    //    vInpVAT = 0;

                    //if (!string.IsNullOrEmpty(txtTVAInpValue.Text.Trim()))
                    //    vInpTVA = Convert.ToDecimal(txtTVAInpValue.Text.Trim());
                    //else
                    //    vInpTVA = 0;



                    //if (!string.IsNullOrEmpty(txtATVInpValue.Text.Trim()))
                    //    vInpATV = Convert.ToDecimal(txtATVInpValue.Text.Trim());
                    //else
                    //    vInpATV = 0;

                    //if (!string.IsNullOrEmpty(txtOthersInpValue.Text.Trim()))
                    //    vInpOthers = Convert.ToDecimal(txtOthersInpValue.Text.Trim());
                    //else
                    //    vInpOthers = 0;
                    #endregion InputValue

                    #region FixedCnF
                    if (FixedCnF == true)
                    {
                        //txtCnFAmount.Text = vInpCnf.ToString();
                        //txtCnFRate.Text = Convert.ToString(vInpCnf * 100 / vsubTotal);
                        vCnfAmt = vInpCnf;
                        vCnfRate = vInpCnf * 100 / vsubTotal;
                    }
                    else
                    {
                        //txtCnFAmount.Text = Convert.ToString(vsubTotal * vInpCnf / 100);
                        //txtCnFRate.Text = vInpCnf.ToString();
                        vCnfAmt = vsubTotal * vInpCnf / 100;
                        vCnfRate = vInpCnf;
                    }
                    #endregion FixedCnF
                    #region FixedInsurance

                    decimal vTotalInsuranceApply = 0;
                    vTotalInsuranceApply = vsubTotal + vCnfAmt;

                    if (FixedInsurance == true)
                    {
                        //txtInsAmount.Text = vInpInsurance.ToString();
                        //txtInsRate.Text = Convert.ToString(vInpInsurance * 100 / vTotalInsuranceApply);

                        vInsuranceAmt = vInpInsurance;
                        vInsuranceRate = vInpInsurance * 100 / vTotalInsuranceApply;
                    }
                    else
                    {
                        //txtInsAmount.Text = Convert.ToString(vTotalInsuranceApply * vInpInsurance / 100);
                        //txtInsRate.Text = vInpInsurance.ToString();
                        vInsuranceAmt = vTotalInsuranceApply * vInpInsurance / 100;
                        vInsuranceRate = vInpInsurance;
                    }
                    #endregion FixedInsurance
                    #region CalculativeAV

                    if (CalculativeAV == true)
                    {
                        vAVAmount = vsubTotal + vCnfAmt + vInsuranceAmt;
                    }
                    else
                    {
                        vAVAmount = vInpAV;
                    }
                    //txtAVAmount.Text = Convert.ToString(vAVAmount);
                    //if (vAVAmount == 0)
                    //vAVAmount = 1;
                    //if (!string.IsNullOrEmpty(txtAVAmount.Text.Trim()))
                    //    vAVAmount = Convert.ToDecimal(txtAVAmount.Text.Trim());
                    //else
                    //{
                    //    MessageBox.Show("There is no value in AV field, Please input AV first", this.Text);
                    //    return;
                    //}
                    //vsubTotal = 1;
                    if (vAVAmount == 0)
                    {
                        //MessageBox.Show("There is no value in AV field, Please input AV first", this.Text);
                        throw new ArgumentException("There is no value in AV field, Please input AV first");

                        //return;
                    }

                    #endregion CalculativeAV

                    #region FixedCD
                    if (FixedCD == true)
                    {
                        //txtCDAmount.Text = vInpCD.ToString();
                        //txtCDRate.Text = Convert.ToString(vInpCD * 100 / vAVAmount);

                        vCDAmt = vInpCD;
                        vCDRate = vInpCD * 100 / vAVAmount;
                    }
                    else
                    {
                        //txtCDAmount.Text = Convert.ToString(vAVAmount * vInpCD / 100);
                        //txtCDRate.Text = vInpCD.ToString();

                        vCDAmt = vAVAmount * vInpCD / 100;
                        vCDRate = vInpCD;
                    }
                    #endregion FixedCD
                    #region FixedRD
                    if (FixedRD == true)
                    {
                        vRDAmt = vInpRD;
                        vRDRate = vInpRD * 100 / vAVAmount;
                    }
                    else
                    {
                        //txtRDAmount.Text = Convert.ToString(vAVAmount * vInpRD / 100);
                        //txtRDRate.Text = vInpRD.ToString();

                        vRDAmt = vAVAmount * vInpRD / 100;
                        vRDRate = vInpRD;
                    }
                    #endregion FixedCD
                    #region FixedTVB

                    decimal vTotalTVBApply = 0;
                    vTotalTVBApply = vAVAmount + vCDAmt + vRDAmt;

                    if (FixedTVB == true)
                    {
                        //txtTVBAmount.Text = vInpTVB.ToString();
                        //txtTVBRate.Text = Convert.ToString(vInpTVB * 100 / vTotalTVBApply);

                        vTVBAmt = vInpTVB;
                        vTVBRate = vInpTVB * 100 / vTotalTVBApply;
                    }
                    else
                    {
                        //txtTVBAmount.Text = Convert.ToString(vTotalTVBApply * vInpTVB / 100);
                        //txtTVBRate.Text = vInpTVB.ToString();

                        vTVBAmt = vTotalTVBApply * vInpTVB / 100;
                        vTVBRate = vInpTVB;
                    }

                    #endregion FixedTVB
                    #region FixedSD

                    decimal vTotalSDApply = 0;
                    vTotalSDApply = vTotalTVBApply + vTVBAmt;

                    if (FixedSD == true)
                    {
                        //txtSDAmount.Text = vInpSD.ToString();
                        //txtSDRate.Text = Convert.ToString(vInpSD * 100 / vTotalSDApply);

                        vSDAmt = vInpSD;
                        vSDRate = vInpSD * 100 / vTotalSDApply;
                    }
                    else
                    {
                        //txtSDAmount.Text = Convert.ToString(vTotalSDApply * vInpSD / 100);
                        //txtSDRate.Text = vInpSD.ToString();

                        vSDAmt = vTotalSDApply * vInpSD / 100;
                        vSDRate = vInpSD;
                    }

                    #endregion FixedSD
                    #region FixedVAT

                    decimal vTotalVATApply = 0;
                    vTotalVATApply = vTotalSDApply + vSDAmt;

                    if (FixedVATImp == true)
                    {
                        //txtVATAmount.Text = vInpVAT.ToString();
                        //txtVATRateI.Text = Convert.ToString(vInpVAT * 100 / vTotalVATApply);

                        vVATAmt = vInpVAT;
                        vVATRate = vInpVAT * 100 / vTotalVATApply;
                    }
                    else
                    {
                        //txtVATAmount.Text = Convert.ToString(vTotalVATApply * vInpVAT / 100);
                        //txtVATRateI.Text = vInpVAT.ToString();

                        vVATAmt = vTotalVATApply * vInpVAT / 100;
                        vVATRate = vInpVAT;
                    }

                    #endregion FixedVAT
                    #region FixedTVA
                    vTotalVATApply = vTotalSDApply + vSDAmt;
                    if (FixedTVA == true)
                    {
                        //txtTVAAmount.Text = vInpTVA.ToString();
                        //txtTVARate.Text = Convert.ToString(vInpTVA * 100 / vTotalVATApply);

                        vTVAAmt = vInpTVA;
                        vTVARate = vInpTVA * 100 / vTotalVATApply;
                    }
                    else
                    {
                        //txtTVAAmount.Text = Convert.ToString(vTotalVATApply * vInpTVA / 100);
                        //txtTVARate.Text = vInpTVA.ToString();

                        vTVAAmt = vTotalVATApply * vInpTVA / 100;
                        vTVARate = vInpTVA;
                    }

                    #endregion FixedTVA
                    #region FixedATV
                    decimal vTotalATVApply = 0;
                    vTotalATVApply = vTotalVATApply + vTVAAmt;

                    if (FixedATV == true)
                    {
                        //txtATVAmount.Text = vInpATV.ToString();
                        //txtATVRate.Text = Convert.ToString(vInpATV * 100 / vTotalATVApply);
                        vATVAmt = vInpATV;
                        vATVRate = vInpATV * 100 / vTotalATVApply;
                    }
                    else
                    {
                        //txtATVAmount.Text = Convert.ToString(vTotalATVApply * vInpATV / 100);
                        //txtATVRate.Text = vInpATV.ToString();

                        vATVAmt = vTotalATVApply * vInpATV / 100;
                        vATVRate = vInpATV;
                    }

                    #endregion FixedTVA
                    #region FixedOthers
                    decimal vTotalOthersApply = 0;
                    vTotalOthersApply = vTotalVATApply;

                    if (FixedOthers == true)
                    {
                        //txtOthersAmount.Text = vInpOthers.ToString();
                        //txtOthersRate.Text = Convert.ToString(vInpOthers * 100 / vTotalOthersApply);

                        vOthersAmt = vInpOthers;
                        vOthersRate = vInpOthers * 100 / vTotalOthersApply;
                    }
                    else
                    {
                        //txtOthersAmount.Text = Convert.ToString(vTotalOthersApply * vInpOthers / 100);
                        //txtOthersRate.Text = vInpOthers.ToString();

                        vOthersAmt = vTotalOthersApply * vInpOthers / 100;
                        vOthersRate = vInpOthers;
                    }

                    #endregion FixedOthers
                    #region details

                    PurchaseDutiesVM purchaseDuty = new PurchaseDutiesVM();

                    purchaseDuty.PIDutyID = "";
                    purchaseDuty.ItemNo = itemNo;
                    purchaseDuty.Quantity = Convert.ToDecimal(vQuantity);
                    //purchaseDuty.l

                    purchaseDuty.CnFInp = vInpCnf;
                    purchaseDuty.CnFRate = vCnfRate;
                    purchaseDuty.CnFAmount = vCnfAmt;

                    purchaseDuty.InsuranceInp = vInpInsurance;
                    purchaseDuty.InsuranceRate = vInsuranceRate;
                    purchaseDuty.InsuranceAmount = vInsuranceAmt;

                    purchaseDuty.AssessableInp = vInpAV;
                    purchaseDuty.AssessableValue = vAVAmount;

                    purchaseDuty.CDInp = vInpCD;
                    purchaseDuty.CDRate = vCDRate;
                    purchaseDuty.CDAmount = vCDAmt;

                    purchaseDuty.RDInp = vInpRD;
                    purchaseDuty.RDRate = vRDRate;
                    purchaseDuty.RDAmount = vRDAmt;

                    purchaseDuty.TVBInp = vInpTVB;
                    purchaseDuty.TVBRate = vTVBRate;
                    purchaseDuty.TVBAmount = vTVBAmt;

                    purchaseDuty.SDInp = vInpSD;
                    purchaseDuty.SD = vSDRate;
                    purchaseDuty.SDAmount = vSDAmt;

                    purchaseDuty.VATInp = vInpVAT;
                    purchaseDuty.VATRate = vVATRate;
                    purchaseDuty.VATAmount = vVATAmt;

                    purchaseDuty.TVAInp = vInpTVA;
                    purchaseDuty.TVARate = vTVARate;
                    purchaseDuty.TVAAmount = vTVAAmt;

                    purchaseDuty.ATVInp = vInpATV;
                    purchaseDuty.ATVRate = vATVRate;
                    purchaseDuty.ATVAmount = vATVAmt;

                    purchaseDuty.OthersInp = vInpOthers;
                    purchaseDuty.OthersRate = vOthersRate;
                    purchaseDuty.OthersAmount = vOthersAmt;
                    purchaseDuty.Remarks = row["Remarks"].ToString();

                    purchaseDuty.SetCost();
                    purchaseDuties.Add(purchaseDuty);

                    Icounter++;
                    #endregion details

                }
            }
            catch (ArgumentNullException aeg)
            {
                //if (transaction != null)
                //{
                //    transaction.Rollback();
                //}
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + aeg.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            catch (Exception ex)
            {
                //if (transaction != null)
                //{
                //    transaction.Rollback();
                //}
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            return purchaseDuties;

        }
        public void SetCost()
        {
            //LineCost = CnFAmount +
            //           InsuranceAmount +
            //           AssessableValue +
            //           CDAmount +
            //           RDAmount +
            //           TVBAmount +
            //           SDAmount +
            //           VATAmount +
            //           TVAAmount +
            //           ATVAmount +
            //           OthersAmount;
            //if (LineCost > 0)
            //{
            //    UnitCost = LineCost / Quantity;
            //}
        }
        public decimal ReturnQty(string purchaseReturnId, string itemNo)
        {
            #region Initializ

            decimal retResults = 0;
            int countId = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region Return Qty

                sqlText = "  ";

                sqlText = "select Sum(isnull(PurchaseInvoiceDetails.Quantity,0)) from PurchaseInvoiceDetails ";
                sqlText += "where ItemNo = @itemNo and PurchaseReturnId =@purchaseReturnId";
                sqlText += "group by ItemNo";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                cmd.Parameters.AddWithValue("@purchaseReturnId", purchaseReturnId);
                cmd.Parameters.AddWithValue("@itemNo", itemNo);


                if (cmd.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmd.ExecuteScalar();
                }

                #endregion Return Qty

            }

            #endregion try

            #region Catch and Finall
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
            finally
            {
                if (currConn == null)
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

        public string FindItemIdFromPurchase(string PurchaseInvoiceNo, string ItemNo, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                if (vcurrConn == null)
                {
                    if (currConn == null)
                    {
                        currConn = _dbsqlConnection.GetConnection();
                        if (currConn.State != ConnectionState.Open)
                        {
                            currConn.Open();
                            transaction = currConn.BeginTransaction("Import Data");
                        }
                    }
                }

                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(PurchaseInvoiceNo) && string.IsNullOrEmpty(ItemNo))
                {
                    throw new ArgumentNullException("FindItemId", "Invalid Item Information");
                }


                #endregion Validation
                #region Find

                sqlText = " ";
                sqlText = @" SELECT isnull(sum(AssessableValue),0)AssessableValue
FROM PurchaseInvoiceDuties PID  
WHERE 1=1";
                sqlText += "and PurchaseInvoiceNo=@PurchaseInvoiceNo  ";
                sqlText += "and ItemNo=@ItemNo ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;

                cmd1.Parameters.AddWithValue("@PurchaseInvoiceNo", PurchaseInvoiceNo);
                cmd1.Parameters.AddWithValue("@ItemNo", ItemNo);

                object obj1 = cmd1.ExecuteScalar();

                retResults = obj1.ToString();





                #endregion Find
            }

            #endregion try

            #region Catch and Finall

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

            finally
            {
                if (vcurrConn == null)
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

        public string[] PurchaseUpdateDuty(DataTable DtDuty)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            string importID = "";
            string ItemNo = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            string PostStatus = "";
            ProductDAL productDal = new ProductDAL();
            CommonImport cImport = new CommonImport();


            #endregion Initializ

            #region Try
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();

                transaction = currConn.BeginTransaction(MessageVM.PurchasemsgMethodNameInsert);

                #endregion open connection and transaction


                #region Data
                for (int i = 0; i < DtDuty.Rows.Count; i++)
                {
                    #region Fiscal Year Check

                    string transactionDate = DtDuty.Rows[i]["InvoiceDateTime"].ToString().Trim();
                    string transactionYearCheck = Convert.ToDateTime(transactionDate).ToString("yyyy-MM-dd");
                    if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                    {

                        #region YearLock
                        sqlText = "";

                        sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                       " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                        DataTable dataTable = new DataTable("ProductDataT");
                        SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                        cmdIdExist.Transaction = transaction;
                        SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                        reportDataAdapt.Fill(dataTable);

                        if (dataTable == null)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }

                        else if (dataTable.Rows.Count <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else
                        {
                            if (dataTable.Rows[0]["MLock"].ToString() != "N")
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                            }
                            else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                            }
                        }
                        #endregion YearLock
                        #region YearNotExist
                        sqlText = "";
                        sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                        DataTable dtYearNotExist = new DataTable("ProductDataT");

                        SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                        cmdYearNotExist.Transaction = transaction;
                        //countId = (int)cmdIdExist.ExecuteScalar();

                        SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                        YearNotExistDataAdapt.Fill(dtYearNotExist);

                        if (dtYearNotExist == null)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }

                        else if (dtYearNotExist.Rows.Count < 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                        else
                        {
                            if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                                || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                            }
                        }
                        #endregion YearNotExist

                    }


                    #endregion Fiscal Year CHECK

                    #region FindItemId
                    if (string.IsNullOrEmpty(DtDuty.Rows[i]["Item_Code"].ToString().Trim()))
                    {
                        throw new ArgumentNullException("Please insert value in 'Item_Code' field.");
                    }
                    ItemNo = cImport.FindItemId(DtDuty.Rows[i]["Item_Name"].ToString().Trim()
                                               , DtDuty.Rows[i]["Item_Code"].ToString().Trim(), null, null);
                    if (string.IsNullOrEmpty(ItemNo))
                    {
                        throw new ArgumentNullException("Item Not Found In Database. Item Name: " + DtDuty.Rows[i]["Item_Name"].ToString().Trim()
                            + "(" + DtDuty.Rows[i]["Item_Code"].ToString().Trim() + ")");

                    }
                    else
                    {

                        string accvalue = FindItemIdFromPurchase(DtDuty.Rows[i]["ID"].ToString().Trim()
                                                 , ItemNo, null, null);
                        if (Convert.ToDecimal(accvalue) <= 0)
                        {
                            throw new ArgumentNullException("This Item Not Found In This Purchase. Item Name: " + DtDuty.Rows[i]["Item_Name"].ToString().Trim()
                            + "(" + DtDuty.Rows[i]["Item_Code"].ToString().Trim() + ")");
                        }
                        else
                        {
                            importID = DtDuty.Rows[i]["ID"].ToString().Trim();
                            DataRow[] ImportRaws;//= new DataRow[];//
                            if (!string.IsNullOrEmpty(importID))
                            {
                                DtPurchaseD = DtDuty;
                                ImportRaws = DtDuty.Select("ID='" + importID + "'");
                            }
                            else
                            {
                                ImportRaws = null;
                            }

                            if (ImportRaws != null && ImportRaws.Length > 0)
                            {

                                PurchaseDutiesVM duty = new PurchaseDutiesVM();
                                duty = DutyCalculation(ImportRaws, null, null).FirstOrDefault();
                                #region Update only DetailTable

                                sqlText = "";

                                sqlText += " update PurchaseInvoiceDuties set  ";
                                sqlText += " CnFInp         =@dutyCnFInp, ";
                                sqlText += " CnFRate        =@dutyCnFRate, ";
                                sqlText += " CnFAmount      =@dutyCnFAmount, ";
                                sqlText += " InsuranceInp   =@dutyInsuranceInp, ";
                                sqlText += " InsuranceRate  =@dutyInsuranceRate, ";
                                sqlText += " InsuranceAmount=@dutyInsuranceAmount, ";
                                sqlText += " AssessableInp  =@dutyAssessableInp, ";
                                sqlText += " AssessableValue=@dutyAssessableValue, ";
                                sqlText += " CDInp          =@dutyCDInp, ";
                                sqlText += " CDRate         =@dutyCDRate, ";
                                sqlText += " CDAmount       =@dutyCDAmount, ";
                                sqlText += " RDInp          =@dutyRDInp, ";
                                sqlText += " RDRate         =@dutyRDRate, ";
                                sqlText += " RDAmount       =@dutyRDAmount, ";
                                sqlText += " TVBInp         =@dutyTVBInp, ";
                                sqlText += " TVBRate        =@dutyTVBRate, ";
                                sqlText += " TVBAmount      =@dutyTVBAmount, ";
                                sqlText += " SDInp          =@dutySDInp, ";
                                sqlText += " SD             =@dutySD, ";
                                sqlText += " SDAmount       =@dutySDAmount, ";
                                sqlText += " VATInp         =@dutyVATInp, ";
                                sqlText += " VATRate        =@dutyVATRate, ";
                                sqlText += " VATAmount      =@dutyVATAmount, ";
                                sqlText += " TVAInp         =@dutyTVAInp, ";
                                sqlText += " TVARate        =@dutyTVARate, ";
                                sqlText += " TVAAmount      =@dutyTVAAmount, ";
                                sqlText += " ATVInp         =@dutyATVInp, ";
                                sqlText += " ATVRate        =@dutyATVRate, ";
                                sqlText += " ATVAmount      =@dutyATVAmount, ";
                                sqlText += " OthersInp      =@dutyOthersInp, ";
                                sqlText += " OthersRate     =@dutyOthersRate, ";
                                sqlText += " OthersAmount   =@dutyOthersAmount, ";
                                sqlText += " Remarks        =@dutyRemarks";

                                sqlText += " where  PurchaseInvoiceNo=@importID ";
                                sqlText += " and   ItemNo=@ItemNo";



                                SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                                cmdInsDetail.Transaction = transaction;

                                cmdInsDetail.Parameters.AddWithValue("@dutyCnFInp", duty.CnFInp);
                                cmdInsDetail.Parameters.AddWithValue("@dutyCnFRate", duty.CnFRate);
                                cmdInsDetail.Parameters.AddWithValue("@dutyCnFAmount", duty.CnFAmount);
                                cmdInsDetail.Parameters.AddWithValue("@dutyInsuranceInp", duty.InsuranceInp);
                                cmdInsDetail.Parameters.AddWithValue("@dutyInsuranceRate", duty.InsuranceRate);
                                cmdInsDetail.Parameters.AddWithValue("@dutyInsuranceAmount", duty.InsuranceAmount);
                                cmdInsDetail.Parameters.AddWithValue("@dutyAssessableInp", duty.AssessableInp);
                                cmdInsDetail.Parameters.AddWithValue("@dutyAssessableValue", duty.AssessableValue);
                                cmdInsDetail.Parameters.AddWithValue("@dutyCDInp", duty.CDInp);
                                cmdInsDetail.Parameters.AddWithValue("@dutyCDRate", duty.CDRate);
                                cmdInsDetail.Parameters.AddWithValue("@dutyCDAmount", duty.CDAmount);
                                cmdInsDetail.Parameters.AddWithValue("@dutyRDInp", duty.RDInp);
                                cmdInsDetail.Parameters.AddWithValue("@dutyRDRate", duty.RDRate);
                                cmdInsDetail.Parameters.AddWithValue("@dutyRDAmount", duty.RDAmount);
                                cmdInsDetail.Parameters.AddWithValue("@dutyTVBInp", duty.TVBInp);
                                cmdInsDetail.Parameters.AddWithValue("@dutyTVBRate", duty.TVBRate);
                                cmdInsDetail.Parameters.AddWithValue("@dutyTVBAmount", duty.TVBAmount);
                                cmdInsDetail.Parameters.AddWithValue("@dutySDInp", duty.SDInp);
                                cmdInsDetail.Parameters.AddWithValue("@dutySD", duty.SD);
                                cmdInsDetail.Parameters.AddWithValue("@dutySDAmount", duty.SDAmount);
                                cmdInsDetail.Parameters.AddWithValue("@dutyVATInp", duty.VATInp);
                                cmdInsDetail.Parameters.AddWithValue("@dutyVATRate", duty.VATRate);
                                cmdInsDetail.Parameters.AddWithValue("@dutyVATAmount", duty.VATAmount);
                                cmdInsDetail.Parameters.AddWithValue("@dutyTVAInp", duty.TVAInp);
                                cmdInsDetail.Parameters.AddWithValue("@dutyTVARate", duty.TVARate);
                                cmdInsDetail.Parameters.AddWithValue("@dutyTVAAmount", duty.TVAAmount);
                                cmdInsDetail.Parameters.AddWithValue("@dutyATVInp", duty.ATVInp);
                                cmdInsDetail.Parameters.AddWithValue("@dutyATVRate", duty.ATVRate);
                                cmdInsDetail.Parameters.AddWithValue("@dutyATVAmount", duty.ATVAmount);
                                cmdInsDetail.Parameters.AddWithValue("@dutyOthersInp", duty.OthersInp);
                                cmdInsDetail.Parameters.AddWithValue("@dutyOthersRate", duty.OthersRate);
                                cmdInsDetail.Parameters.AddWithValue("@dutyOthersAmount", duty.OthersAmount);
                                cmdInsDetail.Parameters.AddWithValue("@dutyRemarks", duty.Remarks);
                                cmdInsDetail.Parameters.AddWithValue("@importID", importID);
                                cmdInsDetail.Parameters.AddWithValue("@ItemNo", ItemNo);

                                transResult = (int)cmdInsDetail.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                    MessageVM.PurchasemsgUpdateNotSuccessfully);
                                }

                                #endregion Update only DetailTable

                            }
                        }

                    }
                    #endregion FindItemId
                }

                #endregion Data

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Commit
                #region SuccessResult


                retResults[0] = "Success";
                retResults[1] = MessageVM.PurchasemsgUpdateSuccessfully;
                //retResults[2] = Master.Id;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        public decimal FormatingNumeric(decimal value, int DecPlace)
        {
            object outPutValue = 0;
            string decPointLen = "";
            try
            {

                for (int i = 0; i < DecPlace; i++)
                {
                    decPointLen = decPointLen + "0";
                }
                if (value < 1000)
                {
                    var a = "0." + decPointLen + "";
                    outPutValue = value.ToString(a);
                }
                else
                {
                    var a = "0,0." + decPointLen + "";
                    outPutValue = value.ToString(a);

                }


            }
            #region Catch

            catch (Exception ex)
            {

            }
            #endregion Catch

            return Convert.ToDecimal(outPutValue);
        }


        //==================SelectAll=================
        public List<PurchaseMasterVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, PurchaseMasterVM likeVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<PurchaseMasterVM> VMs = new List<PurchaseMasterVM>();
            PurchaseMasterVM vm;
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
 pih.Id
,pih.PurchaseInvoiceNo
,pih.VendorID
,pih.InvoiceDateTime
,ISNULL(pih.TotalAmount,0) TotalAmount
,ISNULL(pih.TotalVATAmount,0) TotalVATAmount
,pih.SerialNo
,pih.Comments
,pih.CreatedBy
,pih.CreatedOn
,pih.LastModifiedBy
,pih.LastModifiedOn
,pih.BENumber
,pih.ProductType
,pih.TransactionType
,pih.ReceiveDate
,pih.Post
,pih.CurrencyID
,ISNULL(pih.CurrencyRateFromBDT,0) CurrencyRateFromBDT
,pih.WithVDS
,pih.PurchaseReturnId
,pih.ImportIDExcel
,pih.SerialNo1
,pih.LCNumber
,pih.LCDate
,ISNULL(pih.LandedCost,0) LandedCost
,pih.CustomHouse
,v.VendorName VendorName
,v.VendorGroupID VendorGroup

FROM PurchaseInvoiceHeaders pih left outer join Vendors v
on pih.VendorID=v.VendorID
WHERE  1=1
";


                if (Id > 0)
                {
                    sqlText += @" and pih.Id=@Id";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]) || conditionValues[i] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        if (conditionFields[i].ToLower().Contains("like"))
                        {
                            sqlText += " AND " + conditionFields[i] + " '%'+ " + " @" + cField.Replace("like", "").Trim() + " +'%'";
                        }
                        else if (conditionFields[i].Contains(">") || conditionFields[i].Contains("<"))
                        {
                            sqlText += " AND " + conditionFields[i] + " @" + cField;

                        }
                        else
                        {
                            sqlText += " AND " + conditionFields[i] + "= @" + cField;
                        }
                    }
                }
                if (likeVM != null)
                {
                    if (!string.IsNullOrWhiteSpace(likeVM.PurchaseInvoiceNo))
                    {
                        sqlText += " AND pih.PurchaseInvoiceNo like @PurchaseInvoiceNo ";
                    }
                    if (!string.IsNullOrWhiteSpace(likeVM.BENumber))
                    {
                        sqlText += " AND pih.BENumber like @BENumber ";
                    }
                    if (!string.IsNullOrWhiteSpace(likeVM.SerialNo))
                    {
                        sqlText += " AND pih.SerialNo like @SerialNo ";
                    }
                }
                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]) || conditionValues[j] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        if (conditionFields[j].ToLower().Contains("like"))
                        {
                            objComm.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(), conditionValues[j]);
                        }
                        else
                        {
                            objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                        }
                    }
                }
                if (likeVM != null)
                {
                    if (!string.IsNullOrWhiteSpace(likeVM.PurchaseInvoiceNo))
                    {
                        objComm.Parameters.AddWithValue("@PurchaseInvoiceNo", "%" + likeVM.PurchaseInvoiceNo + "%");
                    }
                    if (!string.IsNullOrWhiteSpace(likeVM.BENumber))
                    {
                        objComm.Parameters.AddWithValue("@BENumber", "%" + likeVM.BENumber + "%");
                    }
                    if (!string.IsNullOrWhiteSpace(likeVM.SerialNo))
                    {
                        objComm.Parameters.AddWithValue("@SerialNo", "%" + likeVM.SerialNo + "%");
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
                    vm = new PurchaseMasterVM();
                    vm.Id = dr["Id"].ToString();
                    vm.PurchaseInvoiceNo = dr["PurchaseInvoiceNo"].ToString();
                    vm.VendorID = dr["VendorID"].ToString();
                    vm.InvoiceDate = Ordinary.DateTimeToDate(dr["InvoiceDateTime"].ToString());
                    vm.TotalAmount = Convert.ToDecimal(dr["TotalAmount"].ToString());
                    vm.TotalVATAmount = Convert.ToDecimal(dr["TotalVATAmount"].ToString());
                    vm.SerialNo = dr["SerialNo"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.BENumber = dr["BENumber"].ToString();
                    vm.ProductType = dr["ProductType"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.ReceiveDate = Ordinary.DateTimeToDate(dr["ReceiveDate"].ToString());
                    vm.Post = dr["Post"].ToString();
                    vm.WithVDS = dr["WithVDS"].ToString();
                    vm.LCNumber = dr["LCNumber"].ToString();
                    vm.LCDate = Ordinary.DateTimeToDate(dr["LCDate"].ToString());
                    vm.LandedCost = Convert.ToDecimal(dr["LandedCost"].ToString());
                    vm.CustomHouse = dr["CustomHouse"].ToString();
                    vm.VendorName = dr["VendorName"].ToString();
                    vm.ReturnId = dr["PurchaseReturnId"].ToString();
                    vm.VendorGroup = dr["VendorGroup"].ToString();

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
        public List<PurchaseDetailVM> SelectPurchaseDetail(string PurchaseInvoiceNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<PurchaseDetailVM> VMs = new List<PurchaseDetailVM>();
            PurchaseDetailVM vm;
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
 pd.PurchaseInvoiceNo
,pd.POLineNo
,pd.ItemNo
,ISNULL(pd.Quantity,0) Quantity
,ISNULL(pd.CostPrice,0) CostPrice
,ISNULL(pd.NBRPrice,0) NBRPrice
,pd.UOM
,pd.Comments
,pd.CreatedBy
,pd.CreatedOn
,pd.LastModifiedBy
,pd.LastModifiedOn
,pd.Type
,pd.ProductType
,pd.BENumber
,pd.InvoiceDateTime
,pd.ReceiveDate
,pd.Post
,ISNULL(pd.UOMQty,0) UOMQty
,ISNULL(pd.UOMPrice,0) UOMPrice
,ISNULL(pd.UOMc,0) UOMc
,pd.UOMn
,ISNULL(pd.DollerValue,0) DollerValue
,ISNULL(pd.CurrencyValue,0) CurrencyValue
,ISNULL(pd.RebateRate,0) ,RebateRate
,ISNULL(pd.RebateAmount,0) RebateAmount
,ISNULL(pd.SubTotal,0) SubTotal
,ISNULL(pd.CnFAmount,0) CnFAmount
,ISNULL(pd.InsuranceAmount,0) InsuranceAmount
,ISNULL(pd.AssessableValue,0) AssessableValue
,ISNULL(pd.CDAmount,0) CDAmount
,ISNULL(pd.RDAmount,0) RDAmount
,ISNULL(pd.SD,0) SD
,ISNULL(pd.SDAmount,0) SDAmount
,ISNULL(pd.TVBAmount,0) TVBAmount
,ISNULL(pd.VATRate,0) VATRate
,ISNULL(pd.VATAmount,0) VATAmount
,ISNULL(pd.TVAAmount,0) TVAAmount
,ISNULL(pd.ATVAmount,0) ATVAmount
,ISNULL(pd.OthersAmount,0) OthersAmount
,pd.TransactionType
,pd.PurchaseReturnId
,pd.ReturnTransactionType
,p.ProductName
,p.ProductCode
FROM PurchaseInvoiceDetails pd left outer join Products p on pd.ItemNo=p.ItemNo
WHERE  1=1

";

                if (PurchaseInvoiceNo != null)
                {
                    sqlText += "AND pd.PurchaseInvoiceNo=@PurchaseInvoiceNo";
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

                if (PurchaseInvoiceNo != null)
                {
                    objComm.Parameters.AddWithValue("@PurchaseInvoiceNo", PurchaseInvoiceNo);
                }
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
                ProductDAL ProductdDal;
                while (dr.Read())
                {
                    vm = new PurchaseDetailVM();
                    vm.PurchaseInvoiceNo = dr["PurchaseInvoiceNo"].ToString();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.Quantity = Convert.ToDecimal(dr["Quantity"].ToString());
                    vm.UnitPrice = Convert.ToDecimal(dr["CostPrice"].ToString());
                    vm.NBRPrice = Convert.ToDecimal(dr["NBRPrice"].ToString());
                    vm.UOM = dr["UOM"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.Type = dr["Type"].ToString();
                    vm.ProductType = dr["ProductType"].ToString();
                    vm.BENumber = dr["BENumber"].ToString();
                    vm.Post = dr["Post"].ToString() == "Y" ? true : false;
                    vm.UOMQty = Convert.ToDecimal(dr["UOMQty"].ToString());
                    vm.UOMPrice = Convert.ToDecimal(dr["UOMPrice"].ToString());
                    vm.UOMc = Convert.ToDecimal(dr["UOMc"].ToString());
                    vm.UOMn = dr["UOMn"].ToString();
                    vm.RebateRate = Convert.ToDecimal(dr["RebateRate"].ToString());
                    vm.RebateAmount = Convert.ToDecimal(dr["RebateAmount"].ToString());
                    vm.SubTotal = Convert.ToDecimal(dr["SubTotal"].ToString());
                    vm.CnFAmount = Convert.ToDecimal(dr["CnFAmount"].ToString());
                    vm.InsuranceAmount = Convert.ToDecimal(dr["InsuranceAmount"].ToString());
                    vm.AssessableValue = Convert.ToDecimal(dr["AssessableValue"].ToString());
                    vm.CDAmount = Convert.ToDecimal(dr["CDAmount"].ToString());
                    vm.RDAmount = Convert.ToDecimal(dr["RDAmount"].ToString());
                    vm.SD = Convert.ToDecimal(dr["SD"].ToString());
                    vm.SDAmount = Convert.ToDecimal(dr["SDAmount"].ToString());
                    vm.TVBAmount = Convert.ToDecimal(dr["TVBAmount"].ToString());
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                    vm.VATAmount = Convert.ToDecimal(dr["VATAmount"].ToString());
                    vm.TVAAmount = Convert.ToDecimal(dr["TVAAmount"].ToString());
                    vm.ATVAmount = Convert.ToDecimal(dr["ATVAmount"].ToString());
                    vm.OthersAmount = Convert.ToDecimal(dr["OthersAmount"].ToString());
                    vm.ReturnTransactionType = dr["ReturnTransactionType"].ToString();

                    vm.ReceiveDate = dr["ReceiveDate"].ToString();
                    vm.InvoiceDateTime = dr["InvoiceDateTime"].ToString();
                    vm.ProductName = dr["ProductName"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    vm.Total = vm.SubTotal + vm.VATAmount + vm.SDAmount;

                    //string[] conditionalFields = new string[] { "ItemNo" };
                    //string[] conditionalValues = new string[] { vm.ItemNo };
                    //var product = new ProductDAL().SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();
                    //vm.ProductName = product.ProductName;
                    //vm.ProductCode = product.ProductCode;

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

        public List<PurchaseMasterVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<PurchaseMasterVM> VMs = new List<PurchaseMasterVM>();
            PurchaseMasterVM vm;
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
af.Id
,af.PurchaseInvoiceNo
FROM PurchaseInvoiceHeaders af
WHERE  1=1
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new PurchaseMasterVM();
                    vm.Id = dr["Id"].ToString();
                    vm.PurchaseInvoiceNo = dr["PurchaseInvoiceNo"].ToString();
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

        public string[] ImportExcelFile(PurchaseMasterVM paramVM)
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
            #endregion

            #region try
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                #region Excel Reader

                string FileName = paramVM.File.FileName;
                string Fullpath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + FileName;
                System.IO.File.Delete(Fullpath);
                if (paramVM.File != null && paramVM.File.ContentLength > 0)
                {
                    paramVM.File.SaveAs(Fullpath);
                }


                FileStream stream = File.Open(Fullpath, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;
                if (FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();


                dt = ds.Tables[0];
                reader.Close();
                System.IO.File.Delete(Fullpath);
                #endregion

                DataTable dtPurchaseM = new DataTable();
                dtPurchaseM = ds.Tables["PurchaseM"];

                DataTable dtPurchaseD = new DataTable();
                dtPurchaseD = ds.Tables["PurchaseD"];


                DataTable dtPurchaseI = new DataTable();
                dtPurchaseI = ds.Tables["PurchaseI"];


                DataTable dtPurchaseT = new DataTable();



                dtPurchaseM.Columns.Add("Transection_Type");
                dtPurchaseM.Columns.Add("Created_By");
                dtPurchaseM.Columns.Add("LastModified_By");
                foreach (DataRow row in dtPurchaseM.Rows)
                {
                    row["Transection_Type"] = paramVM.TransactionType;
                    row["Created_By"] = paramVM.CreatedBy;
                    row["LastModified_By"] = paramVM.CreatedBy;

                }


                //dt = ds.Tables[0].Select("empCode <>''").CopyToDataTable();

                #region Data Insert
                PurchaseDAL puchaseDal = new PurchaseDAL();
                retResults = puchaseDal.ImportData(dtPurchaseM, dtPurchaseD, dtPurchaseI, dtPurchaseT);
                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException("", retResults[1]);
                }
                #endregion

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
                return retResults;
            }
            finally
            {
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }


        public List<PurchaseDetailVM> RateCheck(List<PurchaseDetailVM> VMs)
        {
            try
            {
                foreach (PurchaseDetailVM vm in VMs)
                {
                    string msg = "";
                    string rateChange = "";

                    string[] retResults = new string[4];
                    retResults[0] = "Info";
                    msg = "Item (" + vm.ProductName + " - " + vm.ProductCode+")";

                    rateChange = RateChangePercent(vm.ItemNo, vm.UnitPrice);
                    if (string.IsNullOrWhiteSpace(rateChange))
                    {
                        msg = msg + "\n is not included in any price declaration \nUnit Price";

                    }
                    else
                    {
                        msg = msg + rateChange + "\n Unit Price";
                    }
                    retResults[1] = msg;
                    vm.retResults = retResults;

                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }
            return VMs;
        }

        public string RateChangePercent(string ItemNo, decimal unitPrice)
        {
            string result = string.Empty;
            try
            {
                string vRateChangePromotePercent = _cDal.settings("Purchase", "RateChangePromote");
                decimal RateChangePromote = Convert.ToDecimal(vRateChangePromotePercent);


                decimal plusRateChangePromotPercent = 0;
                decimal minusRateChangePromotPercent = 0;
                plusRateChangePromotPercent = Convert.ToDecimal("+" + RateChangePromote);
                minusRateChangePromotPercent = Convert.ToDecimal("-" + RateChangePromote);
                ProductDAL productDal = new ProductDAL();
                decimal bomVatablePrice = productDal.GetLastVatableFromBOM(ItemNo, null, null);
                if (bomVatablePrice <= 0)
                {
                    result = "This item is not included in any price declaration";
                    return result;
                }
                decimal changes = (unitPrice - bomVatablePrice) * 100 / bomVatablePrice;
                if (changes > plusRateChangePromotPercent || changes < minusRateChangePromotPercent)
                {
                    result = "In Purchase price : " + unitPrice.ToString("0,0.0000") + "\nIn Last Price Declaration : " + bomVatablePrice.ToString("0,0.0000") + "" +
                                    "\nChanges : " + changes.ToString("0,0.0000") + "%";


                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }
            return result;

        }

    }
}