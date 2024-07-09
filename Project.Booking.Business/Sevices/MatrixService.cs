using Project.Booking.Business.Interfaces;
using Project.Booking.Constants;
using Project.Booking.Data;
using Project.Booking.Extensions;
using Project.Booking.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Business.Sevices
{
    public class MatrixService : IMatrix
    {
        private readonly OnlineBookingEntities _context;
        public MatrixService()
        {
            _context = new OnlineBookingEntities();
        }
        public MatrixView GetMatrix(string projectCode, string builds)
        {
            var matrixView = new MatrixView();
            var project = _context.tm_Project.FirstOrDefault(e => e.ProjectCode == projectCode && e.FlagActive == true);
            if (project == null)
                throw new Exception(Constant.Message.Error.PROJECT_DOES_NOT_EXISTS);
            matrixView.ProjectName = project.ProjectNameEN;

            var arrBuild = (!string.IsNullOrEmpty(builds.ToStringNullable())) ?
                                builds.Split(',').ToArray() : new string[] { };
            var query = from u in _context.tm_Unit.Where(e => e.ProjectID == project.ID && e.FlagActive == true)
                        join b in _context.tm_Build
                            on u.BuildID equals b.ID
                        join f in _context.tm_Floor
                            on u.FloorID equals f.ID
                        where builds.Contains(b.Name)
                        select new { u, b, f };
            var data = query.AsEnumerable().Select(e => new UnitView
            {
                ID = e.u.ID,
                ProjectID = (Guid)e.u.ProjectID,
                BuildID = (int)e.u.BuildID,
                Build = e.b.Name,
                Floor = e.f.ID,
                FloorName = e.f.Name,
                Room = e.u.Room.AsInt(),
                UnitCode = e.u.UnitCode,
                UnitStatusID = e.u.UnitStatusID.AsInt(),
                UnitStatusColor = e.u.tm_UnitStatus.Color,
                UnitStatusName = e.u.tm_UnitStatus.Name,
            }).OrderBy(e => e.Build).ThenByDescending(e => e.Floor).ThenBy(e => e.Room).ToList();

            foreach (var buildName in data.Select(e => e.Build).Distinct().OrderBy(e => e))
            {
                var projectID = data.Where(e => e.Build == buildName).FirstOrDefault().ProjectID;
                var buildID = data.Where(e => e.Build == buildName).FirstOrDefault().BuildID;

                var buildView = new BuildView();
                var buildItem = data.Where(e => e.Build == buildName).ToList();
                buildView.FloorMax = buildItem.Max(e => e.Floor);
                buildView.FloorMin = buildItem.Min(e => e.Floor);
                buildView.RoomMax = buildItem.Max(e => e.Room);
                buildView.RoomMin = buildItem.Min(e => e.Room);
                buildView.MatrixConfigs = getMatrixConfig(projectID, buildID);
                for (int floor = buildView.FloorMax; floor >= buildView.FloorMin; floor--)
                {
                    var floorView = new FloorView();
                    var floorItem = buildItem.Where(e => e.Floor == floor).ToList();
                    for (int room = buildView.RoomMin; room <= buildView.RoomMax; room++)
                    {
                        var unit = floorItem.Where(e => e.Room == room).FirstOrDefault() ?? new UnitView();
                        floorView.FloorName = floor;
                        floorView.UnitList.Add(unit);
                    }
                    buildView.BuildName = buildName;
                    buildView.FloorList.Add(floorView);
                }
                matrixView.BuildList.Add(buildView);
            }
            return matrixView;
        }
        public List<MatrixConfig> getMatrixConfig(Guid projectID, int buildID)
        {
            return _context.tr_Matrix_Config.Where(e => e.ProjectID == projectID && e.BuildID == buildID && e.FlagActive == true)
                        .AsEnumerable().Select(e => new MatrixConfig
                        {
                            ID = e.ID,
                            ProjectID = e.ProjectID,
                            BuildID = e.BuildID,
                            Text = e.Text,
                            RowNo = e.RowNo,
                            ColSpan = e.ColSpan,
                            LineOrder = e.LineOrder,
                            Style = e.Style                           
                        }).OrderBy(e => e.RowNo).ThenBy(e => e.LineOrder).ToList();
        }
        public UnitView GetUnitByID(Guid unitID)
        {
            var query = from u in _context.tm_Unit.Where(e => e.FlagActive == true && e.ID == unitID)
                                                  .Include(e => e.tm_Project)
                                                  .Include(e => e.tm_UnitType)
                                                  .Include(e => e.tm_Build)
                                                  .Include(e => e.tm_Floor)
                                                  .Include(e => e.tm_ModelType)
                                                  .Include(e => e.tm_UnitStatus)
                        select new { u };
            return query.AsEnumerable().Select(e => new UnitView
            {
                ID = e.u.ID,
                ProjectName = e.u.tm_Project.ProjectNameTH,
                UnitCode = e.u.UnitCode,
                UnitTypeName = e.u.tm_UnitType.Name,
                BuildName = e.u.tm_Build.Name,
                FloorName = e.u.tm_Floor.Name,
                ModelTypeName = e.u.tm_ModelType.Name,
                Area = e.u.Area,
                AreaIncrease = e.u.AreaIncrease,
                SellingPrice = e.u.SellingPrice,
                Discount = e.u.Discount,
                SpecialPrice = e.u.SpecialPrice,
                BookingAmount = e.u.BookingAmount,
                UnitStatusID = (int)e.u.UnitStatusID,
                UnitStatusName = e.u.tm_UnitStatus.Name,
                UnitStatusColor = e.u.tm_UnitStatus.Color
            }).SingleOrDefault();
        }
    }
}
