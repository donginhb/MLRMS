using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace BEMAttendance
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();
            //config.Routes.MapHttpRoute(name: "regRecord", routeTemplate: "bem/v1/reg-record", defaults: new { controller = "RegRecord" });
            //config.Routes.MapHttpRoute(name: "listSync", routeTemplate: "bem/v1/list-sync-time", defaults: new { controller = "ListSyncTime" });
            //config.Routes.MapHttpRoute(name: "timeSync", routeTemplate: "bem/v1/time-sync", defaults: new { controller = "TimeSync" });
            //config.Routes.MapHttpRoute(name: "regImgRecords", routeTemplate: "bem/v1/regImgRecords", defaults: new { controller = "RecordWithImage" });
            //config.Routes.MapHttpRoute(name: "keepLive", routeTemplate: "bem/v1/keeplive", defaults: new { controller = "KeepLive" });

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "bem/v1/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            //只支持JSON
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
        }
    }
}
