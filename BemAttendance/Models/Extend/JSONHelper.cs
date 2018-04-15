using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace BEMAttendance.Models.Extend
{
    public class JSONHelper
    {
        #region 对象类型序列化为json 字符
        /// <summary>
        /// 对象类型序列化为json 字符
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="jsonObject">待转换实体</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>string</returns>
        public static string ObjectToJson<T>(Object jsonObject, Encoding encoding)
        {
            string result = String.Empty;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                serializer.WriteObject(ms, jsonObject);
                result = encoding.GetString(ms.ToArray());
            }
            return result;
        }
        #endregion

        #region json字符反序列化为对象
        /// <summary>
        /// json字符反序列化为对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>T</returns>
        public static T JsonToObject<T>(string json, Encoding encoding)
        {
            T resultObject = default(T);
            try
            {
                resultObject = Activator.CreateInstance<T>();
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(encoding.GetBytes(json)))
                {
                    resultObject = (T)serializer.ReadObject(ms);
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            return resultObject;
        }
        #endregion
    }
}