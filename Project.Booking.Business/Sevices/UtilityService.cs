using Project.Booking.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Booking.Data;
using Project.Booking.Model;
using System.Transactions;
using Project.Booking.Constants;
using Project.Booking.Extensions;
using Project.Booking.Security;
using System.Web.ModelBinding;

namespace Project.Booking.Business.Sevices
{
    public class UtilityService : IUtility
    {
        OnlineBookingEntities db = new OnlineBookingEntities();

        private UserProfile UserProfile;
        public UtilityService(UserProfile _userProfile)
        {
            UserProfile = _userProfile;
        }

        #region Register
        public void SaveRegister(UserProfile model)
        {
            try
            {
                TransactionOptions option = new TransactionOptions();
                option.Timeout = new TimeSpan(0, 10, 0);
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
                {
                    SaveRegisterData(model);
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void SaveRegisterData(UserProfile model)
        {
            using (var context = new OnlineBookingEntities())
            {
                model.Email = model.Email.ToStringNullable();
                var queryEmail = context.tm_Register.Where(e => e.Email == model.Email && e.ID != model.ID);
                if (queryEmail.Any())
                {
                    throw new Exception(Constant.Message.Error.REGISTER_EMAIL_ALREADY);
                }

                var query = context.tm_Register.Where(e => e.ID == model.ID);
                if (!query.Any())
                {
                    var item = SetRegisterData(new tm_Register(), model);
                    context.Entry(item).State = System.Data.Entity.EntityState.Added;
                }
                else
                {
                    var item = SetRegisterData(query.FirstOrDefault(), model);
                    context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                context.SaveChanges();
            }
        }
        private tm_Register SetRegisterData(tm_Register item, UserProfile model)
        {
            if (item.ID == Guid.Empty)
            {
                item.ID = Guid.NewGuid();
                model.ID = item.ID;
                UserProfile.ID = item.ID;
                item.ActivateDate = null;
                item.FlagActive = true;
                item.CreateDate = DateTime.Now;
                item.CreateBy = UserProfile.ID;
                item.RegisterTypeID = Constant.Ext.REGISTER_TYPE_CUSTOMER;
            }
            item.FirstName = model.FirstName.ToStringNullable();
            item.LastName = model.LastName.ToStringNullable();
            item.Email = model.Email.ToStringNullable();
            item.Mobile = model.Mobile.ToStringNullable();
            item.CitizenID = model.CitizenID.ToStringNullable();
            if (!string.IsNullOrEmpty(model.EncryptPassword))
            {
                item.Password = model.EncryptPassword;
            }
            
            item.UpdateDate = DateTime.Now;
            item.UpdateBy = UserProfile.ID;
            model.ActivateDate = item.ActivateDate;
            return item;
        }
        public UserProfile GetRegister(string email)
        {
            var query = db.tm_Register.Where(e => e.Email == email && e.FlagActive == true);
            if (query.Any())
            {
                return query.AsEnumerable().Select(e => new UserProfile
                {
                    ID = e.ID,
                    Code = e.Code,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Password = e.Password,
                    Mobile = e.Mobile,
                    Email = e.Email,
                    CitizenID = e.CitizenID,
                    //DescryptPassword = e.Password,
                    DescryptPassword = AES.Decrypt(e.Password),
                    RegisterTypeID = e.RegisterTypeID,
                    ActivateDate = e.ActivateDate
                }).FirstOrDefault();
            }
            else
            {
                throw new Exception(Constant.Message.Error.REGISTER_EMAIL_NOT_FOUND);
            }
        }

        public string SaveActivate(Guid registerID)
        {
            var email = string.Empty;
            try
            {
                TransactionOptions option = new TransactionOptions();
                option.Timeout = new TimeSpan(0, 10, 0);
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
                {
                   email =  saveActivateData(registerID);
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return email;
        }
        private string saveActivateData(Guid registerID) {
            var email = string.Empty;
            using (var context = new OnlineBookingEntities())
            {
                var item = context.tm_Register.FirstOrDefault(e => e.ID == registerID && e.FlagActive == true);
                if (item != null)
                {
                    item.ActivateDate = DateTime.Now;
                    context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    email = item.Email;
                }
                else
                    throw new Exception(Constant.Message.Error.REGISTER_NOT_FOUND);                
            }
            return email;
        }
        #endregion

        #region Authentication
        public UserProfile Authentcation(UserProfile model)
        {

            model.EncryptPassword = AES.Encrypt(model.Password);
            var query = db.tm_Register.Where(e => e.Email == model.Email && e.Password.Equals(model.EncryptPassword)
                        && e.FlagActive == true);
            if (query.Any())
            {
                var email = query.FirstOrDefault().Email;
                var user = GetRegister(email);
                if (user == null)
                {
                    throw new Exception(Constant.Message.Error.REGISTER_EMAIL_NOT_FOUND);
                }
                return user;
            }
            else
            {
                throw new Exception(Constant.Message.Error.LOGIN_FAIL);
            }
        }
        #endregion

        #region Update SignIn & SignOut
        public void SaveSignInOut(UserProfile model) {
            try
            {
                TransactionOptions option = new TransactionOptions();
                option.Timeout = new TimeSpan(0, 10, 0);
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
                {
                    SaveSignInOutData(model);
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void SaveSignInOutData(UserProfile model) {
            using (var context = new OnlineBookingEntities())
            {
                var query = context.tm_Register.Where(e => e.ID == model.ID);
                if (query.Any())
                {
                    var item = query.FirstOrDefault();
                    if (model.IsSignIn) item.LastSignInDate = DateTime.Now;
                    if (model.IsSignOut) item.LastSignOutDate = DateTime.Now;
                    context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }
        #endregion
    }
}
