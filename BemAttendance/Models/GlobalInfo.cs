using BEMAttendance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEM.Models
{
    /// <summary>
    /// 存放全局信息，包括设备信息及任务更新列表等；
    /// </summary>
    public static class GlobalInfo
    {
    }
    public enum OPERATETYPE
    {
        NULL,
        ADD,
        EDIT,
        DEBIND,  //解除绑定，但是不删除
        DELETE,  //删除记录
    }
}