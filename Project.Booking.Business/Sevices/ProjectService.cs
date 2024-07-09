using Project.Booking.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Booking.Data;
using Project.Booking.Model;
using Project.Booking.Extensions;
using Project.Booking.Constants;
using System.Transactions;
using HeyRed.Mime;
using System.IO;
using System.Web;
using System.Threading;

namespace Project.Booking.Business.Sevices
{
    public class ProjectService : IProject
    {
        OnlineBookingEntities db = new OnlineBookingEntities();

        #region Properties
        private IMaster _master;
        private IMaster master
        {
            get
            {
                return (_master == null) ? _master = new MasterService() : _master;
            }
        }
        private UserProfile UserProfile { get; set; }
        #endregion

        public ProjectService(UserProfile _userProfile)
        {
            UserProfile = _userProfile;
        }

        #region Project
        public ProjectView GetProjectData()
        {

            var model = new ProjectView();
            model.WebImageList = master.GetWebImageList();
            model.ProjectList = GetProjectList();
            return model;
        }
        private List<ProjectModel> GetProjectList(Guid? projectID = null)
        {
            var query = from p in db.tm_Project.Where(e => e.FlagActive == true
                        && (e.ID == projectID || projectID == null)
                        && (e.IsOnlineBooking == true || UserProfile.RegisterTypeID == Constant.Ext.REGISTER_TYPE_SALEKIT))
                        select new { p };
            var data = query.AsEnumerable().Select(e => new ProjectModel
            {
                ID = e.p.ID,
                ProjectCode = e.p.ProjectCode,
                ProjectNameEN = e.p.ProjectNameEN,
                ProjectNameTH = e.p.ProjectNameTH,
                Location = e.p.Location,
                Vocation = e.p.Vocation,
                LineOrder = e.p.LineOrder,
                TransferPaymentExpireDate = e.p.TransferPaymentExpireDate,  
                ProjectResourceList = GetProjectResourceList(e.p.ID)
            }).OrderBy(e => e.LineOrder).ToList();

            return data;
        }
        public List<ProjectResource> GetProjectResourceList(Guid projectId)
        {
            var query = from pr in db.tr_ProjectResource.Where(e => e.ProjectID == projectId && e.FlagActive == true)
                        join r in db.tm_Resource.Where(e => e.IsActive == true)
                            on pr.ResourceID equals r.ID into _r
                        from r2 in _r.DefaultIfEmpty()
                        select new { pr, r2 };
            return query.AsEnumerable().Select(e => new ProjectResource
            {
                ResourceTypeID = e.pr.ResourceTypeID,
                ImageUrl = e.pr.ImageUrl,
                ResourceFilepath = (e.r2 != null) ? e.r2.FilePath : null,
                LineOrder = e.pr.LineOrder
            }).OrderBy(e => e.LineOrder).ToList();
        }
        #endregion

