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
    public class AuditRepo
    {

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public AuditRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public AuditRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }


        public List<AuditVM> SelectAll(string Id = "0", string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new AuditDAL().SelectAllList(Id, conditionFields, conditionValues, null, null, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] ImportFile(AuditVM vm)
        {
            try
            {

                return new AuditDAL().ImportFile(vm, null, null, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
