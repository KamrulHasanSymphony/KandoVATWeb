using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SymOrdinary;
//using SymRepository.Sage;
//using SymViewModel.Sage;
using System.Threading;
using System.Net.Mail;
using VATViewModel.DTOs;
using SymRepository.VMS;
using SymphonySofttech.Utilities;
using System.Data;
using Newtonsoft.Json;
using SymVATWebUI.Areas.VMS.Models;
using VATServer.Library;
using VATServer.Ordinary;

namespace SymVATWebUI.Areas.VMS.Controllers
{
    public class HomeController : Controller
    {

        [AllowAnonymous]
        public ActionResult Login()
        {
            string[] result = new string[6];
            result[0] = "Fail1";
            result[1] = "Fail1";

            CommonRepo _cRepo = new CommonRepo();

            if (!_cRepo.SuperInformationFileExist())
            {
                result[0] = "Fail"; result[1] = "Super Information File not exist";
                return View();
            }

            LoginVM vm = new LoginVM();
            return View(vm);
        }

        [AllowAnonymous]
        public ActionResult Index(bool changeBranch = false)
        {
            LoginVM vm = new LoginVM();
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            try
            {

                int userId = Convert.ToInt32(identity.UserId);

                #region Branch Conditions

                var userBranches = new UserBranchDetailRepo(identity).SelectAll(userId);
                if (userBranches == null)
                {
                    Session["BranchId"] = "1";
                    Session["BranchCode"] = "";

                }
                else if (userBranches.Count == 1)
                {
                    Session["BranchId"] = userBranches[0].BranchId.ToString();
                    Session["BranchCode"] = userBranches[0].BranchCode;
                }
                else
                {
                    if (changeBranch)
                    {
                        return PartialView("Branch", userBranches);
                    }
                }

                SetOrdinaryVATDesktop(Session["BranchId"].ToString());

                #endregion

            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();

                Session["result"] = "Fail~" + msg;

                SymOrdinary.FileLogger.Log("HomeController", "Index", ex.ToString());

            }
            return View(vm);

        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginVM vm, string returnUrl)
        {

            //vm.LoginBranchId = 1;
            //checkThread(Work);
            string[] result = new string[6];
            result[0] = "Fail1";
            result[1] = "Fail1";
            string workStationIP = "127.0.0.1";
            CommonRepo _cRepo = new CommonRepo();
            CompanyProfileRepo _companyRepo = new CompanyProfileRepo();

            try
            {
                //////WebClient client = new WebClient();
                //////var realip = client.DownloadString("http://ipinfo.io");
                //////workStationIP = realip.Replace("\n", "").Replace(" ", "").Replace(",", ":").Replace("{", "").Replace("}", "").Replace("\"", "").ToString();
                //////workStationIP = workStationIP.Split(':')[1];
            }
            catch (Exception e)
            {

                //throw;
            }
            try
            {
                string SessionDate = vm.SessionDate;

                Session["SoftwareVersion"] = "21-NOV-2023"; //"18-May-2023";

                Session["SessionDate"] = SessionDate;

                Session["SessionYear"] = Convert.ToDateTime(SessionDate).ToString("yyyy");

                Ordinary.CompanyLogoPath = new AppSettingsReader().GetValue("CompanyLogoPath", typeof(string)).ToString();
                Ordinary.ReportHeaderLogo = new AppSettingsReader().GetValue("ReportHeaderLogo", typeof(string)).ToString();

                string[] retResults = new string[2];

                SymphonyVATSysCompanyInformationRepo _sysComRepo = new SymphonyVATSysCompanyInformationRepo();
                SymphonyVATSysCompanyInformationVM sysComVM = new SymphonyVATSysCompanyInformationVM();
                CompanyProfileVM companyVM = new CompanyProfileVM();

                CommonRepo _CommonRepo = new CommonRepo();


                sysComVM = _sysComRepo.SelectAll(vm.CompanyID, null, null).FirstOrDefault();

                vm.DatabaseName = sysComVM.DatabaseName;

                UserInformationRepo _userRepo = new UserInformationRepo();
                UserInformationVM userVM = new UserInformationVM();
                DatabaseInfoVM.dataSource = vm.DatabaseName;
                _cRepo.LoginSuccess(vm.DatabaseName);
                _CommonRepo.AddUserInfo();

                userVM = _userRepo.SelectForLogin(vm).FirstOrDefault();

                if (userVM != null && !string.IsNullOrWhiteSpace(userVM.UserID))
                {
                    if (userVM.IsLock)
                    {
                        retResults[0] = "Fail"; retResults[1] = "User temporarily locked. Please contact your administrator!";
                        Session["result"] = retResults[0] + "~" + retResults[1];
                        return RedirectToAction("Login");
                    }

                    string vCompanyName = Converter.DESDecrypt(DBConstant.PassPhrase, DBConstant.EnKey, sysComVM.CompanyName);
                    Session["CompanyName"] = vCompanyName;

                    Session[vm.DatabaseName] = Converter.DESDecrypt(DBConstant.PassPhrase, DBConstant.EnKey, vm.CompanyID);

                    #region Update Branch Info

                    _CommonRepo.AddBranchInfo();

                    #endregion


                    string[] rol = { "Admin" };
                    string roleTicket = ShampanIdentity.CreateRoleTicket(rol);

                    string basicTicket = ShampanIdentity.CreateBasicTicket(userVM.UserName, userVM.FullName + "~" + userVM.Designation
                        , sysComVM.CompanyID, vCompanyName, "0", "0", workStationIP, "", vm.SessionDate
                        , userVM.UserID, true// userVM.IsAdmin
                        , vm.DatabaseName
                        , companyVM.Address1
                        , companyVM.Address2
                        , companyVM.Address3
                        , companyVM.TelephoneNo
                        , companyVM.FaxNo
                    );

                    int timeOut = Convert.ToInt32(new AppSettingsReader().GetValue("COOKIE_TIMEOUT", typeof(string)));
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, FormsAuthentication.FormsCookieName, DateTime.Now, DateTime.Now.AddMinutes(30), true, basicTicket);
                    FormsAuthentication.SetAuthCookie(userVM.UserName + vm.DatabaseName, true);
                    string encTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpContext.Response.Cookies.Add(new HttpCookie("VMS", encTicket));
                    HttpContext.Application["BasicTicket" + userVM.UserName + vm.DatabaseName] = basicTicket;
                    HttpContext.Application["RoleTicket" + userVM.UserName + vm.DatabaseName] = roleTicket;

                    var appPath = HttpContext.Request.ApplicationPath.ToString();

                    #region User Role Select

                    // need to create repo
                    UserMenuAllRollDAL dal = new UserMenuAllRollDAL();
                    UserMenuRollsDAL UserMenuRollsDal = new UserMenuRollsDAL();

                    DataTable allRoles = dal.UserMenuAllRollsSelectAll();
                    DataTable userRoles = UserMenuRollsDal.UserMenuRollsSelectAll(userVM.UserID);

                    #endregion

                    Session["COOKIE_TIMEOUT"] = timeOut;
                    Session["LogInBranch"] = vCompanyName;
                    Session["LogInLoginTime"] = DateTime.Now.ToString("dd/MMM/yyyy HH:mm:ss");
                    Session["LogInUserName"] = userVM.UserName;
                    Session["BranchId"] = "0";
                    Session["LogInUserId"] = userVM.UserID;
                    Session["AllRoles"] = JsonConvert.SerializeObject(allRoles);
                    Session["UserRoles"] = JsonConvert.SerializeObject(userRoles);

                    #region Moved code

                    companyVM = new CompanyProfileVM();
                    _companyRepo = new CompanyProfileRepo();
                    companyVM = _companyRepo.SelectAll().FirstOrDefault();
                    retResults[0] = "Success"; retResults[1] = "Login Successed";
                    Session["result"] = retResults[1];

                    _userRepo.UpdateUserWromgAttempt(vm.UserName);


                    SettingRepo _SettingRepo = new SettingRepo();
                    SettingsVM varSettingVM = new SettingsVM();
                    string CompanyCode = "";
                    string[] cFields = { "SettingGroup", "SettingName" };
                    string[] cValues = { "CompanyCode", "Code" };

                    varSettingVM = _SettingRepo.SelectAll(0, cFields, cValues).FirstOrDefault();
                    CompanyCode = varSettingVM.SettingValue;
                    Session["CompanyCode"] = CompanyCode;


                    #endregion

                    // need to re-assign in place of the useage
                    OrdinaryVATDesktop.CompanyName = companyVM.CompanyLegalName;
                    OrdinaryVATDesktop.VatRegistrationNo = companyVM.VatRegistrationNo;
                    OrdinaryVATDesktop.Address1 = companyVM.Address1;
                    OrdinaryVATDesktop.Address2 = companyVM.Address2;
                    OrdinaryVATDesktop.Address3 = companyVM.Address3;

                    UserInfoVM.IsMainSetting = true;

                    Session[sysComVM.CompanyID + "CompanyName"] = companyVM.CompanyLegalName;
                    Session[sysComVM.CompanyID + "VatRegistrationNo"] = companyVM.VatRegistrationNo;
                    Session[sysComVM.CompanyID + "Address1"] = companyVM.Address1;
                    Session[sysComVM.CompanyID + "FaxNo"] = companyVM.FaxNo;
                    Session[sysComVM.CompanyID + "TelephoneNo"] = companyVM.TelephoneNo;

                    Session[sysComVM.CompanyID + "Section"] = companyVM.Section;

                    
                    //int ChangeDate=0;

                    //var SettingVM = _SettingRepo.SelectAll(0, new []{"SettingGroup", "SettingName" },new []{"Password", "ChangeDate" }).FirstOrDefault();

                    //DateTime LastPasswordChangeDate = DateTime.Now;
                    //LastPasswordChangeDate = Convert.ToDateTime(userVM.LastPasswordChangeDate);

                    //ChangeDate = Convert.ToInt32(SettingVM.SettingValue ?? "120"); ;
                    //DateTime NewDate = LastPasswordChangeDate.AddDays(ChangeDate);

                    //DateTime CurrentDateTime = DateTime.Now;
                    //if (CurrentDateTime > NewDate)
                    //{
                        
                    //}

                    if (!string.IsNullOrWhiteSpace(returnUrl) && returnUrl != "/")
                    {
                        return Redirect(returnUrl);
                    }

                    else
                    {
                        string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
                        return Redirect("/VMS/Home/Index?changeBranch=true");
                    }

                }
                else
                {
                    _userRepo.WorngAttemptProcess(true, vm.UserName);
                    retResults[0] = "Fail"; retResults[1] = "User Name / Password is invalid!";
                    Session["result"] = retResults[0] + "~" + retResults[1];
                    return RedirectToAction("Login");
                }

                //Session["result"] = retResults[0] + "~" + retResults[1];
            }
            catch (Exception e)
            {
                string msg = e.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
                SymOrdinary.FileLogger.Log("HomeController", "Login", e.ToString());

                return Redirect("/VMS/Home/Login");
            }
        }

