using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymOrdinary;
using VATServer.Library;
using VATServer.Library.Integration;
using VATViewModel.DTOs;
using System.Web;

namespace SymRepository.VMS
{
    public class IntegrationRepo
    {

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        string UserID = "";
        public IntegrationRepo()
        {
            connVM = null;
        }

        public IntegrationRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            UserID = identity.UserId;
        }
        public IntegrationRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        #region BCL - Beximco Communication Ltd.
        // private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        // public IntegrationRepo(ShampanIdentity identity)
        //{
        //    connVM.SysDatabaseName = identity.InitialCatalog;
        //    connVM.SysUserName = SysDBInfoVM.SysUserName;
        //    connVM.SysPassword = SysDBInfoVM.SysPassword;
        //    connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        //}
        public DataTable GetSource_SaleData_Master_BCL_Trading(IntegrationParam vm)
        {
            try
            {
                return new BeximcoIntegrationDAL().GetSource_SaleData_Master(vm,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSale_DBData_BCL_Trading(IntegrationParam vm)
        {
            try
            {
                return new BeximcoIntegrationDAL().GetSaleData(vm,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSale_DBData_ACI(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;
                return new ImportDAL().GetACISaleData_Web(vm,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable GetACISaleEngineChassisData(string ID, string ItemCode, string BranchCode)
        {
            try
            {
                return new ImportDAL().GetACISaleEngineChassisData(ID, ItemCode, BranchCode, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetACISaleData_Web_CBHygine(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;
                return new ImportDAL().GetACISaleData_Web_CBHygine(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetPurchase_DBData_ACI(IntegrationParam vm)
        {
            try
            {
                ////return new ImportDAL().GetPurchaseACIDbData(vm, connVM);
                return new ImportDAL().GetACIPurchaseDataWeb(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetACI_CB_ElecticalPurchaseDataWeb(IntegrationParam vm)
        {
            try
            {
                ////return new ImportDAL().GetPurchaseACIDbData(vm, connVM);
                return new ImportDAL().GetACI_CB_ElecticalPurchaseDataWeb(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        



        public ResultVM SaveSale_BCL_Trading(DataTable dtSales, IntegrationParam vm)
        {
            try
            {
                return new BeximcoIntegrationDAL().SaveSale(dtSales, vm,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveCredit_BCL_Trading(IntegrationParam vm)
        {
            try
            {
                return new BeximcoIntegrationDAL().SaveCredit_Pre(vm,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        public ResultVM Multiple_SalePost(IntegrationParam paramVM)
        {
            try
            {
                return new BeximcoIntegrationDAL().PostSource_SaleData(paramVM,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        


        public ResultVM Multiple_SalePost_ACI(IntegrationParam paramVM)
        {
            try
            {
                return new ImportDAL().PostSource_SaleData(paramVM,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM Multiple_IssuePost_ACI(IntegrationParam paramVM)
        {
            try
            {
                return new ImportDAL().PostSource_IssueData(paramVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        #region Backup Methods
        
        ////public DataTable GetSource_CreditData_Master_BCL_Trading(IntegrationParam vm)
        ////{
        ////    try
        ////    {
        ////        return new BeximcoIntegrationDAL().GetSource_CreditData_Master(vm);

        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        throw ex;
        ////    }

        ////}


        ////public DataTable GetSource_CreditData_Detail_BCL_Trading(IntegrationParam vm)
        ////{
        ////    try
        ////    {
        ////        return new BeximcoIntegrationDAL().GetCreditData(vm);

        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        throw ex;
        ////    }

        ////}

        #endregion


        public DataTable GetSource_TransferData_Master_BCL_Trading(IntegrationParam vm)
        {
            try
            {
                return new BeximcoIntegrationDAL().GetSource_TransferData_Master(vm,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSource_TransferData_Master_ACI(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().GetSource_TransferData_Master(vm,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSource_TransferData_Detail_BCL_Trading(IntegrationParam vm)
        {
            try
            {
                return new BeximcoIntegrationDAL().GetTransferData(vm,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable GetSource_TransferData_Detail_ACI(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;
                //////return new ImportDAL().GetTransferData(vm,connVM);
                return new ImportDAL().GetTransferData_Web(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveTransfer_BCL_Trading(IntegrationParam vm)
        {
            try
            {
                return new BeximcoIntegrationDAL().SaveTransfer_Pre(vm,connVM);
                 
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public ResultVM SaveTransfer_ACI(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;

                return new ImportDAL().SaveTransfer_ACI(vm, connVM, UserID);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM Multiple_TransferPost(IntegrationParam paramVM)
        {
            try
            {
                return new BeximcoIntegrationDAL().PostSource_TransferData(paramVM,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public ResultVM Multiple_TransferPost_ACI(IntegrationParam paramVM)
        {
            try
            {
                return new ImportDAL().PostSource_TransferData(paramVM,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        public string[] SaveToTemp_BCL(DataTable dtSale, int BranchId, string transactionType, string token)
        {
            try
            {
                return new SaleDAL().SaveToTemp(dtSale, BranchId, transactionType, token,connVM);
            }
            catch (Exception e)
            {
                throw;
            }
        }


        public ResultVM SaveSale_BCL_Trading_Pre(IntegrationParam vm)
        {
            try
            {
                return new BeximcoIntegrationDAL().SaveSale_Pre(vm,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveSale_ACI(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;
                return new ImportDAL().SaveACISale_Web(vm,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

       

        public ResultVM SaveSale_SMC(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;
                vm.callBack = () => { };
                vm.SetSteps = (step) => { };
                
                return new SMCIntegrationDAL().SaveSale_Pre(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveSale_EON(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;
                vm.callBack = () => { };
                vm.SetSteps = (step) => { };

                return new EONIntegrationDAL().SaveSale_EON_Pre(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SavePurchase_EON(IntegrationParam vm)
        {
            try
            {
                //vm.SysDbInfoVmTemp = connVM;
                //vm.callBack = () => { };
                //vm.SetSteps = (step) => { };

                return new EONIntegrationDAL().SavePurchase_EON_Pre(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

       

        public ResultVM SaveTransfer_SMC(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;
                vm.callBack = () => { };
                vm.SetSteps = (step) => { };
                
                return new SMCIntegrationDAL().SaveTransfer_Pre(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SavePurchase_ACI(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().SaveACIPurchase_Web(vm,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public ResultVM SaveSale_BCL_Trading_Setp2(SaleMasterVM vm)
        {
            try
            {
                return new BeximcoIntegrationDAL().SaveSale_Setp2(vm,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        #endregion

        #region KCCL Intrigation

        public DataTable GetSaleKohinoorDbData(string invoiceNo, DataTable conInfo)
        {
            try
            {
                return new ImportDAL().GetSaleKohinoorDbData(invoiceNo, conInfo,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveSale_KCCL(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;
                return new ImportDAL().SaveKCCLSale_Web(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSource_TransferData_Master_KCCL(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().GetKCCLTransferData_Web(vm.RefNo,vm.BranchId,vm.TransactionType, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveTransfer_KCCL(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;

                return new ImportDAL().SaveKCCLTransfer_Web(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        #endregion

        #region SMC Integration

        public DataTable GetSource_PurchaseData_Master_SMC(IntegrationParam vm)
        {
            try
            {
                ////return new SMCIntegrationDAL().GetSource_PurchaseData_Master_SMC(vm, connVM);
                return new SMCIntegrationDAL().GetPurchaseDataMaster_SMC(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSMCPurchaseDetailDataWeb(IntegrationParam vm)
        {
            try
            {
                ////return new SMCIntegrationDAL().GetPurchaseSMC_DbData_Web(vm, connVM);
                return new SMCIntegrationDAL().GetPurchaseDataDetails_SMC(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SavePurchase_SMC(IntegrationParam vm)
        {
            try
            {
                return new SMCIntegrationDAL().SaveSMCPurchase_Web(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSource_IssueData_Master_SMC(IntegrationParam vm)
        {
            try
            {
                return new SMCIntegrationDAL().GetIssueSMCDbDataWeb(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable GetIssue_DBData_SMC(IntegrationParam vm)
        {
            try
            {
                return new SMCIntegrationDAL().GetIssue_DetailSMCDbDataWeb(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveIssue_SMC(IntegrationParam vm)
        {
            try
            {
                return new SMCIntegrationDAL().SaveSMCIssue_Web(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSource_ReceiveData_Master_SMC(IntegrationParam vm)
        {
            try
            {
                ////return new SMCIntegrationDAL().GetReceiveSMCDbDataWeb(vm, connVM);
                return new SMCIntegrationDAL().GetReceiveDbDataWeb_SMC(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable GetReceive_DBData_SMC(IntegrationParam vm)
        {
            try
            {
                ////return new SMCIntegrationDAL().GetReceive_DetailSMCDbDataWeb(vm, connVM);
                return new SMCIntegrationDAL().GetReceiveDetailDataWeb_SMC(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public ResultVM SaveReceive_SMC(IntegrationParam vm)
        {
            try
            {
                return new SMCIntegrationDAL().SaveSMCReceive_Web(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetTollReceiveDataMaster_SMC(IntegrationParam vm)
        {
            try
            {
                
                return new SMCIntegrationDAL().GetTollReceiveDataMaster_SMC(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetTollReceiveDataDetails_SMC(IntegrationParam vm)
        {
            try
            {
                
                return new SMCIntegrationDAL().GetTollReceiveDataDetails_SMC(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveSMCTollReceive_Web(IntegrationParam vm)
        {
            try
            {
                return new SMCIntegrationDAL().SaveSMCTollReceive_Web(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        #endregion

        #region Unilever Integration
        public DataTable GetSource_SaleData_Unilever(IntegrationParam vm)
        {
            try
            {
                return new UNILEVERIntegrationDAL().GetSource_SaleData_Master(vm, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable GetUnileverSaleDataDetails_Web(IntegrationParam vm)
        {
            try
            {
                return new UNILEVERIntegrationDAL().GetSource_SaleData_dis_Details(vm, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable GetUnileverSaleDataReturnDetails_Web(IntegrationParam vm)
        {
            try
            {
                return new UNILEVERIntegrationDAL().GetSource_SaleDataReturn_dis_Details(vm, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSource_SaleCNData_Unilever(IntegrationParam vm)
        {
            try
            {
                return new UNILEVERIntegrationDAL().GetSource_SaleCNData_Master(vm, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSource_SaleDNData_Unilever(IntegrationParam vm)
        {
            try
            {
                return new UNILEVERIntegrationDAL().GetSource_SaleDNData_Master(vm, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public ResultVM SaveSale_Unilever(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;
                return new UNILEVERIntegrationDAL().SaveUnileverSale_Web(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        public DataTable GetSource_DistinctPurchaseData_Master(IntegrationParam vm)
        {
            try
            {
                return new UNILEVERIntegrationDAL().GetSource_DistinctPurchaseData_Master(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSource_DistinctPurchaseReturnData_Master(IntegrationParam vm)
        {
            try
            {
                return new UNILEVERIntegrationDAL().GetSource_PurchaseReturnData_Master(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable GetSource_PurchaseReturnData_Master(IntegrationParam vm)
        {
            try
            {
                return new UNILEVERIntegrationDAL().GetSource_PurchaseReturnDetailsData_Master(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public DataTable GetSource_PurchaseDetailsData_Master(IntegrationParam vm)
        {
            try
            {
                ////return new ImportDAL().GetPurchaseACIDbData(vm, connVM);
                return new UNILEVERIntegrationDAL().GetSource_PurchaseDetailsData_Master(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SavePurchase_Unilever(IntegrationParam vm)
        {
            try
            {
                return new UNILEVERIntegrationDAL().SaveUnileverPurchase_Web(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion


        public DataTable GetSource_SaleData_Master_ACI(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().GetSource_SaleData_Master_ACI(vm,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSource_SaleData_Master_ACI_CBHygine(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().GetSource_SaleData_Master_ACI_CBHygine(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public DataTable GetSource_PurchaseData_Master_ACI(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().GetSource_PurchaseData_Master_ACI(vm,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSource_PurchaseData_Master_ACI_CB_ELECTRICAL(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().GetSource_PurchaseData_Master_ACI_CB_ELECTRICAL(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        public DataTable GetSource_ReceiveData_Master_ACI(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().GetReceiveACIDbDataWeb(vm,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable GetReceive_DBData_ACI(IntegrationParam vm)
        {
            try
            {
                //////return new ImportDAL().GetReceive_DetailACIDbDataWeb(vm, connVM);
                return new ImportDAL().GetReceive_DetailACIData_Web(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public ResultVM SaveReceive_ACI(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().SaveACIReceive_Web(vm,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSource_IssueData_Master_ACI(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().GetIssueACIDbDataWeb(vm,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable GetIssue_DBData_ACI(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().GetIssue_DetailACIDbDataWeb(vm,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveIssue_ACI(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().SaveACIIssue_Web(vm,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<BombaySaleDetailsVM> SelectAllBSMwareData(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, BombaySaleDetailsVM likeVM = null, string Orderby = "Y", string[] ids = null)
        {
            try
            {
                return new SaleDAL().SelectAllBSMwareDataList(Id, conditionFields, conditionValues, null, null);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable SelectAllBSMWDeatailData(BombaySaleDetailsVM vm)
        {
            try
            {
                return new SaleDAL().SelectAllBSMWDeatailData(vm.ID, null, null);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSaleData_EON(IntegrationParam vm)
        {
            try
            {
                return new EONIntegrationDAL().GetSaleData(vm, null, null, connVM, vm.Processed);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetPurchaseData_EON(IntegrationParam vm)
        {
            try
            {
                return new EONIntegrationDAL().GetPurchaseData(vm,connVM,null,null,vm.Processed);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetPIssueData_EON(IntegrationParam vm)
        {
            try
            {
                return new EONIntegrationDAL().GetPIssueData(vm,connVM,null,null,vm.Processed);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public ResultVM SavePIssue_EON(IntegrationParam vm)
        {
            try
            {

                return new EONIntegrationDAL().SavePIssue_EON_Pre(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetPReceiveData_EON(IntegrationParam vm)
        {
            try
            {
                return new EONIntegrationDAL().GetPReceiveData(vm,connVM,null,null,vm.Processed);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SavePReceive_EON(IntegrationParam vm)
        {
            try
            {

                return new EONIntegrationDAL().SavePReceive_EON_Pre(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetTransferData__EON(IntegrationParam vm)
        {
            try
            {
                return new EONIntegrationDAL().GetTransferData(vm, connVM,null,null,vm.Processed);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveTransferData_EON(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;
                vm.callBack = () => { };
                vm.SetSteps = (step) => { };

                return new EONIntegrationDAL().SaveTransferdata_EON_Web(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetCustomerEONAPIData(DataTable conInfo)
        {
            try
            {
                //return new ImportDAL().GetCustomerACIDbData(conInfo, connVM);
                return new EONIntegrationDAL().GetCustomerDataAPI(conInfo, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] SaveSale_BERGER(IntegrationParam vm)
        {
            try
            {
                //vm.SysDbInfoVmTemp = connVM;
                //vm.callBack = () => { };
                //vm.SetSteps = (step) => { };

                return new BergerIntegrationDAL().ProcessSalesData(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetPReceiveData_Berger(IntegrationParam vm)
        {
            try
            {
                return new BergerIntegrationDAL().GetPReceiveData(vm, null, null, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SavePReceive_Berger(IntegrationParam vm)
        {
            try
            {

                return new BergerIntegrationDAL().SavePReceive(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetTransferData_Berger(IntegrationParam vm)
        {
            try
            {
                return new BergerIntegrationDAL().GetTransferData(vm, connVM, null, null, vm.Processed);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveTransferData_Berger(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;
                vm.callBack = () => { };
                vm.SetSteps = (step) => { };

                return new BergerIntegrationDAL().SaveTransferdata_Berger_Web(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSaleData_Decathlon(IntegrationParam vm)
        {
            try
            {
                return new DecathlonIntegrationDAL().GetSaleData(vm, null, null, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveSale_Decathlon(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;
                vm.callBack = () => { };
                vm.SetSteps = (step) => { };

                return new DecathlonIntegrationDAL().SaveSale_Decathlon_Pre(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetPurchaseData_Decathlon(IntegrationParam vm)
        {
            try
            {
                return new DecathlonIntegrationDAL().GetPurchseDataAPI(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SavePurchase_Decathlon(IntegrationParam vm)
        {
            try
            {
                //vm.SysDbInfoVmTemp = connVM;
                //vm.callBack = () => { };
                //vm.SetSteps = (step) => { };

                return new DecathlonIntegrationDAL().SavePurchase_Decathlon_Pre(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetTransferData__Decathlon(IntegrationParam vm)
        {
            try
            {
                return new DecathlonIntegrationDAL().GetTransferData(vm, connVM, null, null, vm.Processed);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveTransferData_Decathlon(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;
                vm.callBack = () => { };
                vm.SetSteps = (step) => { };

                return new DecathlonIntegrationDAL().SaveTransferdata_Decathlon_Web(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSaleData_ShumiHotCake(IntegrationParam vm)
        {
            try
            {
                return new ShumiHotCakeIntegrationDAL().GetSaleData(vm, null, null, connVM, vm.Processed);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveSale_ShumiHotCake(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;
                vm.callBack = () => { };
                vm.SetSteps = (step) => { };

                return new ShumiHotCakeIntegrationDAL().SaveSale_ShumiHotCake_Pre(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        public DataTable GetSaleData_ShumiHotCakeCtg(IntegrationParam vm)
        {
            try
            {
                return new ShumiHotCakeIntegrationDAL().GetSaleDataCtg(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveSale_ShumiHotCakeCtg(IntegrationParam vm)

        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;
                vm.callBack = () => { };
                vm.SetSteps = (step) => { };

                return new ShumiHotCakeIntegrationDAL().SaveSale_ShumiHotCake_PreCtg(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSource_PurchaseData_Master_JAPFA(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().GetSource_PurchaseData_Master_JAPFA(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetPurchase_DBData_JAPFA(IntegrationParam vm)
        {
            try
            {
                ////return new ImportDAL().GetPurchaseACIDbData(vm, connVM);
                return new ImportDAL().GetJAPFAPurchaseDataWeb(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SavePurchase_JAPFA(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().SaveJAPFAPurchase_Web(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSource_IssueData_Master_JAPFA(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().GetIssueJAPFADbDataWeb(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetIssue_DBData_JAPFA(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().GetIssue_DetailJAPFADbDataWeb(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveIssue_JAPFA(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().SaveJAPFAIssue_Web(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSource_ReceiveData_Master_JAPFA(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().GetReceiveJAPFADbDataWeb(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetReceive_DBData_JAPFA(IntegrationParam vm)
        {
            try
            {
                //////return new ImportDAL().GetReceive_DetailACIDbDataWeb(vm, connVM);
                return new ImportDAL().GetReceive_DetailJAPFADbDataWeb(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveReceive_JAPFA(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().SaveJAPFAReceive_Web(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSource_SaleData_Master_JAPFA(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().GetSource_SaleData_Master_JAPFA(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSale_DBData_JAPFA(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;
                return new ImportDAL().GetJAPFASaleData_Web(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveSale_JAPFA(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;
                return new ImportDAL().SaveJAPFaSale_Web(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public DataTable GetSource_TransferData_Master_JAFPA(IntegrationParam vm)
        {
            try
            {
                return new ImportDAL().GetSource_TransferData_Master_JAPFA(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSource_TransferData_Detail_JAPFA(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;
                //////return new ImportDAL().GetTransferData(vm,connVM);
                return new ImportDAL().GetTransferData_JAPFA(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveTransfer_JAPFA(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;

                return new ImportDAL().SaveTransfer_JAPFA(vm, connVM, UserID);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetPurchaseData_Nourish(IntegrationParam vm)
        {
            try
            {
                return new NourishIntegrationDAL().GetPurchaseMaster(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetPurchase_DBData_Nourish(IntegrationParam vm)
        {
            try
            {
                ////return new ImportDAL().GetPurchaseACIDbData(vm, connVM);
                return new NourishIntegrationDAL().GetDetails(vm);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SavePurchase_Nourish(IntegrationParam vm)
        {
            try
            {
                //vm.SysDbInfoVmTemp = connVM;
                //vm.callBack = () => { };
                //vm.SetSteps = (step) => { };

                return new NourishIntegrationDAL().SavePurchase(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetPIssueData_Nourish(IntegrationParam vm)
        {
            try
            {
                return new NourishIntegrationDAL().GetIssueMaster(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetIssue_DBData_Nourish(IntegrationParam vm)
        {
            try
            {
                return new NourishIntegrationDAL().GetIssueDetails(vm);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveIssue_Nourish(IntegrationParam vm)
        {
            try
            {
                return new NourishIntegrationDAL().SaveIssue(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetPReceiveData_Nourish(IntegrationParam vm)
        {
            try
            {
                return new NourishIntegrationDAL().GetReceiveMaster(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetReceive_DBData_Nourish(IntegrationParam vm)
        {
            try
            {              
                return new NourishIntegrationDAL().GetReceiveDetails(vm);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveReceive_Nourish(IntegrationParam vm)
        {
            try
            {

                return new NourishIntegrationDAL().SaveReceive(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetTransferData__Nourish(IntegrationParam vm)
        {
            try
            {
                return new NourishIntegrationDAL().GetTransferMaster(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetSource_TransferData_Detail_Nourish(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;
                //////return new ImportDAL().GetTransferData(vm,connVM);
                return new NourishIntegrationDAL().GetTransferDetails(vm);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResultVM SaveTransferData_Nourish(IntegrationParam vm)
        {
            try
            {
                vm.SysDbInfoVmTemp = connVM;
                vm.callBack = () => { };
                vm.SetSteps = (step) => { };

                return new NourishIntegrationDAL().SaveReceive(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
