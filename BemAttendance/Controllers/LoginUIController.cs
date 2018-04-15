using BEMAttendance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
namespace BEMAttendance.Controllers
{
    public class LoginUIController : Controller
    {
        [System.Web.Mvc.HttpGet]
        public ActionResult Index()
        {
            string name, error;
            if (CookieHelper.HasCookie(out name, out error) == false)
            {
                ViewData["reloadJS"] = true;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Device");
            }
        }
        [System.Web.Mvc.HttpGet]
        public ActionResult Logout()
        {
            HttpCookie cookieName = System.Web.HttpContext.Current.Request.Cookies.Get("bemlogin");
            if(cookieName==null)
            {
                return RedirectToAction("Index", "LoginUI");
            }
            cookieName.Expires = DateTime.Now.AddDays(-1);
            System.Web.HttpContext.Current.Response.Cookies.Add(cookieName);
            return RedirectToAction("Index", "LoginUI");
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult Index(sysadmin user)
        {
            if (string.IsNullOrEmpty(user.AdminCode))
            {
                return Content("false:用户名不能为空", "text/html");
            }
            if (string.IsNullOrEmpty(user.AdminPwd))
            {
                return Content("false:密码不能为空", "text/html");
            }
            if (user.AdminCode.Length > 30)
            {
                return Content("false:用户名长度不能超过30", "text/html");
            }
            if (user.AdminPwd.Length > 32)
            {
                return Content("false:密码长度不能超过32", "text/html");
            }
                string encryptStr = EncryptHelper.GetEncrypt(user.AdminPwd);
            try
            {
                using (mlrmsEntities db = new mlrmsEntities())
                {
                    if (db.sysadmin.Where(m => (m.AdminCode == user.AdminCode) && (m.AdminPwd == encryptStr)).Count() > 0)
                    {
                        System.Text.Encoding enc = System.Text.Encoding.GetEncoding("gb2312");
                        string encodeStr = HttpUtility.UrlEncode(user.AdminCode, enc);
                        System.Web.HttpCookie authCookie = new System.Web.HttpCookie("bemlogin", encodeStr);
                        authCookie.Expires = DateTime.Now.AddMinutes(30);
                        System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
                        //return RedirectToAction("Index", "Device");
                        return Content("true:登录成功", "text/html");
                    }
                    else
                    {
                        return Content("false:用户名或密码错误", "text/html");
                    }
                }
            }
            catch(Exception ex)
            {
                LogHelper.Error("系统登录失败", ex);
                return Content("false:数据库连接失败，请检查配置", "text/html");
            }
               
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult CheckData(string userName,string userPwd)
        {
            if(string.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("UserName", "用户名不能为空");
                return View("Index");
            }
            if(string.IsNullOrEmpty(userPwd))
            {
                ModelState.AddModelError("Passwd", "密码不能为空");
                return View("Index");
            }
            if(userName.Length>30)
            {
                ModelState.AddModelError("UserName", "用户名长度不能超过30");
                return View("Index");
            }
            if(userPwd.Length>32)
            {
                ModelState.AddModelError("Passwd", "密码长度不能超过32");
                return View("Index");
            }
            string encryptStr = EncryptHelper.GetEncrypt(userPwd);
            using (mlrmsEntities db = new mlrmsEntities())
            {
                if (db.sysadmin.Where(m => (m.AdminName == userName) && (m.AdminPwd == encryptStr)).Count() > 0)
                {
                    return Content("True", "text/html");
                }
                else
                {
                    ModelState.AddModelError("Title", "用户名或密码错误");
                    return View("Index");
                }
            }
        }
    }
}
