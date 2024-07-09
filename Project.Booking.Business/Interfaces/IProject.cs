using Project.Booking.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Business.Interfaces
{
    public interface IProject
    {
        ProjectView GetProjectData();
        ProjectDetailView GetProjectDetailData(Guid projectID);
        ProjectDetailView GetFloorList(Guid projectID, int buildID, int? floorID = null);
        Unit GetUnitDetail(Guid unitID);
        ProjectRegisterQuota GetRegisterQuota(Guid projectID, Guid registerID);

        void SaveTransferPayment(ProjectTransPayment model);
        List<ProjectTransPayment> GetTransferPaymentResourceList(Guid projectID, Guid registerID);
        void SaveDeletePayment(Guid ID);

        DateTime? GetRegisterAllowBookDate(Guid registerID);
        List<Unit> GetUnitAvailable(Guid projectID);
    }
}
