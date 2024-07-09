using Project.Booking.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Business.Interfaces
{
    public interface IPayment
    {
        Payment SavePayment(Guid bookingID);
        void SavePaymentCredit(PaymentCredit model);
        PaymentCredit GetPaymentCredit(Guid paymentID);
        Payment GetPayment(Guid paymentID);
    }
}
