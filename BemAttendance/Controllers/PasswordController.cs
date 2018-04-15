using BEMAttendance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
namespace BEMAttendance.Controllers
{
    public class PasswordController : Controller
    {
        VisitorRole role;
        string department1Code = string.Empty;
        // GET: Manager
        [System.Web.Mvc.HttpGet]
        public ActionResult Index()
        {
            string name, error, loginName;
            if (CookieHelper.HasCookie(out name, out error) == false)
            {
                return RedirectToAction("", "LoginUI");
            }
            else
            {
                new RoleHelper().GetRoles(name, out role, out department1Code, out loginName);
                ViewData["VisitorRole"] = role;
                ViewData["username"] = loginName;
            }
            using (mlrmsEntities db = new mlrmsEntities())
            {
                var userList = db.sysadmin.Where(m => m.AdminCode == name);
                if (userList == null || userList.Count() <= 0)
                {
                    return Content("Not Found", "text/html");
                }
                sysadmin item = userList.First();
                Manager manager = new Manager();
                manager.UserCode = item.AdminCode;
                manager.UserName = item.AdminName;
                manager.AdminId = item.AdminID;
                return View(manager);
            }
        }
        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> Index(Manager info)
        {
            string name, error, loginName;
            if (CookieHelper.HasCookie(out name, out error) == false)
            {
                return RedirectToAction("", "LoginUI");
            }
            else
            {
                new RoleHelper().GetRoles(name, out role, out department1Code, out loginName);
                ViewData["VisitorRole"] = role;
                ViewData["username"] = loginName;
            }
            try
            {
                if (ModelState.IsValid)
                {
                    using (mlrmsEntities db = new mlrmsEntities())
                    {
                        var userList = db.sysadmin.Where(m => m.AdminID == info.AdminId);
                        if (userList == null || userList.Count() <= 0)
                        {
                            return Content("Not Found", "text/html");
                        }
                        sysadmin item = userList.First();
                        string pwd = EncryptHelper.GetEncrypt(info.Passwd);
                        if (pwd != item.AdminPwd)   //如果密码验证不通过
                        {
                            ModelState.AddModelError("Passwd", "密码错误");
                            return View();
                        }
                        item.AdminPwd = EncryptHelper.GetEncrypt(info.NewPwd);
                        db.sysadmin.Attach(item);
                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        ViewData["status"] = "true";
                        return RedirectToAction("Logout", "LoginUI");
                    }
                }
            }
            catch
            {
                ViewData["status"] = "false";
            }
            Manager manager = new Manager();
            manager.UserName = info.UserName;
            //系统登出
            return View(manager);
        }
    }
}