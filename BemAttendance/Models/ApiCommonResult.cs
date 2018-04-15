using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEM.Models
{
    public class ApiCommonResult
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public object results { get; set; }
        public ApiCommonResult(int errcode,string errmsg,object results)
        {
            this.errcode = errcode;
            this.errmsg = errmsg;
            this.results = results;
        }
    }
}