using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEM.Models
{
    public enum ErrorCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        OK,
        /// <summary>
        /// 保活超时
        /// </summary>
        OverTime,
        /// <summary>
        /// 同步时间获取失败
        /// </summary>
        SyncConfigFail,
        /// <summary>
        /// 设备未添加
        /// </summary>
        DeviceNotExist,
        /// <summary>
        /// 提交的数据格式错误
        /// </summary>
        DataInputFormateError,
        /// <summary>
        /// 资源未找到
        /// </summary>
        ObjectNotFound,
        /// <summary>
        /// 数据库操作失败
        /// </summary>
        DbError,
        /// <summary>
        /// 服务端数据处理异常
        /// </summary>
        ServerException,
        /// <summary>
        /// 鉴权失败
        /// </summary>
        Unauthorized,
        /// <summary>
        /// 已经登录
        /// </summary>
        Logged,
        /// <summary>
        /// 查询超时
        /// </summary>
        QueryExceed,
        /// <summary>
        /// 非法请求
        /// </summary>
        IllegalRequest
    }
    public class ErrorCodeTransfer
    {
        public static string GetErrorString(ErrorCode code)
        {
            string msg = string.Empty;
            switch(code)
            {
                case ErrorCode.OK:
                    msg = "成功";
                 break;
                case ErrorCode.OverTime:
                    msg = "保活超时";
                    break;
                case ErrorCode.SyncConfigFail:
                    msg = "设备名单同步时间获取失败";
                    break;
                case ErrorCode.DeviceNotExist:
                    msg = "设备未添加";
                    break;
                case ErrorCode.DataInputFormateError:
                    msg = "提交的消息体格式有误";
                    break;
                case ErrorCode.ObjectNotFound:
                    msg = "资源未找到";
                    break;
                case ErrorCode.DbError:
                    msg = "数据库操作失败";
                    break;
                case ErrorCode.ServerException:
                    msg = "服务端数据处理异常";
                     break;
                case ErrorCode.Unauthorized:
                    msg = "鉴权失败";
                    break;
                case ErrorCode.QueryExceed:
                    msg = "查询越界";
                    break;
                default:
                    break;

            }
            return msg;
          
        }
    }
}