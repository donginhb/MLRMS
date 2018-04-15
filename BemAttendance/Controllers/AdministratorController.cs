using BEMAttendance.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
namespace BEMAttendance.Controllers
{
    public class AdministratorController : Controller
    {
        VisitorRole role;
        string department1Code = string.Empty;


        [System.Web.Mvc.HttpGet]
        public ActionResult Index()
        {
            string name, error,loginName;
            if (CookieHelper.HasCookie(out name, out error) == false)
            {
                return RedirectToAction("", "LoginUI");
            }
            else
            {
                new RoleHelper().GetRoles(name, out role, out department1Code,out loginName);
                ViewData["VisitorRole"] = role;
                ViewData["username"] = loginName;
            }
            try
            {
                using (mlrmsEntities db = new mlrmsEntities())
                {
                    //读取非超级管理员
                    var userList = from adm in db.sysadmin
                                   where adm.AdminType != 0
                                   select new SysAdminEx
                                   {
                                       AdminID = adm.AdminID,
                                       AdminCode = adm.AdminCode,
                                       AdminName = adm.AdminName,
                                       AdminPwd = adm.AdminPwd,
                                       AdminType = adm.AdminType,
                                       CreateDate = adm.CreateDate
                                   };

                    if (userList.Count() <= 0)
                    {
                        return View();
                    }
                    return View(userList.ToList());
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("数据库读取失败", ex);
                return Content("数据库读取失败");
            }
        }
        [System.Web.Mvc.HttpGet]
        public ActionResult Create()
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
            return View();
        }
        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> Create(NewAdmin info)
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
                using (mlrmsEntities db = new mlrmsEntities())
                {
                    var userList = db.sysadmin.Where(m => m.AdminCode == info.UserCode);

                    if (userList.Count() > 0)
                    {
                        ModelState.AddModelError("UserCode", "该管理员已存在");
                        return View(info);
                    }
                    sysadmin item = new sysadmin();
                    item.AdminName = info.UserName;
                    item.CreateDate = DateTime.Now;
                    item.AdminCode = info.UserCode;
                    item.AdminType = 1;
                    item.AdminPwd = EncryptHelper.GetEncrypt("123456");
                    db.sysadmin.Add(item);

                    int result = await db.SaveChangesAsync();
                    return RedirectToAction("", "administrator");
                }
            }
            catch(Exception ex)
            {
                LogHelper.Error("创建管理员失败", ex);
                ViewData["status"] = "false";
            }
            return View(info);
        }

        public async Task<ActionResult> Delete(string id)
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
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "LoginUI");
            }
            long pwdID = 0;
            if (long.TryParse(id, out pwdID) == false)
            {
                return Content("入参错误", "text/html");
            }
            using (mlrmsEntities db = new mlrmsEntities())
            {
                sysadmin matchOne = db.sysadmin.Where(m => m.AdminID == pwdID).ToList().First();
                db.sysadmin.Attach(matchOne);
                db.Entry(matchOne).State = System.Data.Entity.EntityState.Deleted;
                await db.SaveChangesAsync();
              
                return RedirectToAction("", "administrator");
            }
        }
        [System.Web.Mvc.HttpGet]
        public ActionResult Reset(string id)
        {
            try
            {
                using (mlrmsEntities db = new mlrmsEntities())
                {
                    int ID = int.Parse(id);
                    var usr = db.sysadmin.Where(m => m.AdminID == ID).First();
                    usr.AdminPwd = EncryptHelper.GetEncrypt("123456");
                    db.sysadmin.Attach(usr);
                    db.Entry(usr).State = System.Data.Entity.EntityState.Modified;
                    int result = db.SaveChanges();
                    if (result > 0)
                    {
                        return Content("True", "text/html");
                    }
                    else
                    {
                        return Content("False", "text/html");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("密码重置出错:" + ex.Message);
                return Content("False", "text/html");
            }
        }
    }
}