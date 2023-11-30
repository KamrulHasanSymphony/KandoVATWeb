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
    public class TenderRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public TenderRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public TenderRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }

        //private SymOrdinary.ShampanIdentity identity;

        //public TenderRepo(SymOrdinary.ShampanIdentity identity)
        //{
        //    // TODO: Complete member initialization
        //    this.identity = identity;
        //}
        public List<TenderMasterVM> SelectAll(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new TenderDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public List<TenderDetailVM> SelectAllDetails(string tenderId = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new TenderDAL().SelectAllDetails(tenderId, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] TenderInsert(TenderMasterVM Master, List<TenderDetailVM> Details, SqlTransaction transaction=null, SqlConnection currConn=null)
        {
            try
            {
                return new TenderDAL().TenderInsert(Master, Details, transaction, currConn,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] TenderUpdate(TenderMasterVM Master, List<TenderDetailVM> Details)
        {
            try
            {
                return new TenderDAL().TenderUpdate(Master, Details,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
