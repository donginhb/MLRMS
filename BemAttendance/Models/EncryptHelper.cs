using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace BEMAttendance.Models
{
    public class EncryptHelper
    {
        public static string GetEncrypt(string pwd)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(pwd, "MD5");
        }
    }
}