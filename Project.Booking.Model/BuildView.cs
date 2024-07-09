using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
    public class BuildView
    {
        public int FloorMax { get; set; }
        public int FloorMin { get; set; }
        public int RoomMax { get; set; }
        public int RoomMin { get; set; }

        public string BuildName { get; set; }

        private List<FloorView> _floorList;
        public List<FloorView> FloorList
        {
            get
            {
                _floorList = _floorList ?? new List<FloorView>();
                return _floorList;
            }
            set { _floorList = value; }
        }

        private List<MatrixConfig> _matrixConfigs;

        public List<MatrixConfig> MatrixConfigs
        {
            get
            {
                _matrixConfigs = _matrixConfigs ?? new List<MatrixConfig>();
                return _matrixConfigs;
            }
            set { _matrixConfigs = value; }
        }

    }
}
