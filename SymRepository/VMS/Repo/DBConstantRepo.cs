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
    public class DBConstantRepo
    {
        public string PassPhrase() 
        {
            try
            {
                return DBConstant.PassPhrase;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string EnKey()
        {
            try
            {
                return DBConstant.EnKey;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }
}
