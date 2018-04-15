using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace BEMAttendance.Models
{
    public class LogHelper
    {
        private static readonly ILog _loger = LogManager.GetLogger("logInfo");

        public static void Warn(string message)
        {
            _loger.Warn(message);
        }
        public static void Warn(string message, Exception exception)
        {
            _loger.Warn(message, exception);
        }
        public static void Info(string message)
        {
            _loger.Info(message);
        }
        public static void Info(string message, Exception exception)
        {
            _loger.Info(message, exception);
        }
        public static void Error(string message)
        {
            _loger.Error(message);
        }
        public static void Error(string message, Exception exception)
        {
            _loger.Error(message, exception);
        }
        public static void Fatal(string message)
        {
            _loger.Fatal(message);
        }
        public static void Fatal(string message, Exception exception)
        {
            _loger.Fatal(message, exception);
        }
    }
}