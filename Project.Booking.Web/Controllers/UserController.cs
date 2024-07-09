using Project.Booking.Constants;
using Project.Booking.Extensions;
using Project.Booking.Model;
using Project.Booking.Security;
using Project.Booking.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Booking.Web.Controllers
{
    [CustomAuthenticationFilter]
    public class UserController : BaseController
    {
        // GET: User
        [Route("user/profile")]
        public ActionResult index()
        {
            var model = utility.GetRegister(UserProfile.Email);

            return View(model);
        }

        [HttpPost]
        public JsonResult SaveUserProfile(UserProfile model)
        {
            try
            {
                ValidateUserProfile(model);

                if (!string.IsNullOrEmpty(model.Password))
                {
                    model.EncryptPassword = AES.Encrypt(model.Password);
                }       
                
                utility.SaveRegister(model);

                return Json(new
                {
                    message = Constant.Message.Success.SAVE_USER_SUCCESS,
                    success = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = InnerException(ex)
                });
            }
        }
        private void ValidateUserProfile(UserProfile model)
        {
            if (string.IsNullOrEmpty(model.FirstName.ToStringNullable())
                || string.IsNullOrEmpty(model.LastName.ToStringNullable())
                || string.IsNullOrEmpty(model.Email.ToStringNullable())
                || string.IsNullOrEmpty(model.Mobile.ToStringNullable())
                //|| string.IsNullOrEmpty(model.CitizenID.ToStringNullable())
                )
                throw new Exception(Constant.Message.Error.REGISTER_PLEASE_FILL_OUT);
            else if (!IsValidEmail(model.Email.ToStringNullable()))
                throw new Exception(Constant.Message.Error.EMAIL_FORMAT_INVALID);
            //else if (!VerifyCitizenID.IsValidCheckPersonID(model.CitizenID.ToStringNullable()))
            //{
            //    throw new Exception(Constant.Message.Error.CITIZEN_FORMAT_INVALID);
            //}

            //validate change password
            if (!string.IsNullOrEmpty(model.Password)
                || !string.IsNullOrEmpty(model.ConfirmPassword))
            {
                if (PasswordAdvisor.CheckStrength(model.Password) < PasswordScore.Medium)
                {
                    throw new Exception(Constant.Message.Error.REGISTER_PASSWORS_ADVISOR);
                }
                else if(!model.Password.Equals(model.ConfirmPassword))
                {
                    throw new Exception(Constant.Message.Error.REGISTER_CONFIRM_PASSWORD_INVALID);
                }
            }
        }

        #region Private Function
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        #endregion

    }
}