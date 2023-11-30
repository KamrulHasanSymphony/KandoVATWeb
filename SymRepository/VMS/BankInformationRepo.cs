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
    public class BankInformationRepo
    {
        private SymOrdinary.ShampanIdentity identity;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public BankInformationRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public BankInformationRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        
        //public BankInformationRepo(SymOrdinary.ShampanIdentity identity)
        //{
        //    // TODO: Complete member initialization
        //    this.identity = identity;
        //}
        public List<BankInformationVM> DropDown() 
        {
            try
            {
                return new BankInformationDAL().DropDown(connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] InsertToBankInformation(BankInformationVM vm)
        {
            try
            {
                return new BankInformationDAL().InsertToBankInformation(vm,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateBankInformation(BankInformationVM vm)
        {
            try
            {
                return new BankInformationDAL().UpdateBankInformation(vm,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<BankInformationVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new BankInformationDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] Delete(BankInformationVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new BankInformationDAL().Delete(vm, ids, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
