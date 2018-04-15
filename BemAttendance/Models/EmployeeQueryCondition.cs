using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class EmployeeQueryCondition
    {
        public string searchkey { get; set; }
        public QueryEmloyeeSubType queryType { get; set; }
        public string department { get; set; }
        public string status { get; set; }
    }
    public enum QueryEmloyeeSubType
    {
        ALL,
        UserCode,
        UserName,
        UserID,
        UserDepartment1,
        UserDepartment1Code,
    }
}