        #region Project Detail
        public ProjectDetailView GetProjectDetailData(Guid projectID)
        {
            var model = new ProjectDetailView();
            var project = GetProjectList(projectID);
            if (project.Any())
            {
                model.Project = project.FirstOrDefault();
                model.Project.ProjectConfig = getProjectConfig(projectID);

                model.BuildList = master.GetProjectBuildList(projectID);
                if (model.BuildList.Any())
                {
                    var buildID = model.BuildList.FirstOrDefault().BuildID;
                    model.FloorList = master.GetProjectBuildFloorList(projectID, buildID.AsInt());

                    if (model.FloorList.Any())
                    {
                        var floorItem = model.FloorList.FirstOrDefault();
                        model.FloorPlanFilePath = floorItem.FloorPlanFilePath;
                        model.UnitList = master.GetUnitList(projectID, buildID, floorItem.FloorID);
                        model.UnitAnnotationList = GetUnitAnnotationList(model.UnitList);
                    }
                }
                model.GalleryList = GetProjectGalleryList(projectID);

                //get for matrix
                var builds = db.tm_Unit.Where(e => e.ProjectID == projectID && e.FlagActive == true)
                            .Select(e => e.tm_Build.Name).OrderBy(e => e).Distinct().ToArray();
                model.Builds = string.Join(",", builds);
            }

            return model;
        }
        public dynamic GetUnitAnnotationList(List<Unit> units)
        {
            return units.Where(e => e.AnnotationID != null).AsEnumerable().Select(e => new
            {
                id = e.ID,
                type = "Annotation",
                unitCode = e.UnitCode,
                unitStatus = e.UnitStatusColor,
                target = new
                {
                    selector = new
                    {
                        type = e.SelectorType,
                        conformsTo = "http://www.w3.org/TR/media-frags/",
                        value = e.SelectorValue
                    }
                }
            }).ToList();
        }
        public ProjectDetailView GetFloorList(Guid projectID, int buildID, int? floorID = null)
        {
            var model = new ProjectDetailView();
            model.FloorList = master.GetProjectBuildFloorList(projectID, buildID, floorID);

            if (model.FloorList.Any())
            {
                var floorItem = model.FloorList.FirstOrDefault();
                model.FloorPlanFilePath = floorItem.FloorPlanFilePath;
                model.UnitList = master.GetUnitList(projectID, buildID, floorItem.FloorID);
                model.UnitAnnotationList = GetUnitAnnotationList(model.UnitList);
            }
            return model;
        }
        private List<ProjectGallery> GetProjectGalleryList(Guid projectID)
        {
            var query = from g in db.tr_ProjectGallery.Where(e => e.ProjectID == projectID && e.FlagActie == true)
                        select new { g };
            return query.AsEnumerable().Select(e => new ProjectGallery
            {
                ID = e.g.ID,
                Name = e.g.Name,
                LineOrder = e.g.LineOrder,
                ResourceList = GetProjectGalleryResourceList(e.g.ID)
            }).OrderBy(e => e.LineOrder).ToList();
        }
        private List<ProjectGalleryResource> GetProjectGalleryResourceList(int projectGlleryID)
        {
            var query = from pg in db.tr_ProjectGalleryResource.Where(e => e.ProjectGalleryID == projectGlleryID && e.FlagActie == true)
                        join r in db.tm_Resource.Where(e => e.IsActive == true)
                            on pg.ResourceID equals r.ID
                        select new { pg, r };
            return query.AsEnumerable().Select(e => new ProjectGalleryResource
            {
                FilePath = e.r.FilePath,
                LineOrder = e.pg.LineOrder
            }).OrderBy(e => e.LineOrder).ToList();
        }
        private ProjectConfig getProjectConfig(Guid projectID)
        {
            var data = db.tm_ProjectConfig.Where(e => e.ProjectID == projectID)
                        .AsEnumerable().Select(e => new ProjectConfig
                        {
                            ProjectID = e.ProjectID.AsGuid(),
                            CountDownDate = e.CountDownDate,
                            CountDownTitle = e.CountDownTitle
                        }).FirstOrDefault() ?? new ProjectConfig();
            return data;

        }
        public DateTime? GetRegisterAllowBookDate(Guid registerID)
        {
            var register = db.tm_Register.FirstOrDefault(e => e.FlagActive == true && e.ID == registerID);
            return register.AllowBookDate;
        }
        public List<Unit> GetUnitAvailable(Guid projectID)
        {
            var query = from u in db.tm_Unit.Where(e => e.ProjectID == projectID && e.FlagActive == true
                                    && e.UnitStatusID == Constant.UnitStatus.AVAILABLE)
                        join pb in db.tr_ProjectBuild.Where(e => e.ProjectID == projectID && e.FlagActive == true)
                            on u.BuildID equals pb.BuildID
                        join pbf in db.tr_ProjectBuildFloor.Where(e => e.ProjectID == projectID
                                    && e.FlagActive == true)
                            on new { u.BuildID, u.FloorID } equals new { pbf.BuildID, pbf.FloorID }
                        join b in db.tm_Build
                            on u.BuildID equals b.ID
                        join f in db.tm_Floor
                            on u.FloorID equals f.ID
                        group u by new
                        {
                            u.BuildID,
                            BuildName = b.Name,
                            u.FloorID,
                            FloorName = f.Name
                        } into g
                        select new
                        {
                            g.Key.BuildID,
                            g.Key.BuildName,
                            g.Key.FloorID,
                            g.Key.FloorName,
                            CntUnitAvailable = g.Count()
                        };
            var data = query.AsEnumerable().Select(e => new Unit
            {
                BuildID = e.BuildID,
                BuildName = e.BuildName,
                FloorID = e.FloorID,
                FloorName = e.FloorName.ToInt().AsInt(),
                CntUnitAvailable = e.CntUnitAvailable
            }).OrderBy(e => e.BuildID).ThenBy(e => e.FloorID).ToList();
            return data;
        }
        #endregion

        #region Unit Detail
        public Unit GetUnitDetail(Guid unitID)
        {
            var model = master.GetUnitDetail(unitID);
            return model;
        }
        #endregion

        public ProjectRegisterQuota GetRegisterQuota(Guid projectID, Guid registerID)
        {
            var quota = new ProjectRegisterQuota();
            var query = db.tr_ProjectRegisterQuota.Where(e => e.ProjectID == projectID
                            && e.RegisterID == registerID && e.FlagActive == true);
            var queryBook = db.ts_Booking.Where(e => e.ProjectID == projectID
                            && e.CustomerID == registerID && e.FlagActive == true
                            && e.BookingStatusID != Constant.BookingStatus.CANCEL
                            && e.BookingStatusID != null && e.BookingStatusID != 0);
            quota.UseQuota = queryBook.Count();
            quota.Quota = query.FirstOrDefault()?.Quota;
            return quota;
        }

