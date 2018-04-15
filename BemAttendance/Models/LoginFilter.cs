using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEMAttendance.Models
{
    [AttributeUsage(AttributeTargets.All,AllowMultiple =true,Inherited =true)]
    public class LoginFilter:ActionFilterAttribute
    {
        /// <summary>  
            /// OnActionExecuting是Action执行前的操作  
            /// </summary>  
            /// <param name="filterContext"></param>  
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //判断Cookie用户名密码是否存在  
            HttpCookie cookieName = System.Web.HttpContext.Current.Request.Cookies.Get("name");
            if (cookieName == null)
            {
                filterContext.Result = new RedirectResult("/Login/Index");
            }
        }
    }
}