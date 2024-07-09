using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
    public class MatrixView
    {
        public string ProjectName { get; set; }

        private List<BuildView> _buildList;
        public List<BuildView> BuildList
        {
            get
            {
                _buildList = _buildList ?? new List<BuildView>();
                return _buildList;
            }
            set { _buildList = value; }
        }

    }
}
