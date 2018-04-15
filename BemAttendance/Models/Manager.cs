using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class Manager
    {
        public long AdminId { get; set; }

        [DisplayName("用户ID")]
        [StringLength(30, ErrorMessage = "长度不能超过30个字符")]
        [Required(ErrorMessage = "不能为空")]
        public string UserCode { get; set; }

        [DisplayName("用户名")]
        [StringLength(30, ErrorMessage = "长度不能超过30个字符")]
        [Required(ErrorMessage = "不能为空")]
        public string UserName { get; set; }

        [DisplayName("原密码")]
        [Required(ErrorMessage = "不能为空")]
        [StringLength(18, ErrorMessage = "长度不能超过18个字符")]
        public string Passwd { get; set; }

        [DisplayName("新密码")]
        [Required(ErrorMessage = "不能为空")]
        [RegularExpression(@"^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{6,18}$",ErrorMessage ="密码必须为长度在6到18之间的数字和字母的组合")]
        public string NewPwd { get; set; }

        [DisplayName("确认密码")]
        [Required(ErrorMessage = "不能为空")]
        [RegularExpression(@"^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{6,18}$", ErrorMessage = "密码必须为长度在6到18之间的数字和字母的组合")]
        [Compare("NewPwd")]
        public string NewPwdConfirm { get; set; }
    }
}