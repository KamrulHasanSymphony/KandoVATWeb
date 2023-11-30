using SymOrdinary;
using SymRepository.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
namespace SymVATWebUI.Areas.Common.Controllers
{
    [Authorize]
    public class CompanyController : Controller
    {
        //
        // GET: /Common/Company/

        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        CompanyRepo compRepo = new CompanyRepo();
        public ActionResult Index()
        {
          return  RedirectToAction("Edit");
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Create()
        {
            CompanyVM company = compRepo.SelectAll().FirstOrDefault();
            if (company !=null)
            {
               // return RedirectToAction("Edit");
            }
            return View();
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(CompanyVM company)
        {
            string[] result = new string[6];
            company.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            company.CreatedBy = Ordinary.UserName;
            company.CreatedFrom = Ordinary.WorkStationIP;
            try
            {
                result = compRepo.Insert(company);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Edit");
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View(company);
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Edit()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            CompanyVM company = compRepo.SelectById(Convert.ToInt32(identity.CompanyId));
            return View(company);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(CompanyVM company)
        { 
            string[] result = new string[6];            
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            company.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            company.LastUpdateBy = Ordinary.UserName;
            company.LastUpdateFrom = Ordinary.WorkStationIP;
            company.CurrentBranch=Convert.ToInt32(identity.BranchId);
            try
            {
                result = compRepo.Update(company);
                Session["result"] = result[0] + "~" + result[1];
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
            }
            try
            {
                company.Year= DateTime.Parse(company.YearStart).ToString("yyyy");
            }
            catch (Exception)
            {
            }
            return View(company);
        }
    }
}
