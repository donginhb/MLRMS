using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BEM.Models;
using BEMAttendance;
using Employee = BEMAttendance.employee;
namespace BEM.Models
{
    public class ClientTask
    {
        /// <summary>
        /// 任务ID，用于任务下发时产生，任务下发完毕后，通过该任务ID，把列表中的任务清空；
        /// </summary>
        public string TaskID { get; set; }
        /// <summary>
        /// 任务下发的目标设备ID
        /// </summary>
        public string ClientID { get; set; }
        ///// <summary>
        ///// 下发的名单信息
        ///// </summary>
        //public List<Employee> Employees { get; set; }
        ///// <summary>
        ///// 任务下发类型
        ///// </summary>
        //public TaskOperate OperateType { get; set; }
        ///// <summary>
        ///// 任务创建的时间
        ///// </summary>
        //public DateTime CreateTime { get; set; }
    }
    public enum TaskOperate
    {
        ADD,
        UPDATE,
        DELETE
    }
}