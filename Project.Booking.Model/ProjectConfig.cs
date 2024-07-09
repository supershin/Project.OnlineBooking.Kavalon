using Project.Booking.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
    public class ProjectConfig
    {
        public Guid ProjectID { get; set; }
        public DateTime? CountDownDate { get; set; }
        public string CountDownTitle { get; set; }
        public double CountDownSecond { get {
                return (this.CountDownDate.AsDate() - DateTime.Now).TotalSeconds;
            } }
    }
}
