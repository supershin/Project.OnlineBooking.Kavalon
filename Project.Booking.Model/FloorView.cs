using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
    public class FloorView
    {
        public int FloorName { get; set; }

        private List<UnitView> _unitList;
        public List<UnitView> UnitList
        {
            get {
                _unitList = _unitList ?? new List<UnitView>();
                return _unitList; }
            set { _unitList = value; }
        }


    }
}
