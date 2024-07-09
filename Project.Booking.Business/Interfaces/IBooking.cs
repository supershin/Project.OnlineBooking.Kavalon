using Project.Booking.Data;
using Project.Booking.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Business.Interfaces
{
    public interface IBooking
    {
        List<BookingModel> GetBookingHistory(Guid? bookingID = null, OnlineBookingEntities context = null);
        Guid SaveBooking(Guid unitID);
        void UpdateUnitStatus(Guid unitID, int statusID);
        Guid SaveCancelBooking(Guid bookingID);
        CheckoutViewModel GetBookingCheckout(Guid bookingID);
        void SaveBookingCustomer(BookingModel model);
        ts_Booking UpdateBookingStatus(Guid bookingID, int statusID);
        void SaveUpdateUnitStatus_VIP(Guid unitID);
        List<BookingVIP> GetBookingVIPSendMail(int length);
        void SaveBookingVIPSendMail(BookingVIP model);
        List<BookingModel> GetBookingSendMail(Guid bookingID);
    }
}
