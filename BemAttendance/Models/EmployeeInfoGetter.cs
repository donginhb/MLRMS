using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class EmployeeInfoGetter
    {
        public  DataTable GetTable(List<UserEx> list)
        {
            DataTable dt = new DataTable();
            for(int i=0;i<9;i++)
            {
                DataColumn dc = new DataColumn("col" + i);
                dt.Columns.Add(dc);
            }

            DataRow dr1 = dt.NewRow();
            dr1[0] = "员工编号";
            dr1[1] = "员工名称";
            dr1[2] = "部门名称";
            dr1[3] = "部门编号";
            dr1[4] = "身份证号";
            dr1[5] = "性别";
            dt.Rows.Add(dr1);
            try
            {
                foreach (var item in list)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = item.UserCard;
                    dr[1] = item.UserName;
                    dr[2] = item.Department1;
                    dr[3] = item.Department1Code;
                    dr[4] = item.IDCard;
                    dr[5] = item.Gender;
                    dt.Rows.Add(dr);
                }
            }
            catch
            {

            }
            return dt;
        }
    }
}