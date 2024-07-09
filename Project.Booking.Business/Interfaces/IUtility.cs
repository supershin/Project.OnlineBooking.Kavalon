using Project.Booking.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Business.Interfaces
{
    public interface IUtility
    {
        UserProfile GetRegister(string email);
        void SaveRegister(UserProfile model);
        UserProfile Authentcation(UserProfile model);
        void SaveSignInOut(UserProfile model);
    }
}
