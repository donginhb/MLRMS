using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class FtpHelper
    {
        public static string VideoFtpURL { get; set; }
        public static string PackageFtpURL { get; set; }
        public static bool NeedUpToEC { get; set; }
    }
}