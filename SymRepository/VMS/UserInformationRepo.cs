using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Web;
using SymOrdinary;
using VATViewModel.DTOs;
using VATServer.Library;

namespace SymRepository.VMS
{
    public class UserInformationRepo
    {

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public UserInformationRepo(ShampanIdentity identity)
        {
            //////ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            ////DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

        }
        public UserInformationRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public UserInformationRepo()
        {
            //////ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            //////DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
        }
        public List<UserInformationVM> SelectForLogin(LoginVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new UserInformationDAL().SelectForLogin(vm, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<UserInformationVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new UserInformationDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] InsertToUserInformationNew(UserInformationVM vm)
        {
            try
            {
                return new UserInformationDAL().InsertToUserInformationNew(vm, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<UserInformationVM> DropDown()
        {
            try
            {
                return new UserInformationDAL().DropDown(connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] UpdateUserPasswordNew(string UserName, string UserPassword, string LastModifiedBy, string LastModifiedOn, string databaseName)
        {
            try
            {
                return new UserInformationDAL().UpdateUserPasswordNew(UserName, UserPassword, LastModifiedBy, LastModifiedOn, databaseName, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
         public  void WorngAttemptProcess(bool IsUser=false,string UserName="")
        {
            try
            {
                new UserInformationDAL().WorngAttemptProcess(IsUser, UserName);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
         public  void UpdateUserWromgAttempt(string UserName="")
        {
            try
            {
                new UserInformationDAL().UpdateUserWromgAttempt( UserName);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        
    }
}
