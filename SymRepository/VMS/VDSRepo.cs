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
    public class VDSRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public VDSRepo()
        {
                connVM = null;
        }
        public VDSRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public VDSRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
		
		
        public List<VDSMasterVM> SelectVDSDetail(string VDSId, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,string TransectionType="")
        {
            try
            {
                return new VDSDAL().SelectVDSDetail(VDSId, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM, TransectionType);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
