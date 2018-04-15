using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models.Params
{
    public class ElectricParas
    {
        public double electricAmount { get; set; }
        public int slaveID { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public DateTime time { get; set; }
    }
}