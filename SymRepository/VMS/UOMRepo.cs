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
    public class UOMRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public UOMRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public UOMRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public string[] InsertToUOM(UOMConversionVM vm)
        {
            try
            {
                return new UOMDAL().InsertToUOMNew(vm,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateUOM(UOMConversionVM vm)
        {
            try
            {
                return new UOMDAL().UpdateUOM(vm,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] Delete(UOMConversionVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new UOMDAL().Delete(vm, ids, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<UOMConversionVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new UOMDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }
}
