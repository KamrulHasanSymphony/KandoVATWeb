using SymOrdinary;
using SymRepository.VMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace SymVATWebUI.Areas.API
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SaleService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SaleService.svc or SaleService.svc.cs at the Solution Explorer and start debugging.
    public class SaleService : ISaleService
    {
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        public string[] InsertSale(string saleXML)
        {
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            try
            {
                retResults = new SaleInvoiceRepo(identity).InsertSaleApi(saleXML);
                return retResults;
            }
            catch (Exception)
            {
                return retResults;
            }

        }
    }
}
