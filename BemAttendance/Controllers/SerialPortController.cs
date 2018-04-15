using BEMAttendance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BEMAttendance.Controllers
{
    public class SerialPortController : Controller
    {
        VisitorRole role;
        string department1Code = string.Empty;
        string LoginName;

        List<serialservice> pageList = null;
        // GET: SerialPort
        public ActionResult Index(string searchkey, string index)
        {
            string name, error;
            if (CookieHelper.HasCookie(out name, out error) == false)
            {
                return RedirectToAction("", "LoginUI");
            }
            else
            {
                new RoleHelper().GetRoles(name, out role, out department1Code, out LoginName);
                ViewData["VisitorRole"] = role;
                ViewData["username"] = LoginName;
            }
            if (string.IsNullOrEmpty(index))
                index = "1";
            if (searchkey == string.Empty)
            {
                return RedirectToAction("", "Device");
            }
            if (searchkey == null)
                searchkey = string.Empty;


            DeviceQueryCondition condition = new DeviceQueryCondition();
            condition.searchkey = searchkey;
            ViewData["requestData"] = condition;
            List<serialservice> totalList = null;
            try
            {
                using (mlrmsEntities db = new mlrmsEntities())
                {
                    int totalCount = db.serialservice.Count();
                    if (totalCount == 0)
                    {
                        return View();
                    }
                    PageModel page = new PageModel() { SearchKeyWord = searchkey, SearchType = 0, CurrentIndex = Int32.Parse(index), TotalCount = totalCount };
                    totalList = db.serialservice.Where(p => p.name.ToLower().Contains(searchkey.ToLower()) || p.code.ToString().Contains(searchkey.ToLower())).OrderBy(m => m.name).Skip((page.CurrentIndex - 1) * page.PageSize).Take(page.PageSize).ToList();
                    ViewData["pagemodel"] = page;
                    return View(totalList);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("设备列表加载失败", ex);
                return View();
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
        public async Task<ActionResult> Create(serialservice info)
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
            if (ModelState.IsValid)
            {
                using (mlrmsEntities db = new mlrmsEntities())
                {
                    if (db.serialservice.Where(m => m.code == info.code||m.name==info.name).Count() > 0)
                    {
                        ModelState.AddModelError("Title", "相同设备编号或设备名称的串口服务器已存在，请更换后重新添加");
                        return View(info);
                    }
                   
                    try
                    {
                        db.serialservice.Add(info);
                        await db.SaveChangesAsync();
                        //MvcApplication.Client.Refresh(dev.slaveid, true);
                        return RedirectToAction("Index", "serialport");
                    }
                    catch
                    {
                        ModelState.AddModelError("Title", "数据保存失败");
                        return View(info);
                    }
                }
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
                return RedirectToAction("Index", "serialport");
            }
            try
            {
                using (mlrmsEntities db = new mlrmsEntities())
                {
                    if (db.device.Where(m => m.servicecode == id).Count() > 0)
                    {
                        return Content("1", "text/html");
                    }
                    else
                    {
                        serialservice matchOne = db.serialservice.Where(m => m.code == id).ToList().First();
                        db.serialservice.Attach(matchOne);
                        db.Entry(matchOne).State = System.Data.Entity.EntityState.Deleted;
                        await db.SaveChangesAsync();
                        return Content("0", "text/html");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("串口服务器删除异常", ex);
                return Content("2", "text/html");
            }
        }
    }
}