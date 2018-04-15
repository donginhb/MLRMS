using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class DepartmentOne
    {
        public string dptName { get; set; }
        public string dptCode { get; set; }
        public bool HasBindDevice { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if ((obj).GetType().Equals(this.GetType()) == false)
            {
                return false;
            }
            DepartmentOne temp = null;
            temp = (DepartmentOne)obj;
            return this.dptCode.Equals(temp.dptCode);
        }
        public override int GetHashCode()
        {
            return this.dptCode.GetHashCode();
        }
    }
}