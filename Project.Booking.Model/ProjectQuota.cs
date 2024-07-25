using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
    public class ProjectQuota
    {
        public int ID { get; set; }
        public Guid ProjectID { get; set; }
        public string TransferBank { get; set; }
        public string TransferAccountNo { get; set; }
        public string Name{ get; set; }
        public int Quota { get; set; }
        public decimal? TotalPrice { get; set; }
        public int LineOrder { get; set; }
    }
}
