using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Newtonsoft.Json;
using SymRepository.VMS;
using VATViewModel.DTOs;

namespace SymVATWebUI.Filters
{
    //public class UrlFilterAttribute : ActionFilterAttribute
    //{
    //    public override void OnActionExecuting(ActionExecutingContext filterContext)
    //    {
    //        string rawUrl = filterContext.HttpContext.Request.RawUrl;
    //        Uri url = filterContext.HttpContext.Request.Url;

    //        var query = System.Web.HttpUtility.ParseQueryString(url.Query);

    //        ActionDescriptor descriptor = filterContext.ActionDescriptor;

    //        string actionName = descriptor.ActionName;
    //        string controllerName = descriptor.ControllerDescriptor.ControllerName;


    //        var request = filterContext.HttpContext.Request;

    //        var content = request.ContentEncoding;
    //        var form = request.Form;

            

    //        if (rawUrl.Contains("VMS"))
    //        {

    //            #region Home

    //            if (controllerName.ToLower() == "home")
    //            {
                   


    //            }

    //            #endregion

    //            #region Setup

    //            #region Item Information

    //            #region ProductCategory

    //            if (controllerName.ToLower() == "productcategory")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110110110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110110110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110110110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #region Product

    //            if (controllerName.ToLower() == "product")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110110120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110110120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110110120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #region Overhead

    //            if (controllerName.ToLower() == "overhead")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110110130"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110110130"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110110130"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #region HSCode

    //            if (controllerName.ToLower() == "hscode")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110110150"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110110150"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110110150"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #endregion

    //            #region Vendor

    //            #region VendorGroup

    //            if (controllerName.ToLower() == "vendorgroup")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110120110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110120110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110120110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #region Vendor

    //            if (controllerName.ToLower() == "vendor")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110120120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110120120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110120120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #endregion

    //            #region Customer

    //            #region CustomerGroup

    //            if (controllerName.ToLower() == "customergroup")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110130110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110130110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110130110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #region Customer

    //            if (controllerName.ToLower() == "customer")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110130120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110130120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110130120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #endregion

    //            #region Bank/Vehicle

                
    //            #region Bank Information

    //            if (controllerName.ToLower() == "bankinformation")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110140110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110140110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110140110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #region Vehicle

    //            if (controllerName.ToLower() == "vehicle")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110140120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110140120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110140120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #endregion

    //            #region Price Declaration

    //            #region VAT-4.3

    //            if (controllerName.ToLower() == "vat1")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110150110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110150110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110150110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #region Service

    //            if (controllerName.ToLower() == "service")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110150120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110150120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110150120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #region Tender

    //            if (controllerName.ToLower() == "tender")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110150130"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110150130"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110150130"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #endregion

    //            #region Company

    //            #region CompanyProfile

    //            if (controllerName.ToLower() == "companyprofile")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110160110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110160110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110160110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #region BranchProfiles

    //            if (controllerName.ToLower() == "branchprofiles")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110160120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110160120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110160120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #endregion Company

    //            #region FiscalYear

    //            if (controllerName.ToLower() == "fiscalyear")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110170110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110170110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110170110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #region Configuration

    //            #region Setting

    //            if (controllerName.ToLower() == "setting")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110180110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110180110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110180110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #region Prefix

    //            if (controllerName.ToLower() == "Prefix")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110180120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110180120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110180120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #endregion

    //            #region Import

    //            if (controllerName.ToLower() == "import")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110190110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //            }


    //            #endregion

    //            #region Measurement

    //            #region UOM

    //            if (controllerName.ToLower() == "uom")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110200110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110200110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110200110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #region UOMConversion

    //            if (controllerName.ToLower() == "uomconversion")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110200120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110200120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110200120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #endregion

    //            #region Currency

    //            #region Currency

    //            if (controllerName.ToLower() == "currency")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110210110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110210110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110210110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #region CurrencyConversion

    //            if (controllerName.ToLower() == "currencyconversion")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("110210120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("110210120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("110210120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion

