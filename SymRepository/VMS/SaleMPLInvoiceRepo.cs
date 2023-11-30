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
    public class SaleMPLInvoiceRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public SaleMPLInvoiceRepo()
        {
            connVM = null;
        }
        public SaleMPLInvoiceRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
        public SaleMPLInvoiceRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        

        public string[] SalesMPLInsert(SalesInvoiceMPLHeaderVM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new SaleMPLDAL().SalesMPLInsert(Master,transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string[] SalesMPLUpdate(SalesInvoiceMPLHeaderVM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new SaleMPLDAL().SalesMPLUpdate(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<SalesInvoiceMPLHeaderVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SalesInvoiceMPLHeaderVM likeVM = null, string transactionType = null, string Orderby = "Y", string[] ids = null, string Is6_3TollCompleted = null)
        {
            try
            {
                return new SaleMPLDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, likeVM, connVM, transactionType, Orderby, ids, Is6_3TollCompleted);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SalesInvoiceMPLDetailVM> SearchSaleMPLDetailList(string salesInvoiceMPLHeaderId)
        {
            try
            {
                return new SaleMPLDAL().SearchSaleMPLDetailList(salesInvoiceMPLHeaderId, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SalesInvoiceMPLBankPaymentVM> SearchSaleMPLBankPaymentList(string salesInvoiceMPLHeaderId)
        {
            try
            {
                return new SaleMPLDAL().SearchSaleMPLBankPaymentList(salesInvoiceMPLHeaderId, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SalesInvoiceMPLCRInfoVM> SearchSaleMPLCRInfoList(string salesInvoiceMPLHeaderId)
        {
            try
            {
                return new SaleMPLDAL().SearchSaleMPLCRInfoList(salesInvoiceMPLHeaderId, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SalesInvoiceMPLCRInfoVM> SearchSaleMPLCRInfoListById(string Id)
        {
            try
            {
                return new SaleMPLDAL().SearchSaleMPLCRInfoListById(Id, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }   
        public List<EnumModeOfPaymentVM> DropDown(string type)
        {
            try
            {
                return new SaleMPLDAL().DropDown(type,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EnumVehicleTypeVM> DropDownEnumVehicleType()
        {
            try
            {
                return new SaleMPLDAL().DropDownEnumVehicleType(connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<SalesInvoiceMPLCRInfoVM> SelectAll(SalesInvoiceMPLCRInfoVM vm)
        {
            try
            {
                return new SaleMPLDAL().SelectAllList(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetCRInfoColumn()
        {
            try
            {
                string[] columnName = new string[] { "CR Code" };
                IEnumerable<object> enumList = from e in columnName select new { Id = e.ToString().Replace(" ", ""), Name = e.ToString() };
                return enumList;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public string[] SalesPost(SalesInvoiceMPLHeaderVM Master)
        {
            try
            {
                return new SaleMPLDAL().SalesPost(Master,null,null,connVM,false,"");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public string[] UpdateSalesDetailsTankInfo(SalesInvoiceMPLDetailVM details, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new SaleMPLDAL().UpdateSalesDetailsTankInfo(details, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
