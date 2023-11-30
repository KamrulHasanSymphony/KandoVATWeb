using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using VATServer.Library;
using VATViewModel.DTOs;
using VMSAPI;
using System.Linq;

namespace SymRepository.VMS
{
    public class ReceiveRepo
    {

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public ReceiveRepo()
        {
               connVM = null;
        }
        public ReceiveRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public ReceiveRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }

        public string[] ReceiveInsert(ReceiveMasterVM Master, List<ReceiveDetailVM> Details, List<TrackingVM> Trackings, SqlTransaction transaction=null, SqlConnection currConn=null,string UserId = "")
        {
            return new ReceiveDAL().ReceiveInsert(Master, Details, Trackings, transaction, currConn, 0, connVM,UserId);
        }

        public List<ReceiveMasterVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,ReceiveMasterVM likeVM=null)
        {
            try
            {
                return new ReceiveDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction,likeVM,connVM);
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
                return new ReceiveDAL().SelectReceiveDetail(ReceiveNo, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] ReceiveUpdate(ReceiveMasterVM Master, List<ReceiveDetailVM> Details, List<TrackingVM> Trackings,string UserId = "")
        {
            return new ReceiveDAL().ReceiveUpdate(Master, Details, Trackings, connVM, null, null, UserId);
        }

        public string[] ReceivePost(ReceiveMasterVM Master, List<ReceiveDetailVM> Details, List<TrackingVM> Trackings,string UserId = "")
        {
            try
            {
                return new ReceiveDAL().ReceivePost(Master, Details, Trackings, null, null, connVM, UserId);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] MultiplePost(string[] Ids)
        {
            try
            {
                return new ReceiveDAL().MultiplePost(Ids,connVM);

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
                return new ReceiveDAL().GetExcelDataWeb(Ids,null,null,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public string[] ImportExcelFile(ReceiveMasterVM paramVM)
        {
            try
            {
                return new ReceiveDAL().ImportExcelFile(paramVM,connVM); 
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public IEnumerable<object> GetReceiveColumn()
        {
            try
            {
                string[] columnNames = new string[] { "Receive No", "Import ID" };
                IEnumerable<object> enumList = from e in columnNames select new { Id = e.ToString().Replace(" ", ""), Name = e.ToString() };
                return enumList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string TrackingUpdate(List<TrackingVM> Trackings, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new TrackingDAL().TrackingUpdate(Trackings, Vtransaction, VcurrConn, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<TrackingVM> SelectTrakingsDetail(List<ReceiveDetailVM> DetailVMs, string ID, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new ReceiveDAL().GetTrackingsWeb(DetailVMs, ID, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region API

        public string SaveReceive(string xml)
        {
            try
            {
                ReceiveAPI api = new ReceiveAPI();

                var result = api.SaveReceive(xml);

                return result;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        #endregion
    }
}
