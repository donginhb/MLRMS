using BEM.Models;
using BEMAttendance.Models;
using BEMAttendance.Models.Params;
using DataCollection.Model;
using DataCollection.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
namespace BEMAttendance.Controllers
{
    public class DeviceController : Controller
    {
        List<DeviceInfo> pageList = null;

        VisitorRole role;
        string department1Code = string.Empty;
        string LoginName;

        public ActionResult Index(string searchkey, string index, int? searchType)
        {
            string name, error;
            if (CookieHelper.HasCookie(out name, out error) == false)
            {
                return RedirectToAction("", "LoginUI");
            }
            else
            {
                new RoleHelper().GetRoles(name, out role, out department1Code,out LoginName);
                ViewData["VisitorRole"] = role;
                ViewData["username"] = LoginName;
            }
            if (string.IsNullOrEmpty(index))
                index = "1";
            if(searchkey==string.Empty)
            {
                return RedirectToAction("", "Device");
            }
            if (searchkey==null)
                searchkey = string.Empty;

            if (searchType==null)
            {
                searchType = (int)QueryDeviceSubType.ALL;
            }

            DeviceQueryCondition condition = new DeviceQueryCondition();
            condition.searchkey = searchkey;
            if (string.IsNullOrEmpty(searchkey))
            {
                searchType = (int)QueryDeviceSubType.ALL;  //当searchkey为空时，默认加载全部
            }
            condition.queryType = (QueryDeviceSubType)searchType;
            ViewData["requestData"] = condition;
            List<device> totalList = null;
            try
            {
                using (mlrmsEntities db = new mlrmsEntities())
                {
                    int totalCount = db.device.Count();
                    if (totalCount == 0)
                    {
                        return View();
                    }
                    PageModel page = new PageModel() { SearchKeyWord = searchkey, SearchType = searchType, CurrentIndex = Int32.Parse(index), TotalCount = totalCount };
                    totalList = db.device.Where(p => p.name.ToLower().Contains(searchkey.ToLower()) || p.slaveid.ToString().Contains(searchkey.ToLower())).OrderBy(m => m.type).Skip((page.CurrentIndex - 1) * page.PageSize).Take(page.PageSize).ToList();
                    pageList = new List<DeviceInfo>();
                    foreach(var item in totalList)
                    {
                        DeviceInfo info = new DeviceInfo();
                        info.slaveID = item.slaveid;
                        info.note = item.note;
                        info.devName = item.name;
                        info.serviceCode = item.servicecode;
                        info.serviceName = item.servicename;
                        info.port = item.port;
                        info.runStatus = 1;
                        info.type = item.type;
                        info.typeName = EnumParser.TypeParser(item.type);
                        info.subType = item.subtype.HasValue ?item.subtype.Value : -1;
                        info.subTypeName = EnumParser.SubTypeParser(info.subType);
                        pageList.Add(info);
                    }
                    ViewData["pagemodel"] = page;
                    return View(pageList);
                }
            }
            catch(Exception ex)
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
            List<DeviceEnumType> list = new List<DeviceEnumType>();
            list.Add(new DeviceEnumType() { code = "1", name = "热泵" });
            list.Add(new DeviceEnumType() { code = "3", name = "水表" });
            list.Add(new DeviceEnumType() { code = "4", name = "电表" });
            list.Add(new DeviceEnumType() { code = "5", name = "水泵" });

            SelectList dptSelectlist1 = new SelectList(list, "code", "name");
            ViewData["dptName1List"] = dptSelectlist1;
            SelectList serialList = null;

            List<DeviceEnumType> list2 = new List<DeviceEnumType>();
            list2.Add(new DeviceEnumType() { code = "5", name = "25P" });
            list2.Add(new DeviceEnumType() { code = "6", name = "50P" });

            SelectList dptSelectlist2 = new SelectList(list2, "code", "name");
            ViewData["subList"] = dptSelectlist2;
            using (mlrmsEntities db = new mlrmsEntities())
            {
                var slist = db.serialservice.Select(m => new { code = m.code, name = m.name }).ToList();
                string code = slist.First().code;
                serialList = new SelectList(slist, "code", "name", code);
                ViewData["serialList"] = serialList;
            }
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> Create(DeviceInfo info,string dptName1List, string subList,string serialList)
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
            info.type = int.Parse(dptName1List);
            if(info.type==(int)DeviceType.MainEngine || info.type==(int)DeviceType.WaterPump)
            {
                info.subType = int.Parse(subList);
            }
            List<DeviceEnumType> list = new List<DeviceEnumType>();
            list.Add(new DeviceEnumType() { code = "1", name = "热泵主机" });
            list.Add(new DeviceEnumType() { code = "3", name = "水表" });
            list.Add(new DeviceEnumType() { code = "4", name = "电表" });
            list.Add(new DeviceEnumType() { code = "2", name = "热能表" });

            SelectList dptSelectlist1 = new SelectList(list, "code", "name", dptName1List);
            ViewData["dptName1List"] = dptSelectlist1;

            List<DeviceEnumType> list2 = new List<DeviceEnumType>();
            list2.Add(new DeviceEnumType() { code = "5", name = "25P" });
            list2.Add(new DeviceEnumType() { code = "6", name = "50P" });

            SelectList dptSelectlist2 = new SelectList(list2, "code", "name", subList);
            ViewData["subList"] = dptSelectlist2;

            SelectList serialListx = null;
            serialservice seviceItem = null;
            using (mlrmsEntities db = new mlrmsEntities())
            {
                var slist = db.serialservice.ToList();
  
                serialListx = new SelectList(slist, "code", "name", serialList);
                ViewData["serialList"] = serialListx;
                if(slist!=null||slist.Count>0)
                {
                    seviceItem = slist.Where(m => m.code == serialList).First();
                }
            }
            if(info.port>seviceItem.portcount)
            {
                ModelState.AddModelError("Title", "配置的端口号不能大于该串口服务器的串口数，请重新配置");
                return View(info);
            }
            info.serviceCode = seviceItem.code;
            info.serviceName = seviceItem.name;

            if (ModelState.IsValid)
                {
                    using (mlrmsEntities db = new mlrmsEntities())
                    {
                        if (db.device.Where(m => m.port == info.port && m.servicecode == info.serviceCode).Count() > 0)
                        {
                            ModelState.AddModelError("Title", "串口服务器的端口已配置，请更换端口号重新添加");
                            return View(info);
                        }
                        if (db.device.Where(m => m.slaveid == info.slaveID && m.port == info.port && m.servicecode == info.serviceCode).Count() > 0)
                        {
                            ModelState.AddModelError("Title", "相同设备地址的设备已存在，请更换设备地址后重新添加");
                            return View(info);
                        }

                        device dev = new device();
                        dev.note = info.note;
                        dev.slaveid = info.slaveID;
                        dev.servicecode = info.serviceCode;
                        dev.servicename = info.serviceName;
                        dev.port = info.port;
                        dev.type = int.Parse(dptName1List);
                        dev.subtype = info.subType == 0 ? null : (int?)int.Parse(subList);
                        dev.name = info.devName;
                        dev.note = info.note;
                        db.device.Add(dev);
                        try
                        {
                            await db.SaveChangesAsync();
                            MvcApplication.Client.Refresh(dev.slaveid, true);
                            return RedirectToAction("Index", "Device");
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

        public async Task<ActionResult> Delete(string id,string serialCode,string port)
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
            int slaveID = int.Parse(id);
            int servicePort = int.Parse(port);
            using (mlrmsEntities db = new mlrmsEntities())
            {
                device matchOne = db.device.Where(m => m.slaveid ==slaveID &&m.servicecode==serialCode&& m.port== servicePort).ToList().First();
                db.device.Attach(matchOne);
                db.Entry(matchOne).State = System.Data.Entity.EntityState.Deleted;
                await db.SaveChangesAsync();
                //通知客户端已经移除；
                MvcApplication.Client.Refresh(slaveID, false);
            }
            return RedirectToAction("Index", "Device");
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult Operate(string id)
        {
            string name, error, loginName;
            DeviceInfo info = null;

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
                string[] ss = id.Split('#');
                int slaveID = int.Parse(ss[0]);
                string serialCode = ss[1];
                int serialPort = int.Parse(ss[2]);
                using (mlrmsEntities db = new mlrmsEntities())
                {
                    info = (from dev in db.device
                            where dev.slaveid== slaveID &&dev.port==serialPort&&dev.servicecode==serialCode
                            select new DeviceInfo
                            {
                                slaveID = dev.slaveid,
                                note = dev.note,
                                devName = dev.name,
                                type = dev.type,
                                subType=(int)dev.subtype,
                            }).First();
                }
                if(info!=null)
                {
                    info.subTypeName = EnumParser.SubTypeParser(info.subType);
                }
                EngineData data = HashGetEngineData.GetData(slaveID);
                info.runStatus = data.State;
                info.SetMode = data.Mode.ToString();
                info.SetTemp = data.Temp.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.Error("打开配置页面失败", ex);
            }
            return View(info);
        }
        [System.Web.Mvc.HttpGet]
        public ActionResult SetTemp(string slaveid,string temp)
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
            double t = 0;
            int id = int.Parse(slaveid);
            if(double.TryParse(temp,out t)==false)
            {
                return Content("输入的温度格式不对", "text/html");
            }
            else
            {
              if( MvcApplication.Client.SetTemp(id, t)==true)
                {
                    return Content("OK", "text/html");
                }
              else
                {
                    return Content("温度设置失败", "text/html");
                }
            }
        }
        [System.Web.Mvc.HttpGet]
        public ActionResult SetMode(string slaveid,string mode)
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
            int id = int.Parse(slaveid);
            int md = int.Parse(mode);
            if (MvcApplication.Client.SetMode(id, md) == true)
            {
                return Content("OK", "text/html");
            }
            else
            {
                return Content("模式设置失败", "text/html");
            }
        }
        [System.Web.Mvc.HttpGet]
        public ActionResult SetRunStatus(string slaveid, string mode)
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
            int id = int.Parse(slaveid);
            int md = int.Parse(mode);
            bool value = Convert.ToBoolean(md);
            if (MvcApplication.Client.Operate(id, value) == true)
            {
                return Content("OK", "text/html");
            }
            else
            {
                if(mode=="0")
                {
                    return Content("停止设备失败", "text/html");
                }
                else
                {
                    return Content("开启设备成功", "text/html");
                }
       
            }

        }
        [System.Web.Mvc.HttpGet]
        public ActionResult GetDeviceStatus(string slaveid)
        {
            string deviceCode = slaveid.TrimStart().TrimEnd();
            int id = int.Parse(deviceCode);
            try
            {
                EngineData data=  HashGetEngineData.GetData(id);
                if(data!=null)
                {
                    return Content("OK:"+data.State, "text/html");
                }
                else
                {
                    return Content("Error:", "text/html");
                }
         
            }
            catch
            {
                return Content("Error:", "text/html");
            }
            
        }
        
    }
    class DeviceEnumType
    {
        public string code { get; set; }
        public string name { get; set; }
    }
}
