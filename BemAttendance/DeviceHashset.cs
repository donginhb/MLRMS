using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollection.Redis
{
    class DeviceHashName
    {
        //static public bool Init()
        //{
        //    foreach(var pair in DeviceFactory.DeviceList)
        //    {
        //        IDevice dev = pair.Value;

        //        RedisHandle.HashSet<string>(dev.SlaveId.ToString(), Type, ((int)dev.Type).ToString());
        //    }
        //    return true;
        //}

        //public static bool DeleteDev(string hashid)
        //{
        //    try
        //    {
        //        var fields = RedisHandle.HashGetKeysEx(hashid);
        //        if (fields != null)
        //        {
        //            RedisHandle.HashDeleteKeysEx(hashid, fields);
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
           
        //    return true;
        //}

        static public string Type = "Type";

        static public string WM_Quatity = "WM_Quatity";
        static public string EM_Quatity = "EM_Quatity";

        static public string HM_ColdQuatity = "HM_ColdQuatity";
        static public string HM_WarmQuatity = "HM_WarmQuatity";

        static public string ME_State = "State";
        static public string ME_Mode = "Mode";
        static public string ME_Temp = "Temp";
        static public string ME_Error = "Error";
        static public string ME_ColdQuatity = "ME_ColdQuatity";
        static public string ME_WarmQuatity = "ME_WarmQuatity";
    }
}
