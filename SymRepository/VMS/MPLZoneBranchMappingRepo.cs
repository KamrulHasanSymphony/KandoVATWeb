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
    public class MPLZoneBranchMappingRepo
    {
        private SymOrdinary.ShampanIdentity identity;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public MPLZoneBranchMappingRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public MPLZoneBranchMappingRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }


        public List<MPLZoneBranchMappingVM> DropDown() 
        {
            try
            {
                return new MPLZoneBranchMappingDAL().DropDown(connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] InsertToMPLZoneBranchMapping(MPLZoneBranchMappingVM vm)
        {
            try
            {
                return new MPLZoneBranchMappingDAL().InsertToMPLZoneBranchMapping(vm, connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateMPLZoneBranchMapping(MPLZoneBranchMappingVM vm)
        {
            try
            {
                return new MPLZoneBranchMappingDAL().UpdateMPLZoneBranchMapping(vm, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<MPLZoneBranchMappingVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new MPLZoneBranchMappingDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MPLZoneBranchMappingVM> GetZoneCodeWise(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new MPLZoneBranchMappingDAL().GetZoneCodeWise(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] Delete(MPLZoneBranchMappingVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new MPLZoneBranchMappingDAL().Delete(vm, ids, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
