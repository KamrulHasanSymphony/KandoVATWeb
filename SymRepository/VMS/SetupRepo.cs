using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using VATServer.Library;
using VATViewModel.DTOs;

namespace SymRepository.VMS
{
    public class SetupRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public SetupRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public DataSet ResultVATStatus(DateTime StartDate, string databaseName) {
            try
            {
                return new SetupDAL().ResultVATStatus(StartDate, databaseName,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }
}
