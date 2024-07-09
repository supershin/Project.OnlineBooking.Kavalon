using Project.Booking.Business.Interfaces;
using Project.Booking.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Booking.Data;
using Project.Booking.Model;
using System.Data.Entity;

namespace Project.Booking.Business.Sevices
{
    public class MasterService : IMaster
    {
        OnlineBookingEntities db = new OnlineBookingEntities();

        public List<WebImage> GetWebImageList()
        {
            var query = from w in db.tm_WebImage.Where(e => e.FlagActive == true)
                        join r in db.tm_Resource.Where(e => e.IsActive == true)
                            on w.ResourceID equals r.ID into _r
                        from r2 in _r.DefaultIfEmpty()
                        select new { w, r2 };

            return query.AsEnumerable().Select(e => new WebImage
            {
                ImageUrl = e.w.ImageUrl,
                ResourceFilePath = (e.r2 != null) ? e.r2.FilePath : null,
                LineOrder = e.w.LineOrder
            }).OrderBy(e => e.LineOrder).ToList();
        }
        public List<ProjectModel> GetProjectList()
        {
            return db.tm_Project.Where(e => e.FlagActive == true)
                .AsEnumerable().Select(e => new ProjectModel
                {
                    ID = e.ID,
                    ProjectCode = e.ProjectCode,
                    ProjectNameEN = e.ProjectNameEN,
                    ProjectNameTH = e.ProjectNameTH,
                    Location = e.Location,
                    Vocation = e.Vocation,
                    LineOrder = e.LineOrder
                }).OrderBy(e => e.LineOrder).ToList();
        }
        public List<ProjectBuild> GetProjectBuildList(Guid projectID)
        {
            var query = from pb in db.tr_ProjectBuild.Where(e => e.ProjectID == projectID && e.FlagActive == true)
                        join b in db.tm_Build.Where(e => e.FlagActive == true)
                          on pb.BuildID equals b.ID
                        select new { pb, b };
            return query.AsEnumerable().Select(e => new ProjectBuild
            {
                BuildID = e.b.ID,
                BuildName = e.b.Name,
                LineOrder = e.pb.LineOrder
            }).OrderBy(e => e.LineOrder).ToList();
        }
        public List<ProjectBuildFloor> GetProjectBuildFloorList(Guid projectID, int buildID, int? floorID = null)
        {
            var query = from bf in db.tr_ProjectBuildFloor.Where(e => e.ProjectID == projectID && e.BuildID == buildID
                                    && (e.FloorID == floorID || floorID == null) && e.FlagActive == true)
                        join f in db.tm_Floor.Where(e => e.FlagActive == true)
                            on bf.FloorID equals f.ID
                        join r in db.tm_Resource.Where(e => e.IsActive == true)
                            on bf.ResourceID equals r.ID into _r
                        from r2 in _r.DefaultIfEmpty()
                        select new { bf, f, r2 };
            return query.AsEnumerable().Select(e => new ProjectBuildFloor
            {
                FloorID = e.bf.FloorID,
                FloorName = e.f.Name,
                LineOrder = e.bf.LineOrder,
                FloorPlanFilePath = (e.r2 != null) ? e.r2.FilePath : null
            }).OrderBy(e => e.LineOrder).ToList();
        }
        public List<Unit> GetUnitList(Guid projectID, int? buildID, int? floorID)
        {
            var grBuildIDs = db.tr_ProjectBuildGroup.Where(e => e.ProjectID == projectID && e.BuildID == buildID).Select(e => e.ParentBuildID).ToArray();

            var query = from u in db.tm_Unit.Where(e => e.ProjectID == projectID && e.FlagActive == true                        
                        && (e.BuildID == buildID || buildID == null || grBuildIDs.Contains(e.BuildID))
                        && (e.FloorID == floorID || floorID == null))
                        join a in db.tm_Annotation
                            on u.AnnotationID equals a.ID into _a
                        from a2 in _a.DefaultIfEmpty()
                        join ust in db.tm_UnitStatus.Where(e => e.FlagActive == true)
                            on u.UnitStatusID equals ust.ID into _ust
                        from ust2 in _ust.DefaultIfEmpty()                        
                        select new { u, a2, ust2 };
            return query.AsEnumerable().Select(e => new Unit
            {
                ProjectID = e.u.ProjectID,
                ID = e.u.ID,
                UnitCode = e.u.UnitCode,
                BuildID = e.u.BuildID,
                FloorID = e.u.FloorID,
                ModelTypeID = e.u.ModelTypeID,
                Area = e.u.Area,
                SellingPrice = e.u.SellingPrice,
                UnitStatusID = e.u.UnitStatusID,
                AnnotationID = e.u.AnnotationID,
                SelectorType = (e.a2 != null) ? e.a2.SelectorType : null,
                SelectorValue = (e.a2 != null) ? e.a2.SelectorValue : null,
                UnitStatus = (e.ust2 != null) ? e.ust2.Name : null,
                UnitStatusColor = (e.ust2 != null) ? e.ust2.Color : null
            }).OrderBy(e => e.UnitCode).ToList();
        }
        public Unit GetUnitByID(Guid unitID)
        {
            var query = from u in db.tm_Unit.Where(e=>e.ID == unitID)
                                    .Include(e=>e.tm_Project)                        
                        join a in db.tm_Annotation
                            on u.AnnotationID equals a.ID into _a
                        from a2 in _a.DefaultIfEmpty()
                        join ust in db.tm_UnitStatus.Where(e => e.FlagActive == true)
                            on u.UnitStatusID equals ust.ID into _ust
                        from ust2 in _ust.DefaultIfEmpty()
                        select new { u, a2, ust2 };
            return query.AsEnumerable().Select(e => new Unit
            {
                ProjectID = e.u.ProjectID,
                ProjectName = e.u.tm_Project.ProjectNameTH,
                ID = e.u.ID,
                UnitCode = e.u.UnitCode,
                BuildID = e.u.BuildID,
                FloorID = e.u.FloorID,
                ModelTypeID = e.u.ModelTypeID,
                Area = e.u.Area,
                SellingPrice = e.u.SellingPrice,
                UnitStatusID = e.u.UnitStatusID,
                AnnotationID = e.u.AnnotationID,
                SelectorType = (e.a2 != null) ? e.a2.SelectorType : null,
                SelectorValue = (e.a2 != null) ? e.a2.SelectorValue : null,
                UnitStatus = (e.ust2 != null) ? e.ust2.Name : null,
                UnitStatusColor = (e.ust2 != null) ? e.ust2.Color : null
            }).FirstOrDefault();
        }
        public Unit GetUnitDetail(Guid ID)
        {
            var nowDate = DateTime.Now.Date;
            var query = from u in db.tm_Unit.Where(e => e.ID == ID && e.FlagActive == true)
                        join p in db.tm_Project.Where(e => e.FlagActive == true)
                            on u.ProjectID equals p.ID
                        join ust in db.tm_UnitStatus.Where(e => e.FlagActive == true)
                            on u.UnitStatusID equals ust.ID
                        join mo in db.tm_ModelType.Where(e => e.FlagActive == true)
                            on u.ModelTypeID equals mo.ID into _mo
                        from mo2 in _mo.DefaultIfEmpty()
                        join ut in db.tm_UnitType.Where(e => e.FlagActive == true)
                            on u.UnitTypeID equals ut.ID into _ut
                        from ut2 in _ut.DefaultIfEmpty()
                        join pm in db.tr_ProjectModelType.Where(e => e.FlagActive == true)
                            on new { u.ProjectID, u.ModelTypeID } equals new { pm.ProjectID, pm.ModelTypeID } into _pm
                        from pm2 in _pm.DefaultIfEmpty()
                        join r in db.tm_Resource.Where(e => e.IsActive == true)
                            on pm2.ResourceID equals r.ID into _r
                        from r2 in _r.DefaultIfEmpty()
                        join usp in db.tr_UnitSpecialPromotion.Where(e => e.FlagActive == true)
                            on u.ID equals usp.UnitID into _usp
                        from usp2 in _usp.DefaultIfEmpty()
                        join sp in db.tr_ProjectSpecialPromotion.Where(e => e.FlagActive == true && e.StartDate <= nowDate
                        && ( e.EndDate >= nowDate || e.EndDate == null ))
                            on usp2.SpecialPromotionID equals sp.ID into _sp
                        from sp2 in _sp.DefaultIfEmpty()                        
                        select new { u, p, mo2, ut2, ust, r2,sp2 };

            if (query.Any())
            {
                return query.AsEnumerable().Select(e => new Unit
                {
                    ID = e.u.ID,
                    ProjectID = e.u.ProjectID,                    
                    ProjectName = e.p.ProjectNameTH,
                    SpecialPromotion = (e.sp2 != null) ? e.sp2.Name : null,
                    UnitCode = e.u.UnitCode,
                    BuildID = e.u.BuildID,
                    FloorID = e.u.FloorID,
                    ModelTypeName = (e.mo2 != null) ? e.mo2.Name : null,
                    ModelTypePath = (e.r2 != null) ? e.r2.FilePath : null,
                    UnitTypeName = (e.ut2 != null) ? e.ut2.Name : null,
                    Area = e.u.Area,
                    AreaIncrease = e.u.AreaIncrease,
                    SellingPrice = e.u.SellingPrice,
                    SpecialPrice = e.u.SpecialPrice,
                    Discount = e.u.Discount,
                    BookingAmount = e.u.BookingAmount,
                    ContractAmount = e.u.ContractAmount,
                    UnitStatusID = e.u.UnitStatusID,
                    UnitStatus = e.ust.Name,
                    UnitStatusColor = e.ust.Color,
                    IsOnlineBooking = e.p.IsOnlineBooking.AsBool()
                }).FirstOrDefault();
            }
            return new Unit();
        }
        public Company GetCompany(Guid projectID, OnlineBookingEntities context = null)
        {
            db = (context != null) ? context : db;
            var query = from p in db.tm_Project.Where(e => e.FlagActive == true && e.ID == projectID)
                        join c in db.tm_Company.Where(e => e.FlagActive == true)
                           on p.CompanyID equals c.ID
                        select new { p, c };
            return query.AsEnumerable().Select(e => new Company
            {
                ID = e.c.ID,
                CompanyName = e.c.CompanyName,
                OmisePublicKey = e.c.OmisePublicKey,
                OmiseSecurityKey = e.c.OmiseSecurityKey
            }).FirstOrDefault();
        }
    }
}
