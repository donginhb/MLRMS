using BEMAttendance.Models.Params;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class HeatPumpInfoGetter
    {
        public DataTable GetTable(List<HeatPumpParas> list)
        {
            DataTable dt = new DataTable();
            for(int i=0;i<6;i++)
            {
                DataColumn dc = new DataColumn("col" + i);
                dt.Columns.Add(dc);
            }
            DataRow dr1 = dt.NewRow();
            dr1[0] = "设备地址";
            dr1[1] = "设备名称";
            dr1[2] = "设备型号";
            dr1[3] = "采集时间";
            dr1[4] = "累计热量";
            dr1[5] = "累计冷量";
            dt.Rows.Add(dr1);
            try
            {
                foreach(var item in list)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = item.slaveID;
                    dr[1] = item.name;
                    dr[2] = item.subTypeName;
                    dr[3] = item.time;
                    dr[4] = item.warmEnergy;
                    dr[5] = item.coldEnergy;
                    dt.Rows.Add(dr);
                }
            }
            catch { }
            return dt;
        }
    }

    public class WaterSupplyInfoGetter
    {
        public DataTable GetTable(List<WaterMeterParas> list)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < 5; i++)
            {
                DataColumn dc = new DataColumn("col" + i);
                dt.Columns.Add(dc);
            }
            DataRow dr1 = dt.NewRow();
            dr1[0] = "设备地址";
            dr1[1] = "设备名称";
            dr1[2] = "设备型号";
            dr1[3] = "采集时间";
            dr1[4] = "用水量";
            dt.Rows.Add(dr1);
            try
            {
                foreach (var item in list)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = item.slaveID;
                    dr[1] = item.name;
                    dr[2] ="水表";
                    dr[3] = item.time;
                    dr[4] = item.waterPara;
                    dt.Rows.Add(dr);
                }
            }
            catch { }
            return dt;
        }
    }

    public class ElectricInfoGetter
    {
        public DataTable GetTable(List<ElectricParas> list)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < 5; i++)
            {
                DataColumn dc = new DataColumn("col" + i);
                dt.Columns.Add(dc);
            }
            DataRow dr1 = dt.NewRow();
            dr1[0] = "设备地址";
            dr1[1] = "设备名称";
            dr1[2] = "设备型号";
            dr1[3] = "采集时间";
            dr1[4] = "用电量";
            dt.Rows.Add(dr1);
            try
            {
                foreach (var item in list)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = item.slaveID;
                    dr[1] = item.name;
                    dr[2] = "电表";
                    dr[3] = item.time;
                    dr[4] = item.electricAmount;
                    dt.Rows.Add(dr);
                }
            }
            catch { }
            return dt;
        }
    }

    public class WaterPumpInfoGetter
    {
        public DataTable GetTable(List<WaterPumpParas> list)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < 6; i++)
            {
                DataColumn dc = new DataColumn("col" + i);
                dt.Columns.Add(dc);
            }
            DataRow dr1 = dt.NewRow();
            dr1[0] = "设备地址";
            dr1[1] = "设备名称";
            dr1[2] = "设备型号";
            dr1[3] = "采集时间";
            dr1[4] = "累计热量";
            dr1[5] = "累计冷量";
            dt.Rows.Add(dr1);
            try
            {
                foreach (var item in list)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = item.slaveID;
                    dr[1] = item.name;
                    dr[2] = "水泵";
                    dr[3] = item.time;
                    dr[4] = item.warmEnergy;
                    dr[5] = item.coldEnergy;
                    dt.Rows.Add(dr);
                }
            }
            catch { }
            return dt;
        }
    }
}