using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class CookieHelper
    {
       public static bool HasCookie(out string name,out string error)
        {
            error = string.Empty;
            name = string.Empty;
            HttpCookie cookieName = System.Web.HttpContext.Current.Request.Cookies.Get("bemlogin");
            if (cookieName == null)
            {
                error = "未登录";
                return false;
            }
            System.Text.Encoding enc = System.Text.Encoding.GetEncoding("gb2312");
            name = HttpUtility.UrlDecode(cookieName.Value, enc);

            System.Web.HttpContext.Current.Response.Cookies.Remove("bemlogin");
            //更新Cookie
            string encodeStr = HttpUtility.UrlEncode(name, enc);
            System.Web.HttpCookie authCookie = new System.Web.HttpCookie("bemlogin", encodeStr);
            authCookie.Expires = DateTime.Now.AddMinutes(30);
            System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
            return true;
        }
    }
}