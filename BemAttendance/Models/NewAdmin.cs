using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class NewAdmin
    {
        public long AdminId { get; set; }
        [DisplayName("管理员编号")]
        [StringLength(18, ErrorMessage = "长度不能超过18个字符")]
        [Required(ErrorMessage = "不能为空")]
        [RegularExpression(@"^\w+$", ErrorMessage = "只能由数字、字母或下划线组成")]
        public string UserCode { get; set; }

        [DisplayName("管理员名称")]
        [StringLength(30, ErrorMessage = "长度不能超过30个字符")]
        [Required(ErrorMessage = "不能为空")]
        [RegularExpression(@"^[a-zA-Z0-9_·\u4e00-\u9fa5]+$", ErrorMessage = "只能由数字、字母、汉字、下划线或·组成")]
        public string UserName { get; set; }

        [DisplayName("密码")]
        [Required(ErrorMessage = "不能为空")]
        [StringLength(32, ErrorMessage = "长度不能超过32个字符")]
        public string Passwd { get; set; }

        [DisplayName("确认密码")]
        [Required(ErrorMessage = "不能为空")]
        [StringLength(32, ErrorMessage = "长度不能超过32个字符")]
        [Compare("Passwd")]
        public string NewPwdConfirm { get; set; }

        [DisplayName("所属部门")]
        [Required(ErrorMessage = "不能为空")]
        [StringLength(32, ErrorMessage = "长度不能超过32个字符")]
        public string Deparment { get; set; }

        public int UserType { get; set; }
    }
}