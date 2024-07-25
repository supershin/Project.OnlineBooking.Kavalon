using Project.Booking.Constants;
using Project.Booking.Extensions;
using Project.Booking.Model;
using Project.Booking.Security;
using Project.Booking.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Booking.Web.Controllers
{
    [CustomAuthenticationFilter]
    public class FavouriteController : BaseController
    {
        // GET: User
        [Route("favourite/index")]
        public ActionResult Index()
        {
            return View();
        }
    }
}