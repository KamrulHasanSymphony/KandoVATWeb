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
    public class MPLBankPaymentReceiveRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public MPLBankPaymentReceiveRepo()
        {
            connVM = null;
        }
        public MPLBankPaymentReceiveRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
        public MPLBankPaymentReceiveRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }


        public string[] InsertToMPLBankPaymentReceive(MPLBankPaymentReceiveVM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new MPLBankPaymentReceiveDAL().InsertToMPLBankPaymentReceive(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string[] UpdateMPLBankPaymentReceive(MPLBankPaymentReceiveVM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new MPLBankPaymentReceiveDAL().UpdateMPLBankPaymentReceive(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<MPLBankPaymentReceiveVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, string Orderby = "Y", string SelectTop = "100")
        {
            try
            {
                return new MPLBankPaymentReceiveDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM, null,  Orderby, SelectTop);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public List<MPLBankPaymentReceiveVM> DropDown()
        {
            try
            {
                return new MPLBankPaymentReceiveDAL().DropDown(connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
