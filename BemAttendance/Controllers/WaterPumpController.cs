using BEMAttendance.Models;
using BEMAttendance.Models.Params;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BEMAttendance.Controllers
{
    public class WaterPumpController : Controller
    {
        const int page_size = 20;
        static int totalCount;


        VisitorRole role;
        string department1Code = string.Empty;

        // GET: AttendanceAccount
        public ActionResult Index(string beginTime1, string endTime1, string searchkey, string dptName1List, string index)
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
            DateTime dt = DateTime.Now;
            if (string.IsNullOrEmpty(beginTime1) && string.IsNullOrEmpty(endTime1))
            {
                beginTime1 = dt.ToString("yyyy/MM/01 00:00");
                endTime1 = dt.ToString("yyyy/MM/dd 23:59");
            }

            if (string.IsNullOrEmpty(searchkey))
            {
                searchkey = string.Empty;
            }
            if (string.IsNullOrEmpty(index))
            {
                index = "1";
            }
            if (string.IsNullOrEmpty(dptName1List))
            {
                dptName1List = "0";
            }
            AttendanceQueryCondition requestData = new AttendanceQueryCondition();  //回传给界面显示用
            requestData.timeStart1 = beginTime1;
            requestData.timeEnd1 = endTime1;
            requestData.searchKey = searchkey;
            requestData.department1 = dptName1List;
            ViewData["requestData"] = requestData;
            List<WaterPumpParas> attendanceResults = new List<WaterPumpParas>();
            try
            {
                SelectList dptSelectlist1 = null;
                using (mlrmsEntities db = new mlrmsEntities())
                {
                    var dptList = db.device.Where(m=>m.type==5).Select(m => new { slaveid = m.slaveid, devname = m.name }).ToList();
                    dptList.Insert(0, new { slaveid = 0, devname = "全部" });
                    int heatPumpCode = int.Parse(dptName1List);
                    dptSelectlist1 = new SelectList(dptList, "slaveid", "devname", heatPumpCode);
                    ViewData["dptName1List"] = dptSelectlist1;
                }
                if (DateTime.Parse(beginTime1) > DateTime.Parse(endTime1))
                {
                    ViewData["timeValid"] = true;
                    return View();
                }
                attendanceResults = GetPageAttendanceInfos(Convert.ToInt32(index), beginTime1, endTime1, searchkey, dptName1List);
            }
            catch (Exception ex)
            {
                //记录日志
                LogHelper.Error("水泵能耗读取异常", ex);
            }
            if (attendanceResults == null || attendanceResults.Count == 0)
            {
                return View();
            }
            PageModelV2 page = null;
            page = new PageModelV2() { SearchKeyWord = searchkey, DptName = dptName1List, BeginTime = beginTime1, EndTime = endTime1, CurrentIndex = Int32.Parse(index), TotalCount = totalCount };
            ViewData["pagemodel"] = page;
            return View(attendanceResults);
        }
        public List<WaterPumpParas> GetPageAttendanceInfos(int index, string beginTime1, string endTime1, string searchkey, string slaveCode)
        {
            DateTime dtStart = DateTime.Parse(beginTime1);
            DateTime dtEnd = DateTime.Parse(endTime1);
            int slaveID = int.Parse(slaveCode);
            List<WaterPumpParas> attendances = new List<WaterPumpParas>();
            try
            {
                using (mlrmsEntities db = new mlrmsEntities())
                {
                    IQueryable<WaterPumpParas> dynamicAttendance = null;
                    if (slaveCode != "0")
                    {
                        if (index == 1)
                        {
                            totalCount = (from heatPump in db.devices
                                          join bd in db.hmbindings
                                          on heatPump.slaveid equals bd.bindingid
                                          join dev in db.heatmeterstatistics
                                          on bd.hmid equals dev.slaveid
                                          where heatPump.type == 5
                                          where (heatPump.slaveid == slaveID)
                                          where (dev.time >= dtStart && dev.time <= dtEnd)
                                          where (heatPump.name.ToLower().Contains(searchkey.ToLower()))
                                          select heatPump).Count();
                        }
                        dynamicAttendance = (from heatPump in db.devices
                                             join bd in db.hmbindings
                                             on heatPump.slaveid equals bd.bindingid
                                             join dev in db.heatmeterstatistics
                                             on bd.hmid equals dev.slaveid
                                             where heatPump.type == 5
                                             where (heatPump.slaveid == slaveID)
                                             where (dev.time >= dtStart && dev.time <= dtEnd)
                                             where (heatPump.name.ToLower().Contains(searchkey.ToLower()))
                                             select new WaterPumpParas
                                             {
                                                 coldEnergy = dev.coldenergy,
                                                 warmEnergy = dev.warmenergy,
                                                 slaveID = heatPump.slaveid,
                                                 name = heatPump.name,
                                                 time = dev.time,
                                             }).OrderByDescending(m => m.time).Skip((index - 1) * page_size).Take(page_size);
                    }
                    else
                    {
                        if (index == 1)
                        {
                            totalCount = (from heatPump in db.devices
                                          join bd in db.hmbindings
                                          on heatPump.slaveid equals bd.bindingid
                                          join dev in db.heatmeterstatistics
                                          on bd.hmid equals dev.slaveid
                                          where heatPump.type ==5
                                          where (dev.time >= dtStart && dev.time <= dtEnd)
                                          where (heatPump.name.ToLower().Contains(searchkey.ToLower()))
                                          select heatPump).Count();
                        }
                        dynamicAttendance = (from heatPump in db.devices
                                             join bd in db.hmbindings
                                             on heatPump.slaveid equals bd.bindingid
                                             join dev in db.heatmeterstatistics
                                             on bd.hmid equals dev.slaveid
                                             where heatPump.type == 5
                                             where (dev.time >= dtStart && dev.time <= dtEnd)
                                             where (heatPump.name.ToLower().Contains(searchkey.ToLower()))
                                             select new WaterPumpParas
                                             {
                                                 coldEnergy = dev.coldenergy,
                                                 warmEnergy = dev.warmenergy,
                                                 slaveID = heatPump.slaveid,
                                                 name = heatPump.name,
                                                 time = dev.time,
                                             }).OrderByDescending(m => m.time).Skip((index - 1) * page_size).Take(page_size);

                    }
                    attendances = dynamicAttendance.ToList();
                    if (attendances == null || attendances.Count == 0)
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("获取水泵能耗信息失败", ex);
            }
            return attendances;
        }

        public List<WaterPumpParas> GetAllAttendanceInfos(string beginTime1, string endTime1, string searchkey, string slaveCode)
        {
            DateTime dtStart = DateTime.Parse(beginTime1);
            DateTime dtEnd = DateTime.Parse(endTime1);
            int slaveID = int.Parse(slaveCode);
            List<WaterPumpParas> attendances = new List<WaterPumpParas>();
            try
            {
                using (mlrmsEntities db = new mlrmsEntities())
                {
                    IQueryable<WaterPumpParas> dynamicAttendance = null;
                    if (slaveCode != "0")
                    {

                        dynamicAttendance = (from heatPump in db.devices
                                             join bd in db.hmbindings
                                             on heatPump.slaveid equals bd.bindingid
                                             join dev in db.heatmeterstatistics
                                             on bd.hmid equals dev.slaveid
                                             where heatPump.type == 5
                                             where (heatPump.slaveid == slaveID)
                                             where (dev.time >= dtStart && dev.time <= dtEnd)
                                             where (heatPump.name.ToLower().Contains(searchkey.ToLower()))
                                             select new WaterPumpParas
                                             {
                                                 coldEnergy = dev.coldenergy,
                                                 warmEnergy = dev.warmenergy,
                                                 slaveID = heatPump.slaveid,
                                                 name = heatPump.name,
                                                 time = dev.time,
                                             }).OrderByDescending(m => m.time);
                    }
                    else
                    {
                        dynamicAttendance = (from heatPump in db.devices
                                             join bd in db.hmbindings
                                             on heatPump.slaveid equals bd.bindingid
                                             join dev in db.heatmeterstatistics
                                             on bd.hmid equals dev.slaveid
                                             where heatPump.type == 5
                                             where (dev.time >= dtStart && dev.time <= dtEnd)
                                             where (heatPump.name.ToLower().Contains(searchkey.ToLower()))
                                             select new WaterPumpParas
                                             {
                                                 coldEnergy = dev.coldenergy,
                                                 warmEnergy = dev.warmenergy,
                                                 slaveID = heatPump.slaveid,
                                                 name = heatPump.name,
                                                 time = dev.time,
                                             }).OrderByDescending(m => m.time);

                    }
                    attendances = dynamicAttendance.ToList();
                    if (attendances == null || attendances.Count == 0)
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("获取水泵能耗记录失败", ex);
            }
            return attendances;
        }

        public FileResult Export(string beginTime1, string endTime1, string searchkey, string slaveid)
        {
            DateTime dt = DateTime.Now;
            if (string.IsNullOrEmpty(beginTime1) && string.IsNullOrEmpty(endTime1))
            {
                beginTime1 = dt.ToString("yyyy/MM/01 00:00");
                endTime1 = dt.ToString("yyyy/MM/dd 23:59");
            }
            if (string.IsNullOrEmpty(slaveid))
            {
                slaveid = "0";
            }
            byte[] bytes = new byte[1];
            List<WaterPumpParas> list = GetAllAttendanceInfos(beginTime1, endTime1, searchkey, slaveid);
            ITransferData transdata = TransferDataFactory.GetUtil(DataFileType.CSV);
            DataTable dataTable = new WaterPumpInfoGetter().GetTable(list);
            MemoryStream stream = transdata.GetStream(dataTable) as MemoryStream;
            stream.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(stream, new UTF8Encoding(true));
            string strContent = sr.ReadToEnd();
            Encoding ecd = new UTF8Encoding(true);
            bytes = ecd.GetBytes(strContent);
            byte[] bytesConvent = new byte[bytes.Length + 3];
            bytesConvent[0] = 0xEF;
            bytesConvent[1] = 0xBB;
            bytesConvent[2] = 0xBF;
            Array.Copy(bytes, 0, bytesConvent, 3, bytes.Length);
            return File(bytesConvent, "application/vnd.ms-excel", Server.UrlEncode(string.Format("水泵能耗信息{0}_{1}.csv", DateTime.Parse(beginTime1).ToString("yyyy-MM-dd-HH-mm-ss"), DateTime.Parse(endTime1).ToString("yyyy-MM-dd-HH-mm-ss"))));

        }
    }
}