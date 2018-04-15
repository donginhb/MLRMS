using BEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BEMAttendance.Models
{
    public class UpdatesHelper
    {
        BemEntities db = new BemEntities();
        /// <summary>
        /// 通知某个员工的所有设备，有员工信息更新
        /// </summary>
        /// <param name="empCode"></param>
        public void HasEmpUpdate(string empCode)
        {
            try
            {
                using (BemEntities db = new BemEntities())
                {
                    var devList = db.deviceemployeerelation.Where(m => m.EmpCode == empCode);
                    if (devList.Count() == 0)
                    {
                        return;
                    }
                    foreach (var dev in devList)
                    {
                        ClientTask task = new ClientTask();
                        task.ClientID = dev.DevCode;
                        task.TaskID = Guid.NewGuid().ToString();
                        TaskManager.TaskEnqueue(task);
                    }
                }    
            }
            catch (Exception ex)
            {
                LogHelper.Error("更新下发失败", ex);
            }

        }

        public bool UpdateRelation(string empCode,OPERATETYPE type)
        {
            try
            {
                List<deviceemployeerelation> devEmps = db.deviceemployeerelation.Where(m => m.EmpCode == empCode).ToList();
                foreach(var item in devEmps)
                {
                    item.Operate = (sbyte)type;
                    db.deviceemployeerelation.Attach(item);
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
               int result=db.SaveChanges();
                return result > 0;
            }
            catch(Exception ex)
            {
                LogHelper.Error("更新关系表失败", ex);
                return false;
            }
        }
        /// <summary>
        /// 从设备上移除对应员工
        /// </summary>
        /// <param name="empCode"></param>
        /// <returns></returns>
        public bool DeleteRelation(string empCode, OPERATETYPE type)
        {
            try
            {
                using (BemEntities db = new BemEntities())
                {
                    var devList = db.deviceemployeerelation.Where(m => m.EmpCode == empCode);
                    if (devList.Count() == 0)
                    {
                        return false;
                    }
                    var groups = devList.GroupBy(m => m.DevCode);
                    List<string> devCodes = new List<string>();
                    foreach (var dev in devList)
                    {
                        devCodes.Add(dev.DevCode);
                    }
                    int count = 0;
                    count = db.Database.ExecuteSqlCommand(string.Format("update DeviceEmployeeRelation set operate={0} where EmpCode='{1}';", (int)type, empCode));
                    if (count > 0)
                    {
                        foreach (string devCode in devCodes)
                        {
                            ClientTask task = new ClientTask();
                            task.ClientID = devCode;
                            task.TaskID = Guid.NewGuid().ToString();
                            TaskManager.TaskEnqueue(task);
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                LogHelper.Error("删除绑定关系失败", ex);
                return false;
            }

        }

        public bool DeleteRelationByDev(string devCode)
        {
            try
            {
                using (BemEntities db = new BemEntities())
                {
                    int count = 0;
                    count = db.Database.ExecuteSqlCommand(string.Format("update DeviceEmployeeRelation set operate={0} where DevCode='{1}';",(int)OPERATETYPE.DEBIND, devCode));
                    if (count > 0)
                    {
                        ClientTask task = new ClientTask();
                        task.ClientID = devCode;
                        task.TaskID = Guid.NewGuid().ToString();
                        TaskManager.TaskEnqueue(task);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                LogHelper.Error("删除绑定关系失败", ex);
                return false;
            }
        }

        public bool DeleteRelationByDev(string devCode,List<string> excepts)
        {
            StringBuilder condition = new StringBuilder();
            try
            {
                bool hasCondition = false;

    
                if(excepts!=null&&excepts.Count>0)
                {
                    hasCondition = true;
                    condition.Append("not in (");
                    int index = 0;
                    foreach (var itm in excepts)
                    {
                        if(index==0)
                        {
                            condition.Append(string.Format("'{0}'", itm));
                        }
                        else
                        {
                            condition.Append(string.Format(",'{0}'", itm));
                        }
                        index++;
                    }
                    condition.Append(")");
                }
                using (BemEntities db = new BemEntities())
                {
                    int count = 0;
                    if(hasCondition==false)
                    {
                        count = db.Database.ExecuteSqlCommand(string.Format("update DeviceEmployeeRelation set operate={0} where DevCode='{1}';", (int)OPERATETYPE.DEBIND, devCode));
                    }
                    else
                    {
                        count = db.Database.ExecuteSqlCommand(string.Format("update DeviceEmployeeRelation set operate={0} where DevCode='{1}' and EmpCode {2};", (int)OPERATETYPE.DEBIND, devCode, condition.ToString()));
                    }
                   
                    if (count > 0)
                    {
                        ClientTask task = new ClientTask();
                        task.ClientID = devCode;
                        task.TaskID = Guid.NewGuid().ToString();
                        TaskManager.TaskEnqueue(task);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                LogHelper.Error("删除绑定关系失败", ex);
                return false;
            }
        }

        public bool DeleteRelation(string empCode,string devCode)
        {
            try
            {
                using (BemEntities db = new BemEntities())
                {
                    int count = 0;
                    count = db.Database.ExecuteSqlCommand(string.Format("update DeviceEmployeeRelation set operate={0} where EmpCode='{1}' and DevCode='{2}';",(int)OPERATETYPE.DEBIND, empCode,devCode));
                    if (count > 0)
                    {
                        ClientTask task = new ClientTask();
                        task.ClientID = devCode;
                        task.TaskID =  Guid.NewGuid().ToString();
                        TaskManager.TaskEnqueue(task);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                LogHelper.Error("删除绑定关系失败", ex);
                return false;
            }
        }
    }
}