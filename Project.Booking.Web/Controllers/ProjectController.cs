using Microsoft.Ajax.Utilities;
using Omise.Resources;
using Project.Booking.Business.Sevices;
using Project.Booking.Constants;
using Project.Booking.Extensions;
using Project.Booking.Model;
using Project.Booking.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Booking.Web.Controllers
{
    [CustomAuthenticationFilter]
    public class ProjectController : BaseController
    {
        // eg: /project
        // eg: /project/Index            
        public ActionResult Index()
        {
            var model = project.GetProjectData();
            return View(model);
        }

        // eg: /project/detail
        // eg: /project/detail/en
        // eg: /project/detail/he
        [Route("project/detail/{projectid?}/{projecttype?}")]
        public ActionResult Detail(Guid projectID)
        {            
            ViewData["ProjectID"] = projectID;

            var model = project.GetProjectDetailData(projectID);
            model.Quota = project.GetRegisterQuota(model.Project.ID, UserProfile.ID);
            model.PaymentResources = project.GetTransferPaymentResourceList(model.Project.ID, UserProfile.ID);
            if (isConutDownComplete(model)) {
                getAllowBookDate(model);
                return View(model);
            }                
            else
            {
                return View($"~/Views/Project/CountDown_{model.Project.ProjectCode}.cshtml", model);
            }
        }

        [HttpPost]
        public JsonResult GetFloorList(Guid projectID, int buildID, int? floorID)
        {
            try
            {
                var model = project.GetFloorList(projectID, buildID, floorID);
                var partialFloorList = (floorID == null) ? RenderPartialViewToString("Partial_Floor_SelectList", model.FloorList) : null;
                var partialFloorPlan = RenderPartialViewToString("Partial_Detail_Booking_FloorPlan", model);
                return Json(new
                {
                    htmlFloorSelectList = partialFloorList,
                    htmlFloorPlan = partialFloorPlan,
                    success = true
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
        [HttpPost]
        public JsonResult GetUnitDetail(Guid ID)
        {
            try
            {
                var model = project.GetUnitDetail(ID);
                model.RandomView = getUnitRandomView(Constant.UNIT_RANDOM_VIEW);
                var partialUnitDetail = RenderPartialViewToString("Partial_Detail_Booking_Unit_Selected", model);

                return Json(new
                {
                    htmlUnitDetail = partialUnitDetail,
                    success = true
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
        [HttpPost]
        public JsonResult SaveTransferPayment(ProjectTransPayment model)
        {
            try
            {
                model.hfc = System.Web.HttpContext.Current.Request.Files;
                model.AppPath = AppDomain.CurrentDomain.BaseDirectory;
                validateTransferPayment(model);
                project.SaveTransferPayment(model);
                var transferResources = project.GetTransferPaymentResourceList(model.ProjectID.AsGuid(), UserProfile.ID);
                var html = RenderPartialViewToString("Partial_Detail_MyQuota_Transfer_List", transferResources);
                return Json(new
                {
                    success = true,
                    message = Constant.Message.Success.SAVE_SUCCESS,
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
        private bool isConutDownComplete(ProjectDetailView model)
        {
            var countDownDate = model.Project.ProjectConfig.CountDownDate;
            if (countDownDate == null
                || UserProfile.RegisterTypeID == Constant.Ext.REGISTER_TYPE_SALEKIT
                || UserProfile.ID == new Guid("CB1E281D-0CF9-49FB-93F4-CD3B782F5571")
                )
                return true;
            else
            {
                if (countDownDate.AsDate().Ticks <= DateTime.Now.Ticks)
                    return true;
                else return false;
            }
        }
        private void getAllowBookDate(ProjectDetailView model)
        {
            var allowBookDate = project.GetRegisterAllowBookDate(UserProfile.ID);
            model.AllowBookDate = allowBookDate;           
        }
        private int getUnitRandomView(int maxRandomNumber)
        {
            Random rnd = new Random();
            return rnd.Next(1, maxRandomNumber + 1);
        }
        private void validateTransferPayment(ProjectTransPayment model)
        {
            if (model.TransferDate == null)
                throw new Exception(Constant.Message.Error.PLEASE_INPUT_TRASFER_PAYMENT_DATE);
            else if (model.Amount <= 0)
                throw new Exception(Constant.Message.Error.PLEASE_INPUT_TRASFER_PAYMENT_AMOUNT);

            for (int iCnt = 0; iCnt <= model.hfc.Count - 1; iCnt++)
            {
                if (model.hfc[iCnt].ContentLength <= 0)
                {
                    throw new Exception(Constant.Message.Error.PLEASE_ATTACH_TRASFER_PAYMENT);
                }
            }
        }
        [HttpPost]
        public JsonResult SaveDeletePayment(ProjectTransPayment model)
        {
            try
            {               
                project.SaveDeletePayment(model.ID);
                var transferResources = project.GetTransferPaymentResourceList(model.ProjectID.AsGuid(), UserProfile.ID);
                var html = RenderPartialViewToString("Partial_Detail_MyQuota_Transfer_List", transferResources);
                return Json(new
                {
                    success = true,
                    message = Constant.Message.Success.SAVE_SUCCESS,
                    html
                });
            }
            catch (Exception ex)
            {
                var transferResources = project.GetTransferPaymentResourceList(model.ProjectID.AsGuid(), UserProfile.ID);
                var html = RenderPartialViewToString("Partial_Detail_MyQuota_Transfer_List", transferResources);
                return Json(new
                {
                    success = false,
                    html,
                    message = InnerException(ex)
                });
            }
        }
        [HttpPost]
        public JsonResult GetUnitAvailable(Guid ProjectID)
        {
            try
            {
                var model = project.GetUnitAvailable(ProjectID);
                var html = RenderPartialViewToString("Partial_View_Unit_Available_Modal", model);
                return Json(new
                {
                    success = true,
                    message = Constant.Message.Success.SAVE_SUCCESS,
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