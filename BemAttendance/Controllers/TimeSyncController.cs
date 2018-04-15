using BEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BEM.Controllers
{
    [RoutePrefix("bem/v1/time-sync")]
    public class TimeSyncController : ApiController
    {
        /// <summary>
        /// 时间同步
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetTimeSync(HttpRequestMessage request)
        {
           string deviceCode;

            if (request.Headers.Authorization == null || TokenHelper.CheckAuth(request.Headers.Authorization.Parameter, out deviceCode) == false)
            {
                return Content<ApiErrorInfo>(HttpStatusCode.Unauthorized, new ApiErrorInfo() { errcode = (int)ErrorCode.Unauthorized, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.Unauthorized) });
            }
            //检查超时
            if (!TokenHelper.IsTokenOnline(deviceCode, DateTime.Now))
            {
                return Content<ApiErrorInfo>(HttpStatusCode.InternalServerError, new ApiErrorInfo() { errcode = (int)ErrorCode.OverTime, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.OverTime) });
            }
            return Ok(new { serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
        }
    }
}
