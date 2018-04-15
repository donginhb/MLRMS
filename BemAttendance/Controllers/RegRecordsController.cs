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
    [RoutePrefix("bem/v1/regRecords")]
    public class RegRecordsController : ApiController
    {
        // POST: api/RegRecord
        [HttpPost]
        public async Task<IHttpActionResult> PostRecords(RecordsQuery queryItem)
        {
            try
            {
                if(queryItem==null)
                {
                    return Content<ApiErrorInfo>(HttpStatusCode.BadRequest, new ApiErrorInfo() { errcode = (int)ErrorCode.DataInputFormateError, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.DataInputFormateError)});
                }
                DateTime dt1 = DateTime.Now;
                DateTime dt2 = DateTime.Now;
                if(DateTime.TryParse(queryItem.startTime,out dt1)==false||DateTime.TryParse(queryItem.endTime,out dt2)==false)
                {
                    return Content<ApiErrorInfo>(HttpStatusCode.BadRequest, new ApiErrorInfo() { errcode = (int)ErrorCode.DataInputFormateError, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.DataInputFormateError) });
                }
                if((dt2-dt1).TotalDays>60)
                {
                    return Content<ApiErrorInfo>(HttpStatusCode.InternalServerError, new ApiErrorInfo() { errcode = (int)ErrorCode.QueryExceed, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.QueryExceed) });
                }
                using (BemEntities db = new BemEntities())
                {
                    var recordsList = from record in db.regrecord
                                      join dev in db.clientdevice
                                      on record.RecordDeviceID equals dev.DevCode
                                      where record.RecordTime >= dt1 && record.RecordTime <= dt2
                                      select new
                                      {
                                          empName = record.RecordEmpName,
                                          empCode = record.RecordEmpCode,
                                          empID = record.RecordEmpID,
                                          departmentName = record.RecordGroupName,
                                          departmentCode = record.RecordGroupCode,
                                          recordTime = record.RecordTime,
                                          deviceCode = record.RecordDeviceID,
                                          deviceName = dev.DevName
                                      };
                    if (!string.IsNullOrEmpty(queryItem.empCode))
                    {
                        recordsList = recordsList.Where(m => m.empCode == queryItem.empCode);
                    }
                    if (!string.IsNullOrEmpty(queryItem.deviceCode))
                    {
                        recordsList = recordsList.Where(m => m.deviceCode == queryItem.deviceCode);
                    }
                    if (!string.IsNullOrEmpty(queryItem.departmentCode))
                    {
                        recordsList = recordsList.Where(m => m.departmentCode == queryItem.departmentCode);
                    }
                    var contentList = recordsList.OrderBy(m => m.recordTime).ToList();
                    if (contentList.Count > 0)
                    {
                        return Ok(contentList);
                    }
                    else
                    {
                        return Content<ApiErrorInfo>(HttpStatusCode.NoContent, new ApiErrorInfo() { errcode = (int)ErrorCode.ObjectNotFound, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.ObjectNotFound) });
                    }
                }
            
            }
            catch(Exception ex)
            {
                LogHelper.Error("查询原始记录出错", ex);
                return Content<ApiErrorInfo>(HttpStatusCode.InternalServerError, new ApiErrorInfo() { errcode = (int)ErrorCode.DbError, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.DbError) });
            }
        }
    }
}
