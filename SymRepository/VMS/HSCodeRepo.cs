using System;
using System.Collections.Generic;
using VATViewModel.DTOs;
using VATServer.Library;
using System.Data.SqlClient;
using SymOrdinary;
using System.Web;
using System.Linq;



namespace SymRepository.VMS
{
    public class HSCodeRepo
    {
         private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        // private SymOrdinary.ShampanIdentity identity;
         public HSCodeRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }


         public HSCodeRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
         //public HSCodeRepo()
         //{
         //    // TODO: Complete member initialization
         //}

         //public HSCodeRepo(SymOrdinary.ShampanIdentity identity)
         //{
         //    // TODO: Complete member initialization
         //    this.identity = identity;
         //}


        public string[] InsertToHSCode(HSCodeVM vm)
        {
            try
            {
                return new HSCodeDAL().InsertToHSCode(vm,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] UpdateToHSCode(HSCodeVM vm)
        {
            try
            {
                return new HSCodeDAL().UpdateHSCode(vm,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public List<HSCodeVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new HSCodeDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(HSCodeVM vm, string[] a)
        {
            try
            {
                return new HSCodeDAL().Delete(vm,a,null,null,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetHSCodeColumn()
        {
            try
            {
                string[] columnName = new string[] { "Code", "HSCode" };
                IEnumerable<object> enumList = from e in columnName select new { Id = e.ToString().Replace(" ", ""), Name = e.ToString() };
                return enumList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        
    }
}
