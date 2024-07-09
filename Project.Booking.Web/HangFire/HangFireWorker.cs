using Microsoft.AspNet.SignalR;
using Project.Booking.Business.Sevices;
using Project.Booking.Constants;
using Project.Booking.Model;
using Project.Booking.Web.Hubs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Project.Booking.Web.HangFire
{
    public class HangFireWorker
    {
        public HangFireWorker()
        {

        }
        public void UpdateBookingPaymentOverdue()
        {
            HangFireService service = new HangFireService();
            var units = service.UpdateBookingPaymentOverdue(Constant.PAYMENT_DUE_DURATION);
            NotifyUnitStatusSignalR(units);

        }
        private void NotifyUnitStatusSignalR(List<Unit> units)
        {
            var notifyHub = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            foreach (var unit in units)
            {
                SendUnitSignalModel sendUnit = new SendUnitSignalModel(unit);
                notifyHub.Clients.All.sendUnitStatus(sendUnit);
            }
        }
    }
}