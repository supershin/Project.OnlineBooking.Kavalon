using Project.Booking.Data;
using Project.Booking.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Business.Interfaces
{
    public interface IMaster
    {
        List<WebImage> GetWebImageList();
        List<ProjectModel> GetProjectList();
        List<ProjectBuild> GetProjectBuildList(Guid projectID);
        List<ProjectBuildFloor> GetProjectBuildFloorList(Guid projectID, int buildID, int? floorID = null);
        List<Unit> GetUnitList(Guid projectID, int? buildID, int? floorID);
        Unit GetUnitByID(Guid unitID);
        Unit GetUnitDetail(Guid ID);
        Company GetCompany(Guid projectID, OnlineBookingEntities context = null);
    }
}
