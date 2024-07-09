using Microsoft.AspNet.SignalR;
using Project.Booking.Business.Interfaces;
using Project.Booking.Business.Sevices;
using Project.Booking.Model;
using Project.Booking.Sessions;
using Project.Booking.Web.Hubs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Booking.Web.Controllers
{
    public class BaseController : Controller
    {
        protected string baseUrl = "";
        public UserProfile UserProfile
        {
            get
            {
                if (Session[SessionKey.Autentication.UserProfile] == null)
                {
                    return null;
                }
                else
                {
                    return (UserProfile)Session[SessionKey.Autentication.UserProfile];
                }
            }
            set
            {
                Session[SessionKey.Autentication.UserProfile] = value;
            }
        }

        #region *** Service Wrapper ***     
        private IUtility _utility;
        protected IUtility utility
        {
            get
            {
                return (_utility == null) ? _utility = new UtilityService(new UserProfile()) : _utility;
            }
        }
        private IMaster _master;
        protected IMaster master
        {
            get
            {
                return (_master == null) ? _master = new MasterService() : _master;
            }
        }
        private IProject _project;
        protected IProject project
        {
            get
            {
                return (_project == null) ? _project = new ProjectService(UserProfile) : _project;
            }
        }
        private IBooking _booking;
        protected IBooking booking
        {
            get
            {
                return (_booking == null) ? _booking = new BookingService(UserProfile) : _booking;
            }
        }
        private IPayment _payment;
        protected IPayment payment
        {
            get
            {
                return (_payment == null) ? _payment = new PaymentService(UserProfile) : _payment;
            }
        }
        private IMatrix _matrix;
        protected IMatrix matrix
        {
            get
            {
                return (_matrix == null) ? _matrix = new MatrixService() : _matrix;
            }
        }
        #endregion

        #region *** Executing Action ***
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = System.Web.HttpContext.Current;
            HttpRequestBase req = filterContext.HttpContext.Request;
            HttpResponseBase res = filterContext.HttpContext.Response;

            var builder = new UriBuilder(req.Url.Scheme, req.Url.Host, req.Url.Port, req.ApplicationPath);
            var url = new Uri(builder.ToString());
            baseUrl = url.ToString();
            baseUrl = baseUrl.EndsWith("/") ? baseUrl : string.Concat(baseUrl, "/");
            ViewBag.baseUrl = baseUrl;
            ViewBag.isBestViewBrowser = BestViewBrowser(req.Browser);
        }
        #endregion

        #region Seletc List        
        #endregion

        #region Protected function
        protected string InnerException(Exception ex)
        {
            return (ex.InnerException != null) ? InnerException(ex.InnerException) : ex.Message;
        }
        protected string RenderPartialViewToString(string viewName, object model)
        {
            //Original source code: http://craftycodeblog.com/2010/05/15/asp-net-mvc-render-partial-view-to-string/
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
        protected void NotifyUnitStatusSignalR(SendUnitSignalModel model)
        {
            var notifyHub = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            notifyHub.Clients.All.sendUnitStatus(model);
        }
        protected bool BestViewBrowser(HttpBrowserCapabilitiesBase browser)
        {
            if (browser.Browser.ToUpper() == "CHROME" ||
                 browser.Browser.ToUpper() == "EDGE")
                return true;
            else return false;
        }
        #endregion


    }
}