using SymServices.VMS.Library;
using SymViewModel.VMS.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace SymRepository.VMS.Repo
{
    public class SDDepositRepo
    {
        public string[] DepositInsert(SDDepositVM Master)
        {
            return new SDDepositDAL().DepositInsert(Master);
        }

        public List<SDDepositVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new SDDepositDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] DepositUpdate(SDDepositVM Master)
        {
            return new SDDepositDAL().DepositUpdate(Master);
        }
    }
}
