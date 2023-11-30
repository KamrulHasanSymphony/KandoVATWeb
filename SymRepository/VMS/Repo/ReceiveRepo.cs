using SymServices.VMS.Library;
using SymViewModel.VMS.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace SymRepository.VMS.Repo
{
    public class ReceiveRepo
    {
        public string[] ReceiveInsert(ReceiveMasterVM Master, List<ReceiveDetailVM> Details, List<TrackingVM> Trackings, SqlTransaction transaction=null, SqlConnection currConn=null)
        {
            return new ReceiveDAL().ReceiveInsert(Master, Details, Trackings, transaction, currConn);
        }

        public List<ReceiveMasterVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new ReceiveDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<ReceiveDetailVM> SelectReceiveDetail(string ReceiveNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new ReceiveDAL().SelectReceiveDetail(ReceiveNo, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] ReceiveUpdate(ReceiveMasterVM Master, List<ReceiveDetailVM> Details, List<TrackingVM> Trackings)
        {
            return new ReceiveDAL().ReceiveUpdate(Master, Details, Trackings);
        }
    }
}
