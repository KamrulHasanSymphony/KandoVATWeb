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
   public class Client6_3InvoiceRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public Client6_3InvoiceRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
        public Client6_3InvoiceRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public ResultVM InsertToClient6_3Invoice(Client6_3VM vm)
        {
            try
            {
               
                return new Client6_3DAL().Insert(vm,null,null,connVM);
                
            }
            catch (Exception ex)
            {
                
                throw ex;
            }

        }

        public ResultVM Update(Client6_3VM vm)
        {
            try
            {
                return new Client6_3DAL().Update(vm,null,null, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ResultVM Post(ParameterVM vm)
        {
            try
            {
                return new Client6_3DAL().Post(vm, null, null, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }




        public string[] Delete(Client6_3VM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {

                //Client6_3DAL().
               // return new CurrenciesDAL().Delete(vm, ids, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return null;
        }

        public List<Client6_3VM> SelectAll( string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new Client6_3DAL().SelectVM( conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       public List<Client6_3DetailVM> SelectDetail( string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
       {
           try
           {
               return new Client6_3DAL().SelectDetailVM( conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
           }
           catch (Exception ex)
           {

               throw ex;
           }

       }
       


        public List<Client6_3VM> DropDown()
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
        //        return new Client6_3DAL().TollSearch(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


    }
}
