using Project.Booking.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
    public class Unit : tm_Unit
    {        
        public string ProjectName { get; set; }
        public string ModelTypeName { get; set; }
        public string ModelTypePath { get; set; }
        public string UnitTypeName { get; set; }
        
        public string SelectorType { get; set; }
        public string SelectorValue { get; set; }
        public string UnitStatus { get; set; }
        public string UnitStatusColor { get; set; }
        public string SpecialPromotion { get; set; }
        public string BuildName { get; set; }
        public int FloorName { get; set; }
        public int RandomView { get; set; }
        public bool IsOnlineBooking { get; set; }
        public int CntUnitAvailable { get; set; }
    }
}
