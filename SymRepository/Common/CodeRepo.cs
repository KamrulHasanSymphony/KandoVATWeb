using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
   public class CodeRepo
    {
       
        CodeDAL _codeDAL = new CodeDAL();

        #region Methods

        //==================Search Codes=================
        public DataSet SearchCodes()
        {
            DataSet dataSet = new DataSet("Search Codes");
            try
            {
                dataSet = _codeDAL.SearchCodes();
            }
            catch (Exception ex)
            {
                throw ex;
            }
          
           return dataSet;
         
        }

        //==================Update Codes=================
        public string[] CodeUpdate(List<CodeVM> codeVMs)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "Fail";
            retResults[3] = "Fail";
            try
            {
                retResults = _codeDAL.CodeUpdate(codeVMs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retResults;
        }

        public string CodeDataInsert(string CodeGroup, string CodeName, string prefix, string Lenth,SqlConnection currConn, SqlTransaction transaction)
        {
            string retResults = "0";
            try
            {
                retResults = _codeDAL.CodeDataInsert(CodeGroup, CodeName, prefix, Lenth, currConn, transaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            #region Results

            return retResults;
            #endregion


        }
        #endregion
    }
}
