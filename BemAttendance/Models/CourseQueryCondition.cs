using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class CourseQueryCondition
    {
        public string timeStart { get; set; }
        public string timeEnd { get; set; }
        public string searchKey { get; set; }
    }
}