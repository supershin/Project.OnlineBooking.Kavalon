using Project.Booking.Business.Interfaces;
using Project.Booking.Constants;
using Project.Booking.Data;
using Project.Booking.Extensions;
using Project.Booking.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.UI;

namespace Project.Booking.Business.Sevices
{
    public class PrebookService : IPrebook
    {
        OnlineBookingEntities db = new OnlineBookingEntities();
        private UserProfile UserProfile { get; set; }
        private IPayment _payment;
        private IProject _projectService;
        public PrebookService(UserProfile _userProfile)
        {
            this.UserProfile = _userProfile;
            _payment = new PaymentService(_userProfile);
            _projectService = new ProjectService(_userProfile);
        }

        #region Project Register Quota
        public List<ProjectQuota> GetProjectQuotaList(Guid projectID, int? id = null)
        {
            return db.tr_ProjectQuota.AsEnumerable().Where(e => e.ProjectID == projectID
                    && (e.ID == id || id == null)).Select(e => new ProjectQuota
                    {
                        ID = e.ID,
                        Name = e.Name,
                        Quota = e.Quota.AsInt(),
                        TotalPrice = e.TotalPrice,
                        LineOrder = e.LineOrder.AsInt()
                    }).OrderBy(e => e.LineOrder).ToList();
        }
        public List<ProjectRegisterQuota> GetProjectRegisterQuotaList(Guid projectID, Guid registerID)
        {
            var query = from pr in db.tr_ProjectRegisterQuota.Where(e => e.FlagActive == true
                                    && e.CancelDate == null && e.ProjectID == projectID && e.RegisterID == registerID)
                        join pq in db.tr_ProjectQuota
                            on pr.ProjectQuotaID equals pq.ID
                        join sta in db.tm_Ext
                            on pr.StatusID equals sta.ID into _sta
                        from sta2 in _sta.DefaultIfEmpty()
                        select new
                        {
                            pr,
                            pq,
                            sta2,
                            PaymentAmount = (from pm in db.ts_Payment.Where(p => p.ProjectRegisterQuotaID == pr.ID
                                                         && p.FlagActive == true)
                                             join pmc in db.ts_Payment_Credit.Where(e => e.FlagActive == true)
                                                on pm.ID equals pmc.PaymentID into _pmc
                                             from pmc2 in _pmc.DefaultIfEmpty()
                                             join pmt in db.ts_Payment_Transfer
                                               on pm.ID equals pmt.PaymentID into _pmt
                                             from pmt2 in _pmt.DefaultIfEmpty()
                                             select new { pmc2, pmt2 }).Sum(s => (s.pmc2.Amount != null) ? s.pmc2.Amount : (s.pmt2 != null) ? s.pmt2.Amount : 0)
                        };
            var data = query.AsEnumerable().Select(e => new ProjectRegisterQuota
            {
                ID = e.pr.ID,
                ProjectQuotaName = e.pq.Name,
                StatusName = (e.sta2 != null) ? e.sta2.Name : null,
                TotalPrice = e.pq.TotalPrice,
                Amount = e.PaymentAmount
            }).ToList();
            return data;
        }
        public void SaveProjectRegisterQuota(ProjectRegisterQuota model)
        {

            try
            {
                TransactionOptions option = new TransactionOptions();
                option.Timeout = new TimeSpan(1, 0, 0);
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
                {
                    saveProjectRegisterQuotaData(model);
                    savePaymentData(model);
                    if (model.PaymentTypeID == Constant.Ext.PAYMENT_TYPE_TRANSFER_ID)
                    {
                        saveResourceData(model);
                        savePaymentTransferData(model);
                        _projectService.UploadFile(model.AppPath + model.FilePath, model.hfc[0]);
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void saveProjectRegisterQuotaData(ProjectRegisterQuota model)
        {
            using (var context = new OnlineBookingEntities())
            {
                var projectQuota = context.tr_ProjectQuota.FirstOrDefault(e => e.ID == model.ProjectQuotaID && e.FlagActive == true);
                model.TotalPrice = projectQuota.TotalPrice;
                model.Quota = projectQuota.Quota;

                var item = context.tr_ProjectRegisterQuota.FirstOrDefault(e => e.ID == model.ID);
                if (item == null)
                {
                    item = setProjectRegisterQuota(context, new tr_ProjectRegisterQuota(), model);
                    context.Entry(item).State = System.Data.Entity.EntityState.Added;
                }
                else
                {
                    item = setProjectRegisterQuota(context, item, model);
                    context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                context.SaveChanges();
            }
        }
        private tr_ProjectRegisterQuota setProjectRegisterQuota(OnlineBookingEntities context, tr_ProjectRegisterQuota item, ProjectRegisterQuota model)
        {
            if (item.ID == 0)
            {
                item.ID = (context.tr_ProjectRegisterQuota.Any()) ? context.tr_ProjectRegisterQuota.Max(e => e.ID) + 1 : 1;
                item.ProjectID = model.ProjectID;
                item.RegisterID = UserProfile.ID;
                item.ProjectQuotaID = model.ProjectQuotaID;
                item.Quota = model.Quota;
                item.TotalPrice = model.TotalPrice;
                item.FlagActive = true;
                item.CreateDate = DateTime.Now;
                item.CreateBy = UserProfile.ID;

                model.ID = item.ID;
            }
            item.UpdateDate = DateTime.Now;
            item.UpdateBy = UserProfile.ID;
            return item;
        }
        private void saveResourceData(ProjectRegisterQuota model)
        {
            using (var context = new OnlineBookingEntities())
            {
                var item = setResource(new tm_Resource(), model);
                context.Entry(item).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }
        }
        public ProjectQuota GetProjectQuota(ProjectQuota model)
        {
            var query = from pq in db.tr_ProjectQuota.Where(e => e.ProjectID == model.ProjectID
                                    && e.FlagActive == true && e.ID == model.ID)
                        join p in db.tm_Project
                            on pq.ProjectID equals p.ID
                        join c in db.tm_Company
                            on p.CompanyID equals c.ID
                        select new { pq, c };
            var data = query.AsEnumerable().Select(e => new ProjectQuota
            {
                ID = e.pq.ID,
                Quota = e.pq.Quota.AsInt(),
                TotalPrice = e.pq.TotalPrice,
                Name = e.pq.Name,
                TransferBank = e.c.TransferBank,
                TransferAccountNo = e.c.TransferAccountNo
            }).FirstOrDefault();
            if (data == null)
                throw new Exception(Constant.Message.Error.PROJECT_QUOTA_NOT_FOUND);
            return data;
        }
        #endregion

        #region Payment        
        public void savePaymentData(ProjectRegisterQuota model)
        {
            using (var context = new OnlineBookingEntities())
            {
                var item = setPayment(context, new ts_Payment(), model);
                context.Entry(item).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }
        }
        private ts_Payment setPayment(OnlineBookingEntities context, ts_Payment item, ProjectRegisterQuota model)
        {
            item.ID = Guid.NewGuid();
            item.BookingID = null;
            item.ProjectRegisterQuotaID = model.ID;
            item.PaymentTypeID = model.PaymentTypeID;
            item.PaymentNo = _payment.GeneratePaymentNumber(context, model.ProjectID.AsGuid());
            item.PaymentDate = DateTime.Now;
            item.FlagActive = true;
            item.CreateDate = DateTime.Now;
            item.CreateBy = UserProfile.ID;
            item.UpdateDate = DateTime.Now;
            item.UpdateBy = UserProfile.ID;

            model.PaymentID = item.ID;
            return item;
        }
        private tm_Resource setResource(tm_Resource item, ProjectRegisterQuota model)
        {
            var id = Guid.NewGuid();
            var file = model.hfc[0];
            var dir = $"Uploads/PaymentTransferResource/{DateTime.Now.ToString("yyyyMMdd")}";
            var extension = FormatExtension.GetExtension(file.ContentType);
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
        #endregion

        #region Transfer
        private void savePaymentTransferData(ProjectRegisterQuota model)
        {
            using (var context = new OnlineBookingEntities())
            {
                var item = setPaymentTransfer(new ts_Payment_Transfer(), model);
                context.Entry(item).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }
        }
        private ts_Payment_Transfer setPaymentTransfer(ts_Payment_Transfer item, ProjectRegisterQuota model)
        {
            item.ID = Guid.NewGuid();
            item.PaymentID = model.PaymentID;
            item.ResourceID = model.ResourceID;
            item.TransferDate = model.TransferDate;
            item.Hours = model.Hours;
            item.Minutes = model.Minutes;
            item.Amount = model.Amount;
            item.CreateDate = DateTime.Now;
            item.CreateBy = UserProfile.ID;
            return item;
        }
        #endregion
    }
}
