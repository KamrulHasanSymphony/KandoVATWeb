using SymServices.VMS.Library;
using SymViewModel.VMS.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace SymRepository.VMS.Repo
{
    public class SaleInvoiceRepo
    {
        public string[] SalesInsert(SaleMasterVM Master, List<SaleDetailVM> Details, List<SaleExportVM> ExportDetails, List<TrackingVM> Trackings, SqlTransaction transaction=null, SqlConnection currConn=null) {
            return new SaleDAL().SalesInsert( Master,  Details,  ExportDetails, Trackings, transaction, currConn);
        }
        public List<SaleMasterVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null) {
            try
            {
                return new SaleDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<SaleDetailVM> SelectSaleDetail(string saleInvoiceNo,string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new SaleDAL().SelectSaleDetail(saleInvoiceNo,conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] SalesUpdate(SaleMasterVM Master, List<SaleDetailVM> Details, List<SaleExportVM> ExportDetails, List<TrackingVM> Trackings) 
        {
            return new SaleDAL().SalesUpdate(Master, Details, ExportDetails, Trackings);
        }
    }
}
