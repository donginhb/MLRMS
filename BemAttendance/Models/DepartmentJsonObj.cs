using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class DepartmentJsonObj
    {
        public string dptCode { get; set; }
        public string dptName { get; set; }
        public string dptParent { get; set; }
    }
}