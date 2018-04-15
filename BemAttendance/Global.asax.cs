using BEM.Models;
using BEMAttendance.Models;
using BEMAttendance.Models.Extend;
using BEMAttendance.Models.Thrift;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BEMAttendance
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static ThriftClient _client;

        public static ThriftClient Client
        {
            get { return _client; }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //连接SDK
            try
            {
                ECProcess.ECIPaddress = ConfigurationManager.AppSettings["eyecloudServerIP"];
                ECProcess.ECPort = int.Parse(ConfigurationManager.AppSettings["eyecloudServerPort"]);
                FtpHelper.VideoFtpURL = ConfigurationManager.AppSettings["videoFtpBase"];
                FtpHelper.PackageFtpURL= ConfigurationManager.AppSettings["packageFtpBase"];
                FtpHelper.NeedUpToEC =bool.Parse(ConfigurationManager.AppSettings["needUpToEC"]);

                //替换默认的JSON序列化器
                HttpConfiguration fConfig = GlobalConfiguration.Configuration;
                fConfig.Formatters.Remove(fConfig.Formatters.JsonFormatter);
                fConfig.Formatters.Insert(0, new JilFormatter());
                //TokenHelper.InitStatus();  //状态初始化
                //GlobalInfo.InitTask();     //初始化还未下发的任务列表

                _client = new ThriftClient(); //初始化Thrift连接
                LogHelper.Info("网站启动:"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch(Exception ex)
            {
                LogHelper.Error("获取EyeCloud Server配置信息失败", ex);
            }
        
        }
    }
}
