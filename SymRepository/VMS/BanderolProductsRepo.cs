using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymServices.VMS;
using SymViewModel.VMS;
using System.Data;
using System.Data.SqlClient;

namespace SymRepository.VMS
{
    public class BanderolProductsRepo
    {
        public string[] InsertToBanderolProducts(BanderolProductVM vm)
        {
            try
            {
                return new BanderolProductsDAL().InsertToBanderolProducts(vm);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateBanderolProduct(BanderolProductVM vm)
        {
            try
            {
                return new BanderolProductsDAL().UpdateBanderolProduct(vm);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] DeleteBanderolProduct(string BandProductId)
        {
            try
            {
                return new BanderolProductsDAL().DeleteBanderolProduct(BandProductId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SearchBanderolProducts(string ProductName, string ProductCode, string BanderolId, string BanderolName, string PackagingId, string PackagingNature, string ActiveStatus)
        {
            try
            {
                return new BanderolProductsDAL().SearchBanderolProducts(ProductName, ProductCode, BanderolId, BanderolName, PackagingId, PackagingNature, ActiveStatus);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SearchBanderol(string BandProductId)
        {
            try
            {
                return new BanderolProductsDAL().SearchBanderol(BandProductId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }
}
