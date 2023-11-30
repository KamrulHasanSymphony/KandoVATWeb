using SymServices.VMS.Library;
using SymViewModel.VMS.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace SymRepository.VMS.Repo
{
    public class SetupRepo
    {
        public DataSet ResultVATStatus(DateTime StartDate, string databaseName) {
            try
            {
                return new SetupDAL().ResultVATStatus(StartDate, databaseName);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }
}
