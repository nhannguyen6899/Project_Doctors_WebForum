using Doctors_WebForum.Models.ADO;
using Doctors_WebForum.Models.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Doctors_WebForum.Controllers
{
    public class ProfileController : Controller
    {
        private Project_Doctors_WebForumEntities db = new Project_Doctors_WebForumEntities();

        public ActionResult Notification()
        {
            return View();
        }

        private bool isValidContentType(String contentType)
        {
            return contentType.Equals("image/png") || contentType.Equals("image/gif") || contentType.Equals("image/jpg") || contentType.Equals("image/jpeg");
        }

        private bool isValidContentLenght(int contentLenght)
        {
            return ((contentLenght / 1024) / 1024) < 1;
        }

        // Get Profile
        [HttpGet]
        public ActionResult User(int id, int? page)
        {
            var count = db.Doctors.Count(e => e.Id.Equals(id) && e.Role == false);
            if (count == 1)
            {
                // tạo biến phân trang
                int pageSize = 3;
                int pageNumber = (page ?? 1);

                var doctor = db.Doctors.SingleOrDefault(a => a.Id.Equals(id) && a.Role == false);
                var doctors = db.Doctors.Where(a => a.Id.Equals(id) && a.Role == false);
                if (doctor.StatusBlock == true)
                {
                    if (doctor.Id == Convert.ToInt32(Session["Id"])) // user đang đăng nhập (id = idsession) thì dc coi trang cá nhân của mình
                    {
                        // Convert get day,month,year Birthday
                        DateTime dateTime = doctor.Birthday ?? DateTime.Now;
                        int shortDate = dateTime.Day;
                        int shortMonth = dateTime.Month;
                        int shortYear = dateTime.Year;

                        // Convert get day,month,year CreateDate
                        DateTime createDate = doctor.CreateDate ?? DateTime.Now;
                        int shortCreateDate = createDate.Day;
                        int shortCreateMonth = createDate.Month;
                        int shortCreateYear = createDate.Year;

                        var detailProfile = doctors.Select(x => new DoctorViewModel
                        {
                            Id = x.Id,
                            Firstname = x.Firstname,
                            Lastname = x.Lastname,
                            Email = x.Email,
                            Phone = x.Phone ?? default(int),
                            Address = x.Address,
                            Gender = x.Gender,
                            Day = shortDate,
                            Month = shortMonth,
                            Year = shortYear,
                            DayCreateDate = shortCreateDate,
                            MonthCreateDate = shortCreateMonth,
                            YearCreateDate = shortCreateYear,
                            Image = x.Image,

                        }).ToList();

                        ViewBag.detailProfile = detailProfile;

                        ////////////////////////////////////////

                        List<Post> post = db.Posts.Where(x => x.Doctor_ID.Equals(id)).ToList();
                        var postView = post.Select(x => new PostViewModel
                        {
                            Id = x.Id,
                            PostName = x.PostName,
                            PostContent = x.PostContent,
                            Description = x.Description,
                            CreateDate = x.CreateDate,
                            Views = x.Views,
                            StatusBlock = x.StatusBlock,
                            DoctorName = x.Doctor.Firstname + " " + x.Doctor.Lastname,
                            Doctor_ID = x.Doctor_ID,

                            ImageDoctor = x.Doctor.Image
                        }).ToList();

                        ////////////////////////////////////////

                        // Đếm số bài viết của cá nhân
                        List<Post> listPost = db.Posts.ToList();
                        var listPostView = listPost.Select(x => new PostViewModel
                        {
                            Id = x.Id,
                            Doctor_ID = x.Doctor_ID,
                        }).ToList();
                        ViewBag.listPost = listPostView;

                        //  count số like theo Id Post
                        List<LikePost> likePosts = db.LikePosts.ToList();
                        ViewBag.listLikePost = likePosts;

                        //  count số comment theo Id Post ra view Get
                        List<Comment> listCommentView = db.Comments.ToList();
                        ViewBag.listComment = listCommentView;

                        return View(postView.OrderBy(x => x.Id).ToPagedList(pageNumber, pageSize));
                    }
                    ViewBag.thongbao = "Thông tin người này đã bị ẩn";
                    return View("Notification");
                }
                else
                {

                    // Convert get day,month,year Birthday
                    DateTime dateTime = doctor.Birthday ?? DateTime.Now;
                    int shortDate = dateTime.Day;
                    int shortMonth = dateTime.Month;
                    int shortYear = dateTime.Year;

                    // Convert get day,month,year CreateDate
                    DateTime createDate = doctor.CreateDate ?? DateTime.Now;
                    int shortCreateDate = createDate.Day;
                    int shortCreateMonth = createDate.Month;
                    int shortCreateYear = createDate.Year;

                    var detailProfile = doctors.Select(x => new DoctorViewModel
                    {
                        Id = x.Id,
                        Firstname = x.Firstname,
                        Lastname = x.Lastname,
                        Email = x.Email,
                        Phone = x.Phone ?? default(int),
                        Address = x.Address,
                        Gender = x.Gender,
                        Day = shortDate,
                        Month = shortMonth,
                        Year = shortYear,
                        DayCreateDate = shortCreateDate,
                        MonthCreateDate = shortCreateMonth,
                        YearCreateDate = shortCreateYear,
                        Image = x.Image,

                    }).ToList();
                    ViewBag.detailProfile = detailProfile;

                    //  count số like theo Id Post
                    List<LikePost> likePosts = db.LikePosts.ToList();
                    ViewBag.listLikePost = likePosts;

                    //  count số comment theo Id Post ra view Get
                    List<Comment> listCommentView = db.Comments.ToList();
                    ViewBag.listComment = listCommentView;

                    if (doctor.Id == Convert.ToInt32(Session["Id"]))
                    {

                        // Đếm số bài viết của cá nhân
                        List<Post> listPost = db.Posts.ToList();
                        var listPostView = listPost.Select(x => new PostViewModel
                        {
                            Id = x.Id,
                            Doctor_ID = x.Doctor_ID,
                        }).ToList();
                        ViewBag.listPost = listPostView;

                        // Get ds bài viết trong trang cá nhân
                        List<Post> post = db.Posts.Where(x => x.Doctor_ID.Equals(id)).ToList();
                        var postView = post.Select(x => new PostViewModel
                        {
                            Id = x.Id,
                            PostName = x.PostName,
                            PostContent = x.PostContent,
                            Description = x.Description,
                            CreateDate = x.CreateDate,
                            Views = x.Views,
                            StatusBlock = x.StatusBlock,
                            DoctorName = x.Doctor.Firstname + " " + x.Doctor.Lastname,
                            Doctor_ID = x.Doctor_ID,

                            ImageDoctor = x.Doctor.Image
                        }).ToList();
                        return View(postView.OrderBy(x => x.Id).ToPagedList(pageNumber, pageSize));
                    }
                    else
                    {

                        // Đếm số bài viết của cá nhân
                        List<Post> listPost = db.Posts.Where(x => x.StatusBlock == false).ToList();
                        var listPostView = listPost.Select(x => new PostViewModel
                        {
                            Id = x.Id,
                            Doctor_ID = x.Doctor_ID,
                        }).ToList();
                        ViewBag.listPost = listPostView;

                        // Get ds bài viết trong trang cá nhân
                        List<Post> post = db.Posts.Where(x => x.Doctor_ID.Equals(id) && x.StatusBlock == false).ToList();
                        var postView = post.Select(x => new PostViewModel
                        {
                            Id = x.Id,
                            PostName = x.PostName,
                            PostContent = x.PostContent,
                            Description = x.Description,
                            CreateDate = x.CreateDate,
                            Views = x.Views,
                            StatusBlock = x.StatusBlock,
                            DoctorName = x.Doctor.Firstname + " " + x.Doctor.Lastname,
                            Doctor_ID = x.Doctor_ID,

                            ImageDoctor = x.Doctor.Image
                        }).ToList();
                        return View(postView.OrderBy(x => x.Id).ToPagedList(pageNumber, pageSize));
                    }
                    
                }
            }
            else
            {
                ViewBag.thongbao = "Người dùng được yêu cầu không thể được tìm thấy";
                return View("Notification");
            }
            
        }

        /// ////////////////////////////////////////////////////////////////////////////

        // Edit Profile
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var doc = db.Doctors.SingleOrDefault(x => x.Id.Equals(id));
            var count = db.Doctors.Count(e => e.Id.Equals(id));
            if (count == 1)
            {
                if (doc.Id == Convert.ToInt32(Session["Id"]))
                {
                    var doctor = db.Doctors.Find(id);

                    DateTime dateTime = doctor.Birthday ?? DateTime.Now;
                    int shortDate = dateTime.Day;
                    int shortMonth = dateTime.Month;
                    int shortYear = dateTime.Year;

                    DateTime createDate = doctor.CreateDate ?? DateTime.Now;
                    int shortCreateDate = createDate.Day;
                    int shortCreateMonth = createDate.Month;
                    int shortCreateYear = createDate.Year;

                    var detailProfile = new EditDoctorViewModel()
                    {
                        Id = doctor.Id,
                        Firstname = doctor.Firstname,
                        Lastname = doctor.Lastname,
                        Email = doctor.Email,
                        Phone = doctor.Phone ?? default(int),
                        Address = doctor.Address,
                        Gender = doctor.Gender,
                        Day = shortDate,
                        Month = shortMonth,
                        Year = shortYear,
                        DayCreateDate = shortCreateDate,
                        MonthCreateDate = shortCreateMonth,
                        YearCreateDate = shortCreateYear,
                        Image = doctor.Image
                    };

                    ////////////////////////////////////////

                    // Đếm số bài viết của cá nhân
                    List<Post> listPost = db.Posts.ToList();
                    var listPostView = listPost.Select(x => new PostViewModel
                    {
                        Id = x.Id,
                        Doctor_ID = x.Doctor_ID,
                    }).ToList();
                    ViewBag.listPost = listPostView;

                    //  count số like theo Id Post
                    List<LikePost> likePosts = db.LikePosts.ToList();
                    ViewBag.listLikePost = likePosts;

                    return View(detailProfile);
                }
                else
                {
                    ViewBag.thongbao = "Người dùng được yêu cầu không thể được tìm thấy";
                    return View("Notification");
                }                
            }
            else
            {
                ViewBag.thongbao = "Người dùng được yêu cầu không thể được tìm thấy";
                return View("Notification");
            }
        }
        
        [HttpPost]
        public ActionResult Edit(EditDoctorViewModel doctorViewModel, HttpPostedFileBase photo)
        {

            var id = Convert.ToInt32(Session["Id"]);
            var doctors = db.Doctors.Find(id);

            DateTime dateTime = doctors.Birthday ?? DateTime.Now;
            int shortDate = dateTime.Day;
            int shortMonth = dateTime.Month;
            int shortYear = dateTime.Year;

            DateTime createDate = doctors.CreateDate ?? DateTime.Now;
            int shortCreateDate = createDate.Day;
            int shortCreateMonth = createDate.Month;
            int shortCreateYear = createDate.Year;

            var detailProfile = new EditDoctorViewModel()
            {
                Id = doctors.Id,
                Firstname = doctors.Firstname,
                Lastname = doctors.Lastname,
                Email = doctors.Email,
                Phone = doctors.Phone ?? default(int),
                Address = doctors.Address,
                Gender = doctors.Gender,
                Day = shortDate,
                Month = shortMonth,
                Year = shortYear,
                DayCreateDate = shortCreateDate,
                MonthCreateDate = shortCreateMonth,
                YearCreateDate = shortCreateYear,
                Image = doctors.Image
            };

            ////////////////////////////////////////

            // Đếm số bài viết của cá nhân
            List<Post> listPost = db.Posts.ToList();
            var listPostView = listPost.Select(x => new PostViewModel
            {
                Id = x.Id,
                Doctor_ID = x.Doctor_ID,
            }).ToList();
            ViewBag.listPost = listPostView;

            //  count số like theo Id Post
            List<LikePost> likePosts = db.LikePosts.ToList();
            ViewBag.listLikePost = likePosts;

            /////////////////////////////////////////////////////////////////////////////////////


            var doctor = db.Doctors.SingleOrDefault(a => a.Id.Equals(doctorViewModel.Id));
            if (photo != null)
            {
                if (!isValidContentType(photo.ContentType))
                {
                    ViewBag.Error = "Only JPG, JPEG, PNG & GIF files are allowed";
                    return View(detailProfile);
                }
                else if (!isValidContentLenght(photo.ContentLength))
                {
                    ViewBag.Error = "Your file is too large";
                    return View(detailProfile);
                }
                else
                {
                    if (photo.ContentLength > 0)
                    {
                        // detele image old
                        if (doctorViewModel.Image != null)
                        {
                            var filePathDetele = Server.MapPath("~/Content/Images/" + doctorViewModel.Image);
                            if (System.IO.File.Exists(filePathDetele))
                            {
                                System.IO.File.Delete(filePathDetele);
                            }
                        }

                        // add image new
                        var fileName = Path.GetFileName(photo.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                        photo.SaveAs(path);
                        doctor.Image = photo.FileName;
                        Session["Img"] = photo.FileName.ToString();
                    }
                }
            }
            
            doctor.Firstname = doctorViewModel.Firstname;
            doctor.Lastname = doctorViewModel.Lastname;
            doctor.Phone = doctorViewModel.Phone;
            doctor.Email = doctorViewModel.Email;
            doctor.Address = doctorViewModel.Address;
            doctor.Birthday = new DateTime(doctorViewModel.Year, doctorViewModel.Month, doctorViewModel.Day);
            doctor.Gender = doctorViewModel.Gender;
            db.SaveChanges();
            return RedirectToAction("User/"+doctor.Id);
           
        }

        ///////////////////////////////////////////////////////////////////////////////

        // Privacy (chọn chế độ riêng tư thông tin cá nhân)
        [HttpGet]
        public ActionResult Privacy(int id)
        {
            var doc = db.Doctors.SingleOrDefault(x => x.Id.Equals(id));
            var count = db.Doctors.Count(e => e.Id.Equals(id));
            if (count == 1)
            {
                if (doc.Id == Convert.ToInt32(Session["Id"]))
                {
                    var doctor = db.Doctors.Find(id);

                    // Convert get day,month,year CreateDate
                    DateTime createDate = doctor.CreateDate ?? DateTime.Now;
                    int shortCreateDate = createDate.Day;
                    int shortCreateMonth = createDate.Month;
                    int shortCreateYear = createDate.Year;

                    var detailProfile = new DoctorViewModel()
                    {
                        Id = doctor.Id,
                        Firstname = doctor.Firstname,
                        Lastname = doctor.Lastname,
                        DayCreateDate = shortCreateDate,
                        MonthCreateDate = shortCreateMonth,
                        YearCreateDate = shortCreateYear,
                        Image = doctor.Image,
                        StatusBlock = doctor.StatusBlock
                    };

                    ////////////////////////////////////////

                    // Đếm số bài viết của cá nhân
                    List<Post> listPost = db.Posts.ToList();
                    var listPostView = listPost.Select(x => new PostViewModel
                    {
                        Id = x.Id,
                        Doctor_ID = x.Doctor_ID,
                    }).ToList();
                    ViewBag.listPost = listPostView;

                    //  count số like theo Id Post
                    List<LikePost> likePosts = db.LikePosts.ToList();
                    ViewBag.listLikePost = likePosts;

                    return View(detailProfile);
                }
                else
                {
                    ViewBag.thongbao = "Người dùng được yêu cầu không thể được tìm thấy";
                    return View("Notification");
                }
            }
            else
            {
                ViewBag.thongbao = "Người dùng được yêu cầu không thể được tìm thấy";
                return View("Notification");
            }
        }

        [HttpPost]
        public ActionResult Privacy(DoctorViewModel doctorViewModel)
        {
            var doctor = db.Doctors.SingleOrDefault(a => a.Id.Equals(doctorViewModel.Id));
            doctor.StatusBlock = doctorViewModel.StatusBlock;
            db.SaveChanges();
            return RedirectToAction("User/" + doctor.Id);
        }

    }
}