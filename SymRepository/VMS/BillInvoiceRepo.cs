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
    public class BillInvoiceRepo
    {

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public BillInvoiceRepo()
        {
                connVM = null;
        }
        public BillInvoiceRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public BillInvoiceRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }

        public string[] BillInvoiceInsert(BillInvoiceMasterVM Master, List<BillInvoiceDetailVM> BillInvoices)
        {
            return new BillInvoiceDAL().BillInvoiceInsert(Master,BillInvoices, null, null, connVM);
        }

        public List<BillInvoiceMasterVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new BillInvoiceDAL().SelectAllList(Id, conditionFields, conditionValues, null, null, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<BillInvoiceDetailVM> SelectAllDetails(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new BillInvoiceDAL().SelectAllListdetails(Id, conditionFields, conditionValues, null, null, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }
}
