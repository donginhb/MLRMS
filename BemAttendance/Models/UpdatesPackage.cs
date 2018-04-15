using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class UpdatesPackage
    {
        public softwareupdate updateInfo { get; set; }
        public List<clientdevice> devices { get; set; }
    }

}