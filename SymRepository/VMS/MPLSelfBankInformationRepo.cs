using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using VATServer.Library;
using VATViewModel.DTOs;
using SymOrdinary;
using System.Web;


namespace SymRepository.VMS
{
    public class MPLSelfBankInformationRepo
    {
        private SymOrdinary.ShampanIdentity identity;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public MPLSelfBankInformationRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public MPLSelfBankInformationRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }

        public List<MPLBankInformationVM> DropDown()
        {
            try
            {
                return new MPLSelfBankInformationDAL().DropDown(connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] InsertToMPLBankInformation(MPLBankInformationVM vm)
        {
            try
            {
                return new MPLSelfBankInformationDAL().InsertToMPLBankInformation(vm, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] UpdateMPLBankInformation(MPLBankInformationVM vm)
        {
            try
            {
                return new MPLSelfBankInformationDAL().UpdateMPLBankInformation(vm, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<MPLBankInformationVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new MPLSelfBankInformationDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] Delete(MPLBankInformationVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new MPLSelfBankInformationDAL().Delete(vm, ids, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
