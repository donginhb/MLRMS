using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollection.Redis
{
    class HashGetEngineData
    {
        public static EngineData GetData(int slaveId)
        {
            EngineData data = new EngineData();
            try
            {
                if (RedisHandle.HashExist<string>(slaveId.ToString(), DeviceHashName.ME_State))
                {
                    var state = Convert.ToInt32(RedisHandle.HashGet<string>(slaveId.ToString(), DeviceHashName.ME_State));
                    data.State = state;
                }

                if (RedisHandle.HashExist<string>(slaveId.ToString(), DeviceHashName.ME_Mode))
                {
                    var mode = Convert.ToInt32(RedisHandle.HashGet<string>(slaveId.ToString(), DeviceHashName.ME_Mode));
                    data.Mode = mode;
                }

                if (RedisHandle.HashExist<string>(slaveId.ToString(), DeviceHashName.ME_Temp))
                {
                    var temp = Convert.ToDouble(RedisHandle.HashGet<string>(slaveId.ToString(), DeviceHashName.ME_Temp));
                    data.Temp = temp;
                }
            }
            catch
            {
                data = null;
            }

            return data;
        }
    }

    class EngineData
    {
        public int Mode { set; get; }

        public int State { set; get; }

        public double Temp { set; get; }

        public EngineData()
        {
            Mode = 0;
            State = 0;
            Temp = 0.0;
        }
    }
}
