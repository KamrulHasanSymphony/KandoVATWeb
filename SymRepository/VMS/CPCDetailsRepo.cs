using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using VATServer.Library;
using VATViewModel.DTOs;

namespace SymRepository.VMS
{
    public class CPCDetailsRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public CPCDetailsRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public CPCDetailsRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }

        public string[] InsertToCPCDetails(CPCDetailsVM vm)
        {
            try
            {
                return new CPCDetailsDAL().InsertToCPCDetails(vm, false, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] UpdateCPCDetails(CPCDetailsVM vm)
        {
            try
            {
                return new CPCDetailsDAL().UpdateCPCDetails(vm, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<CPCDetailsVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new CPCDetailsDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] Delete(CPCDetailsVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new CPCDetailsDAL().Delete(vm, ids, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<CPCDetailsVM> DropDown(string Type = "")
        {
            try
            {
                return new CPCDetailsDAL().DropDown(Type, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
