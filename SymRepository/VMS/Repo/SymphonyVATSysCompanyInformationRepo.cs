using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymServices.VMS.Library;
using SymViewModel.VMS.DTOs;

namespace SymRepository.VMS.Repo
{
    public class SymphonyVATSysCompanyInformationRepo
    {
        public List<SymphonyVATSysCompanyInformationVM> DropDown()
       {
           try
           {
               return new CompanyInformationDAL().DropDown();
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
        public List<SymphonyVATSysCompanyInformationVM> SelectAll(string CompanyID = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
       {
           try
           {
               return new CompanyInformationDAL().SelectAll(CompanyID, conditionFields, conditionValues, VcurrConn, Vtransaction);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

    }
}
