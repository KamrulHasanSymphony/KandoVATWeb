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
    public class VendorRepo
    {
         private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
         public VendorRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
         public VendorRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
         
        public List<VendorVM> DropDown() {
            try
            {
                return new VendorDAL().DropDown(connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string[] InsertToVendorNewSQL(VendorVM vm)
        {
            try
            {
                return new VendorDAL().InsertToVendorNewSQL(vm,false,null,null,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateVendorNewSQL(VendorVM vm)
        {
            try
            {
                return new VendorDAL().UpdateVendorNewSQL(vm,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        public ResultVM IntegrationSyncVendors(IntegrationParam vm)
        {
            try
            {
                //return new ProductDAL().InsertToProduct(vm, Trackings, ItemType);
                return new ImportDAL().VendorSync_ACI(vm,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public List<VendorVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new VendorDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] Delete(VendorVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new VendorDAL().Delete(vm, ids, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public IEnumerable<object> GetVendorColumn()
        {
            try
            {
                string[] columnName = new string[] { "Vendor Code", "Vendor Name", "Address 1", "TIN No", "Contact Person", "VAT Registration No" };
                IEnumerable<object> enumList = from e in columnName select new { Id = e.ToString().Replace(" ", ""), Name = e.ToString() };
                return enumList;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public DataTable GetExcelData(List<string> Ids)
        {
            try
            {
                return new VendorDAL().GetExcelData(Ids, null, null, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
