using Project.Booking.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
    public class PaymentCredit : ts_Payment_Credit
    {
        public DateTime PaymentDate { get; set; }
        public string PaymentNo { get; set; }
        public Guid ProjectID { get; set; }
        public Guid BookingID { get; set; }
    }
}
