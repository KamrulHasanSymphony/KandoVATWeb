using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using VATViewModel.DTOs;
using VATServer.Library;
using SymOrdinary;
using System.Web;

namespace SymRepository.VMS
{
    public class VehicleRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public VehicleRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public VehicleRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        //private SymOrdinary.ShampanIdentity identity;

        //public VehicleRepo(SymOrdinary.ShampanIdentity identity)
        //{
        //    // TODO: Complete member initialization
        //    this.identity = identity;
        //}
        public List<VehicleVM> DropDown() 
        {
            try
            {
                return new VehicleDAL().DropDown(connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetExcelData(List<string> Ids)
        {
            try
            {
                return new VehicleDAL().GetExcelData(Ids, null, null, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //public DataTable GetExcelAddress(List<string> Ids)
        //{
        //    try
        //    {
        //        return new VehicleDAL().GetExcelAddress(Ids, null, null, connVM);

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}
        public List<VehicleVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new VehicleDAL().SelectAllLst(Id, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] InsertToVehicle(VehicleVM vm) {
            try
            {
                return new VehicleDAL().InsertToVehicle(vm,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateToVehicle(VehicleVM vm) {
            try
            {
                return new VehicleDAL().UpdateToVehicle(vm,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] Delete(VehicleVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new VehicleDAL().Delete(vm,ids, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<string> Autocomplete(string term)
        {
            try
            {
                return new VehicleDAL().AutocompleteVehicle(term,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



       
    }
}
