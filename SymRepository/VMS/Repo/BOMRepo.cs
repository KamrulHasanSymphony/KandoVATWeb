using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymServices.VMS.Library;
using SymViewModel.VMS.DTOs;
using System.Data;
using System.Data.SqlClient;

namespace SymRepository.VMS.Repo
{
    public class BOMRepo
    {
        public string[] BOMInsert(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster)
        {
            try
            {
                return new BOMDAL().BOMInsert(bomItems, bomOHs, bomMaster);
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
                return new BOMDAL().BOMImport(bomItems, bomOHs, bomMaster);
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
                return new BOMDAL().BOMUpdate(bomItems, bomOHs, bomMaster);
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
                return new BOMDAL().BOMPost(bomMaster);
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
                return new BOMDAL().SearchBOMMaster(finItem, vatName, effectDate, CustomerID);
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
                return new BOMDAL().SearchBOMRaw(finItem, vatName, effectDate, CustomerID);
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
                return new BOMDAL().SearchOH(finItem, vatName, effectDate, CustomerID);
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
                return new BOMDAL().SearchBOMMasterNew(BOMId);
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
                return new BOMDAL().SearchBOMRawNew(BOMId);
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
                return new BOMDAL().SearchOHNew(BOMId);
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
                return new BOMDAL().ServiceInsert(Details);
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
                return new BOMDAL().ServiceUpdate(Details);
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
                return new BOMDAL().ServicePost(Details);
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
                return new BOMDAL().ServiceDelete(Details);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SearchVAT1DTNew(string FinishItemName, string EffectDate, string VATName, string post, string PCode, string CustomerID = "0")
        {
            try
            {
                return new BOMDAL().SearchVAT1DTNew(FinishItemName, EffectDate, VATName, post, PCode, CustomerID);
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
                return new BOMDAL().SearchInputValues(FinishItemName, EffectDate, VATName, post, FinishItemNo);
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
                return new BOMDAL().SearchServicePrice(BOMId);
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
                return new BOMDAL().UseQuantityDT(FinishItemNo,Quantity, EffectDate, CustomerID );
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
                return new BOMDAL().FindBOMID(itemNo, VatName, effectDate, currConn, transaction, CustomerID);
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
                return new BOMDAL().FindBOMIDOverHead(itemNo, VatName, effectDate, currConn, transaction, CustomerID);
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
                return new BOMDAL().DeleteBOM(itemNo, VatName, effectDate, currConn, transaction, CustomerID);
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
                return new BOMDAL().FindCostingFrom(itemNo, effectDate, curConn, transaction, CustomerID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<BOMNBRVM> SelectAll(string BOMId = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new BOMDAL().SelectAll(BOMId, conditionFields, conditionValues, VcurrConn, Vtransaction);
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
                return new BOMDAL().SelectAllItems(BOMId, conditionFields, conditionValues, VcurrConn, Vtransaction);
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
                return new BOMDAL().SelectAllOverheads(BOMId, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
