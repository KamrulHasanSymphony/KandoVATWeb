using SymOrdinary;
using System;
using System.Data;
using System.Web;
using VATServer.Library;
using VATViewModel.DTOs;

namespace SymRepository.VMS
{
    public class BCLIntegrationRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public BCLIntegrationRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public BCLIntegrationRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public BCLIntegrationRepo()
        {
            connVM = null;
        }

        public DataTable LoadData(IntegrationParam param)
        {
            try
            {
                return new BCLIntegration().LoadData(param,connVM);
            }
            catch (Exception e)
            {
                throw;
            }
        }


        public string[] SaveSale(DataTable salesData, IntegrationParam param)
        {
            try
            {
                return new BCLIntegration().SaveSale(salesData, param,connVM);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public string[] SaveToTemp(DataTable dtSaleM, int BranchId, string transactionType, string token, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new SaleDAL().SaveToTemp(dtSaleM, BranchId, transactionType, token,connVM);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}