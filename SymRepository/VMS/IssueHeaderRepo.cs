using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using VATServer.Library;
using VATViewModel.DTOs;
using VMSAPI;

namespace SymRepository.VMS
{
    public class IssueHeaderRepo
    {

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public IssueHeaderRepo()
        {
            connVM = null;
        }
        public IssueHeaderRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public IssueHeaderRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }

        public string[] IssueInsert(IssueMasterVM Master, SqlTransaction transaction = null, SqlConnection currConn = null)
        {
            return new IssueDAL().IssueInsert(Master, Master.Details, transaction, currConn, connVM);
        }

        public string[] IssueUpdate(IssueMasterVM Master, string UserId = "")
        {
            return new IssueDAL().IssueUpdate(Master, Master.Details, connVM, UserId);
        }

        public string[] IssuePost(IssueMasterVM Master, string UserId = "")
        {
            return new IssueDAL().IssuePost(Master, Master.Details, null, null, connVM, UserId);

        }

        public List<IssueMasterVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM likeVM = null)
        {
            return new IssueDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, likeVM, connVM);
        }

        public List<IssueDetailVM> SelectIssueDetail(string issueNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            return new IssueDAL().SelectIssueDetail(issueNo, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
        }
        public string[] ImportExcelFile(IssueMasterVM paramVM)
        {
            try
            {
                return new IssueDAL().ImportExcelFile(paramVM, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] MultiplePost(string[] Ids)
        {
            try
            {
                return new IssueDAL().MultiplePost(Ids, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetExcelDataWeb(List<string> Ids)
        {
            try
            {
                return new IssueDAL().GetExcelDataWeb(Ids, null, null, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM UpdateAvgPrice_New(AVGPriceVm vm)
        {
            try
            {
                return new IssueDAL().UpdateAvgPrice_New(vm, null, null);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ResultVM UpdateAvgPrice_New_Refresh(AVGPriceVm vm)
        {
            try
            {
                return new IssueDAL().UpdateAvgPrice_New_Refresh(vm, null, null,connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] SaveVAT6_1_Permanent(VAT6_1ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new VATRegistersDAL().SaveVAT6_1_Permanent(vm, null, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] SaveVAT6_1_Permanent_Branch(VAT6_1ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new VATRegistersDAL().SaveVAT6_1_Permanent_Branch(vm, null, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] SaveVAT6_2_Permanent(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new VATRegistersDAL().SaveVAT6_2_Permanent(vm, null, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] SaveVAT6_2_Permanent_Branch(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new VATRegistersDAL().SaveVAT6_2_Permanent_Branch(vm, null, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] SaveVAT6_2_1_Permanent(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new IssueDAL().SaveVAT6_2_1_Permanent(vm, null, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] SaveVAT6_2_1_Permanent_Branch(VAT6_2ParamVM vm)
        {
            try
            {
                return new IssueDAL().SaveVAT6_2_1_Permanent_Branch(vm, null, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }


        #region API

        public string SaveIssue(string xml)
        {
            try
            {
                IssueAPI api = new IssueAPI();

                var result = api.SaveIssue(xml);

                return result;
            }
            catch (Exception e)
            {

                throw e;
            }
        }


        #endregion

        public string[] SaveVAT6_1_Permanent_Stored(VAT6_1ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new IssueDAL().SaveVAT6_1_Permanent_Stored(vm, null, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] SaveVAT6_1_Permanent_Stored_Branch(VAT6_1ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new IssueDAL().SaveVAT6_1_Permanent_Stored_Branch(vm, null, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] SaveVAT6_1_Permanent_DayWise(VAT6_1ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new IssueDAL().SaveVAT6_1_Permanent_DayWise(vm, null, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] SaveVAT6_1_Permanent_DayWise_Branch(VAT6_1ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new IssueDAL().SaveVAT6_1_Permanent_DayWise_Branch(vm, null, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] SaveVAT6_2_Permanent_Stored(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new IssueDAL().SaveVAT6_2_Permanent_Stored(vm, null, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] SaveVAT6_2_Permanent_Stored_Branch(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new IssueDAL().SaveVAT6_2_Permanent_Stored_Branch(vm, null, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] SaveVAT6_2_Permanent_DayWise(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new IssueDAL().SaveVAT6_2_Permanent_DayWise(vm, null, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] SaveVAT6_2_Permanent_DayWise_Branch(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new IssueDAL().SaveVAT6_2_Permanent_DayWise_Branch(vm, null, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
