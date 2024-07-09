using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
    public class SendUnitSignalModel
    {
        private readonly Unit _unit;
        public SendUnitSignalModel(Unit unit)
        {
            _unit = unit;
            this.ProjectName = _unit.ProjectName;
            this.UnitID = _unit.ID;
            this.UnitCode = _unit.UnitCode;
            this.UnitStatusName = _unit.UnitStatus;
            this.UnitStatusColor = _unit.UnitStatusColor;
        }
        public SendUnitSignalModel()
        {           
        }
        public string ProjectName { get; set; }
        public Guid UnitID { get; set; }
        public string UnitCode { get; set; }
        public string UnitStatusName { get; set; }
        public string UnitStatusColor { get; set; }
    }
}
