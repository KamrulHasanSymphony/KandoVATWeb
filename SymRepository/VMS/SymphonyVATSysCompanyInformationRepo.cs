using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Library;
using VATViewModel.DTOs;

namespace SymRepository.VMS
{
    public class SymphonyVATSysCompanyInformationRepo
    {
        public List<SymphonyVATSysCompanyInformationVM> DropDown( string CompanyList = "")
       {
           try
           {
               return new CompanyInformationDAL().DropDown(null, CompanyList);
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
