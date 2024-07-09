using Project.Booking.Business.Interfaces;
using Project.Booking.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Project.Booking.Data;
using Project.Booking.Constants;
using Project.Booking.Extensions;

namespace Project.Booking.Business.Sevices
{
    public class PaymentService : IPayment
    {
        OnlineBookingEntities db = new OnlineBookingEntities();

        #region Properties
        private IBooking _booking;
        private IBooking booking
        {
            get
            {
                return (_booking == null) ? _booking = new BookingService(UserProfile) : _booking;
            }
        }
        private IMaster _master;
        private IMaster master
        {
            get
            {
                return (_master == null) ? _master = new MasterService() : _master;
            }
        }
        private UserProfile UserProfile { get; set; }
        #endregion

        public PaymentService(UserProfile _userProfile)
        {
            this.UserProfile = _userProfile;
        }

        #region Payment
        public Payment GetPayment(Guid paymentID)
        {
            var query = from pm in db.ts_Payment.Where(e => e.ID == paymentID && e.FlagActive == true)
                        select new { pm };
            var model = query.AsEnumerable().Select(e => new Payment
            {
                ID = e.pm.ID,
                PaymentNo = e.pm.PaymentNo,
                PaymentTypeID = e.pm.PaymentTypeID,
                PaymentDate = e.pm.PaymentDate,
                PaymentCredit = GetPaymentCreditComplete(e.pm.ID),
                Booking = booking.GetBookingHistory(e.pm.BookingID).FirstOrDefault()
            }).FirstOrDefault();
            return model;
        }
        public Payment SavePayment(Guid bookingID)
        {

            try
            {
                TransactionOptions option = new TransactionOptions();
                option.Timeout = new TimeSpan(1, 0, 0);
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
                {
                    var payment = SavePaymentData(bookingID);
                    scope.Complete();
                    return payment;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private Payment SavePaymentData(Guid bookingID)
        {
            using (var context = new OnlineBookingEntities())
            {
                var checkout = new CheckoutViewModel();
                checkout.Booking = booking.GetBookingHistory(bookingID, context).FirstOrDefault();
                checkout.Company = master.GetCompany(checkout.Booking.ProjectID.AsGuid(), context);
                VerifyBookingStatusForPayment(checkout.Booking.BookingStatusID.AsInt(), checkout.Booking.PaymentDueDate.AsDate());

                var model = new Payment();
                model.BookingID = bookingID;
                model.PaymentNo = GeneratePaymentNumber(context, checkout.Booking.ProjectID.AsGuid());
                model.Company = checkout.Company;
                model.Booking = checkout.Booking;

                var item = SetPayment(new ts_Payment(), model);
                context.Entry(item).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
                return model;
            }
        }
        private ts_Payment SetPayment(ts_Payment item, Payment model)
        {
            item.ID = Guid.NewGuid();
            item.BookingID = model.BookingID;
            item.PaymentTypeID = Constant.Ext.PAYMENT_TYPE_CREDIT_ID;
            item.PaymentNo = model.PaymentNo;
            item.PaymentDate = DateTime.Now;
            item.FlagActive = true;
            item.CreateDate = DateTime.Now;
            item.CreateBy = UserProfile.ID;
            item.UpdateDate = DateTime.Now;
            item.UpdateBy = UserProfile.ID;

            model.ID = item.ID;
            return item;
        }
        private string GeneratePaymentNumber(OnlineBookingEntities context, Guid projectID)
        {
            var projectCode = context.tm_Project.Where(e => e.FlagActive == true && e.ID == projectID).FirstOrDefault().ProjectCode;

            var dt = DateTime.Now;
            string year = dt.Year.ToString().Right(2);
            string month = dt.Month.ToString("00");
            string paymentNumber = string.Format("{0}-{1}{2}{3}", "PAY", projectCode, year, month);

            var query = context.ts_Payment.Where(e => e.PaymentNo.Contains(paymentNumber));
            if (query.Any())
            {
                var maxNumber = query.OrderByDescending(e => e.PaymentNo).FirstOrDefault().PaymentNo;
                var newNumber = Convert.ToInt32(maxNumber.Right(5)) + 1;
                paymentNumber = string.Format("{0}{1}", paymentNumber, newNumber.ToString("00000"));
            }
            else paymentNumber = string.Format("{0}{1}", paymentNumber, "00001");
            return paymentNumber;
        }
        #endregion

        #region Credit
        private PaymentCredit GetPaymentCreditComplete(Guid paymentID) {
            return db.ts_Payment_Credit.Where(e => e.PaymentID == paymentID && e.FlagActive == true)
                    .AsEnumerable().Select(e => new PaymentCredit
                    {
                        CardBank = e.CardBank,
                        CardName = e.CardName,
                        CardBrand = e.CardBrand,
                        CardLastDigit = e.CardLastDigit,
                        Amount = e.Amount,
                        Status = e.Status
                    }).FirstOrDefault();
        }
        public void SavePaymentCredit(PaymentCredit model)
        {
            try
            {
                TransactionOptions option = new TransactionOptions();
                option.Timeout = new TimeSpan(1, 0, 0);
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
                {
                    SavePaymentCreditData(model);
                    if (model.Status == Constant.OmiseChargeStatus.SUCCESSFUL)
                    {
                        var bookingItem = booking.UpdateBookingStatus(model.BookingID, Constant.BookingStatus.PAYMENT_SUCCESS);
                        booking.UpdateUnitStatus(bookingItem.UnitID.AsGuid(), Constant.UnitStatus.PAYMENT);
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SavePaymentCreditData(PaymentCredit model)
        {
            using (var context = new OnlineBookingEntities())
            {
                var query = context.ts_Payment_Credit.Where(e => e.ID == model.ID && e.FlagActive == true);
                if (!query.Any())
                {
                    var item = SetPaymentCredit(new ts_Payment_Credit(), model);
                    context.Entry(item).State = System.Data.Entity.EntityState.Added;
                }
                else
                {
                    var item = SetPaymentCredit(query.FirstOrDefault(), model);
                    context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                context.SaveChanges();
            }
        }
        private ts_Payment_Credit SetPaymentCredit(ts_Payment_Credit item, PaymentCredit model)
        {
            if (item.ID == Guid.Empty)
            {
                item.ID = Guid.NewGuid();
                item.FlagActive = true;
                item.CreateDate = DateTime.Now;
                item.CreateBy = UserProfile.ID;
            }
            item.PaymentID = model.PaymentID;
            item.ChargeID = model.ChargeID;
            item.IP = model.IP;
            item.Customer = model.Customer;
            item.Refunded = model.Refunded;
            item.Amount = model.Amount;
            item.Currency = model.Currency;
            item.Description = model.Description;
            item.Status = model.Status;
            item.FailureCode = model.FailureCode;
            item.FailureMessage = model.FailureMessage;
            item.OmiseTransaction = model.OmiseTransaction;
            item.Paid = model.Paid;
            item.Reversed = model.Reversed;
            item.Authorized = model.Authorized;
            item.Capture = model.Capture;
            item.ReturnURI = model.ReturnURI;
            item.CardFinancing = model.CardFinancing;
            item.CardBank = model.CardBank;
            item.CardFirstDigit = model.CardFirstDigit;
            item.CardLastDigit = model.CardLastDigit;
            item.CardBrand = model.CardBrand;
            item.CardExpirationMonth = model.CardExpirationMonth;
            item.CardExpirationYear = model.CardExpirationYear;
            item.CardName = model.CardName;
            item.AuthorizeURI = model.AuthorizeURI;
            item.UpdateDate = DateTime.Now;
            item.UpdateBy = UserProfile.ID;
            return item;
        }
        public PaymentCredit GetPaymentCredit(Guid paymentID)
        {
            var query = from cr in db.ts_Payment_Credit.Where(e => e.PaymentID == paymentID && e.FlagActive == true)
                        join pa in db.ts_Payment.Where(e => e.FlagActive == true)
                            on cr.PaymentID equals pa.ID
                        join bk in db.ts_Booking.Where(e => e.FlagActive == true)
                            on pa.BookingID equals bk.ID
                        select new { cr, pa, bk };
            return query.AsEnumerable().Select(e => new PaymentCredit
            {
                ID = e.cr.ID,
                ProjectID = e.bk.ProjectID.AsGuid(),
                PaymentID = e.cr.PaymentID,
                BookingID = e.pa.BookingID.AsGuid(),
                ChargeID = e.cr.ChargeID
            }).FirstOrDefault();
        }
        #endregion


        private void VerifyBookingStatusForPayment(int bookingStatusID, DateTime paymentDueDate)
        {
            if (!bookingStatusID.Equals(Constant.BookingStatus.WAIT_PAYMENT)
                || paymentDueDate <= DateTime.Now)
                throw new Exception(Constant.Message.Error.BOOKING_PAYMENT_INVALID);
        }
    }
}
