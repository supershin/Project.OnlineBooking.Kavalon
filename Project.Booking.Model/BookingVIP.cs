using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
    public class BookingVIP
    {
        public Guid ID { get; set; }
        public string ProjectName { get; set; }
        public string BookingNumber { get; set; }
        public string UnitCode { get; set; }
        public string Email { get; set; }
        public Guid BookingID { get; set; }
        public bool IsSendMail { get; set; }
        public string Message { get; set; }
    }
}
