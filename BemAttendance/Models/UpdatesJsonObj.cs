using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEM.Models
{
    public class UpdatesJsonObj
    {
        public string taskID { get; set; }
        public List<UpdatesSubItem> updatesSubItems { get; set; }
    }
    public class UpdatesSubItem
    {
        public int operateType { get; set; }
        public List<EmployeeJsonObj> employees { get; set; }
    }
}