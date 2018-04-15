using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class StudentAbsence
    {
        public employee emp { get; set; }
        public bool absence { get; set; }
        public DateTime clockTime { get; set; }
    }
}