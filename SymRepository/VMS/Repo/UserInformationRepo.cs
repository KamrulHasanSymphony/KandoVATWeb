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
    public class UserInformationRepo
    {

        public List<UserInformationVM> SelectForLogin(LoginVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new UserInformationDAL().SelectForLogin(vm, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<UserInformationVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new UserInformationDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] InsertToUserInformationNew(UserInformationVM vm)
        {
            try
            {
                return new UserInformationDAL().InsertToUserInformationNew(vm);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
