using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VATServer.Library;
using VATViewModel.DTOs;

namespace SymRepository.VMS
{
    public class SettingMasterRepo
    {
         private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public SettingMasterRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public SettingMasterRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public SettingMasterRepo()
        {
             connVM = null;
        }

        public List<SettingsVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new SettingDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] SettingsUpdatelistMaster(List<SettingsVM> vms, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            return new SettingDAL().SettingsUpdatelistMaster(vms, connVM);
        }

        public List<SettingsVM> DropDownSettingMaster()
        {
            return new SettingDAL().DropDownSettingMaster(connVM);
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

        public void SettingChangeMaster()
        {
            try
            {
                new CommonDAL().SettingChangeMaster(connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
