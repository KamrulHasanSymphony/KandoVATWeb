using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using VATViewModel.DTOs;
using VATServer.Library;
using SymOrdinary;
using System.Web;

namespace SymRepository.VMS
{
    public class ProductRepo
    {
         private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

         public ProductRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

         public ProductRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
         //public ProductRepo()
         //{
         //    // TODO: Complete member initialization
         //}

        public List<BOMNBRVM> DropDownBOMReferenceNo(string itemNo, string VatName, string effectDate, string CustomerID)
        {
            return new ProductDAL().DropDownBOMReferenceNo(itemNo, VatName, effectDate, CustomerID, null, null, connVM);
        }

        public List<ProductVM> DropDownAll() 
        {
            return new ProductDAL().DropDownAll(connVM);
        }


        public string[] UpdateToExitProduct(ProductVM vm) 
        {
            return new ProductDAL().UpdateToExitProduct(vm,null,null,connVM);
        }


        public DataTable SearchProductDT(string ItemNo, string ProductName, string CategoryID, string CategoryName, string UOM, string IsRaw, string SerialNo, string HSCodeNo, string ActiveStatus, string Trading, string NonStock, string ProductCode, string databaseName, string customerId = "0")
        {
            return new ProductDAL().SearchProductDT( ItemNo,  ProductName,  CategoryID,  CategoryName,  UOM,  IsRaw,  SerialNo,  HSCodeNo,  ActiveStatus,  Trading,  NonStock,  ProductCode,  databaseName, customerId,"",0,connVM);
        }

