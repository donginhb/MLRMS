using BEM.Models;
using BEMAttendance;
using BEMAttendance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Employee = BEMAttendance.employee;
namespace BEM.Controllers
{
    [RoutePrefix("bem/v1/updates")]
    public class UpdatesController : ApiController
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetUpdates(HttpRequestMessage request)
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
            try
            {
                LogHelper.Info(string.Format("设备[{0}]开始GetUpdates", deviceCode));

                UpdatesJsonObj updatesJsonObj = null;
                await new TaskFactory().StartNew(delegate ()
                { updatesJsonObj = TaskManager.GetTasks(deviceCode); });

                if (updatesJsonObj == null)
                {
                    return Content<ApiErrorInfo>(HttpStatusCode.NotFound, new ApiErrorInfo() { errcode = (int)ErrorCode.ObjectNotFound, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.ObjectNotFound) });
                }
                return Ok(updatesJsonObj);
            }
            catch(Exception ex )
            {
                LogHelper.Error("获取更新信息失败", ex);
                return Content<ApiErrorInfo>(HttpStatusCode.InternalServerError, new ApiErrorInfo() { errcode = (int)ErrorCode.ServerException, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.ServerException) });
            }
        }

        public void DeleteUpdatesByServer(string devCode)
        {
            string taskId = string.Empty;
            try
            {
                LogHelper.Info(string.Format("设备[{0}]请求删除绑定",devCode));
                if (TaskManager.CheckDeleteLegal(devCode,out taskId) == true)
                {
                    //进行清理操作
                    using (BemEntities db = new BemEntities())
                    {
                        using (var scope = db.Database.BeginTransaction())
                        {
                            try
                            {
                                db.Database.ExecuteSqlCommand(string.Format("delete from deviceemployeerelation where Operate={0} and devCode='{1}';", (int)OPERATETYPE.DEBIND, devCode));//解除绑定

                                db.Database.ExecuteSqlCommand(string.Format("delete from deviceemployeerelation where Operate={0} and devCode='{1}';", (int)OPERATETYPE.DELETE, devCode));//解除绑定
                                db.Database.ExecuteSqlCommand(string.Format("update deviceemployeerelation set Operate=0 where devCode='{0}';", devCode));
                                scope.Commit();                //正常完成，提交事务
                                TaskManager.RemoveTasks(taskId);   //从任务缓存中移除任务
                            }
                            catch (Exception ex)
                            {
                                scope.Rollback();               //发生异常后回滚
                                LogHelper.Error("GetAllEmployees更新清理出错", ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetAllEmployees清除更新信息失败", ex);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetUpdates(HttpRequestMessage request,string id)
        {
            string deviceCode;
            if (request.Headers.Authorization == null || TokenHelper.CheckAuth(request.Headers.Authorization.Parameter, out deviceCode) == false)
            {
                return Content<ApiErrorInfo>(HttpStatusCode.Unauthorized, new ApiErrorInfo() { errcode = (int)ErrorCode.Unauthorized, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.Unauthorized) });
            }
            try
            {
                LogHelper.Info(string.Format("设备[{0}]GetUpdates完毕", deviceCode));
                if (TaskManager.CheckDeleteLegal(deviceCode, id) == true)
                {
                    //进行清理操作
                    using (BemEntities db = new BemEntities())
                    {
                        using (var scope = db.Database.BeginTransaction())
                        {
                            try
                            {
                                db.Database.ExecuteSqlCommand(string.Format("delete from deviceemployeerelation where Operate={0} and devCode='{1}';",(int)OPERATETYPE.DEBIND, deviceCode));//解除绑定

                                //var list = (from devRel in db.deviceemployeerelation
                                //           join emp in db.employee
                                //           on devRel.EmpCode equals emp.EmpCode
                                //           where devRel.Operate == 4
                                //           where devRel.DevCode==deviceCode
                                //           select emp).ToList();

                                db.Database.ExecuteSqlCommand(string.Format("delete from deviceemployeerelation where Operate={0} and devCode='{1}';",(int) OPERATETYPE.DELETE,deviceCode));//解除绑定
                                db.Database.ExecuteSqlCommand(string.Format("update deviceemployeerelation set Operate=0 where devCode='{0}';", deviceCode));
                                scope.Commit();                //正常完成，提交事务
                                TaskManager.RemoveTasks(id);   //从任务缓存中移除任务
                                return Ok();
                            }
                            catch (Exception ex)
                            {
                                scope.Rollback();               //发生异常后回滚
                                LogHelper.Error("更新清理出错", ex);
                                return Content<ApiErrorInfo>(HttpStatusCode.InternalServerError, new ApiErrorInfo() { errcode = (int)ErrorCode.ServerException, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.ServerException) });
                            }
                        }
                    }
                }
                else
                {
                    return Content<ApiErrorInfo>(HttpStatusCode.Forbidden, new ApiErrorInfo() { errcode = (int)ErrorCode.IllegalRequest, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.IllegalRequest) });
                }
            }
            catch(Exception ex)
            {
                LogHelper.Error("清除更新信息失败", ex);
                return Content<ApiErrorInfo>(HttpStatusCode.InternalServerError, new ApiErrorInfo() { errcode = (int)ErrorCode.ServerException, errmsg = ErrorCodeTransfer.GetErrorString(ErrorCode.ServerException) });
            }
        }
    }
}
