using Doctors_WebForum.Models.ADO;
using Doctors_WebForum.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Doctors_WebForum.Controllers
{
    public class SearchController : Controller
    {
        private Project_Doctors_WebForumEntities db = new Project_Doctors_WebForumEntities();

        public ActionResult Notification()
        {
            return View();
        }

        // GET: Search
        public ActionResult Result(string keyword)
        {

            var count = db.Posts.Count(e => e.PostName.Contains(keyword));
            if (count == 1)
            {
                //  count số like theo Id Post ra view Get
                List<LikePost> listPostView = db.LikePosts.ToList();
                ViewBag.listLikePost = listPostView;

                //  count số comment theo Id Post ra view Get
                List<Comment> listCommentView = db.Comments.ToList();
                ViewBag.listComment = listCommentView;

                //  count số ReplyComment theo Id Post ra view Get
                List<ReplyComment> listReplyCommentView = db.ReplyComments.ToList();
                ViewBag.listReplyCommentView = listReplyCommentView;

                List<Post> doctorList = db.Posts.Where(e => e.PostName.Contains(keyword) || keyword == null).ToList();

                List<PostViewModel> doctorViewList = doctorList.Select(x => new PostViewModel
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

                return View(doctorViewList);
            }
            else
            {
                ViewBag.thongbao = "Các từ khóa chủ đề yêu cầu không được tìm thấy";
                return View("Notification");
            }
        }
    }
}