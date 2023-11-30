using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public  class DBUpdateRepo
    {
        DBUpdateDAL _dal = new DBUpdateDAL();
        #region DB Update
        public string[] DBTableAdd(string TableName, string FieldName, string DataType)
        {
            try
            {
                return new DBUpdateDAL().DBTableAdd(TableName, FieldName, DataType, null, null);
            }
            catch (Exception)
            {

                throw;
            }

        }
        public string[] DBTableFieldAdd(string TableName, string FieldName, string DataType, bool NullType)
        {
            try
            {
                return new DBUpdateDAL().DBTableFieldAdd(TableName, FieldName, DataType, NullType, null, null);
            }
            catch (Exception)
            {

                throw;
            }

        }
        public string[] DBTableFieldRemove(string TableName, string FieldName)
        {
            try
            {
                return new DBUpdateDAL().DBTableFieldRemove(TableName, FieldName, null, null);
            }
            catch (Exception)
            {

                throw;
            }

        }
        public string[] DBTableFieldAlter(string TableName, string FieldName, string DataType)
        {
            try
            {
                return new DBUpdateDAL().DBTableFieldAlter(TableName, FieldName, DataType, null, null);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
