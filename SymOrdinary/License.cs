using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOrdinary
{
   public static class License
    {
       static string DataBase = "";

       public static string DataBaseName(string CompanyName)
       {
           //kamrul
           if (CompanyName.ToLower()=="KajolBrothersHRM".ToLower() || CompanyName.ToLower()=="AnupamPrintersHRM".ToLower())
           {
               DataBase = "KajolBrothersHRM~AnupamPrintersHRM" ;
           }
           if (CompanyName.ToLower()=="KajolBrothersHRMDemo".ToLower())
           {
               DataBase = "KajolBrothersHRMDemo~AnupamPrintersHRM" ;
           }
           return DataBase;
       }
    }
}
