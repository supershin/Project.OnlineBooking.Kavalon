using Omise;
using Omise.Models;
using Project.Booking.Business.Sevices;
using Project.Booking.Constants;
using Project.Booking.Extensions;
using Project.Booking.Model;
using Project.Booking.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Project.Booking.Web.Controllers
{
    [CustomAuthenticationFilter]
    public class PaymentController : BaseController
    {
        // GET: Payment
        [HttpPost]
        public async Task<ActionResult> Checkout()
        {
            try
            {
                var bookingID = new Guid(Request.Form["BookingID"]);
                var itemPayment = payment.SavePayment(bookingID);
                var paymentCredit = await CreateCharge(itemPayment);
                payment.SavePaymentCredit(paymentCredit);
                RedirectToAuthorizeSecure(paymentCredit);
                return RedirectToAction("index", "booking");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = InnerException(ex);
                return RedirectToAction("index", "booking");
            }

        }
        private async Task<PaymentCredit> CreateCharge(Payment model)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            //Create charge on omise                
            var token = Request.Form["omiseToken"];
            var source = Request.Form["omiseSource"];
            var omise = new Client(pkey: model.Company.OmisePublicKey, skey: model.Company.OmiseSecurityKey);            
            long amount = Convert.ToInt64((model.Booking.BookingAmount) * 100);
            string description = string.Format("OnlineBooking|{0}|{1}|{2}|{3}"
                , model.Booking.ProjectCode, model.Booking.UnitCode, model.Booking.BookingNumber, model.PaymentNo);

            var obj = new Omise.Models.CreateChargeRequest();
            obj.Amount = amount;
            obj.Currency = "THB";
            obj.ReturnUri = baseUrl + "Payment/CreditProcess/" + model.ID;
            obj.Description = description;
            if (!string.IsNullOrEmpty(token))            
                obj.Card = token;            
            //else if (!string.IsNullOrEmpty(source))
            //{

            //}
            var charge = await omise.Charges.Create(obj);

            //set payment credit from charge
            var paymentCredit = SetPaymentCredit(new PaymentCredit() ,charge);
            paymentCredit.PaymentID = model.ID;
            return paymentCredit;

            ////Check Status
            //if (credit.Status == SystemConstants.OmiseChargeStatus.FAILED)
            //{
            //    throw new Exception(SystemConstants.Message.ERROR.PAYMETN_CREDIT_INVALID);
            //}
            //else
            //{
            //    //redirec to 3d secure
            //    Response.Redirect(charge.AuthorizeURI);
            //}


        }
        private PaymentCredit SetPaymentCredit(PaymentCredit item, Charge charge) {            
            item.ChargeID = charge.Id;
            item.IP = charge.IP;
            item.Customer = charge.Customer;
            item.Refunded = charge.Refunded;
            item.Amount = charge.Amount;
            item.Currency = charge.Currency;
            item.Description = charge.Description;
            item.Status = charge.Status.ToString();
            item.FailureCode = charge.FailureCode;
            item.FailureMessage = charge.FailureMessage;
            item.OmiseTransaction = charge.Transaction;
            item.Paid = charge.Paid;
            item.Reversed = charge.Reversed;
            item.Authorized = charge.Authorized;
            item.Capture = charge.Capture;
            item.ReturnURI = charge.ReturnURI;
            item.CardFinancing = charge.Card.Financing;
            item.CardBank = charge.Card.Bank;
            item.CardFirstDigit = null;
            item.CardLastDigit = charge.Card.LastDigits;
            item.CardBrand = charge.Card.Brand;
            item.CardExpirationMonth = charge.Card.ExpirationMonth;
            item.CardExpirationYear = charge.Card.ExpirationYear;
            item.CardName = charge.Card.Name;
            item.AuthorizeURI = charge.AuthorizeURI;
            item.CreateDate = charge.Created;
            return item;
        }
        private void RedirectToAuthorizeSecure(PaymentCredit credit) {
            //Check Status
            if (credit.Status == Constant.OmiseChargeStatus.FAILED)
            {
                throw new Exception(Constant.Message.Error.PAYMETN_CREDIT_INVALID);
            }
            else
            {
                //redirec to 3d secure
                 Response.Redirect(credit.AuthorizeURI);
            }
        }

        [Route("payment/creditprocess/{paymentID?}")]
        public async Task<ActionResult> CreditProcess(Guid paymentID)
        {            
            try
            {
                var credit = await RetrieveCharge(paymentID);
                payment.SavePaymentCredit(credit);                

                //Send Mail if success
                if (credit.Status == Constant.OmiseChargeStatus.SUCCESSFUL)
                {
                    var model = payment.GetPayment(paymentID);

                    var unit = master.GetUnitByID(model.Booking.UnitID.AsGuid());
                    NotifyUnitStatusSignalR(new SendUnitSignalModel(unit));

                    SendMailPayment(model);
                }

                return RedirectToAction("creditcomplete", "payment",new { paymentID = paymentID });
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = InnerException(ex);
                return RedirectToAction("Index", "Booking");
            }
        }
        private async Task<PaymentCredit> RetrieveCharge(Guid paymentID) {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            var credit = payment.GetPaymentCredit(paymentID);
            var company = master.GetCompany(credit.ProjectID);

            //Get Omise charge
            var omise = new Client(pkey: company.OmisePublicKey, skey: company.OmiseSecurityKey);            
            var charge = await omise.Charges.Get(credit.ChargeID);

            SetPaymentCredit(credit, charge);

            return credit;
        }

        [Route("payment/creditcomplete/{paymentID?}")]
        public ActionResult CreditComplete(Guid paymentID) {
            var model = payment.GetPayment(paymentID);
            return View(model);
        }

        private void SendMailPayment(Payment model) {
            string template = RenderPartialViewToString("~/Views/Templates/Payment_Email.cshtml", model);
            var email = new Email();
            email.To = new List<string> { UserProfile.Email };
            email.Subject = Constant.Email.SUBJECT.PAYMENT;
            email.Body = template;
            (new MailService()).SendMail(email);
        }
    }
}