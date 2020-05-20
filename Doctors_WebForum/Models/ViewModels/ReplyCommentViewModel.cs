using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Doctors_WebForum.Models.ViewModels
{
    public class ReplyCommentViewModel
    {
        public int Id { get; set; }

        [AllowHtml]
        public string CommentContent { get; set; }
        public Nullable<System.DateTime> CommentDate { get; set; }
        public int Comment_ID { get; set; }
        public int Doctor_ID { get; set; }
    }
}