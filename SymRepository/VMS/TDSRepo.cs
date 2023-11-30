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
    public class TDSRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public TDSRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public TDSRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        //public TDSRepo()
        //{
        //    // TODO: Complete member initialization
        //}

        public List<TDSsVM> SelectAll(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new TDSsDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertToTDS(TDSsVM vm)
        {
            try
            {
                return new TDSsDAL().InsertToTDSsNew(vm,false,null,null,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] UpdateToTDSsNew(TDSsVM vm)
        {
            try
            {
                return new TDSsDAL().UpdateToTDSsNew(vm,connVM);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(TDSsVM vm, string[] a)
        {
            try
            {
                return new TDSsDAL().Delete(vm, a,null,null,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
