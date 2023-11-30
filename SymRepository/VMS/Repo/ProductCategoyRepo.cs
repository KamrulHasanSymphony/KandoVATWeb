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
    public class ProductCategoryRepo
    {
        public List<ProductCategoryVM> DropDown()
        {
            try
            {
                return new ProductCategoryDAL().DropDown();
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
                return new ProductCategoryDAL().InsertToProductCategory(vm);
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
                return new ProductCategoryDAL().UpdateProductCategory(vm);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] DeleteCategoryInformation(string CategoryID, string databaseName)
        {
            try
            {
                return new ProductCategoryDAL().DeleteCategoryInformation(CategoryID, databaseName);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SearchProductCategory(string CategoryID, string CategoryName, string IsRaw, string ActiveStatus, string databaseName)
        {
            try
            {
                return new ProductCategoryDAL().SearchProductCategory(CategoryID, CategoryName, IsRaw, ActiveStatus, databaseName);
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
                return new ProductCategoryDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
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
                return new ProductCategoryDAL().Delete(vm, ids, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
