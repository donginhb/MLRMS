using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class AttendanceObj
    {
        public long RecordID { get; set; }
        public string IDCard { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string DeviceCode { get; set; }
        public string DeviceName { get; set; }
        public string DeviceAddress { get; set; }
        public string DepartmentCode { get; set; }
        public string Department1 { get; set; }
        public DateTime ClockTime { get; set; }
        public string DeviceManager { get; set; }
    }
}