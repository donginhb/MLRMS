using BEM.Models;
using BEMAttendance;
using BEMAttendance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BEM.Controllers
{
    [RoutePrefix("bem/v1/keeplive")]
    public class KeepLiveController : ApiController
    {
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetKeepLive(HttpRequestMessage request)
        {
            string deviceCode;
           
            if (TokenHelper.CheckAuth(request.Headers.Authorization.Parameter, out deviceCode) == false)
            {
                LogHelper.Info(string.Format("设备[{0}]鉴权失败："+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),deviceCode));
                return Content<ApiErrorInfo>(HttpStatusCode.Unauthorized, new ApiErrorInfo() { errcode = (int)ErrorCode.Unauthorized, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.Unauthorized) });
            }
            if(!TokenHelper.UpdateLiveTime(deviceCode, DateTime.Now))
            {
                LogHelper.Info("设备"+deviceCode+"保活失败：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                LogHelper.Info(string.Format("设备[{0}]设备保活失败", deviceCode));
                return Content<ApiErrorInfo>(HttpStatusCode.InternalServerError, new ApiErrorInfo(){ errcode = (int)ErrorCode.OverTime, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.OverTime) });
            }            
            //检查名单更新列表，和同步时间修改列表；
            bool list_Updated = false;
            bool syncTime_Updated = false;

            bool pack_Updated = false;

            bool video_Updated = false;
            //获取是否有名单下发更新
            list_Updated = TaskManager.HasListUpdate(deviceCode);
            //获取同步时间是否有更新
            syncTime_Updated = TaskManager.HasConfigUpdate(deviceCode);

            pack_Updated = TaskManager.HasPackUpdate(deviceCode);

            video_Updated = TaskManager.HasVideoUpdate(deviceCode);

            return Ok(new { haveListUpdate = list_Updated, haveSynctimeUpdate = syncTime_Updated, havePackUpdate = pack_Updated,haveMovUpdate=video_Updated });
        }
    }
}
