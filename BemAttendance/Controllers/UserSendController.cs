using BEM.Models;
using BEMAttendance.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ClientDevice = BEMAttendance.clientdevice;
using Employee = BEMAttendance.employee;
using DeviceEmployeeRelation = BEMAttendance.deviceemployeerelation;
namespace BEMAttendance.Controllers
{
    public class UserSendController : Controller
    {
        string LoginName;
        VisitorRole role;
        string department1Code = string.Empty;

        [System.Web.Mvc.HttpGet]
        public ActionResult Index()
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
            List<CompanyItem> unBindItemList = new List<CompanyItem>();
            List<CompanyItem> bindItemList = new List<CompanyItem>();
            List<ClientDevice> deviceList = new List<ClientDevice>();
            List<CompanyItem> bindResultList = new List<CompanyItem>();
            List<CompanyItem> unBindResultList = new List<CompanyItem>();
            try
            {
                using (BemEntities db = new BemEntities())
                {
                    deviceList = db.clientdevice.ToList();
                    ViewData["devList"] = deviceList;

                    //var companyList = from emp in db.employee
                    //                  join rel in db.deviceemployeerelation
                    //                  on emp.EmpCode equals rel.EmpCode into dc
                    //                  from dci in dc.DefaultIfEmpty()
                    //                  group dci by new { emp.EmpGroupCode, emp.EmpGroupName, emp.EmpCode, emp.EmpName, dci.DevCode } into g
                    //                  select new { grpName = g.Key.EmpGroupName, grpCode = g.Key.EmpGroupCode, empName = g.Key.EmpName, empCode = g.Key.EmpCode, devCode = g.Key.DevCode };
                    var unbindList = from item in (from emp in db.employee
                                                   join rel in db.deviceemployeerelation
                                                   on emp.EmpCode equals rel.EmpCode into dc
                                                   from dci in dc.DefaultIfEmpty()
                                                   where string.IsNullOrEmpty(dci.DevCode) == true
                                                   select new { emp.EmpCode, emp.EmpName, emp.EmpGroupCode, emp.EmpGroupName })
                                     group item by new { item.EmpGroupCode, item.EmpGroupName } into g
                                     select g;

                   var bindlist= from item in (from emp in db.employee
                                               join rel in db.deviceemployeerelation
                                               on emp.EmpCode equals rel.EmpCode into dc
                                               from dci in dc.DefaultIfEmpty()
                                               where string.IsNullOrEmpty(dci.DevCode) == false
                                               select new { emp.EmpCode, emp.EmpName, emp.EmpGroupCode, emp.EmpGroupName })
                                 group item by new { item.EmpGroupCode, item.EmpGroupName } into g
                                 select g;
                    foreach (var item in unbindList)
                    {
                        CompanyItem item1 = new CompanyItem();
                        item1.CompanyCode = item.Key.EmpGroupCode;
                        item1.CompanyName = item.Key.EmpGroupName;
                        item1.subList = new List<Employee>();
                        foreach(var subItem in item)
                        {
                            item1.subList.Add(new Employee() { EmpCode = subItem.EmpCode, EmpName = subItem.EmpName });
                        }
                        unBindItemList.Add(item1);
                    }
                    foreach (var item in bindlist)
                    {
                        CompanyItem item1 = new CompanyItem();
                        item1.CompanyCode = item.Key.EmpGroupCode;
                        item1.CompanyName = item.Key.EmpGroupName;
                        item1.subList = new List<Employee>();
                        foreach (var subItem in item)
                        {
                            if (item1.subList.Where(m => m.EmpCode == subItem.EmpCode).Count() <= 0)
                            {
                                item1.subList.Add(new Employee() { EmpCode = subItem.EmpCode, EmpName = subItem.EmpName });
                            }
                        }
                        bindItemList.Add(item1);
                    }
                    //foreach (var company in result)
                    //{
                    //    CompanyItem item1 = new CompanyItem();
                    //    item1.CompanyCode = company.Key.grpCode;
                    //    item1.CompanyName = company.Key.grpName;
                    //    item1.subList = new List<Employee>();

                    //    CompanyItem item2 = new CompanyItem();
                    //    item2.CompanyCode = company.Key.grpCode;
                    //    item2.CompanyName = company.Key.grpName;
                    //    item2.subList = new List<Employee>();
                    //    foreach (var emp in company)
                    //    {
                    //        if (string.IsNullOrEmpty(emp.devCode))
                    //        {
                    //            item1.subList.Add(new Employee() { EmpCode = emp.empCode, EmpName = emp.empName });
                    //        }
                    //        else
                    //        {
                    //            if (item2.subList.Where(m => m.EmpCode == emp.empCode).Count() <= 0)
                    //            {
                    //                item2.subList.Add(new Employee() { EmpCode = emp.empCode, EmpName = emp.empName });
                    //            }
                    //        }
                    //    }

                    //    unBindItemList.Add(item1);
                    //    bindItemList.Add(item2);
                    //}

                    foreach (var item in unBindItemList)
                    {
                        if (item.subList.Count > 0)
                        {
                            unBindResultList.Add(item);
                        }
                    }
                    foreach (var item in bindItemList)
                    {
                        if (item.subList.Count > 0)
                        {
                            bindResultList.Add(item);
                        }
                    }
                    ViewData["notBindList"] = unBindResultList;
                    ViewData["bindList"] = bindResultList;
                    return View();
                }
            }
            catch(Exception ex)
            {
                LogHelper.Error("设备下发获取部门信息失败", ex);
            }
            return View();
        }
        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> Index(string empData,string devData)
        {

            int count = 0;
            List<string> empCodes = new List<string>();
            List<string> devCodes = new List<string>();
            try
            {
                string[] splitEmp = empData.Split(',');
             
                for (int i=0;i< splitEmp.Length-1;i++)
                {
                    empCodes.Add(splitEmp[i].Split('=')[1]);
                }
                string[] spliteDev = devData.Split(',');
                for(int j=0;j<spliteDev.Length-1;j++)
                {
                    devCodes.Add(spliteDev[j].Split('=')[1]);
                }
                using (BemEntities db = new BemEntities())
                {
                    foreach (string dev in devCodes)
                    {
                        List<DeviceEmployeeRelation> relation = new List<DeviceEmployeeRelation>();
                        foreach (string emp in empCodes)
                        {
                            DeviceEmployeeRelation relationItem = new DeviceEmployeeRelation();
                            relationItem.DevCode = dev;
                            relationItem.EmpCode = emp;
                            relationItem.HasGet = 0;
                            relationItem.Operate = (int)OPERATETYPE.ADD;
                            relation.Add(relationItem);
                        }

                        db.deviceemployeerelation.AddRange(relation);
                        count += await db.SaveChangesAsync();
                    }
                }
            } 
            catch(Exception ex)
            {
                LogHelper.Error("设备下发失败", ex);
                return Content("设备下发失败", "text/html");
            }
            if(count<=0)
            {
                LogHelper.Info("设备下发失败count=" + count);
                return Content("设备下发失败", "text/html");
            }
            LogHelper.Info("设备下发成功时间:" + DateTime.Now);
            SendToClient(devCodes);
            return Content("OK", "text/html");
        }
        private void SendToClient(List<string> devs)
        {
            foreach(string devCode in devs)
            {
                ClientTask task = new ClientTask();
                task.ClientID = devCode;
                task.TaskID = Guid.NewGuid().ToString();
                TaskManager.TaskEnqueue(task);
            }
        }
    }
}