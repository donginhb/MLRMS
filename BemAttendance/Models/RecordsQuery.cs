using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class RecordsQuery
    {
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string empCode { get; set; }
        public string deviceCode { get; set; }
        public string departmentCode { get; set; }
    }
}