using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollection.Model
{
    public enum DeviceType
    {
        /// <summary>
        /// 主机
        /// </summary>
        MainEngine = 1,

        /// <summary>
        /// 热能表
        /// </summary>
        HeatMeter = 2,

        /// <summary>
        /// 水表
        /// </summary>
        WaterMeter = 3,

        /// <summary>
        /// 电表
        /// </summary>
        ElectricMeter = 4,
        /// <summary>
        /// 水泵
        /// </summary>
        WaterPump=5,
    }

    public enum DeviceSubType
    {
        /// <summary>
        /// 25P主机
        /// </summary>
        MainEngine_25P = 5,

        /// <summary>
        /// 50P主机
        /// </summary>
        MainEngine_50P = 6,
    }

    public enum MainEngineMode
    {
        /// <summary>
        /// 制冷模式
        /// </summary>
        ColdMode = 0,

        /// <summary>
        /// 制热模式
        /// </summary>
        WarmMode = 1
    }

    public enum MainEngineState
    {
        /// <summary>
        /// 开启模式
        /// </summary>
        Open = 1,

        /// <summary>
        /// 停止模式
        /// </summary>
        Close = 0,
        /// <summary>
        /// 通信异常
        /// </summary>
        Exception=2
    }
   public class EnumParser
    {
        public static string TypeParser(int type)
        {

            string str = string.Empty;
            switch(type)
            {
                case 1:
                    str = "热泵";
                    break;
                case 2:
                    str = "热能表";
                    break;
                case 3:
                    str = "水表";
                    break;
                case 4:
                    str = "电表";
                    break;
                case 5:
                    str = "水泵";
                    break;
                default:
                    str = "未知";
                    break;
            }
            return str;
        }
       public static string SubTypeParser(int type)
        {
            string str = string.Empty;
            switch (type)
            {
                case 5:
                    str = "25P";
                    break;
                case 6:
                    str = "50P";
                    break;
                default:
                    str = "未知";
                    break;
            }
            return str;
        }

    }
}
