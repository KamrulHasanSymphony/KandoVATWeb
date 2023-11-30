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
    public class VAT7Repo
    {
         private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
         public VAT7Repo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

         public VAT7Repo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public List<VAT7VM> SelectAll(string Id= null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new VAT7DAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Vat7update(VAT7VM Master, List<VAT7VM> Details)
        {
            try
            {
                return new VAT7DAL().Vat7update(Master,Details,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] Vat7Insert(VAT7VM Master, List<VAT7VM> Details)
        {
            try
            {
                return new VAT7DAL().Vat7Insert(Master, Details,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] VAT7Post(VAT7VM Master, List<VAT7VM> Details)
        {
            try
            {
                return new VAT7DAL().VAT7Post(Master, Details,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }
}
