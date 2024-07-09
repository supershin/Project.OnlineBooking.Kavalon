using Project.Booking.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
    public class ProjectBuildFloor : tr_ProjectBuildFloor
    {
        public string FloorName { get; set; }
        public string FloorPlanFilePath { get; set; }
    }
}
