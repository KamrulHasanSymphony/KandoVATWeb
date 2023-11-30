using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using SymServices.VMS;
//using SymViewModel.VMS;
using System.Data;
using System.Data.SqlClient;
using VATViewModel.DTOs;
using VATServer.Library;
using SymOrdinary;
using System.Web;

namespace SymRepository.VMS
{
    public class AdjustmentRepo
    {
         private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
         public AdjustmentRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
         public AdjustmentRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public string[] InsertAdjustmentName(AdjustmentVM vm)
        {
            try
            {
                return new AdjustmentDAL().InsertAdjustmentName(vm,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateAdjustmentName(AdjustmentVM vm)
        {
            try
            {
                return new AdjustmentDAL().UpdateAdjustmentName(vm, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] DeleteAdjustmentName(string AdjId)
        {
            try
            {
                return new AdjustmentDAL().DeleteAdjustmentName(AdjId, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        public List<AdjustmentVM> SelectAll(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new AdjustmentDAL().SelectAllList(Id, conditionFields, conditionValues, null, null, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<AdjustmentVM> DropDown()
        {
            try
            {
                return new AdjustmentDAL().DropDown(connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
    }
}
