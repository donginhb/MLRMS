using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using StackExchange.Redis;
using DataCollection.Model;
using BEMAttendance.Models.Extend;
using System.Text;

namespace DataCollection.Redis
{
    public class RedisHandle
    {
        private static ConnectionMultiplexer _redis = null;
        private static readonly string _constring = "127.0.0.1:6379";
        private static readonly object _locker = new object();

        private static ConnectionMultiplexer Manager()
        {
            if (_redis == null)
            {
                lock (_locker)
                {
                    if (_redis != null) return _redis;
                    _redis = GetManager();
                    return _redis;
                }
            }
            return _redis;
        }

        private static ConnectionMultiplexer GetManager()
        {
            return ConnectionMultiplexer.Connect(_constring);
        }

        ///////////////////   hash  ////////////////////////
        public static bool HashExist<T>(string hashId, string key) where T : class
        {
            var db = Manager().GetDatabase();
            return db.HashExists(hashId, key);
        }

        public static bool HashSet<T>(string hashId, string key, T t) where T : class
        {
            var db = Manager().GetDatabase();
            string value = JSONHelper.ObjectToJson<T>(t, Encoding.UTF8);
            return db.HashSet(hashId, key, value);
        }

        public static T HashGet<T>(string hashId, string key) where T : class
        {
            var db = Manager().GetDatabase();
            string value = db.HashGet(hashId, key);
            if (value == null) return null;
            return JSONHelper.JsonToObject<T>(value, Encoding.UTF8);
        }

        public static List<string> HashGetKeys(string hashid)
        {
            var db = Manager().GetDatabase();
            var keys = db.HashKeys(hashid);
            List<string> res = new List<string>();
            foreach(var k in keys)
            {
                res.Add(k.ToString());
            }
            return res;
        }

        public static RedisValue[] HashGetKeysEx(string hashid)
        {
            var db = Manager().GetDatabase();
            var keys = db.HashKeys(hashid);
            return keys;
        }

        public static bool HashDeleteKeysEx(string hashid, RedisValue[] keys)
        {
            var db = Manager().GetDatabase();
            db.HashDelete(hashid, keys);
            return true;
        }

        public static void HashIncrBy(string hashId, string key, int t)
        {
            var db = Manager().GetDatabase();
            db.HashIncrement(hashId, key, t);
        }

        public static Dictionary<string, T> HashGetAll<T>(string hashid) where T : class
        {
            var result = new Dictionary<string, T>();
            var db = Manager().GetDatabase();
            var entitis = db.HashGetAll(hashid);
            if(entitis != null && entitis.Count() > 0)
            {
                foreach(var e in entitis)
                {
                    var value = JSONHelper.JsonToObject<T>(e.Value,Encoding.UTF8);
                    result[e.Name] = value;
                }
            }
            return result;
        }
        /////////////////////  list /////////////////////////
        public static void ListPrepend<T>(string key, T t) where T : class
        {
            var value = JSONHelper.ObjectToJson<T>(t,Encoding.UTF8);
            var db = Manager().GetDatabase();
            db.ListLeftPush(key, value);
        }
        public static void ListRightPush<T>(string key, T t) where T : class
        {
            var value = JSONHelper.ObjectToJson<T>(t,Encoding.UTF8);
            var db = Manager().GetDatabase();
            db.ListRightPush(key, value);
        }
        public static void ListLeftPop(string key)
        {
            var db = Manager().GetDatabase();
            db.ListLeftPop(key);
        }
        public static void ListRightPop(string key)
        {
            var db = Manager().GetDatabase();
            db.ListRightPop(key);
        }
        public static long ListCount(string key)
        {
            var db = Manager().GetDatabase();
            return db.ListLength(key);
        }
        public static List<T> ListGetAll<T>(string key) where T : class
        {
            var result = new List<T>();
           
            var db = Manager().GetDatabase();
            var entitis = db.ListRange(key, 0, ListCount(key));
            if (entitis != null && entitis.Count() > 0)
            {
                foreach(var e in entitis)
                {
                    var value = JSONHelper.JsonToObject<T>(e,Encoding.UTF8);
                    result.Add(value);
                }
            }
            return result;
        }
    }
}