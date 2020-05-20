using Doctors_WebForum.Models.ADO;
using Doctors_WebForum.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Doctors_WebForum.Controllers
{    
    public class HomeController : Controller
    {
        private Project_Doctors_WebForumEntities db = new Project_Doctors_WebForumEntities();

        // Get Specialize and Topic 
        public ActionResult Index()
        {
            // lấy danh sách chuyên môn ra view Index
            List<Specialize> listSpecialize = db.Specializes.ToList();           
            var listSpecializeView = listSpecialize.Select(x => new SpecializeViewModel
            {
                Id = x.Id,
                SpecializeName = x.SpecializeName,
                Description = x.Description,
            }).ToList();

            // lấy ra các topic theo specialize
            List<Topic> listTopic = db.Topics.ToList();
            var listTopicView = listTopic.Select(x => new TopicViewModel
            {
                Id = x.Id,
                TopicName = x.TopicName,
                Description = x.Description,
                Specialize_ID = x.Specialize_ID
            }).ToList();

            ViewBag.listTopic = listTopicView;

            // lấy ra số thành viên đã đăng kí thành công
            var query = from table in db.Doctors
                        where table.Role != true && table.RegistrationStatus == true
                        select table;
            int count = query.Count();
            ViewBag.countUser = count;

            // lấy ra các bài viết để count bài viết theo topic
            List<Post> listPost = db.Posts.ToList();
            var listPostView = listPost.Select(x => new PostViewModel
            {
                Id = x.Id,
                Topic_ID = x.Topic_ID,
                StatusBlock = x.StatusBlock
            }).ToList();
            ViewBag.listPost = listPostView;

            //  count số comment theo Id Post ra view Get
            List<Comment> listCommentView = db.Comments.ToList();
            ViewBag.listComment = listCommentView;

            //  count số ReplyComment theo Id Post ra view Get
            List<ReplyComment> listReplyCommentView = db.ReplyComments.ToList();
            ViewBag.listReplyCommentView = listReplyCommentView;

            return View(listSpecializeView);
        }  

    }
}