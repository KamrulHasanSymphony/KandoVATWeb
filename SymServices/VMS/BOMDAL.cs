using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SymOrdinary;
using SymViewModel.VMS;
using System.Reflection;


namespace SymServices.VMS
{
    public class BOMDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        private const string FieldDelimeter = DBConstant.FieldDelimeter;


        #endregion

        public DataTable SearchBOMMaster(string finItem, string vatName, string effectDate, string CustomerID = "0")
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("BOM");

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
                //commonDal.TableFieldAdd("BOMRaws", "PBOMId", "varchar(20)", currConn);

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT   
B.FinishItemNo, 
P.ProductName, 
isnull(NULLIF(b.Comments, ''),'-')Comments,
b.VATName,
b.WholeSalePrice,
b.RawOHCost,
b.VATRate,b.SD,b.PacketPrice,b.NBRPrice,b.TradingMarkUp,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo
,isnull(b.CustomerID,0)CustomerID

FROM dbo.BOMs AS B inner JOIN
dbo.Products AS P ON B.FinishItemNo = P.ItemNo
left outer join Customers cus on isnull(b.CustomerID,0)=cus.CustomerID

 WHERE 
b.FinishItemNo=@finItem and b.EffectDate=@effectDate and b.VATName=@vatName

                                ";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(b.CustomerId,0)='" + CustomerID + "' ";
                }
                SqlCommand objCommCBOM = new SqlCommand();
                objCommCBOM.Connection = currConn;
                objCommCBOM.CommandText = sqlText;
                objCommCBOM.CommandType = CommandType.Text;

                #endregion

                #region param

                if (finItem == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@finItem"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@finItem", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@finItem"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@finItem"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@finItem", finItem);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@finItem"].Value = finItem;
                    }
                }

                if (vatName == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@vatName"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@vatName", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@vatName"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@vatName"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@vatName", vatName);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@vatName"].Value = vatName;
                    }
                }

                if (effectDate == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@effectDate"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@effectDate", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@effectDate"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@effectDate"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@effectDate", effectDate);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@effectDate"].Value = effectDate;
                    }
                }


                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCBOM);
                dataAdapter.Fill(dataTable);


            }
            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public DataTable SearchBOMRaw(string finItem, string vatName, string effectDate, string CustomerID = "0")
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("BOM");

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
--declare @finItem varchar(20);
--declare @vatName varchar(50);
--declare @effectDate datetime;
-- SET @finItem=62;
-- SET @vatName='vat 1'
-- SET @effectDate='2013-07-01'

SELECT   


stat,rawitemtype,BOMLineNo, 
RawItemNo, 
ProductName, 
UOM,
UseQuantity, 
WastageQuantity, 
Cost, 
RebateRate, 
ActiveStatus,UnitCost,
ProductCode ,HSCodeNo,
UOMPrice, 
UOMc, 
UOMn, 
UOMUQty, 
UOMWQty,isnull(PBOMId,0)PBOMId,PInvoiceNo
,isnull(TransactionType,'0')TransactionType
,isnull(CustomerID,0)CustomerID

from(

SELECT   
'A' as stat,rawitemtype,R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID

FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
WHERE 
r.FinishItemNo=@finItem and r.EffectDate=@effectDate and r.VATName=@vatName
and RawItemType='Raw' ";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(r.CustomerId,0)='" + CustomerID + "' ";
                }
                sqlText += @" 
union 

SELECT  
'B' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
WHERE 
r.FinishItemNo=@finItem and r.EffectDate=@effectDate and r.VATName=@vatName
 and RawItemType='Pack'
 ";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(r.CustomerId,0)='" + CustomerID + "' ";
                }
                sqlText += @" 
union 

SELECT  
'C' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
WHERE 
r.FinishItemNo=@finItem and r.EffectDate=@effectDate and r.VATName=@vatName
 and RawItemType='Finish'
 ";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(r.CustomerId,0)='" + CustomerID + "' ";
                }
                sqlText += @" 
union 

SELECT  
'D' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
WHERE 
r.FinishItemNo=@finItem and r.EffectDate=@effectDate and r.VATName=@vatName

 and RawItemType='Trading'
 ";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(r.CustomerId,0)='" + CustomerID + "' ";
                }
                sqlText += @" 


union
SELECT  
'E' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
WHERE 
r.FinishItemNo=@finItem and r.EffectDate=@effectDate and r.VATName=@vatName
and RawItemType='Overhead'
 ";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(r.CustomerId,0)='" + CustomerID + "' ";
                }
                sqlText += @" 
union
SELECT  
'F' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
WHERE 
r.FinishItemNo=@finItem and r.EffectDate=@effectDate and r.VATName=@vatName
and RawItemType='WIP'
 ";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(r.CustomerId,0)='" + CustomerID + "' ";
                }
                sqlText += @" 
union
SELECT  
'G' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
WHERE 
r.FinishItemNo=@finItem and r.EffectDate=@effectDate and r.VATName=@vatName
and RawItemType='Service'
 ";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(r.CustomerId,0)='" + CustomerID + "' ";
                }
                sqlText += @" 
) as v1 
order by stat,BOMLineNo
";


                SqlCommand objCommCBOM = new SqlCommand();
                objCommCBOM.Connection = currConn;
                objCommCBOM.CommandText = sqlText;
                objCommCBOM.CommandType = CommandType.Text;

                #endregion

                #region param

                if (finItem == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@finItem"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@finItem", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@finItem"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@finItem"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@finItem", finItem);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@finItem"].Value = finItem;
                    }
                }

                if (vatName == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@vatName"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@vatName", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@vatName"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@vatName"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@vatName", vatName);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@vatName"].Value = vatName;
                    }
                }

                if (effectDate == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@effectDate"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@effectDate", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@effectDate"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@effectDate"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@effectDate", effectDate);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@effectDate"].Value = effectDate;
                    }
                }


                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCBOM);
                dataAdapter.Fill(dataTable);
            }
            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public DataTable SearchOH(string finItem, string vatName, string effectDate, string CustomerID = "0")
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("OH");

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
                b.HeadName HeadNameOld,
                b.HeadID,
                p.ProductCode OHCode,p.ProductName HeadName,
                b.HeadAmount,
                b.OHLineNo,
                b.FinishItemNo,
                isnull(b.AdditionalCost,0)AdditionalCost,
                convert(varchar,b.EffectDate,120)EffectDate,
                isnull(b.RebatePercent,0)RebatePercent
                ,isnull(b.CustomerID,0)CustomerID
                FROM BOMCompanyOverhead b  LEFT OUTER JOIN
                products p ON b.HeadID=p.ItemNo               
                WHERE FinishItemNo=@finItem and EffectDate=@effectDate and VATName=@vatName ";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(b.CustomerId,0)='" + CustomerID + "' ";
                }
                sqlText += @" 

                order by OHLineNo";

                SqlCommand objCommCBOHead = new SqlCommand();
                objCommCBOHead.Connection = currConn;
                objCommCBOHead.CommandText = sqlText;
                objCommCBOHead.CommandType = CommandType.Text;

                #endregion

                #region param

                if (finItem == "")
                {
                    if (!objCommCBOHead.Parameters.Contains("@finItem"))
                    {
                        objCommCBOHead.Parameters.AddWithValueAndNullHandle("@finItem", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOHead.Parameters["@finItem"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOHead.Parameters.Contains("@finItem"))
                    {
                        objCommCBOHead.Parameters.AddWithValueAndNullHandle("@finItem", finItem);
                    }
                    else
                    {
                        objCommCBOHead.Parameters["@finItem"].Value = finItem;
                    }
                }

                if (vatName == "")
                {
                    if (!objCommCBOHead.Parameters.Contains("@vatName"))
                    {
                        objCommCBOHead.Parameters.AddWithValueAndNullHandle("@vatName", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOHead.Parameters["@vatName"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOHead.Parameters.Contains("@vatName"))
                    {
                        objCommCBOHead.Parameters.AddWithValueAndNullHandle("@vatName", vatName);
                    }
                    else
                    {
                        objCommCBOHead.Parameters["@vatName"].Value = vatName;
                    }
                }

                if (effectDate == "")
                {
                    if (!objCommCBOHead.Parameters.Contains("@effectDate"))
                    {
                        objCommCBOHead.Parameters.AddWithValueAndNullHandle("@effectDate", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOHead.Parameters["@effectDate"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOHead.Parameters.Contains("@effectDate"))
                    {
                        objCommCBOHead.Parameters.AddWithValueAndNullHandle("@effectDate", effectDate);
                    }
                    else
                    {
                        objCommCBOHead.Parameters["@effectDate"].Value = effectDate;
                    }
                }


                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCBOHead);
                dataAdapter.Fill(dataTable);


            }
            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        #region New
        public DataTable SearchBOMMasterNew(string BOMId)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("BOM");

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
                //commonDal.TableFieldAdd("BOMRaws", "PBOMId", "varchar(20)", currConn);

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT   
B.FinishItemNo, 
P.ProductName, 
isnull(NULLIF(b.Comments, ''),'-')Comments,
b.VATName,
b.WholeSalePrice,
b.RawOHCost,
b.VATRate,b.SD,b.PacketPrice,b.NBRPrice,b.TradingMarkUp,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo
,isnull(b.CustomerID,0)CustomerID

FROM dbo.BOMs AS B inner JOIN
dbo.Products AS P ON B.FinishItemNo = P.ItemNo
left outer join Customers cus on isnull(b.CustomerID,0)=cus.CustomerID

 WHERE 1=1 
and b.BOMId=@BOMId 

                                ";

                SqlCommand objCommCBOM = new SqlCommand();
                objCommCBOM.Connection = currConn;
                objCommCBOM.CommandText = sqlText;
                objCommCBOM.CommandType = CommandType.Text;

                #endregion

                #region param

                if (BOMId == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@BOMId"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@BOMId", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@BOMId"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@BOMId"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@BOMId", BOMId);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@BOMId"].Value = BOMId;
                    }
                }



                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCBOM);
                dataAdapter.Fill(dataTable);


            }
            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public DataTable SearchBOMRawNew(string BOMId)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("BOM");

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
--declare @finItem varchar(20);
--declare @vatName varchar(50);
--declare @effectDate datetime;
-- SET @finItem=62;
-- SET @vatName='vat 1'
-- SET @effectDate='2013-07-01'

SELECT   


stat,rawitemtype,BOMLineNo, 
RawItemNo, 
ProductName, 
UOM,
UseQuantity, 
WastageQuantity, 
Cost, 
RebateRate, 
ActiveStatus,UnitCost,
ProductCode ,HSCodeNo,
UOMPrice, 
UOMc, 
UOMn, 
UOMUQty, 
UOMWQty,isnull(PBOMId,0)PBOMId,PInvoiceNo
,isnull(TransactionType,'0')TransactionType
,isnull(CustomerID,0)CustomerID
,isnull(IssueOnProduction,'Y')IssueOnProduction

