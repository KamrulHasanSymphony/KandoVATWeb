using SymOrdinary;
using SymRepository.VMS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using VATViewModel.DTOs;

namespace SymVATWebUI.Areas.VMS.Controllers
{
    public class ChartController : Controller
    {
        //
        // GET: /VMS/Chart/

        #region Chart

        public class chart
        {
            public string Section { get; set; }
            public string Gender { get; set; }
            public decimal Person { get; set; }
        }

        public class chartMulti
        {
            public string Section { get; set; }
            public string PeriodName { get; set; }
            //public List<int> Persons = new List<int>();
            public List<decimal> values = new List<decimal>();
        }

        ChartRepo _Chartrepo = new ChartRepo();

        [AllowAnonymous]
        public ActionResult ChartBar()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            VATReturnVM vm = new VATReturnVM();

            DateTime dateTime = DateTime.Now;

            vm.PeriodName = dateTime.ToString("MMMM-yyyy");

            return View(vm);
        }

        public ActionResult VAT9_1ChartBar(VATReturnVM vm)
        {


            List<string> Descriptions = new List<string>();
            List<string> Sections = new List<string>();
            List<object> objectList = new List<object>();
            List<chartMulti> chs = new List<chartMulti>();
            chartMulti ch = new chartMulti();
            DataSet ds = new DataSet();
            DataTable returndt = new DataTable();
            DataTable dt = new DataTable();

            int i = Convert.ToInt32(vm.previousmonth);

            DateTime dateTime = Convert.ToDateTime(vm.PeriodName);

            dateTime = dateTime.AddMonths(-i);

            vm.PeriodName = dateTime.ToString("MMMM-yyyy");

            ds = _Chartrepo.vat9_1forChartBar(vm);

            dt = ds.Tables[0].Copy();

            DataView dv = new DataView(dt);

            DataTable dtSection = dv.ToTable(true, "Section");
            DataTable dtDescription = dv.ToTable(true, "Description");


            foreach (DataRow sec1 in dtDescription.Rows)
            {
                Descriptions.Add(sec1["Description"].ToString());
            }

            foreach (DataRow gend in dtSection.Rows)
            {
                Sections.Add(gend["Section"].ToString());

            }
            foreach (DataRow gen in dtSection.Rows)
            {
                ch = new chartMulti();
                ch.Section = gen["Section"].ToString();
                ch.PeriodName = vm.PeriodName;

                //DataRow[] results = ds.Tables[1].Select("Gender =" + ch.Gender);

                foreach (DataRow sec in dtDescription.Rows)
                {

                    //DataRow[] management = ds.Tables[2].Select("select Person where Section='" + sec["Section"].ToString() + "' and Gender='" + ch.Gender + "' ");

                    string Description = sec["Description"].ToString();

                    DataRow[] result = dt.Select("Description = '" + Description + "' AND Section = '" + ch.Section + "'");

                    decimal value = 0;
                    //int c = Convert.ToInt32(_repo.TotalEmployeeSectionGender(sec["Section"].ToString(), ch.Gender));
                    if (result.Length != 0)
                    {
                        value = Convert.ToDecimal(result[0].ItemArray[2].ToString());

                    }

                    ch.values.Add(value);

                    //ch.Persons.Add(per);
                }
                chs.Add(ch);
            }
            objectList.Add(Descriptions);
            objectList.Add(Sections);
            objectList.Add(chs);
            return Json(objectList, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult ChartPie()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            VATReturnVM vm = new VATReturnVM();

            DateTime dateTime = DateTime.Now;

            vm.PeriodName = dateTime.ToString("MMMM-yyyy");

            return View(vm);
        }

        public ActionResult VAT9_1ChartPie(VATReturnVM vm)
        {
            List<object> objectList = new List<object>();
            List<VATReturnVM> chs = new List<VATReturnVM>();
            VATReturnVM ch = new VATReturnVM();

            int i = Convert.ToInt32(vm.previousmonth);

            DateTime dateTime = Convert.ToDateTime(vm.PeriodName);

            dateTime = dateTime.AddMonths(-i);

            vm.PeriodName = dateTime.ToString("MMMM-yyyy");

            var emp = _Chartrepo.SelectAllvat9_1forChartPie(vm);



            foreach (var a in emp)
            {
                ch = new VATReturnVM();

                ch.Description = a.Description;
                ch.Value = a.Value;
                ch.Section = a.Section;
                chs.Add(ch);
            }



            objectList.Add(chs);
            objectList.Add(vm);

            return Json(objectList, JsonRequestBehavior.AllowGet);
        }

        #endregion



        public ActionResult Index(VATReturnVM vm)
        {
            return View(vm);
        }



    }
}
