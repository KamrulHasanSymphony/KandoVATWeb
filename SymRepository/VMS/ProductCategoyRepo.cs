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
    public class ProductCategoryRepo
    {
         private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

         public ProductCategoryRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

         public ProductCategoryRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public List<ProductCategoryVM> DropDown(string IsRaw)
        {
            try
            {
                return new ProductCategoryDAL().DropDown(IsRaw,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<IdName> DropDownProductType()
        {
            try
            {
                return new ProductCategoryDAL().DropDownProductType(connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] InsertToProductCategory(ProductCategoryVM vm)
        {
            try
            {
                return new ProductCategoryDAL().InsertToProductCategory(vm,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateToExitProduct(ProductVM vm)
        {
            try
            {
                return new ProductDAL().UpdateToExitProduct(vm, null,null,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateProductCategory(ProductCategoryVM vm)
        {
            try
            {
                return new ProductCategoryDAL().UpdateProductCategory(vm,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<ProductCategoryVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null) 
        {
            try
            {
                return new ProductCategoryDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] Delete(ProductCategoryVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new ProductCategoryDAL().Delete(vm, ids, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