from(

SELECT   
'A' as stat,rawitemtype,R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
,isnull(r.IssueOnProduction,'Y')IssueOnProduction

FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
 WHERE 1=1
 and r.BOMId=@BOMId 
 
and RawItemType='Raw' ";

                sqlText += @" 
union 

SELECT  
'B' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
,isnull(r.IssueOnProduction,'Y')IssueOnProduction

FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
 WHERE 1=1
 and r.BOMId=@BOMId 
 
 
 and RawItemType='Pack'
 ";

                sqlText += @" 
union 

SELECT  
'C' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
,isnull(r.IssueOnProduction,'Y')IssueOnProduction
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo

 WHERE 1=1
 and r.BOMId=@BOMId 
 
 
 and RawItemType='Finish'
 ";

                sqlText += @" 
union 

SELECT  
'D' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
,isnull(r.IssueOnProduction,'Y')IssueOnProduction
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
 WHERE 1=1
 and r.BOMId=@BOMId 
  

 and RawItemType='Trading'
 ";

                sqlText += @" 


union
SELECT  
'E' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
,isnull(r.IssueOnProduction,'Y')IssueOnProduction
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
 WHERE 1=1
 and r.BOMId=@BOMId 
  
and RawItemType='Overhead'
 ";

                sqlText += @" 
union
SELECT  
'F' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
,isnull(r.IssueOnProduction,'Y')IssueOnProduction
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
 WHERE 1=1
 and r.BOMId=@BOMId 
  
and RawItemType='WIP'
 ";

                sqlText += @" 
union
SELECT  
'G' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
,isnull(r.IssueOnProduction,'Y')IssueOnProduction
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
 WHERE 1=1
 and r.BOMId=@BOMId 
  
and RawItemType='Service'
 ";
                sqlText += @" 
) as v1 
order by stat,BOMLineNo
";


                SqlCommand objCommCBOM = new SqlCommand();
                objCommCBOM.Connection = currConn;
                objCommCBOM.CommandText = sqlText;
                objCommCBOM.CommandType = CommandType.Text;

                #endregion

                #region param

                if (BOMId == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@BOMId"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@BOMId", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@BOMId"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@BOMId"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@BOMId", BOMId);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@BOMId"].Value = BOMId;
                    }
                }



                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCBOM);
                dataAdapter.Fill(dataTable);
            }
            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public DataTable SearchOHNew(string BOMId)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("OH");

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
                b.HeadName HeadNameOld,
                b.HeadID,
                p.ProductCode OHCode,p.ProductName HeadName,
                b.HeadAmount,
                b.OHLineNo,
                b.FinishItemNo,
                isnull(b.AdditionalCost,0)AdditionalCost,
                convert(varchar,b.EffectDate,120)EffectDate,
                isnull(b.RebatePercent,0)RebatePercent
                ,isnull(b.CustomerID,0)CustomerID
                FROM BOMCompanyOverhead b  LEFT OUTER JOIN
                products p ON b.HeadID=p.ItemNo       

 WHERE 1=1
 and b.BOMId=@BOMId 
        
                ";

                sqlText += @"       order by OHLineNo";

                SqlCommand objCommCBOM = new SqlCommand();
                objCommCBOM.Connection = currConn;
                objCommCBOM.CommandText = sqlText;
                objCommCBOM.CommandType = CommandType.Text;

                #endregion

                #region param

                if (BOMId == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@BOMId"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@BOMId", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@BOMId"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@BOMId"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@BOMId", BOMId);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@BOMId"].Value = BOMId;
                    }
                }



                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCBOM);
                dataAdapter.Fill(dataTable);


            }
            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public string[] BOMImport(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster)
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

            string fItemNo = string.Empty;
            string PostStatus = "";

            int IDExist = 0;
            string VATName = "";
            int nextBOMId = 0;


            #endregion Initializ

            #region Try

            try
            {
                #region open connection and transaction

                if (bomItems == null && !bomItems.Any())
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    "Sorry,No Item found to insert.");
                }
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("BOMRaws", "PBOMId", "varchar(20)", currConn);

                transaction = currConn.BeginTransaction(MessageVM.PurchasemsgMethodNameInsert);

                #endregion open connection and transaction

                #region Fiscal Year Check

                string transactionDate = bomMaster.EffectDate;
                string transactionYearCheck = Convert.ToDateTime(bomMaster.EffectDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue ||
                    Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
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
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
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
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) <
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            ||
                            Convert.ToDateTime(transactionYearCheck) >
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }
                    }

                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK

                #region CheckVATName

                VATName = bomMaster.VATName;
                if (string.IsNullOrEmpty(VATName))
                {
                    throw new ArgumentNullException("BOMImport", MessageVM.msgVatNameNotFound);

                }

                #endregion CheckVATName

                #region Insert BOM Table

                if (bomItems == null)
                {
                    throw new ArgumentNullException("BOMImport", MessageVM.PurchasemsgNoDataToSave);
                }
                int BOMLineNo = 0;

                #region Find Finish Item Name

                sqlText = "";
                sqlText = "select ItemNo from Products ";
                sqlText += " where ProductName =@bomMasterFinishItemName";

                SqlCommand cmdFItemName = new SqlCommand(sqlText, currConn);
                cmdFItemName.Transaction = transaction;
                cmdFItemName.Parameters.AddWithValueAndNullHandle("@bomMasterFinishItemName", bomMaster.FinishItemName);

                fItemNo = (string)cmdFItemName.ExecuteScalar();

                if (fItemNo == null || string.IsNullOrEmpty(fItemNo))
                {
                    throw new ArgumentNullException("BOMImport",
                                                    "Sorry,Inserted Finish Item ('" + bomMaster.FinishItemName +
                                                    "') not save in database, Please check the Item information");
                }

                #endregion Find Finish Item Name

                #region Find Finish Item UOM

                sqlText = "";
                sqlText = "select UOM from Products ";
                sqlText += " where ProductName =@bomMasterFinishItemName ";

                SqlCommand cmdFUom = new SqlCommand(sqlText, currConn);
                cmdFUom.Transaction = transaction;
                cmdFUom.Parameters.AddWithValueAndNullHandle("@bomMasterFinishItemName", bomMaster.FinishItemName);

                string fUom = (string)cmdFUom.ExecuteScalar();

                if (fUom == null || string.IsNullOrEmpty(fUom))
                {
                    throw new ArgumentNullException("BOMImport",
                                                    "Sorry,Inserted Finish Item ('" + bomMaster.FinishItemName +
                                                    "') not save in database, Please check the Item information");
                }

                #endregion Find Finish Item UOM

                #region Checking other BOM after this date

                sqlText = "";
                sqlText = "select count(bomid) from boms ";
                sqlText += " where FinishItemNo =@fItemNo ";
                sqlText += " and effectdate>@bomMasterEffectDate";
                sqlText += " and VATName=@VATName";

                SqlCommand cmdOtherBom = new SqlCommand(sqlText, currConn);
                cmdOtherBom.Transaction = transaction;
                cmdOtherBom.Parameters.AddWithValueAndNullHandle("@fItemNo", fItemNo);
                cmdOtherBom.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdOtherBom.Parameters.AddWithValueAndNullHandle("@VATName", VATName);


                int otherBom = (int)cmdOtherBom.ExecuteScalar();

                if (otherBom > 0)
                {
                    throw new ArgumentNullException("BOMImport",
                                                    "Sorry,You cannot update this price declaration. Another declaration exist after this.");
                }

                #endregion Checking other BOM after this date

                decimal LastNBRPrice = 0;
                decimal NBRWithSDAmount = bomMaster.NBRWithSDAmount;
                decimal LastNBRWithSDAmount = 0;
                decimal TotalQuantity = 0;
                decimal SDAmount = bomMaster.SDAmount;
                decimal VATAmount = bomMaster.VatAmount;
                decimal WholeSalePrice = bomMaster.WholeSalePrice;
                decimal LastMarkupAmount = 0;
                decimal LastSDAmount = 0;
                decimal MarkupAmount = bomMaster.MarkupValue;

                #region Find Last Declared NBRPrice

                var vFinishItemNo = fItemNo;
                //var vEffectDate = bomMaster.EffectDate.ToString("yyyy-MM-dd HH:mm:ss");
                var vEffectDate = bomMaster.EffectDate;

                sqlText = "";
                sqlText += "select top 1 NBRPrice from BOMs WHERE FinishItemNo=@vFinishItemNo";
                sqlText += " AND EffectDate<@vEffectDate ";
                sqlText += " AND VATName=@VATName ";
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastNBRPrice = new SqlCommand(sqlText, currConn);
                cmdFindLastNBRPrice.Transaction = transaction;
                cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@vEffectDate", vEffectDate);
                cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@VATName", VATName);


                object objLastNBRPrice = cmdFindLastNBRPrice.ExecuteScalar();
                if (objLastNBRPrice != null)
                {
                    LastNBRPrice = Convert.ToDecimal(objLastNBRPrice);
                }

                sqlText = "";
                sqlText += "select top 1 NBRWithSDAmount from BOMs WHERE FinishItemNo=@vFinishItemNo ";
                sqlText += " AND EffectDate<@vEffectDate ";
                sqlText += " AND VATName=@VATName ";
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastNBRWithSDAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastNBRWithSDAmount.Transaction = transaction;
                cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@vEffectDate", vEffectDate);
                cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@VATName", VATName);


                object objLastNBRWithSDAmount = cmdFindLastNBRWithSDAmount.ExecuteScalar();
                if (objLastNBRWithSDAmount != null)
                {
                    LastNBRWithSDAmount = Convert.ToDecimal(objLastNBRWithSDAmount);
                }
                sqlText = "";
                sqlText += "select top 1 SDAmount from BOMs WHERE FinishItemNo=@vFinishItemNo ";
                sqlText += " AND EffectDate<@vEffectDate ";
                sqlText += " AND VATName=@VATName";
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastSDAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastSDAmount.Transaction = transaction;
                cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@vEffectDate", vEffectDate);
                cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                object objLastSDAmount = cmdFindLastSDAmount.ExecuteScalar();
                if (objLastSDAmount != null)
                {
                    LastSDAmount = Convert.ToDecimal(objLastSDAmount);
                }

                sqlText = "";
                sqlText += "select top 1 MarkUpValue from BOMs WHERE FinishItemNo=@vFinishItemNo ";
                sqlText += " AND EffectDate<@vEffectDate ";
                sqlText += " AND VATName=@VATName";
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastMarkupAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastMarkupAmount.Transaction = transaction;
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@vEffectDate", vEffectDate);
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                object objLastMarkupAmount = cmdFindLastMarkupAmount.ExecuteScalar();
                if (objLastMarkupAmount != null)
                {
                    LastMarkupAmount = Convert.ToDecimal(objLastMarkupAmount);
                }

                #endregion Find Last Declared NBRPrice

                #region Insert BOMs Master Data

                #region Find Transaction Exist



                sqlText = "";
                sqlText += "select COUNT(FinishItemNo) from BOMs WHERE FinishItemNo=@vFinishItemNo ";
                sqlText += " AND EffectDate=@vEffectDate ";
                sqlText += " AND VATName=@VATName";
                SqlCommand cmdFindBOMId = new SqlCommand(sqlText, currConn);
                cmdFindBOMId.Transaction = transaction;
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@vEffectDate", vEffectDate);
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                IDExist = (int)cmdFindBOMId.ExecuteScalar();
                if (bomMaster.VATName != "VAT 1 (Tender)")
                {
                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException("BOMImport",
                                                        "Price declaration for this item already exist in same date.");
                    }
                }

                #endregion Find Transaction Exist

                #region Generate BOMId

                sqlText = "";
                sqlText = "select isnull(max(cast(BOMId as int)),0)+1 FROM  BOMs";
                SqlCommand cmdGenId = new SqlCommand(sqlText, currConn);
                cmdGenId.Transaction = transaction;
                nextBOMId = (int)cmdGenId.ExecuteScalar();

                if (nextBOMId <= 0)
                {
                    throw new ArgumentNullException("BOMImport", "Sorry,Unable to generate BOMId.");
                }

                #endregion Generate BOMId


                #region Insert only BOM


                bomMaster.LastNBRPrice = LastNBRPrice;
                bomMaster.LastNBRWithSDAmount = LastNBRWithSDAmount;
                bomMaster.LastSDAmount = LastSDAmount;
                bomMaster.LastMarkupValue = LastMarkupAmount;

                sqlText = "";
                sqlText += " insert into BOMs(";
                sqlText += " BOMId,";
                sqlText += " FinishItemNo,";
                sqlText += " EffectDate,";
                sqlText += " VATName,";
                sqlText += " VATRate,";
                sqlText += " UOM,";
                sqlText += " SD,";
                sqlText += " TradingMarkUp,";
                sqlText += " Comments,";
                sqlText += " ActiveStatus,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn,";
                sqlText += " RawTotal,";
                sqlText += " PackingTotal,";
                sqlText += " RebateTotal,";
                sqlText += " AdditionalTotal,";
                sqlText += " RebateAdditionTotal,";
                sqlText += " NBRPrice,";
                sqlText += " Packetprice,";
                sqlText += " RawOHCost,";
                sqlText += " LastNBRPrice,";
                sqlText += " LastNBRWithSDAmount,";
                sqlText += " TotalQuantity,";
                sqlText += " SDAmount,";
                sqlText += " VATAmount,";
                sqlText += " WholeSalePrice,";
                sqlText += " NBRWithSDAmount,";
                sqlText += " MarkUpValue,";
                sqlText += " LastMarkUpValue,";
                sqlText += " LastSDAmount,";
                sqlText += " LastAmount,";
                sqlText += " Post";

                sqlText += " )";
                sqlText += " values( ";
                sqlText += "@nextBOMId ,";
                sqlText += "@vFinishItemNo ,";
                sqlText += "@bomMasterEffectDate ,";
                sqlText += "@bomMasterVATName ,";
                sqlText += "@bomMasterVATRate,";
                sqlText += "@fUom ,";
                sqlText += "@bomMasterSDRate,";
                sqlText += "@bomMasterTradingMarkup,";
                sqlText += "@bomMasterComments ,";
                sqlText += "@bomMasterActiveStatus ,";
                sqlText += "@bomMasterCreatedBy ,";
                sqlText += "@bomMasterCreatedOn ,";
                sqlText += "@bomMasterLastModifiedBy ,";
                sqlText += "@bomMasterLastModifiedOn ,";
                sqlText += "@bomMasterRawTotal,";
                sqlText += "@bomMasterPackingTotal,";
                sqlText += "@bomMasterRebateTotal,";
                sqlText += "@bomMasterAdditionalTotal,";
                sqlText += "@bomMasterRebateAdditionTotal,";
                sqlText += "@bomMasterPNBRPrice,";
                sqlText += "@bomMasterPPacketPrice,";
                sqlText += "@bomMasterRawOHCost,";
                sqlText += "@bomMasterLastNBRPrice,";
                sqlText += "@bomMasterLastNBRWithSDAmount,";
                sqlText += "@bomMasterTotalQuantity,";
                sqlText += "@bomMasterSDAmount,";
                sqlText += "@bomMasterVatAmount,";
                sqlText += "@bomMasterWholeSalePrice,";
                sqlText += "@bomMasterNBRWithSDAmount,";
                sqlText += "@bomMasterMarkupValue,";
                sqlText += "@bomMasterLastMarkupValue,";
                sqlText += "@bomMasterLastSDAmount,";
                sqlText += "@bomMasterLastSDAmount,";
                sqlText += "@bomMasterPost ";


                sqlText += ")	";




                SqlCommand cmdInsMaster = new SqlCommand(sqlText, currConn);
                cmdInsMaster.Transaction = transaction;
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@nextBOMId", nextBOMId);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo ?? Convert.DBNull);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate ?? Convert.DBNull);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName ?? Convert.DBNull);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterVATRate", bomMaster.VATRate);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@fUom", fUom ?? Convert.DBNull);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterSDRate", bomMaster.SDRate);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterTradingMarkup", bomMaster.TradingMarkup);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterComments", bomMaster.Comments ?? Convert.DBNull);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterActiveStatus", bomMaster.ActiveStatus ?? Convert.DBNull);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterCreatedBy", bomMaster.CreatedBy ?? Convert.DBNull);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterCreatedOn", bomMaster.CreatedOn ?? Convert.DBNull);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedBy", bomMaster.LastModifiedBy ?? Convert.DBNull);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedOn", bomMaster.LastModifiedOn ?? Convert.DBNull);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterRawTotal", bomMaster.RawTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterPackingTotal", bomMaster.PackingTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterRebateTotal", bomMaster.RebateTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterAdditionalTotal", bomMaster.AdditionalTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterRebateAdditionTotal", bomMaster.RebateAdditionTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterPNBRPrice", bomMaster.PNBRPrice);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterPPacketPrice", bomMaster.PPacketPrice);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterRawOHCost", bomMaster.RawOHCost);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastNBRPrice", bomMaster.LastNBRPrice);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastNBRWithSDAmount", bomMaster.LastNBRWithSDAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterTotalQuantity", bomMaster.TotalQuantity);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterSDAmount", bomMaster.SDAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterVatAmount", bomMaster.VatAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterWholeSalePrice", bomMaster.WholeSalePrice);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterNBRWithSDAmount", bomMaster.NBRWithSDAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterMarkupValue", bomMaster.MarkupValue);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastMarkupValue", bomMaster.LastMarkupValue);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastSDAmount", bomMaster.LastSDAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastSDAmount", bomMaster.LastSDAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterPost", bomMaster.Post ?? Convert.DBNull);


                transResult = (int)cmdInsMaster.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    MessageVM.PurchasemsgSaveNotSuccessfully);
                }

                #endregion Insert only BOM

                #endregion

                foreach (var BItem in bomItems.ToList())
                {

                    #region Find Raw Item

                    sqlText = "";
                    sqlText = "select ItemNo from Products ";
                    sqlText += " where ProductName =@BItemRawItemName ";

                    SqlCommand cmdRItemNo = new SqlCommand(sqlText, currConn);
                    cmdRItemNo.Transaction = transaction;
                    cmdRItemNo.Parameters.AddWithValueAndNullHandle("@BItemRawItemName", BItem.RawItemName);

                    string RItemNo = (string)cmdRItemNo.ExecuteScalar();

                    if (RItemNo == null || string.IsNullOrEmpty(RItemNo))
                    {
                        throw new ArgumentNullException("BOMImport",
                                                        "Sorry,Inserted Raw Item ('" + BItem.RawItemName +
                                                        "') not save in database, Please check the Item information");
                    }

                    #endregion Find Raw Item

                    #region Find Raw Item UOM

                    sqlText = "";
                    sqlText = "select UOM from Products ";
                    sqlText += " where ProductName =@BItemRawItemName";

                    SqlCommand cmdRUom = new SqlCommand(sqlText, currConn);
                    cmdRUom.Transaction = transaction;
                    cmdRUom.Parameters.AddWithValueAndNullHandle("@BItemRawItemName", BItem.RawItemName);

                    string rUom = (string)cmdRUom.ExecuteScalar();

                    if (rUom == null || string.IsNullOrEmpty(rUom))
                    {
                        throw new ArgumentNullException("BOMImport",
                                                        "Sorry,Inserted Raw Item ('" + BItem.RawItemName +
                                                        "') not save in database, Please check the Item information");
                    }

                    #endregion Find Raw Item UOM

                    #region UOMc

                    decimal rUomc = 0;
                    if (BItem.UOM == rUom)
                    {
                        rUomc = 1;
                    }
                    else
                    {
                        sqlText = "";
                        sqlText = "SELECT top 1 u.Convertion FROM UOMs u";
                        sqlText += " WHERE u.UOMFrom=@rUom  AND u.UOMTo=@BItemUOM AND u.ActiveStatus='Y'";

                        SqlCommand cmdUomc = new SqlCommand(sqlText, currConn);
                        cmdUomc.Transaction = transaction;
                        cmdUomc.Parameters.AddWithValueAndNullHandle("@rUom", rUom);
                        cmdUomc.Parameters.AddWithValueAndNullHandle("@BItemUOM", BItem.UOM);


                        object objrUomc = cmdUomc.ExecuteScalar();
                        rUomc = Convert.ToDecimal(objrUomc);
                        if (rUomc <= 0 || objrUomc == null)
                        {
                            throw new ArgumentNullException("BOMImport",
                                                            "Sorry,Inserted UOM  ('" + BItem.UOM + "' and '" + rUom +
                                                            "') conversion not save in database,Item name :'" +
                                                            BItem.RawItemName + "'  Please check the Item information");
                        }
                    }

                    decimal uOMPrice = 0;
                    decimal uOMUQty = 0;
                    decimal uOMWQty = 0;
                    decimal totalQty = 0;
                    decimal unitCost = 0;
                    uOMPrice = BItem.UnitCost / rUomc;
                    uOMUQty = rUomc * BItem.UseQuantity;
                    uOMWQty = rUomc * BItem.WastageQuantity;
                    totalQty = uOMUQty + uOMWQty;
                    //unitCost=BItem

                    #endregion UOMc

                    #region Generate BOMRaw Id

                    int nextBOMRawId = 0;
                    sqlText = "";
                    sqlText = "select isnull(max(cast(BOMRawId as int)),0)+1 FROM  BOMRaws";
                    SqlCommand cmdGenRawId = new SqlCommand(sqlText, currConn);
                    cmdGenRawId.Transaction = transaction;
                    nextBOMRawId = (int)cmdGenRawId.ExecuteScalar();

                    if (nextBOMRawId <= 0)
                    {
                        throw new ArgumentNullException("BOMImport", "Sorry,Unable to generate BOMRawId.");
                    }

                    #endregion Generate BOMRaw Id


                    BOMLineNo++;
                    sqlText = "";
                    sqlText += " insert into BOMRaws(";
                    sqlText += " BOMRawId,";
                    sqlText += " BOMId,";
                    sqlText += " BOMLineNo,";
                    sqlText += " FinishItemNo,";
                    sqlText += " RawItemNo,";
                    sqlText += " RawItemType,";
                    sqlText += " EffectDate,";
                    sqlText += " VATName,";
                    sqlText += " UseQuantity,";
                    sqlText += " WastageQuantity,";
                    sqlText += " Cost,";
                    sqlText += " UOM,";
                    sqlText += " VATRate,";
                    sqlText += " VATAmount,";
                    sqlText += " SD,";
                    sqlText += " SDAmount,";
                    sqlText += " TradingMarkUp,";
                    sqlText += " RebateRate,";
                    sqlText += " MarkUpValue,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " UnitCost,";
                    sqlText += " UOMn,";
                    sqlText += " UOMc,";
                    sqlText += " UOMPrice,";
                    sqlText += " UOMUQty,";
                    sqlText += " UOMWQty,";
                    sqlText += " TotalQuantity,";
                    sqlText += " PBOMId,";
                    sqlText += " Post";



                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "@nextBOMRawId,";
                    sqlText += "@nextBOMId,";
                    sqlText += "@BOMLineNo ,";
                    sqlText += "@fItemNo,";
                    sqlText += "@RItemNo,";
                    sqlText += "@BItem.RawItemType,";
                    sqlText += "@bomMaster.EffectDate,";
                    sqlText += "@VATName,";
                    sqlText += "@BItemUseQuantity,";
                    sqlText += "@BItemWastageQuantity,";
                    sqlText += "@BItemCost,";
                    sqlText += "@BItemUOM,";
                    sqlText += "@BItemVATRate,";
                    sqlText += "@BItemVatAmount ,";
                    sqlText += "@BItemSD,";
                    sqlText += "@BItemSDAmount,";
                    sqlText += "@BItemTradingMarkUp,";
                    sqlText += "@BItemRebateRate,";
                    sqlText += "@MarkupAmount,";
                    sqlText += "@BItemCreatedBy,";
                    sqlText += "@BItemCreatedOn,";
                    sqlText += "@BItemLastModifiedBy,";
                    sqlText += "@BItemLastModifiedOn,";
                    sqlText += "@BItemUnitCost,";
                    sqlText += "@rUom,";
                    sqlText += "@rUomc,";
                    sqlText += "@uOMPrice,";
                    sqlText += "@uOMUQty,";
                    sqlText += "@uOMWQty,";
                    sqlText += "@totalQty,";
                    sqlText += "0 ,";
                    sqlText += "@bomMasterPost";



                    sqlText += ")	";




                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@nextBOMRawId", nextBOMRawId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@nextBOMId", nextBOMId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BOMLineNo", BOMLineNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@fItemNo", fItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@RItemNo", RItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemRawItemType", BItem.RawItemType ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", Ordinary.DateToDate(bomMaster.EffectDate));
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VATName", VATName ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUseQuantity", BItem.UseQuantity);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemWastageQuantity", BItem.WastageQuantity);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemCost", BItem.Cost);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOM", BItem.UOM ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemVATRate", BItem.VATRate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemVatAmount", BItem.VatAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemSD", BItem.SD);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemSDAmount", BItem.SDAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemTradingMarkUp", BItem.TradingMarkUp);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItem.RebateRate", BItem.RebateRate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MarkupAmount", MarkupAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemCreatedBy", BItem.CreatedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemCreatedOn", BItem.CreatedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemLastModifiedBy", BItem.LastModifiedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemLastModifiedOn", BItem.LastModifiedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUnitCost", BItem.UnitCost);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@rUom", rUom ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@rUomc", rUomc);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@uOMPrice", uOMPrice);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@uOMUQty", uOMUQty);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@uOMWQty", uOMWQty);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@totalQty", totalQty);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterPost", bomMaster.Post ?? Convert.DBNull);


                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("BOMImport", MessageVM.PurchasemsgSaveNotSuccessfully);
                    }

                }

                #endregion Insert Detail Table

                #region Insert BOMCompanyOverhead Table

                if (bomOHs == null)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    MessageVM.PurchasemsgNoDataToSave);
                }




                int OHLineNo = 0;

                foreach (var OHItem in bomOHs.ToList())
                {
                    #region Generate Overhead Id

                    sqlText = "";
                    sqlText = "select isnull(max(cast(BOMOverHeadId as int)),0)+1 FROM  BOMCompanyOverhead";
                    SqlCommand cmdGenOHId = new SqlCommand(sqlText, currConn);
                    cmdGenOHId.Transaction = transaction;
                    int nextOHId = (int)cmdGenOHId.ExecuteScalar();

                    if (nextOHId <= 0)
                    {
                        throw new ArgumentNullException("BOMImport", "Sorry,Unable to generate Overhead Id.");
                    }

                    #endregion Generate Overhead Id

                    #region Find Transaction Exist

                    OHLineNo++;
                    sqlText = "";
                    sqlText += "select COUNT(HeadName) from BOMCompanyOverhead WHERE FinishItemNo=@vFinishItemNo";
                    sqlText += " AND HeadName=@OHItemHeadName AND EffectDate=@bomMasterEffectDate and VATName=@VATName";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemHeadName", OHItem.HeadName);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    IDExist = (int)cmdFindId.ExecuteScalar();
                    if (bomMaster.VATName != "VAT 1 (Tender)")
                    {
                        if (IDExist > 0)
                        {
                            throw new ArgumentNullException("bOMInsert", MessageVM.PurchasemsgFindExistID);
                        }
                    }

                    #endregion Find Transaction Exist

                    #region Insert only OH

                    decimal vRebateAmount = 0;
                    decimal vAddingAmount = 0;


                    sqlText = "";
                    sqlText += " insert into BOMCompanyOverhead(";
                    sqlText += " BOMOverHeadId,";
                    sqlText += " BOMId,";
                    sqlText += " HeadName,";
                    sqlText += " HeadAmount,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " FinishItemNo,";
                    sqlText += " EffectDate,";
                    sqlText += " OHLineNo,";
                    sqlText += " VATName,";
                    sqlText += " RebatePercent, ";
                    sqlText += " RebateAmount,";
                    sqlText += " AdditionalCost, ";
                    sqlText += " Post ";
                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "@nextOHId,";
                    sqlText += "@nextBOMId,";
                    sqlText += "@OHItemHeadName,";
                    sqlText += "@OHItemHeadAmount,";
                    sqlText += "@OHItemCreatedBy,";
                    sqlText += "@OHItemCreatedOn,";
                    sqlText += "@OHItemLastModifiedBy,";
                    sqlText += "@OHItemLastModifiedOn,";
                    sqlText += "@vFinishItemNo,";
                    sqlText += "@bomMasterEffectDate,";
                    sqlText += "@OHLineNo,";
                    sqlText += "@VATName,";
                    sqlText += "@OHItemRebatePercent,";
                    sqlText += "@OHItemRebateAmount,";
                    sqlText += "@OHItemAdditionalCost,";
                    sqlText += "@OHItemPost";

                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@nextOHId", nextOHId);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@nextBOMId", nextBOMId);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemHeadName", OHItem.HeadName);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemHeadAmount", OHItem.HeadAmount);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemCreatedBy", OHItem.CreatedBy);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemCreatedOn", OHItem.CreatedOn);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemLastModifiedBy", OHItem.LastModifiedBy);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemLastModifiedOn", OHItem.LastModifiedOn);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHLineNo", OHLineNo);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@VATName", VATName);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemRebatePercent", OHItem.RebatePercent);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemRebateAmount", OHItem.RebateAmount);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemAdditionalCost", OHItem.AdditionalCost);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemPost", OHItem.Post);


                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.PurchasemsgSaveNotSuccessfully);
                    }

                    #endregion Insert only OH
                }

                #endregion Insert BOMCompanyOverhead Table

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
                retResults[1] = MessageVM.bomMsgSaveSuccessfully;
                //nextBOMId
                retResults[2] = "" + nextBOMId;
                retResults[3] = "" + PostStatus;

                #endregion SuccessResult

            }
            #endregion Try
            #region Catch and Finall

            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public string[] BOMPreInsert(BOMNBRVM vm)
        {
            #region Initializ

            string[] retResults = new string[5];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            retResults[4] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";
            string PostStatus = "";
            int IDExist = 0;
            //string VATName = "";
            int nextBOMId = 0;

            #endregion Initializ
            #region Try
            try
            {
                #region Ready Data to Insert

                int countRaw = 0;
                int countOh = 0;
                Decimal rawTotal = 0;
                Decimal packingTotal = 0;
                Decimal rebateTotal = 0;
                Decimal rebateAdditionalTotal = 0;
                Decimal additionalTotal = 0;
                Decimal vMarkupValue;
                Decimal vSDAmount;
                Decimal vVatAmount;

                foreach (var rawItem in vm.Items)
                {
                    vMarkupValue = rawItem.Cost * vm.TradingMarkup / 100;
                    vSDAmount = (rawItem.Cost + vMarkupValue) * vm.SDRate / 100;
                    vVatAmount = (rawItem.Cost + vMarkupValue + vSDAmount) * vm.VATRate / 100;

                    rawItem.BOMLineNo = countRaw.ToString();
                    rawItem.VATRate = vm.VATRate;
                    rawItem.VatAmount = vVatAmount;
                    rawItem.VatName = vm.VATName;
                    rawItem.SD = vm.SDRate;
                    rawItem.SDAmount = vSDAmount;
                    rawItem.TradingMarkUp = vm.TradingMarkup;
                    rawItem.CreatedBy = vm.CreatedBy;
                    rawItem.CreatedOn = vm.CreatedOn;
                    rawItem.LastModifiedBy = vm.LastModifiedBy;
                    rawItem.LastModifiedOn = vm.LastModifiedOn;
                    rawItem.Post = "N";
                    rawItem.CustomerID = vm.CustomerID??"0";
                    rawItem.FinishItemNo = vm.ItemNo;
                    if (rawItem.RawItemType != "Pack" && rawItem.RawItemType != "Overhead")
                    {
                        rawTotal += rawItem.Cost;
                    }
                    else if (rawItem.RawItemType == "Pack")
                    {
                        packingTotal += rawItem.Cost;
                    }
                    else if (rawItem.RawItemType == "Overhead")
                    {
                        rebateTotal += rawItem.Cost;
                    }
                    countRaw++;
                }
                //////overhead item
                foreach (var overhead in vm.Overheads)
                {
                    overhead.CreatedBy = vm.CreatedBy;
                    overhead.CreatedOn = vm.CreatedOn;
                    overhead.LastModifiedBy = vm.LastModifiedBy;
                    overhead.LastModifiedOn = vm.LastModifiedOn;
                    overhead.Post = "N";
                    overhead.CustomerID = vm.CustomerID??"0";
                    overhead.FinishItemNo = vm.ItemNo;
                    overhead.EffectDate = vm.EffectDate;
                    overhead.OHLineNo = countOh.ToString();
                    overhead.RebateAmount = overhead.HeadAmount - overhead.AdditionalCost;
                    additionalTotal += overhead.AdditionalCost;
                    countOh++;
                    rebateAdditionalTotal += overhead.HeadAmount;

                }
                BOMOHVM bomOh1 = new BOMOHVM();
                bomOh1.HeadName = "Margin";
                bomOh1.OHCode = "ovh0";
                bomOh1.HeadID = "ovh0";
                bomOh1.HeadAmount = vm.Margin;
                bomOh1.CreatedBy = vm.CreatedBy;
                bomOh1.CreatedOn = vm.CreatedOn;
                bomOh1.LastModifiedBy = vm.LastModifiedBy;
                bomOh1.LastModifiedOn = vm.LastModifiedOn;
                bomOh1.FinishItemNo = vm.ItemNo;
                bomOh1.EffectDate = vm.EffectDate;
                bomOh1.OHLineNo = "100";
                bomOh1.RebatePercent = 0;
                bomOh1.Post = "N";
                bomOh1.AdditionalCost = bomOh1.HeadAmount;
                vm.Overheads.Add(bomOh1);

                //////////////NBR
                //////vm.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //////vm.CreatedBy = identity.Name;
                //////vm.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                vm.Post = "N";
                vm.ActiveStatus = "Y";

                var abc = rawTotal + packingTotal + rebateTotal + additionalTotal + vm.Margin;
                vm.RawTotal = rawTotal;
                vm.PackingTotal = packingTotal;
                vm.RebateTotal = rebateTotal;
                vm.RebateAdditionTotal = rebateAdditionalTotal;
                vm.AdditionalTotal = additionalTotal;

                var MarkupValue = abc * vm.TradingMarkup / 100;
                var nbrPrice = abc + MarkupValue;
                var SDAmount = (nbrPrice) * vm.SDRate / 100;
                var VatAmount = (nbrPrice + SDAmount) * vm.VATRate / 100;

                vm.PNBRPrice = nbrPrice;
                vm.SDAmount = SDAmount;
                vm.VatAmount = VatAmount;
                vm.ReceiveCost = rawTotal + packingTotal + rebateTotal + additionalTotal;
                vm.RawOHCost = rawTotal + packingTotal + rebateTotal;
                vm.NBRWithSDAmount = nbrPrice + nbrPrice * vm.SDRate / 100;
                #endregion
                #region Insert Data
                if (vm.Operation.ToLower() == "add")
                {
                    retResults = BOMInsert(vm.Items, vm.Overheads, vm);
                }
                else
                {
                    retResults = BOMUpdate(vm.Items, vm.Overheads, vm);
                }
                #endregion
            }
            #endregion
            #region Catch and Finall

            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = ""; //catch ex

                FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
                return retResults;
            }
            finally
            {

            }

            #endregion Catch and Finall
            #region Result

            return retResults;

            #endregion Result
        }

        public string[] BOMInsert(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster)
        {
            #region Initializ

            string[] retResults = new string[5];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            retResults[4] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";
            string PostStatus = "";
            int IDExist = 0;
            //string VATName = "";
            int nextBOMId = 0;

            #endregion Initializ

            #region Try

            try
            {

                #region open connection and transaction

                if (bomItems == null && !bomItems.Any())
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert, "Sorry,No Item found to insert.");
                }
                currConn = _dbsqlConnection.GetConnection();

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd(""@DateTo"", "PBOMId", "varchar(20)", currConn);

                transaction = currConn.BeginTransaction(MessageVM.bomMsgMethodNameInsert);


                #endregion open connection and transaction

                #region Fiscal Year Check

                string transactionDate = bomMaster.EffectDate;
                string transactionYearCheck = Convert.ToDateTime(bomMaster.EffectDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue ||
                    Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock

                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                               " where @transactionYearCheck between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValueAndNullHandle("@transactionYearCheck", transactionYearCheck);

                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
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
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) <
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            ||
                            Convert.ToDateTime(transactionYearCheck) >
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }
                    }

                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK

                #region CheckVATName

                if (string.IsNullOrEmpty(bomMaster.VATName))
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert, MessageVM.msgVatNameNotFound);

                }

                #endregion CheckVATName

                #region Insert BOM Table

                if (bomItems == null || bomItems.Count <= 0)
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert, MessageVM.PurchasemsgNoDataToSave);
                }

                int BOMLineNo = 0;


                #region finish Product Exists

                sqlText = "";
                sqlText = " select count(ItemNo) from Products ";
                sqlText += " where ItemNo =@bomMasterItemNo ";

                SqlCommand cmdfindFItem = new SqlCommand(sqlText, currConn);
                cmdfindFItem.Transaction = transaction;
                cmdfindFItem.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);

                int findFItem = (int)cmdfindFItem.ExecuteScalar();

                if (findFItem <= 0)
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                    "Price declaration Item not in database please check the Item ('" +
                                                    bomMaster.FinishItemName + "')");
                }

                #endregion Product Exists

                #region Find Transaction Exist

                sqlText = "";
                sqlText += "select COUNT(FinishItemNo) from BOMs WHERE FinishItemNo=@bomMasterItemNo";
                //sqlText += " AND EffectDate=@bomMaster.EffectDate.Date + "' ";
                sqlText += " AND EffectDate=@bomMasterEffectDate";
                sqlText += " AND VATName=@bomMasterVATName";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                }
                SqlCommand cmdFindBOMId = new SqlCommand(sqlText, currConn);
                cmdFindBOMId.Transaction = transaction;
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                int IDExist1 = (int)cmdFindBOMId.ExecuteScalar();
                if (bomMaster.VATName != "VAT 1 (Tender)")
                {
                    if (IDExist1 > 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                        "Price declaration for this item ('" + bomMaster.FinishItemName +
                                                        "') already exist in same date  ('" + bomMaster.EffectDate +
                                                        "') .");
                    }
                }

                #endregion Find Transaction Exist

                decimal LastNBRPrice = 0;

                decimal LastNBRWithSDAmount = 0;
                decimal LastMarkupAmount = 0;
                decimal LastSDAmount = 0;
                decimal MarkupAmount = bomMaster.MarkupValue;

                #region Find Last Declared NBRPrice

                #region LastNBRPrice

                if (bomMaster.LastNBRPrice <= 0)
                {
                    sqlText = "";
                    sqlText += "select top 1 NBRPrice from BOMs WHERE FinishItemNo=@bomMasterItemNo ";
                    sqlText += " AND EffectDate<@bomMasterEffectDate ";
                    sqlText += " AND VATName=@bomMasterVATName ";
                    sqlText += " and Post='Y'";
                    if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                    { }
                    else
                    {
                        sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                    }

                    sqlText += " order by EffectDate desc";
                    SqlCommand cmdFindLastNBRPrice = new SqlCommand(sqlText, currConn);
                    cmdFindLastNBRPrice.Transaction = transaction;
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                    object objLastNBRPrice = cmdFindLastNBRPrice.ExecuteScalar();
                    if (objLastNBRPrice != null)
                    {
                        LastNBRPrice = Convert.ToDecimal(objLastNBRPrice);
                    }
                }
                else
                {
                    LastNBRPrice = bomMaster.LastNBRPrice;
                }

                #endregion LastNBRPrice

                sqlText = "";
                sqlText += "select top 1 NBRWithSDAmount from BOMs WHERE FinishItemNo=@bomMasterItemNo";
                sqlText += " AND EffectDate<@bomMasterEffectDate";
                sqlText += " AND VATName=@bomMasterVATName";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                }
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastNBRWithSDAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastNBRWithSDAmount.Transaction = transaction;
                cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                object objLastNBRWithSDAmount = cmdFindLastNBRWithSDAmount.ExecuteScalar();
                if (objLastNBRWithSDAmount != null)
                {
                    LastNBRWithSDAmount = Convert.ToDecimal(objLastNBRWithSDAmount);
                }
                sqlText = "";
                sqlText += "select top 1 SDAmount from BOMs WHERE FinishItemNo=@bomMasterItemNo";
                sqlText += " AND EffectDate<@bomMasterEffectDate";
                sqlText += " AND VATName=@bomMasterVATName ";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                }
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastSDAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastSDAmount.Transaction = transaction;
                cmdFindLastSDAmount.Transaction = transaction;
                cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                object objLastSDAmount = cmdFindLastSDAmount.ExecuteScalar();
                if (objLastSDAmount != null)
                {
                    LastSDAmount = Convert.ToDecimal(objLastSDAmount);
                }

                sqlText = "";
                sqlText += "select top 1 MarkUpValue from BOMs WHERE FinishItemNo=@bomMasterItemNo";
                sqlText += " AND EffectDate<@bomMasterEffectDate ";
                sqlText += " AND VATName=@bomMasterVATName ";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                }
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastMarkupAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastMarkupAmount.Transaction = transaction;
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                object objLastMarkupAmount = cmdFindLastMarkupAmount.ExecuteScalar();
                if (objLastMarkupAmount != null)
                {
                    LastMarkupAmount = Convert.ToDecimal(objLastMarkupAmount);
                }

                #endregion Find Last Declared NBRPrice

                #region Insert BOMs Master Data


                #region Generate BOMId

                nextBOMId = 0;
                sqlText = "";
                sqlText = "select isnull(max(cast(BOMId as int)),0)+1 FROM  BOMs";

                SqlCommand cmdGenId = new SqlCommand(sqlText, currConn);
                cmdGenId.Transaction = transaction;
                nextBOMId = (int)cmdGenId.ExecuteScalar();

                if (nextBOMId <= 0)
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert, "Sorry,Unable to generate BOMId.");
                }

                #endregion Generate BOMId


                #region Insert only BOM

                if (bomMaster.IsImported != "Y")
                {
                    bomMaster.LastNBRPrice = LastNBRPrice;
                    bomMaster.LastNBRWithSDAmount = LastNBRWithSDAmount;
                    bomMaster.LastSDAmount = LastSDAmount;
                    bomMaster.LastMarkupValue = LastMarkupAmount;

                }

                sqlText = "";
                sqlText += " insert into BOMs(";
                sqlText += " BOMId,";
                sqlText += " FinishItemNo,";
                sqlText += " EffectDate,";
                sqlText += " VATName,";
                sqlText += " VATRate,";
                sqlText += " UOM,";
                sqlText += " SD,";
                sqlText += " TradingMarkUp,";
                sqlText += " Comments,";
                sqlText += " ActiveStatus,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn,";
                sqlText += " RawTotal,";
                sqlText += " PackingTotal,";
                sqlText += " RebateTotal,";
                sqlText += " AdditionalTotal,";
                sqlText += " RebateAdditionTotal,";
                sqlText += " NBRPrice,";
                sqlText += " Packetprice,";
                sqlText += " RawOHCost,";
                sqlText += " LastNBRPrice,";
                sqlText += " LastNBRWithSDAmount,";
                sqlText += " TotalQuantity,";
                sqlText += " SDAmount,";
                sqlText += " VATAmount,";
                sqlText += " WholeSalePrice,";
                sqlText += " NBRWithSDAmount,";
                sqlText += " MarkUpValue,";
                sqlText += " LastMarkUpValue,";
                sqlText += " LastSDAmount,";
                sqlText += " LastAmount,";
                sqlText += " CustomerId,";
                sqlText += " Post";

                sqlText += " )";
                sqlText += " values(	";
                sqlText += "@nextBOMId ,";
                sqlText += "@bomMasterItemNo ,";
                sqlText += "@bomMasterEffectDate ,";
                sqlText += "@bomMasterVATName ,";
                sqlText += "@bomMasterVATRate ,";
                sqlText += "@bomMasterUOM ,";
                sqlText += "@bomMasterSDRate ,";
                sqlText += "@bomMasterTradingMarkup ,";
                sqlText += "@bomMasterComments ,";
                sqlText += "@bomMasterActiveStatus ,";
                sqlText += "@bomMasterCreatedBy ,";
                sqlText += "@bomMasterCreatedOn ,";
                sqlText += "@bomMasterLastModifiedBy ,";
                sqlText += "@bomMasterLastModifiedOn ,";
                sqlText += "@bomMasterRawTotal ,";
                sqlText += "@bomMasterPackingTotal ,";
                sqlText += "@bomMasterRebateTotal ,";
                sqlText += "@bomMasterAdditionalTotal ,";
                sqlText += "@bomMasterRebateAdditionTotal ,";
                sqlText += "@bomMasterPNBRPrice ,";
                sqlText += "@bomMasterPPacketPrice ,";
                sqlText += "@bomMasterRawOHCost ,";
                sqlText += "@bomMasterLastNBRPrice ,";
                sqlText += "@bomMasterLastNBRWithSDAmount ,";
                sqlText += "@bomMasterTotalQuantity ,";
                sqlText += "@bomMasterSDAmount ,";
                sqlText += "@bomMasterVatAmount ,";
                sqlText += "@bomMasterWholeSalePrice ,";
                sqlText += "@bomMasterNBRWithSDAmount ,";
                sqlText += "@bomMasterMarkupValue ,";
                sqlText += "@bomMasterLastMarkupValue ,";
                sqlText += "@bomMasterLastSDAmount ,";
                sqlText += "@bomMasterLastSDAmount ,";
                sqlText += "@bomMasterCustomerID  ,";
                sqlText += "@bomMasterPost ";


                sqlText += ")	";




                SqlCommand cmdInsMaster = new SqlCommand(sqlText, currConn);
                cmdInsMaster.Transaction = transaction;
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@nextBOMId", nextBOMId);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterVATRate", bomMaster.VATRate);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterUOM", bomMaster.UOM);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterSDRate", bomMaster.SDRate);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterTradingMarkup", bomMaster.TradingMarkup);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterComments", bomMaster.Comments);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterActiveStatus", bomMaster.ActiveStatus);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterCreatedBy", bomMaster.CreatedBy);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterCreatedOn", bomMaster.CreatedOn);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedBy", bomMaster.LastModifiedBy);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedOn", bomMaster.LastModifiedOn);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterRawTotal", bomMaster.RawTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterPackingTotal", bomMaster.PackingTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterAdditionalTotal", bomMaster.AdditionalTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterRebateTotal", bomMaster.RebateTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterRebateAdditionTotal", bomMaster.RebateAdditionTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterPNBRPrice", bomMaster.PNBRPrice);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterPPacketPrice", bomMaster.PPacketPrice);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterRawOHCost", bomMaster.RawOHCost);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastNBRPrice", bomMaster.LastNBRPrice);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastNBRWithSDAmount", bomMaster.LastNBRWithSDAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterTotalQuantity", bomMaster.TotalQuantity);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterSDAmount", bomMaster.SDAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterVatAmount", bomMaster.VatAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterWholeSalePrice", bomMaster.WholeSalePrice);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterNBRWithSDAmount", bomMaster.NBRWithSDAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterMarkupValue", bomMaster.MarkupValue);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastMarkupValue", bomMaster.LastMarkupValue);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastSDAmount", bomMaster.LastSDAmount);
                //cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastSDAmount", bomMaster.LastSDAmount);
                cmdInsMaster.Parameters.AddWithValue("@bomMasterCustomerID", bomMaster.CustomerID??"0");
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterPost", bomMaster.Post);


                transResult = (int)cmdInsMaster.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                    "Price declaration for this item ('" + bomMaster.FinishItemName +
                                                    "') and VAT Name ( '" + bomMaster.VATName +
                                                    "' ) not save in date date ('" + bomMaster.EffectDate + "') .");
                }

                #endregion Insert only BOM

                #region Update PacketPrice

                sqlText = "";

                sqlText += " update BOMs set  ";

                sqlText += " Packetprice=@bomMasterPPacketPrice";
                sqlText += " where  FinishItemNo =@bomMasterItemNo  ";
                sqlText += " and EffectDate=@bomMasterEffectDate";
                sqlText += " and VATName= @bomMasterVATName";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                }
                SqlCommand cmdUpdateP = new SqlCommand(sqlText, currConn);
                cmdUpdateP.Transaction = transaction;
                cmdUpdateP.Parameters.AddWithValueAndNullHandle("@bomMasterPPacketPrice", bomMaster.PPacketPrice);
                cmdUpdateP.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdUpdateP.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdUpdateP.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdUpdateP.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                transResult = (int)cmdUpdateP.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                    "Price declaration for this item ('" + bomMaster.FinishItemName +
                                                    "') and VAT Name ( '" + bomMaster.VATName +
                                                    "' ) not save in date date ('" + bomMaster.EffectDate + "') .");
                }

                #endregion Update PacketPrice


                #endregion

                #region "@DateTo"

                foreach (var BItem in bomItems.ToList())
                {
                    if (BItem.RawItemNo == "41")
                    {

                    }
                    #region Raw Product Exists

                    sqlText = "";
                    sqlText = "select count(ItemNo) from Products ";
                    sqlText += " where ItemNo =@BItemRawItemNo  ";

                    SqlCommand cmdfindRItem = new SqlCommand(sqlText, currConn);
                    cmdfindRItem.Transaction = transaction;
                    cmdfindRItem.Parameters.AddWithValueAndNullHandle("@BItemRawItemNo", BItem.RawItemNo);

                    int findRItem = (int)cmdfindRItem.ExecuteScalar();

                    if (findRItem <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                        "Price declaration Material Name not in database please check theMaterial Name ('" +
                                                        BItem.RawItemName + "')");
                    }

                    //GetProductNo

                    #endregion Raw Product Exists

                    #region Generate BOMRaw Id


                    int nextBOMRawId = 0;
                    sqlText = "";
                    sqlText = "select isnull(max(cast(BOMRawId as int)),0)+1 FROM  BOMRaws";
                    SqlCommand cmdGenRawId = new SqlCommand(sqlText, currConn);
                    cmdGenRawId.Transaction = transaction;
                    nextBOMRawId = (int)cmdGenRawId.ExecuteScalar();

                    if (nextBOMRawId <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                        "Sorry,Unable to generate BOMRawId.");
                    }

                    #endregion Generate BOMRaw Id


                    BOMLineNo++;

                    sqlText = "";
                    sqlText += " insert into BOMRaws(";
                    sqlText += " BOMRawId,";
                    sqlText += " BOMId,";
                    sqlText += " BOMLineNo,";
                    sqlText += " FinishItemNo,";
                    sqlText += " RawItemNo,";
                    sqlText += " RawItemType,";
                    sqlText += " EffectDate,";
                    sqlText += " VATName,";
                    sqlText += " UseQuantity,";
                    sqlText += " WastageQuantity,";
                    sqlText += " Cost,";
                    sqlText += " UOM,";
                    sqlText += " VATRate,";
                    sqlText += " VATAmount,";
                    sqlText += " SD,";
                    sqlText += " SDAmount,";
                    sqlText += " TradingMarkUp,";
                    sqlText += " RebateRate,";

                    sqlText += " MarkUpValue,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " UnitCost,";
                    sqlText += " UOMn,";
                    sqlText += " UOMc,";
                    sqlText += " UOMPrice,";
                    sqlText += " UOMUQty,";
                    sqlText += " UOMWQty,";
                    sqlText += " TotalQuantity,";
                    sqlText += " PBOMId,";
                    sqlText += " Post,";
                    sqlText += " PInvoiceNo,";
                    sqlText += " CustomerID,";
                    sqlText += " IssueOnProduction,";
                    sqlText += " TransactionType";


                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "@nextBOMRawId,";
                    sqlText += "@nextBOMId,";
                    sqlText += "@BOMLineNo,";
                    sqlText += "@bomMasterItemNo,";
                    sqlText += "@BItemRawItemNo,";
                    sqlText += "@BItemRawItemType,";
                    sqlText += "@bomMaterEffectDate,";
                    sqlText += "@bomMaterVATName,";
                    sqlText += "@BItemUseQuantity,";
                    sqlText += "@BItemWastageQuantity,";
                    sqlText += "@BItemCost,";
                    sqlText += "@BItemUOM,";
                    sqlText += "@BItemVATRate,";
                    sqlText += "@BItemVatAmount,";
                    sqlText += "@BItemSD,";
                    sqlText += "@BItemSDAmount,";
                    sqlText += "@BItemTradingMarkUp,";
                    sqlText += "@BItemRebateRate,";
                    sqlText += "@MarkuAmount,";
                    sqlText += "@BItemCreatedBy,";
                    sqlText += "@BItemCreatedOn,";
                    sqlText += "@BItemLastModifiedBy,";
                    sqlText += "@BItemLastModifiedOn,";
                    sqlText += "@BItemUnitCost,";
                    sqlText += "@BItemUOMn,";
                    sqlText += "@BItemUOMc,";
                    sqlText += "@BItemUOMPrice,";
                    sqlText += "@BItemUOMUQty,";
                    sqlText += "@BItemUOMWQty,";
                    sqlText += "@BItemTotalQuantity,";
                    sqlText += "@BItemPBOMId,";
                    sqlText += "@BItemPost,";
                    sqlText += "@BItemPInvoiceNo,";
                    sqlText += "@bomMaterCustomerID,";
                    sqlText += "@BItemIssueOnProduction,";
                    sqlText += "@BItemTransactionType";

                    sqlText += ")	";




                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@nextBOMRawId", nextBOMRawId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@nextBOMId", nextBOMId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BOMLineNo", BOMLineNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemRawItemNo", BItem.RawItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemRawItemType", BItem.RawItemType ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMaterEffectDate", Ordinary.DateToDate(bomMaster.EffectDate));
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMaterVATName", bomMaster.VATName ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUseQuantity", BItem.UseQuantity);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemWastageQuantity", BItem.WastageQuantity);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemCost", BItem.Cost);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOM", BItem.UOM ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemVATRate", BItem.VATRate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemVatAmount", BItem.VatAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemSD", BItem.SD);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemSDAmount", BItem.SDAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemTradingMarkUp", BItem.TradingMarkUp);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemRebateRate", BItem.RebateRate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MarkuAmount", MarkupAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemCreatedBy", BItem.CreatedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemCreatedOn", BItem.CreatedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemLastModifiedBy", BItem.LastModifiedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemLastModifiedOn", BItem.LastModifiedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUnitCost", BItem.UnitCost);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMn", BItem.UOMn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMc", BItem.UOMc);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMPrice", BItem.UOMPrice);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMUQty", BItem.UOMUQty);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMWQty", BItem.UOMWQty);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemTotalQuantity", BItem.TotalQuantity);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemPBOMId", BItem.PBOMId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemPost", BItem.Post ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemPInvoiceNo", BItem.PInvoiceNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMaterCustomerID", bomMaster.CustomerID ?? "0");
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemIssueOnProduction", BItem.IssueOnProduction ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemTransactionType", BItem.TransactionType ?? Convert.DBNull);

                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                        "Price declaration for this item ('" + bomMaster.FinishItemName +
                                                        "') and VAT Name ( '" + bomMaster.VATName +
                                                        "' ) and Material Name ('" + BItem.RawItemName +
                                                        "') not save in date date ('" + bomMaster.EffectDate + "') .");
                    }

                }

                #endregion "@DateTo"

                #endregion Insert Detail Table

                #region Insert BOMCompanyOverhead Table

                if (bomOHs == null)
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                    "Price declaration for this item ('" + bomMaster.FinishItemName +
                                                    "') and VAT Name ( '" + bomMaster.VATName +
                                                    "' ) not save in date date ('" + bomMaster.EffectDate + "') .");
                }


                int OHLineNo = 0;

                foreach (var OHItem in bomOHs.ToList())
                {

                    #region Raw Product Exists

                    //if (OHItem.HeadName.Trim() != "Margin")
                    //{

                    sqlText = "";
                    sqlText = "select count(ItemNo) from Products ";
                    sqlText += " where ProductName =@OHItemHeadName  ";

                    SqlCommand cmdfindRItem = new SqlCommand(sqlText, currConn);
                    cmdfindRItem.Transaction = transaction;
                    cmdfindRItem.Parameters.AddWithValueAndNullHandle("@OHItemHeadName", OHItem.HeadName);

                    int findRItem = (int)cmdfindRItem.ExecuteScalar();

                    if (findRItem <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                        "Price declaration Overhead not in database please check the Overhead Item ('" +
                                                        OHItem.HeadName + "') Code ('" +
                                                        OHItem.OHCode + "') ");
                    }
                    //}
                    //GetProductNo

                    #endregion Raw Product Exists

                    #region Generate Overhead Id

                    sqlText = "";
                    sqlText = "select isnull(max(cast(BOMOverHeadId as int)),0)+1 FROM  BOMCompanyOverhead";
                    SqlCommand cmdGenOHId = new SqlCommand(sqlText, currConn);
                    cmdGenOHId.Transaction = transaction;
                    int nextOHId = (int)cmdGenOHId.ExecuteScalar();

                    if (nextOHId <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                        "Sorry,Unable to generate Overhead Id.");
                    }

                    #endregion Generate Overhead Id

                    #region Find Transaction Exist

                    OHLineNo++;
                    sqlText = "";
                    sqlText += "select COUNT(HeadName) from BOMCompanyOverhead WHERE FinishItemNo=@bomMasterItemNo ";
                    sqlText += " AND HeadName=@OHItemHeadName AND EffectDate=@bomMasterEffectDate  and VATName=@bomMasterVATName ";
                    if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                    { }
                    else
                    {
                        sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                    }
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemHeadName", OHItem.HeadName);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                    IDExist = (int)cmdFindId.ExecuteScalar();
                    if (bomMaster.VATName != "VAT 1 (Tender)")
                    {
                        if (IDExist > 0)
                        {
                            throw new ArgumentNullException("BOM Insert", MessageVM.PurchasemsgFindExistID + ", Material name '(" + OHItem.HeadName + ")'");
                        }
                    }

                    #endregion Find Transaction Exist

                    #region Insert only OH



                    sqlText = "";
                    sqlText += " insert into BOMCompanyOverhead(";
                    sqlText += " BOMOverHeadId,";
                    sqlText += " BOMId,";
                    sqlText += " HeadID,";
                    sqlText += " HeadName,";
                    sqlText += " HeadAmount,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " FinishItemNo,";
                    sqlText += " EffectDate,";
                    sqlText += " OHLineNo,";
                    sqlText += " VATName,";
                    sqlText += " RebatePercent, ";
                    sqlText += " RebateAmount,";
                    sqlText += " AdditionalCost, ";
                    sqlText += " CustomerID, ";
                    sqlText += " Post ";
                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "@nextOHId,";
                    sqlText += "@nextBOMId,";
                    sqlText += "@OHItemHeadID,";
                    sqlText += "@OHItemHeadName,";
                    sqlText += "@OHItemHeadAmount,";
                    sqlText += "@OHItemCreatedBy,";
                    sqlText += "@OHItemCreatedOn,";
                    sqlText += "@OHItemLastModifiedBy,";
                    sqlText += "@OHItemLastModifiedOn,";
                    sqlText += "@bomMasterItemNo,";
                    sqlText += "@bomMasterEffectDate,";
                    sqlText += "@OHLineNo,";
                    sqlText += "@bomMasterVATName,";
                    sqlText += "@OHItemRebatePercent,";
                    sqlText += "@OHItemRebateAmount,";
                    sqlText += "@OHItemAdditionalCost,";
                    sqlText += "@bomMasterCustomerID,";
                    sqlText += "@OHItemPost";

                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@nextOHId", nextOHId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@nextBOMId", nextBOMId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemHeadID", OHItem.HeadID);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemHeadName", OHItem.HeadName);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemHeadAmount", OHItem.HeadAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemCreatedBy", OHItem.CreatedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemCreatedOn", OHItem.CreatedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemLastModifiedBy", OHItem.LastModifiedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemLastModifiedOn", OHItem.LastModifiedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHLineNo", OHLineNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemRebatePercent", OHItem.RebatePercent);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemRebateAmount", OHItem.RebateAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemAdditionalCost", OHItem.AdditionalCost);
                    cmdInsDetail.Parameters.AddWithValue("@bomMasterCustomerID", bomMaster.CustomerID??"0");
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemPost", OHItem.Post);


                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert + "sql" + sqlText,
                                                        "Price declaration for this item ('" + bomMaster.FinishItemName +
                                                        "') and VAT Name ( '" + bomMaster.VATName +
                                                        "' ) and Material Name ('" + OHItem.HeadName +
                                                        "') not save in date date ('" + bomMaster.EffectDate + "') .");
                    }

                    #endregion Insert only OH
                }

                #endregion Insert BOMCompanyOverhead Table

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
                retResults[1] = MessageVM.bomMsgSaveSuccessfully;
                //nextBOMId
                retResults[2] = "" + nextBOMId;
                retResults[3] = "" + PostStatus;

                #endregion SuccessResult
            }
            #endregion Try
            #region Catch and Finall

            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = ""; //catch ex

                transaction.Rollback();


                FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
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

            #endregion Catch and Finall

            #region Result

            return retResults;

            #endregion Result
        }

        public string[] BOMUpdate(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster)
        {

            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = bomMaster.BOMId;


            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            string VATName = "";


            #endregion Initializ

            #region Try

            try
            {


                #region open connection and transaction

                if (bomItems == null && !bomItems.Any())
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    "Sorry,No Item found to insert.");
                }
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction(MessageVM.PurchasemsgMethodNameInsert);

                #endregion open connection and transaction

                #region Fiscal Year Check

                string transactionYearCheck = Convert.ToDateTime(bomMaster.EffectDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue ||
                    Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
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
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                    }

                    #endregion YearLock

                    #region YearNotExist

                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) <
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            ||
                            Convert.ToDateTime(transactionYearCheck) >
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }
                    }

                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK

                #region CheckVATName

                VATName = bomMaster.VATName;
                if (string.IsNullOrEmpty(VATName))
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgVatNameNotFound);

                }

                #endregion CheckVATName

                #region update BOM Table

                if (bomItems == null)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    MessageVM.PurchasemsgNoDataToSave);
                }




                decimal LastNBRPrice = 0;
                decimal LastNBRWithSDAmount = 0;
                decimal LastMarkupAmount = 0;
                decimal LastSDAmount = 0;
                decimal MarkupAmount = bomMaster.MarkupValue;
                int BOMLineNo = 0;

                #region Find Last Declared NBRPrice

                var vFinishItemNo = bomItems.First().FinishItemNo;
                var vEffectDate = bomItems.First().EffectDate;
                sqlText = "";
                sqlText += "select top 1 NBRPrice from BOMs WHERE FinishItemNo='" + vFinishItemNo + "' ";
                sqlText += " AND EffectDate<'" + vEffectDate + "' ";
                sqlText += " AND VATName='" + VATName + "' ";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                }
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastNBRPrice = new SqlCommand(sqlText, currConn);
                cmdFindLastNBRPrice.Transaction = transaction;
                cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                object objLastNBRPrice = cmdFindLastNBRPrice.ExecuteScalar();
                if (objLastNBRPrice != null)
                {
                    LastNBRPrice = Convert.ToDecimal(objLastNBRPrice);
                }

                sqlText = "";
                sqlText += "select top 1 NBRWithSDAmount from BOMs WHERE FinishItemNo='" + vFinishItemNo + "' ";
                sqlText += " AND EffectDate<'" + vEffectDate + "' ";
                sqlText += " AND VATName='" + VATName + "' ";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                }
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastNBRWithSDAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastNBRWithSDAmount.Transaction = transaction;
                cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                object objLastNBRWithSDAmount = cmdFindLastNBRWithSDAmount.ExecuteScalar();
                if (objLastNBRWithSDAmount != null)
                {
                    LastNBRWithSDAmount = Convert.ToDecimal(objLastNBRWithSDAmount);
                }
                sqlText = "";
                sqlText += "select top 1 SDAmount from BOMs WHERE FinishItemNo='" + vFinishItemNo + "' ";
                sqlText += " AND EffectDate<'" + vEffectDate + "' ";
                sqlText += " AND VATName='" + VATName + "' ";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                }
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastSDAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastSDAmount.Transaction = transaction;
                cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                object objLastSDAmount = cmdFindLastSDAmount.ExecuteScalar();
                if (objLastSDAmount != null)
                {
                    LastSDAmount = Convert.ToDecimal(objLastSDAmount);
                }

                sqlText = "";
                sqlText += "select top 1 MarkUpValue from BOMs WHERE FinishItemNo=@vFinishItemNo ";
                sqlText += " AND EffectDate<@vEffectDate ";
                sqlText += " AND VATName=@VATName ";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                }
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastMarkupAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastMarkupAmount.Transaction = transaction;
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@vEffectDate", vEffectDate);
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@VATName", VATName);
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                object objLastMarkupAmount = cmdFindLastMarkupAmount.ExecuteScalar();
                if (objLastMarkupAmount != null)
                {
                    LastMarkupAmount = Convert.ToDecimal(objLastMarkupAmount);
                }


                #endregion Find Last Declared NBRPrice

                bomMaster.LastNBRPrice = LastNBRPrice;
                bomMaster.LastNBRWithSDAmount = LastNBRWithSDAmount;
                bomMaster.LastSDAmount = LastSDAmount;
                bomMaster.LastMarkupValue = LastMarkupAmount;





                #region BOM Master Update

                sqlText = "";

                sqlText += " update BOMs set  ";
                sqlText += " EffectDate=@bomMasterEffectDate,";
                sqlText += " VATName=@bomMasterVATName,";
                sqlText += " VATRate=@bomMasterVATRate ,";
                sqlText += " UOM=@bomMasterUOM,";
                sqlText += " SD=@bomMasterSDRate ,";
                sqlText += " TradingMarkUp=@bomMasterTradingMarkup ,";
                sqlText += " Comments=@bomMasterComments,";
                sqlText += " ActiveStatus=@bomMasterActiveStatus,";
                sqlText += " LastModifiedBy=@bomMasterLastModifiedBy,";
                sqlText += " LastModifiedOn=@bomMasterLastModifiedOn,";
                sqlText += " RawTotal=@bomMasterRawTotal ,";
                sqlText += " PackingTotal=@bomMasterPackingTotal ,";
                sqlText += " RebateTotal=@bomMasterRebateTotal ,";
                sqlText += " AdditionalTotal=@bomMasterAdditionalTotal ,";
                sqlText += " RebateAdditionTotal=@bomMasterRebateAdditionTotal ,";
                sqlText += " NBRPrice=@bomMasterPNBRPrice ,";
                sqlText += " PacketPrice=@bomMasterPPacketPrice ,";
                sqlText += " RawOHCost=@bomMasterRawOHCost ,";
                sqlText += " LastNBRPrice=@bomMasterLastNBRPrice ,";
                sqlText += " LastNBRWithSDAmount=@bomMasterLastNBRWithSDAmount ,";
                sqlText += " TotalQuantity=@bomMasterTotalQuantity, ";
                sqlText += " SDAmount=@bomMasterSDAmount, ";
                sqlText += " VATAmount=@bomMasterVatAmount, ";
                sqlText += " WholeSalePrice=@bomMasterWholeSalePrice, ";
                sqlText += " NBRWithSDAmount=@bomMasterNBRWithSDAmount, ";
                sqlText += " MarkUpValue=@bomMasterMarkupValue, ";
                sqlText += " LastMarkUpValue=@bomMasterLastMarkupValue, ";
                sqlText += " LastSDAmount=@bomMasterLastSDAmount, ";
                sqlText += " LastAmount=@bomMasterLastAmount ";
                sqlText += " where 1=1";
                sqlText += " and FinishItemNo=@bomMasterItemNo";
                sqlText += " and EffectDate=@bomMasterEffectDate";
                sqlText += " AND VATName=@VATName";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                }

                SqlCommand cmdMasterUpdate = new SqlCommand(sqlText, currConn);
                cmdMasterUpdate.Transaction = transaction;

                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName ?? Convert.DBNull);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterVATRate", bomMaster.VATRate);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterUOM", bomMaster.UOM ?? Convert.DBNull);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterSDRate", bomMaster.SDRate);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterTradingMarkup", bomMaster.TradingMarkup);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterComments", bomMaster.Comments ?? Convert.DBNull);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterActiveStatus", bomMaster.ActiveStatus ?? Convert.DBNull);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedBy", bomMaster.LastModifiedBy ?? Convert.DBNull);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedOn", bomMaster.LastModifiedOn);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterRawTotal", bomMaster.RawTotal);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterPackingTotal", bomMaster.PackingTotal);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterRebateTotal", bomMaster.RebateTotal);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterAdditionalTotal", bomMaster.AdditionalTotal);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterRebateAdditionTotal", bomMaster.RebateAdditionTotal);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterPNBRPrice", bomMaster.PNBRPrice);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterPPacketPrice", bomMaster.PPacketPrice);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterRawOHCost", bomMaster.RawOHCost);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterLastNBRPrice", bomMaster.LastNBRPrice);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterLastNBRWithSDAmount", bomMaster.LastNBRWithSDAmount);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterTotalQuantity", bomMaster.TotalQuantity);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterSDAmount", bomMaster.SDAmount);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterVatAmount", bomMaster.VatAmount);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterWholeSalePrice", bomMaster.WholeSalePrice);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterNBRWithSDAmount", bomMaster.NBRWithSDAmount);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterMarkupValue", bomMaster.MarkupValue);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterLastMarkupValue", bomMaster.LastMarkupValue);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterLastSDAmount", bomMaster.LastSDAmount);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterLastAmount", bomMaster.LastAmount);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo ?? Convert.DBNull);
                //cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                transResult = (int)cmdMasterUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                    MessageVM.PurchasemsgUpdateNotSuccessfully);
                }


                #endregion BOM Master Update

                foreach (var BItem in bomItems.ToList())
                {
                    //BOMLineNo++;

                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "select COUNT(FinishItemNo) from BOMRaws WHERE ";

                    sqlText += " FinishItemNo=@bomMasterItemNo ";
                    sqlText += " and EffectDate=@bomMasterEffectDate ";
                    sqlText += " AND VATName=@VATName ";

                    //sqlText += " BOMId ='" + bomMaster.BOMId + "'  ";
                    sqlText += " AND RawItemNo='" + BItem.RawItemNo + "'";
                    if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                    { }
                    else
                    {
                        sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                    }
                    //sqlText += " AND VATName='" + VATName + "' ";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@VATName", VATName);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                    int IDExist1 = (int)cmdFindId.ExecuteScalar();

                    #endregion Find Transaction Exist

                    if (IDExist1 <= 0)
                    {
                        #region Not Exist then Insert


                        #region Generate BOMRaw Id

                        int nextBOMRawId = 0;
                        sqlText = "";
                        sqlText = "select isnull(max(cast(BOMRawId as int)),0)+1 FROM  BOMRaws";
                        SqlCommand cmdGenRawId = new SqlCommand(sqlText, currConn);
                        cmdGenRawId.Transaction = transaction;
                        nextBOMRawId = (int)cmdGenRawId.ExecuteScalar();

                        if (nextBOMRawId <= 0)
                        {
                            throw new ArgumentNullException("BOMUpdate", "Sorry,Unable to generate BOMRawId.");
                        }

                        #endregion Generate BOMRaw Id

                        #region Insert only BOM

                        sqlText = "";
                        sqlText += " insert into BOMRaws(";
                        sqlText += " BOMRawId,";
                        sqlText += " BOMId,";
                        sqlText += " BOMLineNo,";
                        sqlText += " FinishItemNo,";
                        sqlText += " RawItemNo,";
                        sqlText += " RawItemType,";
                        sqlText += " EffectDate,";
                        sqlText += " VATName,";
                        sqlText += " UseQuantity,";
                        sqlText += " WastageQuantity,";
                        sqlText += " Cost,";
                        sqlText += " UOM,";
                        sqlText += " VATRate,";
                        sqlText += " VATAmount,";
                        sqlText += " SD,";
                        sqlText += " SDAmount,";
                        sqlText += " TradingMarkUp,";
                        sqlText += " RebateRate,";

                        sqlText += " MarkUpValue,";
                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn,";
                        sqlText += " UnitCost,";
                        sqlText += " UOMn,";
                        sqlText += " UOMc,";
                        sqlText += " UOMPrice,";
                        sqlText += " UOMUQty,";
                        sqlText += " UOMWQty,";
                        sqlText += " TotalQuantity,";
                        sqlText += " PBOMId,";
                        sqlText += " PInvoiceNo,";
                        sqlText += " Post,";
                        sqlText += " CustomerID,";
                        sqlText += " IssueOnProduction,";
                        sqlText += " TransactionType";



                        sqlText += " )";
                        sqlText += " values(	";
                        sqlText += "@nextBOMRawId,";
                        sqlText += "@bomMasterBOMId,";
                        sqlText += "@BItemBOMLineNo,";
                        sqlText += "@bomMasterItemNo,";
                        sqlText += "@BItemRawItemNo,";
                        sqlText += "@BItemRawItemType,";
                        sqlText += "@bomMasterEffectDate,";
                        sqlText += "@bomMasterVATName,";
                        sqlText += "@BItemUseQuantity,";
                        sqlText += "@BItemWastageQuantity,";
                        sqlText += "@BItemCost,";
                        sqlText += "@BItemUOM,";
                        sqlText += "@BItemVATRate,";
                        sqlText += "@BItemVatAmount,";
                        sqlText += "@BItemSD,";
                        sqlText += "@BItemSDAmount,";
                        sqlText += "@BItemTradingMarkUp,";
                        sqlText += "@BItemRebateRate,";
                        sqlText += "@MarkupAmount,";
                        sqlText += "@BItemCreatedBy,";
                        sqlText += "@BItemCreatedOn,";
                        sqlText += "@BItemLastModifiedBy,";
                        sqlText += "@BItemLastModifiedOn,";
                        sqlText += "@BItemUnitCost,";
                        sqlText += "@BItemUOMn,";
                        sqlText += "@BItemUOMc,";
                        sqlText += "@BItemUOMPrice,";
                        sqlText += "@BItemUOMUQty,";
                        sqlText += "@BItemUOMWQty,";
                        sqlText += "@BItemTotalQuantity,";
                        sqlText += "@BItemPBOMId,";
                        sqlText += "@BItemPInvoiceNo,";
                        sqlText += "@BItemPost,";
                        sqlText += "@bomMasterCustomerID,";
                        sqlText += "@BItemIssueOnProduction,";
                        sqlText += "@BItemTransactionType";

                        sqlText += ")	";

                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@nextBOMRawId", nextBOMRawId);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterBOMId", bomMaster.BOMId ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemBOMLineNo", BItem.BOMLineNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemRawItemNo", BItem.RawItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemRawItemType", BItem.RawItemType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUseQuantity", BItem.UseQuantity);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemWastageQuantity", BItem.WastageQuantity);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemCost", BItem.Cost);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOM", BItem.UOM ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemVATRate", BItem.VATRate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemVatAmount", BItem.VatAmount);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemSD", BItem.SD);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemSDAmount", BItem.SDAmount);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemTradingMarkUp", BItem.TradingMarkUp);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemRebateRate", BItem.RebateRate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MarkupAmount", MarkupAmount);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemCreatedBy", BItem.CreatedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemCreatedOn", BItem.CreatedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemLastModifiedBy", BItem.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemLastModifiedOn", BItem.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUnitCost", BItem.UnitCost);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMn", BItem.UOMn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMc", BItem.UOMc);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMPrice", BItem.UOMPrice);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMUQty", BItem.UOMUQty);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMWQty", BItem.UOMWQty);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemTotalQuantity", BItem.TotalQuantity);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemPBOMId", BItem.PBOMId ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemPInvoiceNo", BItem.PInvoiceNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemPost", BItem.Post);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemIssueOnProduction", BItem.IssueOnProduction ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemTransactionType", BItem.TransactionType ?? Convert.DBNull);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("BOMUpdate", MessageVM.PurchasemsgSaveNotSuccessfully);
                        }

                        #endregion Insert only BOM

                        #endregion Not Exist then Insert


                    }
                    else
                    {
                        #region else Update



                        sqlText = "";

                        sqlText += " update BOMRaws set  ";
                        sqlText += " BOMLineNo          =@BOMLineNo ,";
                        sqlText += " RawItemType        =@BItemRawItemType ,";
                        sqlText += " EffectDate         =@bomMasterEffectDate ,";
                        sqlText += " VATName            =@bomMasterVATName ,";
                        sqlText += " UseQuantity        =@BItemUseQuantity ,";
                        sqlText += " WastageQuantity    =@BItemWastageQuantity ,";
                        sqlText += " Cost               =@BItemCost ,";
                        sqlText += " UOM                =@BItemUOM ,";
                        sqlText += " VATRate            =@BItemVATRate ,";
                        sqlText += " VATAmount          =@BItemVatAmount, ";
                        sqlText += " SD                 =@BItemSD ,";
                        sqlText += " SDAmount           =@BItemSDAmount, ";
                        sqlText += " TradingMarkUp      =@BItemTradingMarkUp ,";
                        sqlText += " RebateRate         =@BItemRebateRate ,";
                        sqlText += " MarkUpValue        =@BItemMarkUpValue ,";
                        sqlText += " LastModifiedBy     =@BItemLastModifiedBy ,";
                        sqlText += " LastModifiedOn     =@BItemLastModifiedOn ,";
                        sqlText += " UnitCost           =@BItemUnitCost ,";
                        sqlText += " UOMn               =@BItemUOMn ,";
                        sqlText += " UOMc               =@BItemUOMc ,";
                        sqlText += " UOMPrice           =@BItemUOMPrice ,";
                        sqlText += " UOMUQty            =@BItemUOMUQty ,";
                        sqlText += " UOMWQty            =@BItemUOMWQty ,";
                        sqlText += " PBOMId             =@BItemPBOMId ,";
                        sqlText += " PInvoiceNo         =@BItemPInvoiceNo ,";
                        sqlText += " TotalQuantity      =@BItemTotalQuantity, ";
                        sqlText += " IssueOnProduction  =@BItemIssueOnProduction, ";
                        sqlText += " TransactionType    =@BItemTransactionType ";

                        sqlText += " WHERE  ";

                        sqlText += " FinishItemNo       =@bomMasterItemNo";
                        sqlText += " and EffectDate     =@bomMasterEffectDate";
                        sqlText += " AND VATName        =@bomMasterVATName ";
                        sqlText += " AND RawItemNo      =@BItemRawItemNo ";

                        if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                        { }
                        else
                        {
                            sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                        }

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Transaction = transaction;

                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BOMLineNo", BOMLineNo);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemRawItemType", BItem.RawItemType ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemUseQuantity", BItem.UseQuantity);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemWastageQuantity", BItem.WastageQuantity);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemCost", BItem.Cost);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemUOM", BItem.UOM ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemVATRate", BItem.VATRate);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemVatAmount", BItem.VatAmount);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemSD", BItem.SD);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemSDAmount", BItem.SDAmount);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemTradingMarkUp", BItem.TradingMarkUp);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemRebateRate", BItem.RebateRate);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemMarkUpValue", BItem.MarkUpValue);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemLastModifiedBy", BItem.LastModifiedBy ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemLastModifiedOn", BItem.LastModifiedOn);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemUnitCost", BItem.UnitCost);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemUOMn", BItem.UOMn ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemUOMc", BItem.UOMc);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemUOMPrice", BItem.UOMPrice);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemUOMUQty", BItem.UOMUQty);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemUOMWQty", BItem.UOMWQty);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemPBOMId", BItem.PBOMId ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemPInvoiceNo", BItem.PInvoiceNo ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemTotalQuantity", BItem.TotalQuantity);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemIssueOnProduction", BItem.IssueOnProduction);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemTransactionType", BItem.TransactionType ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemRawItemNo", BItem.RawItemNo ?? Convert.DBNull);

                        transResult = (int)cmdUpdate.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                            MessageVM.PurchasemsgUpdateNotSuccessfully);
                        }



                        #endregion else Update


                    }




                }

                #region Remove row at BOMRaws

                sqlText = "";
                sqlText += " SELECT  distinct RawItemNo";
                sqlText += " from BOMRaws WHERE  ";
                sqlText += " FinishItemNo='" + bomMaster.ItemNo + "' ";
                sqlText += " and EffectDate='" + bomMaster.EffectDate + "'";
                sqlText += " AND VATName='" + bomMaster.VATName + "' ";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)='" + bomMaster.CustomerID + "' ";
                }
                DataTable dt = new DataTable("Previous");
                SqlCommand cmdRIF = new SqlCommand(sqlText, currConn);
                cmdRIF.Transaction = transaction;
                SqlDataAdapter dta = new SqlDataAdapter(cmdRIF);
                dta.Fill(dt);
                foreach (DataRow pItem in dt.Rows)
                {
                    var p = pItem["RawItemNo"].ToString();

                    //var tt= Details.Find(x => x.ItemNo == p);
                    var tt = bomItems.Count(x => x.RawItemNo.Trim() == p.Trim());
                    if (tt == 0)
                    {
                        sqlText = "";
                        sqlText += " delete FROM BOMRaws ";
                        sqlText += " WHERE BOMId =@bomMasterBOMId";
                        sqlText += " AND RawItemNo=@p";
                        if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                        { }
                        else
                        {
                            sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                        }
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterBOMId", bomMaster.BOMId);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@p", p);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    }

                }

                #endregion Remove row at BOMRaws

                #endregion update BOM Table

                #region update BOMCompanyOverhead Table

                if (bomOHs == null)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    MessageVM.PurchasemsgNoDataToSave);
                }
                int OHLineNo = 0;
                foreach (var OHItem in bomOHs.ToList())
                {


                    OHLineNo++;

                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "select COUNT(HeadName) from BOMCompanyOverhead WHERE ";

                    sqlText += " FinishItemNo=@bomMasterItemNo";
                    sqlText += " and EffectDate=@bomMasterEffectDate";
                    sqlText += " AND VATName=@bomMasterVATName";
                    if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                    { }
                    else
                    {
                        sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                    }
                    //sqlText += " BOMId ='" + bomMaster.BOMId + "'  ";

                    sqlText += " AND HeadID=@OHItemHeadID";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemHeadID", OHItem.HeadID);

                    int IDExist1 = (int)cmdFindId.ExecuteScalar();

                    #endregion Find Transaction Exist

                    if (IDExist1 <= 0)
                    {
                        #region Generate Overhead Id

                        sqlText = "";
                        sqlText = "select isnull(max(cast(BOMOverHeadId as int)),0)+1 FROM  BOMCompanyOverhead";
                        SqlCommand cmdGenOHId = new SqlCommand(sqlText, currConn);
                        cmdGenOHId.Transaction = transaction;
                        int nextOHId = (int)cmdGenOHId.ExecuteScalar();

                        if (nextOHId <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            "Sorry,Unable to generate Overhead Id.");
                        }

                        #endregion Generate Overhead Id

                        #region Not Exist then Insert

                        #region Insert only OH

                        decimal vRebateAmount = 0;
                        decimal vAddingAmount = 0;


                        sqlText = "";
                        sqlText += " insert into BOMCompanyOverhead(";
                        sqlText += " BOMOverHeadId,";
                        sqlText += " BOMId,";
                        sqlText += " HeadID,";
                        sqlText += " HeadName,";
                        sqlText += " HeadAmount,";
                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn,";
                        sqlText += " FinishItemNo,";
                        sqlText += " EffectDate,";
                        sqlText += " OHLineNo,";
                        sqlText += " VATName,";
                        sqlText += " RebatePercent, ";
                        sqlText += " RebateAmount,";
                        sqlText += " AdditionalCost, ";
                        sqlText += " CustomerID, ";
                        sqlText += " Post ";
                        sqlText += " )";
                        sqlText += " values(	";
                        sqlText += "@nextOHId,";
                        sqlText += "@bomMasterBOMId,";
                        sqlText += "@OHItemHeadID,";
                        sqlText += "@OHItemHeadName,";
                        sqlText += "@OHItemHeadAmount,";
                        sqlText += "@OHItemCreatedBy,";
                        sqlText += "@OHItemCreatedOn,";
                        sqlText += "@OHItemLastModifiedBy,";
                        sqlText += "@OHItemLastModifiedOn,";
                        sqlText += "@OHItemFinishItemNo,";
                        sqlText += "@OHItemEffectDate,";
                        sqlText += "@OHLineNo,";
                        sqlText += "@VATName,";
                        sqlText += "@OHItemRebatePercent,";
                        sqlText += "@OHItemRebateAmount,";
                        sqlText += "@OHItemAdditionalCost,";
                        sqlText += "@bomMasterCustomerID,";
                        sqlText += "@OHItemPost";

                        sqlText += ")	";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@nextOHId", nextOHId);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterBOMId", bomMaster.BOMId);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemHeadID", OHItem.HeadID);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemHeadName", OHItem.HeadName);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemHeadAmount", OHItem.HeadAmount);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemCreatedBy", OHItem.CreatedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemCreatedOn", OHItem.CreatedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemLastModifiedBy", OHItem.LastModifiedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemLastModifiedOn", OHItem.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemFinishItemNo", OHItem.FinishItemNo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHLineNo", OHLineNo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VATName", VATName);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemRebatePercent", OHItem.RebatePercent);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemRebateAmount", OHItem.RebateAmount);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemAdditionalCost", OHItem.AdditionalCost);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemPost", OHItem.Post);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.PurchasemsgSaveNotSuccessfully);
                        }

                        #endregion Insert only OH

                        #endregion Not Exist then Insert
                    }
                    else
                    {
                        #region else Update

                        sqlText = "";

                        sqlText += " update BOMCompanyOverhead set  ";
                        sqlText += " OHLineNo           = @OHLineNo  ,";
                        sqlText += " EffectDate         = @OHItemEffectDate  ,";
                        sqlText += " VATName            = @VATName  ,";
                        sqlText += " HeadName           = @OHItemHeadName  ,";
                        sqlText += " HeadAmount         = @OHItemHeadAmount  ,";
                        sqlText += " LastModifiedBy     = @OHItemLastModifiedBy  ,";
                        sqlText += " LastModifiedOn     = @OHItemLastModifiedOn , ";
                        sqlText += " RebatePercent      = @OHItemRebatePercent  ,";
                        sqlText += " RebateAmount       = @OHItemRebateAmount , ";
                        sqlText += " AdditionalCost     = @OHItemAdditionalCost  ";
                        sqlText += " where  HeadID      = @OHItemHeadID ";
                        sqlText += " and FinishItemNo   = @OHItemFinishItemNo ";
                        sqlText += " and EffectDate     = @OHItemEffectDate ";
                        sqlText += " and VATName        = @VATName ";
                        if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                        { }
                        else
                        {
                            sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                        }

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Transaction = transaction;

                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHLineNo", OHLineNo);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHItemEffectDate", OHItem.EffectDate);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@VATName", VATName ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHItemHeadName", OHItem.HeadName ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHItemHeadAmount", OHItem.HeadAmount);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHItemLastModifiedBy", OHItem.LastModifiedBy ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHItemLastModifiedOn", OHItem.LastModifiedOn);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHItemRebatePercent", OHItem.RebatePercent);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHItemRebateAmount", OHItem.RebateAmount);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHItemAdditionalCost", OHItem.AdditionalCost);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHItemHeadID", OHItem.HeadID ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHItemFinishItemNo", OHItem.FinishItemNo ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID ", bomMaster.CustomerID ?? Convert.DBNull);

                        transResult = (int)cmdUpdate.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                            MessageVM.PurchasemsgUpdateNotSuccessfully);
                        }

                        #endregion else Update
                    }


                }

                #endregion update BOMCompanyOverhead Table

                #region Remove row at BOMCompanyOverhead

                sqlText = "";
                sqlText += " SELECT  distinct HeadID";
                sqlText += " from BOMCompanyOverhead WHERE  ";
                sqlText += " FinishItemNo=@bomMasterItemNo  ";
                sqlText += " and EffectDate=@bomMasterEffectDate ";
                sqlText += " AND VATName=@bomMasterVATName  ";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID  ";
                }
                dt = new DataTable("Previous");
                cmdRIF = new SqlCommand(sqlText, currConn);
                cmdRIF.Transaction = transaction;
                cmdRIF.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdRIF.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdRIF.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdRIF.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                dta = new SqlDataAdapter(cmdRIF);
                dta.Fill(dt);
                foreach (DataRow pHeadID in dt.Rows)
                {
                    var p = pHeadID["HeadID"].ToString();
                    var tt = bomOHs.Count(x => x.HeadID.Trim() == p.Trim());
                    if (tt == 0)
                    {
                        sqlText = "";
                        sqlText += " delete FROM BOMCompanyOverhead WHERE ";

                        sqlText += " FinishItemNo=@bomMasterItemNo  ";
                        sqlText += " and EffectDate=@bomMasterEffectDate ";
                        sqlText += " AND VATName=@bomMasterVATName  ";

                        //sqlText += " WHERE BOMId =@bomMaster.BOMId   ";
                        sqlText += " AND HeadID=@p ";
                        if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                        { }
                        else
                        {
                            sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                        }
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@p", p);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);
                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    }

                }

                #endregion Remove row at BOMCompanyOverhead

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
                retResults[1] = MessageVM.bomMsgUpdateSuccessfully;

                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            //catch (SqlException sqlex)
            //{
            //    transaction.Rollback();
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public string[] BOMPost(BOMNBRVM bomMaster)
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
            int countId = 0;
            string sqlText = "";



            #endregion Initializ

            #region Try

            try
            {
                #region open connection and transaction

                //if (bomItems == null && !bomItems.Any())
                //{
                //    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, "Sorry,No Item found to insert.");
                //}
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("BOMRaws", "PBOMId", "varchar(20)", currConn);

                transaction = currConn.BeginTransaction(MessageVM.PurchasemsgMethodNameInsert);

                #endregion open connection and transaction

                #region Fiscal Year Check

                /*Checking existance of provided bank Id information*/

                #region Fiscal Year Check

                string transactionDate = bomMaster.EffectDate;
                string transactionYearCheck = Convert.ToDateTime(bomMaster.EffectDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue ||
                    Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
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
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
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
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) <
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            ||
                            Convert.ToDateTime(transactionYearCheck) >
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }
                    }

                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK


                #endregion Fiscal Year Check

                #region CheckVATName

                if (string.IsNullOrEmpty(bomMaster.VATName))
                {
                    throw new ArgumentNullException("BOMPost", MessageVM.msgVatNameNotFound);

                }

                #endregion CheckVATName

                #region Checking other BOM after this date

                sqlText = "";
                sqlText = "select count(bomid) from boms ";
                sqlText += " where  ";
                sqlText += " FinishItemNo=@bomMasterItemNo ";
                sqlText += " and EffectDate=@bomMasterEffectDate ";
                sqlText += " AND VATName=@bomMasterVATName  ";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                {
                    sqlText += " AND isnull(CustomerId,0)='0' ";

                }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";

                }

                //sqlText += " and effectdate>'" + bomMaster.EffectDate + "'";
                sqlText += " and Post='Y'";
                //sqlText += " and VATName='" + bomMaster.VATName + "'";

                SqlCommand cmdOtherBom = new SqlCommand(sqlText, currConn);
                cmdOtherBom.Transaction = transaction;
                cmdOtherBom.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdOtherBom.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdOtherBom.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdOtherBom.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                int otherBom = (int)cmdOtherBom.ExecuteScalar();

                if (otherBom > 0)
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                    "Sorry,You cannot update this price declaration. Another declaration exist after this.");
                }

                #endregion Checking other BOM after this date

                #region Find Transaction Exist

                sqlText = "";
                sqlText += "select COUNT(FinishItemNo) from BOMs ";
                sqlText += " where  ";
                sqlText += " FinishItemNo=@bomMasterItemNo ";
                sqlText += " and EffectDate=@bomMasterEffectDate ";
                sqlText += " AND VATName=@bomMasterVATName ";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                {
                    sqlText += " AND isnull(CustomerId,0)='0' ";
                }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                }
                sqlText += " and Post='Y'";
                SqlCommand cmdFindBOMId = new SqlCommand(sqlText, currConn);
                cmdFindBOMId.Transaction = transaction;
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                int IDExist = (int)cmdFindBOMId.ExecuteScalar();
                if (bomMaster.VATName != "VAT 1 (Tender)")
                {
                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                        "Price declaration for this item ('" + bomMaster.FinishItemName +
                                                        "') already exist in same date  ('" + bomMaster.EffectDate +
                                                        "') .");
                    }
                }

                #endregion Find Transaction Exist

                #region Update Post

                sqlText = "";
                sqlText += " update BOMs set";
                sqlText += " LastModifiedBy= @bomMasterLastModifiedBy ,";
                sqlText += " LastModifiedOn= @bomMasterLastModifiedOn , ";
                sqlText += " Post='Y'";

                sqlText += "  where FinishItemNo=@bomMasterItemNo ";
                sqlText += " AND EffectDate=@bomMasterEffectDate ";
                sqlText += " AND VATName=@bomMasterVATName";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                {
                    sqlText += " AND isnull(CustomerId,0)='0' ";
                }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@ bomMasterCustomerID ";
                }
                SqlCommand cmdMaster = new SqlCommand(sqlText, currConn);
                cmdMaster.Transaction = transaction;
                cmdMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedBy", bomMaster.LastModifiedBy);
                cmdMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedOn", bomMaster.LastModifiedOn);
                cmdMaster.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdMaster.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdMaster.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdMaster.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                transResult = (int)cmdMaster.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException("BOMPost", MessageVM.msgUpdateNBRPrice);
                }
                sqlText = "";
                sqlText += " update BOMRaws set";
                sqlText += " LastModifiedBy= @bomMasterLastModifiedBy ,";
                sqlText += " LastModifiedOn= @bomMasterLastModifiedOn, ";
                sqlText += " Post='Y'";
                sqlText += "  where FinishItemNo=@bomMasterItemNo";
                sqlText += " AND EffectDate=@bomMasterEffectDate ";
                sqlText += " AND VATName=@bomMasterVATName ";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                {
                    sqlText += " AND isnull(CustomerId,0)='0' ";

                }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                }
                //sqlText += "  where FinishItemNo='" + bomMaster.ItemNo";
                //sqlText += " AND EffectDate='" + bomMaster.EffectDate.Date ";
                //sqlText += " AND VATName='" + bomMaster.VATName ";
                SqlCommand cmdRaws = new SqlCommand(sqlText, currConn);
                cmdRaws.Transaction = transaction;
                cmdRaws.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedBy", bomMaster.LastModifiedBy);
                cmdRaws.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedOn", bomMaster.LastModifiedOn);
                cmdRaws.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdRaws.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdRaws.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdRaws.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);
                transResult = (int)cmdRaws.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException("BOMPost", MessageVM.msgUpdateNBRPrice);
                }
                sqlText = "";
                sqlText += " update BOMCompanyOverhead set";
                sqlText += " LastModifiedBy= @bomMasterLastModifiedBy ,";
                sqlText += " LastModifiedOn= @bomMasterLastModifiedOn, ";
                sqlText += " Post='Y'";
                sqlText += "  where FinishItemNo=@bomMasterItemNo";
                sqlText += " AND EffectDate=@bomMasterEffectDate ";
                sqlText += " AND VATName=@bomMasterVATName ";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                {
                    sqlText += " AND isnull(CustomerId,0)='0' ";
                }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                }
                //sqlText += "  where FinishItemNo='" + bomMaster.ItemNo + "'";
                //sqlText += " AND EffectDate='" + bomMaster.EffectDate.Date + "' ";
                //sqlText += " AND VATName='" + bomMaster.VATName + "' ";
                SqlCommand cmdOverheads = new SqlCommand(sqlText, currConn);
                cmdOverheads.Transaction = transaction;
                cmdOverheads.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedBy", bomMaster.LastModifiedBy);
                cmdOverheads.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedOn", bomMaster.LastModifiedOn);
                cmdOverheads.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdOverheads.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdOverheads.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdOverheads.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                transResult = (int)cmdOverheads.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException("BOMPost", MessageVM.msgUpdateNBRPrice);
                }

                #endregion Update Post

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
                retResults[1] = "Data successfully post.";
                retResults[2] = "" + bomMaster.BOMId;
                //retResults[3] = "" + PostStatus;

                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public string[] ServiceInsert(List<BOMNBRVM> Details)
        {
            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            string VATName = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            int IDExist = 0;
            int BOMLineNo = 0;
            int nextBOMId = 0;
            string savedBOM = string.Empty;
            DataTable BOMDT = new DataTable("BOM");

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
                transaction = currConn.BeginTransaction(MessageVM.spMsgMethodNameInsert);

                #endregion open connection and transaction

                #region Insert into Details(Insert complete in Header)

                #region Validation for Detail

                if (Details.Count() <= 0)
                {
                    throw new ArgumentNullException(MessageVM.spMsgMethodNameInsert, MessageVM.spMsgNoDataToSave);
                }



                #endregion Validation for Detail

                #region Insert Detail Table

                foreach (var Item in Details.ToList())
                {


                    #region Fiscal Year Check

                    string transactionDate = Item.EffectDate;
                    string transactionYearCheck = Convert.ToDateTime(Item.EffectDate).ToString("yyyy-MM-dd");
                    if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue ||
                        Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                    {

                        #region YearLock

                        sqlText = "";

                        sqlText +=
                            "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                            " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                        DataTable dataTable = new DataTable("ProductDataT");
                        SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                        cmdIdExist.Transaction = transaction;
                        SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                        reportDataAdapt.Fill(dataTable);

                        if (dataTable == null)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }

                        else if (dataTable.Rows.Count <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                        else
                        {
                            if (dataTable.Rows[0]["MLock"].ToString() != "N")
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearisLock);
                            }
                            else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearisLock);
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
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }

                        else if (dtYearNotExist.Rows.Count < 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }
                        else
                        {
                            if (Convert.ToDateTime(transactionYearCheck) <
                                Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                                ||
                                Convert.ToDateTime(transactionYearCheck) >
                                Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearNotExist);
                            }
                        }

                        #endregion YearNotExist

                    }


                    #endregion Fiscal Year CHECK

                #endregion Validation for Fiscal Year

                    #region CheckVATName

                    VATName = Item.VATName;
                    if (string.IsNullOrEmpty(VATName))
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgVatNameNotFound);

                    }

                    #endregion CheckVATName

                    #region Insert BOM Table


                    decimal LastNBRPrice = 0;
                    decimal LastNBRWithSDAmount = 0;
                    decimal LastMarkupAmount = 0;
                    decimal LastSDAmount = 0;

                    #region Find Last Declared NBRPrice

                    var vFinishItemNo = Item.ItemNo;
                    var vEffectDate = Item.EffectDate;
                    sqlText = "";
                    sqlText += "select top 1 isnull(NBRPrice,0) from BOMs WHERE FinishItemNo=@vFinishItemNo ";
                    sqlText += " AND EffectDate<@vEffectDate";
                    sqlText += " AND VATName=@VATName";

                    sqlText += " order by EffectDate desc";
                    SqlCommand cmdFindLastNBRPrice = new SqlCommand(sqlText, currConn);
                    cmdFindLastNBRPrice.Transaction = transaction;
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@vEffectDate", vEffectDate);
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@VATName", VATName);


                    object objLastNBRPrice = cmdFindLastNBRPrice.ExecuteScalar();
                    if (objLastNBRPrice != null)
                    {
                        LastNBRPrice = Convert.ToDecimal(objLastNBRPrice);
                    }

                    sqlText = "";
                    sqlText += "select top 1 isnull(NBRWithSDAmount,0) from BOMs WHERE FinishItemNo=@vFinishItemNo ";
                    sqlText += " AND EffectDate<@vEffectDate";
                    sqlText += " AND VATName=@VATName ";
                    sqlText += " order by EffectDate desc";
                    SqlCommand cmdFindLastNBRWithSDAmount = new SqlCommand(sqlText, currConn);
                    cmdFindLastNBRWithSDAmount.Transaction = transaction;
                    cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@vEffectDate", vEffectDate);
                    cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    object objLastNBRWithSDAmount = cmdFindLastNBRWithSDAmount.ExecuteScalar();
                    if (objLastNBRWithSDAmount != null)
                    {
                        LastNBRWithSDAmount = Convert.ToDecimal(objLastNBRWithSDAmount);
                    }
                    sqlText = "";
                    sqlText += "select top 1 isnull(SDAmount,0) from BOMs WHERE FinishItemNo=@vFinishItemNo";
                    sqlText += " AND EffectDate<@vEffectDate";
                    sqlText += " AND VATName=@VATName ";
                    sqlText += " order by EffectDate desc";
                    SqlCommand cmdFindLastSDAmount = new SqlCommand(sqlText, currConn);
                    cmdFindLastSDAmount.Transaction = transaction;
                    cmdFindLastSDAmount.Transaction = transaction;
                    cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@vEffectDate", vEffectDate);
                    cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    object objLastSDAmount = cmdFindLastSDAmount.ExecuteScalar();
                    if (objLastSDAmount != null)
                    {
                        LastSDAmount = Convert.ToDecimal(objLastSDAmount);
                    }

                    sqlText = "";
                    sqlText += "select top 1 MarkUpValue from BOMs WHERE FinishItemNo=@vFinishItemNo";
                    sqlText += " AND EffectDate<@vEffectDate";
                    sqlText += " AND VATName=@VATName ";
                    sqlText += " order by EffectDate desc";
                    SqlCommand cmdFindLastMarkupAmount = new SqlCommand(sqlText, currConn);
                    cmdFindLastMarkupAmount.Transaction = transaction;
                    cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@vEffectDate", vEffectDate);
                    cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    object objLastMarkupAmount = cmdFindLastMarkupAmount.ExecuteScalar();
                    if (objLastMarkupAmount != null)
                    {
                        LastMarkupAmount = Convert.ToDecimal(objLastMarkupAmount);
                    }

                    #endregion Find Last Declared NBRPrice

                    ////////////////////

                    #region Find Transaction Exist

                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "select COUNT(FinishItemNo) from BOMs WHERE FinishItemNo=@vFinishItemNo  ";
                    sqlText += " AND EffectDate=@vEffectDate ";
                    sqlText += " AND VATName=@VATName ";
                    SqlCommand cmdFindBOMId = new SqlCommand(sqlText, currConn);
                    cmdFindBOMId.Transaction = transaction;
                    cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@vEffectDate", vEffectDate);
                    cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@VATName", VATName);


                    IDExist = (int)cmdFindBOMId.ExecuteScalar();

                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        "Price declaration for this item already exist in same date.");
                    }

                    #endregion Find Transaction Exist

                    #endregion Find Transaction Exist

                    #region Generate BOMId

                    sqlText = "";
                    sqlText = "select isnull(max(cast(BOMId as int)),0)+1 FROM  BOMs";
                    SqlCommand cmdGenId = new SqlCommand(sqlText, currConn);
                    cmdGenId.Transaction = transaction;
                    nextBOMId = (int)cmdGenId.ExecuteScalar();

                    if (nextBOMId <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        "Sorry,Unable to generate BOMId.");
                    }

                    #endregion Generate BOMId

                    #region Insert only BOM


                    Item.LastNBRPrice = LastNBRPrice;
                    Item.LastNBRWithSDAmount = LastNBRWithSDAmount;
                    Item.LastSDAmount = LastSDAmount;
                    Item.LastMarkupValue = LastMarkupAmount;


                    sqlText = "";
                    sqlText += " insert into BOMs(";
                    sqlText += " BOMId,";
                    sqlText += " FinishItemNo,";
                    sqlText += " EffectDate,";
                    sqlText += " VATName,";
                    sqlText += " VATRate,";
                    sqlText += " UOM,";
                    sqlText += " SD,";
                    sqlText += " TradingMarkUp,";
                    sqlText += " Comments,";
                    sqlText += " ActiveStatus,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " RawTotal,";
                    sqlText += " PackingTotal,";
                    sqlText += " RebateTotal,";
                    sqlText += " AdditionalTotal,";
                    sqlText += " RebateAdditionTotal,";
                    sqlText += " NBRPrice,";
                    sqlText += " Packetprice,";
                    sqlText += " RawOHCost,";
                    sqlText += " LastNBRPrice,";
                    sqlText += " LastNBRWithSDAmount,";
                    sqlText += " TotalQuantity,";
                    sqlText += " SDAmount,";
                    sqlText += " VATAmount,";
                    sqlText += " WholeSalePrice,";
                    sqlText += " NBRWithSDAmount,";
                    sqlText += " MarkUpValue,";
                    sqlText += " LastMarkUpValue,";
                    sqlText += " LastSDAmount,";
                    sqlText += " LastAmount,";
                    sqlText += " Post";

                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "@nextBOMId,";
                    sqlText += "@ItemItemNo,";
                    sqlText += "@ItemEffectDate,";
                    sqlText += "@ItemVATName,";
                    sqlText += "@ItemVATRate,";
                    sqlText += "@ItemUOM,";
                    sqlText += "@ItemSDRate,";
                    sqlText += "@ItemTradingMarkup,";
                    sqlText += "@ItemComments,";
                    sqlText += "@ItemActiveStatus,";
                    sqlText += "@ItemCreatedBy,";
                    sqlText += "@ItemCreatedOn,";
                    sqlText += "@ItemLastModifiedBy,";
                    sqlText += "@ItemLastModifiedOn,";
                    sqlText += "@ItemRawTotal,";
                    sqlText += "@ItemPackingTotal,";
                    sqlText += "@ItemRebateTotal,";
                    sqlText += "@ItemAdditionalTotal,";
                    sqlText += "@ItemRebateAdditionTotal,";
                    sqlText += "@ItemPNBRPrice,";
                    sqlText += "@ItemPPacketPrice,";
                    sqlText += "@ItemRawOHCost,";
                    sqlText += "@LastNBRPrice,";
                    sqlText += "@LastNBRWithSDAmount,";
                    sqlText += "@ItemTotalQuantity,";
                    sqlText += "@ItemSDAmount,";
                    sqlText += "@ItemVatAmount,";
                    sqlText += "@ItemWholeSalePrice,";
                    sqlText += "@ItemNBRWithSDAmount,";
                    sqlText += "@ItemMarkupValue,";
                    sqlText += "@ItemLastMarkupValue,";
                    sqlText += "@LastSDAmount,";
                    sqlText += "@ItemLastSDAmount,";
                    sqlText += "@ItemPost";


                    sqlText += ")	";




                    SqlCommand cmdInsMaster = new SqlCommand(sqlText, currConn);
                    cmdInsMaster.Transaction = transaction;
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@nextBOMId", nextBOMId);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", Item.EffectDate);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemVATName", Item.VATName ?? Convert.DBNull);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemVATRate", Item.VATRate);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemUOM", Item.UOM ?? Convert.DBNull);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemSDRate", Item.SDRate);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemTradingMarkup", Item.TradingMarkup);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemComments", Item.Comments ?? Convert.DBNull);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemActiveStatus", Item.ActiveStatus);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemCreatedBy", Item.CreatedBy ?? Convert.DBNull);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemCreatedOn", Item.CreatedOn);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedBy", Item.LastModifiedBy ?? Convert.DBNull);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedOn", Item.LastModifiedOn);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemRawTotal", Item.RawTotal);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemPackingTotal", Item.PackingTotal);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemRebateTotal", Item.RebateTotal);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemAdditionalTotal", Item.AdditionalTotal);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemRebateAdditionTotal", Item.RebateAdditionTotal);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemPNBRPrice", Item.PNBRPrice);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemPPacketPrice", Item.PPacketPrice);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemRawOHCost", Item.RawOHCost);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@LastNBRPrice", LastNBRPrice);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@LastNBRWithSDAmount", LastNBRWithSDAmount);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemTotalQuantity", Item.TotalQuantity);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemSDAmount", Item.SDAmount);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemVatAmount", Item.VatAmount);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemWholeSalePrice", Item.WholeSalePrice);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemNBRWithSDAmount", Item.NBRWithSDAmount);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemMarkupValue", Item.MarkupValue);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemLastMarkupValue", Item.LastMarkupValue);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@LastSDAmount", LastSDAmount);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemLastSDAmount", Item.LastSDAmount);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemPost", Item.Post);

                    transResult = (int)cmdInsMaster.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.PurchasemsgSaveNotSuccessfully);
                    }

                    #endregion Insert only BOM

                    savedBOM = savedBOM + nextBOMId + "','";
                }
                savedBOM = savedBOM.Substring(0, savedBOM.Length - 3);

                    #endregion Insert Detail Table

                #endregion Insert into Details(Insert complete in Header)

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
                retResults[1] = MessageVM.spMsgSaveSuccessfully;
                retResults[2] = savedBOM;

                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            //catch (Exception sqlex)
            //{
            //    transaction.Rollback();
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public string[] ServiceUpdate(List<BOMNBRVM> Details)
        {
            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";


            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            string VATName = "";

            #endregion Initializ

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.spMsgMethodNameUpdate);

                #endregion open connection and transaction

                #region Validation for Detail

                if (Details.Count() <= 0)
                {
                    throw new ArgumentNullException(MessageVM.spMsgMethodNameUpdate, MessageVM.spMsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {

                    #region Validation for Fiscal Year

                    #region Fiscal Year Check

                    string transactionYearCheck = Convert.ToDateTime(Item.EffectDate).ToString("yyyy-MM-dd");
                    if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue ||
                        Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                    {

                        #region YearLock

                        sqlText = "";

                        sqlText +=
                            "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                            " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                        DataTable dataTable = new DataTable("ProductDataT");
                        SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                        cmdIdExist.Transaction = transaction;
                        SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                        reportDataAdapt.Fill(dataTable);

                        if (dataTable == null)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }

                        else if (dataTable.Rows.Count <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                        else
                        {
                            if (dataTable.Rows[0]["MLock"].ToString() != "N")
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearisLock);
                            }
                            else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearisLock);
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
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }

                        else if (dtYearNotExist.Rows.Count < 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }
                        else
                        {
                            if (Convert.ToDateTime(transactionYearCheck) <
                                Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                                ||
                                Convert.ToDateTime(transactionYearCheck) >
                                Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearNotExist);
                            }
                        }

                        #endregion YearNotExist

                    }


                    #endregion Fiscal Year CHECK

                    #endregion Validation for Fiscal Year

                    #region CheckVATName

                    VATName = Item.VATName;
                    var vFinishItemNo = Item.ItemNo;
                    var vEffectDate = Item.EffectDate;
                    if (string.IsNullOrEmpty(VATName))
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgVatNameNotFound);

                    }

                    #endregion CheckVATName

                    #region update BOM Table

                    if (Item == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.PurchasemsgNoDataToSave);
                    }

                    #region Checking bom exist

                    sqlText = "";
                    sqlText = "select count(bomid) from boms ";
                    sqlText += " where FinishItemNo =@ItemItemNo ";
                    sqlText += " and effectdate=@ItemEffectDate ";
                    sqlText += " and VATName=@VATName";

                    SqlCommand cmdIsExist = new SqlCommand(sqlText, currConn);
                    cmdIsExist.Transaction = transaction;
                    cmdIsExist.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                    cmdIsExist.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", Item.EffectDate);
                    cmdIsExist.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    int isExist = (int)cmdIsExist.ExecuteScalar();

                    if (isExist <= 0)
                    {
                        throw new ArgumentNullException("BOMUpdate",
                                                        "Update not completed, Please save fitst of this item: '" +
                                                        Item.ItemNo + "'");
                    }

                    #endregion Checking other BOM after this date

                    #region Checking other BOM after this date

                    sqlText = "";
                    sqlText = "select count(bomid) from boms ";
                    sqlText += " where FinishItemNo =@Item.ItemNo ";
                    sqlText += " and effectdate>@Item.EffectDate ";
                    sqlText += " and VATName=@VATName ";
                    sqlText += " and Post='Y'";

                    SqlCommand cmdIsPosted = new SqlCommand(sqlText, currConn);
                    cmdIsPosted.Transaction = transaction;
                    cmdIsPosted.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                    cmdIsPosted.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", Item.EffectDate);
                    cmdIsPosted.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    int isPosted = (int)cmdIsPosted.ExecuteScalar();

                    if (isPosted > 0)
                    {
                        throw new ArgumentNullException("BOMUpdate", "Data already posted.You cannot update this data.");
                    }

                    #endregion Checking other BOM after this date

                    #region Checking other BOM after this date

                    sqlText = "";
                    sqlText = "select count(bomid) from boms ";
                    sqlText += " where FinishItemNo =@Item.ItemNo  ";
                    sqlText += " and effectdate>@Item.EffectDate ";
                    sqlText += " and VATName=@VATName ";

                    SqlCommand cmdOtherBom = new SqlCommand(sqlText, currConn);
                    cmdOtherBom.Transaction = transaction;
                    cmdOtherBom.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                    cmdOtherBom.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", Item.EffectDate);
                    cmdOtherBom.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    int otherBom = (int)cmdOtherBom.ExecuteScalar();

                    if (otherBom > 0)
                    {
                        throw new ArgumentNullException("BOMUpdate",
                                                        "Sorry,You cannot update this price declaration. Another declaration exist after this.");
                    }

                    #endregion Checking other BOM after this date

                    decimal LastNBRPrice = 0;
                    decimal LastNBRWithSDAmount = 0;
                    decimal LastMarkupAmount = 0;
                    decimal LastSDAmount = 0;

                    #region Find Last Declared NBRPrice


                    sqlText = "";
                    sqlText += "select top 1 isnull(NBRPrice,0) from BOMs WHERE FinishItemNo=@vFinishItemNo";
                    sqlText += " AND EffectDate<@vEffectDate ";
                    sqlText += " AND VATName=@VATName ";
                    sqlText += " order by EffectDate desc";
                    SqlCommand cmdFindLastNBRPrice = new SqlCommand(sqlText, currConn);
                    cmdFindLastNBRPrice.Transaction = transaction;
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@vEffectDate", vEffectDate);
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    object objLastNBRPrice = cmdFindLastNBRPrice.ExecuteScalar();
                    if (objLastNBRPrice != null)
                    {
                        LastNBRPrice = Convert.ToDecimal(objLastNBRPrice);
                    }

                    sqlText = "";
                    sqlText += "select top 1 isnull(NBRWithSDAmount,0) from BOMs WHERE FinishItemNo=@vFinishItemNo ";
                    sqlText += " AND EffectDate<@vEffectDate ";
                    sqlText += " AND VATName=@VATName ";
                    sqlText += " order by EffectDate desc";
                    SqlCommand cmdFindLastNBRWithSDAmount = new SqlCommand(sqlText, currConn);
                    cmdFindLastNBRWithSDAmount.Transaction = transaction;
                    cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@vEffectDate", vEffectDate);
                    cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    object objLastNBRWithSDAmount = cmdFindLastNBRWithSDAmount.ExecuteScalar();
                    if (objLastNBRWithSDAmount != null)
                    {
                        LastNBRWithSDAmount = Convert.ToDecimal(objLastNBRWithSDAmount);
                    }
                    sqlText = "";
                    sqlText += "select top 1 isnull(SDAmount,0) from BOMs WHERE FinishItemNo=@vFinishItemNo ";
                    sqlText += " AND EffectDate<@vEffectDate ";
                    sqlText += " AND VATName=@VATName ";
                    sqlText += " order by EffectDate desc";
                    SqlCommand cmdFindLastSDAmount = new SqlCommand(sqlText, currConn);
                    cmdFindLastSDAmount.Transaction = transaction;
                    cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@vEffectDate", vEffectDate);
                    cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    object objLastSDAmount = cmdFindLastSDAmount.ExecuteScalar();
                    if (objLastSDAmount != null)
                    {
                        LastSDAmount = Convert.ToDecimal(objLastSDAmount);
                    }

                    sqlText = "";
                    sqlText += "select top 1 MarkUpValue from BOMs WHERE FinishItemNo=@vFinishItemNo  ";
                    sqlText += " AND EffectDate<@vEffectDate ";
                    sqlText += " AND VATName=@VATName ";
                    sqlText += " order by EffectDate desc";
                    SqlCommand cmdFindLastMarkupAmount = new SqlCommand(sqlText, currConn);
                    cmdFindLastMarkupAmount.Transaction = transaction;
                    cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@vEffectDate", vEffectDate);
                    cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    object objLastMarkupAmount = cmdFindLastMarkupAmount.ExecuteScalar();
                    if (objLastMarkupAmount != null)
                    {
                        LastMarkupAmount = Convert.ToDecimal(objLastMarkupAmount);
                    }


                    #endregion Find Last Declared NBRPrice

                    Item.LastNBRPrice = LastNBRPrice;
                    Item.LastNBRWithSDAmount = LastNBRWithSDAmount;
                    Item.LastSDAmount = LastSDAmount;
                    Item.LastMarkupValue = LastMarkupAmount;

                    #region BOM Master Update

                    sqlText = "";

                    sqlText += " update BOMs set  ";

                    sqlText += " EffectDate             =@ItemEffectDate ,";
                    sqlText += " VATName                =@ItemVATName ,";
                    sqlText += " VATRate                =@ItemVATRate  ,";
                    sqlText += " UOM                    =@ItemUOM ,";
                    sqlText += " SD                     =@ItemSDRate  ,";
                    sqlText += " TradingMarkUp          =@ItemTradingMarkup  ,";
                    sqlText += " Comments               =@ItemComments ,";
                    sqlText += " ActiveStatus           =@ItemActiveStatus ,";
                    sqlText += " LastModifiedBy         =@ItemLastModifiedBy ,";
                    sqlText += " LastModifiedOn         =@ItemLastModifiedOn ,";
                    sqlText += " RawTotal               =@ItemRawTotal  ,";
                    sqlText += " PackingTotal           =@ItemPackingTotal  ,";
                    sqlText += " RebateTotal            =@ItemRebateTotal  ,";
                    sqlText += " AdditionalTotal        =@ItemAdditionalTotal  ,";
                    sqlText += " RebateAdditionTotal    =@ItemRebateAdditionTotal  ,";
                    sqlText += " NBRPrice               =@ItemPNBRPrice  ,";
                    sqlText += " PacketPrice            =@ItemPPacketPrice  ,";
                    sqlText += " RawOHCost              =@ItemRawOHCost  ,";
                    sqlText += " LastNBRPrice           =@LastNBRPrice  ,";
                    sqlText += " LastNBRWithSDAmount    =@LastNBRWithSDAmount  ,";
                    sqlText += " TotalQuantity          =@ItemTotalQuantity , ";
                    sqlText += " SDAmount               =@ItemSDAmount , ";
                    sqlText += " VATAmount              =@ItemVatAmount , ";
                    sqlText += " WholeSalePrice         =@ItemWholeSalePrice , ";
                    sqlText += " NBRWithSDAmount        =@ItemNBRWithSDAmount , ";
                    sqlText += " MarkUpValue            =@ItemMarkupValue , ";
                    sqlText += " LastMarkUpValue        =@ItemLastMarkupValue , ";
                    sqlText += " LastSDAmount           =@LastSDAmount , ";
                    sqlText += " LastAmount             =@ItemLastAmount , ";
                    sqlText += " Post                   =@ItemPost ";
                    sqlText += " where FinishItemNo     =@ItemItemNo  ";
                    sqlText += " and effectdate          =@ItemEffectDate";
                    sqlText += " and VATName             =@VATName";

                    SqlCommand cmdMasterUpdate = new SqlCommand(sqlText, currConn);
                    cmdMasterUpdate.Transaction = transaction;

                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", Item.EffectDate);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemVATName", Item.VATName ?? Convert.DBNull);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemVATRate", Item.VATRate);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemUOM", Item.UOM ?? Convert.DBNull);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemSDRate", Item.SDRate);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemTradingMarkup", Item.TradingMarkup);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemComments", Item.Comments ?? Convert.DBNull);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemActiveStatus", Item.ActiveStatus ?? Convert.DBNull);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedBy", Item.LastModifiedBy ?? Convert.DBNull);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedOn", Item.LastModifiedOn);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemRawTotal", Item.RawTotal);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemPackingTotal", Item.PackingTotal);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemRebateTotal", Item.RebateTotal);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemAdditionalTotal", Item.AdditionalTotal);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemRebateAdditionTotal", Item.RebateAdditionTotal);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemPNBRPrice", Item.PNBRPrice);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemPPacketPrice", Item.PPacketPrice);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemRawOHCost", Item.RawOHCost);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@LastNBRPrice", LastNBRPrice);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@LastNBRWithSDAmount", LastNBRWithSDAmount);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemTotalQuantity", Item.TotalQuantity);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemSDAmount", Item.SDAmount);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemVatAmount", Item.VatAmount);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemWholeSalePrice", Item.WholeSalePrice);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemNBRWithSDAmount", Item.NBRWithSDAmount);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemMarkupValue", Item.MarkupValue);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemLastMarkupValue", Item.LastMarkupValue);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@LastSDAmount", LastSDAmount);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemLastAmount", Item.LastAmount);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemPost", Item.Post ?? Convert.DBNull);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", Item.EffectDate);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@VATName", VATName ?? Convert.DBNull);

                    transResult = (int)cmdMasterUpdate.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                        MessageVM.PurchasemsgUpdateNotSuccessfully);
                    }

                    #region Remove row at BOMs

                    sqlText = "";
                    sqlText += " SELECT  distinct BOMId";
                    sqlText += " from BOMs where FinishItemNo =@ItemItemNo and effectdate=@ItemEffectDate and VATName=@VATName";

                    DataTable dt = new DataTable("Previous");
                    SqlCommand cmdService = new SqlCommand(sqlText, currConn);
                    cmdService.Transaction = transaction;
                    cmdService.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                    cmdService.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", Item.EffectDate);
                    cmdService.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    SqlDataAdapter dta = new SqlDataAdapter(cmdService);
                    dta.Fill(dt);
                    foreach (DataRow pBomID in dt.Rows)
                    {
                        var p = pBomID["BOMId"].ToString();
                        var tt = Details.Count(x => x.BOMId.Trim() == p.Trim());
                        if (tt == 0)
                        {
                            sqlText = "";
                            sqlText += " delete FROM BOMs ";
                            sqlText += " WHERE BOMId =@p";
                            SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                            cmdInsDetail.Transaction = transaction;
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@p", p);

                            transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        }

                    }

                    #endregion Remove row at BOMs

                }


                    #endregion Update Detail Table

                    #endregion  Update into Details(Update complete in Header)



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
                retResults[1] = MessageVM.spMsgUpdateSuccessfully;

                #endregion SuccessResult

            }
                #endregion Try

            #region Catch and Finall

            //catch (SqlException sqlex)
            //{
            //    transaction.Rollback();
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public string[] ServicePost(List<BOMNBRVM> Details)
        {
            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";


            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            string VATName = "";

            #endregion Initializ

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.spMsgMethodNameUpdate);

                #endregion open connection and transaction

                #region Validation for Detail

                if (Details.Count() <= 0)
                {
                    throw new ArgumentNullException(MessageVM.spMsgMethodNameUpdate, MessageVM.spMsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {

                    #region Validation for Fiscal Year

                    #region Fiscal Year Check

                    string transactionYearCheck = Convert.ToDateTime(Item.EffectDate).ToString("yyyy-MM-dd");
                    if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue ||
                        Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                    {

                        #region YearLock

                        sqlText = "";

                        sqlText +=
                            "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                            " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                        DataTable dataTable = new DataTable("ProductDataT");
                        SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                        cmdIdExist.Transaction = transaction;
                        SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                        reportDataAdapt.Fill(dataTable);

                        if (dataTable == null)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }

                        else if (dataTable.Rows.Count <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                        else
                        {
                            if (dataTable.Rows[0]["MLock"].ToString() != "N")
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearisLock);
                            }
                            else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearisLock);
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
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }

                        else if (dtYearNotExist.Rows.Count < 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }
                        else
                        {
                            if (Convert.ToDateTime(transactionYearCheck) <
                                Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                                ||
                                Convert.ToDateTime(transactionYearCheck) >
                                Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearNotExist);
                            }
                        }

                        #endregion YearNotExist

                    }


                    #endregion Fiscal Year CHECK

                    #endregion Validation for Fiscal Year

                    #region CheckVATName

                    VATName = Item.VATName;
                    if (string.IsNullOrEmpty(VATName))
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgVatNameNotFound);

                    }

                    #endregion CheckVATName

                    #region update BOM Table

                    if (Item == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.PurchasemsgNoDataToSave);
                    }

                    #region Checking bom exist

                    sqlText = "";
                    sqlText = "select count(bomid) from boms ";
                    sqlText += " where BOMId =@ItemBOMId";

                    SqlCommand cmdIsExist = new SqlCommand(sqlText, currConn);
                    cmdIsExist.Transaction = transaction;
                    cmdIsExist.Parameters.AddWithValueAndNullHandle("@ItemBOMId", Item.BOMId);

                    int isExist = (int)cmdIsExist.ExecuteScalar();

                    if (isExist <= 0)
                    {
                        throw new ArgumentNullException("BOMUpdate",
                                                        "Post not completed, Please save fitst of this item: '" +
                                                        Item.ItemNo + "'");
                    }

                    #endregion Checking other BOM after this date

                    #region Checking other BOM after this date

                    sqlText = "";
                    sqlText = "select count(bomid) from boms ";
                    sqlText += " where BOMId =@ItemBOMId";
                    sqlText += " and Post='Y'";

                    SqlCommand cmdIsPosted = new SqlCommand(sqlText, currConn);
                    cmdIsPosted.Transaction = transaction;
                    cmdIsPosted.Parameters.AddWithValueAndNullHandle("@ItemBOMId", Item.BOMId);

                    int isPosted = (int)cmdIsPosted.ExecuteScalar();

                    if (isPosted > 0)
                    {
                        throw new ArgumentNullException("BOMUpdate", "Data already posted.You cannot update this data.");
                    }

                    #endregion Checking other BOM after this date

                    #region Checking other BOM after this date

                    sqlText = "";
                    sqlText = "select count(bomid) from boms ";
                    sqlText += " where FinishItemNo =@ItemItemNo ";
                    sqlText += " and effectdate>@ItemEffectDate ";
                    sqlText += " and VATName=@VATName ";

                    SqlCommand cmdOtherBom = new SqlCommand(sqlText, currConn);
                    cmdOtherBom.Transaction = transaction;
                    cmdOtherBom.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                    cmdOtherBom.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", Item.EffectDate);
                    cmdOtherBom.Parameters.AddWithValueAndNullHandle("@VATName", VATName);


                    int otherBom = (int)cmdOtherBom.ExecuteScalar();

                    if (otherBom > 0)
                    {
                        throw new ArgumentNullException("BOMUpdate",
                                                        "Sorry,You cannot update this price declaration. Another declaration exist after this.");
                    }

                    #endregion Checking other BOM after this date





                    #region BOM Master Update

                    sqlText = "";

                    sqlText += " update BOMs set  ";
                    sqlText += " LastModifiedBy=@ItemLastModifiedBy,";
                    sqlText += " LastModifiedOn=@ItemLastModifiedOn ,";
                    sqlText += " Post=@ItemPost ";

                    sqlText += " where ";
                    sqlText += " FinishItemNo=@ItemItemNo ";
                    sqlText += " and EffectDate=@ItemEffectDate ";
                    sqlText += " AND VATName=@ItemVATName ";


                    SqlCommand cmdMasterUpdate = new SqlCommand(sqlText, currConn);
                    cmdMasterUpdate.Transaction = transaction;
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedBy", Item.LastModifiedBy);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedOn", DateTime.Now.ToString());
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemPost", "Y");
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", Item.EffectDate);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemVATName", Item.VATName);


                    transResult = (int)cmdMasterUpdate.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                        MessageVM.PurchasemsgUpdateNotSuccessfully);
                    }

                }


                    #endregion Update Detail Table

                    #endregion  Update into Details(Update complete in Header)


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
                retResults[1] = "Successfully Posted";

                #endregion SuccessResult

            }
                #endregion Try

            #region Catch and Finall

            //catch (SqlException sqlex)
            //{
            //    transaction.Rollback();
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            catch (Exception ex)
            {
                transaction.Rollback();
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                return retResults;
                //throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public string[] ServiceDelete(List<BOMNBRVM> Details)
        {
            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";


            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;

            string sqlText = "";

            #endregion Initializ

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.spMsgMethodNameUpdate);

                #endregion open connection and transaction

                #region Delete Detail Table

                foreach (var Item in Details.ToList())
                {

                    #region Validation for Fiscal Year

                    #region Fiscal Year Check

                    string transactionYearCheck = Convert.ToDateTime(Item.EffectDate).ToString("yyyy-MM-dd");
                    if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue ||
                        Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                    {

                        #region YearLock

                        sqlText = "";

                        sqlText +=
                            "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                            " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                        DataTable dataTable = new DataTable("ProductDataT");
                        SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                        cmdIdExist.Transaction = transaction;
                        SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                        reportDataAdapt.Fill(dataTable);

                        if (dataTable == null)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }

                        else if (dataTable.Rows.Count <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                        else
                        {
                            if (dataTable.Rows[0]["MLock"].ToString() != "N")
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearisLock);
                            }
                            else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearisLock);
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
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }

                        else if (dtYearNotExist.Rows.Count < 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }
                        else
                        {
                            if (Convert.ToDateTime(transactionYearCheck) <
                                Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                                ||
                                Convert.ToDateTime(transactionYearCheck) >
                                Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearNotExist);
                            }
                        }

                        #endregion YearNotExist

                    }


                    #endregion Fiscal Year CHECK

                    #endregion Validation for Fiscal Year


                    sqlText = "";
                    sqlText = "select count(bomid) from boms ";
                    sqlText += " where ";

                    sqlText += " FinishItemNo=@ItemItemNo ";
                    sqlText += " and EffectDate=@ItemEffectDate";
                    sqlText += " AND VATName=@ItemVATName ";

                    sqlText += " and Post='Y'";

                    SqlCommand cmdIsPosted = new SqlCommand(sqlText, currConn);
                    cmdIsPosted.Transaction = transaction;
                    cmdIsPosted.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                    cmdIsPosted.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", Item.EffectDate);
                    cmdIsPosted.Parameters.AddWithValueAndNullHandle("@ItemVATName", Item.VATName);


                    int isPosted = (int)cmdIsPosted.ExecuteScalar();

                    if (isPosted > 0)
                    {
                        throw new ArgumentNullException("BOMDelete", "Data already posted.You cannot delete this data.");
                    }






                    #region BOM Master Delete

                    sqlText = "";

                    sqlText += " Delete BOMs ";
                    sqlText += " where ";
                    sqlText += " FinishItemNo=@ItemItemNo ";
                    sqlText += " and EffectDate=@ItemEffectDate ";
                    sqlText += " AND VATName=@ItemVATName ";

                    SqlCommand cmdMasterUpdate = new SqlCommand(sqlText, currConn);
                    cmdMasterUpdate.Transaction = transaction;
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", Item.EffectDate);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemVATName", Item.VATName ?? Convert.DBNull);


                    transResult = (int)cmdMasterUpdate.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                        MessageVM.PurchasemsgUpdateNotSuccessfully);
                    }

                }


                    #endregion Delete Detail Table



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
                retResults[1] = "Successfully Deleted.";

                #endregion SuccessResult

            }
                #endregion Try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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



        #region SearchVAT1DT

        public DataTable SearchVAT1DTNew(string FinishItemName, string EffectDate, string VATName, string post,
                                         string PCode, string CustomerID = "0")
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("BOM");

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQL Statement

                sqlText =
                    @"
                        select distinct b.BOMId, b.FinishItemNo,p.productname,
                        convert(varchar, b.EffectDate,120)EffectDate
                        ,b.VATName,b.NBRPrice,p.ItemNo,p.ProductCode,b.UOM,

                        ISNULL(NULLIF(p.HSCodeNo,''),'NA')HSCodeNo
                        ,isnull(NULLIF(b.Comments, ''),'-')Comments
                        ,isnull(b.SD,0)SD,isnull(b.VATRate,0)VATRate,isnull(b.TradingMarkUp,0)TradingMarkUp,b.Post
                        ,isnull(b.CustomerID,0)CustomerID
                        ,isnull(c.CustomerName,'NA')CustomerName
                        from boms b 
                        left outer join products P on b.FinishItemNo =p.ItemNo                 
                        left outer join Customers c on b.CustomerID =c.CustomerID                 
                        WHERE 
                        (p.productname  LIKE '%' + @FinishItemName + '%' OR @FinishItemName IS NULL) 
                        AND (b.EffectDate = @EffectDate or @EffectDate is null)
                        AND (b.VATName = @VATName or @VATName is null) 
                         AND (p.ProductCode = @PCode or @PCode is null)";
                if (!string.IsNullOrEmpty(post))
                {
                    sqlText += "  AND (b.post = '" + post + "')";

                }
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(b.CustomerId,0)='" + CustomerID + "' ";
                }
                sqlText += " order by b.FinishItemNo asc,EffectDate desc";

                #endregion

                #region SQL Command

                SqlCommand objCommCBOM = new SqlCommand();
                objCommCBOM.Connection = currConn;

                objCommCBOM.CommandText = sqlText;
                objCommCBOM.CommandType = CommandType.Text;

                if (FinishItemName == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@FinishItemName"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@FinishItemName", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@FinishItemName"].Value = System.DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@FinishItemName"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@FinishItemName", FinishItemName);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@FinishItemName"].Value = FinishItemName;
                    }
                }
                if (PCode == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@PCode"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@PCode", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@PCode"].Value = System.DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@PCode"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@PCode", PCode);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@PCode"].Value = PCode;
                    }
                }

                if (EffectDate == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@EffectDate"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@EffectDate", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@EffectDate"].Value = System.DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@EffectDate"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@EffectDate", EffectDate);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@EffectDate"].Value = EffectDate;
                    }
                }
                if (VATName == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@VATName"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@VATName", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@VATName"].Value = System.DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@VATName"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@VATName", VATName);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@VATName"].Value = VATName;
                    }
                }

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCBOM);
                dataAdapter.Fill(dataTable);
            }
            #endregion
            #region Catch & Finally

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

            #endregion

            return dataTable;
        }



        #endregion

        #region SearchInputValues

        public DataTable SearchInputValues(string FinishItemName, string EffectDate, string VATName, string post,
                                           string FinishItemNo)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("BOM");

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQL Statement

                sqlText =
                    @"
                        select distinct b.BOMId, b.FinishItemNo,p.ProductCode,p.ProductName,
                        b.EffectDate,b.VATName,b.NBRPrice,b.UOM
                        ,isnull(NULLIF(b.Comments, ''),'-')Comments
                        ,isnull(b.SD,0)SD,isnull(b.VATRate,0)VATRate,isnull(b.TradingMarkUp,0)TradingMarkUp,b.Post

                        from boms b 
						left outer join
                        products P on b.FinishItemNo =p.ItemNo 
						Inner join (
                                        select VATName,FinishItemNo, max(EffectDate) effDate from BOMS 

                                        group by VATName, FinishItemNo ) v
                                        on b.VATName = v.VATName
                                        and b.EffectDate = v.effDate
                                        and b.FinishItemNo=v.FinishItemNo
                                        and (b.FinishItemNo = @FinishItemNo or @FinishItemNo is null)";

                if (!string.IsNullOrEmpty(post))
                {
                    sqlText += "  AND (b.post = '" + post + "')";

                }
                sqlText += " order by b.FinishItemNo asc,EffectDate desc";

                #endregion

                #region SQL Command

                SqlCommand objCommCBOM = new SqlCommand();
                objCommCBOM.Connection = currConn;

                objCommCBOM.CommandText = sqlText;
                objCommCBOM.CommandType = CommandType.Text;

                //if (FinishItemName == "")
                //{
                //    if (!objCommCBOM.Parameters.Contains("@FinishItemName"))
                //    {
                //        objCommCBOM.Parameters.AddWithValueAndNullHandle("@FinishItemName", System.DBNull.Value);
                //    }
                //    else
                //    {
                //        objCommCBOM.Parameters["@FinishItemName"].Value = System.DBNull.Value;
                //    }
                //}
                //else
                //{
                //    if (!objCommCBOM.Parameters.Contains("@FinishItemName"))
                //    {
                //        objCommCBOM.Parameters.AddWithValueAndNullHandle("@FinishItemName", FinishItemName);
                //    }
                //    else
                //    {
                //        objCommCBOM.Parameters["@FinishItemName"].Value = FinishItemName;
                //    }
                //}
                if (FinishItemNo == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@FinishItemNo"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@FinishItemNo", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@FinishItemNo"].Value = System.DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@FinishItemNo"))
                    {
                        objCommCBOM.Parameters.AddWithValueAndNullHandle("@FinishItemNo", FinishItemNo);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@FinishItemNo"].Value = FinishItemNo;
                    }
                }

                //if (EffectDate == "")
                //{
                //    if (!objCommCBOM.Parameters.Contains("@EffectDate"))
                //    {
                //        objCommCBOM.Parameters.AddWithValueAndNullHandle("@EffectDate", System.DBNull.Value);
                //    }
                //    else
                //    {
                //        objCommCBOM.Parameters["@EffectDate"].Value = System.DBNull.Value;
                //    }
                //}
                //else
                //{
                //    if (!objCommCBOM.Parameters.Contains("@EffectDate"))
                //    {
                //        objCommCBOM.Parameters.AddWithValueAndNullHandle("@EffectDate", EffectDate);
                //    }
                //    else
                //    {
                //        objCommCBOM.Parameters["@EffectDate"].Value = EffectDate;
                //    }
                //}
                //if (VATName == "")
                //{
                //    if (!objCommCBOM.Parameters.Contains("@VATName"))
                //    {
                //        objCommCBOM.Parameters.AddWithValueAndNullHandle("@VATName", System.DBNull.Value);
                //    }
                //    else
                //    {
                //        objCommCBOM.Parameters["@VATName"].Value = System.DBNull.Value;
                //    }
                //}
                //else
                //{
                //    if (!objCommCBOM.Parameters.Contains("@VATName"))
                //    {
                //        objCommCBOM.Parameters.AddWithValueAndNullHandle("@VATName", VATName);
                //    }
                //    else
                //    {
                //        objCommCBOM.Parameters["@VATName"].Value = VATName;
                //    }
                //}

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCBOM);
                dataAdapter.Fill(dataTable);
            }
            #endregion
            #region Catch & Finally

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

            #endregion

            return dataTable;
        }



        #endregion

        public DataTable SearchServicePrice(string BOMId)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("BOM");

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQL Statement

                sqlText = "";
                sqlText +=
                    "           SELECT b.BOMId,b.FinishItemNo ItemNo,p.ProductCode PCode,p.ProductName ProductName,";
                sqlText += " b.UOM UOM,b.NBRPrice BasePrice,b.TradingMarkUp Other,b.MarkUpValue OtherAmount,";
                sqlText +=
                    " b.sd SDRate,b.SDAmount SDAmount,b.VATRate VATRate,b.VATAmount VATAmount,b.WholeSalePrice SalePrice,";
                sqlText += " b.Comments Comment,p.HSCodeNo HSCodeNo,b.EffectDate EffectDate";
                sqlText += " FROM BOMs b LEFT OUTER JOIN";
                sqlText += " products p ON b.FinishItemNo=p.ItemNo";
                sqlText += " WHERE b.BOMId in('" + BOMId + "')";


                #endregion

                #region SQL Command

                SqlCommand objCommCBOM = new SqlCommand();
                objCommCBOM.Connection = currConn;

                objCommCBOM.CommandText = sqlText;
                objCommCBOM.CommandType = CommandType.Text;


                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCBOM);
                dataAdapter.Fill(dataTable);
            }
            #endregion
            #region Catch & Finally

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return dataTable;
        }


        public DataTable UseQuantityDT(string FinishItemNo, decimal Quantity, string EffectDate, string CustomerID = "0")
        {
            #region Objects & Variables


            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("BOM Use Qty");

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

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"
                            SELECT b.effectdate, fp.ProductCode FCode,fp.ProductName FProName,Rp.ProductName RProName, 
isnull(isnull(UOMUQty,b.UseQuantity),0)*@Quantity UseQuantity,
isnull(isnull(UOMWQty,b.WastageQuantity),0)*@Quantity WastageQuantity,
isnull(isnull(isnull(UOMUQty,b.UseQuantity),0)*@Quantity +isnull(isnull(UOMWQty,b.WastageQuantity),0)*@Quantity,0) TotalUseQuantity
,isnull(isnull(Rp.QuantityInHand,0)+isnull(Rp.OpeningBalance,0),0) QuantityInHand
,isnull(isnull(Rp.QuantityInHand,0)+isnull(Rp.OpeningBalance,0),0)-
isnull(isnull(isnull(UOMUQty,b.UseQuantity),0)*@Quantity +isnull(isnull(UOMWQty,b.WastageQuantity),0)*@Quantity,0) Rest

from boms b inner join
(select distinct FinishItemNo, 
max(EffectDate)EffectDate from boms
group by FinishItemNo) M on
b.FinishItemNo=m.FinishItemNo and b.EffectDate<=m.EffectDate  LEFT OUTER JOIN
products RP ON b.RawItemNo=Rp.ItemNo LEFT OUTER JOIN
products FP ON b.finishItemNo =fp.ItemNo
WHERE b.FinishItemNo=@FinishItemNo AND 
b.EffectDate<=@EffectDate 
ORDER BY Rp.ProductName";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                if (!objComm.Parameters.Contains("@FinishItemNo"))
                {
                    objComm.Parameters.AddWithValueAndNullHandle("@FinishItemNo", FinishItemNo);
                }
                else
                {
                    objComm.Parameters["@FinishItemNo"].Value = FinishItemNo;
                }

                if (!objComm.Parameters.Contains("@EffectDate"))
                {
                    objComm.Parameters.AddWithValueAndNullHandle("@EffectDate", EffectDate);
                }
                else
                {
                    objComm.Parameters["@EffectDate"].Value = EffectDate;
                }


                if (!objComm.Parameters.Contains("@Quantity"))
                {
                    objComm.Parameters.AddWithValueAndNullHandle("@Quantity", Quantity);
                }
                else
                {
                    objComm.Parameters["@Quantity"].Value = Quantity;
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #endregion
            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
        public string FindBOMID(string itemNo, string VatName, string effectDate, SqlConnection currConn,
                              SqlTransaction transaction, string CustomerID = "0")
        {
            #region Initializ

            string sqlText = "";
            string BomId = string.Empty;

            #endregion

            #region Try

            try
            {

                #region Validation

                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("FindBOMID", "There is No data to find Price");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue ||
                         Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("FindBOMID", "There is No data to find Price");

                }

                #endregion Validation

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

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo=@itemNo ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);


                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("FindBOMID", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Last BOMId

                sqlText = "  ";
                sqlText += " select top 1  CONVERT(varchar(10), isnull(BOMId,0))BOMId from BOMs";
                sqlText += " where ";
                sqlText += " FinishItemNo=@itemNo ";
                sqlText += " and vatname=@VatName ";
                //sqlText += " and effectdate<='" + effectDate.Date + "'";
                sqlText += " and effectdate<=@effectDate";
                sqlText += " and post='Y' ";
                sqlText += " order by effectdate desc ";

                SqlCommand cmdBomId = new SqlCommand(sqlText, currConn);
                cmdBomId.Transaction = transaction;
                cmdBomId.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);
                cmdBomId.Parameters.AddWithValueAndNullHandle("@VatName", VatName);
                cmdBomId.Parameters.AddWithValueAndNullHandle("@effectDate", effectDate);


                var tt = "";
                if (cmdBomId.ExecuteScalar() == null)
                {
                    //throw new ArgumentNullException("FindBOMID",
                    //                                "No Price declaration found for this item");
                    BomId = string.Empty;
                }
                else
                {
                    BomId = cmdBomId.ExecuteScalar().ToString();
                }
                //BomId = tt;
                #endregion Last BOMId

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                //if (currConn == null)
                //{
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();

                }
                //}
            }

            #endregion

            #region Results

            return BomId;

            #endregion

        }
        public string FindBOMIDOverHead(string itemNo, string VatName, string effectDate, SqlConnection currConn,
                                SqlTransaction transaction, string CustomerID = "0")
        {
            #region Initializ

            string sqlText = "";
            string BomId = string.Empty;

            #endregion

            #region Try

            try
            {

                #region Validation

                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("FindBOMID", "There is No data to find Price");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue ||
                         Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("FindBOMID", "There is No data to find Price");

                }

                #endregion Validation

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

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo=@itemNo ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("FindBOMID", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Last BOMId

                sqlText = "  ";
                sqlText += " select top 1  CONVERT(varchar(10), isnull(BOMId,0))BOMId from BOMCompanyOverhead";
                sqlText += " where ";
                sqlText += " HeadID=@itemNo ";
                sqlText += " and vatname=@VatName  ";
                //sqlText += " and effectdate<='" + effectDate.Date + "'";
                sqlText += " and effectdate<=@effectDate ";
                sqlText += " and post='Y' ";
                sqlText += " order by effectdate desc ";

                SqlCommand cmdBomId = new SqlCommand(sqlText, currConn);
                cmdBomId.Transaction = transaction;
                cmdBomId.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);
                cmdBomId.Parameters.AddWithValueAndNullHandle("@VatName", VatName);
                cmdBomId.Parameters.AddWithValueAndNullHandle("@effectDate", effectDate);


                var tt = "";
                if (cmdBomId.ExecuteScalar() == null)
                {
                    //throw new ArgumentNullException("FindBOMID",
                    //                                "No Price declaration found for this item");
                    BomId = string.Empty;
                }
                else
                {
                    BomId = cmdBomId.ExecuteScalar().ToString();
                }
                //BomId = tt;
                #endregion Last BOMId

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                //if (currConn == null)
                //{
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();

                }
                //}
            }

            #endregion

            #region Results

            return BomId;

            #endregion

        }

        public string[] DeleteBOM(string itemNo, string VatName, string effectDate, SqlConnection currConn,
                                  SqlTransaction transaction, string CustomerID = "0")
        {


            #region Initializ

            string sqlText = "";
            string BomId = string.Empty;

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            int transResult = 0;

            #endregion

            #region Try

            try
            {

                #region Validation

                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("FindBOMID", "There is No data.");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue ||
                         Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("FindBOMID", "There is No data to find Price");

                }

                #endregion Validation

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

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo=@itemNo ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("FindBOMID", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Last BOMId

                sqlText = "  ";
                sqlText += " select top 1  CONVERT(varchar(10), isnull(BOMId,0))BOMId  from BOMs";
                sqlText += " where ";
                sqlText += " FinishItemNo=@itemNo ";
                sqlText += " and vatname=@VatName ";
                sqlText += " and effectdate<=@effectDate ";
                //sqlText += " and post='Y' ";
                sqlText += " and post='N' ";
                sqlText += " order by effectdate desc ";

                SqlCommand cmdBomId = new SqlCommand(sqlText, currConn);
                cmdBomId.Transaction = transaction;
                cmdBomId.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);
                cmdBomId.Parameters.AddWithValueAndNullHandle("@VatName", VatName);
                cmdBomId.Parameters.AddWithValueAndNullHandle("@effectDate", effectDate);

                if (cmdBomId.ExecuteScalar() == null)
                {
                    BomId = string.Empty;

                    #region SuccessResult

                    retResults[0] = "Fail";
                    retResults[1] = "This transaction was posted.";

                    #endregion SuccessResult

                    return retResults;
                }
                else
                {
                    BomId = (string)cmdBomId.ExecuteScalar();
                }

                #endregion Last BOMId

                if (!string.IsNullOrEmpty(BomId))
                {
                    #region Remove row at BOMCompanyOverhead

                    sqlText = "";
                    sqlText += " Delete from BOMCompanyOverhead WHERE  ";
                    sqlText += " FinishItemNo=@itemNo";
                    sqlText += " and EffectDate=@effectDate";
                    sqlText += " AND VATName=@VatName";
                    sqlText += " AND BOMId=@BomId";

                    SqlCommand cmdRemove = new SqlCommand(sqlText, currConn);
                    cmdRemove.Transaction = transaction;
                    cmdRemove.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo ?? Convert.DBNull);
                    cmdRemove.Parameters.AddWithValueAndNullHandle("@effectDate", effectDate);
                    cmdRemove.Parameters.AddWithValueAndNullHandle("@VatName", VatName ?? Convert.DBNull);
                    cmdRemove.Parameters.AddWithValueAndNullHandle("@BomId", BomId ?? Convert.DBNull);

                    transResult = (int)cmdRemove.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("RemoveBOM", "There is No data to remove");
                    }

                    #endregion Remove row at BOMCompanyOverhead

                    #region Remove row at BOMRaws

                    sqlText = "";
                    sqlText += " Delete from BOMRaws WHERE  ";
                    sqlText += " FinishItemNo=@itemNo";
                    sqlText += " and EffectDate=@effectDate";
                    sqlText += " AND VATName=@VatName";
                    sqlText += " AND BOMId=@BomId";

                    cmdRemove = new SqlCommand(sqlText, currConn);
                    cmdRemove.Transaction = transaction;
                    cmdRemove.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo ?? Convert.DBNull);
                    cmdRemove.Parameters.AddWithValueAndNullHandle("@effectDate", effectDate);
                    cmdRemove.Parameters.AddWithValueAndNullHandle("@VatName", VatName ?? Convert.DBNull);
                    cmdRemove.Parameters.AddWithValueAndNullHandle("@BomId", BomId ?? Convert.DBNull);

                    transResult = (int)cmdRemove.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("RemoveBOM", "There is No data to remove");
                    }

                    #endregion Remove row at BOMRaws

                    #region Remove row at BOMs

                    sqlText = "";
                    sqlText += " Delete from BOMs WHERE  ";
                    sqlText += " FinishItemNo=@itemNo";
                    sqlText += " and EffectDate=@effectDate";
                    sqlText += " AND VATName=@VatName";
                    sqlText += " AND BOMId=@BomId";

                    cmdRemove = new SqlCommand(sqlText, currConn);
                    cmdRemove.Transaction = transaction;
                    cmdRemove.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo ?? Convert.DBNull);
                    cmdRemove.Parameters.AddWithValueAndNullHandle("@effectDate", effectDate);
                    cmdRemove.Parameters.AddWithValueAndNullHandle("@VatName", VatName ?? Convert.DBNull);
                    cmdRemove.Parameters.AddWithValueAndNullHandle("@BomId", BomId ?? Convert.DBNull);

                    transResult = (int)cmdRemove.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("RemoveBOM", "There is No data to remove");
                    }

                    #endregion Remove row at BOMs

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
                    retResults[1] = "Successfully Deleted.";

                    #endregion SuccessResult
                }

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();

                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public string[] FindCostingFrom(string itemNo, string effectDate, SqlConnection curConn, SqlTransaction transaction, string CustomerID = "0")
        {
            #region Initialize

            string sqlText = "";
            string[] retResults = new string[2];
            retResults[0] = "0";
            retResults[1] = "No Data";
            int transResult = 0;
            string purchaseInvoiceNo = "";

            #endregion Initialize

            #region Try

            try
            {
                #region validation

                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("FindLastCosting", "There is No data to find ID.");
                }

                #endregion validation

                #region open connection and transaction

                if (curConn == null)
                {
                    curConn = _dbsqlConnection.GetConnection();

                    if (curConn.State != ConnectionState.Open)
                    {
                        curConn.Open();
                    }

                }
                //transaction = curConn.BeginTransaction(MessageVM.bomMsgMethodNameInsert);
                #endregion open connection and transaction



                #region Find purchase ID

                sqlText = "";
                sqlText += " select Top 1 PurchaseInvoiceNo from PurchaseInvoiceDetails";
                sqlText += " where ItemNo=@itemNo ";
                sqlText += " and ReceiveDate<=@effectDate ";

                sqlText += " order by ReceiveDate desc ";

                SqlCommand cmd = new SqlCommand(sqlText, curConn);
                cmd.Transaction = transaction;
                cmd.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);
                cmd.Parameters.AddWithValueAndNullHandle("@effectDate", effectDate);

                if (cmd.ExecuteScalar() == null)
                {
                    purchaseInvoiceNo = string.Empty;

                    sqlText = "";
                    sqlText += " select Top 1 CAST(id as varchar) Id from Costing";
                    sqlText += " where ItemNo=@itemNo ";
                    sqlText += " and InputDate<=@effectDate";
                    sqlText += " order by InputDate desc ";

                    cmd = new SqlCommand(sqlText, curConn);
                    cmd.Transaction = transaction;
                    cmd.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);
                    cmd.Parameters.AddWithValueAndNullHandle("@effectDate", effectDate);

                    if (cmd.ExecuteScalar() != null)
                    {
                        retResults[0] = (string)cmd.ExecuteScalar();
                        retResults[1] = "FromCosting";
                    }

                }
                else
                {
                    purchaseInvoiceNo = (string)cmd.ExecuteScalar();

                    sqlText = "";
                    //                    sqlText += @" Declare @ReceiveDate datetime;
                    //  Declare @InputDate datetime;";
                    //                    sqlText += "\r\n select top 1 @ReceiveDate=ReceiveDate from PurchaseInvoiceDetails";
                    //                    sqlText += " where ItemNo='" + itemNo + "' ";
                    //                    sqlText += " and ReceiveDate<='" + effectDate + "'";
                    //                    sqlText += " order by ReceiveDate desc ";

                    sqlText += @" 
  Declare @InputDate datetime;
DECLARE @ID varchar(120);";

                    sqlText += "  select top 1 @InputDate=InputDate,@ID=id from Costing ";
                    sqlText += " where ItemNo='" + itemNo + "' ";
                    sqlText += " and InputDate<='" + effectDate + "'";
                    sqlText += " order by InputDate desc ";
                    sqlText += "\r\n";
                    sqlText += @" if @InputDate!=''
                                     begin
                                     select Top 1 case when p.ReceiveDate > @InputDate 
                                     then p.PurchaseInvoiceNo  else CAST(@ID as varchar) end AS ID ,
                                     case when p.ReceiveDate > @InputDate 
                                     then 'P'  else 'C' end AS FromPlace 
                                     from PurchaseInvoiceDetails p
                                     ";

                    sqlText += " where p.ItemNo='" + itemNo + "' ";
                    sqlText += " and p.ReceiveDate <='" + effectDate + "'";
                    sqlText += " order by p.ReceiveDate desc ";
                    sqlText += " end";

                    DataTable dataTable = new DataTable("CostDataT");
                    cmd = new SqlCommand(sqlText, curConn);
                    cmd.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmd);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable.Rows.Count <= 0)
                    {
                        retResults[0] = purchaseInvoiceNo;
                        retResults[1] = "FromPurchase";
                    }
                    else
                    {
                        if (dataTable.Rows[0]["FromPlace"].ToString() == "P")
                        {
                            retResults[0] = dataTable.Rows[0]["ID"].ToString();
                            retResults[1] = "FromPurchase";
                        }
                        else if (dataTable.Rows[0]["FromPlace"].ToString() == "C")
                        {
                            retResults[0] = dataTable.Rows[0]["ID"].ToString();
                            retResults[1] = "FromCosting";
                        }
                        else
                        {
                            retResults[0] = "0";
                            retResults[1] = "No Data";
                        }
                    }




                    //if (cmd.ExecuteScalar() == null)
                    //{
                    //    //retResults[0] = "0";
                    //    //retResults[1] = "No Data";
                    //    retResults[0] = purchaseInvoiceNo;
                    //    retResults[1] = "FromPurchase";
                    //}
                    //else
                    //{
                    //    string id = (string)cmd.ExecuteScalar();
                    //    if (!string.IsNullOrEmpty(id))
                    //    {
                    //        retResults[0] = id;
                    //        retResults[1] = "FromPurchase";
                    //    }
                    //    else
                    //    {
                    //        retResults[0] = id;
                    //        retResults[1] = "FromCosting";
                    //    }
                    //}



                }

                #endregion Find purchase ID


                #region Find costing

                //if (string.IsNullOrEmpty(purchaseInvoiceNo))
                //{
                //    sqlText = "";
                //    sqlText += " select Top 1 Id from Costing";
                //    sqlText += " where ItemNo='" + itemNo + "' ";
                //    sqlText += " and InputDate<='" + effectDate + "'";
                //    sqlText += " order by InputDate desc ";

                //    cmd = new SqlCommand(sqlText, curConn);
                //    cmd.Transaction = transaction;
                //    if (cmd.ExecuteScalar() == null)
                //    {
                //        retResults[0] = "0";
                //        retResults[1] = "No Data";
                //    }
                //    else
                //    {
                //        retResults[0] = (string) cmd.ExecuteScalar();
                //        retResults[1] = "FromCosting";
                //    }


                //}

                #endregion Find costing

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Commit

                //#region SuccessResult

                //retResults[0] = "Success";
                //retResults[1] = "Successfully Deleted.";

                //#endregion SuccessResult
            }
            #endregion Try
            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                if (curConn.State == ConnectionState.Open)
                {
                    curConn.Close();

                }
            }

            #endregion

            return retResults;

        }

        public List<BOMNBRVM> SelectAll(string BOMId = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, BOMNBRVM likeVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<BOMNBRVM> VMs = new List<BOMNBRVM>();
            BOMNBRVM vm;
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
 bm.BOMId
,bm.FinishItemNo
,bm.EffectDate
,bm.VATName
,isnull(bm.VATRate,0)VATRate
,isnull(bm.SD,0)SD
,isnull(bm.TradingMarkUp,0)TradingMarkUp
,bm.Comments
,bm.ActiveStatus
,bm.CreatedBy
,bm.CreatedOn
,bm.LastModifiedBy
,bm.LastModifiedOn
,isnull(bm.RawTotal,0)RawTotal
,isnull(bm.PackingTotal,0)PackingTotal
,isnull(bm.RebateTotal,0)RebateTotal
,isnull(bm.AdditionalTotal,0)AdditionalTotal
,isnull(bm.RebateAdditionTotal,0)RebateAdditionTotal
,isnull(bm.NBRPrice,0)NBRPrice
,isnull(bm.PacketPrice,0)PacketPrice
,bm.RawOHCost
,isnull(bm.LastNBRPrice,0)LastNBRPrice
,isnull(bm.LastNBRWithSDAmount,0)LastNBRWithSDAmount
,isnull(bm.TotalQuantity,0)TotalQuantity
,isnull(bm.SDAmount,0)SDAmount
,isnull(bm.VATAmount,0)VATAmount
,isnull(bm.WholeSalePrice,0)WholeSalePrice
,isnull(bm.NBRWithSDAmount,0)NBRWithSDAmount
,isnull(bm.MarkUpValue,0)MarkUpValue
,isnull(bm.LastMarkUpValue,0)LastMarkUpValue
,isnull(bm.LastSDAmount,0)LastSDAmount
,isnull(bm.LastAmount,0)LastAmount
,bm.Post
,bm.UOM
,bm.CustomerID
,p.ProductName
,p.ProductCode
,p.CategoryID
,c.CustomerName
FROM BOMs  bm left outer join Products p 
on bm.FinishItemNo=p.ItemNo left outer join Customers c
on bm.CustomerID=c.CustomerID
WHERE  1=1 
";
                if (BOMId != null && BOMId != "0")
                {
                    sqlText += @" and bm.BOMId=@BOMId";
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
                    if (!string.IsNullOrWhiteSpace(likeVM.FinishItemName))
                    {
                        sqlText += " AND p.ProductName like @ProductName ";
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
                    if (!string.IsNullOrWhiteSpace(likeVM.FinishItemName))
                    {
                        objComm.Parameters.AddWithValue("@ProductName", "%" + likeVM.FinishItemName + "%");
                    }
                }
                if (BOMId != null)
                {
                    objComm.Parameters.AddWithValueAndNullHandle("@BOMId", BOMId);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new BOMNBRVM();

                    vm.BOMId = dr["BOMId"].ToString();
                    vm.ItemNo = dr["FinishItemNo"].ToString();
                    vm.EffectDate = dr["EffectDate"].ToString();
                    vm.VATName = dr["VATName"].ToString();
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                    vm.SDRate = Convert.ToDecimal(dr["SD"].ToString());
                    vm.TradingMarkup = Convert.ToDecimal(dr["TradingMarkUp"].ToString());
                    vm.Comments = dr["Comments"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.RawTotal = Convert.ToDecimal(dr["RawTotal"].ToString());
                    vm.PackingTotal = Convert.ToDecimal(dr["PackingTotal"].ToString());
                    vm.RebateTotal = Convert.ToDecimal(dr["RebateTotal"].ToString());
                    vm.AdditionalTotal = Convert.ToDecimal(dr["AdditionalTotal"].ToString());
                    vm.RebateAdditionTotal = Convert.ToDecimal(dr["RebateAdditionTotal"].ToString());
                    vm.PNBRPrice = Convert.ToDecimal(dr["NBRPrice"].ToString());
                    vm.PPacketPrice = Convert.ToDecimal(dr["PacketPrice"].ToString());
                    vm.RawOHCost = Convert.ToDecimal(dr["RawOHCost"].ToString());
                    vm.LastNBRPrice = Convert.ToDecimal(dr["LastNBRPrice"].ToString());
                    vm.LastNBRWithSDAmount = Convert.ToDecimal(dr["LastNBRWithSDAmount"].ToString());
                    vm.TotalQuantity = Convert.ToDecimal(dr["TotalQuantity"].ToString());
                    vm.SDAmount = Convert.ToDecimal(dr["SDAmount"].ToString());
                    vm.VatAmount = Convert.ToDecimal(dr["VATAmount"].ToString());
                    vm.WholeSalePrice = Convert.ToDecimal(dr["WholeSalePrice"].ToString());
                    vm.NBRWithSDAmount = Convert.ToDecimal(dr["NBRWithSDAmount"].ToString());
                    vm.MarkupValue = Convert.ToDecimal(dr["MarkUpValue"].ToString());
                    vm.LastMarkupValue = Convert.ToDecimal(dr["LastMarkUpValue"].ToString());
                    vm.LastSDAmount = Convert.ToDecimal(dr["LastSDAmount"].ToString());
                    vm.LastAmount = Convert.ToDecimal(dr["LastAmount"].ToString());
                    vm.Post = dr["Post"].ToString();
                    vm.UOM = dr["UOM"].ToString();
                    vm.CustomerID = dr["CustomerID"].ToString();
                    vm.FinishItemName = dr["ProductName"].ToString();
                    vm.CustomerName = dr["CustomerName"].ToString();
                    vm.FinishItemCode = dr["ProductCode"].ToString();
                    vm.FinishCategory = dr["CategoryID"].ToString();

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

        public List<BOMItemVM> SelectAllItems(string BOMId = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<BOMItemVM> VMs = new List<BOMItemVM>();
            BOMItemVM vm;
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
 braw.BOMRawId
,p.ProductName
,braw.BOMId
,braw.BOMLineNo
,braw.FinishItemNo
,braw.RawItemNo
,braw.RawItemType
,braw.EffectDate
,braw.VATName
,isnull(braw.UseQuantity,0) UseQuantity 
,isnull(braw.WastageQuantity,0) WastageQuantity 
,isnull(braw.Cost,0) Cost 
,braw.UOM
,isnull(braw.VATRate,0)VATRate 
,isnull(braw.VATAmount,0)VATAmount 
,isnull(braw.SD,0)SD 
,isnull(braw.SDAmount,0)SDAmount 
,isnull(braw.TradingMarkUp,0)TradingMarkUp 
,isnull(braw.RebateRate,0)RebateRate 
,isnull(braw.MarkUpValue,0)MarkUpValue 
,braw.CreatedBy
,braw.CreatedOn
,braw.LastModifiedBy
,braw.LastModifiedOn
,braw.UnitCost
,braw.UOMn
,isnull(braw.UOMc,0)UOMc 
,isnull(braw.UOMPrice,0)UOMPrice 
,isnull(braw.UOMUQty,0)UOMUQty 
,isnull(braw.UOMWQty,0)UOMWQty 
,isnull(braw.TotalQuantity,0)TotalQuantity 
,braw.Post
,braw.PBOMId
,braw.PInvoiceNo
,braw.TransactionType
,braw.CustomerID
,braw.IssueOnProduction
,p.ProductCode

from BOMRaws braw 
left outer join Products p on braw.RawItemNo=p.ItemNo where 1=1 ";

                if (BOMId != null)
                {
                    sqlText += @" and braw.BOMId=@BOMId";
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
                        sqlText += " AND braw." + conditionFields[i] + "=@" + cField;
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
                        objComm.Parameters.AddWithValueAndNullHandle("@" + cField, conditionValues[j]);
                    }
                }

                if (BOMId != null)
                {
                    objComm.Parameters.AddWithValueAndNullHandle("@BOMId", BOMId);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new BOMItemVM();

                    vm.BOMRawId = dr["BOMRawId"].ToString();
                    vm.BOMId = dr["BOMId"].ToString();
                    vm.BOMLineNo = dr["BOMLineNo"].ToString();
                    vm.FinishItemNo = dr["FinishItemNo"].ToString();
                    vm.RawItemNo = dr["RawItemNo"].ToString();
                    vm.RawItemType = dr["RawItemType"].ToString();
                    vm.EffectDate = dr["EffectDate"].ToString();
                    vm.UseQuantity = Convert.ToDecimal(dr["UseQuantity"].ToString());
                    vm.WastageQuantity = Convert.ToDecimal(dr["WastageQuantity"].ToString());
                    vm.Cost = Convert.ToDecimal(dr["Cost"].ToString());
                    vm.UOM = dr["UOM"].ToString();
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                    //vm.VATAmount = dr["VATAmount"].ToString();
                    vm.SD = Convert.ToDecimal(dr["SD"].ToString());
                    vm.SDAmount = Convert.ToDecimal(dr["SDAmount"].ToString());
                    vm.TradingMarkUp = Convert.ToDecimal(dr["TradingMarkUp"].ToString());
                    vm.RebateRate = Convert.ToDecimal(dr["RebateRate"].ToString());
                    vm.MarkUpValue = Convert.ToDecimal(dr["MarkUpValue"].ToString());
                    //vm.CreatedBy = dr["CreatedBy"].ToString();
                    //vm.CreatedOn = dr["CreatedOn"].ToString();
                    //vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    //vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.UnitCost = Convert.ToDecimal(dr["UnitCost"].ToString());
                    vm.UOMn = dr["UOMn"].ToString();
                    vm.UOMc = Convert.ToDecimal(dr["UOMc"].ToString());
                    vm.UOMPrice = Convert.ToDecimal(dr["UOMPrice"].ToString());
                    vm.UOMUQty = Convert.ToDecimal(dr["UOMUQty"].ToString());
                    vm.UOMWQty = Convert.ToDecimal(dr["UOMWQty"].ToString());
                    vm.TotalQuantity = Convert.ToDecimal(dr["TotalQuantity"].ToString());
                    vm.Post = dr["Post"].ToString();
                    vm.PBOMId = dr["PBOMId"].ToString();
                    vm.PInvoiceNo = dr["PInvoiceNo"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.CustomerID = dr["CustomerID"].ToString();
                    vm.IssueOnProduction = dr["IssueOnProduction"].ToString();
                    vm.RawItemName = dr["ProductName"].ToString();
                    vm.RawItemCode = dr["ProductCode"].ToString();
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

        public List<BOMOHVM> SelectAllOverheads(string BOMId = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<BOMOHVM> VMs = new List<BOMOHVM>();
            BOMOHVM vm;
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
 bco.BOMOverHeadId
,bco.BOMId
,bco.OHLineNo
,bco.HeadName
,bco.FinishItemNo
,bco.EffectDate
,bco.VATName
,isnull(bco.HeadAmount,0) HeadAmount 
,bco.CreatedBy
,bco.CreatedOn
,bco.LastModifiedBy
,bco.LastModifiedOn
,bco.Info5
,isnull(bco.RebatePercent,0) RebatePercent 
,isnull(bco.RebateAmount,0) RebateAmount 
,isnull(bco.AdditionalCost,0) AdditionalCost 
,bco.Post
,bco.HeadID
,bco.CustomerID
,p.ProductCode
from BOMCompanyOverhead bco left outer join Products p on bco.HeadID=p.ItemNo 
where 1=1 
";

                if (BOMId != null)
                {
                    sqlText += @" and bco.BOMId=@BOMId";
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
                        objComm.Parameters.AddWithValueAndNullHandle("@" + cField, conditionValues[j]);
                    }
                }

                if (BOMId != null)
                {
                    objComm.Parameters.AddWithValueAndNullHandle("@BOMId", BOMId);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new BOMOHVM();

                    vm.BOMId = dr["BOMId"].ToString();
                    vm.OHLineNo = dr["OHLineNo"].ToString();
                    vm.HeadName = dr["HeadName"].ToString();
                    vm.FinishItemNo = dr["FinishItemNo"].ToString();
                    vm.EffectDate = dr["EffectDate"].ToString();
                    vm.HeadAmount = Convert.ToDecimal(dr["HeadAmount"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.RebatePercent = Convert.ToDecimal(dr["RebatePercent"].ToString());
                    vm.RebateAmount = Convert.ToDecimal(dr["RebateAmount"].ToString());
                    vm.AdditionalCost = Convert.ToDecimal(dr["AdditionalCost"].ToString());
                    vm.Post = dr["Post"].ToString();
                    vm.HeadID = dr["HeadID"].ToString();
                    vm.CustomerID = dr["CustomerID"].ToString();
                    vm.OHCode = dr["ProductCode"].ToString();

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
    }
}
