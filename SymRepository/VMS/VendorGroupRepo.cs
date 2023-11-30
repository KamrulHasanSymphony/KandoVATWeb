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
    public class VendorGroupRepo
    {
        // private SymOrdinary.ShampanIdentity identity;
         private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
         public VendorGroupRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

         public VendorGroupRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public string[] InsertToVendorGroup(VendorGroupVM vm)
        {
            try
            {
                return new VendorGroupDAL().InsertToVendorGroup(vm,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateToVendorGroup(VendorGroupVM vm)
        {
            try
            {
                return new VendorGroupDAL().UpdateToVendorGroup(vm,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<VendorGroupVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new VendorGroupDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(VendorGroupVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new VendorGroupDAL().Delete(vm, ids, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<VendorGroupVM> DropDown()
        {
            try
            {
                return new VendorGroupDAL().DropDown(connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
