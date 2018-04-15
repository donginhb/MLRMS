using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models.Params
{
    public class HeatPumpParas
    {
        public double coldEnergy { get; set; }
        public double warmEnergy { get; set; }
        public int slaveID { get; set; }
        public string name { get; set; }
        public int type { get; set; }
        public string typeName { get; set; }
        public int? subType { get; set; }
        public string subTypeName { get; set; }
        public DateTime time { get; set; }
    }
}