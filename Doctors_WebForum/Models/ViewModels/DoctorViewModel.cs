using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Doctors_WebForum.Models.ViewModels
{
    public class DoctorViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Họ không được để trống")]
        [MaxLength(15, ErrorMessage = "Họ không thể lớn hơn 15 kí tự")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        [MaxLength(15, ErrorMessage = "Tên không thể lớn hơn 15 kí tự")]
        public string Lastname { get; set; }

        public int Phone { get; set; }

        [MaxLength(50, ErrorMessage = "Địa chỉ không thể lớn hơn 50 kí tự")]
        public string Address { get; set; }

        [MaxLength(50, ErrorMessage = "Email không thể lớn hơn 50 kí tự")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [MaxLength(50, ErrorMessage = "Tên đăng nhập không thể lớn hơn 50 kí tự")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MaxLength(50, ErrorMessage = "Mật khẩu không thể lớn hơn 50 kí tự")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Xác minh mật khẩu và mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }

        public string Image { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }

        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public string Gender { get; set; }

        public Nullable<System.DateTime> CreateDate { get; set; }

        public int DayCreateDate { get; set; }
        public int MonthCreateDate { get; set; }
        public int YearCreateDate { get; set; }

        public bool StatusBlock { get; set; }
        public bool Active { get; set; }
        public bool Role { get; set; }
    }

    /// //////////////////////////////////////////////////////////////////////////////////

    public class RegisterDoctorViewModel
    {
        [Required(ErrorMessage = "Họ không được để trống")]
        [MaxLength(15, ErrorMessage = "Họ không thể lớn hơn 15 kí tự")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        [MaxLength(15, ErrorMessage = "Tên không thể lớn hơn 15 kí tự")]
        public string Lastname { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Required(ErrorMessage = "Email không được để trống")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [MaxLength(50, ErrorMessage = "Tên đăng nhập không thể lớn hơn 50 kí tự")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MaxLength(50, ErrorMessage = "Mật khẩu không thể lớn hơn 50 kí tự")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Xác minh mật khẩu và mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }

        public Nullable<System.DateTime> Birthday { get; set; }

        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public string Gender { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }

        public bool StatusBlock { get; set; }
        public bool RegistrationStatus { get; set; }
        public bool Active { get; set; }
        public bool Role { get; set; }
    }

    /////////////////////////////////////////////////////////////////////////////

    public class LoginDoctorViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [MaxLength(50, ErrorMessage = "Tên đăng nhập không thể lớn hơn 50 kí tự")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MaxLength(50, ErrorMessage = "Mật khẩu không thể lớn hơn 50 kí tự")]
        public string Password { get; set; }
    }

    /////////////////////////////////////////////////////////////////////////////

    public class EditDoctorViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Họ không được để trống")]
        [MaxLength(15, ErrorMessage = "Họ không thể lớn hơn 15 kí tự")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        [MaxLength(15, ErrorMessage = "Tên không thể lớn hơn 15 kí tự")]
        public string Lastname { get; set; }

        public Nullable<int> Phone { get; set; }

        [MaxLength(50, ErrorMessage = "Địa chỉ không thể lớn hơn 50 kí tự")]
        public string Address { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Required(ErrorMessage = "Email không được để trống")]
        public string Email { get; set; }

        public string Image { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }

        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public string Gender { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }

        public int DayCreateDate { get; set; }
        public int MonthCreateDate { get; set; }
        public int YearCreateDate { get; set; }
    }

    /////////////////////////////////////////////////////////////////////////////

    public class ChangePasswordDoctorViewModel
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MaxLength(50, ErrorMessage = "Mật khẩu không thể lớn hơn 50 kí tự")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [MaxLength(50, ErrorMessage = "Mật khẩu mới không thể lớn hơn 50 kí tự")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới và Xác minh mật khẩu mới không khớp.")]
        public string ConfirmNewPassword { get; set; }

        public string Image { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }

        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public Nullable<System.DateTime> CreateDate { get; set; }

        public int DayCreateDate { get; set; }
        public int MonthCreateDate { get; set; }
        public int YearCreateDate { get; set; }
    }

    //////////////////////////////////////////////////////////////////////
    

    public class ForgotPasswordDoctorViewModel
    {
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Required(ErrorMessage = "Email không được để trống")]
        public string Email { get; set; }
    }

    public class RecoverPasswordDoctorViewModel
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [MaxLength(50, ErrorMessage = "Mật khẩu mới không thể lớn hơn 50 kí tự")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu mới và Xác minh mật khẩu mới không khớp.")]
        public string ConfirmNewPassword { get; set; }

        public string Email { get; set; }

        public string Image { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }

        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public Nullable<System.DateTime> CreateDate { get; set; }

        public int DayCreateDate { get; set; }
        public int MonthCreateDate { get; set; }
        public int YearCreateDate { get; set; }
    }

}