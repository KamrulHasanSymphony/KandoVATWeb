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
    public class SettingRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public SettingRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public SettingRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public SettingRepo()
        {
             connVM = null;
        }

        public List<SettingsVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new SettingDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] settingsDataUpdate(SettingsVM vm, SqlConnection VcurrConn=null, SqlTransaction Vtransaction=null) {
            return new SettingDAL().settingsDataUpdate(vm, VcurrConn,Vtransaction,connVM);
        }

        public List<SettingsVM> DropDownAll() {
            return new SettingDAL().DropDownAll(connVM);
        }


        public ResultVM DBUpdate(string CompanyId, int BranchId = 0, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            try
            {
                new SettingDAL().SettingsUpdate(CompanyId, BranchId, connVM);
                rVM.Status = "Success";
                rVM.Message = "DB Update Successfully!";
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return rVM;
        }

        public void SettingChange()
        {
            try
            {
                new CommonDAL().SettingChange(connVM);
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }


    }
}
