using Omise.Resources;
using Project.Booking.Constants;
using Project.Booking.Extensions;
using Project.Booking.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Booking.Web.Controllers
{
    public class PrebookController : BaseController
    {
        [HttpPost]
        public JsonResult SaveProjectRegisterQuota(ProjectRegisterQuota model)
        {
            try
            {
                model.hfc = System.Web.HttpContext.Current.Request.Files;
                model.AppPath = AppDomain.CurrentDomain.BaseDirectory;
                validateProjectRegisterQuota(model);
                prebook.SaveProjectRegisterQuota(model);
                var projectRegisterQuotas = prebook.GetProjectRegisterQuotaList(model.ProjectID.AsGuid(), UserProfile.ID);                                ;
                var html = RenderPartialViewToString("~/Views/Project/Partial_Detail_MyQuota_Prebook_List.cshtml", projectRegisterQuotas);
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
        [HttpPost]
        public JsonResult GetProjectQuota(ProjectQuota model)
        {
            try
            {
                if (model.ID == 0)
                    throw new Exception(Constant.Message.Error.PROJECT_QUOTA_NOT_FOUND);
                var projectQuota = prebook.GetProjectQuota(model);
                var html = RenderPartialViewToString("~/Views/Project/Partial_Detail_MyQuota_Prebook_Transfer_Modal.cshtml", projectQuota);
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
        private void validateProjectRegisterQuota(ProjectRegisterQuota model)
        {
            if (model.PaymentTypeID == Constant.Ext.PAYMENT_TYPE_TRANSFER_ID)
            {
                if (model.TransferDate == null)
                    throw new Exception(Constant.Message.Error.PLEASE_INPUT_PAYMENT_TRANSFER_DATE);
                else if (model.Amount <= 0)
                    throw new Exception(Constant.Message.Error.PLEASE_INPUT_PAYMENT_TRANSFER_AMOUNT);

                for (int iCnt = 0; iCnt <= model.hfc.Count - 1; iCnt++)
                {
                    if (model.hfc[iCnt].ContentLength <= 0)
                    {
                        throw new Exception(Constant.Message.Error.PLEASE_INPUT_PAYMENT_TRANSFER_ATTACH);
                    }
                }
            }
        }
    }
}