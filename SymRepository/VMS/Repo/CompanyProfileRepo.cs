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
    public class CompanyProfileRepo
    {
        public string[] UpdateCompanyProfileNew(CompanyProfileVM companyProfiles)
        {
            try
            {
                return new CompanyprofileDAL().UpdateCompanyProfileNew(companyProfiles);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public List<CompanyProfileVM> SelectAll(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new CompanyprofileDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
