using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using VATServer.Library;
using VATViewModel.DTOs;
using SymOrdinary;
using System.Web;


namespace SymRepository.VMS
{
    public class MPLZoneProfileRepo
    {
        private SymOrdinary.ShampanIdentity identity;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public MPLZoneProfileRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public MPLZoneProfileRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }


        public List<MPLZoneProfileVM> DropDown() 
        {
            try
            {
                return new MPLZoneProfileDAL().DropDown(connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] InsertToMPLZoneProfile(MPLZoneProfileVM vm)
        {
            try
            {
                return new MPLZoneProfileDAL().InsertToMPLZoneProfile(vm, connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateMPLZoneProfile(MPLZoneProfileVM vm)
        {
            try
            {
                return new MPLZoneProfileDAL().UpdateMPLZoneProfile(vm, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<MPLZoneProfileVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new MPLZoneProfileDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] Delete(MPLZoneProfileVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new MPLZoneProfileDAL().Delete(vm, ids, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
