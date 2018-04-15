using BEM.Models;
using BEMAttendance.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
namespace BEMAttendance.Controllers
{
    [RoutePrefix("bem/v1/package")]
    public class PackageController : ApiController
    {
        //public IHttpActionResult GetPackage(HttpRequestMessage request)
        //{
        //    string deviceCode = string.Empty;
        //    if (request.Headers.Authorization == null || TokenHelper.CheckAuth(request.Headers.Authorization.Parameter, out deviceCode) == false)
        //    {
        //        return Content<ApiErrorInfo>(HttpStatusCode.Unauthorized, new ApiErrorInfo() { errcode = (int)ErrorCode.Unauthorized, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.Unauthorized) });
        //    }
        //    //检查超时
        //    if (!TokenHelper.IsTokenOnline(deviceCode, DateTime.Now))
        //    {
        //        return Content<ApiErrorInfo>(HttpStatusCode.InternalServerError, new ApiErrorInfo() { errcode = (int)ErrorCode.OverTime, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.OverTime) });
        //    }
        //    try
        //    {
        //        string filePath= System.Web.Hosting.HostingEnvironment.MapPath("~/Content");
        //        string packPath = filePath + "\\package";
        //        if(!System.IO.Directory.Exists(packPath))
        //        {
        //           return Content<ApiErrorInfo>(HttpStatusCode.NotFound, new ApiErrorInfo() { errcode = (int)ErrorCode.ObjectNotFound, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.ObjectNotFound) });
        //        }
        //        DirectoryInfo info = new System.IO.DirectoryInfo(packPath);
        //       if(info.GetFiles().Where(m=>System.IO.Path.GetExtension(m.Name).ToLower()==".apk").Count()==0)
        //        {
        //            return Content<ApiErrorInfo>(HttpStatusCode.NotFound, new ApiErrorInfo() { errcode = (int)ErrorCode.ObjectNotFound, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.ObjectNotFound) });
        //        }
        //        FileInfo fi = info.GetFiles().Where(m => System.IO.Path.GetExtension(m.Name).ToLower() == ".apk").First();
        //        byte[] bytes = null;
        //        using (FileStream fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //        {
        //            bytes = new byte[fs.Length];
        //            fs.Read(bytes, 0, bytes.Length);
        //            fs.Flush();
        //        }
        //        var result = new HttpResponseMessage(HttpStatusCode.OK)
        //        {
        //            Content = new ByteArrayContent(bytes)
        //        };
        //        result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
        //        {
        //            FileName = Path.GetFileName(fi.Name)
        //        };
        //        result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
        //        var response = ResponseMessage(result);
        //        return response;
        //    }
        //    catch(Exception ex)
        //    {
        //        LogHelper.Error("获取升级包失败", ex);
        //        return Content<ApiErrorInfo>(HttpStatusCode.InternalServerError, new ApiErrorInfo() { errcode = (int)ErrorCode.ServerException, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.ServerException) });
        //    }
        //}

        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetPackage(HttpRequestMessage request)
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
                if (db.softwareupdate.Where(m => m.type == 0).Count() > 0)
                {
                    sf = db.softwareupdate.Where(m => m.type == 0).OrderByDescending(m => m.uploadTime).First();
                }
            }
            if (sf == null)
            {
                return Content<ApiErrorInfo>(HttpStatusCode.NotFound, new ApiErrorInfo() { errcode = (int)ErrorCode.ObjectNotFound, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.ObjectNotFound) });
            }
            else
            {
                string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content");
                string packPath = filePath + "\\apk\\" + sf.version;

                if (!System.IO.File.Exists(packPath))
                {
                    return Content<ApiErrorInfo>(HttpStatusCode.NotFound, new ApiErrorInfo() { errcode = (int)ErrorCode.ObjectNotFound, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.ObjectNotFound) });
                }
                string path = string.Format(@"{0}/{1}", FtpHelper.PackageFtpURL, sf.version);
                FtpPath vf = new FtpPath();
                vf.FtpURL = path;
                return Ok(vf);
            }
        }

        public IHttpActionResult GetPackage(HttpRequestMessage request,string  id)
        {
            string deviceCode;
            if (request.Headers.Authorization == null || TokenHelper.CheckAuth(request.Headers.Authorization.Parameter, out deviceCode) == false)
            {
                return Content<ApiErrorInfo>(HttpStatusCode.Unauthorized, new ApiErrorInfo() { errcode = (int)ErrorCode.Unauthorized, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.Unauthorized) });
            }
            try
            {
                TaskManager.RemoveUpdate(id);
                return Ok();
            }
            catch(Exception ex)
            {
                LogHelper.Error("软件包更新下载确认失败", ex);
                return Content<ApiErrorInfo>(HttpStatusCode.InternalServerError, new ApiErrorInfo() { errcode = (int)ErrorCode.ServerException, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.ServerException) });
            }
        }
    }
}