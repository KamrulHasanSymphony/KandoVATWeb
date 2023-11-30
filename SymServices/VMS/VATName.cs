using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymServices.VMS
{
   public class VATName
    {
       public static string[] vATName = new string[] { 
           "VAT 1",//0
           "VAT 1 Ka (Tarrif)",//1
           "VAT 1 Kha (Trading)", //2
           "VAT 1 Ga (Export)",//3
           "VAT 1 Gha (Fixed)",//4
           "VAT 1 (Internal Issue)",//5
           "VAT 1 (Toll Issue)",//6
           "VAT 1 (Toll Receive)",//7
           "VAT 1 (Tender)",//8
           "VAT 1 (Package)" ,//9
           "VAT 1 (Wastage)" };//10

        public IList<string> VATNameList
        {
            get
            {
                return vATName.ToList<string>();
            }
        }

    }
}
