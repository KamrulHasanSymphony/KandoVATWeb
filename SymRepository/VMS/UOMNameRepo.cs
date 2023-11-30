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
    public class UOMNameRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public UOMNameRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public UOMNameRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public string[] InsertToUOMName(UOMNameVM vm)
        {
            try
            {
                return new UOMNameDAL().InsertToUOMName(vm,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateUOMName(UOMNameVM vm)
        {
            try
            {
                return new UOMNameDAL().UpdateUOMName(vm,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] Delete(UOMNameVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new UOMNameDAL().Delete(vm, ids, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<UOMNameVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new UOMNameDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<UOMNameVM> DropDown()
        {
            try
            {
                return new UOMNameDAL().DropDown(connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
