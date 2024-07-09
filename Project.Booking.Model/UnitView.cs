using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
    public class UnitView
    {
        public Guid ID { get; set; }
        public Guid ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string UnitCode { get; set; }
        public decimal? Area { get; set; }
        public decimal? AreaIncrease { get; set; }
        public int BuildID { get; set; }
        public string Build { get; set; }
        public string BuildName { get; set; }
        public int Floor { get; set; }
        public string FloorName { get; set; }
        public int Room { get; set; }
        public string UnitTypeName { get; set; }
        public string ModelTypeName { get; set; }
        public int UnitStatusID { get; set; }
        public string UnitStatusName { get; set; }
        public string UnitStatusColor { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? SpecialPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal? BookingAmount { get; set; }
    }
}
