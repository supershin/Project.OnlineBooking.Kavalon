using Project.Booking.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
    public class ProjectGalleryResource :tr_ProjectGalleryResource
    {
        public string FilePath { get; set; }
        public string GalleryName { get; set; }
    }
}
