using Project.Booking.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
    public class Payment : ts_Payment
    {
        public BookingModel Booking { get; set; }
        public PaymentCredit PaymentCredit { get; set; }
        public Company Company { get; set; }
    }
}
