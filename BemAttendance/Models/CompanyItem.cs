using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Employee = BEMAttendance.employee;
namespace BEMAttendance.Models
{
    public class CompanyItem
    {
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public List<Employee> subList { get; set; }
    }
}