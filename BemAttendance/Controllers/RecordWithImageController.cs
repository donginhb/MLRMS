using BEM.Models;
using BEMAttendance.Models;
using BEMAttendance.Models.Extend;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using RegRecord = BEMAttendance.regrecord;
namespace BEMAttendance.Controllers
{
    [RoutePrefix("bem/v1/regImgRecords")]
    public class RecordWithImageController : ApiController
    { 
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> PostRegRecord(HttpRequestMessage request,RegImgJsonObj regRecordObj)
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
                SaveImage(regRecordObj.empEye, regRecordObj.empFace, regRecordObj.eyeType, newRecord.RecordID, newRecord.RecordEmpCode, newRecord.RecordEmpName, newRecord.RecordTime, regRecordObj.eyeWidth, regRecordObj.eyeHeight);
                SaveTxt(regRecordObj.debugFile, newRecord.RecordEmpCode, newRecord.RecordTime, regRecordObj.eyeType);
                return Ok();
            }
            catch (DbEntityValidationException dbex)
            {
                LogHelper.Error("数据添加失败" + dbex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage + "empCode=" + regRecordObj.empCode + "empName" + regRecordObj.empName);
                return Content<ApiErrorInfo>(HttpStatusCode.InternalServerError, new ApiErrorInfo() { errcode = (int)ErrorCode.DbError, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.DbError) });
            }
            catch (Exception ex)
            {
                LogHelper.Error("数据添加失败", ex);
                return Content<ApiErrorInfo>(HttpStatusCode.InternalServerError, new ApiErrorInfo() { errcode = (int)ErrorCode.DbError, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.DbError) });
            }
        }

        private void SaveImage(string pic1, string pic2,string eyeType,long id, string empCode, string empName, DateTime uploadTime,int nWidth,int nHeight)
        {
            try
            {
                string saveFacePath = System.Web.HttpContext.Current.Server.MapPath("~\\Content\\RegFaceImages");
                string saveEyePath = System.Web.HttpContext.Current.Server.MapPath("~\\Content\\RegEyeImages");

                if (!Directory.Exists(saveFacePath))
                {
                    Directory.CreateDirectory(saveFacePath);
                }
                if(!Directory.Exists(saveEyePath))
                {
                    Directory.CreateDirectory(saveEyePath);
                }
                string eyeFolderName = saveEyePath + "\\" + uploadTime.ToString("yyyy-MM-dd");
                string faceFolderName = saveFacePath + "\\" + uploadTime.ToString("yyyy-MM-dd");

                if (!Directory.Exists(eyeFolderName))
                {
                    Directory.CreateDirectory(eyeFolderName);
                }
                if(!Directory.Exists(faceFolderName))
                {
                    Directory.CreateDirectory(faceFolderName);
                }
                byte[] bytes1 = null;
                byte[] bytes2 = null;
                if (!string.IsNullOrEmpty(pic1))   //眼图
                {
                    bytes1 = Convert.FromBase64String(pic1);
                }
                if (!string.IsNullOrEmpty(pic2))  //脸图
                {
                    bytes2 = Convert.FromBase64String(pic2);
                }
                TimeSpan ts = uploadTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                string timeUtc = Convert.ToInt64(ts.TotalSeconds).ToString();   //获取时间戳

                if (string.IsNullOrEmpty(empCode))
                {
                    ECItem ecItem = new ECItem();
                    ecItem.eyeImgDes = 2;
                    if (bytes1 != null)
                    {
                        string fileName = string.Format("{0}\\{1}_{2}_{3}.bmp", eyeFolderName,timeUtc, id,eyeType.ToUpper());
                        try
                        {
                            using (FileStream fs = File.Create(fileName))
                            {
                                fs.Write(bytes1, 0, bytes1.Length);
                                fs.Flush();
                            }
                            ecItem.eyeFileName = fileName;
                            ecItem.eyeImgWidth = nWidth;
                            ecItem.eyeImgHeight = nHeight;
                            string type = eyeType.ToLower();
                            if (type == "l")
                            {
                                ecItem.eyeImgDes = 0;
                            }
                            else if (type == "r")
                            {
                                ecItem.eyeImgDes = 1;
                            }
                            else
                            {
                                ecItem.eyeImgDes = 2;
                            }
                        }
                        catch(Exception ex )
                        {
                            LogHelper.Error("保存眼图失败", ex);
                        }
                    }

                    if (bytes2 != null)
                    {
                        using (MemoryStream ms = new MemoryStream(bytes2))
                        {
                            Image img = Image.FromStream(ms);
                            string fileName = string.Format("{0}\\{1}_{2}_2.jpg", faceFolderName, timeUtc, id);
                            img.Save(fileName, ImageFormat.Jpeg);
                            ecItem.faceFileName = fileName;
                            ecItem.id = id;
                            ecItem.dateTime = DateTime.Now;
                        }
                    }
                    if(FtpHelper.NeedUpToEC)
                    {
                        ECProcess.EnqueueTask(ecItem);
                    }
                }
                else
                {
                    ECItem ecItem = new ECItem();
                    ecItem.eyeImgDes = 2;
                    if (bytes1 != null)
                    {
                        string fileName = string.Format("{0}\\{1}_{2}_{3}.bmp", eyeFolderName, timeUtc, empCode, id);
                        try
                        {
                            using (FileStream fs = File.Create(fileName))
                            {
                                fs.Write(bytes1, 0, bytes1.Length);
                                fs.Flush();
                            }
                                ecItem.eyeFileName = fileName;
                                ecItem.eyeImgWidth = nWidth;
                                ecItem.eyeImgHeight = nHeight;
                                string type = eyeType.ToLower();
                                if(type=="l")
                                {
                                    ecItem.eyeImgDes = 0;
                                }
                                else if(type=="r")
                                {
                                    ecItem.eyeImgDes = 1;
                                }
                                else
                                {
                                    ecItem.eyeImgDes = 2;
                                }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error("保存眼图失败", ex);
                        }
                    }
                    if (bytes2 != null)
                    {
                        using (MemoryStream ms = new MemoryStream(bytes2))
                        {
                            Image img = Image.FromStream(ms);
                            string fileName = string.Format("{0}\\{1}_{2}_{3}_2.jpg", faceFolderName, timeUtc, empCode, id);
                            img.Save(fileName, ImageFormat.Jpeg);
                            ecItem.faceFileName = fileName;
                            ecItem.id = id;
                            ecItem.dateTime = DateTime.Now;
                        }
                    }
                    if(FtpHelper.NeedUpToEC)
                    {
                        ECProcess.EnqueueTask(ecItem);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("保存图片失败", ex);
            }
        }

        private void SaveTxt(string txt,string empCode,DateTime uploadTime,string eyeType)
        {
            if(string.IsNullOrEmpty(txt))
            {
                return;
            }
            try
            {
                string saveFilePath = System.Web.HttpContext.Current.Server.MapPath("~\\Content\\DebugFile");

                if (!Directory.Exists(saveFilePath))
                {
                    Directory.CreateDirectory(saveFilePath);
                }
                TimeSpan ts = uploadTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                string timeUtc = Convert.ToInt64(ts.TotalSeconds).ToString();
                string fileName = string.Format("{0}\\{1}_{2}_{3}.txt", saveFilePath, timeUtc, empCode, eyeType.ToUpper());
                using (FileStream fs = File.Create(fileName))
                {
                    byte[] bytes = Convert.FromBase64String(txt);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush();
                }
            }
            catch(Exception ex)
            {
                LogHelper.Error("保存TXT失败", ex);
            }
        }
    }
}
