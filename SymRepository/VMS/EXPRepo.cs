using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATViewModel.DTOs;
using VATServer.Library;
using SymRepository.VMS;
using SymRepository;
using System.Data.SqlClient;
using System.Data;
using SymOrdinary;
using System.Web;

namespace SymRepository.VMS
{
    public class EXPRepo
    {
         private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public EXPRepo()
        {
                connVM = null;
        }
        public EXPRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public EXPRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public List<SalesInvoiceExpVM> SelectAll(int Id  ,string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new SalesInvoiceExpDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertToTDS(SalesInvoiceExpVM vm)
        {
            try
            {
                return new SalesInvoiceExpDAL().InsertToSalesInvoiceExp(vm,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] UpdateToTDSsNew(SalesInvoiceExpVM vm)
        {
            try
            {
                return new SalesInvoiceExpDAL().UpdateSalesInvoiceExps(vm,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(SalesInvoiceExpVM vm, string[] a)
        {
            try
            {
                return new SalesInvoiceExpDAL().Delete(vm, a,null,null,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
