using Project.Booking.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
    public class BookingModel : ts_Booking
    {
        public string BaseUrl { get; set; }
        public string ModelTypePath { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string UnitCode { get; set; }
        public string ModelTypeName { get; set; }
        public string UnitTypeName { get; set; }
        public decimal? Area { get; set; }
        public decimal? AreaIncrease { get; set; }        
        public string BookingStatus { get; set; }
        public string BookingStatusColor { get; set; }
        public string SpecialPromotion { get; set; }
        public PaymentCredit PaymentCredit { get; set; }
        public string MasterPlanPath { get; set; }
        public string FloorPlanPath { get; set; }
        public string UnitPlanPath { get; set; }
    }
}
