using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace SymRepository.VMS
{
    public class DBConstantRepo
    {
        public string PassPhrase() 
        {
            try
            {
                return "";
                //return DBConstant.PassPhrase;
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
                return "";
                //return DBConstant.EnKey;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }
}
