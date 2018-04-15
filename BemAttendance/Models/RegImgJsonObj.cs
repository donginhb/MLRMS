using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class RegImgJsonObj
    {
        public string empCode { get; set; }
        public string empName { get; set; }
        public string empID { get; set; }
        public string groupCode { get; set; }
        public string groupName { get; set; }
        public string devCode { get; set; }
        public string recordTime { get; set; }
        public string eyeType { get; set; }
        public string empEye { get; set; }
        public int eyeWidth { get; set; }
        public int eyeHeight { get; set; }
        public string empFace { get; set; }
        public string debugFile { get; set; }
    }
}