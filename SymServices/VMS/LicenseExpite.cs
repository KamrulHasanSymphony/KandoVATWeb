using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymServices.VMS
{
    public class LicenseExpite
    {
        public static string ExpiteDate(string CompanyName)
        {
            //kamrul

            string LicenseDate = "19800101";
            if (CompanyName.ToLower().Contains("demo")
                          || CompanyName == "My_Company_DB"
                          || CompanyName == "NP_DB"
                )
            {
                LicenseDate = "20191220";
            }
            else if (CompanyName == "CPB_DB" // CPB_DB
              || CompanyName == "CPB_DB"
               )
            {
                LicenseDate = "20190215";
            }
            else if (CompanyName == "RTCL_DB" // Rupsha
                         
                          )
            {
                LicenseDate = "20190615";
            }
            else if (CompanyName.ToUpper() == "NITA_DB"
                      )
            {
                LicenseDate = "20190615";
            }
            else if (CompanyName == "PCCL_Demo_DB" // Padma Group
                          || CompanyName == "PCCL_DB"
                          || CompanyName == "PCMCL_DB"
                          || CompanyName == "PPML_DB"
                          || CompanyName == "PDBL_BD"
                           )
            {
                LicenseDate = "20190215";
            }
            else if (CompanyName == "HO_DB" // HO_DB
             || CompanyName == "HO_DB"
              )
            {
                LicenseDate = "20180115";
            }
            else if (CompanyName == "Demo_DB" // Demo_DB
             || CompanyName == "Demo_DB"
             || CompanyName == "Demo_DB"
             || CompanyName == "Demo_DB"
             || CompanyName == "Demo_DB"
             || CompanyName == "Matador_Group_DB"
             || CompanyName == "MBPI_Demo_DB"
             || CompanyName == "BSCL_DB"
             || CompanyName == "BSCL_2_DB"
                
              )
            {
                LicenseDate = "20181019";
            }
            else if (CompanyName == "APPL_DB" // HO_DB
            || CompanyName == "HO_DB"
             )
            {
                LicenseDate = "20181225";
            }
            else if (CompanyName == "BVCPSCTG_DB" // HO_DB
          || CompanyName == "BVCPS_DB"
           )
            {
                LicenseDate = "20190215";
            }
            return LicenseDate;
        }
    }

}
