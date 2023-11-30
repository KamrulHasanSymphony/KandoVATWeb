using SymServices.VMS.Library;
using SymViewModel.VMS.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace SymRepository.VMS.Repo
{
    public class PurchaseRepo
    {
        public string[] PurchaseInsert(PurchaseMasterVM Master, List<PurchaseDetailVM> Details, List<PurchaseDutiesVM> Duties,List<TrackingVM> Trackings, SqlTransaction transaction=null, SqlConnection currConn=null)
        {
            return new PurchaseDAL().PurchaseInsert(Master, Details, Duties, Trackings, transaction, currConn);
        }

        public List<PurchaseMasterVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PurchaseDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<PurchaseDetailVM> SelectPurchaseDetail(string PurchaseInvoiceNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PurchaseDAL().SelectPurchaseDetail(PurchaseInvoiceNo, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] PurchaseUpdate(PurchaseMasterVM Master, List<PurchaseDetailVM> Details, List<PurchaseDutiesVM> Duties, List<TrackingVM> Trackings) 
        {
            return new PurchaseDAL().PurchaseUpdate(Master, Details, Duties, Trackings);
        }

        public List<PurchaseMasterVM> DropDown()
        {
            try
            {
                return new PurchaseDAL().DropDown();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
