using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VATServer.Library;
using VATViewModel.DTOs;

namespace SymRepository.VMS
{
   public class Toll6_3InvoiceRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public Toll6_3InvoiceRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
        public Toll6_3InvoiceRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public string[] InsertToToll6_3Invoice(Toll6_3InvoiceVM vm, List<Toll6_3InvoiceDetailVM> Details)
        {
            try
            {
               
                return new Toll6_3InvoiceDAL().Insert(vm, vm.Details,null,null,connVM);
                
            }
            catch (Exception ex)
            {
                
                throw ex;
            }

            return null;
        }

        public string[] Update(Toll6_3InvoiceVM vm)
        {
            try
            {
                return new Toll6_3InvoiceDAL().Update(vm, vm.Details,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return null;
        }



        public string[] Delete(Toll6_3InvoiceVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {

                //Toll6_3InvoiceDAL().
               // return new CurrenciesDAL().Delete(vm, ids, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return null;
        }

        public List<Toll6_3InvoiceVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new Toll6_3InvoiceDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

       public List<Toll6_3InvoiceDetailVM> SelectDetail(string TollNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
       {
           try
           {
               return new Toll6_3InvoiceDAL().SelectDetail(TollNo, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
           }
           catch (Exception ex)
           {

               throw ex;
           }
           return null;

       }
       


        public List<Toll6_3InvoiceVM> DropDown()
        {
            try
            {
                //return new CurrenciesDAL().DropDown(connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return null;
        }

        //public DataTable TollSearch()
        //{
        //    try
        //    {
        //        return new Toll6_3InvoiceDAL().TollSearch(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public string[] Toll6_3InvoicePost(Toll6_3InvoiceVM Master, string UserId = "")
        {
            return new Toll6_3InvoiceDAL().Post(Master, null, null, connVM);

        }

        public string[] UpdateTollCompleted(string flag, string tollNo)
        {
            return new Toll6_3InvoiceDAL().UpdateTollCompleted(flag,tollNo);

        }

    }
}
