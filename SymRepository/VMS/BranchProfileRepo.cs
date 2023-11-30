using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VATServer.Library;
using VATViewModel.DTOs;

namespace SymRepository.VMS
{
    public class BranchProfileRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public BranchProfileRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public BranchProfileRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public BranchProfileRepo()
        {
             connVM = null;
        }
        public List<BranchProfileVM> SelectAll(string Id = null, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                BranchProfileDAL Dal = new BranchProfileDAL();
                List<BranchProfileVM> VMs = Dal.SelectAllList(Id, conditionFields, conditionValues,null,null,connVM);
                return VMs;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public DataTable SelectAl(string Id = null, string[] conditionFields = null, string[] conditionValues = null, bool Dt = true)
        {
            try
            {
                return new BranchProfileDAL().SelectAll(Id, conditionFields, conditionValues, null, null, true,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
