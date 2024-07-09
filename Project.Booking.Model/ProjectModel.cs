using Project.Booking.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
    public class ProjectModel :tm_Project
    {
        private ProjectConfig _projectConfig;
        public ProjectConfig ProjectConfig
        {
            get
            {
                _projectConfig = _projectConfig ?? new ProjectConfig();
                return _projectConfig;
            }
            set { _projectConfig = value; }
        }

        private List<ProjectResource> _projectResource;

        public List<ProjectResource> ProjectResourceList
        {
            get { return (_projectResource)??new List<ProjectResource>(); }
            set { _projectResource = value; }
        }

    }
}
