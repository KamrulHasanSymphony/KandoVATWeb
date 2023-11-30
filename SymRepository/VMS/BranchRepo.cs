using System;
using System.Collections.Generic;
using VATServer.Library;
using VATViewModel.DTOs;
using System.Data.SqlClient;
using SymOrdinary;
using System.Web;
using System.Data;

namespace SymRepository.VMS
{
    public class BranchRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public BranchRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public BranchRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public List<BranchVM> DropDown()
        {
            try
            {
                return new BranchDAL().DropDown(connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<string> Autocomplete(string term)
        {
            try
            {
                return new BranchProfileDAL().AutocompleteBranch(term, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<BranchProfileVM> DropDownBranchProfile()
        {
            try
            {
                return new BranchProfileDAL().DropDown(connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<BranchProfileVM> UserDropDownBranchProfile(int Id)
        {
            try
            {
                return new BranchProfileDAL().BranchDropDown(Id,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<BranchProfileVM> SelectAll(string Id=null , string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                BranchProfileDAL Dal = new BranchProfileDAL();
                List<BranchProfileVM> VMs = Dal.SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
                return VMs;
                //return new BranchProfileDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SelectAllBranch(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                BranchProfileDAL Dal = new BranchProfileDAL();
                DataTable dt = Dal.SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, true, connVM);
                return dt;
                //return new BranchProfileDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] InsertBranch(BranchProfileVM VM)
        {
            try
            {

                return new BranchProfileDAL().InsertToBranchProfileNew(VM,false,null,null,connVM);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        public string[] UpdateBranchInformation(BranchProfileVM VM)
        {
            try
            {
                string[] retResult = new string[6];
                BranchProfileDAL dal = new BranchProfileDAL();
                retResult = dal.UpdateToBranchProfileNew(VM,connVM);
                return retResult;
                //return new BranchProfileDAL().UpdateToBranchProfileNew(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] DeleteBranch(BranchProfileVM VM, string[] a,SqlConnection connection=null,SqlTransaction transaction=null)
        {
            try
            {
                return new BranchProfileDAL().Delete(VM, a, connection, transaction,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
