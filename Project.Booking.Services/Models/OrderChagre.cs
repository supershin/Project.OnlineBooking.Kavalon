using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Project.Booking.Services.Models
{
    public class OrderCharge
    {
        public int? CompanyID { get; set; }
        public string ChargeID { get; set; }
        public Guid OrderID { get; set; }
        public int? PaymentTypeID { get; set; }
        public int? MethodID { get; set; }
        public bool IsSettled { get; set; }
        public string transaction_state { get; set; }
        [JsonProperty("object")]        
        public string Object { get; set; }
        public string message { get; set; }
    }
}
