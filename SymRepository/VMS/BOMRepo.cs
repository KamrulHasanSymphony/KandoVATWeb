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
    public class BOMRepo
    {
         private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
         public BOMRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
         public BOMRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        //private SymOrdinary.ShampanIdentity identity;

        //public BOMRepo(SymOrdinary.ShampanIdentity identity)
        //{
        //    // TODO: Complete member initialization
        //    this.identity = identity;
        //}
        public string[] BOMPreInsert(BOMNBRVM vm)
        {
            try
            {
                return new BOMDAL().BOMPreInsert(vm,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] BOMInsert(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster)
        {
            try
            {
                return new BOMDAL().BOMInsert(bomItems, bomOHs, bomMaster,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] BOMImport(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster)
        {
            try
            {
                return new BOMDAL().BOMImport(bomItems, bomOHs, bomMaster,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] BOMUpdate(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster)
        {
            try
            {
                return new BOMDAL().BOMUpdate(bomItems, bomOHs, bomMaster,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] BOMPost(BOMNBRVM bomMaster)
        {
            try
            {
                return new BOMDAL().BOMPost(bomMaster,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SearchBOMMaster(string finItem, string vatName, string effectDate, string CustomerID = "0")
        {
            try
            {
                return new BOMDAL().SearchBOMMaster(finItem, vatName, effectDate, CustomerID,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SearchBOMRaw(string finItem, string vatName, string effectDate, string CustomerID = "0")
        {
            try
            {
                return new BOMDAL().SearchBOMRaw(finItem, vatName, effectDate, CustomerID,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SearchOH(string finItem, string vatName, string effectDate, string CustomerID = "0")
        {
            try
            {
                return new BOMDAL().SearchOH(finItem, vatName, effectDate, CustomerID,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SearchBOMMasterNew(string BOMId)
        {
            try
            {
                return new BOMDAL().SearchBOMMasterNew(BOMId,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SearchBOMRawNew(string BOMId)
        {
            try
            {
                return new BOMDAL().SearchBOMRawNew(BOMId,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SearchOHNew(string BOMId)
        {
            try
            {
                return new BOMDAL().SearchOHNew(BOMId,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] ServiceInsert(List<BOMNBRVM> Details)
        {
            try
            {
                return new BOMDAL().ServiceInsert(Details,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] ServiceUpdate(List<BOMNBRVM> Details)
        {
            try
            {
                return new BOMDAL().ServiceUpdate(Details,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] ServicePost(List<BOMNBRVM> Details)
        {
            try
            {
                return new BOMDAL().ServicePost(Details,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] ServiceDelete(List<BOMNBRVM> Details)
        {
            try
            {
                return new BOMDAL().ServiceDelete(Details,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SearchInputValues(string FinishItemName, string EffectDate, string VATName, string post, string FinishItemNo)
        {
            try
            {
                return new BOMDAL().SearchInputValues(FinishItemName, EffectDate, VATName, post, FinishItemNo,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SearchServicePrice(string BOMId)
        {
            try
            {
                return new BOMDAL().SearchServicePrice(BOMId,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable UseQuantityDT(string FinishItemNo, decimal Quantity, string EffectDate, string CustomerID = "0")
        {
            try
            {
                return new BOMDAL().UseQuantityDT(FinishItemNo,Quantity, EffectDate, CustomerID,connVM );
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string FindBOMID(string itemNo, string VatName, string effectDate, SqlConnection currConn,SqlTransaction transaction, string CustomerID = "0")
        {
            try
            {
                return new BOMDAL().FindBOMID(itemNo, VatName, effectDate, currConn, transaction, CustomerID,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string FindBOMIDOverHead(string itemNo, string VatName, string effectDate, SqlConnection currConn, SqlTransaction transaction, string CustomerID = "0")
        {
            try
            {
                return new BOMDAL().FindBOMIDOverHead(itemNo, VatName, effectDate, currConn, transaction, CustomerID,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] DeleteBOM(string itemNo, string VatName, string effectDate, SqlConnection currConn, SqlTransaction transaction, string CustomerID = "0")
        {
            try
            {
                return new BOMDAL().DeleteBOM(itemNo, VatName, effectDate, currConn, transaction, CustomerID,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] FindCostingFrom(string itemNo, string effectDate, SqlConnection curConn, SqlTransaction transaction, string CustomerID = "0")
        {
            try
            {
                return new BOMDAL().FindCostingFrom(itemNo, effectDate, curConn, transaction, CustomerID,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<BOMNBRVM> SelectAll(string BOMId = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, BOMNBRVM likeVM = null)
        {
            try
            {
                return new BOMDAL().SelectAllList(BOMId, conditionFields, conditionValues, VcurrConn, Vtransaction, likeVM,connVM);
            }
            catch (Exception e)
            {
                
                throw e;
            }
        }
        public List<BOMNBRVM> SelectPreviousBOM(BOMNBRVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, BOMNBRVM likeVM = null)
        {
            try
            {
                return new BOMDAL().SelectPreviousBOM(vm, conditionFields, conditionValues, VcurrConn, Vtransaction, likeVM, connVM);
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        public List<BOMItemVM> SelectAllItems(string BOMId = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new BOMDAL().SelectAllItems(BOMId, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<BOMOHVM> SelectAllOverheads(string BOMId = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new BOMDAL().SelectAllOverheads(BOMId, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable GetBOMExcelData(List<string> Ids)
        {
            try
            {
                return new BOMDAL().GetBOMExcelData(Ids, null, null, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

         public DataTable GetCompareData(List<string> BOMIdList,bool isOverhead = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new BOMDAL().GetCompareData(BOMIdList, isOverhead, null, null, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


         public string[] ImportFile(BOMNBRVM vm)
         {
             try
             {

                 return new BOMDAL().ImportFile(vm, null, null, connVM);
             }
             catch (Exception ex)
             {

                 throw ex;
             }
         }
    }
}
