using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymServices.VMS.Library;
using SymViewModel.VMS.DTOs;
using System.Data;
using System.Data.SqlClient;

namespace SymRepository.VMS.Repo
{
    public class ProductRepo
    {
        public List<ProductVM> DropDownAll() 
        {
            return new ProductDAL().DropDownAll();
        }

        public DataTable SearchProductDT(string ItemNo, string ProductName, string CategoryID, string CategoryName, string UOM, string IsRaw, string SerialNo, string HSCodeNo, string ActiveStatus, string Trading, string NonStock, string ProductCode, string databaseName, string customerId = "0")
        {
            return new ProductDAL().SearchProductDT( ItemNo,  ProductName,  CategoryID,  CategoryName,  UOM,  IsRaw,  SerialNo,  HSCodeNo,  ActiveStatus,  Trading,  NonStock,  ProductCode,  databaseName, customerId);
        }

        public List<ProductVM> GetProductByType(string type)
        {
            return new ProductDAL().GetProductByType(type);
        }

        public List<ProductVM> DropDown()
        {
            return new ProductDAL().DropDown();
        }

        public List<ProductVM> DropDownProductByCategory(string catId)
        {
            return new ProductDAL().DropDownByCategory(catId);
        }

        public string[] InsertToProduct(ProductVM vm, List<TrackingVM> Trackings, string ItemType)
        {
            try
            {
                return new ProductDAL().InsertToProduct(vm, Trackings, ItemType);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateProduct(ProductVM vm, List<TrackingVM> Trackings, string ItemType)
        {
            try
            {
                return new ProductDAL().UpdateProduct(vm, Trackings, ItemType);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] Delete(ProductVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new ProductDAL().Delete(vm, ids, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<ProductVM> SelectAll(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new ProductDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<ProductVM> SelectAllOverhead(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new ProductDAL().SelectAllOverhead(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable AvgPriceNew(string itemNo, string tranDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, bool isPost) 
        {
            return new ProductDAL().AvgPriceNew(itemNo, tranDate, VcurrConn, Vtransaction, isPost);
        }

        public DataTable GetLIFOPurchaseInformation(string itemNo, string receiveDate)
        {
            return new ProductDAL().GetLIFOPurchaseInformation(itemNo, receiveDate);
        }

        public decimal GetLastNBRPriceFromBOM(string itemNo, string VatName, string effectDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, string CustomerID = "0")
        {
            return new ProductDAL().GetLastNBRPriceFromBOM(itemNo, VatName, effectDate, VcurrConn, Vtransaction, CustomerID);
        }


    }
}
