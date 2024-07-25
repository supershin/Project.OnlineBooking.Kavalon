using Project.Booking.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
    public class ProjectDetailView
    {

        private ProjectModel _project;
        public ProjectModel Project
        {
            get { return (_project) ?? new ProjectModel(); }
            set { _project = value; }
        }

        public string Builds { get; set; }

        private List<ProjectBuild> _projectBuild;
        public List<ProjectBuild> BuildList
        {
            get { return (_projectBuild) ?? new List<ProjectBuild>(); }
            set { _projectBuild = value; }
        }

        private List<ProjectBuildFloor> _projectBuildFloor;
        public List<ProjectBuildFloor> FloorList
        {
            get { return (_projectBuildFloor) ?? new List<ProjectBuildFloor>(); }
            set { _projectBuildFloor = value; }
        }

        private List<Unit> _unit;
        public List<Unit> UnitList
        {
            get { return (_unit) ?? new List<Unit>(); }
            set { _unit = value; }
        }

        public dynamic UnitAnnotationList { get; set; }
        public string FloorPlanFilePath { get; set; }

        public List<ProjectGallery> GalleryList { get; set; }

        private ProjectRegisterQuota _Quota;
        public ProjectRegisterQuota Quota
        {
            get
            {
                _Quota = _Quota ?? new ProjectRegisterQuota();
                return _Quota;
            }
            set { _Quota = value; }
        }

        private List<ProjectTransPayment> _PaymentResources;
        public List<ProjectTransPayment> PaymentResources
        {
            get { return (_PaymentResources) ?? new List<ProjectTransPayment>(); }
            set { _PaymentResources = value; }
        }

        public bool isAllowBoo { get; set; }
        public DateTime? AllowBookDate { get; set; }
        public double CountDownAllowBookDateSecond
        {
            get
            {
                if (this.AllowBookDate != null)
                    return (this.AllowBookDate.AsDate() - DateTime.Now).TotalSeconds;
                return 0;
            }
        }
        public List<ProjectQuota> ProjectQuotaList { get; set; } = new List<ProjectQuota>();

        public List<ProjectRegisterQuota> PreBookList { get; set; } = new List<ProjectRegisterQuota>();
    }
}
