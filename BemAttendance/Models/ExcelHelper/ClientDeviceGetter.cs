using BEMAttendance;
using BEMAttendance.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class ClientDeviceGetter
    {
        public DataTable GetTable(List<clientdevice> list)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < 5; i++)
            {
                DataColumn dc = new DataColumn("col" + i);
                dt.Columns.Add(dc);
            }
            try
            {
                foreach (var item in list)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = item.DevCode;
                    dr[1] = item.DevName;
                    dr[2] = item.DevIP;
                    dr[3] = item.DevAddress;
                    dr[4] = item.DevManager;
                    dt.Rows.Add(dr);
                }
            }
            catch(Exception ex)
            {
                LogHelper.Error("导出设备出错", ex);
            }
            return dt;
        }
    }
}