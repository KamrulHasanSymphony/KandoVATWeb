using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Library;
using VATViewModel.DTOs;

namespace SymRepository.VMS
{
    public class ChartRepo
    {

        _9_1_VATReturnDAL _DAL = new _9_1_VATReturnDAL();


        public DataSet vat9_1forChartBar(VATReturnVM vm)
        {
            DataSet dataSet = new DataSet();

            try
            {
                dataSet = _DAL.SelectAllvat9_1forChartBar(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dataSet;
        }

        public List<VATReturnVM> SelectAllvat9_1forChartPie(VATReturnVM vm)
        {
            List<VATReturnVM> dataSet = new List<VATReturnVM>();
            try
            {
                dataSet = _DAL.SelectAllvat9_1forChartPie(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dataSet;
        }



    }
}
