using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class PageModelV2
    {
        public string SearchKeyWord { get; set; }
        public int? SearchType { get; set; }
        public virtual string ActionName
        {
            get { return "Index"; }
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
        public string BeginTime { get; set; }
        public string EndTime { get; set; }
        //public string UserName { get; set; }
        //public string UserCode { get; set; }
        //public string DevCode { get; set; }
        public string CompanyName { get; set; }
        public string DptName { get; set; }
        //public string IdCard { get; set; }
        //public bool AdvanceSearch { get; set; }
    }
}