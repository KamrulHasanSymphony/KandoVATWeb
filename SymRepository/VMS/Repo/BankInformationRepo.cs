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
    public class BankInformationRepo
    {
        public List<BankInformationVM> DropDown() 
        {
            try
            {
                return new BankInformationDAL().DropDown();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] InsertToBankInformation(BankInformationVM vm)
        {
            try
            {
                return new BankInformationDAL().InsertToBankInformation(vm);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateBankInformation(BankInformationVM vm)
        {
            try
            {
                return new BankInformationDAL().UpdateBankInformation(vm);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<BankInformationVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new BankInformationDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public string[] Delete(BankInformationVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new BankInformationDAL().Delete(vm, ids, VcurrConn, Vtransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
