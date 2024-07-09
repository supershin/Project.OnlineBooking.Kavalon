using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Booking.Web.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult InternalServer()
        {
            return View();
        }
        public ActionResult NotFound()
        {
            return View();
        }
        public ActionResult UnAuthorized()
        {
            return View();
        }
    }
}