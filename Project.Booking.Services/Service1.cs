using Project.Booking.Services.Services.WisePay;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Threading.Tasks;

namespace Project.Booking.Services
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer(); // name space(using System.Timers;) 
        DatabaseContext db = new DatabaseContext();
        KPaymentService _kpaymentService;
        public Service1()
        {
            InitializeComponent();
            _kpaymentService = new KPaymentService();
        }

        public void OnDebug()
        {
            this.OnStart(null);

        }

        protected override void OnStart(string[] args)
        {
            //initOnlineBooking();
            initKPaymentSettled();
        }

        protected override void OnStop()
        {
            //WriteToFile(Constant.Service.UPDATE_OVER_DUE_BOOKING, "service is stopped at " + DateTime.Now);
            //timer.Stop();
            WriteToFile(Constant.Service.UPDATE_KPAYMENT_SETTLED, "service is stopped at " + DateTime.Now);
        }
        private void WriteToFile(string fileName, string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\Service_" + fileName + "_Log_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
        private string InnerException(Exception ex)
        {
            return (ex.InnerException != null) ? InnerException(ex.InnerException) : ex.Message;
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            try
            {
                WriteToFile(Constant.Service.UPDATE_OVER_DUE_BOOKING, "service is recall at " + DateTime.Now);
                db.UpdateBookingOverduePayment();
            }
            catch (Exception ex)
            {
                WriteToFile(Constant.Service.UPDATE_OVER_DUE_BOOKING, "service is error at " + DateTime.Now);
                WriteToFile(Constant.Service.UPDATE_OVER_DUE_BOOKING, string.Format("****** error is " + InnerException(ex)));
            }
        }


        #region online booking
        private void initOnlineBooking()
        {
            //booking online update overdue payment
            var interval = Convert.ToDouble(Constant.ONLINE_BOOKING_CANCEL_ORDER_INTERVAL);
            WriteToFile(Constant.Service.UPDATE_OVER_DUE_BOOKING, "Service online booking is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = interval; //number in milisecinds  
            timer.Enabled = true;
        }
        #endregion

        #region Wise Pay
        private void initKPaymentSettled()
        {
            WriteToFile(Constant.Service.UPDATE_KPAYMENT_SETTLED, "service is started at " + DateTime.Now);
            MyScheduler.IntervalInDays(
                Constant.WisePay.SettledInterval.HOURS,
                Constant.WisePay.SettledInterval.MUNITES,
                Constant.WisePay.SettledInterval.DAYS,
            () =>
            {
                runKPaymentSettled();
            });
        }
        private async void runKPaymentSettled()
        {
            try
            {
                WriteToFile(Constant.Service.UPDATE_KPAYMENT_SETTLED, "service is recall at " + DateTime.Now);
                Console.WriteLine("service is recall at " + DateTime.Now);
                await _kpaymentService.SaveUpdateSettled();
            }
            catch (Exception ex)
            {
                WriteToFile(Constant.Service.UPDATE_KPAYMENT_SETTLED, "service is error at " + DateTime.Now);
                WriteToFile(Constant.Service.UPDATE_KPAYMENT_SETTLED, string.Format("****** error is " + InnerException(ex)));
            }
        }
        #endregion
    }
}
