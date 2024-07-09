using Project.Booking.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Business.Interfaces
{
    public interface IMatrix
    {
        MatrixView GetMatrix(string projectCode, string builds);
        UnitView GetUnitByID(Guid unitID);
    }
}
