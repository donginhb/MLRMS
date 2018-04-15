using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class UserEx
    {
        public long Id { get; set; }

        public string Gender { get; set; }
        [DisplayName("员工编号")]
        [Required(ErrorMessage = "不能为空")]
        [StringLength(18, ErrorMessage = "长度不能超过18个字符")]
        [RegularExpression(@"^[0-9a-zA-Z_]+$", ErrorMessage = "只能由数字、字母或下划线组成")]
        public string UserCard { get; set; }

        [DisplayName("员工姓名")]
        [Required(ErrorMessage = "不能为空")]
        [StringLength(30, ErrorMessage = "长度不能超过30个字符")]
        [RegularExpression(@"^[a-zA-Z0-9_·\u4e00-\u9fa5]+$", ErrorMessage = "只能由数字、字母、汉字、下划线或·组成")]
        public string UserName { get; set; }

        [DisplayName("部门名称")]
        //[Required(ErrorMessage = "不能为空")]
        [StringLength(30, ErrorMessage = "长度不能超过30个字符")]
        public string Department1 { get; set; }

        [DisplayName("一级部门编号")]
        //[Required(ErrorMessage = "不能为空")]
        [StringLength(30, ErrorMessage = "长度不能超过30个字符")]
        [RegularExpression(@"^[0-9a-zA-Z_]+$", ErrorMessage = "只能由数字、字母或下划线组成")]
        public string Department1Code { get; set; }

        [DisplayName("身份证号")]
        //[Required(ErrorMessage = "不能为空")]
        [RegularExpression(@"^(\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$", ErrorMessage ="请输入合法的身份证号")]
        [StringLength(18, ErrorMessage = "长度不能超过18个字符")]
        public string IDCard { get; set; }

        [DisplayName("手机号")]
        [RegularExpression(@"^[1]+[3,4,5,7,8]+\d{9}", ErrorMessage ="请输入合法的手机号")]
        public string Telephone { get; set; }


        public Nullable<System.DateTime> CreateDate { get; set; }

        public bool IrisEnrolled { get; set; }

        public override bool Equals(object obj)
        {
            if(obj==null)
            {
                return false;
            }
            if((obj).GetType().Equals(this.GetType())==false)
            {
                return false;
            }
            UserEx temp = null;
            temp = (UserEx)obj;
            return this.UserCard.Equals(temp.UserCard);
        }
        //public override int GetHashCode()
        //{
        //    return this.UserCard.GetHashCode();
        //}
    }
}