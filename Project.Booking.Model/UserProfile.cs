using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Booking.Constants;
using Project.Booking.Data;
using Project.Booking.Extensions;
namespace Project.Booking.Model
{
    public class UserProfile : tm_Register
    {        
        public string baseUrl { get; set; }
        public string returnUrl { get; set; }        
        public string EncryptPassword { get; set; }
        public string DescryptPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public bool Accept { get; set; }

        //public bool IsValidCheckPersonID()
        //{
        //    string pwd = this.CitizenID;
        //    bool isValid = false;
        //    //args.IsValid = (args.Value.Length >= 8);
        //    int pin = 0;
        //    int j = 13;
        //    int pin_num = 0;
        //    if (pwd.ToString() == "")
        //    {
        //        isValid = false;
        //        return isValid;
        //    }
        //    bool ChkPinID = true;
        //    if (ChkPinID == false) { isValid = false; return isValid; }
        //    if (pwd.ToString().Length == 13)
        //    {
        //        for (int i = 0; i < pwd.Length; i++)
        //        {
        //            if (i != 12)
        //            {
        //                pin = Convert.ToInt16(pwd.ToString().Substring(i, 1)) * j + pin;
        //            }
        //            j--;
        //        }
        //        pin_num = (11 - (pin % 11)) % 10;
        //        if (Convert.ToInt16(pwd.ToString().Substring(12, 1)) != pin_num)
        //        {
        //            isValid = false;
        //            return isValid;
        //        }
        //    }
        //    else
        //    {
        //        isValid = false;
        //        return isValid;
        //    }
        //    isValid = true;
        //    return isValid;
        //}

        public bool IsValidCheckPersonID()
        {
            try
            {
                string pid = this.CitizenID.ToStringNullable();
                char[] numberChars = pid.ToCharArray();

                int total = 0;
                int mul = 13;
                int mod = 0, mod2 = 0;
                int nsub = 0;
                int numberChars12 = 0;

                for (int i = 0; i < numberChars.Length - 1; i++)
                {
                    int num = 0;
                    int.TryParse(numberChars[i].ToString(), out num);

                    total = total + num * mul;
                    mul = mul - 1;

                    //Debug.Log(total + " - " + num + " - "+mul);
                }

                mod = total % 11;
                nsub = 11 - mod;
                mod2 = nsub % 10;

                //Debug.Log(mod);
                //Debug.Log(nsub);
                //Debug.Log(mod2);


                int.TryParse(numberChars[12].ToString(), out numberChars12);

                //Debug.Log(numberChars12);

                if (mod2 != numberChars12)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw new Exception(Constant.Message.Error.CITIZEN_FORMAT_INVALID);
            }            
        }

        public bool IsSignIn { get; set; }
        public bool IsSignOut { get; set; }
    }
}
