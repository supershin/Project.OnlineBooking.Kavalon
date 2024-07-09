using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Services
{
    public static class Constant
    {
        public static readonly string ONLINE_BOOKING_CANCEL_ORDER_INTERVAL = ConfigurationManager.AppSettings["ServiceInterval"];

        public static class Service {
            public static readonly string UPDATE_OVER_DUE_BOOKING = "UpdateOverdueBooking";
            public static readonly string UPDATE_KPAYMENT_SETTLED = "UpdateKPaymentSettled";                        
        }
       
        public static class WisePay {
            public static class OrderStatus
            {
                public static readonly int WAIT_PAYMENT = 1;
                public static readonly int PAYMENT_SUCCESS = 2;
                public static readonly int CANCEL = 3;
                public static readonly int VOID = 4;
            }
            public static class PaymentType
            {
                public static readonly int CARD = 10;
                public static readonly string CARD_NAME = "Card";
                public static readonly int OTHER = 11;
            }
            public static class PaymentMethod
            {
                public static readonly int QR_CODE = 12;
                public static readonly int UNIONPAY = 18;
            }
            public static class BANK
            {
                public static readonly int KABNK_ID = 1;
            }
            public static class KBANK
            {
                public static class TransactionState
                {
                    public static readonly string PRE_AUTHORIZE = "Pre-Authorized";
                    public static readonly string AUTHORIZE = "Authorized";
                    public static readonly string DECLINED = "Declined";
                    public static readonly string VOIDED = "Voided";
                    public static readonly string VOID_QR = "VOID";
                    public static readonly string INITIALIZE = "Initialize";
                    public static readonly string SETTLED = "Settled";
                    public static readonly string CAPTURED = "Captured";
                }
                public static class Status
                {
                    public static readonly string SUCCESS = "success";
                    public static readonly string FAIL = "fail";
                    public static readonly string PENDING = "pending";
                }
                public static class GatewayAPI
                {
                    public static readonly string Url = ConfigurationManager.AppSettings["WisePay.KPayment.Url"];
                    public static readonly string Charge = "/card/v2/charge";
                    public static readonly string QRInquiry = "/card/v2/charge";
                }
            }
            public static class SettledInterval
            {
                public static readonly int HOURS = Convert.ToInt32(ConfigurationManager.AppSettings["WisePay.Interval.H"]);
                public static readonly int MUNITES = Convert.ToInt32(ConfigurationManager.AppSettings["WisePay.Interval.M"]);
                public static readonly int DAYS = Convert.ToInt32(ConfigurationManager.AppSettings["WisePay.Interval.Day"]);
            }
        }
    }
}
