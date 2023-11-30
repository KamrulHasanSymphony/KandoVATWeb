using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using VATServer.Library;
using VATViewModel.DTOs;

namespace SymRepository.VMS
{
    public class SDDepositRepo
    {
          private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
          public SDDepositRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

          public SDDepositRepo(ShampanIdentity identity, HttpSessionStateBase session)
          {
              connVM.SysDatabaseName = identity.InitialCatalog;
              connVM.SysUserName = SysDBInfoVM.SysUserName;
              connVM.SysPassword = SysDBInfoVM.SysPassword;
              connVM.SysdataSource = SysDBInfoVM.SysdataSource;

              connVM = Ordinary.StaticValueReAssign(identity, session);
          }
        public string[] DepositInsert(SDDepositVM Master)
        {
            return new SDDepositDAL().DepositInsert(Master,connVM);
        }

        public List<SDDepositVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new SDDepositDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] DepositUpdate(SDDepositVM Master)
        {
            return new SDDepositDAL().DepositUpdate(Master,connVM);
        }

        public string[] DepositPost(SDDepositVM Master)
        {
            try
            {
                return new SDDepositDAL().DepositPost(Master,connVM);
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}
