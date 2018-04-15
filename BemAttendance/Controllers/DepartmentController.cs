using BEMAttendance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CheckInGroup = BEMAttendance.checkingroup;

namespace BEMAttendance.Controllers
{
    public class DepartmentController : Controller
    {
        static int total_count;
        const int page_size = 15;


        VisitorRole role;
        string department1Code = string.Empty;

        // GET: Department
        public ActionResult Index(string index)
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
            if (string.IsNullOrEmpty(index))
                index = "1";
            int indexInt = 0;
            if (!int.TryParse(index,out indexInt))
            {
                indexInt = 1;
            }
            List<CheckInGroup> totalList = null;
            try
            {
                using (BemEntities db = new BemEntities())
                {
                    if (indexInt == 1)
                    {
                        total_count = db.checkingroup.Count();
                    }
                    totalList = db.checkingroup.OrderBy(m => m.GroupCode.Length).ThenBy(m => m.GroupCode).Skip((indexInt - 1) * page_size).Take(page_size).ToList();
                    if (totalList == null || totalList.Count == 0)
                    {
                        return View();
                    }
                    PageModel page = new PageModel() { CurrentIndex = indexInt, TotalCount = total_count };
                    ViewData["pagemodel"] = page;
                    return View(totalList);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("获取部门信息异常", ex);
                return View();
            }
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult Create(CheckInGroup group)
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
                using (BemEntities db = new BemEntities())
                {
                    if (db.checkingroup.Where(m => (m.GroupCode == group.GroupCode || m.GroupName == group.GroupName)).Count() > 0)
                    {
                        return Content("false:相同部门编号或部门名称的记录已存在", "text/html");
                    }
                    group.GroupParent = "0";
                    db.checkingroup.Add(group);
                    db.SaveChangesAsync();
                    return Content("true:OK", "text/html");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("部门新增失败", ex);
                return Content("false:部门新增失败", "text/html");
            }
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
            using (BemEntities db = new BemEntities())
            {
                CheckInGroup group = db.checkingroup.Where(m => m.GroupCode == id).ToList().First();
                db.checkingroup.Attach(group);
                db.Entry(group).State = System.Data.Entity.EntityState.Deleted;
                await db.SaveChangesAsync();
            }
            return RedirectToAction("", "department");
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult Edit(string id)
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
                return RedirectToAction("Index", "Device");
            }
            using (BemEntities db = new BemEntities())
            {
                CheckInGroup matchOne = db.checkingroup.Where(m => m.GroupCode == id).ToList().First();
                return View(matchOne);
            }
  
        }

        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> Edit(CheckInGroup info)
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
                    using (BemEntities db = new BemEntities())
                    {
                        if (db.checkingroup.Where(m => (m.GroupCode == info.GroupCode && m.GroupName == info.GroupName)).Count() > 0)
                        {
                            return RedirectToAction("Index", "Department"); //没做任何改变
                        }
                        if (db.checkingroup.Where(m => (m.GroupName == info.GroupName)).Count() > 0)
                        {
                            ModelState.AddModelError("Title", "相同部门名称的记录已存在");
                            return View();
                        }
                        CheckInGroup matchOne = db.checkingroup.Where(m => m.GroupCode == info.GroupCode).ToList().First();
                        if (matchOne != null)
                        {
                            matchOne.GroupName = info.GroupName;
                            db.checkingroup.Attach(matchOne);
                            db.Entry(matchOne).State = System.Data.Entity.EntityState.Modified;
                            string cmdStr = string.Format("update employee set EmpGroupName='{0}' where EmpGroupCode= '{1}';", info.GroupName, info.GroupCode);
                            db.Database.ExecuteSqlCommand(cmdStr);
                            await db.SaveChangesAsync();
                            return RedirectToAction("Index", "Department");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.Error("编辑部门异常", e);
                ModelState.AddModelError("Title", "数据保存异常");
            }
        
            return View();
        }
    }
}