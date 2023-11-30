using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using SymOrdinary;
using SymphonySofttech.Utilities;
using SymRepository.VMS;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace SymVATWebUI.Areas.API
{
    public class AuthHelper
    {
        public void Authenticate(SaleAPIVM vm)
        {
            SymphonyVATSysCompanyInformationRepo _sysComRepo = new SymphonyVATSysCompanyInformationRepo();
            CommonRepo _commonRepo = new CommonRepo();
            ;

            UserInformationRepo _userRepo = new UserInformationRepo();

            _commonRepo.SuperInformationFileExist(AppDomain.CurrentDomain.BaseDirectory);


            var enBin = Converter.DESEncrypt(DBConstant.PassPhrase, DBConstant.EnKey, vm.Bin.Trim());

            var sysInfo = _sysComRepo.SelectAll(null, new[] { "Bin" }, new[] { enBin }).FirstOrDefault();

            if (sysInfo == null)
            {
                throw new Exception("BIN does not exist");
            }

            var dbName =
                sysInfo.DatabaseName; //Converter.DESDecrypt(DBConstant.PassPhrase, DBConstant.EnKey, sysInfo.DatabaseName);

            List<UserInformationVM> vms = _userRepo.SelectForLogin(new LoginVM()
            {
                DatabaseName = dbName,
                UserName = vm.UserName,
                UserPassword = vm.Password
            });

            if (!vms.Any())
            {
                throw new Exception("Wrong UserName/Password");
            }

            _commonRepo.LoginSuccess(dbName);

            ReportDSRepo reportDsdal = new ReportDSRepo();
            DataSet ReportResult = reportDsdal.ComapnyProfileString(sysInfo.CompanyID, vms[0].UserID);

            settingVM.SettingsDT = ReportResult.Tables[2];
            settingVM.SettingsDTUser = ReportResult.Tables[3];
            settingVM.UserInfoDT = ReportResult.Tables[4];
        }
    }
}