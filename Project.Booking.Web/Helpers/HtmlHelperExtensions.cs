using Project.Booking.Model;
using Project.Booking.Sessions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Project.Booking.Web
{
    public static class HtmlHelperExtensions
    {
        public static UserProfile UserProfile(this HtmlHelper helper)
        {

            var userProfile = (UserProfile)HttpContext.Current.Session[SessionKey.Autentication.UserProfile];
            if (userProfile == null) userProfile = new UserProfile();
            return userProfile;

        }
        public static string CurrentVersion(this HtmlHelper helper)
        {
            try
            {
                var version = Assembly.GetExecutingAssembly().GetName();
                //return string.Format("{0} Version{1}", version.Name, version.Version);
                return string.Format("{0}", version.Version);
            }
            catch
            {
                return "?.?.?.?";
            }
        }
        public static string CurrentCompileDate(this HtmlHelper helper)
        {
            try
            {
                var version = Assembly.GetExecutingAssembly();
                return System.IO.File.GetLastWriteTime(version.Location).ToString("yyyy/MM/dd HH:mm", new CultureInfo("en-US"));
            }
            catch
            {
                return "?.?.?.?";
            }
        }
    }
}