using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEM.Models
{
    public class EmployeeJsonObj
    {
        public string empCode { get; set; }
        public string empID { get; set; }
        public string empName { get; set; }
        public string empBirth { get; set; }
        public string empGender { get; set; }
        public string empGroupName { get; set; } 
        public string empGroupCode { get; set; }                        
        public int empLeftNum { get; set; }
        public int empRightNum { get; set; }
        public int empTemplateSize { get; set; }
        public string empTemplate { get; set; }
        public string empLeftEyeImg { get; set; }
        public string empRightEyeImg { get; set; }
        public string empFaceImg { get; set; }
        public string empFaceTemplate { get; set; }
        public string empTelephone { get; set; }
        public string empAdmin { get; set; }
    }
}