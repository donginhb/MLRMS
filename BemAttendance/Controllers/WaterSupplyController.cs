﻿using BEMAttendance.Models;
using BEMAttendance.Models.Params;
using DataCollection.Model;
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
    public class WaterSupplyController : Controller
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
            List<WaterMeterParas> attendanceResults = new List<WaterMeterParas>();
            try
            {
                SelectList dptSelectlist1 = null;
                using (mlrmsEntities db = new mlrmsEntities())
                {
                    var dptList = db.device.Where(m => m.type == 3).Select(m => new { slaveid = m.slaveid, devname = m.name }).ToList();
                    dptList.Insert(0, new { slaveid = 0, devname = "全部" });
                    int waterCode= int.Parse(dptName1List);
                    dptSelectlist1 = new SelectList(dptList, "slaveid", "devname", waterCode);
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
                LogHelper.Error("水表记录读取异常", ex);
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
        public List<WaterMeterParas> GetPageAttendanceInfos(int index, string beginTime1, string endTime1, string searchkey, string slaveCode)
        {
            DateTime dtStart = DateTime.Parse(beginTime1);
            DateTime dtEnd = DateTime.Parse(endTime1);
            int slaveID = int.Parse(slaveCode);
            List<WaterMeterParas> attendances = new List<WaterMeterParas>();
            try
            {
                using (mlrmsEntities db = new mlrmsEntities())
                {
                    IQueryable<WaterMeterParas> dynamicAttendance = null;
                    if (slaveCode != "0")
                    {
                        if (index == 1)
                        {
                            totalCount = (from dev in db.device
                                          join water in db.watermeterstatistics
                                          on dev.slaveid equals water.slaveid
                                          where dev.type == 3
                                          where (dev.slaveid == slaveID)
                                          where (water.time >= dtStart && water.time <= dtEnd)
                                          //where (dev.name.ToLower().Contains(searchkey.ToLower()))
                                          select dev).Count();
                        }
                        dynamicAttendance = (from dev in db.device
                                             join water in db.watermeterstatistics
                                             on dev.slaveid equals water.slaveid
                                             where dev.type == 3
                                             where (dev.slaveid == slaveID)
                                             where (water.time >= dtStart && water.time <= dtEnd)
                                             //where (dev.name.ToLower().Contains(searchkey.ToLower()))
                                             select new WaterMeterParas
                                             {
                                                 waterPara = water.wateramount,
                                                 slaveID = water.slaveid,
                                                 name = dev.name,
                                                 time = water.time,
                                             }).OrderByDescending(m => m.time).Skip((index - 1) * page_size).Take(page_size);
                    }
                    else
                    {
                        if (index == 1)
                        {
                            totalCount = (from dev in db.device
                                          join water in db.watermeterstatistics
                                          on dev.slaveid equals water.slaveid
                                          where dev.type == 3
                                          where (water.time >= dtStart && water.time <= dtEnd)
                                          where (dev.name.ToLower().Contains(searchkey.ToLower()))
                                          select dev).Count();
                        }
                        dynamicAttendance = (from dev in db.device
                                             join water in db.watermeterstatistics
                                             on dev.slaveid equals water.slaveid
                                             where dev.type == 3
                                             where (water.time >= dtStart && water.time <= dtEnd)
                                             where (dev.name.ToLower().Contains(searchkey.ToLower()))
                                             select new WaterMeterParas
                                             {
                                                 waterPara = water.wateramount,
                                                 slaveID = water.slaveid,
                                                 name = dev.name,
                                                 time = water.time,
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
                LogHelper.Error("获取用水量信息失败", ex);
            }
            return attendances;
        }

        public List<WaterMeterParas> GetAllAttendanceInfos(string beginTime1, string endTime1, string searchkey, string slaveCode)
        {
            DateTime dtStart = DateTime.Parse(beginTime1);
            DateTime dtEnd = DateTime.Parse(endTime1);
            if(string.IsNullOrEmpty(slaveCode))
            {
                slaveCode = "0";
            }
            int slaveID = int.Parse(slaveCode);
            List<WaterMeterParas> attendances = new List<WaterMeterParas>();
            try
            {
                using (mlrmsEntities db = new mlrmsEntities())
                {
                    IQueryable<WaterMeterParas> dynamicAttendance = null;
                    if (slaveCode != "0")
                    {

                        dynamicAttendance = (from dev in db.device
                                             join water in db.watermeterstatistics
                                             on dev.slaveid equals water.slaveid
                                             where dev.type == 3
                                             where (dev.slaveid == slaveID)
                                             where (water.time >= dtStart && water.time <= dtEnd)
                                             //where (dev.name.ToLower().Contains(searchkey.ToLower()))
                                             select new WaterMeterParas
                                             {
                                                 waterPara =water.wateramount,
                                                 slaveID = water.slaveid,
                                                 name = dev.name,
                                                 time = water.time,
                                             }).OrderByDescending(m => m.time);
                    }
                    else
                    {
                        dynamicAttendance = (from dev in db.device
                                             join water in db.watermeterstatistics
                                             on dev.slaveid equals water.slaveid
                                             where dev.type == 3
                                             where (water.time >= dtStart && water.time <= dtEnd)
                                             //where (dev.name.ToLower().Contains(searchkey.ToLower()))
                                             select new WaterMeterParas
                                             {
                                                 waterPara = water.wateramount,
                                                 slaveID = water.slaveid,
                                                 name = dev.name,
                                                 time = water.time,
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
                LogHelper.Error("获取用水量信息失败", ex);
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
            List<WaterMeterParas> list = GetAllAttendanceInfos(beginTime1, endTime1, searchkey, slaveid);
            ITransferData transdata = TransferDataFactory.GetUtil(DataFileType.CSV);
            DataTable dataTable = new WaterSupplyInfoGetter().GetTable(list);
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
            return File(bytesConvent, "application/vnd.ms-excel", Server.UrlEncode(string.Format("用水量信息{0}_{1}.csv", DateTime.Parse(beginTime1).ToString("yyyy-MM-dd-HH-mm-ss"), DateTime.Parse(endTime1).ToString("yyyy-MM-dd-HH-mm-ss"))));

        }
    }
}