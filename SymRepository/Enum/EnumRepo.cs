using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Library;

namespace SymRepository.Enum
{
    public class EnumRepo
    {
        EnumDAL _enumDAL = new EnumDAL();
        #region VAT/VMS

        public IEnumerable<object> GetDepositTypeList()
        {
            try
            {
                return _enumDAL.GetDepositTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetTreasuryDepositTypeList()
        {
            try
            {
                return _enumDAL.GetTreasuryDepositTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetSDDepositTypeList()
        {
            try
            {
                return _enumDAL.GetSDDepositTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetPurchaseVATTypeList()
        {
            try
            {
                return _enumDAL.GetPurchaseVATTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetPurchaseVATTypeListWeb()
        {
            try
            {
                return _enumDAL.GetPurchaseVATTypeListWeb();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetEnumVATList()
        {
            try
            {
                return _enumDAL.GetEnumVATList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> RecordSelectList()
        {
            try
            {
                return _enumDAL.RecordSelectList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IEnumerable<object> GetEnumVATExportList()
        {
            try
            {
                return _enumDAL.GetEnumVATExportList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IEnumerable<object> GetPriceDeclarationList()
        {
            try
            {
                return _enumDAL.GetPriceDeclarationList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> MasterTableList()
        {
            try
            {
                return _enumDAL.MasterTableList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetTypeList()
        {
            try
            {
                return _enumDAL.GetTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> GetTransferIsuueTypes()
        {
            try
            {
                return _enumDAL.GetTransferIssueTypesList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GL
        public IEnumerable<object> GLStatusList()
        {
            try
            {
                return _enumDAL.GLStatusList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IEnumerable<object> GLDocumentTypeList()
        {
            try
            {
                return _enumDAL.GLDocumentTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public IEnumerable<object> GLGetAccountNatureList()
        {
            try
            {
                return _enumDAL.GLGetAccountNatureList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        public IEnumerable<object> SaleTypesList()
        {
            try
            {
                return _enumDAL.SaleTypesList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> DecisionList()
        {
            try
            {
                return _enumDAL.DecisionList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> AdjTypesList()
        {
            try
            {
                return _enumDAL.AdjTypesList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> PrinterList()
        {
            try
            {
                return _enumDAL.PrinterList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> VatTypesList()
        {
            try
            {
                return _enumDAL.VatTypesList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> productTypeList()
        {
            try
            {
                return _enumDAL.productTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> productTransferTypeList()
        {
            try
            {
                return _enumDAL.GetProductTransferTypes();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> DaysOfWeekList()
        {
            try
            {
                return _enumDAL.DaysOfWeekList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetStructureTypeList()
        {
            try
            {
                return _enumDAL.GetStructureTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Acc 

        public IEnumerable<object> GetPostStatusList()
        {
            try
            {
                return _enumDAL.GetPostStatusList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> GetAreaNameList()
        {
            try
            {
                return _enumDAL.GetAreaNameList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetAccountNatureList()
        {
            try
            {
                return _enumDAL.GetAccountNatureList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> GetReportTypeList()
        {
            try
            {
                return _enumDAL.GetReportTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> GetAccountTypeList()
        {
            try
            {
                return _enumDAL.GetAccountTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> GetTransactionTypeList()
        {
            try
            {
                return _enumDAL.GetTransactionTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetSaleCollectionGroupByList()
        {
            try
            {
                return _enumDAL.GetSaleCollectionGroupByList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetNonStockTypeList()
        {
            try
            {
                return _enumDAL.GetNonStockTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetGroupTypeList()
        {
            try
            {
                return _enumDAL.GetGroupTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetProductTypeList()
        {
            try
            {
                return _enumDAL.GetProductTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IEnumerable<object> GetLetterNameList()
        {
            try
            {
                return _enumDAL.GetLetterNameList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> AbsentDeductFromList()
        {
            try
            {
                return _enumDAL.AbsentDeductFromList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> DaysCountList()
        {
            try
            {
                return _enumDAL.DaysCountList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> GetSalaryHeadTypeList()
        {
            try
            {
                return _enumDAL.GetSalaryHeadTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetSalarySheetNameList()
        {
            try
            {
                return _enumDAL.GetSalarySheetNameList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetHoliDayTypeNameList()
        {
            try
            {
                return _enumDAL.GetHoliDayTypeNameList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetAttnStatusNameList()
        {
            try
            {
                return _enumDAL.GetAttnStatusNameList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public IEnumerable<object> GetLoanInterestPolicyList()
        {
            try
            {
                return _enumDAL.GetLoanInterestPolicyList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> GetSalaryProcessNameList()
        {
            try
            {
                return _enumDAL.GetSalaryProcessNameList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IEnumerable<object> GetEarningRptParamNameList()
        {
            try
            {
                return _enumDAL.GetEarningRptParamNameList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetDeductionRptParamNameList()
        {
            try
            {
                return _enumDAL.GetDeductionRptParamNameList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetPFTaxRptParamNameList()
        {
            try
            {
                return _enumDAL.GetPFTaxRptParamNameList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<string, string> VATReturnPartName()
        {
            try
            {
                CommonDAL _dal = new CommonDAL();

                return _dal.VATReturnPartName();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<string, string> VATReturnValue(string PartName)
        {
            try
            {
                CommonDAL _dal = new CommonDAL();

                return _dal.VATReturnValue(PartName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}
