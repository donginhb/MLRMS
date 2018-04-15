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
    [RoutePrefix("bem/v1/list-sync-time")]
    public class ListSyncTimeController : ApiController
    {
        //BemEntities db = new BemEntities();
        [AllowAnonymous]
        public IHttpActionResult GetSyncConfigTime(HttpRequestMessage request)
        {
            string deviceCode;
            if (TokenHelper.CheckAuth(request.Headers.Authorization.Parameter, out deviceCode) == false)
            {
                return Content<ApiErrorInfo>(HttpStatusCode.Unauthorized, new ApiErrorInfo() { errcode = (int)ErrorCode.Unauthorized, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.Unauthorized) });
            }
            try
            {
                using (BemEntities db = new BemEntities())
                {
                    var config = db.syncconfig.Find(deviceCode);
                    return Ok(new { synctimeStart = config.SyncBegin, synctimeEnd = config.SyncEnd });
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("获取名单同步配置失败", ex);
                return Content<ApiErrorInfo>(HttpStatusCode.InternalServerError, new ApiErrorInfo() { errcode = (int)ErrorCode.SyncConfigFail, errmsg = "同步时间获取失败" });
            }
        }

    }
}
