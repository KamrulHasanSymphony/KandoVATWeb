using SymServices.VMS.Library;
using SymViewModel.VMS.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace SymRepository.VMS.Repo
{
    public class DepositRepo
    {
        public string[] DepositInsert(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory)
        {
            return new DepositDAL().DepositInsert(Master, vds, adjHistory, null, null);
        }

        public List<DepositMasterVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new DepositDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] DepositUpdate(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory)
        {
            return new DepositDAL().DepositUpdate(Master, vds, adjHistory);
        }
    }
}
