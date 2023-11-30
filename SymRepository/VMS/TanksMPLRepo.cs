using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Linq;
using VATViewModel.DTOs;
using VATServer.Library;
using VMSAPI;
using SymOrdinary;

namespace SymRepository.VMS
{
    public class TanksMPLRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public TanksMPLRepo()
        {
            connVM = null;
        }
        public TanksMPLRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
        public TanksMPLRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }


        public string[] TanksMPLInsert(TankMPLsVM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new TanksMPLDAL().TanksMPLInsert(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string[] TanksMPLUpdate(TankMPLsVM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new TanksMPLDAL().TanksMPLUpdate(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<TankMPLsVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, string ActiveStatus = "", string SelectTop = "100")
        {
            try
            {
                return new TanksMPLDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM, null, ActiveStatus, SelectTop);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<TankMPLsVM> DropDown(int branchId, string ItemNo)
        {
            try
            {
                return new TanksMPLDAL().DropDown(branchId, ItemNo,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
