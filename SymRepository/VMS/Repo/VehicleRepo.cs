using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymServices.VMS.Library;
using SymViewModel.VMS.DTOs;
using System.Data;
using System.Data.SqlClient;

namespace SymRepository.VMS.Repo
{
    public class VehicleRepo
    {
        public List<VehicleVM> DropDown() 
        {
            try
            {
                return new VehicleDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<VehicleVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new VehicleDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] InsertToVehicle(VehicleVM vm) {
            try
            {
                return new VehicleDAL().InsertToVehicle(vm);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateToVehicle(VehicleVM vm) {
            try
            {
                return new VehicleDAL().UpdateToVehicle(vm);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] DeleteToVehicle(string VehicleID, string databaseName) 
        {
            try
            {
                return new VehicleDAL().DeleteToVehicle(VehicleID,databaseName);
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
                return new VehicleDAL().Delete(vm,ids, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public DataTable SearchVehicleDataTable(string VehicleID, string VehicleType, string VehicleNo, string ActiveStatus) 
        {
            try
            {
                return new VehicleDAL().SearchVehicleDataTable(VehicleID,VehicleType,VehicleNo,ActiveStatus);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        //public static string InsertToVehicle1(SqlCommand objCommVehicle, string VehicleID, string VehicleType, string VehicleNo, string Description, string Comments, string ActiveStatus, string CreatedBy, DateTime CreatedOn, string LastModifiedBy, DateTime LastModifiedOn) 
        //{
        //    try
        //    {
        //        return new VehicleDAL().InsertToVehicle1(objCommVehicle,VehicleID,VehicleType,VehicleNo,Description,Comments,ActiveStatus,CreatedBy, CreatedOn,LastModifiedBy,LastModifiedOn);
        //    }
        //    catch (Exception ex)
        //    {
                
        //        throw ex;
        //    }
        //}
    }
}