        [AllowAnonymous]
        public RedirectResult LoginRedirect(string returnUrl)
        {
            var rUrl = new Uri(Request.Url, returnUrl).ToString();
            return Redirect("~/Home/Login/?returnUrl=" + Url.Encode(rUrl));
        }

        [AllowAnonymous]
        public ActionResult LogOut()
        {
            string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            HttpCookie authCookie = Request.Cookies[CompanyName];

            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null && authCookie.Value != "")
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                authCookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Response.Cookies.Add(authCookie);
            }
            Session["UserType"] = "";
            Session["result"] = "";




            return RedirectToAction("Login");
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetUserBranch()
        {
            return PartialView("_selectBranch", new PopUpViewModel());
        }

        //[AllowAnonymous]
        //[HttpGet]
        //public ActionResult ForgetPassword()
        //{
        //    GLUserVM vm = new GLUserVM();
        //    return PartialView(vm);
        //}

        //[AllowAnonymous]
        //[HttpPost]
        //public ActionResult ForgetPassword(GLUserVM vm)
        //{
        //    string[] result = new string[6];
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(vm.LogId))
        //        {
        //            Session["result"] = "Fail" + "~" + "Please Input Log Id";
        //            return RedirectToAction("Login");
        //        }

        //        if (string.IsNullOrWhiteSpace(vm.Email))
        //        {
        //            Session["result"] = "Fail" + "~" + "Please Input Email";
        //            return RedirectToAction("Login");
        //        }
        //        string newPassword = Ordinary.RandomString(4);

        //        vm.Password = newPassword;

        //        GLUserRepo _userRepo = new GLUserRepo();
        //        result = _userRepo.ForgetPassword(vm);
        //        if (result[0] != "Fail")
        //        {
        //            vm.Password = newPassword;
        //            new GLEmailRepo().EmailPassword(vm);
        //        }

        //        Session["result"] = result[0] + "~" + result[1];
        //        return RedirectToAction("Login");

        //    }
        //    catch (Exception)
        //    {
        //        Session["result"] = "Fail~Data Not Succeessfully!";
        //        FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
        //        return RedirectToAction("Login");
        //    }
        //}

        public void SetBranchCode(string branchCode, string branchId, string BranchName)
        {
            Session["BranchCode"] = branchCode;
            Session["BranchId"] = branchId;
            Session["LogInBranch"] = Session["CompanyName"] + " - " + branchCode + "(" + BranchName + ")";

            SetOrdinaryVATDesktop(branchId);

        }


        private void SetOrdinaryVATDesktop(string branchId)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            OrdinaryVATDesktop.IsWCF = new BranchProfileRepo(identity).SelectAll(branchId).FirstOrDefault().IsWCF;



        }


        [Authorize]
        [HttpGet]
        public ActionResult CharInfo()
        {
            return View();
        }



        [Authorize]
        [HttpGet]
        public ActionResult GetSaleData(string firstMonth, string secondMonth)
        {
            try
            {
                SaleInvoiceRepo repo = new SaleInvoiceRepo();

                DateTime tempDate = Convert.ToDateTime(firstMonth);

                DataTable dtfirstMonth = repo.GetDateWiseSale(tempDate.ToString("yyyy-MM-01"),
                    Convert.ToDateTime(tempDate.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1)
                        .ToString("yyyy-MM-dd"));

                tempDate = Convert.ToDateTime(secondMonth);

                DataTable dtSecondMonth = repo.GetDateWiseSale(tempDate.ToString("yyyy-MM-01"),
                    Convert.ToDateTime(tempDate.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1)
                        .ToString("yyyy-MM-dd"));

                ChartData chartData = new ChartData();

                chartData.datasets.Add(new ChartLine()
                {
                    label = Convert.ToDateTime(firstMonth).ToString("MMMM-yyyy"),
                    borderColor = "blue",
                    data = dtfirstMonth.Rows.Cast<DataRow>()
                        .Select(x => Convert.ToDecimal(x["SaleValue"])).ToList()

                });


                chartData.datasets.Add(new ChartLine()
                {
                    label = Convert.ToDateTime(secondMonth).ToString("MMMM-yyyy"),
                    borderColor = "red",
                    data = dtSecondMonth.Rows.Cast<DataRow>()
                        .Select(x => Convert.ToDecimal(x["SaleValue"])).ToList()

                });

                return Json(chartData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                 return Json(new ChartData(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetSaleVAT(string firstMonth, string secondMonth)
        {
            try
            {
                SaleInvoiceRepo repo = new SaleInvoiceRepo();

                DateTime tempDate = Convert.ToDateTime(firstMonth);

                DataTable dtfirstMonth = repo.GetDateWiseSale(tempDate.ToString("yyyy-MM-01"),
                    Convert.ToDateTime(tempDate.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1)
                        .ToString("yyyy-MM-dd"));

                tempDate = Convert.ToDateTime(secondMonth);

                DataTable dtSecondMonth = repo.GetDateWiseSale(tempDate.ToString("yyyy-MM-01"),
                    Convert.ToDateTime(tempDate.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1)
                        .ToString("yyyy-MM-dd"));

                ChartData chartData = new ChartData();

                chartData.datasets.Add(new ChartLine()
                {
                    label = Convert.ToDateTime(firstMonth).ToString("MMMM-yyyy"),
                    borderColor = "blue",
                    data = dtfirstMonth.Rows.Cast<DataRow>()
                        .Select(x => Convert.ToDecimal(x["VAT"])).ToList()

                });


                chartData.datasets.Add(new ChartLine()
                {
                    label = Convert.ToDateTime(secondMonth).ToString("MMMM-yyyy"),
                    borderColor = "red",
                    data = dtSecondMonth.Rows.Cast<DataRow>()
                        .Select(x => Convert.ToDecimal(x["VAT"])).ToList()

                });

                return Json(chartData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new ChartData(), JsonRequestBehavior.AllowGet);
            }
        }
    }
}
