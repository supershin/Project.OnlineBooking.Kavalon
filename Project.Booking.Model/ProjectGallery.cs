using Project.Booking.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
    public class ProjectGallery :tr_ProjectGallery
    {
        public List<ProjectGalleryResource> ResourceList { get; set; }
    }
}
