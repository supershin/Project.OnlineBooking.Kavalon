using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Constants
{
    public static class Constant
    {
        public static readonly int PAYMENT_DUE_DURATION = Convert.ToInt32(ConfigurationManager.AppSettings["payment.due.duration"]);
        public static readonly int BOOKING_QUOTA = Convert.ToInt32(ConfigurationManager.AppSettings["booking.quota"]);
        public static readonly int UNIT_RANDOM_VIEW = Convert.ToInt32(ConfigurationManager.AppSettings["unit.random.view"]);
        public static readonly string ONLINE_BOOKING_CONN = ConfigurationManager.AppSettings["onnlingbooking.conn"].ToString();
        public static readonly Guid ADMIN_TITLE_ID = new Guid(ConfigurationManager.AppSettings["admin.title.id"].ToString());
        public static readonly Guid REDIRECT_PROJECT_ID = new Guid(ConfigurationManager.AppSettings["redirect.project.id"].ToString());

        public static class Ext
        {
            public static readonly int RESOURCE_TYPE_PROJECT_CARD_ID = 1;
            public static readonly int RESOURCE_TYPE_PROJECT_SECTION_MAIN_PAGE_ID = 2;
            public static readonly int PAYMENT_TYPE_CREDIT_ID = 4;
            public static readonly int REGISTER_TYPE_SALEKIT = 6;
            public static readonly int REGISTER_TYPE_CUSTOMER = 7;
            public static readonly int TRANSFER_PAYMENR_STATUS_VERIFY = 8;
            public static readonly int TRANSFER_PAYMENR_STATUS_APPROVE = 9;
            public static readonly int TRANSFER_PAYMENR_STATUS_FAIL = 10;
        }

        public static class Email
        {
            public static readonly string FROM = ConfigurationSettings.AppSettings["Email.From"].ToString();
            public static readonly string SENDER = ConfigurationSettings.AppSettings["Email.Sender"].ToString();
            public static readonly string HOST = ConfigurationSettings.AppSettings["Email.Host"].ToString();
            public static readonly string USER_NAME = ConfigurationSettings.AppSettings["Email.UserName"].ToString();
            public static readonly string PASSWORD = ConfigurationSettings.AppSettings["Email.Password"].ToString();
            public static readonly int PORT = Convert.ToInt32(ConfigurationSettings.AppSettings["Email.Port"]);

            public static class SUBJECT
            {
                public static readonly string ACTIVATE = ConfigurationSettings.AppSettings["Email.Subject.Activate"].ToString();
                public static readonly string REGISTER = ConfigurationSettings.AppSettings["Email.Subject.Register"].ToString();
                public static readonly string FORGOT_PASSWORD = ConfigurationSettings.AppSettings["Email.Subject.ForgotPassword"].ToString();
                public static readonly string PAYMENT = ConfigurationSettings.AppSettings["Email.Subject.Payment"].ToString();
                public static readonly string BOOKING = ConfigurationSettings.AppSettings["Email.Subject.Booking"].ToString();
            }
        }

        public class Message
        {
            public class Success
            {
                public static readonly string REGISTER_SUCCESS = "ลงทะเบียนสำเร็จ";
                public static readonly string LOGIN_SUCCESS = "ยินดีต้อนรับเข้าสู่ระบบ";
                public static readonly string SAVE_SUCCESS = "Save successful.";
                public static readonly string SAVE_BOOKING_SUCCESS = "Save booking successful.";
                public static readonly string SAVE_USER_SUCCESS = "บันทึกข้อมูลเรียบร้อย ระบบจะให้ทำการ Login ใหม่อีกครั้ง";
            }
            public class Error
            {
                public static readonly string SESSION_TIME_OUT = "Session timeout please sign in again";
                public static readonly string LOGIN_FAIL = "อีเมล หรือรหัสผ่านไม่ถูกต้อง";
                public static readonly string REGISTER_EMAIL_NOT_FOUND = "ไม่พบอีเมลในระบบ";
                public static readonly string REGISTER_ERROR = "ลงทะเบียนไม่สำเร็จ";
                public static readonly string REGISTER_PLEASE_FILL_OUT = "โปรดระบุข้อมูลให้ครบถ้วน";
                public static readonly string REGISTER_CONFIRM_PASSWORD_INVALID = "ยืนยันรหัสผ่านไม่ถูกต้อง";
                public static readonly string REGISTER_PLEASE_ACCEPT = "โปรดยอมรับเงื่อนไข และนโยบายคามเป็นส่วนตัว";
                public static readonly string REGISTER_PASSWORS_ADVISOR = "รหัสผ่านต้องมีอย่างน้อย 8 ตัวอักษร ประกอบด้วยตัวอักษรพิมพ์ใหญ่ (A-Z) ตัวอักษรพิมพ์เล็ก (a-z) และตัวเลข (0-9) เท่านั้น";
                public static readonly string REGISTER_EMAIL_ALREADY = "อีเมลนี้มีในระบบแล้ว";
                public static readonly string REGISTER_EMAIL_INVALID = "อีเมลไม่ถูกต้อง";
                public static readonly string EMAIL_FORMAT_INVALID = "อีเมลไม่ถูกต้อง";
                public static readonly string CITIZEN_FORMAT_INVALID = "บัตรประชาชน ไม่ถูกต้อง";
                public static readonly string LOGIN_PLEASE_FILL_OUT = "อีเมล หรือรหัสผ่านไม่ถูกต้อง";
                public static readonly string UNIT_NOT_AVAILABLE = "Unit not available";
                public static readonly string BOOKING_DOES_NOT_EXISTS = "ไม่พบรายการจองนี้";
                //public static readonly string BOOKING_OVER_QUOTA = "บันทึกไม่สำเร็จ สามารถจองได้พร้อมกันสูงสุด {0} ยูนิต";
                public static readonly string BOOKING_OVER_QUOTA = "You do not have quota";
                public static readonly string BOOKING_PAYMENT_INVALID = "ไม่สามารถชำระเงินได้ เนื่องจากการจองไม่อยู่ในสถานะรอการชำระเงิน หรือเกินกำหนดการชำระเงิน";
                public static readonly string SAVE_BOOKING_CUSTOMER_INVALID = "ไม่สามารถบันทึกข้อมูลลูกค้าได้ เนื่องจากการจองไม่อยู่ในสถานะรอการชำระเงิน หรือเกินกำหนดการชำระเงิน";
                public static readonly string BOOKING_CUSTOMER_INVALID = "โปรดระบุข้อมูลลูกค้าที่ต้องการแก้ไขให้ครบถ้วน";
                public static readonly string OMISE_KEY_NOT_FOUND = "ไม่พบ Omise key ในระบบ";
                public static readonly string PAYMETN_CREDIT_INVALID = "การทำรายการผิดพลาด! โปรดตรวจสอบข้อมูลการชำระเงิน";
                public static readonly string BOOKING_CANCEL_BY_SERVICE = "ระบบได้ทำการยกเลิกรายการแล้ว เนื่องจากเกินกำหนดชำระเงิน";
                public static readonly string PROJECT_DOES_NOT_EXISTS = "ไม่พบโครงการนี้";
                public static readonly string BOOKING_CANCEL_INVALID = "ไม่สามารถยกเลิกได้ เนื่องจากสถานะไม่เท่ากับรอชำระเงิน หรือมีการยกเลิกไปแล้ว";
                public static readonly string PLEASE_INPUT_TRASFER_PAYMENT_DATE = "Please input transfer date.";
                public static readonly string PLEASE_INPUT_TRASFER_PAYMENT_AMOUNT = "Please input transfer amount.";
                public static readonly string PLEASE_ATTACH_TRASFER_PAYMENT = "Please attach transfer payment.";
                public static readonly string TRANSACTION_IS_VERIFY = "Can not delete. Transaction is verify success.";
                public static readonly string REGISTER_NOT_FOUND = "ไม่พบข้อมูลการลงทะเบียน";                
            }
        }

        public static class UnitStatus
        {
            public static readonly int CLOSE = 0;
            public static readonly int AVAILABLE = 1;
            public static readonly int BOOKING = 2;
            public static readonly int PAYMENT = 3;
            public static readonly int CONTRACT = 4;
        }
        public static class BookingStatus
        {
            public static readonly int CANCEL = 0;
            public static readonly int WAIT_PAYMENT = 1;
            public static readonly int PAYMENT_SUCCESS = 2;            
        }        
        public static class OmiseChargeStatus
        {
            public static readonly string FAILED = "Failed";
            public static readonly string PENDDING = "Pending";
            public static readonly string SUCCESSFUL = "Successful";
        }
        public static class HangFire
        {
            public static readonly string CONN = ConfigurationManager.AppSettings["hangfire.conn"].ToString();
        }
        public static class Build
        {
            public static readonly int MASTER_PLAN = 6;
        }
        public static class Floor
        {
            public static readonly int FLOOR_1 = 1;
        }
    }
}
