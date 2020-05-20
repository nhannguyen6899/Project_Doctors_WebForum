using Doctors_WebForum.Models.ADO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Doctors_WebForum.Models.ViewModels
{

    public class DoctorViewModelAdmin
    {
        public int Id { get; set; }

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public Nullable<int> Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }



        [Required(ErrorMessage = "UserName cannot be empty")]
        [MaxLength(50, ErrorMessage = "Password cannot be greater than 50")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password cannot be empty")]
        [MaxLength(50, ErrorMessage = "Password cannot be greater than 50")]
        public string Password { get; set; }
        public string Image { get; set; }



        public Nullable<System.DateTime> Birthday { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public bool Active { get; set; }
        public bool Role { get; set; }



    }

    /// ///////////////////////////////////////////////////////////////

    public class LoginDoctorViewModelAdmin
    {
        [Required(ErrorMessage = "Tài kHoản Không tồn tại")]
        [MaxLength(50, ErrorMessage = "Username cannot be greater than 50")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Mật khẩu Không đúng")]
        [MaxLength(50, ErrorMessage = "Password cannot be greater than 50")]
        public string Password { get; set; }
    }




    /// ///////////////////////////////////////////////////////////////////////

    public class ChangePasswordDoctorViewModelAdmin
    {
        public int Id { get; set; }


        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Mật khẩu Không đúng")]
        [MaxLength(50, ErrorMessage = "Password cannot be greater than 50")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "New Password cannot be empty")]
        [Display(Name = "Mật khẩu mới :")]
        [MaxLength(50, ErrorMessage = "New Password cannot be greater than 50")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu :")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu và Nhập lại mật khẩu không khớp")]
        public string ConfirmNewPassword { get; set; }
    }

    ////////////////////////////////////////////////////////////////////

    public class RecoverPasswordDoctorViewModelAdmin
    {
        [Required(ErrorMessage = "Email không được bỏ trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "New Password cannot be empty")]
        [MaxLength(50, ErrorMessage = "New Password cannot be greater than 50")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu và Nhập lại mật khẩu không khớp")]
        public string ConfirmNewPassword { get; set; }
    }
    ////////////////////////////////////////////////////////////////////
    public class InforDoctorViewModelAdmin
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Họ không được bỏ trống")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Tên không được bỏ trống")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được bỏ trống")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Email không được bỏ trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        public string Image { get; set; }
    }

    ////////////////////////////////////////////////////////////////////

    public class TopicViewModelAdmin
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui Lòng Nhập Chủ Đề")]
        public string TopicName { get; set; }

        [Required(ErrorMessage = "Vui Lòng Nhập Mô Tả Chủ Đề")]
        public string Description { get; set; }

        public int Specialize_ID { get; set; }
        public string SpecializeName { get; set; }


        [NotMapped]
        public List<Specialize> specializes { get; set; }
    }

    ////////////////////////////////////////////////////////////////////

    public class SpecializeViewModelAdmin
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui Lòng Nhập Chuyên Môn")]
        public string SpecializeName { get; set; }

        [Required(ErrorMessage = "Vui Lòng Nhập Mô Tả Chuyên Môn")]
        public string Description { get; set; }


    }

    ////////////////////////////////////////////////////////////////////

    public class PostViewModelAdmin
    {
        public int Id { get; set; }
        public string PostName { get; set; }
        public string PostContent { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> Likes { get; set; }
        public Nullable<int> Views { get; set; }
        public bool StatusBlock { get; set; }

        public int Topic_ID { get; set; }
        public string TopicName { get; set; }

    }


}