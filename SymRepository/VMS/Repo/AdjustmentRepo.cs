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
    public class AdjustmentRepo
    {
        public string[] InsertAdjustmentName(AdjustmentVM vm)
        {
            try
            {
                return new AdjustmentDAL().InsertAdjustmentName(vm);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateAdjustmentName(AdjustmentVM vm)
        {
            try
            {
                return new AdjustmentDAL().UpdateAdjustmentName(vm);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] DeleteAdjustmentName(string AdjId)
        {
            try
            {
                return new AdjustmentDAL().DeleteAdjustmentName(AdjId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SearchAdjustmentName(string AdjName, string ActiveStatus)
        {
            try
            {
                return new AdjustmentDAL().SearchAdjustmentName(AdjName, ActiveStatus);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
