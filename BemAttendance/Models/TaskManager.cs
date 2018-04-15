using BEMAttendance;
using BEMAttendance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEM.Models
{
    /// <summary>
    /// 任务管理类，负责任务的缓存及返回
    /// </summary>
    public class TaskManager
    {
        List<ClientTask> _taskList = new List<ClientTask>();

        List<string> _listSyncConfigList = new List<string>();

        List<string> _packageUpdateList = new List<string>();

        List<string> _videoUpdateList = new List<string>();

        private TaskManager() { }

        private static TaskManager _taskQueueHelper = new TaskManager();

        private static readonly object _readWriteLock1 = new object();

        private static readonly object _readWriteLock2 = new object();

        private static readonly object _readWriteLock3 = new object();

        private static readonly object _readWriteLock4 = new object();

        public static void TaskEnqueue(ClientTask task)
        {
            lock(_readWriteLock1)
            {
                if(_taskQueueHelper._taskList.Where(m=>m.ClientID==task.ClientID).Count()==0)
                {
                    _taskQueueHelper._taskList.Add(task);
                }
            }
            BEMAttendance.Models.LogHelper.Info(string.Format("设备{0}更新任务入队列",task.ClientID));
        }

        public static  void SaveListConfig(string deviceID)
        {
            if(!_taskQueueHelper._listSyncConfigList.Contains(deviceID))
            {
                lock(_readWriteLock2)
                {
                    _taskQueueHelper._listSyncConfigList.Add(deviceID);
                } 
            }
        }

        public static void SavePackUpdate(string deviceID)
        {
            if(!_taskQueueHelper._packageUpdateList.Contains(deviceID))
            {
                lock(_readWriteLock3)
                {
                    _taskQueueHelper._packageUpdateList.Add(deviceID);
                }
            }
        }
        public static void SaveVideoUpdate(string deviceID)
        {
            if (!_taskQueueHelper._videoUpdateList.Contains(deviceID))
            {
                lock (_readWriteLock4)
                {
                    _taskQueueHelper._videoUpdateList.Add(deviceID);
                }
            }
        }
        /// <summary>
        /// 指定的设备是否有名单下发更新
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public static bool HasListUpdate(string clientID)
        {
            if(_taskQueueHelper._taskList.Count==0)
            {
                LogHelper.Info("名单更新列表为空");
            }
            else
            {
                foreach(var item in _taskQueueHelper._taskList)
                {
                    LogHelper.Info("分别为" + item.ClientID);
                }
            }
            lock(_readWriteLock1)
            {
               return  _taskQueueHelper._taskList.Where(e => e.ClientID == clientID).Count() > 0;
            }

        }
        /// <summary>
        /// 指定设备是否有配置更新
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public static bool HasConfigUpdate(string clientID)
        {
            return _taskQueueHelper._listSyncConfigList.Count(e => e == clientID) > 0;
        }

        public static bool HasPackUpdate(string clientID)
        {
            lock(_readWriteLock3)
            {
                return _taskQueueHelper._packageUpdateList.Count(e => e == clientID) > 0;
            }
        }
        public static bool HasVideoUpdate(string clientID)
        {
            lock(_readWriteLock4)
            {
                return _taskQueueHelper._videoUpdateList.Count(e => e == clientID) > 0;
            }
        }
        /// <summary>
        /// 获取指定设备的任务列表
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public static UpdatesJsonObj GetTasks(string clientID)
        {
            ClientTask task = _taskQueueHelper._taskList.Where(item => item.ClientID == clientID).First();
            if(task==null)
            {
                return null;
            }
            UpdatesJsonObj obj = new UpdatesJsonObj();
            obj.taskID = task.TaskID;
            using (BemEntities db = new BemEntities())
            {
                    var updates = from devRel in db.deviceemployeerelation
                                  join user in db.employee
                                  on devRel.EmpCode equals user.EmpCode into dic
                                  from item in dic.DefaultIfEmpty()
                                  where devRel.DevCode == clientID
                                  where devRel.Operate != 0
                                  group new { devRel, item } by devRel.Operate into g
                                  select new
                                  {
                                      g.Key,
                                      g
                                  };
                    obj.updatesSubItems = new List<UpdatesSubItem>();
                    foreach (var itm in updates)
                    {
                        UpdatesSubItem subItem = new UpdatesSubItem();
                        subItem.operateType = (sbyte)itm.Key;
                        subItem.employees = new List<EmployeeJsonObj>();
                        foreach (var emp in itm.g)
                        {
                            EmployeeJsonObj employee1 = new EmployeeJsonObj();
                        if (emp.item != null)
                        {
                            employee1.empCode = emp.item.EmpCode;
                            employee1.empID = emp.item.EmpIDCard;
                            employee1.empName = emp.item.EmpName;
                            employee1.empTelephone = emp.item.Telephone == null ? string.Empty : emp.item.Telephone;
                            employee1.empAdmin = emp.item.Administrator == null ? string.Empty : emp.item.Administrator;
                            employee1.empBirth = emp.item.EmpBirth.ToString("yyyy-MM-dd HH:mm:ss");
                            employee1.empGender = emp.item.EmpGender.HasValue ? (emp.item.EmpGender == 0 ? "女" : "男") : "女";
                            employee1.empGroupName = emp.item.EmpGroupName;
                            employee1.empGroupCode = emp.item.EmpGroupCode;
                            employee1.empLeftNum = emp.item.LeftNum.HasValue ? (int)emp.item.LeftNum : 0;
                            employee1.empRightNum = emp.item.RightNum.HasValue ? (int)emp.item.RightNum : 0;
                            employee1.empTemplate = emp.item.Template == null ? string.Empty : Convert.ToBase64String(emp.item.Template);
                            employee1.empLeftEyeImg = string.Empty;
                            employee1.empRightEyeImg = string.Empty;
                            employee1.empTemplateSize = emp.item.TemplateSize.HasValue ? (int)emp.item.TemplateSize : 0;
                            employee1.empFaceTemplate = emp.item.FaceTemplate == null ? string.Empty : Convert.ToBase64String(emp.item.FaceTemplate);
                            employee1.empFaceImg = string.Empty;
                        }
                        else
                        {
                            employee1.empCode = emp.devRel.EmpCode;
                            employee1.empID = string.Empty;
                            employee1.empName = string.Empty;
                            employee1.empTelephone = string.Empty;
                            employee1.empAdmin = string.Empty;
                            employee1.empBirth = string.Empty;
                            employee1.empGender = string.Empty;
                            employee1.empGroupName = string.Empty;
                            employee1.empGroupCode = string.Empty;
                            employee1.empLeftNum = 0;
                            employee1.empRightNum = 0;
                            employee1.empTemplate = string.Empty;
                            employee1.empLeftEyeImg = string.Empty;
                            employee1.empRightEyeImg = string.Empty;
                            employee1.empTemplateSize = 0;
                            employee1.empFaceTemplate = string.Empty;
                            employee1.empFaceImg = string.Empty;
                        }
                            subItem.employees.Add(employee1);
                        }
                        obj.updatesSubItems.Add(subItem);
                    }
            }
            return obj;
        }
        /// <summary>
        /// 任务下发完成后删除任务
        /// </summary>
        /// <param name="taskID"></param>
        public static void RemoveTasks(string id)
        {
            lock(_readWriteLock1)
            {
                if(_taskQueueHelper._taskList==null||_taskQueueHelper._taskList.Count==0)
                {
                    return;
                }
                _taskQueueHelper._taskList.RemoveAll(item => item.TaskID == id);
            }
        }
        /// <summary>
        /// 移除同步时间配置更新
        /// </summary>
        /// <param name="id"></param>
        public static void RemoveConfig(string id)
        {
            lock(_readWriteLock2)
            {
                _taskQueueHelper._listSyncConfigList.Remove(id);
            }
        }

        public static void RemoveUpdate(string id)
        {
            lock(_readWriteLock3)
            {
                _taskQueueHelper._packageUpdateList.Remove(id);
            }
        }
        public static void RemoveVideoUpdate(string id)
        {
            lock (_readWriteLock4)
            {
                _taskQueueHelper._videoUpdateList.Remove(id);
            }
        }
        public static bool CheckDeleteLegal(string clientID,string taskID)
        {
            if(_taskQueueHelper._taskList.Where(m=>m.ClientID==clientID&&m.TaskID==taskID).Count()>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckDeleteLegal(string clientID,out string taskID)
        {
            taskID = string.Empty;
            if (_taskQueueHelper._taskList.Where(m => m.ClientID == clientID).Count() > 0)
            {
                taskID = _taskQueueHelper._taskList.Where(m => m.ClientID == clientID).First().TaskID;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}