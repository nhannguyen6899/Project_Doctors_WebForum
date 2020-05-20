using Doctors_WebForum.Models.ADO;
using Doctors_WebForum.Models.Business;
using Doctors_WebForum.Models.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Doctors_WebForum.Controllers
{
    public class PostController : Controller
    {
        private Project_Doctors_WebForumEntities db = new Project_Doctors_WebForumEntities();

        public ActionResult Notification()
        {
            return View();
        }

        // Create Post
        public ActionResult Create(int id)
        {
            var count = db.Topics.Count(e => e.Id.Equals(id));
            if (count == 1)
            {
                return View(new PostViewModel());
            }
            else
            {
                ViewBag.thongbao = "Chủ đề yêu cầu không thể được tìm thấy";
                return View("Notification");
            }            
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(PostViewModel postViewModel, int id)
        {
            var id_doctor = Convert.ToInt32(Session["Id"]);
            // Add post 
            Post post = new Post()
            {
                PostName = postViewModel.PostName,
                PostContent = postViewModel.PostContent,
                CreateDate = DateTime.Now,
                StatusBlock = postViewModel.StatusBlock,
                Doctor_ID = id_doctor,
                Topic_ID = id,
            };
            db.Posts.Add(post);

            // Add likePost
            LikePost likePosts = new LikePost()
            {
                StatusLike = false,
                Post_ID = post.Id,
                Doctor_ID = id_doctor
            };

            db.LikePosts.Add(likePosts);            
            db.SaveChanges();
            return RedirectToAction("DetailPost/"+ post.Id, "Post");
        }

        /// ////////////////////////////////////////////////////////////////

        // Edit Post
        [HttpGet]
        public ActionResult Edit(int id)
        {
            // lấy thông tin chi tiết bài viết ra view Edit
            int count = db.Posts.Count(e => e.Id.Equals(id));
            if (count == 1)
            {
                var post = db.Posts.SingleOrDefault(a => a.Id.Equals(id));
                var detailPost = new PostViewModel
                {
                    Id = post.Id,
                    PostName = post.PostName,
                    PostContent = post.PostContent,
                    CreateDate = post.CreateDate,
                    StatusBlock = post.StatusBlock,
                    Doctor_ID = post.Doctor_ID,
                    ImageDoctor = post.Doctor.Image
                };
                return View(detailPost);
            }
            else
            {
                ViewBag.thongbao = "Bài viết yêu cầu không thể được tìm thấy";
                return View("Notification");
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(PostViewModel postViewModel)
        {           
            var post = db.Posts.SingleOrDefault(a => a.Id.Equals(postViewModel.Id));
            post.PostName = postViewModel.PostName;
            post.PostContent = postViewModel.PostContent;
            post.StatusBlock = postViewModel.StatusBlock;

            db.SaveChanges();
            return RedirectToAction("DetailPost/" + postViewModel.Id, "Post");
        }

        // Delete Post
        public ActionResult DeletePost(int id)
        {
            var post = db.Posts.Where(x => x.Id == id);
            var likePost = db.LikePosts.Where(x => x.Post_ID == id);
            var comment = db.Comments.Where(x => x.Post_ID == id);

            var ReplyComment = from c in db.Comments
                              join rc in db.ReplyComments on c.Id equals rc.Comment_ID
                               where c.Post_ID == id
                               select rc;

            db.ReplyComments.RemoveRange(ReplyComment);
            db.Comments.RemoveRange(comment);
            db.LikePosts.RemoveRange(likePost);
            db.Posts.RemoveRange(post);

            db.SaveChanges();
            return RedirectToAction("Index","Home");
        }

        // Delete Comment
        public ActionResult DeleteComment(int id)
        {
            // get id post để Redirect về DetailPost
            var com = db.Comments.SingleOrDefault(x => x.Id == id);
            var post = db.Posts.SingleOrDefault(x => x.Id == com.Post_ID);

            // xóa Comment
            var comment = db.Comments.Where(x => x.Id == id);
            var ReplyComment = db.ReplyComments.Where(x => x.Comment_ID == id);

            db.ReplyComments.RemoveRange(ReplyComment);
            db.Comments.RemoveRange(comment);
            db.SaveChanges();
            return RedirectToAction("DetailPost/" + post.Id, "Post");
        }

        // Delete ReplyComment
        public ActionResult DeleteReplyComment(int id)
        {
            // get id post để Redirect về DetailPost
            var rep = db.ReplyComments.SingleOrDefault(x => x.Id == id);
            var com = db.Comments.SingleOrDefault(x => x.Id == rep.Comment_ID);
            var post = db.Posts.SingleOrDefault(x => x.Id == com.Post_ID);

            // xóa replyComment
            var ReplyComment = db.ReplyComments.Where(x => x.Id == id);
            db.ReplyComments.RemoveRange(ReplyComment);
            db.SaveChanges();
            return RedirectToAction("DetailPost/" + post.Id, "Post");

        }

        // Get DetailPost 
        [HttpGet]
        public ActionResult DetailPost(int id, int? page)
        {
            // lấy ra số thành viên đã đăng kí thành công 
            var query = from table in db.Doctors
                        where table.Role != true && table.RegistrationStatus == true
                        select table;
            int countUser = query.Count();
            ViewBag.countUser = countUser;

            var pos = db.Posts.SingleOrDefault(x => x.Id.Equals(id));
            int count = db.Posts.Count(e => e.Id.Equals(id));
            if (count == 1)
            {
                if (pos.Doctor_ID == Convert.ToInt32(Session["Id"]))
                {
                    count = db.Posts.Count(e => e.Id.Equals(id));
                }
                else
                {
                    count = db.Posts.Count(e => e.Id.Equals(id) && e.StatusBlock == false);
                }
            }
            else
            {
                ViewBag.thongbao = "Bài viết yêu cầu không thể được tìm thấy";
                return View("Notification");
            }


            if (count == 1)
            {
                var idSess = Convert.ToInt32(Session["Id"]);

                // lấy thông tin chi tiết bài viết ra view detailPost
                var post = db.Posts.Where(a => a.Id.Equals(id));
                var detailPost = post.Select(x => new PostViewModel
                {
                    Id = x.Id,
                    PostName = x.PostName,
                    PostContent = x.PostContent,
                    CreateDate = x.CreateDate,
                    Doctor_ID = x.Doctor_ID,
                    ImageDoctor = x.Doctor.Image
                });
                ViewBag.detailPost = detailPost;

                //  count số like theo Id Post
                List<LikePost> listPost = db.LikePosts.Where(x => x.StatusLike == true).ToList();
                ViewBag.listLikePost = listPost;

                // lấy giá trị like là true hoặc false ra để gán button like post or unlike
                var likePost = db.LikePosts.SingleOrDefault(x => x.Post_ID == id && x.Doctor_ID == idSess);
                ViewBag.likePostView = likePost;

                // lấy danh sách thông tin doctor
                var doctor = db.Doctors.ToList();
                ViewBag.doctor = doctor;

                // lấy danh sách trả lời comment
                var replyComment = db.ReplyComments.ToList();
                ViewBag.replyComment = replyComment;

                // tạo biến phân trang
                int pageSize = 5;
                int pageNumber = (page ?? 1);

                // lấy danh sách bình luân bài viết ra view detailPost
                var comment = db.Comments.Where(a => a.Post_ID.Equals(id)).ToList();
                var listComment = comment.Select(x => new CommentViewModel
                {
                    Id = x.Id,
                    CommentContent = x.CommentContent,
                    CommentDate = x.CommentDate,
                    Doctor_ID = x.Doctor_ID
                }).ToList();

                return View(listComment.OrderBy(x => x.CommentDate).ToPagedList(pageNumber, pageSize));
            }
            else
            {
                ViewBag.thongbao = "Các bài viết yêu cầu không thể được tìm thấy";
                return View("Notification");
            }
        }
       
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult DetailPost(PostViewModel postView, CommentViewModel commentViewModel , string btnlikePost)
        {
            var idSess = Convert.ToInt32(Session["Id"]);
            // Add like and unlike
            if (!string.IsNullOrEmpty(btnlikePost))
            {

                var likePost = db.LikePosts.SingleOrDefault(x => x.Post_ID == postView.Id && x.Doctor_ID == idSess);
                if (likePost != null)
                {
                    if (idSess == likePost.Doctor_ID)
                    {
                        if (likePost.StatusLike == false)
                        {
                            likePost.StatusLike = true;
                        }
                        else
                        {
                            likePost.StatusLike = false;
                        }
                    }
                }
                else
                {
                    LikePost like = new LikePost()
                    {
                        StatusLike = true,
                        Post_ID = postView.Id,
                        Doctor_ID = idSess,
                    };
                    db.LikePosts.Add(like);
                }
            }         

            db.SaveChanges();
            return RedirectToAction("DetailPost/"+ postView.Id);

        }

        //////////////////////////////////////////////////////////////////////////////////

        // Get Post Theo Topic 
        public ActionResult Get(int id, int? page)
        {
            // lấy ra số thành viên đã đăng kí thành công
            var query = from table in db.Doctors
                        where table.Role != true && table.RegistrationStatus == true
                        select table;
            int countUser = query.Count();
            ViewBag.countUser = countUser;

            var count = db.Topics.Count(e => e.Id.Equals(id));
            if (count == 1)
            {
                // tạo biến phân trang
                int pageSize = 5;
                int pageNumber = (page ?? 1);

                var topic = db.Topics.Where(a => a.Id.Equals(id));
                var getTopic = topic.Select(x => new TopicViewModel
                {
                    Id = x.Id,
                    TopicName = x.TopicName,
                    Description = x.Description
                });
                ViewBag.TopicName = getTopic;

                //  count số like theo Id Post ra view Get
                List<LikePost> listPostView = db.LikePosts.ToList();
                ViewBag.listLikePost = listPostView;

                //  count số comment theo Id Post ra view Get
                List<Comment> listCommentView = db.Comments.ToList();
                ViewBag.listComment = listCommentView;

                //  count số ReplyComment theo Id Post ra view Get
                List<ReplyComment> listReplyCommentView = db.ReplyComments.ToList();
                ViewBag.listReplyCommentView = listReplyCommentView;

                // lấy danh sách bài viết theo topic
                var listPost = db.Posts.Where(a => a.Topic_ID.Equals(id) && a.StatusBlock == false).ToList();
                List<PostViewModel> listpostView = listPost.Select(x => new PostViewModel
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

                var listPosts = db.Posts.Where(a => a.Topic_ID.Equals(id)).FirstOrDefault();
                ViewBag.listPosts = listPosts;

                return View(listpostView.OrderByDescending(x => x.CreateDate).ToPagedList(pageNumber, pageSize));
            }
            else
            {
                ViewBag.thongbao = "Các chủ đề yêu cầu không thể được tìm thấy.";
                return View("Notification");
            }
        }


        // Add Comment
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Comment(int id, string Comment)
        {
            var id_doctor = Convert.ToInt32(Session["Id"]);
            // Add Comment 
            Comment comment = new Comment()
            {
                CommentContent = Comment,
                CommentDate = DateTime.Now,
                Post_ID = id,
                Doctor_ID = id_doctor
            };
            db.Comments.Add(comment);
            db.SaveChanges();
            return RedirectToAction("DetailPost/" + id, "Post");
        }


        // Add Reply Comment
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ReplyComment(int id, int idPost, string replyComment)
        {
            var id_doctor = Convert.ToInt32(Session["Id"]);
            // Add post 
            ReplyComment replyComments = new ReplyComment()
            {
                CommentContent = replyComment,
                CommentDate = DateTime.Now,
                Comment_ID = id,
                Doctor_ID = id_doctor,
            };
            db.ReplyComments.Add(replyComments);
            db.SaveChanges();
            return RedirectToAction("DetailPost/" + idPost, "Post");
        }


        // Edit Comment
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditComment(int id, string editComment)
        {
            var comment = db.Comments.SingleOrDefault(a => a.Id.Equals(id));
            var post = db.Posts.SingleOrDefault(a => a.Id.Equals(comment.Post_ID));

            comment.CommentContent = editComment;
            db.SaveChanges();
            return RedirectToAction("DetailPost/" + post.Id, "Post");
        }


        // Edit ReplyComment
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditReplyComment(int id, string editReplyComment)
        {
            var rep = db.ReplyComments.SingleOrDefault(a => a.Id.Equals(id));
            var comment = db.Comments.SingleOrDefault(a => a.Id.Equals(rep.Comment_ID));
            var post = db.Posts.SingleOrDefault(a => a.Id.Equals(comment.Post_ID));

            rep.CommentContent = editReplyComment;
            db.SaveChanges();
            return RedirectToAction("DetailPost/" + post.Id, "Post");
        }

    }
}