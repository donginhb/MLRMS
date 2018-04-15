using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class AttendanceQueryCondition
    {
        public string timeStart1 { get; set; }
        public string timeEnd1   { get; set; }
        public string timeStart2 { get; set; }
        public string timeEnd2 { get; set; }
        public string searchKey { get; set; }
        public string IDCard { get; set; }
        public string devCode { get; set; }
        public string userName { get; set; }
        public string userCode { get; set; }
        public string department1Code { get; set; }
        public string department1 { get; set; }
        public string companyName { get; set; }
    }
}