using Doctors_WebForum.Models.ADO;
using Doctors_WebForum.Models.ViewModels;
using PagedList;
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
    public class AdminController : Controller
    {
        private Project_Doctors_WebForumEntities db = new Project_Doctors_WebForumEntities();
        // GET: Admin
        public ActionResult Index()
        {
            if (Session["Email"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
  
        private bool isValidContentType(String contentType)
        {
            return contentType.Equals("image/png") || contentType.Equals("image/gif") || contentType.Equals("image/jpg") || contentType.Equals("image/jpeg");
        }

        private bool isValidContentLenght(int contentLenght)
        {
            return ((contentLenght / 1024) / 1024) < 1;
        }

        public ActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginDoctorViewModelAdmin doc)
        {
            var obj = db.Doctors.SingleOrDefault(a => a.Username.Equals(doc.Username) && a.Role == true);
            if (obj != null)
            {
                var checkPassword = BCrypt.Net.BCrypt.Verify(doc.Password, obj.Password);
                if (checkPassword)
                {
                    Session["Email"] = obj.Email;
                    Session["ImgAdmin"] = obj.Image;                    
                    Session["IdAdmin"] = obj.Id.ToString();
                    return RedirectToAction("Index", "Admin");
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

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Admin");
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            if (Session["Email"] != null)
            {
                var id = Session["IdAdmin"];
                var doctor = db.Doctors.Find(Convert.ToInt32(id));
                var Change = new ChangePasswordDoctorViewModelAdmin()
                {
                    Id = Convert.ToInt32(id)
                };
                return View(Change);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordDoctorViewModelAdmin doctorViewModel)
        {
            var obj = db.Doctors.SingleOrDefault(a => a.Id.Equals(doctorViewModel.Id));
            var checkPassword = BCrypt.Net.BCrypt.Verify(doctorViewModel.Password, obj.Password);
            if (checkPassword)
            {
                obj.Password = BCrypt.Net.BCrypt.HashPassword(doctorViewModel.NewPassword);
                db.SaveChanges();
                TempData["Message"] = "Change password successfully";
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                ViewBag.Error = "Wrong old password";
                return View();
            }
        }



        [HttpGet]
        public ActionResult Information()
        {
            if (Session["Email"] != null)
            {
                var id = Session["IdAdmin"];
                var doctor = db.Doctors.Find(Convert.ToInt32(id));
                var doc = new InforDoctorViewModelAdmin()
                {
                    Id = Convert.ToInt32(id),
                    Firstname = doctor.Firstname,
                    Lastname = doctor.Lastname,
                    Email = doctor.Email,
                    Address = doctor.Address,
                    Image = doctor.Image
                };
                return View(doc);
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        [HttpPost]
        public ActionResult Information(InforDoctorViewModelAdmin infor, HttpPostedFileBase photo)
        {

            var id = Session["IdAdmin"];
            var doctor = db.Doctors.Find(Convert.ToInt32(id));
            var docview = new InforDoctorViewModelAdmin()
            {
                Id = Convert.ToInt32(id),
                Firstname = doctor.Firstname,
                Lastname = doctor.Lastname,
                Email = doctor.Email,
                Address = doctor.Address,
                Image = doctor.Image
            };

            var doc = db.Doctors.SingleOrDefault(a => a.Id.Equals(infor.Id));
            if (photo != null)
            {
                if (!isValidContentType(photo.ContentType))
                {
                    ViewBag.Error = "Only JPG, JPEG, PNG & GIF files are allowed";
                    return View(docview);
                }
                else if (!isValidContentLenght(photo.ContentLength))
                {
                    ViewBag.Error = "Your file is too large";
                    return View(docview);
                }
                else
                {
                    if (photo.ContentLength > 0)
                    {
                        // detele image old
                        if (infor.Image != null)
                        {
                            var filePathDetele = Server.MapPath("~/Content/cssAdmin/app/media/img/users/" + infor.Image);
                            if (System.IO.File.Exists(filePathDetele))
                            {
                                System.IO.File.Delete(filePathDetele);
                            }
                        }

                        // add image new
                        var fileName = Path.GetFileName(photo.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/cssAdmin/app/media/img/users/"), fileName);
                        photo.SaveAs(path);
                        doc.Image = photo.FileName;
                        Session["Img"] = photo.FileName.ToString();
                    }
                }
            }

            doc.Id = infor.Id;
            doc.Firstname = infor.Firstname;
            doc.Lastname = infor.Lastname;
            doc.Email = infor.Email;
            doc.Address = infor.Address;
            db.SaveChanges();

            return RedirectToAction("Information", "Admin");
        }

        // forgot password

        public ActionResult Forgot()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Forgot(string email)
        {
            var data = db.Doctors.Where(x => x.Email == email && x.RegistrationStatus == true && x.Role == true).FirstOrDefault();
            if (data != null)
            {
                Reset(data.Email);
                ViewBag.Error = " Cảm ơn bạn. Một thư xác nhận sẽ được gửi đến địa chỉ email " + data.Email + " trong ít phút, bạn vui lòng mở hộp thư và bấm vào URL bên trong thư để hoàn tất";
            }
            else
            {
                ViewBag.Error = "Email Không tồn tại";
            }

            return View();
        }

        [HttpGet]
        public ActionResult Recover(string email)
        {
            var data = db.Doctors.SingleOrDefault(x => x.Email == email);
            var doc = new RecoverPasswordDoctorViewModelAdmin
            {
                Email = data.Email
            };
            return View(doc);
        }



        [HttpPost]
        public ActionResult Recover(RecoverPasswordDoctorViewModelAdmin doc)
        {

            var data = db.Doctors.SingleOrDefault(x => x.Email.Equals(doc.Email));
            data.Password = BCrypt.Net.BCrypt.HashPassword(doc.NewPassword);
            db.SaveChanges();
            return RedirectToAction("Login");
        }



        public void Reset(string email)
        {
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "Forgot" + ".cshtml");
            var resetInfo = db.Doctors.Where(x => x.Email == email).FirstOrDefault();
            var url = "http://localhost:3398/" + "Admin/Recover?email=" + resetInfo.Email;
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

        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult GetDoctor(string Search, int? i)
        {
            if (Session["Email"] != null)
            {

                List<Doctor> listdoctor = db.Doctors.Where(s => s.Firstname.Contains(Search) || s.Lastname.Contains(Search) || s.Username.Contains(Search)|| s.Email.Contains(Search) || Search == null && s.Role != true).ToList();

                var doc = listdoctor.Select(x => new DoctorViewModelAdmin
                {
                    Id = x.Id,
                    Firstname = x.Firstname,
                    Lastname = x.Lastname,
                    Email = x.Email,
                    Birthday = x.Birthday,
                    Address = x.Address,
                    Gender = x.Gender,
                    Phone = x.Phone,
                    Active = x.Active,
                    Role = x.Role,
                    Image = x.Image,
                    Username = x.Username,
                    Password = x.Password,
                    CreateDate = x.CreateDate
                }).ToList();
                return View(doc.ToPagedList(i ?? 1, 5));

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }

        }
        public ActionResult DeleteDoc(int id)
        {
            var doc = db.Doctors.Where(p => p.Id == id);
            var post = db.Posts.Where(z => z.Doctor_ID == id).ToList();

            var com = from p in db.Posts
                      join c in db.Comments on p.Id equals c.Post_ID
                      where p.Doctor_ID == id || c.Doctor_ID == id 
                      select c;

            var likePos = from p in db.Posts
                          join lp in db.LikePosts on p.Id equals lp.Post_ID
                          where p.Doctor_ID == id || lp.Doctor_ID == id
                          select lp;

            var RepCom = from c in db.Comments
                         join rc in db.ReplyComments on c.Id equals rc.Comment_ID
                         where c.Doctor_ID == id || rc.Doctor_ID == id
                         select rc;

            db.ReplyComments.RemoveRange(RepCom);
            db.LikePosts.RemoveRange(likePos);
            db.Comments.RemoveRange(com);
            db.Posts.RemoveRange(post);
            db.Doctors.RemoveRange(doc);

            db.SaveChanges();
            return RedirectToAction("GetDoctor", "Admin");
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult GetPost(string Search, int? i)
        {
            if (Session["Email"] != null)
            {
                //  count số like theo Id Post
                List<LikePost> listPost = db.LikePosts.Where(x => x.StatusLike == true).ToList();
                ViewBag.listLikePost = listPost;

                List<Post> listpost = db.Posts.Where(s => s.Description.Contains(Search) || s.PostContent.Contains(Search) || s.PostName.Contains(Search) || Search == null).ToList();
                var post = listpost.Select(x => new PostViewModelAdmin
                {
                    Id = x.Id,
                    PostName = x.PostName,
                    PostContent = x.PostContent,
                    Description = x.Description,
                    StatusBlock = x.StatusBlock,
                    Views = x.Views,
                    CreateDate = x.CreateDate,
                    Topic_ID = x.Topic_ID,
                    TopicName = x.Topic.TopicName,
                }).ToList();
                return View(post.ToPagedList(i ?? 1, 5));
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }

        }

        public ActionResult DeletePost(int id)
        {
            var post = db.Posts.Where(p => p.Id == id);
            var com = db.Comments.Where(z => z.Post_ID == id).ToList();
            var likePos = db.LikePosts.Where(z => z.Post_ID == id).ToList();
            var RepCom = from c in db.Comments
                         join rc in db.ReplyComments on c.Id equals rc.Comment_ID
                         where c.Post_ID == id
                         select rc;

            db.ReplyComments.RemoveRange(RepCom);
            db.LikePosts.RemoveRange(likePos);
            db.Comments.RemoveRange(com);
            db.Posts.RemoveRange(post);

            db.SaveChanges();

            return RedirectToAction("GetPost", "Admin");
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult GetSpecialize(string Search, int? i)
        {
            if (Session["Email"] != null)
            {

                List<Specialize> listSpecialize = db.Specializes.Where(s => s.Description.Contains(Search) || s.SpecializeName.Contains(Search) || Search == null).ToList();

                var spe = listSpecialize.Select(x => new SpecializeViewModelAdmin
                {
                    Id = x.Id,
                    SpecializeName = x.SpecializeName,
                    Description = x.Description,
                }).ToList();
                return View(spe.ToPagedList(i ?? 1, 5));
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }

        }

        [HttpGet]
        public ActionResult CreateNewSpe()
        {
            if (Session["Email"] != null)
            {
                return View("CreateNewSpe", new SpecializeViewModelAdmin());
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }

        }

        [HttpPost]
        public ActionResult CreateNewSpe(SpecializeViewModelAdmin specializeViewModel)
        {

            var spe = (from s in db.Specializes
                       select s).FirstOrDefault();
            spe.SpecializeName = specializeViewModel.SpecializeName;
            spe.Description = specializeViewModel.Description;


            db.Specializes.Add(spe);
            db.SaveChanges();

            return RedirectToAction("GetSpecialize", "Admin");


        }

        [HttpGet]
        public ActionResult UpdateSpe(int id)
        {
            if (Session["Email"] != null)
            {

                var specialize = db.Specializes.Find(id);
                var Spe = new SpecializeViewModelAdmin()
                {
                    Id = specialize.Id,
                    Description = specialize.Description,
                    SpecializeName = specialize.SpecializeName

                };
                return View(Spe);

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }

        }

        [HttpPost]
        public ActionResult UpdateSpe(SpecializeViewModelAdmin specializeViewModel)
        {
            var spe = db.Specializes.SingleOrDefault(a => a.Id.Equals(specializeViewModel.Id));

            spe.Id = specializeViewModel.Id;
            spe.SpecializeName = specializeViewModel.SpecializeName;
            spe.Description = specializeViewModel.Description;


            db.SaveChanges();
            return RedirectToAction("GetSpecialize", "Admin");

        }


        public ActionResult DeleteSpe(int id)
        {

            var spe = db.Specializes.Where(p => p.Id == id);
            var top = db.Topics.Where(p => p.Specialize_ID == id);

            var post = from t in db.Topics
                       join p in db.Posts on t.Id equals p.Topic_ID
                       where t.Specialize_ID == id
                       select p;

            var com = from p in db.Posts
                      join c in db.Comments on p.Id equals c.Post_ID
                      where p.Topic.Specialize_ID == id
                      select c;

            var likePos = from p in db.Posts
                          join lp in db.LikePosts on p.Id equals lp.Post_ID
                          where p.Topic.Specialize_ID == id
                          select lp;

            var RepCom = from c in db.Comments
                         join rc in db.ReplyComments on c.Id equals rc.Comment_ID
                         where c.Post.Topic.Specialize_ID == id
                         select rc;



            db.ReplyComments.RemoveRange(RepCom);
            db.LikePosts.RemoveRange(likePos);
            db.Comments.RemoveRange(com);
            db.Posts.RemoveRange(post);
            db.Topics.RemoveRange(top);
            db.Specializes.RemoveRange(spe);


            db.SaveChanges();


            return RedirectToAction("GetSpecialize", "Admin");
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult GetTopic(string Search, int? i)
        {
            if (Session["Email"] != null)
            {
                List<Topic> listtopic = db.Topics.Where(s => s.Description.Contains(Search) || s.TopicName.Contains(Search) || Search == null).ToList();

                var top = listtopic.Select(x => new TopicViewModelAdmin
                {
                    Id = x.Id,
                    TopicName = x.TopicName,
                    Description = x.Description,
                    Specialize_ID = x.Specialize_ID,
                    SpecializeName = x.Specialize.SpecializeName
                }).ToList();
                return View(top.ToPagedList(i ?? 1, 5));
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }

        }

        [HttpGet]
        public ActionResult CreateNewTop()
        {
            if (Session["Email"] != null)
            {
                TopicViewModelAdmin top = new TopicViewModelAdmin();
                top.specializes = db.Specializes.ToList();
                ViewBag.Specializelist = new SelectList(top.specializes, "Id", "SpecializeName");

                return View(top);

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }

        }

        [HttpPost]
        public ActionResult CreateNewTop(TopicViewModelAdmin topicViewModel)
        {

            var top = (from s in db.Topics
                       select s).FirstOrDefault();
            top.TopicName = topicViewModel.TopicName;
            top.Description = topicViewModel.Description;
            top.Specialize_ID = topicViewModel.Specialize_ID;

            db.Topics.Add(top);
            db.SaveChanges();


            return RedirectToAction("GetTopic", "Admin");

        }

        [HttpGet]
        public ActionResult UpdateTop(int id)
        {
            if (Session["Email"] != null)
            {

                var topic = db.Topics.Find(id);
                var top = new TopicViewModelAdmin()
                {
                    Id = topic.Id,
                    Description = topic.Description,
                    TopicName = topic.TopicName,
                    Specialize_ID = topic.Specialize_ID,
                    SpecializeName = topic.Specialize.SpecializeName,


                };

                List<Specialize> list = db.Specializes.ToList();
                List<SpecializeViewModelAdmin> spe = list.Select(x => new SpecializeViewModelAdmin
                {
                    Id = x.Id,
                    Description = x.Description,
                    SpecializeName = x.SpecializeName,
                }).ToList();

                ViewBag.Specializelist = new SelectList(spe, "Id", "SpecializeName");
                return View(top);

            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }

        }

        [HttpPost]
        public ActionResult UpdateTop(TopicViewModelAdmin topicViewModel)
        {
            var top = db.Topics.SingleOrDefault(a => a.Id.Equals(topicViewModel.Id));

            top.Id = topicViewModel.Id;
            top.TopicName = topicViewModel.TopicName;
            top.Description = topicViewModel.Description;
            top.Specialize_ID = topicViewModel.Specialize_ID;
            db.SaveChanges();

            TempData["Message"] = "You are not authorized.";
            return RedirectToAction("GetTopic", "Admin");

        }

        public ActionResult DeleteTop(int id)
        {
            var top = db.Topics.Where(p => p.Id == id);
            var post = db.Posts.Where(z => z.Topic_ID == id).ToList();

            var com = from p in db.Posts
                      join c in db.Comments on p.Id equals c.Post_ID
                      where p.Topic_ID == id
                      select c;

            var likePos = from p in db.Posts
                          join lp in db.LikePosts on p.Id equals lp.Post_ID
                          where p.Topic_ID == id
                          select lp;

            var RepCom = from c in db.Comments
                         join rc in db.ReplyComments on c.Id equals rc.Comment_ID
                         where c.Post.Topic_ID == id
                         select rc;

            db.ReplyComments.RemoveRange(RepCom);
            db.LikePosts.RemoveRange(likePos);
            db.Comments.RemoveRange(com);
            db.Posts.RemoveRange(post);
            db.Topics.RemoveRange(top);
            db.SaveChanges();
            return RedirectToAction("GetTopic", "Admin");
        }

    }
}