        public IEnumerable<object> GetProductColumn()
        {
            try
            {
                string[] columnName = new string[] { "Product Name", "Product Code", "Serial No", "HS Code No"};
                IEnumerable<object> enumList = from e in columnName select new { Id = e.ToString().Replace(" ", ""), Name = e.ToString() };
                return enumList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ProductVM> GetProductByType(string type)
        {
            return new ProductDAL().GetProductByType(type,connVM);
        }

        public List<ProductVM> DropDown(int CategoryID=0, string IsRaw="")
        {
            try
            {
                return new ProductDAL().DropDown(CategoryID, IsRaw, connVM);

            }
            catch (Exception ex)
            {
                SymOrdinary.FileLogger.Log("ProductRepo", "DropDown", ex.Message + "\n" + ex.StackTrace);

                throw ex; 
            }
        }

        public List<ProductVM> DropDownProductByCategory(string catId)
        {
            return new ProductDAL().DropDownByCategory(catId,connVM);
        }

        public string[] InsertToProduct(ProductVM vm, List<TrackingVM> Trackings, string ItemType)
        {
            try
            {
                //return new ProductDAL().InsertToProduct(vm, Trackings, ItemType);
                return new ProductDAL().InsertToProduct(vm, null,null,false,null,null,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        public ResultVM IntegrationSyncProducts(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().ProductSync_ACI(vm,connVM);
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
                return new ProductDAL().UpdateProduct(vm, Trackings, ItemType,connVM);
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
                return new ProductDAL().Delete(vm, ids, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] DeleteToProductStock(ProductStockVM psVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new ProductDAL().DeleteToProductStock(psVM,null,null,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<ProductVM> SelectAll(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,ProductVM likeVM=null)
        {
            try
            {
                return new ProductDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, likeVM, connVM);
            }
            catch (Exception ex)
            {
                SymOrdinary.FileLogger.Log("ProductRepo", "SelectAll", ex.Message + "\n" + ex.StackTrace);

                throw ex;
            }
        }

        public List<ProductVM> SelectAllList(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, ProductVM likeVM = null)
        {
            try
            {
                return new ProductDAL().SelectAllList(conditionFields,conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        
        public List<ProductNameVM> SelectProductName(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, ProductVM likeVM = null)
        {
            try
            {
                return new ProductDAL().SelectProductName(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, likeVM,connVM);
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
                return new ProductDAL().SelectAllOverhead(Id, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable AvgPriceNew(string itemNo, string tranDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, bool isPost, bool Vat16 = true, bool Vat17 = true, bool transfer = true, string UserId="") 
        {
            return new ProductDAL().AvgPriceNew(itemNo, tranDate, VcurrConn, Vtransaction, isPost, Vat16, Vat17, transfer, connVM,UserId);
        }

        public DataTable GetLIFOPurchaseInformation(string itemNo, string receiveDate, string PurchaseInvoiceNo = "")
        {
            return new ProductDAL().GetLIFOPurchaseInformation(itemNo, receiveDate, PurchaseInvoiceNo,connVM);
        }

        public decimal GetLastNBRPriceFromBOM(string itemNo, string VatName, string effectDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, string CustomerID = "0")
        {
            return new ProductDAL().GetLastNBRPriceFromBOM_VatName(itemNo, VatName, effectDate, VcurrConn, Vtransaction, CustomerID,connVM);
        }

        //Cavinkare BOMReference 
        public decimal CavinkareGetLastNBRPriceFromBOM(string itemNo, string VatName, string effectDate, string FixBOMReferenceName,SqlConnection VcurrConn, SqlTransaction Vtransaction, string CustomerID = "0")
        {
            return new ProductDAL().CavinkareGetLastNBRPriceFromBOM(itemNo, VatName, effectDate,FixBOMReferenceName, VcurrConn, Vtransaction, CustomerID, connVM);
        }

        public string GetTransactionType(string itemNo, string effectDate)
        {
            try
            {
                return new ProductDAL().GetTransactionType(itemNo, effectDate,connVM);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public ProductVM GetProductWithCostPrice(string productCode, string purchaseNo, string effectDate)
        {
            try
            {
                return new ProductDAL().GetProductWithCostPrice(productCode, purchaseNo, effectDate, connVM);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public DataTable ProductDTByItemNo(string ItemNo, string ProductName = "")
        {
            try
            {
                return new ProductDAL().ProductDTByItemNo(ItemNo, ProductName,null,null,connVM);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public string[] InsertToProductName(ProductNameVM pVM)
        {
            try
            {
                return new ProductDAL().InsertToProductNames(pVM,null,null,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public DataTable SelectProductName(string Id = null)
        //{
        //    try
        //    {
        //        return new ProductDAL().SearchProductNames(Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public DataTable SelectProductName(string ItemNo)
        {
            try
            {
                return new ProductDAL().SearchProductNames(ItemNo,"","",connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] updateProductName(ProductNameVM pVM)
        {
            try
            {
                return new ProductDAL().UpdateToProductNames(pVM,null,null,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] DeleteProductName(string itemNo, string id)
        {
            try
            {
                return new ProductDAL().DeleteProductNames(itemNo, id,null,null,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //////public List<VehicleVM> SelectAll(int Id , string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, VehicleVM likeVM = null)
        //////{
        //////    try
        //////    {
        //////        return new VehicleDAL().SelectAllLst(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, null);
        //////    }
        //////    catch (Exception ex)
        //////    {

        //////        throw ex;
        //////    }
        //////}


        public DataSet GetReconsciliationData(string fromDate, string toDate)
        {
            try
            {
                return new ProductDAL().GetReconsciliationData(fromDate, toDate,null,null,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetBOMReferenceNo(string productCode, string vatName, string issueDatetime)
        {
            try
            {
                return new ProductDAL().GetBOMReferenceNo(productCode, vatName, issueDatetime,null,null,"0",connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InserToProductStock(ProductStockVM pVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            try
            {
                return new ProductDAL().InserToProductStock(pVM,null,null,connVM,UserId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] UpdateToProductStockWeb(ProductStockVM pVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            try
            {
                return new ProductDAL().UpdateToProductStockWeb(pVM, null, null, connVM, UserId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable SearchProductStock(string ItemNo, string id, List<UserBranchDetailVM> userBranchs)
        {
            try
            {
                return new ProductDAL().SearchProductStock(ItemNo, id,connVM,userBranchs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<UserBranchDetailVM> SelectAllLst(string UserId)
        {
            try
            {
                return new UserBranchDetailDAL().SelectAllLst(0, new[] { "uf.UserId" }, new[] { UserId },null,null,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public DataTable SelectBOMRaw(string itemNo, string effectDate)
        {
            return new ProductDAL().SelectBOMRaw(itemNo, effectDate,null,null,connVM);
        }



        public DataTable GetExcelData(List<string> Ids)
        {
            try
            {
                return new ProductDAL().GetExcelDataWeb(Ids, null, null, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetExcelProductDetails(List<string> Ids)
        {
            try
            {
                return new ProductDAL().GetExcelProductDetailsWeb(Ids, null, null, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM Delete6_1Permanent(string itemNo)
        {
            try
            {
                return new ProductDAL().Delete6_1Permanent(itemNo);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ResultVM Delete6_1Permanent_Branch(string itemNo)
        {
            try
            {
                return new ProductDAL().Delete6_1Permanent_Branch(itemNo);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataSet SelectNegInventoryData(string vattype)
        {
            try
            {
                return new ProductDAL().SelectNegInventoryData(vattype);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM Delete6_2Permanent(string itemNo)
        {
            try
            {
                return new ProductDAL().Delete6_2Permanent(itemNo);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ResultVM Delete6_2Permanent_Branch(string itemNo)
        {
            try
            {
                return new ProductDAL().Delete6_2Permanent_Branch(itemNo);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ResultVM Delete6_2_1Permanent(string itemNo)
        {
            try
            {
                return new ProductDAL().Delete6_2_1Permanent(itemNo);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ResultVM Delete6_2_1Permanent_Branch(string itemNo)
        {
            try
            {
                return new ProductDAL().Delete6_2_1Permanent_Branch(itemNo);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable GetUprocessCount()
        {
            try
            {
                return new ProductDAL().GetUprocessCount();

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM ProcessFreshStock(ParameterVM vm)
        {
            try
            {
                return new ProductDAL().ProcessFreshStock(vm);
            }
            catch (Exception)
            {

                throw;
            }
        }

        //public DataTable SelectNegInventoryData(string vattype)
        //{
        //    try
        //    {
        //        return new ProductDAL().SelectNegInventoryData(vattype);

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}

        

    }
}
