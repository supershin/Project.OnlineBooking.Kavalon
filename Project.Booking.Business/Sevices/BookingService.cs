using Project.Booking.Business.Interfaces;
using Project.Booking.Constants;
using Project.Booking.Data;
using Project.Booking.Extensions;
using Project.Booking.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Project.Booking.Business.Sevices
{
    public class BookingService : IBooking
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
        private readonly IProject project;

        private UserProfile UserProfile { get; set; }
        #endregion        
        public BookingService(UserProfile _userProfile)
        {
            UserProfile = _userProfile;
            project = new ProjectService(_userProfile);
        }

        public List<BookingModel> GetBookingHistory(Guid? bookingID = null, OnlineBookingEntities context = null)
        {
            db = (context != null) ? context : db;
            var nowDate = DateTime.Now.Date;
            var query = from bk in db.ts_Booking.Where(e => e.FlagActive == true && e.CustomerID == UserProfile.ID
                                    && (e.ID == bookingID || bookingID == null))
                        join bks in db.tm_BookingStatus
                            on bk.BookingStatusID equals bks.ID
                        join p in db.tm_Project.Where(e => e.FlagActive == true)
                            on bk.ProjectID equals p.ID
                        join u in db.tm_Unit.Where(e => e.FlagActive == true)
                            on bk.UnitID equals u.ID
                        join ut in db.tm_UnitType.Where(e => e.FlagActive == true)
                            on u.UnitTypeID equals ut.ID into _ut
                        from ut2 in _ut.DefaultIfEmpty()
                        join mo in db.tm_ModelType.Where(e => e.FlagActive == true)
                            on u.ModelTypeID equals mo.ID into _mo
                        from mo2 in _mo.DefaultIfEmpty()
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
                        && (e.EndDate >= nowDate || e.EndDate == null))
                            on usp2.SpecialPromotionID equals sp.ID into _sp
                        from sp2 in _sp.DefaultIfEmpty()
                        select new { bk, p, u, bks, ut2, mo2, pm2, r2, sp2 };
            return query.AsEnumerable().Select(e => new BookingModel
            {
                ID = e.bk.ID,
                BookingNumber = e.bk.BookingNumber,
                ProjectID = e.bk.ProjectID,
                ProjectCode = e.p.ProjectCode,
                ProjectName = e.p.ProjectNameTH,
                SpecialPromotion = (e.sp2 != null) ? e.sp2.Name : null,
                UnitID = e.u.ID,
                UnitCode = e.u.UnitCode,
                ModelTypePath = (e.r2 != null) ? e.r2.FilePath : null,
                ModelTypeName = (e.mo2 != null) ? e.mo2.Name : null,
                UnitTypeName = (e.ut2 != null) ? e.ut2.Name : null,
                Area = e.u.Area,
                AreaIncrease = e.u.AreaIncrease,
                SellingPrice = e.bk.SellingPrice,
                SpecialPrice = e.bk.SpecialPrice,
                Discount = e.bk.Discount,
                BookingAmount = e.bk.BookingAmount,
                ContractAmount = e.bk.ContractAmount,
                BookingStatusID = e.bk.BookingStatusID,
                BookingDate = e.bk.BookingDate,
                PaymentDueDate = e.bk.PaymentDueDate,
                BookingStatus = e.bks.Name,
                BookingStatusColor = e.bks.Color,
                CustomerID = e.bk.CustomerID,
                CustomerFirstName = e.bk.CustomerFirstName,
                CustomerLastName = e.bk.CustomerLastName,
                CustomerCitizenID = e.bk.CustomerCitizenID,
                CustomerEmail = e.bk.CustomerEmail,
                CustomerMobile = e.bk.CustomerMobile,
                PaymentCredit = (e.bk.BookingStatusID == Constant.BookingStatus.PAYMENT_SUCCESS) ?
                                GetBookingPayment(e.bk.ID) : null,
                MasterPlanPath = getFloorPlan(e.bk.ProjectID.AsGuid(), Constant.Build.MASTER_PLAN, Constant.Floor.FLOOR_1),
                FloorPlanPath = getFloorPlan(e.bk.ProjectID.AsGuid(), e.u.BuildID.AsInt(), e.u.FloorID.AsInt()),
                UnitPlanPath = (e.r2 != null) ? e.r2.FilePath : null
            }).OrderByDescending(e => e.BookingDate).ToList();
        }
        public Guid SaveBooking(Guid unitID)
        {
            Guid bookingID = Guid.Empty;
            try
            {
                TransactionOptions option = new TransactionOptions();
                option.Timeout = new TimeSpan(1, 0, 0);
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
                {
                    bookingID = SaveBookingData(unitID);
                    UpdateUnitStatus(unitID, Constant.UnitStatus.BOOKING);
                    saveUnitHistory(unitID, Constant.UnitStatus.BOOKING, bookingID, Constant.BookingStatus.WAIT_PAYMENT, UserProfile.ID);
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return bookingID;
        }
        private Guid SaveBookingData(Guid unitID)
        {
            using (var context = new OnlineBookingEntities())
            {
                //check unit avaulable
                var unit = context.tm_Unit.Where(e => e.ID == unitID && e.FlagActive == true).FirstOrDefault();
                if (unit.UnitStatusID != Constant.UnitStatus.AVAILABLE)
                    throw new Exception(Constant.Message.Error.UNIT_NOT_AVAILABLE);

                //check quota                
                var quota = project.GetRegisterQuota(unit.ProjectID.AsGuid(), UserProfile.ID);
                if (quota.UseQuota >= quota.Quota.AsInt() || quota.Quota.AsInt() == 0)
                    throw new Exception(Constant.Message.Error.BOOKING_OVER_QUOTA);


                //get booking number
                var booking = new BookingModel();
                booking.BookingNumber = GenerateBookingNumber(context, unit.ProjectID.AsGuid());

                var item = SetBookingData(booking, unit);
                context.Entry(item).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();

                return item.ID;
            }
        }
        private ts_Booking SetBookingData(BookingModel booking, tm_Unit unit)
        {
            var item = new ts_Booking();
            item.ID = Guid.NewGuid();
            item.ProjectID = unit.ProjectID;
            item.UnitID = unit.ID;
            item.BookingNumber = booking.BookingNumber;
            item.BookingAmount = unit.BookingAmount;
            item.ContractAmount = unit.ContractAmount;
            item.SellingPrice = unit.SellingPrice;
            item.SpecialPrice = unit.SpecialPrice;
            item.Discount = unit.Discount;
            item.CustomerID = UserProfile.ID;
            item.CustomerFirstName = UserProfile.FirstName;
            item.CustomerLastName = UserProfile.LastName;
            item.CustomerEmail = UserProfile.Email;
            item.CustomerMobile = UserProfile.Mobile;
            item.CustomerCitizenID = UserProfile.CitizenID;
            item.BookingDate = DateTime.Now;
            item.BookingStatusID = Constant.BookingStatus.WAIT_PAYMENT;
            item.PaymentDueDate = item.BookingDate.AsDate().AddMinutes(Constant.PAYMENT_DUE_DURATION);
            item.FlagActive = true;
            item.CreateDate = DateTime.Now;
            item.CreateBy = UserProfile.ID;
            item.UpdateDate = DateTime.Now;
            item.UpdateBy = UserProfile.ID;
            return item;
        }
        private string GenerateBookingNumber(OnlineBookingEntities context, Guid projectID)
        {
            var projectCode = context.tm_Project.Where(e => e.FlagActive == true && e.ID == projectID).FirstOrDefault().ProjectCode;

            var dt = DateTime.Now;
            string year = dt.Year.ToString().Right(2);
            string month = dt.Month.ToString("00");
            string bookingNumner = string.Format("{0}-{1}{2}{3}", "BO", projectCode, year, month);

            var query = context.ts_Booking.Where(e => e.BookingNumber.Contains(bookingNumner));
            if (query.Any())
            {
                var maxBookNumber = query.OrderByDescending(e => e.BookingNumber).FirstOrDefault().BookingNumber;
                var newBookNumber = Convert.ToInt32(maxBookNumber.Right(5)) + 1;
                bookingNumner = string.Format("{0}{1}", bookingNumner, newBookNumber.ToString("00000"));
            }
            else bookingNumner = string.Format("{0}{1}", bookingNumner, "00001");
            return bookingNumner;
        }
        public void UpdateUnitStatus(Guid unitID, int statusID)
        {
            using (var context = new OnlineBookingEntities())
            {
                var query = context.tm_Unit.Where(e => e.ID == unitID);
                if (query.Any())
                {
                    var item = query.FirstOrDefault();
                    item.UnitStatusID = statusID;
                    item.UpdateDate = DateTime.Now;
                    item.UpdateBy = UserProfile.ID;
                    context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }
        public Guid SaveCancelBooking(Guid bookingID)
        {
            Guid? unitID = Guid.Empty;
            try
            {
                TransactionOptions option = new TransactionOptions();
                option.Timeout = new TimeSpan(1, 0, 0);
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
                {
                    unitID = SaveCancelBookingData(bookingID);
                    UpdateUnitStatus(unitID.AsGuid(), Constant.UnitStatus.AVAILABLE);
                    saveUnitHistory(unitID.AsGuid(), Constant.UnitStatus.AVAILABLE, bookingID, Constant.BookingStatus.CANCEL, UserProfile.ID);
                    scope.Complete();
                }
                return unitID.AsGuid();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private Guid? SaveCancelBookingData(Guid bookingID)
        {
            using (var context = new OnlineBookingEntities())
            {
                var query = context.ts_Booking.Where(e => e.ID == bookingID);
                if (query.Any())
                {
                    var item = query.FirstOrDefault();
                    if (item.PaymentOverDueDate != null)
                    {
                        throw new Exception(Constant.Message.Error.BOOKING_CANCEL_BY_SERVICE);
                    }
                    if (item.BookingStatusID.AsInt() != Constant.BookingStatus.WAIT_PAYMENT)
                    {
                        throw new Exception(Constant.Message.Error.BOOKING_CANCEL_INVALID);
                    }
                    item.BookingStatusID = Constant.BookingStatus.CANCEL;
                    item.CancelDate = DateTime.Now;
                    item.UpdateDate = DateTime.Now;
                    item.UpdateBy = UserProfile.ID;
                    context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return item.UnitID;
                }
                return null;
            }
        }
        public CheckoutViewModel GetBookingCheckout(Guid bookingID)
        {
            var model = new CheckoutViewModel();
            model.Booking = GetBookingHistory(bookingID).FirstOrDefault();
            model.Company = master.GetCompany(model.Booking.ProjectID.AsGuid());
            return model;
        }
        public void SaveBookingCustomer(BookingModel model)
        {
            try
            {
                TransactionOptions option = new TransactionOptions();
                option.Timeout = new TimeSpan(1, 0, 0);
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
                {
                    SaveBookingCustomerData(model);
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void SaveBookingCustomerData(BookingModel model)
        {
            using (var context = new OnlineBookingEntities())
            {
                var query = context.ts_Booking.Where(e => e.ID == model.ID && e.FlagActive == true);
                if (query.Any())
                {
                    var item = query.FirstOrDefault();

                    if (item.BookingStatusID != Constant.BookingStatus.WAIT_PAYMENT
                        || item.PaymentDueDate <= DateTime.Now)
                    {
                        throw new Exception(Constant.Message.Error.SAVE_BOOKING_CUSTOMER_INVALID);
                    }
                    item = SetBookingCustomer(item, model);
                    context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }

            }
        }
        private ts_Booking SetBookingCustomer(ts_Booking item, BookingModel model)
        {
            item.CustomerFirstName = model.CustomerFirstName.ToStringNullable();
            item.CustomerLastName = model.CustomerLastName.ToStringNullable();
            item.CustomerCitizenID = model.CustomerCitizenID.ToStringNullable();
            item.CustomerEmail = model.CustomerEmail.ToStringNullable();
            item.CustomerMobile = model.CustomerMobile.ToStringNullable();
            item.UpdateDate = DateTime.Now;
            item.UpdateBy = UserProfile.ID;
            return item;
        }
        public ts_Booking UpdateBookingStatus(Guid bookingID, int statusID)
        {
            using (var context = new OnlineBookingEntities())
            {
                var query = context.ts_Booking.Where(e => e.ID == bookingID && e.FlagActive == true);
                if (query.Any())
                {
                    var item = query.FirstOrDefault();
                    item.BookingStatusID = statusID;
                    item.UpdateDate = DateTime.Now;
                    item.UpdateBy = UserProfile.ID;
                    context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return item;
                }
                return null;
            }
        }
        private PaymentCredit GetBookingPayment(Guid bookingID)
        {
            var query = from pm in db.ts_Payment.Where(e => e.FlagActive == true && e.BookingID == bookingID)
                        join pc in db.ts_Payment_Credit.Where(e => e.FlagActive == true && e.Status == Constant.OmiseChargeStatus.SUCCESSFUL)
                            on pm.ID equals pc.PaymentID
                        select new { pm, pc };
            return query.AsEnumerable().Select(e => new PaymentCredit
            {
                PaymentNo = e.pm.PaymentNo,
                PaymentDate = e.pm.PaymentDate.AsDate(),
                Amount = e.pc.Amount,
                CardName = e.pc.CardName,
                CardBank = e.pc.CardBank,
                CardBrand = e.pc.CardBrand,
                CardLastDigit = e.pc.CardLastDigit
            }).FirstOrDefault();

        }
        private string getFloorPlan(Guid projectID, int buildID, int floorID)
        {
            var query = from pb in db.tr_ProjectBuildFloor.Where(e => e.FlagActive == true
                                    && e.ProjectID == projectID && e.BuildID == buildID && e.FloorID == floorID)
                        join r in db.tm_Resource
                            on pb.ResourceID equals r.ID
                        select new { pb, r };
            if (query.Any())
                return query.FirstOrDefault().r.FilePath;
            return string.Empty;
        }
        public void SaveUpdateUnitStatus_VIP(Guid unitID)
        {

            try
            {
                TransactionOptions option = new TransactionOptions();
                option.Timeout = new TimeSpan(1, 0, 0);
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
                {
                    var isVIP = saveUpdateUnitStatus_VIP(unitID);
                    if (isVIP)
                    {
                        saveUnitHistory(unitID, Constant.UnitStatus.BOOKING, null, null, Constant.ADMIN_TITLE_ID);
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private bool saveUpdateUnitStatus_VIP(Guid unitID)
        {
            bool isVIP = false;
            using (var context = new OnlineBookingEntities())
            {
                var query = context.tr_UnitVIP.Where(e => e.UnitID == unitID && e.FlagActive == true);
                if (query.Any())
                {
                    var unit = context.tm_Unit.Where(e => e.ID == unitID && e.UnitStatusID == Constant.UnitStatus.AVAILABLE
                                && e.FlagActive == true).FirstOrDefault();
                    if (unit != null)
                    {
                        unit.UnitStatusID = Constant.UnitStatus.BOOKING;
                        unit.UserUpdateDate = DateTime.Now;
                        unit.UserUpdateByID = Constant.ADMIN_TITLE_ID;
                        context.Entry(unit).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        isVIP = true;
                    }
                }
            }
            return isVIP;
        }
        private void saveUnitHistory(Guid unitID, int unitStatusID, Guid? bookingID, int? bookingStatusID, Guid userID)
        {
            using (var context = new OnlineBookingEntities())
            {
                ts_Unitbooking_History unitHis = new ts_Unitbooking_History();
                unitHis.UnitID = unitID;
                unitHis.UnitStatusID = unitStatusID;
                unitHis.BookingID = bookingID;
                unitHis.BookingStatusID = bookingStatusID;
                unitHis.CreateDate = DateTime.Now;
                unitHis.CreateByID = userID;
                context.Entry(unitHis).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }
        }

        public List<BookingVIP> GetBookingVIPSendMail(int length)
        {
            var BookingIDs = new Guid[] { };
            var query = from v in db.ts_Booking_VIP.Where(e => e.IsSendMail == null || e.IsSendMail == false)
                       join p in db.tm_Project
                        on v.ProjectID equals p.ID
                       select new { v, p };

            var data = query.AsEnumerable().Select(e => new BookingVIP
                       {
                           ID = e.v.ID,
                           ProjectName = e.p.ProjectNameEN,
                           UnitCode =e.v.UnitCode,                           
                           BookingID = e.v.BookingID.AsGuid(),
                           BookingNumber = e.v.BookingNumber,
                           IsSendMail = e.v.IsSendMail.AsBool(),
                           Email = e.v.CustomerEmail
                       }).Take(length).ToList();
            return data;
        }
        public void SaveBookingVIPSendMail(BookingVIP model)
        {

            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                saveBookingVIPSendMailData(model);
                scope.Complete();
            }
        }
        private void saveBookingVIPSendMailData(BookingVIP model)
        {
            var item = db.ts_Booking_VIP.FirstOrDefault(e => e.ID == model.ID);
            item.IsSendMail = model.IsSendMail;
            db.Entry(item).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public List<BookingModel> GetBookingSendMail(Guid bookingID)
        {            
            
            var query = from bk in db.ts_Booking.Where(e=>e.ID == bookingID)
                        join bks in db.tm_BookingStatus
                            on bk.BookingStatusID equals bks.ID
                        join p in db.tm_Project.Where(e => e.FlagActive == true)
                            on bk.ProjectID equals p.ID
                        join u in db.tm_Unit.Where(e => e.FlagActive == true)
                            on bk.UnitID equals u.ID
                        join ut in db.tm_UnitType.Where(e => e.FlagActive == true)
                            on u.UnitTypeID equals ut.ID into _ut
                        from ut2 in _ut.DefaultIfEmpty()
                        join mo in db.tm_ModelType.Where(e => e.FlagActive == true)
                            on u.ModelTypeID equals mo.ID into _mo
                        from mo2 in _mo.DefaultIfEmpty()
                        join pm in db.tr_ProjectModelType.Where(e => e.FlagActive == true)
                            on new { u.ProjectID, u.ModelTypeID } equals new { pm.ProjectID, pm.ModelTypeID } into _pm
                        from pm2 in _pm.DefaultIfEmpty()
                        join r in db.tm_Resource.Where(e => e.IsActive == true)
                            on pm2.ResourceID equals r.ID into _r
                        from r2 in _r.DefaultIfEmpty()
                        join usp in db.tr_UnitSpecialPromotion.Where(e => e.FlagActive == true)
                           on u.ID equals usp.UnitID into _usp
                        from usp2 in _usp.DefaultIfEmpty()                     
                        select new { bk, p, u, bks, ut2, mo2, pm2, r2};
            return query.AsEnumerable().Select(e => new BookingModel
            {
                ID = e.bk.ID,
                BookingNumber = e.bk.BookingNumber,
                ProjectID = e.bk.ProjectID,
                ProjectCode = e.p.ProjectCode,
                ProjectName = e.p.ProjectNameTH,                
                UnitID = e.u.ID,
                UnitCode = e.u.UnitCode,
                ModelTypePath = (e.r2 != null) ? e.r2.FilePath : null,
                ModelTypeName = (e.mo2 != null) ? e.mo2.Name : null,
                UnitTypeName = (e.ut2 != null) ? e.ut2.Name : null,
                Area = e.u.Area,
                AreaIncrease = e.u.AreaIncrease,
                SellingPrice = e.bk.SellingPrice,
                SpecialPrice = e.bk.SpecialPrice,
                Discount = e.bk.Discount,
                BookingAmount = e.bk.BookingAmount,
                ContractAmount = e.bk.ContractAmount,
                BookingStatusID = e.bk.BookingStatusID,
                BookingDate = e.bk.BookingDate,
                PaymentDueDate = e.bk.PaymentDueDate,
                BookingStatus = e.bks.Name,
                BookingStatusColor = e.bks.Color,
                CustomerID = e.bk.CustomerID,
                CustomerFirstName = e.bk.CustomerFirstName,
                CustomerLastName = e.bk.CustomerLastName,
                CustomerCitizenID = e.bk.CustomerCitizenID,
                CustomerEmail = e.bk.CustomerEmail,
                CustomerMobile = e.bk.CustomerMobile,
                PaymentCredit = (e.bk.BookingStatusID == Constant.BookingStatus.PAYMENT_SUCCESS) ?
                                GetBookingPayment(e.bk.ID) : null,
                MasterPlanPath = getFloorPlan(e.bk.ProjectID.AsGuid(), Constant.Build.MASTER_PLAN, Constant.Floor.FLOOR_1),
                FloorPlanPath = getFloorPlan(e.bk.ProjectID.AsGuid(), e.u.BuildID.AsInt(), e.u.FloorID.AsInt()),
                UnitPlanPath = (e.r2 != null) ? e.r2.FilePath : null
            }).OrderByDescending(e => e.BookingDate).ToList();
        }
    }
}
