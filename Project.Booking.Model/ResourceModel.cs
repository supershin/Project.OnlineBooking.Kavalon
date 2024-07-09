using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
    public class ResourceModel
    {
        public System.Guid ID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string MimeType { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}
