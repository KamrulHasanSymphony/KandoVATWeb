using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymServices.Sage;
using SymViewModel.Sage;
using System.Data;

namespace SymRepository.Sage
{
    public class GLSettingRepo
    {
                GLSettingDAL _settingDAL= new GLSettingDAL();
        public DataSet SearchSetting()
        {
            DataSet dataSet = new DataSet("SearchSetting");
            try
            {
                dataSet = _settingDAL.SearchSettings();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dataSet;
        }
        public string[] SettingsUpdate(List<GLSettingVM> vm)
        {
            try
            {
                return _settingDAL.SettingsUpdate(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] settingsDataUpdate(GLSettingVM vm)
        {
            string[] result = new string[6];
            try
            {
                result = _settingDAL.settingsDataUpdate(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<GLSettingVM> SettingsAll(int branchID = 0)
        {
            List<GLSettingVM> SettingsAll ;
            try
            {
                SettingsAll = _settingDAL.SettingsAll(branchID);
            }
            catch (Exception ex)
            {
                throw;
            }
            return SettingsAll;
        }
        public string settingValue(string settingGroup, string settingName)
          {  string retResults = "0";
            try
            {
                retResults = _settingDAL.settingValue(settingGroup, settingName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #region Results
            return retResults;
            #endregion
        }

        public string[] settingsDataInsert(GLSettingVM vm, string settingGroup, string settingName, string settingType, string settingValue)
        {
            string[] retResults = new string[6];
            try
            {
                retResults = _settingDAL.settingsDataInsert( vm,settingGroup, settingName, settingType, settingValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #region Results
            return retResults;
            #endregion
        }

        public decimal FormatingNumeric(decimal value, int DecPlace)
        {
            object outPutValue = 0;
            try
            {
                outPutValue=FormatingNumeric( value,  DecPlace);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Convert.ToDecimal(outPutValue);
        }
    }
}
