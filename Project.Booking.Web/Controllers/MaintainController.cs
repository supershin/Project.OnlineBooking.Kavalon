
using System;
using System.Web.Mvc;

namespace Project.Booking.Web.Controllers
{
    
    public class MaintainController : BaseController
    {
        // GET: User
        [Route("maintain/thankyou")]
        public ActionResult thankyou()
        {
            ViewBag.BaseUrl = baseUrl;
            return View();
        }

    }
}