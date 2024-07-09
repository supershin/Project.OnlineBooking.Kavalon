using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Project.Booking.Model;

namespace Project.Booking.Web.Hubs
{
    public class NotifyHub : Hub
    {
        [HubMethodName("sendUnitStatus")]
        public void sendUnitStatus(SendUnitSignalModel obj)
        {
            Clients.All.sendUnitStatus(obj);
        }
    }
}