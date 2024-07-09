using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
    public class ProjectTransPayment
    {
        public string AppPath { get; set; }
        public System.Web.HttpFileCollection hfc { get; set; }
        public int iFile { get; set; }
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> ProjectID { get; set; }
        public Nullable<System.Guid> RegisterID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public Nullable<System.Guid> ResourceID { get; set; }
        public Nullable<System.DateTime> TransferDate { get; set; }
        public decimal Amount { get; set; }
        public Nullable<int> StatusID { get; set; }
        public string StatusName { get; set; }
        public Nullable<bool> FlagActive { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
