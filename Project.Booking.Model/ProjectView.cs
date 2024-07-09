using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
   public class ProjectView
    {
        private List<WebImage> _webImage;
        public List<WebImage> WebImageList
        {
            get { return (_webImage) ?? new List<WebImage>(); }
            set { _webImage = value; }
        }

        private List<ProjectModel> _project;
        public List<ProjectModel> ProjectList
        {
            get { return (_project) ?? new List<ProjectModel>(); }
            set { _project = value; }
        }
    }
}
