using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using VATServer.Library;
using VATViewModel.DTOs;

namespace SymRepository.VMS
{
    public class ImportRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public ImportRepo()
        {
            connVM = null;
        }
        public ImportRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public ImportRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }

        public string[] ImportExcelFile(ImportVM paramVM,DataTable dt = null, SysDBInfoVMTemp connVMs = null)
        {

            try
            {
                return new ImportDAL().ImportExcelFile(paramVM,null,connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<ErrorMessage> ImportExcelFile_Web(ImportVM paramVM)
        {

            try
            {
                return new ImportDAL().ImportExcelFile_Web(paramVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable GetProductACIDbData(DataTable conInfo)
        {
            try
            {
                string dbName = connVM.SysDatabaseName;
                return new ImportDAL().GetProductACIDbData(conInfo, dbName,connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable GetProductUnileverDbData(DataTable conInfo)
        {
            try
            {
                string dbName = connVM.SysDatabaseName;
                return new ImportDAL().GetProductUnileverDbData(conInfo, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] ImportProduct(List<ProductVM> products, List<TrackingVM> trackings, List<ProductVM> productDetails = null, DataTable productStocks = null)
        {
            try
            {
                return new ImportDAL().ImportProduct(products, trackings, productDetails, productStocks, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] ImportProductSync(List<ProductVM> products, List<TrackingVM> trackings, List<ProductVM> productDetails = null, DataTable productStocks = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new ImportDAL().InsertImportProductSync(products, trackings, productDetails, productStocks, connVM);
                //return new ImportDAL().ImportProductSync(products, trackings, productDetails, productStocks, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] UpdateACIMaster(List<string> ids, DataTable db, string tableName = "Vendors")
        {
            try
            {
                return new ImportDAL().UpdateACIMaster(ids, db, tableName,connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string[] UpdateUnileverMaster(List<string> ids, DataTable db, string tableName = "Vendors")
        {
            try
            {
                return new ImportDAL().UpdateUnileverMaster(ids, db, tableName, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string[] ImportCustomer(List<CustomerVM> customers)
        {
            try
            {
                return new ImportDAL().ImportCustomer(customers, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public DataTable GetCustomerACIDbData(DataTable conInfo)
        {
            try
            {
                return new ImportDAL().GetCustomerACIDbData(conInfo,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetCustomerUnileverDbData(DataTable conInfo)
        {
            try
            {
                return new ImportDAL().GetCustomerUnileverDbData(conInfo, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetVendorACIDbData(DataTable conInfo)
        {
            try
            {
                return new ImportDAL().GetVendorACIDbData(conInfo,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] ImportVendor(List<VendorVM> vendors)
        {
            try
            {
                return new ImportDAL().ImportVendor(vendors, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
