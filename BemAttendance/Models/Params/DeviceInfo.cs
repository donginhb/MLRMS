using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models.Params
{
    public class DeviceInfo
    {
        [DisplayName("设备地址")]
        [RegularExpression(@"^([1-9]\d*|[0]{1,1})$", ErrorMessage = "设备地址只能为数字")]
        public int slaveID { get; set; }

        [DisplayName("设备名称")]
        [Required(ErrorMessage = "设备名称不能为空")]
        [StringLength(30, ErrorMessage = "长度不能超过30个字符")]
        [RegularExpression(@"^[a-zA-Z0-9_\u4e00-\u9fa5]+$", ErrorMessage = "设备名称只能由数字、字母、汉字或下划线组成")]
        public string devName { get; set; }
        public int type { get; set; }
        public string typeName { get; set; }
        public int subType { get; set; }
        public string subTypeName { get; set; }
        public int runStatus { get; set; }

        public string serviceCode { get; set; }

        [DisplayName("串口服务器")]
        public string serviceName { get; set; }

        [DisplayName("端口号")]
        [Required(ErrorMessage = "端口号不能为空")]
        [RegularExpression(@"^[1-9]$|^1\d$|^2\d$|^3[0]$", ErrorMessage = "端口号只能为1-30之间的正整数")]
        public int port { get; set; }

        [DisplayName("备注")]
        [StringLength(30, ErrorMessage = "备注最多不能超过128个字符")]
        public string note { get; set; }
        public DateTime lastUpdateTime { get; set; }

        public string SetTemp { get; set; }
        public string SetMode { get; set; }
    }
}