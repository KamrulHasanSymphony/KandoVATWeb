using System;
using System.Collections.Generic;
using System.Data;
using SymServices.VMS.Library;
using SymViewModel.VMS.DTOs;

namespace SymRepository.Vms
{
    public class DataUpdateRepo
    {
        public List<DBUpdateViewModel> TableName()
        {
            try
            {
                return new DBUpdateDAL().TableName();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable SelectAll(string tableName="")
        {
            try
            {
                return new DBUpdateDAL().SelectAll(tableName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Update(string query)
        {
            try
            {
                return new DBUpdateDAL().Update(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
