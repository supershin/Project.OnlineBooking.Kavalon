using Project.Booking.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Booking.Extensions;
using Project.Booking.Constants;
using Project.Booking.Security;
using Project.Booking.Business.Sevices;

namespace Project.Booking.Web.Controllers
{
    public class RegisterController : BaseController
    {
        // GET: Utility
        public ActionResult Index(string returnUrl = null)
        {
            //var model = master.GetWebImageList();
            if (UserProfile != null)
            {
                return RedirectToAction("Detail", "Project", new { projectID = Constant.REDIRECT_PROJECT_ID });
                //return RedirectToAction("Index", "Project");
            }
            ViewBag.returnUrl = returnUrl;
            return View();
        }
        [Route("Register/Activate/{ID}")]
        public ActionResult Activate(Guid ID)
        {
            string email = utility.SaveActivate(ID);
            UserProfile = utility.GetRegister(email);
            UserProfile.IsSignIn = true;
            UserProfile.IsSignOut = false;
            utility.SaveSignInOut(UserProfile);
            return RedirectToAction("Detail", "Project", new { projectID = Constant.REDIRECT_PROJECT_ID });
        }
        #region Register
        [HttpPost]
        public JsonResult SaveRegister(UserProfile model)
        {
            try
            {
                ValidateRegister(model);
                model.EncryptPassword = AES.Encrypt(model.Password);
                //model.EncryptPassword = FormatExtension.RandomString(5);
                model.Password = model.EncryptPassword;
                utility.SaveRegister(model);
                //verifyAuthen(model);
                if (model.ActivateDate == null)
                    SendMailActivate(model);
                //SendMailRegister(model);
                return Json(new
                {
                    activateDate = model.ActivateDate,
                    message = Constant.Message.Success.REGISTER_SUCCESS,
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
        private void ValidateRegister(UserProfile model)
        {
            if (string.IsNullOrEmpty(model.FirstName.ToStringNullable())
                || string.IsNullOrEmpty(model.LastName.ToStringNullable())
                || string.IsNullOrEmpty(model.Email.ToStringNullable())
                || string.IsNullOrEmpty(model.Mobile.ToStringNullable())
                //|| string.IsNullOrEmpty(model.CitizenID.ToStringNullable())
                || string.IsNullOrEmpty(model.Password)
                || string.IsNullOrEmpty(model.ConfirmPassword)
                )
                throw new Exception(Constant.Message.Error.REGISTER_PLEASE_FILL_OUT);
            else if (!IsValidEmail(model.Email.ToStringNullable()))
                throw new Exception(Constant.Message.Error.EMAIL_FORMAT_INVALID);
            //else if (!VerifyCitizenID.IsValidCheckPersonID(model.CitizenID.ToStringNullable()))
            //{
            //    throw new Exception(Constant.Message.Error.CITIZEN_FORMAT_INVALID);
            //}
            else if (PasswordAdvisor.CheckStrength(model.Password) < PasswordScore.Medium)
            {
                throw new Exception(Constant.Message.Error.REGISTER_PASSWORS_ADVISOR);
            }
            else if (!model.Password.Equals(model.ConfirmPassword))
                throw new Exception(Constant.Message.Error.REGISTER_CONFIRM_PASSWORD_INVALID);
            else if (!model.Accept)
                throw new Exception(Constant.Message.Error.REGISTER_PLEASE_ACCEPT);

        }
        private void SendMailRegister(UserProfile model)
        {
            model.baseUrl = baseUrl;
            string template = RenderPartialViewToString("~/Views/Templates/Register_Email.cshtml", model);
            var email = new Email();
            email.To = new List<string> { model.Email };
            email.Subject = Constant.Email.SUBJECT.REGISTER;
            email.Body = template;

            (new MailService()).SendMail(email);

        }
        private void SendMailActivate(UserProfile model)
        {
            model.baseUrl = baseUrl;
            string template = RenderPartialViewToString("~/Views/Templates/Activate_Email.cshtml", model);
            var email = new Email();
            email.To = new List<string> { model.Email };
            email.Subject = Constant.Email.SUBJECT.ACTIVATE;
            email.Body = template;

            (new MailService()).SendMail(email);

        }
        #endregion

        #region Authentication
        [HttpPost]
        public JsonResult Authentication(UserProfile model)
        {
            try
            {
                ValidateAuthentication(model);
                verifyAuthen(model);

                if (model.ActivateDate == null)
                {
                    SendMailActivate(model);
                    UserProfile = null;
                }
                else
                    model.returnUrl = getRedirectURL(model.returnUrl);
                return Json(new
                {
                    message = Constant.Message.Success.LOGIN_SUCCESS,
                    success = true,
                    activateDate = model.ActivateDate,
                    model.returnUrl
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = InnerException(ex),
                    model.returnUrl
                });
            }
        }
        private string getRedirectURL(string redirectUrl)
        {
            if (string.IsNullOrEmpty(redirectUrl.ToStringNullable()))
                if (UserProfile.RegisterTypeID == Constant.Ext.REGISTER_TYPE_CUSTOMER)
                    //redirect to kave uni.verse
                    return baseUrl + "project/detail/7ee1b4d9-50dc-4cad-91e6-bf8b25e9cb9a";
            return redirectUrl;
        }
        private void ValidateAuthentication(UserProfile model)
        {
            if (string.IsNullOrEmpty(model.Email.ToStringNullable())
                || string.IsNullOrEmpty(model.Password))
                throw new Exception(Constant.Message.Error.LOGIN_PLEASE_FILL_OUT);
        }
        private void verifyAuthen(UserProfile model)
        {
            UserProfile = utility.Authentcation(model);
            UserProfile.IsSignIn = true;
            UserProfile.IsSignOut = false;
            model.ActivateDate = UserProfile.ActivateDate;
            model.ID = UserProfile.ID;
            if (model.ActivateDate != null)
                utility.SaveSignInOut(UserProfile);
        }
        public ActionResult LogOut()
        {
            if (UserProfile != null)
            {
                UserProfile.IsSignIn = false;
                UserProfile.IsSignOut = true;
                utility.SaveSignInOut(UserProfile);
            }
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Register");
        }
        #endregion

        #region Forgot
        [HttpPost]
        public JsonResult ForgotPassword(UserProfile model)
        {
            try
            {
                ValidateForgotPassword(model);
                var register = utility.GetRegister(model.Email);
                SendMailForgot(register);

                return Json(new
                {
                    success = true,
                    message = Constant.Message.Success.REGISTER_SUCCESS
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
        private void ValidateForgotPassword(UserProfile model)
        {
            if (string.IsNullOrEmpty(model.Email.ToStringNullable()))
                throw new Exception(Constant.Message.Error.REGISTER_EMAIL_INVALID);
            else if (!IsValidEmail(model.Email))
                throw new Exception(Constant.Message.Error.EMAIL_FORMAT_INVALID);
        }
        private void SendMailForgot(UserProfile model)
        {
            model.baseUrl = baseUrl;
            string template = RenderPartialViewToString("~/Views/Templates/Forgot_Email.cshtml", model);
            var email = new Email();
            email.To = new List<string> { model.Email };
            email.Subject = Constant.Email.SUBJECT.FORGOT_PASSWORD;
            email.Body = template;

            (new MailService()).SendMail(email);

        }
        #endregion

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