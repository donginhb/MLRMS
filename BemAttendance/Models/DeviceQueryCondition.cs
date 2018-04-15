using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class DeviceQueryCondition
    {
        public string searchkey { get; set; }
        public QueryDeviceSubType queryType { get; set; }
    }
    public enum QueryDeviceSubType
    {
        ALL,
        DevCode,
        DevName,
        DevAddress,
        DevManager
    }
}