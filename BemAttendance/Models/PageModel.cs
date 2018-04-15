using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class PageModel
    {
        public string id { get; set; }
        public string SearchKeyWord { get; set; }
        public int? SearchType { get; set; }
        private string actionName = "Index";
        public string DptSelectCode { get; set; }
        public string RegStatus { get; set; }
        public virtual string ActionName
        {
            get { return actionName; }
            set { actionName = value; }
        }
        public int TotalCount { get; set; }
        public int CurrentIndex { get; set; }
        public int TotalPages
        {
            get { return (int)Math.Ceiling((double)TotalCount / (double)PageSize); }
        }
        public virtual int PageSize
        {
            get { return 15; }
        }
        public virtual int DisplayMaxPages
        {
            get { return 10; }
        }
        public bool IsHasPrePage
        {
            get { return CurrentIndex != 1; }
        }
        public bool IsHasNextPage
        {
            get
            {
                return CurrentIndex != TotalPages;
            }
        }
    }
}