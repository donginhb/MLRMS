using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.IdentityModel.Tokens;
using System.IdentityModel.Protocols.WSTrust;
using BEMAttendance;
using BEMAttendance.Models;
using ClientDevice = BEMAttendance.clientdevice;
using System.Collections.Concurrent;

namespace BEM.Models
{
    public class TokenHelper
    {
        private const string symmetricKey = "cXdlcnR5dWlvcGFzZGZnaGprbHp4Y3Zibm0xMjM0NTY=";
        const int TIMEOUT = 120;
        //设备和Token的对应列表，键值设备编号，值为Token
        private static ConcurrentDictionary<string, string> DeviceTokenList = new ConcurrentDictionary<string, string>();
        //设备和保活时间的对应表，键值为设备编号,值为时间
        private static ConcurrentDictionary<string, DateTime> DeviceKeepLiveTimeList = new ConcurrentDictionary<string, DateTime>();
        /// <summary>
        /// 创建Token值
        /// </summary>
        /// <param name="id"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static string GenerateJWTToken(string id,string role)
        {
            var key = Convert.FromBase64String(symmetricKey);
            var credentials = new SigningCredentials(
                new InMemorySymmetricSecurityKey(key),
                "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
                "http://www.w3.org/2001/04/xmlenc#sha256");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, id),
                    new Claim(ClaimTypes.Role, role)
                }),
                TokenIssuerName = "corp",
                AppliesToAddress = "http://www.example.com",
                SigningCredentials = credentials,
                Lifetime = new Lifetime(DateTime.UtcNow, DateTime.UtcNow.AddYears(10))
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        private static bool IsDeviceExist(string id)
        {
            return DeviceTokenList.ContainsKey(id);
        }
        /// <summary>
        /// 通过Token值来获取对应的设备编号
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GetDeviceCodeFromToken(string token)
        {
            string deviceCode = string.Empty;  
            try
            {
                deviceCode = DeviceTokenList.Where(m => m.Value == token).First().Key;
            }
            catch
            {
                LogHelper.Error("未找到该token对应的设备编号");
            }
            return deviceCode;
        }

        public static void SaveToken(string deviceCode,string token,DateTime dateTime)
        {
                if (IsDeviceExist(deviceCode))
                {
                    DeviceTokenList[deviceCode] = token;
                    DeviceKeepLiveTimeList[deviceCode] = dateTime;
                    return;
                }
                   DeviceTokenList.TryAdd(deviceCode, token);
                   DeviceKeepLiveTimeList.TryAdd(deviceCode, dateTime);
        }
        /// <summary>
        /// 检查授权
        /// </summary>
        /// <param name="token"></param>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        public static bool CheckAuth(string token,out string deviceCode)
        {
             deviceCode = string.Empty;
             if (string.IsNullOrEmpty(token))
             {
                LogHelper.Error("上传token为空");
                 return false;
             }
             deviceCode = GetDeviceCodeFromToken(token);
             if (string.IsNullOrEmpty(deviceCode))
             {
                LogHelper.Error("通过Token获取设备编号为空");
                return false;
             }
             return true;
        }
        //初始化
        public static void InitStatus()
        {
           try
            {
                List<ClientDevice> devList = new List<ClientDevice>();
                using (BemEntities db = new BemEntities())
                {
                     devList = db.clientdevice.OrderByDescending(m => m.DevCreateTime).ToList();
                    if(devList==null||devList.Count==0)
                    {
                        LogHelper.Info("InitStatus 获取设备初始状态为空");
                        return;
                    }
                }
                string token;
                DateTime dt = DateTime.MinValue;
                foreach(var item in devList)
                {
                    if(string.IsNullOrEmpty(item.DevToken))
                    {
                        continue;
                    }
                    else
                    {
                        token = item.DevToken;
                    }
                    dt = item.DevUpdateTime.HasValue ? (DateTime)item.DevUpdateTime : dt;
                    DeviceTokenList.TryAdd(item.DevCode, token);
                    DeviceKeepLiveTimeList.TryAdd(item.DevCode, dt);
                }
            }
            catch(Exception ex)
            {
                LogHelper.Error("状态初始化失败", ex);
            }
        }

        public static void CheckAlive()
        {
             DateTime dt = DateTime.Now;
             ConcurrentDictionary<string, DateTime> removeKeepliveList = new ConcurrentDictionary<string, DateTime>();

                foreach (KeyValuePair<string, DateTime> item in DeviceKeepLiveTimeList)
                {
                    if ((dt - item.Value).TotalSeconds > TIMEOUT)                    //如果离线了
                    {
                        if(ChangeStatus(item.Key, 0, DateTime.MinValue))
                        {
                            removeKeepliveList.TryAdd(item.Key,item.Value);
                            string outStr = string.Empty;
                            DeviceTokenList.TryRemove(item.Key, out outStr);
                        }
                    }
                }
                foreach(KeyValuePair<string, DateTime> item in removeKeepliveList)
                {
                    DateTime str;
                    DeviceKeepLiveTimeList.TryRemove(item.Key, out str);
                }
        }
        private static bool ChangeStatus(string devCode,sbyte online,DateTime dt)
        {
                try
                {
                     int result = 0;
                    using (BemEntities db = new BemEntities())
                    {
                        if(online==1)
                        {
                          result= db.Database.ExecuteSqlCommand(string.Format("update ClientDevice SET DevStatus={0},DevUpdateTime='{1}' WHERE DevCode='{2}';", online,dt.ToString("yyyy-MM-dd HH:mm:ss") ,devCode));
                        }
                       else
                       {
                         result= db.Database.ExecuteSqlCommand(string.Format("update ClientDevice SET DevStatus={0} WHERE DevCode='{1}';", online, devCode));
                        }
                     
                        return result>0;
                    }
                }
                catch(Exception ex) {
                    LogHelper.Error("["+devCode+"]设备状态更新数据库失败", ex);
                    return false;
                }
        }
        public static void RemoveKeepLive(string devCode)
        {
            DateTime outDt;
            string str;
            DeviceKeepLiveTimeList.TryRemove(devCode, out outDt);       //保活列表中移除
            DeviceTokenList.TryRemove(devCode, out str);
        }
        public static bool UpdateLiveTime(string deviceCode,DateTime time)
        {
                DateTime dt = DateTime.MinValue;
                string str;
                DateTime outDt;

                foreach (KeyValuePair<string, DateTime> item in DeviceKeepLiveTimeList)
                {
                    if (item.Key == deviceCode)
                    {
                        dt = item.Value;
                        break;
                    }
                }
                if (dt != DateTime.MinValue)
                {
                    if ((time - dt).TotalSeconds > TIMEOUT)
                    {
                        if(ChangeStatus(deviceCode, 0, DateTime.MinValue))  //离线
                        {
                            DeviceKeepLiveTimeList.TryRemove(deviceCode,out outDt);       //保活列表中移除
                            DeviceTokenList.TryRemove(deviceCode,out str);
                        }
                        return false;
                    }
                    else
                    {
                        DeviceKeepLiveTimeList[deviceCode] = time;
                        try
                        {
                            ChangeStatus(deviceCode, 1, time);
                        }
                        catch { }
                        return true;
                    }
                }
            return false;
        }

        public static bool IsTokenOnline(string deviceCode, DateTime time)
        {
            DateTime dt = DateTime.MinValue;
            foreach (KeyValuePair<string, DateTime> item in DeviceKeepLiveTimeList)
            {
                if (item.Key == deviceCode)
                {
                    dt = item.Value;
                    break;
                }
            }
            if (dt != DateTime.MinValue)
            {
                if ((time - dt).TotalSeconds > TIMEOUT)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}