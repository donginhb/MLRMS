using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEM.Models
{
    public class LoginInfo
    {
        public string accessCode { get; set; }
        public string loginSignature { get; set; }
        public string devType { get; set; }
        public string devIP { get; set; }
        public string version { get; set; }
    }
}