    //            #endregion

    //            #endregion Setup

    //            #region Purchase

    //            if (controllerName.ToLower() == "purchase")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    string transactionType = query["TransactionType"];

    //                    if (transactionType != null)
    //                    {
    //                        #region other

    //                        if (transactionType.ToLower() == "other")
    //                        {
    //                            if (!IsPermitted("120110110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Import

    //                        else if (transactionType.ToLower() == "import")
    //                        {
    //                            if (!IsPermitted("120110120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region InputService

    //                        else if (transactionType.ToLower() == "inputservice")
    //                        {
    //                            if (!IsPermitted("120110130"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region PurchaseReturn

    //                        else if (transactionType.ToLower() == "purchasereturn")
    //                        {
    //                            if (!IsPermitted("120110140"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Service

    //                        else if (transactionType.ToLower() == "service")
    //                        {
    //                            if (!IsPermitted("120110150"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region ServiceNS

    //                        else if (transactionType.ToLower() == "servicens")
    //                        {
    //                            if (!IsPermitted("120110120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Increase

    //                        else if (transactionType.ToLower() == "purchasecn")
    //                        {
    //                            if (!IsPermitted("170120120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Decrease

    //                        else if (transactionType.ToLower() == "purchasedn")
    //                        {
    //                            if (!IsPermitted("170120110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion


