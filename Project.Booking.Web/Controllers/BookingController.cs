using Project.Booking.Business.Sevices;
using Project.Booking.Constants;
using Project.Booking.Extensions;
using Project.Booking.Model;
using Project.Booking.Web.Filters;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Booking.Web.Controllers
{
    [CustomAuthenticationFilter]
    public class BookingController : BaseController
    {
        // GET: Booking
        public ActionResult Index()
        {
            string unitBooked = "";
            var model = booking.GetBookingHistory();
            if (model.Any())
            {
                var quota = project.GetRegisterQuota(model.FirstOrDefault().ProjectID.AsGuid(), UserProfile.ID);
                unitBooked = $"Unit Booked {quota.UseQuota.AsInt()} of {quota.Quota.AsInt()}";
            }
            ViewBag.UnitBooked = unitBooked;

            return View(model);
        }

        [HttpPost]
        public JsonResult SaveBooking(Guid unitID)
        {
            try
            {
                booking.SaveUpdateUnitStatus_VIP(unitID);
                var bookingID = booking.SaveBooking(unitID);
                var model = master.GetUnitByID(unitID);
                var quota = project.GetRegisterQuota(model.ProjectID.AsGuid(), UserProfile.ID);
                var unitDetail = project.GetUnitDetail(unitID);
                var bookingModel = booking.GetBookingHistory(bookingID).FirstOrDefault();
                var partialUnitDetail = RenderPartialViewToString("~/Views/Project/Partial_Detail_Booking_Unit_Selected.cshtml", unitDetail);
                NotifyUnitStatusSignalR(new SendUnitSignalModel(model));
                SendMailBooking(bookingModel);
                return Json(new
                {
                    success = true,
                    message = Constant.Message.Success.SAVE_BOOKING_SUCCESS,
                    htmlPartialUnitDetail = partialUnitDetail,
                    useQuota = quota.UseQuota.AsInt(),
                    quota = $"{quota.UseQuota.AsInt()} of {quota.Quota.AsInt()}",
                    myQuota = quota.Quota.AsInt()
                });
            }
            catch (Exception ex)
            {
                var unitStatus = master.GetUnitByID(unitID);
                NotifyUnitStatusSignalR(new SendUnitSignalModel(unitStatus));
                var model = project.GetUnitDetail(unitID);
                var quota = project.GetRegisterQuota(model.ProjectID.AsGuid(), UserProfile.ID);
                var partialUnitDetail = RenderPartialViewToString("~/Views/Project/Partial_Detail_Booking_Unit_Selected.cshtml", model);
                return Json(new
                {
                    success = false,
                    message = InnerException(ex),
                    quota = $"{quota.UseQuota.AsInt()} of {quota.Quota.AsInt()}",
                    htmlPartialUnitDetail = partialUnitDetail
                });
            }
        }
        [HttpPost]
        public JsonResult SaveCancelBooking(Guid bookingID)
        {
            try
            {
                var unitID = booking.SaveCancelBooking(bookingID);
                var model = master.GetUnitByID(unitID);
                NotifyUnitStatusSignalR(new SendUnitSignalModel(model));
                return Json(new
                {
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
        public JsonResult SaveBookingCustomer(BookingModel model)
        {
            var success = true;
            var message = Constant.Message.Success.SAVE_SUCCESS;
            var refresh = false;
            var partialCustomer = "";
            var partialDetail = "";
            var partialAction = "";
            try
            {
                ValidateBookingCustomer(model);
                refresh = true;
                booking.SaveBookingCustomer(model);
            }
            catch (Exception ex)
            {
                success = false;
                message = InnerException(ex);
            }
            if (refresh)
            {
                model = booking.GetBookingHistory(model.ID).FirstOrDefault();
                partialCustomer = RenderPartialViewToString("Partial_Booking_Customer", model);
                partialDetail = RenderPartialViewToString("Partial_Booking_Detail", model);
                partialAction = RenderPartialViewToString("Partial_Booking_Action", model);
            }

            return Json(new
            {
                success,
                message,
                refresh,
                htmlDetail = partialDetail,
                htmlAction = partialAction,
                htmlCustomer = partialCustomer
            });
        }
        [HttpPost]
        public JsonResult GetTermpayment(Guid bookingID)
        {
            var model = booking.GetBookingCheckout(bookingID);
            try
            {
                VerifyBookingStatusForPayment(model.Booking.BookingStatusID.AsInt(), model.Booking.PaymentDueDate.AsDate());
                var partialTermPayment = RenderPartialViewToString("Partial_TermPayment_Modal", model);
                return Json(new
                {
                    htmlTermPayment = partialTermPayment,
                    success = true
                });
            }
            catch (Exception ex)
            {
                var partialCustomer = RenderPartialViewToString("Partial_Booking_Customer", model.Booking);
                var partialDetail = RenderPartialViewToString("Partial_Booking_Detail", model.Booking);
                var partialAction = RenderPartialViewToString("Partial_Booking_Action", model.Booking);
                return Json(new
                {
                    success = false,
                    htmlDetail = partialDetail,
                    htmlAction = partialAction,
                    htmlCustomer = partialCustomer,
                    message = InnerException(ex)
                });
            }
        }

        private void VerifyBookingStatusForPayment(int bookingStatusID, DateTime paymentDueDate)
        {
            if (!bookingStatusID.Equals(Constant.BookingStatus.WAIT_PAYMENT)
                || paymentDueDate <= DateTime.Now)
                throw new Exception(Constant.Message.Error.BOOKING_PAYMENT_INVALID);
        }
        private void ValidateBookingCustomer(BookingModel model)
        {
            if (string.IsNullOrEmpty(model.CustomerFirstName.ToStringNullable())
                || string.IsNullOrEmpty(model.CustomerLastName.ToStringNullable())
                || string.IsNullOrEmpty(model.CustomerEmail.ToStringNullable())
                || string.IsNullOrEmpty(model.CustomerMobile.ToStringNullable()))

                throw new Exception(Constant.Message.Error.BOOKING_CUSTOMER_INVALID);
            //else if (!VerifyCitizenID.IsValidCheckPersonID(model.CustomerCitizenID.ToStringNullable()))
            //    throw new Exception(Constant.Message.Error.CITIZEN_FORMAT_INVALID);
            else if (!IsValidEmail(model.CustomerEmail.ToStringNullable()))
                throw new Exception(Constant.Message.Error.EMAIL_FORMAT_INVALID);
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        private void SendMailBooking(BookingModel model)
        {
            try
            {
                model.BaseUrl = baseUrl;
                model.BaseUrl = "https://portal.rhombho.co.th/salekit/";
                string template = RenderPartialViewToString("~/Views/Templates/Booking_Email.cshtml", model);
                var email = new Email();
                email.To = new List<string> { model.CustomerEmail };
                email.Subject = Constant.Email.SUBJECT.BOOKING;
                email.Body = template;
                (new MailService()).SendMail(email);
            }
            catch (Exception)
            {

            }

        }

        [HttpGet]
        public ActionResult BookingVIPSendMail(int length)
        {

            var bookingVIPs = booking.GetBookingVIPSendMail(length);
            var errors = VIPSendMail(bookingVIPs);
            return View(errors);
        }
        private List<BookingVIP> VIPSendMail(List<BookingVIP> lst)
        {
            List<BookingVIP> error = new List<BookingVIP>();
            foreach (var vip in lst)
            {
                try
                {
                    var itemBooking = booking.GetBookingSendMail(vip.BookingID).FirstOrDefault();
                    //itemBooking.CustomerEmail = "siripoj@assetwise.co.th";
                    itemBooking.CustomerEmail = vip.Email;
                    SendMailBooking(itemBooking);                      
                    vip.IsSendMail = true;
                    booking.SaveBookingVIPSendMail(vip);
                }
                catch (Exception ex)
                {
                    error.Add(new BookingVIP
                    {
                        ProjectName = vip.ProjectName,
                        BookingNumber = vip.BookingNumber,
                        UnitCode = vip.UnitCode,
                        Email = vip.Email,
                        Message = ex.Message
                    });
                }
            }
            return error;
        }
    }
}