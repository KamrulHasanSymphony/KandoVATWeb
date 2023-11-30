using SymServices.VMS.Library;
using SymViewModel.VMS.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace SymRepository.VMS.Repo
{
    public class VDSRepo
    {

        public List<VDSMasterVM> SelectVDSDetail(string VDSId, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new VDSDAL().SelectVDSDetail(VDSId, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
