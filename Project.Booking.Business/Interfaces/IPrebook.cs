using Project.Booking.Data;
using Project.Booking.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Business.Interfaces
{
    public interface IPrebook
    {
        List<ProjectQuota> GetProjectQuotaList(Guid projectID, int? id = null);
        void SaveProjectRegisterQuota(ProjectRegisterQuota model);
        ProjectQuota GetProjectQuota(ProjectQuota model);
        List<ProjectRegisterQuota> GetProjectRegisterQuotaList(Guid projectID, Guid registerID);
    }
}
