using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BEM.Models;
using BEMAttendance;
using BEMAttendance.Models;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data.Entity.Validation;
using RegRecord = BEMAttendance.regrecord;
namespace BEM.Controllers
{
    [RoutePrefix("bem/v1/reg-record")]
    public class RegRecordController : ApiController
    {
        // POST: api/RegRecord
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> PostRegRecord(HttpRequestMessage request,RegRecordJsonObj regRecordObj)
        {
            string deviceCode;
            if (TokenHelper.CheckAuth(request.Headers.Authorization.Parameter, out deviceCode) == false)
            {
                return Content<ApiErrorInfo>(HttpStatusCode.Unauthorized, new ApiErrorInfo() { errcode = (int)ErrorCode.Unauthorized, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.Unauthorized) });
            }
            //检查超时
            if (!TokenHelper.IsTokenOnline(deviceCode, DateTime.Now))
            {
                return Content<ApiErrorInfo>(HttpStatusCode.InternalServerError, new ApiErrorInfo() { errcode = (int)ErrorCode.OverTime, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.OverTime) });
            }
            if (regRecordObj == null)
            {
                return Content<ApiErrorInfo>(HttpStatusCode.BadRequest, new ApiErrorInfo() { errcode = (int)ErrorCode.DataInputFormateError, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.DataInputFormateError) });
            }
            try
            {
                RegRecord newRecord = new RegRecord();
                newRecord.RecordDeviceID = regRecordObj.devCode == null ? string.Empty : regRecordObj.devCode;
                newRecord.RecordEmpCode = regRecordObj.empCode == null ? string.Empty : regRecordObj.empCode;
                newRecord.RecordEmpName = regRecordObj.empName == null ? string.Empty : regRecordObj.empName;
                newRecord.RecordEmpID = regRecordObj.empID == null ? string.Empty : regRecordObj.empID;
                newRecord.RecordGroupCode = regRecordObj.groupCode == null ? string.Empty : regRecordObj.groupCode;
                newRecord.RecordGroupName = regRecordObj.groupName == null ? string.Empty : regRecordObj.groupName;
                newRecord.RecordTime = DateTime.Parse(regRecordObj.recordTime);
                using (BemEntities db = new BemEntities())
                {
                    db.regrecord.Add(newRecord);
                    await db.SaveChangesAsync();
                }
                SaveImage(regRecordObj.empImage1,regRecordObj.empImage2, newRecord.RecordID,newRecord.RecordEmpCode,newRecord.RecordEmpName,newRecord.RecordTime);
                return Ok();
            }
            catch(DbEntityValidationException dbex)
            {
                LogHelper.Error("数据添加失败" + dbex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage + "empCode=" + regRecordObj.empCode+"empName"+regRecordObj.empName);
                return Content<ApiErrorInfo>(HttpStatusCode.InternalServerError, new ApiErrorInfo() { errcode = (int)ErrorCode.DbError, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.DbError) });
            }
            catch(Exception ex)
            {
                LogHelper.Error("数据添加失败", ex);
                return Content<ApiErrorInfo>(HttpStatusCode.InternalServerError, new ApiErrorInfo() { errcode = (int)ErrorCode.DbError, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.DbError) });
            }
        }
        private void SaveImage(string pic1,string pic2,long id,string empCode,string empName,DateTime uploadTime)
        {
            try
            {
                //string savePath = Url.Content("~\\Content\\RegImages");
                string savePath = System.Web.HttpContext.Current.Server.MapPath("~\\Content\\RegFaceImages");
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                string folderName = savePath +"\\"+ uploadTime.ToString("yyyy-MM-dd");
                if(!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }
                byte[] bytes1 = null;
                byte[] bytes2 = null;
                if(!string.IsNullOrEmpty(pic1))
                {
                    bytes1 = Convert.FromBase64String(pic1);
                }
                if(!string.IsNullOrEmpty(pic2))
                {
                    bytes2 = Convert.FromBase64String(pic2);
                }
                TimeSpan ts = uploadTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                string timeUtc = Convert.ToInt64(ts.TotalSeconds).ToString();   //获取时间戳
                if (string.IsNullOrEmpty(empCode))
                {
                    if(bytes1!=null)
                    {
                        using (MemoryStream ms = new MemoryStream(bytes1))
                        {
                            Image img = Image.FromStream(ms);
                            img.Save(string.Format("{0}\\{1}_{2}_1.jpg", folderName,timeUtc,id) ,ImageFormat.Jpeg);
                        }
                    }

                    if(bytes2!=null)
                    {
                        using (MemoryStream ms = new MemoryStream(bytes2))
                        {
                            Image img = Image.FromStream(ms);
                            img.Save(string.Format("{0}\\{1}_{2}_2.jpg", folderName,timeUtc,id),ImageFormat.Jpeg);
                        }
                    }
                }
                else
                {
                    if (bytes1 != null)
                    {
                        using (MemoryStream ms = new MemoryStream(bytes1))
                        {
                            Image img = Image.FromStream(ms);
                            img.Save(string.Format("{0}\\{1}_{2}_{3}_1.jpg", folderName,timeUtc,empCode,id),ImageFormat.Jpeg);
                        }
                    }
                    if (bytes2 != null)
                    {
                        using (MemoryStream ms = new MemoryStream(bytes2))
                        {
                            Image img = Image.FromStream(ms);
                            img.Save(string.Format("{0}\\{1}_{2}_{3}_2.jpg", folderName, timeUtc, empCode, id), ImageFormat.Jpeg);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                LogHelper.Error("保存门禁图片失败", ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        private bool RegRecordExists(long id)
        {
            using (BemEntities db = new BemEntities())
            {
                return db.regrecord.Count(e => e.RecordID == id) > 0;
            }
        }
    }
}