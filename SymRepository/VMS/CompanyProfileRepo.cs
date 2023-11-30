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
    public class CompanyProfileRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public CompanyProfileRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public CompanyProfileRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public CompanyProfileRepo( )
        {
            connVM = null;
        }

            
        //private SymOrdinary.ShampanIdentity identity;

        //public CompanyProfileRepo(SymOrdinary.ShampanIdentity identity)
        //{
        //    // TODO: Complete member initialization
        //    this.identity = identity;
        //}


        public string[] UpdateCompanyProfileNew(CompanyProfileVM companyProfiles)
        {
            try
            {
                return new CompanyprofileDAL().UpdateCompanyProfileNew(companyProfiles,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public List<CompanyProfileVM> SelectAll(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new CompanyprofileDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
