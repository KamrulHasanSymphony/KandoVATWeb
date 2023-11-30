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
    public class BanderolsRepo
    {
        public string[] InsertToBanderol(BanderolVM vm)
        {
            try
            {
                return new BanderolsDAL().InsertToBanderol(vm);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateBanderol(BanderolVM vm)
        {
            try
            {
                return new BanderolsDAL().UpdateBanderol(vm);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] DeleteBanderolInformation(string BanderolID)
        {
            try
            {
                return new BanderolsDAL().DeleteBanderolInformation(BanderolID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SearchBanderols(string BanderolName, string BandeSize, string OpeningDateFrom, string OpeningDateTo, string ActiveStatus)
        {
            try
            {
                return new BanderolsDAL().SearchBanderols(BanderolName, BandeSize, OpeningDateFrom, OpeningDateTo, ActiveStatus);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }
}
