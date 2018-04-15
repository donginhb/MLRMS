using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class RoleHelper
    {
        mlrmsEntities db = new mlrmsEntities();
        public void GetRoles(string loginName, out VisitorRole role, out string dptCode,out string adminName)
        {
            role = VisitorRole.Guest;
            dptCode = string.Empty;
            adminName = string.Empty;
            try
            {
                sysadmin admin = db.sysadmin.Where(m => m.AdminCode == loginName).First();
                if(admin!=null)
                {
                    if (loginName == "admin")
                    {
                        role = VisitorRole.Admin;
                        adminName = "admin";
                    }
                    else
                    {
                        role = VisitorRole.SubAdmin;
                        adminName = admin.AdminName;
                        dptCode = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("获取账户信息出错", ex);
            }
        }
    }
    public enum VisitorRole
    {
        //超级管理员
        Admin,
        //辖行管理员
        SubAdmin,
        //普通访客
        Guest,
    }
}