using Project.Booking.Constants;
using Project.Booking.Model;
using Project.Booking.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace Project.Booking.Web.Filters
{
    public class CustomAuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            var userProfile = (UserProfile)filterContext.HttpContext.Session[SessionKey.Autentication.UserProfile];
            if (userProfile == null)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new
                        {
                            // put whatever data you want which will be sent
                            // to the client
                            success = false,
                            message = Constant.Message.Error.SESSION_TIME_OUT,
                            sessionTimeOut = true,
                            returnUrl = filterContext.RequestContext.HttpContext.Request.RawUrl
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    //Redirecting the user to the Login View of Account Controller  
                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                     { "controller", "Register" },
                     { "action", "Index" },
                      { "returnUrl",filterContext.RequestContext.HttpContext.Request.RawUrl}
                    });
                }

            }
        }
    }
}