        #region Transfer Payment
        public List<ProjectTransPayment> GetTransferPaymentResourceList(Guid projectID, Guid registerID)
        {
            var query = from tr in db.tr_ProjectTransferPayment.Where(e => e.ProjectID == projectID && e.RegisterID == registerID && e.FlagActive == true)
                        join r in db.tm_Resource
                          on tr.ResourceID equals r.ID
                        join m in db.tm_Ext
                          on tr.StatusID equals m.ID
                        select new { tr, r, m };
            var data = query.AsEnumerable().Select(e => new ProjectTransPayment
            {
                ID = e.tr.ID,
                CreateDate = e.tr.CraeteDate,
                Amount = e.tr.Amount.AsDecimal(),
                FileName = e.r.FileName,
                FilePath = e.r.FilePath,
                StatusID = e.tr.StatusID,
                StatusName = e.m.Name,
                UpdateDate = e.tr.UpdateDate,
                TransferDate = e.tr.TransferDate
            }).OrderByDescending(e => e.TransferDate).ToList();
            return data;
        }
        public void SaveTransferPayment(ProjectTransPayment model)
        {
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(0, 10, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                saveTransferPaymentResource(model);
                saveTransferPaymentData(model);
                scope.Complete();
            }
        }
        private void saveTransferPaymentData(ProjectTransPayment model)
        {
            using (var context = new OnlineBookingEntities())
            {
                var item = setTransferPayment(new tr_ProjectTransferPayment(), model);
                context.Entry(item).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }
        }
        private tr_ProjectTransferPayment setTransferPayment(tr_ProjectTransferPayment item, ProjectTransPayment model)
        {
            item.ID = Guid.NewGuid();
            item.ProjectID = model.ProjectID;
            item.RegisterID = UserProfile.ID;
            item.ResourceID = model.ResourceID;
            item.TransferDate = model.TransferDate;
            item.Amount = model.Amount;
            item.StatusID = Constant.Ext.TRANSFER_PAYMENR_STATUS_VERIFY;
            item.FlagActive = true;
            item.CraeteDate = DateTime.Now;
            item.CreateBy = UserProfile.ID;
            item.UpdateDate = DateTime.Now;
            item.UpdateBy = UserProfile.ID;

            model.ID = item.ID;
            return item;
        }
        private void saveTransferPaymentResource(ProjectTransPayment model)
        {
            using (var context = new OnlineBookingEntities())
            {
                for (int iCnt = 0; iCnt <= model.hfc.Count - 1; iCnt++)
                {
                    var file = model.hfc[iCnt];
                    if (model.hfc[iCnt].ContentLength > 0)
                    {
                        model.iFile = iCnt;
                        var item = setResource(new tm_Resource(), model);
                        context.Entry(item).State = System.Data.Entity.EntityState.Added;
                        context.SaveChanges();

                        var filePath = Path.Combine(model.AppPath, model.FilePath);
                        UploadFile(filePath, file);
                    }
                }
            }
        }
        private tm_Resource setResource(tm_Resource item, ProjectTransPayment model)
        {
            var id = Guid.NewGuid();
            var file = model.hfc[model.iFile];
            var dir = $"Uploads/TransferPayment/{DateTime.Now.ToString("yyyyMMdd")}";
            var extension = GetExtension(file.ContentType);
            var fileName = $"{id}.{extension}";
            item.ID = id;
            item.FileName = file.FileName;
            item.FilePath = $"{dir}/{fileName}";
            item.MimeType = file.ContentType;
            item.IsActive = true;
            item.CreateDate = DateTime.Now;
            item.CreateBy = UserProfile.ID;
            item.UpdateDate = DateTime.Now;
            item.UpdateBy = UserProfile.ID;

            model.ResourceID = id;
            model.FilePath = item.FilePath;
            return item;
        }
        private string GetExtension(string contentType)
        {
            //MimeTypesMap.GetMimeType("filename.jpeg"); // => image/jpeg
            return MimeTypesMap.GetExtension(contentType); // => jpeg
        }
        public void UploadFile(string filePath, HttpPostedFile post)
        {
            // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)            
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            post.SaveAs(filePath);
        }
        public void SaveDeletePayment(Guid ID)
        {
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(0, 10, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                SaveDeletePaymentData(ID);
                scope.Complete();
            }
        }
        private void SaveDeletePaymentData(Guid ID)
        {
            using (var context = new OnlineBookingEntities())
            {
                var item = context.tr_ProjectTransferPayment.FirstOrDefault(e => e.ID == ID);
                if (item.StatusID != Constant.Ext.TRANSFER_PAYMENR_STATUS_VERIFY)
                    throw new Exception(Constant.Message.Error.TRANSACTION_IS_VERIFY);
                item.FlagActive = false;
                item.UpdateDate = DateTime.Now;
                item.UpdateBy = UserProfile.ID;
                context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }
        #endregion
    }
}
