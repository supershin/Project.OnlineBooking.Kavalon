using Project.Booking.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
    public class ProjectRegisterQuota  : tr_ProjectRegisterQuota
    {
        public Guid? PaymentID { get; set; }
        public int? PaymentTypeID { get; set; }
        public string ProjectQuotaName { get; set; }
        public string StatusName { get; set; }
        public int? UseQuota { get; set; }

        /// <summary>
        /// Payment Transfer
        /// </summary>
        public DateTime? TransferDate { get; set; }
        public string Hours { get; set; }
        public string Minutes { get; set; }
        public decimal? Amount { get; set; }        
        public Guid? ResourceID { get; set; }
        public string AppPath { get; set; }
        public System.Web.HttpFileCollection hfc { get; set; }
        public string FilePath { get; set; }
    }
}
