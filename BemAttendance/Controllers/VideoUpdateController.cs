using BEM.Models;
using BEMAttendance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BEMAttendance.Controllers
{
    [RoutePrefix("bem/v1/videoupdate")]
    public class VideoUpdateController : ApiController
    {
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetVideo(HttpRequestMessage request)
        {
            string deviceCode = string.Empty;
            ////检查授权
            if (request.Headers.Authorization == null || TokenHelper.CheckAuth(request.Headers.Authorization.Parameter, out deviceCode) == false)
            {
                return Content<ApiErrorInfo>(HttpStatusCode.Unauthorized, new ApiErrorInfo() { errcode = (int)ErrorCode.Unauthorized, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.Unauthorized) });
            }
            //检查超时
            if (!TokenHelper.IsTokenOnline(deviceCode, DateTime.Now))
            {
                return Content<ApiErrorInfo>(HttpStatusCode.InternalServerError, new ApiErrorInfo() { errcode = (int)ErrorCode.OverTime, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.OverTime) });
            }
            softwareupdate sf = null;
            using (BemEntities db = new BemEntities())
            {
                if(db.softwareupdate.Where(m=>m.type==1).Count()>0)
                {
                    sf = db.softwareupdate.Where(m => m.type == 1).OrderByDescending(m => m.uploadTime).First();
                }
            }
            if(sf==null)
            {
                return Content<ApiErrorInfo>(HttpStatusCode.NotFound, new ApiErrorInfo() { errcode = (int)ErrorCode.ObjectNotFound, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.ObjectNotFound) });
            }
            else
            {
                string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content");
                string packPath = filePath + "\\video\\" + sf.version;

                if (!System.IO.File.Exists(packPath))
                {
                    return Content<ApiErrorInfo>(HttpStatusCode.NotFound, new ApiErrorInfo() { errcode = (int)ErrorCode.ObjectNotFound, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.ObjectNotFound) });
                }
                string path = string.Format(@"{0}/{1}", FtpHelper.VideoFtpURL, sf.version);
                FtpPath vf = new FtpPath();
                vf.FtpURL = path;
                return Ok(vf);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetPackage(HttpRequestMessage request, string id)
        {
            string deviceCode;
            if (request.Headers.Authorization == null || TokenHelper.CheckAuth(request.Headers.Authorization.Parameter, out deviceCode) == false)
            {
                return Content<ApiErrorInfo>(HttpStatusCode.Unauthorized, new ApiErrorInfo() { errcode = (int)ErrorCode.Unauthorized, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.Unauthorized) });
            }
            try
            {
                TaskManager.RemoveVideoUpdate(id);
                return Ok();
            }
            catch (Exception ex)
            {
                LogHelper.Error("视频包更新下载确认失败", ex);
                return Content<ApiErrorInfo>(HttpStatusCode.InternalServerError, new ApiErrorInfo() { errcode = (int)ErrorCode.ServerException, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.ServerException) });
            }
        }
    }
    public class FtpPath
    {
        public string FtpURL { get; set; }
    }
}
