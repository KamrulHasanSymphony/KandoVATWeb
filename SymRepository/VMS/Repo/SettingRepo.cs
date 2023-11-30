using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymServices.VMS.Library;
using SymViewModel.VMS.DTOs;
using System.Data;
using System.Data.SqlClient;

namespace SymRepository.VMS.Repo
{
    public class SettingRepo
    {
        public List<SettingsVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new SettingDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] settingsDataUpdate(SettingsVM vm, SqlConnection VcurrConn=null, SqlTransaction Vtransaction=null) {
            return new SettingDAL().settingsDataUpdate(vm, VcurrConn,Vtransaction);
        }

        public List<SettingsVM> DropDownAll() {
            return new SettingDAL().DropDownAll();
        }

    }
}
