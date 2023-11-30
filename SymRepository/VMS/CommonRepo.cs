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
    public class CommonRepo
    {

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public CommonRepo()
        {
            connVM = null;
        }

        public CommonRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
        public CommonRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public DataTable Select(ParameterVM paramVM)
        {
            try
            {
                return new CommonDAL().Select(paramVM, null, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public void AddBranchInfo()
        {
            try
            {

                new CommonDAL().AddBranchInfo(connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void AddUserInfo()
        {
            try
            {

                new CommonDAL().AddUserInfo(connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }



        public string GetTargetId(string tableName, string columnName, string currentId, string btn)
        {
            try
            {
                return new CommonDAL().GetTargetId(tableName, columnName, currentId, btn, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetTargetIdForTtype(string tableName, string columnName, string currentId, string btn, string ttype)
        {
            try
            {
                return new CommonDAL().GetTargetIdForTtype(tableName, columnName, currentId, btn, ttype, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<IdName> IdNameDropdown(string tableName, string Id, string Name, string AllData, string code)
        {
            try
            {
                return new CommonDAL().IdNameDropdown(tableName, Id, Name, AllData, code, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<IdName> IdNameDropdownOverhead(string Id, string Name, string AllData, string code)
        {
            try
            {
                return new CommonDAL().IdNameDropdownOverhead(Id, Name, AllData, code, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<IdName> IdNameTtype(string tableName, string Id, string Name, string type, string code)
        {
            try
            {
                return new CommonDAL().IdNameTtype(tableName, Id, Name, type, code, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string settings(string SettingGroup, string SettingName)
        {
            try
            {
                return new CommonDAL().settings(SettingGroup, SettingName, null, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string settingsDesktop(string SettingGroup, string SettingName)
        {
            try
            {
                return new CommonDAL().settingsDesktop(SettingGroup, SettingName, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string settingsMaster(string SettingGroup, string SettingName)
        {
            try
            {
                return new CommonDAL().settingsMaster(SettingGroup, SettingName, null, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void settingsUpdateMaster(string SettingGroup, string SettingName, string settingValue)
        {
            try
            {
                new CommonDAL().settingsUpdateMaster(SettingGroup, SettingName, settingValue, null, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ResultVM UpdateProcessFlag(string flag = "N")
        {
            try
            {
                return new CommonDAL().UpdateProcessFlag("N", null, null);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ResultVM CheckProcessFlag(string itemNo, string CategoryID, string type)
        {
            try
            {
                return new CommonDAL().CheckProcessFlag(itemNo, CategoryID, type, null, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string settingValue(string SettingGroup, string SettingName)
        {
            try
            {
                return new CommonDAL().settingValue(SettingGroup, SettingName, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public bool SuperInformationFileExist()
        {
            try
            {
                return new CommonDAL().SuperInformationFileExist(connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool LoginSuccess(string DatabaseName)
        {
            try
            {
                return new CommonDAL().LoginSuccess(DatabaseName, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string ServerDateTime()
        {
            try
            {
                return new CommonDAL().ServerDateTime(connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public bool SuperInformationFileExist(string path)
        {
            try
            {
                return new CommonDAL().SuperInformationFileExist(path, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable SuperAdministrator()
        {
            try
            {
                return new CommonDAL().SuperAdministrator(connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] ErrorLogs(ErrorLogVM vm)
        {
            try
            {
                return new CommonDAL().InsertErrorLogs(vm, null, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