    //                    }
    //                    else
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                { "Action", "Login" } });
    //                    }


    //                }
    //                #endregion

    //                #region create

    //                if (actionName.ToLower() == "create")
    //                {
    //                    string transactionType = query["tType"];

    //                    if (transactionType != null)
    //                    {
    //                        #region other

    //                        if (transactionType.ToLower() == "other")
    //                        {
    //                            if (!IsPermitted("120110110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Import

    //                        else if (transactionType.ToLower() == "import")
    //                        {
    //                            if (!IsPermitted("120110120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region InputService

    //                        else if (transactionType.ToLower() == "inputservice")
    //                        {
    //                            if (!IsPermitted("120110130"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region PurchaseReturn

    //                        else if (transactionType.ToLower() == "purchasereturn")
    //                        {
    //                            if (!IsPermitted("120110140"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Service

    //                        else if (transactionType.ToLower() == "service")
    //                        {
    //                            if (!IsPermitted("120110150"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region ServiceNS

    //                        else if (transactionType.ToLower() == "servicens")
    //                        {
    //                            if (!IsPermitted("120110120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Increase

    //                        else if (transactionType.ToLower() == "purchasecn")
    //                        {
    //                            if (!IsPermitted("170120120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Decrease

    //                        else if (transactionType.ToLower() == "purchasedn")
    //                        {
    //                            if (!IsPermitted("170120110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion


    //                    }
    //                    else
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                { "Action", "Login" } });
    //                    }


    //                }
    //                #endregion

    //                #region edit

    //                if (actionName.ToLower() == "edit")
    //                {
    //                    string transactionType = query["TransactionType"];

    //                    if (transactionType != null)
    //                    {
    //                        #region other

    //                        if (transactionType.ToLower() == "other")
    //                        {
    //                            if (!IsPermitted("120110110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Import

    //                        else if (transactionType.ToLower() == "import")
    //                        {
    //                            if (!IsPermitted("120110120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region InputService

    //                        else if (transactionType.ToLower() == "inputservice")
    //                        {
    //                            if (!IsPermitted("120110130"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region PurchaseReturn

    //                        else if (transactionType.ToLower() == "purchasereturn")
    //                        {
    //                            if (!IsPermitted("120110140"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Service

    //                        else if (transactionType.ToLower() == "service")
    //                        {
    //                            if (!IsPermitted("120110150"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region ServiceNS

    //                        else if (transactionType.ToLower() == "servicens")
    //                        {
    //                            if (!IsPermitted("120110120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Increase

    //                        else if (transactionType.ToLower() == "purchasecn")
    //                        {
    //                            if (!IsPermitted("170120120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Decrease

    //                        else if (transactionType.ToLower() == "purchasedn")
    //                        {
    //                            if (!IsPermitted("170120110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion


    //                    }
    //                    else
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                { "Action", "Login" } });
    //                    }


    //                }
    //                #endregion

    //            }



    //            #endregion Purchase

    //            #region Production

    //            #region IssueHeader

    //            if (controllerName.ToLower() == "IssueHeader")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    string transactionType = query["transactionType"];

    //                    if (transactionType != null)
    //                    {
    //                        #region other

    //                        if (transactionType.ToLower() == "other")
    //                        {
    //                            if (!IsPermitted("130110110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region IssueReturn

    //                        else if (transactionType.ToLower() == "issuereturn")
    //                        {
    //                            if (!IsPermitted("130110120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Transfer Raw

    //                        else if (transactionType.ToLower() == "transfer raw")
    //                        {
    //                            if (!IsPermitted("120110130"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

                          
    //                    }
    //                    else
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                { "Action", "Login" } });
    //                    }


    //                }
    //                #endregion

    //                #region create

    //                if (actionName.ToLower() == "create")
    //                {
    //                    string transactionType = query["tType"];

    //                    if (transactionType != null)
    //                    {

    //                        #region other

    //                        if (transactionType.ToLower() == "other")
    //                        {
    //                            if (!IsPermitted("130110110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region IssueReturn

    //                        else if (transactionType.ToLower() == "issuereturn")
    //                        {
    //                            if (!IsPermitted("130110120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Transfer Raw

    //                        else if (transactionType.ToLower() == "transfer raw")
    //                        {
    //                            if (!IsPermitted("120110130"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                    }
    //                    else
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                { "Action", "Login" } });
    //                    }


    //                }
    //                #endregion

    //                #region edit

    //                if (actionName.ToLower() == "edit")
    //                {
    //                    string transactionType = query["TransactionType"];

    //                    if (transactionType != null)
    //                    {
    //                        #region other

    //                        if (transactionType.ToLower() == "other")
    //                        {
    //                            if (!IsPermitted("130110110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region IssueReturn

    //                        else if (transactionType.ToLower() == "issuereturn")
    //                        {
    //                            if (!IsPermitted("130110120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Transfer Raw

    //                        else if (transactionType.ToLower() == "transfer raw")
    //                        {
    //                            if (!IsPermitted("120110130"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion


    //                    }
    //                    else
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                { "Action", "Login" } });
    //                    }


    //                }
    //                #endregion

    //            }



    //            #endregion IssueHeader

    //            #region Receive

    //            if (controllerName.ToLower() == "receive")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    string transactionType = query["transactionType"];

    //                    if (transactionType != null)
    //                    {
    //                        #region WIP

    //                        if (transactionType.ToLower() == "wip")
    //                        {
    //                            if (!IsPermitted("130120110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region FG(ttype=Other)

    //                        else if (transactionType.ToLower() == "other")
    //                        {
    //                            if (!IsPermitted("130120120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region ReceiveReturn

    //                        else if (transactionType.ToLower() == "ReceiveReturn")
    //                        {
    //                            if (!IsPermitted("130120130"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region PackageProduction

    //                        else if (transactionType.ToLower() == "PackageProduction")
    //                        {
    //                            if (!IsPermitted("130120140"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                    }
    //                    else
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                { "Action", "Login" } });
    //                    }


    //                }
    //                #endregion

    //                #region create

    //                if (actionName.ToLower() == "create")
    //                {
    //                    string transactionType = query["tType"];

    //                    if (transactionType != null)
    //                    {
    //                        #region WIP

    //                        if (transactionType.ToLower() == "wip")
    //                        {
    //                            if (!IsPermitted("130120110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region FG(ttype=Other)

    //                        else if (transactionType.ToLower() == "other")
    //                        {
    //                            if (!IsPermitted("130120120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region ReceiveReturn

    //                        else if (transactionType.ToLower() == "ReceiveReturn")
    //                        {
    //                            if (!IsPermitted("130120130"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region PackageProduction

    //                        else if (transactionType.ToLower() == "PackageProduction")
    //                        {
    //                            if (!IsPermitted("130120140"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                    }
    //                    else
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                { "Action", "Login" } });
    //                    }


    //                }
    //                #endregion

    //                #region edit

    //                if (actionName.ToLower() == "edit")
    //                {
    //                    string transactionType = query["TransactionType"];

    //                    if (transactionType != null)
    //                    {
    //                        #region WIP

    //                        if (transactionType.ToLower() == "wip")
    //                        {
    //                            if (!IsPermitted("130120110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region FG(ttype=Other)

    //                        else if (transactionType.ToLower() == "other")
    //                        {
    //                            if (!IsPermitted("130120120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region ReceiveReturn

    //                        else if (transactionType.ToLower() == "ReceiveReturn")
    //                        {
    //                            if (!IsPermitted("130120130"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region PackageProduction

    //                        else if (transactionType.ToLower() == "PackageProduction")
    //                        {
    //                            if (!IsPermitted("130120140"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion


    //                    }
    //                    else
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                { "Action", "Login" } });
    //                    }


    //                }
    //                #endregion

    //            }

    //            #endregion Receive

    //            #endregion Production

    //            #region Sale

    //            #region saleinvoice

    //            if (controllerName.ToLower() == "saleinvoice")
    //            {
    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    string transactionType = query["TransactionType"];

    //                    if (transactionType != null)
    //                    {
    //                        #region other

    //                        if (transactionType.ToLower() == "other")
    //                        {
    //                            if (!IsPermitted("140110110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }
    //                        #endregion 

    //                        #region Export

    //                        else if (transactionType.ToLower() == "export")
    //                        {
    //                            if (!IsPermitted("140110120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Tender

    //                        else if (transactionType.ToLower() == "tender")
    //                        {
    //                            if (!IsPermitted("140110130"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region TradingTender

    //                        else if (transactionType.ToLower() == "tradingtender")
    //                        {
    //                            if (!IsPermitted("140110140"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Service
    //                        else if (transactionType.ToLower() == "service")
    //                        {
    //                            if (!IsPermitted("140110150"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region ServiceNS
    //                        else if (transactionType.ToLower() == "servicens")
    //                        {
    //                            if (!IsPermitted("140110160"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region credit

    //                        else if (transactionType.ToLower() == "credit")
    //                        {
    //                            if (!IsPermitted("170130110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Debit

    //                        else if (transactionType.ToLower() == "debit")
    //                        {
    //                            if (!IsPermitted("170130120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion


    //                    }
    //                    else
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                { "Action", "Login" } });
    //                    }


    //                }
    //                #endregion

    //                #region create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    string transactionType = query["tType"];

    //                    if (transactionType != null)
    //                    {
    //                        #region other

    //                        if (transactionType.ToLower() == "other")
    //                        {
    //                            if (!IsPermitted("140110110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }
    //                        #endregion

    //                        #region Export

    //                        else if (transactionType.ToLower() == "export")
    //                        {
    //                            if (!IsPermitted("140110120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Tender

    //                        else if (transactionType.ToLower() == "tender")
    //                        {
    //                            if (!IsPermitted("140110130"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region TradingTender

    //                        else if (transactionType.ToLower() == "tradingtender")
    //                        {
    //                            if (!IsPermitted("140110140"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Service
    //                        else if (transactionType.ToLower() == "service")
    //                        {
    //                            if (!IsPermitted("140110150"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region ServiceNS
    //                        else if (transactionType.ToLower() == "servicens")
    //                        {
    //                            if (!IsPermitted("140110160"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region credit

    //                        else if (transactionType.ToLower() == "credit")
    //                        {
    //                            if (!IsPermitted("170130110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Debit

    //                        else if (transactionType.ToLower() == "debit")
    //                        {
    //                            if (!IsPermitted("170130120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                    }
    //                    else
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                { "Action", "Login" } });
    //                    }


    //                }
    //                #endregion 
                    
    //                #region CreateEdit

    //                //else if (actionName.ToLower() == "createedit")
    //                //{
    //                //    string operation = form["Operation"];
    //                //    string saleInvoice = form["SalesInvoiceNo"];
    //                //    string id = form["Id"];

    //                //    // database access

    //                //    if (id != null)
    //                //    {
    //                //        SaleInvoiceRepo repo = new SaleInvoiceRepo();

    //                //        SaleMasterVM vm = repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();

    //                //        if (vm != null)
    //                //        {
    //                //            if (vm.TransactionType.ToLower() == "other")
    //                //            {
    //                //                if (!IsPermitted("140110110"))
    //                //                {
    //                //                    filterContext.Result = new RedirectToRouteResult(
    //                //                        new RouteValueDictionary {{ "Controller", "Home" },
    //                //                            { "Action", "Login" } });
    //                //                }
    //                //            }
    //                //        }
    //                //    }
    //                //    else
    //                //    {
    //                //        filterContext.Result = new RedirectToRouteResult(
    //                //            new RouteValueDictionary {{ "Controller", "Home" },
    //                //                { "Action", "Login" } });
    //                //    }

    //                //}

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    string transactionType = query["TransactionType"];

    //                    if (transactionType != null)
    //                    {
    //                        #region other

    //                        if (transactionType.ToLower() == "other")
    //                        {
    //                            if (!IsPermitted("140110110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }
    //                        #endregion

    //                        #region Export

    //                        else if (transactionType.ToLower() == "export")
    //                        {
    //                            if (!IsPermitted("140110120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Tender

    //                        else if (transactionType.ToLower() == "tender")
    //                        {
    //                            if (!IsPermitted("140110130"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region TradingTender

    //                        else if (transactionType.ToLower() == "tradingtender")
    //                        {
    //                            if (!IsPermitted("140110140"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Service
    //                        else if (transactionType.ToLower() == "service")
    //                        {
    //                            if (!IsPermitted("140110150"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region ServiceNS
    //                        else if (transactionType.ToLower() == "servicens")
    //                        {
    //                            if (!IsPermitted("140110160"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region credit

    //                        else if (transactionType.ToLower() == "credit")
    //                        {
    //                            if (!IsPermitted("170130110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Debit

    //                        else if (transactionType.ToLower() == "debit")
    //                        {
    //                            if (!IsPermitted("170130120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                    }
    //                    else
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion 

    //            }

    //            #endregion saleinvoice

    //            #region Transfer Issue/Receive

    //            #region TransferIssue

    //            if (controllerName.ToLower() == "transferissue")
    //            {
    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    string transactionType = query["transactionType"];

    //                    if (transactionType != null)
    //                    {
    //                        #region FG(Out)

    //                        if (transactionType.ToLower() == "62out")
    //                        {
    //                            if (!IsPermitted("140130140"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }
    //                        #endregion

    //                        #region RM(Out)

    //                        else if (transactionType.ToLower() == "61out")
    //                        {
    //                            if (!IsPermitted("140130130"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                    }
    //                    else
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                { "Action", "Login" } });
    //                    }


    //                }
    //                #endregion

    //                #region create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    string transactionType = query["tType"];

    //                    if (transactionType != null)
    //                    {
    //                        #region FG(Out)

    //                        if (transactionType.ToLower() == "62out")
    //                        {
    //                            if (!IsPermitted("140130140"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }
    //                        #endregion

    //                        #region RM(Out)

    //                        else if (transactionType.ToLower() == "61out")
    //                        {
    //                            if (!IsPermitted("140130130"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                    }
    //                    else
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                { "Action", "Login" } });
    //                    }


    //                }
    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    string transactionType = query["TransactionType"];

    //                    if (transactionType != null)
    //                    {
    //                        #region FG(Out)

    //                        if (transactionType.ToLower() == "62out")
    //                        {
    //                            if (!IsPermitted("140130140"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }
    //                        #endregion

    //                        #region RM(Out)

    //                        else if (transactionType.ToLower() == "61out")
    //                        {
    //                            if (!IsPermitted("140130130"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                    }
    //                    else
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //            }
    //            #endregion TransferIssue

    //            #region TransferReceive

    //            if (controllerName.ToLower() == "transferreceive")
    //            {
    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    string transactionType = query["transactionType"];

    //                    if (transactionType != null)
    //                    {
    //                        #region FG(In)

    //                        if (transactionType.ToLower() == "62in")
    //                        {
    //                            if (!IsPermitted("140130120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }
    //                        #endregion

    //                        #region RM(In)

    //                        else if (transactionType.ToLower() == "61in")
    //                        {
    //                            if (!IsPermitted("140130110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                    }
    //                    else
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                { "Action", "Login" } });
    //                    }


    //                }
    //                #endregion

    //                #region create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    string transactionType = query["tType"];

    //                    if (transactionType != null)
    //                    {
    //                        #region FG(In)

    //                        if (transactionType.ToLower() == "62in")
    //                        {
    //                            if (!IsPermitted("140130120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }
    //                        #endregion

    //                        #region RM(In)

    //                        else if (transactionType.ToLower() == "61in")
    //                        {
    //                            if (!IsPermitted("140130110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                    }
    //                    else
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                { "Action", "Login" } });
    //                    }


    //                }
    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    string transactionType = query["TransactionType"];

    //                    if (transactionType != null)
    //                    {
    //                        #region FG(In)

    //                        if (transactionType.ToLower() == "62in")
    //                        {
    //                            if (!IsPermitted("140130120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }
    //                        #endregion

    //                        #region RM(In)

    //                        else if (transactionType.ToLower() == "61in")
    //                        {
    //                            if (!IsPermitted("140130110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                    }
    //                    else
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //            }
    //            #endregion TransferReceive

    //            #endregion Transfer Issue/Receive


    //            #endregion Sale

    //            #region Deposit

    //            if (controllerName.ToLower() == "deposit")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    string transactionType = query["TransactionType"];

    //                    if (transactionType != null)
    //                    {
    //                        #region Treasury

    //                        if (transactionType.ToLower() == "treasury")
    //                        {
    //                            if (!IsPermitted("150110110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Purchase VDS

    //                        else if (transactionType.ToLower() == "vds")
    //                        {
    //                            if (!IsPermitted("150120110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region SaleVDS

    //                        else if (transactionType.ToLower() == "salevds")
    //                        {
    //                            if (!IsPermitted("150120120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region SD

    //                        else if (transactionType.ToLower() == "sd")
    //                        {
    //                            if (!IsPermitted("150130110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                    }
    //                    else
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                { "Action", "Login" } });
    //                    }


    //                }
    //                #endregion

    //                #region create

    //                if (actionName.ToLower() == "create")
    //                {
    //                    string transactionType = query["TransactionType"];

    //                    if (transactionType != null)
    //                    {
    //                        #region Treasury

    //                        if (transactionType.ToLower() == "treasury")
    //                        {
    //                            if (!IsPermitted("150110110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Purchase VDS

    //                        else if (transactionType.ToLower() == "vds")
    //                        {
    //                            if (!IsPermitted("150120110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region SaleVDS

    //                        else if (transactionType.ToLower() == "salevds")
    //                        {
    //                            if (!IsPermitted("150120120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region SD

    //                        else if (transactionType.ToLower() == "sd")
    //                        {
    //                            if (!IsPermitted("150130110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion


    //                    }
    //                    else
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                { "Action", "Login" } });
    //                    }


    //                }
    //                #endregion

    //                #region edit

    //                if (actionName.ToLower() == "edit")
    //                {
    //                    string transactionType = query["TransactionType"];

    //                    if (transactionType != null)
    //                    {
    //                        #region Treasury

    //                        if (transactionType.ToLower() == "treasury")
    //                        {
    //                            if (!IsPermitted("150110110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region Purchase VDS

    //                        else if (transactionType.ToLower() == "vds")
    //                        {
    //                            if (!IsPermitted("150120110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region SaleVDS

    //                        else if (transactionType.ToLower() == "salevds")
    //                        {
    //                            if (!IsPermitted("150120120"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion

    //                        #region SD

    //                        else if (transactionType.ToLower() == "sd")
    //                        {
    //                            if (!IsPermitted("150130110"))
    //                            {
    //                                filterContext.Result = new RedirectToRouteResult(
    //                                    new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                            }
    //                        }

    //                        #endregion


    //                    }
    //                    else
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                { "Action", "Login" } });
    //                    }


    //                }
    //                #endregion

    //            }



    //            #endregion Deposit

    //            #region Adjustment

    //            #region Adjustment Head

    //            #region AdjustmentName

    //            if (controllerName.ToLower() == "adjustmentname")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("170110110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("170110110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("170110110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion AdjustmentName

    //            #region AdjustmentHistory

    //            if (controllerName.ToLower() == "adjustmenthistory")
    //            {

    //                #region index

    //                if (actionName.ToLower() == "index")
    //                {
    //                    if (!IsPermitted("170110120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  create

    //                else if (actionName.ToLower() == "create")
    //                {
    //                    if (!IsPermitted("170110120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region edit

    //                else if (actionName.ToLower() == "edit")
    //                {
    //                    if (!IsPermitted("170110120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion


    //            }


    //            #endregion AdjustmentHistory

    //            #endregion Adjustment Head

    //            #endregion Adjustment

    //            #region NBRReport

    //            if (controllerName.ToLower() == "nbrreport")
    //            {

    //                #region  VAT 4.3

    //                if (actionName.ToLower() == "printvat1")
    //                {
    //                    if (!IsPermitted("180110110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  VAT 6.1

    //                else if (actionName.ToLower() == "printvat16")
    //                {
    //                    if (!IsPermitted("180120110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region VAT 6.2

    //                else if (actionName.ToLower() == "printvat17")
    //                {
    //                    if (!IsPermitted("180130110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region VAT 6.3

    //                else if (actionName.ToLower() == "previewvat6_3")
    //                {
    //                    if (!IsPermitted("180170110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region VAT 6.5

    //                else if (actionName.ToLower() == "transferisuue")
    //                {
    //                    if (!IsPermitted("180180110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region VAT 6.10

    //                else if (actionName.ToLower() == "vat6_10report")
    //                {
    //                    if (!IsPermitted("180150110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region VAT 9.1

    //                else if (actionName.ToLower() == "viewvat9_1")
    //                {
    //                    if (!IsPermitted("180140110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region chakka

    //                else if (actionName.ToLower() == "chakka")
    //                {
    //                    if (!IsPermitted("180230110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region VATChakKha

    //                else if (actionName.ToLower() == "chakkha")
    //                {
    //                    if (!IsPermitted("180230120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region SD Report

    //                else if (actionName.ToLower() == "printvatsd")
    //                {
    //                    if (!IsPermitted("180160110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //            }


    //            #endregion

    //            #region MISReport

    //            if (controllerName.ToLower() == "misreport")
    //            {

    //                #region  Purchase

    //                if (actionName.ToLower() == "printpurchase")
    //                {
    //                    if (!IsPermitted("190110110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //                #region  Sale

    //                else if (actionName.ToLower() == "printsale")
    //                {
    //                    if (!IsPermitted("190130110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region Production

    //                #region Issue

    //                else if (actionName.ToLower() == "printissue")
    //                {
    //                    if (!IsPermitted("190120110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region Receive

    //                else if (actionName.ToLower() == "printreceive")
    //                {
    //                    if (!IsPermitted("190120120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #endregion

    //                #region Stock

    //                #region Stock

    //                else if (actionName.ToLower() == "printstock")
    //                {
    //                    if (!IsPermitted("190140110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region Receive Sale

    //                else if (actionName.ToLower() == "printreceivestock")
    //                {
    //                    if (!IsPermitted("190140120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region Reconsciliation

    //                else if (actionName.ToLower() == "printreconsciliation")
    //                {
    //                    if (!IsPermitted("190140130"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region Branch Stock Movement

    //                else if (actionName.ToLower() == "printbranchstockmovement")
    //                {
    //                    if (!IsPermitted("190140140"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #endregion

    //                #region Deposit

    //                #region Deposit

    //                else if (actionName.ToLower() == "printdeposit")
    //                {
    //                    if (!IsPermitted("190150110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region Current Account

    //                else if (actionName.ToLower() == "printcurrentaccount")
    //                {
    //                    if (!IsPermitted("190150120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #endregion

    //                #region Other

    //                #region Adjustment

    //                else if (actionName.ToLower() == "printadjustment")
    //                {
    //                    if (!IsPermitted("190160110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region Co-Efficient

    //                else if (actionName.ToLower() == "printcoefficient")
    //                {
    //                    if (!IsPermitted("190160120"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region Wastage

    //                else if (actionName.ToLower() == "printwastage")
    //                {
    //                    if (!IsPermitted("190160130"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region SerialStock

    //                else if (actionName.ToLower() == "printserialstock")
    //                {
    //                    if (!IsPermitted("190160150"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #region Purchase LC

    //                else if (actionName.ToLower() == "printpurchaselc")
    //                {
    //                    if (!IsPermitted("190160160"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }
    //                }

    //                #endregion

    //                #endregion Other

    //            }


    //            #endregion

    //            #region UserInformation

    //            if (controllerName.ToLower() == "userinformation")
    //            {

    //                #region ChangePassword

    //                if (actionName.ToLower() == "changepassword")
    //                {
    //                    if (!IsPermitted("200120110"))
    //                    {
    //                        filterContext.Result = new RedirectToRouteResult(
    //                            new RouteValueDictionary {{ "Controller", "Home" },
    //                                        { "Action", "Login" } });
    //                    }

    //                }

    //                #endregion

    //            }

    //            #endregion


    //        }


    //        base.OnActionExecuting(filterContext);
    //    }


    //    private bool IsPermitted(string formId)
    //    {
    //        try
    //        {
    //            DataTable menu = JsonConvert.DeserializeObject<DataTable>(HttpContext.Current.Session["AllRoles"].ToString());
    //            DataTable userMenu = JsonConvert.DeserializeObject<DataTable>(HttpContext.Current.Session["UserRoles"].ToString());

    //            if (menu == null)
    //                throw new Exception("Roles Not Found");

    //            if (userMenu == null)
    //                throw new Exception("User Roles Not Found");

    //            bool flag = true;

    //            if (formId.Length == 9)
    //            {
    //                DataRow[] rows = menu.Select("FormID = '" + formId + "'");

    //                if (rows.Length == 0)
    //                    return false;

    //                DataRow[] userRows = userMenu.Select("FormID = '" + formId + "'");

    //                if (userRows.Length == 0)
    //                    return false;


    //                if (rows[0]["Access"].ToString() != "1" || (userRows[0]["Access"].ToString() != "1"))
    //                {
    //                    flag = false;
    //                }
    //            }
    //            else
    //            {
    //                DataRow[] rows = menu.Select("FormID = '" + formId + "'");

    //                if (rows.Length == 0)
    //                    return false;


    //                if (rows[0]["Access"].ToString() != "1")
    //                {
    //                    flag = false;
    //                }
    //            }

    //            return flag;
    //        }
    //        catch (Exception e)
    //        {
    //            return false;
    //        }

    //    }
    //}
}