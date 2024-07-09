using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project.Booking.Extensions
{
    public enum PasswordScore
    {
        Blank = 0,
        VeryWeak = 1,
        Weak = 2,
        Medium = 3,
        Strong = 4,
        VeryStrong = 5
    }

    public static class PasswordAdvisor
    {
        public static PasswordScore CheckStrength(string password)
        {
            int score = 0;

            if (password.Length < 1)
                return PasswordScore.Blank;
            if (password.Length < 4)
                return PasswordScore.VeryWeak;

            //if (password.Length >= 8)
            //    score++;  
            if (Regex.Match(password, @"^.*(?=.{8,})", RegexOptions.ECMAScript).Success)
                score++;
            if (Regex.Match(password, @"(?=.*\d)", RegexOptions.ECMAScript).Success)
                score++;
            if (Regex.Match(password, @"(?=.*[a-z])", RegexOptions.ECMAScript).Success &&
              Regex.Match(password, @"(?=.*[A-Z])", RegexOptions.ECMAScript).Success)
                score++;
            if ((PasswordScore)score < PasswordScore.Medium)
                return (PasswordScore)score;
            if (password.Length >= 12)
                score++;
            if (Regex.Match(password, @"(?=.*[!*@#$%^&+=]).*$", RegexOptions.ECMAScript).Success)
                score++;            
            return (PasswordScore)score;
        }
    }
}
