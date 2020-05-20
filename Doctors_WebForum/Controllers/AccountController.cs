using Doctors_WebForum.Models.ADO;
using Doctors_WebForum.Models.Business;
using Doctors_WebForum.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Doctors_WebForum.Controllers
{
    public class AccountController : Controller
    {
        private Project_Doctors_WebForumEntities db = new Project_Doctors_WebForumEntities();

        public ActionResult Notification()
        {
            return View();
        }

        public void BuildEmailTemplate(int id)
        {
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "Text" + ".cshtml");
            var regInfo = db.Doctors.Where(x => x.Id == id).FirstOrDefault();
            var url = "http://localhost:3398/" + "Account/Confirm?id=" + id;
            body = body.Replace("@ViewBag.ConfirmationLink", url);
            body = body.ToString();
            BuildEmailTemplate("Bạn tạo tài khoản thành công", body, regInfo.Email);
        }

        private static void BuildEmailTemplate(string subjectText, string bodyText, string sendTo)
        {
            string from, to, bcc, cc, subject, body;
            from = "vunhattan99@gmail.com";
            to = sendTo.Trim();
            bcc = "";
            cc = "";
            subject = subjectText;
            StringBuilder sb = new StringBuilder();
            sb.Append(bodyText);
            body = sb.ToString();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from);
            mail.To.Add(new MailAddress(to));
            if (!string.IsNullOrEmpty(cc))
            {
                mail.CC.Add(new MailAddress(cc));
            }

            if (!string.IsNullOrEmpty(bcc))
            {
                mail.Bcc.Add(new MailAddress(bcc));
            }
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SendEmail(mail);

        }

        private static void SendEmail(MailMessage mail)
        {
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new System.Net.NetworkCredential("vunhattan99@gmail.com", "Nhattan123");

            try
            {
                smtp.Send(mail);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private bool isValidContentType(String contentType)
        {
            return contentType.Equals("image/png") || contentType.Equals("image/gif") || contentType.Equals("image/jpg") || contentType.Equals("image/jpeg");
        }

        private bool isValidContentLenght(int contentLenght)
        {
            return ((contentLenght / 1024) / 1024) < 1;
        }

        // Register Account
        [HttpGet]
        public ActionResult Register()
        {
            // lấy ra số thành viên đã đăng kí thành công
            var query = from table in db.Doctors
                        where table.Role != true && table.RegistrationStatus == true
                        select table;
            int countUser = query.Count();
            ViewBag.countUser = countUser;

            return View(new RegisterDoctorViewModel());
        }

        [HttpPost]
        public ActionResult Register(RegisterDoctorViewModel doctorViewModel, HttpPostedFileBase photo)
        {

            // Check Username
            Doctor doctor = new Doctor();
            if (doctorViewModel.Username != null)
            {
                var count = db.Doctors.Count(e => e.Username.Equals(doctorViewModel.Username));
                if (count == 1)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
                }
            }

            if (doctorViewModel.Email != null)
            {
                var count = db.Doctors.Count(e => e.Email.Equals(doctorViewModel.Email));
                if (count == 1)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");
                }
            }

            if (ModelState.IsValid)
            {
                doctor.Firstname = doctorViewModel.Firstname;
                doctor.Lastname = doctorViewModel.Lastname;
                doctor.Email = doctorViewModel.Email;
                doctor.Username = doctorViewModel.Username;
                doctor.Gender = doctorViewModel.Gender;
                doctor.Birthday = new DateTime(doctorViewModel.Year, doctorViewModel.Month, doctorViewModel.Day);
                doctor.CreateDate = DateTime.Now;
                doctor.Password = BCrypt.Net.BCrypt.HashPassword(doctorViewModel.Password);

                db.Doctors.Add(doctor);
                db.SaveChanges();
                BuildEmailTemplate(doctor.Id);
                ViewBag.thongbao = "Cảm ơn bạn. Một thư xác nhận sẽ được gửi đến địa chỉ email " + doctor.Email + " trong ít phút, bạn vui lòng mở hộp thư và bấm vào URL bên trong thư để hoàn tất";
                return View("Notification");
            }
            else
            {
                return View("Register", doctorViewModel);
            }
        }

        /// ///////////////////////////////////////////////////////


        // Login
        public ActionResult Login()
        {
            // lấy ra số thành viên đã đăng kí thành công
            var query = from table in db.Doctors
                        where table.Role != true && table.RegistrationStatus == true
                        select table;
            int countUser = query.Count();
            ViewBag.countUser = countUser;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginDoctorViewModel doctorViewModel)
        {
            var obj = db.Doctors.SingleOrDefault(a => a.Username.Equals(doctorViewModel.Username) && a.Role == false);
            if (obj != null)
            {
                var checkPassword = BCrypt.Net.BCrypt.Verify(doctorViewModel.Password, obj.Password);
                if (checkPassword)
                {
                    if (obj.RegistrationStatus == true)
                    {
                        Session["UserName"] = obj.Username.ToString();
                        Session["Id"] = obj.Id;
                        if (obj.Image != null)
                        {
                            Session["Img"] = obj.Image.ToString();
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Error = "Tài khoản của bạn chưa được xác nhận, vui lòng vào mail để xác nhận";
                        return View("Login");
                    }
                }
                else
                {
                    ViewBag.Error = "Password Ivalid";
                    return View("Login");
                }                   
            }
            else
            {
                ViewBag.Error = "Username Ivalid";
                return View("Login");
            }
        }

        /// //////////////////////////////////////////////////////////////////////////////

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Account");
        }


        ////////////////////////////////////////////////////////////////////////////

        // ChangePassword
        [HttpGet]
        public ActionResult ChangePassword(int id)
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

                    var detailProfile = new ChangePasswordDoctorViewModel()
                    {
                        Id = doctor.Id,
                        Firstname = doctor.Firstname,
                        Lastname = doctor.Lastname,
                        Day = shortDate,
                        Month = shortMonth,
                        Year = shortYear,
                        DayCreateDate = shortCreateDate,
                        MonthCreateDate = shortCreateMonth,
                        YearCreateDate = shortCreateYear,
                        Image = doctor.Image,
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
        public ActionResult ChangePassword(ChangePasswordDoctorViewModel doctorViewModel)
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

            var detailProfile = new ChangePasswordDoctorViewModel()
            {
                Id = doctors.Id,
                Firstname = doctors.Firstname,
                Lastname = doctors.Lastname,
                Day = shortDate,
                Month = shortMonth,
                Year = shortYear,
                DayCreateDate = shortCreateDate,
                MonthCreateDate = shortCreateMonth,
                YearCreateDate = shortCreateYear,
                Image = doctors.Image,
            };

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

            ////////////////////////////////////////////

            var doctor = db.Doctors.SingleOrDefault(a => a.Id.Equals(doctorViewModel.Id));
            var checkPassword = BCrypt.Net.BCrypt.Verify(doctorViewModel.Password, doctor.Password);
            if (checkPassword)
            {
                doctor.Password = BCrypt.Net.BCrypt.HashPassword(doctorViewModel.NewPassword);
                db.SaveChanges();
                return RedirectToAction("User/" + doctor.Id, "Profile");
            }
            else
            {
                ViewBag.Error = "Wrong old password";
            }

            return View(detailProfile);
        }

        public ActionResult Confirm(int id)
        {
            ViewBag.id = id;
            return View();
        }

        public ActionResult RegisterConfirm(int id)
        {
            var data = db.Doctors.Where(x => x.Id == id).FirstOrDefault();
            data.RegistrationStatus = true;
            db.SaveChanges();
            return RedirectToAction("Login", "Account");
        }

        // Quên mật khẩu
        public ActionResult ForgotPassword()
        {
            return View(new ForgotPasswordDoctorViewModel());
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordDoctorViewModel doctorViewModel)
        {

            string message = "";
            bool status = false;

            var data = db.Doctors.Where(x => x.Email == doctorViewModel.Email && x.RegistrationStatus == true).FirstOrDefault();
            if (data != null)
            {
                Reset(data.Email);
                ViewBag.thongbao = "Bạn hãy vào mail để xác nhận hộp thư và đổi lại mật khẩu";
                return View("Notification");
            }
            else
            {
                ViewBag.Error = "Email không tồn tại hoặc chưa được kích hoạt";
            }

            return View();
        }

        // gửi mail để sang recover để đặt lại mật khẩu
        public void Reset(string email)
        {
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "Reset" + ".cshtml");
            var resetInfo = db.Doctors.Where(x => x.Email == email).FirstOrDefault();
            var url = "http://localhost:3398/" + "Account/Recover?email=" + resetInfo.Email;
            body = body.Replace("@ViewBag.RecoverLink", url);
            body = body.ToString();
            Reset("Recover Password", body, resetInfo.Email);
        }

        private static void Reset(string subjectText, string bodyText, string sendTo)
        {
            string from, to, bcc, cc, subject, body;
            from = "vunhattan99@gmail.com";
            to = sendTo.Trim();
            bcc = "";
            cc = "";
            subject = subjectText;
            StringBuilder sb = new StringBuilder();
            sb.Append(bodyText);
            body = sb.ToString();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from);
            mail.To.Add(new MailAddress(to));
            if (!string.IsNullOrEmpty(cc))
            {
                mail.CC.Add(new MailAddress(cc));
            }

            if (!string.IsNullOrEmpty(bcc))
            {
                mail.Bcc.Add(new MailAddress(bcc));
            }
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SendEmail(mail);
        }


        // Đặt lại mật khẩu mới
        [HttpGet]
        public ActionResult Recover(string email)
        {
            var data = db.Doctors.SingleOrDefault(x => x.Email == email);

            DateTime dateTime = data.Birthday ?? DateTime.Now;
            int shortDate = dateTime.Day;
            int shortMonth = dateTime.Month;
            int shortYear = dateTime.Year;

            DateTime createDate = data.CreateDate ?? DateTime.Now;
            int shortCreateDate = createDate.Day;
            int shortCreateMonth = createDate.Month;
            int shortCreateYear = createDate.Year;

            var doctor = new RecoverPasswordDoctorViewModel
            {
                Email = data.Email,
                Id = data.Id,
                Firstname = data.Firstname,
                Lastname = data.Lastname,
                Day = shortDate,
                Month = shortMonth,
                Year = shortYear,
                DayCreateDate = shortCreateDate,
                MonthCreateDate = shortCreateMonth,
                YearCreateDate = shortCreateYear,
                Image = data.Image,
            };

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

            return View(doctor);
        }

        [HttpPost]
        public ActionResult Recover(RecoverPasswordDoctorViewModel doctorViewModel)
        {
            var data = db.Doctors.SingleOrDefault(x => x.Email.Equals(doctorViewModel.Email));
            data.Password = BCrypt.Net.BCrypt.HashPassword(doctorViewModel.Password);
            db.SaveChanges();
            return RedirectToAction("Login");
        }


    }
}