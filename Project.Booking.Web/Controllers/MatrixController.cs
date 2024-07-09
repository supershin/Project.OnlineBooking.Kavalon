using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Booking.Web.Controllers
{    
    public class MatrixController : BaseController
    {
        // GET: Matrix      
        public ActionResult Index(string projectCode,string builds)
        {
            var model = matrix.GetMatrix(projectCode, builds);
            return View(model);
        }
        [HttpPost]
        public ActionResult GetUnit(Guid ID)
        {
            try
            {
                var unit = matrix.GetUnitByID(ID);
                var html = RenderPartialViewToString("Partial_Unit_Modal", unit);
                return Json(new
                {
                    success = true,
                    html
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = InnerException(ex)
                });
            }
        }
    }
}